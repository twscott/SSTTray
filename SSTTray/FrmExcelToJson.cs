using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FirstOhm;

using Newtonsoft.Json;

namespace TaskTrayApplication
{
    public partial class frmExcelToJson : Form
    {
        ExcelLib ex = new ExcelLib();
        public frmExcelToJson()
        {
            InitializeComponent();
        }

        private void textScriptPath_TextChanged(object sender, EventArgs e)
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

        private void btnExcelConvert_Click(object sender, EventArgs e)
        {
            if (!CommonClass.ifFileExists(textScriptPath.Text))
            {
                MessageBox.Show("檔案不存在", "檔案錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtExcelCol.Text == "")
            {
                MessageBox.Show("Excel 欄位不可為空白", "欄位屬性錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> dictKeys = JsonConvert.DeserializeObject<List<string>>(txtExcelCol.Text);
            //其中 "" 代表 string.IsNullOrEmpty
            Dictionary<string, string> skipData=null;
            
            if(textScriptPath.Text!="")
                skipData = JsonConvert.DeserializeObject<Dictionary<string, string>>(txtSkipDict.Text);
            txtJson.Text =  ex.excelToJson(textScriptPath.Text, dictKeys, skipData, "工作表1");
        }

        private void FrmExcelToJson_Load(object sender, EventArgs e)
        {
            List<string> excelToJsonList = new List<string>();
            foreach (KeyValuePair<string, string> peopertyItem in Constants.propertyDic)
            {
                if (peopertyItem.Key[0] == '#')
                    continue;
                if (peopertyItem.Key.Contains("excelToJson_"))
                    excelToJsonList.Add(peopertyItem.Key);
            }
            if (excelToJsonList.Count > 0)
                UI_CommonClass.fillInCombo(comboStandardLabel, excelToJsonList, false);
        }

        private void comboStandardLabel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboStandardLabel.Text == "")
                return;
            Dictionary<string, string> excelToJsonDict = CommonApp.parsePropertyToDict(comboStandardLabel.Text);
            if(excelToJsonDict !=null)
            {
                txtExcelCol.Text = excelToJsonDict["DataCol"];
                txtSkipDict.Text = excelToJsonDict["skipDataDict"];
                textScriptPath.Text = excelToJsonDict["excelPath"];
                txtWorkSheet.Text = excelToJsonDict["workSheet"];
            } else
            {
                MessageBox.Show("讀取 Property 錯誤", "讀取 Property 錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        ////lblName:要列印的標籤，例如 '標準標籤_成品入庫'
        //private void parseStandardLableProperty(string lblName)
        //{
        //    try
        //    {
        //        string stdLabelAtt = Constants.getProperty(lblName);
        //        Dictionary<string, string> stdLblDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(stdLabelAtt);
        //        txtExcelCol.Text = stdLblDict["DataCol"];
        //        txtSkipDict.Text = stdLblDict["skipDataDict"];
        //        textScriptPath.Text = stdLblDict["excelPath"];
        //    } catch(Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "讀取 Property 錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        private void btnUdpSend_Click(object sender, EventArgs e)
        {
            string jsonToSend = CommonApp.genExcelToJsonProperty(comboStandardLabel.Text, textScriptPath.Text,
                             txtExcelCol.Text, txtSkipDict.Text);
            txtUdpToSend.Text = jsonToSend;
            Refresh();
            try
            {
                UDPLib.udpSend(jsonToSend, Constants.getProperty("UDPServer"), UDPLib.hostPort);
                MessageBox.Show("執行 " + jsonToSend + " 完成", "執行 UDP 命令", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@jsonToSend, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
