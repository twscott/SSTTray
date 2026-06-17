using Newtonsoft.Json;

using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirstOhm;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

namespace TaskTrayApplication
{
    class IO_Simulation
    {
        #region Declare
        public bool actionRunning = false;
        bool runForEver = false;
        string currFilePath = null;
        string currApType = null;
        bool autoRun = false;
        Process currProcess = null;
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);
        DataTable config_dt = null;
        int timeInterval = 180000;
        int initTime1 = 20000;
        int initTime2 = 20000;
        int loopTime = 100000;
        #endregion

        #region Fields
        private const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        private const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */
        private const int MOUSEEVENTF_RIGHTUP = 0x0010; /* right button up */
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; /* middle button down */
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040; /* middle button up */
        private const int MOUSEEVENTF_XDOWN = 0x0080; /* x button down */
        private const int MOUSEEVENTF_XUP = 0x0100; /* x button down */
        private const int MOUSEEVENTF_WHEEL = 0x0800; /* wheel button rolled */
        private const int MOUSEEVENTF_VIRTUALDESK = 0x4000; /* map to entire virtual desktop */
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000; /* absolute move */

        private SynchronizationContext context = null;
        private DateTime start, end;
        private bool first = true;
        private List<ActionEntry> actions;
        public Thread runActionThread;
        private bool byTextEntry = false;
        private Hashtable schedualeList;
        private enum ClickType
        {
            click = 0,
            rightClick = 1,
            doubleClick = 2,
            SendKeys = 3,
            StartRun = 4,
            StopRun = 5,
            ClossProcess = 6,
            Shift = 7 //切換輸入法
        }
        #endregion

        #region calss defind ActionEntry
        private class ActionEntry
        {
            int x;
            int y;
            string text;
            public int interval;
            ClickType type;
            public ActionEntry(int x, int y, string text, int interval, ClickType type)
            {
                this.x = x;
                this.y = y;
                this.text = text;
                this.interval = interval;
                this.type = type;
            }

            public int X
            {
                set { x = value; }
                get { return x; }
            }
            public int Y
            {
                set { y = value; }
                get { return y; }
            }
            public string Text
            {
                set { text = value; }
                get { return text; }
            }

            public int Interval
            {
                set { interval = value; }
                get { return interval; }
            }
            public ClickType Type
            {
                set { type = value; }
                get { return type; }
            }
        }
        private class VirtualMouse
        {
            [DllImport("user32.dll")]
            static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
            private const int MOUSEEVENTF_MOVE = 0x0001;
            private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
            private const int MOUSEEVENTF_LEFTUP = 0x0004;
            private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
            private const int MOUSEEVENTF_RIGHTUP = 0x0010;
            private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
            private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
            private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
            public static void Move(int xDelta, int yDelta)
            {
                mouse_event(MOUSEEVENTF_MOVE, xDelta, yDelta, 0, 0);
            }
            public static void MoveTo(int x, int y)
            {
                mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, x, y, 0, 0);
            }

            public static void MoveTo(float x, float y)
            {
                float min = 0;
                float max = 65535;

                int mappedX = (int)Remap(x, 0.0f, 1920.0f, min, max);
                int mappedY = (int)Remap(y, 0.0f, 1080.0f, min, max);

                mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, mappedX, mappedY, 0, 0);
            }


            public static float Remap(float value, float from1, float to1, float from2, float to2)
            {
                return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            }

            public static void LeftClick()
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
            }

            public static void LeftDown()
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
            }

            public static void LeftUp()
            {
                mouse_event(MOUSEEVENTF_LEFTUP, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
            }

            public static void RightClick()
            {
                mouse_event(MOUSEEVENTF_RIGHTDOWN, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
                mouse_event(MOUSEEVENTF_RIGHTUP, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
            }

            public static void RightDown()
            {
                mouse_event(MOUSEEVENTF_RIGHTDOWN, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
            }

            public static void RightUp()
            {
                mouse_event(MOUSEEVENTF_RIGHTUP, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
            }
        }
        class winIOcontrol
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

            [DllImport("user32")]
            public static extern int SetCursorPos(int x, int y);

            #region Fields
            private const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
            private const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
            private const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
            private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */
            private const int MOUSEEVENTF_RIGHTUP = 0x0010; /* right button up */
            private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; /* middle button down */
            private const int MOUSEEVENTF_MIDDLEUP = 0x0040; /* middle button up */
            private const int MOUSEEVENTF_XDOWN = 0x0080; /* x button down */
            private const int MOUSEEVENTF_XUP = 0x0100; /* x button down */
            private const int MOUSEEVENTF_WHEEL = 0x0800; /* wheel button rolled */
            private const int MOUSEEVENTF_VIRTUALDESK = 0x4000; /* map to entire virtual desktop */
            private const int MOUSEEVENTF_ABSOLUTE = 0x8000; /* absolute move */

            private SynchronizationContext context = null;
            private DateTime start, end;
            private bool first = true;
            private List<ActionEntry> actions;
            //private Thread runActionThread;
            private bool byTextEntry;
            private Hashtable schedualeList;
            #endregion
        }
        #endregion
        public void ReadXmlFromDB(string taskName)
        {
            //Get data from XML file
            XmlSerializer ser = new XmlSerializer(typeof(ActionsEntry));
            string sqlStr = "SELECT `action` FROM `autoExecute`  Where `funcName`= '" + taskName + "'";
            string configResult = CommonClass.getSQLScalar(sqlStr, Constants.ACLConnString).ToString();
            //lvActions.Items.Clear();
            if (string.IsNullOrEmpty(configResult))
            {
                CommonClass.writeLog("監控程式", "OpenFileXml", "sysset 查無 " + taskName + " 錄製執行碼");
                return;
            }

            try
            {
                ActionsEntryAction[] entry = JsonConvert.DeserializeObject<ActionsEntryAction[]>(configResult);
                if (actions == null)
                    actions = new List<ActionEntry>();
                else
                    actions.Clear();
                foreach (ActionsEntryAction ae in entry)
                {
                    string point = ae.X.ToString() + "," + ae.Y.ToString();
                    string interval = (ae.interval).ToString();
                    ListViewItem lvi = new ListViewItem(new string[] { point, ((ClickType)(ae.Type)).ToString(), interval, ae.Text });
                    ActionEntry acion = new ActionEntry(ae.X, ae.Y, ae.Text, ae.interval, (ClickType)(ae.Type));
                    actions.Add(acion);
                }
                //currProcess = getSysName();
                if (runActionThread == null || !runActionThread.IsAlive)
                {
                    runActionThread = new Thread(RunAction);
                    runActionThread.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Clicer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async void RunAction()
        {
        int channel = 1;
            String savePath;
            int tmpIntervl = 0;
            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
            await semaphoreSlim.WaitAsync();
            try
            {
                actionRunning = true;
                if (string.IsNullOrEmpty(currFilePath))
                {
                    currFilePath = Constants.getProperty("Filepath") + @"\" + currProcess + @"\";
                }
                foreach (ActionEntry action in actions)
                {
                    if (action.Type.Equals(ClickType.StopRun))
                    {
                        CommonClass.killProcessByName(action.Text.Split(',')[1]);
                        currProcess = null;
                    }
                    else if (action.Type.Equals(ClickType.StartRun))
                    {
                        string execCommand = action.Text.Split(',')[1];
                        int extensionPos = execCommand.IndexOf(".exe");
                        currProcess = System.Diagnostics.Process.Start(execCommand.Substring(0, extensionPos + 5), execCommand.Substring(extensionPos + 5));
                    }
                    else if (action.Type.Equals(ClickType.ClossProcess))
                    {
                        if (currProcess != null)
                        {
                            currProcess.Close();
                            currProcess = null;
                        }

                    }
                    else if (action.Type.Equals(ClickType.SendKeys) && action.Text == "screenshot")
                    {
                        savePath = currFilePath + ("A" + channel + @"\");
                        CommonClass.createIfMissing(savePath);
                        saveScreenShot(savePath + DateTime.Now.ToString("yyyyMMddhhmmssss") + ".jpg");
                        channel++;
                    }
                    else if (action.Type.Equals(ClickType.SendKeys))
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(WorkSendKeys), action);
                    }
                    else if (action.Type.Equals(ClickType.Shift))
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(WorkSendWait), action);
                    }
                    else// if (entry is ClickEntry)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(WorkClick), action);
                    }

                    tmpIntervl = action.Interval.Equals(0) ? 0 : action.Interval * 1000 - 100;
                    await ayncWait(tmpIntervl);
                }
            }
            finally
            {
                semaphoreSlim.Release();
                actionRunning = false;
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkEnableButtons), null);
        }

        private void WorkSendKeys(object state)
        {
            this.context.Send(new SendOrPostCallback(delegate (object _state)
            {
                ActionEntry action = state as ActionEntry;
                SendKeys.Send(action.Text);
            }), state);
        }

        //SendKeys.SendWait("^( )");
        private void WorkSendWait(object state)
        {
            this.context.Send(new SendOrPostCallback(delegate (object _state)
            {
                ActionEntry action = state as ActionEntry;
                if (action.Text == "{SHIFT}")
                    SendKeys.SendWait("^( )");
                else if (action.Text == "{CTR-SPACE}")
                    SendKeys.SendWait("^( )");
            }), state);
        }

        private void WorkClick(object state)
        {
            this.context.Send(new SendOrPostCallback(delegate (object _state)
            {
                ActionEntry action = state as ActionEntry;
                SetCursorPos(action.X, action.Y);
                Thread.Sleep(100);
                if (action.Type.Equals(ClickType.click))
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }
                else if (action.Type.Equals(ClickType.doubleClick))
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    Thread.Sleep(100);
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }
                else //if (action.Type.Equals(ClickType.rightClick))
                {
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                }
            }), state);
        }
        private void WorkEnableButtons(object state)
        {
            this.context.Send(new SendOrPostCallback(delegate (object _state)
            {
            }), state);
        }

        //Thread sleep 但不會影響 GUI
        //呼叫方式: private async void btnTaskDelay_Click(object sender, EventArgs e)
        //          {await PutTaskDelay();}
        async Task ayncWait(int milisecs)
        {
            await Task.Delay(milisecs);
        }

        private void saveScreenShot(string filePath)
        {
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(bitmap as Image);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            using (System.IO.MemoryStream oMS = new System.IO.MemoryStream())
            {
                //將oTarImg儲存（指定）到記憶體串流中
                bitmap.Save(oMS, System.Drawing.Imaging.ImageFormat.Jpeg);
                //將串流整個讀到陣列中，寫入某個路徑中的某個檔案裡
                using (System.IO.FileStream oFS = System.IO.File.Open(filePath, System.IO.FileMode.OpenOrCreate))
                { oFS.Write(oMS.ToArray(), 0, oMS.ToArray().Length); }
            }
        }
    }
}
