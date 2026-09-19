using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Newtonsoft.Json;

namespace TaskTrayApplication
{
    /// <summary>
    /// B1: 官方 shioaji server HTTP 客戶端（localhost 免認證）。
    /// 使用「快照（全量、無狀態）」；此為 B1 全量軌的資料源（B2 自選軌的 SSE 另建）。
    /// 機密由 server 自 .env（SJ_API_KEY/SJ_SEC_KEY）載入，本類別不觸碰任何金鑰。
    /// </summary>
    static class ShioajiHttpClient
    {
        public const string BaseUrl = "http://127.0.0.1:8080";

        static string Http(string method, string path, string jsonBody = null, int timeoutSec = 20)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(BaseUrl + path);
            req.Method = method;
            req.Timeout = timeoutSec * 1000;
            req.ReadWriteTimeout = timeoutSec * 1000;
            if (jsonBody != null)
            {
                req.ContentType = "application/json";
                byte[] data = Encoding.UTF8.GetBytes(jsonBody);
                req.ContentLength = data.Length;
                using (Stream s = req.GetRequestStream())
                    s.Write(data, 0, data.Length);
            }
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            using (StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                return sr.ReadToEnd();
        }

        /// <summary>檢查 server 是否可達（GET /api/v1/info）。</summary>
        public static bool IsServerUp()
        {
            try { return Http("GET", "/api/v1/info").Contains("\"simulation\""); }
            catch { return false; }
        }

        /// <summary>server 資訊摘要（供 log/mail 用）。</summary>
        public static string ServerInfo()
        {
            try
            {
                string j = Http("GET", "/api/v1/info", null, 8);
                dynamic d = JsonConvert.DeserializeObject(j);
                return "version=" + (string)(d.version ?? "?") + ", simulation=" + (bool?)(d.simulation ?? false);
            }
            catch (Exception ex) { return "server unreachable: " + ex.Message; }
        }

        /// <summary>
        /// 全量快照：POST /api/v1/data/snapshots，分批（預設 ≤200 檔/請求）。
        /// exchangeCodes = (exchange, code) 清單（exchange: TSE/OTC/OES）。
        /// 回傳 server JSON 物件（欄位名為 server 版：close/total_volume/change_rate/datetime…）。
        /// </summary>
        public static List<Dictionary<string, object>> GetSnapshots(List<KeyValuePair<string, string>> exchangeCodes, int batch = 200)
        {
            List<Dictionary<string, object>> rtn = new List<Dictionary<string, object>>();
            if (exchangeCodes == null || exchangeCodes.Count == 0)
                return rtn;
            for (int i = 0; i < exchangeCodes.Count; i += batch)
            {
                List<KeyValuePair<string, string>> chunk = exchangeCodes.Skip(i).Take(batch).ToList();
                StringBuilder sb = new StringBuilder("{\"contracts\":[");
                for (int j = 0; j < chunk.Count; j++)
                {
                    if (j > 0) sb.Append(",");
                    sb.Append("{\"security_type\":\"STK\",\"exchange\":\"")
                      .Append(chunk[j].Key).Append("\",\"code\":\"").Append(chunk[j].Value).Append("\"}");
                }
                sb.Append("]}");
                string json = Http("POST", "/api/v1/data/snapshots", sb.ToString(), 30);
                rtn.AddRange(JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json));
            }
            return rtn;
        }
    }
}