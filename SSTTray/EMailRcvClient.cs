using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EAGetMail; //add EAGetMail namespace

namespace FirstOhm
{
    //參考網站：https://www.emailarchitect.net/eagetmail/kb/csharp.aspx?cat=1
    class EMailRcvClient
    {
        // Generate an unqiue email file name based on date time
        string _generateFileName(int sequence=0)
        {
            DateTime currentDateTime = DateTime.Now;
            return string.Format("{0}-{1:000}-{2:000}.eml",
                currentDateTime.ToString("yyyyMMddHHmmss"),
                currentDateTime.Millisecond,
                sequence);
        }
        List<string> tobeProcess = new List<string>() { "Power fail"};
        //public void receiveMail(string imapServer, string acc= "ups@firstohm.com.tw", string pwd= "Firstpower@alert")
        public void receiveIMap(string imapServer, string acc = "scott.tseng@firstohm.com.tw", string pwd = "Firstpower@alert")
        {
            try
            {
                // Create a folder named "inbox" under current directory
                // to save the email retrieved.
                string localInbox = string.Format("{0}\\inbox", Directory.GetCurrentDirectory());
                // If the folder is not existed, create it.
                if (!Directory.Exists(localInbox))
                {
                    Directory.CreateDirectory(localInbox);
                }

                MailServer oServer = new MailServer(imapServer, acc, pwd, ServerProtocol.Imap4);

                // Enable SSL/TLS connection, most modern email server require SSL/TLS by default
                oServer.SSLConnection = true;
                oServer.Port = 993;
                
                // if your server doesn't support SSL/TLS, please use the following codes
                // oServer.SSLConnection = false;
                // oServer.Port = 143;

                MailClient oClient = new MailClient("TryIt");
                oClient.Connect(oServer);

                MailInfo[] infos = oClient.GetMailInfos();
                Console.WriteLine("Total {0} email(s)\r\n", infos.Length);
                for (int i = 0; i < infos.Length; i++)
                {
                    MailInfo info = infos[i];
                    Console.WriteLine("Index: {0}; Size: {1}; UIDL: {2}",
                        info.Index, info.Size, info.UIDL);

                    // Receive email from IMAP4 server
                    Mail oMail = oClient.GetMail(info);

                    Console.WriteLine("From: {0}", oMail.From.ToString());
                    Console.WriteLine("Subject: {0}\r\n", oMail.Subject);

                    // Generate an unqiue email file name based on date time.
                    string fileName = _generateFileName(i + 1);
                    string fullPath = string.Format("{0}\\{1}", localInbox, fileName);

                    // Save email to local disk
                    oMail.SaveAs(fullPath, true);

                    // Mark email as deleted from IMAP4 server.
                    oClient.Delete(info);
                }

                // Quit and expunge emails marked as deleted from IMAP4 server.
                oClient.Quit();
                Console.WriteLine("Completed!");
            }
            catch (Exception ep)
            {
                Console.WriteLine(ep.Message);
            }
        }

        public void receivePop3(string imapServer, string acc = "ups@conutri.com", string pwd = "firstpower@alert")
        {
            try
            {
                // Create a folder named "inbox" under current directory
                // to save the email retrieved.
                string localInbox = string.Format("{0}\\inbox", Directory.GetCurrentDirectory());
                // If the folder is not existed, create it.
                if (!Directory.Exists(localInbox))
                {
                    Directory.CreateDirectory(localInbox);
                }

                MailServer oServer = new MailServer(imapServer, acc, pwd, ServerProtocol.Pop3);

                // Enable SSL/TLS connection, most modern email server require SSL/TLS by default
                oServer.SSLConnection = false;
                oServer.Port = 110;

                // if your server doesn't support SSL/TLS, please use the following codes
                // oServer.SSLConnection = false;
                // oServer.Port = 995;

                MailClient oClient = new MailClient("TryIt");
                oClient.Connect(oServer);

                MailInfo[] infos = oClient.GetMailInfos();
                Console.WriteLine("Total {0} email(s)\r\n", infos.Length);
                for (int i = 0; i < infos.Length; i++)
                {
                    MailInfo info = infos[i];
                    Console.WriteLine("Index: {0}; Size: {1}; UIDL: {2}",
                        info.Index, info.Size, info.UIDL);

                    // Receive email from IMAP4 server
                    Mail oMail = oClient.GetMail(info);
                    foreach(string chkItem in tobeProcess)
                    {
                        if(oMail.Subject.Contains(chkItem))
                        {
                            processMaile(chkItem, oMail);
                            // Mark email as deleted from POP3 server.
                            oClient.Delete(info);
                        }
                    }
                }

                // Quit and expunge emails marked as deleted from IMAP4 server.
                oClient.Quit();
                Console.WriteLine("Completed!");
            }
            catch (Exception ep)
            {
                Console.WriteLine(ep.Message);
            }
        }

        public void processMaile(string chkItem, Mail oMail)
        {
            switch (chkItem)
            {
                case "Power fail":
                    LineLib linelib = new LineLib();
                    linelib.pushTextMessage("scott.tseng", "停電通知：" + Environment.NewLine +  oMail.TextBody);
                    break;
            }
        }

    }
}
