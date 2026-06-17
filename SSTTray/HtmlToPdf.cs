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

namespace TaskTrayApplication
{
    public partial class HtmlToPdf : Form
    {
        ExcelLib exl = new ExcelLib();
        public HtmlToPdf()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                //ofd.Filter = "Excel 活頁簿 Excel 97-2003 (*.xlsx)|*.xlsx|(*.xls)|*.xls|文字檔 (Tab 字元分隔) (*.txt)|*.txt";
                ofd.Filter = "(*.html)|*.html|(*.htm)|*.htm";
                if (ofd.ShowDialog() == DialogResult.OK)
                    txtSourceFile.Text = ofd.FileName;
                else
                    txtSourceFile.Text = string.Empty;
            }
            this.Refresh();
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            String rtnStr = null;
            bool ifSuccess = true;
            int printOrientation = radioPortaratr.Checked ? 0 : 1;
            if (!CommonClass.ifFileorFolderExists(txtSourceFile.Text, printOrientation))
            {
                MessageBox.Show("請先選取要轉換的 html 檔案!!", "html 轉 pdf", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(radioSingleFile.Checked)
            {
                ifSuccess = exl.saveHtmlToPdf(txtSourceFile.Text, out rtnStr, txtSourceFile.Text.Replace(".html", ".pdf"), printOrientation);
            } else
            {
                string fullPath = CommonClass.getFileInfo(txtSourceFile.Text, "directory");
                string subName = ".*";
                string rtnMsg = "";
                if (!string.IsNullOrEmpty(comboUDPSubName.Text))
                    subName = "." + comboUDPSubName.Text;
                string[] allFiles = CommonClass.getAllfilesInDirectory(fullPath, "*" + subName);
                for (int i=0; i< allFiles.Length; i++)
                {
                    try
                    {
                        ifSuccess = ifSuccess && exl.saveHtmlToPdf(allFiles[i], out rtnStr, allFiles[i].Replace(CommonClass.getFileInfo(allFiles[i], "extension"), ".pdf"), printOrientation);
                        if (!ifSuccess)
                            rtnMsg += rtnStr + System.Environment.NewLine;
                    } catch(Exception ex)
                    {
                        //do nothing
                    }
                }
                if (rtnMsg == "")
                {
                    ifSuccess = true;
                    rtnStr = "完成檔案轉換";
                }
                else
                {
                    ifSuccess = false;
                    rtnStr = rtnMsg;
                }
            }

            if (!ifSuccess)
                MessageBox.Show(rtnStr, "html 轉 pdf", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show(rtnStr + "完成", "html 轉 pdf", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void HtmlToPdf_Load(object sender, EventArgs e)
        {
            string tempfileSource = Constants.getProperty("PdfPath");
            if (string.IsNullOrEmpty(tempfileSource))
                txtSourceFile.Text = @"D:\Temp";
            else
                txtSourceFile.Text = tempfileSource;
        }

        private void radioAllFiles_CheckedChanged(object sender, EventArgs e)
        {
            label2.Visible = comboUDPSubName.Visible = radioAllFiles.Checked;

        }
    }
}
