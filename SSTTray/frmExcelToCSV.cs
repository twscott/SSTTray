using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirstOhm;

using Newtonsoft.Json;

namespace TaskTrayApplication
{
    public partial class frmExcelToCSV : Form
    {
        ExcelLib ex = new ExcelLib();
        public frmExcelToCSV()
        {
            InitializeComponent();
        }

        private void frmExcelToCSV_Load(object sender, EventArgs e)
        {
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                //ofd.Filter = "Excel 活頁簿 Excel 97-2003 (*.xls)|*.xls|(*.xlsx)|*.xlsx|文字檔 (Tab 字元分隔) (*.txt)|*.txt";
                ofd.Filter = "*.xlsx|";
                if (ofd.ShowDialog() == DialogResult.OK)
                    textScriptPath.Text = ofd.FileName;
                else
                    textScriptPath.Text = string.Empty;
            }
            this.Refresh();
        }

        private void btnUdpSend_Click(object sender, EventArgs e)
        {
            try
            {
                txtJson.Text = "";
                Dictionary<string, string> stdPrnDict = new Dictionary<string, string>()
                {
                    {"func","excelToCsv" },{"excelFile", textScriptPath.Text },
                    {"Delimeter", comboDelimeter.Text}, //Excel 的資料欄位 Header
                    {"workSheet", txtWorkSheet.Text }
                };
                string jsonToSend = JsonConvert.SerializeObject(stdPrnDict);
                txtUdpToSend.Text = jsonToSend;
                Refresh();
                try
                {
                    UDPLib.udpSend(jsonToSend, Constants.getProperty("UDPServer"), UDPLib.hostPort);
                    CommonClass.wait(2);
                    showCsvContent();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@jsonToSend, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"excelToCSV?excelFile={textScriptPath.Text}", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void showCsvContent()
        {
            try
            {
                using (StreamReader sr = new StreamReader(txtCSV.Text))
                {
                    txtJson.Text = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnExcelConvert_Click(object sender, EventArgs e)
        {
            txtJson.Text = "";
            if (!CommonClass.ifFileExists(textScriptPath.Text))
            {
                MessageBox.Show("檔案不存在", "檔案錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //txtCSV.Text= ex.excelToCsv(textScriptPath.Text);
            txtCSV.Text = ex.SaveExcelToCSVWithDelimiter(textScriptPath.Text, txtWorkSheet.Text, comboDelimeter.Text);
            showCsvContent();
        }

    }
}
