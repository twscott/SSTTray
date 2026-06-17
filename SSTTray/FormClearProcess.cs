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
    public partial class FormClearProcess : Form
    {
        public FormClearProcess()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                int killCnt = CommonClass.ifProcessRunning(txtProcessName.Text);
                if(killCnt > 0)
                    CommonClass.killProcessByName(txtProcessName.Text);
                MessageBox.Show("刪除 " + killCnt  + " 個程序", "Kill Process", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Kill Process 失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
    }
}
