using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FirstOhm;
using Newtonsoft.Json;

namespace TaskTrayApplication
{
    class CommonApp
    {
        public static string propertyPath = @"C:\SSTTray\Property.txt";
        public static DateTime lastFtpFailLine = DateTime.Now.AddDays(-1);

        public static string portal_root_path="C:/xampp/htdocs/portal";
        public static string startPrintTime = null; //1:正在列印報表, 0:目前沒有在印表
        public static string endPrintTime = null;

        public static List<int> akeebaBKPic = new List<int>();
        public static string myIP = null;
        public static int sstProcessId = -1;
        //udpJobList 是為了防止還在列印當中， 卻來新的 UDP 要求， 而造成 excel 檔案無法開啟
        //因此會將 新來的 Job 放到 Queue 裡面 等待執行
        //static Dictionary<string, List<object>> udpJobList = new Dictionary<string, List<object>>()
        //        { {"printlabel", new List<object>()}, { "htmlToPdf", new List<object>()},
        //        {"roReport", new List<object>()}, { "dailyReport", new List<object>()} ,
        //        { "roFeedReport", new List<object>()}};

        //防止 Dead Lock
        public static int addToList(string itemType, string itemToAdd, int udpType=0)
        {
            string sqlStr = "insert into  `mfo_dailyreportqueue` (udp_command,udp_params, udpType) values " +
                $"(@itemType, @itemToAdd, @udpType)";
            Dictionary<string, string> sqlParams = new Dictionary<string, string>();
            sqlParams.Add("@itemType", itemType);
            sqlParams.Add("@itemToAdd", itemToAdd);
            sqlParams.Add("@udpType", udpType.ToString());
            //CommonClass.execSQLNonQuery(sqlStr, sqlParams);
            CommonClass.execSQLNonQueryParams(sqlStr, sqlParams, Constants.updQueueConn);
            //CommonClass.execSQLNonQuery(sqlStr);
            sqlStr = "select count(*) from mfo_dailyreportqueue where printFaileCnt < 3 ";
            return Convert.ToInt16(CommonClass.getSQLScalar(sqlStr, Constants.updQueueConn));
        }

        public static int addToList(List<string> itemList)
        {
            string sqlStr = "insert into  `mfo_dailyreportqueue` (udp_command,udp_params) values (@udp_command, @udp_params)";
            Dictionary<string, string> sqlParams = new Dictionary<string, string>();
            if (itemList.Count < 2)
                return 0;
            else if (itemList.Count == 2)
            {
                sqlParams.Add("@udp_command", itemList[0]);
                sqlParams.Add("@udp_params", itemList[1]);
            } else
            {
                sqlParams.Add("@udp_command", itemList[0]);
                itemList.RemoveAt(0);

                sqlParams.Add("@udp_params", "[" + string.Join(",", itemList) + "]");
            }

            CommonClass.execSQLNonQueryParams(sqlStr, sqlParams);
            sqlStr = "select count(*) from mfo_dailyreportqueue where < 3)  ";
            return Convert.ToInt16(CommonClass.getSQLScalar(sqlStr));
            //lock (udpJobList)
            //{
            //    udpJobList[itemType].Add(itemToAdd);
            //}
        }

        //防止 Dead Lock
        public static void removeFromList(string itemType, string reportQueueID=null)
        {
            string sqlStr = null;
            if (!string.IsNullOrEmpty(reportQueueID))
            {
                sqlStr = "select MIN(ID) from mfo_dailyreportqueue  where `udp_command` = '" + itemType + "'";
                var udpJobID = CommonClass.getSQLScalar(sqlStr, Constants.updQueueConn);
                reportQueueID = udpJobID?.ToString();
            }

            if (reportQueueID == null)
                return;
            else
                sqlStr = $"delete from mfo_dailyreportqueue where ID={reportQueueID}";
            CommonClass.execSQLNonQuery(sqlStr, Constants.updQueueConn);
            CommonApp.endPrintTime = DateTime.Now.ToString("yyyy/MM/dd");

        }

        //防止 Dead Lock
        public static DataRow  getUdpJobList(int udpType=0)
        {
            string sqlStr = $"select ID, `udp_command`, `udp_params`, ID from mfo_dailyreportqueue where printFaileCnt < 3 and udpType={udpType} Order by ID ";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.updQueueConn);
            if (dt.Rows.Count==0)
                return null;
            else
            {
                if(dt.Rows[0]["udp_command"].ToString()== "excelToJson")
                {
                    CommonClass.execSQLNonQuery($"delete from mfo_dailyreportqueue where ID='{dt.Rows[0]["ID"]}'", Constants.updQueueConn);
                }
                return dt.Rows[0];
            }
        }

        public static void processUDPList(List<string> obj=null, bool chkJoblist=true)
        {
            string rtnStr;
            string proceCommand = null;
            string proceParams=null;
            string propertyName=null;
            int beginPos = 0;
            int endPos = 0;
            DataRow udpJob;
            int reTry = 0; //如果失敗， 預計重試（retry）次數
            List<KeyValuePair<string, string>> receiveList=null;
            List<Dictionary<string, string>> receiveDict=null;
            Dictionary<string, string> stdLblDict = null;
            int jobLength = 0; //過去 15 分鐘共有幾筆資料
            if (obj != null)
            {
                if (obj.Count >= 1)
                {
                    //將收到的資料存到 DB
                    //jobLength : 過去 15 分鐘共有幾筆資料
                    jobLength = addToList(obj[0], "\"" + string.Join("\";\"", obj) + "\"");
                }
                if (obj != null && obj[0] == "dailyReport")
                {
                    receiveList = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(obj[1]);
                    FirstohmPrds.setOfficialLoginout(searchInKeyValList(receiveList, "shiftName"), 
                        Convert.ToDateTime(searchInKeyValList(receiveList, "reportDate")),
                                        searchInKeyValList(receiveList, "empID"));
                }
            }
            double minutesDiff = CommonApp.startPrintTime == null ? 9999 : DateTime.Now.Subtract(DateTime.Parse(CommonApp.startPrintTime)).TotalMinutes;
            if (!string.IsNullOrEmpty(CommonApp.endPrintTime) || (string.IsNullOrEmpty(CommonApp.endPrintTime) && minutesDiff > 10))
            {
                process_udp_all();
            }
        }

        public static DateTime fewDaysAgo(DateTime fromDate, int nDays)
        {
            string sqlStr = $"select distinct transDate from tradeData where transDate<'{fromDate.ToString("yyyy-MM-dd")}' " +
                $" order by transDate desc  limit {nDays}";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            return Convert.ToDateTime(dt.Rows[dt.Rows.Count - 1][0]);
        }

        public static void processUDPDict(Dictionary<string, string> obj, bool chkJoblist = true)
        {
            int reTry = 0; //如果失敗， 預計重試（retry）次數
            int jobLength = 0; //過去 15 分鐘共有幾筆資料
            Dictionary<string, string> newParams = new Dictionary<string, string>();
            if (obj == null)
                return;
            //將 Job 加入 mfo_dailyreportqueue
            string tempStr;
            if (obj.ContainsKey("propertyName"))
            {

            }
            foreach (KeyValuePair<string, string> dictItem in obj)
            {
                tempStr = dictItem.Value.Replace("\"", "^");
                if(string.IsNullOrEmpty(dictItem.Value))
                    newParams.Add(dictItem.Key, dictItem.Value);
                else if (dictItem.Value[0] == '{')
                    newParams.Add(dictItem.Key, dictItem.Value.Replace("\"", "^"));
                else
                    newParams.Add(dictItem.Key, dictItem.Value);
            }
            if (obj.ContainsKey("func")) //標準標籤/報表
                jobLength = addToList(obj["func"], JsonConvert.SerializeObject(newParams).Replace("\"", "~"), 1);
            else if (obj.ContainsKey("udp_func") && obj.ContainsKey("udp_type")) //新版 udp Json
                jobLength = addToList(obj["udp_func"], JsonConvert.SerializeObject(newParams).Replace("\"", "~"), 1);
            double minutesDiff = CommonApp.startPrintTime == null ? 9999 : DateTime.Now.Subtract(DateTime.Parse(CommonApp.startPrintTime)).TotalMinutes;
            if(!string.IsNullOrEmpty(CommonApp.endPrintTime) || (string.IsNullOrEmpty(CommonApp.endPrintTime) && minutesDiff > 10))
            {
                process_udp_all();
            }
            
        }

        public static void process_udp_all()
        {
            int jobCnt = 0;
            
            do
            {
                process_udp();
                new_process_udp();
                string sqlStr = "SELECT count(*) FROM `mfo_dailyreportqueue` where `printFaileCnt` < 3 ";
                jobCnt = Convert.ToInt32(CommonClass.getSQLScalar(sqlStr, Constants.updQueueConn));
            } while (jobCnt > 0);
        }

        public static void process_udp()
        {
            string rtnStr;
            string proceCommand = null;
            string proceParams = null;
            string propertyName = null;
            int beginPos = 0;
            int endPos = 0;
            Dictionary<string, string> stdLblDict = new Dictionary<string, string>();
            List<KeyValuePair<string, string>> receiveList = new List<KeyValuePair<string, string>>();
            int reTry = 0; //如果失敗， 預計重試（retry）次數
            

            //從資料庫取得未列印的列印工作
            DataRow udpJob = getUdpJobList();
            do
            {
                if (udpJob == null)
                {
                    CommonApp.endPrintTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    break;
                }

                //proceCommand = udpJobList[obj[0]][0];
                proceCommand = udpJob["udp_command"].ToString();
                List<string> udpMsg = ((string)udpJob["udp_params"]).Split(';').ToList();
                CommonApp.startPrintTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");  //新列印開始
                CommonApp.endPrintTime = null; //新列印開始
                if (proceCommand.Contains("stdLabel") || proceCommand.Contains("stdReport"))
                {
                    if (udpMsg.Count >= 3)
                    {
                        //讀 Property 的標準標籤
                        propertyName = udpMsg[2].ToString();
                        beginPos = udpMsg[1].IndexOf('[');
                        endPos = udpMsg[1].LastIndexOf(']');
                        if (beginPos >= 0 && endPos > beginPos)
                        {
                            proceParams = udpMsg[1].Substring(beginPos, endPos - beginPos + 1);
                            proceParams = proceParams.Replace("\r\n", "");
                            stdLblDict = new Dictionary<string, string>();
                            stdLblDict.Add("labelData", proceParams);
                        }
                        else
                        {
                            proceParams = udpMsg[1];
                            beginPos = udpMsg[1].IndexOf('{');
                            endPos = udpMsg[1].LastIndexOf('}');
                            proceParams = udpMsg[1].Substring(beginPos, endPos - beginPos + 1);
                            proceParams = proceParams.Replace(":\"{", ":'{");
                            proceParams = proceParams.Replace("}\"", "}'");
                            stdLblDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(proceParams);
                        }
                    }
                    else
                    {   //不是從 property 來而是從 火箭程式來的 Json
                        beginPos = udpMsg[1].IndexOf('{');
                        endPos = udpMsg[1].LastIndexOf('}');
                        proceParams = udpMsg[1].Substring(beginPos, endPos - beginPos + 1);
                        proceParams = proceParams.Replace("\r\n", "");
                        proceParams = proceParams.Replace(":\"[", ":'[");
                        proceParams = proceParams.Replace("]\"", "]'");
                        proceParams = proceParams.Replace(":\"{", ":'{");
                        proceParams = proceParams.Replace("}\"", "}'");

                        //stdLblDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(proceParams.Replace('^', '\''));
                        stdLblDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(proceParams);
                    }
                }
                else if (udpMsg.Count >= 2)
                {
                    beginPos = udpMsg[1].IndexOf('[');
                    endPos = udpMsg[1].LastIndexOf(']');
                    if (beginPos < 0 || beginPos < 0)
                        proceParams = udpMsg[1];
                    else
                        proceParams = udpMsg[1].Substring(beginPos, endPos - beginPos + 1);
                    proceParams = proceParams.Replace("\r\n", "");
                    receiveList = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(proceParams);
                }
                else
                {
                    proceCommand = udpJob["udp_command"].ToString();
                    proceParams = udpJob["udp_params"].ToString();
                    //List<KeyValuePair<string, string>> receiveList = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(proceCommand);
                    receiveList = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(proceParams);

                }
                try
                {
                    //不能讓 UDP process 中斷, queue 裡面有東西, 會造成 再也無法傳送
                    switch (proceCommand)
                    {
                        case "plattingNote": //列印電鍍 便條紙
                            if (!CommonApp.printPlatingNote(receiveList, out rtnStr))
                                CommonClass.writeLog("TaskTray", "processUDP--printPlatingNote", 5, rtnStr);
                            break;
                        case "printlabel": //列印 RO Label
                                           //List<KeyValuePair<string, string>> roToPrint = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(proceCommand);
                            if (!CommonApp.printRO(receiveList, out rtnStr))
                                CommonClass.writeLog("TaskTray", "processUDP--printlabel", 5, rtnStr);
                            break;
                        case "printExcel":
                            if (!CommonApp.printExcel(receiveList, out rtnStr))
                                CommonClass.writeLog("TaskTray", "printExcel", 5, rtnStr);
                            break;
                        case "htmlToPdf":
                            string htmlFileName = proceCommand;
                            string destFile;
                            List<string> failList = new List<string>();
                            break;
                        case "capsLabel": //組帽標籤
                            if (!CommonApp.printCapsLabel(receiveList, out rtnStr))
                                CommonClass.writeLog("TaskTray", "printExcel", 5, rtnStr);
                            break;

                            //List<KeyValuePair<string, string>> htmlTOpdf = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(obj[1]);
                            ExcelLib exl = new ExcelLib();
                            foreach (KeyValuePair<string, string> destItem in receiveList)
                            {
                                destFile = string.IsNullOrEmpty(destItem.Value) ? destItem.Key.Replace(".html", ".pdf") : destItem.Value;
                                if (File.Exists(destFile))
                                    File.Delete(destFile);
                                if (!exl.saveHtmlToPdf(destItem.Key, out rtnStr, destFile, 1))
                                {
                                    failList.Add(rtnStr);
                                }
                            }
                            if (failList.Count >= 1)
                                CommonClass.writeLog("TaskTray", "processUDP--htmlToPdf", 5, string.Join(";", failList));
                            break;
                        case "roReport": //列印 RO 章入庫報表
                            if (!printROReport(receiveList, out rtnStr))
                                CommonClass.writeLog("TaskTray", "processUDP--roReport", 5, rtnStr);
                            else
                                CommonClass.writeLog("TaskTray", "processUDP--roReport", 1, "列印 RO 章入庫報表  完成");
                            break;
                        case "roFeedReport": //列印進料簽收單
                            if (!printROFeed(receiveList, out rtnStr))
                                CommonClass.writeLog("TaskTray", "processUDP--roFeedReport", 5, rtnStr);
                            else
                                CommonClass.writeLog("TaskTray", "processUDP--roFeedReport", 1, "列印 進料簽收單 完成");
                            break;
                        case "procurementReport": //採購申請單
                            if ((reTry = printProcurementReport(receiveList, out rtnStr)) > 0)
                                CommonClass.writeLog("TaskTray", "processUDP--procurementReport", 5, rtnStr);
                            else
                                CommonClass.writeLog("TaskTray", "processUDP--procurementReport", 1, "列印 RO 採購申請單  完成");
                            break;
                        case "dailyReport": //列印 工作日報
                            if ((reTry = printDailyReport(receiveList, out rtnStr)) > 0)
                            {
                                if (searchInKeyValList(receiveList, "retryCnt") != null) //代表需要重印
                                {
                                    int reportRetry = int.Parse(searchInKeyValList(receiveList, "retryCnt")); //reportRetry 使用者指定要重印的次數
                                    if (reportRetry > reTry && reportRetry != 0) //可以重印幾次
                                    {
                                        proceParams = proceParams.IndexOf("'reTry'") < 0 ? proceParams.Substring(0, proceParams.Length - 1) + ",{'Key':'reTry','Value':'" + reTry + "'}]" :
                                                      proceParams.Replace("{'Key':'reTry','Value':'" + (reTry - 1) + "'}", "{ 'Key':'reTry','Value':'" + reTry + "'}");
                                        //proceParams = proceParams.Replace("{'Key':'reTry','Value':'" + (reTry - 1) + "'}", "{ 'Key':'reTry','Value':'" + reTry + "'}");
                                        addToList(proceCommand, proceParams);
                                    }
                                    if (reportRetry > reTry && reportRetry == 0)
                                    {//可以重印幾次 
                                        CommonClass.writeLog("TaskTray", "dailyReport--dailyReport", 1, "列印 工作日報  完成, ['" + proceCommand + "','" + proceParams + "']");
                                    }
                                    else
                                        CommonClass.writeLog("TaskTray", "dailyReport--dailyReport", 5, "excelSQL 查無資料，請確認 property.txt 設定是否正確" +
                                            (string.IsNullOrEmpty(rtnStr) ? "" : rtnStr));

                                }
                            }
                            else
                                CommonClass.writeLog("TaskTray", "dailyReport", 1, "列印 工作日報  完成, ['" + proceCommand + "','" + proceParams + "']");
                            break;
                        case "stdLabelFromProperty": //這個是讀 Property 的標準報表
                             if (!CommonApp.printStdLabelFromProperty(stdLblDict, propertyName.Replace("\"", ""), out rtnStr))
                                    CommonClass.writeLog("TaskTray", "printExcel", 5, rtnStr);
                            break;
                        case "printStdLabelFromJson":
                            if (!CommonApp.printStdLabelFromJson(stdLblDict, propertyName.Replace("\"", ""), out rtnStr))
                                CommonClass.writeLog("TaskTray", "printExcel", 5, rtnStr);
                            break;
                        case "restartQueue":
                            removeFromList(proceCommand);
                            processUDPList(null, false);

                            break;
                        case "reboot": //N 分鐘後重新啟動電腦
                            int waitSecs = 1;
                            CommonClass.rebootComputer();
                            break;
                        case "restart": //N 分鐘後重新啟動火箭程式
                            CommonClass.restartApp();
                            break;
                    };
                    removeFromList(proceCommand, udpJob["ID"].ToString());
                }
                catch (Exception ex)
                {
                    string sqlStr = $"update mfo_dailyreportqueue set printFaileCnt=printFaileCnt+1 where ID={udpJob["ID"].ToString()}";
                    CommonClass.execSQLNonQuery(sqlStr);
                    CommonClass.writeLog("TaskTray", "processUDP", 5, proceCommand + "/$$/" + proceParams + "/$$/" + ex.Message, ex);
                    CommonApp.endPrintTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                }
                udpJob = getUdpJobList();
            } while (udpJob != null);
            CommonApp.endPrintTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }
        public static void new_process_udp()
        {
            int reTry = 0; //如果失敗， 預計重試（retry）次數
            int jobLength = 0; //過去 15 分鐘共有幾筆資料
            DataRow udpJob = null;
            Dictionary<string, string> newParams = new Dictionary<string, string>();
            string proceCommand = null;
            string rtnStr = null;
            udpJob = getUdpJobList(1);
            do
            {
                if (udpJob == null)
                    break;
                //proceCommand = udpJobList[obj[0]][0];
                CommonApp.startPrintTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//新列印開始
                CommonApp.endPrintTime = null; //新列印開始
                proceCommand = udpJob["udp_command"].ToString();
                string udpStaticData = udpJob["udp_params"].ToString().Replace(System.Environment.NewLine, "").Replace("~", "\"");
                Dictionary<string, string> udpDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(udpStaticData);
                Dictionary<string, string> propertyDict = null;
                if (udpDict.ContainsKey("propertyName"))
                {
                    string stdReportAtt = Constants.getProperty(udpDict["propertyName"]);
                    if (!string.IsNullOrEmpty(stdReportAtt))
                        propertyDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(stdReportAtt);
                }
                else
                    propertyDict = new Dictionary<string, string>();

                foreach (KeyValuePair<string, string> dictItem in udpDict)
                {
                    if (propertyDict.ContainsKey(dictItem.Key))
                    {
                        if (string.IsNullOrEmpty(dictItem.Value))
                            propertyDict[dictItem.Key] = "";
                        else if (dictItem.Value[0] == '{')
                            propertyDict[dictItem.Key] = dictItem.Value.Replace("^", "\"");
                        else
                            propertyDict[dictItem.Key] = dictItem.Value;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(dictItem.Value))
                            propertyDict.Add(dictItem.Key, "");
                        else if (dictItem.Value[0] == '{')
                            propertyDict.Add(dictItem.Key, dictItem.Value.Replace("^", "\""));
                        else
                            propertyDict.Add(dictItem.Key, dictItem.Value);
                    }
                }
                try
                {
                    //不能讓 UDP process 中斷, queue 裡面有東西, 會造成 再也無法傳送
                    switch (proceCommand)
                    {
                        case "stdLabel": //標準標籤， 從 Windows Form 直接讀取列印屬性與資料
                            if (!CommonApp.printStdLabel(propertyDict, out rtnStr))
                                CommonClass.writeLog("TaskTray", "printExcel", 5, rtnStr);
                            break;
                        case "stdReport": //標準報表，從 Windows Form 直接讀取列印屬性與資料
                            if (!CommonApp.printStdReport(propertyDict, out rtnStr))
                                CommonClass.writeLog("TaskTray", "printExcel", 5, rtnStr);
                            break;
                        case "excelToCsv":
                            if (!CommonApp.excelToCsv(propertyDict, out rtnStr)) 
                                CommonClass.writeLog("TaskTray", "excelToCsv", 5, rtnStr);
                            break;
                        default:
                            process_new_udpJson(propertyDict);
                            break;
                    }
                    removeFromList(proceCommand, udpJob["ID"].ToString());
                }
                catch (Exception ex)
                {
                    CommonClass.writeLog("TaskTray", "processUDPDict\\proceCommand", 5, ex.Message);
                    removeFromList(proceCommand, udpJob["ID"].ToString());
                }
                udpJob = getUdpJobList(1);
            } while (udpJob != null);
            CommonApp.endPrintTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        ////lblName:要列印的標籤，例如 '標準標籤_成品入庫'
        //private Dictionary<string, string> parseStandardReportProperty(string reportName, string sqlData = null)
        //{
        //    string stdReportAtt = Constants.getProperty(reportName);
        //    Dictionary<string, string> stdReportDict = null;
        //    try
        //    {
        //        stdReportDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(stdReportAtt);
        //        if (stdReportDict.Keys.Contains("excelFile"))
        //            txtReportTemplateFile.Text = stdReportDict["excelFile"].Replace("^", "\"");
        //        if (stdReportDict.Keys.Contains("printAttrs"))
        //            txtReportPrintAttrs.Text = stdReportDict["printAttrs"].Replace("^", "\"");
        //        if (stdReportDict.Keys.Contains("staticLayout"))
        //            txtStaticLayout.Text = stdReportDict["staticLayout"].Replace("^", "\"");
        //        if (stdReportDict.Keys.Contains("sqlData"))
        //        {
        //            if (txtReportData.Text == "" || chkboxReportDataFrom.Checked)
        //            {
        //                txtReportData.Text = stdReportDict["sqlData"].Replace("^", "\"");
        //            }
        //        }
        //        if (stdReportDict.Keys.Contains("staticData"))
        //        {
        //            if (txtStaticData.Text == "" || chkboxReportDataFrom.Checked)
        //            {
        //                txtStaticData.Text = stdReportDict["staticData"].Replace("^", "\"");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"{reportName} Json 內容錯誤", "Property Json 錯誤");
        //    }
        //    return stdReportDict;
        //}
        public static bool printExcel(List<KeyValuePair<string, string>> roToPrint, out string rtnStr)
        {
            ExcelLib exl = new ExcelLib();
            ExcelElements excelEE = null;
            String printer = null;
            rtnStr = null;
            try
            {
                String FileFullPath=null;
                DateTime reportDate = Convert.ToDateTime(roToPrint[1].Value);
                switch (roToPrint[0].Value)
                {
                    case "RoFeed":
                        FileFullPath=Constants.getProperty("ROFeedOutput") + @"\" + "進料簽收_" + reportDate.ToString("yyyyMMdd") + "000000_進料簽收單.xlsx";
                        printer = Constants.getProperty("ROLabelPrinter");
                        break;
                }
                if (string.IsNullOrEmpty(FileFullPath))
                {
                    rtnStr = "查無簽收單 檔案名稱";
                    return false;
                }

                if(!CommonClass.ifFileExists(FileFullPath))
                {
                    rtnStr = "查無簽收單檔案名稱：" + FileFullPath;
                    return false;
                }
                excelEE = exl.openExcel(FileFullPath);
                excelEE.mWSheet = exl.getWorksheet(excelEE,"報表");
                exl.printExcelBySpecifyPrinter(excelEE, printer);
                exl.closeExcel(excelEE, null, false);
                rtnStr = "列印完成!!";
                return true;
            }
            catch (Exception ex)
            {
                if (excelEE != null)
                    exl.closeExcel(excelEE, null, false);
                if (ex.HResult == -2146827284)
                    rtnStr = "系統找不到您指定的印表機！！！";
                else
                    rtnStr = "ex.Message！！！";
                return false;
            }
        }
        #region 列印 電鍍
        //列印電鍍便條紙
        public static bool printPlatingNote(List<KeyValuePair<string, string>> noteToPrint, out string rtnStr)
        {
            ExcelLib exl = new ExcelLib();
            ExcelElements excelEE = null;
            try
            {
                string sqlStr = "SELECT concat(concat(substring(a.`BATCHID`,7,2),'-',a.`PLATSerID`), IF(b.`packQuant` is null,'', Concat(', ', substring(a.`BATCHID`,7,2),'-',Round(b.`PLATSerID`)))) 編號, " +
                                " concat(c.SIZE,IF(b.`packQuant` is null,'', Concat(', ',d.SIZE))) '尺寸'," +
                                " Concat(Round(a.`packQuant`),IF(b.`packQuant` is null,'', Concat(', ',Round(b.`packQuant`)))) packQuant , " +
                                " DATE_FORMAT(CURRENT_DATE,'%m-%d') 送貨日期, DATE_FORMAT(a.`expectReceive`,'%m-%d') 回電日期 " +
                                " FROM `mfo_platingpack` a " +
                                " Left Join mfo_platingpack b on a.`combineTo`=concat(b.`BATCHID`,'-',b.`PLATSerID`) " +
                                " Left Join view_signinfo c on c.SIGNID=a.`SRCSIGNID` " +
                                " Left Join view_signinfo d on d.SIGNID=a.`SRCSIGNID` " +
                                //" where a.BATCHID='20210202' and (a.combineTo not like '*%' || a.combineTo is null) order by 編號";
                                " where a.status in (1,2,3) and (a.combineTo not like '*%' || a.combineTo is null) order by 編號";
                DataTable dt = CommonClass.getSQLDataTable(sqlStr);
                Dictionary<string, String> dictExcelData = null;
                List<Dictionary<string, string>> toExcelList = new List<Dictionary<string, string>>();

                string ColStr;
                int rowIdx = 0;
                foreach (DataRow myRow in dt.Rows) //產生 流程單
                {
                    if (rowIdx % 2 == 0)
                    {
                        dictExcelData = new Dictionary<string, String>();
                        dictExcelData.Add("B1", noteToPrint[0].Value);
                        ColStr = "C";
                    }
                    else
                    {
                        dictExcelData.Add("D1", noteToPrint[0].Value);
                        ColStr = "F";
                    }
                        
                    dictExcelData.Add(ColStr + "1", myRow["編號"].ToString());
                    dictExcelData.Add(ColStr + "2", myRow["尺寸"].ToString());
                    dictExcelData.Add(ColStr + "3", myRow["packQuant"].ToString());
                    dictExcelData.Add(ColStr + "4", myRow["送貨日期"].ToString());
                    dictExcelData.Add(ColStr + "5", myRow["回電日期"].ToString());
                    toExcelList.Add(dictExcelData);
                }
                String FileFullPath = Constants.getProperty("電鍍便條紙母版");
                String printer = Constants.getProperty("電鍍Printer");
                String ourputPath = Constants.getProperty("電鍍output");
                if (string.IsNullOrEmpty(ourputPath))
                    ourputPath = null;
                else
                    ourputPath += @"\電鍍便條紙_" + noteToPrint[0].Value + "_" + DateTime.Now.ToString() + ".xlsx";
                excelEE = exl.openExcel(FileFullPath);

                Microsoft.Office.Interop.Excel.Worksheet destWorkSheet = exl.openExcelSheet(excelEE, "報表");
                Microsoft.Office.Interop.Excel.Worksheet tempWorkSheet = exl.openExcelSheet(excelEE, "格式");
                Microsoft.Office.Interop.Excel.Worksheet cleanFormWorkSheet = exl.openExcelSheet(excelEE, "CleanForm");

                int pageBreak = 3; //pageBreak : 每 Copy/Paste 幾次, 加一個 跳頁符號
                int rowOffset = 5; //rowOffset 每個 Label 佔幾個 Row
                                   //rowRepeat : 每個 Row 印幾個 Label
                int colOffset = 6; //colOffset 每個 Label 佔幾個 Col
                int rowRepeat = 1; //rowRepeat: 每個 Row 印幾個 Label
                excelEE.oXL.Visible = true;
                exl.outputToExcel3(tempWorkSheet, destWorkSheet, cleanFormWorkSheet, toExcelList,
                        "A1:F5", pageBreak, false, true, 0, rowOffset, 0, CommonClass.getFileInfo(FileFullPath, "directory"), (float)1.5, -20);
                excelEE.mWSheet = destWorkSheet;
                exl.printExcelBySpecifyPrinter(excelEE, printer);
                exl.closeExcel(excelEE, ourputPath, false);
                rtnStr = "列印完成!!";
                return true;
            }
            catch (Exception ex)
            {
                if (excelEE != null)
                    exl.closeExcel(excelEE, null, false);
                if (ex.HResult == -2146827284)
                    rtnStr = "系統找不到您指定的印表機！！！";
                else
                    rtnStr = "ex.Message！！！";
                return false;
            }
        }

        //列印電鍍成品檢驗表
        public static bool printPlatingExameForm(DateTime reportDate, out string rtnStr, string reportType = "日報表")
        {
            ExcelLib excel = null;
            string templateFolder = Constants.getProperty("Ro章日報母板路徑", @"C:\sysTray\母版");
            string outputFolder = Constants.getProperty("ROFeedOutput", @"C:\sysTray");
            string templateFile = Constants.getProperty("RO報表", @"原物料進料檢驗日報表.xlsx");
            string outputFileName = outputFolder + @"\" + reportType + "_" + reportDate.ToString("yyyyMMddHHmmss") + "_" + templateFile;
            //string sqlStr = "SELECT `Supplier`,`size`,`CFMF`,`RO`,`SPECIAL`,Date(`buyDate`), Date(`varifyDate`), '' Pack, `quant`,`needVarify`," +
            //                " '' OK,'' NG, `roCode` FROM `ro_stamp` where Date(ro_date) = '" + reportDate.ToString("yyyy-MM-dd") + "'";
            string sqlStr = "SELECT `Supplier`,`size`,`CFMF`,`RO`,`SPECIAL`,date_format(varifyDate,'%m/%d') 到廠日期,date_format(varifyDate,'%m/%d'), " +
                "'1' Pack, `quant`,IF(`needVarify`=1,' ','V'),  IF(`needVarify`=1,'V',' ') OK,'' NG, Concat(if(Mid(roCode,1,1)='A','A',''), substring(roCode,3)) " +
                " FROM  `ro_stamp` where Date(buyDate) = '" + reportDate.ToString("yyyy-MM-dd") + "' order by roid";
            int currPage = 1;
            if (excel == null)
                excel = new ExcelLib();
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.WarehouseConnString);
            if (dt.Rows.Count == 0)
            {
                rtnStr = "當天沒有 RO 章入庫記錄";
                return false;
            }

            dt = CommonClass.insertEmptyRow(dt, "size");
            ExcelElements ee = excel.openExcel(templateFolder + @"\" + templateFile, true);
            //string formateJsonStr = "{ 'PageMode':'1','PageRows':'41','header':'1:41','pageDataRows':'20','dataRowStart':'5','columnEnd':'N',  'datePos': 'M3'}";
            string formateJsonStr = Constants.getProperty("ROFeed格式");
            Dictionary<string, string> HeaderAndFooter = JsonConvert.DeserializeObject<Dictionary<string, string>>(formateJsonStr);
            excel.fillExcelCellValue(ee.mWorkSheets["Template"], "M3", CommonClass.ToSimpleTaiwanDate(reportDate));
            currPage = excel.report_dtPageMode(dt, ee, HeaderAndFooter, currPage, currPage > 1 ? false : true);
            currPage = currPage + 1;
            ee.mWSheet = ee.mWorkSheets["報表"];
            string destPrinter = Constants.getProperty("ROReportPrinter", "Epson EPL-6200 (左下)");
            rtnStr = excel.printExcelBySpecifyPrinter(ee, destPrinter);
            //excel.printExcel(ee.mWorkSheets["報表"]);
            excel.closeExcel(ee, outputFileName, false);
            if (string.IsNullOrEmpty(rtnStr))
            {
                rtnStr = "RO 章入庫記錄列印完成";
                return true;
            }
            else
                return false;
        }

        #endregion

        #region 列印 RO 章
        public static bool printRO(List<KeyValuePair<string, string>> roToPrint, out string rtnStr)
        {
            Dictionary<String, String> dictExcelData;
            Dictionary<String, String> printAttrib = null;
            List<Dictionary<String, String>> toExcelList = new List<Dictionary<String, String>>();
            ExcelLib exl = new ExcelLib();
            ExcelElements excelEE = null;

            try
            {
                foreach (KeyValuePair<string, string> roItem in roToPrint)
                {
                    if(roItem.Key == "property")
                    {
                        string property = Constants.getProperty(roItem.Value, "RO補登");
                        printAttrib = JsonConvert.DeserializeObject<Dictionary<String, String>>(property);
                        continue;
                    }
                    string[] roSize = roItem.Key.Split('x');
                    dictExcelData = new Dictionary<String, String>();
                    dictExcelData.Add("B1", "QRCode:" + roItem.Key + "~" + roItem.Value);
                    dictExcelData.Add("C1", roSize[0]);
                    if (roSize.Length >= 2)
                    {
                        dictExcelData.Add("C2", roSize[1]);
                    }
                    if (roItem.Value.IndexOf("A") >= 0) //未帽
                        dictExcelData.Add("A3", "A" + CommonClass.Right(roItem.Value, 8));
                    else
                        dictExcelData.Add("A3", CommonClass.Right(roItem.Value, 8));
                    toExcelList.Add(dictExcelData);
                }
                String FileFullPath = null;
                String printer = null;
                if (printAttrib !=null && printAttrib.ContainsKey("ROLabelTemplate"))
                    FileFullPath = printAttrib["ROLabelTemplate"];
                else
                     FileFullPath = Constants.getProperty("ROLabelTemplate");
                if (printAttrib != null && printAttrib.ContainsKey("ROLabelPrinter"))
                    printer = printAttrib["ROLabelPrinter"];
                else
                    printer = Constants.getProperty("ROLabelPrinter");
                excelEE = exl.openExcel(FileFullPath);

                Microsoft.Office.Interop.Excel.Worksheet destWorkSheet = exl.openExcelSheet(excelEE, "RO章");
                Microsoft.Office.Interop.Excel.Worksheet tempWorkSheet = exl.openExcelSheet(excelEE, "RO章範本");
                Microsoft.Office.Interop.Excel.Worksheet cleanFormWorkSheet = exl.openExcelSheet(excelEE, "CleanForm");

                int pageBreak = 1; //pageBreak : 每 Copy/Paste 幾次, 加一個 跳頁符號
                int rowOffset = 3; //rowOffset 每個 Label 佔幾個 Row
                                   //rowRepeat : 每個 Row 印幾個 Label
                int colOffset = 2; //colOffset 每個 Label 佔幾個 Col
                int rowRepeat = 1; //rowRepeat: 每個 Row 印幾個 Label
                excelEE.oXL.Visible = true;
                exl.outputToExcel3(tempWorkSheet, destWorkSheet, cleanFormWorkSheet, toExcelList,
                        "A1:C3", pageBreak, false, true, 0, rowOffset, 0, CommonClass.getFileInfo(FileFullPath, "directory"), (float)1.5, -20);
                excelEE.mWSheet = destWorkSheet;
                exl.printExcelBySpecifyPrinter(excelEE, printer);
                exl.closeExcel(excelEE, null, false);
                rtnStr = "列印完成!!";
                return true;
            }
            catch (Exception ex)
            {
                if (excelEE != null)
                    exl.closeExcel(excelEE, null, false);
                if (ex.HResult == -2146827284)
                    rtnStr = "系統找不到您指定的印表機！！！";
                else
                    rtnStr = "ex.Message！！！";
                return false;
            }
        }

        //reportType 為 日報表 或 簽收單
        public static bool printROReport(DateTime reportDate, out string rtnStr, string reportType="日報表")
        {
            ExcelLib excel = null;
            string templateFolder = Constants.getProperty("Ro章日報母板路徑", @"C:\sysTray\母版");
            string outputFolder = Constants.getProperty("ROFeedOutput", @"C:\sysTray");
            string templateFile = Constants.getProperty("RO報表", @"原物料進料檢驗日報表.xlsx");
            string outputFileName = outputFolder + @"\" + reportType + "_" + reportDate.ToString("yyyyMMddHHmmss") + "_" + templateFile;
            //string sqlStr = "SELECT `Supplier`,`size`,`CFMF`,`RO`,`SPECIAL`,Date(`buyDate`), Date(`varifyDate`), '' Pack, `quant`,`needVarify`," +
            //                " '' OK,'' NG, `roCode` FROM `ro_stamp` where Date(ro_date) = '" + reportDate.ToString("yyyy-MM-dd") + "'";
            string sqlStr = $"SELECT a.`Supplier` , a.`size` , a.`CFMF` , a.`RO` , a.`SPECIAL` , " +
                $" DATE_FORMAT(a.`varifyDate`,'%m/%d') '到廠日期' , DATE_FORMAT(a.`varifyDate`,'%m/%d'), " +
                $" '1' AS 'Pack', b.`M_QTY` 'quant' , IF(a.`needVarify` = 1,' ','V'), IF(a.`needVarify`=1,'V',' ') 'OK' , " +
                $" '' AS 'NG', CONCAT(if(Mid(a.`roCode`,1,1)='A','A',''), SUBSTRING(a.`roCode`,3)) " +
                $" FROM `ro_stamp` a " +
                $" LEFT JOIN `events` b ON a.`size` = b.`SIZE` AND a.`roCode` = b.`ROCode` AND b.`EVENT_NAME` = '驗貨入庫' " +
                $" WHERE Date(a.`buyDate`) = '{reportDate.ToString("yyyy-MM-dd")}' " +
                $" ORDER BY a.`size`, a.`roCode1`, a.`roCode2` "; 
            int currPage = 1;
            if (excel == null)
                excel = new ExcelLib();
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.WarehouseConnString);
            if (dt.Rows.Count == 0)
            {
                rtnStr = "當天沒有 RO 章入庫記錄";
                return false;
            }

            dt = CommonClass.insertEmptyRow(dt, "size");
            ExcelElements ee = excel.openExcel(templateFolder + @"\" + templateFile, true);
            //string formateJsonStr = "{ 'PageMode':'1','PageRows':'41','header':'1:41','pageDataRows':'20','dataRowStart':'5','columnEnd':'N',  'datePos': 'M3'}";
            string formateJsonStr = Constants.getProperty("ROFeed格式");
            Dictionary<string, string> HeaderAndFooter = JsonConvert.DeserializeObject<Dictionary<string, string>>(formateJsonStr);
            excel.fillExcelCellValue(ee.mWorkSheets["Template"], "M3", CommonClass.ToSimpleTaiwanDate(reportDate));
            currPage = excel.report_dtPageMode(dt, ee, HeaderAndFooter, currPage, currPage > 1 ? false : true);
            currPage = currPage + 1;
            ee.mWSheet = ee.mWorkSheets["報表"];
            string destPrinter = Constants.getProperty("ROReportPrinter", "Epson EPL-6200 (左下)");
            rtnStr = excel.printExcelBySpecifyPrinter(ee, destPrinter);
            //excel.printExcel(ee.mWorkSheets["報表"]);
            excel.closeExcel(ee, outputFileName, false);
            if (string.IsNullOrEmpty(rtnStr))
            {
                rtnStr = "RO 章入庫記錄列印完成";
                return true;
            }
            else
                return false;
        }

        //列印 RO 每日入庫報表
        public static bool printROReport(List<KeyValuePair<string, string>> roToPrint, out string rtnStr)
        {
            rtnStr = null;
            DateTime reportDate = Convert.ToDateTime(roToPrint[0].Value);
            return printROReport(reportDate, out rtnStr);
        }


        //列印 RO 進料簽收單
        public static bool printROFeed(DateTime reportDate, out string rtnStr, string reportType= "進料簽收", bool rePrint=false)
        {
            ExcelLib excel = null;
            string templateFolder = Constants.getProperty("Ro章日報母板路徑", @"C:\sysTray\母版");
            string outputFolder = Constants.getProperty("ROFeedOutput", @"C:\sysTray");
            string templateFile = Constants.getProperty("ROFeed報表", @"原物料進料檢驗日報表.xlsx");
            string outputFileName = outputFolder + @"\" + reportType + "_" + reportDate.ToString("yyyyMMddHHmmss") + "_" + templateFile;
            if(CommonClass.ifFileExists(outputFileName)) {
                outputFileName = CommonClass.addSeqToFileName(outputFileName);
            }
            //string sqlStr = "SELECT `Supplier`,`size`,`CFMF`,`RO`,`SPECIAL`,Date(`buyDate`), Date(`varifyDate`), '' Pack, `quant`,`needVarify`," +
            //                " '' OK,'' NG, `roCode` FROM `ro_stamp` where Date(ro_date) = '" + reportDate.ToString("yyyy-MM-dd") + "'";
            string sqlStr = "SELECT `Supplier`,`size`,`CAP`,CFMF 品名, `RO`, `SPECIAL`,'1' Pack, `quant`," +
                " Concat(IF(`CAP`='已帽','','A'),substring(roCode, 3)) " +
                " FROM `ro_stamp` where buyDate != '0000-00-00' and Date(ro_date) = '" + reportDate.ToString("yyyy-MM-dd") + "' ";
            if(!rePrint)
                sqlStr += " And sign = 0 ";
            sqlStr += " ORDER BY roid";
            int currPage = 1;
            if (excel == null)
                excel = new ExcelLib();
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.WarehouseConnString);
            if (dt.Rows.Count == 0)
            {
                rtnStr = "當天沒有 RO 章入庫記錄";
                return false;
            }

            dt = CommonClass.insertEmptyRow(dt, "size");
            ExcelElements ee = excel.openExcel(templateFolder + @"\" + templateFile, true);
            string formateJsonStr = Constants.getProperty("ROFeed格式", "{ 'PageMode':'1','PageRows':'41','header':'1:41','pageDataRows':'20','dataRowStart':'5','columnEnd':'N',  'datePos': 'M3'}");
            Dictionary<string, string> HeaderAndFooter = JsonConvert.DeserializeObject<Dictionary<string, string>>(formateJsonStr);
            excel.fillExcelCellValue(ee.mWorkSheets["Template"], "I3", CommonClass.ToSimpleTaiwanDate(reportDate));
            currPage = excel.report_dtPageMode(dt, ee, HeaderAndFooter, currPage, currPage > 1 ? false : true);
            currPage = currPage + 1;
            ee.mWSheet = ee.mWorkSheets["報表"];
            string destPrinter = Constants.getProperty("ROFeedReportPrinter", "Epson EPL-6200 (左下)");
            excel.printExcelBySpecifyPrinter(ee, destPrinter);
            //excel.printExcel(ee.mWorkSheets["報表"]);

            excel.closeExcel(ee, outputFileName, false);
            sqlStr = "update ro_stamp set sign = 1 where buyDate != '0000-00-00' and Date(ro_date) = '" + reportDate.ToString("yyyy-MM-dd") + "' " +
                     " And sign = 0";
            CommonClass.execSQLNonQuery(sqlStr, Constants.WarehouseConnString);
            rtnStr = "RO 章入庫記錄列印完成";

            return true;
        }

        //列印 RO 進料簽收單
        public static bool printROFeed(List<KeyValuePair<string, string>> roToPrint, out string rtnStr)
        {
            rtnStr = null;
            DateTime reportDate = Convert.ToDateTime(roToPrint[0].Value);
            bool rePrint = false;
            foreach(KeyValuePair<string, string> keyValue in roToPrint)
            {
                if(keyValue.Key == "rePrint")
                {
                    if (keyValue.Value == "1")
                        rePrint = true;
                }
            }
            return printROFeed(reportDate, out rtnStr,rePrint:rePrint);

        }
        #endregion

        #region 列印 組帽標籤
        public static bool printCapsLabel(List<KeyValuePair<string, string>> receiveList, out string rtnStr)
        {
            string referanceStandard = CommonApp.searchInKeyValList(receiveList, "referanceStandard");
            string baseNum = CommonApp.searchInKeyValList(receiveList, "baseNum");
            string labelcnt = CommonApp.searchInKeyValList(receiveList, "labelcnt");
            string pno = CommonApp.searchInKeyValList(receiveList, "pno");
            string mfr = CommonApp.searchInKeyValList(receiveList, "mfr");
            string lno = CommonApp.searchInKeyValList(receiveList, "lno");
            //廠商來的資料如有特殊符號 '/'
            if (lno.Contains("||"))
                lno = lno.Replace("||", "/");
            string maxInt = CommonApp.searchInKeyValList(receiveList, "maxInt");
            string minInt = CommonApp.searchInKeyValList(receiveList, "minInt");
            string bno = CommonApp.searchInKeyValList(receiveList, "bno");
            string date = CommonApp.searchInKeyValList(receiveList, "date");
            List<double> rtnList = new List<double>();
            List<double> E24List = new List<double>()
                                {
                                    0.01,0.0105,0.011,0.0121,0.0133,0.0147,0.0162,0.0178,0.0196,0.0215,0.0237,0.0261,
                                    0.0287,0.0316,0.0348,0.0383,0.0422,0.0464,0.0511,0.0562,0.0619,0.0681,0.075,
                                    0.0825,0.0909,0.1,0.11,0.121,0.133,0.147,0.162,0.178,0.196,0.215,0.237,0.261,
                                    0.287,0.316,0.348,0.383,0.422,0.464,0.511,0.562,0.619,0.681,0.75,0.825,0.909,
                                    1,1.1,1.21,1.33,1.47,1.62,1.78,1.96,2.15,2.37,2.61,2.87,3.16,3.48,3.83,4.22,4.64,
                                    5.11,5.62,6.19,6.81,7.5,8.25,9.09,10,11,12.1,13.3,14.7,16.2,17.8,19.6,21.5,23.7,
                                    26.1,28.7,31.6,34.8,38.3,42.2,46.4,51.1,56.2,61.9,68.1,75,82.5,90.9,100,110,121,
                                    133,147,162,178,196,215,237,261,287,316,348,383,422,464,511,562,619,681,750,825,
                                    909,1000,1100,1210,1330,1470,1620,1780,1960,2150,2370,2610,2870,3160,3480,3830,
                                    4220,4640,5110,5620,6190,6810,7500,8250,9090,10000,11000,12100,13300,14700,16200,
                                    17800,19600,21500,23700,26100,28700,31600,34800,38300,42200,46400,51100,56200,
                                    61900,68100,75000,82500,90900,100000,110000,121000,133000,147000,162000,178000,
                                    196000,215000,237000,261000,287000,316000,348000,383000,422000,464000,511000,
                                    562000,619000,681000,750000,825000,909000,1000000,1100000,1210000,1330000,1470000,1620000,1780000,1960000,2150000,2370000,2610000,2870000,3160000
                                };
            List<double> E48List = new List<double>()
                                {
                                    0.01,0.0105,0.011,0.0115,0.0121,0.0127,0.0133,0.014,0.0147,0.0154,0.0162,
                                    0.0169,0.0178,0.0187,0.0196,0.0205,0.0215,0.0226,0.0237,0.0249,0.0261,0.0274,
                                    0.0287,0.0301,0.0316,0.0332,0.0348,0.0365,0.0383,0.0402,0.0422,0.0442,0.0464,
                                    0.0487,0.0511,0.0536,0.0562,0.059,0.0619,0.0649,0.0681,0.0715,0.075,0.0787,
                                    0.0825,0.0866,0.0909,0.0953,0.1,0.105,0.11,0.115,0.121,0.127,0.133,0.14,
                                    0.147,0.154,0.162,0.169,0.178,0.187,0.196,0.205,0.215,0.226,0.237,0.249,
                                    0.261,0.274,0.287,0.301,0.316,0.332,0.348,0.365,0.383,0.402,0.422,0.442,
                                    0.464,0.487,0.511,0.536,0.562,0.59,0.619,0.649,0.681,0.715,0.75,0.787,0.825,
                                    0.866,0.909,0.953,1,1.05,1.1,1.15,1.21,1.27,1.33,1.4,1.47,1.54,1.62,1.69,
                                    1.78,1.87,1.96,2.05,2.15,2.26,2.37,2.49,2.61,2.74,2.87,3.01,3.16,3.32,3.48,
                                    3.65,3.83,4.02,4.22,4.42,4.64,4.87,5.11,5.36,5.62,5.9,6.19,6.49,6.81,7.15,
                                    7.5,7.87,8.25,8.66,9.09,9.53,10,10.5,11,11.5,12.1,12.7,13.3,14,14.7,15.4,
                                    16.2,16.9,17.8,18.7,19.6,20.5,21.5,22.6,23.7,24.9,26.1,27.4,28.7,30.1,31.6,
                                    33.2,34.8,36.5,38.3,40.2,42.2,44.2,46.4,48.7,51.1,53.6,56.2,59,61.9,64.9,
                                    68.1,71.5,75,78.7,82.5,86.6,90.9,95.3,100,105,110,115,121,127,133,140,147,
                                    154,162,169,178,187,196,205,215,226,237,249,261,274,287,301,316,332,348,365,
                                    383,402,422,442,464,487,511,536,562,590,619,649,681,715,750,787,825,866,909,
                                    953,1000,1050,1100,1150,1210,1270,1330,1400,1470,1540,1620,1690,1780,1870,
                                    1960,2050,2150,2260,2370,2490,2610,2740,2870,3010,3160,3320,3480,3650,3830,
                                    4020,4220,4420,4640,4870,5110,5360,5620,5900,6190,6490,6810,7150,7500,7870,
                                    8250,8660,9090,9530,10000,10500,11000,11500,12100,12700,13300,14000,14700,
                                    15400,16200,16900,17800,18700,19600,20500,21500,22600,23700,24900,26100,
                                    27400,28700,30100,31600,33200,34800,36500,38300,40200,42200,44200,46400,
                                    48700,51100,53600,56200,59000,61900,64900,68100,71500,75000,78700,82500,
                                    86600,90900,95300,100000,105000,110000,115000,121000,127000,133000,140000,
                                    147000,154000,162000,169000,178000,187000,196000,205000,215000,226000,237000,
                                    249000,261000,274000,287000,301000,316000,332000,348000,365000,383000,402000,
                                    422000,442000,464000,487000,511000,536000,562000,590000,619000,649000,681000,
                                    715000,750000,787000,825000,866000,909000,953000,1000000,
                                    1050000,1100000,1150000,1210000,1270000,1330000,1400000,1470000,1540000,1620000,1690000,1780000,1870000,1960000,2050000,2150000,2260000,2370000,2490000,2610000,2740000,2870000,3010000
                                };
            List<double> E96List = new List<double>()
                                {
                                    0.01,0.0102,0.0105,0.0107,0.011,0.0113,0.0115,0.0118,0.0121,0.0124,0.0127,0.013,0.0133,
                                    0.0137,0.014,0.0143,0.0147,0.015,0.0154,0.0158,0.0162,0.0165,0.0169,0.0174,0.0178,0.0182,
                                    0.0187,0.0191,0.0196,0.02,0.0205,0.0215,0.0221,0.0226,0.0232,0.0237,0.0243,0.0249,0.0255,
                                    0.0261,0.0267,0.0274,0.028,0.0287,0.0294,0.0301,0.0309,0.0316,0.0324,0.0332,0.034,0.0348,
                                    0.0357,0.0365,0.0374,0.0383,0.0392,0.0402,0.0412,0.0422,0.0432,0.0442,0.0453,0.0464,0.0475,
                                    0.0487,0.0499,0.0511,0.0523,0.0536,0.0459,0.0562,0.0576,0.059,0.0604,0.0619,0.0634,0.0649,
                                    0.0655,0.0681,0.0698,0.0715,0.0732,0.075,0.0768,0.0787,0.0806,0.0825,0.0845,0.0866,0.0887,
                                    0.0909,0.0931,0.0953,0.0976,0.1,0.102,0.105,0.107,0.11,0.113,0.115,0.118,0.121,0.124,0.127,
                                    0.13,0.133,0.137,0.14,0.143,0.147,0.15,0.154,0.158,0.162,0.165,0.169,0.174,0.178,0.182,
                                    0.187,0.191,0.196,0.2,0.205,0.215,0.221,0.226,0.232,0.237,0.243,0.249,0.255,0.261,0.267,
                                    0.274,0.28,0.287,0.294,0.301,0.309,0.316,0.324,0.332,0.34,0.348,0.357,0.365,0.374,0.383,
                                    0.392,0.402,0.412,0.422,0.432,0.442,0.453,0.464,0.475,0.487,0.499,0.511,0.523,0.536,0.459,
                                    0.562,0.576,0.59,0.604,0.619,0.634,0.649,0.655,0.681,0.698,0.715,0.732,0.75,0.768,0.787,
                                    0.806,0.825,0.845,0.866,0.887,0.909,0.931,0.953,0.976,1,1.02,1.05,1.07,1.1,1.13,1.15,
                                    1.18,1.21,1.24,1.27,1.3,1.33,1.37,1.4,1.43,1.47,1.5,1.54,1.58,1.62,1.65,1.69,1.74,1.78,
                                    1.82,1.87,1.91,1.96,2,2.05,2.15,2.21,2.26,2.32,2.37,2.43,2.49,2.55,2.61,2.67,2.74,2.8,
                                    2.87,2.94,3.01,3.09,3.16,3.24,3.32,3.4,3.48,3.57,3.65,3.74,3.83,3.92,4.02,4.12,4.22,4.32,
                                    4.42,4.53,4.64,4.75,4.87,4.99,5.11,5.23,5.36,4.59,5.62,5.76,5.9,6.04,6.19,6.34,6.49,6.55,
                                    6.81,6.98,7.15,7.32,7.5,7.68,7.87,8.06,8.25,8.45,8.66,8.87,9.09,9.31,9.53,9.76,10,10.2,
                                    10.5,10.7,11,11.3,11.5,11.8,12.1,12.4,12.7,13,13.3,13.7,14,14.3,14.7,15,15.4,15.8,16.2,
                                    16.5,16.9,17.4,17.8,18.2,18.7,19.1,19.6,20,20.5,21.5,22.1,22.6,23.2,23.7,24.3,24.9,25.5,
                                    26.1,26.7,27.4,28,28.7,29.4,30.1,30.9,31.6,32.4,33.2,34,34.8,35.7,36.5,37.4,38.3,39.2,
                                    40.2,41.2,42.2,43.2,44.2,45.3,46.4,47.5,48.7,49.9,51.1,52.3,53.6,45.9,56.2,57.6,59,60.4,
                                    61.9,63.4,64.9,65.5,68.1,69.8,71.5,73.2,75,76.8,78.7,80.6,82.5,84.5,86.6,88.7,90.9,93.1,
                                    95.3,97.6,100,102,105,107,110,113,115,118,121,124,127,130,133,137,140,143,147,150,154,
                                    158,162,165,169,174,178,182,187,191,196,200,205,215,221,226,232,237,243,249,255,261,267,
                                    274,280,287,294,301,309,316,324,332,340,348,357,365,374,383,392,402,412,422,432,442,453,
                                    464,475,487,499,511,523,536,459,562,576,590,604,619,634,649,655,681,698,715,732,750,768,
                                    787,806,825,845,866,887,909,931,953,976,1000,1020,1050,1070,1100,1130,1150,1180,1210,
                                    1240,1270,1300,1330,1370,1400,1430,1470,1500,1540,1580,1620,1650,1690,1740,1780,1820,
                                    1870,1910,1960,2000,2050,2150,2210,2260,2320,2370,2430,2490,2550,2610,2670,2740,2800,
                                    2870,2940,3010,3090,3160,3240,3320,3400,3480,3570,3650,3740,3830,3920,4020,4120,4220,4320,
                                    4420,4530,4640,4750,4870,4990,5110,5230,5360,4590,5620,5760,5900,6040,6190,6340,6490,6550,
                                    6810,6980,7150,7320,7500,7680,7870,8060,8250,8450,8660,8870,9090,9310,9530,9760,10000,
                                    10200,10500,10700,11000,11300,11500,11800,12100,12400,12700,13000,13300,13700,14000,
                                    14300,14700,15000,15400,15800,16200,16500,16900,17400,17800,18200,18700,19100,19600,20000,
                                    20500,21500,22100,22600,23200,23700,24300,24900,25500,26100,26700,27400,28000,28700,29400,
                                    30100,30900,31600,32400,33200,34000,34800,35700,36500,37400,38300,39200,40200,41200,42200,
                                    43200,44200,45300,46400,47500,48700,49900,51100,52300,53600,45900,56200,57600,59000,60400,
                                    61900,63400,64900,65500,68100,69800,71500,73200,75000,76800,78700,80600,82500,84500,86600,
                                    88700,90900,93100,95300,97600,100000,102000,105000,107000,110000,113000,115000,118000,
                                    121000,124000,127000,130000,133000,137000,140000,143000,147000,150000,154000,158000,
                                    162000,165000,169000,174000,178000,182000,187000,191000,196000,200000,205000,215000,
                                    221000,226000,232000,237000,243000,249000,255000,261000,267000,274000,280000,287000,
                                    294000,301000,309000,316000,324000,332000,340000,348000,357000,365000,374000,383000,
                                    392000,402000,412000,422000,432000,442000,453000,464000,475000,487000,499000,511000,
                                    523000,536000,459000,562000,576000,590000,604000,619000,634000,649000,655000,681000,
                                    698000,715000,732000,750000,768000,787000,806000,825000,845000,866000,887000,909000,
                                    931000,953000,976000,1000000
                                };
            List<double> E192List = new List<double>()
                                {
                                    0.01,0.0102,0.0104,0.0105,0.0106,0.0107,0.0109,0.011,0.0113,0.0114,0.0115,0.0117,0.0118,0.012,0.0121,0.0123,0.0124,0.0126,0.0127,0.0129,0.013,0.0132,0.0133,0.0135,0.0137,0.0138,0.014,0.0142,0.0143,0.0145,0.0147,0.0149,0.015,0.0152,0.0154,0.0156,0.0158,0.016,0.0162,0.0164,0.0165,0.0167,0.0169,0.0172,0.0174,0.0176,0.0178,0.018,0.0182,0.0184,0.0187,0.0189,0.0191,0.0193,0.0196,0.0198,0.02,0.0203,0.0205,0.0208,0.021,0.0213,0.0215,0.0218,0.0221,0.0223,0.0226,0.0229,0.0232,0.0234,0.0237,0.024,0.0243,0.0246,0.0249,0.0252,0.0255,0.0258,0.0261,0.0264,0.0267,0.0271,0.0274,0.0277,0.028,0.0284,0.0287,0.0291,0.0294,0.0298,0.0301,0.0305,0.0309,0.0312,0.0316,0.032,0.0324,0.0328,0.0332,0.0336,0.034,0.0344,0.0348,0.0352,0.0357,0.0361,0.0365,0.037,0.0374,0.0379,0.0383,0.0388,0.0392,0.0397,0.0402,0.0407,0.0412,0.0417,0.0422,0.0427,0.0432,0.0437,0.0442,0.0448,0.0453,0.0459,0.0464,0.047,0.0475,0.0481,0.0487,0.0493,0.0499,0.0505,0.0511,0.0517,0.0523,0.053,0.0536,0.0542,0.0549,0.0556,0.0562,0.0569,0.0576,0.0583,0.059,0.0597,0.0604,0.0612,0.0619,0.0626,0.0634,0.0642,0.0649,0.0657,0.0665,0.0673,0.0681,0.069,0.0698,0.0706,0.0715,0.0723,0.0741,0.075,0.0759,0.0768,0.0777,0.0787,0.0796,0.0806,0.0816,0.0825,0.0835,0.0845,0.0856,0.0866,0.0876,0.0887,0.0898,0.0909,0.092,0.0931,0.0942,0.0953,0.0965,0.0976,0.0988,0.1,0.102,0.104,0.105,0.106,0.107,0.109,0.11,0.113,0.114,0.115,0.117,0.118,0.12,0.121,0.123,0.124,0.126,0.127,0.129,0.13,0.132,0.133,0.135,0.137,0.138,0.14,0.142,0.143,0.145,0.147,0.149,0.15,0.152,0.154,0.156,0.158,0.16,0.162,0.164,0.165,0.167,0.169,0.172,0.174,0.176,0.178,0.18,0.182,0.184,0.187,0.189,0.191,0.193,0.196,0.198,0.2,0.203,0.205,0.208,0.21,0.213,0.215,0.218,0.221,0.223,0.226,0.229,0.232,0.234,0.237,0.24,0.243,0.246,0.249,0.252,0.255,0.258,0.261,0.264,0.267,0.271,0.274,0.277,0.28,0.284,0.287,0.291,0.294,0.298,0.301,0.305,0.309,0.312,0.316,0.32,0.324,0.328,0.332,0.336,0.34,0.344,0.348,0.352,0.357,0.361,0.365,0.37,0.374,0.379,0.383,0.388,0.392,0.397,0.402,0.407,0.412,0.417,0.422,0.427,0.432,0.437,0.442,0.448,0.453,0.459,0.464,0.47,0.475,0.481,0.487,0.493,0.499,0.505,0.511,0.517,0.523,0.53,0.536,0.542,0.549,0.556,0.562,0.569,0.576,0.583,0.59,0.597,0.604,0.612,0.619,0.626,0.634,0.642,0.649,0.657,0.665,0.673,0.681,0.69,0.698,0.706,0.715,0.723,0.741,0.75,0.759,0.768,0.777,0.787,0.796,0.806,0.816,0.825,0.835,0.845,0.856,0.866,0.876,0.887,0.898,0.909,0.92,0.931,0.942,0.953,0.965,0.976,0.988,1,1.02,1.04,1.05,1.06,1.07,1.09,1.1,1.13,1.14,1.15,1.17,1.18,1.2,1.21,1.23,1.24,1.26,1.27,1.29,1.3,1.32,1.33,1.35,1.37,1.38,1.4,1.42,1.43,1.45,1.47,1.49,1.5,1.52,1.54,1.56,1.58,1.6,1.62,1.64,1.65,1.67,1.69,1.72,1.74,1.76,1.78,1.8,1.82,1.84,1.87,1.89,1.91,1.93,1.96,1.98,2,2.03,2.05,2.08,2.1,2.13,2.15,2.18,2.21,2.23,2.26,2.29,2.32,2.34,2.37,2.4,2.43,2.46,2.49,2.52,2.55,2.58,2.61,2.64,2.67,2.71,2.74,2.77,2.8,2.84,2.87,2.91,2.94,2.98,3.01,3.05,3.09,3.12,3.16,3.2,3.24,3.28,3.32,3.36,3.4,3.44,3.48,3.52,3.57,3.61,3.65,3.7,3.74,3.79,3.83,3.88,3.92,3.97,4.02,4.07,4.12,4.17,4.22,4.27,4.32,4.37,4.42,4.48,4.53,4.59,4.64,4.7,4.75,4.81,4.87,4.93,4.99,5.05,5.11,5.17,5.23,5.3,5.36,5.42,5.49,5.56,5.62,5.69,5.76,5.83,5.9,5.97,6.04,6.12,6.19,6.26,6.34,6.42,6.49,6.57,6.65,6.73,6.81,6.9,6.98,7.06,7.15,7.23,7.41,7.5,7.59,7.68,7.77,7.87,7.96,8.06,8.16,8.25,8.35,8.45,8.56,8.66,8.76,8.87,8.98,9.09,9.2,9.31,9.42,9.53,9.65,9.76,9.88,10,10.2,10.4,10.5,10.6,10.7,10.9,11,11.3,11.4,11.5,11.7,11.8,12,12.1,12.3,12.4,12.6,12.7,12.9,13,13.2,13.3,13.5,13.7,13.8,14,14.2,14.3,14.5,14.7,14.9,15,15.2,15.4,15.6,15.8,16,16.2,16.4,16.5,16.7,16.9,17.2,17.4,17.6,17.8,18,18.2,18.4,18.7,18.9,19.1,19.3,19.6,19.8,20,20.3,20.5,20.8,21,21.3,21.5,21.8,22.1,22.3,22.6,22.9,23.2,23.4,23.7,24,24.3,24.6,24.9,25.2,25.5,25.8,26.1,26.4,26.7,27.1,27.4,27.7,28,28.4,28.7,29.1,29.4,29.8,30.1,30.5,30.9,31.2,31.6,32,32.4,32.8,33.2,33.6,34,34.4,34.8,35.2,35.7,36.1,36.5,37,37.4,37.9,38.3,38.8,39.2,39.7,40.2,40.7,41.2,41.7,42.2,42.7,43.2,43.7,44.2,44.8,45.3,45.9,46.4,47,47.5,48.1,48.7,49.3,49.9,50.5,51.1,51.7,52.3,53,53.6,54.2,54.9,55.6,56.2,56.9,57.6,58.3,59,59.7,60.4,61.2,61.9,62.6,63.4,64.2,64.9,65.7,66.5,67.3,68.1,69,69.8,70.6,71.5,72.3,74.1,75,75.9,76.8,77.7,78.7,79.6,80.6,81.6,82.5,83.5,84.5,85.6,86.6,87.6,88.7,89.8,90.9,92,93.1,94.2,95.3,96.5,97.6,98.8,100,102,104,105,106,107,109,110,113,114,115,117,118,120,121,123,124,126,127,129,130,132,133,135,137,138,140,142,143,145,147,149,150,152,154,156,158,160,162,164,165,167,169,172,174,176,178,180,182,184,187,189,191,193,196,198,200,203,205,208,210,213,215,218,221,223,226,229,232,234,237,240,243,246,249,252,255,258,261,264,267,271,274,277,280,284,287,291,294,298,301,305,309,312,316,320,324,328,332,336,340,344,348,352,357,361,365,370,374,379,383,388,392,397,402,407,412,417,422,427,432,437,442,448,453,459,464,470,475,481,487,493,499,505,511,517,523,530,536,542,549,556,562,569,576,583,590,597,604,612,619,626,634,642,649,657,665,673,681,690,698,706,715,723,741,750,759,768,777,787,796,806,816,825,835,845,856,866,876,887,898,909,920,931,942,953,965,976,988,1000,1020,1040,1050,1060,1070,1090,1100,1130,1140,1150,1170,1180,1200,1210,1230,1240,1260,1270,1290,1300,1320,1330,1350,1370,1380,1400,1420,1430,1450,1470,1490,1500,1520,1540,1560,1580,1600,1620,1640,1650,1670,1690,1720,1740,1760,1780,1800,1820,1840,1870,1890,1910,1930,1960,1980,2000,2030,2050,2080,2100,2130,2150,2180,2210,2230,2260,2290,2320,2340,2370,2400,2430,2460,2490,2520,2550,2580,2610,2640,2670,2710,2740,2770,2800,2840,2870,2910,2940,2980,3010,3050,3090,3120,3160,3200,3240,3280,3320,3360,3400,3440,3480,3520,3570,3610,3650,3700,3740,3790,3830,3880,3920,3970,4020,4070,4120,4170,4220,4270,4320,4370,4420,4480,4530,4590,4640,4700,4750,4810,4870,4930,4990,5050,5110,5170,5230,5300,5360,5420,5490,5560,5620,5690,5760,5830,5900,5970,6040,6120,6190,6260,6340,6420,6490,6570,6650,6730,6810,6900,6980,7060,7150,7230,7410,7500,7590,7680,7770,7870,7960,8060,8160,8250,8350,8450,8560,8660,8760,8870,8980,9090,9200,9310,9420,9530,9650,9760,9880,10000,10200,10400,10500,10600,10700,10900,11000,11300,11400,11500,11700,11800,12000,12100,12300,12400,12600,12700,12900,13000,13200,13300,13500,13700,13800,14000,14200,14300,14500,14700,14900,15000,15200,15400,15600,15800,16000,16200,16400,16500,16700,16900,17200,17400,17600,17800,18000,18200,18400,18700,18900,19100,19300,19600,19800,20000,20300,20500,20800,21000,21300,21500,21800,22100,22300,22600,22900,23200,23400,23700,24000,24300,24600,24900,25200,25500,25800,26100,26400,26700,27100,27400,27700,28000,28400,28700,29100,29400,29800,30100,30500,30900,31200,31600,32000,32400,32800,33200,33600,34000,34400,34800,35200,35700,36100,36500,37000,37400,37900,38300,38800,39200,39700,40200,40700,41200,41700,42200,42700,43200,43700,44200,44800,45300,45900,46400,47000,47500,48100,48700,49300,49900,50500,51100,51700,52300,53000,53600,54200,54900,55600,56200,56900,57600,58300,59000,59700,60400,61200,61900,62600,63400,64200,64900,65700,66500,67300,68100,69000,69800,70600,71500,72300,74100,75000,75900,76800,77700,78700,79600,80600,81600,82500,83500,84500,85600,86600,87600,88700,89800,90900,92000,93100,94200,95300,96500,97600,98800,100000,102000,104000,105000,106000,107000,109000,110000,113000,114000,115000,117000,118000,120000,121000,123000,124000,126000,127000,129000,130000,132000,133000,135000,137000,138000,140000,142000,143000,145000,147000,149000,150000,152000,154000,156000,158000,160000,162000,164000,165000,167000,169000,172000,174000,176000,178000,180000,182000,184000,187000,189000,191000,193000,196000,198000,200000,203000,205000,208000,210000,213000,215000,218000,221000,223000,226000,229000,232000,234000,237000,240000,243000,246000,249000,252000,255000,258000,261000,264000,267000,271000,274000,277000,280000,284000,287000,291000,294000,298000,301000,305000,309000,312000,316000,320000,324000,328000,332000,336000,340000,344000,348000,352000,357000,361000,365000,370000,374000,379000,383000,388000,392000,397000,402000,407000,412000,417000,422000,427000,432000,437000,442000,448000,453000,459000,464000,470000,475000,481000,487000,493000,499000,505000,511000,517000,523000,530000,536000,542000,549000,556000,562000,569000,576000,583000,590000,597000,604000,612000,619000,626000,634000,642000,649000,657000,665000,673000,681000,690000,698000,706000,715000,723000,741000,750000,759000,768000,777000,787000,796000,806000,816000,825000,835000,845000,856000,866000,876000,887000,898000,909000,920000,931000,942000,953000,965000,976000,988000,1000000,
                                    102000,1040000,1050000,1060000,1070000,1090000,1100000,1130000,1140000,1150000,1170000,1180000,1200000,1210000,1230000,1240000,1260000,1270000,1290000,1300000,1320000,1330000,1350000,1370000,1380000,1400000,1420000,1430000,1450000,14700001490000,1500000,1520000,1540000,1560000,1580000,1600000,1620000,1640000,1650000,1670000,1690000,1720000,1740000,1760000,1780000,1800000,1820000,1840000,1870000,1890000,1910000,1930000,1960000,1980000,2000000,2030000,2050000,2080000,2100000,2130000,2150000,2180000,2210000,2230000,2260000,2290000,2320000,2340000,2370000,2400000,2430000,2460000,2490000,2520000,2550000,2580000,2610000,2640000,2670000,2710000,2740000,2770000,2800000,2840000,2870000,2910000,2940000,2980000,3010000
                                };
            double inputDbl;
            int pickCnt;
            List<double> MyList;
            if (!double.TryParse(baseNum, out inputDbl) && inputDbl< 0)
            {
                rtnStr = "baseNum 必須大於 0";
                return false;
            }

            if (!int.TryParse(labelcnt, out pickCnt) && pickCnt < 0)
            {
                rtnStr = "labelcnt 必須大於 0";
                return false;
            }

            if (referanceStandard == "E24")
            {
                bno = "E24   10%";
                MyList = E24List;
            }
            else if (referanceStandard == "E48")
            {
                bno = "E48   5%";
                MyList = E48List;
            }
            else if (referanceStandard == "E96")
            {
                bno = "E96   1%";
                MyList = E96List;
            }
            else 
            {
                bno = "E192   1%";
                MyList = E192List;
            }

            int i;
            for (i = 0; i < MyList.Count - 2; i++)
            {
                if (inputDbl >= MyList[i] && inputDbl < MyList[i + 1])
                    break;
            }
            rtnList.Add(inputDbl);
            for (int j = i + 1; j <= i + pickCnt && i <= MyList.Count; j++)
            {
                rtnList.Add(MyList[j]);
            }

            Dictionary<String, String> dictExcelData;
            List<Dictionary<String, String>> toExcelList = new List<Dictionary<String, String>>();
            List<string> finalList = new List<string>();

            for (i = 0; i < rtnList.Count; i++)
            {
                if(rtnList[i]>=1000000)
                    finalList.Add((rtnList[i] / 1000000).ToString("0.######") + "M");
                else if (rtnList[i] >= 1000)
                    finalList.Add((rtnList[i] / 1000).ToString("0.###") + "K");
                else
                    finalList.Add((rtnList[i] ).ToString("0.##"));

            }

            string lowestRng = inputDbl.ToString();
            string highestRng = finalList[pickCnt - 1];

            for (i = 0; i < pickCnt ; i++)
            {
                dictExcelData = new Dictionary<String, String>();
                dictExcelData.Add("B1", $"{Convert.ToInt32(pno) + i}");
                dictExcelData.Add("B2", mfr);
                dictExcelData.Add("B3", lno);
                dictExcelData.Add("B4", minInt);
                dictExcelData.Add("B5", maxInt);
                dictExcelData.Add("B6", bno);
                dictExcelData.Add("B7", finalList[i]);
                dictExcelData.Add("B8", finalList[i + 1]);
                dictExcelData.Add("B9", "'" + date);
                toExcelList.Add(dictExcelData);
            }

            ExcelLib exl = new ExcelLib();
            ExcelElements excelEE = null;
            try
            {
                //string udpBodyJson = "[\"capsLabel\",\"" + JsonConvert.SerializeObject(toExcelList).Replace("\"", "'") + "\"]";
                //UDPLib.udpSend(udpBodyJson, UDPLib.TasktrayIP, UDPLib.TasktrayPort);
                String FileFullPath = Constants.getProperty("CapsLabelTemplate", @"C:\sysTray\母版\組帽分類標籤母版.xlsx");
                String printer = Constants.getProperty("CapsLabelPrinter", "Microsoft Print to PDF");
                excelEE = exl.openExcel(FileFullPath);

                Microsoft.Office.Interop.Excel.Worksheet destWorkSheet = exl.openExcelSheet(excelEE, "標籤");
                Microsoft.Office.Interop.Excel.Worksheet tempWorkSheet = exl.openExcelSheet(excelEE, "template");
                Microsoft.Office.Interop.Excel.Worksheet cleanFormWorkSheet = exl.openExcelSheet(excelEE, "CleanForm");

                int pageBreak = 1; //pageBreak : 每 Copy/Paste 幾次, 加一個 跳頁符號
                int rowOffset = 9; //rowOffset 每個 Label 佔幾個 Row
                int colOffset = 1; //colOffset 每個 Label 佔幾個 Col
                int rowRepeat = 1; //rowRepeat: 每個 Row 印幾個 Label
                excelEE.oXL.Visible = true;
                exl.outputToExcel3(tempWorkSheet, destWorkSheet, cleanFormWorkSheet, toExcelList,
                        "A1:C10", pageBreak, false, true, 0, rowOffset, 0, CommonClass.getFileInfo(FileFullPath, "directory"), (float)1.5, -20);
                excelEE.mWSheet = destWorkSheet;
                exl.printExcelBySpecifyPrinter(excelEE, printer);
                //exl.closeExcel(excelEE, @"D:\庫存管理系統\test.xlsx", false);
                exl.closeExcel(excelEE, null, false);

                //rtnStr = "列印完成!!";
                //MessageBox.Show("列印組帽標籤表到火箭程式完成");
                rtnStr = "列印完成!!";
                return true;
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2146827284)
                {
                    rtnStr = "系統找不到您指定的印表機！！！";
                }
                else
                {
                    rtnStr = "ex.Message！！！";
                }
                return false;
            }
        }

        public static string valToStr(string valStr)
        {
            string rtnStr;
            double valDbl = 0;
            if (CommonClass.IsNumeric(valStr))
                valDbl = double.Parse(valStr);
            else
                return "0";

            if (valDbl >= 1000)
                rtnStr = Math.Round(valDbl / 1000, 1).ToString() + "K";
            else if (valDbl >= 1000000)
                rtnStr = Math.Round(valDbl / 1000000, 1).ToString() + "M";
            else
                rtnStr = Math.Round(valDbl, 1).ToString();
            return rtnStr;
        }
        #endregion

        #region 標準列印
        #region 列印標準標籤
        public static List<Dictionary<string, string>>  genToexcelListData(Dictionary<string, string> excelLayout, List<Dictionary<string, string>> labelData)
        {
            List<Dictionary<string, string>> toExcelList = new List<Dictionary<string, string>>();
            foreach(Dictionary<string, string> labelDataDict in labelData)
            {
                Dictionary<string, string> outDict = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> cellData in labelDataDict)
                {
                    if(excelLayout.Keys.Contains(cellData.Key))
                    {
                        outDict.Add(excelLayout[cellData.Key], cellData.Value);
                    }
                }
                if (outDict.Count > 0)
                    toExcelList.Add(outDict);
            }
            return toExcelList;
        }

        public static string genStdLabelCommandFromProperty(string propertyName,
            string labelData = "", string staticData = "")
        {
            Dictionary<string, string> stdPrnDict = new Dictionary<string, string>()
            {
                {"func","stdLabel"},
                {"propertyName", $"{propertyName}"} //property Name
            };
            if (!string.IsNullOrEmpty(labelData))
                stdPrnDict.Add("labelData", $"{labelData}");
            //User事項準備好的資料
            if (!string.IsNullOrEmpty(staticData))
                stdPrnDict.Add("staticData", $"{ staticData}");
            return JsonConvert.SerializeObject(stdPrnDict);
        }

        public static string genStdLabelCommand(string excelFile, 
            string printAttrs, string excelLayout, string labelData = "", string staticData="")
        {
            Dictionary<string, string> stdPrnDict = new Dictionary<string, string>()
            {
                {"func","stdLabel"},
                {"excelFile", $"{excelFile}"}, //母板
                {"printAttrs", $"{printAttrs}"}, //列印屬性
                {"excelLayout", $"{excelLayout}" } //excel 的儲存格 位置要放的內容
            };
            //SQL：可含 Connection 及 多個 SQL 指令
            if (!string.IsNullOrEmpty(labelData))
                stdPrnDict.Add("labelData", $"{labelData}");
            //User事項準備好的資料
            if (!string.IsNullOrEmpty(staticData))
                stdPrnDict.Add("staticData", $"{ staticData}");
            return JsonConvert.SerializeObject(stdPrnDict);
        }

        public static string genStdReportFromProperty(string propertyName,
        string sqlData = "", string staticData = "")
        {
            Dictionary<string, string> stdPrnDict = new Dictionary<string, string>()
            {
                {"func","stdReportFromPropert"},
                {"propertyName", $"{propertyName}"} //property Name
            };
            //SQL：可含 Connection 及 多個 SQL 指令
            if (!string.IsNullOrEmpty(sqlData))
                stdPrnDict.Add("sqlData", $"{ sqlData}");
            //User事項準備好的資料
            if (!string.IsNullOrEmpty(staticData))
                stdPrnDict.Add("staticData", $"{ staticData}");
            return JsonConvert.SerializeObject(stdPrnDict);
        }

        public static string genExcelToJsonProperty(string property, string excelFile,
            string DataCol, string skipDataDict, string workSheet="工作表1")
        {
            Dictionary<string, string> stdPrnDict = new Dictionary<string, string>()
            {
                {"func","excelToJson"},
                {"property",property},
                {"excelFile", excelFile}, //母板
                {"DataCol", DataCol}, //Excel 的資料欄位 Header
                {"skipDataDict", skipDataDict }, //excel 讀進來時從左到右的欄位, 若有空白欄， 即代表不取該欄位的  {"workSheet", $"{skipDataDict}" } //excel 讀進來時從左到右的欄位, 若有空白欄， 即代表不取該欄位的值
                {"workSheet", workSheet }
            };
            return JsonConvert.SerializeObject(stdPrnDict);
        }


        public static string genStdReportCommand(string excelFile,
            string printAttrs, string staticLayout, string sqlData = "", string staticData = "")
        {
            Dictionary<string, string> stdPrnDict = new Dictionary<string, string>()
            {
                {"func","stdReport"},
                {"excelFile", $"{excelFile}"}, //母板
                {"printAttrs", $"{printAttrs}"}, //列印屬性
                {"staticLayout", $"{staticLayout}" } //excel 的儲存格 位置要放的內容
            };
            //SQL：可含 Connection 及 多個 SQL 指令
            if (!string.IsNullOrEmpty(sqlData))
                stdPrnDict.Add("sqlData", $"{ sqlData}");
            //User事項準備好的資料
            if (!string.IsNullOrEmpty(staticData))
                stdPrnDict.Add("staticData", $"{ staticData}");
            return JsonConvert.SerializeObject(stdPrnDict);
        }

        public static Dictionary<string, string> parseStandardLableProperty(string lblName)
        {
            string stdLabelAtt = Constants.getProperty(lblName);
            Dictionary<string, string> stdLblDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(stdLabelAtt);
            return stdLblDict;
        }
        public static bool printStdLabel(Dictionary<string, string> receiveDict, out string rtnStr)
        {
            List<Dictionary<string, string>> labelData = null;
            Dictionary<string, string> printAttrs = null;
            Dictionary<string, string> excelLayout = null;
            string excelFilePath = null;
            if (receiveDict.ContainsKey("excelFile"))
                excelFilePath = receiveDict["excelFile"]; //母版位置
            if (receiveDict.ContainsKey("printAttrs"))
                printAttrs = JsonConvert.DeserializeObject<Dictionary<string, string>>(receiveDict["printAttrs"]);
            if (receiveDict.ContainsKey("excelLayout"))
                excelLayout  = JsonConvert.DeserializeObject<Dictionary<string, string>>(receiveDict["excelLayout"]);
            if(receiveDict.ContainsKey("labelData"))
                labelData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(receiveDict["labelData"]);
            Dictionary<string, string> staticData = null;
            if(receiveDict.ContainsKey("staticData"))
                staticData = JsonConvert.DeserializeObject<Dictionary<string, string>>(receiveDict["staticData"].Replace("^", "'"));
            ExcelLib ex1 = new ExcelLib();
            ExcelElements excelEE = ex1.openExcel(excelFilePath);
            
            List<Dictionary<string, string>> toExcelList = genToexcelListData(excelLayout, labelData);
            int pageBreak = printAttrs.Keys.Contains("pageBreak") ? int.Parse(printAttrs["pageBreak"]) : 3; //pageBreak : 每 Copy/Paste 幾次, 加一個 跳頁符號
            int rowOffset = printAttrs.Keys.Contains("rowOffset") ? int.Parse(printAttrs["rowOffset"]) : 5; //rowOffset 每個 Label 佔幾個 Row
                                                                                                            //rowRepeat : 每個 Row 印幾個 Label
            int colOffset = printAttrs.Keys.Contains("colOffset") ? int.Parse(printAttrs["colOffset"]) : 6; //colOffset 每個 Label 佔幾個 Col
            int rowRepeat = printAttrs.Keys.Contains("rowRepeat") ? int.Parse(printAttrs["rowRepeat"]) : 1; //rowRepeat: 每個 Row 印幾個 Label
            string tempRange = printAttrs.Keys.Contains("tempRange") ? printAttrs["tempRange"] : "A1:C3";
            bool ifPrint = printAttrs.Keys.Contains("ifPrint")?bool.Parse(printAttrs["ifPrint"]) : false;
            bool ifClose = printAttrs.Keys.Contains("ifClose")?bool.Parse(printAttrs["ifClose"]) : false; ;
            bool excelShow = printAttrs.Keys.Contains("excelShow")?bool.Parse(printAttrs["excelShow"]) : true; ;
            float qrSize = printAttrs.Keys.Contains("qrSize")?float.Parse(printAttrs["qrSize"]) : (float)1.5;  //qrCode 的Size
            int qrTopMargin = printAttrs.Keys.Contains("qrTopMargin")?int.Parse(printAttrs["qrTopMargin"]) : -20; //qrCode 的 Margin
            int qrLeftMargin = printAttrs.Keys.Contains("qrLeftMargin") ? int.Parse(printAttrs["qrLeftMargin"]) : -20; //qrCode 的 Margin 
            float newqrSize = printAttrs.Keys.Contains("newqrSize") ? float.Parse(printAttrs["newqrSize"]) : (float)35; 
            excelEE.oXL.Visible = excelShow;
            Microsoft.Office.Interop.Excel.Worksheet destWorkSheet = ex1.openExcelSheet(excelEE, "報表");
            Microsoft.Office.Interop.Excel.Worksheet tempWorkSheet = ex1.openExcelSheet(excelEE, "格式");
            Microsoft.Office.Interop.Excel.Worksheet cleanFormWorkSheet = ex1.openExcelSheet(excelEE, "CleanForm");
            excelEE.mWSheet = destWorkSheet;
            //有指定印表機
            if (printAttrs.ContainsKey("printer") && !string.IsNullOrEmpty(printAttrs["printer"]))
                ex1.outputToExcel3(tempWorkSheet, destWorkSheet, cleanFormWorkSheet, toExcelList,
                tempRange, pageBreak, ifPrint, ifClose, colOffset, rowOffset, rowRepeat,
                CommonClass.getFileInfo(excelFilePath, "directory"), qrSize, qrTopMargin,
                printAttrs["printer"], excelEE,newqrSize,qrLeftMargin);
             else 
                ex1.outputToExcel3(tempWorkSheet, destWorkSheet, cleanFormWorkSheet, toExcelList,
                tempRange, pageBreak, ifPrint, ifClose, colOffset, rowOffset, rowRepeat, 
                CommonClass.getFileInfo(excelFilePath, "directory"), qrSize, qrTopMargin);
            if(ifClose)
                ex1.closeExcel(excelEE, null, false);
            
            rtnStr = "";
            return true;
        }
        //列印的屬性以火箭的 Property 為主， 送來的Json 為輔
        public static bool printStdLabelFromProperty(Dictionary<string, string> receiveDict, string propertyName, out string rtnStr)
        {
            Dictionary<string, string> stdLblDict = parseStandardLableProperty(propertyName);
            if(stdLblDict.ContainsKey("excelFile"))
            {
                if (receiveDict.ContainsKey("excelFile"))
                    receiveDict["excelFile"] = stdLblDict["excelFile"];
                else
                {
                    receiveDict.Add("excelFile", stdLblDict["excelFile"]);
                }
            }
            if (stdLblDict.ContainsKey("printAttrs"))
            {
                if (receiveDict.ContainsKey("printAttrs"))
                    receiveDict["printAttrs"] = stdLblDict["printAttrs"];
                else
                {
                    receiveDict.Add("printAttrs", stdLblDict["printAttrs"]);
                }
            }
            if (stdLblDict.ContainsKey("excelLayout"))
            {
                if (receiveDict.ContainsKey("excelLayout"))
                    receiveDict["excelLayout"] = stdLblDict["excelLayout"];
                else
                {
                    receiveDict.Add("excelLayout", stdLblDict["excelLayout"]);
                }
            }
            return printStdLabel(receiveDict, out rtnStr);

            //List<Dictionary<string, string>> labelData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(receiveDict["labelData"]);
            //ExcelLib ex1 = new ExcelLib();
            //ExcelElements excelEE = ex1.openExcel(excelFilePath);

            //List<Dictionary<string, string>> toExcelList = genToexcelListData(excelLayout, labelData);
            //int pageBreak = printAttrs.Keys.Contains("pageBreak") ? int.Parse(printAttrs["pageBreak"]) : 3; //pageBreak : 每 Copy/Paste 幾次, 加一個 跳頁符號
            //int rowOffset = printAttrs.Keys.Contains("rowOffset") ? int.Parse(printAttrs["rowOffset"]) : 5; //rowOffset 每個 Label 佔幾個 Row
            //                                                                                                //rowRepeat : 每個 Row 印幾個 Label
            //int colOffset = printAttrs.Keys.Contains("colOffset") ? int.Parse(printAttrs["colOffset"]) : 6; //colOffset 每個 Label 佔幾個 Col
            //int rowRepeat = printAttrs.Keys.Contains("rowRepeat") ? int.Parse(printAttrs["rowRepeat"]) : 1; //rowRepeat: 每個 Row 印幾個 Label
            //string tempRange = printAttrs.Keys.Contains("tempRange") ? printAttrs["tempRange"] : "A1:C3";
            //bool ifPrint = printAttrs.Keys.Contains("ifPrint") ? bool.Parse(printAttrs["ifPrint"]) : false;
            //bool ifClose = printAttrs.Keys.Contains("ifClose") ? bool.Parse(printAttrs["ifClose"]) : false; ;
            //float qrSize = printAttrs.Keys.Contains("qrSize") ? float.Parse(printAttrs["qrSize"]) : (float)1.5;  //qrCode 的Size
            //int qrTopMargin = printAttrs.Keys.Contains("qrTopMargin") ? int.Parse(printAttrs["qrTopMargin"]) : -20; //qrCode 的 Margin
            //bool excelShow = printAttrs.Keys.Contains("excelShow") ? bool.Parse(printAttrs["excelShow"]) : false; ;
            //Microsoft.Office.Interop.Excel.Worksheet destWorkSheet = ex1.openExcelSheet(excelEE, "報表");
            //Microsoft.Office.Interop.Excel.Worksheet tempWorkSheet = ex1.openExcelSheet(excelEE, "格式");
            //Microsoft.Office.Interop.Excel.Worksheet cleanFormWorkSheet = ex1.openExcelSheet(excelEE, "CleanForm");
            //excelEE.oXL.Visible = excelShow;
            ///*
            // * public int outputToExcel3(Microsoft.Office.Interop.Excel.Worksheet templateWS, Microsoft.Office.Interop.Excel.Worksheet targetWS,
            //   Microsoft.Office.Interop.Excel.Worksheet cleanFormWS,
            //   List<Dictionary<string, String>> toExcelList, string tempRange, int pageBreak = 1,
            //   bool ifPrint = false, bool ifClose = true, int colOffset = 0, int rowOffset = 0, int rowRepeat = 1, string excelPath = null,
            //   float qrSize = 1, int qrTopMargin = 5)
            // */

            //ex1.outputToExcel3(tempWorkSheet, destWorkSheet, cleanFormWorkSheet, toExcelList,
            //tempRange, pageBreak, ifPrint, ifClose, colOffset, rowOffset, rowRepeat, CommonClass.getFileInfo(excelFilePath, "directory"), qrSize, qrTopMargin);

            ////ex1.outputToExcel4(tempWorkSheet, destWorkSheet, toExcelList,pageBreak, ifPrint, ifClose, CommonClass.getFileInfo(excelFilePath, "directory"), qrSize, qrTopMargin);
            ////ex1.copyPasteRange(tempWorkSheet, destWorkSheet, printAttrs["tempRange"], "A1", false);
            ////ex1.copyPasteSpecial(cleanFormWorkSheet, destWorkSheet, null, null, Microsoft.Office.Interop.Excel.XlPasteType.xlPasteFormats);
            //rtnStr = "";
            //if (ifClose)
            //    ex1.closeExcel(excelEE, null, false);
            //return true;
        }

        //列印的屬性以火箭的 Property 為主， 送來的Json 為輔
        public static bool printStdLabelFromJson(Dictionary<string, string> receiveDict, string propertyName, out string rtnStr)
        {
            Dictionary<string, string> stdLblDict = parseStandardLableProperty(propertyName);
            if (stdLblDict.ContainsKey("excelFile"))
            {
                if (!receiveDict.ContainsKey("excelFile"))
                {
                    receiveDict.Add("excelFile", stdLblDict["excelFile"]);
                }
            }
            if (stdLblDict.ContainsKey("printAttrs"))
            {
                if (!receiveDict.ContainsKey("printAttrs"))
                {
                    receiveDict.Add("printAttrs", stdLblDict["printAttrs"]);
                }
            }
            if (stdLblDict.ContainsKey("excelLayout"))
            {
                if (!receiveDict.ContainsKey("excelLayout"))
                {
                    receiveDict.Add("excelLayout", stdLblDict["excelLayout"]);
                }
            }

            if (stdLblDict.ContainsKey("labelData"))
            {
                if (!receiveDict.ContainsKey("labelData"))
                {
                    receiveDict.Add("labelData", stdLblDict["labelData"]);
                }
            }
            return printStdLabel(receiveDict, out rtnStr);

            //List<Dictionary<string, string>> labelData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(receiveDict["labelData"]);
            //ExcelLib ex1 = new ExcelLib();
            //ExcelElements excelEE = ex1.openExcel(excelFilePath);

            //List<Dictionary<string, string>> toExcelList = genToexcelListData(excelLayout, labelData);
            //int pageBreak = printAttrs.Keys.Contains("pageBreak") ? int.Parse(printAttrs["pageBreak"]) : 3; //pageBreak : 每 Copy/Paste 幾次, 加一個 跳頁符號
            //int rowOffset = printAttrs.Keys.Contains("rowOffset") ? int.Parse(printAttrs["rowOffset"]) : 5; //rowOffset 每個 Label 佔幾個 Row
            //                                                                                                //rowRepeat : 每個 Row 印幾個 Label
            //int colOffset = printAttrs.Keys.Contains("colOffset") ? int.Parse(printAttrs["colOffset"]) : 6; //colOffset 每個 Label 佔幾個 Col
            //int rowRepeat = printAttrs.Keys.Contains("rowRepeat") ? int.Parse(printAttrs["rowRepeat"]) : 1; //rowRepeat: 每個 Row 印幾個 Label
            //string tempRange = printAttrs.Keys.Contains("tempRange") ? printAttrs["tempRange"] : "A1:C3";
            //bool ifPrint = printAttrs.Keys.Contains("ifPrint") ? bool.Parse(printAttrs["ifPrint"]) : false;
            //bool ifClose = printAttrs.Keys.Contains("ifClose") ? bool.Parse(printAttrs["ifClose"]) : false; ;
            //float qrSize = printAttrs.Keys.Contains("qrSize") ? float.Parse(printAttrs["qrSize"]) : (float)1.5;  //qrCode 的Size
            //int qrTopMargin = printAttrs.Keys.Contains("qrTopMargin") ? int.Parse(printAttrs["qrTopMargin"]) : -20; //qrCode 的 Margin
            //bool excelShow = printAttrs.Keys.Contains("excelShow") ? bool.Parse(printAttrs["excelShow"]) : false; ;
            //Microsoft.Office.Interop.Excel.Worksheet destWorkSheet = ex1.openExcelSheet(excelEE, "報表");
            //Microsoft.Office.Interop.Excel.Worksheet tempWorkSheet = ex1.openExcelSheet(excelEE, "格式");
            //Microsoft.Office.Interop.Excel.Worksheet cleanFormWorkSheet = ex1.openExcelSheet(excelEE, "CleanForm");
            //excelEE.oXL.Visible = excelShow;
            ///*
            // * public int outputToExcel3(Microsoft.Office.Interop.Excel.Worksheet templateWS, Microsoft.Office.Interop.Excel.Worksheet targetWS,
            //   Microsoft.Office.Interop.Excel.Worksheet cleanFormWS,
            //   List<Dictionary<string, String>> toExcelList, string tempRange, int pageBreak = 1,
            //   bool ifPrint = false, bool ifClose = true, int colOffset = 0, int rowOffset = 0, int rowRepeat = 1, string excelPath = null,
            //   float qrSize = 1, int qrTopMargin = 5)
            // */

            //ex1.outputToExcel3(tempWorkSheet, destWorkSheet, cleanFormWorkSheet, toExcelList,
            //tempRange, pageBreak, ifPrint, ifClose, colOffset, rowOffset, rowRepeat, CommonClass.getFileInfo(excelFilePath, "directory"), qrSize, qrTopMargin);

            ////ex1.outputToExcel4(tempWorkSheet, destWorkSheet, toExcelList,pageBreak, ifPrint, ifClose, CommonClass.getFileInfo(excelFilePath, "directory"), qrSize, qrTopMargin);
            ////ex1.copyPasteRange(tempWorkSheet, destWorkSheet, printAttrs["tempRange"], "A1", false);
            ////ex1.copyPasteSpecial(cleanFormWorkSheet, destWorkSheet, null, null, Microsoft.Office.Interop.Excel.XlPasteType.xlPasteFormats);
            //rtnStr = "";
            //if (ifClose)
            //    ex1.closeExcel(excelEE, null, false);
            //return true;
        }
        #endregion

        #region 列印標準報表
        public static string genStdReportByProperty(string propName, string sqlData, string staticData)
        {
            Dictionary<string, string> stdPrnDict = new Dictionary<string, string>()
            {
                {"func","stdReport"},
                {"propertyName", $"{propName}"} //Property 名稱
            };
            //SQL：可含 Connection 及 多個 SQL 指令
            if (!string.IsNullOrEmpty(sqlData))
                stdPrnDict.Add("sqlData", $"{ sqlData}");
            //User事項準備好的資料
            if (!string.IsNullOrEmpty(staticData))
                stdPrnDict.Add("staticData", $"{ staticData}");
            return JsonConvert.SerializeObject(stdPrnDict);
        }

        public static Dictionary<string, string> parseStandardReportProperty(string lblName)
        {
            string stdLabelAtt = Constants.getProperty(lblName);
            Dictionary<string, string> stdLblDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(stdLabelAtt);
            return stdLblDict;
        }

        //標準報表. 標準標籤 SQL 指令轉換
        public static string translateSQLData(string sqlStr, Dictionary<string, string> staticData)
        {
            sqlStr = sqlStr.Replace("#Date", DateTime.Now.ToString("yyyy-MM-dd"));
            string sqlTemp = sqlStr;
            string replaceStr;
            int endPos = 0;
            for(int i=0; i >=0;)
            {
                i = sqlTemp.IndexOf('$');
                if (i < 0)
                    break;
                endPos = sqlTemp.IndexOf(' ', i);
                if (endPos > 0)
                    replaceStr = sqlTemp.Substring(i + 1, (endPos - i - 2));
                else
                    replaceStr = sqlTemp.Substring(i + 1);
                replaceStr = replaceStr.Replace("'", "");
                if(staticData.ContainsKey(replaceStr))
                    sqlStr = sqlStr.Replace($"${replaceStr}", staticData[replaceStr]);
                sqlTemp = sqlTemp.Replace($"${replaceStr}", " ");
            }
            return sqlStr;
        }

        //標準報表. 標準標籤 靜態資料轉換
        public static Dictionary<string, string> translateStaticDict(Dictionary<string, string>staticData, 
            Dictionary<string, string> excelLayout, Dictionary<string, string> srcDict = null , DataTable dt=null, int dtIdx=0)
        {
            string tempStr=null;
            Dictionary<string, string> rtnDict = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> layoutItem in excelLayout)
            {
                tempStr = layoutItem.Value.Replace("$", "");
                if (srcDict != null && srcDict.Keys.Contains(tempStr))
                    rtnDict.Add(layoutItem.Key, srcDict[tempStr]);
                else if(dt!=null && dt.Columns.Contains(tempStr))
                    rtnDict.Add(layoutItem.Key, dt.Rows[dtIdx][tempStr].ToString());
                //先看是否 靜態 Diction 有沒有此資料
                if (staticData != null && staticData.Keys.Contains(tempStr) && !rtnDict.ContainsKey(layoutItem.Key))
                    rtnDict.Add(layoutItem.Key, staticData[tempStr]);
            }
            return rtnDict;
        }
        //excelToJson
        public static Dictionary<string, string> parsePropertyToDict(string propertyName)
        {
            try
            {
                string stdLabelAtt = Constants.getProperty(propertyName);
                Dictionary<string, string> stdLblDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(stdLabelAtt);
                return stdLblDict;
            }
            catch (Exception ex)
            {
                return null;

            }
        }

        public static bool process_new_udpJson(Dictionary<string, string> receiveDict)
        {
            if(!receiveDict.ContainsKey("udp_func") || string.IsNullOrEmpty(receiveDict["udp_func"]))
                return false;
            string sqlStr = null;
            switch(receiveDict["udp_func"])
            {
                case "defectZuMou":
                    return do_defectZuMou(receiveDict);
                    break;
                case "zumaoDailyReport":
                    return do_zumaoDailyReport(receiveDict);
                    break;
                case "zumaoRoStempReport":
                    return do_zumaoRoStempReport(receiveDict);
                    break;
                case "NJ_收據釘貼單toPdf": //收據釘貼單toPdf
                    return do_printReceiptToPdf(receiveDict);
                    break;
                case "do_DtailtoPdf": //NJ_費用申請單toPdf
                    return do_DtailtoPdf(receiveDict);
                    break;
                case "do_askForPayment":
                    do_askForPayment(receiveDict);
                    break;
                case "assessment":
                    do_assessment(receiveDict);
                    break;

            }
            return true;
        }

       
        public static bool excelToCsv(Dictionary<string, string> receiveDict, out string rtnStr)
        {

            string excelFile = receiveDict["excelFile"];
            ExcelLib ex = new ExcelLib();
            string delimiterStr = receiveDict.ContainsKey("Delimeter")?receiveDict["Delimeter"]:",";
            if (receiveDict.ContainsKey("workSheet"))
            {
                ex.SaveExcelToCSVWithDelimiter(excelFile, receiveDict["workSheet"], delimiterStr);
            } 
            else
            {
                ex.excelToCsv(excelFile);
            }
            
            rtnStr = "轉換成功";
            return true;
        }

        //組帽 RO章 統計報表
        public static bool do_zumaoRoStempReport(Dictionary<string, string> receiveDict)
        {
            List<string> testSatndard = new List<string>();
            ///測試用 SQL
            DateTime startDate = DateTime.Parse(receiveDict["reportDate"] + "/01");
            DateTime endDate = CommonClass.lastDayOfMonth(startDate);
            string flowStep = receiveDict["flowStep"];
            string sqlStr = $"SELECT b.SIZE, b.SourceRoStemp, sum(a.`AccQuan`) 完成量 , sum(b.`BATCH_QTY`)*1000 發料量 , " +
                $" Round(sum(a.`AccQuan`)/(sum(b.`BATCH_QTY`)*1000),2) 良率 " +
                $" FROM `mostrecentcapsign` a " +
                $" inner join cap_subflow b on a.`SUBFLOWID` =b.SUBFLOWID " +
                $" where a.`FLOW_STEP`='{flowStep}' and Date(a.`Finish_Time`) >= '{startDate.ToString("yyyy-MM-dd")}' " +
                $" and Date(a.`Finish_Time`) <= '{endDate.ToString("yyyy-MM-dd")}' " +
                $" group by `SIZE`, `SourceRoStemp` order by `SIZE`, `SourceRoStemp`";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            if (dt.Rows.Count == 0)
            {
                return true;
            }
            string templateFile = receiveDict["excelFile"].Replace("/", "\\"); //母版位置

            //string reportName = CommonClass.getFileInfo(templateFile, "filenamewithoutextension");
            //printAttrs.Add("ReportName", reportName);
            Dictionary<string, string> excelLayout = JsonConvert.DeserializeObject<Dictionary<string, string>>(receiveDict["staticLayout"].Replace("^", "'"));
            Dictionary<string, string> staticData = null;
            string outputFolder = receiveDict["OutputPath"];
            string tempValue = null;
            string tempKey = null;
            int currPage = 1;
            ExcelLib excel = new ExcelLib();
            ExcelElements ee = excel.openExcel(templateFile, true);
            string currSize = null;
            int currCol = 2;
            double sumIssue = 0;
            double sumDoneQuant = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if(dr["SIZE"].ToString() != currSize)
                {
                    if(sumIssue > 0)
                    {
                        excel.writeCell(ee.mWorkSheets[currSize], "C2", sumIssue.ToString("0.##"));
                        excel.writeCell(ee.mWorkSheets[currSize], "G2", sumDoneQuant.ToString("0.##"));
                        excel.writeCell(ee.mWorkSheets[currSize], "L2", Math.Round(sumDoneQuant / sumIssue * 100, 2).ToString("0.##")); ee = excel.copyExcelSheet(ee, "母版", dr["SIZE"].ToString());
                    }

                    currCol = 2;
                    sumIssue = 0;
                    sumDoneQuant = 0;
                    currSize = dr["SIZE"].ToString();
                    excel.copyExcelSheet(ee, "母版", currSize);
                }
                //========================================

                //ee.mWorkSheets[currSize]
                excel.writeCell(ee.mWorkSheets[currSize], excel.cellIdxToStr(3, currCol), dr["SourceRoStemp"].ToString());
                excel.writeCell(ee.mWorkSheets[currSize], excel.cellIdxToStr(4, currCol), dr["良率"].ToString());
                excel.writeCell(ee.mWorkSheets[currSize], excel.cellIdxToStr(5, currCol), dr["發料量"].ToString());
                currCol++;
                sumIssue += Convert.ToDouble(dr["發料量"]);
                sumDoneQuant += Convert.ToDouble(dr["完成量"]);
            }
            string outputFileName = outputFolder + @"/" + $"組帽良率統計表_{startDate.ToString("yyyyMM")}.xlsx";
            excel.closeExcel(ee, outputFileName, false);
            return true;
        }

        public static void fillStaticData(Dictionary<string, string> excelDict, Dictionary<string, string>  layoutDict,
            Dictionary<string, string> staticDict, Dictionary<string, string> receiveDict)
        {
            string tempKey ;
            string tempValue ;
            foreach (KeyValuePair<string, string> layoutItem in layoutDict)
            {
                tempKey = layoutItem.Key;
                if (receiveDict != null && receiveDict.Keys.Contains(tempKey))
                    tempValue = receiveDict[tempKey]; 
                else if (staticDict != null && staticDict.Keys.Contains(tempKey))
                    tempValue = staticDict[tempKey];
                else if (layoutItem.Key == "#Date")
                {
                    tempValue = "'" + DateTime.Now.ToString("yyyy/MM/dd");
                }
                else if (layoutItem.Key == "#DateTime")
                    tempValue = "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                else
                    tempValue = layoutItem.Key;
                if (tempKey == null)
                    continue;
                if (!excelDict.ContainsKey(tempKey))
                    excelDict.Add(layoutItem.Value, tempValue);
                else
                    excelDict[layoutItem.Value] = tempValue;
            }
        }

        //拉力報表, 只有 組帽 製程 會做
        public static bool do_defectZuMou(Dictionary<string, string> receiveDict)
        {
            List<string> testSatndard = new List<string>();
            ///測試用 SQL
            List<DateTime> getShifTime = FirstohmPrds.getShifTime(receiveDict["shiftType"], DateTime.Parse(receiveDict["reportDate"]) );
            string sqlStr = $"SELECT `SourceRoStemp`, `RO`, b.TestSet, `CFMF`, Size, a.`SIGNID` " +
                $" FROM `view_cap_signinfo` a " +
                $" left Join cap_test b on a.`SIGNID`=b.SignID " +
                $" where `FLOW_STEP`='組帽' and `signFinish` >= '{getShifTime[0].AddHours(-4).ToString("yyyy-MM-dd HH:mm")}' " +
                $" and `signFinish` <= '{getShifTime[1].AddHours(4).ToString("yyyy-MM-dd HH:mm")}' and USER_ID='{receiveDict["EmpNo"]}' " +
                $" ORDER BY Size ";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            if (dt.Rows.Count == 0)
            {
                return true;
            }
            Dictionary<string, string>staticDataDict = new Dictionary<string, string>();
            bool ifMetal = false;
            bool ifValid = false;
            double minPullVal = 0;
            double examPullVal = 0;
            List<int> pass = new List<int>();
            staticDataDict.Add("EmpNo", receiveDict["EmpNo"]);
            staticDataDict.Add("EmpName", receiveDict["EmpName"]);
            staticDataDict.Add("RptDate", receiveDict["reportDate"]);
            staticDataDict.Add("ShiftType", receiveDict["shiftType"]);
            if (receiveDict.ContainsKey("staticData"))
            {
                receiveDict.Remove("staticData");
                receiveDict.Add("staticData", JsonConvert.SerializeObject(staticDataDict));
            }
            dt.Columns.Add("lali0", typeof(String));
            dt.Columns.Add("lali1", typeof(String));
            dt.Columns.Add("lali2", typeof(String));
            dt.Columns.Add("lali3", typeof(String));
            dt.Columns.Add("lali4", typeof(String));
            dt.Columns.Add("lali5", typeof(String));
            dt.Columns.Add("lali6", typeof(String));
            dt.Columns.Add("lali7", typeof(String));
            dt.Columns.Add("lali8", typeof(String));
            dt.Columns.Add("lali9", typeof(String));
            List<string> laliList = null;
            int rowIdx = 0;
            foreach(DataRow dr in dt.Rows)
            {
                testSatndard.Clear();
                if (dr["TestSet"] == DBNull.Value || dr["TestSet"].ToString() == "")
                {
                    rowIdx++;
                    continue;
                }

                laliList = dr["TestSet"].ToString().Split(',').ToList();
                if (laliList.Count > 0 && laliList[0] == "0")
                {
                    rowIdx++;
                    continue;
                }

                ifValid = true;
                ifMetal = FirstohmPrds.ifMetal(dr["RO"].ToString(), dr["CFMF"].ToString());
                minPullVal = FirstohmPrds.getMinPullValue(dr["Size"].ToString(), ifMetal);
                laliList = dr["TestSet"].ToString().Split(',').ToList();

                for (int i = 0; i < laliList.Count(); i++)
                {
                    examPullVal = double.Parse(laliList[i]);
                    testSatndard.Add(minPullVal.ToString("0.#"));
                    if (examPullVal < minPullVal)
                    {
                        ifValid = false;
                        dr[$"lali{i}"] = "*" + laliList[i];
                    } else    
                        dr[$"lali{i}"] = laliList[i];
                }
                //if (ifValid)
                //    pass.Add(rowIdx);
                if (ifValid)
                {
                    pass.Add(rowIdx);
                    sqlStr = $"update `cap_test` set `testStandard`='{string.Join(",", testSatndard)}' , `exame`=1 " +
                             $" where `SignID`='{dr["SignID"]}' and `EMPID`='{receiveDict["EmpNo"]}'";
                }
                else
                {
                    sqlStr = $"update `cap_test` set `testStandard`='{string.Join(",", testSatndard)}' , `exame`=2" +
                            $" where `SignID`='{dr["SignID"]}' and `EMPID`='{receiveDict["EmpNo"]}'";
                }
                CommonClass.execSQLNonQuery(sqlStr);
                rowIdx++;
            }

            for (int i = pass.Count - 1; i >= 0; i--)
                dt.Rows.RemoveAt(pass[i]);
            dt.AcceptChanges();

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i]["lali1"] == DBNull.Value || dt.Rows[i]["lali1"] == "" || dt.Rows[i]["lali1"] == "0")
                {
                    dt.Rows.RemoveAt(i);
                }
            }
            dt.AcceptChanges();
            if (dt.Rows.Count == 0)
                return true;
            string currSize = dt.Rows[dt.Rows.Count - 1]["Size"].ToString();
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if(currSize != dt.Rows[i]["Size"].ToString())
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = "尺寸:"; dr[1] = currSize;
                    currSize = dt.Rows[i]["Size"].ToString();
                    dt.Rows.InsertAt(dr, i+1);
                    
                }
            }
            DataRow drTemp = dt.NewRow();
            drTemp[0] = "尺寸:"; drTemp[1] = currSize;
            dt.Rows.InsertAt(drTemp, 0);
            currSize = dt.Rows[0]["Size"].ToString();
            ///////////////////////////////////////////
            dt.Columns.Remove("TestSet");
            dt.Columns.Remove("CFMF");
            dt.Columns.Remove("Size");
            dt.Columns.Remove("SIGNID");
            return printStdReport(dt, receiveDict);
        }

        public static void rptPropertyBasic(Dictionary<string, string> receiveDict, Dictionary<string, string> printAttrs)
        {
            if (receiveDict.ContainsKey("printAttrs"))
            {
                string formateJsonStr = receiveDict["printAttrs"].Replace("^", "'");
                printAttrs = JsonConvert.DeserializeObject<Dictionary<string, string>>(formateJsonStr);
            }
            else
                printAttrs = new Dictionary<string, string>();
            string templateFile = null; //母版位置
            string reportNameWithExt = null;
            string reportName = null;
            if (receiveDict.ContainsKey("excelFile"))
            {
                templateFile = receiveDict["excelFile"].Replace("/", "\\"); //母版位置
                                                                            //reportNameWithExt = CommonClass.getFileInfo(templateFile, "name");
                reportName = CommonClass.getFileInfo(templateFile, "filenamewithoutextension");
            }
            string tempKey = null;
            string tempValue = null;
            printAttrs.Add("ReportName", receiveDict["formtype_name"]);
            Dictionary<string, string> excelLayout = null;
            Dictionary<string, string> staticData = null;
            if (receiveDict.ContainsKey("staticLayout"))
            {
                excelLayout = JsonConvert.DeserializeObject<Dictionary<string, string>>
                             (receiveDict["staticLayout"].Replace("^", "'"));
            }
            else
                excelLayout = new Dictionary<string, string>();
        }

        //收據釘貼單
        public static bool do_printReceiptToPdf(Dictionary<string, string> receiveDict)
        {
            DataTable dt = null;
            string sqlStr = null;
            Dictionary<string, string> printAttrs = null;
            string formateJsonStr = null;                

            try
            {
                if (receiveDict.ContainsKey("printAttrs"))
                {
                    formateJsonStr = receiveDict["printAttrs"].Replace("^", "'");
                    printAttrs = JsonConvert.DeserializeObject<Dictionary<string, string>>(formateJsonStr);
                }
                else
                    printAttrs = new Dictionary<string, string>();
                string templateFile = null; //母版位置
                string reportNameWithExt = null;
                string reportName = null;
                if (receiveDict.ContainsKey("excelFile"))
                {
                    templateFile = receiveDict["excelFile"].Replace("/", "\\"); //母版位置
                    //reportNameWithExt = CommonClass.getFileInfo(templateFile, "name");
                    reportName = CommonClass.getFileInfo(templateFile, "filenamewithoutextension");
                }
                string tempKey = null;
                string tempValue = null;
                printAttrs.Add("ReportName", receiveDict["formtype_name"]);
                Dictionary<string, string> excelLayout = null;
                Dictionary<string, string> staticData = null;
                if (receiveDict.ContainsKey("staticLayout"))
                {
                    excelLayout = JsonConvert.DeserializeObject<Dictionary<string, string>>
                                 (receiveDict["staticLayout"].Replace("^", "'"));
                }
                else
                    excelLayout = new Dictionary<string, string>();

                int currPage = 1;
                string outputFolder = null;

                outputFolder = null;
                if (receiveDict.ContainsKey("output_path"))
                    outputFolder = $"{portal_root_path}/{receiveDict["output_path"].Replace("\\", "/")}";
                //portal_root_path:C:\xampp\htdocs\portal
                reportNameWithExt = $"{(receiveDict.ContainsKey("formtype_name") ? receiveDict["formtype_name"] + "_" : "")} " +
                        $"{DateTime.Now.ToString("MMddHHmmss")}_{reportName}.pdf";

                string outputFileName = $"{outputFolder}/{reportNameWithExt}";
                CommonClass.createIfMissing(outputFolder);
                if (CommonClass.ifFileExists(outputFileName))
                {
                    outputFileName = CommonClass.addSeqToFileName(outputFileName);
                }
                bool ifPrint = printAttrs.Keys.Contains("ifPrint") ? bool.Parse(printAttrs["ifPrint"]) : false;
                bool ifClose = printAttrs.Keys.Contains("ifClose") ? bool.Parse(printAttrs["ifClose"]) : false;
                bool excelShow = printAttrs.Keys.Contains("excelShow") ? bool.Parse(printAttrs["excelShow"]) : false;

                Dictionary<string, string> excelDict = new Dictionary<string, string>();
                fillStaticData(excelDict, excelLayout, staticData, receiveDict);
                List<Dictionary<string, string>> receiptItems = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(receiveDict["detail"]);
                List<string> detalColumns = JsonConvert.DeserializeObject<List<string>>(receiveDict["detalColumns"]);
                int pageIdx = 0;
                int pageSize = 46;
                string tempStr = null;
                //////////////////////////////////////////////////////////
                ExcelLib excel = null;
                if (excel == null)
                    excel = new ExcelLib();

                ExcelElements ee = excel.openExcel(templateFile, true);
                if (printAttrs.Keys.Contains("excelShow"))
                    ee.oXL.Visible = excelShow;
                for (int pageDataItem = 0, allDataItem = 0; allDataItem < receiptItems.Count; 
                     pageDataItem++, allDataItem++)
                {
                    //寫表頭
                    foreach (KeyValuePair<string, string> dictItem in excelDict)
                        excel.fillExcelCellValue(ee.mWorkSheets["母版"], dictItem.Key, dictItem.Value);
                    tempStr = "";
                    foreach (string detailColShowItem in detalColumns)
                    {
                        tempStr += $"{receiptItems[allDataItem][detailColShowItem]}{Environment.NewLine}";
                    }
                    if (pageDataItem < 5)
                    {
                        excel.fillExcelCellValue(ee.mWorkSheets["母版"], $"B{7 + pageDataItem * 8}", tempStr);
                    }
                    else
                    {
                        excel.fillExcelCellValue(ee.mWorkSheets["母版"], $"F{7 + (pageDataItem - 5) * 8}", tempStr);
                    }

                    if (pageDataItem >= 9 || pageDataItem == receiptItems.Count - 1)
                    {
                        pageDataItem = -1;
                        pageIdx = allDataItem / 10;
                        excel.copyPasteRange(ee.mWorkSheets["母版"], ee.mWorkSheets["報表"], $"A1..I{pageSize}", $"A{pageIdx * pageSize + 1}", true);
                        if (allDataItem == 0)
                            excel.copyPasteSpecial(ee.mWorkSheets["母版"], ee.mWorkSheets["報表"], null, null, Microsoft.Office.Interop.Excel.XlPasteType.xlPasteFormats);
                        excel.copyPasteRange(ee.mWorkSheets["cleanForm"], ee.mWorkSheets["母版"], $"A1..I{pageSize}", $"A{pageIdx * pageSize + 1}", true);
                        excel.copyPasteSpecial(ee.mWorkSheets["cleanForm"], ee.mWorkSheets["母版"], null, null, Microsoft.Office.Interop.Excel.XlPasteType.xlPasteFormats);
                    }
                }
                ee.mWSheet = ee.mWorkSheets["報表"];
                string destPrinter = "Epson EPL-6200 (左下)";
                if (ifPrint)
                {
                    //使用 property 共用的印表機
                    destPrinter = printAttrs["ReportPrinter"];
                    if (printAttrs.ContainsKey("printer"))
                    {
                        //使用 本屬性 自用的印表機
                        destPrinter = printAttrs["printer"];
                    }
                    excel.printExcelBySpecifyPrinter(ee, destPrinter);
                }
                excel.saveToPdf(ee, outputFileName);
                excel.closeExcel(ee, null, false);

                string tnStr = printAttrs["ReportName"] + " 列印完成";
                return true;
            } 
            catch(Exception ex)
            {
                return false;
            }
        }

        //費用申請單 to Pdf
        public static bool do_DtailtoPdf(Dictionary<string, string> receiveDict)
        {
            DataTable dt = null;
            string sqlStr = null;
            Dictionary<string, string> printAttrs = null;
            string formateJsonStr = null;

            try
            {
                if (receiveDict.ContainsKey("printAttrs"))
                {
                    formateJsonStr = receiveDict["printAttrs"].Replace("^", "'");
                    printAttrs = JsonConvert.DeserializeObject<Dictionary<string, string>>(formateJsonStr);
                }
                else
                    printAttrs = new Dictionary<string, string>();
                string templateFile = null; //母版位置
                string reportNameWithExt = null;
                string reportName = null;
                if (receiveDict.ContainsKey("excelFile"))
                {
                    templateFile = receiveDict["excelFile"].Replace("/", "\\"); //母版位置
                    //reportNameWithExt = CommonClass.getFileInfo(templateFile, "name");
                    reportName = CommonClass.getFileInfo(templateFile, "filenamewithoutextension");
                }
                string tempKey = null;
                string tempValue = null;
                printAttrs.Add("ReportName", receiveDict["formtype_name"]);
                Dictionary<string, string> excelLayout = null;
                Dictionary<string, string> staticData = null;
                if (receiveDict.ContainsKey("staticLayout"))
                {
                    excelLayout = JsonConvert.DeserializeObject<Dictionary<string, string>>
                                 (receiveDict["staticLayout"].Replace("^", "'"));
                }
                else
                    excelLayout = new Dictionary<string, string>();

                int currPage = 1;
                string outputFolder = null;
                List<Dictionary<string, string>> receiptItems = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(CommonClass.convertBadJson(receiveDict["detail"]));
                dt = CommonClass.dicttoDT<string>(receiptItems);
      
                return printStdReport(dt, receiveDict, "母版", "報表");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool do_askForPayment(Dictionary<string, string> receiveDict)
        {
            DataTable dt = null;
            string sqlStr = null;
            Dictionary<string, string> printAttrs = null;

            try
            {
                rptPropertyBasic(receiveDict, printAttrs);
                int currPage = 1;
                string outputFolder = null;
                List<Dictionary<string, string>> receiptItems = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(CommonClass.convertBadJson(receiveDict["detail"]));
                dt = CommonClass.dicttoDT<string>(receiptItems);
                dt = CommonClass.DataTableFilterSort1(dt, null, "廠商");
                dt.Columns["年"].SetOrdinal(0);
                dt.Columns["月"].SetOrdinal(1);
                dt.Columns["日"].SetOrdinal(2);
                dt.Columns.Add("金額SUM", typeof(string));
                string tempStr = null;
                string tempVender = null;
                double tempSum = 0;
                DateTime tempDate;
                int dowIdx = 0;
                foreach(DataRow dr in dt.Rows)
                {
                    if(dr["廠商"].ToString() != tempVender)
                    {
                        if (tempVender != null)
                            dt.Rows[dowIdx-1]["金額SUM"] = tempSum.ToString("0.#");
                        tempVender = dr["廠商"].ToString();
                        tempSum = 0;
                    }
                    tempSum += Convert.ToDouble(dr["金額"]);
                    dowIdx++;
                }
                dt.Rows[dowIdx - 1]["金額SUM"] = tempSum.ToString("0.#");

                for (int i=0; i<dt.Rows.Count; i++)
                {

                    if(i < dt.Rows.Count-1 &&  dt.Rows[i]["金額SUM"]!=null && dt.Rows[i]["金額SUM"] != DBNull.Value)
                    {
                        i++;
                        DataRow row = dt.NewRow();
                        dt.Rows.InsertAt(row, i);
                    }
                }
                dt.Columns.Remove("收據日期");
                dt.AcceptChanges();

                if (!receiveDict.ContainsKey("managersigniture") && receiveDict.ContainsKey("manager"))
                    receiveDict.Add("managersigniture", receiveDict["receiveDict"]);
                if (!receiveDict.ContainsKey("usersigniture") && receiveDict.ContainsKey("user_name"))
                    receiveDict.Add("managersigniture", receiveDict["user_name"]);
                return printStdReport(dt, receiveDict, "母版", "報表");
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool do_assessment(Dictionary<string,string> receiveDict , int reprintidx = 0)
        {
            
            try
            {
                string rptTitle = receiveDict["year"] + " " + receiveDict["yearType"];
                string excelFile = receiveDict["excelFile"].Replace("/", "\\");
                string outputFolder;
                if (receiveDict["fromWhere"] == "portal")
                {
                    outputFolder = (receiveDict["portalOutputPath"] + "/" + receiveDict["year"] + receiveDict["yearType"]).Replace("/", "\\");
                }
                else if (receiveDict["fromWhere"] == "portal-new")
                {
                    outputFolder = (receiveDict["portalnewOutputPath"] + "/" + receiveDict["year"] + receiveDict["yearType"]).Replace("/", "\\");
                }
                else
                {
                    string sendmailbody = $"{receiveDict["year"]} {receiveDict["yearType"]} 考核表找無fromWhere";
                    CommonClass.smtpSendMail(sendmailbody,
                                new Dictionary<string, string>() { { "資訊部", "id@firstohm.com.tw" } },
                                true, "火箭自動通知 列印考核表發生嚴重錯誤");
                    return false;
                }
                if (!Directory.Exists(outputFolder))
                {
                    CommonClass.createIfMissing(outputFolder);
                }
                if (!receiveDict.ContainsKey("userid"))
                    CommonClass.deleteFoldersAndFiles(outputFolder, false, true);

                string half;
                if (receiveDict["yearType"] == "上半年")
                    half = "1";
                else if (receiveDict["yearType"] == "下半年")
                    half = "2";
                else
                    half = "-1";

                string sqlStr = $"SELECT a.`id_apply`,a.`form_type_id`,a.`user_id`,a.`year` '年份' , a.`half` '年度' , a.`title` '職稱' , " +
                    $" b.`department` '部門' , a.`name` '姓名' , a.`onBordDate` '到職日' , a.`age` '年齡' , a.`jobDiscript` '基本職務內容' , " +
                    $" a.`dynamicInput` '年度工作目標' , a.`train` '發展培育' , a.`job_change` '異動希望' , a.`suggestion` '對公司建議' , a.`otherResults` '具體成果' , " +
                    $" a.`supervisor_rank` '主管評分' , a.`supervisor_comment` '主管評語' , a.`final_result` " +
                    $" FROM `xyz_uj_assessmentrtp_views` a " +
                    $" JOIN `xyz_hikashop_user` b ON a.`user_id` = b.`user_cms_id` " +
                    $" WHERE b.`workstatus` = 0 " +
                    $" AND a.`year` = {receiveDict["year"]} " +
                    $" AND a.`half` = {half} ";
                if (receiveDict.ContainsKey("userid")) 
                {
                    if(receiveDict["userid"] == null || receiveDict["userid"] == "")
                    {
                        string sendmailbody = $"{receiveDict["year"]} {receiveDict["yearType"]} send URL userid 為 NULL 或 空白";
                        CommonClass.smtpSendMail(sendmailbody,
                                    new Dictionary<string, string>() { { "資訊部", "id@firstohm.com.tw" } },
                                    true, "火箭自動通知 列印考核表發生嚴重錯誤");
                        return false;
                    }
                    sqlStr += $" AND a.`user_id` = {receiveDict["userid"]} ";
                }
                string connstring;
                if (receiveDict["fromWhere"] == "portal")
                {
                    connstring = Constants.portalConnString;
                }
                else if (receiveDict["fromWhere"] == "portal-new")
                {
                    connstring = Constants.portalnewConnString;
                }
                else
                {
                    string sendmailbody = $"{receiveDict["year"]} {receiveDict["yearType"]} 考核表找無fromWhere";
                    CommonClass.smtpSendMail(sendmailbody,
                                new Dictionary<string, string>() { { "資訊部", "id@firstohm.com.tw" } },
                                true, "火箭自動通知 列印考核表發生嚴重錯誤");
                    return false;
                }
                DataTable dt = CommonClass.getSQLDataTable(sqlStr, connstring);

                ExcelLib excel = new ExcelLib();
                Dictionary<string, string> dicToExcelData;
                string jsonDynamic;
                List<List<Dictionary<string, String>>> dynaInput;
                bool saveAsPdf;
                foreach (DataRow dr in dt.Rows)
                {
                    saveAsPdf = true;
                    ExcelElements ee = excel.openExcel(excelFile);
                    ee.mWSheet = excel.getWorksheet(ee, "母版");
                    dicToExcelData = new Dictionary<string, string>();
                    jsonDynamic = dr["年度工作目標"].ToString().Replace("\"", "'");
                    dynaInput = JsonConvert.DeserializeObject<List<List<Dictionary<string, string>>>>(jsonDynamic);
                    string yearType;
                    switch (dr["年度"].ToString())
                    {
                        case "1":
                            yearType = "上半年";
                            break;
                        case "2":
                            yearType = "下半年";
                            break;
                        default:
                            yearType = "未知";
                            break;
                    }

                    dicToExcelData.Add("A1", dr["年份"].ToString() + " " + yearType + " 工作目標管理與績效考核表");
                    dicToExcelData.Add("B2", dr["部門"].ToString());
                    dicToExcelData.Add("I2", dr["職稱"].ToString());
                    dicToExcelData.Add("B3", dr["姓名"].ToString());
                    dicToExcelData.Add("F3", dr["到職日"].ToString());
                    dicToExcelData.Add("J3", dr["年齡"].ToString());
                    dicToExcelData.Add("C4", dr["基本職務內容"].ToString().Replace("<br>", "\n"));
                    int idx = 5;
                    double personalTotal = 0;
                    foreach (List<Dictionary<string, string>> listdic in dynaInput)
                    {
                        idx++;
                        if (idx > 23)
                        {
                            string sendmailbody = $"火箭列印考核表出現錯誤\n" +
                                $"{dr["年份"].ToString()}{yearType}{dr["姓名"].ToString()}\n" +
                                $"工作目標超過18筆，考核版行數不足請修改母版";
                            CommonClass.smtpSendMail(sendmailbody,
                                new Dictionary<string, string>() { { "資訊部", "id@firstohm.com.tw" } },
                                true, "火箭自動通知");
                            saveAsPdf = false;
                            break;
                        }
                        foreach (Dictionary<string, string> dic in listdic)
                        {
                            if (dic.ContainsKey("工作目標"))
                                dicToExcelData.Add("C" + idx, dic["工作目標"]);
                            if (dic.ContainsKey("期限"))
                                dicToExcelData.Add("H" + idx, dic["期限"]);
                            if (dic.ContainsKey("比例"))
                                dicToExcelData.Add("I" + idx, dic["比例"] + "%");
                            if (dic.ContainsKey("完成日期"))
                                dicToExcelData.Add("J" + idx, dic["完成日期"]);
                            if (dic.ContainsKey("執行成果"))
                            {
                                double personal;
                                dicToExcelData.Add("K" + idx, dic["執行成果"] + "%");
                                if (double.TryParse(dic["執行成果"], out personal))
                                    personalTotal += personal;
                            }
                        }
                    }
                    dicToExcelData.Add("C24", personalTotal.ToString() + "%");
                    dicToExcelData.Add("C25", dr["發展培育"].ToString().Replace("<br>", "\n"));
                    dicToExcelData.Add("C30", dr["具體成果"].ToString().Replace("<br>", "\n"));
                    dicToExcelData.Add("C31", dr["異動希望"].ToString().Replace("<br>", "\n"));
                    dicToExcelData.Add("C32", dr["對公司建議"].ToString().Replace("<br>", "\n"));
                    dicToExcelData.Add("C34", dr["主管評分"].ToString().Replace("<br>", "\n"));
                    dicToExcelData.Add("E34", dr["主管評語"].ToString().Replace("<br>", "\n"));
                    dicToExcelData.Add("K37", DateTime.Now.ToString("yyyy/MM/dd"));
                    excel.dictToExcel(ee.mWSheet, dicToExcelData);
                    string outputFile = outputFolder + $"\\{dr["部門"].ToString().Replace("/","_").Replace("\\","_")}_{dr["姓名"].ToString()}.xlsx";
                    if (saveAsPdf)
                    {
                        
                        try
                        {//由於存成PDF會偶發性發生錯誤，故使用try/catch重新列印
                            excel.saveToPdf(ee, outputFile.Replace(".xlsx", ".pdf"));
                            excel.closeExcel(ee, null, false);
                        }
                        catch (Exception ex)
                        {
                            excel.closeExcel(ee, null, false);
                            reprintidx++;
                            if(reprintidx <= 5)
                            {
                                Dictionary<string, string> rePrintReceiveDict = new Dictionary<string, string>();
                                foreach (KeyValuePair<string, string> dic in receiveDict)
                                    rePrintReceiveDict.Add(dic.Key, dic.Value);
                                if (!rePrintReceiveDict.ContainsKey("userid"))
                                    rePrintReceiveDict.Add("userid", dr["user_id"].ToString());
                                do_assessment(rePrintReceiveDict, reprintidx);
                            }
                            else
                            {
                                string sendmailbody = $"{receiveDict["year"]} {receiveDict["yearType"]} {dr["姓名"].ToString()} 考核表轉PDF 5次 發生錯誤\n" +
                                            ex.Message + "\n" + ex.StackTrace.ToString();
                                CommonClass.smtpSendMail(sendmailbody,
                                            new Dictionary<string, string>() { { "資訊部", "id@firstohm.com.tw" } },
                                            true, "火箭自動通知 列印考核表發生嚴重錯誤");
                            }
                        }
                        
                    }
                    else
                    {
                        excel.closeExcel(ee, outputFile, false);
                    }
                }
                if (!receiveDict.ContainsKey("userid"))
                {
                    string sendmailbody = $"{receiveDict["year"]} {receiveDict["yearType"]} 考核表已列印完成請至\n" +
                        $"【{outputFolder}】查看是否正確";
                    CommonClass.smtpSendMail(sendmailbody,
                                new Dictionary<string, string>() { { "資訊部", "id@firstohm.com.tw" } },
                                true, "火箭自動通知");
                }
                return true;
            }
            catch(Exception ex)
            {
                string sendmailbody = $"{receiveDict["year"]} {receiveDict["yearType"]} 考核表列印錯誤\n" +
                        ex.Message+"\n" + ex.StackTrace.ToString();
                CommonClass.smtpSendMail(sendmailbody,
                            new Dictionary<string, string>() { { "資訊部", "id@firstohm.com.tw" } },
                            true, "火箭自動通知 列印考核表發生嚴重錯誤");
                return false;
            }
        }

        public static bool process_zumao_DailyReport(Dictionary<string, string> receiveDict, DataTable dt, string doFlowStep)
        {
            Dictionary<string, string> staticDataDict = new Dictionary<string, string>();
            Dictionary<string, double> sizeStatisticDict = new Dictionary<string, double>();
            List<int> pass = new List<int>();
            int rowIdx = 0;
            string currSize = dt.Rows[dt.Rows.Count - 1]["Size"].ToString();
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (!sizeStatisticDict.ContainsKey(dt.Rows[i]["Size"].ToString()))
                    sizeStatisticDict.Add(dt.Rows[i]["Size"].ToString(), 0);
                sizeStatisticDict[dt.Rows[i]["Size"].ToString()] += Convert.ToDouble(dt.Rows[i]["OutputQuan"]);
                if (currSize != dt.Rows[i]["Size"].ToString())
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = "尺寸:"; dr[1] = currSize;
                    currSize = dt.Rows[i]["Size"].ToString();
                    dt.Rows.InsertAt(dr, i+1);
                }
            }
            DataRow drTemp = dt.NewRow();
            drTemp[0] = "尺寸:"; drTemp[1] = currSize;
            dt.Rows.InsertAt(drTemp, 0);
            currSize = dt.Rows[0]["Size"].ToString();
            ///////////////////////////////////////////
            dt.Columns.Remove("Size");
            string sizeStatisticStr = null;
            foreach (KeyValuePair<string, double> sizeItem in sizeStatisticDict)
            {
                sizeStatisticStr += $" {sizeItem.Key}: {sizeItem.Value};";
            }
            staticDataDict.Add("尺寸統計", sizeStatisticStr);
            staticDataDict.Add("EmpNo", receiveDict["EmpNo"]);
            staticDataDict.Add("EmpName", receiveDict["EmpName"]);
            staticDataDict.Add("RptDate", receiveDict["reportDate"]);
            staticDataDict.Add("ShiftType", receiveDict["shiftType"]);
            staticDataDict.Add("Size", String.Join(" ; ", new List<string>(sizeStatisticDict.Keys)));
            staticDataDict.Add("Flow_Step", $"{doFlowStep}");
            staticDataDict.Add("flowStep", $"{doFlowStep}");
            if (receiveDict.ContainsKey("staticData"))
            {
                receiveDict.Remove("staticData");
                receiveDict.Add("staticData", JsonConvert.SerializeObject(staticDataDict));
            }
            dt.Columns.Remove("FLOW_STEP");
            return printStdReport(dt, receiveDict);
        }


        //組帽日報表， 改用 newJson 列印
        public static bool do_zumaoDailyReport(Dictionary<string, string> receiveDict)
        {
            List<string> testSatndard = new List<string>();
            ///測試用 SQL
            List<DateTime> getShifTime = FirstohmPrds.getShifTime(receiveDict["shiftType"], DateTime.Parse(receiveDict["reportDate"]));
            string sqlStr;
            bool finalResut = false;
            DataTable dt = null;
            sqlStr = "SELECT a.`MachineID`, b.`SourceRoStemp`, b.`RO`, SUM(a.`OutputQuan`) OutputQuan , b.SIZE, a.FLOW_STEP " +
                $" FROM `cap_sign` a " +
                $" LEFT JOIN `cap_subflow` b ON a.`SUBFLOWID`= b.`SUBFLOWID` " +
                $" WHERE USER_ID = '{receiveDict["EmpNo"]}' and a.`Finish_Time` >= '{getShifTime[0].AddHours(-4).ToString("yyyy-MM-dd HH:mm")}' " +
                $" and a.`Finish_Time` <= '{getShifTime[1].AddHours(4).ToString("yyyy-MM-dd HH:mm")}'  " +
                $" GROUP BY a.`MachineID`, b.`SourceRoStemp` , a.`SUBFLOWID` , a.`FLOW_STEP` ORDER BY MachineID ";

            DataTable sourceDT = CommonClass.getSQLDataTable(sqlStr);

            if (sourceDT.Rows.Count == 0) //無資料
            {
                return true;
            }
            //if (receiveDict["flowStep"] == "組帽")
            //    return process_zumao_DailyReport(receiveDict, sourceDT);
            //else //篩料 or 分類
            //{
                dt = CommonClass.DataTableFilterSort1(sourceDT, "FLOW_STEP='組帽'", "SIZE");
                if (dt.Rows.Count > 0)
                {
                //dt.Columns.RemoveAt(5);

                finalResut = process_zumao_DailyReport(receiveDict, dt, "組帽");
                    if (!finalResut)
                        return false;
                }

                dt = CommonClass.DataTableFilterSort1(sourceDT, "FLOW_STEP='篩料'", "SIZE");
                if(dt.Rows.Count > 0)
                {
                //dt.Columns.RemoveAt(5);
                finalResut = process_zumao_DailyReport(receiveDict, dt, "篩料");
                    if (!finalResut)
                        return false;
                }

                dt = CommonClass.DataTableFilterSort1(sourceDT, "FLOW_STEP='分類'", "SIZE");
                if (dt.Rows.Count > 0)
                {
                //dt.Columns.RemoveAt(5);
                return process_zumao_DailyReport(receiveDict, dt, "分類");
                }
                else
                    return finalResut;
            //}
/////////////////////////////////////////////////////////////////////////////////////////////////
            //Dictionary<string, string> staticDataDict = new Dictionary<string, string>();
            //Dictionary<string, double> sizeStatisticDict = new Dictionary<string, double>();
            //List<int> pass = new List<int>();
            //int rowIdx = 0;
            //if (dt.Rows.Count == 0)
            //    return true;
            //string currSize = dt.Rows[dt.Rows.Count - 1]["Size"].ToString();
            //for (int i = dt.Rows.Count - 1; i >= 0; i--)
            //{
            //    if (!sizeStatisticDict.ContainsKey(dt.Rows[i]["Size"].ToString()))
            //        sizeStatisticDict.Add(dt.Rows[i]["Size"].ToString(), 0);
            //    sizeStatisticDict[dt.Rows[i]["Size"].ToString()] += Convert.ToDouble(dt.Rows[i]["OutputQuan"]);
            //    if (currSize != dt.Rows[i]["Size"].ToString())
            //    {
            //        DataRow dr = dt.NewRow();
            //        dr[0] = "尺寸:"; dr[1] = currSize;
            //        currSize = dt.Rows[i]["Size"].ToString();
            //        dt.Rows.InsertAt(dr, i);
            //    }
            //}
            //DataRow drTemp = dt.NewRow();
            //drTemp[0] = "尺寸:"; drTemp[1] = currSize;
            //dt.Rows.InsertAt(drTemp, 0);
            //currSize = dt.Rows[0]["Size"].ToString();
            /////////////////////////////////////////////
            //dt.Columns.Remove("Size");
            //string sizeStatisticStr = null;
            //foreach(KeyValuePair<string, double> sizeItem in sizeStatisticDict)
            //{
            //    sizeStatisticStr += $" {sizeItem.Key}: {sizeItem.Value};";
            //}
            //staticDataDict.Add("尺寸統計", sizeStatisticStr);
            //staticDataDict.Add("EmpNo", receiveDict["EmpNo"]);
            //staticDataDict.Add("EmpName", receiveDict["EmpName"]);
            //staticDataDict.Add("RptDate", receiveDict["reportDate"]);
            //staticDataDict.Add("ShiftType", receiveDict["shiftType"]);
            //staticDataDict.Add("Size", String.Join(" ; ", new List<string>(sizeStatisticDict.Keys)));

            //if (receiveDict.ContainsKey("staticData"))
            //{
            //    receiveDict.Remove("staticData");
            //    receiveDict.Add("staticData", JsonConvert.SerializeObject(staticDataDict));
            //}
            //return printStdReport(dt, receiveDict);
        }

        public static bool printStdReport(DataTable dt, Dictionary<string, string> receiveDict, string templateWSname = "Template", string desttWSname = "報表", string rptUser = null)
        {
            string templateFile = receiveDict["excelFile"].Replace("/", "\\"); //母版位置
            string reportNameWithExt = CommonClass.getFileInfo(templateFile, "name");
            string reportName = CommonClass.getFileInfo(templateFile, "filenamewithoutextension");
            string formateJsonStr = receiveDict["printAttrs"].Replace("^", "'");

            Dictionary<string, string> printAttrs = JsonConvert.DeserializeObject<Dictionary<string, string>>(formateJsonStr);
            printAttrs.Add("ReportName", reportName);
            Dictionary<string, string> excelLayout = JsonConvert.DeserializeObject<Dictionary<string, string>>(receiveDict["staticLayout"].Replace("^", "'"));
            Dictionary<string, string> sqlData = null;
            Dictionary<string, string> staticData = null;
            if (receiveDict.ContainsKey("sqlData"))
                sqlData = JsonConvert.DeserializeObject<Dictionary<string, string>>(receiveDict["sqlData"].Replace("^", "\"").Replace("~", "\""));
            if (receiveDict.ContainsKey("staticData"))
                staticData = JsonConvert.DeserializeObject<Dictionary<string, string>>(receiveDict["staticData"].Replace("^", "'"));
            string tempValue = null;
            string tempKey = null;
            int currPage = 1;
            ExcelLib excel = null;
            string outputFolder = receiveDict.ContainsKey("OutputPath") ? receiveDict["OutputPath"] : printAttrs["OutputPath"].Replace("/", "\\");
            CommonClass.createIfMissing(outputFolder);
            string rtpDateStr = null;
            if (string.IsNullOrEmpty(rptUser))
            {
                if (receiveDict.ContainsKey("rptUser"))
                    rptUser = receiveDict["rptUser"];
                else if (receiveDict.ContainsKey("EmpName"))
                    rptUser = receiveDict["EmpName"];
                else if (staticData != null && staticData.ContainsKey("EmpName"))
                    rptUser = staticData["EmpName"];
                else
                    rptUser = "";
            }

            if (receiveDict.ContainsKey("reportDate"))
            {
                receiveDict["reportDate"] = receiveDict["reportDate"].Replace("-", @"/");
                rtpDateStr = DateTime.Parse(receiveDict["reportDate"]).ToString("yyyyMMdd");
                reportNameWithExt = $"{rtpDateStr}_{rptUser}_" +
                    $"{DateTime.Now.ToString("MMddHHmmss")}_{reportNameWithExt}";
            }
            else if (staticData != null && staticData.ContainsKey("RptDate"))
            {
                staticData["RptDate"] = staticData["RptDate"].Replace("-", @"/");
                rtpDateStr = DateTime.Parse(staticData["RptDate"]).ToString("yyyyMMdd");
                reportNameWithExt = $"{rtpDateStr}_{rptUser}_" +
                    $"{DateTime.Now.ToString("MMddHHmmss")}_{reportNameWithExt}";
            }
            else
            {
                rtpDateStr = DateTime.Now.ToString("yyyyMMdd");
                reportNameWithExt = $"{rtpDateStr}_{rptUser}_{DateTime.Now.ToString("MMddHHmmss")}_{reportNameWithExt}";
            }

            //string outputFileName = outputFolder.Replace("/", @"\") + @"\" + reportNameWithExt.Replace("母版", "");
            string outputFileName = outputFolder.Replace("/", @"\") + reportNameWithExt.Replace("母版", "");
            excelLayout = translateStaticDict(staticData, excelLayout, receiveDict, dt, 1);
            if (CommonClass.ifFileExists(outputFileName))
            {
                outputFileName = CommonClass.addSeqToFileName(outputFileName);
            }
            bool ifPrint = printAttrs.Keys.Contains("ifPrint") ? bool.Parse(printAttrs["ifPrint"]) : false;
            bool ifPdf = printAttrs.Keys.Contains("ifPdf") ? bool.Parse(printAttrs["ifPdf"]) : false;
            bool ifClose = printAttrs.Keys.Contains("ifClose") ? bool.Parse(printAttrs["ifClose"]) : false;
            bool excelShow = printAttrs.Keys.Contains("excelShow") ? bool.Parse(printAttrs["excelShow"]) : false;

            if (excel == null)
                excel = new ExcelLib();

            ExcelElements ee = excel.openExcel(templateFile, true);
            if (printAttrs.Keys.Contains("excelShow"))
                ee.oXL.Visible = excelShow;
            foreach (KeyValuePair<string, string> layoutItem in excelLayout)
            {
                if (layoutItem.Value.Substring(0, 1) == "$") //從staticData 提取資料
                {
                    tempKey = layoutItem.Value.Substring(1);
                    if (staticData.Keys.Contains(tempKey))
                        tempValue = staticData[tempKey];
                    else
                        continue;
                }
                else if (layoutItem.Value == "#Date")
                {
                    tempValue = "'" + DateTime.Now.ToString("yyyy/MM/dd");
                }
                else if (layoutItem.Value == "#DateTime")
                    tempValue = "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                else
                    tempValue = layoutItem.Value;
                //excel.fillExcelCellValue(ee.mWorkSheets[templateWSname], layoutItem.Key, tempValue);
                if (tempValue.Contains("ATTPic:")) {
                    tempValue = tempValue.Replace("ATTPic:", "");
                    tempValue = $"ATTPic:{printAttrs["attPicPath"]}\\{tempValue}";
                }

                excel.fillExcelAllSolution(ee.mWorkSheets[templateWSname], layoutItem.Key, tempValue);

            }
            if (dt.Rows.Count > 0)
                currPage = excel.report_dtPageModeNewpageDontPlusOneRow(dt, ee, printAttrs, currPage, (currPage > 1 ? false : true), desttWSname, templateWSname, false, true, 12,
                    (receiveDict.ContainsKey("dataColsIdx") ? receiveDict["dataColsIdx"] : null));
            currPage = currPage + 1;
            ee.mWSheet = ee.mWorkSheets[desttWSname];
            string destPrinter = "Epson EPL-6200 (左下)";
            if (ifPrint)
            {
                //使用 property 共用的印表機
                destPrinter = printAttrs["ReportPrinter"];

                if (printAttrs.ContainsKey("printer"))
                {
                    //使用 本屬性 自用的印表機
                    destPrinter = printAttrs["printer"];
                }
                excel.printExcelBySpecifyPrinter(ee, destPrinter);
            }

            if (ifPdf)
            {

                string pdf_outputFileNam = outputFileName.Replace("_母版", "");
                pdf_outputFileNam = outputFileName.Replace(".xlsx", ".pdf");
                excel.saveToPdf(ee, pdf_outputFileNam, desttWSname);
            }

            if (ifClose)
                excel.closeExcel(ee, outputFileName, false);

            if (sqlData != null && sqlData.ContainsKey("afterPrintSql"))
            {
                string sqlStr = sqlData["afterPrintSql"];
                CommonClass.execSQLNonQuery(sqlStr, Constants.WarehouseConnString);
            }

            string tnStr = printAttrs["ReportName"] + " 列印完成";
            return true;
        }

        public static bool printStdReport(Dictionary<string, string> receiveDict, out string rtnStr)
        {
            string templateFile = receiveDict["excelFile"].Replace("/","\\"); //母版位置
            string reportNameWithExt = CommonClass.getFileInfo(templateFile, "name");
            string reportName = CommonClass.getFileInfo(templateFile, "filenamewithoutextension");
            string formateJsonStr = receiveDict["printAttrs"].Replace("^", "'");
            Dictionary<string, string> printAttrs = JsonConvert.DeserializeObject<Dictionary<string, string>>(formateJsonStr);
            printAttrs.Add("ReportName", reportName);
            Dictionary<string, string> excelLayout = JsonConvert.DeserializeObject<Dictionary<string, string>>(receiveDict["staticLayout"].Replace("^", "'"));
            Dictionary<string, string> sqlData = JsonConvert.DeserializeObject<Dictionary<string, string>>(receiveDict["sqlData"].Replace("^", "\"").Replace("~", "\""));
            Dictionary<string, string> staticData = JsonConvert.DeserializeObject<Dictionary<string, string>>(receiveDict["staticData"].Replace("^", "'"));
            string tempValue = null;
            string tempKey = null;
            ExcelLib excel = null;
            //string templateFolder = Constants.getProperty("Ro章日報母板路徑", @"C:\sysTray\母版");
            string outputFolder = printAttrs["OutputPath"].Replace("/", "\\");
            //if(receiveDict.ContainsKey("reportDate"))
            //{
            //    reportNameWithExt = $"{receiveDict["reportDate"]}_{(receiveDict.ContainsKey("EmpName") ? receiveDict["EmpName"] + "_" : "")} " +
            //        $"{DateTime.Now.ToString("MMddHHmmss")}_{reportNameWithExt}";
            //} 
            //else if(staticData.ContainsKey("reportDate"))
            //    reportNameWithExt = $"{staticData["reportDate"]}_{(staticData.ContainsKey("EmpName") ? staticData["EmpName"] + "_" : "")} " +
            //        $"{DateTime.Now.ToString("MMddHHmmss")}_{reportNameWithExt}";

            string rtpDateStr = null;
            if (receiveDict.ContainsKey("reportDate"))
            {
                receiveDict["reportDate"] = receiveDict["reportDate"].Replace("-", @"/");
                rtpDateStr = DateTime.Parse(receiveDict["reportDate"]).ToString("yyyyMMdd");
                reportNameWithExt = $"{rtpDateStr}_{(receiveDict.ContainsKey("EmpName") ? receiveDict["EmpName"] + "_" : "")} " +
                    $"{DateTime.Now.ToString("MMddHHmmss")}_{reportNameWithExt}";
            }
            else if (staticData.ContainsKey("reportDate"))
            {
                staticData["reportDate"] = staticData["reportDate"].Replace("-", @"/");
                rtpDateStr = DateTime.Parse(staticData["reportDate"]).ToString("yyyyMMdd");
                reportNameWithExt = $"{rtpDateStr}_{(staticData.ContainsKey("EmpName") ? staticData["EmpName"] + "_" : "")} " +
                    $"{DateTime.Now.ToString("MMddHHmmss")}_{reportNameWithExt}";
            }

            string outputFileName = outputFolder + @"\" + reportNameWithExt;
            excelLayout = translateStaticDict(staticData, excelLayout, receiveDict);
            if (CommonClass.ifFileExists(outputFileName))
            {
                outputFileName = CommonClass.addSeqToFileName(outputFileName);
            }
            bool ifPrint = printAttrs.Keys.Contains("ifPrint") ? bool.Parse(printAttrs["ifPrint"]) : false;
            bool ifClose = printAttrs.Keys.Contains("ifClose") ? bool.Parse(printAttrs["ifClose"]) : false; ;
            bool excelShow = printAttrs.Keys.Contains("excelShow") ? bool.Parse(printAttrs["excelShow"]) : false; ;
            string sqlStr = translateSQLData(sqlData["dataSql"].ToString(), staticData);
            int currPage = 1;
            string ConnStr = null;
            if (sqlData.Keys.Contains("connName"))
                ConnStr = Constants.getConstsByName(sqlData["connName"]);
            else
                ConnStr = Constants.ConnString;
            if(ConnStr==null)
                ConnStr = Constants.ConnString;
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, ConnStr);
            if (dt.Rows.Count == 0)
            {
                rtnStr = sqlStr + " 無資料";
                return false;
            }
            if (excel == null)
                excel = new ExcelLib();
            dt = CommonClass.insertEmptyRow(dt, "size");
            ExcelElements ee = excel.openExcel(templateFile, true);
            if(printAttrs.Keys.Contains("excelShow"))
            ee.oXL.Visible = excelShow;
            //string formateJsonStr = Constants.getProperty("ROFeed格式", "{ 'PageMode':'1','PageRows':'41','header':'1:41','pageDataRows':'20','dataRowStart':'5','columnEnd':'N',  'datePos': 'M3'}");
            //Dictionary<string, string> HeaderAndFooter = JsonConvert.DeserializeObject<Dictionary<string, string>>(formateJsonStr);
            foreach (KeyValuePair<string, string> layoutItem in excelLayout)
            {
                if(layoutItem.Value.Substring(0,1)=="$" ) //從staticData 提取資料
                {
                    tempKey = layoutItem.Value.Substring(1);
                    if (staticData.Keys.Contains(tempKey))
                        tempValue = staticData[tempKey];
                    else
                        continue;
                } else if (layoutItem.Value == "#Date")
                {
                    tempValue = "'" + DateTime.Now.ToString("yyyy/MM/dd");
                } else if (layoutItem.Value == "#DateTime")
                    tempValue = "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                else
                    tempValue = layoutItem.Value;
                excel.fillExcelCellValue(ee.mWorkSheets["Template"], layoutItem.Key, tempValue);
            }
            
            currPage = excel.report_dtPageMode(dt, ee, printAttrs, currPage, currPage > 1 ? false : true);
            currPage = currPage + 1;
            ee.mWSheet = ee.mWorkSheets["報表"];
            string destPrinter = "Epson EPL-6200 (左下)";
            if (ifPrint)
            {
                //使用 property 共用的印表機
                destPrinter = printAttrs["ReportPrinter"];

                if (printAttrs.ContainsKey("printer"))
                {
                    //使用 本屬性 自用的印表機
                    destPrinter = printAttrs["printer"];
                }
                excel.printExcelBySpecifyPrinter(ee, destPrinter);
            }
            if(ifClose) 
                excel.closeExcel(ee, outputFileName, false);
            if(sqlData.ContainsKey("afterPrintSql"))
            {
                sqlStr = sqlData["afterPrintSql"];
                CommonClass.execSQLNonQuery(sqlStr, Constants.WarehouseConnString);
            }

            rtnStr = printAttrs["ReportName"] + " 列印完成";

            return true;
        }
        public static bool printStdReportFromProperty(Dictionary<string, string> receiveDict, string propertyName, out string rtnStr)
        {
            Dictionary<string, string> stdLblDict = parseStandardLableProperty(propertyName);
            foreach(KeyValuePair<string, string> receiveItem in receiveDict)
            {
                if (stdLblDict.ContainsKey(receiveItem.Key))
                    stdLblDict[receiveItem.Key] = receiveItem.Value;
                else
                    stdLblDict.Add(receiveItem.Key, receiveItem.Value);
            }
            return printStdReport(stdLblDict, out rtnStr);
        }
        #endregion
        #endregion
        public static string writeToExcelFromDB(string sourcePath, string dataPath)
        {
            //string dataPath = @"M:\Absence\";
            ExcelLib excelLib = new ExcelLib();
            ExcelElements excel2 = null;
            Dictionary<string, string> configData = new Dictionary<string, string>();
            //string dataPath = Constants.sourcePath;
            string destExcelFile = dataPath + @"\請假申請記錄" + DateTime.Now.Year.ToString() + @".xlsx";
            try
            {
                if (!CommonClass.ifFileExists(sourcePath))
                    CommonClass.copyFile(sourcePath + @"\請假申請記錄(母版).xlsx", destExcelFile);
                else
                    return "查無 母板 檔案： " + sourcePath + @"\請假申請記錄(母版).xlsx  !!!";
                excel2 = excelLib.openExcel(destExcelFile);

                Microsoft.Office.Interop.Excel.Worksheet motherSheet = excelLib.openExcelSheet(excel2, "母版");
                string processDate = null, last_apply_id = "-1";

                configData = excelLib.getConfigInSheet(excel2, "參數設定");
                if (configData.ContainsKey("last id_apply") && !string.IsNullOrEmpty(configData["last id_apply"]))
                {
                    last_apply_id = configData["last id_apply"];
                }
                else
                {
                    last_apply_id = "-1";
                }

                //Append Data To Excel
                string sqlStr = "SELECT a.id_apply, b.name 姓名, c.department 部門, d.name 代理人, e.department 代理人部門, " +
                                " `start_date` 開始時間, `end_date` 結束時間, `leaveType` 假別, `cause` 請假事由, a.issue_time 核准時間, " +
                                " Concat('file://192.168.1.22/portal/Absence/',DATE_FORMAT(a.`issue_time`, '%Y%m%d-%H-%i-%s-'),Right(c.ext,3),  '.pdf') as pdf檔 " +
                                " FROM `xyz_uj_leave` a " +
                                " Left Join xyz_users b on a.`user_id` = b.id " +
                                " Left Join xyz_hikashop_user c on a.user_id = c.user_cms_id " +
                                " Left Join xyz_users d on a.`deputy` = d.id " +
                                " Left Join xyz_hikashop_user e on a.deputy = e.user_cms_id ";
                //" Left Join xyz_uj_approval_check f on a.`id_apply` = f.`id_apply` and a.`form_type_id`=f.`form_type_id` and f.`ifagree`=1 ";
                sqlStr += " Where a.id_apply > '" + last_apply_id + "' And final_result= 1 AND leave_delete_status = 0 Order by a.`start_date` ";

                DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.portalConnString);

                string jsonDataToExcel = null;
                if (dt.Rows.Count > 0)
                {
                    jsonDataToExcel = DynamicJsonConverter.DtToJson(dt);
                }
                else
                {
                    CommonClass.writeLog("firstohmService", "writeToExcelFromDB", "查無需寫入資料!!!");
                    return "查無需寫入資料!!!";
                }

                excelLib.appendJsonRowsToExcel(excel2, jsonDataToExcel, "開始時間", 11, 0, 7);
                excel2 = excelLib.openExcelSheet2(excel2, "參數設定");
                configData["last id_apply"] = dt.Rows[dt.Rows.Count - 1]["id_apply"].ToString();
                configData["last issue_date"] = dt.Rows[dt.Rows.Count - 1]["核准時間"].ToString();
                //configData = CommonClass.drToDict(dt, dt.Rows.Count-1);
                excelLib.setConfigInSheet(configData, excel2);
                //-----------------------------------------------------------------------------------------
                //Delete Data From Excel
                sqlStr = "SELECT `id_apply`, substring(`start_date`,1,7) as sheetName, " +
                        " Concat('file://192.168.1.22/portal/Absence/',DATE_FORMAT(a.`issue_time`, '%Y%m%d-%H-%i-%s-'),Right(c.ext,3),  '.pdf') as pdf檔 " +
                        " FROM `xyz_uj_leave` a " +
                        " Left Join xyz_hikashop_user c on a.user_id = c.user_cms_id " +
                        " WHERE a.`leave_delete_status`= 1 AND a.id_apply > '" + last_apply_id + "'";
                dt = CommonClass.getSQLDataTable(sqlStr, Constants.portalConnString);
                string sheetName = null;
                foreach (DataRow myRow in dt.Rows)
                {
                    sheetName = myRow["sheetName"].ToString();
                    excelLib.deleteRowsFromExcel(excel2, myRow, sheetName);
                }
                CommonClass.writeLog("firstohmService", "writeToExcelFromDB", "寫入 " + destExcelFile + " 完成!!!");
            }
            finally
            {
                if (excel2 != null)
                {
                    //closeExcel(ExcelElements excelEE, string saveAsPath = null, bool ifSave = true)
                    excelLib.closeExcel(excel2, null, true);
                    destExcelFile = dataPath + "請假申請記錄" + DateTime.Now.Year.ToString() + @".xlsx";
                }
            }
            CommonClass.copyFile(sourcePath + "請假申請記錄" + DateTime.Now.Year.ToString() + @".xlsx", destExcelFile);
            return "已寫入檔案: " + destExcelFile + "請假申請記錄" + DateTime.Now.Year.ToString() + @".xlsx";
        }

        #region 工作日報表
        public static DataTable parseList(DataTable dt, string parseColName)
        {
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    if (dr[parseColName] == DBNull.Value)
                    {
                        dr["合格"] = "V";
                    }
                    else if (dr[parseColName].ToString() == "" || dr[parseColName].ToString() == "[]")
                    {
                        dr["合格"] = "V";
                    }
                    else
                    {
                        //string[] material_issue = dr[parseColName].ToString().Split(',');
                        List<string> material_issue = JsonConvert.DeserializeObject<List<string>>(dr[parseColName].ToString());
                        foreach (string itemStr in material_issue)
                        {
                            dr[itemStr] = "V";
                        }
                    }
                } catch(Exception ex)
                {
                    //continue
                }

            }
            return dt;
        }

        public static DataTable parseDict(DataTable dt, string parseColName)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[parseColName] == DBNull.Value)
                {
                    //do Nothing
                }
                else if (dr[parseColName].ToString() == "" || dr[parseColName].ToString() == "[]")
                {
                    //do Nothing
                }
                else
                {
                    //string[] material_issue = dr[parseColName].ToString().Split(',');
                    Dictionary<string, string> material_issue = JsonConvert.DeserializeObject<Dictionary<string, string>>(dr[parseColName].ToString());
                    foreach (KeyValuePair<string, string> itemStr in material_issue)
                    {
                        dr[itemStr.Key] = itemStr.Value;
                    }
                }
            }
            return dt;
        }

        private static string genSizeStr(DataTable sourceDT)
        {
            DataTable tempDT = CommonClass.DataTableGroupBy(sourceDT, "size", "size");
            bool bingo = false;
            string otherStr = "";
            List<String> rtnList = new List<String>() { "0.8×1.99", "1×3.15", "1.7×5.4", "2×5.2" };
            foreach (DataRow dr in tempDT.Rows)
            {
                bingo = false;
                switch (dr["size"].ToString())
                {
                    case "0.8x1.9":
                        rtnList[0] = "■" + rtnList[0];
                        break;
                    case "1x3.15":
                        rtnList[1] = "■" + rtnList[1];
                        break;
                    case "1.7x5.4":
                        rtnList[2] = "■" + rtnList[2];
                        break;
                    case "2x5.2":
                        rtnList[3] = "■" + rtnList[3];
                        break;
                    default:
                        otherStr += "," + dr["size"].ToString();
                        break;
                }
            }
            for (int i = 0; i < rtnList.Count; i++)
            {
                if (rtnList[i][0] != '■')
                    rtnList[i] = "□" + rtnList[i];
            }
            if (!string.IsNullOrEmpty(otherStr))
            {
                rtnList.Add("■其它" + otherStr.Substring(1));
            }
            else
                rtnList.Add("□其它");
            return string.Join(" ", rtnList);
        }

        public static DataTable mergeDT(DataTable dt, string mergeSQL, List<string> mergeKey)
        {
            DataRow[] tempDrArr;
            List<DataRow> rtnDataRow = new List<DataRow>();
            string filterCondition = null;
            string recCondition;
            List<string> updateCols = new List<string>();
            DataTable secondDB = null;
            for (int i = 0; i < mergeKey.Count; i++)
            {
                filterCondition = mergeKey[0] + "='{" + i + "}'";
                if (i < mergeKey.Count - 1)
                {
                    filterCondition += " and ";
                }
            }
            if (filterCondition == null)
                return dt;

            foreach (DataRow dr in dt.Rows)
            {
                secondDB = CommonClass.getSQLDataTable(string.Format(mergeSQL, dr["SUBFLOWID"].ToString()));
                if (updateCols.Count <= 0)
                {
                    foreach (DataColumn dc in secondDB.Columns)
                    {
                        if (!mergeKey.Contains(dc.ColumnName))
                            updateCols.Add(dc.ColumnName);
                    }
                }

                for (int i = 0; i < mergeKey.Count; i++)
                    filterCondition = filterCondition.Replace("{" + i + "}", dr[mergeKey[0]].ToString());
                tempDrArr = CommonClass.searchInDT(secondDB, filterCondition);
                if (tempDrArr.Length == 0)
                { //沒有 NG 記錄 
                    rtnDataRow.Add(CommonClass.cloneDataRow(dr, dt));
                    continue;
                }
                for (int i = 0; i < tempDrArr.Length; i++)
                {
                    DataRow drTemp;
                    if (i > 0)
                        drTemp = dt.NewRow(); //Create New Row
                    else
                        drTemp = CommonClass.cloneDataRow(dr, dt);
                    for (int j = 0; j < updateCols.Count; j++)
                    {
                        drTemp[updateCols[j]] = tempDrArr[i][j + 1].ToString();
                    }
                    rtnDataRow.Add(drTemp);
                }
            }
            if (rtnDataRow.Count > 0)
            {
                dt.Rows.Clear();
                foreach (DataRow dr in rtnDataRow)
                    dt.Rows.Add(dr);
            }
            return dt;
        }

        public static String getRealLoginDate(string shiftName, DateTime workDay)
        {
            if (shiftName == "晚班")
                return workDay.AddDays(1).ToString("yyyy-MM-dd");
            else
                return workDay.ToString("yyyy-MM-dd");
        }

        // □早班__________ □中班__________ □晚班___________
        private static string genShiftAndUser(string shift, string userName, string loginTime, string logoutTime, DateTime workDay )
        {
            List<DateTime> login_out;
            DateTime loginT, logoutT;
            string[] shifTime = FirstohmPrds.getSimpleShifTime(shift);
            if (!DateTime.TryParse(loginTime, out loginT) || !DateTime.TryParse(logoutTime, out logoutT))
            {
                login_out = FirstohmPrds.getShifTime(shift, workDay);
                loginT = login_out[0];
                logoutT = login_out[1];
            }
            string timeSpan = (CommonClass.roundDown((((logoutT - loginT).TotalHours)), 0) + (((logoutT - loginT).TotalHours * 10 % 10) >= 5 ? 0.5 : 0)).ToString();
            string rtnStr = "";
            switch (shift)
            {
                case "早班":
                    rtnStr = "■早班" + timeSpan + "H " + userName + " □中班_______ □晚班_______";
                    break;
                case "中班":
                    rtnStr = "□早班_______ ■中班" + timeSpan + "H " + userName + " □晚班_______";
                    break;
                case "晚班":
                    rtnStr = "□早班_______ □中班_______ ■晚班" + timeSpan + "H " + userName;
                    break;
            }
            return rtnStr;
        }

        //計算報表下方 個別 尺寸/Toler 統計資料
        private static string generralSlotComment(DataTable dt, string flowStep, string empID, String reportDate,
            List<string> sizeList = null, List<double> templateTolSlot = null)
        {
            StringBuilder rtnStr = new StringBuilder("");
            string currSize = null;
            double currTol = 0;
            double currQuant = 0;
            double currNGQuant = 0;
            double totoalCnt = 0;
            SortedDictionary<string, Dictionary<double, double>> sizeTolDic = new SortedDictionary<string, Dictionary<double, double>>();
            //Step 1, 把要獨立出來的 項目 放入 Dictionary
            if (sizeList != null)
            {
                foreach (string sizeItem in sizeList)
                {
                    sizeTolDic.Add(sizeItem, new Dictionary<double, double>());
                    if (dt.Columns.Contains("NGquant"))
                        sizeTolDic.Add("NG" + sizeItem, new Dictionary<double, double>());
                    if(templateTolSlot!=null)
                    {
                        foreach (double tolItem in templateTolSlot)
                        {
                            sizeTolDic[sizeItem].Add(tolItem, 0);
                            if (dt.Columns.Contains("NGquant"))
                                sizeTolDic["NG" + sizeItem].Add(tolItem, 0);
                        }
                    }
                }
            }

            //Step 2, 塞入 Data
            foreach (DataRow dr in dt.Rows)
            {
                currSize = dr["Size"].ToString().Trim();
                if (dr["Tol"] != DBNull.Value && dr["Tol"].ToString()!="")
                {
                    if (CommonClass.IsNumeric(dr["Tol"].ToString()))
                        currTol = Convert.ToDouble(dr["Tol"]);
                    else
                        currTol = FirstohmPrds.getTol(dr["Tol"].ToString());


                }
                if (dr["quant"] != DBNull.Value)
                    currQuant = Convert.ToDouble(dr["quant"]);
                if (dt.Columns.Contains("NGquant") && dr["NGquant"] != DBNull.Value)
                    currNGQuant = Convert.ToDouble(dr["NGquant"]);
                if (sizeTolDic.ContainsKey(currSize))
                {
                    if (sizeTolDic[currSize].ContainsKey(currTol))
                    {   //須分 Tol
                        sizeTolDic[currSize][currTol] += currQuant;
                        if (dt.Columns.Contains("NGquant"))
                            sizeTolDic["NG" + currSize][currTol] += currNGQuant;
                    }
                    else
                    {   //不分 Tol， 都用 -999 當代號
                        if (!sizeTolDic[currSize].ContainsKey(-999))
                            //給定該 Tol（-999）的初值--0
                            sizeTolDic[currSize].Add(-999, 0);

                        sizeTolDic[currSize][-999] += currQuant;
                        if (dt.Columns.Contains("NGquant"))
                        {
                            if (!sizeTolDic["NG" + currSize].ContainsKey(-999))
                                //給定該 Tol（-999）的初值--0
                                sizeTolDic["NG" + currSize].Add(-999, 0);
                            sizeTolDic["NG" + currSize][-999] += currNGQuant;
                        }
                    }
                }
                //不在 sizeList 就不用計算
                //else
                //{
                //    sizeTolDic.Add(currSize, new Dictionary<double, double>() { { -999, currQuant } });
                //    if (dt.Columns.Contains("NGquant"))
                //        sizeTolDic.Add("NG" + currSize, new Dictionary<double, double>() { { -999, currNGQuant } });
                //}
            }
            string sqlStr = "Delete from mfo_emp_perform where PERM_DATE = '" + reportDate +
                "' AND EmpID = '" + empID + "' AND Flow_Step='" + flowStep + "' AND PermType='MachineID'";
            CommonClass.execSQLNonQuery(sqlStr);
            double NGVal = 0;
            //Step3, 將資料逐一寫出
            foreach (KeyValuePair<String, Dictionary<double, double>> kv in sizeTolDic)
            {
                if (kv.Key.IndexOf("NG") >= 0)
                    continue;
                foreach (KeyValuePair<double, double> printItem in kv.Value)
                {
                    if (printItem.Value > 0)
                    {
                        if (printItem.Key == -999)
                        {
                            if (dt.Columns.Contains("NGquant"))
                            {
                                NGVal = CommonClass.roundDown(sizeTolDic["NG" + kv.Key][printItem.Key] / 1000, 1);
                                rtnStr.Append(kv.Key + "計" + CommonClass.roundDown(printItem.Value / 1000, 1).ToString("0.#") + "/" + NGVal.ToString("0.#") + "Kpcs  ");
                            }
                            else
                                rtnStr.Append(kv.Key + "計" + CommonClass.roundDown(printItem.Value / 1000, 1).ToString("0.#") + "Kpcs  ");
                        }
                        else
                        {
                            if (dt.Columns.Contains("NGquant"))
                            {
                                NGVal = CommonClass.roundDown(sizeTolDic["NG" + kv.Key][printItem.Key] / 1000, 1);
                                rtnStr.Append(kv.Key + " " + printItem.Key.ToString() + "%計" + CommonClass.roundDown(printItem.Value / 1000, 1).ToString("0.#") + "/" + NGVal.ToString("0.#") + "Kpcs  ");
                            }
                            else
                                rtnStr.Append(kv.Key + " " + printItem.Key.ToString() + "%計" + CommonClass.roundDown(printItem.Value / 1000, 1).ToString("0.#") + "Kpcs  ");
                        }
                        insertToEmpPerform(reportDate, flowStep, empID, kv.Key, printItem);
                    }
                }
            }
            return rtnStr.ToString();
        }

        private static string waijianGenerralSlotComment(DataTable dt, string flowStep, string empID, String reportDate,
    List<string> sizeList = null, List<double> templateTolSlot = null)
        {
            List<string> rtnStr = new List<string>();
            double okQuant = 0;
            double ngQuant = 0;
            
            foreach(string sizeItem in sizeList)
            {
                if (templateTolSlot != null && templateTolSlot.Count >= 1)
                {
                    foreach (double tolItem in templateTolSlot)
                    {
                        okQuant = CommonClass.DataTableSum(dt, "quant", "size='" + sizeItem + "' and Tol=" + tolItem.ToString("0.##"));
                        ngQuant = CommonClass.DataTableSum(dt, "NGquant", "size='" + sizeItem + "' and Tol=" + tolItem.ToString("0.##"));
                        if (okQuant > 0 || ngQuant > 0)
                            rtnStr.Add(sizeItem + "/" + tolItem.ToString("0.##") + ":" + (okQuant / 1000).ToString("0.#") + "/" + (okQuant / 1000).ToString("0.#"));
                    }
                } else
                {
                    okQuant = CommonClass.DataTableSum(dt, "quant", "size='" + sizeItem + "'");
                    ngQuant = CommonClass.DataTableSum(dt, "NGquant", "size='" + sizeItem + "'");
                    if (okQuant > 0 || ngQuant > 0)
                        rtnStr.Add(sizeItem + ":" + (okQuant/1000).ToString("0.#") + "/" + (ngQuant / 1000).ToString("0.#"));
                }
                
                
            }
            if (rtnStr.Count > 0)
                return string.Join(" ; ", rtnStr);
            else
                return "";
        }

        private static string slotCommentMachine(DataTable dt, string reportDate, string flowStep, string empID)
        {
            StringBuilder rtnStr = new StringBuilder("");
            string currMachine = null;
            double currTol = 0;
            double currQuant = 0;
            double totoalCnt = 0;

            //Step 1, 把要獨立出來的 項目 放入 Dictionary
            SortedDictionary<string, Dictionary<double, double>> sizeTolDic = new SortedDictionary<string, Dictionary<double, double>> { { "G", new Dictionary<double, double>() { { 0.1, 0 } } } };

            //Step 2, 塞入 Data
            foreach (DataRow dr in dt.Rows)
            {
                currMachine = dr["MachineID"].ToString();
                if (dr["Tol"] != DBNull.Value)
                    currTol = Convert.ToDouble(dr["Tol"]);
                if (dr["quant"] != DBNull.Value)
                    currQuant = Convert.ToDouble(dr["quant"]);
                if (sizeTolDic.ContainsKey(currMachine))
                {
                    if (sizeTolDic[currMachine].ContainsKey(currTol))
                        sizeTolDic[currMachine][currTol] += currQuant;
                    else
                    {
                        if (!sizeTolDic[currMachine].ContainsKey(-999))
                            sizeTolDic[currMachine].Add(-999, 0);
                        sizeTolDic[currMachine][-999] += currQuant;
                    }
                }
                else
                {
                    //Machine 必須特別記錄 0.1 % 的總數
                    if (currMachine == "G" && currTol == 0.1)
                    {
                        sizeTolDic.Add(currMachine, new Dictionary<double, double>() { { 0.1, currQuant } });
                        sizeTolDic.Add(currMachine, new Dictionary<double, double>() { { -999, 0 } });
                    }
                    else if (currMachine == "G" && currTol != 0.1)
                    {
                        sizeTolDic.Add(currMachine, new Dictionary<double, double>() { { -999, currQuant } });
                        sizeTolDic.Add(currMachine, new Dictionary<double, double>() { { 0.1, 0 } });
                    }
                    else
                        sizeTolDic.Add(currMachine, new Dictionary<double, double>() { { -999, currQuant } });
                }
            }
            string sqlStr = "Delete from mfo_emp_perform where PERM_DATE = '" + reportDate +
                "' AND EmpID = '" + empID + "' AND Flow_Step='" + flowStep + "' AND PermType='MachineID'";
            CommonClass.execSQLNonQuery(sqlStr);
            //Step3, 將資料逐一寫出
            foreach (KeyValuePair<String, Dictionary<double, double>> kv in sizeTolDic)
            {
                foreach (KeyValuePair<double, double> printItem in kv.Value)
                {
                    if (printItem.Value > 0)
                    {
                        if (printItem.Key == -999)
                            rtnStr.Append(kv.Key + "計" + CommonClass.roundDown(printItem.Value / 1000, 1).ToString("0.#") + "Kpcs  ");
                        else
                            rtnStr.Append(kv.Key + " " + printItem.Key.ToString() + "%計" + CommonClass.roundDown(printItem.Value / 1000, 1).ToString("0.#") + "Kpcs  ");
                        insertToEmpPerform(reportDate, flowStep, empID, kv.Key, printItem);
                    }
                }
            }
            return rtnStr.ToString();
        }

        private static void insertToEmpPerform(string reportDate, string flowStep, string empID, string permType, KeyValuePair<double, double> printItem)
        {
            Dictionary<string, string> sqlParams = new Dictionary<string, string>();
            sqlParams.Clear();
            sqlParams.Add("@PERM_DATE", reportDate);
            sqlParams.Add("@EmpID", empID);
            sqlParams.Add("@Flow_Step", flowStep);
            sqlParams.Add("@PermType", "Size");
            sqlParams.Add("@PermContent", permType);
            sqlParams.Add("@Tol", printItem.Key.ToString("0.##"));
            sqlParams.Add("@Quant", printItem.Value.ToString("0.##"));
            string sqlStr = "Insert into  mfo_emp_perform (PERM_DATE,EmpID,Flow_Step,PermType,PermContent,Tol,Quant)" +
                     " Values(@PERM_DATE, @EmpID, @Flow_Step, @PermType, @PermContent, @Tol, @Quant)";
            CommonClass.execSQLNonQueryParams(sqlStr, sqlParams);
        }
        private static StdExcelReport commonReport(string rptType, string UserSQL, string DgvSQL, string ExcelSQL, string WinFormTitle, string exeFunction,
            List<string> removeDbCol, List<string> sizeList = null, List<double> tolSlot = null, string extraComment = null,
            string mergeSQL = null, List<string> mergeKey = null)
        {
            rptType = (rptType == "底漆2" ? "底漆" : rptType);
            string formateJsonStr = Constants.getProperty(rptType + "格式", "{'header':'1:7','footer': '10:10','pageDataRows':'31','dataRowStart':'8','columnEnd':'N',  'sizePos': 'A3',  'datePos': 'K3',  'empShift': 'A4'}");
            Dictionary<string, string> HeaderAndFooter = JsonConvert.DeserializeObject<Dictionary<string, string>>(formateJsonStr);
            StdExcelReport stdReport = new StdExcelReport(rptType + "報表", HeaderAndFooter,
               UserSQL, DgvSQL, ExcelSQL, WinFormTitle, exeFunction, removeDbCol);
            if (sizeList != null)
            {
                stdReport.sizeList = sizeList;
                stdReport.tolSlot = tolSlot;
            }

            if (!string.IsNullOrEmpty(mergeSQL))
            {
                stdReport.mergeSQL = mergeSQL;
                stdReport.mergeKey = mergeKey;
            }
            stdReport.extraComment = extraComment;
            return stdReport;
        }

        private static string genUserSql(string stepFlow)
        {
            string UserSQL = "SELECT USER_ID 員工編號, b.EMPNAME 姓名 , c.`Llogin` 登入時間 , c.`Llogout` 登出時間 " +
             " FROM mfo_sign a " +
             " Left Join mfo_employee b on a.USER_ID=b.EMPID " +
             " Left Join mfo_loginout c on Date(`Llogin`)= '{0}' and a.USER_ID = c.`EMPID` " +
             " WHERE FLOW_STEP like '%" + stepFlow + "%' and OutputQuan > 0 {1} " +
             " group by USER_ID having sum(`OutputQuan`) > 0 ";
            return UserSQL;
        }

        public static StdExcelReport initDailyReport(string flowStep)
        {
            string exeFunction = null;

            string WinFormTitle = flowStep;
            if (Constants.getProperty((flowStep == "底漆2" ? "底漆" : flowStep) + "exeFunction", "0") == "1")
                exeFunction = "parseLise"; //除了 SQL 以外，資料的額外來源(Json List)， default : null Do nothing
            else if (Constants.getProperty((flowStep == "底漆2" ? "底漆" : flowStep) + "exeFunction", "0") == "2")
                exeFunction = "parseDict"; //除了 SQL 以外，資料的額外來源(Json Dictionary)， default : null Do nothing
            string UserSQL = genUserSql(flowStep);

            String DgvSQL = null;
            
            String ExcelSQL = Constants.getProperty((flowStep=="底漆2"?"底漆": flowStep) + "ExcelSQL");
            string tempProperty = Constants.getProperty((flowStep == "底漆2" ? "底漆" : flowStep) + "removeDBCol", "size, subFlowID, User_ID, EMPID, material_issue, LoginTime, LogOutTime, SUBFLOWID,L1, L2, cuid, bempid, auid, dempid, L1, L2");
            List<string> removeDBCol = null;
            List<string> sizeList = null;
            List<double> tolSlot = null;
            removeDBCol = tempProperty.Split(',').ToList();
            tempProperty = Constants.getProperty((flowStep == "底漆2" ? "底漆" : flowStep) + "sizeList", "1.7x5.4, 1x3.15, 1.7x5.2");
            sizeList = tempProperty.Split(',').ToList();
            tempProperty = Constants.getProperty((flowStep == "底漆2" ? "底漆" : flowStep) + "tolSlot", "1, 0.5, 0.25, 0.1");
            foreach (string lotItem in tempProperty.Split(',').ToList())
            {
                if (tolSlot == null)
                    tolSlot = new List<double>();
                if (!string.IsNullOrEmpty(lotItem))
                {
                    tolSlot.Add(double.Parse(lotItem));
                }
            }
            //註解行的儲存格位置
            string extraComment = Constants.getProperty((flowStep == "底漆2" ? "底漆" : flowStep) + "Comment", "A36"); 
            return commonReport(flowStep, UserSQL, DgvSQL, ExcelSQL, WinFormTitle, exeFunction, removeDBCol, sizeList, tolSlot, extraComment);
        }

        public  static void setOfficialLoginout(string shifName, DateTime workDate, string empID)
        {
            int PreOfficialMins = int.Parse(Constants.getProperty("PreOfficialMins", "30"));
            int PostOfficialMins = int.Parse(Constants.getProperty("PostOfficialMins", "30"));
            string shiftCondition = FirstohmPrds.getSfitTimeCondition(shifName, workDate, " between", PreOfficialMins, PostOfficialMins);
            //string sqlStr = "delete FROM `mfo_loginout` where TIMESTAMPDIFF(HOUR, `LoginTime`,`LogOutTime`) > 16 and ";
            //CommonClass.execSQLNonQuery(sqlStr);
            string sqlStr = $"SELECT ID, `EMPID`, LoginTime as realLogin, (`LoginTime`) LoginTime, (`LogOutTime`) LogOutTime " +
                            $" FROM `mfo_loginout` " +
                            $" where LoginTime {shiftCondition} " +
                            $" and `EMPID`='{empID}' order by LogOutTime desc";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            
            if (dt.Rows.Count == 0)
                return;
            //string logTimeID = dt.Rows[0]["ID"].ToString();
            string logTimeID = Math.Round(CommonClass.DataTableMin(dt,"ID", "1=1")).ToString();
            DataTable sortedDT = CommonClass.DataTableFilterSort1(dt, null, "LoginTime");
            DateTime loginTime = Convert.ToDateTime(sortedDT.Rows[0]["LoginTime"]);
            DateTime logoutTime=DateTime.Now;
            foreach(DataRow dr in dt.Rows)
            {
                logoutTime = Convert.ToDateTime(dr["LogOutTime"]);
                if ((logoutTime - loginTime).TotalHours < 16)
                {
                    break;
                }
            }

            double hourDiff = (logoutTime - loginTime).TotalHours;
            if(hourDiff > 16 )
            {
                List<DateTime> shiftTime = FirstohmPrds.getShifTime(shifName, workDate);
                if (Math.Abs((shiftTime[0] - loginTime).TotalHours) > 3)
                    loginTime = shiftTime[0];
                if (Math.Abs((shiftTime[1] - logoutTime).TotalHours) > 3)
                    logoutTime = shiftTime[0];
            }
            sqlStr = $"update  `mfo_loginout` set `Llogin`='0000-00-00', `Llogout`='0000-00-00',byWho='{empID}' " +
                     $" Where LoginTime {shiftCondition} and EMPID='{empID}'";
            CommonClass.execSQLNonQuery(sqlStr);
            sqlStr = $"Update mfo_loginout " +
            $" Set Llogout = '{logoutTime.ToString("yyyy-MM-dd HH:mm:ss")}', Llogin = '{loginTime.ToString("yyyy-MM-dd HH:mm:ss")}', byWho='{empID}', " +
            $" performDate='{workDate.ToString("yyyy-MM-dd")}', shiftType='{shifName}' " +
            $" Where ID={logTimeID}";
            CommonClass.execSQLNonQuery(sqlStr);
        }

        private static ExcelElements getReportExcel(string txtEmpID, string reportType, out string templateFile)
        {
            ExcelLib excel = new ExcelLib();
            string templateFolder = Constants.getProperty("templateFolder", @"C:\排單資料\標準\Report");
            templateFile = Constants.getProperty(reportType, "A-085切割生產日報與自主檢查表.xlsx");

            //string outputFileName = null; //No output
            int reportPage = 0, currPage = 1;
            if (excel == null)
                excel = new ExcelLib();
            List<String> chkedUser = new List<string>() { txtEmpID };
            ExcelElements ee = excel.openExcel(templateFolder + @"\" + templateFile);
            ee.oXL.Visible = true;
            return ee;
        }

        public static string empidToEmpName(string empID)
        {
            string sqlStr = "SELECT `DEPT`,`EMPNAME` ,`ifLeader`,`ifAvailable` FROM `mfo_employee` where `EMPID`='" + empID + "'";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            if (dt.Rows.Count == 0)
                return null;
            else
                return dt.Rows[0]["EMPNAME"].ToString();
        }

        //public static int printDailyReport(string flow_step, string txtEmpID,
        //    string shiftName, DateTime workDay, out string rtnStr, int retryCnt = 0)
        //{
        //    ExcelLib excel = new ExcelLib();
        //    ExcelElements ee = null;
        //    StdExcelReport stdReport = initDailyReport(flow_step);
        //    string empName = null;
        //    rtnStr = null;
        //    if (string.IsNullOrEmpty(flow_step))
        //    {
        //        rtnStr = "請選取要列印的工作日報";
        //        return retryCnt + 1;
        //    }

        //    int reportPage = 0, currPage = 1;
        //    List<String> chkedUser = new List<string>() { txtEmpID };
        //    string templateFile = null;
            
        //    //--------------------------------------------------------
        //    //寫入 Header User Data
        //    DataTable dt;
        //    int excelStartRow = 1, excelEndRow = 0;
        //    if (stdReport.headerAndFooter.ContainsKey("PageMode") && !stdReport.headerAndFooter.ContainsKey("PageRows"))
        //    {
        //        MessageBox.Show("設定檔必須有 PageRows 參數 ", "列印報表", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        rtnStr = "設定檔必須有 PageRows 參數， 請確認";
        //        return retryCnt + 1;
        //    }


        //    int sizeRow = Convert.ToInt16(stdReport.headerAndFooter["sizePos"].Substring(1, 1));
        //    int dateRow = Convert.ToInt16(stdReport.headerAndFooter["datePos"].Substring(1, 1));
        //    int empShiftRow = Convert.ToInt16(stdReport.headerAndFooter["empShift"].Substring(1, 1));
        //    string sizeCol = stdReport.headerAndFooter["sizePos"].Substring(0, 1); //A
        //    string dateCol = stdReport.headerAndFooter["datePos"].Substring(0, 1);//K
        //    string empShiftCol = stdReport.headerAndFooter["empShift"].Substring(0, 1); //A
        //    //{0} : UserID
        //    //{1} : llogin between 'xxxx' and 'xxxx'
        //    //{2} : performDate
        //    if (stdReport.reportType.IndexOf("切割") >= 0)
        //    {
        //        stdReport.specialFooterSQL = "SELECT `MachineID`, Round(SUM(time_to_sec(timediff(`Finish_Time`, `Start_TIME`)))/3600,1) machineTime " +
        //                    " FROM `mfo_sign` where USER_ID='{0}'  {1} and OutputQuan>0 " +
        //                    " group by `MachineID` order by machineTime";
        //    }

        //    //每個 作業員
        //    foreach (string srtSelUsers in chkedUser)
        //    {
        //        //讀取 ExcelSql
        //        String sqlStr;
        //        //String.Format(stdReport.excelSQL, dtp1.Value.AddDays(1).ToString("yyyy-MM-dd"), srtSelUsers);
        //        int PreOfficialMins = int.Parse(Constants.getProperty("PreOfficialMins", "30"));
        //        int PostOfficialMins = int.Parse(Constants.getProperty("PostOfficialMins", "30"));
        //        string shiftCondition = FirstohmPrds.getSfitTimeCondition(shiftName, workDay, " between", PreOfficialMins, PostOfficialMins);
        //        shiftCondition = $" shiftType='{shiftName}' and performDate='{workDay}'";
        //        if (shiftName == "晚班")
        //            sqlStr = String.Format(stdReport.excelSQL, shiftCondition, srtSelUsers);
        //        else
        //            sqlStr = String.Format(stdReport.excelSQL, shiftCondition, srtSelUsers, shiftName);
        //        //if (flow_step == "底漆2")
        //        //    sqlStr = sqlStr.Replace("\"", "'").Replace(" in ('底漆1', '花蓮底漆')", "='底漆2'");
        //        dt = CommonClass.getSQLDataTable(sqlStr);
        //        string whereStr = CommonClass.getWhereInSQL(sqlStr);
        //        if (dt.Rows.Count == 0)
        //        {
        //            //continue; 派單系統可以印多人， 火箭程式不印多人
        //            rtnStr = "查無資料： " + sqlStr;
        //            return 0;
        //        }
        //        //Copy from Line 1779
        //        if (flow_step.Contains("切割") || flow_step.Contains("加壓") || flow_step.Contains("外檢") || flow_step.Contains("全檢"))
        //            dt = CommonClass.insertEmptyRow(dt, "SIZE");
        //        //底漆、色碼、貼帶
        //        else if (flow_step.Contains("底漆") || flow_step.Contains("色碼") || flow_step.Contains("貼帶"))
        //            dt = CommonClass.insertEmptyRow(dt, "機台別");

        //        ee = getReportExcel(txtEmpID, stdReport.reportType, out templateFile);
        //        ee.mWorkSheets["報表"].PageSetup.PrintArea = "A:" + stdReport.headerAndFooter["columnEnd"];
        //        empName = dt.Rows[0]["EMPNAME"].ToString();
        //        if (dt.Columns.Contains("CoWorker") && dt.Rows[0]["CoWorker"] != DBNull.Value && dt.Rows[0]["CoWorker"].ToString() != "")
        //        {
        //            empName += "," + empidToEmpName(dt.Rows[0]["CoWorker"].ToString());
        //        }
        //        excel.fillExcelCellValue(ee.mWorkSheets["Template"], sizeCol + (excelStartRow + sizeRow - 1), genSizeStr(dt));
        //        excel.fillExcelCellValue(ee.mWorkSheets["Template"], dateCol + (excelStartRow + dateRow - 1), CommonClass.ToFullTaiwanDate(workDay, 2));
        //        excel.fillExcelCellValue(ee.mWorkSheets["Template"], empShiftCol + (excelStartRow + empShiftRow - 1), genShiftAndUser(shiftName, empName, dt.Rows[0]["LoginTime"].ToString(), dt.Rows[0]["LogOutTime"].ToString(), workDay));

        //        if (stdReport.specialFooterSQL != null)
        //        {
        //            List<String> fullMachine = new List<String>(), halfMachine = new List<String>();
        //            string footerSql = string.Format(stdReport.specialFooterSQL, srtSelUsers, FirstohmPrds.getSfitTimeCondition(shiftName, workDay, " and `Finish_Time` between", 0, 60));
        //            DataTable footerDt = CommonClass.getSQLDataTable(footerSql);
        //            foreach (DataRow dr in footerDt.Rows)
        //            {
        //                if (Convert.ToSingle(dr["machineTime"]) >= 6.5)
        //                {
        //                    string mStr = dr["MachineID"].ToString();
        //                    if (!fullMachine.Contains(mStr))
        //                        fullMachine.Add(mStr);
        //                }
        //                else
        //                    halfMachine.Add(dr["MachineID"].ToString() + "(" + dr["machineTime"].ToString() + ")");
        //            }
        //            if (fullMachine.Count > 0)
        //                //excel.fillExcelCellValue(ee.mWorkSheets["Template"], "I40", string.Join(",", fullMachine));
        //                excel.fillExcelCellValue(ee.mWorkSheets["Template"], "D41", fullMachine.Count.ToString());
        //            else
        //                excel.fillExcelCellValue(ee.mWorkSheets["Template"], "D41", "0");
        //            if (halfMachine.Count > 0)
        //                excel.fillExcelCellValue(ee.mWorkSheets["Template"], "F40", string.Join(",", halfMachine));
        //            else
        //                excel.fillExcelCellValue(ee.mWorkSheets["Template"], "F40", "");
        //        }

        //        ////由於外檢的 Excel SQL 特別複雜， 因此其 where 需要小改一下
        //        //if (stdReport.reportType.IndexOf("外檢") >= 0)
        //        //{
        //        //    whereStr = whereStr.Replace("c.", "a.").Replace("底漆", "外檢").Replace("  group by a.user_id   ", "");
        //        //}
        //        //計算統計資料
        //        String addDataSQL = null;
        //        if (stdReport.reportType == "色碼報表")
        //        {
        //            addDataSQL = "SELECT a.Tol , a.MachineID , Sum(OutputQuan) quant, SUBSTRING_INDEX(a.SIZE, 'x', 1) size1 " +
        //                        "FROM view_signInfo a " +
        //                        //"Left Join mfo_loginout c on Date(`LoginTime`)= '" + getRealLoginDate(shiftName, workDay) + "' and a.USER_ID = c.`EMPID` " +
        //                        " Left Join mfo_loginout c on {0}  and EMPID = '{1}' " + 
        //                        whereStr + "  and  `FLOW_STEP` like '%色碼%' " +
        //                        " group by a.MachineID, a.Tol " +
        //                        " order by a.MachineID, a.Tol";
        //        }
        //        else if (stdReport.reportType.IndexOf("外檢") >= 0)
        //        {
        //            string reportType = stdReport.reportType.IndexOf("外檢底漆") >= 0 ? "外檢1" : "外檢2";
        //            addDataSQL = "SELECT a.Tol , sizeStandard(a.SIZE) SIZE, Sum(OutputQuan) quant,  Sum(a.DefectQuan) NGquant, sizeToNo(a.SIZE) size1 " +
        //                        " FROM view_signInfo a " +
        //                        //" Left Join mfo_loginout c on Date(`LoginTime`)= '" + getRealLoginDate(shiftName, workDay) + "' and a.USER_ID = c.`EMPID` " + 
        //                        " Left Join mfo_loginout c on {0} and EMPID = '{1}' " +
        //                        "  where `FLOW_STEP` like '%" + reportType  + "%' and a.signFinish between c.Llogin and c.LlogOut " +
        //                        " and a.USER_ID='{1}'" +
        //                        " group by sizeStandard(SIZE), Tol order by SIZE1, sizeStandard(Size), Tol";
        //        }
        //        else if (stdReport.reportType.IndexOf("貼帶") >= 0)
        //        {
        //            //沿用大家都 Column Name
        //            //其中 Size 其實是 貼帶的 MachineID, Size1 則是 MichineID 的第一碼
        //            addDataSQL = "SELECT '' Tol , SUBSTRING(`MachineID`, 1, position('-' in MachineID)-1) SIZE , Sum(OutputQuan) quant, SUBSTRING(`MachineID`, 1, position('-' in MachineID)-1) size1 " +
        //                        "FROM view_signInfo a left Join view_subflowinfo d on a.SUBFLOWID = d.SUBFLOWID " +
        //                        //" Left Join mfo_loginout c on Date(`LoginTime`)= '" +  getRealLoginDate(shiftName, workDay) + "' and a.USER_ID = c.`EMPID` " +
        //                        " Left Join mfo_loginout c on {0}  and EMPID = '{1}' " +
        //                        whereStr + "  and `FLOW_STEP` like '%貼帶%' " +
        //                        " group by a.SIZE order by a.Size ";
        //        }
        //        else if (stdReport.reportType.IndexOf("底漆") >= 0)
        //        {
        //            addDataSQL = "SELECT '' Tol , SUBSTRING(`MachineID`, 1, position('-' in MachineID)-1) SIZE , Sum(OutputQuan) quant, SUBSTRING(`MachineID`, 1, position('-' in MachineID)-1) size1 " +
        //                        "FROM view_signInfo a left Join view_subflowinfo d on a.SUBFLOWID = d.SUBFLOWID " +
        //                        //" Left Join mfo_loginout c on Date(`LoginTime`)= '" +  getRealLoginDate(shiftName, workDay) + "' and a.USER_ID = c.`EMPID` " +
        //                        " Left Join mfo_loginout c on {0}  and EMPID = '{1}' " +
        //                        whereStr + "  and `FLOW_STEP` like '%底漆%' " +
        //                        " group by a.SIZE order by a.Size ";
        //        }
        //        else if (stdReport.reportType.IndexOf("全檢") >= 0)
        //        {
        //            addDataSQL = "";
        //        }
        //        else
        //        addDataSQL = "SELECT Tol , sizeStandard(SIZE) SIZE , Sum(OutputQuan) quant, sizeToNo(SIZE) size1 " +
        //            "FROM view_signInfo a  Left Join mfo_loginout c on {0}  and EMPID = '{1}' " +
        //            whereStr + " group by sizeStandard(SIZE), Tol order by SIZE1, sizeStandard(Size), Tol";
        //        addDataSQL = String.Format(addDataSQL, shiftCondition, txtEmpID);
        //        DataTable tempDt = null;
        //        if (!string.IsNullOrEmpty(addDataSQL))
        //            tempDt = CommonClass.getSQLDataTable(addDataSQL);
        //        string tempStr = null;
        //        if (stdReport.reportType == "色碼報表")
        //            tempStr = slotCommentMachine(tempDt, workDay.ToString("yyyy-MM-dd"), flow_step, txtEmpID);
        //        else if (stdReport.reportType == "底漆報表")
        //        {
        //            //底漆統計要算兩次
        //            //'0.8x1.9' and a.Vals<=50， 尺寸:2.5x8以上(含2.5x8),  阻值: 1E以下, 阻值範圍:1 %
        //            // 符合以上條件的底漆要先塗再切
        //            addDataSQL = "SELECT Tol , sizeStandard(SIZE) SIZE , Sum(OutputQuan) quant, sizeToNo(SIZE) size1 " +
        //                         " FROM view_signInfo_diqiflag  a  Left Join mfo_loginout c on Date(`LoginTime`)= '" +
        //                          getRealLoginDate(shiftName, workDay) + "' and a.USER_ID = c.`EMPID` AND flow_step like '%底漆%' " +
        //                          " and ((sizeStandard(a.size)='0.8x1.9' and a.Vals<=50 ) or (sizeToNo(SIZE) >= 2.5 and Vals < 1 and Tol=1)) " +
        //                          whereStr + " and  (diqiFlag= 1)  group by sizeStandard(SIZE), Tol order by SIZE1, sizeStandard(Size), Tol";
        //            tempDt = CommonClass.getSQLDataTable(addDataSQL);
        //            List<string> diqiSize = new List<string>() { "0.8x1.9", "2.5x8", "3.5x10" };
        //            if (tempDt.Rows.Count > 0)
        //                tempStr += "(1)" + generralSlotComment(tempDt, flow_step, srtSelUsers, workDay.ToString("yyyy-MM-dd"), diqiSize, null);
        //            addDataSQL = "SELECT Tol , sizeStandard(SIZE) SIZE , Sum(OutputQuan) quant, sizeToNo(SIZE) size1 " +
        //                         " FROM view_signInfo_diqiflag  a  Left Join mfo_loginout c on " +
        //                         $" shiftType='{shiftName}' and performDate='{workDay}'" +
        //                          " and a.USER_ID = c.`EMPID` AND flow_step like '%底漆%' " +
        //                          whereStr + " and (diqiflag=2 or diqiflag='')  group by sizeStandard(SIZE), Tol order by SIZE1, sizeStandard(Size), Tol";
        //            tempDt = CommonClass.getSQLDataTable(addDataSQL);
        //            if (tempDt.Rows.Count > 0)
        //            {
        //                string diqi2Str = generralSlotComment(tempDt, flow_step, srtSelUsers, workDay.ToString("yyyy-MM-dd"), stdReport.sizeList, null);
        //                if (!string.IsNullOrEmpty(tempStr))
        //                {
        //                    tempStr += "  (2)" + diqi2Str;
        //                }
        //                else
        //                {
        //                    tempStr = "(2)" + diqi2Str;
        //                }
        //            }
        //        }
        //        else if (stdReport.reportType == "全檢報表")
        //        {
        //            addDataSQL = "SELECT Tol , sizeStandard(SIZE) SIZE, Sum(OutputQuan) quant, sizeToNo(SIZE) size1 " +
        //                         " FROM view_signInfo a  Left Join mfo_loginout c on Date(`LoginTime`)= '" +
        //                          getRealLoginDate(shiftName, workDay) + "' and a.USER_ID = c.`EMPID` AND (flow_step like '%全檢1%' or flow_step like '%全檢分類%') " +
        //                          whereStr + " group by sizeStandard(SIZE), Tol order by SIZE1, sizeStandard(Size), Tol";
        //            tempDt = CommonClass.getSQLDataTable(addDataSQL);
        //            tempStr = generralSlotComment(tempDt, flow_step, srtSelUsers, workDay.ToString("yyyy-MM-dd"), stdReport.sizeList, stdReport.tolSlot);
        //            if (!string.IsNullOrEmpty(tempStr))
        //                tempStr = "(1)" + tempStr;
        //            addDataSQL = "SELECT Tol , sizeStandard(SIZE) SIZE, Sum(OutputQuan) quant, sizeToNo(SIZE) size1 " +
        //                         " FROM view_signInfo a  " +
        //                            " Left Join mfo_loginout c on Date(`LoginTime`)= '" +
        //                          getRealLoginDate(shiftName, workDay) + "' and a.USER_ID = c.`EMPID` AND flow_step like '%全檢2%' " +
        //                          whereStr + " group by sizeStandard(SIZE), Tol order by SIZE1, sizeStandard(Size), Tol";
        //            tempDt = CommonClass.getSQLDataTable(addDataSQL);
        //            tempStr += " (2)" + generralSlotComment(tempDt, flow_step, srtSelUsers, workDay.ToString("yyyy-MM-dd"), stdReport.sizeList, stdReport.tolSlot);
        //        }
        //        else
        //            tempStr = generralSlotComment(tempDt, flow_step, srtSelUsers, workDay.ToString("yyyy-MM-dd"), stdReport.sizeList, stdReport.tolSlot);
        //        excel.fillExcelCellValue(ee.mWorkSheets["Template"], stdReport.extraComment, tempStr);

        //        //以下程式依據各表格而異
        //        switch (stdReport.exeFunction)
        //        {
        //            case "parseLise":
        //                dt = parseList(dt, "material_issue");
        //                break;
        //            case "parseDict":
        //                dt = parseDict(dt, "material_issue");
        //                break;
        //            case "mergeDT":
        //                dt = mergeDT(dt, stdReport.mergeSQL, stdReport.mergeKey);
        //                break;
        //            default:
        //                break;
        //        }
        //        ////////////////////////////////////////////////////
        //        foreach (string colName in stdReport.removeDbCol)
        //        {
        //            if (dt.Columns.Contains(colName))
        //                dt.Columns.Remove(colName);
        //        }

        //        if (stdReport.headerAndFooter.ContainsKey("PageMode") && stdReport.headerAndFooter["PageMode"] == "1")
        //        {
        //            currPage = excel.report_dtPageMode(dt, ee, stdReport.headerAndFooter, currPage, currPage > 1 ? false : true);
        //            currPage = currPage + 1;
        //        }
        //        else
        //        {
        //            excelStartRow = excel.report_dtToExcel(dt, ee, stdReport.headerAndFooter, excelStartRow, excelStartRow > 1 ? false : true);
        //            excelStartRow = excelEndRow + 1;
        //        }
        //    }
        //    ee.mWSheet = ee.mWorkSheets["報表"];
        //    string destPrinter = Constants.getProperty("DailyReportPrinter", "Epson EPL-6200 (左下)");
        //    excel.printExcelBySpecifyPrinter(ee, destPrinter);
        //    CommonClass.wait(1);
        //    string outputFolder = Constants.getProperty("reportOutput", @"C:\排單資料\標準\ReportOutput");
        //    string outputFileName = outputFolder + @"\" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + empName + "_" + templateFile;
        //    excel.closeExcel(ee, outputFileName, false);
        //    return 0;
        //}

        public static void referToLoginout(string sqlStr, string tableName= "mfo_sign")
        {
            int startPos = sqlStr.IndexOf("Left Join mfo_loginout ");
            int endPos = sqlStr.IndexOf("order by ");
            try
            {
                if (endPos >= 0)
                    sqlStr = "SELECT `SIGNID`, ID FROM view_signinfo  a " + sqlStr.Substring(startPos, endPos - startPos);
                else
                    sqlStr = "SELECT `SIGNID`, ID FROM view_signinfo  a " + sqlStr.Substring(startPos);
                DataTable dt = CommonClass.getSQLDataTable(sqlStr);
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        sqlStr = $"update  `mfo_sign` set `loginID`='{dr["ID"]}' where `SIGNID`='{dr["SIGNID"]}'";
                        CommonClass.execSQLNonQuery(sqlStr);
                    }
                    catch (Exception ex)
                    {
                        CommonClass.writeLog("火箭程式", $"referToLoginout({sqlStr})", 5, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonClass.writeLog("火箭程式", $"referToLoginout({sqlStr})", 5, ex.Message);
            }
        }

        public static int printDailyReport(string flow_step, string txtEmpID,
            string shiftName, DateTime workDay, out string rtnStr, int retryCnt = 0)
        {
            ExcelLib excel = new ExcelLib();
            ExcelElements ee = null;
            StdExcelReport stdReport = initDailyReport(flow_step);
            string empName = null;
            rtnStr = null;
            if (string.IsNullOrEmpty(flow_step))
            {
                rtnStr = "請選取要列印的工作日報";
                return retryCnt + 1;
            }

            int reportPage = 0, currPage = 1;
            List<String> chkedUser = new List<string>() { txtEmpID };
            string templateFile = null;

            //--------------------------------------------------------
            //寫入 Header User Data
            
            int excelStartRow = 1, excelEndRow = 0;
            if (stdReport.headerAndFooter.ContainsKey("PageMode") && !stdReport.headerAndFooter.ContainsKey("PageRows"))
            {
                MessageBox.Show("設定檔必須有 PageRows 參數 ", "列印報表", MessageBoxButtons.OK, MessageBoxIcon.Error);
                rtnStr = "設定檔必須有 PageRows 參數， 請確認";
                return retryCnt + 1;
            }


            int sizeRow = Convert.ToInt16(stdReport.headerAndFooter["sizePos"].Substring(1, 1));
            int dateRow = Convert.ToInt16(stdReport.headerAndFooter["datePos"].Substring(1, 1));
            int empShiftRow = Convert.ToInt16(stdReport.headerAndFooter["empShift"].Substring(1, 1));
            string sizeCol = stdReport.headerAndFooter["sizePos"].Substring(0, 1); //A
            string dateCol = stdReport.headerAndFooter["datePos"].Substring(0, 1);//K
            string empShiftCol = stdReport.headerAndFooter["empShift"].Substring(0, 1); //A
            DataTable dt;
            //{0} : UserID
            //{1} : llogin between 'xxxx' and 'xxxx'
            //{2} : performDate
            if (stdReport.reportType.IndexOf("切割") >= 0)
            {
                stdReport.specialFooterSQL = "SELECT `MachineID`, Round(SUM(time_to_sec(timediff(`Finish_Time`, `Start_TIME`)))/3600,1) machineTime " +
                            " FROM `mfo_sign` where USER_ID='{0}'  {1} and OutputQuan>0 " +
                            " group by `MachineID` order by machineTime";
            }
            String sqlStr = $"SELECT date_format(Llogin,'%Y-%m-%d %H:%i') Llogin, date_format(Llogout,'%Y-%m-%d %H:%i') Llogout " +
                $" FROM `mfo_loginout` where `EMPID`='{txtEmpID}' and `shiftType`='{shiftName}' and `performDate`='{workDay.ToString("yyyy-MM-dd")}' " +
                $" and Llogin > '0000:00:00' ORDER BY Llogout DESC";
            dt = CommonClass.getSQLDataTable(sqlStr);
            if (dt.Rows.Count == 0)
                return 0;
            //string llogin = Convert.ToDateTime(dt.Rows[0]["Llogin"]).AddHours(-4).ToString("yyyy/MM/dd HH:mm");
            //string llogout = Convert.ToDateTime(dt.Rows[0]["Llogout"]).AddHours(4).ToString("yyyy/MM/dd HH:mm");
            string llogin = Convert.ToDateTime(dt.Rows[0]["Llogin"]).ToString("yyyy/MM/dd HH:mm");
            string llogout = Convert.ToDateTime(dt.Rows[0]["Llogout"]).ToString("yyyy/MM/dd HH:mm");
            //每個 作業員
            foreach (string srtSelUsers in chkedUser)
            {
                //讀取 ExcelSql
                
                //String.Format(stdReport.excelSQL, dtp1.Value.AddDays(1).ToString("yyyy-MM-dd"), srtSelUsers);
                int PreOfficialMins = int.Parse(Constants.getProperty("PreOfficialMins", "30"));
                int PostOfficialMins = int.Parse(Constants.getProperty("PostOfficialMins", "30"));
                string shiftCondition = FirstohmPrds.getSfitTimeCondition(shiftName, workDay, " between", PreOfficialMins, PostOfficialMins);
                shiftCondition = $" shiftType='{shiftName}' and performDate='{workDay}'";
                //if (shiftName == "晚班")
                //    sqlStr = String.Format(stdReport.excelSQL, shiftCondition, srtSelUsers);
                //else
                    sqlStr = String.Format(stdReport.excelSQL, srtSelUsers, llogin, llogout);
                //if (flow_step == "底漆2")
                //    sqlStr = sqlStr.Replace("\"", "'").Replace(" in ('底漆1', '花蓮底漆')", "='底漆2'");
                dt = CommonClass.getSQLDataTable(sqlStr);
                string whereStr = null;
                if (stdReport.reportType.IndexOf("外檢") >= 0)
                    whereStr = CommonClass.getWhereInSQL(sqlStr, 1000);
                else
                    whereStr = CommonClass.getWhereInSQL(sqlStr);
                if (dt.Rows.Count == 0)
                {
                    //continue; 派單系統可以印多人， 火箭程式不印多人
                    rtnStr = "查無資料： " + sqlStr;
                    CommonClass.writeLog("火箭", "列印工作日報，printDailyReport", 4, $"查無資料:{sqlStr}");
                    return 0;
                }

                //referToLoginout(sqlStr);
                //Copy from Line 1779
                if (flow_step.Contains("切割") || flow_step.Contains("加壓") || flow_step.Contains("外檢") || flow_step.Contains("全檢"))
                    dt = CommonClass.insertEmptyRow(dt, "SIZE");
                //底漆、色碼、貼帶
                else if (flow_step.Contains("底漆") || flow_step.Contains("色碼") || flow_step.Contains("貼帶"))
                    dt = CommonClass.insertEmptyRow(dt, "機台別");

                ee = getReportExcel(txtEmpID, stdReport.reportType, out templateFile);
                ee.mWorkSheets["報表"].PageSetup.PrintArea = "A:" + stdReport.headerAndFooter["columnEnd"];
                empName = dt.Rows[0]["EMPNAME"].ToString();
                if (dt.Columns.Contains("CoWorker") && dt.Rows[0]["CoWorker"] != DBNull.Value && dt.Rows[0]["CoWorker"].ToString() != "")
                {
                    empName += "," + empidToEmpName(dt.Rows[0]["CoWorker"].ToString());
                }
                excel.fillExcelCellValue(ee.mWorkSheets["Template"], sizeCol + (excelStartRow + sizeRow - 1), genSizeStr(dt));
                excel.fillExcelCellValue(ee.mWorkSheets["Template"], dateCol + (excelStartRow + dateRow - 1), CommonClass.ToFullTaiwanDate(workDay, 2));
                excel.fillExcelCellValue(ee.mWorkSheets["Template"], empShiftCol + (excelStartRow + empShiftRow - 1), genShiftAndUser(shiftName, empName, llogin, llogout , workDay));

                if (stdReport.specialFooterSQL != null)
                {
                    List<String> fullMachine = new List<String>(), halfMachine = new List<String>();
                    string footerSql = string.Format(stdReport.specialFooterSQL, srtSelUsers, FirstohmPrds.getSfitTimeCondition(shiftName, workDay, " and `Finish_Time` between", 0, 60));
                    DataTable footerDt = CommonClass.getSQLDataTable(footerSql);
                    foreach (DataRow dr in footerDt.Rows)
                    {
                        if (Convert.ToSingle(dr["machineTime"]) >= 6.5)
                        {
                            string mStr = dr["MachineID"].ToString();
                            if (!fullMachine.Contains(mStr))
                                fullMachine.Add(mStr);
                        }
                        else
                            halfMachine.Add(dr["MachineID"].ToString() + "(" + dr["machineTime"].ToString() + ")");
                    }
                    if (fullMachine.Count > 0)
                        //excel.fillExcelCellValue(ee.mWorkSheets["Template"], "I40", string.Join(",", fullMachine));
                        excel.fillExcelCellValue(ee.mWorkSheets["Template"], "D41", fullMachine.Count.ToString());
                    else
                        excel.fillExcelCellValue(ee.mWorkSheets["Template"], "D41", "0");
                    if (halfMachine.Count > 0)
                        excel.fillExcelCellValue(ee.mWorkSheets["Template"], "F40", string.Join(",", halfMachine));
                    else
                        excel.fillExcelCellValue(ee.mWorkSheets["Template"], "F40", "");
                }

                //計算統計資料
                String addDataSQL = null;
                if (stdReport.reportType == "色碼報表")
                {
                    addDataSQL = "SELECT  a.FLOW_STEP , a.Tol , a.MachineID , Sum(OutputQuan) quant, SUBSTRING_INDEX(a.SIZE, 'x', 1) size1 " +
                                "FROM view_signInfo a " +
                                //" Left Join mfo_loginout c on {0}  and EMPID = '{1}' " +
                                //whereStr + "  and  `FLOW_STEP` like '%色碼%' " +
                                whereStr +
                                " group by a.MachineID, a.Tol,a.FLOW_STEP " +
                                " order by a.MachineID, a.Tol";
                }
                else if (stdReport.reportType.IndexOf("外檢") >= 0)
                {
                    string reportType = stdReport.reportType.IndexOf("外檢底漆") >= 0 ? "外檢1" : "外檢2";
                    addDataSQL = "SELECT a.Tol , sizeStandard(a.SIZE) SIZE, Sum(OutputQuan) quant,  Sum(a.DefectQuan) NGquant, sizeToNo(a.SIZE) size1 " +
                                " FROM view_signInfo a " +
                                //" Left Join mfo_loginout c on Date(`LoginTime`)= '" + getRealLoginDate(shiftName, workDay) + "' and a.USER_ID = c.`EMPID` " + 
                                //" Left Join mfo_loginout c on {0} and EMPID = '{1}' " +
                                //"  where `FLOW_STEP` like '%" + reportType + "%' and a.signFinish between c.Llogin and c.LlogOut " +
                                //" and a.USER_ID='{1}'" +
                                whereStr + 
                                " group by sizeStandard(SIZE), Tol order by SIZE1, sizeStandard(Size), Tol";
                }
                else if (stdReport.reportType.IndexOf("貼帶") >= 0)
                {
                    addDataSQL = "";
                }
                //else if (stdReport.reportType.IndexOf("底漆") >= 0)
                //{
                //    //addDataSQL = "SELECT '' Tol , SUBSTRING(`MachineID`, 1, position('-' in MachineID)-1) SIZE , Sum(OutputQuan) quant, SUBSTRING(`MachineID`, 1, position('-' in MachineID)-1) size1 " +
                //    //            " FROM view_signInfo a " +
                //    //            " left Join view_subflowinfo d on a.SUBFLOWID = d.SUBFLOWID " +
                //    //            //" Left Join mfo_loginout c on {0}  and EMPID = '{1}' " +
                //    //            whereStr + "  and `FLOW_STEP` like '%底漆%' " +
                //    //            " group by a.SIZE order by a.Size ";
                //    addDataSQL = "SELECT '' Tol , Concat(a.size, `MachineID` SIZE , Sum(OutputQuan) quant, `MachineID` size1 " +
                //                " FROM view_signInfo a " +
                //                " left Join view_subflowinfo d on a.SUBFLOWID = d.SUBFLOWID " +
                //                //" Left Join mfo_loginout c on {0}  and EMPID = '{1}' " +
                //                whereStr + "  and `FLOW_STEP` like '%底漆%' " +
                //                " group by a.SIZE order by a.Size ";
                //}
                else if (stdReport.reportType.IndexOf("全檢") >= 0)
                {
                    addDataSQL = "";
                }
                else if (stdReport.reportType.IndexOf("有腳焊接") >= 0)
                {
                    addDataSQL = "";
                }
                else
                    addDataSQL = "SELECT Tol , sizeStandard(SIZE) SIZE , Sum(OutputQuan) quant, sizeToNo(SIZE) size1 " +
                        "FROM view_signInfo a  " +
                        //" Left Join mfo_loginout c on {0}  and EMPID = '{1}' " +
                        whereStr + " group by sizeStandard(SIZE), Tol order by SIZE1, sizeStandard(Size), Tol";
                addDataSQL = String.Format(addDataSQL, shiftCondition, txtEmpID);
                DataTable tempDt = null;
                if (!string.IsNullOrEmpty(addDataSQL))
                    tempDt = CommonClass.getSQLDataTable(addDataSQL);
                string tempStr = null;
                if (stdReport.reportType == "色碼報表")
                {
                    //色碼 以機台統計，透明漆分開計算
                    Dictionary<string, double> clearVarnishTotalDic = new Dictionary<string, double>();//透明漆 dic<"機台","數量">
                    Dictionary<string, double> otherTotalDic = new Dictionary<string, double>();//其餘 dic<"機台","數量">
                    foreach(DataRow dr in tempDt.Rows)
                    {
                        if(dr["FLOW_STEP"].ToString() == "透明漆")
                        {
                            if (!clearVarnishTotalDic.ContainsKey(dr["MachineID"].ToString()))
                                clearVarnishTotalDic.Add(dr["MachineID"].ToString(), double.Parse(dr["quant"].ToString()));
                            else
                                clearVarnishTotalDic[dr["MachineID"].ToString()] += double.Parse(dr["quant"].ToString());
                        }
                        else
                        {
                            if (!otherTotalDic.ContainsKey(dr["MachineID"].ToString()))
                                otherTotalDic.Add(dr["MachineID"].ToString(), double.Parse(dr["quant"].ToString()));
                            else
                                otherTotalDic[dr["MachineID"].ToString()] += double.Parse(dr["quant"].ToString());
                        }
                    }
                    tempStr = "(色):";
                    foreach (KeyValuePair<string, double> dic in otherTotalDic)
                        tempStr += $"{dic.Key}計{(dic.Value/1000).ToString("##0.##")}Kpcs  ";
                    tempStr += "\n(透):";
                    foreach(KeyValuePair<string, double> dic in clearVarnishTotalDic)
                        tempStr += $"{dic.Key}計{(dic.Value / 1000).ToString("##0.##")}Kpcs  ";
                }
                else if (stdReport.reportType == "底漆報表")
                {
                    //底漆統計要算四次
                    //'0.8x1.9' and a.Vals<=50， 尺寸:2.5x8以上(含2.5x8),  阻值: 1E以下, 阻值範圍:1 %
                    // 符合以上條件的底漆要先塗再切
                    addDataSQL = "SELECT Tol , sizeStandard(SIZE) SIZE , Sum(OutputQuan) quant, sizeToNo(SIZE) size1 " +
                                 " FROM view_signInfo_diqiflag  a  " +
                                  //" Left Join mfo_loginout c on Date(`LoginTime`)= '" +
                                  // getRealLoginDate(shiftName, workDay) + "' and a.USER_ID = c.`EMPID` AND flow_step like '%底漆%' " +
                                  // " and ((sizeStandard(a.size)='0.8x1.9' and a.Vals<=50 ) or (sizeToNo(SIZE) >= 2.5 and Vals < 1 and Tol=1))
                                  // and((sizeStandard(a.size) = '0.8x1.9' and a.Vals <= 50) or(sizeToNo(SIZE) >= 2.5 and Vals < 1 and Tol = 1))" +
                                  whereStr + " and  (diqiFlag= 1)  " +
                                  " group by sizeStandard(SIZE), Tol order by SIZE1, sizeStandard(Size), Tol";
                    tempDt = CommonClass.getSQLDataTable(addDataSQL);
                    //List<string> diqiSize = new List<string>() { "0.8x1.9", "2.5x8", "3.5x10" };
                    //List<double> tolSlot = new List<double>() {0.1 };
                    string diqi2Str = null;
                    if (tempDt.Rows.Count > 0)
                        tempStr += "(1)" + generralSlotComment(tempDt, flow_step, srtSelUsers, workDay.ToString("yyyy-MM-dd"), stdReport.sizeList, stdReport.tolSlot);
                    addDataSQL = "SELECT Tol , sizeStandard(SIZE) SIZE , Sum(OutputQuan) quant, sizeToNo(SIZE) size1 " +
                                 " FROM view_signInfo_diqiflag  a " +
                                 // " Left Join mfo_loginout c on  shiftType='{shiftName}' and performDate='{workDay}'" +
                                 // " and a.USER_ID = c.`EMPID` AND flow_step like '%底漆%' " +
                                  whereStr + " and (diqiflag=2 or diqiflag='')  group by sizeStandard(SIZE), Tol order by SIZE1, sizeStandard(Size), Tol";
                    tempDt = CommonClass.getSQLDataTable(addDataSQL);
                    if (tempDt.Rows.Count > 0)
                    {
                        diqi2Str = generralSlotComment(tempDt, flow_step, srtSelUsers, workDay.ToString("yyyy-MM-dd"), stdReport.sizeList, stdReport.tolSlot);
                        if (!string.IsNullOrEmpty(tempStr))
                        {
                            tempStr += "  (2)" + diqi2Str;
                        }
                        else
                        {
                            tempStr = "(2)" + diqi2Str;
                        }
                    }
                    addDataSQL = "SELECT Tol , sizeStandard(SIZE) SIZE , Sum(OutputQuan) quant, sizeToNo(SIZE) size1 " +
                                " FROM view_signInfo_diqiflag  a  " +
                                 whereStr + " and  (diqiFlag= '透')  " +
                                 " group by sizeStandard(SIZE), Tol order by SIZE1, sizeStandard(Size), Tol";
                    tempDt = CommonClass.getSQLDataTable(addDataSQL);
                    if (tempDt.Rows.Count > 0)
                        tempStr += "(透)" + generralSlotComment(tempDt, flow_step, srtSelUsers, workDay.ToString("yyyy-MM-dd"), stdReport.sizeList, stdReport.tolSlot);

                    addDataSQL = "SELECT  Concat(sizeStandard(SIZE), `MachineID`) MSIZE , MachineID, Size, Round(Sum(OutputQuan)/1000,1) quant   " +
                     " FROM view_signInfo_diqiflag  a " +
                      whereStr + " and (diqiflag=2 or diqiflag='' or diqiflag='透' or diqiflag=1)  " +
                      " group by MSIZE " +
                      " order by MachineID ,MSIZE, Size";
                            tempDt = CommonClass.getSQLDataTable(addDataSQL);
                    if (tempDt.Rows.Count > 0) {
                        tempStr += $"{Environment.NewLine}";
                        foreach (DataRow dr in tempDt.Rows)
                        {
                            tempStr += $"({dr["MachineID"]}):{dr["Size"]} {dr["quant"]}Kpcs; ";
                        }
                    }
                    
                }
                else if (stdReport.reportType == "全檢報表")
                {
                    addDataSQL = "SELECT Tol , sizeStandard(SIZE) SIZE, Sum(OutputQuan) quant, sizeToNo(SIZE) size1 " +
                                 " FROM view_signInfo a  " +
                                 //" Left Join mfo_loginout c on Date(`LoginTime`)= '" +
                                 // getRealLoginDate(shiftName, workDay) + "' and a.USER_ID = c.`EMPID` AND (flow_step like '%全檢1%' or flow_step like '%全檢分類%') " +
                                  whereStr + " AND (flow_step like '%全檢1%' or flow_step like '%全檢分類%') " + 
                                  " group by sizeStandard(SIZE), Tol order by SIZE1, sizeStandard(Size), Tol";
                    tempDt = CommonClass.getSQLDataTable(addDataSQL);
                    tempStr = generralSlotComment(tempDt, flow_step, srtSelUsers, workDay.ToString("yyyy-MM-dd"), stdReport.sizeList, stdReport.tolSlot);
                    if (!string.IsNullOrEmpty(tempStr))
                        tempStr = "(1)" + tempStr;
                    addDataSQL = "SELECT Tol , sizeStandard(SIZE) SIZE, Sum(OutputQuan) quant, sizeToNo(SIZE) size1 " +
                                 " FROM view_signInfo a  " +
                                   // " Left Join mfo_loginout c on Date(`LoginTime`)= '" +
                                  // getRealLoginDate(shiftName, workDay) + "' and " +
                                  // " a.USER_ID = c.`EMPID` AND flow_step like '%全檢2%' " +
                                  whereStr + " AND flow_step like '%全檢2%' " +
                                  " group by sizeStandard(SIZE), Tol order by SIZE1, sizeStandard(Size), Tol";
                    tempDt = CommonClass.getSQLDataTable(addDataSQL);
                    tempStr += " (2)" + generralSlotComment(tempDt, flow_step, srtSelUsers, workDay.ToString("yyyy-MM-dd"), stdReport.sizeList, stdReport.tolSlot);
                }
                else if(stdReport.reportType == "貼帶報表")
                {
                    addDataSQL = "SELECT `MachineID` , Round(SUM(`OutputQuan`)/1000,1) 'OutputQuan' " +
                        " FROM `view_signinfo` " +
                        whereStr +
                        " GROUP BY `MachineID` " +
                        " ORDER BY `MachineID` ";
                    tempDt = CommonClass.getSQLDataTable(addDataSQL);
                    tempStr = "";
                    foreach (DataRow dr in tempDt.Rows)
                    {
                        if (tempStr != "")
                            tempStr += "、";
                        tempStr += $"({dr["MachineID"].ToString()}) {dr["OutputQuan"].ToString()} Kpcs";
                    }
                }
                else if(stdReport.reportType == "塗裝報表")
                {
                    addDataSQL = $"SELECT `FLOW_STEP` , `SIZE` , SUM(`OutputQuan`) 'SUMQUAN' " +
                        $" FROM `view_signinfo` " +
                        $" {whereStr} " +
                        $" GROUP BY `FLOW_STEP` , `SIZE` " +
                        $" ORDER BY `SIZE` ";
                    tempDt = CommonClass.getSQLDataTable(addDataSQL);
                    Dictionary<string, double> tempDic = new Dictionary<string, double>() {  };
                    foreach(DataRow dr in tempDt.Rows)
                    {
                        string key = "";
                        if (dr["FLOW_STEP"].ToString() == "底漆1")
                            key = $"(底1){dr["SIZE"].ToString()}";
                        else if (dr["FLOW_STEP"].ToString() == "底漆2")
                            key = $"(底){dr["SIZE"].ToString()}";
                        else if (dr["FLOW_STEP"].ToString() == "花蓮底漆")
                            key = $"(底){dr["SIZE"].ToString()}";
                        else if (dr["FLOW_STEP"].ToString() == "花蓮色碼")
                            key = $"(色){dr["SIZE"].ToString()}";
                        else if (dr["FLOW_STEP"].ToString() == "透明漆")
                            key = $"(透){dr["SIZE"].ToString()}";
                        double quan;
                        if (!double.TryParse(dr["SUMQUAN"].ToString(), out quan))
                            quan = 0;
                        if (!tempDic.ContainsKey(key))
                            tempDic.Add(key, quan);
                        else
                            tempDic[key] += quan;
                    }
                    tempStr = null;
                    foreach(KeyValuePair<string,double> keyPair in tempDic)
                    {
                        if (tempStr == null)
                            tempStr = $"{keyPair.Key}:{(keyPair.Value/1000).ToString("##0.###")} Kpcs";
                        else
                        {
                            tempStr += "，";
                            tempStr += $"{keyPair.Key}:{(keyPair.Value / 1000).ToString("##0.###")} Kpcs";
                        }
                    }
                        
                }
                else if (stdReport.reportType == "有腳焊接報表")
                {
                    tempStr = "";
                    addDataSQL = $"SELECT `SIZE` , ROUND(SUM(`OutputQuan`)/1000,3) 'sumQuan' " +
                        $" FROM `mfo_sign` " +
                        $" {whereStr.Replace("a.","")} " +
                        $" GROUP BY `SIZE` ";
                    tempDt = CommonClass.getSQLDataTable(addDataSQL);
                    foreach(DataRow dr in tempDt.Rows)
                    {
                        if (tempStr == "")
                            tempStr = $"{dr["SIZE"].ToString()} : {dr["sumQuan"].ToString()} Kpcs";
                        else
                            tempStr += $",{dr["SIZE"].ToString()} : {dr["sumQuan"].ToString()} Kpcs";
                    }
                }
                else
                    tempStr = generralSlotComment(tempDt, flow_step, srtSelUsers, workDay.ToString("yyyy-MM-dd"), stdReport.sizeList, stdReport.tolSlot);
                excel.fillExcelCellValue(ee.mWorkSheets["Template"], stdReport.extraComment, tempStr);

                //以下程式依據各表格而異
                switch (stdReport.exeFunction)
                {
                    case "parseLise":
                        dt = parseList(dt, "material_issue");
                        break;
                    case "parseDict":
                        dt = parseDict(dt, "material_issue");
                        break;
                    case "mergeDT":
                        dt = mergeDT(dt, stdReport.mergeSQL, stdReport.mergeKey);
                        break;
                    default:
                        break;
                }
                ////////////////////////////////////////////////////
                foreach (string colName in stdReport.removeDbCol)
                {
                    if (dt.Columns.Contains(colName))
                        dt.Columns.Remove(colName);
                }

                if (stdReport.headerAndFooter.ContainsKey("PageMode") && stdReport.headerAndFooter["PageMode"] == "1")
                {
                    currPage = excel.report_dtPageMode(dt, ee, stdReport.headerAndFooter, currPage, currPage > 1 ? false : true);
                    currPage = currPage + 1;
                }
                else
                {
                    excelStartRow = excel.report_dtToExcel(dt, ee, stdReport.headerAndFooter, excelStartRow, excelStartRow > 1 ? false : true);
                    excelStartRow = excelEndRow + 1;
                }
            }
            ee.mWSheet = ee.mWorkSheets["報表"];
            string destPrinter = Constants.getProperty("DailyReportPrinter", "Epson EPL-6200 (左下)");
            string printAtt = Constants.getProperty($"{flow_step}格式", "{'PageMode':'1','PageRows':'39','header':'1:39','pageDataRows':'27','dataRowStart':'8','columnEnd':'P',  'sizePos': 'A3',  'datePos': 'P4',  'empShift': 'A4', 'ifPrint':'1', 'ifClose'='1'}");
            Dictionary<string, string> printAttDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(printAtt);
            if(printAttDict["ifPrint"]=="1")
                excel.printExcelBySpecifyPrinter(ee, destPrinter);
            CommonClass.wait(1);
            string outputFolder = Constants.getProperty("reportOutput", @"C:\排單資料\標準\ReportOutput");
            string outputFileName = outputFolder + @"\" + workDay.ToString("yyyyMMdd") +   "_" + empName + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + templateFile;
            if (printAttDict["ifClose"] == "1")
                excel.closeExcel(ee, outputFileName, false);
            return 0;
        }

        public static string searchInKeyValList(List<KeyValuePair<string, string>> searchList, string searchKey)
        {
            foreach(KeyValuePair<string, string>kv in searchList)
            {
                if (kv.Key.ToLower() == searchKey.ToLower())
                    return kv.Value;
            }
            return null;
        }

        public static int printDailyReport(List<KeyValuePair<string, string>> dailyReportsParams, out string rtnStr, int retry=0)
        {
            rtnStr = null;
            string flowStep = searchInKeyValList(dailyReportsParams, "flowStep");
            string strRetryCnt = searchInKeyValList(dailyReportsParams, "reTry");
            if (!string.IsNullOrEmpty(strRetryCnt))
                retry = int.Parse(strRetryCnt);
            else
                retry = 0;
            //重印的次數
            int outChkRetry = 0;
            if (string.IsNullOrEmpty(flowStep))
            {
                rtnStr = "部門錯誤無法列印！！";
                return retry + 1;
            }
            else if (flowStep == "外檢")
            {
                CommonClass.execSQLNonQuery("UPDATE mfo_sign AS a " +
                    " INNER JOIN mfo_outcheck AS b ON a.`SIGNID` = b.`SIGNID` " +
                    " SET `officialNG` = b.examType " +
                    " WHERE a.Finish_Time >= subDate(CURRENT_DATE,5) and " +
                    " a.`FLOW_STEP` like '%檢%' ");
                if ((outChkRetry = printDailyReport("外檢底漆", searchInKeyValList(dailyReportsParams, "empID"),
                                searchInKeyValList(dailyReportsParams, "shiftName"),
                                Convert.ToDateTime(searchInKeyValList(dailyReportsParams, "reportDate")), out rtnStr, retry)) == 0)
                {
                    CommonClass.wait(5);
                    return printDailyReport("外檢色碼", searchInKeyValList(dailyReportsParams, "empID"),
                                searchInKeyValList(dailyReportsParams, "shiftName"),
                                Convert.ToDateTime(searchInKeyValList(dailyReportsParams, "reportDate")), out rtnStr, retry);
                }
                else
                    return outChkRetry;
            }
            else
            {
                return printDailyReport(flowStep, searchInKeyValList(dailyReportsParams, "empID"),
                        searchInKeyValList(dailyReportsParams, "shiftName"),
                        //*******//Convert.ToDateTime(searchInKeyValList(dailyReportsParams, "reportDate")), out rtnStr, retry);
                        Convert.ToDateTime(searchInKeyValList(dailyReportsParams, "reportDate")), out rtnStr, retry);
            }
        }
        #endregion

        #region FTP retrive Conutri Data
        //dataIdx : 0 乾燥室 1~5
        //dataIdx > 0, 代表 1. 這是 該筆資料的 Log_ID， 2. 乾燥室 6~10
        public static int retribeConutriTP(int dataIdx, string ipAdd, string userAcc, string pwd, string remotePath, string localPath, out string rtnStr)
        {
            FTPExtensions ftp=null;
            rtnStr = null;
            string localFile, remoteFile;
            try
            {
                if (dataIdx == 0)
                {
                    remoteFile = remotePath + @"\" + "H0001.csv";
                    localFile = localPath + @"\" + "H0001.csv";
                }
                else
                {
                    remoteFile = remotePath + @"\" + "H0002.csv";
                    localFile = localPath + @"\" + "H0002.csv";
                }
                DataTable dt = null;
                CommonClass.deleteFiles(localPath, DateTime.Now, "csv");
                if (ftp == null)
                    ftp = new FTPExtensions(ipAdd, userAcc, pwd);
                if(!ftp.Download(remoteFile, localFile))
                {
                    rtnStr = "連線 FTP 失敗！！";
                    if(CommonApp.lastFtpFailLine.Date != DateTime.Now.Date)
                    {
                        LineLib linelib = new LineLib();
                        linelib.pushTextMessageOuter("曾建明", "溫度監測機制 停止  請確認,請確認");
                        linelib.pushTextMessageOuter("蕭人碩", "溫度監測機制 停止  請確認,請確認");
                        linelib.pushTextMessageOuter("郭佳霖", "溫度監測機制 停止  請確認,請確認");
                        linelib.pushTextMessageOuter("康起禎", "屏東溫控 PLC 機制停止， 請確認");
                        CommonApp.lastFtpFailLine = DateTime.Now;
                    }
                    
                    return -1;
                }
                if (CommonClass.ifFileExists(localFile))
                    dt = CommonClass.CsvToDataTable(localFile, "\t", 1, 2);
                else
                {
                    rtnStr = "下載失敗！！";
                    return -1;
                }

                if (dt.Rows.Count <= 0)
                {
                    rtnStr = "下載檔案無資料 !!";
                    return -1;
                }
                DataRow dr = dt.Rows[dt.Rows.Count - 1];
                Dictionary<string, string> sqlParams = new Dictionary<string, string>();
                String sqlStr;
                if (dataIdx == 0)
                {
                    sqlParams.Add("@DataTime", dr[1].ToString() + " " + dr[0].ToString());
                    sqlParams.Add("@outDoorTemp", dr["外部環境溫度"].ToString());
                    sqlParams.Add("@outDoorhumidity", dr["外部環境濕度"].ToString());
                    sqlParams.Add("@DryRoom01TP", dr["乾燥室溫度01"].ToString());
                    sqlParams.Add("@DryRoom01HM", dr["乾燥室濕度01"].ToString());
                    sqlParams.Add("@DryRoom02TP", dr["乾燥室溫度02"].ToString());
                    sqlParams.Add("@DryRoom02HM", dr["乾燥室濕度02"].ToString());
                    sqlParams.Add("@DryRoom03TP", dr["乾燥室溫度03"].ToString());
                    sqlParams.Add("@DryRoom03HM", dr["乾燥室濕度03"].ToString());
                    sqlParams.Add("@DryRoom04TP", dr["乾燥室溫度04"].ToString());
                    sqlParams.Add("@DryRoom04HM", dr["乾燥室濕度04"].ToString());
                    sqlParams.Add("@DryRoom05TP", dr["乾燥室溫度05"].ToString());
                    sqlParams.Add("@DryRoom05HM", dr["乾燥室濕度05"].ToString());
                    sqlStr = "Insert into  tplog (DataTime,outDoorTemp,outDoorhumidity,DryRoom01TP,DryRoom01HM,DryRoom02TP,DryRoom02HM, " +
                                    "DryRoom03TP,DryRoom03HM,DryRoom04TP,DryRoom04HM,DryRoom05TP,DryRoom05HM) " +
                                    "Values(@DataTime, @outDoorTemp, @outDoorhumidity, @DryRoom01TP, @DryRoom01HM, @DryRoom02TP, @DryRoom02HM, " +
                                    "@DryRoom03TP, @DryRoom03HM, @DryRoom04TP, @DryRoom04HM, @DryRoom05TP, @DryRoom05HM)" +
                                    " ON DUPLICATE KEY UPDATE " +
                                    "outDoorTemp=@outDoorTemp,outDoorhumidity=@outDoorhumidity,DryRoom01TP=@DryRoom01TP," +
                                    "DryRoom01HM=@DryRoom01HM,DryRoom02TP=@DryRoom02TP,DryRoom02HM=@DryRoom02HM, " +
                                    "DryRoom03TP=@DryRoom03TP,DryRoom03HM=@DryRoom03HM,DryRoom04TP=@DryRoom04TP, " +
                                    "DryRoom04HM=@DryRoom04HM,DryRoom05TP=@DryRoom05TP,DryRoom05HM=@DryRoom05HM";
                }
                else
                {
                    sqlParams.Add("@DryRoom06TP", dr["乾燥室溫度06"].ToString());
                    sqlParams.Add("@DryRoom06HM", dr["乾燥室濕度06"].ToString());
                    sqlParams.Add("@DryRoom07TP", dr["乾燥室溫度07"].ToString());
                    sqlParams.Add("@DryRoom07HM", dr["乾燥室濕度07"].ToString());
                    sqlParams.Add("@DryRoom08TP", dr["乾燥室溫度08"].ToString());
                    sqlParams.Add("@DryRoom08HM", dr["乾燥室濕度08"].ToString());
                    sqlParams.Add("@DryRoom09TP", dr["乾燥室溫度09"].ToString());
                    sqlParams.Add("@DryRoom09HM", dr["乾燥室濕度09"].ToString());
                    sqlParams.Add("@DryRoom10TP", dr["乾燥室溫度10"].ToString());
                    sqlParams.Add("@DryRoom10HM", dr["乾燥室濕度10"].ToString());
                    sqlStr = "Update tplog set DryRoom06TP=@DryRoom06TP,DryRoom06HM=@DryRoom06HM," +
                              " DryRoom07HM=@DryRoom07HM, DryRoom07TP=@DryRoom07TP, " +
                              " DryRoom08HM=@DryRoom08HM, DryRoom08TP = @DryRoom08TP," +
                              " DryRoom09HM = @DryRoom09HM,DryRoom09TP = @DryRoom09TP, " +
                              " DryRoom10HM = @DryRoom10HM,DryRoom10TP = @DryRoom10TP" +
                              " where `Log_ID` = " + dataIdx;
                }

                int sqlID = CommonClass.execSQLNonQueryParams(sqlStr, sqlParams, Constants.ConutriTCConnString, true);
                return sqlID;
            }
            catch (Exception ex)
            {
                rtnStr = ex.Message;
                return -1;
            }
        }
        #endregion

        #region 採購
        //return 0:完成, 3:資料有錯
        public static int 採購申請(List<KeyValuePair<string, string>> procurementParams, out string rtnStr, bool ifOpenExcel=true)
        {
            try
            {
                DateTime reportDate = DateTime.Now;
                string startDate = searchInKeyValList(procurementParams, "startDate");
                string endDate = searchInKeyValList(procurementParams, "endDate");
                string size = searchInKeyValList(procurementParams, "size");
                string cfmf = searchInKeyValList(procurementParams, "cfmf");
                //string rtnStr = "";
                //設定Excel範本路徑
                ExcelLib excel = null;
                string sqlStr = "", sqlStrOrderCode = "";
                string templateFolder = Constants.getProperty("採購表格母板路徑", @"C:\procurement\母版\原料");
                string outputFolder = Constants.getProperty("ProcurmentOutputPath"); //, @"C:\procurement"

                string templateFile = "";
                string outputFileName = "";

                string OrderCode = "", RequireDate = "";

                Dictionary<string, string> sqlParams = new Dictionary<string, string>();
                sqlParams.Add("@StartDate", startDate);
                sqlParams.Add("@EndDate", endDate);

                if (size == "其它")
                {
                    sqlStr = "SELECT A.`MFOID` 工令單號, A.`SIZE` 尺寸, MB.RTYPE 型號, CONCAT(MB.TOL, '%') 阻值, A.`RO`, A.Quant, A.`Note`備註" +
                        " FROM PRC_Application A LEFT JOIN mfo_flow.mfo_base MB ON A.BASEUID = MB.UID" +
                        " WHERE A.ApplicationDate BETWEEN @StartDate AND @EndDate and A.Valid = 0 and A.`SIZE` not in ('1x3.15', '1.7x5.4', '0.8x1.9') and A.status > 0;";
                    sqlStrOrderCode = "SELECT A.OrderCode 請購單號, A.RequireDate 出貨日" +
                        " FROM PRC_Application A LEFT JOIN mfo_flow.mfo_base MB ON A.BASEUID = MB.UID" +
                        " WHERE A.OrderCode <> '' and A.ApplicationDate BETWEEN @StartDate AND @EndDate and A.Valid = 0 and A.`SIZE` not in ('1x3.15', '1.7x5.4', '0.8x1.9') and A.status > 0;";
                    templateFile = Constants.getProperty("採購原料表格母板", @"申請表.xlsx");
                    outputFileName = outputFolder + @"\" + reportDate.ToString("yyyyMMddHHmmss") + "_" + templateFile;
                }
                else
                {
                    sqlParams.Add("@Size", size);
                    sqlParams.Add("@CFMF", cfmf);
                    sqlStr = "SELECT arr.RO, IF(b.Unit is Null, 'Kpcs', b.Unit) Unit, '', '' required, IF(b.`Quant` is Null, 0,b.`Quant`) Quant, b.`Note` 備註, b.MFOID" + //a.`RO`, a.CFMF //sum(`QTY`) //'' remark //'K' unit
                        " FROM `material` a" +
                        " Left Join(select Size, RO, CFMF, sum(Quant)  Quant, ApplicationDate, Note, MFOID, Unit  from prc_application" + //匯出欄位加入備註和工令單號，Added by Michael, 2020.12.24
                        " where ApplicationDate BETWEEN @StartDate AND @EndDate and Valid = 0 and status > 0" +
                        " group by `SIZE`,`RO`,`CFMF`) b on a.Size = b.Size and a.CFMF = b.CFMF and A.RO = b.RO" +
                        " RIGHT JOIN PRC_ApplicationReportROs arr on arr.RO = a.RO" +
                        " Where a.`SIZE`= @Size " + (("CF,MF").Contains(cfmf) ? "and a.CFMF = @CFMF" : "and a.CFMF NOT IN ('CF', 'MF')") +
                        //" GROUP BY RO order by ro;";
                        " GROUP BY arr.RO order by arr.RID;";
                    sqlStrOrderCode = "SELECT b.OrderCode 請購單號, b.RequireDate 出貨日" + //匯出表頭加入出貨日，Added by Michael, 2020.12.24
                        " FROM `material` a" +
                        " Left Join(select OrderCode, Size, RO, CFMF, sum(Quant)  Quant, ApplicationDate, RequireDate  from prc_application" +
                        " where ApplicationDate BETWEEN @StartDate AND @EndDate and Valid = 0 and status > 0" +
                        " group by `SIZE`,`RO`,`CFMF`) b on a.Size = b.Size and a.CFMF = b.CFMF and A.RO = b.RO" +
                        " RIGHT JOIN PRC_ApplicationReportROs arr on arr.RO = a.RO" +
                        " Where b.OrderCode <> '' and a.`SIZE`= @Size " + (("CF,MF").Contains(cfmf) ? "and a.CFMF = @CFMF" : "and a.CFMF NOT IN ('CF', 'MF')") +
                        //" GROUP BY RO order by ro;";
                        " GROUP BY arr.RO order by arr.RID;";

                    templateFile = Constants.getProperty("採購原料表格母板", @"請料單.xls");
                    outputFileName = outputFolder + @"\" + reportDate.ToString("yyyyMMddHHmmss") + "_" + templateFile;
                }

                int currPage = 1;
                if (excel == null)
                    excel = new ExcelLib();
                DataTable dt = CommonClass.getSQLDataTableParams(sqlStr, sqlParams, Constants.ProcurmentConnection);
                //原料申請加入訂購單號，Added by Michael, 2020.11.26
                DataTable dtOrderCode = CommonClass.getSQLDataTableParams(sqlStrOrderCode, sqlParams, Constants.ProcurmentConnection);

                if (CommonClass.DataTableSum(dt, "Quant", "") == 0) // || dtOrderCode.Rows.Count == 0
                {
                    rtnStr = "無原料申請或庫存記錄";
                    return 3;
                }
                if (dtOrderCode.Rows.Count == 0)
                {
                    rtnStr = "沒有原料記錄";
                    return 3;
                }
                else
                {
                    OrderCode = dtOrderCode.Rows[0][0].ToString();
                    RequireDate = dtOrderCode.Rows[0][1].ToString();
                }
                //設定Excel檔參數
                ExcelElements ee = excel.openExcel(templateFolder + @"\" + templateFile); //, true
                                                                                          //ee.oXL.Visible = true;
                string formateJsonStr = "";
                if (size == "其它")
                    formateJsonStr = "{ 'PageMode':'1','PageRows':'24','header':'1:24','pageDataRows':'21','dataRowStart':'3','columnEnd':'G',  'datePos': 'L3'}";
                else
                    formateJsonStr = "{ 'PageMode':'1','PageRows':'42','header':'1:42','pageDataRows':'36','dataRowStart':'5','columnEnd':'I',  'datePos': 'L3'}";

                //填入表頭和表尾
                Dictionary<string, string> HeaderAndFooter = JsonConvert.DeserializeObject<Dictionary<string, string>>(formateJsonStr);
                if (size != "其它")
                {
                    string[] RDate = CommonClass.ToSimpleTaiwanDate(reportDate).Split('/');
                    excel.fillExcelCellValue(ee.mWorkSheets["Template"], "H2", "申請日期: " + CommonClass.ToSimpleTaiwanDate(reportDate));
                    excel.fillExcelCellValue(ee.mWorkSheets["Template"], "E2", "請購單號: " + OrderCode);
                    //匯出表頭加入出貨日，Added by Michael, 2020.12.24
                    excel.fillExcelCellValue(ee.mWorkSheets["Template"], "H3", "交貨日期: " + CommonClass.ToSimpleTaiwanDate(Convert.ToDateTime(RequireDate)));

                    string Categories = "□0.8x1.9 □1x3.15 □1.7x5.4 □CF  □MF  其它_____";
                    if (("0.8x1.9,1x3.15,1.7x5.4").Contains(size)) Categories = Categories.Replace("□" + size, "■" + size);
                    if (("CF,MF").Contains(cfmf)) Categories = Categories.Replace("□" + cfmf, "■" + cfmf);
                    else if (cfmf == "其它") Categories = Categories.Replace("其它_____", "其它" + cfmf);
                    excel.fillExcelCellValue(ee.mWorkSheets["Template"], "A3", Categories);
                }
                else
                {
                    excel.fillExcelCellValue(ee.mWorkSheets["Template"], "E1", "請購單號: " + OrderCode);
                }
                //填入資料
                currPage = excel.report_dtPageMode(dt, ee, HeaderAndFooter, currPage, currPage > 1 ? false : true);
                currPage = currPage + 1;
                ee.mWSheet = ee.mWorkSheets["報表"];
                //輸出至印表機
                //string destPrinter = Constants.getProperty("ROReportPrinter", "Epson EPL-6200 (左下)");
                //excel.printExcelBySpecifyPrinter(ee, destPrinter);
                //excel.printExcel(ee.mWorkSheets["報表"]);
                //excel.saveToPdf(ee, outputFileName.Replace(".xlsx", "").Replace(".xls", "")); //, "報表"

                excel.closeExcel(ee, outputFileName, false);
                if(ifOpenExcel)
                {
                    ExcelElements ExcelForView = excel.openExcel(outputFileName);
                    ExcelForView.oXL.Visible = true;
                }
                //------------------------------------
                rtnStr = "原料申請表存檔完成";
                return 0;
            } catch(Exception ex)
            {
                rtnStr = ex.Message;
                return 1;
            }
        }
        public static int 繞線申請(List<KeyValuePair<string, string>> procurementParams, out string rtnStr, bool ifOpenExcel = true)
        {
            try
            {
                DateTime reportDate = DateTime.Now;
                string startDate = searchInKeyValList(procurementParams, "startDate");
                string endDate = searchInKeyValList(procurementParams, "endDate");
                //string rtnStr = "";
                //設定Excel範本路徑
                ExcelLib excel = null;
                string sqlStr = "", sqlStrOrderCode = "";
                string templateFolder = Constants.getProperty("繞線表格母板路徑", @"C:\procurement\母版\繞線申請");
                string outputFolder = Constants.getProperty("raoxianOutputPath"); //, @"C:\procurement"

                string templateFile = "";
                string outputFileName = "";

                string OrderCode = "", RequireDate = "";

                Dictionary<string, string> sqlParams = new Dictionary<string, string>();
                sqlParams.Add("@StartDate", startDate);
                sqlParams.Add("@EndDate", endDate);

                sqlStr = "SELECT A.`MFOID` 工令單號, A.`SIZE` 尺寸, A.RTYPE 型號, A.VAL 阻值, IF(LEFT(A.RTYPE, 2) = 'WA'," +
                   " '有帽白棒', A.`RO`) RO, A.Quant, A.`Note` 請購備註" + //, CONCAT(A.TOL, '%')
                   " FROM PRC_WiringApplication A" +
                   " WHERE A.ApplicationDate BETWEEN @StartDate AND @EndDate  and A.status > 0;";
                templateFile = Constants.getProperty("採購原料表格母板", "申請表.xlsx");
                outputFileName = outputFolder + @"\" + reportDate.ToString("yyyyMMddHHmmss") + "_繞線" + templateFile;

                int currPage = 1;
                if (excel == null)
                    excel = new ExcelLib();
                DataTable dt = CommonClass.getSQLDataTableParams(sqlStr, sqlParams, Constants.ProcurmentConnection);

                if (CommonClass.DataTableSum(dt, "Quant", "") == 0) // || dtOrderCode.Rows.Count == 0
                {
                    rtnStr = "查無記錄";
                    return 3;
                }
                //設定Excel檔參數
                ExcelElements ee = excel.openExcel(templateFolder + @"\" + templateFile);
                //ee.oXL.Visible = true;
                string formateJsonStr = "";
                formateJsonStr = "{ 'PageMode':'1','PageRows':'24','header':'1:24','pageDataRows':'21','dataRowStart':'3','columnEnd':'G',  'datePos': 'L3'}";

                //填入表頭和表尾
                Dictionary<string, string> HeaderAndFooter = JsonConvert.DeserializeObject<Dictionary<string, string>>(formateJsonStr);
                excel.fillExcelCellValue(ee.mWorkSheets["Template"], "E1", "請購單號: " + OrderCode);

                //填入資料
                currPage = excel.report_dtPageMode(dt, ee, HeaderAndFooter, currPage, currPage > 1 ? false : true);
                currPage = currPage + 1;
                ee.mWSheet = ee.mWorkSheets["報表"];
                //輸出至印表機
                //string destPrinter = Constants.getProperty("ROReportPrinter", "Epson EPL-6200 (左下)");
                //excel.printExcelBySpecifyPrinter(ee, destPrinter);
                //excel.printExcel(ee.mWorkSheets["報表"]);
                //excel.saveToPdf(ee, outputFileName.Replace(".xlsx", "").Replace(".xls", "")); //, "報表"

                excel.closeExcel(ee, outputFileName, false);
                if (ifOpenExcel)
                {
                    ExcelElements ExcelForView = excel.openExcel(outputFileName);
                    ExcelForView.oXL.Visible = true;
                }
                //------------------------------------
                rtnStr = "原料申請表存檔完成";
                return 0;
            }
            catch (Exception ex)
            {
                rtnStr = ex.Message;
                return 1;
            }
        }
        public static int printProcurementReport(List<KeyValuePair<string, string>> procurementParams, out string rtnStr, int retry = 0)
        {
            string dataType = searchInKeyValList(procurementParams, "dataType");
            rtnStr = "";
            switch (dataType)
            {
                case "採購申請":
                case "採購請料":
                    採購申請(procurementParams, out rtnStr);
                    break;
                case "繞線申請":
                    繞線申請(procurementParams, out rtnStr);
                    break;
            }
            return 0;
        }
        #endregion

        #region auto akeeba backup
        public static Dictionary<string, string> webBackupList = new Dictionary<string, string>()
        {
            {"台北Portal", @"http://192.168.1.33:8080/Portal/index.php?option=com_akeeba&view=Backup&key=2u4u+2u04yj3diyidianzu" },
            {"台北決策網", @"http://192.168.1.33:8080/statistics/index.php?option=com_akeeba&view=Backup&key=Diyidianzujuecewang" },
            {"Diggo",@"https://diggo.com.tw/index.php?option=com_akeeba&view=Backup&key=6Xq6Vd0hPzbifZpdX4cyUYi7ft5NIMq8"},
            {"FirstohmPay",@"https://pay.firstohm.com.tw/index.php?option=com_akeeba&view=Backup&key=2u4u+2u04yj3diyicianzu"},
            {"花蓮Portal", @"http://172.168.1.151:8080/Portal/index.php?option=com_akeeba&view=Backup&key=2u4u+2u04yj3diyidianzu" },
            {"portal29", "http://192.168.1.29:8080/portal/index.php?option=com_akeeba&view=Backup&key=BOH2y3m9h4YGuaj2BJhdsHnj6jdcOFI6 "}
        };

        public static void autoAkeebaBackup()
        {
            //List<string> listUrls = new List<string>()
            //{
            //    @"http://192.168.1.33:8080/Portal/index.php?option=com_akeeba&view=Backup&key=2u4u+2u04yj3diyidianzu",
            //    @"https://pay.firstohm.com.tw/index.php?option=com_akeeba&view=Backup&key=2u4u+2u04yj3diyicianzu",
            //    @"https://diggo.com.tw/index.php?option=com_akeeba&view=Backup&key=6Xq6Vd0hPzbifZpdX4cyUYi7ft5NIMq8",
            //    @"https://diggo.com.tw/index.php?option=com_akeeba&view=Backup&key=6Xq6Vd0hPzbifZpdX4cyUYi7ft5NIMq8",
            //};
            foreach (KeyValuePair<string, string> targerUrl in webBackupList)
            {
                akeebaBKPic.Clear();
                akeebaBKPic.Add(CommonClass.startChrom(targerUrl.Value, Constants.ChromeDir));
                CommonClass.wait(5);
            }
        }

        public static void killAkeebaProcess()
        {
            foreach (int pid in akeebaBKPic)
            {
                CommonClass.KillProcess(pid);
                CommonClass.wait(1);
            }
            akeebaBKPic.Clear();
        }

        public static void chkIP_AND_PropertyIP()
        {
            Constants.updQueueConn = Constants.getProperty("updQueueConn", Constants.ConnString);
            List<string> chkIpList = new List<string>() { "192.168.1.33", "192.168.1.29", "172.168.1.151", "172.168.1.35" };
            string localIP = CommonClass.GetLocalIPAddress();
            if (chkIpList.Contains(localIP))
            {
                string updServerIP = Constants.getProperty("UDPServer");
                if (!string.IsNullOrEmpty(updServerIP) && updServerIP != "127.0.0.1" && localIP != updServerIP)
                {
                    MessageBox.Show($"Property內的【UDPServer = {updServerIP}】與本機IP不符");
                    CommonClass.killProcessByName("TaskTrayApplication");
                }
                if (!Constants.updQueueConn.Contains(localIP))
                {
                    MessageBox.Show($"Property內的【updQueueConn = {Constants.updQueueConn}】與本機IP不符");
                    CommonClass.killProcessByName("TaskTrayApplication");
                }
                if (localIP == "172.168.1.151" || localIP == "172.168.1.35")
                {
                    if (!Constants.ConnString.Contains(localIP))
                    {
                        MessageBox.Show($"您的IP為:{localIP}\nConstants為:{Constants.ConnString}");
                        CommonClass.killProcessByName("TaskTrayApplication");
                    }
                }
            }
        }

        #endregion

        #region sst
        public static DateTime sendSSTStartLine = DateTime.Now.AddDays(-1); //sendSSTStartLine：false 代表 本日 do_sst 尚未開始
        //public static DateTime dateSendSSTStartLine = DateTime.Now;
        public static string loginTime = "";
        public static string logoutTime = "";
        public static string sstProcessStart = "";
        public static string sstProcessEnd = "";
        public static int sstProcessErrCnt = 0;
        public static int sstDayProcessErrCnt = 0;
        public static string sstLastErrTime = "";
        public static string sstLastErrMsg = "";
        #endregion

        #region Python AI
        public static void RunPythonScript(string pythonExePath,
        string scriptPath, string workingDirectory)
        {
            try
            {
                // 建立一個 ProcessStartInfo 對象來設定 Python 執行信息
                ProcessStartInfo start = new ProcessStartInfo
                {
                    FileName = pythonExePath, // Python 執行文件的完整路徑
                    Arguments = scriptPath, // 要執行的 Python 腳本的完整路徑
                    UseShellExecute = false, // 不使用操作系統 shell 來啓動程序
                    RedirectStandardOutput = true, // 重定向標準輸出
                    RedirectStandardError = true, // 重定向標準錯誤
                    CreateNoWindow = true, // 不創建窗口
                    WorkingDirectory = workingDirectory // 指定工作目錄
                };

                using (Process process = Process.Start(start))
                {
                    // 讀取標準輸出和標準錯誤
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        Console.WriteLine(result); // 輸出結果
                    }

                    using (StreamReader error = process.StandardError)
                    {
                        string errorMessage = error.ReadToEnd();
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            Console.WriteLine("Error: " + errorMessage); // 輸出錯誤信息
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message); // 處理例外
            }
        }

        public static void ExecutePrediction(string pythonPath, string scriptPath, string dataType,
            string loadPath, string startDate, string endDate, string ifOnline)
        {
            try
            {
                // Specify the path to the Python executable
                //string pythonPath = @"C:\Path\To\Python\python.exe";

                // Specify the path to the Python script
                //string scriptPath = @"C:\Path\To\YourScript\predict.py";

                // Create the process start info
                var psi = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = $"\"{scriptPath}\" \"{dataType}\" \"{loadPath}\" \"{startDate}\" \"{endDate}\"",  // Pass the arguments
                    RedirectStandardOutput = true,   // Redirect output to be able to read it
                    RedirectStandardError = true,
                    UseShellExecute = false,         // Do not use the OS shell
                    CreateNoWindow = true            // Do not create a window
                };

                // Start the process
                using (var process = Process.Start(psi))
                {
                    // Read the output
                    string output = process.StandardOutput.ReadToEnd();
                    string errors = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    // Optionally handle the output or errors
                    Console.WriteLine(output);
                    if (!string.IsNullOrEmpty(errors))
                    {
                        Console.WriteLine("Errors:");
                        Console.WriteLine(errors);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while executing the Python script: {ex.Message}");
            }
        }

        public static void ai_training()
        {
            string pythonPath = Constants.getProperty("pythonPath", @"D:\scott_home\MyStock\pythonTool");
            string AI_Work = Constants.getProperty("AI_Work", @"D:\scott_home\MyStock\pythonTool");
            //CommonClass.run_Python(pythonPath + @"\amt_ai_predictByModel.py", "2024-08-08 forest");
            RunPythonScript(pythonPath,
            $"{AI_Work}\\amt_forestTrain.py", AI_Work);

            RunPythonScript(pythonPath,
            $"{AI_Work}\\amt_xgboostTrain.py", AI_Work);

            ////////////////////////////////////////////////////////////////
            RunPythonScript(pythonPath,
            $"{AI_Work}\\deviation_forestTrain.py",
            AI_Work);

            RunPythonScript(pythonPath,
            $"{AI_Work}\\deviation_xgboostTrain.py",
            AI_Work);
        }

        public static void ai_predict(DateTime startDate, DateTime endDate, string ifOnline="1")
        {
            string pythonPath = Constants.getProperty("pythonPath", @"D:\scott_home\MyStock\pythonTool");
            string ai_work = Constants.getProperty("AI_Work", @"D:\scott_home\MyStock\pythonTool");
            //CommonClass.run_Python(pythonPath + @"\amt_ai_predictByModel.py", "2024-08-08 forest");
            //for(int i = 0; startDate.AddDays(i) < DateTime.Now; i++)
            //{
            CommonApp.ExecutePrediction(pythonPath,
                $"{ai_work}\\ai_predictByModel.py",
                "amt", ai_work, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), ifOnline);
            CommonApp.ExecutePrediction(pythonPath,
                $"{ai_work}\\ai_predictByModel.py",
                "deviation", ai_work, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), ifOnline);
        }
        #endregion
    }
}