using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using FirstOhm;

namespace TaskTrayApplication
{
    public partial class FormUDPQueue : Form
    {
        public FormUDPQueue()
        {
            InitializeComponent();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                if(CommonApp.startPrintTime==null)
                    lblStartPrint.Text = "開始列印時間： 無";
                else
                    lblStartPrint.Text = "開始列印時間：" + CommonApp.startPrintTime;
                if (CommonApp.endPrintTime == null)
                    lblEndprintTime.Text = "完成列印時間： 無";
                else
                    lblEndprintTime.Text = "完成列印時間：" + CommonApp.endPrintTime;
                string sqlStr = "SELECT `ID`,`udp_command` 指令,`udp_params` 參數,`CREATED` 發送時間 FROM `mfo_dailyreportqueue` ";
                DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.updQueueConn);
                dgv1.DataSource = null;
                UI_CommonClass.dtTodgvWithChkbox(dt, dgv1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            try
            {
                List<DataGridViewRow> listDR = UI_CommonClass.GetAllCheckedRows(dgv1, "選取");
                if (listDR == null || listDR.Count == 0)
                    MessageBox.Show("查無資料");
                string sqlStr = "Delete From mfo_dailyreportqueue where ID = ";
                foreach (DataGridViewRow dr in listDR)
                {
                    CommonClass.execSQLNonQuery(sqlStr + dr.Cells["ID"].Value, Constants.updQueueConn);
                }
                MessageBox.Show("刪除完成！！");
                btnQuery.PerformClick();
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void FormUDPQueue_Load(object sender, EventArgs e)
        {
            btnQuery.PerformClick();
        }

        private void chkboxSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            UI_CommonClass.dgvChkAll(dgv1, "選取", chkboxSelectAll.Checked);
        }

        private void btnExecQueue_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv1.Rows.Count == 0)
                    return;
                if (CommonApp.endPrintTime != null && DateTime.Now.Subtract(DateTime.Parse(CommonApp.startPrintTime)).TotalMinutes >= 10)
                    CommonApp.endPrintTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                else
                {
                    DialogResult dr = MessageBox.Show("可能正在列印中, 是否確定要從這邊列印?", "", MessageBoxButtons.YesNo);
                    switch (dr)
                    {
                        case DialogResult.Yes:
                            CommonApp.endPrintTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                            break;
                        default://以上都不符合走這個
                            return;
                    }
                }
                CommonApp.process_udp_all();
                //CommonApp.processUDPList(null, false);
            }
             catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
