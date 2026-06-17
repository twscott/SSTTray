using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using FirstOhm;
using System.Globalization;

namespace TaskTrayApplication
{
    public partial class 模擬_IO : Form
    {
        #region Declare
        public bool actionRunning = false;
        bool runForEver = false;
        string currFilePath = null;
        string currApType = null;
        bool autoRun = false;
        Process currProcess = null;
        Dictionary<string, string> aotoExecStrDict = null;

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
            Shift=7 //切換輸入法
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

            //private void WorkSendKeys(object state)
            //{
            //    this.context.Send(new SendOrPostCallback(delegate (object _state)
            //    {
            //        ActionEntry action = state as ActionEntry;
            //        SendKeys.Send(action.Text);
            //    }), state);
            //}
            //private void WorkClick(object state)
            //{
            //    this.context.Send(new SendOrPostCallback(delegate (object _state)
            //    {
            //        ActionEntry action = state as ActionEntry;
            //        SetCursorPos(action.X, action.Y);
            //        Thread.Sleep(100);
            //        if (action.Type.Equals(ClickType.click))
            //        {
            //            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            //            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            //        }
            //        else if (action.Type.Equals(ClickType.doubleClick))
            //        {
            //            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            //            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            //            Thread.Sleep(100);
            //            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            //            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            //        }
            //        else //if (action.Type.Equals(ClickType.rightClick))
            //        {
            //            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            //            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
            //        }
            //    }), state);
            //}

        //    private void Process_KeyPress(char action, string inputStr)
        //    {
        //        if (byTextEntry) return;
        //        //mouse -- c:left click, d:double click, rLright click
        //        //keyboard t:send key
        //        if (action.Equals('c') || action.Equals('d')
        //            || action.Equals('r') || action.Equals('t'))
        //        {
        //            end = DateTime.Now;
        //            if (first)
        //            {
        //                start = end;
        //                first = false;
        //            }

        //            ClickType ct = ClickType.click;
        //            if (action.Equals('c'))
        //            {
        //                //cl = ClickType.click;
        //            }
        //            else if (action.Equals('d'))
        //            {
        //                ct = ClickType.doubleClick;
        //            }
        //            else if (action.Equals('r'))
        //            {
        //                ct = ClickType.rightClick;
        //            }
        //            else //if (action.Equals('t'))
        //            {
        //                ct = ClickType.SendKeys;
        //            }

        //            int x = Cursor.Position.X;
        //            int y = Cursor.Position.Y;
        //            TimeSpan ts = end - start;
        //            double sec = 0;
        //            sec = ts.TotalSeconds;
        //            sec = Math.Round(sec, 1);
        //            start = end;
        //            string point = x.ToString() + "," + y.ToString();

        //            ListViewItem lvi = new ListViewItem(new string[] { point, ct.ToString(), "0", inputStr });
        //            ActionEntry acion = new ActionEntry(x, y, inputStr, 0, ct);
        //            lvi.Tag = acion;
        //        }
        //    }

        }
        #endregion

        #region Construction
        public 模擬_IO()
        {
            InitializeComponent();
            context = SynchronizationContext.Current;
            actions = new List<ActionEntry>();
            schedualeList = new Hashtable();
        }

        public 模擬_IO(string taskName)
        {
            InitializeComponent();
            context = SynchronizationContext.Current;
            actions = new List<ActionEntry>();
            schedualeList = new Hashtable();
            comboTasks.Text = taskName;
            autoRun = true;
        }
        #endregion

        #region Private Methods
        uint threadId = 0;
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint GetCurrentThreadId();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenThread(uint desiredAccess, bool inheritHandle, uint threadId);

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

        //擷取螢幕
        private async void RunActionSH()
        {
            foreach (ActionEntry action in actions)
            {
                if (action.Type.Equals(ClickType.SendKeys))
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(WorkSendKeys), action);
                }
                else// if (entry is ClickEntry)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(WorkClick), action);
                    if (this.currApType == "operate")
                    {
                        saveScreenShot(this.currFilePath + DateTime.Now.ToString("yyyyMMddhhmmssss") + ".jpg");
                    }
                }
                int tmpIntervl = action.Interval.Equals(0) ? 0 : action.Interval * 1000 - 100;
                await ayncWait(tmpIntervl);
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkEnableButtons), null);
        }

        private async void RunAction()
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
                if(action.Text=="{SHIFT}")
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
                enableButtons(true);
            }), state);
        }


        private void OpenFileXml(bool runIt, string file)
        {
            //Get data from XML file
            XmlSerializer ser = new XmlSerializer(typeof(ActionsEntry));
            using (FileStream fs = System.IO.File.Open(file, FileMode.Open))
            {
                try
                {
                    ActionsEntry entry = (ActionsEntry)ser.Deserialize(fs);
                    lvExec.Items.Clear();
                    foreach (ActionsEntryAction ae in entry.Action)
                    {
                        string point = ae.X.ToString() + "," + ae.Y.ToString();
                        string interval = (ae.interval).ToString();
                        ListViewItem lvi = new ListViewItem(new string[] { point, ((ClickType)(ae.Type)).ToString(), interval, ae.Text });
                        ActionEntry acion = new ActionEntry(ae.X, ae.Y, ae.Text, ae.interval, (ClickType)(ae.Type));
                        lvi.Tag = acion;
                        lvExec.Items.Add(lvi);
                    }

                    if (runIt)
                    {
                        btnTestRecord.PerformClick();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Clicer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void ReadXmlFromDB(ListView listView, bool runIt, string taskName)
        {
            //Get data from XML file
            XmlSerializer ser = new XmlSerializer(typeof(ActionsEntry));
            string sqlStr = "SELECT `action`, srcName FROM `autoExecute`  Where `funcName`= '" + taskName + "'";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.ACLConnString);
            string configResult = null;
            DataRow finalDr=null;
            if (dt.Rows.Count <= 0)
                return;
            if (dt.Rows.Count >= 1)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    if(dr["srcName"].ToString()== CommonClass.getComputerName())
                    {
                        finalDr = dr;
                        break;
                    } 
                }
                if (finalDr == null)
                    finalDr = dt.Rows[0];
            }
            configResult = finalDr["action"].ToString();
            //lvActions.Items.Clear();
            if (string.IsNullOrEmpty(configResult))
            {
                CommonClass.writeLog("監控程式", "OpenFileXml", "sysset 查無 " + taskName + " 錄製執行碼");
                return;
            }

            try
            {
                ActionsEntryAction[] entry = JsonConvert.DeserializeObject<ActionsEntryAction[]>(configResult);

                listView.Items.Clear();
                foreach (ActionsEntryAction ae in entry)
                {
                    string point = ae.X.ToString() + "," + ae.Y.ToString();
                    string interval = (ae.interval).ToString();
                    ListViewItem lvi = new ListViewItem(new string[] { point, ((ClickType)(ae.Type)).ToString(), interval, ae.Text });
                    ActionEntry acion = new ActionEntry(ae.X, ae.Y, ae.Text, ae.interval, (ClickType)(ae.Type));
                    lvi.Tag = acion;
                    listView.Items.Add(lvi);
                    Refresh();
                }
                //btnStart.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Clicer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void enableButtons(bool enabel)
        {
            btnSaveRecord.Enabled = enabel;
            lvExec.Enabled = enabel;
        }

        //timeInterval
        //initTime1 : 進入監視器網站時間
        //initTime2 : XXXX 已經不用, 進入後到準備錄製的時間, 
        public async Task Run_Once_Content(模擬_IO copyAction, int timeInterval, int initTime1 = 18000, int initTime2 = 12000)
        {
            if (copyAction == null)
            {
                copyAction = new 模擬_IO();
                copyAction.WindowState = FormWindowState.Minimized;
            }
            do
            {
                await ayncWait(500);
            } while (copyAction.actionRunning);
            CommonClass.killProcess("Sys1");
            await ayncWait(1000);
            MessageBox.Show("執行一次完成!!!", "執行一次完成", MessageBoxButtons.OK);
        }

        //timeInterval
        //initTime1 : 進入監視器網站時間
        //initTime2 : XXXX 已經不用, 進入後到準備錄製的時間, 
        async Task Run_Forever(模擬_IO copyAction, int timeInterval, int initTime1 = 18000)
        {
            long startMilliseconds;
            long endMilliseconds;
            int waitTime = 0;
            if (copyAction == null)
            {
                copyAction = new 模擬_IO();
                copyAction.WindowState = FormWindowState.Minimized;
            }
            runForEver = true;
            while (runForEver)
            {
                startMilliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                await ayncWait(1000);
                if (!runForEver)
                    break;
            }
            MessageBox.Show("停止自動存檔!!!", "自動存檔自動存檔", MessageBoxButtons.OK);
        }
        public async void runOperate(ListView listView, string channel, string optype, int execSecs)
        {
            if (listView.Items.Count == 0)
                return;
            enableButtons(false);
            CommonClass.createIfMissing(currFilePath);
            if (runActionThread == null || !runActionThread.IsAlive)
            {
                actions.Clear();
                foreach (ListViewItem lvi in listView.Items)
                {
                    actions.Add(lvi.Tag as ActionEntry);
                }
                runActionThread = new Thread(RunAction);
                runActionThread.Start();
            }
            currApType = null;
        }
        private string getSysName()
        {
            return txtTaskName.Text;
        }
        //clickTypeText 代表 要執行 Function， 而非一般鍵盤/滑鼠 IO
        //例如： add_lvActions_Item("@", 0, 0, 2, "killProcess,Microsoft Excel")
        private void add_lvActions_Item(ListView listView, ClickType clickType, int x_pos, int y_pos, decimal waitSec, string inputText, string specialCommand = null)
        {
            int sec;
            ListViewItem lvi = new ListViewItem(new string[] { x_pos + "," + y_pos, specialCommand, waitSec.ToString(), inputText });
            ActionEntry acion = new ActionEntry(x_pos, y_pos, inputText, (int)waitSec, clickType);
            lvi.Tag = acion;
            listView.Items.Add(lvi);
            int index = listView.Items.Count;
            if (waitSec <= 0)
                sec = 2;//預設秒數
            else
                sec = (int)waitSec;
        }
        #endregion

        #region window_events
        IO_Simulation ioSim = new IO_Simulation();
        private void 模擬_IO_Load(object sender, EventArgs e)
        {

            this.BringToFront();
            this.KeyPreview = true;
            this.Focus();
            if(autoRun)
            {
                btnLoadFromDB.PerformClick();
                CommonClass.wait(2);
                if (lvExec.Items.Count >= 1)
                    btnRunOnce.PerformClick();
                autoRun = false;
                this.Close();
            } else
            {
                btnRefreshCombo.PerformClick();
            }
        }

        private void 模擬_IO_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (runActionThread != null && runActionThread.IsAlive)
            {
                runActionThread.Abort();
            }
        }

        private void 模擬_IO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (byTextEntry) return;

            if (e.KeyChar.Equals('k'))
            {
                foreach (ListViewItem litem in lvActions.SelectedItems)
                    litem.Remove();
                return;
            }

            //if (e.KeyChar.Equals('h'))
            //{
            //    add_lvActions_Item(lvActions, ClickType.Shift, 0, 0, txtStartProcessWaitSecs.Value, "+");
            //}
            if (e.KeyChar.Equals('c') || e.KeyChar.Equals('d') || e.KeyChar.Equals('s')
                || e.KeyChar.Equals('r') || e.KeyChar.Equals('t') || e.KeyChar.Equals('e')
                || e.KeyChar.Equals('1') || e.KeyChar.Equals('2'))
            {
                end = DateTime.Now;
                if (first)
                {
                    start = end;
                    first = false;
                }

                ClickType ct = ClickType.click;
                if (e.KeyChar.Equals('c'))
                {
                    //ct = ClickType.click;
                }
                else if (e.KeyChar.Equals('d'))
                {
                    ct = ClickType.doubleClick;
                }
                else if (e.KeyChar.Equals('r'))
                {
                    ct = ClickType.rightClick;
                }
                else //if (e.KeyChar.Equals('t'))
                {
                    ct = ClickType.SendKeys;
                }


                int x = Cursor.Position.X;
                int y = Cursor.Position.Y;
                TimeSpan ts = end - start;
                double sec = 0;
                if (nWait.Value.Equals(0))
                {
                    sec = ts.TotalSeconds;
                    sec = Math.Round(sec, 1);
                }
                else
                {
                    sec = (double)nWait.Value;
                }
                start = end;
                string point = x.ToString() + "," + y.ToString();
                //Key Defind
                //https://www.autoitscript.com/autoit3/docs/functions/Send.htm 
                string text = ct.Equals(ClickType.SendKeys) ? txbEntry.Text : string.Empty;
                if (e.KeyChar.Equals('e'))
                    text = "{ESC}";
                else if (e.KeyChar.Equals('h'))
                    text = "^( )";
                else if (e.KeyChar.Equals('1'))
                    text = "{F11}";
                else if (e.KeyChar.Equals('2'))
                    text = "{F12}";
                else if (e.KeyChar.Equals('s'))
                    text = "screenshot";
                ListViewItem lvi = new ListViewItem(new string[] { point, ct.ToString(), "0", text });
                ActionEntry acion = new ActionEntry(x, y, text, 0, ct);
                lvi.Tag = acion;
                lvActions.Items.Add(lvi);
                int index = lvActions.Items.Count;

                lvActions.Items[index - 1].SubItems[2].Text = sec.ToString();
                (lvActions.Items[index - 1].Tag as ActionEntry).Interval = (int)sec;
            }
        }



        //自動執行一次 按鈕
        async void BtnStartFromDB_Click(object sender, EventArgs e)
        {
            模擬_IO testAction = null;
            System.GC.Collect();
            Run_Once_Content(testAction, timeInterval, this.initTime1);
            MessageBox.Show("執行一次完成!!!", "執行一次自動存檔", MessageBoxButtons.OK);
        }

        private void btnStartProcess_Click(object sender, EventArgs e)
        {
            CommonClass.killProcessByName(textProcessEnd.Text);
            this.currProcess = null;

            //add_lvActions_Item(listView, clickTypeText, x_pos, y_pos, waitSec, inputText, specialCommand = null)
            add_lvActions_Item(lvActions, ClickType.StopRun, 0, 0, nWait.Value, "killProcess," + textProcessEnd.Text, "StopRun");
            Refresh();
            Process currProcess = System.Diagnostics.Process.Start(txtProcessCommand.Text, txtProcessParams.Text);
            add_lvActions_Item(lvActions, ClickType.StartRun, 0, 0, txtStartProcessWaitSecs.Value, "startProcess," + txtProcessCommand.Text.Replace(@"\", "/") + " " + txtProcessParams.Text, "StartRun");
            txtTaskName.Text = "";
        }

        private void lvActions_MouseDown(object sender, MouseEventArgs e)
        {
            int coutselect = lvActions.SelectedItems.Count;
            deleteToolStripMenuItem.Available = coutselect > 0;
            editToolStripMenuItem.Available = coutselect == 1;
        }


        private void btnSaveRecord_Click(object sender, EventArgs e)
        {
            if (txtTaskName.Text == "")
            {
                MessageBox.Show("自動執行名稱 不可以是空白");
                txtTaskName.Focus();
                return;
            }
            XmlSerializer ser = new XmlSerializer(typeof(ActionsEntry));
            ActionsEntry tmpAction = new ActionsEntry();
            List<ActionsEntryAction> tmpActionsEntryActions = new List<ActionsEntryAction>();
            foreach (ListViewItem lvi in lvActions.Items)
            {
                ActionEntry tmpActionEntry = lvi.Tag as ActionEntry;
                ActionsEntryAction tmpActionsEntryAction = new ActionsEntryAction();
                tmpActionsEntryAction.X = tmpActionEntry.X;
                tmpActionsEntryAction.Y = tmpActionEntry.Y;
                tmpActionsEntryAction.Text = tmpActionEntry.Text;
                tmpActionsEntryAction.interval = tmpActionEntry.Interval;
                tmpActionsEntryAction.Type = (int)tmpActionEntry.Type;
                tmpActionsEntryActions.Add(tmpActionsEntryAction);
            }
            string output = JsonConvert.SerializeObject(tmpActionsEntryActions);
            string srvSys = getSysName();
            //string sqlStr = "Insert into `sv_config` (`sysset`,`svtype`,`action`) " +
            //    "Values('" + srvSys + "', '" + comboBeforeStart.Text + "', '" + output + "')  ON DUPLICATE KEY UPDATE" +
            //    "`sysset`='" + srvSys + "',`svtype` = '" + comboBeforeStart.Text + "',`action` = '" + output + "'";
            string sqlStr = "insert into  `autoExecute` (`srcName`,`funcName`,`funcParams`,`action`,`modified`,`remark`) " +
                            "Values('" + CommonClass.getComputerName() + "', '" + txtTaskName.Text + "', '" +
                            txtProcessParams.Text + "', '" + output + "', CURRENT_DATE, + '" + txtNote.Text + "')  " +
                            "ON DUPLICATE KEY UPDATE" +
                            "`srcName`='" + CommonClass.getComputerName() + "',`funcName` = '" + txtTaskName.Text + "'," +
                            "`action` = '" + output + "',modified=CURRENT_DATE, remark='" + txtNote.Text + "'";
            CommonClass.execSQLNonQuery(sqlStr, Constants.ACLConnString);

        }

        private void btnClearRecord_Click(object sender, EventArgs e)
        {
            if (runActionThread != null && runActionThread.IsAlive)
            {
                runActionThread.Abort();
                enableButtons(true);
                for (int i = lvActions.Items.Count - 1; i >= 0; i--)
                    lvActions.Items[i].Remove();
            }
        }

        private void btnTestRecord_Click(object sender, EventArgs e)
        {
            enableButtons(false);
            //currProcess = getSysName();
            if (runActionThread == null || !runActionThread.IsAlive)
            {
                actions.Clear();
                foreach (ListViewItem lvi in lvActions.Items)
                {
                    actions.Add(lvi.Tag as ActionEntry);
                }
                runActionThread = new Thread(RunAction);
                runActionThread.Start();
            }
        }

        private void btnLoadFromDBRecording_Click(object sender, EventArgs e)
        {
            if (txtTaskName.Text == "")
            {
                MessageBox.Show("請輸入執行工作名稱");
                return;
            }

            ReadXmlFromDB(lvActions, true, txtTaskName.Text);
        }

        private void btnStoRecording_Click(object sender, EventArgs e)
        {
            //CommonClass.killProcessByName(textProcessEnd.Text);
            if (currProcess != null)
            {
                currProcess.Close();
                currProcess = null;
            }
            add_lvActions_Item(lvActions, ClickType.ClossProcess, 0, 0, nWait.Value, "closeProcess," + textProcessEnd.Text, "CloseProcess");
        }

        private void txbEntry_Enter(object sender, EventArgs e)
        {
            byTextEntry = true;
        }

        private void txbEntry_Leave(object sender, EventArgs e)
        {
            byTextEntry = false;
        }

        private void updateListContent()
        {
            try
            {
                ListViewItem lvi = lvActions.SelectedItems[0];
                ActionEntry ae = (ActionEntry)lvi.Tag;
                string iniTxt = "x>" + ae.X + ";y>" + ae.Y + ";wait>" + ae.Interval + ";type>" + ae.Type + ";text>" + ae.Text;
                string newText = CommonClass.showPrompt("修改鍵入內容", "請依照原來的內容格式修訂", iniTxt);
                if (string.IsNullOrEmpty(newText))
                    return;
                string[] tagArr = newText.Split(';');
                ActionEntry acion = new ActionEntry(int.Parse(tagArr[0].Split('>')[1]), int.Parse(tagArr[1].Split('>')[1]),
                    tagArr[4].Split('>')[1], int.Parse(tagArr[2].Split('>')[1]), ae.Type);
                lvi = new ListViewItem(new string[] { tagArr[0].Split('>')[1] + "," + tagArr[1].Split('>')[1],
                tagArr[3].Split('>')[1], tagArr[2].Split('>')[1], tagArr[4].Split('>')[1] });
                lvi.Tag = acion;
                int selectedIdx = lvActions.SelectedItems[0].Index;
                lvActions.Items.Insert(selectedIdx, lvi);
            } catch(Exception ex)
            {
                MessageBox.Show("這裡僅能執行， 無法更改！！！");
            }

        }

        private void lvActions_DoubleClick(object sender, EventArgs e)
        {
            updateListContent();
        }

        public void btnLoadFromDB_Click(object sender, EventArgs e)
        {
            ReadXmlFromDB(lvExec, true, comboTasks.Text);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (runActionThread != null && runActionThread.IsAlive)
            {
                runActionThread.Abort();
                enableButtons(true);
                for (int i = lvExec.Items.Count - 1; i >= 0; i--)
                    lvExec.Items[i].Remove();
            }
        }

        private void txtTaskName_Enter(object sender, EventArgs e)
        {
            byTextEntry = true;
        }

        private void txtNote_Enter(object sender, EventArgs e)
        {
            byTextEntry = true;
        }

        private void txtTaskName_Leave(object sender, EventArgs e)
        {
            byTextEntry = false;
        }

        private void txtNote_Leave(object sender, EventArgs e)
        {
            byTextEntry = false;
        }

        private void tabControl1_Enter(object sender, EventArgs e)
        {

        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {
            byTextEntry = false;
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            byTextEntry = true;
        }

        private void btnSimTimer_Click(object sender, EventArgs e)
        {
            enableButtons(false);
            ioSim.ReadXmlFromDB(comboTasks.Text);
            enableButtons(true);
        }

        public void runOnce()
        {
            enableButtons(false);
            //currProcess = getSysName();
            if (runActionThread == null || !runActionThread.IsAlive)
            {
                actions.Clear();
                foreach (ListViewItem lvi in lvExec.Items)
                {
                    actions.Add(lvi.Tag as ActionEntry);
                }
                runActionThread = new Thread(RunAction);
                runActionThread.Start();
            }
            enableButtons(true);
        }

        public void btnRunOnce_Click(object sender, EventArgs e)
        {
            runOnce();
        }

        private async void btnRunTimes_Click(object sender, EventArgs e)
        {
            int miSecs = 3000;
            string abc;
            foreach(ListViewItem lvi in lvExec.Items)
            {
                miSecs += ((TaskTrayApplication.模擬_IO.ActionEntry)lvi.Tag).interval * 1000;
            }

            for (int i = 0; i < nRunTimes.Value; i++)
            {
                btnRunOnce.PerformClick();
                Thread.Sleep(miSecs);
            }

        }

        private void btnRefreshCombo_Click(object sender, EventArgs e)
        {
            string sqlStr = "SELECT DISTINCT `funcName` as displayColumn, " +
                " `funcName` as valueColumn " +
                " FROM `autoExecute`";
            if (chkboxIfSameComputer.Checked)
                sqlStr += " where `srcName` = '" + CommonClass.getComputerName() + "'";
            UI_CommonClass.fillInCombo(comboTasks, sqlStr, false, Constants.ACLConnString);
        }

        private void comboTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboTasks.Text != "")
                btnLoadFromDB.PerformClick();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem litem in lvActions.SelectedItems)
                litem.Remove();
            return;
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateListContent();
        }

        public  Dictionary<string, string> setScheduleContent(string weekName)
        {
            string propertyName = weekName + "AutoExec";
            string propertyContent =  Constants.getProperty(propertyName, "");
            
            if (!string.IsNullOrEmpty(propertyContent) && txtAutoIOScedule.Text != propertyContent)
            {
                txtAutoIOScedule.Text = propertyContent;
                aotoExecStrDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(propertyContent);
            }
            return aotoExecStrDict;
        }

        private void setAutoIOScedule(object sender, EventArgs e)
        {
            setScheduleContent(((RadioButton)sender).Name.Substring(0, 6));
        }

        private void btnTimerOn_Click(object sender, EventArgs e)
        {
            DateTimeFormatInfo dateTimeFormats = new CultureInfo("en-US").DateTimeFormat;
            string theWeekDay = (DateTime.Now.ToString("dddd", dateTimeFormats)).Substring(0, 3);
            setScheduleContent(theWeekDay);
            Refresh();
            if (txtAutoIOScedule.Text == "")
                return;

            timerIO.Interval = 60000; //1 分鐘執行一次
            timerIO.Start();
        }

        private void btnTimerOFF_Click(object sender, EventArgs e)
        {
            timerIO.Stop();
        }

        private void timerIO_Tick(object sender, EventArgs e)
        {
            string currHour = DateTime.Now.Hour.ToString();
            string currMin = DateTime.Now.Minute.ToString();
            foreach (KeyValuePair<string, string> autoItem in aotoExecStrDict)
            {
                string autoHour = autoItem.Key.Split(':')[0];
                string autoMin = autoItem.Key.Split(':')[1];
                if (autoHour == currHour && autoMin == currMin)
                {
                    switch (autoItem.Value)
                    {
                        case "shutdown":
                            CommonClass.Shutdown();
                            break;
                        default:
                            comboTasks.Text = autoItem.Value;
                            ReadXmlFromDB(lvExec, true, autoItem.Value);
                            Refresh();
                            CommonClass.wait(2);
                            runOnce();
                            break;
                    }
                }
            }
        }

        private void txbEntry_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals((Char)Keys.Escape))//Esc
            {
                nWait.Focus();
            }
        }
        #endregion
    }
}
