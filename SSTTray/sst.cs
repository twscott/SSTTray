using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;

using Aspose.Cells;

using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2013.WebExtension;

using FirstOhm;

using Newtonsoft.Json;

using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

// Shioaji 改用 HTTP API（shioaji server daemon），不再使用 NuGet SDK
// 安裝方式: pip install shioaji
// 啟動方式: shioaji server start


namespace TaskTrayApplication
{
    class sst
    {
        public static List<DateTime> last10PanSlot;
        public static LineLib lineLib = null;
        public static string lastDate = null;
        public static string myIP = CommonClass.GetLocalIPAddress();
        //install Shioaji https://www.nuget.org/packages/Shioaji/
        //PM>  NuGet\Install-Package Shioaji -Version 0.0.8-dev2
        //public static string sstConnStr = null;
        public static string sstConnStr = Constants.LocalSSTV2ConnString;
        public static int panMinutes = 3; //分盤時間
        public static double panWeight = 0.6; //超過 30 分鐘,主進量加權
        public static string propertyPath = @"C:\sst\Property.txt";
        private static readonly HttpClient _httpClient = new HttpClient() { BaseAddress = new Uri("http://127.0.0.1:8080"), Timeout = TimeSpan.FromSeconds(30) };
        private static string _shioajiServerUrl = "http://127.0.0.1:8080";

        public static Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        static List<List<string>> tseStockList;
        static List<List<string>> otcStockList;
        public static int showAlert = 0;
        static DataTable activeStockDt = null;
        public static string currStockID = "";
        public static string currStockName = "";
        public static string currStockType = "";
        static DateTime shioajiRunoutTime = DateTime.Now.AddDays(-1);
        public static List<string> recommPerson = new List<string>()
            {"張震","李冠嶔","阮惠慈","鐘建安","鄭瑞宗","許豐祿","林全","林魚","林熱","林輕","榕",
                "羅","妝", "何金城", "林萬通","李營","李贏","嘉偉", "余正君",
                "哲維","江國中","王瞳", "周行",
             "林和彥","李蜀芳","林泓橡","王博","魏明裕","鄭偉群",
             "阮慕華","陳鳳馨","投本比","籌碼K線","投信認養","融資融券",
             "阿土伯","報紙","廣播","自選看漲","自選看跌","研究股","三代半導體","綠能",
             "低軌衛星","電動車","高速",
            "超低布林","連續上漲"};

        public static Dictionary<string, string> alertPriority = new Dictionary<string, string>()
            {
                {"0", "全部" },{"1", "看看" },{"2", "考慮" },{"3", "想買" },{"4", "準備下手" },{"5", "必買" },
                {"6", "自有" },{"7", "已賣" }
            };
        //即時股價 url
        //https://mis.twse.com.tw/stock/api/getStockInfo.jsp?ex_ch=otc_1258.tw|otc_1264.tw|tse_2330.tw|&json=1&delay=0
        //realTimeStockUrl 後面加 &ex_ch=tse_2330.tw|otc_9102.tw|
        static String realTimeStockUrl = "https://mis.twse.com.tw/stock/api/getStockInfo.jsp";
        static int lastTeacherEventID = 0;

        //public static void parseTeacherEvent(string jsonStr)
        //{
        //    List<Dictionary<string, string>> jStockList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonStr);
        //    string sqlStr = null;

        //    foreach (Dictionary<string, string> jStock in jStockList)
        //    {
        //        sqlStr = $"insert into `teacher_parse` (`event_ID`,`teacherName`,`stockid`," +
        //            $"`stockname`,`byorsell`,`reccPrice`,`currPrice`) " +
        //            $" Values ('{jStock["ID"]}', '{jStock["名字"] ?? ""}', '{jStock["股號"] ?? ""}', " +
        //            $" '{jStock["股名"] ?? ""}', '{jStock["買或賣"] ?? ""}', '{jStock["金額"] ?? ""}')";
        //        CommonClass.execSQLNonQuery(sqlStr);
        //    }
        //}

        //public static string proceessTeacherEvent(FirstOhm.WebClient foWebClient)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    string gptResult = null;
        //    while (true)
        //    {
        //        string sqlStr = "SELECT concat(a.`event_ID`, ',', REPLACE(a.`event_msg` , CHAR(10), ' ') ) eventMsg " +
        //        " FROM `teacher_event` a " +
        //        " left join teacher_parse b on a.`event_ID`=b.`event_ID` " +
        //        " where b.parse_ID is null and a.`autoType` in (1,2) " +
        //        " order by a.`event_ID` limit 5";
        //        DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.SSTConnString35);
        //        if (dt.Rows.Count > 0)
        //        {
        //            sb.Clear();
        //            sb.Append(@"請將一段文字簡化成 ID,名字,股名,金額,買或賣,停損金額 的Json list of objects 的格式, 
        //                    例如將以下的文字:101, 余:恭喜3346麗清再創新高價！另外，新會員平盤下買進8940新天地, 轉成以下的 Json
        //                    'ID':'101', '名字':'余','股號':'8940','股名':'新天地','金額':'', '買或賣':'買', '停損金額':''
        //                    以下為要分析的句子, 請轉換成上述的 Json 格式:
        //                ");
        //            foreach (DataRow dr in dt.Rows)
        //            {
        //                sb.Append($"{dr["eventMsg"]}".Replace(Environment.NewLine, " ")).Append("。");
        //            }
        //            gptResult = foWebClient.chatGdp_RestSharp_SyncMode("2wsx@#$%XCVB", sb.ToString());
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //    return gptResult;
        //}

        public static void syncTeacherEvent()
        {

            string sqlStr = "SELECT Max(`event_ID`) FROM `teacher_event` ";
            try
            {
                StringBuilder values = new StringBuilder();
                var maxIDObj = CommonClass.getSQLScalar(sqlStr, Constants.SSTConnString);
                if (maxIDObj == null || maxIDObj == DBNull.Value)
                    lastTeacherEventID = 0;
                else
                    lastTeacherEventID = Convert.ToInt32(maxIDObj);
                string diggoConn = $"Data Source =208.109.25.83; Password =LineTw2182; User ID =lineUser; Database =diggo; port = 3306; charset = utf8; convert zero datetime = True; SslMode = None; ";
                sqlStr = $"delete FROM `igc8d_uj_teacher_event` where `event_msg`=''";
                CommonClass.execSQLNonQuery(sqlStr, diggoConn);
                sqlStr = $"SELECT `event_ID`,`lineUserID`,`groupID`,`eventTime`,`event_msg`,`translate`,`created` FROM `igc8d_uj_teacher_event` where `event_ID` > '{lastTeacherEventID}' ";
                DataTable dt = CommonClass.getSQLDataTable(sqlStr, diggoConn);
                if (dt.Rows.Count == 0)
                    return;
                sqlStr = "INSERT ignore INTO `teacher_event`(`event_ID`, `lineUserID`, `groupID`, `eventTime`, `event_msg`) VALUES ";
                DataRow dr;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dr = dt.Rows[i];
                    values.Append($"('{dr["event_ID"]}','{dr["lineUserID"]}','{dr["groupID"]}', " +
                        $" '{dr["eventTime"]}', '{dr["event_msg"]}')");
                    if (i < dt.Rows.Count - 1)
                        values.Append(", ");
                }
                CommonClass.execSQLNonQuery(sqlStr + values.ToString(), Constants.SSTConnString);
                sqlStr = "delete FROM `teacher_event` WHERE length(`event_msg`) < 10";
                CommonClass.execSQLNonQuery(sqlStr, Constants.SSTConnString);

                sqlStr = "delete FROM `teacher_event` WHERE `event_msg` like '%生活投資%' ||  `event_msg` like '%感謝%' " +
                    " ||  `event_msg` like '%仁棟%'  ||  `event_msg` like '%早安%' " +
                    " ||  `event_msg` like '%錄音檔%' ||  `event_msg` like '%獲利出%' ||  `event_msg` like '%恭賀%' ";
                CommonClass.execSQLNonQuery(sqlStr, Constants.SSTConnString);

                sqlStr = $"update teacher_event set autoType=6, eventTime = FROM_UNIXTIME(eventTime/1000) " +
                    $" WHERE `event_msg` like '%理由%' and autoType = -1 ";
                CommonClass.execSQLNonQuery(sqlStr, Constants.SSTConnString);
                sqlStr = "update teacher_event set autoType=4, eventTime = FROM_UNIXTIME(eventTime/1000) " +
                    " WHERE hour(`created`) < 9 ";
                CommonClass.execSQLNonQuery(sqlStr, Constants.SSTConnString);
                sqlStr = "update teacher_event set autoType=5, eventTime = FROM_UNIXTIME(eventTime/1000)  " +
                    " WHERE hour(`created`) >= 16 ";
                CommonClass.execSQLNonQuery(sqlStr, Constants.SSTConnString);
                //sqlStr = "update teacher_event set autoType=0, eventTime = FROM_UNIXTIME(eventTime/1000)  " +
                //    " WHERE length(`event_msg`) <= 35 and length(`event_msg`) > 0 and autoType = -1 ";
                //CommonClass.execSQLNonQuery(sqlStr + values.ToString(), Constants.SSTConnString);
                sqlStr = "update teacher_event set autoType=1, eventTime = FROM_UNIXTIME(eventTime/1000)  " +
                    " WHERE length(`event_msg`) < 100 and (`event_msg` like '%逢低%' or `event_msg` like '%買進%' " +
                    " or `event_msg` like '%轉進%') and autoType = -1 ";
                CommonClass.execSQLNonQuery(sqlStr, Constants.SSTConnString);
                sqlStr = "update teacher_event set autoType=2, eventTime = FROM_UNIXTIME(eventTime/1000)  " +
                    " WHERE length(`event_msg`) < 100 and (`event_msg` like '%獲利%' or `event_msg` like '%調整%' " +
                    "  or event_msg like '%收回%' or `event_msg` like '%賣出%' or `event_msg` like '%持股賣%' " +
                    "  or `event_msg` like '%各賣%' or `event_msg` like '%掛單%') " +
                    "";
                CommonClass.execSQLNonQuery(sqlStr, Constants.SSTConnString);
                sqlStr = "update teacher_event set autoType=1, eventTime = FROM_UNIXTIME(eventTime/1000)  " +
                    " WHERE length(`event_msg`) < 100 and (`event_msg` like '%小買%' or `event_msg` like '%買回%' " +
                    " or `event_msg` like '%各買%' or `event_msg` like '%買入%' or `event_msg` like '%買進%' " +
                    " or `event_msg` like '%加碼%'  or`event_msg` like '%部位%' or `event_msg` like '%建立%' " +
                    " or `event_msg` like '%可買%' or `event_msg` like '%小買%') ";
                CommonClass.execSQLNonQuery(sqlStr, Constants.SSTConnString);
                sqlStr = "update teacher_event set autoType=1, eventTime = FROM_UNIXTIME(eventTime/1000)  " +
                    " WHERE length(`event_msg`) < 100 and (`event_msg` like '%轉買%' or `event_msg` like '%買回%' " +
                    " or `event_msg` like '%各買%' or `event_msg` like '%買入%' or `event_msg` like '%買進%' " +
                    " or`event_msg` like '%加碼%'  or`event_msg` like '%部位%') ";
                CommonClass.execSQLNonQuery(sqlStr, Constants.SSTConnString);
                sqlStr = "update teacher_event set autoType=7, eventTime = FROM_UNIXTIME(eventTime/1000)  " +
                    " WHERE length(`event_msg`) >= 100  and autoType = -1";
                CommonClass.execSQLNonQuery(sqlStr, Constants.SSTConnString);
                sqlStr = "update teacher_event set autoType=11, eventTime = FROM_UNIXTIME(eventTime/1000)  " +
                    " where `event_msg` like '%存股%' ";
                CommonClass.execSQLNonQuery(sqlStr, Constants.SSTConnString);
                sqlStr = "update teacher_event set autoType=12, eventTime = FROM_UNIXTIME(eventTime/1000)  " +
                    " where `event_msg` like '%台指%' ";
                CommonClass.execSQLNonQuery(sqlStr, Constants.SSTConnString);
                sqlStr = "update teacher_event set autoType=10, eventTime = FROM_UNIXTIME(eventTime/1000)  " +
                    " where `event_msg` like '%期指%' or `event_msg` like '%(期)%' ";
                CommonClass.execSQLNonQuery(sqlStr, Constants.SSTConnString);
                sqlStr = "update teacher_event set autoType=14, eventTime = FROM_UNIXTIME(eventTime/1000)  " +
                    " where `event_msg` like '%強勢%' and autoType<>1 and autoType<>2 and autoType<>13  ";
                CommonClass.execSQLNonQuery(sqlStr, Constants.SSTConnString);
            }
            catch (Exception ex)
            {
                string mailBody = $"時間: {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}{System.Environment.NewLine}" +
                             $"syncTeacherEvent() Exception " + Environment.NewLine +
                             $"error SQL : {sqlStr}" + Environment.NewLine +
                             $"error Msg : {ex.Message}" + Environment.NewLine +
                             $"stackTrace : {ex.StackTrace}";
                CommonClass.sendmailEazy(mailBody, "3", "scott.tseng@firstohm.com.tw", $"syncTeacherEvent() Exception: {ex.Message}");
            }

        }

        //每天早上準備 investBae 資料
        public static void dailyInitInvestBase(string lastDate)
        {
            String sqlStr = null;
            try
            {
                sqlStr = $"insert ignore into `investbase` " +
                    $" (`recDate`,`StockID`,`stockName`,`StockType`,`currPrice`, " +
                    $" `lastPrice`,`lastVol`, `lastDate`, `onTimePrice`) " +
                    $" " +
                    $" select CURRENT_DATE, `StockID`,`stockName`,`StockType`, `EndPrice`, " +
                    $" `EndPrice`,`Vol`, `StockDate`, EndPrice " +
                    $" from weekall a " +
                    $" inner join (select Max(StockDate) maxStockDate from weekall ) b on a.`StockDate`=b.maxStockDate " +
                    $" on duplicate key update `recDate` = VALUES(`recDate`), " +
                    $" `stockName` = VALUES(`stockName`), `StockType` = VALUES(`StockType`), " +
                    $" `currPrice` = VALUES(`currPrice`), `lastPrice` = VALUES(`lastPrice`), " +
                    $" `lastVol` = VALUES(`lastVol`), `lastDate` = VALUES(`lastDate`), " +
                    $" `onTimePrice` = VALUES(`onTimePrice`) ";
                CommonClass.execSQLNonQuery(sqlStr);
            }
            catch(Exception ex)
            {
                //do nothing
            }
            sqlStr = "update `investbase` a " +
                $" inner join stock60days b on a.`StockID`=b.StockID and b.`StockDate` = '{lastDate}' " +
                " set " +
                " a.`avgAmt5D`=b.`MA5`, a.`avgVol5D`= b.MV5, " +
                $" a.`currPrice` = b.EndPrice, a.`lastPrice` = b.EndPrice, " +
                $" `lastVol` = b.Vol, a.`lastDate` = b.StockDate , " +
                $" `onTimePrice` = b.EndPrice, " +
                " a.`onTimeVol`=0, `momentAVDVol`=0, a.`lastVolRate`=0, a.`avg5VolRate`=0, " +
                " `dailyNoPriceCnt`=0, a.`instantMass`=0, a.`instantRise`=0, " +
                " a.`instantFall`=0, a.`messRise`=0, a.`messFall`=0, " +
                " a.`vol0921`=0, a.`priceDiffRate0921`=0, a.panVol5Cnt=0, " +
                " a.panVol10Cnt=0, a.panVol5Dict=null, a.panVol5CntPos=0,  " +
                " a.panVol5CntNeg=0, a.`panVol5QuanPos`=0, a.`panVol5QuanNeg`=0, " +
                " a.`panVol5QuanDiff`=0, a.panVol50CntPos=0, a.panVol50CntNeg=0";
            CommonClass.execSQLNonQuery(sqlStr);
            //try
            //{
            //    sqlStr = $"SELECT Max(`StockDate`) FROM `weekall` where `StockDate` < '{DateTime.Now.ToString("yyyy-MM-dd")}' ";
            //    lastDate = Convert.ToDateTime(CommonClass.getSQLScalar(sqlStr, sst.sstConnStr)).ToString("yyyy-MM-dd");

            //    sqlStr = $"insert ignore into `investbase` " +
            //            $"(`recDate`,`StockID`,`stockName`,`StockType`,`currPrice`, " +
            //            $" `lastPrice`,`lastVol`, `lastDate` ) " +
            //            $" select '{DateTime.Now.ToString("yyyy-MM-dd")}', `StockID`,`stockName`,`StockType`, " +
            //            $" `StockPrice`,`StockPrice`,`Vol`, `TransDate` " +
            //            $" from tradedata a " +
            //            $" inner join (select Max(TransDate) maxTransDate from tradedata ) b " +
            //            $" on a.`TransDate`=b.maxTransDate" +
            //            $" on duplicate key update " +
            //            $" `recDate` = VALUES(`recDate`), " +
            //            $" `stockName` = VALUES(`stockName`), " +
            //            $" `StockType` = VALUES(`StockType`), " +
            //            $" `currPrice` = VALUES(`currPrice`), " +
            //            $" `lastPrice` = VALUES(`lastPrice`), " +
            //            $" `lastVol` = VALUES(`lastVol`), " +
            //            $" `lastDate` = VALUES(`lastDate`)";
            //    CommonClass.execSQLNonQuery(sqlStr);
            //}
            //catch (Exception ex) { }
        }


        //parseType:1 全部， 2:僅 Parse 當天 recommand 的股票
        public static void do_sst(int currHour, int currMin, string dbConnection = null, int parseType = 1)
        {
            string sqlStr = null;
            DateTime currentTime = DateTime.Now;
            if (string.IsNullOrEmpty(dbConnection))
                sst.sstConnStr = Constants.SSTConnString;
            else
                sst.sstConnStr = dbConnection;

            if (myIP.Contains("192.168.3.") || CommonClass.isHoliday(DateTime.Now, true))
                return;
            if (lastDate == null)
            {
                sqlStr = $"SELECT Max(`StockDate`) FROM `weekall` where `StockDate` < '{DateTime.Now.ToString("yyyy-MM-dd")}' ";
                lastDate = Convert.ToDateTime(CommonClass.getSQLScalar(sqlStr, sst.sstConnStr)).ToString("yyyy-MM-dd");
            }

            //每天早上 8：30 
            if (currHour == 8 && currMin == 45)
            {
                sqlStr = $"SELECT Max(`StockDate`) FROM `weekall` where `StockDate` < '{DateTime.Now.ToString("yyyy-MM-dd")}' ";
                lastDate = Convert.ToDateTime(CommonClass.getSQLScalar(sqlStr, sst.sstConnStr)).ToString("yyyy-MM-dd");

                sst.genRecommand(DateTime.Now.ToString("yyyy-MM-dd"));
                if (sst.last10PanSlot == null)
                    sst.last10PanSlot = new List<DateTime>();
                else
                    sst.last10PanSlot.Clear();
            }

            //每天 16：10 同步第一次
            //if (DateTime.Now.Hour == 14 && DateTime.Now.Minute == 50)
            //    sst.syncClosingPrice();

            //每天 18：20 同步第二次 
            if (currHour == 15 && currMin == 6)
            {
                //string sqlStr = $"select count(*) from weekall where `StockDate` = '{DateTime.Now.ToString("yyyy-MM-dd")}' ";
                //if (Convert.ToInt32(CommonClass.getSQLScalar(sqlStr, dbConnection)) < 1000)
                //    sst.syncClosingPrice();
                lastDate = null;
            }

            if (currentTime.TimeOfDay >= new TimeSpan(8, 50, 0)
                && currentTime.TimeOfDay <= new TimeSpan(8, 55, 0))
            {
                    dailyInitInvestBase(lastDate);
            }

            //parseType:1 全部， 2:僅 Parse 當天 recommand 的股票 
            //9:00 以後才會進 shioji
            if (!myIP.Contains("192.168.3.") && !CommonClass.isHoliday(DateTime.Now, true)
                && DateTime.Now.TimeOfDay >= TimeSpan.Parse("9:01:00")
                && DateTime.Now.TimeOfDay <= TimeSpan.Parse("15:05:00"))
            {
                stockAlertLog(parseType, sst.sstConnStr);
                CommonClass.wait(15);
                DataTable dt;
                if (currentTime.TimeOfDay >= new TimeSpan(8, 50, 0)
                    && currentTime.TimeOfDay <= new TimeSpan(13, 30, 0))
                {
                    sqlStr = $"SELECT count(*) cnt  FROM `investbase` where recDate = CURRENT_DATE";
                    dt = CommonClass.getSQLDataTable(sqlStr);
                    if (dt.Rows.Count == 0 || Convert.ToInt32(dt.Rows[0][0]) < 10)
                    {
                        dailyInitInvestBase(lastDate);
                    }

                }

                sqlStr = $"SELECT a.`Log_ID` FROM `alertlog` a " +
                    $" inner join tradedata b on a.`lastDate`=b.TransDate and " +
                    $" a.StockID = b.StockID and a.currVol = b.Vol and a.`CurrPrice`=b.StockPrice " +
                    $" where Time(preTime)=time('09:00') and a.alertDate = CURRENT_DATE ";
                dt = CommonClass.getSQLDataTable(sqlStr);
                if (dt.Rows.Count > 0)
                {
                    List<string> logid_List = CommonClass.dtToList(dt, 0);
                    sqlStr = $"delete from alertlog where Log_ID in ('{string.Join("','", logid_List)}')";
                    CommonClass.execSQLNonQuery(sqlStr);
                }
            }
        }

        public static void sendLine0921()
        {

            string lineMsg = null;
            string sqlStr = "SELECT * FROM `alertlogmaxtime` ";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            if (Convert.ToDateTime(dt.Rows[0][0]) < DateTime.Now.Date)
            {
                lineMsg = $"sstStry Error : Max alert time : {dt.Rows[0][0]}";
            }
            else
            {
                lineMsg = $"Max alert time : {dt.Rows[0][0]} {Environment.NewLine}";
                sqlStr = "SELECT " +
                    " CASE WHEN (`onTimePrice`-`lastPrice`)/`lastPrice` >= 0.097 THEN '1.漲停' " +
                    " WHEN (`onTimePrice`-`lastPrice`)/`lastPrice` <= -0.097 THEN '5.跌停' " +
                    " WHEN (`onTimePrice`-`lastPrice`) >= 0.5 THEN '2.漲' " +
                    " WHEN (`onTimePrice`-`lastPrice`) < 0.5 && (`onTimePrice`-`lastPrice`) > -0.5 THEN '3.平盤' " +
                    " WHEN (`onTimePrice`-`lastPrice`) <= -0.5 THEN '4.跌' END AS score_group, COUNT(*) AS count " +
                    " FROM investbase GROUP BY score_group order by score_group ";
                dt = CommonClass.getSQLDataTable(sqlStr);
                foreach (DataRow dr in dt.Rows)
                {
                    lineMsg += $"{dr[0]}： {dr[1]} {Environment.NewLine}";
                }
                lineMsg += $"{Environment.NewLine}{Environment.NewLine}我的股票{Environment.NewLine}";
                sqlStr = "SELECT CASE WHEN (`onTimePrice`-`lastPrice`)/`lastPrice` >= 0.097 THEN '1.漲停' " +
                    " WHEN (`onTimePrice`-`lastPrice`)/`lastPrice` <= -0.097 THEN '5.跌停' " +
                    " WHEN (`onTimePrice`-`lastPrice`) >= 0.5 THEN '2.漲' " +
                    " WHEN (`onTimePrice`-`lastPrice`) < 0.5 && (`onTimePrice`-`lastPrice`) > -0.5 THEN '3.平盤' " +
                    " WHEN (`onTimePrice`-`lastPrice`) <= -0.5 THEN '4.跌' END AS score_group, COUNT(*) AS count " +
                    " FROM investbase a " +
                    " inner join (SELECT distinct `StockID` FROM `buyin` where `stockLeftCount` > 0 ) b on a.StockID = b.StockID " +
                    " GROUP BY score_group order by score_group";
                dt = CommonClass.getSQLDataTable(sqlStr);
                foreach (DataRow dr in dt.Rows)
                {
                    lineMsg += $"{dr[0]}： {dr[1]} {Environment.NewLine}";
                }
            }
            if (lineLib == null)
                lineLib = new LineLib();
            lineLib.pushTextMessageByWebapiAsync("scott.tseng", $" sendLine0921() {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {Environment.NewLine} {lineMsg}");
            CommonApp.sendSSTStartLine = DateTime.Now;
            //CommonApp.dateSendSSTStartLine = DateTime.Now.Date;
        }

        //parseType:1 全部， 2:僅 Parse 當天 recommand 的股票
        public static void stockAlertLog(int parseType = 1, string dbConnection = null)
        {
            string sqlStr;
            if (string.IsNullOrEmpty(dbConnection))
                sst.sstConnStr = Constants.SSTConnString;
            else
                sst.sstConnStr = dbConnection;

            try
            {
                if (myIP.Contains("192.168.3.") || CommonClass.isHoliday(DateTime.Now, true))
                    return;

                //Alert 的關注檔
                if (DateTime.Now.Hour == 9 && DateTime.Now.Minute <= 11)
                    sst.initDaylySpotliteStock(ref lastDate);
                DataTable dt = sst.getAlertSource(ref lastDate, parseType);
                if (dt == null || dt.Rows.Count == 0)
                    return;
                //if(dt.Rows.Count < 10)
                //{
                //    sqlStr = $"SELECT Max(`StockDate`) FROM `weekall` where `StockDate` < '{lastDate}' ";
                //    lastDate = Convert.ToDateTime(CommonClass.getSQLScalar(sqlStr, sst.connStr)).ToString("yyyy-MM-dd");
                //    dt = sst.getAlertSource(ref lastDate);
                //}
                //sst.stockAlert(dt, lastDate);
                shioajiStockAlert(dt, lastDate, parseType);
            }
            catch (Exception ex)
            {
                CommonApp.sstLastErrTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                CommonApp.sstLastErrMsg = "sst.stockAlertLog() " + ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace;
            }
        }

        //public static string boolinDirection(string inputStr, bool ifEqualShows = true)
        //{
        //    if (inputStr.Contains("↗"))
        //        return "+";
        //    else if (inputStr.Contains("↘"))
        //        return "-";
        //    else if (ifEqualShows)
        //        return "=";
        //    else
        //        return "";

        //}

        /// <summary>
        /// 取得即時股價
        /// </summary>
        /// <param name="inModel"></param>
        /// <returns></returns>
        //public static List<MsgArray> GetRealtimePrice(FormIn inModel)
        //{
        //    List<MsgArray> rtnList = new List<MsgArray>();
        //    try
        //    {
        //        GetRealtimePriceOut outModel = new GetRealtimePriceOut();
        //        outModel.ErrorMsg = "";

        //        List<string> ExCode = new List<string>();
        //        List<string> TempLisy = null;
        //        string[] symbols = null;
        //        if (!string.IsNullOrEmpty(inModel.Q_SYMBOL_1))
        //        {
        //            //上市
        //            symbols = inModel.Q_SYMBOL_1.Split(',');
        //            foreach (string symbol in symbols)
        //            {
        //                ExCode.Add("tse_" + symbol + ".tw");
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(inModel.Q_SYMBOL_3))
        //        {
        //            //上櫃
        //            symbols = inModel.Q_SYMBOL_3.Split(',');
        //            foreach (string symbol in symbols)
        //            {
        //                ExCode.Add("otc_" + symbol + ".tw");
        //            }
        //        }
        //        string url = null;
        //        string downloadedData = "";
        //        for (int i = 0; i < ExCode.Count; i = i + 20)
        //        {
        //            try
        //            {
        //                // 呼叫網址
        //                TempLisy = ExCode.GetRange(i, (i + 20 < ExCode.Count ? 20 : ExCode.Count - i));
        //                url = realTimeStockUrl + $"?json=1&delay=0&ex_ch={String.Join("|", TempLisy)}";
        //                using (System.Net.WebClient wClient = new System.Net.WebClient())
        //                {
        //                    // 取得網頁資料
        //                    wClient.Encoding = Encoding.UTF8;
        //                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        //                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
        //                    downloadedData = wClient.DownloadString(url);
        //                }
        //                TwsePriceSchema jsonPrice = null;
        //                if (downloadedData.Trim().Length > 0)
        //                {
        //                    jsonPrice = JsonConvert.DeserializeObject<TwsePriceSchema>(downloadedData);
        //                    if (jsonPrice.rtcode != "0000")
        //                    {
        //                        throw new Exception("取商品價格失敗: " + jsonPrice.rtmessage);
        //                    }
        //                }

        //                if (jsonPrice == null)
        //                    continue;
        //                else
        //                    rtnList.AddRange(jsonPrice.msgArray);
        //            }
        //            catch (Exception ex)
        //            {
        //                string mailBody = $"時間: {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}{System.Environment.NewLine}" +
        //                        $"SST 雲端存取櫃櫃資料 發生 Exception" +
        //                        $"錯誤原因: {ex.Message}";
        //                CommonClass.sendmailEazy(mailBody, "3", "scott.tseng@firstohm.com.tw", $"SST 雲端存取櫃櫃資料 發生 Exception: {ex.Message}");
        //                continue;
        //            }
        //        }
        //        return rtnList;
        //    }
        //    catch (Exception ex)
        //    {
        //        return rtnList;
        //    }
        //}


        /// <summary>
        /// 每日收盤行情(僅上市資料)
        /// </summary>
        /// <param name="inModel"></param>
        /// <returns></returns>
        //public GetDayPriceOut getDaylyPrice(FormIn inModel)
        //{
        //    try
        //    {
        //        GetDayPriceOut outModel = new GetDayPriceOut();
        //        outModel.ErrorMsg = "";

        //        // 呼叫網址
        //        string twseUrl = "https://www.twse.com.tw/exchangeReport/MI_INDEX";
        //        string download_url = twseUrl + "?response=csv&date=" + inModel.Q_DATE_2 + "&type=ALL";
        //        string downloadedData = "";
        //        using (System.Net.WebClient wClient = new System.Net.WebClient())
        //        {
        //            // 網頁回傳
        //            downloadedData = wClient.DownloadString(download_url);
        //        }
        //        if (downloadedData.Trim().Length > 0)
        //        {
        //            // 回傳前端的資料集
        //            outModel.gridList = new List<StockPriceRow>();
        //            string[] lineStrs = downloadedData.Split('\n');
        //            for (int i = 0; i < lineStrs.Length; i++)
        //            {
        //                string strline = lineStrs[i];
        //                if (strline.Trim().Length == 0)
        //                {
        //                    continue;
        //                }

        //                // 排除非價格部份
        //                if (strline.IndexOf("證券代號") > -1 || strline.IndexOf("(元,股)") > -1)
        //                {
        //                    continue;
        //                }
        //                if (strline.Substring(0, 1) == "=")
        //                {
        //                    strline = strline.TrimStart('=');
        //                }

        //                ArrayList resultLine = new ArrayList();
        //                // 解析資料
        //                this.ParseCSVData(resultLine, strline);
        //                string[] datas = (string[])resultLine.ToArray(typeof(string));

        //                //檢查資料內容
        //                if (datas.Length != 17)
        //                {
        //                    continue;
        //                }

        //                // 股票代碼
        //                string symbolCode = datas[0];

        //                if (symbolCode.Length == 4)
        //                {
        //                    // 輸出資料
        //                    StockPriceRow row = new StockPriceRow();
        //                    row.symbolCode = symbolCode; //股票代碼 
        //                    row.symbolName = datas[1]; //股票名稱
        //                    row.open = datas[5]; //開盤價
        //                    row.high = datas[6]; //最高價
        //                    row.low = datas[7]; //最低價
        //                    row.close = datas[8]; //收盤價
        //                    row.volume = datas[2]; //成交量
        //                    outModel.gridList.Add(row);
        //                }
        //            }
        //            return outModel;
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// 當月各日成交資訊(僅上市資料)
        ///// </summary>
        ///// <param name="inModel"></param>
        ///// <returns></returns>
        //public GetMonthPriceOut getMonthlyPrice(FormIn inModel)
        //{
        //    try
        //    {
        //        GetMonthPriceOut outModel = new GetMonthPriceOut();
        //        outModel.ErrorMsg = "";

        //        // 呼叫網址
        //        string download_url = "http://www.twse.com.tw/exchangeReport/STOCK_DAY?response=csv&date=" + inModel.Q_MONTH_3 + "&stockNo=" + inModel.Q_SYMBOL_3;
        //        string downloadedData = "";
        //        using (System.Net.WebClient wClient = new System.Net.WebClient())
        //        {
        //            // 網頁回傳
        //            downloadedData = wClient.DownloadString(download_url);
        //        }
        //        if (downloadedData.Trim().Length > 0)
        //        {
        //            outModel.gridList = new List<StockPriceRow>();
        //            string[] lineStrs = downloadedData.Split('\n');
        //            for (int i = 0; i < lineStrs.Length; i++)
        //            {
        //                string strline = lineStrs[i];
        //                if (i == 0 || i == 1 || strline.Trim().Length == 0)
        //                {
        //                    continue;
        //                }
        //                // 排除非價格部份
        //                if (strline.IndexOf("說明") > -1 || strline.IndexOf("符號") > -1 || strline.IndexOf("統計") > -1 || strline.IndexOf("ETF") > -1)
        //                {
        //                    continue;
        //                }

        //                ArrayList resultLine = new ArrayList();
        //                // 解析資料
        //                ParseCSVData(resultLine, strline);
        //                string[] datas = (string[])resultLine.ToArray(typeof(string));

        //                //檢查資料內容
        //                if (Convert.ToInt32(datas[1].Replace(",", "")) == 0 || datas[3] == "--" || datas[4] == "--" || datas[5] == "--" || datas[6] == "--")
        //                {
        //                    continue;
        //                }

        //                // 輸出資料
        //                StockPriceRow row = new StockPriceRow();
        //                row.date = datas[0]; //日期
        //                row.open = datas[3]; //開盤價
        //                row.high = datas[4]; //最高價
        //                row.low = datas[5]; //最低價
        //                row.close = datas[6]; //收盤價
        //                row.volume = datas[1]; //成交量
        //                outModel.gridList.Add(row);
        //            }
        //            return outModel;
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// 投本比
        ///// </summary>
        ///// <param name="inModel"></param>
        ///// <returns></returns>
        //public GetDayPriceOut getInVestRate(FormIn inModel)
        //{
        //    try
        //    {
        //        GetDayPriceOut outModel = new GetDayPriceOut();
        //        outModel.ErrorMsg = "";

        //        // 呼叫網址
        //        string twseUrl = "https://www.twse.com.tw/exchangeReport/MI_INDEX";
        //        string download_url = twseUrl + "?response=csv&date=" + inModel.Q_DATE_2 + "&type=ALL";
        //        string downloadedData = "";
        //        using (System.Net.WebClient wClient = new System.Net.WebClient())
        //        {
        //            // 網頁回傳
        //            downloadedData = wClient.DownloadString(download_url);
        //        }
        //        if (downloadedData.Trim().Length > 0)
        //        {
        //            // 回傳前端的資料集
        //            outModel.gridList = new List<StockPriceRow>();
        //            string[] lineStrs = downloadedData.Split('\n');
        //            for (int i = 0; i < lineStrs.Length; i++)
        //            {
        //                string strline = lineStrs[i];
        //                if (strline.Trim().Length == 0)
        //                {
        //                    continue;
        //                }

        //                // 排除非價格部份
        //                if (strline.IndexOf("證券代號") > -1 || strline.IndexOf("(元,股)") > -1)
        //                {
        //                    continue;
        //                }
        //                if (strline.Substring(0, 1) == "=")
        //                {
        //                    strline = strline.TrimStart('=');
        //                }

        //                ArrayList resultLine = new ArrayList();
        //                // 解析資料
        //                this.ParseCSVData(resultLine, strline);
        //                string[] datas = (string[])resultLine.ToArray(typeof(string));

        //                //檢查資料內容
        //                if (datas.Length != 17)
        //                {
        //                    continue;
        //                }

        //                // 股票代碼
        //                string symbolCode = datas[0];

        //                if (symbolCode.Length == 4)
        //                {
        //                    // 輸出資料
        //                    StockPriceRow row = new StockPriceRow();
        //                    row.symbolCode = symbolCode; //股票代碼 
        //                    row.symbolName = datas[1]; //股票名稱
        //                    row.open = datas[5]; //開盤價
        //                    row.high = datas[6]; //最高價
        //                    row.low = datas[7]; //最低價
        //                    row.close = datas[8]; //收盤價
        //                    row.volume = datas[2]; //成交量
        //                    outModel.gridList.Add(row);
        //                }
        //            }
        //            return outModel;
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //private void ParseCSVData(ArrayList result, string data)
        //{
        //    int position = -1;
        //    while (position < data.Length)
        //        result.Add(ParseCSVField(ref data, ref position));
        //}

        //private string ParseCSVField(ref string data, ref int StartSeperatorPos)
        //{
        //    if (StartSeperatorPos == data.Length - 1)
        //    {
        //        StartSeperatorPos++;
        //        return "";
        //    }

        //    int fromPos = StartSeperatorPos + 1;
        //    if (data[fromPos] == '"')
        //    {
        //        int nextSingleQuote = GetSingleQuote(data, fromPos + 1);
        //        StartSeperatorPos = nextSingleQuote + 1;
        //        string tempString = data.Substring(fromPos + 1, nextSingleQuote - fromPos - 1);
        //        tempString = tempString.Replace("'", "''");
        //        return tempString.Replace("\"\"", "\"");
        //    }

        //    int nextComma = data.IndexOf(',', fromPos);
        //    if (nextComma == -1)
        //    {
        //        StartSeperatorPos = data.Length;
        //        return data.Substring(fromPos);
        //    }
        //    else
        //    {
        //        StartSeperatorPos = nextComma;
        //        return data.Substring(fromPos, nextComma - fromPos);
        //    }
        //}

        //private int GetSingleQuote(string data, int SFrom)
        //{
        //    int i = SFrom - 1;
        //    while (++i < data.Length)
        //        if (data[i] == '"')
        //        {
        //            if (i < data.Length - 1 && data[i + 1] == '"')
        //            {
        //                i++;
        //                continue;
        //            }
        //            else
        //                return i;
        //        }
        //    return -1;
        //}

        //public static DataTable getInvestStocks()
        //{
        //    string sqlStr = " SELECT `StockID`,`stockName`,`StockType`, 'IV' `stype`, 0 `stockLeftCount`, " +
        //        " currPrice `BuyInPoint`, currPrice, `lastVol` currVol, 0 thisVol, lastPrice, lastVol, " +
        //        " `avgAmt5D`,`avgVol5D`, avgAmt10D, avgAmt20D, avgAmtSeason, onTimePrice, onTimeVol, " +
        //        " momentAVDVol, reason `recommandBy`, 0 if20Hight, priority, updated, " +
        //        " if(Date(`updated`) = CURRENT_DATE, instantMass, 0) instantMass, " +
        //        " if(Date(`updated`) = CURRENT_DATE, instantRise, 0) instantRise, " +
        //        " if(Date(`updated`) = CURRENT_DATE, instantFall, 0) instantFall, " +
        //        " if(Date(`updated`) = CURRENT_DATE, messRise, 0) messRise, " +
        //        " if(Date(`updated`) = CURRENT_DATE, messFall, 0) messFall " +
        //        " FROM `investbase` " +
        //        $" WHERE `reason` like '%買%' or `reason` like '%轉%'";
        //    DataTable investStockDt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
        //    tseStockList = CommonClass.dtToListListWithCondition(investStockDt, "StockType='上市'", "StockID", 200, true);
        //    otcStockList = CommonClass.dtToListListWithCondition(investStockDt, "StockType='上櫃'", "StockID", 200, true);
        //    return investStockDt;
        //}

        //public static DataTable getActiveStocks()
        //{
        //    string sqlStr = "SELECT `BuyIn_ID`,`DataDate`,`StockID`,`stockName`,`StockType`," +
        //    " `stype`,`stockLeftCount`,`BuyInPoint`, 0.0 currPrice, 0 currVol, 0 thisVol, " +
        //    " lastPrice, lastVol, `avgAmt5D`,`avgVol5D`, avgAmt10D, avgAmt20D, avgAmtSeason, " +
        //    " onTimePrice, onTimeVol, momentAVDVol, " +
        //    " `recommandBy`, if20Hight, priority, updated, " +
        //    " if(Date(`updated`) = CURRENT_DATE, instantMass, 0) instantMass, " +
        //    " if(Date(`updated`) = CURRENT_DATE, instantRise, 0) instantRise, " +
        //    " if(Date(`updated`) = CURRENT_DATE, instantFall, 0) instantFall, " +

        //    " if(Date(`updated`) = CURRENT_DATE, messRise, 0) messRise, " +
        //    " if(Date(`updated`) = CURRENT_DATE, messFall, 0) messFall " +
        //    " FROM `activestocks` ";
        //    List<string> distinctCols = new List<string>() { "BuyIn_ID" };
        //    activeStockDt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
        //    //activeStockDt = CommonClass.DataTableDistinct(activeStockDt, distinctCols);
        //    tseStockList = CommonClass.dtToListListWithCondition(activeStockDt, "StockType='上市'", "StockID", 200, true);
        //    otcStockList = CommonClass.dtToListListWithCondition(activeStockDt, "StockType='上櫃'", "StockID", 200, true);
        //    return activeStockDt;
        //}

        //parseType:1 全部， 2:僅 Parse 當天 recommand 的股票

        public static DataTable getAlertSource(ref string lastDate, int parseType = 1)
        {
            //欄位：lastPrice 昨日收盤價
            //欄位：lastVol 昨日收盤量
            //currPrice 昨日收盤價(同lastPrice)
            //currVol:昨日收盤量(同昨日收盤量)
            //onTimePrice:現價
            //onTimeVol:現量
            //string sqlStr = "SELECT a.*, b.OpenPriec lastOpenPriec " +
            //    " FROM `uniq_alertsource` a " +
            //    $" left Join weekall b on a.`StockID`= b.StockID and b.StockDate = '{lastDate}' " +
            //    " where a.recommandBy like '%買%' || a.`stype` in ('現股', '推薦') ";
            string sqlStr = null;
            DataTable dt = null;
            string whereStr = "";
            //過了下午一點半, 就改成 3 分鐘一次， 把當天的剩下的 quata 都給 興櫃
            //parseType = DateTime.Now.TimeOfDay >= TimeSpan.Parse("13:31:00")?1: parseType;

            if (DateTime.Now.TimeOfDay >= TimeSpan.Parse("13:31:00"))
                whereStr = " where a.StockType='興櫃'";

            sqlStr = $"SELECT a.*, b.StockType , '' stype, '' recommandBy , b.OpenPriec lastOpenPriec, b.`HPrice` D1HPrice, " +
                $" b.`LPrice` D1LPrice, c.`HPrice` D2HPrice, c.`LPrice` D2LPrice, " +
                $" Round(b.vol/90) avgPanVol, " +
                $" b.`StockPrice`, b.Vol, b.lastVol, b.avgVol5D tr_avgVol5D, b.`avgAmt5D` tr_avgAmt5D, a.panVol5Cnt, " +
                $" a.panVol10Cnt, a.panVol5Dict " +
                $" FROM investbase a " +
                $" inner Join tradedata b on a.`StockID`= b.StockID and b.`TransDate` = a.lastDate " +
                $" inner Join tradedata c on c.`StockID`= b.StockID and c.`TransDate` = b.`lastDate` " +
                //(parseType==1?"":$" inner Join alertlist d on d.`StockID`= a.StockID and d.`alertDate` = a.recDate ") +
                whereStr;

            dt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
            return dt;
        }

        #region Shioaji
        private static DateTime _lastCaWarnDate = DateTime.MinValue;

        public static bool shioajiLogin()
        {
            try
            {
                CommonApp.loginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                using (var healthClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) })
                {
                    var response = healthClient.GetAsync($"{_shioajiServerUrl}/api/v1/health").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        CommonApp.logoutTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        checkCaCertExpiry();
                        return true;
                    }
                }
                //shioaji server 不在，嘗試自動啟動（優先使用 VBS 隱藏啟動，無 terminal 視窗）
                string sstDir = AppDomain.CurrentDomain.BaseDirectory;
                string envDir = System.IO.Directory.GetParent(sstDir)?.Parent?.FullName ?? sstDir;
                string bootPath = System.IO.Path.Combine(envDir, "start_shioaji.vbs");
                if (!File.Exists(bootPath))
                    bootPath = System.IO.Path.Combine(sstDir, "start_shioaji.vbs");
                if (!File.Exists(bootPath))
                {
                    bootPath = System.IO.Path.Combine(envDir, "start_shioaji.bat");
                    if (!File.Exists(bootPath))
                        bootPath = System.IO.Path.Combine(sstDir, "start_shioaji.bat");
                }
                if (File.Exists(bootPath))
                {
                    try
                    {
                        var proc = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = bootPath,
                            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                            WorkingDirectory = System.IO.Path.GetDirectoryName(bootPath)
                        };
                        System.Diagnostics.Process.Start(proc);
                        System.Threading.Thread.Sleep(8000);
                        using (var retryClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) })
                        {
                            var retryResp = retryClient.GetAsync($"{_shioajiServerUrl}/api/v1/health").Result;
                            if (retryResp.IsSuccessStatusCode)
                            {
                                CommonApp.logoutTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                checkCaCertExpiry();
                                return true;
                            }
                        }
                    }
                    catch { }
                }
                string errMsg = $"[⚠️ 重大] shioaji server 無法連線，股票資料停止收集！請手動執行 start_shioaji.bat";
                CommonApp.sstLastErrMsg = errMsg;
                CommonApp.sstLastErrTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                EventLog.WriteEntry(Constants.source, errMsg, EventLogEntryType.Error);
                sendLineNotify("shioajiLogin", errMsg);
                return false;
            }
            catch (Exception ex)
            {
                CommonApp.sstLastErrTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                CommonApp.sstLastErrMsg = "shioaji server 連線失敗: " + ex.Message;
                EventLog.WriteEntry(Constants.source, "shioaji server 連線失敗: " + ex.Message, EventLogEntryType.Error);
                sendLineNotify("shioajiLogin", "shioaji server 連線失敗: " + ex.Message);
                return false;
            }
        }

        private static void checkCaCertExpiry()
        {
            try
            {
                string caPath = Environment.GetEnvironmentVariable("SJ_CA_PATH", EnvironmentVariableTarget.User)
                    ?? "C:/ekey/551/G120519258/S/Sinopac.pfx";
                if (!File.Exists(caPath)) return;

                using (var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(caPath, "G120519258",
                    System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet |
                    System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.PersistKeySet |
                    System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable))
                {
                    int daysLeft = (int)(cert.NotAfter - DateTime.Now).TotalDays;
                    if (daysLeft <= 0)
                    {
                        sendLineNotify("CA憑證", $"CA 憑證已過期！({cert.NotAfter:yyyy-MM-dd})");
                    }
                    else if (daysLeft <= 30 && _lastCaWarnDate.Date != DateTime.Now.Date)
                    {
                        _lastCaWarnDate = DateTime.Now;
                        sendLineNotify("CA憑證", $"CA 憑證將在 {daysLeft} 天後到期 ({cert.NotAfter:yyyy-MM-dd})，請盡快更新。");
                    }
                }
            }
            catch { }
        }

        private static void sendLineNotify(string subject, string message)
        {
            try
            {
                if (lineLib == null)
                    lineLib = new LineLib();
                lineLib.pushTextMessageByWebapiAsync($"sstTray {subject}", $"{DateTime.Now:yyyy-MM-dd HH:mm} {message}");
            }
            catch { }
        }

        public static MsgArray stockDictToMsgArray(Dictionary<string, object> stockDict, DataTable dt)
        {
            MsgArray msgObj = new MsgArray();
            DataTable tempdt = null;
            foreach (KeyValuePair<string, object> item in stockDict)
            {
                try
                {
                    switch (item.Key)
                    {
                        case "datetime":
                            msgObj.tk0 = item.Value.ToString();
                            msgObj.d = DateTime.Parse(item.Value.ToString()).ToString("yyyyMMdd");
                            msgObj.ts = new DateTimeOffset(DateTime.Parse(item.Value.ToString())).ToUnixTimeSeconds().ToString();
                            break;
                        case "code":
                            msgObj.c = item.Value.ToString();
                            tempdt = CommonClass.DataTableFilterSort1(dt, $"StockID='{msgObj.c}'");
                            if (tempdt != null && tempdt.Rows.Count > 0)
                            {
                                msgObj.n = tempdt.Rows[0]["stockName"].ToString();
                                msgObj.y = tempdt.Rows[0]["StockPrice"].ToString(); //昨價
                                msgObj.u = tempdt.Rows[0]["avgPanVol"].ToString(); //昨盤均量
                                msgObj.w = tempdt.Rows[0]["Vol"].ToString(); //昨量
                            }

                            break;
                        case "exchange":
                            msgObj.ex = item.Value.ToString().ToLower();
                            msgObj.ch = $"{msgObj.ex}.{msgObj.c}";
                            break;
                        case "total_volume":
                            msgObj.v = Convert.ToInt32(item.Value).ToString();
                            break;
                        case "volume":
                            msgObj.tv = Convert.ToInt32(item.Value).ToString();
                            break;
                        case "open":
                            msgObj.o = Convert.ToDouble(item.Value).ToString("0.##");
                            break;
                        case "high":
                            msgObj.h = Convert.ToDouble(item.Value).ToString("0.##");
                            break;
                        case "low":
                            msgObj.l = Convert.ToDouble(item.Value).ToString("0.##");
                            break;
                        case "close":
                            msgObj.z = Convert.ToDouble(item.Value).ToString("0.##");
                            break;
                        case "tick_type": //最近成交價是 Sell 或 Buy
                            msgObj.mt = item.Value.ToString();
                            break;
                        case "change_price":  //股價變動
                            msgObj.it = Convert.ToDouble(item.Value).ToString("0.##");
                            break;
                        case "change_rate":
                            msgObj.tlong = Convert.ToDouble(item.Value).ToString("0.##");
                            break;
                        case "change_type": //Up or Down
                            msgObj.pz = item.Value.ToString();
                            break;
                        case "average_price":
                            msgObj.p = Convert.ToDouble(item.Value).ToString("0.##");
                            break;
                        case "buy_price":
                            msgObj.i = Convert.ToDouble(item.Value).ToString("0.##");
                            break;
                        case "buy_volume":
                            msgObj.i += ";" + Convert.ToDouble(item.Value).ToString("0.##");
                            break;
                        case "sell_price":
                            msgObj.tk1 = Convert.ToDouble(item.Value).ToString("0.##");
                            break;
                        case "sell_volume":
                            msgObj.tk1 += ";" + Convert.ToDouble(item.Value).ToString("0.##");
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"stockParseErr, {ex.Message}");
                }
            }
            return msgObj;
        }

        //parseType:1 全部， 2:僅 Parse 當天 recommand 的股票
        public static void shioajiStockAlert(DataTable dataSource, string lastDate, int parseType = 1)
        {
            // 檢查 shioaji server 是否活著
            if (!shioajiLogin())
            {
                EventLog.WriteEntry(Constants.source, "shioaji server not available", EventLogEntryType.Warning);
                return;
            }
            // 從 DataTable 收集所有股票代碼（不分上市/上櫃，HTTP API 一次處理）
            List<string> allStocks = new List<string>();
            foreach (DataRow dr in dataSource.Rows)
            {
                string sid = dr["StockID"].ToString();
                if (sid != "t00" && sid != "o00")
                    allStocks.Add(sid);
            }
            if (allStocks.Count == 0)
                return;
            // 每次最多送 500 檔（官方限制）
            int batchSize = 500;
            for (int batchStart = 0; batchStart < allStocks.Count; batchStart += batchSize)
            {
                var batch = allStocks.Skip(batchStart).Take(batchSize).ToList();
                string jsonBody = buildSnapshotJson(batch);
                for (int retry = 0; retry < 3; retry++)
                {
                    try
                    {
                        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                        var response = _httpClient.PostAsync("/api/v1/data/snapshots", content).Result;
                        if (!response.IsSuccessStatusCode)
                        {
                            CommonClass.wait(10);
                            continue;
                        }
                        string responseJson = response.Content.ReadAsStringAsync().Result;
                        var resultLists = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseJson);
                        if (resultLists == null || resultLists.Count == 0)
                        {
                            CommonClass.wait(10);
                            continue;
                        }
                        processContract(resultLists, dataSource, parseType);
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (retry >= 2)
                        {
                            CommonApp.sstLastErrTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                            CommonApp.sstLastErrMsg = "shioajiStockAlert() HTTP 錯誤: " + ex.Message;
                        }
                        CommonClass.wait(10);
                    }
                }
            }
        }

        private static string buildSnapshotJson(List<string> stockIds)
        {
            var contracts = new List<Dictionary<string, string>>();
            foreach (var sid in stockIds)
            {
                contracts.Add(new Dictionary<string, string>
                {
                    { "security_type", "STK" },
                    { "exchange", "TSE" },
                    { "code", sid }
                });
            }
            var body = new Dictionary<string, object> { { "contracts", contracts } };
            return JsonConvert.SerializeObject(body);
        }

        private static void processContract(List<Dictionary<string, object>> resultLists, DataTable dataSource, int parseType)
        {
            List<MsgArray> priceResults = new List<MsgArray>();
            List<string> tseRepeatList = new List<string>();
            List<string> otcRepeatList = new List<string>();
            foreach (Dictionary<string, object> jsonDict in resultLists)
                priceResults.Add(stockDictToMsgArray(jsonDict, dataSource));
            processAlert(dataSource, lastDate, priceResults, tseRepeatList, otcRepeatList, parseType);
        }

        public static void initDaylySpotliteStock(ref string lastDate)
        {

            string dataDate = DateTime.Now.ToString("yyyy-MM-dd");
            DataTable dt = null;
            string sqlStr = null;
            sqlStr = "insert ignore into `alertlist` (`alertDate`,`StockID`,`reason`) " +
                $" select Date(CURRENT_DATE), `StockID`, Concat('強開盤, 差量：', if(onTimeVol > 0, (`panVol5QuanPos`-`panVol5QuanNeg`)*100/`onTimeVol`, 0))  " +
                " from investbase where if(onTimeVol > 0, (`panVol5QuanPos`-`panVol5QuanNeg`)*100/`onTimeVol`, 0) >= 80 ";
            CommonClass.execSQLNonQuery(sqlStr, sstConnStr);

            //sqlStr = $"INSERT INTO `investbase` " +
            //    $" (`recDate`, `StockID`, `stockName`, `StockType`, `lastVol`, lastPrice, lastDate, " +
            //    $" `avgVol5D`, avgAmt5D, onTimeVol, `currPrice`, reason) " +
            //    $" select * from " +
            //    $"(select '{dataDate}',`StockID`, `StockName`, `StockType`, `Vol`, `StockPrice`, `lastDate`, " +
            //    $" `avgVol5D`, avgAmt5D, '0' onTimeVol , '0' currPrice , IF(`recNote` is null, '',recNote) as recNote  " +
            //    $" from tradedata where `TransDate`='{lastDate}' ) b " +
            //    $" ON DUPLICATE KEY UPDATE " +
            //    $" `lastVol`=b.Vol, onTimeVol=0, lastPrice=b.StockPrice, lastDate = '{lastDate}', `avgAmt5D` = b.avgAmt5D, `avgVol5D`=b.avgVol5D, " +
            //    $" `dailyNoPriceCnt`=0, `Priority`=0, `dailyNoVolCnt`=0, `dailyNoDataCnt`=0, `myPredict`=0, " +
            //    $" `invPredict`=0,`instantMass`=0, `instantRise`=0, `instantFall`=0, `messRise`=0, `messFall`=0, `OpenPriec`=0, `stoploss`=0 ";
            //CommonClass.execSQLNonQuery(sqlStr, sstConnStr);

            //sqlStr = $"update `buyin` a  inner join tradedata b on a.`StockID`=b.StockID and b.TransDate='{lastDate}'  " +
            //    $" set  a.`lastPrice`=b.StockPrice, a.`lastVol`=b.Vol, " +
            //    $" a.`transVol`=0,  a.`onTimePrice`=0, a.`onTimeVol`=0, a.`momentAVDVol`=0,  a.`lastVolRate`=b.lastVolRate, " +
            //    $" a.`avg5VolRate`=b.`avg5VolRate`,  a.`dailyNoPriceCnt`=0,  a.`dailyNoVolCnt`=0, a.`dailyNoDataCnt`=0,  a.`myPredict`=0, " +
            //    $" a.`invPredict`=0, a.`instantMass`=0,  a.`instantRise`=0, a.`instantFall`=0, a.`messRise`=0, a.`messFall`=0,  " +
            //    $" a.`OpenPriec`=0, a.`stoploss`=0, a.`lastDate`='{lastDate}', `DataDate`='{dataDate}'";
            //CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
            //sqlStr = $"update `recommandstock` a " +
            //    $" inner join tradedata b on a.`StockID`=b.StockID and b.TransDate='{lastDate}' " +
            //    $" set a.`lastPrice`=b.StockPrice, a.`lastVol`=b.Vol, a.`transVol`=0, a.`onTimePrice`=0, a.`onTimeVol`=0, " +
            //    $" a.`momentAVDVol`=0, a.`lastVolRate`=b.lastVolRate, a.`avg5VolRate`=b.`avg5VolRate`, " +
            //    $" a.`dailyNoPriceCnt`=0, a.`dailyNoVolCnt`=0, a.`dailyNoDataCnt`=0, a.`myPredict`=0, " +
            //    $" a.`invPredict`=0, a.`instantMass`=0, a.`instantRise`=0, a.`instantFall`=0, a.`messRise`=0, a.`messFall`=0, " +
            //    $" a.`OpenPriec`=0, a.`stoploss`=0, a.`lastDate`='{lastDate}', `reccDate`='{dataDate}' ";
            //CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
        }

        // getAllStocksContract 已移除（舊 Shioaji SDK 專用，改用 HTTP API）

        #region stock60days 計算
        //dayNo 0->今天， 1->昨天， MA5 ->0-4 的平均 
        public static string updateConfigDays(int dayNo, DataTable dt)
        {
            string retStr = null;
            if (dt.Rows.Count <= dayNo + 1)
            {
                //MessageBox.Show("來源資料不足");
                return retStr;
            }
            string sqlStr = "UPDATE `stockconfig` SET `CContent`= ";
            switch (dayNo)
            {
                case 0:
                    //最新資料
                    retStr = dt.Rows[0][0].ToString();
                    sqlStr += $" '{retStr}' WHERE `CName`='currDate' ";
                    break;
                case 1:
                    retStr = dt.Rows[1][0].ToString();
                    sqlStr += $" '{retStr}' WHERE `CName`='yestDate' ";
                    break;
                default:
                    retStr = dt.Rows[dayNo][0].ToString();
                    sqlStr += $" '{retStr}' WHERE `CName`='c{dayNo + 1}Date' ";
                    break;
            }
            CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
            return retStr;
        }

        public static List<double> calcMAMV(DataTable dt, int dayCnt)
        {
            try
            {
                List<double> rtnList = new List<double>();
                if (dt.Rows.Count < dayCnt)
                    return null;
                DataTable tempDt = dt.AsEnumerable().Take(dayCnt).CopyToDataTable();
                rtnList.Add(CommonClass.DataTableAvg(tempDt, "EndPrice", "1==1"));
                rtnList.Add(CommonClass.DataTableAvg(tempDt, "Vol", "1==1"));
                return rtnList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int calcStock60Days(string dataDate, bool skipTradedata = true)
        {
            DataTable tempDt = null;
            DataTable calcDt = null;
            List<double> resultData;
            double ma5, ma10, ma14, ma20, ma35, ma60;
            double mv5, mv10, mv14, mv20, mv35, mv60;
            int rtnCnt = 0;
            String sqlStr = null;
            Dictionary<string, string> stockConfigDict = null;
            if (skipTradedata)
            {
                sqlStr = $"insert ignore into `stock60days` (`StockID`,`StockDate`,`lastDate`,`OpenPriec`," +
                $" `EndPrice`,`HPrice`,`LPrice`,`Vol`) " +
                $" select `StockID`,`StockDate`,`lastDate`,`OpenPriec`, " +
                $" `EndPrice`,`HPrice`,`LPrice`,`Vol` from weekall where StockDate='{dataDate}'";
                CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
            }

            sqlStr = "SELECT distinct Date_format(`StockDate`, '%Y/%m/%d') StockDate  " +
                " FROM `stock60days` order by StockDate desc ";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
            updateConfigDays(0, dt); //計算各 平均日 的開始日期
            updateConfigDays(1, dt); //dayNo 0->今天， 1->昨天， MA5 ->0-4 的平均
            updateConfigDays(4, dt);
            updateConfigDays(9, dt);
            updateConfigDays(13, dt);
            updateConfigDays(19, dt);
            updateConfigDays(33, dt);
            updateConfigDays(59, dt);
            sqlStr = "SELECT distinct StockID  FROM `stock60days` order by StockID ";
            dt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
            foreach (DataRow dr in dt.Rows)
            {
                calcStock60DaysByStockID(dataDate, dr["StockID"].ToString());
                rtnCnt++;
            }
            sqlStr = $"SELECT * FROM `stockconfig` ";
            tempDt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
            stockConfigDict = CommonClass.dtToDictionary(tempDt, "CName", "CContent");


            sqlStr = $"update stock60days a " +
            $" inner join(SELECT StockID, Max(`EndPrice`) MaxPrice, Max(`Vol`) MaxVol from `stock60days` " +
            $" where `StockDate` >= '{stockConfigDict["c60Date"]}' and `StockDate` <= '{dataDate}' " +
            $" group by `StockID`) b on a.StockID = b.StockID " +
            $" set a.MaxPrice3M = b.MaxPrice, a.MaxVol3M = b.MaxVol " +
            $" where a.`StockDate`= '{dataDate}'";
            CommonClass.execSQLNonQuery(sqlStr, sstConnStr);

            //計算所有股票的 季波動率
            sqlStr = $"update stock60days a " +
                $" inner join " +
                $"(select `StockID`, IF(Min(`EndPrice`)=0, 0, Round((max(`EndPrice`)-Min(`EndPrice`))/Min(`EndPrice`) * 100)) stable3M " +
                $" FROM `stock60days` x " +
                $" Left join stockconfig y on y.CName='c60Date' " +
                $" where x.`StockDate` >= y.CContent group by `StockID`) b " +
                $" set a.stable3M = b.stable3M where a.StockDate = '{dataDate}' and a.StockID = b.StockID ";
            CommonClass.execSQLNonQuery(sqlStr, sstConnStr);

            sqlStr = $"update stock60days as a " +
                $" inner join stock60days b on a.StockID=b.StockID and a.lastDate=b.StockDate " +
                $" set a.jumpKong = jumpKong(a.OpenPriec, a.EndPrice, b.OpenPriec, b.EndPrice) " +
                $" where a.StockDate = '{dataDate}' ";
            CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
            return rtnCnt;
        }

        //計算 Stock60Days 各均線值
        public static void calcStock60DaysByStockID(string dataDate, string stockid, bool ifCalcDate = false)
        {
            DataTable tempDt = null;
            DataTable calcDt = null;
            List<double> resultData;
            double ma5, ma10, ma14, ma20, ma35, ma60;
            double mv5, mv10, mv14, mv20, mv35, mv60;
            String sqlStr = null;
            if (ifCalcDate)
            {
                sqlStr = "SELECT distinct Date_format(`StockDate`, '%Y/%m/%d') StockDate  " +
                        " FROM `stock60days` order by StockDate desc ";
                DataTable dt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
                updateConfigDays(0, dt); //計算各 平均日 的開始日期
                updateConfigDays(1, dt); //dayNo 0->今天， 1->昨天， MA5 ->0-4 的平均
                updateConfigDays(4, dt);
                updateConfigDays(9, dt);
                updateConfigDays(13, dt);
                updateConfigDays(19, dt);
                updateConfigDays(33, dt);
                updateConfigDays(59, dt);
            }


            ma5 = ma10 = ma14 = ma20 = ma35 = ma60 = mv5 = mv10 = mv14 = mv20 = mv35 = mv60 = 0;
            sqlStr = "SELECT `EndPrice`, `Vol` " +
               $" FROM stock60days where `StockID`='{stockid}' and StockDate <= '{dataDate}' " +
               $" ORDER BY `StockDate` DESC";
            tempDt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
            resultData = calcMAMV(tempDt, 5);
            if (resultData != null)
            {
                ma5 = resultData[0];
                mv5 = resultData[1];
            }
            resultData = calcMAMV(tempDt, 10);
            if (resultData != null)
            {
                ma10 = resultData[0];
                mv10 = resultData[1];
            }
            resultData = calcMAMV(tempDt, 14);
            if (resultData != null)
            {
                ma14 = resultData[0];
                mv14 = resultData[1];
            }
            resultData = calcMAMV(tempDt, 20);
            if (resultData != null)
            {
                ma20 = resultData[0];
                mv20 = resultData[1];
            }
            resultData = calcMAMV(tempDt, 35);
            if (resultData != null)
            {
                ma35 = resultData[0];
                mv35 = resultData[1];
            }
            resultData = calcMAMV(tempDt, 60);
            if (resultData != null)
            {
                ma60 = resultData[0];
                mv60 = resultData[1];
            }
            sqlStr = $"update `stock60days` set `MA5`={ma5},`MA10`={ma10}," +
                     $" `MA14`={ma14},`MA20`={ma20},`MA35`={ma35},`MA60`={ma60}, " +
                     $" `MV5`={mv5},`MV10`={mv10}," +
                     $" `MV14`={mv14},`MV14`={mv20},`MV35`={mv35},`MV60`={mv60} " +
                     $" where `StockID` = '{stockid}' and `StockDate` = '{dataDate}'";
            CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
            sqlStr = $"SELECT * FROM `stockconfig` ";
            tempDt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
            Dictionary<string, string> stockConfigDict = CommonClass.dtToDictionary(tempDt, "CName", "CContent");


            sqlStr = $"insert IGNORE into stockdeduct " +
                $" (`StockID`,`StockDate`, " +
                $" `MA5`,`MV5`,`MA10`,`MV10`,`MA14`,`MV14`, " +
                $" `MA20`,`MV20`,`MA35`,`MV35`,`MA60`,`MV60` ) " +
                $" select '{stockid}', '{dataDate}' , a.`EndPrice` DA5, a.Vol DV5, " +
                $" b.`EndPrice` DA10, b.Vol DV10, c.`EndPrice` DA14, c.Vol DV14, " +
                $" d.`EndPrice` DA20, d.`Vol` DV20, e.`EndPrice` DA35, e.Vol DV35, " +
                $"f.`EndPrice` DA60, f.Vol DV60 " +
                $" from stock60days a " +
                $" left join stock60days b on a.`StockID`=b.StockID and b.StockDate='{stockConfigDict["c10Date"]}' " +
                $" left join stock60days c on a.`StockID`=c.StockID and c.StockDate='{stockConfigDict["c14Date"]}' " +
                $" left join stock60days d on a.`StockID`=d.StockID and d.StockDate='{stockConfigDict["c20Date"]}' " +
                $" left join stock60days e on a.`StockID`=e.StockID and e.StockDate='{stockConfigDict["c35Date"]}' " +
                $" left join stock60days f on a.`StockID`=f.StockID and f.StockDate='{stockConfigDict["c60Date"]}' " +
                $" where a.StockDate='{stockConfigDict["c5Date"]}' and a.StockID='{stockid}' ";
            CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
        }

        public static void 林則平(string dataDate)
        {
            DataTable tempDt = null;
            DataTable calcDt = null;

            String sqlStr = $"update `stock60days` a " +
                $" inner join ( SELECT `StockID`, 1-(Min(`EndPrice`)/Max(`EndPrice`)) SRate " +
                                $" FROM `stock60days` " +
                                $" WHERE `StockDate` >= SUBDATE('{dataDate}', INTERVAL 6 Month)) b " +
                                $" on a.`StockID`=b.`StockID` and a.`StockDate`='{dataDate}' set a.stable=SRate*100 ";
            CommonClass.execSQLNonQuery(sqlStr, sstConnStr);

            CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
            sqlStr = $"SELECT distinct StockID  FROM `stock60days` " +
                $" where `StockDate`='{dataDate}'  order by StockID ";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
            foreach (DataRow dr in dt.Rows)
            {
                sqlStr = $"SELECT `StockDate` " +
                     $" FROM `stock60days` " +
                     $" WHERE `StockID`='{dr["StockID"]}' and `StockDate` <= '{dataDate}' " +
                     $"order by `StockDate` desc limit 19,1";
                var tempVar = CommonClass.getSQLScalar(sqlStr, sstConnStr);
                string tempDate = null;
                if (tempVar != null && tempVar != DBNull.Value)
                {
                    tempDate = Convert.ToDateTime(tempVar).ToString("yyyy/MM/dd");
                    sqlStr = $"SELECT `EndPrice`, Date_Format(`StockDate`, '%y%m%d') dataDate " +
                     $" FROM `stock60days` where `StockDate` >= '{tempDate}' and stockid = '{dr["StockID"]}' " +
                     $" order by EndPrice desc";
                    tempDt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
                    double RCI = 0;
                    if (tempDt != null && tempDt.Rows.Count > 4)
                    {
                        List<int> priceList = CommonClass.dtToIntList(tempDt, "EndPrice"); //List 以價格排序
                        List<int> dateList = CommonClass.dtToIntList(tempDt, "dataDate", true, true); //List 以日期排序
                        int rciSum = 0;

                        int dateIdx = 0;

                        for (int i = 0; i < priceList.Count; i++)
                        {
                            dateIdx = dateList.FindIndex(a => a == priceList[i]);
                            rciSum += (int)Math.Pow((dateIdx - i), 2);
                        }
                        RCI = Math.Round((double)(1 - ((6 * rciSum) / priceList.Count / (Math.Pow(priceList.Count, 2) - 1))) * 100, 0);
                        sqlStr = $"update  `stock60days` set `droprate`='{RCI}' " +
                            $"where `StockID`='{dr["StockID"]}' and `StockDate`= '{dataDate}' ";
                        CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
                    }
                }
            }
        }

        public static void 林則平Stock60daysByStockID(string dataDate, string stockID)
        {
            DataTable tempDt = null;
            DataTable calcDt = null;

            String sqlStr = $"update `stock60days` a " +
            $" inner join ( SELECT `StockID`, Min(`SMonth`) sMonth, Min(`MinPrice`) MinPrice, " +
                    $" Max(`MaxPrice`) MaxPrice, 1-(Min(`MinPrice`)/Max(`MaxPrice`)) SRate " +
                    $" FROM `monthmax` " +
                    $" WHERE monthDiff(`SMonth`, Date_Format('{dataDate}', '%y/%m')) <= 6 ) b " +
                    $" on a.`StockID`=b.`StockID` and a.`StockDate`='{dataDate}' " +
            $" set a.stable=SRate*100 " +
            $" where a.StockID='{stockID}'";
            CommonClass.execSQLNonQuery(sqlStr, sstConnStr);

            sqlStr = "Update stock60days a " +
                " inner join stock60days b on a.`StockID`=b.StockID " +
                " and a.`lastDate`=b.`StockDate` " +
                " Set a.`fluctuation` = if(a.EndPrice>=a.EndPrice, ABS(b.`EndPrice`-a.`OpenPriec`) + " +
                " (a.`OpenPriec`-a.`LPrice`) + (a.`HPrice`-a.`LPrice`) + (a.`HPrice`-a.`EndPrice`) , " +
                " ABS(b.`EndPrice`-a.`OpenPriec`) + ABS(a.`OpenPriec`-a.`HPrice`) + " +
                " (a.`HPrice`-a.`LPrice`) + ABS(a.`LPrice`-a.`EndPrice`)) " +
                $" where a.`StockDate` = '{dataDate}' and a.StockID='{stockID}'";
            CommonClass.execSQLNonQuery(sqlStr, sstConnStr);

            sqlStr = $"SELECT `StockDate` " +
                    $" FROM `stock60days` " +
                    $" WHERE `StockID`='{stockID}' and `StockDate` <= '{dataDate}' " +
                    $" order by `StockDate` desc limit 20";
            tempDt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
            int targetDateIdx = 0;
            string tempDate = null;
            string lastDate = null;
            if (tempDt.Rows.Count > 1)
            {
                targetDateIdx = tempDt.Rows.Count - 1;
                lastDate = Convert.ToDateTime(tempDt.Rows[1]["StockDate"]).ToString("yyyy/MM/dd");
                tempDate = Convert.ToDateTime(tempDt.Rows[targetDateIdx]["StockDate"]).ToString("yyyy/MM/dd");
                sqlStr = $"SELECT `EndPrice`, Date_Format(`StockDate`, '%Y/%m/%d') dataDate " +
                    $" FROM `stock60days` where `StockDate` >= '{tempDate}' and stockid = '{stockID}' " +
                    $" order by EndPrice desc";
                tempDt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
                double RCI = 0;
                if (tempDt != null && tempDt.Rows.Count > 4)
                {
                    List<string> priceList = CommonClass.dtToList(tempDt, "EndPrice"); //List 以價格排序
                    List<string> dateList = CommonClass.dtToList(tempDt, "dataDate", true, true); //List 以日期排序
                    int rciSum = 0;

                    int dateIdx = 0;

                    for (int i = 0; i < priceList.Count; i++)
                    {
                        dateIdx = dateList.FindIndex(a => a == priceList[i]);
                        rciSum += (int)Math.Pow((dateIdx - i), 2);
                    }
                    RCI = Math.Round((double)(1 - ((6 * rciSum) / priceList.Count / (Math.Pow(priceList.Count, 2) - 1))) * 100, 0);
                    sqlStr = $"update  `stock60days` set `droprate`='{RCI}', lastDate='{lastDate}' " +
                        $"where `StockID`='{stockID}' and `StockDate`= '{dataDate}' ";
                    CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
                }
            }
        }

        #endregion



        #region 分盤均量
        //public static DateTime fenPanAVG(string stockDate)
        //{
        //    DateTime rtnDate;
        //    Sinopac.Shioaji.SJList contracts;
        //    string sqlStr = "SELECT Max(`StockDate`) FROM `weekall` ";
        //    rtnDate = Convert.ToDateTime(CommonClass.getSQLScalar("SELECT Max(`StockDate`) FROM `weekall` "));

        //    long avgPanVol = 0;
        //    sqlStr = $"SELECT distinct `StockID`,`StockType` FROM `buyin`";
        //    DataTable dt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
        //    string dataDate = rtnDate.ToString("yyyy-MM-dd");
        //    Stock contract = null;
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        avgPanVol = getAvgPanVol(contract, dataDate);
        //        sqlStr = $"update buyin set momentAVDVol={avgPanVol} where StockID='{dr["StockID"]}'";
        //        CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
        //        sqlStr = $"update tradedata set avgPanVol={avgPanVol} where StockID='{dr["StockID"]}' and TransDate='{dataDate}'";
        //        CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
        //    }
        //    sqlStr = $"SELECT distinct `StockID`,`StockType` FROM `recommandstock` ";
        //    dt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        avgPanVol = getAvgPanVol(contract, dataDate);
        //        sqlStr = $"update recommandstock set momentAVDVol={avgPanVol} where StockID='{dr["StockID"]}'";
        //        CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
        //        sqlStr = $"update tradedata set avgPanVol={avgPanVol} where StockID='{dr["StockID"]}' and TransDate='{stockDate}'";
        //        CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
        //    }
        //    sqlStr = $"SELECT distinct `StockID`,`StockType` FROM `investbase` ";
        //    dt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        try
        //        {
        //            contract = _api.Contracts.Stocks[dr["StockType"].ToString() == "上市" ? "TSE" : "OTC"][dr["StockID"].ToString()];
        //        }
        //        catch (Exception ex)
        //        {
        //            continue;
        //        }
        //        avgPanVol = getAvgPanVol(contract, dataDate);
        //        sqlStr = $"update investbase set momentAVDVol={avgPanVol} where StockID='{dr["StockID"]}'";
        //        CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
        //        sqlStr = $"update tradedata set avgPanVol={avgPanVol} where StockID='{dr["StockID"]}' and TransDate='{stockDate}'";
        //        CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
        //    }
        //    return rtnDate;
        //}
        //public static long getAvgPanVol(Sinopac.Shioaji.Stock contract, string dataDate)
        //{
        //    StringBuilder sb = new StringBuilder("");
        //    Ticks ticks = _api.Ticks(contract, dataDate);
        //    List<long> volSortedList = ticks.volume.ToList();
        //    long volSum = 0;
        //    foreach (long item in volSortedList)
        //        volSum += item;
        //    if (volSortedList.Count == 0)
        //        return 0;
        //    long avgPanvol = volSum / volSortedList.Count;
        //    return avgPanvol;
        //}
        #endregion
        //public static void syncClosingPrice()
        //{
        //    string sqlStr = null;
        //    string lastDate;
        //    try
        //    {
        //        if (_api == null)
        //        {
        //            if (!shioajiLogin())
        //                return;
        //        }
        //        int badCnt = 0;
        //        List<Sinopac.Shioaji.IContract> contracts = getAllStocksContract(out badCnt);
        //        List<dynamic> snapshot = _api.Snapshots(contracts);

        //        sqlStr = "SELECT Date(Max(created)) FROM `alertlog` ";
        //        DateTime maxAlertDate = Convert.ToDateTime(CommonClass.getSQLScalar(sqlStr, sstConnStr));
        //        DateTime shioajiDate = CommonClass.UnixTimeStampToDateTime(Convert.ToDouble(snapshot[0].ts) / 1000000000);
        //        string stockDate = ((maxAlertDate >= shioajiDate) ? maxAlertDate : shioajiDate).ToString("yyyy-MM-dd");


        //        for (int i = 0; i < snapshot.Count; i++)
        //        {
        //            try
        //            {
        //                sqlStr = $"insert into `weekAll` (`StockID`,`StockDate`,StockName, `OpenPriec`," +
        //                 $" `EndPrice`,`HPrice`,`LPrice`,`Vol`,transVol, stockType) " +
        //                 $" values ('{snapshot[i].code}','{stockDate}', '-', {snapshot[i].open}," +
        //                 $"  {snapshot[i].close}, {snapshot[i].high}, {snapshot[i].low}, {snapshot[i].total_volume}, 0, '{snapshot[i].exchange}') " +
        //                 $" ON DUPLICATE KEY UPDATE " +
        //                 $" EndPrice={snapshot[i].close},OpenPriec={snapshot[i].open}, HPrice={snapshot[i].high}, " +
        //                 $" LPrice={snapshot[i].low}, Vol={snapshot[i].total_volume}, transVol=0";
        //                CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //        }
        //        sqlStr = $"SELECT Max(`StockDate`) FROM `weekall` WHERE `StockDate` < '{stockDate}' ";
        //        lastDate = Convert.ToDateTime(CommonClass.getSQLScalar(sqlStr, sstConnStr)).ToString("yyyy/MM/dd");
        //        sqlStr = "update weekAll a " +
        //            $" inner join weekAll b on a.StockID = b.StockID and b.StockDate='{lastDate}' " +
        //            $" set a.StockName = b.StockName, a.lastDate = '{lastDate}', a.StockType = b.StockType " +
        //            $" where a.StockDate = '{stockDate}' ";
        //        CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
        //        sqlStr = "update `weekall` a inner join stockid b on a.`StockID`=b.`id` set a.`StockName`=b.name where a.StockName = '-' ";
        //        CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
        //        /////////////////////////////////////////////////
        //        sqlStr = " delete from buyin where `stockLeftCount` = 0";
        //        CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
        //        string doneRows = sst.initTradedata();
        //        /////////////////////////////////////
        //        sqlStr = "SELECT Max(`StockDate`) maxD, Min(`StockDate`) minD FROM `weekall` ";
        //        DataTable dt = CommonClass.getSQLDataTable(sqlStr, sstConnStr);
        //        DateTime dataDate = Convert.ToDateTime(dt.Rows[0]["minD"]);
        //        /////////////////////////////////////////////////
        //        /// 處理當天無交易
        //        sqlStr = "update weekall a inner join weekall b on a.`StockID`=b.`StockID` and a.`lastDate`=b.`StockDate` set a.`EndPrice`=b.EndPrice where a.EndPrice = 0 ";
        //        CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
        //        sqlStr = $"update recommandstock a inner join weekall b on a.`StockID`=b.`StockID` and b.`StockDate`='{dataDate}' " +
        //            $" set a.`lastPrice`=b.EndPrice, onTimePrice=b.EndPrice " +
        //            $" where a.lastPrice = 0 ";
        //        CommonClass.execSQLNonQuery(sqlStr, sstConnStr);

        //        //CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
        //        //sqlStr = $"update buyin a inner join weekall b on a.`StockID`=b.`StockID` and b.`StockDate`='{dataDate}' " +
        //        //    $" set a.`lastPrice`=b.EndPrice, onTimePrice=b.EndPrice " +
        //        //    $" where a.lastPrice = 0 ";
        //        //CommonClass.execSQLNonQuery(sqlStr, sstConnStr);

        //        //sqlStr = $"update investbase a inner join weekall b on a.`StockID`=b.`StockID` and b.`StockDate`='{dataDate}' " +
        //        //    $" set a.`lastPrice`=b.EndPrice, onTimePrice=b.EndPrice " +
        //        //    $" where a.lastPrice = 0 ";
        //        //CommonClass.execSQLNonQuery(sqlStr, sstConnStr);

        //        while (dataDate <= Convert.ToDateTime(dt.Rows[0]["maxD"]))
        //        {
        //            calcStock60Days(dataDate.ToString("yyyy/MM/dd"));
        //            dataDate = dataDate.AddDays(1);
        //        }
        //        sqlStr = $"select count(*) from stock60days where StockDate = '{dataDate.AddDays(-1).ToString("yyyy-MM-dd")}'";
        //        string cnt = CommonClass.getSQLScalar(sqlStr, sstConnStr).ToString();
        //        /////////////////////////////////////
        //        int updateCnt = pan3Analysis(stockDate);
        //        ////////////////////////////////////
        //        //initTradedata();
        //        //tradedataAdjust(stockDate);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        public static void genRecommand(string dataDate)
        {
            string sqlStr;
            string lastDate;
            string currDate = DateTime.Now.ToString("yyyy-MM-dd");
            //選股 進化
            sqlStr = "SELECT max(`StockDate`) FROM `weekall`";

            lastDate = Convert.ToDateTime(CommonClass.getSQLScalar(sqlStr, sstConnStr)).ToString("yyyy-MM-dd");
            sqlStr = $"insert ignore into alertlist (`alertDate`,`StockID`,`reason`) " +
                $" select distinct '{currDate}', StockID, '自有' " +
                $" FROM `activestocks`";
            CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
            //sqlStr = $"insert ignore into alertlist (`alertDate`,`StockID`,`reason`) " +
            //    $"SELECT distinct '{currDate}', StockID, '周轉4以上,漲幅<=1'  " +
            //    $" FROM `tradedata` where `TransDate`='{lastDate}' and `turnoverRate` >= 4 and " +
            //    $" turnoverRate <= 30 and `StockDiffRate` <= 1";
            //CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
            //sqlStr = $"insert ignore into alertlist (`alertDate`,`StockID`,`reason`) " +
            //    $" SELECT distinct '{currDate}', StockID, '券資比>-20' " +
            //    $" FROM `tradedata` where `TransDate`='{lastDate}' and `quanziRate` >= 20";
            //CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
            sqlStr = $"insert ignore into alertlist (`alertDate`,`StockID`,`reason`) " +
                $" SELECT CURRENT_DATE, `stockid`, `teacherName` " +
                $" FROM `teacher_event` " +
                $" where `created` >= DATE_SUB(CURRENT_DATE,INTERVAL 7 DAY) " +
                $" and `stockid` > '' and (`autoType`=1 or `event_ID` < 0) ";
            CommonClass.execSQLNonQuery(sqlStr, sstConnStr);

        }

        //public static void insertRecommandFromAlertlist(string dataDate, string lastDate)
        //{
        //    string sqlStr = $"insert ignore into recommandstock " +
        //        $"(`reccDate`,`StockID`, `StockName`,StockType,`ByWho`,`currPrice`, `recommandPrice`, onTimePrice, if20Hight, Priority, reason) " +
        //        $"  select * from (" +
        //        $" SELECT '{dataDate}', a.`StockID`, b.StockName , b.StockType , a.`reason`,b.EndPrice, b.EndPrice ep2 , b.EndPrice ep3  , 0 ifH, 0 priority , a.reason r2 " +
        //        $" FROM `alertlist` a " +
        //        $" left Join weekall b on a.`StockID`=b.`StockID` and b.StockDate = '{lastDate} ' where `alertDate` = '{dataDate}' ) t " +
        //        $" ON DUPLICATE KEY UPDATE  reccDate = '{dataDate}', ByWho=t.reason, currPrice=t.EndPrice, recommandPrice=t.EndPrice, onTimePrice=t.EndPrice, reason = t.reason";
        //    CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
        //}

        //pType:
        //pam, 瞬價 2%
        //plv, 盤昨倍
        //p5v, 盤5V
        //pav, 盤5V盤5倍
        //pa10v, 盤5V盤10倍
        //pa20v, 盤5V盤20倍
        //direction:1 漲， 0:跌, 2,漲幅 0

        private class AlertColumnMap
        {
            public string[] IncrementColumns { get; init; }
        }

        private static readonly Dictionary<(string pType, int dir), AlertColumnMap> AlertTypeMap = new()
        {
            [("pam", 1)]   = new() { IncrementColumns = new[] { "paRatePosCnt" } },
            [("pam", 0)]   = new() { IncrementColumns = new[] { "paRateNegCnt" } },
            [("plv", 1)]   = new() { IncrementColumns = new[] { "pLVRatePosCnt" } },
            [("plv", 0)]   = new() { IncrementColumns = new[] { "pLVRateNegCnt" } },
            [("p5v", 1)]   = new() { IncrementColumns = new[] { "p5VRatePosCnt" } },
            [("p5v", 0)]   = new() { IncrementColumns = new[] { "p5VRateNegCnt" } },
            [("pav", 1)]   = new() { IncrementColumns = new[] { "panVol5CntPos" } },
            [("pav", 0)]   = new() { IncrementColumns = new[] { "panVol5CntNeg" } },
            [("pa10v", 1)] = new() { IncrementColumns = new[] { "panVol5CntPos", "pApRatePosCnt" } },
            [("pa10v", 0)] = new() { IncrementColumns = new[] { "panVol5CntNeg", "pApRateNegCnt" } },
            [("pa20v", 1)] = new() { IncrementColumns = new[] { "panVol5CntPos", "pApRatePosCnt", "panVol50CntPos" } },
            [("pa20v", 0)] = new() { IncrementColumns = new[] { "panVol5CntNeg", "pApRateNegCnt", "panVol50CntNeg" } },
        };

        private static readonly string[] AllAlertColumns = {
            "paRatePosCnt", "paRateNegCnt",
            "pLVRatePosCnt", "pLVRateNegCnt",
            "p5VRatePosCnt", "p5VRateNegCnt",
            "panVol5CntPos", "panVol5CntNeg",
            "pApRatePosCnt", "pApRateNegCnt",
            "panVol50CntPos", "panVol50CntNeg"
        };

        public static int insertAlertList(string stockID, string reason, string pType, int direction, bool ifUpdateOnly = false,
            double panLastVolRate = 0, double panAvg5VolRate = 0, double panAvgVolRate = 0, double panAmtRate = 0, int panVol = 0)
        {
            string sqlStr = null;
            string onDupStr = null;
            try
            {
                AlertTypeMap.TryGetValue((pType, direction), out var map);
                if (map != null)
                    onDupStr = string.Join(", ", map.IncrementColumns.Select(c => $"{c} = {c}+1"));

                onDupStr += $",maxPLVR=IF(maxPLVR > {panLastVolRate}, maxPLVR, {(direction == 0 ? -panLastVolRate : panLastVolRate)}), " +
                    $" maxP5VR=IF(maxP5VR > {panAvg5VolRate}, maxP5VR, {(direction == 0 ? -panAvg5VolRate : panAvg5VolRate)}), " +
                    $" maxPAR=IF(maxPAR > {panAvgVolRate}, maxPAR, {(direction == 0 ? -panAvgVolRate : panAvgVolRate)}), " +
                    $" currPanAmtRate = {panAmtRate}, currPanVol={panVol} ";

                if (!ifUpdateOnly)
                {
                    var colValues = AllAlertColumns.Select(c =>
                        map != null && map.IncrementColumns.Contains(c) ? "1" : "0");
                    sqlStr = $"insert ignore into `alertlist` (`alertDate`, `StockID`, `reason`, " +
                        $" {string.Join(", ", AllAlertColumns.Select(c => $"`{c}`"))}, " +
                        $" maxPLVR, maxP5VR, maxPAR, currPanAmtRate, currPanVol) " +
                        $"Values (CURRENT_DATE, '{stockID}', '{reason}', " +
                        $" {string.Join(", ", colValues)}, " +
                        $" {panLastVolRate}, {panAvg5VolRate}, {panAvgVolRate}, {panAmtRate}, {panVol}) ";
                }
                CommonClass.execSQLNonQuery(sqlStr, Constants.ConnString, true);
                return 0;
            }
            catch (Exception ex)
            {
                CommonClass.writeLog("sstTtry", "insertAlertList()", 5, ex.Message, ex, "scott.tseng@firstohm.com.tw");
                return 0;
            }
        }


        #endregion
        //計算 panVol:3分鐘的（當）量
        public static int calcPanVol(DateTime created, DateTime preTime, int currVol, int preVol,
            ref double panWeight, int lastVol)
        {
            if (DateTime.Now.Minute <= 5 && DateTime.Now.Hour == 9)
            {
                preTime = DateTime.Parse($"{DateTime.Now.Date.ToString("yyyy/MM/dd")} 09:00");
                if (currVol == lastVol) //第一盤的量 == 昨量 => 第一盤無量
                    currVol = preVol = 0;
            }

            TimeSpan panTimeSpent = created - preTime;
            // Display the result in minutes
            Double panMinut = panTimeSpent.TotalMinutes;
            panMinut = panMinut < 1 ? 1 : panMinut;
            int volDiff = currVol - preVol < 0 ? currVol : currVol - preVol;
            int panVol = (int)Math.Round((volDiff) * 3 / panMinut, 0);
            panWeight = 1;
            return panVol;
        }

        public static void calcPanVolCnts(double panWeight, int panVol, int avgVol5D, int lastAvgVol5D,
            int lastAvgPanVol, string stockType, double panAmtRate,
            ref double panAvgVolRate, ref double panLastVolRate, ref double panAvg5VolRate,
            ref int panVol5CntPos, ref int panVol5CntNeg, ref int panVol5QuanPos, ref int panVol5QuanNeg,
            ref int instantMass, ref int messRise, ref int messFall)
        {
            panAvgVolRate = lastAvgPanVol == 0 ? panVol : (double)panVol / lastAvgPanVol;
            panLastVolRate = avgVol5D == 0 ? panVol : (double)panVol / avgVol5D;
            panAvg5VolRate = lastAvgVol5D == 0 ? panVol : (double)panVol / lastAvgVol5D;

            if (panAvgVolRate >= 5 && stockType == "興櫃" && panAmtRate > 0)
            {
                panVol5CntPos += 1;
                panVol5QuanPos += panVol;
                instantMass += 1;
                messRise += 1;
            }
            else if (panAvgVolRate >= 5 && stockType == "興櫃" && panAmtRate < 0)
            {
                panVol5CntNeg += 1;
                panVol5QuanNeg += panVol;
                instantMass += 1;
                messFall += 1;
            }
            else if (((panAvgVolRate >= 5 && panVol >= 30) || panVol >= 200 || panAvgVolRate >= 8) && panAmtRate > 0)
            {
                panVol5CntPos += 1;
                panVol5QuanPos += panVol;
                instantMass += 1;
                messRise += 1;
            }
            else if (((panAvgVolRate >= 5 && panVol >= 30) || panVol >= 200 || panAvgVolRate >= 8) && panAmtRate < 0)
            {
                panVol5CntNeg += 1;
                panVol5QuanNeg += panVol;
                instantMass += 1;
                messFall += 1;
            }
            else if ((panAvgVolRate >= 5 && panVol >= 30) || panVol >= 200 || panAvgVolRate >= 8)
                instantMass += 1;
        }

        //return -1: neg, 0:equal, 1:Pos, -99 沒有主進 
        //昨盤均 5 倍以上或興櫃 or 昨盤均 5 倍以上或盤量 >=200
        public static int calcPanVolCntsSimple(double panWeight, int panVol,
            double panAvgVolRate, double panAmtRate, string stockType)
        {
            int rtnVal = -99;
            if (panAvgVolRate >= 5 && stockType == "興櫃" && panAmtRate > 0)
                rtnVal = 1;
            else if (panAvgVolRate >= 5 && stockType == "興櫃" && panAmtRate < 0)
                rtnVal = -1;
            else if (((panAvgVolRate >= 5 && panVol >= 30) || panVol >= 200 || panAvgVolRate >= 8) && panAmtRate > 0)
                rtnVal = 1;
            else if (((panAvgVolRate >= 5 && panVol >= 30) || panVol >= 200 || panAvgVolRate >= 8) && panAmtRate < 0)
                rtnVal = -1;
            else if ((panAvgVolRate >= 5 && panVol >= 30) || panVol >= 200 || panAvgVolRate >= 8)
                rtnVal = 0;
            return rtnVal;
        }

        /// <summary>
        /// 計算盤量等級：正數=上漲方向，負數=下跌方向
        /// 20=20倍主進, -20=20倍主出, 10=10倍主進, -10=10倍主出,
        /// 5=主進, -5=主出, 0=平盤, -99=無訊號
        /// </summary>
        private static int calcPanLevel(double panAvgVolRate, int panVol, double panAmtRate, string stockType, double panWeight)
        {
            if ((panAvgVolRate >= 20 || panVol >= 950) && panAmtRate > 0) return 20;
            if ((panAvgVolRate >= 20 || panVol >= 950) && panAmtRate < 0) return -20;
            if ((panAvgVolRate >= 10 || panVol >= 600) && panAmtRate > 0) return 10;
            if ((panAvgVolRate >= 10 || panVol >= 600) && panAmtRate < 0) return -10;
            int simple = calcPanVolCntsSimple(panWeight, panVol, panAvgVolRate, panAmtRate, stockType);
            if (simple == 1) return 5;
            if (simple == -1) return -5;
            if (simple == 0) return 0;
            return -99;
        }

        private static bool skipStock(MsgArray msgArray, List<string> tseRepeatList, List<string> otcRepeatList)
        {
            if (tseRepeatList != null && msgArray.ex == "tse")
                tseRepeatList.Add(msgArray.c);
            else if (otcRepeatList != null && msgArray.ex == "otc")
                otcRepeatList.Add(msgArray.c);
            return true;
        }

        public static void processAlert(DataTable dataSource, string lastDate,
            List<MsgArray> priceResults, List<string> tseRepeatList = null,
            List<string> otcRepeatList = null, int parseType = 1)
        {
            StringBuilder notifyStrBody = new StringBuilder();
            StringBuilder notifyStrTitle = new StringBuilder();
            string instRuseFallRate = null;
            int tempVolDir = -99;
            int pDiff = 0;
            object objPanScore = null;
            int panvolScore = 0;
            int ifPriceVal = 0;
            int ifVolval = 0;
            string sqlStr = null;
            object tempObj = null;
            double yestPrize = 0;
            double onTimePrice = 0;
            double prePrice = 0;
            double d1HPrice = 0;
            double d1LPrice = 0;
            double d2HPrice = 0;
            double d2LPrice = 0;
            double panAmtDiffRate = 0;
            int panVol5Cnt = 0;
            int panVol10Cnt = 0;
            string panVol5DictJson = null;
            int panAmtRatePosCnt = 0;
            int lastVol = 0;
            int avg5Vol = 0;
            double lastVolRate = 0;
            double avg5VolRate = 0;
            int onTimeVol = 0;
            int preVol = 0;
            int lastPanAvGVol = 0;
            double panAvgVol = 0;
            int panVol = 0;
            int panTrans = 0;
            double transRate = 0;
            int panVolPerTrans = 0;
            double instant5DVolRate = 0;
            double panAmtDiff, panAmtRate, panAvgVolRate, panLastVolRate, panAvg5VolRate;
            string heightLevelStr = "";
            bool alreadyNotStr = false;
            int ifMess = 0;
            int ifRise = 0;
            int ifFalls = 0;
            int messRise = 0;
            int messFall = 0;
            DateTime alertTime;
            DateTime preTime;
            bool t0920Flag = DateTime.Now.Hour == 9 && DateTime.Now.Minute >= 19 && DateTime.Now.Minute <= 21;
            double diffPrice = 0;
            double diffRate = 0;
            double dbYestPrice = 0;
            int priority = 0;
            double shortPower = 1;
            double instantJumpKong = 0;
            int alertID = -1;
            string stockType = null;
            List<string> shortBull = null;
            List<string> shortBear = null;
            CommonApp.sstProcessErrCnt = 0;
            CommonApp.sstProcessStart = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            if (sst.last10PanSlot == null)
                sst.last10PanSlot = new List<DateTime>();
            if (sst.last10PanSlot.Count == 11)
                sst.last10PanSlot.RemoveAt(0);
            sst.last10PanSlot.Add(DateTime.Now);
            DataRow dr = null;
            //即時資訊
            foreach (MsgArray msgArray in priceResults)
            {
                try
                {
                    ifRise = ifFalls = ifMess = 0;
                    if (CommonClass.IsNumeric(msgArray.z) && double.Parse(msgArray.z) > 0)
                        ifPriceVal = 0;
                    else
                        ifPriceVal = 1; //未取到 即時 價格 資訊

                    if (CommonClass.IsNumeric(msgArray.v) && double.Parse(msgArray.v) > 0)
                        ifVolval = 0;

                    else
                        ifVolval = 1; //未取到 即時 成交量 資訊

                    if (ifPriceVal == 1 && ifVolval == 1)
                    {
                        if (skipStock(msgArray, tseRepeatList, otcRepeatList)) continue;
                    };
                    //if (CommonClass.IsNumeric(msgArray.tlong))
                    //    alertTime = DateTime.Parse($"{DateTime.Now.ToString("yyyy/MM/dd")} {msgArray.t}");
                    //else
                    //    alertTime = DateTime.Now;
                    //alertTime = DateTime.Parse($"{CommonClass.yyyymmddToDateFormat(msgArray.d)} {msgArray.t}");
                    alertTime = DateTime.Now;
                    yestPrize = onTimePrice = prePrice = lastVol = avg5Vol = onTimeVol = preVol = lastPanAvGVol = panVol = 0;
                    lastVolRate = avg5VolRate = panAvgVol = diffRate = dbYestPrice = panAmtDiffRate = 0;
                    diffPrice = panVolPerTrans = 0;
                    panAmtDiff = panAmtRate = panAvgVolRate = panLastVolRate = panAvg5VolRate = transRate = instant5DVolRate = 0;
                    messRise = messFall = pDiff = 0;
                    d1HPrice = d2HPrice = d2LPrice = d2LPrice = 0;
                    panVol5Cnt = panVol10Cnt = panAmtRatePosCnt = 0;
                    //如果沒有資料, 就用之前的資料當最新股價
                    if (!CommonClass.IsNumeric(msgArray.z))
                    {
                        if (tseRepeatList != null && msgArray.ex == "tse")
                            tseRepeatList.Add(msgArray.c);
                        else if (otcRepeatList != null && msgArray.ex == "otc")
                            otcRepeatList.Add(msgArray.c);
                        continue;
                    }
                    else
                        onTimePrice = Convert.ToDouble(msgArray.z); //本盤價格

                    if (onTimePrice <= 0)
                    {
                        if (skipStock(msgArray, tseRepeatList, otcRepeatList)) continue;
                    }
                    if (msgArray.ex == "oes")
                        onTimeVol = (int)(Math.Round(Convert.ToDouble(msgArray.v) / 1000, 0)); //到目前的成交量
                    else
                        onTimeVol = Convert.ToInt32(msgArray.v); //到目前的成交量
                    if (onTimeVol <= 0)
                    {
                        if (skipStock(msgArray, tseRepeatList, otcRepeatList)) continue;
                    }

                    if (!CommonClass.IsNumeric(msgArray.v))
                    {
                        if (skipStock(msgArray, tseRepeatList, otcRepeatList)) continue;
                    }
                    ////////////////////////////////////////////////////////////////////////////////////////
                    //foreach (dr in dataSource.Rows)
                    //{
                    //    //dataSource 欄位：
                    //    //lastPrice 昨日收盤價
                    //    //lastVol 昨日收盤量
                    //    //currPrice 昨日收盤價(同lastPrice)
                    //    //currVol:昨日收盤量(同昨日收盤量)
                    //    //onTimePrice:現價
                    //    //onTimeVol:現量
                    //    if (dr["StockID"].ToString() != msgArray.c)
                    //        continue;
                    //    else
                    //        break;
                    //  }
                    dr = CommonClass.DataTableFilterSort1(dataSource, $"StockID='{msgArray.c}'").Rows[0];
                    //現價超過昨價 10%
                    if (CommonClass.IsNumeric(msgArray.y) && double.Parse(msgArray.y) > 0)
                        yestPrize = dbYestPrice = double.Parse(msgArray.y);
                    else if (dr["lastPrice"] != DBNull.Value)
                    {
                        yestPrize = dbYestPrice = Convert.ToDouble(dr["lastPrice"]); //db中記錄的昨價
                        msgArray.y = dr["lastPrice"].ToString();
                    }
                    else
                    {
                        var temp1 = $"SELECT `EndPrice` FROM `stock60days` " +
                            $" where `StockDate` = '{lastDate}' and StockID='{msgArray.c}' ";
                        var tempL = CommonClass.getSQLScalar(temp1, sstConnStr).ToString();
                        yestPrize = dbYestPrice = Convert.ToDouble(tempL); //db中記錄的昨價
                        msgArray.y = yestPrize.ToString();
                    }

                    if (dr["onTimeVol"] != DBNull.Value)
                        preVol = Convert.ToInt32(dr["onTimeVol"]);//前一盤 成交量
                    else
                        preVol = 0;
                    if (dr["onTimePrice"] != DBNull.Value)
                        prePrice = Convert.ToDouble(dr["onTimePrice"]);//前一盤價格
                    ////////////////////////////////////////////////////////////////
                    msgArray.n = dr["stockName"].ToString();
                    diffPrice = Math.Round(onTimePrice - yestPrize, 2);
                    priority = 0;
                    preTime = Convert.ToDateTime(dr["updated"]);//前一盤時間

                    if (!string.IsNullOrEmpty(msgArray.w)) //昨量
                        lastVol = Convert.ToInt32(msgArray.w);
                    else if (dr["lastVol"] != DBNull.Value)
                        lastVol = Convert.ToInt32(dr["lastVol"]);

                    //重複撈資料
                    double panDiffTime = (DateTime.Now - preTime).TotalMinutes;
                    if (DateTime.Now.Hour==9 && DateTime.Now.Minute <= 7 &&  panDiffTime < 3)
                    {
                        string mailBody = $"時間: {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}{System.Environment.NewLine}" +
                            $"號碼: {msgArray.c}   名稱: {msgArray.n}   sType: {msgArray.ex} {System.Environment.NewLine}" +
                            $"錯誤原因: ";
                        CommonClass.sendmailEazy(mailBody, "3", "scott.tseng@firstohm.com.tw", $"第一盤發生重疊： {mailBody} ");
                        continue;
                    }

                    //第一盤或之前
                    if (preTime <= DateTime.Parse($"{DateTime.Now.ToString("yyyy/MM/dd")} 09:00"))
                    {
                        preTime = DateTime.Parse($"{DateTime.Now.ToString("yyyy/MM/dd")} 09:00");
                        preVol = 0;
                        prePrice = yestPrize;
                    }
                    //本盤無交易
                    else if (preTime > DateTime.Parse($"{DateTime.Now.ToString("yyyy/MM/dd")} 09:07"))
                    {
                        if (preVol == onTimeVol)
                            continue;
                    }
                    else
                    { //前盤資料
                        preTime = Convert.ToDateTime(dr["updated"]);
                        preVol = Convert.ToInt32(dr["onTimeVol"]);//前一盤 成交量
                        prePrice = Convert.ToDouble(dr["onTimePrice"]);//前一盤價格
                    }

                    stockType = dr["StockType"].ToString();

                    diffRate = yestPrize == 0 ? 0 : Math.Round((onTimePrice - yestPrize) / yestPrize * 100, 2); //股價幅
                    if (dr["tr_avgVol5D"] == DBNull.Value || dr["tr_avgVol5D"] == null || dr["tr_avgVol5D"] == DBNull.Value) //五日均量
                        avg5Vol = 0;
                    else
                        avg5Vol = Convert.ToInt32(dr["tr_avgVol5D"]);//五日均量
                    if (lastVol > 0) //lastVolRate： 昨量倍
                        lastVolRate = Math.Round((double)onTimeVol / lastVol, 2);
                    if (avg5Vol > 0) //lastVolRate： 5均量倍
                        avg5VolRate = Math.Round((double)onTimeVol / avg5Vol, 2);
                    //if (CommonClass.IsNumeric(msgArray.tv))
                    //    panVol = Convert.ToInt32(msgArray.tv); //這盤的量
                    if (CommonClass.IsNumeric(msgArray.v))
                        panTrans = Convert.ToInt32(msgArray.v) - preVol; //5分鐘的量
                    //if (panVol < 0) //如果傳來的 thisVol 資料很奇怪, 則自己算
                    //panVol = (int)Math.Round((Double)((onTimeVol - preVol) / panTimeSpanMin * 3));
                    panVol = calcPanVol(DateTime.Now, preTime, onTimeVol, preVol, ref panWeight, lastVol);
                    if (panVol == 0) //後面的code無需執行， 沒有意義
                        continue;
                    lastPanAvGVol = (int)Math.Round((Double)avg5Vol / 90);
                    lastPanAvGVol = lastPanAvGVol < 1 ? 1 : lastPanAvGVol;
                    //XXX panTrans XXX 已經沒再用了
                    if (panTrans < 0) //如果傳來的 張數 資料很奇怪, 則自己算
                        panTrans = 0;
                    if (panTrans > 0)
                        transRate = Math.Round((double)(panVol / panTrans), 2); // 5分量/當盤量

                    panAvgVolRate = (double)panVol / lastPanAvGVol;
                    panLastVolRate = (lastVol == 0 ? (double)panVol : (double)panVol / lastVol);
                    panAvg5VolRate = (avg5Vol == 0 ? (double)panVol : (double)panVol / avg5Vol);

                    //可疑 volume 資料 logging（資料照收，但記錄異常供分析）
                    if (onTimeVol < preVol)
                        CommonClass.writeLog("sstTray", "suspiciousData", 3,
                            $"累計量倒退 {msgArray.c} onTimeVol={onTimeVol} preVol={preVol}");
                    if (panVol <= 0)
                        CommonClass.writeLog("sstTray", "suspiciousData", 3,
                            $"panVol<=0 {msgArray.c} panVol={panVol} onTimeVol={onTimeVol} preVol={preVol}");
                    if (panAvgVolRate > 100)
                        CommonClass.writeLog("sstTray", "suspiciousData", 3,
                            $"極高倍率 {msgArray.c} panAvgVolRate={panAvgVolRate:F1} panVol={panVol} lastPanAvGVol={lastPanAvGVol}");

                    if (dr["D1HPrice"] == null || dr["D1HPrice"] == DBNull.Value) //第一天高價
                        d1HPrice = 0;
                    else
                        d1HPrice = Convert.ToDouble(dr["D1HPrice"]);
                    if (dr["D1LPrice"] == null || dr["D1LPrice"] == DBNull.Value) //第一天低價
                        d1LPrice = 0;
                    else
                        d1LPrice = Convert.ToDouble(dr["D1LPrice"]);
                    if (dr["D2HPrice"] == null || dr["D2HPrice"] == DBNull.Value) //第二天高價
                        d2HPrice = 0;
                    else
                        d2HPrice = Convert.ToDouble(dr["D2HPrice"]);
                    if (dr["D2LPrice"] == null || dr["D2LPrice"] == DBNull.Value) //第二天低價
                        d2LPrice = 0;
                    else
                        d2LPrice = Convert.ToDouble(dr["D2LPrice"]);

                    panAmtDiff = onTimePrice - prePrice; //前後盤價差
                    if (yestPrize > 0)
                        panAmtDiffRate = Math.Round(panAmtDiff / yestPrize * 100, 2); //前後盤價幅
                    else
                        panAmtDiffRate = 0;
                    panAmtRate = panAmtDiffRate;

                    shortBull = null;
                    shortBear = null;
                    //取得 短多/短空 比
                    //shortPower = getShortPower(msgArray, shortBull, shortBear);
                    notifyStrBody.Clear();
                    notifyStrTitle.Clear();
                    heightLevelStr = "";
                    alreadyNotStr = false;
                    panVolPerTrans = panTrans == 0 ? 0 : panVol / panTrans; //當盤平均每筆交易的成交量
                    instant5DVolRate = avg5Vol == 0 ? 0 : Math.Round((double)panVol / avg5Vol, 2);  //盤量5均倍
                    double stockTimeSpan = CommonClass.sameDaytimeBetweenInMinutes("09:00", msgArray.t);

                    if (panAmtDiffRate >= 1) ////瞬間跳漲 1% 以上
                    {
                        alreadyNotStr = true;
                        notifyStrTitle.Append($"瞬間跳漲 1%以上||");
                        ifRise = 1;
                        priority = 4;
                        //pType:pa, plv, p5v, pav
                        //direction:1 漲， 0:跌
                        //insertAlertList(msgArray.c, "瞬間跳漲 1%以上", "pam", 1, false, panLastVolRate, panAvg5VolRate, panAvgVolRate);
                        insertAlertList(msgArray.c, "瞬間跳漲 1%以上", "pam", 1, false, panLastVolRate, panAvg5VolRate,
                            panAvgVolRate, panAmtRate, panVol);
                    }
                    else if (panAmtDiffRate <= -1) //瞬間跳殺 1 % 以上
                    {
                        alreadyNotStr = true;
                        ifFalls = 1;
                        //pType:pa, plv, p5v, pav
                        //direction:1 漲， 0:跌
                        insertAlertList(msgArray.c, $"瞬間跳殺 -1% 以上", "pam", 0, false, panLastVolRate, panAvg5VolRate,
                            panAvgVolRate, panAmtRate, panVol);
                        priority = 4;
                    }

                    //統一量級判斷（20/10/5 倍主進主出）
                    int panLevel = calcPanLevel(panAvgVolRate, panVol, panAmtRate, stockType, panWeight);
                    tempVolDir = panLevel > 0 ? 1 : (panLevel < 0 ? -1 : (panLevel == 0 ? 0 : -99));
                    switch (panLevel)
                    {
                        case 20:
                            ifMess = 1; messRise = 1;
                            notifyStrTitle.Append($" 20 主進或盤量 >=950，瞬量拉升||");
                            insertAlertList(msgArray.c, "20 主進", "pa20v", 1, false, panLastVolRate, panAvg5VolRate,
                                panAvgVolRate, panAmtRate, panVol);
                            priority = 4;
                            break;
                        case -20:
                            ifMess = 1; messFall = 1;
                            notifyStrTitle.Append($"20 主出或盤量 >=950，瞬量下殺||");
                            insertAlertList(msgArray.c, "20 主出", "pa20v", 0, false, panLastVolRate, panAvg5VolRate,
                                panAvgVolRate, panAmtRate, panVol);
                            priority = 4;
                            break;
                        case 10:
                            ifMess = 1; messRise = 1;
                            notifyStrTitle.Append($"10 主進或盤量 >=600，瞬量拉升||");
                            insertAlertList(msgArray.c, "昨盤均 10 倍", "pa10v", 1, false, panLastVolRate, panAvg5VolRate,
                                panAvgVolRate, panAmtRate, panVol);
                            priority = 4;
                            break;
                        case -10:
                            ifMess = 1; messFall = 1;
                            notifyStrTitle.Append($"10 主出或盤量 >=600，瞬量下殺||");
                            insertAlertList(msgArray.c, "5盤均 10 倍", "pa10v", 0, false, panLastVolRate, panAvg5VolRate,
                                panAvgVolRate, panAmtRate, panVol);
                            priority = 4;
                            break;
                        case 5:
                            ifMess = 1; messRise = 1;
                            notifyStrTitle.Append($"主進或盤量 >=200，瞬量拉升||");
                            insertAlertList(msgArray.c, "主進", "pav", 1, false, panLastVolRate, panAvg5VolRate,
                                panAvgVolRate, panAmtRate, panVol);
                            priority = 4;
                            break;
                        case -5:
                            messFall = 1;
                            notifyStrTitle.Append($"主出或盤量 >=200，瞬量下殺||");
                            insertAlertList(msgArray.c, "主出", "pav", 0, false, panLastVolRate, panAvg5VolRate,
                                panAvgVolRate, panAmtRate, panVol);
                            priority = 4;
                            break;
                        case 0:
                            ifMess = 1;
                            priority = 4;
                            notifyStrTitle.Append($"昨盤均 5 倍以上或興櫃 or 昨盤均 5 倍以上或盤量 >=200，平盤||");
                            break;
                    }

                    //本盤量 > 5均量的 1/10
                    if (panAvg5VolRate >= 0.5)
                    {
                        ifMess = 1;

                        if (panAmtDiff == 0)
                        {
                            notifyStrTitle.Append($"分盤爆量 5均量的50%以上，瞬量平盤||");
                        }

                        else if (panAmtDiff > 0)
                        {
                            messRise = 1;
                            notifyStrTitle.Append($"分盤爆量 5均量的50%以上，瞬量拉升||");
                            insertAlertList(msgArray.c, "本盤量 > 5均量的 50% 以上且 分盤漲", "p5v", 1, false, panLastVolRate,
                                panAvg5VolRate, panAvgVolRate, panAmtRate, panVol);
                            //pType:pa, plv, p5v, pav
                            //direction:1 漲， 0:跌
                        }
                        else if (panAmtDiff < 0)
                        {
                            messFall = 1;
                            notifyStrTitle.Append($"分盤爆量 5均量的50%以上，瞬量下殺||");
                            insertAlertList(msgArray.c, "本盤量 > 5均量的 50% 以上且 分盤漲", "p5v", 0, false, panLastVolRate,
                                panAvg5VolRate, panAvgVolRate, panAmtRate, panVol);
                        }
                        priority = 4;
                    }

                    //本盤量 > 昨量的 1/10
                    if (panLastVolRate >= 0.5)
                    {
                        ifMess = 1;

                        if (panAmtDiff == 0)
                        {
                            notifyStrTitle.Append($"分盤爆量 昨量的50%以上%，瞬量平盤||");
                        }
                        else if (panAmtDiff > 0)
                        {
                            notifyStrTitle.Append($"分盤爆量 昨量的50%以上%，瞬量拉升||");
                            insertAlertList(msgArray.c, "本盤量 > 昨量的 50% 以上 且 分盤漲 ", "plv", 1, false, panLastVolRate,
                                panAvg5VolRate, panAvgVolRate, panAmtRate, panVol);
                            messRise = 1;
                            //pType:pa, plv, p5v, pav
                            //direction:1 漲， 0:跌
                        }
                        else if (panAmtDiff < 0)
                        {
                            messFall = 1;
                            insertAlertList(msgArray.c, "本盤量 > 昨量的 50% 以上 且 分盤漲 ", "plv", 0, false, panLastVolRate,
                                panAvg5VolRate, panAvgVolRate, panAmtRate, panVol);
                            notifyStrTitle.Append($"分盤爆量 昨量的50%以上%，瞬量下殺||");
                        }

                    }

                    if (string.IsNullOrEmpty(msgArray.n))
                    {
                        msgArray.n = CommonClass.getSQLScalar($"SELECT `name` FROM `stockid` " +
                            $" where id={msgArray.c} ").ToString();
                    }

                    sqlStr = $"select (`paRatePosCnt`+`pLVRatePosCnt`+`p5VRatePosCnt`+`pApRatePosCnt`) - (`paRateNegCnt`+`pLVRateNegCnt`+`p5VRateNegCnt`+`pApRateNegCnt`) pDiff " +
                                $" from alertlist " +
                                $" where StockID='{msgArray.c}' and alertDate = CURRENT_DATE";
                    tempObj = CommonClass.getSQLScalar(sqlStr);
                    if (tempObj != null && tempObj != DBNull.Value)
                    {
                        pDiff = Convert.ToInt32(tempObj);
                    }

                    t0920Flag = false;
                    sqlStr = $"SELECT Round(sum((`panAvgVolRate`/10) * IF(panAmtDiff>0,1,if(panAmtDiff<0,-1,0)))) panvolScore " +
                        $" FROM `alertlog` where alertDate = CURRENT_DATE and `StockID`='{msgArray.c}' " +
                        $" group by `StockID`, alertDate";

                    if ((objPanScore = CommonClass.getSQLScalar(sqlStr)) == null)
                        panvolScore = 0;
                    else if (objPanScore == DBNull.Value)
                        panvolScore = 0;
                    else
                        panvolScore = Convert.ToInt32(objPanScore);
                    if (panAmtDiff != 0 && panAvgVolRate >= 10)
                        panvolScore += (int)((panAvgVolRate / 10) * (panAmtDiff > 0 ? 1 : -1));

                    sqlStr = $"update investbase set `onTimePrice`={msgArray.z}, " +
                                $" recDate = '{DateTime.Now.ToString("yyyy/MM/dd")}', preAlertID={alertID}, " +
                                $" `onTimeVol`={onTimeVol}, momentAVDVol={panAvgVol}, lastVol={lastVol}," +
                                $" dailyNoPriceCnt=dailyNoPriceCnt+{ifPriceVal}, dailyNoVolCnt=dailyNoVolCnt+{ifVolval}, " +
                                $" instantMass= {dr["instantMass"]} + {ifMess}, messRise= {dr["messRise"]} + {messRise}, messFall= {dr["messFall"]} + {messFall}," +
                                $" instantRise={dr["instantRise"]} + {ifRise}, instantFall = {dr["instantFall"]} + {ifFalls}, " +
                                $" instantIdx = '{shortPower}' , OpenPriec={msgArray.o}, lastDate='{lastDate}', " +
                                //$" Hprice=greatest(Hprice, {msgArray.z}), Lprice=least(IF(Lprice=0,{msgArray.z},Lprice), {msgArray.z}), " +
                                $" Hprice=greatest(Hprice, {msgArray.h}), Lprice=least(IF(Lprice=0,{msgArray.z},Lprice), {msgArray.l}), " +
                                $" panVol5Cnt='{panVol5Cnt}', panVol10Cnt='{panVol10Cnt}', panVol5Dict='{panVol5DictJson}', panvolScore={panvolScore}, " +
                                $" panVol50CntPos = panVol50CntPos + {((panAvgVolRate >= 20 || panVol > 950) && panAmtDiff > 0 ? 1 : 0)}, " +
                                $" panVol50CntNeg = panVol50CntNeg + {((panAvgVolRate >= 20 || panVol > 950) && panAmtDiff < 0 ? 1 : 0)} " +

                                (pDiff == 0 ? "" : $", pDiff={pDiff}") +
                                (tempVolDir == 1 ? ", panVol5CntPos=panVol5CntPos+1 " : "") +
                                (tempVolDir == -1 ? ", panVol5CntNeg=panVol5CntNeg+1 " : "") +
                                (tempVolDir == 1 ? $", panVol5QuanPos=panVol5QuanPos+ {panVol} " : "") +
                                (tempVolDir == -1 ? $", panVol5QuanNeg=panVol5QuanNeg+ {panVol} " : "") +
                                (t0920Flag ? $", vol0921='{onTimeVol}',  priceDiffRate0921 = '{diffRate}'" : "") +

                                $" where `StockID`='{msgArray.c}'";
                    CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
                    //tempVolDir = calcPanVolCntsSimple() 昨盤均 5 倍以上或興櫃 or 昨盤均 5 倍以上或盤量 >=200
                    //if (panAvgVolRate > 1 || Math.Abs(panAmtRate) >= 1 || tempVolDir >= -1)
                    if (panAvgVolRate >= 0.5 || Math.Abs(panAmtRate) >= 0.5 || tempVolDir >= -1 )
                    {
                        notifyStrBody.Append($"現價:{onTimePrice}, 漲跌：{diffPrice}, 漲幅：{diffRate}%, 現量:{onTimeVol} ||");
                        notifyStrBody.Append($"昨量倍:{lastVolRate}  5均量倍:{avg5VolRate} ||");
                        notifyStrBody.Append($"盤差價:{panAmtDiff}, 盤價幅度:{panAmtRate} ||");
                        notifyStrBody.Append($"盤量/昨量:{panLastVolRate}, 盤量/5均量:{panAvg5VolRate}, 盤量/昨盤均量:{panAvgVolRate}");
                        if (Convert.ToInt16(dr["messFall"]) + Convert.ToInt16(dr["instantFall"]) + messFall + ifFalls == 0)
                            instRuseFallRate = $"{dr["messRise"]}+{dr["messRise"]}+{dr["instantRise"]} + {ifRise}";
                        else
                            instRuseFallRate = $"({dr["messRise"]}+{dr["messRise"]}+{dr["instantRise"]} + {ifRise})/({dr["messFall"]}+ {dr["instantFall"]}+ { messFall} + {ifFalls})";

                        sqlStr = $"insert into `alertlog` (CREATED, `StockID`,`StockName`, lastDate, " +
                                    $" AlertTitle, `AlertType`," +
                                    $" `CurrPrice`,`CurrVol`, panVol, panTrans, panVolTransRate, diffPrice, " + //11
                                    $" DiffRate, recommandBy, recommandPrice, priority, stockPriority, " +
                                    $"`prePrice`,`preVol`,`preTime`, " +
                                    $" lastVolRate, avg5VolRate, " + //16
                                    $" instantMass, messRise, messFall, " +
                                    $" instantRise, instantFall, InstRiseFallRate, " + //21
                                    $" panAmtDiff, panAmtRate, panAvgVolRate,panLastVolRate,panAvg5VolRate," + //26
                                    $" `instantBuyVol`,`instantSellVol`, `instantIdx`, OpenPriec, instantJumpKong, " +
                                    $" lastPrice, lastVol, avg5Vol, pDiff, alertDate, panvolScore) " +

                                    $"Values('{alertTime.ToString("yyyy-MM-dd HH:mm")}', '{msgArray.c}','{msgArray.n}', '{lastDate}', " +
                                    $" '{notifyStrTitle.ToString()}','{notifyStrBody.ToString()}'," +
                                    $" {msgArray.z},{onTimeVol},{panVol}, {panTrans}, {transRate}, {diffPrice}, " +
                                    $"{diffRate}, '{dr["recommandBy"]}', '0', '{priority}', '{dr["priority"]}', " +
                                    $" '{prePrice}', '{preVol}', '{preTime.ToString("yyyy-MM-dd HH:mm:ss")}'," +

                                    $" {lastVolRate}, {avg5VolRate}," +
                                    $" {ifMess}, {messRise}, {messFall}, " +
                                    $" {ifRise}, {ifFalls}, {instRuseFallRate}, " +
                                    $"{panAmtDiff},{panAmtRate},{panAvgVolRate}, {panLastVolRate},{panAvg5VolRate}, " +
                                    $" '{msgArray.f}','{msgArray.g}', '{shortPower}', '{msgArray.o}', {instantJumpKong}," +
                                    $" {yestPrize}, {lastVol}, {avg5Vol}, {pDiff}, CURRENT_DATE, {panvolScore}) " +
                                    $" ON DUPLICATE KEY UPDATE " +
                                    $" AlertTitle='{notifyStrTitle}', AlertType = '{notifyStrBody}', OpenPriec='{msgArray.o}', " +
                                    $" `CurrPrice`={msgArray.z},`CurrVol`={onTimeVol}, panVol={panVol}, " +
                                    $" panTrans={panTrans}, panVolTransRate={transRate},diffPrice={diffPrice}, " +
                                    $" DiffRate={diffRate}, recommandBy='{dr["recommandBy"]}', recommandPrice='0', " +
                                    $" priority='{priority}', stockPriority='{dr["priority"]}', " +
                                    $"`prePrice`='{prePrice}',`preVol`='{preVol}',`preTime`= '{preTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
                                    $" lastVolRate={lastVolRate}, avg5VolRate={avg5VolRate}, " +
                                    $" instantMass={ifMess}, messRise={messRise}, messFall= {messFall}, " +
                                    $" instantRise={ifRise}, instantFall={ifFalls}, " +
                                    $" panAmtDiff={panAmtDiff}, panAmtRate={panAmtRate}, panAvgVolRate={panAvgVolRate}," +
                                    $" panLastVolRate={panLastVolRate},panAvg5VolRate={(avg5Vol == 0 ? panVol : panVol / avg5Vol)} ," +
                                    $" `instantBuyVol`='{msgArray.f}',`instantSellVol`='{msgArray.g}', `instantIdx`='{shortPower}', instantJumpKong={instantJumpKong}, " +
                                    $" InstRiseFallRate={instRuseFallRate}, panvolScore={panvolScore} ";
                        alertID = CommonClass.execSQLNonQuery(sqlStr, sstConnStr, true);
                        //sqlStr = $"update alertlog a " +
                        //        $" inner join investbase b on a.`StockID`=b.`StockID` " +
                        //        $" set a.`panVol5CntPos` = b.panVol5CntPos , a.`panVol5CntNeg`= b.`panVol5CntNeg`, " +
                        //        $" a.panVol5QuanNeg = b.panVol5QuanNeg, a.panVol5QuanPos = b.panVol5QuanPos, " +
                        //        $" a.panVol50CntPos = b.panVol50CntPos, " +
                        //        $" a.panVol50CntNeg = b.panVol50CntNeg " +
                        //        $" where `Log_ID` = '{alertID}' ";
                        //CommonClass.execSQLNonQuery(sqlStr, sstConnStr);

                        //notifyIcon.ShowBalloonTip(3000, $"{msgArray.c}  {msgArray.n} {notifyStrTitle.ToString()}", notifyStrBody.ToString(), ToolTipIcon.None);
                        priority = 2;
                        //break; //get out of for loop
                        //sqlStr = $"update alertLog a " +
                        //        $" inner join (select Log_ID, sum(if(panAmtRate > 0,1,0))/sum(if(panAmtRate > 0 || panAmtRate < 0 ,1,0)) updownR_b " +
                        //                $" from alertLog where stockid = '{msgArray.c}' and alertDate = CURRENT_DATE ) b on b.Log_ID <= a.Log_ID " +
                        //        $" set a.upDownRate = b.updownR_b " +
                        //        $" where a.Log_ID = '{alertID}' ";
                        //sqlStr = $"update alertLog a " +
                        //        $" inner join (select Log_ID, sum(if(panAmtRate > 0,1,0)) sp_PosSum, sum(if(panAmtRate < 0,1,0)) sp_NegSum " +
                        //                $" from alertLog " +
                        //                $" where stockid = '{msgArray.c}' and alertDate = CURRENT_DATE ) b on b.Log_ID <= a.Log_ID " +
                        //        $" inner join investBase c on c.recDate = a.alertDate and a.stockid = a.stockid " +
                        //        $" set a.upDownRate = if(b.sp_PosSum + b.sp_NegSum = 0, 0.3, b.sp_PosSum/(b.sp_PosSum + b.sp_NegSum)), " +
                        //        $" a.RFR = FRiseFallRate(c.instantRise , c.messRise , c.instantFall , c.messFall)" +
                        //        $" where a.Log_ID = '{alertID}' ";
                        //CommonClass.execSQLNonQuery(sqlStr, sstConnStr);
                    }
                }
                catch (Exception ex)
                {
                    string mailBody = $"時間: {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}{System.Environment.NewLine}" +
                        $"號碼: {msgArray.c}   名稱: {msgArray.n}   sType: {msgArray.ex} {System.Environment.NewLine}" +
                        $"錯誤原因: {ex.Message} {System.Environment.NewLine} " +
                        $"sqlStr=  {sqlStr}";
                    CommonClass.sendmailEazy(mailBody, "3", "scott.tseng@firstohm.com.tw", $"SST 處理資料 發生 Exception: {ex.Message}");
                    EventLog.WriteEntry(Constants.source, $"processAlert() Error : {ex.Message}", EventLogEntryType.Error);
                    CommonApp.sstLastErrTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    CommonApp.sstLastErrMsg = ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace;
                    CommonApp.sstProcessErrCnt++;
                    CommonApp.sstDayProcessErrCnt++;

                }
            }
            t0920Flag = DateTime.Now.Hour == 9 && DateTime.Now.Minute >= 19 && DateTime.Now.Minute <= 21;
            if (CommonApp.sendSSTStartLine.Date < DateTime.Now.Date && t0920Flag)
            {
                sendLine0921();
            }
            CommonApp.sstProcessEnd = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }

    }
    public class FormIn
    {
        public string Q_SYMBOL_1 { get; set; } //例如：9929,2012 (上市股)
        public string Q_DATE_2 { get; set; } //收盤資料的日期
        public string Q_SYMBOL_3 { get; set; } //例如：9929,2012 (上櫃股)
        public string Q_MONTH_3 { get; set; } //月份資料的 月份 例如 20210510
    }

    public class GetRealtimePriceOut
    {
        public string ErrorMsg { get; set; }
        public string realPrice { get; set; }
    }

    public class GetDayPriceOut
    {
        public string ErrorMsg { get; set; }
        public List<StockPriceRow> gridList { get; set; }
    }

    public class StockPriceRow
    {
        public string symbolCode { get; set; }
        public string symbolName { get; set; }
        public string date { get; set; }
        public string open { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string close { get; set; }
        public string volume { get; set; }
    }

    public class GetMonthPriceOut
    {
        public string ErrorMsg { get; set; }
        public List<StockPriceRow> gridList { get; set; }
    }

    //即時股價
    public class TwsePriceSchema
    {
        public QueryTime queryTime { get; set; }
        public string referer { get; set; }
        public string rtmessage { get; set; }
        public string exKey { get; set; }
        public IList<MsgArray> msgArray { get; set; }
        public int userDelay { get; set; }
        public string rtcode { get; set; }
        public int cachedAlive { get; set; }
    }

    public class QueryTime
    {
        public int stockInfoItem { get; set; }
        public string sessionKey { get; set; }
        public string sessionStr { get; set; }
        public string sysDate { get; set; }
        public int sessionFromTime { get; set; }
        public int stockInfo { get; set; }
        public bool showChart { get; set; }
        public int sessionLatestTime { get; set; }
        public string sysTime { get; set; }
    }

    public class MsgArray
    {
        public string ch { get; set; } //Channel，ex. 2330.tw
        public string c { get; set; } //股票代號 Shioaji -- code 
        public string n { get; set; } //name Shioaji -- 自取
        public string nf { get; set; } //公司全名 Shioaji -- 無此資料
        public string ex { get; set; } //上市或上櫃，ex. tse， Shioaji -- exchange 

        public string d { get; set; } //yyyyMMdd Shioaji -- ts 
        public string y { get; set; } //昨日成交價 Shioaji -- 自取 weekall  
        public string o { get; set; } //開盤價 Shioaji -- open 
        public string h { get; set; } //最高成交價 Shioaji -- high
        public string l { get; set; } //最低成交價 Shioaji -- lov  

        public string v { get; set; } //累積成交量, 現量  Shioaji- total_amount
        public string s { get; set; } //這盤的張數  //Shioaji -- 無此資料
        public string ps { get; set; } //試算參考成交量 //Shioaji -- 無此資料 
        public string ts { get; set; } //揭示時間 //Shioaji- ts
        public string tk0 { get; set; }
        public string t { get; set; } //揭示時間 //Shioaji- ts
        public string ip { get; set; } //1:趨跌,2:趨漲,4:暫緩收盤,5:暫緩開盤 //Shioaji -- 無此資料
        public string z { get; set; } //最近成交價 //Shioaji- close
        public string tv { get; set; } ////當盤成交量 //Shioaji- volume

        public string a { get; set; } //五檔賣出價格 //Shioaji -- 目前不取此資料
        public string g { get; set; } //五檔買量//Shioaji -- 目前不取此資料
        public string b { get; set; } //五檔賣價 //Shioaji -- 目前不取此資料
        public string f { get; set; } //五檔賣量 //Shioaji -- 目前不取此資料

        public string mt { get; set; } //Shioaji -- tick_type  Sell or Buy 
        public string it { get; set; } //Shioaji -- change_price, ex. 0.5 
        public string pz { get; set; } //Shioaji -- change_type Down or Up 
        public string tlong { get; set; } //目前時間 Shioaji -- change_rate ex. 0.6 
        public string p { get; set; } //Shioaji -- 	average_price ex.16.75 
        public string i { get; set; } //Shioaji -- buy_price;buy_volume ex. 15.9;247
        public string tk1 { get; set; } //Shioaji -- sell_price;sell_volume  ex.15.95;404
        public string u { get; set; } //漲停價 Shioaji -- 自取 昨盤均量
        public string w { get; set; } //跌停價 Shioaji -- 自取 昨量 
    }
}

