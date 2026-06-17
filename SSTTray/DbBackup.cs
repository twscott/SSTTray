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
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

namespace TaskTrayApplication
{
    public partial class DbBackup : Form
    {
        CommonBackup cmBK = new CommonBackup();
        public int selectType = 0; //0:backup, 1:restotr
        const string bkPath = @"D:\DBBackupTemp";
        public DbBackup()
        {
            InitializeComponent();
        }

        private void DbBackup_Load(object sender, EventArgs e)
        {
            dtp1.Value = DateTime.Now;
        }


        private void btnBackup_Click(object sender, EventArgs e)
        {
            string sqlStr;
            int dataType = 1; //0:Schema only, 1:Data Only, 2:Schema + Data
            string bkResult = null;
            string backupPath = txtBackupPath.Text;
            StringBuilder rtnStr = new StringBuilder("");
            textBKResult.Text = "";
            Refresh();

            if(comboBKServers.Text=="" || comboBackupDB.Text =="")
            {
                MessageBox.Show("請選取要備份的 伺服器 與 資料庫 ", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            if (string.IsNullOrEmpty(backupPath))
            {
                MessageBox.Show("建立資料夾" + backupPath + "錯誤", "備份資料庫失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (radioSchema.Checked)
                dataType = 0;
            else if (radioData.Checked)
                dataType = 1;
            else if (radioView.Checked)
                dataType = 3;
            else if (radioTableFunc.Checked)
                dataType = 4;
            else
                dataType = 2; //schema + data

            if (!string.IsNullOrEmpty(comboBackupDB.Text) && !string.IsNullOrEmpty(comboBKServers.Text))
            {   //備份 Schema + Data 
                if (dataType == 2)
                {
                    //All Table schema
                    if (cmBK.backupDB(comboBackupDB.Text, backupPath, comboBKServers.Text, out bkResult, 0))
                        rtnStr.Append(bkResult + Environment.NewLine);
                    else
                        rtnStr.Append(comboBKServers.Text + " : " + comboBackupDB.Text + " 備份失敗， " + rtnStr + Environment.NewLine);
                    //Data only
                    if (cmBK.backupDB(comboBackupDB.Text, backupPath, comboBKServers.Text, out bkResult, 1))
                        rtnStr.Append(bkResult + Environment.NewLine);
                    else
                        rtnStr.Append(comboBKServers.Text + " : " + comboBackupDB.Text + " 備份失敗， " + rtnStr + Environment.NewLine);
                }
                else if(dataType == 0)
                {
                    //dataType:0 Schema Only, 4: Table+function, 3:View Only
                    //backup Table+function  
                    if (cmBK.backupDB(comboBackupDB.Text, backupPath, comboBKServers.Text, out bkResult, 4))
                        rtnStr.Append(bkResult + Environment.NewLine);
                    else
                        rtnStr.Append(comboBKServers.Text + " : " + comboBackupDB.Text + " 備份失敗， " + rtnStr + Environment.NewLine);
                    //backup View only 
                    if (cmBK.backupDB(comboBackupDB.Text, backupPath, comboBKServers.Text, out bkResult, 3))
                        rtnStr.Append(bkResult + Environment.NewLine);
                    else
                        rtnStr.Append(comboBKServers.Text + " : " + comboBackupDB.Text + " 備份失敗， " + rtnStr + Environment.NewLine);
                }
                else
                {
                    //dataType:0 Schema Only, 1:DataOnly, 3:View Only, 4:Table+Function
                    if (cmBK.backupDB(comboBackupDB.Text.ToString(), backupPath, comboBKServers.Text, out bkResult, dataType))
                        rtnStr.Append(bkResult + Environment.NewLine);
                    else
                        rtnStr.Append(comboBKServers.Text + " : " + comboBackupDB.Text + " 備份失敗， " + rtnStr + Environment.NewLine);
                }
            } 
            //備份整個 Location
            else if (!string.IsNullOrEmpty(comboBKServers.Text))
            {
                cmBK.multiBackupDB(backupPath, comboBKServers.Text, out bkResult, dataType);
                MessageBox.Show("請選取要備份的 伺服器 與 資料庫 ", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } else
            {
                MessageBox.Show("請選取要備份的 伺服器 與 資料庫 ", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            textBKResult.Text = $"{rtnStr}";
        }


        private void btnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                string bkResult = null;
                if (string.IsNullOrEmpty(comboRSServers.Text) || string.IsNullOrEmpty(comboDBRestore.Text))
                {
                    MessageBox.Show("請選取還原資料庫!!", "還原資料庫", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DialogResult dislogResult = MessageBox.Show("即將還原 " + comboRSServers.Text + " 資料庫 " + comboDBRestore.Text + ", 一旦還原無法回复， 您是否確定 ?", "還原資料庫",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dislogResult == DialogResult.No)
                    return;
                if (cmBK.restoreDB(comboRSServers.Text, comboDBRestore.Text, textScriptPath.Text, out bkResult))
                    MessageBox.Show(bkResult, "還原資料庫", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    MessageBox.Show(bkResult, $"還原資料庫失敗： {bkResult}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textErrMsg.Text = bkResult;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text + "失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //action 0:Backup, 1:Restore
        private Dictionary<string, string> changeConnection(string comboSource, int action)
        {
            switch (comboSource)
            {
                case "台北 211.23.138.231":
                case "TP211":
                    //if (action == 0)
                    cmBK.dbBKConnections = cmBK.TaipeiConnections211;
                    //else
                    //    cmBK.dbRSConnections = cmBK.TaipeiConnections;
                    break;
                case "花蓮 172.1681.1.151":
                case "HL151":
                    //if (action == 0)
                    cmBK.dbBKConnections = cmBK.HualianConnections;
                    //else
                    //    cmBK.dbRSConnections = cmBK.HualianConnections;
                    break;
                case "花蓮 172.1681.1.33":
                case "HL33":
                    //if (action == 0)
                    cmBK.dbBKConnections = cmBK.Hualian172_33Connections;
                    //else
                    //    cmBK.dbRSConnections = cmBK.Hualian172_33Connections;
                    break;
                case "花蓮 172.1681.1.35":
                case "HL35":
                    //if (action == 0)
                    cmBK.dbBKConnections = cmBK.Hualian172_35Connections;
                    //else
                    //    cmBK.dbRSConnections = cmBK.Hualian172_33Connections;
                    break;
                case "台北 192.1681.1.33":
                case "TP33":
                    //if (action == 0)
                    cmBK.dbBKConnections = cmBK.Taipei192_33Connections;
                    //else
                    //    cmBK.dbRSConnections = cmBK.Taipei192_33Connections;
                    break;
                //case "Portal 192.168.1.33":
                //case "portal":
                //    //if (action == 0)
                //    //cmBK.dbBKConnections = cmBK.PortalConnections;
                //    //else
                //    //    cmBK.dbRSConnections = cmBK.PortalConnections;
                //    break;
                case "OWN":
                    cmBK.dbBKConnections = cmBK.OWN127_0Connections;
                    break;
            }
            return cmBK.dbBKConnections;
        }


        private void comboServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dictionary<string, string> connDict = changeConnection(comboRSServers.Text, 1);
            textScriptPath.Text = cmBK.getBKPath(comboBKServers.Text);
            if (connDict != null)
                UI_CommonClass.fillInComboByDict(comboDBRestore, connDict);
        }

        private void comboBKServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            changeConnection(comboBKServers.Text, 0);
            Dictionary<string, string> connDict = changeConnection(comboBKServers.Text, 0);
            if (connDict != null)
                UI_CommonClass.fillInComboByDict(comboBackupDB, connDict, "", true);
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                //ofd.Filter = "Excel 活頁簿 Excel 97-2003 (*.xls)|*.xls|(*.xlsx)|*.xlsx|文字檔 (Tab 字元分隔) (*.txt)|*.txt";
                ofd.Filter = "SQL Files (*.sql)|*.sql|Zip Files (*.zip)|*.zip|All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                    textScriptPath.Text = ofd.FileName;
                else
                    textScriptPath.Text = string.Empty;
            }
            this.Refresh();
        }


        string a = DateTime.Now.ToShortDateString();

        private void button1_Click(object sender, EventArgs e)
        {

            int delCnt = cmBK.delBackupFile(deletePath.Text, dtp1.Value);
            MessageBox.Show($"刪除失敗次數 {delCnt} !");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog ofd = new FolderBrowserDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                    deletePath.Text = ofd.SelectedPath;
                else
                    deletePath.Text = string.Empty;
            }
            this.Refresh();
        }


        private void btnCrossRestore_Click(object sender, EventArgs e)
        {
            string restoreDB = comboCrossDBName.Text;
            string sourceSvr = comboCRSourceDB.Text;
            string destSvr = comboCRTargetRestore.Text;
            txtCrossResult.Text = "";
            Refresh();
            string result = "";
            bool rerultBool = true;
            int bkTime = 1; //1:正常  2:中午  3:Sunday
            if (radioMidNight.Checked)
                bkTime = 1;
            else if (radioNoon.Checked)
                bkTime = 2;
            else
                bkTime = 3;
            //如果 destSvr==""， 則由系統選定， 花蓮->台北， 台北->花蓮
            //crossRestoreDB(string destSvrName, string sourceSvrName, string bkPath, int restortType, out string result, string dbName = null, int bkTime = 1)

            //依照  211.23.138.231/ChkIn_Out/DB_BACKUP 所記載的所有 DB 全部 Cross Restore
            //backupType 0:Schema, 1:Data, 2:Schema + Data
            ////backupStatus 1:每天凌晨, 2:每天中午, 3:星期日
            rerultBool = (rerultBool && cmBK.crossRestoreDB(comboCRSourceDB.Text, txtCrossBKPath.Text, 2, 
                out result, comboCrossDBName.Text , 1));
        }


        private void comboCRSourceDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dictionary<string, string> connDict = changeConnection(comboCRSourceDB.Text, 1);
            textScriptPath.Text = cmBK.getBKPath(comboCRSourceDB.Text);
            if (connDict != null)
                UI_CommonClass.fillInComboByDict(comboCrossDBName, connDict);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", $"{txtBackupPath.Text}");
        }


        private void button7_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", @"\\172.168.1.35\ScottWork\backup");
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            string sqlStr;
            Dictionary<string, List<string>> bkJson = null;
            int dataType = 1; //0:Schema only, 1:Data Only, 2:Schema + Data
            string bkResult = null;
            string backupPath = txtBackupPath.Text;
            StringBuilder rtnStr = new StringBuilder("");

            try
            {
                textBKResult.Text = "";
                Refresh();

                if (comboBoxSvr.Text == "" || comboBoxDB.Text == "")
                {
                    MessageBox.Show("請選取要備份的 伺服器 與 資料庫 ", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                if (string.IsNullOrEmpty(backupPath))
                {
                    MessageBox.Show("建立資料夾" + backupPath + "錯誤", "備份資料庫失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(textBKJson.Text))
                {
                    MessageBox.Show("請提供備份 Json", "備份資料庫失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    bkJson = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(textBKJson.Text);
                }

                //0:Schema only, 1:Data Only, 2:Schema + Data
                dataType = 2; //schema + data
                string srcDB = comboBoxDB.Text;
                string srcHost = comboBoxSvr.Text;
                if (cmBK.backupDB(comboBoxDB.Text, backupPath, comboBoxSvr.Text, out bkResult, 4, null, 1, bkJson))
                    rtnStr.Append(bkResult + Environment.NewLine);
                else
                    rtnStr.Append(comboBKServers.Text + " : " + comboBackupDB.Text + "Schema  備份失敗， " + rtnStr + Environment.NewLine);

                if (cmBK.backupDB(srcDB, backupPath, srcHost, out bkResult, 1, null, 1, bkJson))
                    rtnStr.Append(bkResult + Environment.NewLine);
                else
                    rtnStr.Append(comboBKServers.Text + " : " + comboBackupDB.Text + "Data 備份失敗， " + rtnStr + Environment.NewLine);

                textJsonBKResult.Text = $"{rtnStr}";
                MessageBox.Show(rtnStr.ToString(), "備份資料庫完成", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "備份資料庫失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", $"{textBox3.Text}");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", @"\\172.168.1.35\ScottWork\backup");
        }

        private void comboBoxSvr_SelectedIndexChanged(object sender, EventArgs e)
        {
            changeConnection(comboBoxSvr.Text, 0);
            Dictionary<string, string> connDict = changeConnection(comboBoxSvr.Text, 0);
            if (connDict != null)
                UI_CommonClass.fillInComboByDict(comboBoxDB, connDict, "");
        }

        private void comboBoxDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDB.Text.ToLower() == "portal")
            {
                textBKJson.Text = "{'tables':['absence','departments','tp_attend_detail','	tp_chkin_out_statist','xyz_hikashop_user', " +
                    " 'xyz_jevents_catmap','xyz_jevents_filtermap','xyz_jevents_exception','xyz_jevents_icsfile','xyz_jevents_repetition', " +
                    " 'xyz_jevents_rrule','xyz_jevents_translation','xyz_jevents_vevdetail','xyz_jevents_vevent','xyz_jev_defaults', " +
                    " 'xyz_jev_users','xyz_uj_apply_form','xyz_uj_approval_check','xyz_uj_assessment','xyz_uj_assessment_self','xyz_uj_del_log', " +
                    " 'xyz_uj_departments','xyz_uj_dynamicinput','xyz_uj_email','xyz_uj_email_log','xyz_uj_leave','xyz_uj_leaveoff', " +
                    " 'xyz_uj_line','xyz_uj_lineaccount','xyz_uj_line_log','xyz_uj_module_anywhere','xyz_uj_namecard', " +
                    " 'xyz_uj_newstaffdemand','xyz_uj_nonchekin','xyz_uj_official_apv','xyz_uj_overwork','xyz_uj_program_requir', " +
                    " 'xyz_uj_send_mail','xyz_uj_signature','xyz_uj_specialleave','xyz_uj_stamp_apply','xyz_uj_year_statistics','xyz_users'], " +
                    " 'views':['all'], " +
                    " 'functions':['all'], " +
                    " 'procedures':['all']}";
            }
            else if(comboBoxDB.Text.ToLower() == "sst")
            {
                textBKJson.Text = "{'tables':['alertlog'], " +
                    " 'views':[''], " +
                    " 'functions':[''], " +
                    " 'procedures':['']}";
            }
            else
            {
                textBKJson.Text = "{'tables':[''], " +
                                    " 'views':['all'], " +
                                    " 'functions':['all'], " +
                                    " 'procedures':['all']}";
            }
        }

        private void btnBackupZip_Click(object sender, EventArgs e)
        {
            try
            {
                btnBackup.PerformClick();
                string folderName = txtBackupPath.Text + @"\" + comboBKServers.Text + @"\" + DateTime.Now.ToString("yyyyMMdd");
                CommonClass.zipFolder(folderName, comboBackupDB.Text, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("備份壓縮發生錯誤," + ex.Message, "zip folder error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUnzipRestore_Click(object sender, EventArgs e)
        {
            string zipFileName = CommonClass.getFileInfo(textScriptPath.Text, "filenamewithoutextension");
            string folderPath = CommonClass.getFileInfo(textScriptPath.Text, "directory");
            string destFolderPath = folderPath + $"\\{zipFileName}\\";
            if(CommonClass.ifFileorFolderExists(destFolderPath, 1)) 
                CommonClass.deleteFoldersAndFiles(destFolderPath, true);
            CommonClass.createIfMissing(destFolderPath);
            CommonClass.unZip(textScriptPath.Text, destFolderPath);
            string[] allFolders = CommonClass.getAllFoldersInDirectory(destFolderPath);
            string[] sqlFileNames = null;
            string dbName= comboDBRestore.Text;
            string bkResult = "";
            //先還原 Schema
            foreach (string folderName in allFolders)
            {
                if (folderName.ToLower().Contains("dbsschma"))
                {
                    sqlFileNames = CommonClass.getAllfilesInDirectory(folderName, "*.sql");
                    foreach (string sqlFile in sqlFileNames)
                    {
                        if (!cmBK.restoreDB(comboRSServers.Text, dbName, sqlFile, out bkResult))
                        {
                            MessageBox.Show(sqlFile, $"還原資料庫失敗： {bkResult}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                
            }
            //再還原 Data
            foreach (string folderName in allFolders)
            {
                if (folderName.ToLower().Contains("dbdata"))
                {
                    sqlFileNames = CommonClass.getAllfilesInDirectory(folderName, "*.sql");
                    foreach (string sqlFile in sqlFileNames)
                    {
                        if (!cmBK.restoreDB(comboRSServers.Text, dbName, sqlFile, out bkResult))
                        {
                            MessageBox.Show(sqlFile, $"還原資料庫失敗： {bkResult}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
            MessageBox.Show("資料庫還原完成");
        }
        private void textScriptPath_TextChanged(object sender, EventArgs e)
        {
            
            btnRestore.Enabled = !((TextBox)sender).Text.ToLower().Contains(".zip");
            btnUnzipRestore.Enabled = !btnRestore.Enabled;

        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textScriptPath.Text))
                return;
            string selDirectory = CommonClass.getFileInfo(textScriptPath.Text, "directory");
            Process.Start("explorer.exe", $"{selDirectory}");
        }

        private void comboBackupDB_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
