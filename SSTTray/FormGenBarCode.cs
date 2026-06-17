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
    public partial class FormGenBarCode : Form
    {
        public FormGenBarCode()
        {
            InitializeComponent();
        }

        private void btnGenBar_Click(object sender, EventArgs e)
        {
            string conn = null;
            if(radio151.Checked) {
                conn = Constants.MFOFlowConnString;
            } else {
                conn = Constants.MFOFlowConnString172_33;
            }
            if (!string.IsNullOrEmpty(txtSubFlowID.Text))
            {
                txtBarCode.Text = FirstohmPrds.genFlowBar(txtSubFlowID.Text, 0);
            }
            else if (!string.IsNullOrEmpty(txtSignID.Text))
            {
                txtBarCode.Text = FirstohmPrds.genFlowBar(txtSignID.Text, 1);
            }               
            else
                txtBarCode.Text = "";
        }

        private void FormGenBarCode_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtSubFlowID.Text = txtSignID.Text = txtBarCode.Text = "";
        }
    }
}
