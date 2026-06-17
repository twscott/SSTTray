using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using FirstOhm;
using Newtonsoft.Json;

namespace TaskTrayApplication
{
    public partial class UDPSend : Form
    {
        List<string> standardLabelList = new List<string>();
        List<string> standardReportList = new List<string>();
        List<string> newJsonList = new List<string>();
        public UDPSend()
        {
            InitializeComponent();
        }

        private void UDPSend_Load(object sender, EventArgs e)
        {
            try
            {
                textUdpIP.Text = Constants.getProperty("UDPServer");
            }
            catch (Exception ex)
            {
                textUdpIP.Text = "192.168.1.133";
            }
            foreach (KeyValuePair<string, string> peopertyItem in Constants.propertyDic)
            {
                if (peopertyItem.Key[0] == '#')
                    continue;
                if (peopertyItem.Key.Contains("標準標籤_"))
                {
                    standardLabelList.Add(peopertyItem.Key);
                }
                if (peopertyItem.Key.Contains("標準報表_"))
                {
                    standardReportList.Add(peopertyItem.Key);
                }
                if (peopertyItem.Key.Contains("NJ_"))
                {
                    newJsonList.Add(peopertyItem.Key);
                }
            }
            if (standardLabelList.Count > 0)
                UI_CommonClass.fillInCombo(comboStandardLabel, standardLabelList, false);
            if (standardReportList.Count > 0)
                UI_CommonClass.fillInCombo(comboStandardReport, standardReportList, false);
            if (newJsonList.Count > 0)
                UI_CommonClass.fillInCombo(comboNewJson, newJsonList, false);
        }

        private void comboBackupDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (((ComboBox)sender).Text)
            {
                case "Print RO QrCode":
                    txtUDPContent.Text = "[{\"Key\":\"0.8x1.9\",\"Value\":\"20200513-6\"}, " +
                                          "{\"Key\":\"0.8x1.9\",\"Value\":\"20200513-7\"}, " +
                                          "{\"Key\":\"0.8x1.9\",\"Value\":\"20200513-8\"}, " +
                                          "{\"Key\":\"0.8x1.9\",\"Value\":\"20200513-9\"}, " +
                                          "{\"Key\":\"0.8x1.9\",\"Value\":\"20200514-1\"}]";
                    break;
                case "Save HTML To Pdf":
                    txtUDPContent.Text = "[{\"Key\":\"D:/temp/one1.html\",\"Value\":\"\"}]";
                    break;
                case "Print RO Report":
                case "列印進料簽收單":
                    txtUDPContent.Text = "[{\"Key\":\"RoDate\",\"Value\":\"2020-10-14\"}]";
                    break;
                case "列印切割日報表":
                    txtUDPContent.Text = "[{\"Key\":\"flowStep\",\"Value\":\"切割\"},{\"Key\":\"empID\",\"Value\":\"B_007\"},{\"Key\":\"shiftName\",\"Value\":\"早班\"},{\"Key\":\"reportDate\",\"Value\":\"2020/10/28\"},{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "列印底漆日報表":
                    txtUDPContent.Text = "[{'Key':'flowStep','Value':'底漆'},{'Key':'empID','Value':'B_016'},{'Key':'shiftName','Value':'中班'},{'Key':'reportDate','Value':'2020 / 11 / 03'}]";
                    break;
                case "列印外檢日報表":
                    txtUDPContent.Text = "[{\"Key\":\"flowStep\",\"Value\":\"外檢\"},{\"Key\":\"empID\",\"Value\":\"B_057\"},{\"Key\":\"shiftName\",\"Value\":\"早班\"},{\"Key\":\"reportDate\",\"Value\":\"2020/10/28\"},{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "列印加壓日報表":
                    txtUDPContent.Text = "[{\"Key\":\"flowStep\",\"Value\":\"加壓\"},{\"Key\":\"empID\",\"Value\":\"Z_007\"},{\"Key\":\"shiftName\",\"Value\":\"早班\"},{\"Key\":\"reportDate\",\"Value\":\"2020/10/28\"},{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "列印色碼日報表":
                    txtUDPContent.Text = "[{\"Key\":\"flowStep\",\"Value\":\"色碼\"},{\"Key\":\"empID\",\"Value\":\"B_035\"},{\"Key\":\"shiftName\",\"Value\":\"早班\"},{\"Key\":\"reportDate\",\"Value\":\"2020/10/28\"},{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "列印全檢日報表":
                    txtUDPContent.Text = "[{\"Key\":\"flowStep\",\"Value\":\"全檢\"},{\"Key\":\"empID\",\"Value\":\"B_040\"},{\"Key\":\"shiftName\",\"Value\":\"早班\"},{\"Key\":\"reportDate\",\"Value\":\"2020/10/28\"},{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "列印貼帶日報表":
                    txtUDPContent.Text = "[{\"Key\":\"flowStep\",\"Value\":\"貼帶\"},{\"Key\":\"empID\",\"Value\":\"B_055\"}," +
                                         "{\"Key\":\"shiftName\",\"Value\":\"早班\"},{\"Key\":\"reportDate\",\"Value\":\"2020/10/28\"}," +
                                         "{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "列印塗裝日報表":
                    txtUDPContent.Text = "[{\"Key\":\"flowStep\",\"Value\":\"塗裝\"},{\"Key\":\"empID\",\"Value\":\"C_003\"}," +
                                         "{\"Key\":\"shiftName\",\"Value\":\"早班\"},{\"Key\":\"reportDate\",\"Value\":\"2021/02/25\"}," +
                                         "{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "採購申請表":
                case "採購請料單":
                    //txtUDPContent.Text =  "[{\"Key\":\"dataType\",\"Value\":\"採購申請\"}," +
                    //                     "[{\"Key\":\"startDate\",\"Value\":\"2020/8/9\"},{\"Key\":\"endDate\",\"Value\":\"2020/11/9\"}," +
                    //                     "{\"Key\":\"size\",\"Value\":\"1x3.15\"},{\"Key\":\"cfmf\",\"Value\":\"CF\"}," +
                    //                     "{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    txtUDPContent.Text = "[{\"Key\":\"dataType\",\"Value\":\"繞線申請\"}," +
                                         "{\"Key\":\"startDate\",\"Value\":\"2020/8/9\"},{\"Key\":\"endDate\",\"Value\":\"2020/11/9\"}," +
                                         "{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "重啟列印":
                    txtUDPContent.Text = "";
                    break;
                case "RestartApp":
                    //停留一秒
                    txtUDPContent.Text = "1";
                    break;
                case "Reboot":
                    //停留一秒
                    txtUDPContent.Text = "1";
                    break;
                case "重印進料簽收單":
                    //停留一秒
                    txtUDPContent.Text = "[{\"Key\":\"DocType\",\"Value\":\"RoFeed\"},{\"Key\":\"dataDate\",\"Value\":\"2020/12/28\"}]";
                    break;
                case "匯出採購Excel":
                    txtUDPContent.Text = "[{\"Key\":\"dataType\",\"Value\":\"繞線申請\"}," +
                                                "{\"Key\":\"startDate\",\"Value\":\"2020/12/1\"}," +
                                                "{\"Key\":\"endDate\",\"Value\":\"2021/2/18\"}," +
                                                "{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "列印組帽標籤":
                    txtUDPContent.Text = "[{'Key':'referanceStandard','Value':'E24'},{'Key':'baseNum','Value':'1'}," +
                                            "{'Key':'labelcnt','Value':'2'},{ 'Key':'pno','Value':'3'}," +
                                            "{ 'Key':'mfr','Value':'4'},{ 'Key':'lno','Value':'5'}," +
                                            "{ 'Key':'minInt','Value':'7'},{ 'Key':'maxInt','Value':'6'}," +
                                            "{ 'Key':'bno','Value':'E24 10%'}," +
                                            "{ 'Key':'date','Value':'2021/3/29'}]";
                    break;
                case "成品入庫標籤":
                    txtUDPContent.Text = "[{'Key':'mfono','Value':'2-SRMMI205'},{'Key':'tol','rtype':'SRH101T'}," +
                                            "{'Key':'val','Value':'2K2'},{ 'Key':'ro','Value':'201231-3'}," +
                                            "{ 'Key':'quant','Value':'4'}]";
                    break;
                case "半成品入庫標籤":
                    txtUDPContent.Text = "[{'Key':'mfono','Value':'2-SRMMI205'},{'Key':'tol','rtype':'SRH101T'}," +
                                            "{'Key':'val','Value':'2K2'},{ 'Key':'ro','Value':'201231-3'}," +
                                            "{ 'Key':'quant','Value':'4'}]";
                    break;
            }
        }

        private void txtUDPContent_TextChanged(object sender, EventArgs e)
        {
            switch (comboUDPCommand.Text)
            {
                case "Print RO QrCode":
                    txtFinalCommand.Text = "[\"printlabel\",\"" + txtUDPContent.Text.Replace("\"", "'") + "\"]";
                    break;
                case "Save HTML To Pdf":
                    txtFinalCommand.Text = "[\"htmlToPdf\",\"" + txtUDPContent.Text.Replace("\"", "'") + "\"]";
                    break;
                case "Print RO Report":
                    txtFinalCommand.Text = "[\"roReport\",\"" + txtUDPContent.Text.Replace("\"", "'") + "\"]";
                    break;
                case "列印進料簽收單":
                    txtFinalCommand.Text = "[\"roFeedReport\",\"" + txtUDPContent.Text.Replace("\"", "'") + "\"]";
                    break;
                case "列印切割日報表":
                case "列印底漆日報表":
                case "列印外檢日報表":
                case "列印加壓日報表":
                case "列印色碼日報表":
                case "列印全檢日報表":
                case "列印貼帶日報表":
                case "列印塗裝日報表":
                    txtFinalCommand.Text = "[\"dailyReport\",\"" + txtUDPContent.Text.Replace("\"", "'") + "\"]";
                    break;
                case "採購申請表":
                case "採購請料單":
                    txtFinalCommand.Text = "[\"procurementReport\",\"" + txtUDPContent.Text.Replace("\"", "'") + "\"]";
                    break;
                case "重啟列印":
                    txtFinalCommand.Text = "[\"restartQueue\",\"" + txtUDPContent.Text.Replace("\"", "'") + "\"]";
                    break;
                case "RestartApp":
                    //停留一秒
                    txtFinalCommand.Text = "[\"restartApp\",\"" + txtUDPContent.Text.Replace("\"", "'") + "\"]";
                    break;
                case "Reboot":
                    //停留一秒
                    txtFinalCommand.Text = "[\"reboot\",\"" + txtUDPContent.Text.Replace("\"", "'") + "\"]";
                    break;
                case "重印進料簽收單":
                    txtFinalCommand.Text = "[\"printExcel\",\"" + txtUDPContent.Text.Replace("\"", "'") + "\"]";
                    break;
                case "匯出採購Excel":
                    txtFinalCommand.Text = "[\"procurmentExcel\",\"" + txtUDPContent.Text.Replace("\"", "'") + "\"]";
                    break;
                case "列印組帽標籤":
                    txtFinalCommand.Text = "[\"capsLabel\",\"" + txtUDPContent.Text.Replace("\"", "'") + "\"]";
                    break;
                case "成品入庫標準標籤":
                    txtFinalCommand.Text = "[\"stdLabelFromProperty\",\"" + txtUDPContent.Text.Replace("\"", "'") + "\",\"標準標籤_成品入庫\"]";
                    break;
                case "半成品入庫標準標籤":
                    txtFinalCommand.Text = "[\"stdLabelFromProperty\",\"" + txtUDPContent.Text.Replace("\"", "'") + "\",\"標準標籤_半成品入庫\"]";
                    break;
                case "不良品入庫標準標籤":
                    txtFinalCommand.Text = "[\"stdLabelFromProperty\",\"" + txtUDPContent.Text.Replace("\"", "'") + "\",\"標準標籤_不良品入庫\"]";
                    break;
            }

        }

        private void btnUDPSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textUdpIP.Text))
            {
                textUdpIP.Text = "127.0.0.1";
                this.Refresh();
            }

            string jsonToSend = txtFinalCommand.Text;
            try
            {
                //String[] parseData = JsonConvert.DeserializeObject<String[]>(jsonToSend);
                //string[] parseData = JsonConvert.DeserializeObject<string[]>(jsonToSend);
                var serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
                dynamic obj = serializer.Deserialize(jsonToSend, typeof(object));
                UDPLib.udpSend(jsonToSend, textUdpIP.Text, UDPLib.hostPort);
                MessageBox.Show("執行 " + jsonToSend + " 完成", "執行 UDP 命令", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@jsonToSend, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboUDPCommand_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (((ComboBox)sender).Text)
            {
                case "Print RO QrCode":
                    txtUDPContent.Text = "[{\"Key\":\"0.8x1.9\",\"Value\":\"20200513-6\"}, " +
                                          "{\"Key\":\"0.8x1.9\",\"Value\":\"20200513-7\"}, " +
                                          "{\"Key\":\"0.8x1.9\",\"Value\":\"20200513-8\"}, " +
                                          "{\"Key\":\"0.8x1.9\",\"Value\":\"20200513-9\"}, " +
                                          "{\"Key\":\"0.8x1.9\",\"Value\":\"20200514-1\"}]";
                    break;
                case "Save HTML To Pdf":
                    txtUDPContent.Text = "[{\"Key\":\"D:/temp/one1.html\",\"Value\":\"\"}]";
                    break;
                case "Print RO Report":
                case "列印進料簽收單":
                    txtUDPContent.Text = "[{\"Key\":\"RoDate\",\"Value\":\"2020-10-14\"}]";
                    break;
                case "列印切割日報表":
                    txtUDPContent.Text = "[{\"Key\":\"flowStep\",\"Value\":\"切割\"},{\"Key\":\"empID\",\"Value\":\"B_007\"},{\"Key\":\"shiftName\",\"Value\":\"早班\"},{\"Key\":\"reportDate\",\"Value\":\"2020/10/28\"},{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "列印底漆日報表":
                    txtUDPContent.Text = "[{'Key':'flowStep','Value':'底漆'},{'Key':'empID','Value':'B_016'},{'Key':'shiftName','Value':'中班'},{'Key':'reportDate','Value':'2020 / 11 / 03'}]";
                    break;
                case "列印外檢日報表":
                    txtUDPContent.Text = "[{\"Key\":\"flowStep\",\"Value\":\"外檢\"},{\"Key\":\"empID\",\"Value\":\"B_057\"},{\"Key\":\"shiftName\",\"Value\":\"早班\"},{\"Key\":\"reportDate\",\"Value\":\"2020/10/28\"},{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "列印加壓日報表":
                    txtUDPContent.Text = "[{\"Key\":\"flowStep\",\"Value\":\"加壓\"},{\"Key\":\"empID\",\"Value\":\"Z_007\"},{\"Key\":\"shiftName\",\"Value\":\"早班\"},{\"Key\":\"reportDate\",\"Value\":\"2020/10/28\"},{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "列印色碼日報表":
                    txtUDPContent.Text = "[{\"Key\":\"flowStep\",\"Value\":\"色碼\"},{\"Key\":\"empID\",\"Value\":\"B_035\"},{\"Key\":\"shiftName\",\"Value\":\"早班\"},{\"Key\":\"reportDate\",\"Value\":\"2020/10/28\"},{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "列印全檢日報表":
                    txtUDPContent.Text = "[{\"Key\":\"flowStep\",\"Value\":\"全檢\"},{\"Key\":\"empID\",\"Value\":\"B_040\"},{\"Key\":\"shiftName\",\"Value\":\"早班\"},{\"Key\":\"reportDate\",\"Value\":\"2020/10/28\"},{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "列印貼帶日報表":
                    txtUDPContent.Text = "[{\"Key\":\"flowStep\",\"Value\":\"貼帶\"},{\"Key\":\"empID\",\"Value\":\"B_055\"}," +
                                         "{\"Key\":\"shiftName\",\"Value\":\"早班\"},{\"Key\":\"reportDate\",\"Value\":\"2020/10/28\"}," +
                                         "{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "列印塗裝日報表":
                    txtUDPContent.Text = "[{\"Key\":\"flowStep\",\"Value\":\"塗裝\"},{\"Key\":\"empID\",\"Value\":\"C_003\"}," +
                                         "{\"Key\":\"shiftName\",\"Value\":\"早班\"},{\"Key\":\"reportDate\",\"Value\":\"2021/02/25\"}," +
                                         "{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "採購申請表":
                case "採購請料單":
                    //txtUDPContent.Text =  "[{\"Key\":\"dataType\",\"Value\":\"採購申請\"}," +
                    //                     "[{\"Key\":\"startDate\",\"Value\":\"2020/8/9\"},{\"Key\":\"endDate\",\"Value\":\"2020/11/9\"}," +
                    //                     "{\"Key\":\"size\",\"Value\":\"1x3.15\"},{\"Key\":\"cfmf\",\"Value\":\"CF\"}," +
                    //                     "{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    txtUDPContent.Text = "[{\"Key\":\"dataType\",\"Value\":\"繞線申請\"}," +
                                         "{\"Key\":\"startDate\",\"Value\":\"2020/8/9\"},{\"Key\":\"endDate\",\"Value\":\"2020/11/9\"}," +
                                         "{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "重啟列印":
                    txtUDPContent.Text = "";
                    break;
                case "RestartApp":
                    //停留一秒
                    txtUDPContent.Text = "1";
                    break;
                case "Reboot":
                    //停留一秒
                    txtUDPContent.Text = "1";
                    break;
                case "重印進料簽收單":
                    //停留一秒
                    txtUDPContent.Text = "[{\"Key\":\"DocType\",\"Value\":\"RoFeed\"},{\"Key\":\"dataDate\",\"Value\":\"2020/12/28\"}]";
                    break;
                case "匯出採購Excel":
                    txtUDPContent.Text = "[{\"Key\":\"dataType\",\"Value\":\"繞線申請\"}," +
                                                "{\"Key\":\"startDate\",\"Value\":\"2020/12/1\"}," +
                                                "{\"Key\":\"endDate\",\"Value\":\"2021/2/18\"}," +
                                                "{\"Key\":\"retryCnt\",\"Value\":\"3\"}]";
                    break;
                case "列印組帽標籤":
                    txtUDPContent.Text = "[{'Key':'referanceStandard','Value':'E24'},{'Key':'baseNum','Value':'1'}," +
                                            "{'Key':'labelcnt','Value':'2'},{ 'Key':'pno','Value':'3'}," +
                                            "{ 'Key':'mfr','Value':'4'},{ 'Key':'lno','Value':'5'}," +
                                            "{ 'Key':'minInt','Value':'7'},{ 'Key':'maxInt','Value':'6'}," +
                                            "{ 'Key':'bno','Value':'E24 10%'}," +
                                            "{ 'Key':'date','Value':'2021/3/29'}]";
                    break;
                case "成品入庫標準標籤":
                    txtUDPContent.Text =
                        "[{" +
                        "\"mfono\":\"2-SRMMI205\"," +
                        "\"tol\":\"2%\"," +
                        "\"rtype\":\"SRH101T\"," +
                        "\"val\":\"2K2\"," +
                        "\"ro\":\"201231-3\"," +
                        "\"date\":\"2021/8/26\"" +
                        "}, " +
                        "{" +
                        "\"mfono\":\"2-SRMMI205\"," +
                        "\"tol\":\"2%\"," +
                        "\"rtype\":\"SRH101T\"," +
                        "\"val\":\"2K2\"," +
                        "\"ro\":\"201231-3\"," +
                        "\"date\":\"2021/8/26\"" +
                        "}]";
                    break;
                case "組帽拉力不良":
                    txtUDPContent.Text =
                        "[{" +
                        "\"mfono\":\"2-SRMMI205\"," +
                        "\"tol\":\"2%\"," +
                        "\"rtype\":\"SRH101T\"," +
                        "\"val\":\"2K2\"," +
                        "\"ro\":\"201231-3\"," +
                        "\"date\":\"2021/8/26\"" +
                        "}, " +
                        "{" +
                        "\"mfono\":\"2-SRMMI205\"," +
                        "\"tol\":\"2%\"," +
                        "\"rtype\":\"SRH101T\"," +
                        "\"val\":\"2K2\"," +
                        "\"ro\":\"201231-3\"," +
                        "\"date\":\"2021/8/26\"" +
                        "}]";
                    break;
            }
        }

        private void btnSendStandard_Click(object sender, EventArgs e)
        {
            string jsonToSend;
            if (chkboxDataFrom.Checked)
            {
                //直接從 Property 讀取資料
                //string funcName, string excelFile, 
                //string printAttrs, string staticLayout, string sqlData = "", string staticData = ""
                jsonToSend = CommonApp.genStdLabelCommandFromProperty(comboStandardLabel.Text, txtLabelData.Text);
            }
            else
            {
                jsonToSend = CommonApp.genStdLabelCommand(txtTemplateFile.Text,
                             txtPrintAttrs.Text, txtExcelLayout.Text, txtLabelData.Text);
            }
            textData.Text = jsonToSend;
            Refresh();
            try
            {
                UDPLib.udpSend(jsonToSend, textUdpIP.Text, UDPLib.hostPort);
                MessageBox.Show("執行 " + jsonToSend + " 完成", "執行 UDP 命令", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@jsonToSend, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //lblName:要列印的標籤，例如 '標準標籤_成品入庫'
        private Dictionary<string, string> parseStandardLableProperty(string lblName)
        {
            string stdLabelAtt = Constants.getProperty(lblName);
            Dictionary<string, string> stdLblDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(stdLabelAtt);
            txtTemplateFile.Text = stdLblDict["excelFile"];
            txtPrintAttrs.Text = stdLblDict["printAttrs"];
            txtExcelLayout.Text = stdLblDict["excelLayout"];
            txtLabelData.Text = stdLblDict["testData"];
            return stdLblDict;
        }

        //lblName:要列印的標籤，例如 '標準標籤_成品入庫'
        private Dictionary<string, string> parseStandardReportProperty(string reportName, string sqlData = null)
        {
            string stdReportAtt = Constants.getProperty(reportName);
            Dictionary<string, string> stdReportDict = null;
            try
            {
                stdReportDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(stdReportAtt);
                if (stdReportDict.Keys.Contains("excelFile"))
                    txtReportTemplateFile.Text = stdReportDict["excelFile"].Replace("^", "\"");
                if (stdReportDict.Keys.Contains("printAttrs"))
                    txtReportPrintAttrs.Text = stdReportDict["printAttrs"].Replace("^", "\"");
                if (stdReportDict.Keys.Contains("staticLayout"))
                    txtStaticLayout.Text = stdReportDict["staticLayout"].Replace("^", "\"");
                if (stdReportDict.Keys.Contains("sqlData"))
                {
                    if (txtReportData.Text == "" || chkboxReportDataFrom.Checked)
                    {
                        txtReportData.Text = stdReportDict["sqlData"].Replace("^", "\"");
                    }
                }
                if (stdReportDict.Keys.Contains("staticData"))
                {
                    if (txtStaticData.Text == "" || chkboxReportDataFrom.Checked)
                    {
                        txtStaticData.Text = stdReportDict["staticData"].Replace("^", "\"");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{reportName} Json 內容錯誤", "Property Json 錯誤");
            }
            return stdReportDict;
        }

        private void comboStandardLabel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboStandardLabel.Text == "")
                return;

            parseStandardLableProperty(comboStandardLabel.Text);
        }

        private void btnSendStandardReport_Click(object sender, EventArgs e)
        {
            string jsonToSend = null;
            if (chkboxReportDataFrom.Checked)
            {
                //直接從 Property 讀取資料
                jsonToSend = CommonApp.genStdReportByProperty(comboStandardReport.Text, txtStaticData.Text, txtReportData.Text);
            }
            else
            {
                jsonToSend = CommonApp.genStdReportCommand(txtReportTemplateFile.Text,
                             txtReportPrintAttrs.Text, txtStaticLayout.Text, txtReportData.Text, txtStaticData.Text);
            }
            textReportData.Text = jsonToSend;
            Refresh();
            try
            {
                //var serializer = new JavaScriptSerializer();
                //serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
                UDPLib.udpSend(jsonToSend, textUdpIP.Text, UDPLib.hostPort);
                MessageBox.Show("執行 " + jsonToSend + " 完成", "執行 UDP 命令", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@jsonToSend, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboStandardReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboStandardReport.Text == "")
                return;
            txtStaticData.Text = txtReportData.Text = txtReportTemplateFile.Text =
            txtReportPrintAttrs.Text = txtStaticLayout.Text = "";
            Refresh();
            parseStandardReportProperty(comboStandardReport.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textUdpIP.Text))
            {
                textUdpIP.Text = "127.0.0.1";
                this.Refresh();
            }

            string jsonToSend = txtJsonPrint.Text;
            if (string.IsNullOrEmpty(jsonToSend))
            {
                MessageBox.Show("請先輸入 udp Json", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                var serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
                dynamic obj = serializer.Deserialize(jsonToSend, typeof(object));
                UDPLib.udpSend(jsonToSend, textUdpIP.Text, UDPLib.hostPort);
                MessageBox.Show("執行 " + jsonToSend + " 完成", "執行 UDP 命令", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@jsonToSend, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtJsonPrint_TextChanged(object sender, EventArgs e)
        {
            ((TextBox)sender).Text.Replace("\";\"", "\",\"").Replace("';'", "','");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string jsonToSend;
            if (chkboxReportDataFrom.Checked)
            {
                // '(單引號)-> ^, "(雙引號)-> ~ 
                //直接從 Property 讀取資料
                jsonToSend = CommonApp.genStdReportByProperty(comboStandardReport.Text,
                    txtReportData.Text, txtStaticData.Text);
            }
            else
            {
                jsonToSend = CommonApp.genStdReportCommand(txtReportTemplateFile.Text,
                             txtReportPrintAttrs.Text, txtStaticLayout.Text,
                             txtReportData.Text, txtStaticData.Text);
            }
            textReportData.Text = jsonToSend;
            Refresh();
            try
            {
                //var serializer = new JavaScriptSerializer();
                //serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
                UDPLib.udpSend(jsonToSend, textUdpIP.Text, UDPLib.hostPort);
                MessageBox.Show("執行 " + jsonToSend + " 完成", "執行 UDP 命令", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@jsonToSend, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtPrintAttrs_TextChanged(object sender, EventArgs e)
        {

        }


        private void btnNewUDP_Click(object sender, EventArgs e)
        {
            UDPLib.udpSend(txtNewJson.Text, textUdpIP.Text, UDPLib.hostPort);
            MessageBox.Show($"執行 {comboNewJson.Text} 完成", "執行 UDP 命令", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void comboNewJson_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(comboNewJson.Text)
            {
                case "NJ_組帽生產日報":
                    txtNewJson.Text =
                    @"{
						'udp_type':'newJson',
						'udp_func':'zumaoDailyReport',
						'propertyName':'NJ_組帽生產日報',
						'reportDate':'2022/10/11',
						'shiftType':'中班',
						'EmpNo':'Z_006',
						'EmpName':'裴氏清平'
					}";
                    break;
                case "NJ_組帽拉力不合格報表":
                    txtNewJson.Text =
                    @"{
                            'udp_type':'newJson',
	                        'udp_func':'defectZuMou',
	                        'propertyName':'NJ_組帽拉力不合格報表',
	                        'reportDate':'2022/06/08',
	                        'shiftType':'早班',
	                        'EmpNo':'C_100',
	                        'EmpName':'蕭人碩'
                     }";
                    break;
                case "NJ_收據釘貼單toPdf":
                    txtNewJson.Text =
                    @"{
                             'udp_type':'newJson',
                             'propertyName':'NJ_收據釘貼單toPdf',
                             'udp_func':'NJ_收據釘貼單toPdf',
                             'form_type_id':'3', 
                             'id_apply':'88', 
                             'user_id':'229',
                             'user_name':'陳水扁',
                             'apply_date':'2022-08-19 14:43:53', 
                             'formtype_name':'收據釘貼單', 
                             'form_note':'地點:花蓮',
                             'OutputPath':'D:/wamp64/www/portal/PDF/temp',
                             'detail':""[{ '購買日期':'2022-08-02','廠商':'123','內容':'uploadtest','用途':'uploadtest','金額':'20'}]""
                     }";
                    break;
                case "NJ_費用申請單toPdf":
                    txtNewJson.Text =
                    @"{
                             'udp_type':'newJson',
                             'propertyName':'NJ_費用申請單toPdf',
                             'udp_func':'do_DtailtoPdf',
                             'form_type_id':'3', 
                             'id_apply':'88', 
                             'user_id':'229',
                             'case_title':'案名案名案名',
                             'user_name':'郭佳霖',
                             'apply_date':'2022-01-07 16:56:47', 
                             'formtype_name':'費用申請單', 
                             'payType':'匯款',
                             'amount':'3000',
                             'OutputPath':'D:/wamp64/www/portal/PDF/temp',
                             'detail':'[{""購買日期"":""2022-01-07"",""廠商"":""11"",""內容"":""11"",""用途"":""11"",""金額"":""11"",""備註"":""11""},{""購買日期"":""2022-01-07"",""廠商"":""11"",""內容"":""11"",""用途"":""11"",""金額"":null,""備註"":""11""},{""購買日期"":""2022-01-07"",""廠商"":""11"",""內容"":""11"",""用途"":""11"",""金額"":""11"",""備註"":""11""}]'
                     }";
                    break;
                case "NJ_海外出差旅費申請單toPdf":
                    txtNewJson.Text =
                    @"{
                             'udp_type':'newJson',
                             'propertyName':'NJ_海外出差旅費申請單toPdf',
                             'udp_func':'do_DtailtoPdf',
                             'form_type_id':'4', 
                             'id_apply':'94', 
                             'user_id':'229',
                             '申請人':'郭佳霖',
                             'apply_date':'2022-08-19 14:43:53',
                             'formtype_name':'海外出差旅費申請單toPdf', 
                             '出差地點':'test地點', 
                             '目的':'test目的', 
                             '開始年':'100', 
                             '開始月':'12', 
                             '開始日':'10', 
                             '結束年':'100', 
                             '結束月':'12', 
                             '結束日':'20',
                             'USD':'30.1',
                             '總和':'3000',
                             '預支外幣':'200',
                             '出差支出':'300',
                             '私人支用':'5',
                             '歸還金額':'0',

                             'OutputPath':'D:/wamp64/www/portal/PDF/temp',
                             'detail':""[{'日期':'1219','內容':'台北-花蓮火車票','外幣金額':'400','交通費':'490','餐飲':'0','住宿':'0','其他':'0'},{'日期':'1230','內容':'花蓮-台北火車票','外幣金額':'400','交通費':'490','餐飲':'0','住宿':'0','其他':'0'},{'日期':'1220','內容':'請客戶吃飯','外幣金額':'400','交通費':'0','餐飲':'1000','住宿':'0','其他':'0'},{'日期':'1220','內容':'住宿費','外幣金額':'400','交通費':'0','餐飲':'0','住宿':'1000','其他':'0'}]""
                     }";
                    break;
                case "NJ_國內出差旅費車費申請書toPdf":
                    txtNewJson.Text =
                    @"{
                             'udp_type':'newJson',
                             'propertyName':'NJ_國內出差旅費車費申請書toPdf',
                             'udp_func':'do_DtailtoPdf',
                             'form_type_id':'4', 
                             'id_apply':'94', 
                             'user_id':'229',
                             '申請人':'郭佳霖',
                             'apply_date':'2022-08-19 14:43:53', 
                             'formtype_name':'國內出差旅費車費申請書', 
                             '出差地點':'test地點', 
                             '目的':'test目的', 
                             '開始年':'100', 
                             '開始月':'12', 
                             '開始日':'10', 
                             '結束年':'100', 
                             '結束月':'12', 
                             '結束日':'20', 
                             '總和':'3000',
                             'OutputPath':'D:/wamp64/www/portal/PDF/temp',
                             'detail':""[{'日期':'1219','內容':'台北-花蓮火車票','交通費':'490','餐飲':'0','住宿':'0','其他':'0'},{'日期':'1230','內容':'花蓮-台北火車票','交通費':'490','餐飲':'0','住宿':'0','其他':'0'},{'日期':'1220','內容':'請客戶吃飯','交通費':'0','餐飲':'1000','住宿':'0','其他':'0'},{'日期':'1220','內容':'住宿費','交通費':'0','餐飲':'0','住宿':'1000','其他':'0'}]""
                     }";
                    break;
                case "NJ_當月原物料請款":
                    txtNewJson.Text =
                    @"{
                             'udp_type':'newJson',
                             'propertyName':'NJ_當月原物料請款',
                             'udp_func':'do_askForPayment',
                             'form_type_id':'3', 
                             'id_apply':'88', 
                             'user_id':'229',
                             'case_title':'案名案名案名',
                             'user_name':'郭佳霖',
                             'manager':'郭霖佳',
                             'apply_date':'2022-01-07', 
                             'formtype_name':'當月原物料請款', 
                             'payType':'付款方式:□零用金   ■匯款   □期票',
                             'OutputPath':'D:/wamp64/www/portal/PDF/temp',
                             'detail':'[{""購買日期"":""2022/01/07"",""廠商"":""1A"",""內容"":""1A1"",""用途"":""11"",""金額"":""11"",""備註"":""11""},{""購買日期"":""2022/01/07"",""廠商"":""1B"",""內容"":""1B1"",""用途"":""11"",""金額"":""12"",""備註"":""11""},{""購買日期"":""2022/01/09"",""廠商"":""1B"",""內容"":""1B2"",""用途"":""11"",""金額"":""13"",""備註"":""11""}]',
                             'totalAmount':'36',
                             'managersigniture':'ATTPic:D:/wamp64/www/portal/PDF/temp/Ronny.png',
                             'usersigniture':'ATTPic:D:/wamp64/www/portal/PDF/temp/Ronny.png'
                     }";
                    break;
                case "NJ_考核表":
                    txtNewJson.Text =
                    @"{
                             'udp_type':'newJson',
                             'udp_func':'assessment',
                             'propertyName':'NJ_考核表',
                             'year':'2022',
                             'yearType':'上半年',
                             'fromWhere':''
                     }";
                    break;
                default :
                    txtNewJson.Text = "";
                    break;
            }
        }
    }
}
