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
using Newtonsoft.Json;

namespace TaskTrayApplication
{
    public partial class DailyReport : Form
    {
        string dgvSQL = null;
        StdExcelReport stdReport = null;
        ExcelLib excel = null;
        string noLogoutWhere = null; //忘記 Logout 則用各班時間作為起訖時間
        string whereStr = null; //作為計算 尺寸/Tol 數量的依據
        public DailyReport()
        {
            InitializeComponent();
        }

        public DailyReport(StdExcelReport reportInfo)
        {
            stdReport = reportInfo;
            InitializeComponent();
            this.Text += stdReport.winFormTitle;
        }

        private void DailyReport_Load(object sender, EventArgs e)
        {
            dtp1.Value = DateTime.Now.AddDays(-1);
        }

        private void comboUDPCommand_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string shiftName, rtnStr;
            if (radioShift2.Checked)
                shiftName = "中班";
            else if (radioShift3.Checked)
                shiftName = "晚班";
            else
                shiftName = "早班";
            string reportType = comboUDPCommand.Text.Substring(2, 4).Replace("日報", "");
            CommonApp.printDailyReport(reportType, txtEmpID.Text,
            shiftName, dtp1.Value, out rtnStr); // string
        }
    }
}
