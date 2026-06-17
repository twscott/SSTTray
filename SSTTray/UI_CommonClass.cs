using FirstOhm;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace FirstOhm
{
    class UI_CommonClass
    {

        #region Combo
        //輸入 query 指令， function 自動將資料填入 Combo 中
        //select 指令 如： SELECT DISTINCT `SIZE` as displayColumn, `SIZE` as valueColumn FROM `MFO_ATTRIBUTE` 
        //firstBlankItem: 是否插入空白欄
        public static int fillInCombo(ComboBox cmbTripName, String qryStr, bool firstBlankItem = false, String ConnString = Constants.ConnString, String defaultVal = null)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnString))
            {
                try
                {
                    conn.Open();
                    MySqlDataAdapter da = new MySqlDataAdapter(qryStr, conn);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "FIRSTOHM");
                    DataTable firstTable = ds.Tables["FIRSTOHM"];
                    return fillInCombo(cmbTripName, firstTable, firstBlankItem, ConnString, defaultVal);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "fillInCombo");
                    return 0;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        public static int fillInCombo(ComboBox cmbTripName, DataTable firstTable, bool firstBlankItem = false, String ConnString = Constants.ConnString, String defaultVal = null)
        {
            if (firstTable.Rows.Count == 0)
            {
                //MessageBox.Show("經查詢無資料!!!");
            }
            if (firstBlankItem)
            {
                DataRow row = firstTable.NewRow();
                row["displayColumn"] = "";
                row["valueColumn"] = "";
                firstTable.Rows.InsertAt(row, 0);
            }

            cmbTripName.DisplayMember = "displayColumn";

            cmbTripName.ValueMember = "valueColumn";
            cmbTripName.DataSource = firstTable;
            if (!string.IsNullOrEmpty(defaultVal))
                cmbTripName.Text = defaultVal;
            return firstTable.Rows.Count;
        }

        //在Grid 增加一個 Checkbox 的欄位
        public static DataGridView addChkbox2Dgv(DataGridView dgv1, string ColName, int colIdx = 0)
        {
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.Name = ColName;
            checkColumn.HeaderText = ColName;
            checkColumn.Width = 50;
            checkColumn.ReadOnly = false;
            checkColumn.FillWeight = 10; //if the datagridview is resized (on form resize) the checkbox won't take up too much; value is relative to the other columns' fill values
            //dgv1.Columns.Add(checkColumn);
            dgv1.Columns.Insert(colIdx, checkColumn);
            return dgv1;
        }

        //設定DataGridView顯示多列
        public static void setDgvDisplayMultiLine(DataGridView dgv)
        {
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
        }

        //dictionary value-combo display, key- combo value
        public static void fillInCombo(ComboBox cmbTripName, Dictionary<string, string> itemDictionary, string defaultValue = null)
        {
            cmbTripName.DataSource = new BindingSource(itemDictionary, null);
            cmbTripName.DisplayMember = "Value";
            cmbTripName.ValueMember = "Key";
            if (defaultValue != null)
                cmbTripName.Text = defaultValue;
        }

        public static void fillInComboByDict(ComboBox cmbTripName, Dictionary<string, string> itemDictionary, string defaultValue = null, bool firstBlankItem = false)
        {
            cmbTripName.DataSource = null;
            cmbTripName.DataSource = new BindingSource(itemDictionary, null);
            cmbTripName.DisplayMember = "Key";
            cmbTripName.ValueMember = "Value";
            if (firstBlankItem && !itemDictionary.Keys.Contains(""))
                itemDictionary.Add("", "");
            if (defaultValue != null)
                cmbTripName.Text = defaultValue;
            if (!firstBlankItem && itemDictionary.Count > 0)
                cmbTripName.Text = itemDictionary.ElementAt(0).Key;
        }

        public static void fillInCombo(ComboBox cmbTripName, List<string> items, bool firstBlankItem = false)
        {
            if (firstBlankItem)
                items.Insert(0, "");
            cmbTripName.DataSource = items;
        }


        public static void fillInCombo2Lists(ComboBox cmbTripName, List<string> items1, List<string> items2, bool firstBlankItem = false)
        {
            List<string> itemList = new List<string>();
            if (items1 != null)
                foreach (string item in items1)
                    itemList.Add(item);
            if (items2 != null)
                foreach (string item in items2)
                    itemList.Add(item);
            cmbTripName.DataSource = null;

            if (firstBlankItem)
                itemList.Insert(0, "");
            cmbTripName.DataSource = itemList;
        }
        #endregion //Combo

        #region Grid

        //用現成的 datatable, 更改 dgv 顯示的內容
        //filterStr 例如   "尺寸='0.8x1.9'", sortStr 例如  尺寸 DESC 
        public static void refillDgv(DataTable dt, DataGridView dgv, string filterStr, string sortStr = null)
        {
            if (dt == null || dt.Rows.Count < 0)
                return;
            if (string.IsNullOrEmpty(filterStr))
                return;
            //DataTable dtTemp = CommonClass.DataTableFilterSort2(dt, "尺寸='" + comboSize.Text + "'", "尺寸 DESC");
            dgv.DataSource = null;
            DataTable dtTemp = CommonClass.DataTableFilterSort2(dt, filterStr, sortStr);
            UI_CommonClass.dtTodgv(dgv, dtTemp);
        }

        public static bool listToDgv(DataGridView rtnDgv, List<string> dataList, List<string> colList = null, int colNum = 10)
        {
            int i = 0, rowIndex = 0;
            //Process DataGridView Columns
            if (colList != null)
            {
                foreach (string colItem in colList)
                {
                    rtnDgv.Columns.Add("Column", colItem);
                }
            }
            else
            {
                for (i = 0; i < colNum; i++)
                    rtnDgv.Columns.Add("Column", "Col" + i);
            }
            if (rtnDgv.ColumnCount <= 0)
            {
                return false;
            }
            //Write data
            rtnDgv.Rows.Clear();
            foreach (string dataItem in dataList)
            {
                if (i == 0)
                    rowIndex = rtnDgv.Rows.Add();
                rtnDgv.Rows[rowIndex].Cells[i++].Value = dataItem;
                if (i == rtnDgv.ColumnCount)
                    i = 0;
            }
            return true;
        }

        //取得 grid checkbox 有 選取的 Rows
        public static IEnumerable<DataGridViewRow> getDgvChkedRows(DataGridView dgv1)
        {
            List<String> olist = new List<String>();
            IEnumerable<DataGridViewRow> checkedRows = from DataGridViewRow r in dgv1.Rows
                                                       where Convert.ToBoolean(r.Cells[0].Value) == true
                                                       select r;
            return checkedRows;
        }

        public class DataGridViewSelectAllColumnHeaderCell : DataGridViewColumnHeaderCell
        {
            internal static int GetPredictWidth(string text, Font font, Padding padding)
                => TextRenderer.MeasureText("　　" + text, font).Width + padding.Left + padding.Right;
            const int MinPaddingLeft = 5;
            public event EventHandler OnClickEvent;
            public bool Checked { get; set; }
            protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
                DataGridViewElementStates dataGridViewElementState, object value, object formattedValue, string errorText,
                DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
            {
                var strHeadetText = "　" + (formattedValue as string ?? "");
                var sizeText = TextRenderer.MeasureText(strHeadetText, cellStyle.Font);

                var intPaddingLeft = cellStyle.Padding.Left < MinPaddingLeft
                    ? MinPaddingLeft
                    : cellStyle.Padding.Left;

                var locationCheckBox = new Point(
                        (cellBounds.X + intPaddingLeft),
                        cellBounds.Y + (int)Math.Ceiling((double)(cellBounds.Height - sizeText.Height) / 2)
                    );
                var predictWidth = GetPredictWidth(formattedValue as string ?? "", cellStyle.Font, cellStyle.Padding);
                if (OwningColumn.Width != predictWidth)
                {
                    OwningColumn.Width = predictWidth;
                }
                else
                {
                    base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, strHeadetText, strHeadetText, errorText, cellStyle, advancedBorderStyle, paintParts);
                    var checkboxRectangle = new Rectangle(locationCheckBox, new Size(sizeText.Height, sizeText.Height));
                    ControlPaint.DrawCheckBox(graphics, checkboxRectangle, Checked ? ButtonState.Flat | ButtonState.Checked : ButtonState.Flat);
                }
            }
            protected override void OnClick(DataGridViewCellEventArgs e)
            {
                Checked = (!Checked);
                OnClickEvent?.Invoke(this, EventArgs.Empty);
                base.OnClick(e);
            }
            public void PerformClick()
            {
                OnClick(new DataGridViewCellEventArgs(ColumnIndex, RowIndex));
            }
        }

        //儲存訂單新增修改狀態至MFO_MODIFIED TABLE
        public void saveMfoModified(List<string> MfoModifiedData)
        {
            try
            {
                using (MySqlConnection cnSQL = new MySqlConnection(Constants.ConnString))
                {
                    cnSQL.Open();
                    MySqlCommand cmdSQL = new MySqlCommand();
                    cmdSQL.Connection = cnSQL;
                    cmdSQL.CommandText = "INSERT INTO MFO_MODIFIED " +
                        " (ID, CUST, ONAB, ODATE, DELDATE, RTYPE, VAL, QUAN, STATUS, MFONO)" + //MFO_MODIFIED加入MFONO欄位，Added by Michael, 2020.12.3 //ID, CUST, ONAB, ODATE, DELDATE, RTYPE, VAL, QUAN, STATUS
                        " VALUES (";
                    foreach (string Data in MfoModifiedData)
                        cmdSQL.CommandText += "'" + Data + "', ";

                    cmdSQL.CommandText = cmdSQL.CommandText.Substring(0, cmdSQL.CommandText.Length - 2) + ");";

                    //執行SQL指令
                    int iCount = cmdSQL.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "saveMfoModified");
            }
        }

        public class DataGridViewSelectAllColumn : DataGridViewCheckBoxColumn
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public new DataGridViewSelectAllColumnHeaderCell HeaderCell
            {
                //get => base.HeaderCell as DataGridViewSelectAllColumnHeaderCell;
                //set => base.HeaderCell = value;
                get { return base.HeaderCell as DataGridViewSelectAllColumnHeaderCell; }
                set { base.HeaderCell = value; }
            }

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            [Browsable(false)]
            public new DataGridViewColumnSortMode SortMode { get; } = DataGridViewColumnSortMode.NotSortable;

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            [Browsable(false)]
            public new DataGridViewAutoSizeColumnMode AutoSizeMode { get; } = DataGridViewAutoSizeColumnMode.None;

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            [Browsable(false)]
            public new DataGridViewTriState Resizable { get; } = DataGridViewTriState.False;

            public DataGridViewSelectAllColumn() : base()
            {
                this.DefaultHeaderCellType = typeof(DataGridViewSelectAllColumnHeaderCell);
                base.SortMode = this.SortMode;
                base.AutoSizeMode = this.AutoSizeMode;
                base.Resizable = this.Resizable;
                HeaderCell.OnClickEvent += Header_OnClickEvent;
            }

            protected override void OnDataGridViewChanged()
            {
                if (DataGridView != null)
                {
                    DataGridView.CellContentClick += DataGridView_CellContentClick;
                }
                base.OnDataGridViewChanged();
            }

            private void DataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
            {
                DataGridView dgv = sender as DataGridView;
                if (e.ColumnIndex == HeaderCell.ColumnIndex && e.RowIndex >= 0)
                {
                    HeaderCell.Checked = false;
                    dgv.EndEdit();
                    dgv.Invalidate();
                    //DataGridView.EndEdit();
                    //DataGridView.Invalidate();
                }
            }

            /// <summary>
            /// 點選狀態改變
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void Header_OnClickEvent(object sender, EventArgs e)
            {
                DataGridView.EndEdit();
                foreach (DataGridViewRow row in this.DataGridView.Rows)
                {
                    var cell = row.Cells[Index] as DataGridViewCheckBoxCell;
                    if (row.DataBoundItem != null)
                        cell.Value = HeaderCell.Checked;
                }
                DataGridView.RefreshEdit();
            }
        }

        //設定文字方塊自動完成功能，Added by Michael, 2020.5.5
        public void addControlAutoComp(DataTable addItems, TextBox TB)
        {
            AutoCompleteStringCollection autoItems = new AutoCompleteStringCollection();
            for (int i = 0; i < addItems.Rows.Count; i++)
                autoItems.Add(addItems.Rows[i][0].ToString());
            TB.AutoCompleteSource = AutoCompleteSource.CustomSource;
            TB.AutoCompleteMode = AutoCompleteMode.Suggest;
            TB.AutoCompleteCustomSource = autoItems;
        }

        //查詢單筆字串，無資料則傳回字串"0", by Michael
        public string GetString(String sqlCommand, String ConnString = Constants.ConnString)
        {
            string rtnString = "0";

            MySqlConnection con = null;
            try
            {
                con = new MySqlConnection(ConnString);
                con.Open(); //open the connection        
                //String cmdText = sqlCommand;
                MySqlCommand cmd = new MySqlCommand(sqlCommand, con);
                //cmd.Prepare();
                MySqlDataReader reader = cmd.ExecuteReader(); //execute the mysql command

                if (reader != null && reader.Read())
                {
                    //rtnString = reader.GetString(0);
                    rtnString = reader.IsDBNull(0) ? "0" : reader.GetString(0);
                }
            }
            catch (MySqlException err)
            {
                String outp = err.ToString();
                rtnString = err.Number + ", Error: " + err.ToString();
            }
            finally
            {
                if (con != null)
                {
                    con.Close(); //close the connection
                }
            } //remember to close the connection after accessing the database

            return rtnString;
        }

        public static bool dictToDgv(DataGridView rtnDgv, Dictionary<string, string> dataDict, List<string> colList = null, int colNum = 10, List<Color> cellCol = null)
        {
            int i = 0, rowIndex = 0;
            //Process DataGridView Columns
            if (rtnDgv.Columns.Count <= 0)
            {
                if (colList != null)
                {
                    foreach (string colItem in colList)
                    {
                        rtnDgv.Columns.Add("Column", colItem);
                    }
                }
                else
                {
                    for (i = 1; i < colNum + 1; i++)
                        rtnDgv.Columns.Add("Column", "Col" + i);
                }
            }

            //Write data
            rtnDgv.Rows.Clear();
            i = 0;
            rowIndex = rtnDgv.Rows.Add();
            rtnDgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            foreach (KeyValuePair<string, string> item in dataDict)
            {
                //if (i == 0)

                rtnDgv.Rows[rowIndex].Cells[i].Value = item.Key;

                if (item.Value == "1")
                    rtnDgv.Rows[rowIndex].Cells[i++].Style.BackColor = Color.Green;
                else
                    rtnDgv.Rows[rowIndex].Cells[i++].Style.BackColor = Color.Red;

                if (i == rtnDgv.ColumnCount)
                {
                    rowIndex = rtnDgv.Rows.Add();
                    i = 0;
                }
            }
            return true;
        }

        public static ArrayList dgvColToArray(DataGridView dgv2, int colIdx)
        {
            ArrayList rtnArray = new ArrayList();
            foreach (DataGridViewRow row in dgv2.Rows)
            {
                rtnArray.Add(row.Cells[colIdx].Value.ToString().Trim());
            }
            return rtnArray;
        }

        //訂單形成前暫存資料區
        public void IPINTO1(string IPAddress)
        {
            string IP = "IPINT";
            string A = IPAddress;
            StringBuilder sb = new StringBuilder(A);
            char a0 = sb[0];
            char a1 = sb[1];
            char a2 = sb[2];
            char a3 = sb[3];
            char a4 = sb[4];
            char a5 = sb[5];
            char a6 = sb[6];
            char a7 = sb[7];
            char a8 = sb[8];
            char a9 = sb[9];
            char a10 = sb[10];
            char a11 = sb[11];
            try
            {
                CommonClass.dropTable(IP + a10 + a11);

                string selectCmd = "CREATE TABLE " + IP + a10 + a11 + " (ONAB varchar(30),CUST varchar(10),PNAB varchar(30),ODATE varchar(10),DELDATE varchar(10)," +
                    "RTYPE varchar(10),WATT varchar(10),VAL varchar(10),TOLER varchar(5),PPM varchar(5),ONECASE varchar(5),QUAN double,UP double,CU varchar(10),PA varchar(10)," +
                    "PAY varchar(10),SHIPMENT varchar(10),ETAETD int(2),ScheduleNO varchar(10),CARSIZE varchar(10),INCASE varchar(50),BOX varchar(10)," +
                    "RWT varchar(10),REWT varchar(10),INWT varchar(10),OUTWT varchar(10),VALS double,SEQ varchar(3),PONAB varchar(30))";//, FINDATE varchar(10)
                MySqlConnection conn = new MySqlConnection(Constants.ConnString); //ConnString
                conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(selectCmd, conn);
                DataSet ds = new DataSet();
                da.Fill(ds, "PACK_DE2_1");

                //GlobalDs = ds;
                //dataGridView1.DataSource = ds.Tables["PACK_DE2_1"];
                conn.Close();
                da.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "AA");
            }
        }


        public void IPISSUE1(string IPAddress)
        {
            string IP = "IPISS1";
            string A = IPAddress;
            StringBuilder sb = new StringBuilder(A);
            char a0 = sb[0];
            char a1 = sb[1];
            char a2 = sb[2];
            char a3 = sb[3];
            char a4 = sb[4];
            char a5 = sb[5];
            char a6 = sb[6];
            char a7 = sb[7];
            char a8 = sb[8];
            char a9 = sb[9];
            char a10 = sb[10];
            char a11 = sb[11];
            try
            {
                //string ConnString;
                //ConnString = "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm;port=3306;charset=utf8;";
                CommonClass.dropTable(IP + a10 + a11);

                string selectCmd = "CREATE TABLE " + IP + a10 + a11 + "(WATT varchar(10),RTYPE varchar(50),TOLER varchar(5),ONECASE varchar(10),PA varchar(10),CUST varchar(10)" +
                    ",SHIPMENT varchar(10),ODATE varchar(10),DELDATE varchar(10),PONAB varchar(30),PPM varchar(50),REMARK varchar(10),ETAETD int(2),CARSIZE varchar(20)" +//,FINDATE varchar(10)
                    ",PRIMARY KEY (WATT,RTYPE,TOLER,ONECASE,CUST,DELDATE,PONAB,PPM))";
                //",PRIMARY KEY (WATT,RTYPE,TOLER,ONECASE,CUST,DELDATE,PONAB,PPM,FINDATE))";
                //",PRIMARY KEY (WATT,RTYPE,TOLER,ONECASE,CUST,DELDATE,PONAB,PPM,ETAETD))"; //匯出工令單無法分辨同張訂單但工令單號不同，故加ETAETD欄位作PK, Modified by Michael, 2019.1.30
                MySqlConnection conn = new MySqlConnection(Constants.ConnString); //ConnString
                conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(selectCmd, conn);
                DataSet ds = new DataSet();
                da.Fill(ds, "PACK_DE2_1");

                //GlobalDs = ds;
                //dataGridView1.DataSource = ds.Tables["PACK_DE2_1"];
                conn.Close();
                da.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "IPISS1");
            }
        }
        public void IPISSUE2(string IPAddress)
        {
            string IP = "IPISS2";
            string A = IPAddress;
            StringBuilder sb = new StringBuilder(A);
            char a0 = sb[0];
            char a1 = sb[1];
            char a2 = sb[2];
            char a3 = sb[3];
            char a4 = sb[4];
            char a5 = sb[5];
            char a6 = sb[6];
            char a7 = sb[7];
            char a8 = sb[8];
            char a9 = sb[9];
            char a10 = sb[10];
            char a11 = sb[11];
            try
            {
                //string ConnString;
                //ConnString = "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm;port=3306;charset=utf8;";
                CommonClass.dropTable(IP + a10 + a11);

                string selectCmd = "CREATE TABLE " + IP + a10 + a11 + "(CUST varchar(10),PONAB varchar(30),RTYPE varchar(10),VAL varchar(10),WATT varchar(10),TOLER varchar(5)," +
                "PPM varchar(5),QUAN varchar(10),DELDATE varchar(10),ONECASE varchar(10),VALS double)";//,FINDATE varchar(10)
                MySqlConnection conn = new MySqlConnection(Constants.ConnString); //ConnString
                conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(selectCmd, conn);
                DataSet ds = new DataSet();
                da.Fill(ds, "PACK_DE2_1");

                //GlobalDs = ds;
                //dataGridView1.DataSource = ds.Tables["PACK_DE2_1"];
                conn.Close();
                da.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "IPISSUE2");
            }
        }

        //DataGridView 輸出 Excel
        public static void dgvToExcel(DataGridView dgv)
        {
            copyAlltoClipboard(dgv);
            Microsoft.Office.Interop.Excel.Application xlexcel;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlexcel = new Microsoft.Office.Interop.Excel.Application();
            xlexcel.Visible = true;
            xlWorkBook = xlexcel.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            writeExcelHeader(dgv, xlWorkSheet);
            Microsoft.Office.Interop.Excel.Range CR = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[2, 1];
            CR.Select();
            xlWorkSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);
            xlWorkSheet.Cells.EntireColumn.AutoFit();
            MessageBox.Show("匯出完成。");
        }


        private static void writeExcelHeader(DataGridView dgv, Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet)
        {
            /*var startCell = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[1, 1];
            var endCell = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[1, 1];
            var writeRange = xlWorkSheet.Range[startCell, endCell];*/
            int NotVisbleCount = 0;

            xlWorkSheet.Cells[1, 1] = dgv.TopLeftHeaderCell.FormattedValue;
            for (int j = 0; j <= dgv.ColumnCount - 1; j++)
            {
                var startCell = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[1, j + 2 - NotVisbleCount];
                var endCell = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[1, j + 2 - NotVisbleCount];
                var writeRange = xlWorkSheet.Range[startCell, endCell];
                if (dgv.Columns[j].Visible == true) writeRange.Value2 = dgv.Columns[j].HeaderText;
                else NotVisbleCount++;
            }
        }
        private static void copyAlltoClipboard(DataGridView dgv)
        {
            dgv.SelectAll();
            DataObject dataObj = dgv.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        public void DELINTO1(string IPAddress)
        {
            string IP = "IPINT";
            string A = IPAddress;
            StringBuilder sb = new StringBuilder(A);
            char a0 = sb[0];
            char a1 = sb[1];
            char a2 = sb[2];
            char a3 = sb[3];
            char a4 = sb[4];
            char a5 = sb[5];
            char a6 = sb[6];
            char a7 = sb[7];
            char a8 = sb[8];
            char a9 = sb[9];
            char a10 = sb[10];
            char a11 = sb[11];
            try
            {
                //string ConnString;
                //ConnString = "Data Source=211.23.138.231;Password=2u4u 2u04yj3;User ID=firstohm;Database=Firstohm;port=3306;charset=utf8;";

                string selectCmd = "DROP TABLE  " + IP + a10 + a11;
                MySqlConnection conn = new MySqlConnection(Constants.ConnString); //ConnString
                conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(selectCmd, conn);
                DataSet ds = new DataSet();
                da.Fill(ds, "PACK_DE2_1");

                //GlobalDs = ds;
                //dataGridView1.DataSource = ds.Tables["PACK_DE2_1"];
                conn.Close();
                da.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DELINTO1");
            }
        }

        public static void addButton(DataGridView dgv2, string btnName, string btnText, int columnIndex = 0)
        {
            //以下程式加到 Winform
            //private void dgv1_CellClick(object sender, DataGridViewCellEventArgs e)
            //{
            //    if (e.ColumnIndex == dgv1.Columns["uninstall_column"].Index)
            //    {
            //        //Do something with your button.
            //    }
            //}
            DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
            buttonColumn.Name = btnName;
            buttonColumn.Text = btnText;
            if (dgv2.Columns[btnName] == null)
            {
                dgv2.Columns.Insert(columnIndex, buttonColumn);
            }
        }

        //設定 DGV 的 Title
        public static void changeDgvHeader(DataGridView dgv2, Dictionary<string, string> headerDic)
        {
            if (headerDic == null)
                return;
            foreach (KeyValuePair<string, string> entry in headerDic)
            {
                dgv2.Columns[entry.Key].HeaderText = entry.Value;
            }
        }

        public static void dgvHideColumn(DataGridView dgv, List<string> hiddenCol = null, bool ifHite = true)
        {
            if (hiddenCol == null)
                return;
            if (ifHite)
            {
                foreach (var hCol in hiddenCol)
                {
                    dgv.Columns[hCol].Visible = false;
                }
            }
            else
            {
                foreach (DataGridViewColumn hCol in dgv.Columns)
                {
                    hCol.Visible = false;
                }
                foreach (var hCol in hiddenCol)
                {
                    dgv.Columns[hCol].Visible = true;
                }
            }
        }
        public static string getDate(string Date_Time)
        {
            return Date_Time == "" ? "" : DateTime.Parse(Date_Time).ToString("yyyy/MM/dd");
        }


        // dgv1.Columns["MinDigit"].DefaultCellStyle.Format = "##0.00##";
        public static void formatDG(DataGridView dgv2, Dictionary<string, string> headerDic)
        {
            if (headerDic == null)
                return;
            foreach (KeyValuePair<string, string> entry in headerDic)
            {
                dgv2.Columns[entry.Key].DefaultCellStyle.Format = entry.Value;
            }
        }

        public static void setDGVWith(DataGridView dgv2, Dictionary<string, int> headerDic)
        {
            foreach (KeyValuePair<string, int> entry in headerDic)
            {
                try
                {
                    dgv2.Columns[entry.Key].Width = entry.Value; ;
                }
                catch { }

            }
        }

        //刪除DataGridView選取的列
        public static void deleteDgvCheckedRows(DataGridView dgv, string ColName)
        {
            for (int row = 0; row < dgv.Rows.Count - 1; row++)
            {
                if ((bool)dgv.Rows[row].Cells[ColName].FormattedValue)
                {
                    dgv.Rows.RemoveAt(row);
                    row--;
                }
            }

        }

        public static DataGridView CloneDataGrid(DataGridView mainDataGridView)
        {
            DataGridView cloneDataGridView = new DataGridView();

            if (cloneDataGridView.Columns.Count == 0)
            {
                foreach (DataGridViewColumn datagrid in mainDataGridView.Columns)
                {
                    cloneDataGridView.Columns.Add(datagrid.Clone() as DataGridViewColumn);
                }
            }

            DataGridViewRow dataRow = new DataGridViewRow();

            for (int i = 0; i < mainDataGridView.Rows.Count; i++)
            {
                dataRow = (DataGridViewRow)mainDataGridView.Rows[i].Clone();
                int Index = 0;
                foreach (DataGridViewCell cell in mainDataGridView.Rows[i].Cells)
                {
                    dataRow.Cells[Index].Value = cell.Value;
                    Index++;
                }
                cloneDataGridView.Rows.Add(dataRow);
            }
            cloneDataGridView.AllowUserToAddRows = false;
            cloneDataGridView.Refresh();


            return cloneDataGridView;
        }

        public static void dgvAddComboBoxColumn(DataGridView dgv, string HeaderText, string ColName, List<string> Options, int ColIndex, string DPName = null, DataTable dtSource = null)
        {
            DataGridViewComboBoxColumn comboboxColumn;
            comboboxColumn = CreateComboBoxColumn(HeaderText, ColName, DPName, dtSource);
            comboboxColumn.Name = ColName;
            if (dtSource == null) SetAlternateChoicesUsingItems(comboboxColumn, Options);
            dgv.Columns.Insert(ColIndex, comboboxColumn);
        }

        private static void SetAlternateChoicesUsingItems(DataGridViewComboBoxColumn comboboxColumn, List<string> Options)
        {
            foreach (string item in Options)
                comboboxColumn.Items.Add(item);
            //comboboxColumn.Items.AddRange("Mr.", "Ms.", "Mrs.", "Dr.");
        }

        public static DataGridViewComboBoxColumn CreateComboBoxColumn(string HeaderText, string ColName, string DPName, DataTable dtSource)
        {
            DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
            {
                column.HeaderText = HeaderText;
                column.Name = ColName;
                column.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                column.DataPropertyName = DPName;
                if (dtSource != null)
                {
                    column.DataSource = dtSource;
                    column.DisplayMember = dtSource.Columns[1].ColumnName;
                    column.ValueMember = dtSource.Columns[0].ColumnName;
                }
                //column.DropDownWidth = 160;
                //column.Width = 90;
                //column.MaxDropDownItems = 3;
                //column.FlatStyle = FlatStyle.Flat;
            }
            return column;
        }

        public static void dtToDgvWithSelectAllCheckBox(DataTable dt, DataGridView dgv, string Name)
        {
            if (dt == null) // || dt.Rows.Count == 0
                return;
            dgv.DataSource = dt;
            if(Name != null)
            {
                DataGridViewSelectAllColumn HeaderSelectAll = new DataGridViewSelectAllColumn();
                dgv.Columns.Insert(0, HeaderSelectAll);
                dgv.Columns[0].HeaderText = Name;
                dgv.Columns[0].Name = Name;
            }
        }

        //以 Grid 的內容， Insert/Update 到資料庫
        public static void updateDBbyGridData(DataGridView dgv, string tableName, string connection = Constants.ConnString)
        {
            string sqlStr = "", upDatePart = "";
            int idxRow = 0, idxCol = 0, updateCnt = 0;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Cells[0].Value == null || row.Cells[0].Value.ToString() == "")
                    continue;
                sqlStr = "INSERT INTO " + tableName + " (" + dataGridColumnNameToString(dgv, ",") + ") VALUES(" + dataGridRowDataToString(row, "'", ",") + ")  ON DUPLICATE KEY UPDATE " +
                dataGridRowDataToString(row, "'", ",", true);
                execSQLNonQuery(sqlStr, Constants.ACLConnString);
            }
        }

        private static void execSQLNonQuery(string sqlStr, object aCLConnString)
        {
            throw new NotImplementedException();
        }

        public static int execSQLNonQuery(String sqlCommand, String ConnString = Constants.ConnString, bool LAST_INSERT_ID = false)
        {
            int rtnInt = 0;
            MySqlConnection cnSQL = new MySqlConnection(ConnString);
            IDataReader reader = null;
            try
            {

                MySqlCommand cmdSQL = new MySqlCommand();
                cmdSQL.Connection = cnSQL;
                cnSQL.Open();//開啟SqlConnection物件
                cmdSQL.CommandText = sqlCommand;
                rtnInt = cmdSQL.ExecuteNonQuery();
                if (LAST_INSERT_ID)
                {
                    cmdSQL.CommandText = "SELECT LAST_INSERT_ID()";
                    reader = cmdSQL.ExecuteReader();
                    if (reader != null && reader.Read())
                        rtnInt = reader.GetInt32(0);
                    reader.Close();
                }
            }
            finally
            {
                cnSQL.Close();
            }
            return rtnInt;
        }

        public static void dtTodgv(DataGridView dgv, DataTable dt, List<string> hiddenCol = null, bool ifHide = true)
        {
            dgv.DataSource = dt;
            if (hiddenCol != null)
                dgvHideColumn(dgv, hiddenCol, ifHide);
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoResizeColumns();
            dgv.DefaultCellStyle.Font = new Font("標楷體", 14F, GraphicsUnit.Pixel);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("標楷體", 14F, GraphicsUnit.Pixel);
        }


        public static List<string> dtToListWithCondition(DataTable dt, string columnName, string conditionStr, string sort = null)
        {
            DataTable filterDT = DataTableFilterSort1(dt, conditionStr, sort);
            return dtToList(filterDT, columnName, true);
        }

        public static List<string> dtToList(DataTable dt, string columnName = null, bool distinct = false, bool sort = false)
        {
            if (dt == null)
                return null;
            if (columnName == null)
                columnName = dt.Columns[0].ColumnName;
            if (!dt.Columns.Contains(columnName))
                return null;
            List<string> rtnList = new List<string>();
            if (columnName != null)
                foreach (DataRow myRow in dt.Rows)
                {
                    if (!rtnList.Contains(myRow[columnName].ToString()))
                        rtnList.Add(myRow[columnName].ToString());
                }
            else
                foreach (DataRow myRow in dt.Rows)
                {
                    if (!rtnList.Contains(myRow[0].ToString()))
                        rtnList.Add(myRow[0].ToString());
                }
            if (sort)
                rtnList.Sort();
            return rtnList;
        }
        //轉置 dt, 將 Col 轉為 Row, Row 轉為 Col


        public static List<string> dtToList(DataTable dt, string columnName)
        {
            List<string> rtnList = new List<string>();
            foreach (DataRow myRow in dt.Rows)
            {
                rtnList.Add(myRow[columnName].ToString());
            }
            return rtnList;
        }

        public static DataTable DataTableFilterSort1(DataTable oTable, string filterExpression = null, string sortExpression = null)
        {
            DataView dv = new DataView();
            dv.Table = oTable;
            if (!string.IsNullOrEmpty(filterExpression))
                dv.RowFilter = filterExpression;
            if (!string.IsNullOrEmpty(sortExpression))
                dv.Sort = sortExpression;
            DataTable nTable = dv.ToTable();
            return nTable;
        }


        //將 Dictionary 內容轉為 DataGridView 的ColumnNames
        //前置額外的欄位， 可以放到firstFewCols， 例如  "登入帳號,姓名"， 則增加兩個欄位
        public static DataGridView createDGVColumnbyDictionary(DataGridView dgv1, String firstFewCols, Dictionary<string, string> dgvColumn)
        {
            if (firstFewCols != null)
            {
                string[] firstFewColsArr = firstFewCols.Split(',');
                for (int i = 0; i < firstFewColsArr.Length; i++)
                    dgv1.Columns.Add(firstFewColsArr[i], firstFewColsArr[i]);
            }

            foreach (KeyValuePair<string, string> entry in dgvColumn)
            {
                dgv1.Columns.Add(entry.Key, entry.Value);
            }
            dgv1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv1.AutoResizeColumns();
            dgv1.DefaultCellStyle.Font = new Font("標楷體", 14F, GraphicsUnit.Pixel);
            dgv1.ColumnHeadersDefaultCellStyle.Font = new Font("標楷體", 14F, GraphicsUnit.Pixel);
            return dgv1;
        }

        //數字轉ASCII字元, by Michael 2019.1.18
        public char Chr(int Num)
        {
            char C = Convert.ToChar(Num);
            return C;
        }

        //把DataGridView的內容，單數列和雙數列用不同底色，使顯示更清楚，Added by Michael, 2020.6
        public static void dgvRowsColorPair(DataGridView dgv)
        {
            if (dgv.Rows.Count != 0)
                for (int i = 0; i < dgv.Rows.Count; i++)
                    dgv.Rows[i].DefaultCellStyle.BackColor = (i % 2 == 0 ? System.Drawing.Color.Honeydew : System.Drawing.Color.White);
        }

        //Dictionary 為 實際資料
        //List 為 Default Value
        //sql string 的前面幾個欄位， 必須對到 firstFewCols, 即 Column 的 Prefix， 例如 登入帳號,姓名
        public static DataGridView addRowtoDGVbyDictionary(DataGridView dgv1, Dictionary<string, string> dgvColumn, string defaultRowData = null)
        {
            var index = dgv1.Rows.Add();
            if (defaultRowData != null)
            {
                foreach (DataGridViewCell cell in dgv1.Rows[index].Cells)
                {
                    cell.Value = defaultRowData;
                }

            }
            foreach (KeyValuePair<string, string> entry in dgvColumn)
            {
                dgv1.Rows[index].Cells[entry.Key].Value = entry.Value;
            }
            return dgv1;
        }


        //將 DT 資料寫入 dataGridVide
        //colIdx 是哪些 DT 的欄位要寫入
        //repeatSet ColumnSet 要重複幾遍
        //ifSeqNo 第一排是否需要加序號
        public static void dtTodgv(DataTable dt, DataGridView dgv2, List<int> colIdx, int repeatSet, bool ifSeqNo = false)
        {
            if (dt == null || dt.Rows.Count == 0)
                return;
            dgv2.Rows.Clear();
            dgv2.Refresh();
            int RowLimit = dt.Rows.Count / repeatSet;
            int ColLimit = dt.Rows.Count % repeatSet;
            int row = 0; int col = 0;
            int startCol = ifSeqNo ? 1 : 0;
            int colset = 0;
            foreach (DataRow myRow in dt.Rows)
            {
                if (colset == 0 && col == 0)
                {
                    dgv2.Rows.Add();
                }
                if (ifSeqNo && colset == 0)
                    dgv2.Rows[row].Cells[0].Value = row + 1; //序號
                foreach (int itemIdx in colIdx)
                {
                    dgv2.Rows[row].Cells[colset * (colIdx.Count) + startCol + col].Value = myRow[itemIdx].ToString(); //工令單號
                    col++;
                }
                dgv2.Refresh();
                col = 0;
                row++;
                if (row == RowLimit + 2)
                {
                    row = 0;
                    if (colset < ColLimit)
                    {
                        colset++;
                    }
                    else
                    {
                        break; //到此, dt 應該已經讀完
                    }
                }
                else if (row == (RowLimit + 1))
                {
                    row = 0;
                    colset++;
                }
            }
        }

        #region checkBox
        public static DataGridViewRow findFirstRowInDGV(DataGridView dgv, string searchCol, string searchValue)
        {
            int rowIndex = -1;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Cells[searchCol].Value.ToString().Equals(searchValue))
                {
                    return row;
                }
            }
            return null;
        }

        public static DataGridViewRow updRowInDGV(DataGridView dgv, string searchCol, string searchValue, String targetVal = "False")
        {
            int rowIndex = -1;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Cells[searchCol].Value.ToString().Equals(searchValue))
                {
                    row.Cells[searchCol].Value = targetVal;
                }
            }
            return null;
        }



        //全選/全反選
        //chkType 0:全反選, 1:反選
        public static void dgvChkAll(DataGridView dgv2, String chkColName = "選取", bool defaultChk = false)
        {
            for (int i = 0; i < dgv2.Rows.Count - 1; i++)
            {
                dgv2.Rows[i].Cells[chkColName].Value = defaultChk.ToString();
            }
        }

        public static void dgvChkByRowIdx(DataGridView dgv2, int rowIdx, bool defaultChk = true, String chkColName = "選取")
        {
            dgv2.Rows[rowIdx].Cells[chkColName].Value = defaultChk;
        }

        //全選或全反選
        public static void dgvGroupChk(DataGridView dgv2, String chkColName = "選取", bool defaultChk = false)
        {
            int allowInsrt = dgv2.AllowUserToAddRows ? 1 : 0;
            for (int i = 0; i < dgv2.Rows.Count - allowInsrt; i++)
            {
                //if (dgv2.Rows[i].Cells[0].Value != null && dgv2.Rows[i].Cells[0].Value.ToString() != "")
                dgv2.Rows[i].Cells[chkColName].Value = defaultChk;
            }
        }

        public static void dgvGroupChk(DataGridView dgv2, List<string> chkArray = null, string validateInx = null, bool defaultChk = true, string chkInx = null, String chkColName = "選取")
        {
            if (chkArray == null)
                return;
            for (int i = 0; i < dgv2.Rows.Count - 1; i++)
            {
                if (chkArray.Contains(dgv2.Rows[i].Cells[validateInx].Value))
                    dgv2.Rows[i].Cells[chkColName].Value = defaultChk.ToString();
            }
        }

        //由 RowIdx 決定是否選取
        public static void dgvGroupChk(DataGridView dgv2, List<int> rowIdx, String chkColName = "選取", bool defaultChk = false)
        {
            if (rowIdx == null)
                return;
            int allowInsrt = dgv2.AllowUserToAddRows ? 1 : 0;
            for (int i = 0; i < dgv2.Rows.Count - allowInsrt; i++)
            {
                if (rowIdx.Contains(i))
                    dgv2.Rows[i].Cells[chkColName].Value = defaultChk;
            }
        }

        //回傳所選的 某個 Cell 的值, 若為 Multi_Row,則只傳回第一個
        public static List<String> getCheckedCellValue(DataGridView dgv1, string cellName, String chkColName = "選取")
        {
            List<string> rtnList = new List<string>();
            foreach (DataGridViewRow row in dgv1.Rows)
            {
                if (Convert.ToBoolean(row.Cells[chkColName].Value))
                {
                    if (!rtnList.Contains(row.Cells[cellName].Value.ToString()))
                        rtnList.Add(row.Cells[cellName].Value.ToString());
                }
            }
            return rtnList;
        }

        //checkbox
        public static void dtTodgvWithChkbox(DataTable dt, DataGridView dgv2, String chkColName = "選取", bool defaultChk = false, int chkboxPos = 0, bool chkboxEditable = true, bool readOnly = false)
        {
            if (dt == null || dt.Rows.Count == 0)
                return;
            //dt.Columns.Add(new DataColumn(chkColName, typeof(bool)));
            DataColumn Col = dt.Columns.Add(chkColName, System.Type.GetType("System.Boolean"));
            Col.SetOrdinal(chkboxPos);// to put the column in position chkboxPos;
            dgv2.DataSource = dt;
            dgvGroupChk(dgv2, chkColName, defaultChk);
            dgv2.Columns[chkboxPos].ReadOnly = readOnly;
        }

        //以下為範例程式
        public static void dgvWithChkboxProcessCheck(DataGridView dgv1, String chkColName = "選取")
        {
            //以下為範例程式
            //int i = 0, j = 0;
            //string sqlStr;
            //for (i = 0; i < (dgv1.Rows.Count - 1); i++)
            //{
            //    if (!string.IsNullOrEmpty(dgv1.Rows[i].Cells["basebase"].Value.ToString()) && Convert.ToBoolean(dgv1.Rows[i].Cells[chkColName].Value))
            //    {

            //        sqlStr = "Update `MFO_FLOW` SET `BASEID` = '" + dgv1.Rows[i].Cells["basebase"].Value +
            //                    "' Where `FLOWID`='" + dgv1.Rows[i].Cells["FLOWID"].Value + "'";
            //        CommonClass.execSQLNonQuery(sqlStr);
            //        j++;
            //    }
            //}
        }

        public static List<DataGridViewRow> GetAllCheckedRows(DataGridView dgv2, string checkRow = "選取")
        {
            List<DataGridViewRow> allCheckedRows = dgv2.Rows.Cast<DataGridViewRow>()
                            .Where(row => (bool?)row.Cells[checkRow].Value == true)
                            .ToList();
            return allCheckedRows;
        }


        #endregion



        //回傳所選的 某個 Cell 的值, 若為 Multi_Row,則只傳回第一個
        public static String getSelectedCellValue(DataGridView dgv1, string cellName)
        {
            string rtnStr = null;
            if (dgv1.SelectedCells.Count > 0)
            {
                int selectedrowindex = dgv1.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dgv1.Rows[selectedrowindex];
                rtnStr = Convert.ToString(selectedRow.Cells[cellName].Value);
            }
            return rtnStr;
        }

        public static void removeSelectedRow(DataGridView dgv1)
        {
            Int32 selectedRowCount = dgv1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                for (int i = 0; i < selectedRowCount; i++)
                {
                    dgv1.Rows.RemoveAt(dgv1.SelectedRows[0].Index);
                }
            }
        }

        public static void setDGVAttributes(DataGridView dgv1, List<string> hiddenCol = null, float fontSize = 14, Dictionary<String, int> ColWidth = null, Dictionary<String, string> dgFormateDics = null)
        {
            if (hiddenCol != null)
                dgvHideColumn(dgv1, hiddenCol);
            if (ColWidth != null)
                setDGVWith(dgv1, ColWidth);
            if (dgFormateDics != null)
                formatDG(dgv1, dgFormateDics);
            dgv1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv1.AutoResizeColumns();
            dgv1.DefaultCellStyle.Font = new Font("標楷體", fontSize, GraphicsUnit.Pixel);
            dgv1.ColumnHeadersDefaultCellStyle.Font = new Font("標楷體", fontSize, GraphicsUnit.Pixel);
        }

        public static float sumOfaColumn(DataGridView dgv1, String columnName)
        {
            float sum = 0;
            for (int i = 0; i < dgv1.Rows.Count; ++i)
            {
                sum += Convert.ToInt32(dgv1.Rows[i].Cells[columnName].Value);
            }
            return sum;
        }

        public static DataTable prepareGrid(DataGridView dgv1, String selectCmd, String tableName = "ABC", List<string> hiddenCol = null, String ConnString = Constants.ConnString, float fontSize = 14, Dictionary<String, int> ColWidth = null, Dictionary<String, string> dgFormateDics = null)
        {
            DataTable dt = null;
            MySqlConnection conn = new MySqlConnection(ConnString);
            try
            {
                conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(selectCmd, conn);
                DataSet ds = new DataSet();
                da.Fill(ds, tableName);
                dt = ds.Tables[tableName];
                dgv1.DataSource = dt;

                setDGVAttributes(dgv1, hiddenCol, fontSize, ColWidth, dgFormateDics);
                da.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, tableName);
            }
            finally
            {
                conn.Close();
            }
            return dt;
        }
        //Where 的 Parameter 用 Dictionary<string, string> 帶
        public static void prepareGrid(DataGridView dgv1, String selectCmd, String tableName, Dictionary<string, string> sqlparams, List<string> hiddenCol = null, String ConnString = Constants.ConnString)
        {
            MySqlConnection conn = new MySqlConnection(ConnString);
            try
            {
                conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(selectCmd, conn);
                foreach (var paramItem in sqlparams)
                {
                    da.SelectCommand.Parameters.AddWithValue(paramItem.Key, paramItem.Value);
                }
                DataSet ds = new DataSet();
                da.Fill(ds, tableName);

                dgv1.DataSource = ds.Tables[tableName];
                setDGVAttributes(dgv1, hiddenCol);
                da.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, tableName);
            }
            finally
            {
                conn.Close();
            }
        }

        public static void setDGVCellColor(DataGridViewRow dgvRow, int columnIndex, Color setColor, bool ifBackColor = true)
        {
            if (ifBackColor)
                dgvRow.Cells[columnIndex].Style.BackColor = setColor;
            else
                dgvRow.Cells[columnIndex].Style.ForeColor = setColor;
        }

        public static void setDGVColumnColor(DataGridView dgv, List<KeyValuePair<int, Color>> columnsColor, bool ifBackColor = true)
        {
            foreach (KeyValuePair<int, Color> colorCol in columnsColor)
            {
                if (ifBackColor)
                    dgv.Columns[colorCol.Key].DefaultCellStyle.BackColor = colorCol.Value;
                else
                    dgv.Columns[colorCol.Key].DefaultCellStyle.ForeColor = colorCol.Value;
            }
        }

        public static void setDGVCellColor(DataGridViewRow dgvRow, List<KeyValuePair<int, Color>> columnsColor, bool ifBackColor = true)
        {
            foreach (KeyValuePair<int, Color> colorCol in columnsColor)
            {
                if (ifBackColor)
                    dgvRow.Cells[colorCol.Key].Style.BackColor = colorCol.Value;
                else
                    dgvRow.Cells[colorCol.Key].Style.ForeColor = colorCol.Value;
            }
        }

        #region data Dirty
        public static DataGridView chkDirty(DataGridView dgv)
        {
            if (dgv.IsCurrentCellDirty || dgv.IsCurrentRowDirty)
            {
                dgv.CurrentRow.DataGridView.EndEdit();
                dgv.EndEdit();
                CurrencyManager cm = (CurrencyManager)dgv.BindingContext[dgv.DataSource, dgv.DataMember];
                cm.EndCurrentEdit();
            }
            return dgv;
        }

        //使用方法（參考 _3_1_2machines 的 dgv1_CellEndEdit(object sender, DataGridViewCellEventArgs e)）
        //0. 宣告 Class Global  變數 List<int> dirtyRos = new List<int>();
        //1. dgv 增加 _CellEndEdit 事件
        //2. 裡面只放一行程式： CommonClass.setDGVRowDirty(dgv1, e.RowIndex, dirtyRos);
        public static void setDGVRowDirty(DataGridView dgv1, int rowIndex, List<int> dirtyRos)
        {

            DataGridViewRow gridrow = dgv1.Rows[rowIndex];
            DataRowView rowview = (DataRowView)gridrow.DataBoundItem;
            DataRow row = rowview.Row;
            if (row.RowState != DataRowState.Unchanged || dgv1.IsCurrentRowDirty)
            {
                dirtyRos.Add(rowIndex);
            }
        }

        //當刪除某個 Row， 將該筆隱藏起來，讓 User 感覺是被刪除了
        //使用方法（參考 _3_1_2machines 的 dgv1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)）
        //0. 宣告 Class Global  變數 List<string> delKey = new List<delKey>(); //即將被刪除的Row 的 Key， 裝上 Where 即可
        //1. dgv 增加 dgv1_UserDeletingRow 事件
        //2. 裡面只放一行程式： CommonClass.setDGVRowDirty(dgv1, e.RowIndex, dirtyRos);
        public static void deleteDGVRowDirty(DataGridView dgv1)
        {
        }

        #endregion
        public static DataGridView CopyDataGridView(DataGridView dgv_org, DataGridView dgv_copy)
        {
            try
            {
                if (dgv_copy.Columns.Count == 0)
                {
                    foreach (DataGridViewColumn dgvc in dgv_org.Columns)
                    {
                        dgv_copy.Columns.Add(dgvc.Clone() as DataGridViewColumn);
                    }
                }

                DataGridViewRow row = new DataGridViewRow();

                for (int i = 0; i < dgv_org.Rows.Count; i++)
                {
                    row = (DataGridViewRow)dgv_org.Rows[i].Clone();
                    int intColIndex = 0;
                    foreach (DataGridViewCell cell in dgv_org.Rows[i].Cells)
                    {
                        row.Cells[intColIndex].Value = cell.Value;
                        intColIndex++;
                    }
                    dgv_copy.Rows.Add(row);
                }
                dgv_copy.AllowUserToAddRows = false;
                dgv_copy.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Copy DataGridViw");
            }
            return dgv_copy;
        }


        //檢查與前一個 Row 同值， 則清為空白
        public static DataGridView spaceSameKey(DataGridView dgv, int idxCol)
        {
            string cruuMFO = null;
            foreach (DataGridViewRow dgvRow in dgv.Rows)
            {
                if (cruuMFO != dgvRow.Cells[idxCol].FormattedValue.ToString())
                {
                    cruuMFO = dgvRow.Cells[idxCol].FormattedValue.ToString();
                }
                else
                {
                    dgvRow.Cells[idxCol].Value = "";

                }
            }
            if (dgv.IsCurrentCellDirty || dgv.IsCurrentRowDirty)
            {
                dgv.CurrentRow.DataGridView.EndEdit();
                dgv.EndEdit();
                CurrencyManager cm = (CurrencyManager)dgv.BindingContext[dgv.DataSource, dgv.DataMember];
                cm.EndCurrentEdit();
            }
            return dgv;
        }



        //傳回 GDV 的指定欄位的 Array 值
        public static List<String> dataGridColumnValueToArray(DataGridView dgv, int dataIdx)
        {
            List<String> rtnList = new List<String>();
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                rtnList.Add(dgv.Rows[i].Cells[dataIdx].FormattedValue.ToString().Trim());
            }
            return rtnList;
        }

        public static List<String> dataGridColumnNameToArray(DataGridView dgv)
        {
            List<String> rtnList = new List<String>();
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                rtnList.Add(dgv.Columns[i].Name);
            }
            return rtnList;
        }

        public static string dataGridColumnNameToString(DataGridView dgv, string delimeter)
        {
            string rtnStr = null;
            List<string> gdvColumns = dataGridColumnNameToArray(dgv);
            if (gdvColumns != null && gdvColumns.Count > 0)
                rtnStr = String.Join(delimeter, gdvColumns.ToArray());
            return rtnStr;
        }

        //設定 DGV 欄位寬度， KEY: Collumn Index, Value:Pixel
        public static DataGridView dgvSetColWidth(DataGridView dgv1, Dictionary<int, int> colPixels, bool ifWrapContent = true)
        {
            if (ifWrapContent)
            {
                dgv1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }
            foreach (KeyValuePair<int, int> colItem in colPixels)
            {
                DataGridViewColumn column = dgv1.Columns[colItem.Key];
                column.Width = colItem.Value;
                if (ifWrapContent)
                {
                    dgv1.Columns[colItem.Key].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dgv1.Columns[colItem.Key].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }
            }
            if (ifWrapContent)
            {
                dgv1.Columns[dgv1.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dgv1.Columns[dgv1.Columns.Count - 1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }

            return dgv1;
        }

        //取得 dataGridView 的某個 Column 的所有資料， 合併成一個字串
        //作為 SQL 指令 in() 的條件
        public static String dataGridColumnToStringNoduplicate(DataGridView dgv, int dataIdx)
        {
            HashSet<String> rtnList = new HashSet<String>();
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                rtnList.Add("'" + dgv.Rows[i].Cells[dataIdx].FormattedValue.ToString().Trim() + "'");
            }
            string output = string.Join(",", rtnList);
            return output;
        }

        public static List<String> datdataGridRowDataToArray(DataGridViewRow row, string wraper, bool withColumnName = false)
        {
            List<String> rtnList = new List<String>();
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (withColumnName)
                    rtnList.Add(cell.OwningColumn.Name + "=" + wraper + cell.Value.ToString() + wraper);
                else
                    rtnList.Add(wraper + cell.Value.ToString() + wraper);
            }
            return rtnList;
        }

        public static string dataGridRowDataToString(DataGridViewRow row, string wraper, string delimeter, bool withColumnName = false)
        {
            string rtnStr = null;
            List<string> gdvColumns = datdataGridRowDataToArray(row, wraper, withColumnName);
            if (gdvColumns != null && gdvColumns.Count > 0)
                rtnStr = String.Join(delimeter, gdvColumns.ToArray());
            return rtnStr;
        }
        #endregion

        #region ColorFuncs

        public static Color colorFromIntArr(int[] intRGB)
        {
            return Color.FromArgb((byte)intRGB[0], (byte)intRGB[1], (byte)intRGB[2]);
        }

        public static Color colorFromString(string colorString)
        {
            string[] tempRGB = colorString.Split(',');
            int[] intRGB = Array.ConvertAll(tempRGB, s => int.Parse(s));
            return colorFromIntArr(intRGB);
        }

        #endregion

        #region Set Alert Color
        public static dynamic Cast(dynamic obj, Type castTo)
        {
                return Convert.ChangeType(obj, castTo);
        }


        public static T changeType<T>(string inputVal)
        {
            return Cast(inputVal, typeof(T));
        }


        //依照 DataTable 設定 cell 的顏色
        //conditionJson 例如 [{ColumnName:abc,operator:gte, double value, coloe:red}]
        public static void dgvColorAlert<T>(DataGridViewRow dgvDr, string colName, string dgOperator, String cpValue, Color alertCol)
        {
            bool ifHit = false;
            T compareVal;
            if (cpValue.ToString()[0] == '@')
                compareVal = Cast(dgvDr.Cells[cpValue.Substring(1)].Value, typeof(T));
            else
                compareVal = Cast(cpValue, typeof(T));
            if(dgvDr.Cells[colName].Value != null && dgvDr.Cells[colName].Value != DBNull.Value)
            {
                switch (dgOperator)
                {
                    case "gt": // >
                        ifHit = (Cast(dgvDr.Cells[colName].Value, typeof(T)) > compareVal);
                        break;
                    case "gte": // >=
                        ifHit = (Cast(dgvDr.Cells[colName].Value, typeof(T)) >= compareVal);
                        break;
                    case "eq": // ==
                        ifHit = (Cast(dgvDr.Cells[colName].Value, typeof(T)) == compareVal);
                        break;
                    case "neq": // !=
                        ifHit = (Cast(dgvDr.Cells[colName].Value, typeof(T)) != compareVal);
                        break;
                    case "lt": // <
                        ifHit = (Cast(dgvDr.Cells[colName].Value, typeof(T)) < compareVal);
                        break;
                    case "lte": // <=
                        ifHit = (Cast(dgvDr.Cells[colName].Value, typeof(T)) <= compareVal);
                        break;
                }
            
            }
            else
            {
                ifHit = false;
            }
            if (ifHit)
            {
                dgvDr.Cells[colName].Style.ForeColor = alertCol;
            }
        }
        //依照 DataTable 設定 Row 的顏色
        //conditionJson 例如 [{ColumnName:abc,operator:gte, double value, coloe:red}]
        public static void dgvRowColorAlert<T>(DataGridViewRow dgvDr, string colName, string dgOperator, String cpValue, Color alertCol)
        {
            bool ifHit = false;
            T compareVal;
            if (cpValue.ToString()[0] == '@')
                compareVal = Cast(dgvDr.Cells[cpValue.Substring(1)].Value, typeof(T));
            else
                compareVal = Cast(cpValue, typeof(T));
            switch (dgOperator)
            {
                case "gt": // >
                    ifHit = (Cast(dgvDr.Cells[colName].Value, typeof(T)) > compareVal);
                    break;
                case "gte": // >=
                    ifHit = (Cast(dgvDr.Cells[colName].Value, typeof(T)) >= compareVal);
                    break;
                case "eq": // ==
                    ifHit = (Cast(dgvDr.Cells[colName].Value, typeof(T)) == compareVal);
                    break;
                case "neq": // !=
                    ifHit = (Cast(dgvDr.Cells[colName].Value, typeof(T)) != compareVal);
                    break;
                case "lt": // <
                    ifHit = (Cast(dgvDr.Cells[colName].Value, typeof(T)) < compareVal);
                    break;
                case "lte": // <=
                    ifHit = (Cast(dgvDr.Cells[colName].Value, typeof(T)) <= compareVal);
                    break;
            }
            if (ifHit)
            {
                dgvDr.DefaultCellStyle.BackColor = alertCol;
            }
        }


        //cpValue 比較值， 但是先轉成 String, 如果是 欄位， 則為以 @ 開頭的欄位名稱
        public static void dgvColColorAlert<T>(DataGridView dgv1, string colName, string dgOperator, String cpValue, Color alertCol)
        {
            //bool ifHit = false;
            foreach (DataGridViewRow dgvDr in dgv1.Rows)
            {
                dgvColorAlert<T>(dgvDr, colName, dgOperator, cpValue, alertCol);
            }
        }

        public static void dgvMultiColColorAlert<T>(DataGridView dgv1, List<string> colName, List<string> dgOperator, List<String> cpValue, List<Color> alertCol)
        {
            //bool ifHit = false;
            foreach (DataGridViewRow dgvDr in dgv1.Rows)
            {
                
                for (int i = 0; i < colName.Count; i++)
                {
                    if (!dgv1.Columns.Contains(colName[i]))
                        continue;
                    dgvColorAlert<T>(dgvDr, colName[i], dgOperator[i], cpValue[i], alertCol[i]);
                }
            }
        }


        public static void dgvRowColorByCondition<T>(DataGridView dgv1, string colName, string dgOperator, String cpValue, Color alertCol)
        {
            //bool ifHit = false;
            foreach (DataGridViewRow dgvDr in dgv1.Rows)
            {
                dgvRowColorAlert<T>(dgvDr, colName, dgOperator, cpValue, alertCol);
            }
        }

        //如果DataGridView某列的ColName值為EqualValue，設定DataGridView的Header為紅色
        public static void paintRowHeader(object sender, DataGridViewCellPaintingEventArgs e, string ColName, string EqualValue)
        {
            DataGridView dv = sender as DataGridView;
            DataGridViewCellStyle rowStyle;// = dv.RowHeadersDefaultCellStyle;

            if (e.ColumnIndex == -1)
            {
                e.PaintBackground(e.CellBounds, true);
                e.Handled = true;
                //if (/*I want to change this row */)
                foreach (DataGridViewRow dr in dv.Rows)
                {
                    if ((dr.Cells[ColName].Value == null ? "0" : dr.Cells[ColName].Value.ToString()) == EqualValue)
                    {
                        //rowStyle = dv.Rows[e.RowIndex].HeaderCell.Style;
                        rowStyle = dr.HeaderCell.Style;
                        //rowStyle.BackColor = Color.Wheat;
                        rowStyle.BackColor = Color.Red;
                        //dv.Rows[e.RowIndex].HeaderCell.Style = rowStyle;
                        dr.HeaderCell.Style = rowStyle;
                        using (Brush gridBrush = new SolidBrush(Color.LightGray)) //Color.Wheat
                        {
                            using (Brush backColorBrush = new SolidBrush(e.CellStyle.BackColor))
                            {
                                using (Pen gridLinePen = new Pen(gridBrush))
                                {
                                    // Clear cell 
                                    e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
                                    //Bottom line drawing
                                    e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);

                                    // here you force paint of content
                                    e.PaintContent(e.ClipBounds);
                                    e.Handled = true;
                                }
                            }
                        }
                    }

                }

            }

        }

        //給DataTable加流水號，BeginNo為起始值，DataTable首欄空格並須命名rowno，Added by Michael, 2020.10.8
        public static void fillDtRowNo(DataTable dt, int BeginNo)
        {
            for (int i = BeginNo; i < dt.Rows.Count + BeginNo; i++)
                dt.Rows[i - BeginNo]["rowno"] = i;
        }
        #endregion //Set Alert Color

        internal class CalendarCell : DataGridViewCell
        {
        }

        public class CalendarColumn : DataGridViewColumn
        {
            public CalendarColumn() : base(new CalendarCell())
            {

            }

            public override DataGridViewCell CellTemplate
            {
                get
                {
                    return base.CellTemplate;
                }
                set
                {
                    // Ensure that the cell used for the template is a CalendarCell.
                    if (value != null &&
                        !value.GetType().IsAssignableFrom(typeof(CalendarCell)))
                    {
                        throw new InvalidCastException("Must be a CalendarCell");
                    }
                    base.CellTemplate = value;
                }
            }
        }

        //字串的首字轉ASCII碼, by Michael 2019.1.18
        public int ASC(string S)
        {
            int N = Convert.ToInt32(S[0]);
            return N;
        }

        
    }
}
