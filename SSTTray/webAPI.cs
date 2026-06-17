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
    public partial class webAPI : Form
    {
        WebClient webClient = new WebClient();
        public webAPI()
        {
            InitializeComponent();
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            
            string url = "http://localhost:54791/PrdMgn/TestConnection";
            Task<string> result  =  webClient.webapiPost(url, "");
            //Task<string> result = webClient.webapiPost(txtUrl.Text, txtPostData.Text);
            txtResult.Text = result.Result;
        }

        private void webAPI_Load(object sender, EventArgs e)
        {

        }
    }
}
