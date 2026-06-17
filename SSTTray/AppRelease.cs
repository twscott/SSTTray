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
    public partial class AppRelease : Form
    {
        public AppRelease()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                //ofd.Filter = "Excel 活頁簿 Excel 97-2003 (*.xls)|*.xls|(*.xlsx)|*.xlsx|文字檔 (Tab 字元分隔) (*.txt)|*.txt";
                ofd.Filter = "*.apk|";
                if (ofd.ShowDialog() == DialogResult.OK)
                    textAppFilePath.Text = ofd.FileName;
                else
                    textAppFilePath.Text = string.Empty;
            }
            this.Refresh();
        }

        private void showGridview()
        {
            string sqlStr = "SELECT `version_no` 版本號, " +
            " `releaseNote` 更版內容, case `releaseStatus` " +
                " when 0 then '未發布' " +
                " When 1 then '資訊部給工廠' " +
                " when 2 then '工廠測試(指定個人)' " +
                " when 2 then '工廠部分升版(指定製程)' " +
                " when 3 then '全線升版' end 更版狀態, " +
             " `init_releaseDate` 資訊部發布日期 " +
             " FROM `app_release_log` " +
             $" Where `appType`='{comboAppType.Text.Replace(" APP", "")}' " +
             " order by init_releaseDate desc limit 3 ";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            UI_CommonClass.dtTodgv(dgv1, dt);
        }

        private void comboAppType_SelectedIndexChanged(object sender, EventArgs e)
        {
            showGridview();
            switch (comboAppType.Text) 
            {
                case "一般製程 APP":
                    textAppFilePath.Text = @"\\192.168.1.45\FirstohmFiles\Public\程式發布\Android\新版製程";
                    break;
                case "舊版製程 APP":
                    textAppFilePath.Text = @"\\192.168.1.45\FirstohmFiles\Public\程式發布\Android\舊版製程";
                    break;
                case "組帽 APP":
                    textAppFilePath.Text = @"\\192.168.1.45\FirstohmFiles\Public\程式發布\Android\組帽";
                    break;
                case "RO章 APP":
                    textAppFilePath.Text = @"\\192.168.1.45\FirstohmFiles\Public\程式發布\Android\RO章";
                    break; 
            }
        }
        //檢驗新增的版本號
        private bool verifyVersionNo(string lastVersionNo, string newVersionNo)
        {
            string[] lastVerArr=null;
            string[] newVerArr = newVersionNo.Split('.');
            if (newVerArr.Length != 2)
                return false;
            if(!string.IsNullOrEmpty(lastVersionNo))
                lastVerArr  = lastVersionNo.Split('.');
            if (lastVerArr != null)
            {
                //大版號錯誤
                if (int.Parse(lastVerArr[0]) < int.Parse(newVerArr[0]))
                    return false;
                else if (int.Parse(lastVerArr[0]) == int.Parse(newVerArr[0]))
                { //upgrade 小版號
                    if (int.Parse(lastVerArr[1]) < int.Parse(newVerArr[1]))
                        return true;
                    else //小版號錯誤
                        return false;
                }
                else //upgrade 大版號
                    return true;
            }
            else //無舊版號
                return true;
        }

        //檢驗新增的版本號
        private bool verifyDelVersionNo(string lastVersionNo, string newVersionNo)
        {
            if (string.IsNullOrEmpty(lastVersionNo))
                return false;
            if(lastVersionNo!= newVersionNo)
                return false;
            else
                return true;
        }

        private bool UI_verifyVersionNo(string lastVersionNo, bool ifDelVer=false)
        {
            if (txtVersionNo.Text == "")
            {
                MessageBox.Show("版本號碼 不可以為空值", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if(ifDelVer && !verifyDelVersionNo(lastVersionNo, txtVersionNo.Text))
            {
                MessageBox.Show("版本號碼錯誤", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            } else if (!ifDelVer && !verifyVersionNo(lastVersionNo, txtVersionNo.Text))
            {
                MessageBox.Show("版本號碼錯誤", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            if (!ifDelVer &&  txtReleaseNote.Text == "")
            {
                MessageBox.Show("修改內容 不可以為空值", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!ifDelVer && !CommonClass.ifFileExists(textAppFilePath.Text))
            {
                MessageBox.Show($"{textAppFilePath.Text} 檔案不存在", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string lastVersionNo = (dgv1.Rows.Count == 0 ? "" : dgv1.Rows[0].Cells["版本號"].Value.ToString()); 
            if (!UI_verifyVersionNo(lastVersionNo, false))
                return;
            string fileName = CommonClass.getFileInfo(textAppFilePath.Text, "name");
            FTPExtensions ftp = new FTPExtensions("172.168.1.151", "clkuo", "clkuo");
            if (!ftp.Upload($"/wwwroot/firstohmWebapi/{fileName}", textAppFilePath.Text))
                return;
            string ipAddress = CommonClass.GetLocalIPAddress();
            string createBy = Constants.getProperty("ComputerName", "火箭程式");
            string[] verArr = txtVersionNo.Text.Split('.');
            int verInint = int.Parse(verArr[0]) * 10000 + int.Parse(verArr[1]);
            string sqlStr = "insert into `app_release_log` " +
                " (`version_no`,`appType`,`releaseNote`,`releaseStatus`," +
                " `init_releaseBy`,init_IPAddress,`init_releaseDate`, verInint) " +
                " values " +
                $" ('{txtVersionNo.Text}','{comboAppType.Text.Replace(" APP", "")}','{txtReleaseNote.Text}','1'," +
                $" '{createBy}','{ipAddress}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}', {verInint})";
            if(CommonClass.execSQLNonQuery(sqlStr, Constants.MFOFlowConnString, true) ==-1)
                MessageBox.Show("修改內容 不可以為空值", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            showGridview();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            string lastVersionNo = (dgv1.Rows.Count == 0 ? "" : dgv1.Rows[0].Cells["版本號"].Value.ToString());
            if (!UI_verifyVersionNo(lastVersionNo, true))
                return;
            string sqlStr = $"Delete from `app_release_log` " +
                $" WHERE `appType`='{comboAppType.Text.Replace(" APP", "")}' " +
                $" and `releaseStatus` <= 1 and `version_no`= '{txtVersionNo.Text}' ";
            CommonClass.execSQLNonQuery(sqlStr);
            showGridview();
        }

        private void AppRelease_Load(object sender, EventArgs e)
        {
            textAppFilePath.Text = @"\\192.168.1.45\FirstohmFiles\Public\程式發布\Android\新版製程";
            showGridview();
        }
    }
}
