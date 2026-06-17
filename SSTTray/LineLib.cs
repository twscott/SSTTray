using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// nuget LineBot.SDK -- 作者:Martin Hsu
//nuget LineNotify.SDK-- 作者:Martin Hsu
using LineBotSDK.LIFF;
using LineBotSDK.Struct;
using LineBotSDK.Struct.Messages;
using LineBotSDK.Utility;
using System.Data;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using RestSharp.Extensions.MonoHttp;

namespace FirstOhm
{
    public class LineLib
    {
        Dictionary<string, string> lineUserDict = new Dictionary<string, string>()
        {
            {"康起禎","Uacc00e89ec0698756b329c64f0a8edbb"},
            {"曾建明","U568a7ec0df87da8f3bc6b61205583d4d"},
            {"郭佳霖","U9612a576c00bffbe34e9fae2faba1727"},
            {"李尚祐","Uc061a2ba4e0886bf81982c2e83869154"},
            {"楊瀛洲","Uee4a08985a6dbbbeceef23a0fe9235f8"},
            {"花蓮廠務","U0f9da5043bae272a6ee10baed9d45cca"},
            {"花蓮廠長","Ud34a062840cba0503435d7e2ed62914e"},
            {"蕭人碩","U45f1c132afd1c8774dceee97a94d2f23"},
            {"黃逸楓","U81364b1c14201a5b3de8ee5c51fecce6"}
        };
        //string token = @"3pjYn+6gs3G1dISd6w8Xb8vCOIWiz5ojncvzdVB1jV24nSASa/walrkRhrFMU4kqaRBTZQOZ7S3SplRGUMVfk7sdkYHb1bfgpRuGSoRciWKbGNqhnKDddNPD4pn2ARd8rFG92itQIY+mqjGuLOGmO1GUYhWQfeY8sLGRXgo3xvw=";
        string token = @"3pjYn+6gs3G1dISd6w8Xb8vCOIWiz5ojncvzdVB1jV24nSASa/walrkRhrFMU4kqaRBTZQOZ7S3SplRGUMVfk7sdkYHb1bfgpRuGSoRciWKbGNqhnKDddNPD4pn2ARd8rFG92itQIY+mqjGuLOGmO1GUYhWQfeY8sLGRXgo3xvw=";
        string apiKey = Constants.getProperty("lineAppApiKey", "cc833c5faa70ea8c56a019ae8b0bb6fe");
        string lineURL = Constants.getProperty("lineURL", "http://192.168.1.33:8080/Portal");

        private string getUserID(string portalLoginName)
        {
            string sqlStr = "SELECT b.line_username, b.line_userid " +
                            " FROM `xyz_users` a " +
                            " Left Join xyz_hikashop_user b on a.id=b.user_cms_id " +
                            " where a.`username` = '" + portalLoginName + "' ";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.portalConnString);
            if (dt.Rows.Count >= 1)
                return dt.Rows[0]["line_userid"].ToString();
            else
                return null;
        }

        public string pushTextMessage(string portalLoginName, string msgToSend)
        {
            string userid = getUserID(portalLoginName);
            if (string.IsNullOrEmpty(userid))
                return "查無 " + portalLoginName + " 的 LineID";
            MessageUtility.PushTextMessage(token, userid, msgToSend);
            return null;
        }

        public string pushImageMessage(string portalLoginName, string image_url)
        {
            string userid = getUserID(portalLoginName);
            if (string.IsNullOrEmpty(userid))
                return "查無 " + portalLoginName + " 的 LineID";
            MessageUtility.PushImageMessage(token, userid, image_url, image_url);
            return null;
        }

        public string pushVideoMessage(string portalLoginName, string mp4_url, string image_url)
        {
            string userid = getUserID(portalLoginName);
            if (string.IsNullOrEmpty(userid))
                return "查無 " + portalLoginName + " 的 LineID";
            MessageUtility.PushVideoMessage(token, userid, mp4_url, image_url);
            return null;
        }

        // Send the sticker message to user
        //pushStickerMessageushStickerMessage("scott.tseng", "1", "1");
        public string pushStickerMessage(string portalLoginName, string firstID, string secID)
        {
            string userid = getUserID(portalLoginName);
            if (string.IsNullOrEmpty(userid))
                return "查無 " + portalLoginName + " 的 LineID";
            MessageUtility.PushStickerMessage(token, userid, firstID, secID);
            return null;
        }
        //緯度         ，經度
        public string pushLocationMessage(string portalLoginName, string title, string address, decimal laitude, decimal longitude)
        {
            string userid = getUserID(portalLoginName);
            if (string.IsNullOrEmpty(userid))
                return "查無 " + portalLoginName + " 的 LineID";
            MessageUtility.PushLocationMessage(token, userid, title, address, laitude, longitude);
            return null;
        }

        public string pushImageMapMessage(string portalLoginName, string image_url, string altTxt)
        {
            string userid = getUserID(portalLoginName);
            if (string.IsNullOrEmpty(userid))
                return "查無 " + portalLoginName + " 的 LineID";
            // Send the imagemap message to user
            Size size = new Size(800, 416);
            var actions = new List<IImagemapAction>();
            actions.Add(new ImagemapMessageAction("text", new ImagemapArea(0, 0, 400, 416)));
            actions.Add(new ImagemapURIAction("https://www.google.com/", new ImagemapArea(400, 0, 400, 416)));
            MessageUtility.PushImagemapMessage(token, userid, image_url, altTxt, size, actions);
            return null;
        }

        public async Task<string> pushTextMessageByWebapiAsync(string portalLoginName, string msgToSend)
        {
            WebClient webapi = new WebClient();
            //string userid = getUserID(portalLoginName);
            Dictionary<string, string> msgBody = new Dictionary<string, string>();
            //JsonConvert.SerializeObject(msgBody)
            msgBody.Add("msg", msgToSend);
            msgBody.Add("username", portalLoginName);
            await webapi.postmanPost($"{lineURL}/index.php?app=users&format=raw&resource=sendNotify_line_single&option=com_api&key={apiKey}",
                msgBody);
            return null;
        }

        class LineMsg
        {
            public string msg;
            public List<string> users;
        }
        //從 firstohmPay 發 Line
        //public async Task<string> pushTextMessageOuterMultiUser(List<string> userNames , string msgToSend)
        //{
        //    WebClient webapi = new WebClient();
        //    Dictionary<string, string> lineMsg = new Dictionary<string, string>()
        //    {
        //        {"fun","sendlineByParams"},
        //        {"pk",$"{DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day}2u4u2u04yj3"},
        //        {"key","7d72a3ee37f9872f9f9ebf243e592fcd"}
        //    };
        //    LineMsg lineParams = new LineMsg();
        //    List<string> lineUserIDs = new List<string>();
        //    foreach (string userItem in userNames)
        //        lineUserIDs.Add(lineUserDict[userItem]);
        //    lineParams.users = lineUserIDs;
        //    lineParams.msg = msgToSend;
        //    lineMsg.Add("Params", JsonConvert.SerializeObject(lineParams));
        //    await webapi.postmanPost($"https://pay.firstohm.com.tw/index.php?app=users&format=raw&resource=users&option=com_api",
        //        lineMsg);
        //    return null;
        //}

        public async Task<string> pushTextMessageOuter(string userName, string msgToSend)
        {
            WebClient webapi = new WebClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            List<string> lineUserIds = new List<string>();
            msgToSend = webapi.webEncode(msgToSend);

            Dictionary<string, string> lineMsg = new Dictionary<string, string>()
            {
                {"fun","sendlineOuter"},
                {"pk",$"{DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day}2u4u2u04yj3"},
                {"msg", msgToSend },
                {"userid", lineUserDict[userName]}
            };
            await webapi.postmanPost($"{lineURL}/index.php?app=users&format=raw&resource=sendlineOuter&option=com_api&key={apiKey}", lineMsg);
            return null;
        }
        public Task<string> pushTextMessageOuter(List<string> userNamesList, string msgToSend)
        {
            WebClient webapi = new WebClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            msgToSend = webapi.webEncode(msgToSend);
            Dictionary<string, string> lineMsg = new Dictionary<string, string>()
            {
                //{"fun","sendlineByParamsmulti"},
                {"fun","sendlineByParamssingle"},
                {"pk",$"{DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day}2u4u2u04yj3"},
                {"key","7d72a3ee37f9872f9f9ebf243e592fcd"},
                {"msg", msgToSend }
            };
            foreach (string username in userNamesList)
            {
                if (lineMsg.ContainsKey("users"))
                    lineMsg.Remove("users");
                lineMsg.Add("users", lineUserDict[username]);
                webapi.postmanPostOuter("https://pay.firstohm.com.tw/index.php?app=users&format=raw&resource=users&option=com_api",
                lineMsg);
            }
            return null;
        }
    }
}