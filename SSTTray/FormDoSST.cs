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
    public partial class FormDoSST : Form
    {
        public FormDoSST()
        {
            InitializeComponent();
        }


        private void btnDoSST_Click(object sender, EventArgs e)
        {
            txtEndTime.Text = "";
            txtStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            int currHour = DateTime.Now.Hour;
            int currMin = DateTime.Now.Minute;
            Refresh();
            sst.do_sst(currHour, currMin, Constants.SSTConnString);
            txtLogoutTime.Text = CommonApp.loginTime;
            txtLoginTime.Text = CommonApp.logoutTime;
            txtStartTime.Text = CommonApp.sstProcessStart;
            txtEndTime.Text = CommonApp.sstProcessEnd;


            txtStackTrace.Text = CommonApp.sstLastErrMsg;
            txtLastErrorTime.Text = CommonApp.sstLastErrTime;
            txtDayErrCnt.Text = CommonApp.sstDayProcessErrCnt.ToString();
            txtErrorCnt.Text = CommonApp.sstDayProcessErrCnt.ToString();
        }

        private void FormDoSST_Load(object sender, EventArgs e)
        {

        }
    }
}
