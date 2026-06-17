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
    public partial class Form電鍍報表cs : Form
    {
        DataTable dt = null;
        public Form電鍍報表cs()
        {
            InitializeComponent();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            string sqlStr = "SELECT concat(concat(substring(a.`BATCHID`,7,2),'-',a.`PLATSerID`), IF(b.`packQuant` is null,'', Concat(', ', substring(a.`BATCHID`,7,2),'-',Round(b.`PLATSerID`)))) 編號, " +
                            " concat(c.SIZE,IF(b.`packQuant` is null,'', Concat(', ',d.SIZE))) '尺寸'," +
                            " Concat(Round(a.`packQuant`),IF(b.`packQuant` is null,'', Concat(', ',Round(b.`packQuant`)))) packQuant , " +
                            " DATE_FORMAT(CURRENT_DATE,'%m-%d') 送貨日期, DATE_FORMAT(a.`expectReceive`,'%m-%d') 回電日期 " +
                            " FROM `mfo_platingpack` a " +
                            " Left Join mfo_platingpack b on a.`combineTo`=concat(b.`BATCHID`,'-',b.`PLATSerID`) " +
                            " Left Join view_signinfo c on c.SIGNID=a.`SRCSIGNID` " +
                            " Left Join view_signinfo d on d.SIGNID=a.`SRCSIGNID` " +
                            //" where a.BATCHID='20210202' and (a.combineTo not like '*%' || a.combineTo is null) order by 編號";
                            " where a.status in (1,2,3) and (a.combineTo not like '*%' || a.combineTo is null) order by 編號";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            UI_CommonClass.dtTodgv(dgv1, dt);

        }

        private void btnPrintLabel_Click(object sender, EventArgs e)
        {
            string jsonToSend = txtFinalCommand.Text;
            try
            {
                var serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
                dynamic obj = serializer.Deserialize(jsonToSend, typeof(object));
                UDPLib.udpSend(jsonToSend, txtUdpServer.Text, UDPLib.hostPort);
                MessageBox.Show("執行 " + jsonToSend + " 完成", "執行 UDP 命令", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@jsonToSend, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form電鍍報表cs_Load(object sender, EventArgs e)
        {

        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            ExcelLib excel = new ExcelLib();
            excel.dgvToExcel(dgv1);
        }

        private void comboSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboSupplier_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            txtFinalCommand.Text = "[\"plattingNote\", \"[{ 'Key':'Supplier','Value':'" + comboSupplier.Text + "'}]\"]";
        }
    }
}
