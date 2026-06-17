using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Drawing;
using TaskTrayApplication;

namespace FirstOhm
{
    static class Constants
    {
        public const String ConnString = SSTConnString;
        
        /// <summary>
        /// ////////////////////////////////
        /// </summary>
        //public const String SSTConnString = "Data Source = 172.168.1.35; Password=2u4u 2u04yj3;User ID = firstohm; Database=sst;port=3306;charset=utf8;convert zero datetime=True";
        public const String SSTConnString = "Data Source = 127.0.0.1; Password=;User ID =root; Database=sst;port=3306;charset=utf8;convert zero datetime=True";

        //SSTV2 //SSTV2 //SSTV2 //SSTV2 //SSTV2 //SSTV2 //SSTV2 //SSTV2 //SSTV2 //SSTV2 //SSTV2 //SSTV2 //SSTV2 //SSTV2 //SSTV2
        //public const String SSTConnString = "Data Source = 127.0.0.1; Password=;User ID =root; Database=sstv2;port=3306;charset=utf8;convert zero datetime=True";

        public const String LocalSSTConnString = "Data Source = 127.0.0.1; Password=;User ID =root; Database=sst;port=3306;charset=utf8;convert zero datetime=True";
        public const String LocalSSTV2ConnString = "Data Source = 127.0.0.1; Password=;User ID =root; Database=sstv2;port=3306;charset=utf8;convert zero datetime=True";

        public const String SSTConnString35 = "Data Source = 172.168.1.35; Password=2u4u 2u04yj3;User ID = firstohm; Database=sst;port=3306;charset=utf8;convert zero datetime=True";
        //public const String SSTConnString35 = SSTConnString;

        //正常 151
        public const String salesConnString = "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_Sales;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        public const String Firstohm = "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        public const String TEST_MFOFlowConnString = "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=MFO_FLOW_TEST;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        public const String ACLConnString = "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=ChkIn_Out;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        public const string SampleConnection = "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_SAMPLE;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        public const String MFOFlowConnString = "Data Source =172.168.1.151; Password=2u4u 2u04yj3;User ID =firstohm; Database=MFO_FLOW;port=3306;charset=utf8;convert zero datetime=True;default command timeout=120";
        public const String WarehouseConnString = "Data Source = 172.168.1.151; Password=2u4u 2u04yj3;User ID = firstohm;Database=WareHouse;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const String realWHConnection = "Data Source =172.168.1.151; Password = 2u4u 2u04yj3; User ID = firstohm; Database = WareHouse; port = 3306; charset = utf8; convert zero datetime = True;SslMode=None;";
        //public const String testWHConnection = "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse_test;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        public const String MFOFlowConnString172_33 = "Data Source =172.168.1.33; Password=2u4u 2u04yj3;User ID =firstohm; Database=MFO_FLOW;port=3306;charset=utf8;convert zero datetime=True";
        public const String MFOFlowConnString172_35 = "Data Source =172.168.1.35; Password=2u4u 2u04yj3;User ID =firstohm; Database=MFO_FLOW;port=3306;charset=utf8;convert zero datetime=True";
        public const string ProcurmentConnection = "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=procurement;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        public const String OfficialMFOFlowConnString = "Data Source = 172.168.1.151; Password=2u4u 2u04yj3;User ID = firstohm; Database=MFO_FLOW;port=3306;charset=utf8;convert zero datetime=True;default command timeout=120";
        public const string portalConnString = "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=portal;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        public const string portalnewConnString = "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=portal-new;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";

        //測試 172.35
        //public const String salesConnString = "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_Sales;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const String Firstohm = "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=120";
        //public const String ACLConnString = "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=ChkIn_Out;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const string SampleConnection = "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_SAMPLE;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const String MFOFlowConnString = "Data Source = 172.168.1.35; Password=2u4u 2u04yj3;User ID = firstohm; Database=MFO_FLOW;port=3306;charset=utf8;convert zero datetime=True;default command timeout=120";
        //public const String WarehouseConnString = "Data Source = 172.168.1.35; Password=2u4u 2u04yj3;User ID = firstohm;Database=WareHouse;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        ////public const String realWHConnection = "Data Source =172.168.1.35; Password = 2u4u 2u04yj3; User ID = firstohm; Database = WareHouse; port = 3306; charset = utf8; convert zero datetime = True;SslMode=None;";
        ////public const String testWHConnection = "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse_test;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const String MFOFlowConnString172_33 = "Data Source = 172.168.1.35; Password=2u4u 2u04yj3;User ID = firstohm; Database=MFO_FLOW;port=3306;charset=utf8;convert zero datetime=True";
        ////public const string ConutriTCConnString = "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=firstohm;Database=conutritc;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        ////public const string ConutriTCConnString = "Data Source=192.168.2.222;Password=2u4u 2u04yj3;User ID=firstohm;Database=conutritc;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const string ProcurmentConnection = "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=firstohm;Database=procurement;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const String TEST_MFOFlowConnString = "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=MFO_FLOW_TEST;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const String MFOFlowConnString172_35 = "Data Source =172.168.1.35; Password=2u4u 2u04yj3;User ID =firstohm; Database=MFO_FLOW;port=3306;charset=utf8;convert zero datetime=True";
        //public const String OfficialMFOFlowConnString = "Data Source = 172.168.1.151; Password=2u4u 2u04yj3;User ID = firstohm; Database=MFO_FLOW;port=3306;charset=utf8;convert zero datetime=True;default command timeout=120";
        //public const string portalConnString = "Data Source=192.168.1.29;Password=2u04yj32u4u;User ID=firstohm;Database=portal;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const string portalnewConnString = "Data Source=192.168.1.29;Password=2u04yj32u4u;User ID=firstohm;Database=portal-new;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";


        //台北
        //public const String TEST_MFOFlowConnString = "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=MFO_FLOW_TEST;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const String ACLConnString = "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=ChkIn_Out;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const string SampleConnection = "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_SAMPLE;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const String MFOFlowConnString = "Data Source = 211.23.138.231; Password=2u4u 2u04yj3;User ID = firstohm; Database=MFO_FLOW;port=3306;charset=utf8;convert zero datetime=True";
        //public const String WarehouseConnString = "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse_test;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const String realWHConnection = "Data Source = 211.23.138.231; Password = 2u4u 2u04yj3; User ID = firstohm; Database = WareHouse; port = 3306; charset = utf8; convert zero datetime = True;SslMode=None;";
        //public const String testWHConnection = "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse_test;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";

        //花蓮
        //public const String TEST_MFOFlowConnString = "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=MFO_FLOW_TEST;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const String ACLConnString = "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=ChkIn_Out;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const string SampleConnection = "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_SAMPLE;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const String MFOFlowConnString = "Data Source = 172.168.1.151; Password=2u4u 2u04yj3;User ID = firstohm; Database=MFO_FLOW;port=3306;charset=utf8;convert zero datetime=True";
        //public const String WarehouseConnString = "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse_test;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        //public const String realWHConnection = "Data Source = 172.168.1.151; Password = 2u4u 2u04yj3; User ID = firstohm; Database = WareHouse; port = 3306; charset = utf8; convert zero datetime = True;SslMode=None;";
        //public const String testWHConnection = "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse_test;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";


        public const string ConutriTCConnString = "Data Source=192.168.2.222;Password=2u4u 2u04yj3;User ID=firstohm;Database=conutritc;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
        
        public const string severeEmailList = "scott.tseng@firstohm.com.tw;peijou.chiu@firstohm.com.tw;";
        public const String propertyFile = @"C:\SSTTray\Property.txt";

        //Localhost
        //public const String ConnString = "Data Source=localhost;Password=;User ID=root;Database=MFO_FLOW;port=3306;charset=utf8";
        //排能使用, 原始程式來自 FirstohmService
        public const int pre_days = 27; //排能預留工作天數， 此為完工日
        public const int beforeDelDay = 7; //完工日到出貨日還要保留的天數
        public const int backDays = 60; //從開始可排單日，往前找幾天
        public static string ChromeDir = null; //Chrome 的執行位置 
        public static DateTime lastAkeebaBackup = new DateTime(2000,1,1); //Chrome 的執行位置 
        public static string updQueueConn = null;
        

        static public string getConnection(string connName)
        {
            string rtnStr = null;
            switch (connName)
            {
                case "ACLConnString":
                    rtnStr = ACLConnString;
                    break;
                case "SampleConnection":
                    rtnStr = SampleConnection;
                    break;
                case "MFOFlowConnString":
                    rtnStr = MFOFlowConnString;
                    break;
                case "WarehouseConnString":
                    rtnStr = WarehouseConnString;
                    break;
                case "MFOFlowConnString172_33":
                    rtnStr = MFOFlowConnString172_33;
                    break;
                case "ConutriTCConnString":
                    rtnStr = ConutriTCConnString;
                    break;
            }
            return rtnStr;
        }

        public static string getConstsByName(string constName)
        {
            switch (constName)
            {
                case "salesConnString":
                    return salesConnString;
                case "Firstohm":
                    return Firstohm;
                case "ACLConnString":
                    return salesConnString;
                case "SampleConnection":
                    return SampleConnection;
                case "MFOFlowConnString":
                    return MFOFlowConnString;
                case "WarehouseConnString":
                    return WarehouseConnString;
                case "MFOFlowConnString172_33":
                    return MFOFlowConnString172_33;
                case "ConutriTCConnString":
                    return ConutriTCConnString;
                case "ProcurmentConnection":
                    return ProcurmentConnection;
                case "TEST_MFOFlowConnString":
                    return TEST_MFOFlowConnString;
            }
            return ConnString;
        }

        #region Property 
        //使用方法
        //initial read of property File
        static public Dictionary<string, string> propertyDic = null;
        public static Dictionary<string, string> prepareProperty(string propertyPath = @"C:\SSTTray\Property.txt")
        {
            Dictionary<string, string> propertyDic = new Dictionary<string, string>();
            //createIfMissing(propertyPath);
            if (File.Exists(propertyPath))
            {
                foreach (string row in System.IO.File.ReadAllLines(propertyPath))
                {
                    try
                    {
                        if (row.Trim().Substring(0, 1) == ";")
                            continue;
                        propertyDic.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
            return propertyDic;
        }

        public static void reloadProperty(string propertyPath = @"..\Property.txt")
        {
            propertyDic.Clear();
            propertyDic = prepareProperty(CommonApp.propertyPath);
        }


        //使用方法
        static public string getProperty(string propertyName, string defaultVal=null)
        {
            string rtnValue = null;
            if (propertyDic != null && propertyDic.ContainsKey(propertyName))
                rtnValue = propertyDic[propertyName];
            else
                rtnValue = defaultVal;
            return rtnValue;
        }

        #endregion

        //Event Log
        public const string logName = "Application";
        public const string source = "SSTService";
    }
}
