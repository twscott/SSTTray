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
    public partial class AdjustInOut : Form
    {
        List<string> dgv1SignIDList = null;
        public AdjustInOut()
        {
            InitializeComponent();
        }


        private void fillInDgv(DataGridView dgv)
        {
            string flowstepCondition;
            string sqlStr=null;
            DataTable dt=null;
            if (dgv1SignIDList==null)
            {
                flowstepCondition = String.IsNullOrEmpty(comboFlowStep.Text) ? "" : " and FLOW_STEP like '%" + comboFlowStep.Text + "%' ";
                sqlStr = "SELECT `SIGNID`,`SUBFLOWID`,`FLOW_STEP`,`USER_ID`,`InputQuan`,`OutputQuan`, " +
                    " `StepLeft`,`AccQuan`,`Start_TIME`,`Finish_Time` FROM `mfo_sign` " +
                    " where `OutputQuan` <= " + comboMaxLimit.Text + " and OutputQuan > 0 " +
                    " and `Finish_Time` >= '" + dtp1.Text + "' and `Finish_Time` <= '" + dtp2.Text + "' " +
                    flowstepCondition +
                    " ORDER BY `Finish_Time` ";
                dt = CommonClass.getSQLDataTable(sqlStr);
            } else
            {
                sqlStr = "SELECT `SIGNID`,`SUBFLOWID`,`FLOW_STEP`,`USER_ID`,`InputQuan`,`OutputQuan`, " +
                    " `StepLeft`,`AccQuan`,`Start_TIME`,`Finish_Time` FROM `mfo_sign` " +
                    " where SIGNID in (" + String.Join(",", dgv1SignIDList) + ")" +
                    " ORDER BY `Finish_Time` ";
                dt = CommonClass.getSQLDataTable(sqlStr);
            }
            UI_CommonClass.dtTodgv(dgv, dt);
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            dgv1SignIDList = null;
            fillInDgv(dgv1);
            tabControl1.SelectedTab = tabControl1.TabPages[0];
            dgv2.DataSource = null;
        }

        private void AdjustInOut_Load(object sender, EventArgs e)
        {
            dtp1.Value = DateTime.Now.AddMonths(-1);
            dtp2.Value = DateTime.Now.AddDays(-1);
        }
        
        //調整切割
        private void adjustCut(DataRow dr, int matLimit, string targetFlowStep)
        {
            string sqlStr = "SELECT `SIGNID`,`SUBFLOWID`,`FLOW_STEP`,`USER_ID`,`InputQuan`," +
                " `OutputQuan`, `StepLeft`,`AccQuan`,`Start_TIME`,`Finish_Time` " +
                " FROM `mfo_sign` where SUBFLOWID={0} ORDER BY `Finish_Time` ";
            DataTable singDt = null;
            DataRow targetDr = null;
            int targetIdx = 0;
            string targetUpdate = "update `mfo_sign` set `InputQuan`={0}, OutputQuan={1}, AccQuan={2} where `SIGNID`={3}";
            int inputQtyInt=0, outPutQtyInt=0, accQtyInt=0;
            int oldOutPutQtyInt, oldAccQtyInt;
            int preAccQtyInt=0, nextInputQtyInt=0;
            int flowStartQty;
            long signId = 0;
            string nextInputUpd;

            //-------------------------------------------------------
            sqlStr = String.Format(sqlStr, dr["subflowID"].ToString());
            singDt = CommonClass.getSQLDataTable(sqlStr);
            for(targetIdx = singDt.Rows.Count-1; targetIdx >=0; targetIdx--)
            {
                if(Convert.ToInt32(singDt.Rows[targetIdx]["OutputQuan"]) <= matLimit &&
                    singDt.Rows[targetIdx]["FLOW_STEP"].ToString().Contains(targetFlowStep))
                {
                    signId = Convert.ToInt32(singDt.Rows[targetIdx]["SIGNID"]);
                    flowStartQty = Convert.ToInt32(singDt.Rows[0]["InputQuan"]);
                    if(targetIdx > 0)
                    {
                        preAccQtyInt = Convert.ToInt32(singDt.Rows[targetIdx - 1]["AccQuan"]);
                    }
                        
                    oldOutPutQtyInt = Convert.ToInt32(singDt.Rows[targetIdx]["OutputQuan"]);
                    oldAccQtyInt = Convert.ToInt32(singDt.Rows[targetIdx]["AccQuan"]);
                    accQtyInt = oldAccQtyInt - oldOutPutQtyInt;
                    CommonClass.execSQLNonQuery(String.Format(targetUpdate, inputQtyInt.ToString(), outPutQtyInt.ToString(),
                        accQtyInt.ToString(), signId.ToString()));
                    if(singDt.Rows.Count-1 > targetIdx )
                    {
                        if(targetIdx > 0)
                            nextInputQtyInt = Convert.ToInt32(singDt.Rows[targetIdx - 1]["InputQuan"]);
                        if(nextInputQtyInt == oldAccQtyInt && targetIdx+2 >= singDt.Rows.Count)
                        {
                            signId = Convert.ToInt32(singDt.Rows[targetIdx+1]["SIGNID"]);
                            nextInputUpd = $"update `mfo_sign` set `InputQuan`={accQtyInt} where `SIGNID`={signId}";
                            CommonClass.execSQLNonQuery(nextInputUpd);
                        }
                    }
                    break;
                }
            }
        }

        //調整加壓
        private void adjustPress(DataRow dr, int matLimit, string targetFlowStep)
        {
            string sqlStr = "SELECT `SIGNID`,`SUBFLOWID`,`FLOW_STEP`,`USER_ID`,`InputQuan`," +
                " `OutputQuan`, `StepLeft`,`AccQuan`,`Start_TIME`,`Finish_Time` " +
                " FROM `mfo_sign` where SUBFLOWID={0} ORDER BY `Finish_Time` ";
            DataTable singDt = null;
            DataRow targetDr = null;
            int targetIdx = 0;
            string targetUpdate = "update `mfo_sign` set  OutputQuan={0}, AccQuan={1} where `SIGNID`={2}";
            int inputQtyInt, outPutQtyInt, accQtyInt;
            int oldOutPutQtyInt, oldAccQtyInt;
            int preAccQtyInt=0, nextInputQtyInt=0;
            int flowStartQty;
            string preFlowStep=null;
            long signId = 0;
            string nextInputUpd;
            //-------------------------------------------------------
            sqlStr = String.Format(sqlStr, dr["subflowID"].ToString());
            singDt = CommonClass.getSQLDataTable(sqlStr);
            for (targetIdx = singDt.Rows.Count - 1; targetIdx >= 0; targetIdx--)
            {
                if (Convert.ToInt32(singDt.Rows[targetIdx]["OutputQuan"]) <= matLimit &&
                    singDt.Rows[targetIdx]["FLOW_STEP"].ToString().Contains(targetFlowStep))
                {
                    signId = Convert.ToInt32(singDt.Rows[targetIdx]["SIGNID"]);
                    flowStartQty = Convert.ToInt32(singDt.Rows[0]["InputQuan"]);
                    if(targetIdx > 0)
                    {
                        preAccQtyInt = Convert.ToInt32(singDt.Rows[targetIdx - 1]["AccQuan"]);
                        preFlowStep = singDt.Rows[targetIdx - 1]["FLOW_STEP"].ToString();
                    }
                        
                    oldOutPutQtyInt = Convert.ToInt32(singDt.Rows[targetIdx]["OutputQuan"]);
                    oldAccQtyInt = Convert.ToInt32(singDt.Rows[targetIdx]["AccQuan"]);
                    accQtyInt = oldAccQtyInt - oldOutPutQtyInt;
                    if (preFlowStep == targetFlowStep)
                    {
                        inputQtyInt = flowStartQty - preAccQtyInt;
                        outPutQtyInt = accQtyInt - inputQtyInt;
                    }
                    else
                    {
                        inputQtyInt = preAccQtyInt;
                        outPutQtyInt = accQtyInt;
                    }
                    CommonClass.execSQLNonQuery(String.Format(targetUpdate, outPutQtyInt.ToString(),
                        accQtyInt.ToString(), signId.ToString()));
                    break;
                }
            }
        }

        //調整色碼
        private void adjustColor(DataRow dr, int matLimit, string targetFlowStep)
        {
            string sqlStr = "SELECT `SIGNID`,`SUBFLOWID`,`FLOW_STEP`,`USER_ID`,`InputQuan`," +
                " `OutputQuan`, `StepLeft`,`AccQuan`,`Start_TIME`,`Finish_Time` " +
                " FROM `mfo_sign` where SUBFLOWID={0} ORDER BY `Finish_Time` ";
            DataTable singDt = null;
            DataRow targetDr = null;
            int targetIdx = 0;
            string targetUpdate = "update `mfo_sign` set `InputQuan`={0}, OutputQuan={1}, AccQuan={2} where `SIGNID`={3}";
            int inputQtyInt, outPutQtyInt, accQtyInt;
            int oldOutPutQtyInt, oldAccQtyInt;
            int preAccQtyInt=0, nextInputQtyInt=0;
            string preFlowStep=null;
            int flowStartQty;
            long signId = 0;
            string nextInputUpd;
            //-------------------------------------------------------
            sqlStr = String.Format(sqlStr, dr["subflowID"].ToString());
            singDt = CommonClass.getSQLDataTable(sqlStr);
            for (targetIdx = singDt.Rows.Count - 1; targetIdx >= 0; targetIdx--)
            {
                if (Convert.ToInt32(singDt.Rows[targetIdx]["OutputQuan"]) <= matLimit &&
                    singDt.Rows[targetIdx]["FLOW_STEP"].ToString().Contains(targetFlowStep))
                {
                    signId = Convert.ToInt32(singDt.Rows[targetIdx]["SIGNID"]);
                    flowStartQty = Convert.ToInt32(singDt.Rows[0]["InputQuan"]);
                    if(targetIdx > 0)
                    {
                        preAccQtyInt = Convert.ToInt32(singDt.Rows[targetIdx - 1]["AccQuan"]);
                        preFlowStep = singDt.Rows[targetIdx - 1]["FLOW_STEP"].ToString();
                    }
                        
                    oldOutPutQtyInt = Convert.ToInt32(singDt.Rows[targetIdx]["OutputQuan"]);
                    oldAccQtyInt = Convert.ToInt32(singDt.Rows[targetIdx]["AccQuan"]);
                    accQtyInt = oldAccQtyInt - oldOutPutQtyInt;
                    if (preFlowStep == targetFlowStep)
                    {
                        inputQtyInt = flowStartQty - preAccQtyInt;
                        outPutQtyInt = accQtyInt - inputQtyInt;
                    }
                    else
                    {
                        inputQtyInt = preAccQtyInt;
                        outPutQtyInt = accQtyInt;
                    }
                    
                    CommonClass.execSQLNonQuery(String.Format(targetUpdate, inputQtyInt.ToString(), outPutQtyInt.ToString(),
                        accQtyInt.ToString(), signId.ToString()));
                    if (singDt.Rows.Count - 1 > targetIdx)
                    {
                        if(targetIdx > 0)
                            nextInputQtyInt = Convert.ToInt32(singDt.Rows[targetIdx - 1]["InputQuan"]);
                        if (nextInputQtyInt == oldAccQtyInt && targetIdx + 1 >= singDt.Rows.Count)
                        {
                            signId = Convert.ToInt32(singDt.Rows[targetIdx + 1]["SIGNID"]);
                            nextInputUpd = $"update `mfo_sign` set `InputQuan`={accQtyInt} where `SIGNID`={signId}";
                            CommonClass.execSQLNonQuery(nextInputUpd);
                        }
                    }
                    foreach (DataRow quanjianDr in singDt.Rows)
                    {
                        if(Convert.ToInt32(quanjianDr["InputQuan"])== oldOutPutQtyInt && quanjianDr["FLOW_STEP"].ToString()=="外檢2")
                        {
                            nextInputUpd = $"update `mfo_sign` set `InputQuan`={accQtyInt} where `SIGNID`={quanjianDr["SIGNID"]}";
                            CommonClass.execSQLNonQuery(nextInputUpd);
                        }
                    }
                    break;
                }
            }
        }

        private void btnAdjust_Click(object sender, EventArgs e)
        {
            DataTable dt=(DataTable)dgv1.DataSource;
            foreach(DataRow dr in dt.Rows)
            {
                switch (dr["FLOW_STEP"].ToString())
                {
                    case "花蓮切割":
                    case "切割":
                        adjustCut(dr, int.Parse(comboMaxLimit.Text), comboFlowStep.Text);
                        break;
                    case "花蓮底漆":
                    case "底漆":
                    case "底漆1":
                    case "底漆2":
                    case "花蓮色碼":
                    case "色碼":
                        adjustColor(dr, int.Parse(comboMaxLimit.Text), comboFlowStep.Text);
                        break;
                    default: //加壓 與其他製程
                        adjustPress(dr, int.Parse(comboMaxLimit.Text), comboFlowStep.Text);
                        break;
                }
                if (dgv1SignIDList == null)
                    dgv1SignIDList = new List<string>();
                dgv1SignIDList.Add(dr["SIGNID"].ToString());
                if (chkboxFirstRec.Checked)
                    break;
            }
            fillInDgv(dgv2);
            tabControl1.SelectedTab = tabControl1.TabPages[1];
            
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Text != "單筆修改")
                return;
            radioBefore.Checked = (dgv2.DataSource == null || ((DataTable)dgv2.DataSource).Rows.Count <= 0);
            btnQuerySingle.PerformClick();
        }

        private void btnQuerySingle_Click(object sender, EventArgs e)
        {
            string selectedSubflowID = "";
            if(radioBefore.Checked)
            {
                if (dgv1.DataSource == null || ((DataTable)dgv1.DataSource).Rows.Count <= 0 || dgv1.SelectedRows==null)
                    return;
                selectedSubflowID = dgv1.SelectedRows[0].Cells["SUBFLOWID"].Value.ToString();
            } 
            else
            {
                if (dgv2.DataSource == null || ((DataTable)dgv2.DataSource).Rows.Count <= 0 || dgv2.SelectedRows == null)
                    return;
                selectedSubflowID = dgv2.SelectedRows[0].Cells["SUBFLOWID"].Value.ToString();
            }

            string sqlStr = "SELECT `SIGNID`,`SUBFLOWID`,`FLOW_STEP`,`USER_ID`,`InputQuan`,`OutputQuan`, " +
                " `StepLeft`,`AccQuan`,`Start_TIME`,`Finish_Time` " +
                " FROM `mfo_sign` " +
                " where SUBFLOWID='" + selectedSubflowID + "' ORDER BY `Finish_Time`";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            UI_CommonClass.dtTodgv(dgv3, dt);
        }

        private void btnUpdateSingle_Click(object sender, EventArgs e)
        {
            if (dgv3.Rows.Count <= 0)
                return;
            DataTable dt = (DataTable)(dgv3.DataSource);
            Dictionary<string, string> sqlParams = new Dictionary<string, string>();
            string updSql = "update  mfo_sign set InputQuan=@InputQuan, OutputQuan=@OutputQuan, StepLeft=@StepLeft, " +
                "AccQuan=@AccQuan  where SIGNID=@SIGNID ";
            foreach(DataRow dr in dt.Rows)
            {
                sqlParams.Clear();
                for(int i=0; i< dt.Columns.Count; i++)
                {
                    switch(dt.Columns[i].ColumnName)
                    {
                        case "SIGNID":
                            sqlParams.Add("@SIGNID", dr[i].ToString());
                            break;
                        case "InputQuan":
                            sqlParams.Add("@InputQuan", dr[i].ToString());
                            break;
                        case "OutputQuan":
                            sqlParams.Add("@OutputQuan", dr[i].ToString());
                            break;
                        case "StepLeft":
                            sqlParams.Add("@StepLeft", dr[i].ToString());
                            break;
                        case "AccQuan":
                            sqlParams.Add("@AccQuan", dr[i].ToString());
                            break;
                    }
                }
                CommonClass.execSQLNonQueryParams(updSql, sqlParams);
            }
            MessageBox.Show("更改完成!!!");
        }
    }
}
