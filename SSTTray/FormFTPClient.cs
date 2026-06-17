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
    public partial class FormFTPClient : Form
    {
        FTPExtensions ftp = null;
        public FormFTPClient()
        {
            InitializeComponent();
        }

        private void FormFTPClient_Load(object sender, EventArgs e)
        {
            txtPwd.PasswordChar = '*';

        }

        private void btnNewServer_Click(object sender, EventArgs e)
        {
            ftp = new FTPExtensions(txtIPAdd.Text + ":" + txtPort.Text, txtAcc.Text, txtPwd.Text);
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (ftp == null)
                btnNewServer.PerformClick();
            ftp.Upload(txtRemotePath.Text + @"\" + txtFileName.Text, txtLocalPath.Text + @"\" + txtFileName.Text);
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {

            if (ftp == null)
                btnNewServer.PerformClick();
            //ftp.Download(txtRemotePath.Text + @"\" + txtFileName.Text, txtLocalPath.Text + @"\" + txtFileName.Text);
            if (!ftp.Download(txtRemotePath.Text + @"\" + txtFileName.Text, txtLocalPath.Text + @"\" + txtFileName.Text))
            {
                if (CommonApp.lastFtpFailLine.Date != DateTime.Now.Date)
                {
                    LineLib linelib = new LineLib();
                    linelib.pushTextMessageByWebapiAsync("scott.tseng", "PCL 機制停止， 請確認");
                    CommonApp.lastFtpFailLine = DateTime.Now;
                }
            }
        }

        private void btnListFiles_Click(object sender, EventArgs e)
        {
            if (ftp == null)
                btnNewServer.PerformClick();
            string[] remoteFileList = ftp.GetFileList(txtRemotePath.Text);
            foreach(string file in remoteFileList)
            {
                txtResult.Text += file + Environment.NewLine;
            }
        }

        private void btnConutri_Click(object sender, EventArgs e)
        {
            try
            {
                string rtnStr = null;
                txtResult.Text = "";
                int logIdx = 0;
                int failCnt = 0;
                do {
                    
                    logIdx = CommonApp.retribeConutriTP(0, txtIPAdd.Text + ":" + txtPort.Text, txtAcc.Text, txtPwd.Text,
                                 txtRemotePath.Text, txtLocalPath.Text, out rtnStr);
                    txtResult.Text += rtnStr == null ? "" : rtnStr + "; ";
                    Refresh();
                    if (logIdx > 0)
                    {
                        failCnt = 0;
                        break;
                    }
                    else
                    {
                        failCnt++;
                        if(failCnt==3)
                        {
                            return;
                        }
                        CommonClass.wait(2);
                    }
                        
                    
                } while (true);

                int H2Idx = 0;
                if (logIdx > 0)
                    do
                    {
                        H2Idx = CommonApp.retribeConutriTP(logIdx, txtIPAdd.Text + ":" + txtPort.Text, txtAcc.Text, txtPwd.Text,
                                     txtRemotePath.Text, txtLocalPath.Text, out rtnStr);
                        txtResult.Text += rtnStr==null?"": rtnStr+"; ";
                        Refresh();
                        if (H2Idx > 0)
                            break;
                        else
                            CommonClass.wait(5);
                    } while (true);
                else
                {
                    MessageBox.Show("存取資料失敗  " + rtnStr, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("儲存資料完成  ", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch(Exception ex)
            {
                MessageBox.Show("存取資料失敗  " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CommonClass.startProcess(@"C:\Program Files\FileZilla FTP Client\filezilla.exe");
        }
    }
}
