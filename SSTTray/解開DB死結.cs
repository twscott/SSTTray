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
    public partial class 解開DB死結 : Form
    {
        CommonBackup cmBK = new CommonBackup();
        string connStr = null;
        Dictionary<string, string> currConnDict = null;
        public 解開DB死結()
        {
            InitializeComponent();
        }

        private Dictionary<string, string> changeConnection(string comboSource, int action)
        {
            switch (comboSource)
            {
                case "TP211":
                    cmBK.dbBKConnections = cmBK.TaipeiConnections211;
                    break;
                case "HL151":
                case "花蓮 172.1681.1.151":
                    //if (action == 0)
                    cmBK.dbBKConnections = cmBK.HualianConnections;
                    //else
                    //    cmBK.dbRSConnections = cmBK.HualianConnections;
                    break;
                case "HL33":
                case "花蓮 172.1681.1.33":
                    //if (action == 0)
                    cmBK.dbBKConnections = cmBK.Hualian172_33Connections;
                    //else
                    //    cmBK.dbRSConnections = cmBK.Hualian172_33Connections;
                    break;
                case "TP33":
                case "Portal 192.168.1.33":
                case "portal":
                    //if (action == 0)
                    //cmBK.dbBKConnections = cmBK.PortalConnections;
                    //else
                    //    cmBK.dbRSConnections = cmBK.PortalConnections;
                    break;
            }
            return cmBK.dbBKConnections;
        }

        private void comboBKServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            currConnDict = changeConnection(comboDBServers.Text, 0);
            if (currConnDict != null)
                UI_CommonClass.fillInComboByDict(comboDBToKill, currConnDict, "", true);
        }

        private void 解開DB死結_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> connDict = changeConnection(comboDBServers.Text, 1);
            comboDBToKill.Text = cmBK.getBKPath(comboDBServers.Text);
            if (connDict != null)
                UI_CommonClass.fillInComboByDict(comboDBToKill, connDict);
            currConnDict = changeConnection(comboDBServers.Text, 0);
            connStr = currConnDict[comboDBToKill.Text];
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            if (currConnDict == null)
            {
                MessageBox.Show("請先選取 Server");
                return;
            }
            connStr = currConnDict[comboDBToKill.Text];

            CommonClass.killDBProcess(numericUpDown.Value, connStr);
            btnQuery.PerformClick();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (currConnDict == null)
            {
                MessageBox.Show("請先選取 Server");
                return;
            }
            connStr = currConnDict[comboDBToKill.Text];
            DataTable dt = CommonClass.getSQLDataTable("show processList", connStr);
            CommonClass.dtTodgv(dgv1, dt);
        }
    }
}
