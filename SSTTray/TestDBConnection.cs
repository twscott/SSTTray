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
    public partial class TestDBConnection : Form
    {
        Dictionary<string, string> conStrings = null;
        public TestDBConnection()
        {
            InitializeComponent();
        }

        private void getConnDict(bool ifReload=false)
        {
            if (ifReload)
                conStrings = null;
            if (conStrings == null)
            {
                conStrings = new Dictionary<string, string>();
                conStrings.Add("MFOFlowConnString", Constants.MFOFlowConnString);
                conStrings.Add("TEST_MFOFlowConnString", Constants.TEST_MFOFlowConnString);
                conStrings.Add("ACLConnString", Constants.ACLConnString);
                conStrings.Add("SampleConnection", Constants.SampleConnection);
                conStrings.Add("WarehouseConnString", Constants.WarehouseConnString);
                conStrings.Add("MFOFlowConnString172_33", Constants.MFOFlowConnString172_33);
                conStrings.Add("ConutriTCConnString", Constants.ConutriTCConnString);
                conStrings.Add("ProcurmentConnection", Constants.ProcurmentConnection);
            }
        }

        private void TestDBConnection_Load(object sender, EventArgs e)
        {
            getConnDict();
        }

        private void btnConnTest_Click(object sender, EventArgs e)
        {
            string errStr = null;
            if(comboConnections.Text == "全部測試")
            {
                foreach(KeyValuePair<string, string> connItem in conStrings)
                {
                    textConnStr.Text = connItem.Value;
                    try
                    {
                        if (CommonClass.checkDB_Conn(connItem.Value, out errStr))
                            textSucess.Text += connItem.Key + Environment.NewLine;
                        else
                            textFail.Text += connItem.Key + "：" + errStr + Environment.NewLine;
                    } catch(Exception ex)
                    {
                        textFail.Text += connItem.Key + "：" + ex.Message + Environment.NewLine;
                    }
                }
            } else
            {
                textConnStr.Text = conStrings[comboConnections.Text];
                try
                {
                    if (CommonClass.checkDB_Conn(conStrings[comboConnections.Text], out errStr))
                        textSucess.Text += comboConnections.Text + Environment.NewLine;
                    else
                        textFail.Text += comboConnections.Text + "：" + errStr + Environment.NewLine;
                }
                catch (Exception ex)
                {
                    textFail.Text += comboConnections.Text + "：" + ex.Message + Environment.NewLine;
                }
            }
            MessageBox.Show("測試完成");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            textSucess.Text = textFail.Text = textConnStr.Text = "";
        }
    }
}
