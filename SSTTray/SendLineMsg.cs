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
    public partial class SendLineMsg : Form
    {
        LineLib linelib = new LineLib();
        public SendLineMsg()
        {
            InitializeComponent();
        }

        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            //string rtnResult = linelib.pushTextMessage(txtUserToSend.Text, txtLineMsg.Text);
            //if (string.IsNullOrEmpty(rtnResult))
            //    labelResult.Text = "傳送成功";
            //else
            //    labelResult.Text = "傳送失敗 :" + rtnResult;
            linelib.pushTextMessageByWebapiAsync(comboUser.SelectedValue.ToString(), txtLineMsg.Text);
            labelResult.Text = "訊息已傳送";
        }

        private void btnGetYieldInfo_Click(object sender, EventArgs e)
        {

            string sqlStr = "SELECT `工令單號`, `批次號`, `RTYPE`,`Tol`,`VAL`,`PPM`,`完成量`," +
                            " `外檢1良率`, `全檢1良率`,`全檢2良率`,`外檢2良率`, `完成日期` " +
                            " FROM `view_basicyield` where 完成日期 >= subDate(CURRENT_DATE, 7) and 外檢2良率 > 0 " +
                            " order by 外檢2良率 limit 3 ";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.MFOFlowConnString);
            txtLineMsg.Text = "本週最低良率：" + Environment.NewLine;
            for (int i=0; i<dt.Rows.Count; i++)
            {
                foreach (object oObject in dt.Columns)
                {
                    txtLineMsg.Text += oObject.ToString() + " : " + dt.Rows[i][oObject.ToString()].ToString() + Environment.NewLine;
                }
                txtLineMsg.Text += "**************" + Environment.NewLine;
            }
        }

        private void SendLineMsg_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> lineUserDict;
            try
            {
                string sqlstr = "SELECT `name`,`username` " +
                " FROM `xyz_users` a Join xyz_hikashop_user b on a.id = b.user_cms_id where b.line_userid > '' and b.workstatus <> 1 ";
                DataTable dt = CommonClass.getSQLDataTable(sqlstr, Constants.portalConnString);
                lineUserDict = CommonClass.dtToDictionary(dt, "name", "username");
            } catch(Exception ex)
            {
                lineUserDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>("{'康起禎','jons.kang'},{'李尚祐','sy.lee'},{'楊瀛洲','yinchou.yang'},{'花蓮廠務','hualien'},{'花蓮廠長','hl-fd'},{'蕭人碩','ronny.hsiao'},{'黃逸楓','yifeng.huang'}");
            }
            lineUserDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>("{'康起禎':'jons.kang','李尚祐':'sy.lee','楊瀛洲':'yinchou.yang','花蓮廠務':'hualien','花蓮廠長':'hl-fd','蕭人碩':'ronny.hsiao','黃逸楓':'yifeng.huang'}");
            UI_CommonClass.fillInComboByDict(comboUser, lineUserDict);
            comboUser.Text = "曾建明";
        }

        private void btnPaySendLine_Click(object sender, EventArgs e)
        {
            linelib.pushTextMessageOuter(comboUser.Text, txtLineMsg.Text);
            labelResult.Text = "訊息已傳送";
        }
    }
}
