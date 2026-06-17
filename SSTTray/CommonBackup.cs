using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

using FirstOhm;

using MySql.Data.MySqlClient;

namespace FirstOhm
{
    class CommonBackup
    {
        //User 選取的 ConnectionStrings
        public List<string> preViewList = new List<string>();
        public List<string> tableEngindInnoDBList = new List<string>();
        public Dictionary<string, string> dbBKConnections = null;
        public Dictionary<string, string> dbRSConnections = null;
        public string bkPath = @"D:\DBbackup";
        public string HL_BKTargetPath = @"D:\DBbackup\hualian";
        public string TP_BKTargetPath = @"D:\DBbackup\taipei";
        public int BKCnt = 5;
        public CommonBackup()
        {
            //要先產生的 View 
            preViewList = ("view_min_signID, view_mfo_order, ZZZ_ordersInNT,all_ordersinnt, " +
                "zzz_ordersinnt,XXX_ZZZ_orderWithEType_3Rate,view_shortpack,ordersinnt,view_inventory_use," +  //外銷
                "orderinint," +  //內銷
                "view_subflowinfo,view_subflowinfo_nofoot,view_subflowinfo_foot,view_basicyield_slc25_nofoot,view_basicyield_slc25_foot," +
                "view_signinfo_nofoot,view_signinfo_foot,view_signinfo,gensubflowshort4code," + //派單 一般
                "view_platting0,view_platting,view_platpackingdetail," + //派單 電鍍
                "shortresistor_info," +
                "xyz_uj_mysing_from," + //Portal 
                "view_pan3status" //sst
                ).Split(',').ToList();
            //DB ENGINE=InnoDB 的 Table, default 為 MyISAM 
            tableEngindInnoDBList = (
                "xyz_hikashop_zone_link" + //portal, portal-new 
                ",autoExecute" //chkin_out 
            ).Split(',').ToList();
        }


        //台北
        //TP211
        public Dictionary<string, string> TaipeiConnections211 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"MFO_FLOW_TEST1" , "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=MFO_FLOW_TEST1;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"MFO_FLOW" , "Data Source = 211.23.138.231; Password=2u4u 2u04yj3;User ID = firstohm; Database=MFO_FLOW;port=3306;charset=utf8;convert zero datetime=True;default command timeout=180"},
            {"MFO_FLOW_TEST" , "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=MFO_FLOW_TEST;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"WareHouse" , "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"WareHouse_test" , "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse_test;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"WareHouse_test1" , "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse_test1;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm_Sales",  "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_Sales;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm_Sales_Test",  "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_Sales_Test;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm",  "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},

            {"common" , "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=common;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"conutri" , "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=conutri;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"ConutriSale" , "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=ConutriSale;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"html" , "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=html;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"mxtest" , "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=mxtest;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"surveillance" , "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=surveillance;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"web" , "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=web;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"ChkIn_Out" , "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=ChkIn_Out;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm_auto_order" , "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=ChkIn_Out;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm_SAMPLE" , "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_SAMPLE;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"exchange", "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=root;Database=exchange;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"}
        };
        //TP33, 192.33
        public Dictionary<string, string> Taipei192_33Connections = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"MFO_FLOW" , "Data Source =192.168.1.33; Password=2u4u 2u04yj3;User ID = firstohm; Database=MFO_FLOW;port=3306;charset=utf8;convert zero datetime=True;default command timeout=180"},
            {"procurement" , "Data Source =192.168.1.33; Password=2u4u 2u04yj3;User ID = firstohm; Database=procurement;port=3306;charset=utf8;convert zero datetime=True;default command timeout=180"},
            {"MFO_FLOW_TEST1" , "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=MFO_FLOW_TEST1;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},

            {"MFO_FLOW_TEST" , "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=MFO_FLOW_TEST;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"WareHouse" , "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"WareHouse_test" , "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse_test;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"WareHouse_test1" , "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse_test1;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm_Sales",  "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_Sales;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm_Sales_Test",  "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_Sales_Test;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm",        "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm_auto_order" , "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=ChkIn_Out;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"common" , "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=common;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"conutri" , "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=conutri;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"ConutriSale" , "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=ConutriSale;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"html" , "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=html;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"mxtest" , "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=mxtest;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;"},
            {"surveillance" , "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=surveillance;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"web" , "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=web;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"ChkIn_Out" , "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=ChkIn_Out;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm_SAMPLE" , "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_SAMPLE;port=3306;charset=utf8;convert zero datetime=True;SslMode=Non;default command timeout=180"},
            {"exchange", "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=root;Database=exchange;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"statistics" , "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=statistics;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"portal", "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=portal;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"portal-new", "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=portal-new;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"SST", "Data Source = 192.168.1.33; Password=2u4u 2u04yj3;User ID = firstohm; Database=sst;port=3306;charset=utf8;convert zero datetime=True;default command timeout=180" },
            {"SSTV2", "Data Source = 192.168.1.33; Password=2u4u 2u04yj3;User ID = firstohm; Database=sstv2;port=3306;charset=utf8;convert zero datetime=True;default command timeout=180" }
        };

        //花蓮 172.168.1.151
        //HL151
        public Dictionary<string, string> HualianConnections = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
           {"MFO_FLOW_TEST1" , "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=MFO_FLOW_TEST1;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"MFO_FLOW" , "Data Source = 172.168.1.151; Password=2u4u 2u04yj3;User ID = firstohm; Database=MFO_FLOW;port=3306;charset=utf8;convert zero datetime=True;default command timeout=180"},
            {"MFO_FLOW_TEST" , "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=MFO_FLOW_TEST;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"WareHouse" , "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"WareHouse_test" , "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse_test;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"WareHouse_test1" , "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse_test1;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm_Sales",  "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_Sales;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm",  "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm_auto_order" , "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=ChkIn_Out;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"common" , "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=common;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"conutri" , "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=conutri;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"ConutriSale" , "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=ConutriSale;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"html" , "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=html;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"mxtest" , "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=mxtest;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},

            {"surveillance" , "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=surveillance;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"web" , "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=web;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"ChkIn_Out" , "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=ChkIn_Out;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm_SAMPLE" , "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_SAMPLE;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"exchange", "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=root;Database=exchange;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"portal", "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=portal;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"procurement", "Data Source=172.168.1.151;Password=2u4u 2u04yj3;User ID=firstohm;Database=procurement;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"}
        };

        //花蓮 172.168.1.33
        //HL33
        public Dictionary<string, string> Hualian172_33Connections = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            //{"MFO_FLOW" , "Data Source = 172.168.1.33; Password=2u4u 2u04yj3;User ID = firstohm; Database=MFO_FLOW;port=3306;charset=utf8;convert zero datetime=True;default command timeout=180"},
            //{"WareHouse" , "Data Source=172.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            //{"WareHouse_test1" , "Data Source=172.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse_test1;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            //{"Firstohm_Sales",  "Data Source=172.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_Sales;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            //{"Firstohm",  "Data Source=172.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            //{"ChkIn_Out" , "Data Source=172.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=ChkIn_Out;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            //{"Firstohm_SAMPLE" , "Data Source=172.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_SAMPLE;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            //{"Firstohm_auto_order" , "Data Source=172.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=ChkIn_Out;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            //{"exchange", "Data Source=172.168.1.33;Password=2u4u 2u04yj3;User ID=root;Database=exchange;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            //{"procurement", "Data Source=172.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=procurement;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"SST", "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=firstohm;Database=sst;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"SSTV2", "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=firstohm;Database=sstv2;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"}

        };

        //花蓮 172.168.1.35
        //HL35
        public Dictionary<string, string> Hualian172_35Connections = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"MFO_FLOW" , "Data Source = 172.168.1.35; Password=2u4u 2u04yj3;User ID = firstohm; Database=MFO_FLOW;port=3306;charset=utf8;convert zero datetime=True;default command timeout=180"},
            {"WareHouse" , "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"WareHouse_test1" , "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=firstohm;Database=WareHouse_test1;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm_Sales",  "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_Sales;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm",  "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"ChkIn_Out" , "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=firstohm;Database=ChkIn_Out;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm_SAMPLE" , "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm_SAMPLE;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm_auto_order" , "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=firstohm;Database=ChkIn_Out;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"exchange", "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=root;Database=exchange;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"procurement", "Data Source=172.168.1.35;Password=2u4u 2u04yj3;User ID=firstohm;Database=procurement;port=3306;charset=utf8;convert zero datetime=True;SslMode=Non;default command timeout=180"},
            {"SST", "Data Source = 172.168.1.35; Password=2u4u 2u04yj3;User ID = firstohm; Database=sst;port=3306;charset=utf8;convert zero datetime=True;default command timeout=180" },
            {"SSTV2", "Data Source = 172.168.1.35; Password=2u4u 2u04yj3;User ID = firstohm; Database=sstv2;port=3306;charset=utf8;convert zero datetime=True;default command timeout=180" }
        };

        //172.0.0.1
        //OWN
        public Dictionary<string, string> OWN127_0Connections = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"Firstohm_Sales",  "Data Source = 127.0.0.1; Password=;User ID=root;Database=Firstohm_Sales;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"Firstohm",  "Data Source = 127.0.0.1; Password=;User ID=root;Database=Firstohm;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;default command timeout=180"},
            {"MFO_FLOW" , "Data Source = 127.0.0.1; Password=;User ID=root;Database=MFO_FLOW;port=3306;charset=utf8;convert zero datetime=True;default command timeout=180"},
            {"SST", "Data Source = 127.0.0.1; Password=;User ID=root; Database=sst;port=3306;charset=utf8;convert zero datetime=True;default command timeout=180"},
            {"SSTV2", "Data Source = 127.0.0.1; Password=;User ID=root; Database=sstv2;port=3306;charset=utf8;convert zero datetime=True;default command timeout=180"}
        };

        //Portal Server
        //public Dictionary<string, string> PortalConnections = new Dictionary<string, string>()
        //{
        //    {"portal", "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=portal;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;" },
        //    {"portal-new", "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=portal-new;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;" }
        //};

        //指定每個資料庫的主要 SErver
        public Dictionary<string, string> MainDBServer = null;

        //因為設 Foriegn Key 的關係， 有的 DB Trancate Table 時會 出問題， 因此用 以下 List 決定 Script 裡面 Table 的順序
        //在本 List 裡面的 Table 會用 Desc, default 則為 Asc
        List<String> backupOrderDesc = new List<String>() { "MFO_FLOW_TEST", "MFO_FLOW_TEST1", "MFO_FLOW_TEST" };
        // TaipeiConnections, HualianConnections, Taipei192_33Connections,
        // Hualian172_33Connections, PortalConnections
        private Dictionary<string, string> getConnDictionary(string svrName)
        {
            Dictionary<string, string> rtnConnections;
            switch (svrName)
            {
                case "花蓮 172.1681.1.151":
                case "花蓮":
                case "HL151":
                    return HualianConnections;
                    break;

                case "花蓮 172.1681.1.33":
                case "HL33":
                case "花蓮33":
                    return Hualian172_33Connections;
                    break;
                case "花蓮 172.1681.1.35":
                case "HL35":
                case "花蓮35":
                    return Hualian172_35Connections;
                    break;
                case "台北 192.1681.1.33":
                case "TP33":
                case "台北33":
                    return Taipei192_33Connections;
                    break;
                case "台北 211.23.138.231":
                case "台北":
                case "TP211":
                    return TaipeiConnections211;
                    break;
                //case "portal":
                //    return PortalConnections;
                //    break;
                case "OWN":
                    return OWN127_0Connections;
                    break;
                //case "portal":
                //    return PortalConnections;
                //    break;
                default:
                    return null;
            }
        }

        //mainServer: TP or HL
        //如果 mainServer 為 null, 則 MainDBServer 內的所有 DB 都會備份
        // bkTime 1:每天晚上  2:中午   3:週末
        //回傳的 Connction 的 Dictionary
        //其中 Key : serverLocation|DBName, 例如：HL|MFO_FLOW
        public Dictionary<string, string> getConnectionsByServer(string mainServer, int bkTime = 1)
        {
            Dictionary<string, string> sourceDict = null;
            string sqlStr = $"SELECT `DBName`,`DBLocation` FROM `DB_BACKUP` where DBLocation='{mainServer}' ";
            switch (bkTime)
            {
                case 2:
                    sqlStr += " and MOD(`status` DIV 10,10)=1";
                    break;
                case 3:
                    sqlStr += " and MOD(`status` DIV 100,10)=1";
                    break;
                case 1:
                default:
                    sqlStr += " and MOD(`status`,10)=1";
                    break;
            }

            //if (bkTime >=2 )
            //{
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.ACLConnString);
            sourceDict = CommonClass.dtToDictionary(dt, "DBName", "DBLocation");
            //}
            //else
            //    sourceDict = MainDBServer;
            Dictionary<string, string> rtnDict = new Dictionary<string, string>();
            string tempConn = null;
            foreach (KeyValuePair<string, string> entry in sourceDict)
            {
                tempConn = null;
                try
                {
                    switch (entry.Value)
                    {
                        case "TP":
                        case "TP211":
                            tempConn = TaipeiConnections211[entry.Key];
                            break;
                        case "HL":
                        case "HL151":
                            tempConn = HualianConnections[entry.Key];
                            break;
                        case "TP33":
                            tempConn = Taipei192_33Connections[entry.Key];
                            break;
                        //case "portal":
                        //    tempConn = PortalConnections[entry.Key];
                        //    break;
                        case "HL33":
                            tempConn = Hualian172_33Connections[entry.Key];
                            break;
                        case "HL35":
                            tempConn = Hualian172_35Connections[entry.Key];
                            break;
                        case "OWN":
                            tempConn = OWN127_0Connections[entry.Key];
                            break;
                    }
                    if (!string.IsNullOrEmpty(tempConn))
                        rtnDict.Add(entry.Value + "|" + entry.Key, tempConn);
                }
                catch (Exception ex)
                {
                    //return rtnDict;
                }
            }
            return rtnDict;
        }

        //mainServer : "TP211","HL151","TP33","HL35" 
        public string getConnectionsByServer(string mainServer, string dbName, int bkTime = 1)
        {
            string tempConn = null;
            switch (mainServer)
            {
                case "TP":
                case "TP211":
                    tempConn = TaipeiConnections211[dbName];
                    break;
                case "HL":
                case "HL151":
                    tempConn = HualianConnections[dbName];
                    break;
                case "TP33":
                    tempConn = Taipei192_33Connections[dbName];
                    break;
                //case "portal":
                //    tempConn = PortalConnections[entry.Key];
                //    break;
                case "HL33":
                    tempConn = Hualian172_33Connections[dbName];
                    break;
                case "HL35":
                    tempConn = Hualian172_35Connections[dbName];
                    break;
                case "OWN":
                    tempConn = OWN127_0Connections[dbName];
                    break;
            }
            return tempConn;
        }

        #region Restore DB
        public bool restoreDB(string destSvrName, string dbName, string restoreFileDest, out string result)
        {
            string ConnString = null;
            bool resultBool = false;
            string resultMessage = null;
            if (!File.Exists(restoreFileDest))
            {
                result = "所選的檔案 " + restoreFileDest + " 不存在";
                return false;
            }

            if (Path.GetExtension(restoreFileDest).ToLower() != ".sql")
            {
                result = "必須選取 .sql 檔案才能還原資料庫";
                return false; ;
            }

            dbRSConnections = getConnDictionary(destSvrName);
            if (dbRSConnections == null)
            {
                result = "伺服器錯誤：" + destSvrName + "， 請指定正確的伺服器！！";
                return false;
            }

            if (dbRSConnections.Keys.Contains(dbName))
                ConnString = dbRSConnections[dbName];
            else
            {
                result = "目的地無此資料庫: " + destSvrName + @"\" + dbName + " , 還原失敗";
                return false;
            }
            if (CommonClass.checkDB_Conn(ConnString, out result))
            {
                resultBool = CommonClass.execSQLFile(restoreFileDest, ConnString, ref resultMessage);
                if (resultBool)
                    result = "還原資料庫 " + destSvrName + @"\" + dbName + " 完成";
                else
                {
                    CommonClass.writeLog("FirstohmService", "restoreDB ", 5, $"目的 Server{destSvrName} , DB 名稱 {dbName} 還原失敗");
                    result = resultMessage;
                }
                return resultBool;
            }
            else
            {
                result = destSvrName + @"\" + dbName + " Connection String : " + ConnString + " 錯誤, 還原失敗";
                return false;
            }
        }

        private string resportType(int restoreType)
        {
            switch (restoreType)
            {
                case 0:
                    return "DBSschma";
                case 1:
                    return "DBData";
                case 2:
                    return "DBAll";
                default:
                    return "DBData";
            }
        }

        public string getBKPath(string locationName)
        {
            switch (locationName)
            {
                case "台北":
                case "TP":
                case "Portal":

                    bkPath = TP_BKTargetPath;
                    break;
                case "花蓮":
                case "HL":
                    bkPath = HL_BKTargetPath;
                    break;
            }
            CommonClass.createIfMissing(bkPath);
            return bkPath;
        }

        //如果不選， 則由系統選定， 花蓮->台北， 台北->花蓮
        //MessageBox.Show("請選取 目的 資料庫", "目的資料庫", MessageBoxButtons.OK);
        public string flipServer(string locationName)
        {
            string rtnTargetSvr = null;
            switch (locationName)
            {
                case "台北":
                case "TP":
                case "Portal":
                    rtnTargetSvr = "HL";
                    break;
                case "花蓮":
                case "HL":
                    rtnTargetSvr = "TP";
                    break;
            }
            return rtnTargetSvr;
        }

        //如果 dbName == null, 則 還原來源Sever 所屬的所有 DB
        //如果  destSvr=="" => 則由系統選定複製的目的Server， 花蓮->台北， 台北->花蓮
        //來源 script.sql 則由來源 Database 決定
        // restortType  0:Schema only, 1:Data Only, 2:Schema + Data
        //bkTime 1:每天凌晨, 2:每天中午, 3:星期日
        public bool crossRestoreDB(string sourceSvrName, string bkPath, int restortType,
            out string result, string dbName = null, int bkTime = 1)
        {
            string sourceFilePath;
            string destSvrName;
            string specialTime;
            if (bkTime == 2)
                specialTime = @"\Noon";
            else if (bkTime == 3)
                specialTime = @"\Sunday";
            result = "";
            bool rtnBool = true;
            string rtnMessage = null;
            string restoreDBName = "";
            string sqlStr, scriptPath, tempStr;
            if (!string.IsNullOrEmpty(dbName))
                sqlStr = "SELECT  `DBName`, `DBLocation` , `crossLocation`, IfCrossRestore FROM `DB_BACKUP` where mod(`Status`,10)=1 and DBName='" + dbName + "'";
            else if (string.IsNullOrEmpty(sourceSvrName))
                sqlStr = "SELECT  `DBName`, `DBLocation` , `crossLocation`, IfCrossRestore FROM `DB_BACKUP` where mod(`Status`,10)=1";
            else
                sqlStr = "SELECT  `DBName`, `DBLocation` , `crossLocation`, IfCrossRestore FROM `DB_BACKUP` where mod(`Status`,10)=1 and DBLocation='" + sourceSvrName + "'";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.ACLConnString);
            if (dt.Rows.Count == 0)
            {
                return false;
            }
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    restoreDBName = dr["DBName"].ToString();
                    //則由系統選定交叉備份， 花蓮->台北， 台北->花蓮
                    //scriptPath = sourceFilePath + @"\" + DateTime.Now.ToString("yyyymmdd") + specialTime + @"\" + resportType(restortType);

                    backupDB(restoreDBName, bkPath, dr["DBLocation"].ToString(), out tempStr, 0); //backup schema
                    backupDB(restoreDBName, bkPath, dr["DBLocation"].ToString(), out tempStr, 1); ////backup Data
                    if (dr["IfCrossRestore"].ToString() == "1")
                    {
                        destSvrName = dr["crossLocation"].ToString();
                        //先備份再 交叉復原 
                        scriptPath = makePath(bkPath, dr["DBLocation"].ToString()) + @"\" + resportType(0); //先還原 Schema
                        rtnBool = restoreDB(destSvrName, restoreDBName, scriptPath + @"\" + restoreDBName + @".sql", out rtnMessage);
                        scriptPath = makePath(bkPath, dr["DBLocation"].ToString()) + @"\" + resportType(1); //再還原 Data
                        rtnBool = restoreDB(destSvrName, restoreDBName, scriptPath + @"\" + restoreDBName + @".sql", out rtnMessage);
                    }
                    result += rtnMessage + Environment.NewLine;
                }
                catch (Exception ex)
                {
                    result += "還原資料庫 " + restoreDBName + "失敗, " + ex.Message + Environment.NewLine;
                    rtnBool = false;
                    CommonClass.writeLog("FirstohmService\\crossRestoreDB", "crossRestoreDB()", 5, ex.Message);
                }
            }
            return rtnBool;
        }

        #endregion


        #region 備份資料庫
        //備份 http://211.23.138.231/ChkIn_Out/DB_BACKUP 中的所有資料庫
        //依據 Location 決定要備份哪些資料庫
        //mainServer： Null： UI 決定\TP 台北\HL 花蓮 
        public bool backupAllDBs(string backupPath, out string rtnRsult, string host, int backupType = 2, string mainServer = null, int bkTime = 1)
        {
            string bkResult;

            Dictionary<string, string> BKConnections;

            if (string.IsNullOrEmpty(mainServer))
                BKConnections = dbBKConnections;
            else
                BKConnections = getConnectionsByServer(mainServer, bkTime);

            List<string> rtnFail = new List<string>();
            int successCnt = 0;
            string dbName;
            foreach (KeyValuePair<string, string> entry in BKConnections)
            {
                dbName = entry.Key.Split('|')[1];
                if (backupDB(dbName, backupPath, host, out bkResult, backupType, entry.Value))
                    successCnt++;
                else
                    rtnFail.Add(bkResult);
            }
            if (rtnFail.Count > 0)
            {
                rtnRsult = string.Join(" ; ", rtnFail);
                return false;
            }
            else
            {
                rtnRsult = "備份 " + successCnt + " 資料庫完成!!";
                return true;
            }
        }

        //// bkTime 1:每天晚上  2:中午   3:週末
        //backupType 0：SchemaOnly, 1:Data Only, 2:Schema + Data
        //mainServer : "TP211","HL151","TP33","HL35" 
        public bool backupAreaDBs(out string rtnRsult, int backupType = 2, string mainServer = null, int bkTime = 1)
        {
            string backupPath;
            string bkResult;
            string specialPath = null;
            Dictionary<string, string> BKConnections = getConnectionsByServer(mainServer, bkTime);
            switch (bkTime)
            {
                case 2:
                    specialPath += @"\" + "NOON";
                    //CommonClass.createIfMissing(specialPath);
                    break;
                case 3:
                    specialPath += @"\" + "SUNDAY";
                    //CommonClass.createIfMissing(specialPath);
                    break;
                case 1:
                default:
                    specialPath = "";
                    break;
            }
            List<string> rtnFail = new List<string>();
            int successCnt = 0;
            string location, dbName;
            foreach (KeyValuePair<string, string> entry in BKConnections)
            {
                location = entry.Key.Split('|')[0];
                dbName = entry.Key.Split('|')[1];

                backupPath = bkPath + @"\" + location + @"\" + DateTime.Now.ToString("yyyyMMdd") + specialPath;
                if (backupType == 2)
                {
                    //backup Schema
                    if (backupDB(dbName, bkPath, mainServer, out bkResult, 0, entry.Value, bkTime))
                    {
                        if (backupDB(dbName, bkPath, mainServer, out bkResult, 1, entry.Value, bkTime))
                            successCnt++;
                        else
                            rtnFail.Add(bkResult);
                    }
                    else
                        rtnFail.Add(bkResult);
                }
                else
                {
                    if (backupDB(dbName, bkPath, mainServer, out bkResult, backupType, entry.Value, bkTime))
                        successCnt++;
                    else
                        rtnFail.Add(bkResult);
                }
            }
            if (rtnFail.Count > 0)
            {
                rtnRsult = string.Join(" ; ", rtnFail);
                return false;
            }
            else
            {
                rtnRsult = "備份 " + mainServer + " 區域資料庫,  " + successCnt + " 個完成!!";
                return true;
            }
        }

        // dataType ： 0:Schema only, 1:Data Only, 2:Schema + Data
        public bool multiBackupDB(string backupPath, string host, out string bkResult, int dataType = 2, String ConnString = null, int bkTime = 1)
        {
            bool rtnBool = true;
            string sqlStr = null;
            if (!string.IsNullOrEmpty(host))
                sqlStr = "SELECT  `DBName`, `DBLocation` , `crossLocation` FROM `DB_BACKUP` " +
                    $" where mod(`Status`,10)=1 and DBLocation='{host}' ";
            else
            {
                bkResult = "要備份的 伺服器 與 資料庫";
                return false;
            }
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.ACLConnString);
            StringBuilder rtnStr = new StringBuilder("");
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    // dataType ： 0:Schema only, 1:Data Only, 2:Schema + Data
                    if (dataType == 2)
                    {
                        if (backupDB(dr["DBName"].ToString(), backupPath, dr["DBLocation"].ToString(), out bkResult, 0))
                            rtnStr.Append(bkResult + Environment.NewLine);
                        else
                            rtnStr.Append(dr["DBLocation"].ToString() + " : " + dr["DBName"] + " 備份失敗， " + rtnStr + Environment.NewLine);
                        if (backupDB(dr["DBName"].ToString(), backupPath, dr["DBLocation"].ToString(), out bkResult, 1))
                            rtnStr.Append(bkResult + Environment.NewLine);
                        else
                            rtnStr.Append(dr["DBLocation"].ToString() + " : " + dr["DBName"] + " 備份失敗， " + rtnStr + Environment.NewLine);
                    }
                    else
                    {
                        if (backupDB(dr["DBName"].ToString(), backupPath, dr["DBLocation"].ToString(), out bkResult, dataType))
                            rtnStr.Append(bkResult + Environment.NewLine);
                        else
                            rtnStr.Append(dr["DBLocation"].ToString() + " : " + dr["DBName"] + " 備份失敗， " + rtnStr + Environment.NewLine);
                    }
                }
                catch (Exception ex)
                {
                    rtnStr.Append(dr["DBLocation"].ToString() + " : " + dr["DBName"] + " 備份失敗" + ex.Message + Environment.NewLine);
                    rtnBool = false;
                }
            }
            bkResult = rtnStr.ToString();
            return rtnBool;
        }


        //備份單一資料庫
        //backupType: 0: Schema, 1: Data only
        public bool backupDB(string dbName, string folderSource, string host, out string result, int backupType = 2, String ConnString = null, int bkTime = 1, Dictionary<string, List<string>> bkJson = null)
        {
            //backupType: 0: Schema, 1: Data only, 2: schema + Data
            string backupTypeStr = @"\DBSschma";
            //bkTime: 1: 半夜, 1: Noon, 2: Sunday
            string bkTimeStr = "";
            int tableOrder = 0; ////tableOrder : 0 asc, 1:desc
            if (backupOrderDesc.Contains(dbName))
                tableOrder = 1;
            folderSource = makePath(folderSource, host);
            ConnString = getConnectionsByServer(host, dbName, bkTime);
            string fileName = null;
            try
            {
                if (string.IsNullOrEmpty(ConnString))
                {
                    result = "查無所選資料庫！！";
                    return false;
                }

                switch (backupType)
                {
                    case 0: //all schema
                        CommonClass.createIfMissing(folderSource + @"\DBSschma");
                        backupTypeStr = @"\DBSschma\"; //3+4
                        break;
                    case 1: //Data only 
                        CommonClass.createIfMissing(folderSource + @"\DBData");
                        backupTypeStr = @"\DBData\";
                        break;
                    case 3: // View + store procedure
                        CommonClass.createIfMissing(folderSource + @"\DBSschma");
                        backupTypeStr = @"\DBSschma\View";
                        break;
                    case 4: //Table + function
                        CommonClass.createIfMissing(folderSource + @"\DBSschma");
                        backupTypeStr = @"\DBSschma\TableFunc";
                        break;
                }

                switch (bkTime)
                {
                    case 2:
                        backupTypeStr += @"Noon\";
                        break;
                    case 3:
                        backupTypeStr += @"Sunday\";
                        break;
                }
                CommonClass.createIfMissing(folderSource);
                CommonClass.createIfMissing(folderSource + backupTypeStr);
                //fileName = Path.Combine(folderSource + backupTypeStr, dbName + ".sql");
                fileName = folderSource + backupTypeStr + dbName + ".sql";
                //建立空SQL檔案，供後續輸出
                if (File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }
                using (FileStream fs = File.Create(fileName))
                {
                    fs.WriteByte(32);
                }
                write_headings(host, dbName, fileName);


                if (backupType == 2)  //All together
                {
                    //backupType: 0: Schema, 1: Data only, 2: schema + Data
                    write_data(dbName, ConnString, fileName, 0, tableOrder, null, bkJson);
                    write_data(dbName, ConnString, fileName, 1, tableOrder, null, bkJson);
                    gen_Function(dbName, ConnString, fileName, bkJson);
                    gen_view(dbName, ConnString, fileName, bkJson);
                    gen_Procedure(dbName, ConnString, fileName, bkJson);
                }
                else if (backupType == 0)
                {
                    //all Schema
                    write_data(dbName, ConnString, fileName, 0, tableOrder, null, bkJson); //Create Table
                    gen_Function(dbName, ConnString, fileName, bkJson);
                    gen_view(dbName, ConnString, fileName, bkJson);
                    gen_Procedure(dbName, ConnString, fileName, bkJson);
                }
                else if (backupType == 1)
                {
                    write_data(dbName, ConnString, fileName, backupType, tableOrder, null, bkJson);
                }
                else if (backupType == 3)
                {
                    gen_view(dbName, ConnString, fileName, bkJson);
                    gen_Procedure(dbName, ConnString, fileName, bkJson);
                }
                else if (backupType == 4)
                {
                    //backupType 只output create table
                    write_data(dbName, ConnString, fileName, 0, tableOrder, null, bkJson);
                    gen_Function(dbName, ConnString, fileName, bkJson);
                    gen_Procedure(dbName, ConnString, fileName, bkJson);
                    gen_view(dbName, ConnString, fileName, bkJson);
                }


                result = "備份資料庫 " + dbName + " 完成， 檔案位置：" + fileName;
                return true;
            }
            catch (Exception ex)
            {
                result = "備份失敗:" + dbName;
                return false;
            }
        }

        public bool backupTable(string dbName, string tableName, string folderSource, string host, out string result, int backupType = 2, String ConnString = null, int bkTime = 1)
        {
            //backupType: 0: Schema, 1: Data only, 2: schema + Data
            string backupTypeStr = @"\DBSschma";
            //bkTime: 1: 半夜, 1: Noon, 2: Sunday
            string bkTimeStr = "";
            int tableOrder = 0; ////tableOrder : 0 asc, 1:desc
            if (backupOrderDesc.Contains(dbName))
                tableOrder = 1;
            folderSource = makePath(folderSource, host);
            ConnString = getConnectionsByServer(host, dbName, bkTime);
            string fileName = null;
            try
            {
                if (string.IsNullOrEmpty(ConnString))
                {
                    result = "查無所選資料庫！！";
                    return false;
                }

                switch (backupType)
                {
                    case 0: //all schema
                        CommonClass.createIfMissing(folderSource + @"\DBSschma");
                        backupTypeStr = @"\DBSschma\"; //3+4
                        break;
                    case 1: //Data only 
                        CommonClass.createIfMissing(folderSource + @"\DBData");
                        backupTypeStr = @"\DBData\";
                        break;
                    case 3: // View + store procedure
                        CommonClass.createIfMissing(folderSource + @"\DBSschma");
                        backupTypeStr = @"\DBSschma\View";
                        break;
                    case 4: //Table + function
                        CommonClass.createIfMissing(folderSource + @"\DBSschma");
                        backupTypeStr = @"\DBSschma\TableFunc";
                        break;
                }

                switch (bkTime)
                {
                    case 2:
                        backupTypeStr += @"Noon\";
                        break;
                    case 3:
                        backupTypeStr += @"Sunday\";
                        break;
                }
                CommonClass.createIfMissing(folderSource);
                CommonClass.createIfMissing(folderSource + backupTypeStr);
                //fileName = Path.Combine(folderSource + backupTypeStr, dbName + ".sql");
                fileName = $"{folderSource}{backupTypeStr}{dbName}_{tableName}.sql";
                //建立空SQL檔案，供後續輸出
                if (File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }
                using (FileStream fs = File.Create(fileName))
                {
                    fs.WriteByte(32);
                }
                write_headings(host, dbName, fileName, tableName);


                if (backupType == 2)  //All together
                {
                    //backupType: 0: Schema, 1: Data only, 2: schema + Data
                    write_data(dbName, ConnString, fileName, 0, tableOrder, tableName);
                    write_data(dbName, ConnString, fileName, 1, tableOrder, tableName);
                    gen_Function(dbName, ConnString, fileName);
                    gen_view(dbName, ConnString, fileName);
                    gen_Procedure(dbName, ConnString, fileName);
                }
                else if (backupType == 0)
                {
                    //all Schema
                    write_data(dbName, ConnString, fileName, 0, tableOrder, tableName); //Create Table
                    if (tableName == null)
                    {
                        gen_Function(dbName, ConnString, fileName);
                        gen_view(dbName, ConnString, fileName);
                        gen_Procedure(dbName, ConnString, fileName);
                    }
                }
                else if (backupType == 1)
                {
                    write_data(dbName, ConnString, fileName, backupType, tableOrder, tableName);
                }
                else if (backupType == 3)
                {
                    gen_view(dbName, ConnString, fileName);
                    gen_Procedure(dbName, ConnString, fileName);
                }
                else if (backupType == 4)
                {
                    //backupType 只output create table
                    write_data(dbName, ConnString, fileName, 0, tableOrder, tableName);
                    gen_Function(dbName, ConnString, fileName);
                }


                result = "備份資料庫 " + dbName + (tableName == null ? "" : $" / {tableName} ") + " 完成， 檔案位置：" + fileName;
                return true;
            }
            catch (Exception ex)
            {
                result = "備份失敗:" + dbName;
                return false;
            }
        }

        //建立備份資料夾並將超過天數備份資料刪除
        //serverLocation 例如 TP
        public string makePath(string filePath, string serverLocation)
        {

            if (!string.IsNullOrEmpty(serverLocation))
                filePath = Path.Combine(filePath, serverLocation);
            filePath = Path.Combine(filePath, DateTime.Now.ToString("yyyyMMdd"));
            //建立設定路徑
            Directory.CreateDirectory(filePath);
            statusText("建立備份目錄 ：" + filePath);
            return filePath;
        }

        public int delBackupFile(string backupPath, DateTime startDate, String dbLocation = "ALL")
        {
            int undelCnt = 0;
            List<string> tobeDelete = null;
            switch (dbLocation)
            {
                case "ALL":
                    tobeDelete = new List<string>() { "portal", "HL33", "HL35", "HL151", "OWN", "TP33", "TP211" };
                    break;
                case "花蓮":
                    tobeDelete = new List<string>() { "HL33", "HL35", "HL151" };
                    break;
                case "台北":
                    tobeDelete = new List<string>() { "TP33", "TP211" };
                    break;
                case "portal":
                    tobeDelete = new List<string>() { "OWN", "portal" };
                    break;
            }
            DateTime dt = startDate;
            string strChkDate = null;
            DateTime chkDate;
            foreach (string locationPath in tobeDelete)
            {
                string[] folders = Directory.GetFileSystemEntries(backupPath + @"\" + locationPath);
                foreach (string folder in folders)
                {
                    FileInfo fo = new FileInfo(folder);
                    strChkDate = CommonClass.adjustDateFormat("yyyyMMdd", fo.Name);
                    try
                    {
                        chkDate = Convert.ToDateTime(strChkDate);
                        if (chkDate >= DateTime.Now.AddDays(-7) || chkDate.Day == 1) //七日內， 或每個月的第一天保留
                        {
                            //do nothing
                        }
                        else
                        {
                            CommonClass.deleteFoldersAndFiles(folder, true, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        undelCnt++;
                    }
                }
            }
            return undelCnt;
        }

        //將備份多的份數+1份刪除
        public void removeOlder(string backupPath)
        {
            //try
            //{
            //    DateTime dt = DateTime.Now;
            //    int delCnt = 0;
            //    string[] folders = Directory.GetFileSystemEntries(backupPath);
            //    foreach (string folder in folders)
            //    {
            //        FileInfo fo = new FileInfo(folder);
            //        if (fo.LastAccessTime < dt.AddDays(-7)
            //            || (fo.LastAccessTime.DayOfWeek.ToString() == "Saturday" && fo.LastAccessTime >= DateTime.Now.AddDays(-35))
            //            || (fo.LastAccessTime.Day == 28 && fo.LastAccessTime >= dt.AddDays(-180)))
            //        {
            //            //do nothing
            //        }
            //        else
            //        {
            //            try
            //            {
            //                CommonClass.deleteFoldersAndFiles(folder, true, false);
            //                delCnt++;
            //            }
            //            catch (Exception ex) { }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //如果沒刪除就算了
            //}
        }

        //寫入檔案標頭
        //tableName=null 備份DB全部 Tables
        public void write_headings(string host, string database, string file, string tableName = null)
        {
            string[] lines = { "-- phpMyAdmin SQL Dump", "-- version 3.3.7", "-- http://www.phpmyadmin.net"
                    , "--", "-- 主機: " + host, "-- 建立日期: " + DateTime.Now.ToString("MMMM dd, yyyy, HH:mm tt")
                    , "-- 伺服器版本: 5.5.56", "-- PHP 版本: 5.6.27", "", "SET SQL_MODE=\"NO_AUTO_VALUE_ON_ZERO\";"
                    , "set global default_storage_engine=\"InnoDB\";", "", ""
                    , "/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;"
                    , "/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;"
                    , "/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;"
                    , "/*!40101 SET NAMES utf8 */;", "", "--", "-- 資料庫: `" + database + "`", "--",  (tableName==null?"":tableName) + ""
                    , "-- --------------------------------------------------------", ""};
            File.WriteAllLines(@file, lines);
        }

        //寫入資料表
        //backupType  0:schema only, 1:data only 2:schema+data
        public string write_table(string tname, string database, string ConnString, string file, int backupType = 2)
        {
            //string SQL_Table_Cols = "DESCRIBE " + tname;//查詢資料表設定
            string SQL_Table_Cols = $"SHOW FULL COLUMNS FROM {tname}";//查詢資料表設定
            string SQL_Table_Data = "SELECT ";//查詢資料表全部資料內容
            string SQL_Table_Index = $"SHOW INDEX FROM {tname}";//查詢資料表索引及全文索引設定
            int i = 0, j = 0;//決定是否加逗號用
            string incre = "";//記錄累加欄位目前最大值
            string[] type_contains = new string[] { "String", "MySql.Data.Types.MySqlDateTime" };//寫入時要加單引號的欄位
            int MaxInsert = 300;//一筆Insert最大列數
                                //StringBuilder writeStr = new StringBuilder("");
            DataTable dt = CommonClass.getSQLDataTable(SQL_Table_Cols, ConnString);
            List<string> schemaDetail = new List<string>(); //Table schema 的每一行
            List<string> writeStr = new List<string>(); //schema 每一行裡面的 detail
            DataTable dt_index;
            DataTable tempDt1, tempDt2;
            bool ifIndex = false; //判斷 table 是否有使用 Primany/Index 
            string tempStr = null;
            try
            {
                //Write Schema 
                schemaDetail.Add("--");
                schemaDetail.Add("-- 資料表格式： `" + tname + "`");
                schemaDetail.Add("--");
                schemaDetail.Add("");
                //需不需要 Drop/Create Table
                if (backupType == 0) //write 2:schema +Data, 4:table+function 
                {
                    #region 寫入資料表規格 -- Table Schema
                    schemaDetail.Add("DROP TABLE IF EXISTS `" + tname + "`;");
                    schemaDetail.Add("CREATE TABLE IF NOT EXISTS `" + tname + "` (");
                    foreach (DataRow dr in dt.Rows)//寫入各欄位設定
                    {
                        writeStr.Add($"  `{dr[0]}`  {dr[1]} {(dr[1].ToString().ToLower() == "text" ? " CHARACTER SET utf8 " : "")} ");

                        string SQL_Auto_Incre = "SELECT AUTO_INCREMENT FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + database + "' AND TABLE_NAME = '" + tname + "'";
                        if (dr[3].ToString() == "NO")
                            writeStr.Add("NOT NULL");
                        else
                            writeStr.Add("NULL");
                        if (dr[5] != System.DBNull.Value && dr[5].ToString() != "" && dr[5].ToString() != "''")
                        {
                            if (dr[5].ToString().ToUpper().IndexOf("CURRENT_TIMESTAMP") >= 0)
                                writeStr.Add("DEFAULT " + dr[5].ToString());
                            else
                                writeStr.Add("DEFAULT " + "'" + dr[5].ToString() + "'");
                        }
                        //AUTO_INCREMENT
                        if (!string.IsNullOrEmpty(dr[6].ToString()))
                            writeStr.Add(dr[6].ToString());
                        //Comment
                        if (!string.IsNullOrEmpty(dr[8].ToString()))
                            writeStr.Add("COMMENT '" + dr[8].ToString().Replace('\'', '"') + "' ");
                        writeStr.Add(",");
                        schemaDetail.Add(string.Join(" ", writeStr));
                        writeStr.Clear();
                        var increObj = CommonClass.getSQLScalar(SQL_Auto_Incre, ConnString);
                        if (increObj != null && increObj != DBNull.Value)
                            incre = increObj.ToString();
                        else
                            incre = null;
                    }
                    //以下產生 Index
                    i = 0;
                    writeStr.Clear();
                    dt_index = CommonClass.getSQLDataTable(SQL_Table_Index, ConnString);//寫入INDEX行
                    dt_index.TableName = "indexTable";
                    //tempDt1 : Index Name
                    tempDt1 = CommonClass.DataTableDistinct(dt_index, new List<string>() { "Key_name", "Non_unique" });
                    //foreach(DataRow eachIndex in tempDt1.Rows)
                    for (int k = 0; k < tempDt1.Rows.Count; k++) //tempDt1 為 Index 的 名稱
                    {
                        tempDt2 = CommonClass.DataTableFilterSort1(dt_index, "Key_name='" + tempDt1.Rows[k][0].ToString() + "'");
                        foreach (DataRow indexDetail in tempDt2.Rows)
                        {
                            writeStr.Add(indexDetail["Column_name"].ToString());
                        }
                        if (writeStr.Count == 0)
                            continue;
                        //例如 PRIMARY KEY (`CUSTID`)
                        if (tempDt1.Rows[k][0].ToString() == "PRIMARY")
                            tempStr = " PRIMARY KEY ";
                        else if (tempDt1.Rows[k][1].ToString() == "0")
                            tempStr = " UNIQUE KEY " + tempDt1.Rows[k][0];
                        else
                            tempStr = " KEY " + tempDt1.Rows[k][0];
                        schemaDetail.Add(tempStr + " (`" + string.Join("`,`", writeStr) + "`)" +
                        (k == tempDt1.Rows.Count - 1 ? "" : ","));
                        writeStr.Clear();
                        ifIndex = true;
                    }
                    if (tableEngindInnoDBList.Contains(tname))
                        schemaDetail.Add(") ENGINE=InnoDB DEFAULT CHARSET=utf8");
                    else
                        schemaDetail.Add(") ENGINE=MyISAM DEFAULT CHARSET=utf8");
                    if (incre != null && incre.Length != 0)
                    {
                        schemaDetail.Add(" AUTO_INCREMENT=" + incre);
                    }
                    schemaDetail.Add(";");
                    schemaDetail.Add("");
                    #endregion
                }
            }
            catch (Exception ex)
            {
                return "Create " + tname + "schema faile : " + ex.Message;
            }
            //Write Data
            using (StreamWriter appendFile = new System.IO.StreamWriter(@file, true))
            {
                for (int k = 0; k < schemaDetail.Count - 1; k++)
                {
                    if (!ifIndex && k < schemaDetail.Count - 2 && schemaDetail[k + 1].IndexOf("ENGINE=MyISAM") >= 0)
                        appendFile.WriteLine(schemaDetail[k].Substring(0, schemaDetail[k].Length - 1));
                    else
                        appendFile.WriteLine(schemaDetail[k]);
                }
                //backupType: 0: Schema, 1: Data only, 2: schema + Data
                if (backupType == 0) //schema only
                    return null;
                try
                {
                    if (tname.Length >= 4)
                        if ((tname.Substring(0, 4) == "view") || (tname.Substring(0, 4) == "VIEW"))
                        {
                            //判斷這是一個 View
                        }
                        else
                        {
                            #region 寫入資料表內容 Insert .....
                            appendFile.WriteLine("--");
                            appendFile.WriteLine("-- 列出以下資料庫的數據： `" + tname + "`");
                            appendFile.WriteLine("--");
                            appendFile.WriteLine("");
                            appendFile.WriteLine("TRUNCATE TABLE `" + tname + "`;");
                            //appendFile.Write("INSERT INTO `" + tname + "` (");

                            i = 0;
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (i > 0)
                                {
                                    //appendFile.Write(", ");
                                    SQL_Table_Data += ",";
                                }
                                //appendFile.Write("`" + dr[0] + "`");
                                SQL_Table_Data += $"`{dr[0]}`";
                                i++;
                            }
                            //appendFile.Write(") VALUES");
                            SQL_Table_Data += $" FROM `{tname}`";

                            DataTable dt_data = CommonClass.getSQLDataTable(SQL_Table_Data, ConnString);

                            if (dt_data.Rows.Count == 0)
                            {
                                appendFile.WriteLine("");
                                appendFile.WriteLine("-- --------------------------------------------------------");
                                appendFile.WriteLine("");
                            }
                            else
                            {
                                i = 0;
                                foreach (DataRow dr in dt_data.Rows)
                                {
                                    //int row_count = 0;
                                    j = 0;
                                    if ((dr.Table.Rows.IndexOf(dr) % MaxInsert == 0 && i != 0) && dt_data.Rows.Count != i)
                                    {
                                        appendFile.WriteLine(";");
                                    }
                                    else if (i > 0)
                                    {
                                        appendFile.Write(",");
                                    }
                                    if (dr.Table.Rows.IndexOf(dr) % MaxInsert == 0)
                                    {
                                        appendFile.Write(insertHeading(dt, tname));
                                    }
                                    appendFile.WriteLine("");
                                    appendFile.Write("(");
                                    foreach (DataColumn dc in dt_data.Columns)
                                    {
                                        if (j > 0) { appendFile.Write(", "); }
                                        if (dr[dc] == System.DBNull.Value)
                                        {
                                            appendFile.Write("Null");
                                        }
                                        else
                                        {
                                            if (dr[dc].GetType().ToString() == "System.Boolean")
                                            {
                                                appendFile.Write("'" + Convert.ToInt32(dr[dc]).ToString() + "'");
                                            }
                                            else if (dr[dc].GetType().ToString() == "System.DateTime")
                                            {
                                                appendFile.Write("'" + Convert.ToDateTime(dr[dc]).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                                            }
                                            else
                                            {
                                                appendFile.Write("'" + dr[dc].ToString().Replace(@"\", @"\\").Replace("'", "''").Replace("\r", "").Replace("\n", @"\n") + "'");
                                            }
                                        }
                                        j++;
                                    }
                                    i++;
                                    appendFile.Write(")");
                                }
                                appendFile.WriteLine(";");
                                appendFile.WriteLine("");
                            }
                            #endregion
                        }
                    return null;
                }
                catch (Exception ex)
                {
                    return "Insert " + tname + "data Faile : " + ex.Message;
                }
            }
        }

        //寫入資料表和資料內容
        //tableOrder : 0 asc, 1:desc
        //backupType: 0: Schema, 1: Data only, 2: schema + Data
        public void write_data(string database, string ConnString, string file, int backupType = 2, int tableOrder = 0, string tableName = null, Dictionary<string, List<string>> bkJson = null)
        {
            string SQL_Tables = "show full tables where Table_Type = 'BASE TABLE'";
            DataTable dt = CommonClass.getSQLDataTable(SQL_Tables, ConnString);
            if (tableOrder > 0)
                dt = CommonClass.DataTableSort(dt, dt.Columns[0].ColumnName, 1);
            foreach (DataRow dr in dt.Rows)
            {
                if (tableName == null)
                {
                    if (bkJson == null)
                        write_table(dr[0].ToString(), database, ConnString, file, backupType);
                    else if (bkJson["tables"].Contains("all") || bkJson["tables"].Contains(dr[0].ToString()))
                        write_table(dr[0].ToString(), database, ConnString, file, backupType);

                }
                else if (dr[0].ToString() == tableName)
                {
                    if (bkJson == null)
                        write_table(dr[0].ToString(), database, ConnString, file, backupType);
                    else if (bkJson["tables"].Contains("all") || bkJson["tables"].Contains(dr[0].ToString()))
                        write_table(dr[0].ToString(), database, ConnString, file, backupType);
                }
            }
        }

        //建立Insert語句
        public string insertHeading(DataTable dt, string tname)
        {
            //using (StreamWriter appendFile = new System.IO.StreamWriter(@file, true))
            //{
            int i = 0;
            string heading = "INSERT IGNORE INTO `" + tname + "` (";
            foreach (DataRow dr in dt.Rows)
            {
                if (i > 0)
                {
                    heading += ", ";
                }
                heading += "`" + dr[0] + "`";
                i++;
            }
            heading += ") VALUES";
            return heading;
            //}
        }

        //目前備份進度寫入資料庫
        public void statusText(string bkStatus)
        {
            //bkStatus = bkStatus + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + status + Environment.NewLine;
            //CommonClass.writeLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + bkStatus, "MySqlDBBackup", 0, "");
        }

        public string gen_view(string database, string ConnString, string file, Dictionary<string, List<string>> bkJson = null)
        {
            string sqlStr = "SHOW FULL TABLES IN `" + database + "` WHERE TABLE_TYPE LIKE 'VIEW';";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, ConnString);
            DataTable dtView;
            int startPos = 0;
            try
            {
                using (StreamWriter appendFile = new System.IO.StreamWriter(@file, true))
                {
                    //避免 View 有順序的狀況而發生無法產生 View 的問題
                    //一樣的動作做兩次
                    //preViewList 是要先產生的 View
                    List<string> allViews = CommonClass.dtToList(dt, 0);
                    int viewIdx = -1;
                    foreach (string preItem in preViewList)
                    {
                        if ((viewIdx = CommonClass.ifListContantsIgnoreCase(allViews, preItem)) < 0)
                            continue;

                        try
                        {
                            //SELECT VIEW_DEFINITION FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = 'viewTieDaiLeft' 
                            dtView = CommonClass.getSQLDataTable("show create view " + allViews[viewIdx], ConnString);
                        }
                        catch (Exception ex)
                        {
                            //DataBase 沒有 這個 View ， do nothing
                            continue;
                        }

                        appendFile.WriteLine("--");
                        appendFile.WriteLine("-- 資料表 Create View ： `" + preItem + "`");
                        appendFile.WriteLine("--");
                        appendFile.WriteLine("drop view IF Exists " + preItem + ";");

                        startPos = dtView.Rows[0][1].ToString().IndexOf("VIEW `" + allViews[viewIdx]);
                        appendFile.WriteLine("Create " + dtView.Rows[0][1].ToString().Substring(startPos));
                        appendFile.WriteLine(";");
                    }
                    //第二次產生 不在 preViewList 裡面的 View
                    foreach (DataRow dr in dt.Rows)
                    {
                        //if (preViewList.Contains(dr[0].ToString()))
                        //    continue;
                        if (CommonClass.ifListContantsIgnoreCase(preViewList, dr[0].ToString()) >= 0)
                            continue;
                        if (bkJson != null && bkJson["views"][0] != "all" && !bkJson["views"].Contains(dr[0].ToString()))
                            continue;
                        try
                        {
                            //SELECT VIEW_DEFINITION FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = 'viewTieDaiLeft' 
                            dtView = CommonClass.getSQLDataTable("show create view " + dr[0].ToString(), ConnString);
                        }
                        catch (Exception ex)
                        {
                            //DataBase 沒有 這個 View ， do nothing
                            continue;
                        }

                        appendFile.WriteLine("--");
                        appendFile.WriteLine("-- 資料表 Create View ： `" + dr[0] + "`");
                        appendFile.WriteLine("--");
                        appendFile.WriteLine("drop view IF Exists " + dr[0] + ";");

                        startPos = dtView.Rows[0][1].ToString().IndexOf("VIEW `" + dr[0]);
                        appendFile.WriteLine("Create " + dtView.Rows[0][1].ToString().Substring(startPos));
                        appendFile.WriteLine(";");
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return "Create View Error : " + ex.Message;
            }
        }

        public string gen_Function(string database, string ConnString, string file, Dictionary<string, List<string>> bkJson = null)
        {
            string sqlStr = "SHOW FUNCTION STATUS Where db='" + database + "';";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, ConnString);
            DataTable dtView;
            int startPos = 0;
            try
            {
                using (StreamWriter appendFile = new System.IO.StreamWriter(@file, true))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (bkJson != null && !bkJson["functions"].Contains("all") && !bkJson["functions"].Contains(dr["name"].ToString()))
                            continue;
                        appendFile.WriteLine("--");
                        appendFile.WriteLine("-- 資料表 Create Function ： `" + dr["name"].ToString() + "`");
                        appendFile.WriteLine("--");
                        appendFile.WriteLine("drop Function IF Exists " + dr["name"].ToString() + ";");
                        appendFile.WriteLine("DELIMITER $$");
                        dtView = CommonClass.getSQLDataTable("show create Function " + dr["name"].ToString(), ConnString);
                        startPos = dtView.Rows[0]["Create Function"].ToString().IndexOf("FUNCTION `" + dr["name"].ToString());
                        appendFile.WriteLine("Create " + dtView.Rows[0]["Create Function"].ToString().Substring(startPos) + " $$");
                        appendFile.WriteLine("DELIMITER ;");
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return "Create Function Error : " + ex.Message;
            }
        }

        public string gen_Procedure(string database, string ConnString, string file, Dictionary<string, List<string>> bkJson = null)
        {
            string sqlStr =$"SHOW procedure STATUS Where db='{database}';";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, ConnString);
            DataTable dtView;
            int startPos = 0;
            try
            {
                using (StreamWriter appendFile = new System.IO.StreamWriter(@file, true))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (bkJson != null && !bkJson["procedures"].Contains("all") && !bkJson["procedures"].Contains(dr["name"].ToString()))
                            continue;
                        appendFile.WriteLine("--");
                        appendFile.WriteLine("-- 資料表 Create Procedure ： `" + dr["name"].ToString() + "`");
                        appendFile.WriteLine("--");
                        appendFile.WriteLine("drop Procedure IF Exists " + dr["name"].ToString() + ";");
                        appendFile.WriteLine("DELIMITER $$");
                        dtView = CommonClass.getSQLDataTable("show create Procedure " + dr["name"].ToString(), ConnString);
                        startPos = dtView.Rows[0]["Create Procedure"].ToString().IndexOf("PROCEDURE `" + dr["name"].ToString());
                        appendFile.WriteLine("Create " + dtView.Rows[0]["Create Procedure"].ToString().Substring(startPos) + " $$");
                        appendFile.WriteLine("DELIMITER ;");
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return "Create Function Error : " + ex.Message;
            }
        }
        #endregion

        #region 複製資料庫
        public static void copyDB(string srcServer, string srcDB, string destServer, string destDB)
        {
            string connStr = "server=localhost;user=root;port=3306;";
            MySqlConnection conn = new MySqlConnection(connStr);
            MySqlCommand cmd;
            string s0;

            try
            {
                conn.Open();
                s0 = "mysqldump -h localhost -u root -p sourceDB | mysql -h localhost -u root -p ` targetDB ` -pAValidPassword;";
                cmd = new MySqlCommand(s0, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        #endregion
    }
}