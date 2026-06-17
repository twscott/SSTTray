using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using FirstOhm;
using Newtonsoft.Json;

namespace TaskTrayApplication
{
    class UDPLib
    {
        public static int hostPort = 1949;
        //server listen port 1947
        static UdpClient udpClient = new UdpClient(hostPort);
        public static void ThreadRunMethod()
        {
            bool printLable = true;
            while (true)
            {
                try
                {
                    var remoteIp = new IPEndPoint(IPAddress.Any, 0);

                    var receivedBytes = udpClient.Receive(ref remoteIp);

                    if (receivedBytes == null || receivedBytes.Length == 0)
                        continue;
                    string strReceiveStr = Encoding.UTF8.GetString(receivedBytes);
                    if (printLable && Constants.getProperty("UdpPopMsg", "0") == "1")
                    {
                        DialogResult dr = MessageBox.Show("收到 UDP, 內容:" + System.Environment.NewLine +
                            strReceiveStr + System.Environment.NewLine +
                            ", 是否繼續執行 ?", "Received UDP", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.No)
                            continue;
                    }
                    if (strReceiveStr[0]=='[')
                    {
                        //receiveList 是舊寫法
                        List<string> receiveList = JsonConvert.DeserializeObject<List<string>>(strReceiveStr);
                        if (receiveList[0] == "restartApp")
                        {
                            int waitSec = 0;
                            if (receiveList.Count >= 2)
                                int.TryParse(receiveList[1], out waitSec);
                            CommonClass.restartApp(waitSec);
                            return;
                        }
                        else if (receiveList[0] == "reboot")
                        {
                            int waitSec = 0;
                            if (receiveList.Count >= 2)
                                int.TryParse(receiveList[1], out waitSec);
                            CommonClass.rebootComputer(waitSec);
                            return;
                        }
                        CommonApp.processUDPList(receiveList);
                    }
                    else if (strReceiveStr[0] == '{')
                    {
                        //receiveDict 用 Dictionary 傳資料是新寫法
                        Dictionary<string, string> receiveDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(strReceiveStr);
                        CommonApp.processUDPDict(receiveDict);
                    }
                }
                catch (Exception ex)
                {
                    CommonClass.writeLog("TaskTrayApp", "Process Udp Receive", 5, ex.Message, ex);
                }
            }
        }

        public static void udpSend(string sendText, string hostIP, int hostPort)
        {
            try
            {
                var socket = new Socket(AddressFamily.InterNetwork,
                             SocketType.Dgram, ProtocolType.Udp);

                var serverAddr = IPAddress.Parse(hostIP);
                var endPoint = new IPEndPoint(serverAddr, hostPort);
                byte[] sendByte = Encoding.UTF8.GetBytes(sendText);
                socket.SendTo(sendByte, endPoint);
            }
            catch (Exception ex)
            {
                CommonClass.writeLog("TaskTrayApp", "Process Udp Send", 5, ex.Message, ex);
            }
        }
}
}