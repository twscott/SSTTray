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
    public partial class Form產生Excel : Form
    {
        public Form產生Excel()
        {
            InitializeComponent();
        }

        private void Form產生Excel_Load(object sender, EventArgs e)
        {
            txtConnectionStr.Text = Constants.getConnection(comboConnection.Text);
        }

        private void comboConnection_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtConnectionStr.Text = Constants.getConnection(comboConnection.Text);
        }

        private void btnExec_Click(object sender, EventArgs e)
        {
            if (txtSql.Text == "")
                return;
            DataTable dt = CommonClass.getSQLDataTable(txtSql.Text, txtConnectionStr.Text);
            if (dt.Rows.Count > 0)
            {
                ExcelLib excel = new ExcelLib();
                excel.dtToExcel(dt);
            }
        }
    }
}
