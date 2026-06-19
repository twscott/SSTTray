using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using FirstOhm;
using System.Threading;
using Newtonsoft.Json;
using System.Data;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.ExtendedProperties;
using SSTTray.ExportApi;

namespace TaskTrayApplication
{
    public class TaskTrayApplicationContext : ApplicationContext
    {

        NotifyIcon notifyIcon = new NotifyIcon();
        Configuration configWindow = new Configuration();
        DbBackup dbBKWindow = new DbBackup();
        模擬_IO simIO = new 模擬_IO(); //自動執行 akeeba backup
        HtmlToPdf htmlToPdf = new HtmlToPdf();
        UDPSend frmUdpSend = null;
        webAPI webApiForm = new webAPI();
        FormDoSST frmDoSst = null;
        System.Timers.Timer timerSysTray;
        CommonBackup cmBK = new CommonBackup();
        List<string> autoExecItems = null; //要Timer 定期執行的項目
        bool ifAskWhenRestart = true;
        //Dictionary<string, string> aotoExecStrDict = null;
        bool conutriLastFtpStatus = true; //乾燥室 FTP 是否正常， true:是, false：否
        LineLib linelib = new LineLib();
        DateTime doSSTTime=DateTime.Now;
        FirstOhm.WebClient foWebClient = new FirstOhm.WebClient();
        private ApiServer _exportApiServer;

        private void setMenuVisibility()
        {
            string menuItems = Constants.getProperty("Menu");
            if (string.IsNullOrEmpty(menuItems))
                return;
            List<string> menuList = menuItems.Split(',').ToList();
            foreach (MenuItem mi in notifyIcon.ContextMenu.MenuItems)
            {
                mi.Visible = false;
            }

            foreach (string menuitem in menuList)
            {
                foreach (MenuItem mi in notifyIcon.ContextMenu.MenuItems)
                {
                    if (mi.Text == menuitem)
                        mi.Visible = true;
                }
            }
        }

        public TaskTrayApplicationContext()
        {
            MenuItem udpSend = new MenuItem("Sent By UDP", new EventHandler(UDPSend));
            MenuItem udpQueue = new MenuItem("UDP Print Queue工作清單", new EventHandler(UDPQueue));
            MenuItem doSST = new MenuItem("Do SST", new EventHandler(frm_do_sst));


            MenuItem ftpClient = new MenuItem("FTP Client 測試", new EventHandler(FtpClient));
            MenuItem htmlTopdf = new MenuItem("HTML To PDF", new EventHandler(ShowHTMLToPdf));
            MenuItem sqlToExcel = new MenuItem("Sql產生 Excel 檔案", new EventHandler(SQLtoExcel));
            MenuItem sqlToXml = new MenuItem("Sql 轉 Json", new EventHandler(SqlToXML));
            MenuItem excelToXml = new MenuItem("Excel 轉 Json", new EventHandler(ExcelToXML));
            MenuItem excelToCsv = new MenuItem("Excel 轉 .CSV", new EventHandler(ExcelToCSV));
            MenuItem sendLine = new MenuItem("傳送 Line 訊息", new EventHandler(SendLine));
            MenuItem simulateIO = new MenuItem("定期模擬 IO", new EventHandler(SimulateIO));
            MenuItem testWebAPI = new MenuItem("測試WebAPI", new EventHandler(TestWebAPI));
            MenuItem testDBConnection = new MenuItem("測試 DB 連線", new EventHandler(TestDBConn));
            MenuItem configMenuItem = new MenuItem("Configuration", new EventHandler(ShowConfig));
            MenuItem dbBackup = new MenuItem("**備份/還原 DB", new EventHandler(ShowDBBackup));
            MenuItem webBackup = new MenuItem("網站備份", new EventHandler(ShowWebBackup));
            MenuItem dbUnlock = new MenuItem("解開DB死結", new EventHandler(ShowDBUnlock));
            MenuItem clearProcess = new MenuItem("清除 Process", new EventHandler(ClearProcess));
            MenuItem restartApp = new MenuItem("重啟程式或電腦", new EventHandler(RestartAPP));
            MenuItem reloadProperty = new MenuItem("重載屬性檔", new EventHandler(ReloadProperty));
            MenuItem closeExcel = new MenuItem("關閉所有EXCEL", new EventHandler(CloseExcel));
            MenuItem openExcel = new MenuItem("測試OpenExcel", new EventHandler(TestOpenExcel));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));


            //if (CommonClass.ifProcessRunning("TaskTrayApplication") > 1)
            //{
            //    if (ifAskWhenRestart)
            //    {
            //        DialogResult dr = MessageBox.Show("程式已在執行中， 是否要重啟？", "程式重複啟動", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //        if (dr == DialogResult.Yes)
            //            CommonClass.killProcessByName("TaskTrayApplication");
            //    }
            //    else
            //    {
            //        CommonClass.killProcessByName("TaskTrayApplication");
            //    }
            //}
            CommonApp.myIP =  CommonClass.GetLocalIPAddress();
            notifyIcon.Icon = new System.Drawing.Icon("stocks_icon-icons.com_72031.ico"); ;
            notifyIcon.DoubleClick += new EventHandler(ShowMessage);
            //notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { udpSend, printLabel, htmlTopdf, dbBackup, syncBaseFoot, setColorBar, testWebAPI, exitMenuItem });
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] {
                doSST, udpSend, udpQueue, ftpClient,

                htmlTopdf, sqlToXml,sqlToExcel, testWebAPI, testDBConnection,excelToCsv, excelToXml,
                dbUnlock, dbBackup, webBackup, simulateIO, sendLine,restartApp, clearProcess, reloadProperty,closeExcel,openExcel ,exitMenuItem
            });
            notifyIcon.Visible = true;

            //udp server start

            var udpReceiveUdpThread = new Thread(UDPLib.ThreadRunMethod);
            udpReceiveUdpThread.Start();
            Constants.propertyDic = Constants.prepareProperty(CommonApp.propertyPath);

            #region Timer
            string autoExecItemStr = Constants.getProperty("autoExecItems");
            //sst.dailyInitInvestBase(DateTime.Now.ToString("yyyy-MM-dd"));
            //sst.syncTeacherEvent(); 
            //adjust0900Vol();
            sst.do_sst(DateTime.Now.Hour, DateTime.Now.Minute, Constants.SSTConnString, 2);
            //adjust0900Vol();
            detector();
            //detectorPanAmtRateDiff("2023-11-29", true);


            //detectorState(DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")));
            //detectorState(DateTime.Parse("2023-12-04"));
            //重算 or 測試
            //detectorState(DateTime.Parse("2020-01-01"));

            //重新計算本日的交易?度
            //detectorIdx(true);
            //一般交易期間計算
            //detectorIdx(false);

            //detector56();
            //calcRecommand();
            //sst.insertAlertList("123", "xxx", "pa", 0, false, 0.2,3.5,11);
            //sst.shioajiLogin();
            sst_sendLine(3);
            sst_sendLine(2);
            sst_sendLine(1); //上?簡訊

            if (!string.IsNullOrEmpty(autoExecItemStr))
            {
                autoExecItems = autoExecItemStr.Split(',').ToList();
            }
            
            if (autoExecItems != null && autoExecItems.Count > 0)
            {
                CommonApp.sstDayProcessErrCnt = 0;
                System.Timers.Timer timerSysTray = new System.Timers.Timer();
                timerSysTray.Interval = 60000; //1 分鐘執行一次
                timerSysTray.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer_timerSysTray);
                timerSysTray.Start();
            } 
            setMenuVisibility();

            // 啟動 Export API Server（背景執行，不影響主功能）
            StartExportApi();

            //CommonApp.chkIP_AND_PropertyIP();

        }

        private void StartExportApi()
        {
            try
            {
                const int apiPort = 9527;
                const string apiToken = "ssttray-export-key";
                _exportApiServer = new ApiServer(apiPort, apiToken, Constants.SSTConnString);
                _exportApiServer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ExportApi] Failed to initialize: {ex.Message}");
            }
        }


        /*
            1.單正 
            2.先正侯負 
            3.單負 
            4.先負侯正 
            5.只有負值 但是現幅>負幅 ：反轉
            6.只有正值 但是現幅<負幅 ：反轉  
            7.多正 
            8.多負：加速趕底 
            9.混雜現正
            10.混雜現負
         */
        //reCalc: true 全部重算, false:交易期間計算
        private void detectorIdx(bool reCalc = false)
        {
            string sqlStr = null;
            if(reCalc)
            {
                //全部重算
                sqlStr = "SELECT * FROM `detector` ";
            } else
            {
                //交易期間
                sqlStr = "SELECT * FROM `detector` " +
                    " where created >= Date_sub(Now(), interval 30 Minute)  ";

            }
            /////////////////////////////////////////////////////////////////////////////////////////////////
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            DataTable dataDt = null;
            double strengthIdx = -1;
            double strengthVol = -1;
            double maxPanAmt = -1;
            int cntPanAmt = -1;
            double strengthAmt = -1;
            foreach (DataRow dr in dt.Rows)
            {
                sqlStr = $"SELECT sum(if(b.panAmtRate >0 && b.panAvgVolRate > 1 ,1, if(b.panAmtRate <0 && b.panAvgVolRate > 1 , -1,0))) cntPanAmt , " +
                    $" if(count(*)=1, -1, sum(b.panAmtRate)) strengthAmt , " +
                    $" Max(b.panAmtRate) maxPanAmt, " +
                    //$" sum(b.panAmtRate) strengthIdx , " +
                    //$" sum(if(b.panAmtRate > 0, b.panAmtRate, if(b.panAmtRate<0, -1, 0)) * b.panAvgVolRate) strengthVol, " +
                    $" a.panAmtRate a_panAmtRate" +
                    $" FROM `alertLog` a " +
                    $" inner join alertLog b on a.stockid = b.StockID and b.created >= Date_Sub(a.CREATED, interval 30 Minute) and " +
                    $" b.Log_Id < a.Log_id " +
                    $" where a.Log_ID={dr["Log_ID"]} group by b.StockID ";
                dataDt = CommonClass.getSQLDataTable(sqlStr);
                if (dataDt.Rows.Count == 0)
                    continue;
                strengthAmt = Convert.ToDouble(dataDt.Rows[0]["strengthAmt"]);
                //strengthIdx = Convert.ToDouble(dataDt.Rows[0]["strengthIdx"]);
                //strengthVol = Convert.ToDouble(dataDt.Rows[0]["strengthVol"]);
                cntPanAmt = Convert.ToInt16(dataDt.Rows[0]["cntPanAmt"]);
                maxPanAmt = Convert.ToDouble(dataDt.Rows[0]["maxPanAmt"]);
                sqlStr = $"update detector set cntPanAmt={cntPanAmt}, sumPanAmt = {strengthAmt} " +
                         //$" strengthIdx = {strengthIdx}, strengthVol = {strengthVol} " +
                         $" where dtcID={dr["dtcID"]} ";

                CommonClass.execSQLNonQuery(sqlStr);
            }
            sqlStr = $"update detector set sumPanAmt=0 " +
                $" where sumPanAmt = -1 and CURRENT_TIMESTAMP > Date_add(created, interval 15 minute)";
            CommonClass.execSQLNonQuery(sqlStr);
        }
        /*
         5.現幅>負幅(1,7) ：負轉正
         6.現幅<負幅(3,8) ：正轉負 
         */
        private void detector56()
        {

            string sqlStr = null;
            try
            {
                sqlStr = $"UPDATE `detector` a " +
                        $" INNER JOIN `investbase` b " +
                        $" ON a.stockid = b.stockid AND a.`dtcDate` = b.recDate " +
                        $" SET a.sstate = " +
                        $" CASE WHEN a.`StockDiffRate` < priceRate(b.onTimePrice, b.lastPrice) AND " +
                        $" (a.sstate = 3 or a.sstate = 10  or a.sstate = 2 or a.sstate = 8 ) THEN 5 " +
                        $" WHEN a.`StockDiffRate` > priceRate(b.onTimePrice, b.lastPrice) AND " +
                        $" (a.sstate = 1 or a.sstate = 9 or a.sstate = 4 or a.sstate = 7 ) THEN 6 " +
                        $" END, " +
                        $" a.stateStr = ConCat(a.stateStr, " +
                        $" CASE WHEN a.`StockDiffRate` < priceRate(b.onTimePrice, b.lastPrice) AND " +
                        $" (a.sstate = 3 or a.sstate = 10  or a.sstate = 2 or a.sstate = 8) THEN 5 " +
                        $" WHEN a.`StockDiffRate` > priceRate(b.onTimePrice, b.lastPrice) AND " +
                        $" (a.sstate = 1 or a.sstate = 9 or a.sstate = 4 or a.sstate = 7 ) THEN 6 " +
                        $" END, ',') " +
                        $"  where b.updated > a.created ";
                CommonClass.execSQLNonQuery(sqlStr);
            }
            catch(Exception ex)
            {
                CommonClass.smtpSendMail($"sqlStr = {sqlStr} {Environment.NewLine}" +
                    $"{ex.Message}{Environment.NewLine}{Environment.NewLine}{ex.StackTrace}  ",
                new Dictionary<string, string>() { { "Scott Tseng", "scott.tseng@firstohm.com.tw" } },
                true, "detector56() 錯誤");
            }
        }

        private int detectorDecideState(int preState, double maxPanAmt, int cntPanAmt, double sumPanAmt, double strengthIdx,
            double currStockRate, double dtcStockRate)
        {
            switch (preState)
            {
                case 0: //本日尚無狀態
                    if (maxPanAmt > 0)
                        return 1; //單正
                    else
                        return 3; //單負 
                    break;
                case 1: //單正
                    if (maxPanAmt > 0)
                        return 7; //多正
                    else if (maxPanAmt < 0)
                        return 2; //先正侯負
                    break;
                case 2: //先正侯負
                    if (maxPanAmt > 0)
                        return 9; //正混雜
                    else if (maxPanAmt < 0)
                        return 8; //8.多負
                    break;
                case 3://單負 
                    if (maxPanAmt > 0)
                        return 4; //先負侯正
                    else if (maxPanAmt < 0)
                        return 8; //8.多負
                    break;
                case 4: //先負侯正
                    if (maxPanAmt > 0)
                        return 7; //多正
                    else if (maxPanAmt < 0)
                        return 10; //負混雜
                    break;
                case 5: //現幅 > 負幅(2, 3, 8,10) ：負轉正
                    if (maxPanAmt > 0)
                        return 7; //先負侯正
                    else if (maxPanAmt < 0)
                        return 2; //8.多負
                    break;
                case 6: //現幅 < 負幅(1,7,9,4) ：正轉負 
                    if (maxPanAmt > 0)
                        return 4;
                    else if (maxPanAmt < 0)
                        return 8; //先正侯負
                    break;
                case 7: //多正
                    if (maxPanAmt > 0)
                        return 7; //多正
                    else if (maxPanAmt < 0)
                        return 2;
                    break;
                case 8: //8.多負：加速趕底
                    if (maxPanAmt > 0)
                        return 4; //先負侯正
                    else if (maxPanAmt < 0)
                        return 8; //8.多負
                    break;
                case 9: //正混雜
                    if (maxPanAmt > 0)
                        return 7; //先負侯正
                    else if (maxPanAmt < 0)
                        return 8; //8.多負
                    break;
                case 10: //正混雜
                    if (maxPanAmt > 0)
                        return 7; //先負侯正
                    else if (maxPanAmt < 0)
                        return 8; //8.多負
                    break;
                default:
                    return 0;
                    break;
            }
            return 0;
        }
        private void detectorState(DateTime dataDate)
        {
            //string currStockID=null, 
            string preStockID = null, strDataDate = null;
            DateTime currDate = DateTime.Parse("2020-1-1"); //這一筆的日期
            DateTime preDate = DateTime.Parse("2020-1-1"); //前一筆的日期
            int currState = 0, preState=0;  //前一筆的狀況
            string stateStr = null, preStateStr = null;
            string dataDateStr = dataDate.ToString("yyyy-MM-dd");
            DataTable tempDt = null;
            string sqlStr = null;
            try {
                if (dataDate.Date == DateTime.Now.Date)
                {
                    sqlStr = $"SELECT dtcID, dtcDate, a.`StockID`, a.`StockDiffRate` dtcStockRate , a.`maxPanAmt`, a.`cntPanAmt`, " +
                        $" a.`sumPanAmt`, a.`strengthIdx`, a.`sstate`,  " +
                        $" pricerate(b.onTimePrice, b.lastPrice) currStockRate, a.stateStr " +
                        $" FROM `detector` a inner join investBase b on a.stockid = b.StockID and a.dtcDate = b.recDate " +
                        (dataDate == DateTime.Parse("2020-01-01") ? "" : $" where dtcDate >='{dataDateStr}' ") +
                        $" order by `StockID`, Log_ID";
                }
                else
                {
                    //重算 
                    sqlStr = $"update `detector` set sstate= 0, stateStr=null where dtcDate >='{dataDate}' ";
                    CommonClass.execSQLNonQuery(sqlStr);
                    //測試
                    //sqlStr = $"SELECT dtcID, dtcDate, a.`StockID`, a.`StockDiffRate` dtcStockRate , a.`maxPanAmt`, a.`cntPanAmt`, " +
                    //        $" a.`sumPanAmt`, a.`strengthIdx`, a.`sstate`, if(b.StockDiffRate is null,a.`StockDiffRate`, b.StockDiffRate) currStockRate " +
                    //        $" FROM `detector` a " +
                    //        $" inner join tradedata b on a.stockid = b.StockID  and `dtcDate`=transDate  " +
                    ////        $" where dtcDate = '2023-11-15' and a.stockid=4770 " +
                    //        $" where  a.stockid=4770 " +
                    //        $" order by `StockID`, created";

                    //重算 
                    sqlStr = $"SELECT dtcID, dtcDate, a.`StockID`, a.`StockDiffRate` dtcStockRate , a.`maxPanAmt`, a.`cntPanAmt`, " +
                        $" a.`sumPanAmt`, a.`strengthIdx`, a.`sstate`, if(b.StockDiffRate is null,a.`StockDiffRate`, b.StockDiffRate) currStockRate, stateStr " +
                        $" FROM `detector` a " +
                        $" inner join tradedata b on a.stockid = b.StockID and a.dtcDate = b.transDate  " +
                         (dataDate == DateTime.Parse("2020-01-01") ? "" : $" where dtcDate >='{dataDate}' ") +
                        $" order by `StockID`, created";
                }
                DataTable dt = CommonClass.getSQLDataTable(sqlStr);
                if (dt.Rows.Count == 0)
                    return;
                DataRow dr = null;
                int dtcID = -1;
                int j = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dr = dt.Rows[i];
                    dtcID = Convert.ToInt32(dr["dtcID"]);
                    if (dataDate != Convert.ToDateTime(dr["dtcDate"]))
                    {
                        dataDate = Convert.ToDateTime(dr["dtcDate"]);
                        strDataDate = dataDate.ToString("yyyy-MM-dd");
                        preState = 0;
                        preStateStr = "";
                    }
                    for (j = 0; i + j < dt.Rows.Count && (currState = Convert.ToInt32(dt.Rows[i + j]["sstate"])) == 0; j++)
                        break;
                    dr = dt.Rows[i + j];
                    if (i + j >= 1 && dt.Rows[i + j - 1]["StockID"].ToString() == dr["StockID"].ToString())
                    {
                        preDate = Convert.ToDateTime(dt.Rows[i + j - 1]["dtcDate"]);
                        preStockID = dr["StockID"].ToString();
                        preStateStr = dr["stateStr"].ToString();
                    }
                    else
                    {
                        preStockID = "-1";
                        preState = 0;
                        preStateStr = "";
                    }
                    if (i + j == 0)
                    {
                        preState = 0;
                        preStateStr = "";
                    }
                    //前盤已有記錄
                    else if (j >= 0 && dt.Rows[i + j]["stockID"].ToString() == preStockID &&
                        Convert.ToDateTime(dt.Rows[i + j]["dtcDate"]) == preDate)
                    {
                        if (Convert.ToInt16(dt.Rows[i + j - 1]["sstate"]) == 0)
                        {
                            sqlStr = $"select sstate, stateStr from detector where dtcID={dt.Rows[i + j - 1]["dtcID"]}";
                            tempDt = CommonClass.getSQLDataTable(sqlStr);
                            preState = Convert.ToInt16(tempDt.Rows[0]["sstate"]);
                            preStateStr = tempDt.Rows[0]["stateStr"].ToString(); ;
                        }
                        else
                        {
                            preState = Convert.ToInt16(dt.Rows[i + j - 1]["sstate"]);
                            preStateStr = dt.Rows[i + j - 1]["stateStr"].ToString();
                        }
                    }
                    //本盤連續記錄
                    else if (j == 0 && dt.Rows[i + j]["stockID"].ToString() == preStockID &&
                        Convert.ToDateTime(dt.Rows[i + j]["dtcDate"]) == preDate)
                    {
                        if (Convert.ToInt16(dt.Rows[i + j - 1]["sstate"]) == 0)
                        {
                            sqlStr = $"select sstate, stateStr from detector where dtcID={dt.Rows[i + j - 1]["dtcID"]}";
                            tempDt = CommonClass.getSQLDataTable(sqlStr);
                            preState = Convert.ToInt16(tempDt.Rows[0]["sstate"]);
                            preStateStr = tempDt.Rows[0]["stateStr"].ToString();
                        }
                        else
                        {
                            preState = Convert.ToInt16(dt.Rows[i + j - 1]["sstate"]);
                            preStateStr = dt.Rows[i + j - 1]["stateStr"].ToString();
                        }
                    }
                    else
                    { //換人或換天了
                      //preStockID = currStockID = dr["StockID"].ToString();
                        preState = 0;
                    }
                    i = i + j;
                    if (Convert.ToInt16(dr["sstate"]) == 0)
                    {
                        currState = detectorDecideState(preState, Convert.ToDouble(dr["maxPanAmt"]), Convert.ToInt16(dr["cntPanAmt"]), Convert.ToDouble(dr["sumPanAmt"]),
                        Convert.ToDouble(dr["strengthIdx"]), Convert.ToDouble(dr["currStockRate"]), Convert.ToDouble(dr["dtcStockRate"]));
                        sqlStr = $"update detector set sstate = {currState},stateStr= '{preStateStr}{currState},' where dtcID = {dr["dtcID"]} ";
                        CommonClass.execSQLNonQuery(sqlStr);
                    }
                }
            } 
            catch(Exception ex)
            {
                CommonClass.smtpSendMail($"{ex.Message}{Environment.NewLine}{Environment.NewLine}{ex.StackTrace}  ",
                    new Dictionary<string, string>() { { "Scott Tseng", "scott.tseng@firstohm.com.tw" } },
                    true, "detectorState() 錯誤");
            }

            //detector56();
        }

        private void detectorPanAmtRateDiff(string alertDate, bool ifWholeDate = false)
        {
            string sqlStr = null;
            /*
            SELECT alertDate,  lastDate , `StockID`,stockName,

            MAX(CASE WHEN rn = 1 THEN Log_ID END) AS Log_ID, -999, 
            MAX(CASE WHEN rn = 1 THEN panAmtRate END) AS panAmtRate, -999, -999	, 
	        MAX(CASE WHEN rn = 1 THEN Created END) Created_1,
            MAX(CASE WHEN rn = 2 THEN panAmtRate END) AS panAmtRate_2,
            MAX(CASE WHEN rn = 2 THEN Created END) Created_2,
            MAX(CASE WHEN rn = 1 THEN panAmtRate END) - MAX(CASE WHEN rn = 2 THEN panAmtRate END) AS amtRateDiff 
            FROM (
                SELECT
                    `StockID`, `Log_ID`, `StockName`, `panAmtRate`, created, lastDate, alertDate,   
                    (@row_number := CASE WHEN @prevStock = `StockID` THEN @row_number + 1 ELSE 1 END) AS rn,
                    @prevStock := `StockID`
                FROM
                    (SELECT @row_number := 0, @prevStock := NULL) AS vars,
                    (SELECT `StockID`, `Log_ID`, `StockName`, `panAmtRate`, created, lastDate ,alertDate
                     FROM alertlog 
                     where created < '2023-12-06 12:00:00'  
                     ORDER BY `StockID`, `Log_ID` DESC) AS t
            ) AS numbered
            WHERE rn <= 2
            GROUP BY `StockID` 
            having panAmtRate < 1 and panAmtRate > -1 and (amtRateDiff >= 1 || amtRateDiff <= -1);
             */
            string curr_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Dictionary<string, object> spParamDict = new Dictionary<string, object>();
            try
            {
                if (ifWholeDate)
                {
                    for (DateTime dataTime = DateTime.Parse($"{alertDate} 09:00:00");
                         dataTime <= DateTime.Parse($"{alertDate} 13:35:00"); dataTime = dataTime.AddMinutes(5))
                    {
                        spParamDict.Add("IN_alertDate", alertDate);
                        spParamDict.Add("IN_startTime", dataTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        CommonClass.execStoreProcedure("calcDtcPrePanAmtRateDiff", spParamDict);
                        spParamDict.Clear();
                    }
                }
                else
                {
                    spParamDict.Add("IN_alertDate", alertDate);
                    spParamDict.Add("IN_startTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    CommonClass.execStoreProcedure("calcDtcPrePanAmtRateDiff", spParamDict);
                }
            }
            catch(Exception ex )
            {
                CommonClass.smtpSendMail($"sql : {sqlStr} {Environment.NewLine}" +
                    $"{ex.Message}{Environment.NewLine}{Environment.NewLine}{ex.StackTrace}  ",
                new Dictionary<string, string>() { { "Scott Tseng", "scott.tseng@firstohm.com.tw" } },
                true, "sstTray detectorPanAmtRateDiff() 錯誤");
            }
        }

        private static void detectorErrHandler(string sqlStr, Exception ex)
        {
            CommonClass.writeLog("sstTray", "detector", 3,
                $"sql: {sqlStr} | err: {ex.Message}");
            CommonClass.smtpSendMail($"sst: detector() - sql : {sqlStr} {Environment.NewLine}" +
                $"{ex.Message}{Environment.NewLine}{Environment.NewLine}{ex.StackTrace}  ",
                new Dictionary<string, string>() { { "Scott Tseng", "scott.tseng@firstohm.com.tw" } },
                true, "sstTray detector() 錯誤");
        }

        private void detector()
        {
            string sqlStr = null;
            try
            {
                sqlStr = "update alertList a " +
                    " inner join ( select stockid, alertDate , " +
                    " sum(if(panAvgVolRate * sign(panAmtDiff) >=5 , 1, 0)) as panVol5CntPos , " +
                    " sum(if(panAvgVolRate * sign(panAmtDiff) >=10 && panAvgVolRate * sign(panAmtDiff) < 20, 1, 0)) as pApRatePosCnt , " +
                    " sum(if(panAvgVolRate * sign(panAmtDiff) >=20 , 1, 0)) as panVol50CntPos , " +
                    " sum(if(panAvgVolRate * sign(panAmtDiff) <=-5 , 1, 0)) as panVol5CntNeg , " +
                    " sum(if(panAvgVolRate * sign(panAmtDiff) <= -10 && panAvgVolRate * sign(panAmtDiff) > -20, 1, 0)) as pApRateNegCnt , " +
                    " sum(if(panAvgVolRate * sign(panAmtDiff) <= -20 , 1, 0)) as panVol50CntNeg , Max(panAvgVolRate) as maxPar, " +
                    " MaxByAbsValue(max(panAvgVolRate * SIGN(panAmtDiff)), min(panAvgVolRate * SIGN(panAmtDiff))) as maxPLVR, " +
                    " sum(if(panAvgVolRate >= 5, panAvgVolRate * sign(panAmtDiff) ,0)) as maxP5VR, " +
                    " Round(sum((`panAvgVolRate` * Sign(panAmtDiff) / 10)),1) as panvolScore, " +

                    " MaxByAbsValue(max(panAmtRate), min(panAmtRate)) as `maxAmt` , " +
                    " sum(if(panAmtRate >=1, 1,0)) as `posAmtCnt`, " +
                    " sum(if(panAmtRate <=-1, 1,0)) as `negAmtCnt`, " +
                    " sum(if(panAmtRate >=2, 1,0)) as paRatePosCnt,sum(if(panAmtRate <=-2, 1,0)) as paRateNegCnt,  " +
                    " sum(if(`panLastVolRate` * Sign(panAmtDiff) >= 0.5 ,1,0)) as pLVRatePosCnt, " +
                    " sum(if (`panLastVolRate` * Sign(panAmtDiff) <= -0.5 ,1,0)) as pLVRateNegCnt, " +
                    " sum(if (`panAvg5VolRate` * Sign(panAmtDiff) >= 0.5 ,1,0)) as p5VRatePosCnt, " +
                    " sum(if (`panAvg5VolRate` * Sign(panAmtDiff) <= -0.5 ,1,0)) as p5VRateNegCnt, " +
                    " lastDate " +

                    " from alertLog " +
                    $" where alertDate = CURRENT_DATE " +
                    " group by stockid, alertDate) b on a.stockid = b.stockid " +
                    " and a.alertDate = b.alertDate " +

                    " set a.panVol5CntPos=b.panVol5CntPos, a.pApRatePosCnt = b.pApRatePosCnt , " +
                    " a.panVol50CntPos = b.panVol50CntPos, a.panVol5CntNeg=b.panVol5CntNeg, " +
                    " a.pApRateNegCnt = b.pApRateNegCnt, a.panVol50CntNeg = b.panVol50CntNeg, " +
                    " a.`maxPar`=b.`maxPar`, a.`maxAmt` = b.`maxAmt`, " +
                    " a.`posAmtCnt` = b.`posAmtCnt`, a.`negAmtCnt`=b.`negAmtCnt`, " +
                    " a.paRatePosCnt = b.paRatePosCnt, a.paRateNegCnt = b.paRateNegCnt, " +
                    " a.pLVRatePosCnt=b.pLVRatePosCnt, a.pLVRateNegCnt = b.pLVRateNegCnt, " +
                    " a.p5VRatePosCnt = b.p5VRatePosCnt, a.p5VRateNegCnt = b.p5VRateNegCnt, " +
                    " a.panvolScore = b.panvolScore, a.maxPLVR = b.maxPLVR, a.maxP5VR=b.maxP5VR, " +
                    "a.lastDate = b.lastDate ";
                CommonClass.execSQLNonQuery(sqlStr);

                //破昨量時間
                sqlStr = $"update alertlist a " +
                    $" inner join ( SELECT `StockID`, alertDate, Min(created) avg5VolTime " +
                    $" FROM `alertlog` " +
                    $" WHERE avg5VolRate >= 1 and alertDate = CURRENT_DATE " +
                    $" group by alertDate, stockid) b " +
                    $" on a.alertDate = b.alertDate and a.stockid = b.stockid " +
                    $" set a.`avg5VolTime` = b.avg5VolTime ";
                CommonClass.execSQLNonQuery(sqlStr);

                //破5量時間
                sqlStr = $"update alertlist a " +
                    $" inner join ( SELECT `StockID`, alertDate, Min(created) lastVolTime " +
                    $" FROM `alertlog` " +
                    $" WHERE lastVolRate >= 1 and alertDate = CURRENT_DATE " +
                    $" group by alertDate, stockid) b " +
                    $" on a.alertDate = b.alertDate and a.stockid = b.stockid " +
                    $" set a.lastVolTime = b.lastVolTime ";
                CommonClass.execSQLNonQuery(sqlStr);
            }
            catch (Exception ex) { detectorErrHandler(sqlStr, ex); }

            try
            {
                //主進 主出
                sqlStr = $"UPDATE alertlog a " +
                    //$" INNER JOIN( SELECT stockid, alertDate, MAX(`Log_ID`) `Log_ID`," +
                    //$"  SUM( IF( panAmtDiff > 0 AND panAvgVolRate >= 5, panVol, 0 ) ) panVol5QuanPos, " +
                    //$" SUM( IF( panAmtDiff < 0 AND panAvgVolRate >= 5, panVol, 0 ) ) panVol5QuanNeg " +
                    //$" FROM alertLog WHERE alertDate = CURRENT_DATE GROUP BY stockid, alertDate ) b " +
                    //$" ON b.Log_ID = a.Log_ID " +
                    $" Left JOIN alertlist c ON c.alertDate = CURRENT_DATE AND c.stockid = a.stockid " +
                    $" INNER JOIN investbase d ON d.stockid = a.stockid " +
                    $" SET a.panVol5CntPos = c.panVol5CntPos, a.panVol5CntNeg = c.panVol5CntNeg, " +
                    $" a.panVol50CntPos = c.panVol50CntPos, a.panVol50CntNeg = c.panVol50CntNeg, " +
                    $" a.panvolScore = c.panvolScore, a.`instantJumpKong` = priceRate(d.OpenPriec, d.lastPrice), " +
                    //$" a.panVol5QuanPos = b.panVol5QuanPos, a.panVol5QuanNeg = b.panVol5QuanNeg, " +
                    $" a.pDiff =(c.`paRatePosCnt` + c.`pLVRatePosCnt` + c.`p5VRatePosCnt` + c.`pApRatePosCnt`) -(c.`paRateNegCnt` + c.`pLVRateNegCnt` + c.`p5VRateNegCnt` + c.`pApRateNegCnt`), " +
                    $" a.messRise = d.messRise, a.instantMass = d.instantMass, a.messFall = d.messFall, " +
                    $" a.instantRise = d.instantRise, a.instantFall = d.instantFall, " +
                    $" a.RFR =  FRiseFallRate(d.instantRise,d.messRise,d.instantFall,d.messFall) , " +
                    $" a.GroupKey =  DATE_FORMAT(NOW(), '%Y%m%d%H%i%S'), " +
                    $" a.amt=IFNull(PriceRate(a.CurrPrice-a.prePrice, a.lastPrice), 0) , a.amtDiff = IFNull(c.posAmtCnt- c.negAmtCnt,0)  " +
                    $" WHERE a.alertDate = CURRENT_DATE and GroupKey is null ";
                CommonClass.execSQLNonQuery(sqlStr);
                CommonClass.wait(1);
            }
            catch (Exception ex) { detectorErrHandler(sqlStr, ex); }

            try
            {
                sqlStr = $"update investbase a " +
                        $" inner join alertList b on a.recDate = b.alertDate and a.stockid = b.stockid " +
                        $" set a.`panVol5CntPos` = b.panVol5CntPos, a.`panVol5CntNeg` = b.panVol5CntNeg, " +
                        $" a.`panVol50CntPos` = b.panVol50CntPos, a.`panVol50CntNeg` = b.panVol50CntNeg, " +
                        $" a.pDiff = (b.`paRatePosCnt`+b.`pLVRatePosCnt`+b.`p5VRatePosCnt`+b.`pApRatePosCnt`) - (b.`paRateNegCnt`+b.`pLVRateNegCnt`+b.`p5VRateNegCnt`+b.`pApRateNegCnt`), " +
                        $" a.panvolScore = b.panvolScore , " +
                        $" a.lastVolRate = Round(a.onTimeVol/a.lastVol,1), " +
                        $" a.avg5VolRate = Round(a.onTimeVol/ a.avgVol5D ,1)" + 
                        $" where alertDate = CURRENT_DATE";
                CommonClass.execSQLNonQuery(sqlStr);
                CommonClass.wait(1);
            }
            catch (Exception ex) { detectorErrHandler(sqlStr, ex); }
          
            try
            {
                sqlStr = $"insert ignore into detector " +
                    $" (`dtcDate`, `lastDate`, `StockID`, `stockName`, `Log_ID`, " +
                    $" `StockDiffRate`, `maxPanAmt`, `cntPanAmt`, `sumPanAmt`, created, " +
                    $" strengthIdx, strengthVol ) " +

                    $" select x.alertDate, x.lastDate, x.stockid, x.stockName, Log_ID, " +
                    $" priceRate( x.CurrPrice, x.lastPrice ) StockDiffRate, panAmtRate, 0 , -1, x.created, " +
                    $" Round(x.panAmtRate * x.panAvgVolRate,1), Round(if(x.panAmtRate < 0, -1 * x.panAvgVolRate, x.panAvgVolRate),1)" +
                    $" from alertLog x " +
                    $" where (panAmtRate >= 1 or panAmtRate <= -1) " +
                    $" and x.created >= DATE_SUB(CURRENT_TIMESTAMP, INTERVAL 20 MINUTE) ";

                CommonClass.execSQLNonQuery(sqlStr);
                CommonClass.wait(1);
            }
            catch (Exception ex) { detectorErrHandler(sqlStr, ex); }

            try
            {
                detectorState(DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")));
                sqlStr = $"update investbase x " +
                    $" inner join ( select a.stockid, a.statestr, a.dtcDate " +
                                 $" from detector a " +
                                    $" inner join (select StockID, Max(dtcID) maxDtcID " +
                                    $" from detector where dtcDate='{DateTime.Now.ToString("yyyy-MM-dd")}' " +
                                    $" group by StockID ) b on b.maxDtcID = a.dtcID " +
                       $" ) y on x.stockid = y.stockid  and x.recDate = y.dtcDate   " +
                       $" set x.stateStr = y.stateStr; ";
                CommonClass.execSQLNonQuery(sqlStr);
                CommonClass.wait(1);
            }
            catch (Exception ex) { detectorErrHandler(sqlStr, ex); }

            try
            {
                sqlStr = $"update detector a " +
                        $" inner join investbase b on a.stockid = b.stockid and a.dtcDate = b.recDate and a.dtcDate = CURRENT_DATE " +
                        $" inner join (select stockid, Max(dtcID) dtcID " +
                        $" from detector where dtcDate = CURRENT_DATE group by stockid ) c on a.dtcID = c.dtcID " +
                        $" set a. RFR = FRiseFallRate(b.instantRise , b.messRise , b.instantFall , b.messFall) ";
                CommonClass.execSQLNonQuery(sqlStr);
                detectorPanAmtRateDiff(DateTime.Now.ToString("yyyy-MM-dd"), false);
            }
            catch (Exception ex) { detectorErrHandler(sqlStr, ex); }
            //ai_predict
            DateTime now = DateTime.Now;
            TimeSpan startTime = new TimeSpan(9, 0, 0); // 09:00
            TimeSpan endTime = new TimeSpan(13, 35, 0); // 13:35
            TimeSpan currentTime = now.TimeOfDay;
            //執行 AI 預測
            if (currentTime >= startTime && currentTime <= endTime)
                CommonClass.RunBatchFile();
        }

        private void insertRecommand(string sqlStr, string reason)
        {
            sqlStr = "insert ignore into realtimebull " +
                    $" (`bullDate`, stockid, `dataTime`,`dtcID`,`Log_ID`,`Reason`,StockDiffRate,sumPanAmt,cnt  ) " +
                    $" SELECT a.`dtcDate`, a.stockid, " +
                    $" CURRENT_TIMESTAMP, a.dtcID , a.Log_ID, '{reason}', a.StockDiffRate, latest.sumPanAmt, cnt  " +
                    $" FROM `detector` a " +
                    $" INNER JOIN ( " +
                        $"{sqlStr}" +
                     " ) latest " +
                     " ON a.stockID = latest.stockID AND a.created = latest.latest_created " +
                     " GROUP BY a.stockID, a.`dtcDate`, latest.sumPanAmt " +
                     " having sumPanAmt > 0 and a.StockDiffRate <= 6 " +
                     " ORDER BY sumPanAmt DESC limit 20; ";
            CommonClass.execSQLNonQuery(sqlStr);
        }

        private void calcRecommand()
        {
            string dataDate = DateTime.Now.ToString("yyyy-MM-dd");
            string sqlStr = null;
            DateTime currentTime = DateTime.Now;
            try
            {
                if(currentTime.TimeOfDay >= new TimeSpan(10, 30, 0) && currentTime.TimeOfDay <= new TimeSpan(13, 30, 0))
                {
                    sqlStr = $"SELECT b.`StockID`, b.`stockName`, b.`StockType`, " +
                        $" a.Created as latest_created, a.stateStr, a.Log_ID, " +
                        $" a.sumPanAmt, a.`cntPanAmt` cnt " +
                        $" FROM `detector` a " +
                        $" inner join tradedata b on a.StockID = b.StockID and a.lastDate =b.TransDate " +
                        $" inner join investBase c on a.StockID = c.StockID " + 
                        $" inner join tradeData d on d.stockid = b.stockid and d.transDate = b.lastDate  " + 
                        $" inner join alertLog e on e.Log_ID = a.Log_ID " + 
                        $" where dtcDate = '{dataDate}' and " +
                        $" (a.`stateStr` like '1,7,7,' or a.`stateStr` like '%9,7,7,' " +
                        $" or a.`stateStr` like '%4,7,7,') " + 
                        $" and (d.panVol5CntPos - d.panVol5CntNeg +  d.panVol50CntPos*2 - d.panVol50CntNeg*2) >= 0 " + 
                        $" having Time(latest_created) > '10:45:00' ";

                    insertRecommand(sqlStr, "117中午買進");
                    CommonClass.wait(1);
                }
                /************************/
                sqlStr = $" select x.StockID, x.dtcID, x.`Created` latest_created, " +
                    $" x.`sstate`, x.`stateStr`, x.created, " +
                    $" x.`StockDiffRate` dtc_StockDiffRate, `maxPanAmt`," +
                    $" `sumPanAmt`,`strengthIdx`, `strengthVol`, x.cntPanAmt cnt from detector x" +
                    $" inner join ( select a.`StockID`, Max(a.`dtcID`) dtcID from detector a " +
                            $" inner join ( select StockID, Min(`Log_ID`) Log_ID from alertlog " +
                            $" where alertDate = '{dataDate}' and Time(created) < '10:30:00' " +
                            $" and (avg5VolRate > 0.9 || lastVolRate > 0.9) Group by StockID" +
                            $" ) b on a.StockID=b.StockID and a.dtcDate = '{dataDate}' and a.Log_ID > b.Log_ID and " +
                            $" (a.sstate=1 || a.sstate=9 || a.sstate=7 || a.sstate=4) group by a.`StockID`) y " +
                            $" on x.dtcID=y.dtcID " +
                    $" inner join alertLog c on c.Log_ID = x.Log_ID " + 
                    $" inner join tradeData d on d.stockid = x.stockid and d.transDate = x.lastDate  " + 
                    $" where count_negative_values(`cntPanAmt`,`sumPanAmt`,`strengthIdx`,`strengthVol`) <= 1 " +
                    $" and (c.`panVol50CntPos`* 2 - c.`panVol50CntNeg`*2 + c.`panVol5CntPos`- c.`panVol5CntNeg`) > 3 " +
                    $" and (d.panVol5CntPos-d.panVol5CntNeg + d.panVol50CntPos-d.panVol50CntNeg) >= 0";
                insertRecommand(sqlStr, "破量上攻");
                CommonClass.wait(1);
                /******************************************/
                sqlStr = $"SELECT distinct a.`StockID`, a.`StockName`, a.stateStr , " +
                    $" a.`StockType`, b.Created as latest_created, b.Log_ID, " +
                    $" b.sumPanAmt, b.`cntPanAmt` cnt FROM `investBase` a " +
                    $" inner join detector b on a.StockID=b.StockID and " +
                    $" a.recDate=b.dtcDate and " +
                    $" (b.statestr like '1,7,7,%' or b.statestr like '%9,7,7,%' or b.statestr like '%4,7,7,%') " +
                    $" inner join tradeData d on d.stockid = a.stockid and d.transDate = a.recDate  " +
                    $" WHERE recDate = '{dataDate}' and a.`StockType`='興櫃' " +
                    $" and (d.panVol5CntPos-d.panVol5CntNeg + d.panVol50CntPos-d.panVol50CntNeg) >= 0 ";
                insertRecommand(sqlStr, "177 興櫃");
                CommonClass.wait(1);
                /******************************************/
                sqlStr = $"SELECT a.stockID, a.`stockName`, b.stockType, " +
                    $" sum(if(`maxPanAmt` >=2,1,0)) panAMTcnt ,  " +
                    $" a.stateStr , a.Created as latest_created, a.Log_ID, " +
                    $" a.sumPanAmt, a.`cntPanAmt` cnt " +
                    $" FROM `detector` a " +
                    $" inner join investbase b on a.stockid = b.stockid " +
                    $" and (b.panVol50CntPos * 4 + panVol5CntPos - b.panVol50CntNeg * 4 - b.panVol5CntNeg ) >=4 " +
                    $" inner join tradeData d on d.stockid = a.stockid and d.transDate = a.dtcDate " +
                    $" where dtcDate = recDate " +
                    $" and (d.panVol5CntPos-d.panVol5CntNeg + d.panVol50CntPos-d.panVol50CntNeg) >= 0 " +
                    $" group by stockID having panAMTcnt >= 1.5 ";
                insertRecommand(sqlStr, "盤中急漲2%量上漲");
                CommonClass.wait(1);
                /********************************************/
                sqlStr = $"SELECT a.stockID, a.`stockName`, b.stockType, " +
                    $" c.stateStr, a.Created as latest_created, a.Log_ID, a.sumPanAmt, " +
                    $" a.`cntPanAmt` cnt FROM `detector` a " +
                    $" inner join tradedata b on a.StockID = b.StockID and a.lastDate =b.TransDate " +
                    $" inner join investBase c on a.StockID = c.StockID " +
                    $" inner join tradeData d on d.stockid = a.stockid and d.transDate = a.dtcDate " +
                    $" inner join alertLog e on e.Log_ID = a.Log_ID " +
                    $" where dtcDate = '{dataDate}' " +
                    $" and (a.`stateStr` like '1,7,7,' or a.`stateStr` like '%9,7,7,' " +
                    $" and (d.panVol5CntPos-d.panVol5CntNeg + d.panVol50CntPos-d.panVol50CntNeg) >= 0 " +
                    $" or a.`stateStr` like '%4,7,7,') " +
                    $" having Time(latest_created) > '10:45:00'";
                insertRecommand(sqlStr, "117中午買進");
                CommonClass.wait(1);
                /***************************************/
                sqlStr = $"SELECT a.stockID, b.`stockName`, b.stockType, " +
                    $" c.stateStr , a.Created as latest_created, a.Log_ID, " +
                    $" a.sumPanAmt, a.`cntPanAmt` cnt " +
                    $" FROM `detector` a " +
                    $" inner join tradedata b on a.StockID = b.StockID and a.lastDate =b.TransDate and b.`turnoverRate` >=5 and b.`turnoverRate` <=20 " +
                    $" inner join investbase c on a.StockID = c.StockID and a.dtcDate =c.recDate and c.panVol50CntPos > 0 " +
                    $" and c.panVol50CntNeg = 0 and (c.panVol5CntPos >= c.panVol5CntNeg) " +
                    $" inner join ( select Max(dtcID) maxDtcId from detector " +
                    $" where (`stateStr` like '1,7,7,%' or `stateStr` like '%9,7,7,%' or " +
                    $" `stateStr` like '%4,7,7,%') group by stockid, dtcDate ) d " +
                    $" on a.dtcID = d.maxDtcId " +
                    $" inner join alertLog e on e.Log_ID = a.Log_ID " +
                    $" inner join tradeData f on f.stockid = a.stockid and f.transDate = a.dtcDate " +
                    $" where dtcDate = '{dataDate}' " +
                    $" and (f.panVol5CntPos-f.panVol5CntNeg + f.panVol50CntPos-f.panVol50CntNeg) >= 0 ";
                insertRecommand(sqlStr, "周轉N77");
                CommonClass.wait(1);
            }
            catch(Exception ex)
            {
                CommonClass.smtpSendMail($"sql : {sqlStr} {Environment.NewLine}" +
                $"{ex.Message}{Environment.NewLine}{Environment.NewLine}{ex.StackTrace}  ",
                new Dictionary<string, string>() { { "Scott Tseng", "scott.tseng@firstohm.com.tw" } },
                true, "sstTray calcRecommand() 錯誤");
            }
        }

        public void adjust0900Vol()
        {
            string sqlStr = null;
            //  tradedata
            /*  sqlStr =
                $" update alertlog x " +
                $" inner join ( " +
                $" SELECT distinct a.Log_ID, a.stockid, c.stockName,a.CurrPrice, c.lastPrice,  a.diffRate, priceRate(a.CurrPrice , c.lastPrice),  c.StockDiffRate  ,  " +
                $" a.CurrPrice-c.lastPrice  panAmtDiff, " +
                $" Round((a.CurrPrice-c.lastPrice)/c.lastPrice * 100,2) panAmtDiffRate, " +
                $" a.`CurrVol` , a.panVol/(c.lastVol/90) panAvgVolRate , Round(a.CurrVol/c.lastVol,1) panLastVolRate , Round(a.CurrVol/ c.avgVol5D) panAvg5VolRate  " +
                $" FROM  tradedata c  " +
                $" inner join (select Min(Log_ID)Log_ID,  stockid, alertDate from `alertlog` where  Time(preTime)='09:00:00' group by stockid, alertDate ) b on b.stockid = c.stockid  " +
                $" and b.alertDate = c.TransDate  " +
                $" inner join alertlog a on a.Log_ID = b.Log_ID    " +
                $" ) y  on x.Log_ID=y.Log_ID  " +
                $" set x.`panAmtDiff` = y.panAmtDiff, x.`panAmtRate` = y.panAmtDiffRate, x.panAvgVolRate = y.panAvgVolRate , x.panLastVolRate=y.panLastVolRate, x.panAvg5VolRate=y.panAvg5VolRate ";
            */
            sqlStr = $"select Max(recDate) recDate, Max(lastDate) lastDate from investbase";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            string recDate = Convert.ToDateTime(dt.Rows[0]["recDate"]).ToString("yyyy-MM-dd");
            string lastDate = Convert.ToDateTime(dt.Rows[0]["lastDate"]).ToString("yyyy-MM-dd");

            sqlStr = $"update `alertlog` a " +
                $" inner join tradedata b on b.TransDate=a.lastDate and a.stockid = b.StockID " +
                $" and Time(preTime) = '09:00:00' " +
                $" set a.lastPrice = b.StockPrice where alertDate = '{recDate}' ";
            CommonClass.execSQLNonQuery(sqlStr);

            sqlStr =
                $" update alertlog x " +
                $" inner join ( " +
                    $" SELECT distinct a.Log_ID, a.stockid, c.stockName, a.alertDate, a.CurrPrice, c.lastPrice, " +
                    $" a.CurrPrice-c.lastPrice panAmtDiff, priceRate(a.CurrPrice , d.`StockPrice`) panAmtDiffRate , a.`CurrVol` , " +
                    $" a.panVol/(c.lastVol/90) panAvgVolRate , Round(a.CurrVol/c.lastVol,1) panLastVolRate , " +
                    $" Round(a.CurrVol/ c.avgVol5D,1) panAvg5VolRate " +
                    $" FROM investbase c " +
                    $" inner join tradedata d on c.stockid=d.stockid and d.transDate = c.lastDate " +
                    $" inner join ( select Min(Log_ID)Log_ID, stockid, alertDate from `alertlog` " +
                    $" where Time(preTime)='09:00:00' group by stockid, alertDate ) b " +
                    $" on b.stockid = c.stockid and b.alertDate = c.recDate " +
                    $" inner join alertlog a on a.Log_ID = b.Log_ID " +
                $" ) y on x.Log_ID=y.Log_ID " +
                $" set x.`panAmtDiff` = y.panAmtDiff, x.`panAmtRate` = y.panAmtDiffRate, x.panAvgVolRate = y.panAvgVolRate , x.panLastVolRate=y.panLastVolRate, x.panAvg5VolRate=y.panAvg5VolRate";
            CommonClass.execSQLNonQuery(sqlStr);
        }

        public List<string> dtToLineMsg(string msgType, string sqlStr, string dataDate , int idx_Type=1, List<string> msgList = null, bool ifDtc = false)
        {
            string tempStr = $"{msgType}:ID--Name--Type--現幅--str";
            msgList.Add(tempStr);
            int i = 1;
            idx_Type = 0;
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            if (dt.Rows.Count == 0)
                return msgList;

            List<string> a_trade_ID_List = null;
            if (dt != null && dt.Rows.Count > 0)
                a_trade_ID_List = CommonClass.dtToList(dt, 0, true, true, a_trade_ID_List);

            /////////////////////////////////////////////////////
            if (a_trade_ID_List == null || a_trade_ID_List.Count == 0)
                return msgList;
            sqlStr = $"select stockid, stockname, stockType, " +
                $" onTimePrice, priceRate(onTimePrice, lastPrice) " +
                $" from investBase where stockid in ({string.Join(",", a_trade_ID_List)}) ";
            dt = CommonClass.getSQLDataTable(sqlStr);
            foreach (DataRow dr in dt.Rows)
            {
                tempStr = $"{idx_Type}-{i++}**{dr[0]}--{dr[1]}// {dr[2]}// {dr[3]}// {dr[4]}{Environment.NewLine}";
                msgList.Add(tempStr);
            }
            msgList.Add("******************");
            sqlStr = $"INSERT INTO `notifylog`(`alertDate`, `reason`, `StockID`, `StockName`, " +
                $" `stockDiffRate`, `lastVolRate`, `avg5VolRate`, " +
                $" `maxPAR`, `maxAmt`, `posAmtCnt`, `negAmtCnt`, `messRise`, `alertCnt`, " +
                $" createPRate, HPRate, LPRate, serialAmtCnt) " +
                $" select recDate, '{msgType}', a.stockid, a.stockname, " +
                $" priceRate(a.onTimePrice, a.lastPrice), Round(a.onTimeVol/a.lastVol,1), Round(a.onTimeVol/ a.avgVol5D ,1), " +
                $" b.`maxPAR`, b.`maxAmt`, b.`posAmtCnt`, b.`negAmtCnt`, a.`messRise`, 1, " +
                $" priceRate(a.onTimePrice, a.lastPrice), priceRate(a.onTimePrice, a.lastPrice), " +
                $" priceRate(a.onTimePrice, a.lastPrice), " +
                $" if(TIMESTAMPDIFF(SECOND, b.updated, CURRENT_TIMESTAMP) <=60 and b.currPanAmtRate >= 0.5, 1, 0)" +
                $" from investBase a " +
                $" inner join alertList b on a.StockID=b.StockID and a.recDate=b.alertDate " +
                $" where a.stockid in  ({string.Join(",", a_trade_ID_List)}) " +
                $" ON DUPLICATE KEY UPDATE alertDate = VALUES(alertDate), reason = VALUES(reason), " +
                $" StockName = VALUES(StockName), stockDiffRate = VALUES(stockDiffRate), " +
                $" lastVolRate = VALUES(lastVolRate), avg5VolRate = VALUES(avg5VolRate), " +
                $" maxPAR = VALUES(maxPAR), maxAmt = VALUES(maxAmt), posAmtCnt = VALUES(posAmtCnt), " +
                $" negAmtCnt = VALUES(negAmtCnt), messRise = VALUES(messRise), alertCnt = alertCnt + 1, " +
                $" HPRate=(if(HPRate > priceRate(a.onTimePrice, a.lastPrice), HPRate , priceRate(a.onTimePrice, a.lastPrice))), " +
                $" LPRate=(if(LPRate < priceRate(a.onTimePrice, a.lastPrice), LPRate , priceRate(a.onTimePrice, a.lastPrice))), " +
                $" serialAmtCnt = if(TIMESTAMPDIFF(SECOND, b.updated, CURRENT_TIMESTAMP) <=60, serialAmtCnt, if(b.currPanAmtRate >= 0.5, serialAmtCnt + 1, 0)) ";
            CommonClass.execSQLNonQuery(sqlStr);
            return msgList;
        }


        public void sst_sendLine(int lineType, int currMin=3)
        {
            //string sqlStr = "update notifylog set alertCnt = alertCnt * -1 " +
            //    " where alertDate = CURRENT_DATE and `alertCnt` > 0";
            //CommonClass.execSQLNonQuery(sqlStr);
            string sqlStr = null;
            List<string> msgList = new List<string>();
            string dataDate = DateTime.Now.ToString("yyyy-MM-dd");
            //string dataDate = "2023-12-06";
            msgList.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            int idx_Type = 0;
            try
            {
                switch (lineType)
                {
                    case 1: //早上 0630 發簡訊
                        sqlStr = $"SELECT Max(`StockDate`) from stock60days";
                        dataDate = Convert.ToDateTime(CommonClass.getSQLScalar(sqlStr)).ToString("yyyy-MM-dd");
                        msgList.Add($"stock60days Date : {dataDate}");
                        sqlStr = $"select count(*) from stock60days where `StockDate`='{dataDate}' ";
                        msgList.Add($"stock60days Date : {CommonClass.getSQLScalar(sqlStr)}");
                        sqlStr = $"select count(*) from tradedata where `transdate`='{dataDate}' ";
                        msgList.Add($"tradedata Date : {CommonClass.getSQLScalar(sqlStr)}");
                        sqlStr = $"select count(*) from tradedata where `transdate`='{dataDate}' and quanziRate > 0 ";
                        msgList.Add($"券資比 Date : {CommonClass.getSQLScalar(sqlStr)}");
                        break;
                    case 2: //早上 0915 發簡訊
                            //////////////////////////////////////////////////////////////////////////
                            //sqlStr = $"SELECT a.`StockID` " +
                            //                $" FROM `i_baldhead_red` a " +
                            //                $" inner join alertList b on a.stockid = b.stockid " +
                            //                $" and b.alertDate = recDate " +
                            //                $" where stateStr like '1,7,%' " +
                            //                $" and `p5VRatePosCnt` + p5VRatePosCnt * 3 - (`p5VRateNegCnt` + p5VRateNegCnt * 3) > 0";
                            //idx_Type =idx_Type++;
                            //msgList = dtToLineMsg("光頭", sqlStr, dataDate, idx_Type, msgList, false);
                            ////////////////////////////////////////////////////////////////////////////

                        ////破量當盤上漲 3.5 以上， 且stateStr='1,7,', '4,7','9,7'
                        ////sqlStr = $"SELECT distinct a.`StockID` FROM `detector` a inner join alertLog b on a.Log_ID= b.Log_ID where `dtcDate` = CURRENT_DATE and a.`maxPanAmt` >= 3.5 and (`stateStr` = '1,7,' or `stateStr` = '4,7,' or `stateStr` = '9,7,') and (b.avg5VolRate > 0.9 or b.lastVolRate > 0.9) ";
                        ////List<string> a_trade_ID_List = null;
                        //sqlStr = $"SELECT `StockID` FROM `i_gggInR` ";
                        //idx_Type = idx_Type++;
                        //msgList = dtToLineMsg("紅吞黑", sqlStr, dataDate, idx_Type, msgList, true);

                        ////SELECT distinct a.`StockID` FROM `detector` a inner join detector b on a.lastDate = b.dtcDate and a.stockID = b.stockID and ChkRiseStrengthNow(b.`stateStr`) >= 1 where a.`dtcDate` = CURRENT_DATE and ChkRiseStrengthNow(a.`stateStr`)=1 and a.`maxPanAmt` >= 4 and CURRENT_DATE 
                        //// 大幅上漲 4 以上， 且stateStr = '1,7,', '4,7','9,7'
                        //sqlStr = $"SELECT `StockID` FROM `i_morningstart_inst` ";
                        //idx_Type = idx_Type++;
                        //msgList = dtToLineMsg("晨星", sqlStr, dataDate, idx_Type, msgList, true);
                        //break;
                        sqlStr = $"SELECT b.StockID, b.StockName, " +
                            $" priceRate(d.onTimePrice, c.StockPrice) priceRate " +
                            $" FROM `alertlist` a inner join tradeData b on a.stockid = b.stockid and b.transDate = a.alertDate inner join tradeData c on a.stockid = c.stockid and c.transDate = b.lastDate " +
                            $" inner join investbase d on a.stockid = d.stockid and d.recDate = a.alertDate " +
                            $" WHERE Time(a.`lastVolTime`) < '09:40:00' and a.alertDate = d.recDate " +
                            $" and pLVRateNegCnt <= 1 and c.StockDiffRate < 4 " +
                            $"and `paRatePosCnt`-`paRateNegCnt` >= 2 and maxAmt > 2 and a.panvolScore > 5";
                        idx_Type = idx_Type++;
                        msgList = dtToLineMsg("破昨强攻", sqlStr, dataDate, idx_Type, msgList, false);

                        string lastMonth = CommonApp.fewDaysAgo(DateTime.Now, 30).ToString("yyyy-MM-dd");
                        sqlStr = "SELECT a.StockID, a.StockName, amplitude, a.onTimeVol, " +
                            " priceRate(a.onTimePrice, a.lastPrice) priceRate " +
                            " FROM `investBase` a " +
                            " inner join tradedata c on c.transDate = a.lastDate " +
                            " and c.stockid = a.stockid and (jumpkong(a.OpenPriec, a.onTimePrice, c.OpenPriec, c.StockPrice ) >= 3 " +
                            " or (a.ontimePrice-a.OpenPriec)/a.lastPrice * 100 >= 3.6) " +
                            " inner join alertList d on d.alertDate = a.recDate and d.stockid = a.stockid " +
                            " and d.posAmtCnt >=2 and negAmtCnt = 0 " +
                            " inner join ( select stockid,Max(stockPrice) maxStockPrice, Min(stockPrice) minStockPrice, " +
                            " (Max(stockPrice)-Min(stockPrice))/Min(stockPrice) * 100 amplitude " +
                            $" from tradedata where transDate >= '{lastMonth}' and transDate < '{DateTime.Now.ToString("yyyy-MM-dd")}' " +
                            $" group by stockid having amplitude <= 5 ) b on a.stockid = b.stockid " +
                            $" where (a.lastPrice-minStockPrice)/minStockPrice * 100 < 3 and a.onTimeVol >= 80";
                        idx_Type = idx_Type++;
                        msgList = dtToLineMsg("窒息突破", sqlStr, dataDate, idx_Type, msgList, false);

                        sqlStr = $"SELECT a.`StockID` FROM `i_bigPanAmt` a " +
                            $" inner join alertList b on a.StockID=b.StockID and b.alertDate = CURRENT_DATE " +
                            $" where b.posAmtCnt > b.negAmtCnt ";
                        idx_Type = idx_Type++;
                        msgList = dtToLineMsg("BigPanAmt", sqlStr, dataDate, idx_Type, msgList, false);

                        sqlStr = $"SELECT a.`StockID` FROM `i_bigPanVolRate`  a " +
                            $" inner join alertList b on a.StockID=b.StockID and b.alertDate = CURRENT_DATE " +
                            $" where b.posAmtCnt > b.negAmtCnt ";
                        idx_Type = idx_Type++;
                        msgList = dtToLineMsg("BigPanVolRate", sqlStr, dataDate, idx_Type, msgList, false);

                        //sqlStr = $"SELECT Round((`上市上漲`+ `上櫃上漲`)/(`上市下跌`+ `上櫃下跌`),2) 全部漲跌比, Round(`上市上漲`/`上市下跌`,2) 上市漲跌比, Round(`上櫃上漲`/`上櫃下跌`,2) 上櫃漲跌比 FROM `dailysummary`; ";
                        sqlStr = "select Round(上市上漲/上市下跌,1) 上市漲跌比, Round(上櫃上漲/上櫃下跌,1) 上櫃漲跌比, 上市漲停, 上市跌停, 上櫃漲停, 上櫃跌停,Round(興櫃上漲/興櫃下跌,1) 興櫃漲跌比 from dailysummary";
                        DataTable dt = CommonClass.getSQLDataTable(sqlStr);
                        if (dt.Rows.Count > 0)
                        {
                            msgList.Add($"上市比: {dt.Rows[0]["上市漲跌比"]}, 上櫃比: {dt.Rows[0]["上櫃漲跌比"]}, 興櫃比: {dt.Rows[0]["興櫃漲跌比"]}");
                            msgList.Add($"上市漲停 : {dt.Rows[0]["上市漲停"]}, 上市跌停 : {dt.Rows[0]["上市跌停"]}");
                            msgList.Add($"上櫃漲停 : {dt.Rows[0]["上櫃漲停"]}, 上櫃跌停 : {dt.Rows[0]["上櫃跌停"]}");
                        }

                        break; 
                    case 3: //第二次 loop 時發此訊息
                        sqlStr = $"SELECT a.stockid, a.stockName, min(b.panAmtRate) minAmt " +
                            $" FROM `activestocks` a " +
                            $" inner join alertLog b on alertDate = CURRENT_DATE and a.stockid = b.stockid and Time(b.created) < '09:10:00' " +
                            $" where a.stockLeftCount >= 1 " +
                            $" group by stockid having minAmt <= -1.5";
                        idx_Type = idx_Type++;
                        msgList = dtToLineMsg("0910--我股快逃", sqlStr, dataDate, idx_Type, msgList, false);

                        sqlStr = $"SELECT a.StockID, a.stockName, priceRate(a.onTimePrice, a.lastPrice) spd, " +
                            $" b.`maxPAR`, b.`maxAmt`, b.`posAmtCnt`-b.`negAmtCnt`, a.`panvolScore` , c.StockDiffRate " +
                            $" FROM `investbase` a " +
                            $" inner join alertList b on a.recDate = b.alertDate and a.stockid = b.stockid " +
                            $" inner join tradeData c on c.stockid = a.StockID and c.transDate = a.lastDate " +
                            $" and c.StockDiffRate < 2 " +
                            $" where b.`posAmtCnt`-b.`negAmtCnt` > 1 and a.`panvolScore` >= 2 and b.maxPAR > 35 ";
                        idx_Type = idx_Type++;
                        msgList = dtToLineMsg("0910--開盤强檔", sqlStr, dataDate, idx_Type, msgList, false);
                        break;
                }
                if (msgList.Count == 0)
                    return;
                //List<string>  lineUser  = new List<string>() { "曾建明" };
                
                string lineMsg = string.Join(Environment.NewLine, msgList);
                //if (DateTime.Now.TimeOfDay <= TimeSpan.Parse("09:25"))
                if (lineType == 3) //不要 send line
                    return;
                if(currMin==3)
                {
                    linelib.pushTextMessageByWebapiAsync("scott.tseng", $"sst_sendLine() {lineMsg}");
                }   //else
                    //CommonClass.smtpSendMail(lineMsg,new Dictionary<string, string>() { { "Scott Tseng", "scott.tseng@firstohm.com.tw" } },
                    //    true, $"sst 半點警訊 ");
                
            }
            catch (Exception ex)
            {
                //linelib.pushTextMessageByWebapiAsync("scott.tseng", $"sst_sendLine()  開盤發Line錯誤:{ex.Message}");
            }
        }
        public void OnTimer_timerSysTray(object sender, System.Timers.ElapsedEventArgs args)
        {
            try
            {
                string sqlStr = null;
                int currHour = DateTime.Now.Hour;
                int currMin = DateTime.Now.Minute;
                DateTime currentTime = DateTime.Now;
                int dailyLoopCnt = 0;
                StringBuilder sb = new StringBuilder();
                //sst_sendLine
                if ((int)DateTime.Now.DayOfWeek == 0 || (int)DateTime.Now.DayOfWeek == 6 || CommonClass.isHoliday(DateTime.Now, true))
                    return;
                if (!CommonClass.isHoliday(DateTime.Now, true) && currHour == 9 && currMin == 15)
                    sst_sendLine(2);
                else if (!CommonClass.isHoliday(DateTime.Now, true) && currHour == 6 && currMin == 30)
                    sst_sendLine(1);
                ///////////////////////////
                 
                if (autoExecItems.Contains("do_sst") 
                    && currentTime.TimeOfDay >= new TimeSpan(8, 0, 0)
                    && currentTime.TimeOfDay <= new TimeSpan(13, 42, 0))
                {

                    if (doSSTTime.Date <= DateTime.Now.Date)
                    {
                        doSSTTime = DateTime.Now;
                        CommonApp.sstDayProcessErrCnt = 0;
                    }

                    if (frmDoSst != null)
                    {
                        frmDoSst.txtLogoutTime.Text = CommonApp.loginTime;
                        frmDoSst.txtLoginTime.Text = CommonApp.logoutTime;
                        frmDoSst.txtStartTime.Text = CommonApp.sstProcessStart;
                        frmDoSst.txtEndTime.Text = CommonApp.sstProcessEnd;


                        frmDoSst.txtStackTrace.Text = CommonApp.sstLastErrMsg;
                        frmDoSst.txtLastErrorTime.Text = CommonApp.sstLastErrTime;
                        frmDoSst.txtDayErrCnt.Text = CommonApp.sstDayProcessErrCnt.ToString();
                        frmDoSst.txtErrorCnt.Text = CommonApp.sstDayProcessErrCnt.ToString();
                        frmDoSst.Refresh();
                    }
                    //2023-07-26 改為每 30 分鐘全部 Parse 一次
                    if (!CommonClass.isHoliday(DateTime.Now, true))
                    {
                        if (currHour == 8 && currMin >= 45 && currMin % 5 == 0) {
                            sst.shioajiLogin();
                            sst.do_sst(currHour, currMin, Constants.SSTConnString, 1);
                            sqlStr = $"update investbase set stateStr = ''";
                            CommonClass.execSQLNonQuery(sqlStr);
                            dailyLoopCnt = 0;
                        }
                        else if (currHour ==9 &&  currMin <= 30 && currMin > 0  && currMin % 5 == 0 ) //9:03-9:30 3 分鐘一次
                        {
                            dailyLoopCnt++;
                            sst.do_sst(currHour, currMin, Constants.SSTConnString, 1);
                            if (currMin >= 6 && currMin <= 12)
                                adjust0900Vol();
                            detector();
                            if(currMin > 10)
                                calcRecommand();
                            if(dailyLoopCnt==2) //0907 寫 notifyLog 
                                sst_sendLine(3);
                        }
                        else if (currHour == 9 && currMin > 30 && (currMin %5==0)) //9:30-10:00 5 分鐘一次
                        {
                            sst.do_sst(currHour, currMin, Constants.SSTConnString, 1);
                            detector();
                            calcRecommand();
                            sst_sendLine(3);
                        }
                        else if (currHour == 10 &&  (currMin % 10 == 0)) //10:00-11:00 10 分鐘一次
                        {
                            sst.do_sst(currHour, currMin, Constants.SSTConnString, 1); //10:00-11:00 5 分鐘一次
                            detector();
                            calcRecommand();
                            sst_sendLine(3);
                        }
                        else if (currHour >= 11 && currHour < 12 && (currMin % 10 == 0)) //10:00-13:00 15 分鐘一次
                        {
                            sst.do_sst(currHour, currMin, Constants.SSTConnString, 1);
                            detector();
                            calcRecommand();
                            sst_sendLine(3);
                        }
                        //else if (currHour >= 12 && currHour < 13 && (currMin % 20 == 0)) //10:00-13:00 15 分鐘一次
                        //{
                        //    sst.do_sst(currHour, currMin, Constants.SSTConnString, 1);
                        //    detector();
                        //    calcRecommand();
                        //}
                        else if (currHour >= 13 && currMin < 35 && (currMin % 5 == 0)) //10:00-13:00 15 分鐘一次
                        {
                            sst.do_sst(currHour, currMin, Constants.SSTConnString, 1);
                            detector();
                            calcRecommand();
                            sst_sendLine(3);
                        }
                        else if (currHour > 13 && (currMin % 10 == 0)) //收盤以後， 10分鐘一次
                        {
                            sst.do_sst(currHour, currMin, Constants.SSTConnString, 1);
                            detector();
                            calcRecommand();
                        }
                        if (currHour >= 9 && currHour < 14 && (currMin == 30 || currMin == 0))
                            sst_sendLine(2, currMin);
                    }

                    //每日收盤檢查：13:35 確認 alertlog 有無資料
                    if (currHour == 13 && currMin == 35 && !CommonClass.isHoliday(DateTime.Now, true)
                        && (int)DateTime.Now.DayOfWeek != 0 && (int)DateTime.Now.DayOfWeek != 6)
                    {
                        object cntObj = CommonClass.getSQLScalar("SELECT COUNT(*) FROM sst.alertlog WHERE alertDate = CURRENT_DATE");
                        int rowCnt = cntObj != null && cntObj != DBNull.Value ? Convert.ToInt32(cntObj) : 0;
                        if (rowCnt == 0)
                        {
                            string warnMsg = $"sstTray 每日檢查：今日 alertlog 無任何資料，shioaji 資料收集可能異常";
                            CommonClass.writeLog("sstTray", "dailyCheck", 3, warnMsg);
                            CommonClass.smtpSendMail(warnMsg,
                                new Dictionary<string, string>() { { "Scott Tseng", "scott.tseng@firstohm.com.tw" } },
                                true, "sstTray 資料收集異常");
                        }
                        else if (rowCnt < 100)
                        {
                            string warnMsg = $"sstTray 每日檢查：今日 alertlog 僅 {rowCnt} 筆（異常偏低）";
                            CommonClass.writeLog("sstTray", "dailyCheck", 2, warnMsg);
                        }
                    }

                    if (CommonApp.sendSSTStartLine.Date < DateTime.Now.Date)
                        CommonApp.sendSSTStartLine = DateTime.Now;
                    if (frmDoSst != null)
                    {
                        frmDoSst.txtLogoutTime.Text = CommonApp.loginTime;
                        frmDoSst.txtLoginTime.Text = CommonApp.logoutTime;
                        frmDoSst.txtStartTime.Text = CommonApp.sstProcessStart;
                        frmDoSst.txtEndTime.Text = CommonApp.sstProcessEnd;

                        frmDoSst.txtStackTrace.Text = CommonApp.sstLastErrMsg;
                        frmDoSst.txtLastErrorTime.Text = CommonApp.sstLastErrTime;
                        frmDoSst.txtDayErrCnt.Text = CommonApp.sstDayProcessErrCnt.ToString();
                        frmDoSst.txtErrorCnt.Text = CommonApp.sstDayProcessErrCnt.ToString();
                        frmDoSst.Refresh();
                    }
                }

                //啟動 stocktray
                if (autoExecItems.Contains("syncTrade") && currHour == 18 && currMin == 20)
                {
                    string stockTrayExec = Constants.getProperty("stockTrayPath");
                    string stockTrayPath = CommonClass.getFileInfo(stockTrayExec, "directory");
                    CommonApp.sstProcessId = CommonClass.startProcessRtnPID(stockTrayExec, "noInfo", stockTrayPath);
                }

                //Kill stocktray
                if (autoExecItems.Contains("syncTrade") && currentTime.TimeOfDay >= new TimeSpan(23, 00, 0))
                {
                    if (CommonApp.sstProcessId > 0)
                        CommonClass.KillProcess(CommonApp.sstProcessId);
                }

                //老師 Line
                if (autoExecItems.Contains("sync_pay") && currentTime.TimeOfDay >= new TimeSpan(8, 00, 0) 
                    && currentTime.TimeOfDay <= new TimeSpan(14, 00, 0))
                {
                    sst.syncTeacherEvent();
                    if (currMin == 10) //交易時間以外， 1小時一次
                    {
                        //一個小時 Parse 一次建議訊息
                        //sst.proceessTeacherEvent(foWebClient); 
                    }
                }
                else
                {
                    if(currMin==10) //交易時間以外， 1小時一次
                    {
                        sst.syncTeacherEvent();
                        if (currHour == 20) //每天晚上 8 點再 Parse 一次建議訊息
                        {
                            //sst.proceessTeacherEvent(foWebClient);
                        }
                    }
                }

                //以下 自動執行 Akeeba 備份 
                //每天 3AM 備份一次
                if (autoExecItems.Contains("akeeba") && DateTime.Now.Date > Constants.lastAkeebaBackup.Date && currHour == 3)
                {
                    Constants.lastAkeebaBackup = DateTime.Now;
                    CommonApp.autoAkeebaBackup();
                }
                //凌晨四點固定關掉 akeebaBackup Chrome
                if (autoExecItems.Contains("akeeba") && currHour == 5 && CommonApp.akeebaBKPic.Count > 0)
                {
                    CommonApp.killAkeebaProcess();  
                }

                //18:30 備份 SST
                if (autoExecItems.Contains("sst_backup") && currHour == 18 && currMin ==30 )
                {
                    string bkResult = null;
                    cmBK.backupDB("sst", @"D:\DBbackup", "HL35", out bkResult, 0);
                    cmBK.backupDB("sst", @"D:\DBbackup", "HL35", out bkResult, 1);
                    string folderName = @"D:\DBbackup\HL35\" + DateTime.Now.ToString("yyyyMMdd");
                    CommonClass.zipFolder(folderName, "SST", true);
                }
                   
                ///////////////////////////////////////////////////
                List<string> lineNotifyUsers = new List<string>() {  "曾建明", "蕭人碩", "康起禎" };
                ////Conutri 溫濕度, ftpFrequency:預設值為 3
                if (autoExecItems.Contains("conutriTemp") && (mod3++ % ftpFrequency) == 0) //三分鐘一次 
                {
                    retriveConutriData(lineNotifyUsers);
                }
                //***************************************************
                if (autoExecItems.Contains("restartFirstohmService") && DateTime.Now.Hour == 8 && DateTime.Now.Minute == 20) //每天早上 8:20 重啟 Service
                    CommonClass.RestartService("FirstohmService", 15000);
            } 
            catch(Exception ex)
            {
                CommonClass.smtpSendMail($"{ex.Message}{Environment.NewLine}{Environment.NewLine}{ex.StackTrace}  ",
                    new Dictionary<string, string>() { { "Scott Tseng", "scott.tseng@firstohm.com.tw" } },
                    true, "sstTray Timer 錯誤");
            }

        }

        private void retriveConutriData(List<string> lineNotifyList)
        {
            string rtnStr = null;
            try
            {
                string roomNo = "乾燥室 1-5";
                int logIdx = 0;
                for (int i = 0; i < 10 && logIdx == 0; i++)
                {
                    logIdx = CommonApp.retribeConutriTP(0, "192.168.2.131:65530", "admin", "18936109",
                    "/HMI/HMI-000/History/CSV", @"C:\sysTray\download", out rtnStr);
                    if (logIdx == 0)
                        CommonClass.wait(3);
                }
                int H2Done = 0;
                if (logIdx > 0)
                {
                    roomNo = "乾燥室 6-10";
                    H2Done = 0;
                    for (int i = 0; i < 20 && H2Done == 0; i++)
                    {
                        H2Done = logIdx = CommonApp.retribeConutriTP(logIdx, "192.168.2.131:65530", "admin", "18936109",
                                                "/HMI/HMI-000/History/CSV", @"C:\sysTray\download", out rtnStr);
                        if (H2Done == 0)
                            CommonClass.wait(3);
                    }
                }
                if (logIdx == 0 || H2Done == 0)
                {
                    CommonClass.writeLog("TaskTrayApplication", "retriveConutriData()", 4, roomNo + "讀取資料失敗");
                    if(conutriLastFtpStatus)
                    {
                        foreach(string lineUser in lineNotifyList )
                            linelib.pushTextMessageOuter(lineUser, $"{DateTime.Now.ToString("yyyy/MM/dd HH:mm")} 常溫乾燥室 FTP 失敗");
                        conutriLastFtpStatus = false;
                    }
                }
                else
                {
                    if (!conutriLastFtpStatus)
                    {
                        foreach (string lineUser in lineNotifyList)
                            linelib.pushTextMessageOuter(lineUser, $"{DateTime.Now.ToString("yyyy/MM/dd HH:mm")} 常溫乾燥室 溫濕度資料收集 恢復正常運作");
                        conutriLastFtpStatus = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonClass.writeLog("TaskTrayApplication", "retriveConutriData()", 5, ex.Message);
                if (conutriLastFtpStatus)
                {
                    linelib.pushTextMessageOuter("曾建明", $"{DateTime.Now.ToString("yyyy/MM/dd HH:mm")} 常溫乾燥室 FTP 失敗");
                    conutriLastFtpStatus = false;
                }
                return;
            }
        }


        //每分鐘一次
        int mod3 = 0;
        int ftpFrequency = int.Parse(Constants.getProperty("ftpFrequency", "3"));
        string currMin, currHour;
        string autoHour, autoMin;
        
        #endregion
        DateTime ifConutriTempRoomLined(string funcName = "retriveConutriData")
        {
            string sqlStr = $"SELECT max(CREATED)  FROM `mfo_servicelog` " +
                $" WHERE `FUNCTION` like '%{funcName}%' and Date(`CREATED`)= CURRENT_DATE ";
            return Convert.ToDateTime(CommonClass.getSQLScalar(sqlStr));
        }

        void ShowMessage(object sender, EventArgs e)
        {
            // Only show the message if the settings say we can.
            //if (SSTTray.Properties.Settings.Default.ShowMessage)
            //    MessageBox.Show("Hello World");
        }

        void ReleaseApk(object sender, EventArgs e)
        {
            AppRelease toRun = new AppRelease();
            toRun.ShowDialog();
        }

        void UDPSend(object sender, EventArgs e)
        {
            if (frmUdpSend == null)
            {
                frmUdpSend = new UDPSend();
                frmUdpSend.ShowDialog();
            }
            else if (frmUdpSend.Visible)
                frmUdpSend.Focus();
            else
                frmUdpSend.ShowDialog();
        }

        void frm_do_sst(object sender, EventArgs e)
        {
            if(frmDoSst==null)
                frmDoSst = new FormDoSST();
            frmDoSst.ShowDialog();
        }

        void UDPQueue(object sender, EventArgs e)
        {
            FormUDPQueue udpQueuefrm = null;
            if (udpQueuefrm == null)
            {
                udpQueuefrm = new FormUDPQueue();
                udpQueuefrm.ShowDialog();
            }
        }

        void ShowConfig(object sender, EventArgs e)
        {
            // If we are already showing the window meerly focus it.
            if (configWindow.Visible)
                configWindow.Focus();
            else
                configWindow.ShowDialog();
        }

        void ShowWebBackup(object sender, EventArgs e)
        {

            beckupWeb Footfrm;
            if (frmUdpSend == null)
            {
                Footfrm = new beckupWeb();
                Footfrm.ShowDialog();
            }
        }

        void ShowDBBackup(object sender, EventArgs e)
        {
            dbBKWindow.selectType = 0; //備份
            // If we are already showing the window meerly focus it.
            if (dbBKWindow.Visible)
            {
                dbBKWindow.Focus();
            }
            else
                dbBKWindow.ShowDialog();
        }
        
        void ShowDBUnlock(object sender, EventArgs e)
        {
            解開DB死結 Footfrm;
            if (frmUdpSend == null)
            {
                Footfrm = new 解開DB死結();
                Footfrm.ShowDialog();
            }
        }

        void SimulateIO(object sender, EventArgs e)
        {
            if (simIO.Visible)
            {
                simIO.Focus();
            }
            else
                simIO.ShowDialog();
        }

        void ShowHTMLToPdf(object sender, EventArgs e)
        {
            FormWriteToExcel mv = new FormWriteToExcel();
            mv.ShowDialog();
        }

        void TestWebAPI(object sender, EventArgs e)
        {
            // If we are already showing the window meerly focus it.
            if (webApiForm.Visible)
            {
                webApiForm.Focus();
            }
            else
                webApiForm.ShowDialog();
        }

        void TestDBConn(object sender, EventArgs e)
        {
            TestDBConnection mv = new TestDBConnection();
            mv.ShowDialog();
        }

        void ClearProcess(object sender, EventArgs e)
        {
            FormClearProcess formClearProcess = new FormClearProcess();
            formClearProcess.ShowDialog();
        }

        void SendLine(object sender, EventArgs e)
        {
            SendLineMsg fg = new SendLineMsg();
            fg.ShowDialog();
        }

        void SQLtoExcel(object sender, EventArgs e)
        {
            Form產生Excel fg = new Form產生Excel();
            fg.ShowDialog();
        }

        void FtpClient(object sender, EventArgs e)
        {
            FormFTPClient fg = new FormFTPClient();
            fg.ShowDialog();
        }

        void SqlToXML(object sender, EventArgs e)
        {
            FrnSAqlToXml fg = new FrnSAqlToXml();
            fg.ShowDialog();
        }

        void ExcelToXML(object sender, EventArgs e)
        {
            frmExcelToJson fg = new frmExcelToJson();
            fg.ShowDialog();
        }

        void ExcelToCSV(object sender, EventArgs e)
        {
            frmExcelToCSV fg = new frmExcelToCSV();
            fg.ShowDialog();
        }

        void RestartAPP(object sender, EventArgs e)
        {
            FormRespartAP fg = new FormRespartAP();
            fg.ShowDialog();
        }


        void ReloadProperty(object sender, EventArgs e)
        {
            Constants.reloadProperty();
            CommonApp.chkIP_AND_PropertyIP();
            MessageBox.Show("屬性檔重載完成", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void CloseExcel(object sender ,EventArgs e)
        {
            ExcelLib excel = new ExcelLib();
            DialogResult yesNo = MessageBox.Show("請問是否確認關閉所有EXCEL!?", "詢問訊息", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(yesNo == DialogResult.Yes)
                excel.killAllEccels(false);
        }
        void TestOpenExcel(object sender, EventArgs e)
        {
            ExcelLib excel = new ExcelLib();
            excel.openExcel(null);
        }

        void Exit(object sender, EventArgs e)
        {
            // We must manually tidy up and remove the icon before we exit.
            // Otherwise it will be left behind until the user mouses over.
            notifyIcon.Visible = false;

            System.Windows.Forms.Application.Exit();
            System.Environment.Exit(1);
        }
    }
}
