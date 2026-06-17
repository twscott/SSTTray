using Newtonsoft.Json;

using RestSharp;
using RestSharp.Extensions.MonoHttp;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FirstOhm
{   //https://blog.yowko.com/httpclient/
    class WebClient
    {
        //url 例如 "http://jsonplaceholder.typicode.com/"
        public async Task<string> webapiPost1(string url, string postDataJson)
        {
            url = "http://localhost:54791/PrdMgn/TestConnection";
            postDataJson = "";
            HttpClient client = new HttpClient() { BaseAddress = new Uri(url) };
            // 指定 authorization header
            client.DefaultRequestHeaders.Add("authorization", "token {api token}");

            postDataJson = postDataJson.Replace("\\\"", "'");
            // 將轉為 string 的 json 依編碼並指定 content type 存為 httpcontent
            HttpContent contentPost = new StringContent(postDataJson, Encoding.UTF8, "application/json");
            // 發出 post 並取得結果
            HttpResponseMessage response = client.PostAsync("Firstohm/Scott", contentPost).GetAwaiter().GetResult();
            // 將回應結果內容取出並轉為 string 再透過 linqpad 輸出
            string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return result;
        }

        public async Task<string> webapiPost2(string url, string postDataJson)
        {
            string result=null;
            url = "http://localhost:54791/PrdMgn/TestConnection";

            postDataJson = "";
            HttpClient client = new HttpClient() { BaseAddress = new Uri(url) };
            client.BaseAddress = new Uri(url);
            // 指定 authorization header
            //client.DefaultRequestHeaders.Add("authorization", "token {api token}");
            // 指定 request 的 method 與 detail url
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "Firstohm/Scott");

            // 將轉為 string 的 json 依編碼並指定 content type 存為 httpcontent
            request.Content = new StringContent(postDataJson, Encoding.UTF8, "application/json");
            // 發出 post 並將回應內容轉為 string 再透過 linqpad 輸出
            await client.SendAsync(request)
            .ContinueWith(responseTask =>
            {
                var apiResult = responseTask.GetAwaiter().GetResult().Content.ReadAsStringAsync();
            });
            return result;
        }

        public async Task<string> webapiPost(string url, string postDataJson)
        {
            String strResult = null;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        #region 呼叫遠端 Web API
                        HttpResponseMessage response = null;

                        #region  設定相關網址內容
                        var fooFullUrl = $"{url}";

                        // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                        //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Content-Type 用於宣告遞送給對方的文件型態
                        //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                        // https://msdn.microsoft.com/zh-tw/library/system.net.http.stringcontent(v=vs.110).aspx
                        using (var fooContent = new StringContent(postDataJson, Encoding.UTF8, "application/json"))
                        {
                            response = await client.PostAsync(fooFullUrl, fooContent).ConfigureAwait(false);
                            //response = await client.PostAsync(fooFullUrl, fooContent);
                        }
                        #endregion
                        #endregion

                        using (HttpContent content = response.Content)
                        {
                            string data = await content.ReadAsStringAsync();
                            return data;
                        }

                        #region 處理呼叫完成 Web API 之後的回報結果
                        if (response != null)
                        {
                            if (response.IsSuccessStatusCode == true)
                            {
                                // 取得呼叫完成 API 後的回報內容
                                strResult = await response.Content.ReadAsStringAsync();
                            }
                            else
                            {
                                strResult = string.Format("Error Code:{0}, Error Message:{1}", response.StatusCode, response.RequestMessage);
                            }
                        }
                        else
                        {
                            strResult = "應用程式呼叫 API 發生異常";
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        CommonClass.writeLog("TaskTrayApp", "webapiPost3", 5, ex.Message, ex);
                    }
                }
            }

            return strResult;
        }

        public async Task<string> postmanPost(string url, Dictionary<string, string> userDataDict)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            //request.AlwaysMultipartFormData = true;
            request.AddHeader("Cookie", "6d3427c6b3dec0b69077f9e836169c7c=449703aa35edc24dd778762beca0f837");
            request.AddHeader("postman-token", "c28e2b12-8e50-8f89-97d6-88613225daa9");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
            StringBuilder userDataStr = new StringBuilder();
            foreach (KeyValuePair<string, string> uItem in userDataDict)
            {
                userDataStr.Append("------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"");
                userDataStr.Append(uItem.Key);
                userDataStr.Append("\"\r\n\r\n");
                userDataStr.Append(uItem.Value);
                userDataStr.Append("\r\n");
            }
            userDataStr.Append("------WebKitFormBoundary7MA4YWxkTrZu0gW--");
            request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW",
                  //"------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"msg\"\r\n\r\nUPS 停電通知 \\\\r\\\\n測試\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"username\"\r\n\r\nscott.tseng\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--"
                  userDataStr.ToString(),
                  ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return null;
        }

        //從 Pay 模擬 PostMan 送 webApi request
        public string postmanPostOuter(string url, Dictionary<string, string> userDataDict)
        {
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cookie", "6d3427c6b3dec0b69077f9e836169c7c=fdd1ce52c7c108850c06d8b652d07743");
            request.AddParameter("fun", userDataDict["fun"]);
            request.AddParameter("pk", userDataDict["pk"]);
            request.AddParameter("key", userDataDict["key"]);
            request.AddParameter("msg", userDataDict["msg"]);
            request.AddParameter("users", userDataDict["users"]);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        //url 例如 "http://jsonplaceholder.typicode.com/"
        public async Task<string> webapiGet1(string url)
        {
            HttpClient client = new HttpClient() { BaseAddress = new Uri(url) };
            //使用 async 方法從網路 url 上取得回應
            var response = await client.GetAsync("posts");
            //如果 httpstatus code 不是 200 時會直接丟出 expection
            response.EnsureSuccessStatusCode();
            // 將 response 內容 轉為 string
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task<string> webapiGet2(string url)
        {
            HttpClient client = new HttpClient() { BaseAddress = new Uri("url") };
            //使用 async 方法從網路 url 上取得回應
            var response = await client.GetAsync("posts");
            //如果 httpstatus code 不是 200 時會直接丟出 expection
            response.EnsureSuccessStatusCode();
            // 將 response 內容 轉為 string
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task<string> webapiGet3(string url)
        {
            HttpClient client = new HttpClient() { BaseAddress = new Uri(url) };
            //使用 async 方法從網路 url 上取得回應
            var response = await client.GetAsync("posts");
            //如果 httpstatus code 不是 200 時會直接丟出 expection
            response.EnsureSuccessStatusCode();
            // 將 response 內容 轉為 string
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }

        //不好用
        public async Task<string> chatGdp(string apiKey, string prompt)
        {
            //string apiKey = "YOUR_API_KEY"; // Replace with your actual API key
            string endpoint = "https://api.openai.com/v1/chat/completions";

            //string prompt = "What is the meaning of life?";
            string apiKeyHeader = "Bearer " + apiKey;
            string result = null;
            
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", apiKeyHeader);
                var content = new StringContent("{\"messages\":[{\"role\":\"system\",\"content\":\"You are a helpful assistant.\"},{\"role\":\"user\",\"content\":\"What is the meaning of life?\"}],\"model\":\"gpt-3.5-turbo\"}", Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(endpoint, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                   // Console.WriteLine(responseBody);
                    result = await response.Content.ReadAsStringAsync();
                    // Handle the response as per your requirements
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Request failed with status code: " + response.StatusCode).AppendLine();
                    sb.Append("Error message: " + responseBody);
                    result = sb.ToString();
                }
                return result;
            }
        }

        //不好用
        public string chatGdp_SyncMode(string apiKey, string prompt)
        {
            //string apiKey = "YOUR_API_KEY"; // Replace with your actual API key
            string endpoint = "https://api.openai.com/v1/chat/completions";

            //string prompt = "What is the meaning of life?";
            string apiKeyHeader = "Bearer " + apiKey;
            string result = null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", apiKeyHeader);
                var content = new StringContent("{\"messages\":[{\"role\":\"system\",\"content\":\"You are a helpful assistant.\"},{\"role\":\"user\",\"content\":\"What is the meaning of life?\"}],\"model\":\"gpt-3.5-turbo\"}", Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(endpoint, content).GetAwaiter().GetResult();
                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    // Console.WriteLine(responseBody);
                    result = responseBody;
                    // Handle the response as per your requirements
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Request failed with status code: " + response.StatusCode).AppendLine();
                    sb.Append("Error message: " + responseBody);
                    result = sb.ToString();
                }
                return result;
            }
        }

        public string chatGdp_RestSharp_SyncMode(string apiKey, string prompt)
        {
            string apiUrl = "https://api.openai.com/v1/chat/completions";
            RestClient client = new RestClient(apiUrl);

            RestRequest request = new RestRequest(Method.POST);
            Dictionary<string, object> requstContent = new Dictionary<string, object>();
            Dictionary<string, object> messagesDict = new Dictionary<string, object>();
            List<Dictionary<string, object>> msgDictList = new List<Dictionary<string, object>>();
            string contentJson = JsonConvert.SerializeObject(msgDictList);
            messagesDict.Add("role", "system");
            messagesDict.Add("content", "You are a helpful assistant.");
            msgDictList.Add(messagesDict);
            messagesDict = new Dictionary<string, object>(); // Create a new dictionary for the user message
            messagesDict.Add("role", "user");
            messagesDict.Add("content", prompt); // Use the provided prompt
            msgDictList.Add(messagesDict);

            requstContent.Add("model", "gpt-3.5-turbo"); // Add the model parameter
            requstContent.Add("messages", msgDictList);
            requstContent.Add("max_tokens", 2048);
            requstContent.Add("temperature", 0.2);
            requstContent.Add("n", 1);
            requstContent.Add("stop", null);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddJsonBody(requstContent); // Set the request body as the entire requstContent dictionary

            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        public string chatGdp_WebRequest_SyncMode(string apiKey, string prompt)
        {
            // API endpoint and headers
            string apiUrl = "https://api.openai.com/v1/engines/davinci-codex/completions";
            string contentType = "application/json";
            string authorizationHeader = $"Bearer {apiKey}";

            // Request payload
            string jsonPayload = $"{{ \"prompt\": \"{prompt}\", \"max_tokens\": 500 }}";

            // Create the web request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.Method = "POST";
            request.ContentType = contentType;
            request.Headers.Add("Authorization", authorizationHeader);

            // Write the payload to the request stream
            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(jsonPayload);
            }

            // Get the response
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string jsonResponse = reader.ReadToEnd();
                return jsonResponse;
            }
        }

        //先 base64 再 url encode
        public string webEncode(string inputStr)
        {
            byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(inputStr);
            inputStr = Convert.ToBase64String(encbuff);
            inputStr = HttpUtility.UrlEncode(inputStr);
            return inputStr;
        }

        //先 url 再 base64 decode
        public string urlDncode(string inputStr)
        {
            inputStr = HttpUtility.UrlDecode(inputStr);
            inputStr = Encoding.UTF8.GetString(Convert.FromBase64String(inputStr)); ;
            return inputStr;
        }

    }
}
