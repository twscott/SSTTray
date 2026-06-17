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
    public partial class FormIfFoot : Form
    {
        public FormIfFoot()
        {
            InitializeComponent();
        }

        private void btnCauculate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMfono.Text))
                return;
            string sqlStr = "SELECT BASE_ID, `RTYPE`,`VAL`,`TOL`,`IfFoot`, `IfFoot` calcFoot " +
                            " FROM `mfo_base` Where `MFO_LEAD`= '" + txtMfono.Text + "' ORDER BY RTYPE";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            foreach(DataRow dr in dt.Rows)
            {
                dr["calcFoot"] = FirstohmPrds.ifFoot(dr["RTYPE"].ToString());
                if(dr["IfFoot"].ToString()!= dr["calcFoot"].ToString())
                {
                    sqlStr = "update mfo_base set IfFoot= " + dr["calcFoot"].ToString() + " where BASE_ID = " + dr["BASE_ID"].ToString();
                    CommonClass.execSQLNonQuery(sqlStr);
                }
            }
            UI_CommonClass.dtTodgv(dgv1, dt, new List<string>() { "BASE_ID" });

        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            if (FirstohmPrds.SyncBaseFoot(radioAll.Checked))
                btnSyncRtype.PerformClick();
            MessageBox.Show("計算完成！！", "計算 MFO_BASE 有腳無腳", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FormIfFoot_Load(object sender, EventArgs e)
        {
            string sqlStr;
            sqlStr  = "SELECT MFO_LEAD, `RTYPE`,`VAL`,`TOL`,`IfFoot` " +
                            " FROM `mfo_base` " +
                            " Where IfFoot= -1  ORDER BY FINDATE Desc ";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            UI_CommonClass.dtTodgv(dgv1, dt);
        }

        private void btnSyncRtype_Click(object sender, EventArgs e)
        {
            string sqlStr;
            sqlStr = "Select RTYPE from Firstohm.RESISTOR WHERE IfFoot = -1 ";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.Firstohm);
            foreach (DataRow dr in dt.Rows)
            {
                sqlStr = "SELECT `IfFoot`  FROM `mfo_base` where `RTYPE`='" + dr["RTYPE"].ToString() + "' and IfFoot >= 0 limit 1";
                var mfoIfFoot = CommonClass.getSQLScalar(sqlStr, Constants.MFOFlowConnString);
                if(mfoIfFoot != null && mfoIfFoot != DBNull.Value)
                {
                    sqlStr = "UPDATE Firstohm.RESISTOR  " +
                             " SET IfFoot = " + mfoIfFoot.ToString() + ", `createBy`='火箭', created = CURRENT_DATE  " +
                             " WHERE RTYPE = '" + dr["RTYPE"].ToString() + "'";
                    CommonClass.execSQLNonQuery(sqlStr, Constants.Firstohm);
                }
            }

            sqlStr = "Select RT_RTYPE from Firstohm_Sales.RESISTOR_TYPE WHERE IfFoot = -1 ";
            dt = CommonClass.getSQLDataTable(sqlStr, Constants.salesConnString);
            foreach (DataRow dr in dt.Rows)
            {
                sqlStr = "SELECT `IfFoot`  FROM `mfo_base` where `RTYPE`='" + dr["RT_RTYPE"].ToString() + "' and IfFoot >= 0 limit 1";
                var mfoIfFoot = CommonClass.getSQLScalar(sqlStr, Constants.MFOFlowConnString);
                if (mfoIfFoot != null && mfoIfFoot != DBNull.Value)
                {
                    sqlStr = "UPDATE Firstohm_Sales.RESISTOR_TYPE  " +
                             " SET IfFoot = " + mfoIfFoot.ToString() + ", `createBy`='火箭', created = CURRENT_DATE  " +
                             " WHERE RT_RTYPE = '" + dr["RT_RTYPE"].ToString() + "'";
                    CommonClass.execSQLNonQuery(sqlStr, Constants.salesConnString);
                }
            }
        }
    }
}
