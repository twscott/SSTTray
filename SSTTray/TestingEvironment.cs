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

    public partial class TestingEvironment : Form
    {
        public TestingEvironment()
        {
            InitializeComponent();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtMFONO.Text) && string.IsNullOrEmpty(txtMFONO.Text))
            {
                MessageBox.Show("請輸入要刪除的 工令單號 或是 Subflow ID");
                return;
            }

            if (deleteOld(txtMFONO.Text, txtMFONO.Text))
            {
                MessageBox.Show("刪除完成", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                MessageBox.Show("刪除 0 筆資料");
            }
        }

        private bool deleteOld(string mfoID = null, string subFlowID = null, string destConn = null)
        {
            if (destConn == null)
                destConn = Constants.MFOFlowConnString172_33;
            string mfoTBName = radioNoFoot.Checked ? "mfo_flow" : "mfo_flow_foot";
            if (mfoID == null && subFlowID == null)
                return false;
            string sqlStr = null;
            if (mfoID != null)
                sqlStr = "SELECT FLOWID FROM " + mfoTBName + " WHERE `MASTER_MFO_ID`='" + mfoID + "' ";
            else
                sqlStr = "SELECT a.`FLOWID` FROM `mfo_flowsub` a Inner Join " + mfoTBName + " b on a.`FLOWID`=b.FLOWID WHERE a.`SUBFLOWID` = " + subFlowID;

            DataTable mfoDt = CommonClass.getSQLDataTable(sqlStr, destConn);
            foreach (DataRow dr in mfoDt.Rows)
            {
                int DelflowID = Convert.ToInt32(dr["FLOWID"]);
                if (chkboxBase.Checked)
                {
                    sqlStr = "Delete mfo_base from mfo_base Left " +
                             " Join " + mfoTBName + " on mfo_base.UID= " + mfoTBName + ".UID where " + mfoTBName + ".FLOWID=" + DelflowID;
                    CommonClass.execSQLNonQuery(sqlStr, destConn);
                }
                sqlStr = "Delete naibei from naibei " +
                            " Left Join view_signinfo on naibei.SignID = view_signinfo.SIGNID where view_signinfo.FLOWID = " + DelflowID;
                CommonClass.execSQLNonQuery(sqlStr, destConn);

                sqlStr = "Delete tiedailog from tiedailog " +
                         " Left Join " + mfoTBName + " on tiedailog.FlowID = " + mfoTBName + ".FlowID where " + mfoTBName + ".FLOWID = " + DelflowID;
                CommonClass.execSQLNonQuery(sqlStr, destConn);

                sqlStr = "Delete mfo_outcheck from `mfo_outcheck` Left Join mfo_flowsub on mfo_outcheck.SUBFLOWID=mfo_flowsub.SUBFLOWID where mfo_flowsub.FLOWID=" + DelflowID;
                CommonClass.execSQLNonQuery(sqlStr, destConn);

                sqlStr = "Delete mfo_sign from `mfo_sign` Left Join mfo_flowsub on mfo_sign.SUBFLOWID=mfo_flowsub.SUBFLOWID where mfo_flowsub.FLOWID=" + DelflowID;
                CommonClass.execSQLNonQuery(sqlStr, destConn);

                sqlStr = "Delete from mfo_flowsub where mfo_flowsub.FLOWID=" + DelflowID;
                CommonClass.execSQLNonQuery(sqlStr, destConn);
                sqlStr = "Delete from " + mfoTBName + " where FLOWID=" + DelflowID;
                CommonClass.execSQLNonQuery(sqlStr, destConn);
            }
            return true;
        }

        private Dictionary<string, string> copySubflows(string subFlowSqlStr, Dictionary<string, string> mfoidMappingDict, string destConnect = null)
        {
            string sqlStr = null;

            DataTable dt = CommonClass.getSQLDataTable(subFlowSqlStr, Constants.OfficialMFOFlowConnString);
            List<string> subflowIgnorCols = new List<string>() { "SUBFLOWID" };
            Dictionary<string, string> subflowKeyMapping = CommonClass.InsertByDt("mfo_flowsub", "SUBFLOWID", dt, subflowIgnorCols, destConnect);
            if (subflowKeyMapping.Count == 0)
                return null;
            Dictionary<string, string> signKeyMapping;
            foreach (KeyValuePair<string, string> mfoItem in mfoidMappingDict)
            {
                sqlStr = $"update `mfo_flowsub` set `FLOWID`={mfoItem.Value} where FLOWID={mfoItem.Key}";
                CommonClass.execSQLNonQuery(sqlStr, destConnect);
            }
            subflowIgnorCols = new List<string>() { "SIGNID", "Updated" };
            Dictionary<string, string> staticDict = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> subFlowItem in subflowKeyMapping)
            {
                staticDict.Clear();
                staticDict.Add("SUBFLOWID", subFlowItem.Value.ToString());
                sqlStr = $"SELECT * FROM `mfo_sign` where SUBFLOWID={subFlowItem.Key} ";
                dt = CommonClass.getSQLDataTable(sqlStr, Constants.OfficialMFOFlowConnString);
                signKeyMapping = CommonClass.InsertByDt("mfo_sign", "SIGNID", dt, subflowIgnorCols, destConnect, staticDict);
                foreach (KeyValuePair<string, string> signItem in signKeyMapping)
                {
                    sqlStr = $"SELECT * FROM `naibei` where SignID={signItem.Key} ";
                    dt = CommonClass.getSQLDataTable(sqlStr, Constants.OfficialMFOFlowConnString);
                    CommonClass.InsertByDt("naibei", "ID", dt, new List<string>() { "ID", "Created" }, destConnect, new Dictionary<string, string>() { { "SignID", signItem.Key.ToString() } });

                    sqlStr = $"SELECT * FROM `mfo_outcheck` where SignID={signItem.Key} ";
                    dt = CommonClass.getSQLDataTable(sqlStr, Constants.OfficialMFOFlowConnString);
                    CommonClass.InsertByDt("mfo_outcheck", null, dt, subflowIgnorCols, destConnect, new Dictionary<string, string>() { { "SignID", signItem.Key.ToString() }, { "SUBFLOWID", subFlowItem.Value.ToString() } });
                }
            }
            return subflowKeyMapping;
        }

        private bool createNew(string mfoID = null, string subFlowID = null, string destConn = null, bool ifFoot = false)
        {
            if (destConn == null)
                destConn = Constants.MFOFlowConnString172_35;

            if (string.IsNullOrEmpty(mfoID) && string.IsNullOrEmpty(subFlowID))
                return false;
            string sqlStr = null;
            string insertSql = null;
            DataTable signDt;
            DataTable sublowDt;
            List<string> subFlowIDList = new List<string>();
            List<string> shortIDList = new List<string>();
            List<string> sourceSubList = new List<string>();
            string extraSQL = null;
            string signID = null;
            string extraID = null;
            object extraObj = null;
            List<Dictionary<string, object>> dtDict;
            int insertedSubId, insertedMfoId;
            List<string> ignoreCols = new List<string>() { "FLOWID", "UPDATED" };
            Dictionary<string, string> mfoIDMapping = null;  //Key:舊 mfoID, Value:新 mfoID

            if (string.IsNullOrEmpty(mfoID) && string.IsNullOrEmpty(txtSubFlowID.Text))
                return false;
            else if (string.IsNullOrEmpty(mfoID) && !string.IsNullOrEmpty(subFlowID))
            {
                sqlStr = $"SELECT `MASTER_MFO_ID`, ifFoot   FROM `view_subflowinfo` where `SUBFLOWID`='{subFlowID}' ";
                DataTable tempDt = CommonClass.getSQLDataTable(sqlStr, Constants.OfficialMFOFlowConnString);
                if (tempDt.Rows.Count == 0)
                    return false;
                if (tempDt.Rows[0]["MASTER_MFO_ID"] == DBNull.Value)
                    return false;
                mfoID = tempDt.Rows[0]["MASTER_MFO_ID"].ToString();
                if (tempDt.Rows[0]["ifFoot"] != DBNull.Value)
                {
                    int tempInt = Convert.ToInt32(tempDt.Rows[0]["ifFoot"]);
                    ifFoot = (tempInt == 1);
                }
            }
            string mfoTBName = ifFoot ? "mfo_flow_foot" : "mfo_flow";
            if (!string.IsNullOrEmpty(mfoID))
            {
                sqlStr = "SELECT * FROM " + mfoTBName + " WHERE `MASTER_MFO_ID`='" + mfoID + "' ";
                DataTable mfoDT = CommonClass.getSQLDataTable(sqlStr, Constants.OfficialMFOFlowConnString);
                if (mfoDT.Rows.Count == 0)
                    return false;
                mfoIDMapping = CommonClass.InsertByDt("mfo_flow", "FLOWID", mfoDT, ignoreCols, destConn);
                if (mfoIDMapping.Count == 0)
                    return false;
                string flowIDList = string.Join(",", CommonClass.dictToList(mfoIDMapping, 0));
                if (chkboxBase.Checked)
                {
                    foreach (KeyValuePair<string, string> mfoItem in mfoIDMapping)
                    {
                        sqlStr = $"SELECT a.* from mfo_base a join mfo_flow b on a.BASE_ID=b.BASEID where b.FLOWID={mfoItem.Key} ";
                        //產生 mfo_base
                        //mfoDT = CommonClass.getSQLDataTable(sqlStr, Constants.OfficialMFOFlowConnString);
                        extraID = CommonClass.InsertByDt("mfo_base", "BASEID", mfoDT, ignoreCols, destConn).ElementAt(0).Value;
                        //更改 mfo_flow 的 base_id
                        sqlStr = $"update mfo_flow set BASE_ID={extraID} where b.FLOWID={mfoItem.Value} ";
                        CommonClass.execSQLNonQuery(sqlStr, destConn);
                    }
                }
                sqlStr = $"SELECT * FROM `mfo_flowsub`  WHERE `FLOWID` in ({flowIDList})";
            }
            Dictionary<string, string> subflowKeyMapping = null;
            subflowKeyMapping = copySubflows(sqlStr, mfoIDMapping, destConn);
            if (subflowKeyMapping == null)
            {
                txtUserSub.Text = txtResultSubID.Text = txtShortID.Text = "";
                return false;
            }



            txtUserSub.Text = string.Join(", ", CommonClass.dictToList(subflowKeyMapping, 2));
            txtResultSubID.Text = string.Join(", ", CommonClass.dictToList(subflowKeyMapping, 1));
            txtShortID.Text = string.Join(", ", CommonClass.dictToList(subflowKeyMapping, 1));
            return true;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string destConn = comboDestServer.Text == "172.168.1.35" ? Constants.MFOFlowConnString172_35 : Constants.MFOFlowConnString172_35;
            if (string.IsNullOrEmpty(txtSubFlowID.Text) && string.IsNullOrEmpty(txtMFONO.Text))
            {
                MessageBox.Show("請輸入要刪除的 工令單號 或是 Subflow ID");
                return;
            }
            deleteOld(txtMFONO.Text, txtMFONO.Text, destConn);
            if (createNew(txtMFONO.Text, txtSubFlowID.Text, destConn, radioFoot.Checked))
            {
                MessageBox.Show("測試環境建立完成");
                txtFlowStepSQL.Text = "Delete  from `mfo_sign` where `SIGNID` >= (select min(`SIGNID`) " +
                    " from `mfo_sign` WHERE `SUBFLOWID`= 'subidXXX' and `FLOW_STEP` like '%" + comboFlowStep.Text + "Delete from `mfo_sign` where `SIGNID` >= (select min(`SIGNID`) from `mfo_sign` WHERE `SUBFLOWID`= '203411' and `FLOW_STEP` like '%切割%') and `SUBFLOWID`= '203411' ') and `SUBFLOWID`= 'subidXXX' ";
                return;
            }
            else
            {
                MessageBox.Show("測試環境建立失敗", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void TestingEvironment_Load(object sender, EventArgs e)
        {
            string sqlStr = "SELECT `DEPT` displayColumn, `DEPT` valueColumn FROM `mfo_dept` where `RDSEQ` >= 0 ORDER BY `RDSEQ` ";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            UI_CommonClass.fillInCombo(comboFlowStep, dt);
        }
    }
}
