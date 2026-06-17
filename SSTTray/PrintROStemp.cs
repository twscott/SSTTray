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
    public partial class PrintROStemp : Form
    {
        ExcelLib excel = null;
        public PrintROStemp()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            string rtnStr = null;
            if (chkboxReprint.Checked)
            {
                if (!CommonApp.printROFeed(dtp1.Value, out rtnStr, "進料簽收", chkboxReprint.Checked))
                    MessageBox.Show(rtnStr);
                return;
            }
                
            if (radioRo.Checked)
            {
                if (!CommonApp.printROReport(dtp1.Value, out rtnStr))
                    MessageBox.Show(rtnStr);
            } else if(radioRoFeed.Checked)
            {
                if (!CommonApp.printROFeed(dtp1.Value, out rtnStr))
                    MessageBox.Show(rtnStr);
            }

        }

        private void PrintROStemp_Load(object sender, EventArgs e)
        {
            dtp1.Text = "2020-10-14";
        }

        private void chkboxReprint_CheckedChanged(object sender, EventArgs e)
        {
            if(chkboxReprint.Checked)
                dtp1.Value = DateTime.Now.AddDays(-1);
        }
    }
}
