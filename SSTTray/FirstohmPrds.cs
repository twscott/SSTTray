using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstOhm;

using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Drawing;

namespace FirstOhm
{
    class FirstohmPrds
    {
        #region 有腳無腳
        public static List<string> withFoot = new List<string>() { "C3", "CSR", "EFR", "FGE", "HDR", "HVR", "IG", "MO", "MP", "MSD", "MVR","WA-S", "M-S",
                "NFR", "NL", "PMA", "PPR", "PSR", "PWR", "SCP", "SGP", "SL", "SSR", 
                "SWA", "SSWA-S", "SWAT", "WA", "ZOM" };
        public static List<string> withFoot1Word = new List<string>() { "R", "M" };
        public static List<string> noFoot = new List<string>() { "CM", "CSM", "EFP", "EM", "FM", "HFT", "HVM", "MM", "MM-V",
                "MM-P", "MMP", "MMP-V", "SFP", "SFP-V", "SLC", "SM", "SM-V", "SRM", "SRM-H", "SRM-T", "SRM-P", "SWM", "SWMT", "ZMM", "ZMM-V" };
        public static Dictionary<string, int> specialType = new Dictionary<string, int>() { { "C3", 1 }, { "C3M", 2 } };

        //1:Foot, 2:No Foot, 0:unknow
        public static String rTypePrefix(string RTYPE)
        {
            if (RTYPE.Length >= 3 && RTYPE.Substring(0, 3) == "C3M")
                return "C3M";
            else if (RTYPE.Length >= 2 && RTYPE.Substring(0, 2) == "C3")
                return "C3";
            string tempStr = CommonClass.TrimNumbersFromEndOfStr(RTYPE, "-");
            if (CommonClass.Right(tempStr, 1) == "-")
                return tempStr.Substring(0, tempStr.Length - 1);
            else
                return
                    tempStr;

        }
        //判斷有腳無腳
        //1:有腳, 2:無腳  0:搞不清楚
        public static int ifFoot(string rType)
        {
            string RTPrefix = rTypePrefix(rType);
            if (noFoot.Contains(RTPrefix))
                return 2;
            if (withFoot.Contains(RTPrefix))
                return 1;
            if (withFoot1Word.Contains(RTPrefix))
                return 1;
            if (string.IsNullOrEmpty(rType))
                return 0;
            if (rType.Length >= 2 && rType.Substring(0, 2) == "C3")
                return 1;
            if (rType.Length >= 3 && rType.Substring(0, 3) == "C3M")
                return 2;
            MessageBox.Show("無法計算 " + RTPrefix + " 為有腳還是無腳， 請找業務或工廠查詢， " +
                "再告知程式人員修改程式", "無法確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return -1;
        }

        //1:Foot, 2:No Foot, 0:unknow
        public static int ifFootByPrefix(string rTypePrefix)
        {
            if (withFoot.Contains(rTypePrefix))
                return 1;
            if (noFoot.Contains(rTypePrefix))
                return 2;
            if (withFoot1Word.Contains(rTypePrefix))
                return 1;
            return 0;
        }

        //計算 MFO_BASE 的有腳/無腳
        public static bool SyncBaseFoot(bool ifAll = false)
        {
            int ifFoot = 0;
            string currRtype = null;
            string currBaseID = null;
            List<string> removeList = new List<String>();
            List<String> footList = new List<String>();
            List<String> noFootList = new List<String>();
            String sqlStr = "SELECT `BASE_ID`,`RTYPE` FROM `MFO_BASE` " + (ifAll ? "" : "where `IfFoot`=-1 ");
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.MFOFlowConnString);
            bool retBool = false;
            foreach (DataRow dr in dt.Rows)
            {
                currBaseID = dr["BASE_ID"].ToString();
                if (dr["RTYPE"] != DBNull.Value)
                {
                    currRtype = dr["RTYPE"].ToString();
                }

                else
                {
                    removeList.Add(currBaseID);
                    continue;
                }

                if (!string.IsNullOrEmpty(currRtype))
                    ifFoot = FirstohmPrds.ifFoot(currRtype);
                switch (ifFoot)
                {
                    case 0: //0:搞不清楚
                        continue; //do nothing
                    case 1: //1:有腳
                        footList.Add(currBaseID);
                        retBool = true;
                        break;
                    case 2: //2:無腳
                        noFootList.Add(currBaseID);
                        retBool = true;
                        break;
                }
                if (footList.Count > 50)
                {
                    sqlStr = "Update  `MFO_BASE` Set `IfFoot`= 1 Where `BASE_ID` in (" + string.Join(",", footList) + ")";
                    CommonClass.execSQLNonQuery(sqlStr, Constants.MFOFlowConnString);
                    footList.Clear();
                }

                if (noFootList.Count > 50)
                {
                    sqlStr = "Update  `MFO_BASE` Set `IfFoot`= 0 Where `BASE_ID` in (" + string.Join(",", noFootList) + ")";
                    CommonClass.execSQLNonQuery(sqlStr, Constants.MFOFlowConnString);
                    noFootList.Clear();
                }

                if (removeList.Count > 50)
                {
                    sqlStr = "DELETE From `MFO_BASE`  Where `BASE_ID` in (" + string.Join(",", removeList) + ")";
                    CommonClass.execSQLNonQuery(sqlStr, Constants.MFOFlowConnString);
                    removeList.Clear();
                }
            }

            if (footList.Count > 0)
            {
                sqlStr = "Update  `MFO_BASE` Set `IfFoot`= 1 Where `BASE_ID` in (" + string.Join(",", footList) + ")";
                CommonClass.execSQLNonQuery(sqlStr, Constants.MFOFlowConnString);
            }

            if (noFootList.Count > 0)
            {
                sqlStr = "Update  `MFO_BASE` Set `IfFoot`= 0 Where `BASE_ID` in (" + string.Join(",", noFootList) + ")";
                CommonClass.execSQLNonQuery(sqlStr, Constants.MFOFlowConnString);
            }

            if (removeList.Count > 0)
            {
                sqlStr = "DELETE From `MFO_BASE`  Where `BASE_ID` in (" + string.Join(",", removeList) + ")";
                CommonClass.execSQLNonQuery(sqlStr, Constants.MFOFlowConnString);
            }
            return retBool;

        }

        //return : -1 不知道， 0:無腳  1:有腳
        public static string ifFootByBaseMFOID(string masterMFOID)
        {
            string sqlStr = "SELECT `MFONO`,`MFO_LEAD`,`UID`,`IfFoot` FROM `MFO_BASE` WHERE `MFO_LEAD` ='" + masterMFOID + "'";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            if (dt.Rows.Count == 0)
                return "-1";
            else
                return dt.Rows[0]["IfFoot"].ToString();
        }
        #endregion

        #region 基本轉換
        ////將 RO 轉為 數字字串
        //public static string convertRO(string RO)
        //{
        //    int desPosition = RO.IndexOf('-');
        //    string ROS = "0";
        //    string tempStr;
        //    float tempFloat = 0;
        //    if (desPosition >= 0)
        //    {
        //        RO = RO.Substring(0, desPosition);
        //    }

        //    if (RO.IndexOf('E') > 0)
        //        ROS = CommonClass.Right(RO, 1) == "E" ? RO.Substring(0, RO.Length - 1) : RO.Replace('E', '.');
        //    if (RO.IndexOf('K') > 0)
        //    {
        //        tempStr = CommonClass.exctractNumberFromString(RO.Replace('K', '.'));
        //        ROS = CommonClass.Right(RO, 1) == "K" ? RO.Substring(0, RO.Length - 1) + "000" : (float.Parse(tempStr) * 1000).ToString("######");
        //    }
        //    if (RO.IndexOf('M') > 0)
        //    {
        //        tempStr = CommonClass.exctractNumberFromString(RO.Replace('M', '.'));
        //        ROS = CommonClass.Right(RO, 1) == "M" ? RO.Substring(0, RO.Length - 1) + "000000" : (float.Parse(tempStr) * 1000000).ToString("##########");
        //    }

        //    if (CommonClass.IsNumeric(RO))
        //        ROS = RO;
        //    else
        //        ROS = CommonClass.removeNonNumerics(ROS);


        //    if (CommonClass.Right(ROS, 1) == ".")
        //        ROS = ROS.Substring(0, ROS.Length - 1);
        //    if (!float.TryParse(ROS, out tempFloat))
        //    {
        //        ROS = "0";
        //    }
        //    return ROS;
        //}
        public static Dictionary<string, double> TolDict = new Dictionary<string, double>()
        {
            {"A",0.05},{"B",0.1},{"C",0.25},{"D",0.5},{"F",1},{"G",2},{"J",5},{"K",10},{"M",20}
        };

        public static Dictionary<double, string> TolDictReverse = new Dictionary<double, string>()
        {
            {0.05,"A"},{0.1 ,"B"},{0.25,"C"},{0.5 ,"D"},{1 ,"F"},{2 ,"G"},{5 ,"J"},{10 ,"K"},{20 ,"M"}
        };

        //將 RO 轉為 數字字串
        public static string convertRO(string RO)
        {
            int desPosition = RO.IndexOf('-');
            string ROS = "0";
            string tempStr;
            float tempFloat = 0;
            if (desPosition >= 0)
            {
                RO = RO.Substring(0, desPosition);
            }

            if (RO.IndexOf('E') > 0)
                ROS = CommonClass.Right(RO, 1) == "E" ? RO.Substring(0, RO.Length - 1) : RO.Replace('E', '.');
            if (RO.IndexOf('K') > 0)
            {
                tempStr = CommonClass.exctractNumberFromString(RO.Replace('K', '.'));
                ROS = CommonClass.Right(RO, 1) == "K" ? RO.Substring(0, RO.Length - 1) + "000" : (float.Parse(tempStr) * 1000).ToString("######");
            }
            if (RO.IndexOf('M') > 0)
            {
                tempStr = CommonClass.exctractNumberFromString(RO.Replace('M', '.'));
                ROS = CommonClass.Right(RO, 1) == "M" ? RO.Substring(0, RO.Length - 1) + "000000" : (float.Parse(tempStr) * 1000000).ToString("##########");
            }

            if (CommonClass.IsNumeric(RO))
                ROS = RO;
            else
                ROS = CommonClass.removeNonNumerics(ROS);


            if (CommonClass.Right(ROS, 1) == ".")
                ROS = ROS.Substring(0, ROS.Length - 1);
            if (!float.TryParse(ROS, out tempFloat))
            {
                ROS = "0";
            }
            return ROS;
        }

        //無法轉換則回傳 -1
        public static double valToFloat(string val)
        {
            if (val == "")
                return 0;
            float rtnFloat = -1;
            if (float.TryParse(valToNumber(val), out rtnFloat))
                return Math.Round(rtnFloat, 3);
            else
                return -1;
        }
        public static string valToNumber(string val)
        {
            if (val == "")
                return "0";
            float tempDloat = 0;
            String valsNumber = null;
            int idx = 0;
            for (idx = 0; idx < val.Length; idx++)
            {
                if (!Char.IsNumber(val[idx]))
                {
                    break;
                }
            }
            if (idx < val.Length - 1)
                valsNumber = CommonClass.replacingCharByIdx(val, idx, '.');
            else if ((idx == val.Length - 1))
                valsNumber = val.Substring(0, val.Length - 1);

            switch (val[idx])
            {

                case 'K':
                    Single.TryParse(valsNumber, out tempDloat);
                    tempDloat *= 1000;
                    valsNumber = tempDloat.ToString("#########0.###");
                    break;
                case 'M':
                    Single.TryParse(valsNumber, out tempDloat);
                    tempDloat *= 1000000;
                    valsNumber = tempDloat.ToString("#########0.###");
                    break;
            }
            return valsNumber;
        }
        //誤差值由字元轉換%數
        public static string tolerToNumber(string CToler)
        {
            string NToler;
            switch (CToler)
            {
                case "A": NToler = "0.05%"; break;
                case "B": NToler = "0.1%"; break;
                case "C": NToler = "0.25%"; break;
                case "D": NToler = "0.5%"; break;
                case "F": NToler = "1%"; break;
                case "G": NToler = "2%"; break;
                case "J": NToler = "5%"; break;
                case "K": NToler = "10%"; break;
                case "M": NToler = "20%"; break;
                default: NToler = ""; break;
            }
            return NToler;
        }

        //PPM由數字轉換字串
        public static string ppmToString(string PPM)
        {
            string PpmString;
            switch (PPM)
            {
                case "0.1": PpmString = "TKG"; break;
                case "0.2": PpmString = "TKH"; break;
                case "0.5": PpmString = "TKJ"; break;
                case "1": PpmString = "TKK"; break;
                case "2": PpmString = "TKL"; break;
                case "5": PpmString = "TKM"; break;
                case "10": PpmString = "TKN"; break;
                case "15": PpmString = "TKP"; break;
                case "25": PpmString = "TKQ"; break;
                case "50": PpmString = "TKR"; break;
                case "100": PpmString = "TKS"; break;
                case "150": PpmString = "TKT"; break;
                case "250": PpmString = "TKU"; break;
                case "500": PpmString = "TKV"; break;
                case "1000": PpmString = "TKW"; break;
                case "1500": PpmString = "TKX"; break;
                case "2500": PpmString = "TKY"; break;
                case "200": PpmString = "TK2"; break;
                case "300": PpmString = "TK3"; break;
                case "400": PpmString = "TK4"; break;
                case "600": PpmString = "TK6"; break;
                case "700": PpmString = "TK7"; break;
                case "800": PpmString = "TK8"; break;
                case "900": PpmString = "TK9"; break;
                case "": PpmString = ""; break;
                default: PpmString = "TKZ"; ; break;
            }
            return PpmString;
        }

        #endregion

        #region 流程單 BarCode
        public static void parse_barcode(string barcode, out string flowID, out string batchNo, out string subFlowID, out string ifFoot)
        {
            try
            {
                ifFoot = "0";
                string[] barcCode = barcode.Split('-');
                flowID = barcCode[0];
                batchNo = barcCode[1];
                subFlowID = barcCode[2];
                //ifFoot==0 無腳, ifFoot==1 有腳
                if (barcCode.Length == 5)
                    ifFoot = barcCode[3];
                else
                {
                    string sqlStr = "SELECT `ifFoot` FROM `MFO_FLOWSUB` WHERE `SUBFLOWID`=" + subFlowID;
                    ifFoot = CommonClass.getSQLScalar(sqlStr).ToString();
                }
            }
            catch (Exception ex)
            {
                flowID = batchNo = subFlowID = ifFoot = null;
            }
        }


        //Gen FlowBar ba SubFlowID
        //IDType 0:subFlowID,  1:SignID
        public static string genFlowBar(string subFlowID, int IDType = 0, string conn = null)
        {
            string sqlStr;
            if (IDType == 0)
                sqlStr = "SELECT CONCAT(`FLOWID`, '-', `BATCH_NO` + 1, '-', `SUBFLOWID`, '-', `ifFoot`, '-%') " +
                    " FROM `MFO_FLOWSUB` WHERE `SUBFLOWID` = " + subFlowID;
            else
                sqlStr = "SELECT CONCAT(b.`FLOWID`, '-', b.`BATCH_NO` + 1, '-', b.`SUBFLOWID`, '-', b.`ifFoot`, '-%') " +
                    " FROM MFO_SIGN a " +
                    " Inner Join `MFO_FLOWSUB` b on a.SUBFLOWID = b.`SUBFLOWID` " +
                    " WHERE a.`SIGNID` = 16020 " + subFlowID;
            object tempFlowBar = CommonClass.getSQLScalar(sqlStr, conn);
            if (tempFlowBar != null)
                return tempFlowBar.ToString();
            else
                return null;
        }
        #endregion

        #region 電阻材料, SF/MF
        //Material for MF
        //rule 8
        static List<string> MaterialMF = new List<string> { "MM52","MM52V","MMP52","MM52P","SM52",
                                                    "MM207","SM207","MMP207","HFT",
                                                    "SFP101","SFP101V","SFP102",
                                                    "SRM207H","FM26","FM53",
                                                    "SM204","MM204","MMP204","MM204P",
                                                    "3FP204","EM16","PVM","MM204V","SFP204V" };
        static List<string> MaterialSF = new List<string> { "SRM101","SRM101T","SRM101P",
                                                    "SRM207","SRM207T","SRM207P",
                                                    "EFP101","CSM101","CM52","CM207",
                                                    "SRM204","SRM204P","SRM204T","CSM204",
                                                    "CM204","EFP204" };

        static List<string> MFByRtypePreFix = new List<string> { "EM", "MM", "MM-P", "MMP", "SM", "ZMM" };
        static List<string> SpecialByRtypePreFix = new List<string> { "EFP", "SFP", "SRM", "SRM-T", "CSM", "HVM" };
        public static string getCFMF(string rtype)
        {
            if (MaterialMF.Contains(rtype))
                return "MF";
            if (MaterialSF.Contains(rtype))
                return "SF";
            else
                return "";
        }
        public string getCFMFByRtypePrefixNoFoot(string rtypePrefix)
        {
            if (MFByRtypePreFix.Contains(rtypePrefix))
                return "MF";
            if (SpecialByRtypePreFix.Contains(rtypePrefix))
                return "Special";
            else if (rtypePrefix == "M")
                return "MF";
            else
                return "";
        }

        #endregion

        #region 是否金屬
        public static bool ifMetal(string RO, string CFMF)
        {
            if (CFMF == "MF" || CFMF == "MMF")
                return true;
            double RoVal = 0;
            if(double.TryParse(convertRO(RO), out RoVal))
            {
                if (RoVal <= 1)
                    return true;
            }
            return false;
        }

        //Pull value
        public static double getMinPullValue(string size, bool ifMetal)
        {
            switch(size)
            {
                case "0.8x1.99":
                    return 0.4;
                    break;
                case "1.7x5.2":
                    return 4;
                    break;
                case "2x5.2":
                    return 5;
                    break;
                case "3x10":
                case "3.5x10":
                    if (ifMetal)
                        return 8;
                    else
                        return 10;
                    break;
                case "1x3.15":
                    return 2;
                    break;
                case "1.7x5.4":
                    if (ifMetal)
                        return 4;
                    else
                        return 6;
                    break;
                case "2.5x8":
                    if (ifMetal)
                        return 6;
                    else
                        return 8;
                    break;
                default:
                    //double sizeVal = double.Parse(size.Split('x')[0]);
                    //if (sizeVal >=4 )
                    //    break;
                    if (ifMetal)
                        return 10;
                    else
                        return 12;
            }
        }

        #endregion

        #region 電阻色碼
        public static Dictionary<string, string> colorException = new Dictionary<string, string>() { { "0E015-0.05", "棕銀綠-灰-" } };
        public static List<string> colorBand = new List<string>() { "無", "黑", "棕", "紅", "橙", "黃", "綠", "藍", "紫", "灰", "白", "金", "銀" };
        //MSD 42K2 2% 正確：黃紅紅紅紅
        //CSM204 0E2 1%正確色碼:棕黑銀黑
        //M207 10K 1% (1)沒有外加色碼(2)不應該顯示Z0360
        //HVR301 4M 5% 底漆只要顯示第2層:紫色不燃性漆
        //SFP 0E015 0.05%  棕銀綠灰
        //SFP 0E124 5%  棕紅銀黃
        //SFP 0E268 5%  紅藍銀灰

        //Dictionary<double, int> footBand = new Dictionary<double, int>() { {5,4},{1,5},{0.1,6}};
        //Dictionary<double, int> noFootBand = new Dictionary<double, int>() { {5,3},{1,4} };
        //Toler 決定 Dictionary<Toler, 條數>
        //無腳 4 條(含)以下, 最後一條是倍數
        public static Dictionary<double, int> noFootBand = new Dictionary<double, int>() { { 2, 3 }, { 5, 3 }, { 10, 3 },
                                                             { 0.2, 4 }, { 0.5, 4 }, { 1, 4 }, { 0.1, 5 }, {0.05,5}, {0.25,4} };
        public static Dictionary<double, int> footBand = new Dictionary<double, int>() { { 2, 4 }, { 5, 4 }, { 10, 4 },
                                                            { 1, 5 },{0.5,5},{0.25,5},{0.1,5} };
        public static Dictionary<string, string> bandColorDic = new Dictionary<string, string>() {
            {"黑","000000"},
            {"棕","A52A2A"},
            {"紅","FF0000"},
            {"橙","FF6600"},
            {"黃","FFFF00"},
            {"綠","009900"},
            {"藍","0000FF"},
            {"紫","9900FF"},
            {"灰","999999"},
            {"白","FFFFFF"},
            {"金","FFCC00"},
            {"銀","CCCCCC"},
            {"無","FFFFFF"}

        };

        public static Dictionary<int, string> colorIdx = new Dictionary<int, string>() {
            {0,"黑"},{1,"棕"},{2,"紅"},{3,"橙"},{4,"黃"},{5,"綠"},{6,"藍"},{7,"紫"},{8,"灰"},{9,"白"}
        };

        public static Dictionary<double, string> multiIdx = new Dictionary<double, string>() {
            {1,"黑"},{10,"棕"},{100,"紅"},{1000,"橙"},{10000,"黃"},{100000,"綠"},{1000000,"藍"},{10000000,"紫"},{0.1,"金"},{0.01,"銀"}
        };

        public static Dictionary<double, string> tolIdx = new Dictionary<double, string>() {
            {1,"棕"},{2,"紅"},{0.5,"綠"},{0.25,"藍"},{0.1,"紫"},{0.05,"灰"},{5,"金"},{10,"銀"}
        };

        //無腳底漆
        public static Dictionary<string, string> backColorDic = new Dictionary<string, string>() {
            {"金屬塗料"        ,"115, 173, 143"},
            {"金屬 墨綠"       ,"115, 173, 143"},
            {"金屬塗料 墨綠"   ,"115, 173, 143"},
            {"磚紅"            ,"255, 190, 190"},
            {"內 MP106  外 磚紅","255, 190, 190"},
            {"MP106  外 磚紅"  ,"255, 190, 190"},
            {"磚紅不燃性漆"    ,"255, 190, 190"},
            {"碳膜 乳黃"       ,"255, 224, 160"},
            {"CM02 碳膜乳黃"   ,"255, 224, 160"},
            {"粉紅A"           ,"255, 204, 175"},
            {"MM02 藍色A"      ,"30, 130, 200"},
            {"7921 藍色"       ,"115, 195, 170"},
            {"7921 藍色B"      ,"115, 195, 170"},
            {"7921"            ,"115, 195, 170"},
            {"①7921"           ,"115, 195, 170"},
            {"棕"              ,"153, 51, 0" },
            {"MP106 粉紅"      ,"250, 204, 186"},
            {"MP106 粉紅B"     ,"250, 204, 186" },
            {"MO 藍色"         ,"100, 166, 206"},
            {"藍色不燃性漆"    ,"100, 166, 206" },
            {"蘋果綠"          ,"155, 255, 170"},
            {"蘋果綠不燃性漆"  ,"155, 255, 170"},
            {"綠色不燃性漆"    ,"75, 133, 75"},
            {"綠色"            ,"75, 133, 75"},
            {"紫色"            ,"204, 153, 204"},
            {"7921 紫色不燃性漆","204, 153, 204"},
            {"紫色不燃性漆"    ,"204, 153, 204"}
        };


        public static Color getBackColorRGB(string backColor)
        {
            try
            {
                if (string.IsNullOrEmpty(backColor))
                    return Color.FromArgb(255, 255, 255);
                string rgbStr = backColorDic[backColor];
                if (string.IsNullOrEmpty(rgbStr))
                    return Color.FromArgb(255, 255, 255);
                string[] rgb = rgbStr.Split(',');
                return Color.FromArgb(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2]));
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2146232969)
                    return Color.FromArgb(255, 255, 255);
                else
                    MessageBox.Show(ex.HResult + " " + ex.Message);
            }

            return Color.FromArgb(255, 255, 255);
        }

        #region 有腳底漆
        public static Dictionary<string, string> basepainColorCode = new Dictionary<string, string>()
        {
            {"MP106","MP106 粉紅B"},
            {"2070", "MM02 藍色A"},
            {"7921", "7921 藍色"},
            {"①7921", "7921 藍色"},
            {"②紫色不燃性漆"    ,"7921 紫色不燃性漆"},
            {"②磚紅不燃性漆"    ,"磚紅不燃性漆"},
            {"Z0415","粉紅A"},
            {"Z360","CM02 碳膜乳黃"},
            {"蘋果綠不燃性漆","蘋果綠"},
            {"綠色不燃性漆","綠色"},
            {"藍色不燃性漆","MO 藍色"},
            {"紫色不燃性漆","紫色"},
            {"磚紅不燃性漆","磚紅"},
            {"金屬塗料","金屬 墨綠"}
        };

        //底漆+色碼
        public static Dictionary<string, string> basepainColor = new Dictionary<string, string>()
                    {{"HVR","①7921,②紫色不燃性漆,加ㄧ道藍色色碼"},{"無感HVR","紫色不燃性漆,加ㄧ道綠色色碼"}
                    ,{"MSD","紫色不燃性漆,加ㄧ道白色色碼"},{"SGP","紫色不燃性漆,加ㄧ道黑色色碼"}
                    ,{"SCP","紫色不燃性漆,加ㄧ道黑色色碼"},{"PPR","磚紅不燃性漆,加ㄧ道橙色色碼"}
                    ,{"SSR","磚紅不燃性漆,加ㄧ道紅色色碼"},{"HDR","磚紅不燃性漆,加ㄧ道黑色色碼"}
                    ,{"PWR","藍色不燃性漆,加ㄧ道紅色色碼"},{"NL","藍色不燃性漆,加ㄧ道黑色色碼"}
                    ,{"EFR","磚紅不燃性漆,加ㄧ道灰色色碼"},{"PSR","綠色不燃性漆,加ㄧ道紅色色碼"}
                    ,{"CSR","磚紅不燃性漆,加ㄧ道棕色色碼"},{"MVR","①MP106,②磚紅不燃性漆,加ㄧ道紅色色碼"}
                    ,{"C3","蘋果綠不燃性漆,加ㄧ道紅色色碼"},{"NFR","紫色不燃性漆"}
                    ,{"FGE","藍色不燃性漆,加ㄧ道黃色色碼"},{"SWA","綠色不燃性漆,加ㄧ道藍色色碼"},{"MO","藍色不燃性漆"}
                    ,{"M","金屬塗料"},{"WA","綠色不燃性漆"},{"R","紫色不燃性漆"}
                    };

        //以下用以決定底漆顏色
        public static List<string> P7921_1 = new List<string> { "EM16", "MM204V", "MM52", "SM204", "MM207", "SM207" };
        //rule 2
        public static List<string> P7921_H = new List<string> { "SFP102", "MM52V", "MM204V", "MMP204", "MMP52", "MMP204V", "MMP52V", "MM102" };
        //rule 3
        public static List<string> P2070_3 = new List<string> { "MM204", "SM204", "EM16", "MM52", "MM207", "SM207" };
        //rule 4
        public static List<string> MP106_1 = new List<string> { "EFP101", "SFP101", "SRM101", "EFP204", "SFP204", "SRM204" };
        //rule 5
        public static List<string> MP106_H = new List<string> { "SFP101", "SFP204" };
        //rule 6
        public static List<string> MP106_3 = new List<string> { "SFP101V", "SFP204V", "PVM20" };
        //rule 7
        public static List<string> Z0415 = new List<string> { "EFP101", "SFP101", "SRM101", "EFP204", "SFP204", "SRM204" };
        //rule 8
        public static List<string> Z0360 = new List<string> { "CM204", "CM52", "CM207", "EF16", "EF25" };

        #endregion

        #region 無腳底漆
        //priority No.1
        public static Dictionary<string, string> RtypeP1 = new Dictionary<string, string>()
        {
            {"MM102","7921"}, {"SFP102","7921"}
        };

        public static Dictionary<string, string> noFootbasepainP1 = new Dictionary<string, string>()
        {{"MM-V","7921"},{"SM-V","7921"},{"ZMM-V","7921"},
            {"SFP-V","MP106"},{"HFT","Z0415"},{"SLC","PCI"},
            {"SWM","綠色不燃性漆"},{"SWMT","綠色不燃性漆"},{"FM","藍色不燃性漆"},
            {"CM","Z360"}
        };

        //Priority 2
        //RTYPE: "EM","MM","MM-P","MMP","SM","ZMM"
        public static Dictionary<double, string> MFTolP2 = new Dictionary<double, string>()
        {
            {0.1,"7921"},{0.25,"7921"},{0.5,"7921"}
        };

        //Priority 3
        //RTYPE: "EM","MM","MM-P","MMP","SM","ZMM"
        public static Dictionary<string, string> MFValP2 = new Dictionary<string, string>()
        {
            {"< 10000","2070"},{">= 10000","7921"}
        };

        //Priority 2
        //RType: "EFP","SFP","SRM","CSM","HVM"
        public static Dictionary<double, string> SFTolP2 = new Dictionary<double, string>()
        {
            {0.1,"MP106"},{0.25,"MP106"}
        };

        //Priority 3
        //RType: "EFP","SFP","SRM","SRM-T","CSM","HVM"
        public Dictionary<string, string> SFValP2 = new Dictionary<string, string>()
        {
            {"< 10000","Z0415"},{">= 10000","MP106"}
        };
        #endregion

        #endregion

        #region 班別
        //1:早班/2:中班/3:晚班/4:大早班/5:大中班, ～的第三個參數 代表 跨天， 例如 "16:30~00:30~1"
        public static Dictionary<int, string> shiftType = new Dictionary<int, string>() {
            { 1, "8:00~16:30" }, { 2, "16:30~00:30~1" },{ 3, "00:30~08:00~2" }, { 4, "12:30~20:30"}, { 5, "8:30~20:00" }, { 6, "12:30~24:00" }};

        public static List<DateTime> getShifTime(string shifName, DateTime inputDate)
        {
            List<DateTime> rtnListTime = new List<DateTime>();
            string[] shifTime = getSimpleShifTime(shifName);
            DateTime nextDate = inputDate.AddDays(1);
            if (shifTime.Length == 3 && shifTime[2] == "2")
                rtnListTime.Add(Convert.ToDateTime(nextDate.ToString("yyyy-MM-dd") + " " + shifTime[0]));
            else
                rtnListTime.Add(Convert.ToDateTime(inputDate.ToString("yyyy-MM-dd") + " " + shifTime[0]));
            if (shifTime.Length == 3)
                rtnListTime.Add(Convert.ToDateTime(nextDate.ToString("yyyy-MM-dd") + " " + shifTime[1]));
            else
                rtnListTime.Add(Convert.ToDateTime(inputDate.ToString("yyyy-MM-dd") + " " + shifTime[1]));
            return rtnListTime;
        }
        public static List<DateTime> getEmpShifTime(string empID, string shifName, DateTime inputDate)
        {
            List<DateTime> rtnListTime = getShifTime(shifName, inputDate);
            string sqlStr = null;
            if (shifName == "晚班")
            {
                sqlStr = $"SELECT * FROM `mfo_loginout` where `EMPID`='{empID}' and Date(`Llogout`) = '{inputDate.AddDays(1).ToString("yyyy-MM-dd")}'";
            }
            else
            {
                sqlStr = $"SELECT * FROM `mfo_loginout` where `EMPID`='{empID}' and  Date(`Llogin`) = '{inputDate.ToString("yyyy-MM-dd")}'";
            }
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            if (dt.Rows.Count != 0)
            {
                rtnListTime.Clear();
                rtnListTime.Add(Convert.ToDateTime(dt.Rows[0]["Llogin"]));
                rtnListTime.Add(Convert.ToDateTime(dt.Rows[0]["Llogout"]));
            }
            return rtnListTime;
        }
        public static string getPerformDate(string shifName, DateTime loginTime, DateTime logoutTime)
        {
            DateTime performDate;
            switch(shifName)
            {
                case "早班":
                case "中班":
                    performDate = loginTime;
                    break;
                case "晚班":
                    performDate = logoutTime.AddDays(-1);
                    break;
                default:
                    performDate = loginTime;
                    break;
            }
            return performDate.ToString("yyyy/MM/dd");
        }
        //以登入時間 +2Hours , 模糊比對班別
        public static void getShiftNameByLoginTime(DateTime loginTime, out string shiftName, out string performDateStr)
        {
            DateTime Hrs2more = loginTime.AddHours(2);
            if (Hrs2more.Hour <= 2)
            {
                shiftName = "晚班";
                performDateStr = Hrs2more.AddDays(-1).ToString("yyyy/MM/dd");
            }
            else if (Hrs2more.Hour <= 11)
            {
                shiftName = "早班";
                performDateStr = Hrs2more.ToString("yyyy/MM/dd");
            }
            else
            {
                shiftName = "中班";
                performDateStr = Hrs2more.ToString("yyyy/MM/dd");
            }
        }

        public static void getShiftInfo(DateTime loginTime, DateTime logoutTime, out string shiftName, out string performDateStr)
        {
            double hourDiff = (logoutTime - loginTime).TotalHours;
            int middleHour = loginTime.AddMinutes((logoutTime - loginTime).TotalMinutes/2).Hour;
            shiftName = "早班";
            if(hourDiff < 0 || hourDiff > 20)
                shiftName = "無法判別";
            else if (middleHour > 8 && loginTime.Hour <= 16)
                shiftName = "早班";
            else if (middleHour > 16 && loginTime.Hour <= 24)
                shiftName = "中班";
            else
                shiftName = "晚班";
            performDateStr = getPerformDate(shiftName, loginTime, logoutTime);
        }

        public static void setOfficialLoginout(string shifName, DateTime workDate, string empID)
        {
            int PreOfficialMins = int.Parse(Constants.getProperty("PreOfficialMins", "270")); 
            int PostOfficialMins = int.Parse(Constants.getProperty("PostOfficialMins", "270"));
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
            List<string> loginoutID = CommonClass.dtToList(dt, "ID");
            //string logTimeID = dt.Rows[0]["ID"].ToString();
            string logTimeID = Math.Round(CommonClass.DataTableMin(dt, "ID", "1=1")).ToString();
            DataTable sortedDT = CommonClass.DataTableFilterSort1(dt, null, "LoginTime");
            DateTime loginTime = Convert.ToDateTime(sortedDT.Rows[0]["LoginTime"]);
            DateTime logoutTime = DateTime.Now;
            //找出最後的登出時間
            foreach (DataRow dr in dt.Rows)
            {
                logoutTime = Convert.ToDateTime(dr["LogOutTime"]);
                if ((logoutTime - loginTime).TotalHours < 18)
                {
                    break;
                }
            }
            double hourDiff = (logoutTime - loginTime).TotalHours;
            //登出人時間差太多了， 就用標準時間當官方時間
            List<DateTime> shiftTime = FirstohmPrds.getShifTime(shifName, workDate);//班別標準時間
            if (hourDiff > 18 || hourDiff < 3)
            {
                if (Math.Abs((shiftTime[0] - loginTime).TotalHours) > 3)
                    loginTime = shiftTime[0];
                if (Math.Abs((shiftTime[1] - logoutTime).TotalHours) > 3)
                    logoutTime = shiftTime[1];
            }
            if (loginTime > shiftTime[0])
                loginTime = shiftTime[0];
            if (logoutTime < shiftTime[1])
                logoutTime = shiftTime[1];
            loginTime = loginTime.AddMinutes(-1);
            logoutTime = logoutTime.AddMinutes(1);
            sqlStr = $"update  `mfo_loginout` set `Llogin`='0000-00-00', `Llogout`='0000-00-00', " +
                     $" byWho='{empID}' " +
                     $" Where  ID in ({string.Join(",", loginoutID)})";
            CommonClass.execSQLNonQuery(sqlStr);
            sqlStr = $"Update mfo_loginout " +
            $" Set Llogout = '{logoutTime.ToString("yyyy-MM-dd HH:mm:ss")}', Llogin = '{loginTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
            $" performDate='{workDate.ToString("yyyy-MM-dd")}', shiftType='{shifName}',byWho='{empID}' " +
            $" Where ID={logTimeID}";
            CommonClass.execSQLNonQuery(sqlStr);
        }

        //public static void setOfficialLoginout(string shifName, DateTime workDate, string empID)
        //{
        //    int PreOfficialMins = int.Parse(Constants.getProperty("PreOfficialMins", "510"));
        //    int PostOfficialMins = int.Parse(Constants.getProperty("PostOfficialMins", "510"));
        //    string shiftCondition = FirstohmPrds.getSfitTimeCondition(shifName, workDate, " between", PreOfficialMins, PostOfficialMins);
        //    //string sqlStr = "delete FROM `mfo_loginout` where TIMESTAMPDIFF(HOUR, `LoginTime`,`LogOutTime`) > 16 and ";
        //    //CommonClass.execSQLNonQuery(sqlStr);
        //    string sqlStr = $"SELECT ID, `EMPID`, LoginTime as realLogin, (`LoginTime`) LoginTime, (`LogOutTime`) LogOutTime " +
        //                    $" FROM `mfo_loginout` " +
        //                    $" where LoginTime {shiftCondition} " +
        //                    $" and `EMPID`='{empID}' order by LogOutTime desc";
        //    DataTable dt = CommonClass.getSQLDataTable(sqlStr);

        //    if (dt.Rows.Count == 0)
        //        return;
        //    //string logTimeID = dt.Rows[0]["ID"].ToString();
        //    string logTimeID = Math.Round(CommonClass.DataTableMin(dt, "ID", "1=1")).ToString();
        //    DataTable sortedDT = CommonClass.DataTableFilterSort1(dt, null, "LoginTime");
        //    DateTime loginTime = Convert.ToDateTime(sortedDT.Rows[0]["LoginTime"]);
        //    DateTime logoutTime = DateTime.Now;
        //    //找出最後的登出時間
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        logoutTime = Convert.ToDateTime(dr["LogOutTime"]);
        //        if ((logoutTime - loginTime).TotalHours < 16)
        //        {
        //            break;
        //        }
        //    }

        //    double hourDiff = (logoutTime - loginTime).TotalHours;
        //    //登出人時間差太多了， 就用標準時間當官方時間
        //    if (hourDiff > 16 || hourDiff < 3)
        //    {
        //        List<DateTime> shiftTime = FirstohmPrds.getShifTime(shifName, workDate);
        //        if (Math.Abs((shiftTime[0] - loginTime).TotalHours) > 3)
        //            loginTime = shiftTime[0];
        //        if (Math.Abs((shiftTime[1] - logoutTime).TotalHours) > 3)
        //            logoutTime = shiftTime[1];
        //    }
        //    sqlStr = $"update  `mfo_loginout` set `Llogin`='0000-00-00', `Llogout`='0000-00-00' " +
        //             $" Where shiftType='' and EMPID='{empID}'";
        //    CommonClass.execSQLNonQuery(sqlStr);
        //    sqlStr = $"Update mfo_loginout " +
        //    $" Set Llogout = '{logoutTime.ToString("yyyy-MM-dd HH:mm:ss")}', Llogin = '{loginTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
        //    $" performDate='{workDate.ToString("yyyy-MM-dd")}', shiftType='{shifName}' " +
        //    $" Where ID={logTimeID}";
        //    CommonClass.execSQLNonQuery(sqlStr);
        //}

        public static string[] getSimpleShifTime(int shifType)
        {
            string[] shifTime = null;
            DateTime inputDate2;
            switch (shifType)
            {
                case 1:
                    shifTime = FirstohmPrds.shiftType[1].Split('~');
                    break;
                case 2:
                    shifTime = FirstohmPrds.shiftType[2].Split('~');
                    break;
                case 3:
                    shifTime = FirstohmPrds.shiftType[3].Split('~');
                    break;
                case 4:
                    shifTime = FirstohmPrds.shiftType[4].Split('~');
                    break;
                case 5:
                    shifTime = FirstohmPrds.shiftType[5].Split('~');
                    break;
                case 6:
                    shifTime = FirstohmPrds.shiftType[6].Split('~');
                    break;
            }
            return shifTime;
        }

        public static string[] getSimpleShifTime(string shifName)
        {
            int shiftType = 1;
            switch (shifName)
            {
                case "早班":
                    shiftType = 1;
                    break;
                case "中班":
                    shiftType = 2;
                    break;
                case "晚班":
                    shiftType = 3;
                    break;
                case "跨早中":
                    shiftType = 4;
                    break;
                case "大早班":
                    shiftType = 5;
                    break;
                case "大晚班":
                    shiftType = 6;
                    break;
            }
            string[] shifTime = getSimpleShifTime(shiftType);
            return shifTime;
        }

        //hhmm 例如 8:00
        public static string extentTime(string inputDateTime, int shiftMin)
        {
            DateTime rtnTime;
            if (DateTime.TryParse(inputDateTime, out rtnTime))
                return rtnTime.AddMinutes(shiftMin).ToString("yyyy-MM-dd HH:mm");
            else
                return inputDateTime;
        }

        //回傳 開始時間 and 結束時間 ， 例如 '2020-07-01 08:00' and  '2020-07-01 16:30'
        public static string getSfitTimeCondition(DateTime inputDate, int shifType, int beforeMin = 0, int afterMin = 0)
        {
            string shiftCondition = null;
            DateTime inputDate2;
            string[] shifTime = getSimpleShifTime(shifType);
            if (shifTime.Length == 3 && shifTime[2] == "1")
                inputDate2 = inputDate.AddDays(1);
            else if (shifTime.Length == 3 && shifTime[2] == "2")
            {
                inputDate2 = inputDate.AddDays(1);
                inputDate = inputDate.AddDays(1);
            }
            else
                inputDate2 = inputDate;
            if (beforeMin > 0 || afterMin > 0)
                shiftCondition = "'" + extentTime(inputDate.ToString("yyyy-MM-dd") + " " + shifTime[0], -beforeMin) + "' and '" + extentTime(inputDate2.ToString("yyyy-MM-dd") + " " + shifTime[1], afterMin) + "' ";
            else
                shiftCondition = "'" + inputDate.ToString("yyyy-MM-dd") + " " + shifTime[0] + "' and '" + inputDate2.ToString("yyyy-MM-dd") + " " + shifTime[1] + "' ";
            return shiftCondition;
        }
        public static string getLoginTimeCondition(DateTime inputDate, int shifType, int beforeMin = 0, int afterMin = 0)
        {
            string shiftCondition = null;
            DateTime inputDate2;
            string[] shifTime = getSimpleShifTime(shifType);
            if (shifTime.Length == 3 && shifTime[2] == "1")
                inputDate2 = inputDate.AddDays(1);
            else if (shifTime.Length == 3 && shifTime[2] == "2")
            {
                inputDate2 = inputDate.AddDays(1);
                inputDate = inputDate.AddDays(1);
            }
            else
                inputDate2 = inputDate;
            if (beforeMin > 0 || afterMin > 0)
                shiftCondition = "'" + extentTime(inputDate.ToString("yyyy-MM-dd") + " " + shifTime[0], -beforeMin) + "' and '" + extentTime(inputDate2.ToString("yyyy-MM-dd") + " " + shifTime[0], afterMin) + "' ";
            else
                shiftCondition = "'" + inputDate.ToString("yyyy-MM-dd") + " " + shifTime[0] + "' and '" + inputDate2.ToString("yyyy-MM-dd") + " " + shifTime[0] + "' ";
            return shiftCondition;
        }

        //回傳範例 ：  and `Finish_Time` between  '2000-01-01 00：00' and '2000-01-01 24：00'
        //回傳該班的上班 - 下班 時間， beforeMin 為官方時間往前延伸（分）, int afterMin為官方時間往後延伸（分）
        public static string getSfitTimeCondition(string shifName, DateTime inputDate, string sqlWhere = " and `Finish_Time` between", int beforeMin = 0, int afterMin = 0)
        {
            string shiftCondition = null;
            switch (shifName)
            {
                case "早班":
                    shiftCondition = sqlWhere + FirstohmPrds.getSfitTimeCondition(inputDate, 1, beforeMin, afterMin);
                    break;
                case "中班":
                    shiftCondition = sqlWhere + FirstohmPrds.getSfitTimeCondition(inputDate, 2, beforeMin, afterMin);
                    break;
                case "晚班":
                    shiftCondition = sqlWhere + FirstohmPrds.getSfitTimeCondition(inputDate, 3, beforeMin, afterMin);
                    break;
                case "跨早中":
                    shiftCondition = sqlWhere + FirstohmPrds.getSfitTimeCondition(inputDate, 4, beforeMin, afterMin);
                    break;
                case "大早班":
                    shiftCondition = sqlWhere + FirstohmPrds.getSfitTimeCondition(inputDate, 5, beforeMin, afterMin);
                    break;
                case "大晚班":
                    shiftCondition = sqlWhere + FirstohmPrds.getSfitTimeCondition(inputDate, 6, beforeMin, afterMin);
                    break;
            }
            return shiftCondition;
        }

        public static string getLoginTimeCondition(string shifName, DateTime inputDate, string sqlWhere = " and `Finish_Time` between", int beforeMin = 0, int afterMin = 0)
        {
            string shiftCondition = null;
            switch (shifName)
            {
                case "早班":
                    shiftCondition = sqlWhere + FirstohmPrds.getLoginTimeCondition(inputDate, 1, beforeMin, afterMin);
                    break;
                case "中班":
                    shiftCondition = sqlWhere + FirstohmPrds.getLoginTimeCondition(inputDate, 2, beforeMin, afterMin);
                    break;
                case "晚班":
                    shiftCondition = sqlWhere + FirstohmPrds.getLoginTimeCondition(inputDate, 3, beforeMin, afterMin);
                    break;
                case "跨早中":
                    shiftCondition = sqlWhere + FirstohmPrds.getLoginTimeCondition(inputDate, 4, beforeMin, afterMin);
                    break;
                case "大早班":
                    shiftCondition = sqlWhere + FirstohmPrds.getLoginTimeCondition(inputDate, 5, beforeMin, afterMin);
                    break;
                case "大晚班":
                    shiftCondition = sqlWhere + FirstohmPrds.getSfitTimeCondition(inputDate, 6, beforeMin, afterMin);
                    break;
            }
            return shiftCondition;
        }
        #endregion

        #region 公司料號  MPN
        public static readonly Dictionary<string, string> RemarkInMpn = new Dictionary<string, string> { { "棕色漆", "B" } };
        static Dictionary<string, string> ppmDict = new Dictionary<string, string>()
        {
            {"TKG","0.1"},{"TKH","0.2"},
            {"TKJ","0.5"},{"TKK","1"},
            {"TKL","2"  },{"TKM","5"},
            {"TKN","10"},{"TKP","15"},
            {"TKQ","25"},{"TKR","50"},
            {"TKS","100"},{"TKT","150"},
            {"TKU","250"},{"TKV","500"},
            {"TKW","1000"},{"TKX","1500"},
            {"TKY","2500"},{"TK2","200"},
            {"TK3","300"},{"TK4","400"},
            {"TK6","600"},{"TK7","700"},
            {"TK8","800"},{"TK9","900"},
            {"TKZ",""}
        };

        public static Dictionary<int, string> onecaseDict = new Dictionary<int, string>()
        {
            {1000,"1K0"},{2000,"2K0"},{2500,"2K5"},
            {3000,"3K0"},{5000,"5K0"},{6000,"6K0"},
            {10000,"10K"},{100,"100"},{150,"150"},
            {400,"400"},{500,"500"},{200,"200"},
            {250,"250"},{350,"350"},{300,"300"}
        };

        public static Dictionary<double, string> tolDict = new Dictionary<double, string>()
        {
            { 0.1,"B"}, { 0.25,"C"}, { 0.5,"D"},{ 1,"F"}, { 2,"G"}, { 5,"J"},{ 10,"K"},{ 20,"M"}
        };

        public static Dictionary<string, string> packDict = new Dictionary<string, string>()
        {
            {"T/R","TR"},{"T/B","TB"},{"B","BK"}
        };

        public static double getOnecaseCnt(string oncaseStr)
        {
            double rtnDbl=0;
            foreach(KeyValuePair<int, string> onecaseItem in onecaseDict)
            {
                if (onecaseItem.Value == oncaseStr)
                {
                    rtnDbl = Math.Round((double)onecaseItem.Key / 1000, 1);
                    break;
                }
            }
            return rtnDbl;
        }

        public static string getOnecaseStr(string pnab_onecase)
        {
            string strOnecase = null;
            int onecaseInt = 0;
            if (!int.TryParse(pnab_onecase, out onecaseInt))
                return null;
            if(onecaseDict.Keys.Contains(onecaseInt))
                strOnecase = onecaseDict[onecaseInt];
            return strOnecase;
        }

        public static double getTol(string pnab_tol)
        {
            foreach (KeyValuePair<double, string> tolItem in tolDict)
                if (tolItem.Value == pnab_tol)
                    return tolItem.Key;
            return -1;
        }

        public static string getPack(string pnab_pack)
        {
            string strOnecase = null;
            int onecaseInt = 0;
            foreach (KeyValuePair<string, string> packItem in packDict)
                if (packItem.Value == pnab_pack)
                    return packItem.Key;
            return "";
        }

        public static string getValStr(string pnab_Val)
        {
            int i = 0;
            for (i = (pnab_Val.Length - 1); i >= 0; i--)
                if (pnab_Val[i] != '0')
                    break;
            //pnab_Val = pnab_Val.Substring(0,(i + 1)).Replace("R", "E");
            if (pnab_Val == "R")
                pnab_Val = "R0";
            return pnab_Val;
        }

        ////MM204F1R00TKRTR3K0 => MM204-F-1R00-TKR-TR3K0
        public static Dictionary<string, string> pnabToAtt(string PNAB)
        {
            if (PNAB=="")
                return null;
            int PNABLength = PNAB.Length;
            //從後面算的長度, onecase / pack / ppm / val / toler / rtype
            List<int> charLenFromBack = new List<int> { 3, 2, 3, 4, 1 };
            string calcStr;
            List<string> pnabList = new List<string>();
            Dictionary<string, string> rtnDict = new Dictionary<string, string>();
            int j = 0;
            foreach (int i in charLenFromBack)
            {
                pnabList.Add(CommonClass.Right(PNAB, i));
                if (j >= pnabList.Count)
                {
                    break;
                }
                int lastIndex = PNAB.LastIndexOf(pnabList[j++]);
                PNAB = PNAB.Substring(0, lastIndex );
                switch (j)
                {
                    case 1: //onecase
                        rtnDict.Add("onecase", getOnecaseCnt(pnabList[0]).ToString("0.#"));
                        break;
                    case 2: //pack
                        rtnDict.Add("pack", getPack(pnabList[1]));
                        break;
                    case 3://ppm
                        if (ppmDict.Keys.Contains(pnabList[2]))
                            rtnDict.Add("ppm", ppmDict[pnabList[2]]);
                        else
                            rtnDict.Add("ppm", "");
                        break;
                    case 4://val
                        rtnDict.Add("val", getValStr(pnabList[3]));
                        break;
                    case 5: //toler
                        //rtnDict.Add("tol", getTol(pnabList[4]).ToString());
                        rtnDict.Add("tol", pnabList[4].ToString());
                        break;
                }
            }
            rtnDict.Add("rtype", PNAB);
            return rtnDict;
            
        }

        //商品屬性 轉成 公司料號
        public string infoToPnab(string CUST, string RTYPE, string VAL, string WATT, string TOLER, string PPM, string PACK, string ONECASE, string Remark)
        {
            string ppm, pk = "", ocase = "", oocase = "", val = "", value = "", pnabtotal;
            string jbval = "", jbpnabtotal;
            string mhppm, mhpnabtotal;
            string aepnabtotal;
            string sjpnabtotal;
            string gupnabtotal;
            string OLDMPN;

            ppm = ppmDict[PPM];
            oocase = getOnecaseCnt(ONECASE).ToString("#.#");
            switch (VAL)
            {
                case "180R": value = "180E"; break;
                case "0R": jbval = "0R0"; break;
                default: value = VAL; jbval = VAL; break;
            }
            val = pnabRType(VAL, RTYPE);

            if (PPM != "")
                mhppm = "TK" + PPM; //use in mhpnabtotal
            else
                mhppm = "";//use in mhpnabtotal

            if (RTYPE == "JP52") TOLER = "Z"; //新增型號JP52誤差值格式，Added by Michael, 2020.3.18

            pnabtotal = RTYPE + TOLER + val + "" + ppm + pk + ocase;//4D PNAB;
            gupnabtotal = RTYPE + TOLER + val + ppm + pk + ocase;//GU PNAB;

            if (VAL == "0R")
            {
                //modified by Scott, 2017/9/12
                // pnabtotal = RTYPE + "X" + "R000" + "XXX" + pk + ocase;
                if (TOLER == "")//added by Michael, 2018/3/2, 假如Value是0R, Toler也是0, 標籤例: (MPN)ZMM204XR000XXXTR3K0
                    pnabtotal = RTYPE + "X" + "R000" + "XXX" + pk + ocase;
                else
                { //有TOLER時的處理，有PPM就用PPM的代碼，例：MM204FR000TKRTR3K0；無PPM就用TKZ，例：EFP101JR000TKZTR2K0, Modified by Michael, 2020.2.26
                    if (PPM == "")
                        pnabtotal = RTYPE + TOLER + "R000" + "TKZ" + pk + ocase;
                    else
                        pnabtotal = RTYPE + TOLER + "R000" + ppm + pk + ocase;
                }

                gupnabtotal = RTYPE + "X" + "R000" + "XXX" + pk + ocase;//GU PNAB;
            }

            if (RTYPE == "R25" && VAL == "J-W" && TOLER == "J")
                gupnabtotal = "JW-06/65TB5K0";//GU PNAB;
            if (RTYPE == "R25" && VAL == "J-W" && CUST == "CBFIT")
                pnabtotal = "JP52" + "Z" + "JW00" + ppm + pk + ocase; //型號JP52首次鍵入，名稱用R25建檔時公司料號格式，2020.3.18

            if (RTYPE == "SFP201")
                mhpnabtotal = RTYPE + TOLER + mhppm + value;// mh PN
            else
                mhpnabtotal = RTYPE + TOLER + mhppm + value + pk + oocase;// mh PN

            if (RTYPE == "CP100")
                jbpnabtotal = RTYPE + "-" + TOLER + "-" + VAL + "-" + pk;//jb ji MPN
            else if (RTYPE == "CP12")
                jbpnabtotal = RTYPE + "-" + TOLER + "-" + VAL + "-" + pk;//jb ji MPN
            else
                jbpnabtotal = RTYPE + TOLER + jbval + pk + oocase;//jb ji MPN

            aepnabtotal = RTYPE + TOLER + VAL + pk + oocase;//AE MPN
            sjpnabtotal = RTYPE + TOLER + mhppm + VAL + pk + oocase;//SJ MPN
            OLDMPN = RTYPE + TOLER + mhppm + VAL + pk + oocase;

            //jbpnabtotal += Constants.RemarkInMpn[Remark]; //備註有棕色漆的，公司料號後面加上字母B, Added by Michael, 2020.9.2
            pnabtotal += Remark == "棕色漆" ? RemarkInMpn[Remark] : ""; //備註有棕色漆的，公司料號後面加上字母B, Added by Michael, 2020.9.2
            if (CUST == "SDFIL" && Remark.Contains("2020/10/22")) //客戶代號SDFIL，下單日2020/10/22，料號的TB250改成BK050, Added by Michael, 2020.11.3
                pnabtotal = pnabtotal.Replace("TB250", "BK050");
            pnabtotal += (CUST == "SDFIL" && Remark.Contains("43MM") ? "S" : ""); //客戶代號SDFIL，REMARK備註:43MM，公司料號尾端加S, Added by Michael, 2020.10.16

            if (CUST.Substring(0, 2) == "JI") return jbpnabtotal; //CUST.Substring(0, 2) == "JB" || //已標準化標籤全部改用新料號，Modified by Michael, 2020.1.22
            else if ("1.3X2.7,1.7X5.2,2.5X8,4X12".Contains(RTYPE)) return "";//紅電阻半成品不印PNAB，Added by Michael, 2020.3.19
            //else if (CUST.Substring(0, 2) == "MH") return mhpnabtotal;
            //else if (CUST.Substring(0, 2) == "AE") return aepnabtotal;
            //else if (CUST.Substring(0, 2) == "GU") return gupnabtotal;
            //else if (CUST.Substring(0, 2) == "SJ") return sjpnabtotal;
            else return pnabtotal;
        }

        //電阻阻值重新排列-固定四個字元
        private string pnabRType(string VAL, string RTYPE)
        {
            string val = "";

            StringBuilder sb = new StringBuilder(VAL);
            int len = sb.Length;
            if (len == 5)
            {
                val = AllinOnePnab(VAL);
            }
            if (len == 4)
            {
                char a0 = sb[0];
                char a1 = sb[1];
                char a2 = sb[2];
                char a3 = sb[3];
                if (a0 == '0' && a1 == 'R')
                {
                    val = a1.ToString() + a2.ToString() + a3.ToString() + "0";
                }
                else
                {
                    val = VAL;
                }
            }
            if (len == 3)
            {
                val = LoseTwoPnab(VAL);
                val = FourThreePnab(VAL, val);
            }
            if (len == 2)
            {
                val = LoseThreePnab(VAL);
            }
            if (RTYPE == "JP52") //新增型號JP52阻值格式，Added by Michael, 2020.3.18
            {
                switch (VAL)
                {
                    case "J-W":
                        val = "JW00";
                        break;
                    case "JW":
                        val = "JW00";
                        break;
                }
            }

            return val;
        }

        //5字元-4字元
        private static string AllinOnePnab(string VAL)
        {
            string val = "";
            StringBuilder sb = new StringBuilder(VAL);
            char a0 = sb[0];
            char a1 = sb[1];
            char a2 = sb[2];
            char a3 = sb[3];
            char a4 = sb[4];
            switch (a1.ToString())
            {
                case "R":
                    val = a1.ToString() + a2.ToString() + a3.ToString() + a4.ToString();
                    break;
                case "K":
                    val = a1.ToString() + a2.ToString() + a3.ToString() + a4.ToString();
                    break;
                case "M":
                    val = a1.ToString() + a2.ToString() + a3.ToString() + a4.ToString();
                    break;
            }

            return val;
        }
        //3字元-4字元
        private static string LoseTwoPnab(string VAL)
        {
            string val = "";

            StringBuilder sb = new StringBuilder(VAL);
            char a0 = sb[0];
            char a1 = sb[1];
            char a2 = sb[2];
            if (a0 == '0' && a1 == 'R')
            {
                val = a1.ToString() + a2.ToString() + "00";
            }
            else
            {
                switch (a1.ToString())
                {
                    case "R":
                        val = a0.ToString() + a1.ToString() + a2.ToString() + "0";
                        break;
                    case "K":
                        val = a0.ToString() + a1.ToString() + a2.ToString() + "0";
                        break;
                    case "M":
                        val = a0.ToString() + a1.ToString() + a2.ToString() + "0";
                        break;
                }
            }

            return val;
        }
        //3字元-4字元
        private static string FourThreePnab(string VAL, string val)
        {
            //string val = "";

            StringBuilder sb = new StringBuilder(VAL);
            char a0 = sb[0];
            char a1 = sb[1];
            char a2 = sb[2];
            switch (a2.ToString())
            {
                case "R":
                    val = a0.ToString() + a1.ToString() + a2.ToString() + "0";
                    break;
                case "K":
                    val = a0.ToString() + a1.ToString() + a2.ToString() + "0";
                    break;
                case "M":
                    val = a0.ToString() + a1.ToString() + a2.ToString() + "0";
                    break;
            }

            return val;
        }
        //2字元-4字元
        private static string LoseThreePnab(string VAL)
        {
            string val = "";

            StringBuilder sb = new StringBuilder(VAL);
            char a0 = sb[0];
            char a1 = sb[1];
            switch (a1.ToString())
            {
                case "R":
                    val = a0.ToString() + a1.ToString() + "00";
                    break;
                case "K":
                    val = a0.ToString() + a1.ToString() + "00";
                    break;
                case "M":
                    val = a0.ToString() + a1.ToString() + "00";
                    break;
            }

            return val;
        }
        public static string infotopnab(string CUST, string RTYPE, string VAL, string WATT, string TOLER, string PPM, string PACK, string ONECASE, string Remark)
        {
            string ppm, pk = "", ocase = "", oocase = "", val = "", value = "", pnabtotal;
            string jbval = "", jbpnabtotal;
            string mhppm, mhpnabtotal;
            string aepnabtotal;
            string sjpnabtotal;
            string gupnabtotal;
            string OLDMPN;

            #region PPM_PACK_ONECASE對應值
            switch (PPM)
            {
                case "0.1": ppm = "TKG"; break;
                case "0.2": ppm = "TKH"; break;
                case "0.5": ppm = "TKJ"; break;
                case "1": ppm = "TKK"; break;
                case "2": ppm = "TKL"; break;
                case "5": ppm = "TKM"; break;
                case "10": ppm = "TKN"; break;
                case "15": ppm = "TKP"; break;
                case "25": ppm = "TKQ"; break;
                case "50": ppm = "TKR"; break;
                case "100": ppm = "TKS"; break;
                case "150": ppm = "TKT"; break;
                case "250": ppm = "TKU"; break;
                case "500": ppm = "TKV"; break;
                case "1000": ppm = "TKW"; break;
                case "1500": ppm = "TKX"; break;
                case "2500": ppm = "TKY"; break;
                case "200": ppm = "TK2"; break;
                case "300": ppm = "TK3"; break;
                case "400": ppm = "TK4"; break;
                case "600": ppm = "TK6"; break;
                case "700": ppm = "TK7"; break;
                case "800": ppm = "TK8"; break;
                case "900": ppm = "TK9"; break;
                default: ppm = "TKZ"; ; break;
            }
            switch (PACK)
            {
                case "T/R": pk = "TR"; break;
                case "T/B": pk = "TB"; break;
                case "B": pk = "BK"; break;
            }
            switch (ONECASE)
            {
                case "1000": ocase = "1K0"; oocase = "1"; break;
                case "2000": ocase = "2K0"; oocase = "2"; break;
                case "2500": ocase = "2K5"; oocase = "2.5"; break;
                case "3000": ocase = "3K0"; oocase = "3"; break;
                case "5000": ocase = "5K0"; oocase = "5"; break;
                case "6000": ocase = "6K0"; oocase = "6"; break;
                case "10000": ocase = "10K"; oocase = "10"; break;
                case "100": ocase = "100"; break;
                case "150": ocase = "150"; break;
                case "400": ocase = "400"; oocase = "0.4"; break;
                case "500": ocase = "500"; oocase = "0.5"; break;
                case "200": ocase = "200"; break;
                case "250": ocase = "250"; break;
                case "350": ocase = "350"; break;
                case "300": ocase = "300"; break;

            }
            #region 被註解程式
            /*switch (ONECASE)
            {
                case "1000": oocase = "1"; break;
                case "2000": oocase = "2"; break;
                case "2500": oocase = "2.5"; break;
                case "3000": oocase = "3"; break;
                case "5000": oocase = "5"; break;
                case "6000": oocase = "6"; break;
                case "10000": oocase = "10"; break;
                case "500": oocase = "0.5"; break;
                case "400": oocase = "0.4"; break;
            }*/
            #endregion
            #endregion
            switch (VAL)
            {
                case "180R": value = "180E"; break;
                case "0R": jbval = "0R0"; break;
                default: value = VAL; jbval = VAL; break;
            }
            val = btnPnab(VAL, RTYPE);

            if (PPM != "")
                mhppm = "TK" + PPM; //use in mhpnabtotal
            else
                mhppm = "";//use in mhpnabtotal

            if (RTYPE == "JP52") TOLER = "Z"; //新增型號JP52誤差值格式，Added by Michael, 2020.3.18

            pnabtotal = RTYPE + TOLER + val + "" + ppm + pk + ocase;//4D PNAB;
            gupnabtotal = RTYPE + TOLER + val + ppm + pk + ocase;//GU PNAB;

            if (VAL == "0R")
            {
                //modified by Scott, 2017/9/12
                // pnabtotal = RTYPE + "X" + "R000" + "XXX" + pk + ocase;
                if (TOLER == "")//added by Michael, 2018/3/2, 假如Value是0R, Toler也是0, 標籤例: (MPN)ZMM204XR000XXXTR3K0
                    pnabtotal = RTYPE + "X" + "R000" + "XXX" + pk + ocase;
                else
                { //有TOLER時的處理，有PPM就用PPM的代碼，例：MM204FR000TKRTR3K0；無PPM就用TKZ，例：EFP101JR000TKZTR2K0, Modified by Michael, 2020.2.26
                    if (PPM == "")
                        pnabtotal = RTYPE + TOLER + "R000" + "TKZ" + pk + ocase;
                    else
                        pnabtotal = RTYPE + TOLER + "R000" + ppm + pk + ocase;
                }

                gupnabtotal = RTYPE + "X" + "R000" + "XXX" + pk + ocase;//GU PNAB;
            }

            if (RTYPE == "R25" && VAL == "J-W" && TOLER == "J")
                gupnabtotal = "JW-06/65TB5K0";//GU PNAB;
            if (RTYPE == "R25" && VAL == "J-W" && CUST == "CBFIT")
                pnabtotal = "JP52" + "Z" + "JW00" + ppm + pk + ocase; //型號JP52首次鍵入，名稱用R25建檔時公司料號格式，2020.3.18

            if (RTYPE == "SFP201")
                mhpnabtotal = RTYPE + TOLER + mhppm + value;// mh PN
            else
                mhpnabtotal = RTYPE + TOLER + mhppm + value + pk + oocase;// mh PN

            if (RTYPE == "CP100")
                jbpnabtotal = RTYPE + "-" + TOLER + "-" + VAL + "-" + pk;//jb ji MPN
            else if (RTYPE == "CP12")
                jbpnabtotal = RTYPE + "-" + TOLER + "-" + VAL + "-" + pk;//jb ji MPN
            else
                jbpnabtotal = RTYPE + TOLER + jbval + pk + oocase;//jb ji MPN

            aepnabtotal = RTYPE + TOLER + VAL + pk + oocase;//AE MPN
            sjpnabtotal = RTYPE + TOLER + mhppm + VAL + pk + oocase;//SJ MPN
            OLDMPN = RTYPE + TOLER + mhppm + VAL + pk + oocase;

            //之後備註有棕色漆 20210413 不加B
            //pnabtotal += Remark == "棕色漆" ?RemarkInMpn[Remark] : ""; //備註有棕色漆的，公司料號後面加上字母B, Added by Michael, 2020.9.2

            if (CUST == "SDFIL" && Remark.Contains("2020/10/22")) //客戶代號SDFIL，下單日2020/10/22，料號的TB250改成BK050, Added by Michael, 2020.11.3
                pnabtotal = pnabtotal.Replace("TB250", "BK050");
            //pnabtotal += (CUST == "SDFIL" && Remark.Contains("43MM") ? "S" : ""); //客戶代號SDFIL，REMARK備註:43MM，公司料號尾端加S, Added by Michael, 2020.10.16
            pnabtotal += (Remark.Contains("43MM") ? "S" : ""); //不管客戶為何，REMARK備註:43MM，公司料號尾端加S, Added by Ronny 2021.04.09

            if (CUST.Substring(0, 2) == "JI") return jbpnabtotal; //CUST.Substring(0, 2) == "JB" || //已標準化標籤全部改用新料號，Modified by Michael, 2020.1.22
            else if ("1.3X2.7,1.7X5.2,2.5X8,4X12".Contains(RTYPE)) return "";//紅電阻半成品不印PNAB，Added by Michael, 2020.3.19
            //else if (CUST.Substring(0, 2) == "MH") return mhpnabtotal;
            //else if (CUST.Substring(0, 2) == "AE") return aepnabtotal;
            //else if (CUST.Substring(0, 2) == "GU") return gupnabtotal;
            //else if (CUST.Substring(0, 2) == "SJ") return sjpnabtotal;
            else return pnabtotal;
        }

        //電阻阻值重新排列-固定四個字元
        public static string btnPnab(string VAL, string RTYPE)
        {
            string val = "";

            StringBuilder sb = new StringBuilder(VAL);
            int len = sb.Length;
            if (len == 5)
            {
                val = AllinOnePnab(VAL);
            }
            if (len == 4)
            {
                char a0 = sb[0];
                char a1 = sb[1];
                char a2 = sb[2];
                char a3 = sb[3];
                if (a0 == '0' && a1 == 'R')
                {
                    val = a1.ToString() + a2.ToString() + a3.ToString() + "0";
                }
                else
                {
                    val = VAL;
                }
            }
            if (len == 3)
            {
                val = LoseTwoPnab(VAL);
                val = FourThreePnab(VAL, val);
            }
            if (len == 2)
            {
                val = LoseThreePnab(VAL);
            }
            if (RTYPE == "JP52") //新增型號JP52阻值格式，Added by Michael, 2020.3.18
            {
                switch (VAL)
                {
                    case "J-W":
                        val = "JW00";
                        break;
                    case "JW":
                        val = "JW00";
                        break;
                }
            }

            return val;
        }

        #endregion

        #region 良率計算
        public static Dictionary<string, string> yieldStatistic(DataTable dgv1Dt, string startDate, string endDate, string rType, 
            string val, string tol, string ppm) 
        {            
            Dictionary<string, object> spParems = new Dictionary<string, object>()
            {
                {"startDate",startDate},{"endDate",endDate},{"cRtype",rType},
                {"cVal", val },{"cTol", tol },{"cPPM", ppm }
            };
            DataTable dt = null;
            //DataTable dt = CommonClass.spRtnDT("YieldStatistics", spParems, Constants.ConnString);

            if (dt == null || dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value)
                return null;
            Dictionary<string, string> rtnDict = new Dictionary<string, string>();
            rtnDict.Add("流程單數", Convert.ToInt32(dt.Rows[0]["流程單數"]).ToString("0.#"));
            rtnDict.Add("總起始量", Convert.ToInt32(dt.Rows[0]["總起始量"]).ToString());
            rtnDict.Add("總完成量", Convert.ToInt32(dt.Rows[0]["總完成量"]).ToString());
            rtnDict.Add("平均良率", Convert.ToDouble(dt.Rows[0]["加權平均良率"]) > 100 ? "100" : Convert.ToDouble(dt.Rows[0]["加權平均良率"]).ToString("0.0"));
            rtnDict.Add("加權平均良率", Convert.ToDouble(dt.Rows[0]["加權平均良率"]) > 100 ? "100" : Convert.ToDouble(dt.Rows[0]["加權平均良率"]).ToString("0.0"));
            
            double dblMidian = CommonClass.genMidian(dgv1Dt, "流程單良率");
            rtnDict.Add("中位數", dblMidian > 100 ? "100" : dblMidian.ToString("0.0"));
            rtnDict.Add("最低良率", Convert.ToDouble(dt.Rows[0]["最低良率"]) > 100 ? "100" : Convert.ToDouble(dt.Rows[0]["最低良率"]).ToString("0.0"));
            rtnDict.Add("最高良率", Convert.ToDouble(dt.Rows[0]["最高良率"]) > 100 ? "100" : Convert.ToDouble(dt.Rows[0]["最高良率"]).ToString("0.0"));

            List<Int32> yieldsList = CommonClass.dtToIntList(dgv1Dt, "流程單良率");
            Dictionary<int, int> zhongShuDict = CommonClass.CalcZhongShu(yieldsList);
            if (zhongShuDict == null)
            {
                rtnDict.Add("眾數", "樣品數不足分析眾數");
                rtnDict.Add("眾數加權平均", "-1");
            }
            else
            {
                int idx = 0;

                rtnDict.Add("眾數", JsonConvert.SerializeObject(zhongShuDict));
                if (zhongShuDict.Count > 1)
                { //多峰 眾數
                    StringBuilder whereStr = new StringBuilder(" Select Round(SUM(外檢2良率*`起始量`)/SUM(起始量),1) 加權平均良率 From view_basicyield where (");
                    foreach (double targetYile in zhongShuDict.Keys)
                    {
                        if (idx++ > 0)
                            whereStr.Append(" Or ");
                        whereStr.Append($" (流程單良率 >={targetYile - 0.5} and  流程單良率 < {targetYile + 0.5})");
                    }
                    whereStr.Append(")");
                    whereStr.Append($" and `完成日期` >= '{startDate}' and `完成日期` <= '{endDate}' ");
                    whereStr.Append($" and `RTYPE`='{rType}' and `VAL`='{val}' and `TOL`='{tol}' AND PPM='{ppm}'");
                    Double dblZhongShuAvg = Convert.ToDouble(CommonClass.getSQLScalar(whereStr.ToString()));
                    rtnDict.Add("眾數加權平均", dblZhongShuAvg > 100 ? "100" : dblZhongShuAvg.ToString("0.0"));
                }
                else
                {
                    rtnDict.Add("眾數加權平均", zhongShuDict.First().Value.ToString());
                }
            }
            return rtnDict;
        }


        #endregion
    }
}
