using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirstOhm;

namespace TaskTrayApplication
{
    public partial class beckupWeb : Form
    {
        List<int> pidList = new List<int>();
        public beckupWeb()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listUrls.SelectedItems.Count <= 0)
                return;
            
            foreach (string targerUrl in listUrls.SelectedItems)
            {
                pidList.Clear();
                pidList.Add(CommonClass.startChrom(targerUrl, Constants.ChromeDir));
                CommonClass.wait(5);
            }    
        }

        private void beckupWeb_Load(object sender, EventArgs e)
        {
            Constants.ChromeDir = Constants.getProperty("ChromeDir", @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (string targerUrl in listUrls.Items)
            {
                pidList.Clear();
                pidList.Add(CommonClass.startChrom(targerUrl, Constants.ChromeDir));
                CommonClass.wait(5);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string autoExecItems = Constants.getProperty("autoExecItems");
            do
            {
                Constants.lastAkeebaBackup = DateTime.Now;
                CommonApp.autoAkeebaBackup();
            } while (DateTime.Now.Date > Constants.lastAkeebaBackup.Date && DateTime.Now.Hour == 3
                && autoExecItems.Contains("akeeba"));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", @"\\192.168.1.33\xampp\backup\portal");
            Process.Start("explorer.exe", @"\\172.168.1.151\xampp\htdocs\Portal\administrator\components\com_akeeba\backup");
        }
    }
}
