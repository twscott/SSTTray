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
    public partial class FormRespartAP : Form
    {
        public FormRespartAP()
        {
            InitializeComponent();
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            DialogResult dialofResult = MessageBox.Show("電腦即將重啟， 是否確認？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(dialofResult == DialogResult.Yes)
            {
                CommonClass.rebootComputer();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialofResult = MessageBox.Show("電腦即將重啟， 是否確認？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialofResult == DialogResult.Yes)
            {
                CommonClass.restartApp();
            }
        }

        private void btnCauculate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRestartAps.Text))
                return;
            List<string> prgTobeRestart = txtRestartAps.Text.Split(',').ToList();
            int idx = -1;
            string arguments = null;
            string prgName = null;
            foreach(string prg in prgTobeRestart)
            {
                idx = prg.ToLower().IndexOf(".exe");
                if (prg.Length > idx + 4)
                    arguments = prg.Substring(idx + 4);
                else
                    arguments = null;
                prgName = prg.Substring(0, idx + 4);
                CommonClass.killProcessByFullPathName(prgName);
                CommonClass.startProcess(prgName, arguments);
            }
        }

        private void FormRespartAP_Load(object sender, EventArgs e)
        {
            txtRestartAps.Text = Constants.getProperty("Restart");
        }

        private void btnShutDown_Click(object sender, EventArgs e)
        {
            DialogResult dialofResult = MessageBox.Show("電腦即將關機， 是否確認？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialofResult == DialogResult.Yes)
            {
                CommonClass.Shutdown(); 
            }
        }


        private void btnSendMail_Click_1(object sender, EventArgs e)
        {
            CommonClass.dsendmail(txtEmailAdd.Text, "測試", txtSubject.Text, txtBody.Text);
        }

        private void btnReceiveMail_Click(object sender, EventArgs e)
        {
            EMailRcvClient imapC = new EMailRcvClient();
            imapC.receivePop3(txtEmailServer.Text);
        }
    }
}
