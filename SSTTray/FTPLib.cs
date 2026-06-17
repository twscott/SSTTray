using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace FirstOhm
{
    class FTPExtensions
    {
        string ftpServerIP;
        string ftpUserID;
        string ftpPassword;

        public FTPExtensions(string serverIP, string userAcc, string pwd)
        {
            ftpServerIP = serverIP;
            ftpUserID = userAcc;
            ftpPassword = pwd;
        }

        //ftp的上傳功能
        public bool Upload(string remotePath, string localPath)
        {
            FileInfo fileInf = new FileInfo(localPath);

            string uri = "ftp://" + ftpServerIP + remotePath;
            FtpWebRequest reqFTP;

            // 根據uri建立FtpWebRequest物件
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + remotePath));

            // ftp使用者名稱和密碼
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

            // 預設為true，連線不會被關閉
            // 在一個命令之後被執行
            reqFTP.KeepAlive = false;

            // 指定執行什麼命令
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            // 指定資料傳輸型別
            reqFTP.UseBinary = true;

            // 上傳檔案時通知伺服器檔案的大小
            reqFTP.ContentLength = fileInf.Length;

            // 緩衝大小設定為2kb
            int buffLength = 2048;

            byte[] buff = new byte[buffLength];
            int contentLen;

            // 開啟一個檔案流 (System.IO.FileStream) 去讀上傳的檔案
            FileStream fs = fileInf.OpenRead();
            try
            {
                // 把上傳的檔案寫入流
                Stream strm = reqFTP.GetRequestStream();

                // 每次讀檔案流的2kb
                contentLen = fs.Read(buff, 0, buffLength);

                // 流內容沒有結束
                while (contentLen != 0)
                {
                    // 把內容從file stream 寫入 upload stream
                    strm.Write(buff, 0, contentLen);

                    contentLen = fs.Read(buff, 0, buffLength);
                }

                // 關閉兩個流
                strm.Close();
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                 MessageBox.Show(ex.Message, "Upload Error");
                return false;
            }
        }


        //從ftp伺服器上下載檔案的功能
        public bool Download(string remoteFilePath, string localFilePath)
        {
            FtpWebRequest reqFTP;
            string uri = "ftp://" + ftpServerIP ;
            FileStream outputStream = null;
            try
            {
                outputStream = new FileStream(localFilePath, FileMode.Create);
                // 根據uri建立FtpWebRequest物件
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + remoteFilePath));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                // ftp使用者名稱和密碼
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                long cl = response.ContentLength;

                int bufferSize = 2048;

                int readCount;

                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);

                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();

                outputStream.Close();

                response.Close();
                CommonClass.killProcessByName("Microsoft Excel");
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Download Error");
                outputStream.Close();

                return false;
            }
        }

        //從ftp伺服器上獲得檔案列表
        public string[] GetFileList(string remotePath, string recordSeperator="@")
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP;
            WebResponse response;
            // HttpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + remotePath));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append(recordSeperator);
                    line = reader.ReadLine();
                }
                // to remove the trailing '/n'       
                result.Remove(result.ToString().LastIndexOf(recordSeperator), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('@');
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                downloadFiles = null;
                return downloadFiles;
            }
        }
    }
}