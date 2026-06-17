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
    public partial class FrnSAqlToXml : Form
    {
        public FrnSAqlToXml()
        {
            InitializeComponent();
        }

        private void brnGenXml_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            List<string> xparams = txtDressing.Text.Split(';').ToList();
            string[] itemArr;
            foreach(string item in xparams)
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                itemArr = item.Split(':');
                dict.Add(itemArr[0], itemArr[1]);
            }
            DataTable dt = CommonClass.getSQLDataTable(txtSql.Text, txtConnStr.Text);
            dt.TableName = dict["RecordTag"];
            txtResult.Text = (dict["DocumentXm"] + System.Environment.NewLine + CommonClass.convertDatatableToXML(dt)).Replace("DocumentElement", dict["TableTag"]);
        }

        private void txtDressing_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
