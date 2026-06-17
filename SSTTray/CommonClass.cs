//參考/組件/System.Data.DataSetExtensions.dll 需勾選
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

using System.Drawing.Imaging;
using System.Linq;
using System.Net.Mail;
using System.IO;
using System.Text;
using System.Collections;
using System.Diagnostics;
//using System.Security.Cryptography;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Globalization;
//using Newtonsoft.Json;
//using System.Web.Script.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Management;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.ServiceProcess;
using System.IO.Compression;



//#### Ctrl M + Ctrl L + Ctrl L Close all
//CTR-M CTR-O
namespace FirstOhm
{
    public static class CommonClass
    {
        #region database
        public static DataTable InnerJoinDataTables(DataTable dt1, DataTable dt2, string[] columnsToSelect, string[] commonColumns)
        {
            // Create a new DataTable to store the result of the inner join
            DataTable resultTable = new DataTable();

            // Add columns to the result DataTable based on the columns to select
            foreach (string column in columnsToSelect)
            {
                if (dt1.Columns.Contains(column))
                    resultTable.Columns.Add(column, dt1.Columns[column].DataType);
                else if (dt2.Columns.Contains(column))
                    resultTable.Columns.Add(column, dt2.Columns[column].DataType);
                else
                    throw new ArgumentException($"Column '{column}' not found in both DataTables.");
            }

            // Perform the inner join by building a query with multiple conditions
            var query = from row1 in dt1.AsEnumerable()
                        from row2 in dt2.AsEnumerable()
                        where commonColumns.All(cc => row1[cc].ToString() == row2[cc].ToString())
                        select columnsToSelect.Select(column =>
                            row1.Table.Columns.Contains(column) ? row1[column] : row2[column]).ToArray();

            foreach (var rowArray in query)
            {
                resultTable.Rows.Add(rowArray);
            }

            return resultTable;
        }

        public static List<string> dtToListWithCondition(DataTable dt, string condition, string columnName = null, bool distinct = false, bool sort = false)
        {
            if (dt == null || dt.Rows.Count == 0)
                return null;
            dt = DataTableFilterSort1(dt, condition);
            return dtToList(dt, columnName, distinct, sort);
        }

        public static List<List<string>> dtToListListWithCondition(DataTable dt, string condition, string columnName = null, int maxCnts = 100, bool distinct = false, bool sort = false)
        {
            if (dt == null)
                return null;
            dt = DataTableFilterSort1(dt, condition);
            List<List<string>> rtnList = new List<List<string>>();
            List<string> eachList = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i % maxCnts == 0 && eachList.Count != 0)
                {
                    rtnList.Add(eachList);
                    eachList = new List<string>(); ;
                }
                eachList.Add(dt.Rows[i][columnName].ToString());
            }
            if (eachList.Count != 0)
            {
                rtnList.Add(eachList);
            }
            return rtnList;
        }
        //ifDelCol : true, 當 ColNameRow >=0 時 而且 該 Row 所代表的 Column Name  無值, 則直接刪除該 Column
        public static System.Data.DataTable changeDTColNameByDtData(ref System.Data.DataTable dt, int ColNameRow = -1, bool ifDelCol = false, bool ifDelRow = false)
        {
            int colIdx = 0;
            List<int> ColToDelete = new List<int>();
            foreach (DataColumn col in dt.Columns)
            {
                if (ColNameRow < 0)
                    col.ColumnName = "A" + colIdx.ToString(); //以 A1, A2, A3... 當作 Column Name
                else
                {
                    if (dt.Rows[ColNameRow - 1][colIdx] != DBNull.Value)
                        col.ColumnName = dt.Rows[ColNameRow - 1][colIdx].ToString();
                    else
                    {
                        if (ifDelCol)
                            ColToDelete.Add(colIdx);
                        col.ColumnName = "A" + colIdx.ToString();
                    }
                }
                colIdx++;
            }
            if (ifDelCol) //刪除 dt 中無欄位名稱的 Collumn
            {
                ColToDelete.Reverse();
                foreach (int itemIdx in ColToDelete)
                {
                    dt.Columns.RemoveAt(itemIdx);
                }
            }

            if (ifDelRow) //刪除 dt 中, 作為欄位名稱的 Row 及其 之前的 Row
            {
                for (int i = ColNameRow - 1; i >= 0; i--)
                {
                    dt.Rows.RemoveAt(i);
                }
            }
            return dt;
        }

        public static void dtTodgv(DataGridView dgv, DataTable dt, List<string> hiddenCol = null)
        {
            dgv.DataSource = dt;
            if (hiddenCol != null)
                dgvHideColumn(dgv, hiddenCol);
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoResizeColumns();
            dgv.DefaultCellStyle.Font = new Font("標楷體", 14F, GraphicsUnit.Pixel);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("標楷體", 14F, GraphicsUnit.Pixel);
        }

        public static void dgvHideColumn(DataGridView dgv, List<string> hiddenCol = null)
        {
            if (dgv.Columns.Count == 0)
                return;
            if (hiddenCol == null)
                return;
            foreach (var hCol in hiddenCol)
            {
                dgv.Columns[hCol].Visible = false;
            }
        }

        public static void killDBProcess(decimal maxSecs = 600, string connStr = Constants.ConnString)
        {
            DataTable dt = CommonClass.getSQLDataTable("show processList", connStr);
            dt = CommonClass.DataTableFilterSort1(dt, $"Time >= {maxSecs} ", " Time desc, id");
            if (dt.Rows.Count == 0)
                return;
            else
            {
                CommonClass.execSQLNonQuery($"KILL {dt.Rows[0][0]}");
                CommonClass.wait(1);
                dt = CommonClass.getSQLDataTable("show processList", connStr);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        CommonClass.execSQLNonQuery($"KILL {dr[0]}");
                    }
                }
            }
        }

        //從 Select 指令, 取出所有的 Columns Name
        //不過不太準確， 例如 distinct, as xxx 等干擾
        public static List<string> getColsFromQuerySql(string sqlStr)
        {
            return extractStrBetwn2Strings(sqlStr.ToLower(), "select", "from").Trim().Split(',').ToList();
        }

        //從  Newtonsoft.Json.Linq.JToken 資料轉換為 DataTable
        public static DataTable genDtFromJObject(List<string> dataCols, Newtonsoft.Json.Linq.JToken inputData)
        {
            DataTable rtnDt = new DataTable();
            // create columns
            foreach (string col in dataCols)
                rtnDt.Columns.Add(col);
            foreach (Newtonsoft.Json.Linq.JToken childitem in inputData.Children())
            {
                DataRow row = rtnDt.NewRow();
                foreach (string col in dataCols)
                    row[col] = childitem.SelectToken(col);
                rtnDt.Rows.Add(row);
            }
            return rtnDt;
        }

        public static string convertDatatableToXML(DataTable dt)
        {
            MemoryStream str = new MemoryStream();
            dt.WriteXml(str, true);
            str.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(str);
            string xmlstr;
            xmlstr = sr.ReadToEnd();
            return (xmlstr);
        }

        //DataTable 增加欄位
        public static void addColsToDT(ref DataTable sourceDT, string newColumn)
        {
            sourceDT.Columns.Add(newColumn);
        }

        public static DataTable addColsToDT(ref DataTable sourceDT, List<string> colList)
        {
            foreach (string colName in colList)
                addColsToDT(ref sourceDT, colName);
            return sourceDT;
        }

        //colsFrom2edToJoin ： 要從第二個 dt 加入 第一個 dt 的 Columns
        //keysDict -- Key- dtFist 的 Key Column, Value  dtSecond 的Key Column， 可以有多個， 所以用 Dictionary
        //bool ifInsertRows 
        //false: 僅將 dtFirst 與 dtSecond 含共同Key 的 Col Data，加入 dtFirst, 而不 Insert Rows 
        //true: 如果 dtSecond 有 而 dtFirst 不含該 Key, Inser Rows 
        public static DataTable MergeData(DataTable dtFirst, DataTable dtSecond, Dictionary<string, string> keysDict, List<string> colsFrom2edToJoin, bool ifInsertRows = false)
        {
            bool keyIdentical = false;
            DataRow drFirst;
            String keyCondition = null;
            int firstDtIdx = -1;
            Dictionary<string, string> searchKeys = new Dictionary<string, string>();
            DataRow firstDrToAdd = null;
            foreach (string newCol in colsFrom2edToJoin)
                dtFirst.Columns.Add(newCol);
            foreach (DataRow drSec in dtSecond.Rows)
            {
                searchKeys.Clear();
                foreach (KeyValuePair<string, string> dtKey in keysDict)
                {
                    searchKeys.Add(dtKey.Key, drSec[dtKey.Value].ToString());
                }
                firstDtIdx = CommonClass.searchFirstIndt(dtFirst, searchKeys);
                if (firstDtIdx < 0 && ifInsertRows)
                {
                    //在 firstDt 沒有找到同key
                    firstDrToAdd = dtFirst.NewRow();
                    foreach (string newCol in colsFrom2edToJoin)
                        firstDrToAdd[newCol] = drSec[newCol];
                    dtFirst.Rows.Add(firstDrToAdd);
                }
                else if (firstDtIdx >= 0)
                {
                    //在 firstDt 有找到同key
                    foreach (string newCol in colsFrom2edToJoin)
                        dtFirst.Rows[firstDtIdx][newCol] = drSec[newCol];
                }
            }
            return dtFirst;
        }

        //將原本 unin 的垂直 Join 轉為 水平 Join
        //keysList 要比對的 Key, 通常是日期
        //srcNameKey：union 出來 要作為新 ColName 的Data 內容 
        //dataCol ：select 出來的數字

        public static DataTable dtSelfJoin(DataTable dtFirst, List<string> keysList, string srcNameKey, string dataCol)
        {
            bool keyIdentical = false;
            DataRow drFirst;
            String keyCondition = null;
            List<int> firstDtIdxs;
            Dictionary<string, string> searchKeys = new Dictionary<string, string>();
            DataRow firstDrToAdd = null;
            List<string> newColsToJoin = dtToList(dtFirst, srcNameKey, true);
            foreach (string newCol in newColsToJoin)
                dtFirst.Columns.Add(newCol);
            dtFirst.Columns.Add("del");
            foreach (DataRow drSec in dtFirst.Rows)
            {
                if (drSec["del"].ToString() == "1")
                    continue;
                searchKeys.Clear();
                foreach (string dtKey in keysList)
                {
                    searchKeys.Add(dtKey, drSec[dtKey].ToString());
                }
                firstDtIdxs = CommonClass.searchAllIndt(dtFirst, searchKeys);
                for (int i = 0; i < firstDtIdxs.Count; i++)
                {
                    drSec[dtFirst.Rows[firstDtIdxs[i]][srcNameKey].ToString()] = dtFirst.Rows[firstDtIdxs[i]][dataCol];
                    if (i > 0)
                        dtFirst.Rows[firstDtIdxs[i]]["del"] = 1; //準備將此筆資料刪除
                }
            }
            dtFirst = dtDelRows(dtFirst, "del", "1");
            dtFirst.Columns.Remove(srcNameKey);
            dtFirst.Columns.Remove(dataCol);
            dtFirst.Columns.Remove("del");
            return dtFirst;
        }

        //移除條件符合的 dr
        public static DataTable dtDelRows(DataTable dt, string ColName, string compreData)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = dt.Rows[i];
                if (dr[ColName].ToString() == compreData)
                    dr.Delete();
            }
            dt.AcceptChanges();
            return dt;
        }

        //移除條件符合的 dr
        public static DataTable dtDelRows(DataTable dt, string ColName, List<string> compreDataList)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = dt.Rows[i];
                if (compreDataList.Contains(dr[ColName].ToString()))
                    dr.Delete();
            }
            dt.AcceptChanges();
            return dt;
        }

        public static DataTable autoRanking(DataTable dt, string colName = "序號")
        {
            dt.Columns.Add(colName);
            for (int i = 1; i <= dt.Rows.Count; i++)
                dt.Rows[i - 1][colName] = i.ToString();
            return dt;
        }

        public static DataTable DeepCloneDT<DataTable>(DataTable obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (DataTable)formatter.Deserialize(ms);
            }
        }

        //將 Dt 欄位中, List<Dictionary<string, string>> 從dt 的欄位 jsonCol 中取出
        //再依據 outputCol：key 為 object 欄位, Value 為 dt 的新增欄位, 將 object 資料填入各 Row
        //return 新的 dt
        //使用方法可參考： _4_10工令單查RO
        public static DataTable dtInsertColFromJsonCol(DataTable dt, string jsonCol, Dictionary<string, string> outputCol)
        {
            string tempJStr;
            List<Dictionary<string, string>> jsonLists;
            //檢查 dt 是否含有 要新加的欄位
            foreach (KeyValuePair<string, string> outItem in outputCol)
            {
                //Value 為 dt 的新增欄位
                if (!dt.Columns.Contains(outItem.Value))
                {
                    DataColumn newcolumn = new DataColumn();
                    newcolumn.ColumnName = outItem.Value;
                    newcolumn.DataType = System.Type.GetType("System.String");
                    dt.Columns.Add(newcolumn);
                }
            }

            //處理 Data 部分
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[jsonCol] == null || dr[jsonCol] == DBNull.Value)
                    continue;
                try
                {
                    jsonLists = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(dr[jsonCol].ToString());
                }
                catch (Exception ex)
                {
                    continue;
                }
                if (jsonLists == null)
                    continue;
                foreach (Dictionary<string, string> jObj in jsonLists)
                {
                    foreach (KeyValuePair<string, string> outItem in outputCol)
                    {
                        if (dr[outItem.Value] != DBNull.Value)
                            dr[outItem.Value] += "," + jObj[outItem.Key];
                        else
                            dr[outItem.Value] = jObj[outItem.Key];
                    }
                }
            }
            return dt;
        }


        //從 sourceDT 截取 部分欄位， 產生新的 DataTable
        //srcIdxDestColName<int, string>, int 為 sourceDT 的 欄位 idx,  string 為目標 DT的 Colname
        public static DataTable dtTodt(DataTable sourceDT, Dictionary<int, string> srcIdxDestColName)
        {
            // Create a table with abitrary columns for use with the example
            System.Data.DataTable destDT = new System.Data.DataTable();
            List<string> destColList = new List<string>();
            foreach (KeyValuePair<int, string> kvp in srcIdxDestColName)
            {
                destDT.Columns.Add(kvp.Value);
                destColList.Add(kvp.Value);
            }
            int i = 0;
            foreach (DataRow dr in sourceDT.Rows)
            {
                System.Data.DataRow row = destDT.NewRow();
                i = 0;
                foreach (KeyValuePair<int, string> kvp in srcIdxDestColName)
                {
                    row[i++] = dr[kvp.Key];
                }
                destDT.Rows.Add(row);
            }
            return destDT;
        }

        //File從 Row 0 開始算
        public static DataTable CsvToDataTable(string filename, string seperator, int titleRow = 0, int dataRowsStart = 1)
        {
            DataTable dt = new DataTable();
            bool isFirst = true;
            int currRow = -1;
            string tempColName = null;
            FileStream fs = null;
            try
            {
                using (fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                    {
                        while (!sr.EndOfStream)
                        {
                            var eachLineStr = sr.ReadLine();
                            currRow++;
                            if (currRow < titleRow)
                            {
                                continue;
                            }

                            string[] rows = eachLineStr.Split(new string[] { seperator }, StringSplitOptions.None);
                            //exclude first line
                            if (isFirst)
                            {
                                for (int i = 0; i < rows.Length; i++)
                                {
                                    if (string.IsNullOrEmpty(rows[i]))
                                        dt.Columns.Add();
                                    else
                                    {
                                        tempColName = rows[i].Replace("\0", string.Empty);
                                        dt.Columns.Add(tempColName);
                                    }

                                }
                                isFirst = false;
                                continue;
                            }
                            if (currRow < dataRowsStart)
                            {
                                continue;
                            }
                            //split string to array of string use seperator
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < rows.Length; i++)
                            {
                                dr[i] = rows[i];
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }
                fs.Close();
            }
            catch (Exception ex)
            {
                fs.Close();
            }
            return dt;
        }

        //兩個 DataTable 做 Join
        //目前版本 join key 只能是 int
        public static DataTable DataTablesJoin(DataTable dt1, DataTable dt2, int commonKey)
        {
            var query =
                from rHead in dt1.AsEnumerable()
                join rTail in dt2.AsEnumerable()
                on rHead.Field<int>(commonKey) equals rTail.Field<Int32>(commonKey)
                select rHead.ItemArray.Concat(rTail.ItemArray.Skip(1));
            DataTable DtAll = new DataTable();
            foreach (var obj in query)
            {
                DataRow dr = DtAll.NewRow();
                dr.ItemArray = obj.ToArray();
                DtAll.Rows.Add(dr);
            }
            return DtAll;
        }


        public static DataRow cloneDataRow(DataRow sourceRow, DataTable sourceDT)
        {
            var desRow = sourceDT.NewRow();
            desRow.ItemArray = sourceRow.ItemArray.Clone() as object[];
            return desRow;
        }

        //從 datatable 產生 insert sql 指令
        public static string getInserSQLFromDt(string tableName, DataTable dt, int copyIdx = 0, List<string> skipCols = null, string parentCol = null, string parentVal = null)
        {
            StringBuilder insertStr = new StringBuilder("insert into " + tableName + " (");
            List<string> tempListName = new List<string>();
            List<string> tempListData = new List<string>();
            if (dt.Rows.Count >= copyIdx + 1)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    if (skipCols != null && skipCols.Contains(dc.ColumnName))
                        continue;
                    if (dc.ColumnName == parentCol)
                    {
                        tempListName.Add(dc.ColumnName);
                        tempListData.Add("'" + parentVal + "'");
                    }
                    else
                    {
                        tempListName.Add(dc.ColumnName);
                        tempListData.Add("'" + dt.Rows[copyIdx][dc.ColumnName].ToString() + "'");
                    }
                }
                insertStr.Append(string.Join(",", tempListName)).Append(") Values (");
                insertStr.Append(string.Join(",", tempListData)).Append(")");
            }
            return insertStr.ToString();
        }

        public static string getInserSQLFromDt(string tableName, string sqlStr, string connString = Constants.ConnString, int copyIdx = 0, List<string> skipCols = null, string parentCol = null, string parentVal = null)
        {
            DataTable dt = getSQLDataTable(sqlStr, connString);
            return getInserSQLFromDt(tableName, dt, copyIdx, skipCols, parentCol, parentVal);
        }

        public static List<int> getInserSQLFromDt(string tableName, string sqlStr, string srcConnString = Constants.ConnString, string destConnString = Constants.ConnString,
                     List<string> skipCols = null, string parentCol = null, string parentVal = null)
        {
            DataTable dt = getSQLDataTable(sqlStr, srcConnString);
            string sqlInsert = null;
            List<int> insertedID = new List<int>();
            for (int k = 0; k < dt.Rows.Count; k++)
            {
                sqlInsert = CommonClass.getInserSQLFromDt(tableName, dt, k, skipCols, parentCol, parentVal);
                insertedID.Add(CommonClass.execSQLNonQuery(sqlInsert, destConnString, true));
            }
            return insertedID;
        }
        //Type:1, start with, 2:content, 3:end with
        public static string listToSQLWhere(string dbColName, List<string> condList, int type = 1)
        {
            string condStr = null;
            switch (type)
            {
                case 1://abc%
                    condStr = dbColName + " REGEXP '^" + string.Join("|^", condList.ToArray()) + "'";
                    break;
                case 2: //%abc%
                    condStr = dbColName + " REGEXP '" + string.Join("|", condList.ToArray()) + "'";
                    break;
                case 3: //%abc
                    condStr = dbColName + " REGEXP '" + string.Join("$|", condList.ToArray()) + "$'";
                    break;
            }
            return condStr;
        }

        public static double DataTableAvg(DataTable dt, string colName, string conditionExpression)
        {
            return (double)dt.Compute("Avg(" + colName + ")", conditionExpression);


            //min = (double)dt.Compute("Min(tmpColumn)", conditionExpression);
            //variance = (double)dt.Compute("Var(tmpColumn)", conditionExpression);
            //count = (int)dt.Compute("Count(tmpColumn)", conditionExpression);
        }

        public static double DataTableStdev(DataTable dt, string colName, string conditionExpression)
        {
            return (double)dt.Compute("Stdev(" + colName + ")", conditionExpression);
        }

        public static double DataTableMax(DataTable dt, string colName, string conditionExpression)
        {
            return (double)dt.Compute("Max(" + colName + ")", conditionExpression);
        }

        public static double DataTableMin(DataTable dt, string colName, string conditionExpression)
        {
            return Convert.ToDouble(dt.Compute("Min(" + colName + ")", conditionExpression));
        }
        public static double DataTableSum(DataTable dt, string colName, string conditionExpression)
        {

            var rtnVal = dt.Compute("Sum(" + colName + ")", conditionExpression);
            if (rtnVal == DBNull.Value)
                return 0;
            else
                return Convert.ToDouble(rtnVal);
        }

        //direction 0:asc, 1:Desc
        public static DataTable DataTableSort(DataTable oTable, string sortColumn = null, int direction = 0)
        {
            DataTable dt = new DataTable();
            oTable.DefaultView.Sort = sortColumn + (direction == 0 ? " asc" : " desc");
            dt = oTable.DefaultView.ToTable();
            return dt;
        }

        public static DataTable DataTableGroupBy(DataTable dt, string groupColumn, string orderByCol)
        {

            dt = dt.AsEnumerable()
                   .GroupBy(r => new { Col1 = r[groupColumn] })
                   .Select(g => g.OrderBy(r => r[orderByCol]).First())
                   .CopyToDataTable();
            return dt;
        }

        public static DataTable DataTableGroupBy(DataTable dt, int groupColIdx, int orderColIdx)
        {

            dt = dt.AsEnumerable()
                   .GroupBy(r => new { Col1 = r[groupColIdx] })
                   .Select(g => g.OrderBy(r => r[orderColIdx]).First())
                   .CopyToDataTable();
            return dt;
        }

        //使用方法: DataTable nTable = DataTableFilterSort(oTable, "SPEED='10'", "DIST asc");
        public static DataTable DataTableFilterSort1(DataTable oTable, string filterExpression = null, string sortExpression = null)
        {
            DataView dv = new DataView();
            oTable.TableName = "ABC";
            dv.Table = oTable;
            if (!string.IsNullOrEmpty(filterExpression))
                dv.RowFilter = filterExpression;
            if (!string.IsNullOrEmpty(sortExpression))
                dv.Sort = sortExpression;
            DataTable nTable = dv.ToTable();
            return nTable;
        }

        public static DataTable DataTableDistinct(DataTable oTable, List<string> Columns)
        {
            DataView dataView = oTable.DefaultView;
            return dataView.ToTable(true, Columns.ToArray());
        }

        //使用方法: DataTable nTable = DataTableGropeBy_SUM(oTable, Columns, "SPEED='10'");
        //Columns 必須含 2 個 Grouping 的欄位, 與一個計算欄位
        //例如: List<string> Columns = new List<string>() { "尺寸", "誤差值", "完成量" };
        public static DataTable DataTableGropeBy_SUM(DataTable oTable, List<string> Columns, string filterExpression = null)
        {
            //取得資料，PS. Get()是我自己寫的Function，要記得換成你取得DataTable的function

            //將原來的DataTable做Distinct並複製到新DataTable，ToTable的第一個參數是設定是否
            //要做Distinct，當然要設成true，其他參數是要做Group By的欄位名稱
            if (!string.IsNullOrEmpty(filterExpression))
            {
                oTable = DataTableFilterSort1(oTable, filterExpression);
            }
            string[] distinctArr = Columns.Take(Columns.Count() - 1).ToArray();
            DataTable dtGroup = oTable.DefaultView.ToTable(true, distinctArr);
            dtGroup.Columns.Add("SumColumn");

            int count = 0;
            double sumTotal;
            for (int i = 0; i < dtGroup.Rows.Count; i++)
            {
                if (dtGroup.Rows[i][0] == DBNull.Value)
                    continue;

                sumTotal = DataTableSum(oTable, Columns[Columns.Count - 1], distinctArr[0] + " = '" + dtGroup.Rows[i][0] +
"'" + (distinctArr.Length > 1 ? " and " + distinctArr[1] + " = '" + dtGroup.Rows[i][1] + "'" : ""));
                //double avgTotal = DataTableAvg(oTable, Columns[Columns.Count - 1], distinctArr[0] + " = '" + dtGroup.Rows[i][0] + "' and " + distinctArr[1] + " = '" + dtGroup.Rows[i][1] + "'");

                //設定資料
                dtGroup.Rows[i]["SumColumn"] = sumTotal.ToString();

            }
            return dtGroup;
        }


        //使用方法: DataTable nTable = DataTableFilterSort(oTable, "SPEED='10'", "DIST asc");
        public static DataTable DataTableFilterSort2(DataTable oTable, string filterExpression, string sortExpression)
        {
            try
            {
                DataTable nTable = oTable.Select(filterExpression, sortExpression).CopyToDataTable();
                return nTable;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<string> dtToList(DataTable dt, int colIdx = 0, bool ifSkipEmpty = true, bool distinct = false, List<string> rtnList = null)
        {
            string drData = null;
            if (rtnList == null)
                rtnList = new List<string>();
            foreach (DataRow myRow in dt.Rows)
            {
                drData = myRow[colIdx].ToString();
                if (string.IsNullOrEmpty(drData) || drData.ToLower() == "test")
                    continue;
                if (!string.IsNullOrEmpty(drData))
                {
                    if (distinct && rtnList.Contains(drData))
                        continue;
                    rtnList.Add(myRow[colIdx].ToString());
                }
            }
            return rtnList;
        }
        public static List<string> dtToList(DataTable dt, int colIdx = 0)
        {
            List<string> rtnList = new List<string>();
            foreach (DataRow myRow in dt.Rows)
            {
                rtnList.Add(myRow[colIdx].ToString());
            }
            return rtnList;
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
        public static List<double> dtToDoubleList(DataTable dt, string columnName = null, bool distinct = false, bool sort = false)
        {
            if (dt == null)
                return null;
            if (columnName == null)
                columnName = dt.Columns[0].ColumnName;
            if (!dt.Columns.Contains(columnName))
                return null;
            List<double> rtnList = new List<double>();
            if (columnName != null)
                foreach (DataRow myRow in dt.Rows)
                {
                    try
                    {
                        rtnList.Add(Convert.ToDouble(myRow[columnName]));
                    }
                    catch (Exception ex)
                    {
                        rtnList.Add(0);
                    }

                }
            if (sort)
                rtnList.Sort();
            return rtnList;
        }
        public static List<Int32> dtToIntList(DataTable dt, string columnName = null, bool distinct = false, bool sort = false)
        {
            if (dt == null)
                return null;
            if (columnName == null)
                columnName = dt.Columns[0].ColumnName;
            if (!dt.Columns.Contains(columnName))
                return null;
            List<Int32> rtnList = new List<Int32>();
            if (columnName != null)
                foreach (DataRow myRow in dt.Rows)
                {
                    try
                    {
                        rtnList.Add(Convert.ToInt32(myRow[columnName]));
                    }
                    catch (Exception ex)
                    {
                        rtnList.Add(0);
                    }

                }
            if (sort)
                rtnList.Sort();
            return rtnList;
        }
        //轉置 dt, 將 Col 轉為 Row, Row 轉為 Col
        public static DataTable flipDt(DataTable sourceDT, string firstColName,
               List<string> ColNames, List<string> firstColData)
        {
            DataTable destDT = new DataTable();
            destDT.Columns.Add(firstColName);
            int i = 0;
            foreach (string colname in ColNames)
            {
                destDT.Columns.Add(colname);
            }
            //Source 的 ColName 當作 Dest 的第一個 Col 的 Data
            foreach (string colData in firstColData)
            {

                destDT.Rows.Add();
                destDT.Rows[i++][0] = colData;
            }

            i = 0;
            foreach (string colData in firstColData)
            {
                for (int j = 0; j < sourceDT.Rows.Count; j++)
                {
                    destDT.Rows[i][j + 1] = sourceDT.Rows[j][i + 1];
                }
                i++;
            }
            return destDT;
        }

        public static DataRow searchDataTable(System.Data.DataTable dt, string searchColumn, string searchTarget)
        {
            String targetName = null;
            foreach (DataRow myRow in dt.Rows)
            {
                try
                {
                    targetName = myRow[searchColumn].ToString();
                }
                catch (Exception ex)
                {
                    continue;
                }
                if (targetName == searchTarget)
                    return myRow;
            }
            return null;
        }

        public static int searchInDTRtnIdx(DataTable dt, string whereStr)
        {
            int index = -1;
            DataRow[] rows = searchInDT(dt, whereStr);
            if (rows.Count() > 0)
            {
                index = dt.Rows.IndexOf(rows[0]);
            }
            return index;
        }

        //return -1 : 找不到
        //如果找到 return 第一個符合條件的 RowIdx
        public static int searchFirstIndt(System.Data.DataTable dt, Dictionary<string, string> searchKeys)
        {
            int rowIdx = 0;
            bool boolIdentical = true;
            foreach (DataRow myRow in dt.Rows)
            {
                boolIdentical = true;
                foreach (KeyValuePair<string, string> keyItem in searchKeys)
                {
                    if (myRow[keyItem.Key].ToString() != keyItem.Value)
                    {
                        boolIdentical = false;
                        break;
                    }
                }
                if (boolIdentical)
                    return rowIdx;
                else
                    rowIdx++;
            }
            return -1;
        }

        //回傳找到的 Row index List
        public static List<int> searchAllIndt(System.Data.DataTable dt, Dictionary<string, string> searchKeys)
        {
            List<int> rtnList = new List<int>();
            int rowIdx = 0;
            bool boolIdentical = true;
            foreach (DataRow myRow in dt.Rows)
            {
                boolIdentical = true;
                foreach (KeyValuePair<string, string> keyItem in searchKeys)
                {
                    if (myRow[keyItem.Key].ToString() != keyItem.Value)
                    {
                        boolIdentical = false;
                        break;
                    }
                }
                if (boolIdentical)
                    rtnList.Add(rowIdx);
                rowIdx++;
            }
            return rtnList;
        }

        //找出 nameList 有而 DataTable 裡面沒有的名單
        public static List<string> listExtrainDT(System.Data.DataTable dt, List<string> nameList, string searchColumn)
        {
            List<string> outputList = new List<string>();
            DataRow dr = null;
            foreach (string empID in nameList)
            {
                dr = searchDataTable(dt, searchColumn, empID);
                if (dr == null)
                    outputList.Add(empID);
            }
            return outputList;
        }

        //找出 DataTable 有而 nameList 裡面沒有的名單
        public static List<string> dtExtraList(System.Data.DataTable dt, List<string> nameList, string searchColumn)
        {
            List<string> outputList = new List<string>();
            foreach (DataRow myRow in dt.Rows)
            {
                if (nameList.Contains(myRow[searchColumn].ToString()))
                    outputList.Add(myRow[searchColumn].ToString());
            }
            return outputList;
        }

        public static List<string> dtColumnIntoList(System.Data.DataTable dt, string columnName)
        {
            //List<string> list = dt.AsEnumerable().Select(r => r.Field<object>(columnName).ToString()).ToList();
            List<string> list = dt.Rows.OfType<DataRow>()
                .Select(dr => dr.Field<string>(columnName)).ToList();
            return list;
        }

        //Ajust DateTime formate 
        public static string dtChangeDateTimeFormate(DataRow row, string colName, string dateFormate = "yyyy/MM/dd HH:mm")
        {
            //if (ds.Tables[0].Columns[j].DataType.Name == "DateTime")
            DateTime dt = DateTime.Parse(row[colName].ToString());
            return dt.ToString(dateFormate);
        }

        public static double dtAccumulate(DataTable dt, string sorceCol, string destCol, double initVal = 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                initVal += changeType<double>(dr[sorceCol].ToString());
                dr[destCol] = initVal.ToString("0.#");
            }
            return initVal;
        }

        //執行 Store Procedure
        //簡單 call storeprocesure
        public static DataTable spRtnDT(string nom_fonction, Dictionary<string, object> parametros = null, string connStr = Constants.ConnString)
        {
            DataTable dt = new DataTable();
            MySqlConnection connection = new MySqlConnection(connStr);
            MySqlDataAdapter da = new MySqlDataAdapter(nom_fonction, connection);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            if (parametros != null && parametros.Count > 0)
            {
                //existen parámetros los recorremos y agregamos.
                foreach (KeyValuePair<string, object> pars in parametros)
                {
                    da.SelectCommand.Parameters.Add(new MySqlParameter(pars.Key, pars.Value));
                    da.SelectCommand.Parameters[pars.Key].Direction = System.Data.ParameterDirection.Input;
                }
            }
            da.Fill(dt);
            connection.Close();
            return dt;
        }

        public static object execStoreProcedure(string storeProcedure, Dictionary<string, object> parametros = null, string connString = Constants.ConnString)
        {
            string nombreOut = "nombreOut";
            object tipoParOut = null;
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                using (MySqlCommand cmd = new MySqlCommand(storeProcedure, conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    //Se abre la conexión
                    conn.Open();
                    //verificamos si se mando la lista de parámetros
                    if (parametros != null && parametros.Count > 0)
                    {
                        //existen parámetros los recorremos y agregamos.
                        foreach (KeyValuePair<string, object> pars in parametros)
                        {
                            cmd.Parameters.Add(new MySqlParameter(pars.Key, pars.Value));
                            cmd.Parameters[pars.Key].Direction = System.Data.ParameterDirection.Input;
                        }
                    }
                    cmd.Parameters.Add(new MySqlParameter(nombreOut, tipoParOut));
                    cmd.Parameters[nombreOut].Direction = System.Data.ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return cmd.Parameters[nombreOut].Value;
                }
            }
        }

        public static bool execSQLNonQueryTransactList(List<string> sqlStrList, string connString = Constants.ConnString)
        {
            bool rtnBool = false;
            MySqlConnection cnnSQL = new MySqlConnection(connString);
            cnnSQL.Open();//開啟SqlConnection物件
            MySqlCommand cmdSQL = new MySqlCommand();
            MySqlTransaction transaction;
            transaction = cnnSQL.BeginTransaction();
            cmdSQL.Connection = cnnSQL;
            cmdSQL.Transaction = transaction;
            try
            {
                foreach (string sqlStr in sqlStrList)
                {
                    execSQLNonQueryForTransact(sqlStr, cmdSQL, false);
                }
                transaction.Commit();
                rtnBool = true;
            }
            catch (Exception ex)
            {
                rtnBool = false;
                // Attempt to roll back the transaction.
                try
                {
                    transaction.Rollback();
                    throw ex;
                }
                catch (Exception ex2)
                {
                    throw ex2;
                }
            }
            finally
            {
                cnnSQL.Close();
            }
            return rtnBool;
        }

        //這個函數是為了使用 Transaction 
        //bool LAST_INSERT_ID  -- true:回覆 第一筆的 ID 值, false:回覆筆數
        public static int execSQLNonQueryForTransact(String sqlCommand, MySqlCommand cmdSQL, bool LAST_INSERT_ID = false)
        {
            int rtnInt = 0;
            IDataReader reader = null;
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
            return rtnInt;
        }
        public static DataTable changeDataTableType(DataTable dt, int colIdx, Type dataType)
        {
            DataTable dtCloned = dt.Clone();
            dtCloned.Columns[colIdx].DataType = dataType;
            foreach (DataRow row in dt.Rows)
            {
                dtCloned.ImportRow(row);
            }
            return dtCloned;
        }

        //public static string objectToJson(Object obj)
        //{
        //    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        //    return serializer.Serialize(obj);
        //}

        public static string DataTableToJsonObj(DataTable dt)
        {
            DataSet ds = new DataSet();
            ds.Merge(dt);
            StringBuilder JsonString = new StringBuilder();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                JsonString.Append("[");
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                    {
                        if (j < ds.Tables[0].Columns.Count - 1)
                        {
                            JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString() + "\",");
                        }
                        else if (j == ds.Tables[0].Columns.Count - 1)
                        {
                            JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == ds.Tables[0].Rows.Count - 1)
                    {
                        JsonString.Append("}");
                    }
                    else
                    {
                        JsonString.Append("},");
                    }
                }
                JsonString.Append("]");
                return JsonString.ToString();
            }
            else
            {
                return null;
            }
        }

        public static List<object> datarowToList(DataRow dr)
        {
            List<object> list = dr.ItemArray.ToList();
            return list;
        }

        public static Dictionary<string, object> DataRowToDictionary(DataRow datarow, List<string> ignorClos = null)
        {
            var dict = new Dictionary<string, object>();
            foreach (DataColumn col in datarow.Table.Columns)
            {
                if (ignorClos != null && ignorClos.Contains(col.ColumnName))
                    continue;
                dict.Add(col.ColumnName, datarow[col]);
            }
            return dict;
        }

        public static int InsertByDict(string tableName, Dictionary<string, object> dataSource, string connStr = null)
        {
            List<string> colNameList = new List<string>();
            List<string> colDataList = new List<string>();
            foreach (KeyValuePair<string, object> dictItem in dataSource)
            {
                colNameList.Add(dictItem.Key);
                colDataList.Add(dictItem.Value.ToString());
            }
            string sqlStr = $"insert into {tableName} ({string.Join(",", colNameList)}) " +
                $" values('{string.Join("' , '", colDataList)}')";
            if (connStr == null)
                connStr = Constants.ConnString;
            return CommonClass.execSQLNonQuery(sqlStr, connStr, true);
        }

        //回傳 新舊 Table ID 的 Mapping
        //keyCol 作用為傳回 KeyDictionary, 其值為 null, 則不傳回 Dictionary
        ////Key:舊 mfoID, Value:新 mfoID
        public static Dictionary<string, string> InsertByDt(string tableName, string keyCol, DataTable sourceDt, List<string> ignorClos = null, string connStr = null, Dictionary<string, string> staticDict = null)
        {
            List<string> allColNameList = new List<string>();
            List<string> colNameList = new List<string>();
            List<string> colDataList = new List<string>();
            Dictionary<string, string> rtnDict = new Dictionary<string, string>();
            string newKey = null;
            int idx = -1;
            if (connStr == null)
                connStr = Constants.ConnString;
            foreach (DataColumn col in sourceDt.Columns)
            {
                allColNameList.Add(col.ColumnName);
                if (ignorClos != null && ignorClos.Contains(col.ColumnName))
                    continue;
                colNameList.Add(col.ColumnName);
            }
            string sqlStr = $"insert into {tableName} ({string.Join(",", colNameList)}) ";

            foreach (DataRow dr in sourceDt.Rows)
            {
                colDataList.Clear();
                idx = -1;
                foreach (object oObject in dr.ItemArray)
                {
                    idx++;
                    if (ignorClos != null && ignorClos.Contains(allColNameList[idx]))
                        continue;
                    if (staticDict != null && staticDict.Keys.Contains(allColNameList[idx]))
                        colDataList.Add(staticDict[allColNameList[idx]]);
                    else
                        colDataList.Add(oObject.ToString());
                }

                if (string.IsNullOrEmpty(keyCol))
                    continue;
                newKey = CommonClass.execSQLNonQuery(sqlStr + $" values('{string.Join("','", colDataList)}')", connStr, true).ToString();
                rtnDict.Add(dr[keyCol].ToString(), newKey);
            }
            return rtnDict;
        }

        public static Dictionary<string, object> dtToListDict(DataTable dt, List<string> ignoreCols = null)
        {
            Dictionary<string, object> rtnDictionary = null;
            List<Dictionary<string, object>> rtnList = new List<Dictionary<string, object>>();
            foreach (DataRow myRow in dt.Rows)
                rtnList.Add(DataRowToDictionary(myRow, ignoreCols));
            return rtnDictionary;
        }


        /// <summary>
        /// Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (DataRow row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        // whereStr 例如 "X1='abc' OR X2 is null"
        public static DataRow[] searchInDT(DataTable dt, string whereStr)
        {
            DataRow[] filteredRows = dt.Select(whereStr);
            return filteredRows;
        }

        public static DataTable dicttoDT<T>(List<Dictionary<string, T>> list_dict)
        {
            if (list_dict.Count == 0)
                return null;

            var dt = new DataTable();
            try
            {

            }
            catch (Exception ex)
            {
                return null;
            }
            foreach (KeyValuePair<string, T> entry in list_dict[0])
            {
                dt.Columns.Add(entry.Key, typeof(T));
            }

            foreach (Dictionary<string, T> dict in list_dict)
            {
                DataRow row = dt.NewRow();
                foreach (KeyValuePair<string, T> entry in dict)
                {
                    row[entry.Key] = entry.Value;
                }
                dt.Rows.Add(row);
            }
            dt.AcceptChanges();
            return dt;
        }

        public static Dictionary<string, string> dtToDictionary(DataTable dt, string keyCol, string valueCol)
        {
            Dictionary<string, string> rtnDictionary = new Dictionary<string, string>();
            foreach (DataRow myRow in dt.Rows)
            {
                rtnDictionary.Add(myRow[keyCol].ToString(), myRow[valueCol].ToString());
            }
            return rtnDictionary;
        }

        public static int getDictionaryIntValueByStrKey(string key, Dictionary<Object, Object> perList)
        {
            int rtnInt = -1;
            foreach (KeyValuePair<object, object> entry in perList)
            {
                if (entry.Key.ToString() == key)
                    return Convert.ToInt32(entry.Value);
            }
            return rtnInt;
        }

        public static Dictionary<Object, Object> getDataDictionary(String sqlCommand, String keyColumn, String valueCol, String ConnString = Constants.ConnString)
        {
            Dictionary<Object, Object> rtnDictionary = new Dictionary<Object, Object>();
            DataTable dt = getSQLDataTable(sqlCommand);
            foreach (DataRow dtRow in dt.Rows)
            {
                rtnDictionary.Add(dtRow[keyColumn], dtRow[valueCol]);
            }
            return rtnDictionary;
        }

        public static DataTable getSQLDataTable(String sqlCommand, String ConnString = Constants.ConnString)
        {
            using (MySqlConnection SQLCON = new MySqlConnection(ConnString))
            {
                SQLCON.Open();

                MySqlDataAdapter da = new MySqlDataAdapter(sqlCommand, SQLCON);
                DataTable dt = new DataTable();
                da.Fill(dt);
                SQLCON.Close();
                return dt;
            }
        }

        //jsonType 0:Dictionary<string,string>. 1:array of dictionary<string,string>
        public static DataTable extractJsonFromCol(DataTable dt, string colName, string exctractJson, int jsonType = 0)
        {
            string jsonData;
            List<string> roList = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[colName] == DBNull.Value || dr[colName].ToString() == "")
                    continue;
                jsonData = dr[colName].ToString();
                if (jsonType == 0)
                    dr[colName] = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData)[exctractJson];
                else
                {
                    foreach (Dictionary<string, string> jDict in JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonData))
                    {
                        roList.Add(jDict[exctractJson]);
                    }
                    dr[colName] = string.Join(",", roList);
                    roList.Clear();
                }
            }
            return dt;
        }

        public static string combineParams(String sqlCommand, Dictionary<string, string> sqlparams)
        {
            string rtnStr = sqlCommand;
            foreach (var paramItem in sqlparams)
            {
                rtnStr = rtnStr.Replace(paramItem.Key, "'" + paramItem.Value + "'");
            }
            return rtnStr;
        }

        public static DataTable getSQLDataTableParams(String sqlCommand, Dictionary<string, string> sqlparams, String ConnString = Constants.ConnString)
        {
            sqlCommand = combineParams(sqlCommand, sqlparams);
            return getSQLDataTable(sqlCommand, ConnString);
        }

        //若無資料 dbResult != DBNull.Value
        public static Object getSQLScalar(String sqlCommand, String ConnString = Constants.ConnString)
        {
            Object rtnString = null;
            using (MySqlConnection SQLCON = new MySqlConnection(ConnString))
            {
                SQLCON.Open();
                MySqlCommand cmd = SQLCON.CreateCommand();

                cmd.CommandText = sqlCommand;

                rtnString = cmd.ExecuteScalar();
                SQLCON.Close();
            }
            return rtnString;
        }

        public static Object getSQLScalarParams(String sqlCommand, Dictionary<string, string> sqlparams, String ConnString = Constants.ConnString)
        {
            Object rtnObj = null;
            using (MySqlConnection SQLCON = new MySqlConnection(ConnString))
            {
                SQLCON.Open();
                MySqlCommand cmd = SQLCON.CreateCommand();
                cmd.CommandText = sqlCommand;
                try
                {
                    foreach (var paramItem in sqlparams)
                    {
                        cmd.Parameters.AddWithValue(paramItem.Key, paramItem.Value);
                    }
                    rtnObj = cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, "SQL Error:" + sqlCommand);
                }
                finally
                {
                    SQLCON.Close();
                }
            }
            return rtnObj;
        }

        public static int execSQLFromArr(string[][] dataArr, String strSQL, String ConnString = Constants.ConnString, int startRow = 0, int endRow = 0)
        {
            //匯入資料庫前，將資料表清空
            int sqlcount = 0;
            using (MySqlConnection conn = new MySqlConnection(ConnString))
            {
                conn.Open();
                String values = "(";
                for (int i = 0; i < dataArr[0].Length; i++)
                    values += "'{" + i + "}', ";
                values = values.Substring(0, values.Length - 2) + ")";
                for (int i = startRow; i < dataArr.Length - endRow; i++)
                {
                    string[] temp = dataArr[i];
                    if (temp.Length != 21)
                        throw new ArgumentNullException("請務必確認出勤明細檔已加入最後一欄：備註欄！！！");
                    strSQL += string.Format(values, temp[0].Trim(), temp[1].Trim(), temp[2].Trim(), temp[3].Trim(), temp[4].Trim(),
                        temp[5].Trim(), temp[6].Trim(), temp[7].Trim(), temp[8].Trim(), temp[9].Trim(),
                        temp[10].Trim(), temp[11].Trim(), temp[12].Trim(), temp[13].Trim(), temp[14].Trim(),
                        temp[15].Trim(), temp[16].Trim(), temp[17].Trim(), temp[18].Trim(), temp[19].Trim(), temp[19].Trim(), temp[20].Trim());
                    if (i == dataArr.Length - 2)
                        strSQL += ";";
                    else
                        strSQL += ",";
                }
                MySqlCommand cmd = new MySqlCommand(strSQL, conn);
                sqlcount = cmd.ExecuteNonQuery();
                conn.Close();
            }

            return sqlcount;
        }

        //執行 SQL 的 insert/update, 回傳 ID 或是 筆數
        //bool LAST_INSERT_ID  -- true:回覆 第一筆的 ID 值, false:回覆筆數
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

        //sqlCommand : 例如 INSERT INTO room(person,address) VALUES(@person, @address)
        //dictionary 的 Key/Value  例如 {"@person", "Scott"}
        public static int execSQLNonQueryParams(String sqlCommand, Dictionary<string, string> sqlparams, String connection = Constants.ConnString, bool LAST_INSERT_ID = false)
        {
            int rtnInt = -1;
            MySqlConnection cnSQL = new MySqlConnection(connection);
            MySqlCommand cmdSQL = new MySqlCommand();
            cmdSQL.Connection = cnSQL;
            cnSQL.Open();//開啟SqlConnection物件
            IDataReader reader = null;
            MySqlCommand comm = cnSQL.CreateCommand();
            comm.CommandText = sqlCommand;
            try
            {
                foreach (var paramItem in sqlparams)
                {
                    comm.Parameters.AddWithValue(paramItem.Key, paramItem.Value);
                }
                rtnInt = comm.ExecuteNonQuery();
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


        #region tempTable / temp Table
        //string AttendTempTableColumns = "DEPT VARCHAR(10), EMPID VARCHAR(10)"
        public static string creTempTable(string TableName, string TableColumns, String ConnString = Constants.ConnString)
        {
            string TableNameIP;
            TableNameIP = genTempTableName(TableName);
            try
            {
                dropTable(TableNameIP);
                string selectCmd = "CREATE TABLE " + TableNameIP + " (" + TableColumns + ") ENGINE=INNODB DEFAULT CHARSET=UTF8;";
                using (MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(selectCmd, conn);
                    int sqlcount = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (MySqlException err)
            {
                Console.WriteLine(err.Message, "creTempTable");
            }

            return TableNameIP;
        }

        public static void dropTable(String tableName, String ConnString = Constants.ConnString)
        {

            MySqlConnection con = null;
            try
            {
                con = new MySqlConnection(ConnString);
                con.Open(); //open the connection        
                String cmdText = @"drop table " + tableName;
                MySqlCommand cmd = new MySqlCommand(cmdText, con);
                cmd.Prepare();
                cmd.ExecuteNonQuery(); //execute the mysql command
            }
            catch (MySqlException err)
            {
                String outp = err.ToString();
                Console.WriteLine("Error: " + err.ToString());
            }
            finally
            {
                if (con != null)
                {
                    con.Close(); //close the connection
                }
            } //remember to close the connection after accessing the database
        }
        #endregion

        //產生中位數
        //ifUpper 如果筆數是雙號： true, 傳回高值, false 傳回低值
        public static DataRow genMidian(DataTable dt, string columnName, bool ifUpper = true)
        {
            if (dt == null || dt.Rows.Count == 0)
                return null;
            DataTable sortedDt = DataTableSort(dt, columnName);
            int rowID = sortedDt.Rows.Count / 2;
            if (sortedDt.Rows.Count % 2 == 0)
            {
                if (ifUpper)
                    return dt.Rows[rowID + 1];
                else
                    return dt.Rows[rowID];
            }
            else
                return dt.Rows[rowID + 1];



        }

        //用SQL查詢單筆字串，無資料則傳回字串"0", by Michael
        public static string GetString(String sqlCommand, String ConnString = Constants.ConnString)
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

        //將 datatable 轉成 HTML table string
        public static string ExportDatatableToHtml(DataTable dt)
        {
            System.Text.StringBuilder strHTMLBuilder = new System.Text.StringBuilder();
            strHTMLBuilder.Append("<html >");
            strHTMLBuilder.Append("<head>");
            strHTMLBuilder.Append("</head>");
            strHTMLBuilder.Append("<body>");
            strHTMLBuilder.Append("<table border='1px' cellpadding='1' cellspacing='1' bgcolor='lightyellow' style='font-family:Garamond; font-size:smaller'>");

            strHTMLBuilder.Append("<tr >");
            foreach (DataColumn myColumn in dt.Columns)
            {
                strHTMLBuilder.Append("<td >");
                strHTMLBuilder.Append(myColumn.ColumnName);
                strHTMLBuilder.Append("</td>");

            }
            strHTMLBuilder.Append("</tr>");


            foreach (DataRow myRow in dt.Rows)
            {

                strHTMLBuilder.Append("<tr >");
                foreach (DataColumn myColumn in dt.Columns)
                {
                    strHTMLBuilder.Append("<td >");
                    strHTMLBuilder.Append(myRow[myColumn.ColumnName].ToString());
                    strHTMLBuilder.Append("</td>");

                }
                strHTMLBuilder.Append("</tr>");
            }

            //Close tags.  
            strHTMLBuilder.Append("</table>");
            strHTMLBuilder.Append("</body>");
            strHTMLBuilder.Append("</html>");

            string Htmltext = strHTMLBuilder.ToString();

            return Htmltext;
        }

        public static DataRow deleteDataTable(System.Data.DataTable dt, string searchColumn, string searchTarget)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = dt.Rows[i];
                if (dr.RowState.ToString() == "Deleted")
                    continue;
                if (dr[searchColumn].ToString() == searchTarget)
                    dr.Delete();
            }
            return null;
        }
        public static DataTable removedtRowsByList(System.Data.DataTable dt, List<string> nameOrder, string searchColumn)
        {
            DataRow dr;
            foreach (string empID in nameOrder)
            {
                deleteDataTable(dt, searchColumn, empID);
            }
            return dt;
        }

        public static bool checkDB_Conn(string conn_info, out string rtnStr)
        {
            bool rtnBool = false;
            rtnStr = null;
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(conn_info);
                conn.Open();
                rtnStr = null;
                rtnBool = true;
            }
            catch (ArgumentException a_ex)
            {
                rtnStr = a_ex.Message;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    //http://dev.mysql.com/doc/refman/5.0/en/error-messages-server.html
                    case 1042: // Unable to connect to any of the specified MySQL hosts (Check Server,Port)
                        rtnStr = "Unable to connect to any of the specified MySQL hosts (Check Server,Port)";
                        break;
                    case 0: // Access denied (Check DB name,username,password)
                        rtnStr = "Access denied (Check DB name,username,password)";
                        break;
                    default:
                        rtnStr = null;
                        break;
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return rtnBool;
        }

        //當 compareCol 的前後兩筆 內容不一樣時， 插入一筆 空白 Row
        //例如 insertEmptyRow(dt, "size")
        //當 size 0.8x1.9 -> 1.7x5.4, 則插入一筆空白
        public static DataTable insertEmptyRow(DataTable dt, string compareCol)
        {
            string currKey = null;
            int rowIdx = 0;
            if (!dt.Columns.Contains(compareCol))
                return dt;
            List<int> idxToInsert = new List<int>();
            foreach (DataRow dr in dt.Rows)
            {
                if (currKey != null && currKey != dr[compareCol].ToString())
                {
                    idxToInsert.Add(rowIdx);
                }
                currKey = dr[compareCol].ToString();
                rowIdx++;
            }
            idxToInsert.Reverse();

            foreach (int idx in idxToInsert)
            {
                DataRow newBlankRow = dt.NewRow();
                dt.Rows.InsertAt(newBlankRow, idx);
            }
            return dt;
        }

        //從 sqlStr 萃取 Where 的 內容
        // 例如： select a,b,c from a1 where date='2020-01-01' order by a ==> where date='2020-01-01' 
        public static string getWhereInSQL(string sqlStr, int srcStartIdx = 0)
        {
            sqlStr = sqlStr.ToLower();
            int startIdx = sqlStr.IndexOf("where", srcStartIdx);
            int endIdx = 0;
            if (sqlStr.IndexOf("order by", startIdx) >= 0)
            {
                endIdx = sqlStr.IndexOf("order by", startIdx);
            }
            else if (sqlStr.IndexOf("group by", startIdx) >= 0)
                endIdx = sqlStr.IndexOf("order by", startIdx);

            if (endIdx == 0)
                return sqlStr.Substring(startIdx);
            else
                return sqlStr.Substring(startIdx, endIdx - startIdx);


            return null;
        }
        #endregion
        #region DB backup restore（中文有問題， 必須用別的方法）
        //https://stackoverflow.com/questions/12311492/backing-up-database-in-mysql-using-c-sharp
        //需到 nuget 安裝 MySqlBackup.NET
        public static void dbBackup(String bkFile, String connString)
        {
            //string constring = "server=localhost;user=root;pwd=qwerty;database=test;";
            //string bkFile = "C:\\backup.sql";
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ExportToFile(bkFile);
                        conn.Close();
                    }
                }
            }
        }

        public static bool execSQLFile(String bkFile, String connString, ref string rtnStr)
        {
            bool rtnBool = false;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            mb.ImportFromFile(bkFile);
                            conn.Close();
                        }
                        rtnBool = true;
                        rtnStr = null;
                    }
                }
            }
            catch (Exception ex)
            {
                rtnStr = ex.Message;
                rtnBool = false;
            }
            return rtnBool;
        }
        #endregion

        #region datetime
        //兩個時間之間差幾分鐘
        public static double timeBetweenInMinutes(DateTime startTime, DateTime endTime)
        {
            TimeSpan span = endTime.Subtract(startTime);
            return span.TotalMinutes;
        }

        //strStartTime 例如 09:00, strEndTime:10:00
        public static double sameDaytimeBetweenInMinutes(string strStartTime, string strEndTime)
        {
            try
            {
                DateTime startTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + strStartTime);
                DateTime endTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + strEndTime);
                return timeBetweenInMinutes(startTime, endTime);
            }
            catch (Exception ex)
            {
                return -1;
            }

        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        /// <summary>
        /// To the full taiwan date.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        public static string ToFullTaiwanDate(this DateTime datetime, int dtype = 0)
        {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();
            string dateStr;
            if (dtype == 0)
                dateStr = "民國 {0} 年 {1} 月 {2} 日";
            else if (dtype == 1)
                dateStr = "中華民國 {0} 年 {1} 月 {2} 日";
            else
                dateStr = " {0} 年 {1} 月 {2} 日";
            return string.Format(dateStr,
                taiwanCalendar.GetYear(datetime),
                datetime.Month,
                datetime.Day);
        }

        public static string ToSimpleTaiwanDate(this DateTime datetime)
        {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();
            return string.Format("{0}/{1}/{2}",
                taiwanCalendar.GetYear(datetime),
                datetime.Month,
                datetime.Day);
        }

        //傳回基礎月份的第一天
        public static DateTime firstDayOfMonth(DateTime baseDate)
        {
            return new DateTime(baseDate.Year, baseDate.Month, 1);
        }

        //傳回基礎月份的最後一天
        public static DateTime lastDayOfMonth(DateTime baseDate)
        {
            return firstDayOfMonth(baseDate).AddMonths(1).AddDays(-1);
        }
        //Date time ------------------------------------------------------
        //dateFormate, default "yyyy/MM/dd"
        public static void seDatePickerDate(System.Windows.Forms.DateTimePicker tpk, String dateString, String dateFormate = "yyyy/MM/dd")
        {
            tpk.Checked = true;
            //tpk.Value = DateTime.ParseExact(dateString, dateFormate, CultureInfo.InvariantCulture);
            tpk.Value = Convert.ToDateTime(dateString);
            if (tpk.Handle == null)
                System.Threading.Thread.Sleep(0);
            tpk.Checked = false;
        }

        //是否為放假日
        public static bool isHoliday(DateTime theDay)
        {
            bool rtnBool = false;
            String sqlStr = "SELECT `HTYPE` FROM `MFO_HOLIDAYS` WHERE `HDate` ='" + theDay.ToString("yyyy-MM-dd") + "'";

            Object hType = getSQLScalar(sqlStr);
            if (hType != null && hType.ToString() == "放假日")
                rtnBool = true;
            String dayOfWeek = theDay.DayOfWeek.ToString();
            if (!rtnBool && (dayOfWeek == "Saturday" || dayOfWeek == "Sunday"))
            {
                rtnBool = true;
            }
            return rtnBool;
        }

        //是否為放假日
        public static bool isHoliday(DateTime theDay, bool includeWeekend = false)
        {
            return false;
            if (includeWeekend && ((int)theDay.DayOfWeek == 6 || (int)theDay.DayOfWeek == 0))
                return true;

            DateTime workday = DateTime.Now;
            string sqlStr = $"SELECT `isHoliday` FROM `calendar` WHERE `CDATE` = {theDay.ToString("yyyy-MM-dd")}";
            var holidayObj = CommonClass.getSQLScalar(sqlStr, Constants.ACLConnString);
            if (holidayObj == null || holidayObj == DBNull.Value)
                return false;
            int holidayFlag = Convert.ToInt32(holidayObj);
            if (holidayFlag == 1)
                return true;
            else
                return false;
        }

        //是否為工作日
        public static bool isWorkday(DateTime theDay)
        {
            bool rtnBool = false;
            String sqlStr = "SELECT `HTYPE` FROM `MFO_HOLIDAYS` WHERE `HDate` ='" + theDay.ToString("yyyy-MM-dd") + "'";

            Object hType = getSQLScalar(sqlStr);
            if (hType != null && hType.ToString() == "工作日")
                rtnBool = true;
            String dayOfWeek = theDay.DayOfWeek.ToString();
            if (!rtnBool && !(dayOfWeek == "Saturday" || dayOfWeek == "Sunday"))
            {
                rtnBool = true;
            }
            return rtnBool;
        }

        //從 beginDate 開始算， workdays 個工作天 的 日期        
        public static DateTime workdaysAfter(DateTime beginDate, int workdays)
        {
            for (int i = 0; i < workdays;)
            {
                beginDate = beginDate.AddDays(1);
                if (isHoliday(beginDate))
                    continue;
                else
                    i++;
            }
            return beginDate;
        }

        public static int workDaysBetween(DateTime firstDay, DateTime lastDay)
        {
            int rtnValue = 0;
            for (DateTime beginDay = firstDay; beginDay <= lastDay; beginDay = beginDay.AddDays(1))
            {
                if (isWorkday(beginDay))
                    rtnValue++;
            }
            return rtnValue;
        }

        /// <summary>
        /// Calculates number of business days, taking into account:
        ///  - weekends (Saturdays and Sundays)
        ///  - bank holidays in the middle of the week
        /// </summary>
        /// <param name="firstDay">First day in the time interval</param>
        /// <param name="lastDay">Last day in the time interval</param>
        /// <param name="bankHolidays">List of bank holidays excluding weekends</param>
        /// <returns>Number of business days during the 'span'</returns>
        // 計算兩個日期之間有多少個工作日
        public static int businessDaysUntil(DateTime firstDay, DateTime lastDay, DateTime[] holidays)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = (int)firstDay.DayOfWeek;
                int lastDayOfWeek = (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of bank holidays during the time interval
            if (holidays != null)
            {
                foreach (DateTime holiday in holidays)
                {
                    DateTime bh = holiday.Date;
                    if (firstDay <= bh && bh <= lastDay)
                        --businessDays;
                }
            }
            return businessDays;
        }

        //計算上個月的天數, queryDate 輸入日期， 例如：2018-06-12
        public static int daysInLastMonth(string queryDate)
        {
            DateTime theDate;
            if (!DateTime.TryParse(queryDate, out theDate))
                return -1;
            theDate = theDate.AddMonths(-1);
            return System.DateTime.DaysInMonth(theDate.Year, theDate.Month); //上個月的天數

        }

        //inputType : 0- 年/月, 1-年-季
        //outMonth ： (1,2,3)
        public static void getYearMonth(int inputType, string inputDate, out string outYear, out string outMonth, char inputDelimeter = '-')
        {
            string[] dataDate;
            if (inputDate.IndexOf('-') >= 0)
                dataDate = inputDate.Split('-');
            else if (inputDate.IndexOf('/') >= 0)
                dataDate = inputDate.Split('/');
            else
                dataDate = inputDate.Split('\\');
            outYear = dataDate[0];
            List<string> dataMonth = new List<string>();
            if (inputType == 0)
            {
                dataMonth.Add(dataDate[1]);
            }
            else
            {
                int firstMonth = (int.Parse(dataDate[1]) - 1) * 3;
                for (int i = 1; i <= 3; i++)
                    dataMonth.Add((firstMonth + i).ToString());
            }
            outMonth = "(" + string.Join(",", dataMonth) + ")";
        }
        //增加幾個工作日
        //傳入到 originalDate 開始找的日期， -- 出貨日
        //direction: -1, 往前找工作日， 1:往後找工作日
        //public static DateTime AddWorkdays(DateTime originalDate, int workDays, int direction = 1)
        //{
        //    String strOriginalDate = originalDate.ToString("yyyy-MM-dd");
        //    String strQry = "SELECT HDate, HNAME, HTYPE, NOTE, CREATED " +  //查詢十天內上班天/放假天的資料
        //                    " FROM MFO_HOLIDAYS WHERE HDate>= '" + strOriginalDate + "' AND HDate <= DATE_ADD('" + strOriginalDate + "', INTERVAL " + Constants.beforeDelDay.ToString() + " DAY) order by HTYPE ";
        //    List<DateTime> workList = new List<DateTime>(); //上班日 的 LIST
        //    List<DateTime> holidayList = new List<DateTime>(); //放假日的 LIST
        //    DataTable dt = getSQLDataTable(strQry);
        //    foreach (DataRow dtRow in dt.Rows)
        //    {
        //        if (dtRow["HTYPE"].ToString() == "工作日")
        //            workList.Add(Convert.ToDateTime(dtRow["HDate"]));
        //        else
        //            holidayList.Add(Convert.ToDateTime(dtRow["HDate"]));
        //    }

        //    DateTime tmpDate = originalDate;
        //    while (workDays > 0)
        //    {
        //        tmpDate = tmpDate.AddDays(direction);
        //        if (workList.Contains(tmpDate) ||
        //            (tmpDate.DayOfWeek < DayOfWeek.Saturday &&
        //            tmpDate.DayOfWeek > DayOfWeek.Sunday &&
        //            !holidayList.Contains(tmpDate)))
        //            workDays--;
        //    }
        //    return tmpDate;
        //}

        //兩個字串日期比較先後
        //return 0： dateA < dateB
        //return 1： dateA == dateB
        //return 2： dateA > dateB
        //return -1： 傳入資料非日期格式
        public static int strDateTimeCompare(string strDateA, string strDateB)
        {
            int rtnInt = -1;
            try
            {
                DateTime dateA = Convert.ToDateTime(strDateA);
                DateTime dateB = Convert.ToDateTime(strDateB);
                if (dateA < dateB)
                    return 0;
                else if (dateA == dateB)
                    return 1;
                else if (dateA > dateB)
                    return 2;
            }
            catch (Exception ex)
            {
                rtnInt = -1;
            }
            return rtnInt;
        }
        #endregion

        #region string
        //將 字串中的數字萃取出來， 放進 list 中
        public static List<double> ExtractNumbers(string input)
        {
            List<double> numbers = new List<double>();
            // 使用正則表達式搜尋字串中的所有數字
            string pattern = @"(\d+(\.\d+)?)";
            MatchCollection matches = Regex.Matches(input, pattern);
            // 將匹配到的數字轉換為 double 並添加到列表中
            foreach (Match match in matches)
            {
                double number;
                if (double.TryParse(match.Value, out number))
                {
                    numbers.Add(number);
                }
            }
            return numbers;
        }

        //將歐洲的 dd.MM.yy 轉成 yyyy/MM/dd
        //呼叫方式 : adjustDateFormat("dd.MM.yyyy", "2020/11/15")
        public static string adjustDateFormat(string dateFormat, string inputDate)
        {
            DateTime varDatetime;

            if (DateTime.TryParseExact(inputDate, dateFormat,
                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out varDatetime))
                return varDatetime.ToString("yyyy/MM/dd");
            else
                return inputDate;
        }

        //將歐洲的 10.000,00 轉成 10,000.00
        //呼叫方式 : adjustAmountFormat(',', "10,000.00")
        public static string adjustAmountFormat(char decimalFormat, string inputAmt)
        {
            string[] inputArr = inputAmt.Split(decimalFormat);
            string rtnStr;
            if (decimalFormat == '.')
                rtnStr = inputArr[0].Replace(",", ".") + "," + inputArr[1];
            else
                rtnStr = inputArr[0].Replace(".", ",") + "." + inputArr[1];
            return rtnStr;
        }

        //stringCompare("jrwttttt", "jersey") ==> jr******
        //stringCompare("jersey","jrwttttt") ==> j*r***
        //stringCompare("jersey","jrwttytt") ==> j*r**y
        public static string stringCompare(String mainS, String s2)
        {
            string final = string.Empty;
            char[] aa = mainS.ToCharArray();
            foreach (char aaa in aa)
            {
                final = final + "*";
            }
            char[] bb = s2.ToCharArray();
            int startindex = 0;
            foreach (char bbb in bb)
            {
                foreach (char aaa in aa)
                {
                    if (mainS.IndexOf(bbb, startindex) > -1)
                    {
                        final = final.Remove(mainS.IndexOf(bbb, startindex), 1);
                        final = final.Insert(mainS.IndexOf(bbb, startindex), bbb.ToString());
                        startindex = mainS.IndexOf(bbb, startindex) + 1;
                    }
                }
            }
            return final;
        }

        //將兩個字串之間的內容傳出來
        //例如： extractStrBetwn2Strings("西元（2000）年", "（", "）") => 2000
        public static string extractStrBetwn2Strings(String St, string leftQuote, string rightQuote)
        {
            if (St.IndexOf(leftQuote) >= 0)
            {
                int pFrom = St.IndexOf(leftQuote) + leftQuote.Length;
                int pTo = St.LastIndexOf(rightQuote);

                return St.Substring(pFrom, pTo - pFrom);
            }
            else
                return null;

        }

        //
        public static bool ifStrStartWith(string targetStr, List<string> prefixes)
        {
            return prefixes.Any(prefix => targetStr.StartsWith(prefix));
        }

        public static string newLine(string inputStr, string replaceStr = "@")
        {
            return inputStr.Replace(replaceStr, System.Environment.NewLine);
        }

        //如果要結果是 'aaa','bbb','ccc' 則 應該下的指令為
        //  "'" + listToString(tList, "','") + "'";
        public static string listToString(List<string> tList, string delemiter = ", ")
        {
            return String.Join(delemiter, tList.ToArray());
        }

        //從 字串中取出 第幾個字串
        //例如 BARCode Scan 的 字串為 21-23-123, 要取出第三個字串（123）， 則為 getPartialSting(barcode, '-', 2)
        public static String getPartialSting(string originalString, char delimeter, int idex)
        {
            string rtnStr = null;
            string[] ary = originalString.Split(delimeter);
            if (ary.Length >= idex + 1)
            {
                rtnStr = ary[idex];
            }
            return rtnStr;
        }

        //Check if List contants the targetStr
        public static string firstStringInList(List<string> sourceList, string targetStr)
        {
            return sourceList.FirstOrDefault(stringToCheck => stringToCheck.Contains(targetStr));
        }

        public static string Right(string s, int length)
        {
            length = Math.Max(length, 0);

            if (s.Length > length)
            {
                return s.Substring(s.Length - length, length);
            }
            else
            {
                return s;
            }
        }

        //檢查某字元是否為中文字
        public static bool IsChineseLetter(string input, int index)
        {
            int code = 0;
            int chfrom = Convert.ToInt32("4e00", 16); //範圍（0x4e00~0x9fff）轉換成int（chfrom~chend） 
            int chend = Convert.ToInt32("9fff", 16);
            if (input != "")
            {
                code = Char.ConvertToUtf32(input, index); //獲得字串input中指定索引index處字元unicode編碼
                if (code >= chfrom && code <= chend)
                {
                    return true; //當code在中文範圍內返回true 
　　              }
                else
                {
                    return false; //當code不在中文範圍內返回false 
                }
            }
            return false;
        }

        //數字轉ASCII字元, by Michael 2018.2
        public static char Chr(int Num)
        {
            char C = Convert.ToChar(Num);
            return C;
        }

        //字串的首字轉ASCII碼, by Michael 2018.2
        public static int ASC(string S)
        {
            int N = Convert.ToInt32(S[0]);
            return N;
        }

        //字元轉ASCII碼, by Michael 2018.2
        public static int ASC(char C)
        {
            int N = Convert.ToInt32(C);
            return N;
        }

        //產生Temp Table名稱, by Michael 2018.2
        public static string genTempTableName(string tableName)
        {
            string ipAddress = "";
            string[] subIp;

            // 取得本機名稱
            string HostName = Dns.GetHostName();
            // 取得本機的IpHostEntry類別實體，用這個會提示已過時
            //IPHostEntry iphostentry = Dns.GetHostByName(strHostName);

            // 取得本機的IpHostEntry類別實體，MSDN建議新的用法
            IPHostEntry iphostentry = Dns.GetHostEntry(HostName);

            // 取得所有 IP 位址
            foreach (IPAddress ip in iphostentry.AddressList)
            {
                // 只取得IP V4的Address
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    //Console.WriteLine("Local IP: " + ip.ToString());
                    ipAddress = ip.ToString();
                }
                Console.WriteLine(ipAddress);
            }

            subIp = ipAddress.Split('.');
            tableName += subIp[3];

            return tableName;
        }

        public static string removeComma(string inputStr)
        {
            inputStr = inputStr.Trim();
            return inputStr.Substring(1, inputStr.Length - 2);
        }

        public static string removeNewline(string inputStr)
        {
            return Regex.Replace(inputStr, @"\t|\n|\r", "");
        }

        public static bool IsNumeric(object Expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        //"0.03以下" ==> 0.03
        public static string removeNonNumerics(string input)
        {
            return Regex.Replace(input, "[^0-9.]", "");
        }

        //從字串中將數字萃取出來, 例如 abcd23.66rede ==> 23.66
        public static string exctractNumberFromString(string inputStr)
        {
            return Regex.Match(inputStr, @"\d+\.?\d*").Value;
        }

        //將 1,2,3... => A,B,C
        public static String Number2String(int number, bool isCaps = true)
        {
            Char c = (Char)((isCaps ? 65 : 97) + (number - 1));

            return c.ToString();
        }

        public static String replacingCharByIdx(string inputStr, int idx, char newChar)
        {
            StringBuilder sb = new StringBuilder(inputStr);
            sb[idx] = newChar;
            return sb.ToString();
        }

        //不分 大小寫, 檢查 list 裡面是否包含 某字串
        public static int ifListContantsIgnoreCase(List<string> sourceList, string compareStr)
        {
            string compareStrLower = compareStr.ToLower();
            int rtnIdx = -1;
            for (int i = 0; i < sourceList.Count; i++)
            {
                if (sourceList[i].ToLower() == compareStrLower)
                {
                    rtnIdx = i;
                    break;
                }
            }
            return rtnIdx;
        }

        public static double roundDown(double inputNum, int decimalNo = 0)
        {
            double times = Math.Pow(10, decimalNo);
            return Math.Floor(inputNum * times) / times;
        }

        public static double roundUp(double inputNum, int decimalNo = 0)
        {
            double times = Math.Pow(10, decimalNo);
            return Math.Ceiling(inputNum * times) / times;
        }

        //repeated character
        //"---YYY---T---" ==> "YYY-T"
        public static String replaceRepteadChars(string inputStr, string strToReplace = "-")
        {
            string rtnStr = Regex.Replace(inputStr, @"-+", "-");
            rtnStr = rtnStr.Trim('-');
            return rtnStr;
        }

        //將字串中所有重複的字元, 置換成只有一個
        //例如: \n\n\n\abc\n\n\n\ndfe\n\n\n\n ==> \nabc\ndef\n
        // replaceDuplicate(source, "\n")
        //https://stackoverflow.com/questions/13426463/convert-an-array-to-string
        public static string replaceDuplicate(string source, string replaceStr)
        {
            return string.Join(replaceStr, Regex.Split(Regex.Replace(source, @"(?:" + replaceStr + "|[" + replaceStr + "])", replaceStr), replaceStr + @"{ 2,}"));
        }

        //"777YYY888T555" ==> "---YYY---T---"
        public static String TrimNumbersFromEndOfStr(string inputStr, string strToReplace = "")
        {
            //string pattern = @"\d+$";
            string pattern = @"[0-9\-]";
            string rtnStr = null;
            rtnStr = Regex.Replace(inputStr, pattern, strToReplace);
            rtnStr = replaceRepteadChars(rtnStr, "-");
            return rtnStr;
        }

        //計算小數點以下有幾位
        public static int getDecimalCnt(string inputNum)
        {
            int decimalPos = inputNum.IndexOf('.');
            if (decimalPos < 0)
                return 0;
            else
            {
                return inputNum.Length - (decimalPos + 1);
            }
        }

        //ifZero    true : 整數部分有幾個 0
        //          false: 整數部分非零位數
        //若傳入值含小數點, 則整數部分全計入非零位數
        public static int getZeroCnt(string inputNum, bool ifZero)
        {
            int rtnVal = 0;
            int decimalPos = inputNum.IndexOf('.');
            if (decimalPos < 0) //不含小數點
            {
                for (rtnVal = inputNum.Length - 1; rtnVal >= 0; rtnVal--)
                {
                    if (inputNum[rtnVal] != '0')
                    {
                        rtnVal++;
                        break;
                    }
                }
                if (ifZero)
                {
                    return inputNum.Length - rtnVal; //整數部分有幾個 0
                }
                else
                {
                    return rtnVal; //整數部分非零位數
                }
            }
            else //含小數點, 則整數部分全計入非零位數
            {
                if (ifZero)
                    return 0; //整數部分的0 不計算
                else
                    if (decimalPos == 1)
                {
                    if (inputNum[0] == '0')
                        return 0;
                    else
                        return 1;
                }
                return decimalPos; //整數部分非零位數
            }
        }
        #endregion


        #region Process Operation
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);
        const int SW_RESTORE = 9;

        public static int ifProcessRunning(string processName)
        {
            Process[] pname = Process.GetProcessesByName(processName);
            return pname.Length;
        }

        //execName : for example: @"Data\myApp.exe"
        public static Process startProcess(string execName = @"%windir%\system32\cmd.exe")
        {
            Process thisProcess = Process.Start(execName);
            thisProcess.WaitForExit();
            return thisProcess;
            //int id = process.Id;
        }



        public static void startProcess(string execName, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = execName;
            startInfo.Arguments = arguments;
            Process.Start(startInfo);
        }

        public static int startProcessRtnPID(string execName, string arguments, string workingDirectory = null)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = execName;
            startInfo.Arguments = arguments;
            startInfo.WorkingDirectory = workingDirectory;

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForInputIdle();
                return process.Id;
            }
        }

        public static int startChrom(string url, string chromeDir = null)
        {
            if (string.IsNullOrEmpty(chromeDir))
                return startProcessRtnPID(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", url);
            else
                return startProcessRtnPID(chromeDir, url);
        }

        public static Process GetProcessById(int processID)
        {
            return Process.GetProcessById(processID);
        }

        public static void setProcessToFront(Process proc)
        {
            IntPtr handle = proc.MainWindowHandle;
            if (IsIconic(handle))
            {
                ShowWindow(handle, SW_RESTORE);
            }
            SetForegroundWindow(handle);
        }

        //procName 例如 EXCEL.EXE
        public static void killProcess(string procName)
        {
            //execCmd(@"C:\Windows\System32\taskkill.exe", "/f /t /im EXCEL.EXE");
            execCmd(@"C:\Windows\System32\taskkill.exe", "/f /t /im " + procName);
        }

        public static void killProcess(Process proc)
        {
            proc.Kill();
        }

        public static void killProcessByName(string procName)
        {
            foreach (var process in Process.GetProcessesByName(procName))
            {
                process.Kill();
            }
        }

        public static void killProcessByFullPathName(string fullPathProcName)
        {
            string procName = getPathInfo(fullPathProcName, "filename");
            killProcessByName(procName);
        }

        //刪除開啟某檔案的程序
        //例如 processName="Microsoft Excel"， fileName="a123.xsl"
        private static void KillProcess(string processName)
        {
            Hashtable myHashtable = null;

            Process[] AllProcesses = Process.GetProcessesByName(processName);

            // check to kill the right process
            foreach (Process killProcess in AllProcesses)
            {
                if (myHashtable.ContainsKey(killProcess.Id) == false)
                    killProcess.Kill();
            }

            AllProcesses = null;
        }

        public static void killSpecifiedProcesses(string PName, string MainWindowTitle)
        {
            //把處理程序移除
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(PName);
            for (int i = 0; i < processes.Length; i++)
            {
                if (processes[i].MainWindowTitle == MainWindowTitle)
                    processes[i].Kill();
            }
        }

        public static void KillProcess(int pid)
        {
            Process[] process = Process.GetProcesses();

            foreach (Process prs in process)
            {
                if (prs.Id == pid)
                {
                    prs.Kill();
                    break;
                }
            }
        }

        public static void RunBatchFile(string batchFilePath = "D:\\scott_home\\MyStock\\pythonTool\\ai_predict.bat",
    string arguments = "", bool ifwait = false)
        {
            try
            {
                // Create a new process to run the batch file
                Process process = new Process();
                process.StartInfo.FileName = batchFilePath;

                // Add arguments if any (optional)
                process.StartInfo.Arguments = arguments;

                // Prevents command window from appearing (set to true if you want the window hidden)
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.UseShellExecute = true;

                // Start the process and wait for it to exit
                process.Start();
                if(ifwait)
                    process.WaitForExit();

                Console.WriteLine("Batch file executed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while running the batch file: " + ex.Message);
            }
        }

        //執行 windows CMD 命令
        //CommonClass.execCmd(@"taskkill /f /t /im EXCEL.EXE");
        public static Process execCmd(string execfileStr, string argumentStr = null)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //startInfo.FileName = "cmd.exe";
            startInfo.FileName = execfileStr;
            //startInfo.Arguments = "/C copy /b Image1.jpg + Archive.rar Image2.jpg";
            if (argumentStr != null)
                startInfo.Arguments = argumentStr;
            process.StartInfo = startInfo;
            process.Start();
            return process;
        }

        public static void execCmd(string command, string exPath = null, string args = null)
        {
            ProcessStartInfo pInfo = new ProcessStartInfo(command);
            if (!string.IsNullOrEmpty(exPath))
                pInfo.WorkingDirectory = exPath;
            if (!string.IsNullOrEmpty(args))
                pInfo.Arguments = args;
            Process p = Process.Start(pInfo);
            p.WaitForExit();
        }
        #endregion

        #region Collection Caulculation
        //List<string> 是否包含要尋找的文字
        public static bool listContainsString(List<string> theList, string searchStr)
        {
            IEnumerable<string> matchingvalues = theList.Where(stringToCheck => stringToCheck.Contains(searchStr));
            if (matchingvalues.Count() > 0)
                return true;
            return false;
        }

        //回傳 List<string> 第一個包含 searchStr 的 Item
        public static string firstStringInLst(List<string> theList, string searchStr)
        {
            return theList.FirstOrDefault(stringToCheck => stringToCheck.Contains(searchStr));
        }

        //將字串轉為 Dictionary
        //例如： “key1=value1;key2=value2;key3=value3;” 轉成 Dictionary
        public static Dictionary<string, string> convertStsToDictionary(String inputStr)
        {
            return inputStr.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(part => part.Split('='))
               .ToDictionary(split => split[0], split => split[1]);
        }

        //Copy Dictionart
        public static Dictionary<string, string> cloneDictionary(Dictionary<string, string> originalDictionary)
        {
            Dictionary<string, string> clonedDictionary = originalDictionary.ToDictionary(
                    x => x.Key, // Typically no cloning necessary (immuable)
                    x => (string)x.Value.Clone());  // Do the copy how you want
            return clonedDictionary;
        }

        #endregion

        #region 其他
        public static void RestartService(string serviceName, int timeoutMilliseconds = 15000)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //將 Portal 錯誤的二維 Json 改為 物件 一維陣列
        public static string convertBadJson(string jsonStr)
        {
            if (jsonStr.Contains("[["))
                return jsonStr.Replace("[[", "[").Replace("]]", "]").Replace("},{", ",").Replace("],[", ",");
            else
                return jsonStr;
        }

        //keyOrValue：0-key, 1:Value, 2:key:value
        public static List<string> dictToList(Dictionary<string, string> inputDict, int keyOrValue = 0)
        {
            if (keyOrValue == 0)
                return new List<string>(inputDict.Keys);
            else if (keyOrValue == 1)
                return new List<string>(inputDict.Values);
            else
            {
                List<string> rtnStr = new List<string>();
                foreach (KeyValuePair<string, string> kv in inputDict)
                    rtnStr.Add(kv.Key + ":" + kv.Value);
                return rtnStr;
            }

        }

        //keyOrValue：0-key, 1:Value, 2:key:value
        public static List<int> dictToList(Dictionary<int, int> inputDict, int keyOrValue = 0)
        {
            if (keyOrValue == 0)
                return new List<int>(inputDict.Keys);
            else
                return new List<int>(inputDict.Values);
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public static string showPrompt(string promptText, string caption, string initText = "")
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = promptText };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400, Text = initText };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 80, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "Ok", Left = 450, Width = 100, Top = 80, DialogResult = DialogResult.Cancel };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        public static dynamic Cast(dynamic obj, Type castTo)
        {
            return Convert.ChangeType(obj, castTo);
        }
        public static T changeType<T>(string inputVal)
        {
            return Cast(inputVal, typeof(T));
        }

        public static System.IO.MemoryStream GenerateMemoryStreamFromString(string s)
        {
            var repo = new System.IO.MemoryStream();
            var stringBytes = System.Text.Encoding.UTF8.GetBytes(s);
            repo.Write(stringBytes, 0, stringBytes.Length);
            return repo;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            System.IO.Stream stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        //寫一筆資料到 MFO_FLOW.MFO_SERVICELOG
        public static void writeLog(string service, string function, int level, string description, Exception ex = null, string toAddress = "scott.tseng@firstohm.com.tw")
        {
            string sqlStr = "INSERT INTO MFO_FLOW.MFO_SERVICELOG (SERVICE, `FUNCTION`, `Level`, DESCRIPTION)  " +
                            " VALUES(@service, @function, @level, @description)";
            Dictionary<string, string> sqlParam = new Dictionary<string, string>();
            sqlParam.Add("@service", service);
            sqlParam.Add("@function", function);
            sqlParam.Add("@level", level.ToString());
            sqlParam.Add("@description", description);
            execSQLNonQueryParams(sqlStr, sqlParam);
            if (ex == null)
                return;
            else if (level == 5)
            {
                string mailBody = ex.Message + @"\n\n" + ex.StackTrace;
                sendmailEazy(mailBody, "sstTray 發生嚴重錯誤!!", toAddress);
            }
        }

        //寫一筆資料到 Firstohm_Sales.sales_logs
        public static void writeLog(string func_name, string bywho, string log_data)
        {
            MySqlConnection conn = new MySqlConnection(Constants.ConnString);
            conn.Open();
            using (MySqlCommand command = conn.CreateCommand())
            {
                command.CommandText = "insert into Firstohm_Sales.sales_logs (`func_name`, `bywho`,`log_data`)  " +
                                                                    " values(@func_name, @bywho, @log_data)";
                command.Parameters.AddWithValue("@func_name", func_name);
                command.Parameters.AddWithValue("@bywho", bywho);
                command.Parameters.AddWithValue("@log_data", log_data);
                command.ExecuteNonQuery();
            }
            conn.Close();
        }

        //取得 IP， 產生電腦名稱， 例如 getHostName("abc") ==> abc109
        public static string getHostName(string tableName)
        {
            string ipAddress = "";
            string[] subIp;

            // 取得本機名稱
            string HostName = Dns.GetHostName();
            // 取得本機的IpHostEntry類別實體，用這個會提示已過時
            //IPHostEntry iphostentry = Dns.GetHostByName(strHostName);

            // 取得本機的IpHostEntry類別實體，MSDN建議新的用法
            IPHostEntry iphostentry = Dns.GetHostEntry(HostName);

            // 取得所有 IP 位址
            foreach (IPAddress ip in iphostentry.AddressList)
            {
                // 只取得IP V4的Address
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    //Console.WriteLine("Local IP: " + ip.ToString());
                    ipAddress = ip.ToString();
                }
                Console.WriteLine(ipAddress);
            }

            subIp = ipAddress.Split('.');
            tableName += subIp[3];

            return tableName;
        }

        public static string getComputerName()
        {
            return Environment.MachineName;
        }

        public static string numberToChineseNumber(int number)
        {
            string[] chineseNumber = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            string[] unit = { "", "十", "百", "千", "萬", "十萬", "百萬", "千萬", "億", "十億", "百億", "千億", "兆", "十兆", "百兆", "千兆" };
            StringBuilder ret = new StringBuilder();
            string inputNumber = number.ToString();
            int idx = inputNumber.Length;
            bool needAppendZero = false;
            foreach (char c in inputNumber)
            {
                idx--;
                if (c > '0')
                {
                    if (needAppendZero)
                    {
                        ret.Append(chineseNumber[0]);
                        needAppendZero = false;
                    }
                    ret.Append(chineseNumber[(int)(c - '0')] + unit[idx]);
                }
                else
                    needAppendZero = true;
            }
            return ret.Length == 0 ? chineseNumber[0] : ret.ToString();
        }

        public static string numberToChiNumber(int number)
        {
            string[] chineseNumber = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            string[] unit = { " ", " 十", " 百", " 千", " 萬", " 十萬", " 百萬", " 千萬", " 億", " 十億", " 百億", " 千億", "兆", "十兆", "百兆", "千兆" };
            StringBuilder ret = new StringBuilder();
            string inputNumber = number.ToString();
            int idx = inputNumber.Length;
            bool needAppendZero = false;
            foreach (char c in inputNumber)
            {
                idx--;
                if (c > '0')
                {
                    if (needAppendZero)
                    {
                        ret.Append(chineseNumber[0]);
                        needAppendZero = false;
                    }
                    ret.Append(chineseNumber[(int)(c - '0')] + unit[idx]);
                }
                else
                    needAppendZero = true;
            }
            return ret.Length == 0 ? chineseNumber[0] : ret.ToString();
        }

        public static void wait(int secs)
        {
            waitInMs(secs * 1000);
        }
        public static void waitInMs(int mSecs)
        {
            System.Threading.Thread.Sleep(mSecs);
        }

        public static void playSoundd(string theSound = "beep")
        {

            switch (theSound)
            {
                case "asterisk":
                    System.Media.SystemSounds.Asterisk.Play();
                    break;
                case "beep":
                    System.Media.SystemSounds.Beep.Play();
                    break;
                case "exclamation":
                    System.Media.SystemSounds.Exclamation.Play();
                    break;
                case "hand":
                    System.Media.SystemSounds.Hand.Play();
                    break;
                case "question":
                    System.Media.SystemSounds.Question.Play();
                    break;
                default:
                    System.Media.SystemSounds.Beep.Play();
                    break;
            }
        }

        public static void screenShot(string filepath)
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                bitmap.Save(filepath, ImageFormat.Jpeg);
            }
        }

        public static void currScreenShot(Form window, string filepath)
        {
            Rectangle bounds = window.Bounds;
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                }
                bitmap.Save(filepath, ImageFormat.Jpeg);
            }
        }

        public static string GetColumnName(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var value = "";

            if (index >= letters.Length)
                value += letters[index / letters.Length - 1];

            value += letters[index % letters.Length];

            return value;
        }

        //waitSecs 等幾秒後重啟程式
        public static void restartApp(int waitSecs = 0)
        {
            if (waitSecs > 0)
                wait(waitSecs);
            ProcessStartInfo Info = new ProcessStartInfo();
            Info.Arguments = "/C ping 127.0.0.1 -n 2 && \"" + Application.ExecutablePath + "\"";
            Info.WindowStyle = ProcessWindowStyle.Hidden;
            Info.CreateNoWindow = true;
            Info.FileName = "cmd.exe";
            Process.Start(Info);
            Application.Exit();
        }

        //waitSecs 等幾秒後重開機
        public static void rebootComputer(int waitSecs = 0)
        {
            if (waitSecs > 0)
                wait(waitSecs);
            var cmd = new System.Diagnostics.ProcessStartInfo("shutdown.exe", "-r -t 0");
            cmd.CreateNoWindow = true;
            cmd.UseShellExecute = false;
            cmd.ErrorDialog = false;
            System.Diagnostics.Process.Start(cmd);
        }

        //電腦關機
        public static void Shutdown()
        {
            ManagementBaseObject mboShutdown = null;
            ManagementClass mcWin32 = new ManagementClass("Win32_OperatingSystem");
            mcWin32.Get();

            // You can't shutdown without security privileges
            mcWin32.Scope.Options.EnablePrivileges = true;
            ManagementBaseObject mboShutdownParams =
                     mcWin32.GetMethodParameters("Win32Shutdown");

            // Flag 1 means we want to shut down the system. Use "2" to reboot.
            mboShutdownParams["Flags"] = "1";
            mboShutdownParams["Reserved"] = "0";
            foreach (ManagementObject manObj in mcWin32.GetInstances())
            {
                mboShutdown = manObj.InvokeMethod("Win32Shutdown",
                                               mboShutdownParams, null);
            }
        }
        #endregion

        #region changeResolution

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int ChangeDisplaySettings([In] ref DEVMODE lpDevMode, int dwFlags);
        struct DEVMODE
        {
            public const int DM_DISPLAYFREQUENCY = 0x400000;
            public const int DM_PELSWIDTH = 0x80000;
            public const int DM_PELSHEIGHT = 0x100000;
            private const int CCHDEVICENAME = 32;
            private const int CCHFORMNAME = 32;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public DMDO dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        enum DMDO
        {
            DEFAULT = 0,
            D90 = 1,
            D180 = 2,
            D270 = 3
        }

        //intWidth=1280, int intHeight=800, int intFrequency=60
        public static void changeResolution(int intWidth = 1280, int intHeight = 800, int intFrequency = 60)
        {
            long RetVal = 0;
            DEVMODE dm = new DEVMODE();
            dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
            dm.dmPelsWidth = intWidth;
            dm.dmPelsHeight = intHeight;
            dm.dmDisplayFrequency = intFrequency;
            dm.dmFields = DEVMODE.DM_PELSWIDTH | DEVMODE.DM_PELSHEIGHT | DEVMODE.DM_DISPLAYFREQUENCY;
            RetVal = ChangeDisplaySettings(ref dm, 0);
        }
        #endregion

        #region File System
        //移除 text file 中所有 空白行
        public static void removeEmptyLines(string inputFilePath, string outputFilePath)
        {
            // Read all lines from the input file
            string[] lines = File.ReadAllLines(inputFilePath);

            // Remove empty lines
            lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

            // Write the non-empty lines to the output file
            File.WriteAllLines(outputFilePath, lines);
        }

        //delSorceFiles : 壓縮完成是否刪除原檔案
        public static string zipFolder(string sourceDirectory, string dbName, bool delSorceFiles = false)
        {
            string[] folderNameArr = sourceDirectory.Split('\\');
            string zipFile = null;
            for (int i = 0; i < folderNameArr.Length - 1; i++)
            {
                zipFile += folderNameArr[i] + @"\";
            }
            zipFile += folderNameArr[folderNameArr.Length - 1] + $"_{(dbName == null ? "" : dbName)}.zip";
            File.Delete(zipFile);
            ZipFile.CreateFromDirectory(sourceDirectory, zipFile);
            if (delSorceFiles)
                deleteFoldersAndFiles(sourceDirectory, true);
            return zipFile;
        }

        //delSorceFiles : 壓縮完成是否刪除原檔案
        public static string zipFilesInFolder(string sourceDirectory, bool delSorceFiles = false)
        {
            string[] folderNameArr = sourceDirectory.Split('\\');
            string zipFile = null;

            for (int i = 0; i <= folderNameArr.Length - 1; i++)
            {
                zipFile += folderNameArr[i] + @"\";
            }
            zipFile += folderNameArr[folderNameArr.Length - 1] + ".zip";
            File.Delete(zipFile);
            ZipFile.CreateFromDirectory(sourceDirectory, zipFile, CompressionLevel.Fastest, false);
            if (delSorceFiles)
                deleteFilesExclude(sourceDirectory, DateTime.Now.AddDays(1), "zip");
            return zipFile;
        }

        //將 topFolderPath 下面的 目錄 逐一壓縮
        public static void zipSubFolders(string topFolderPath, string dbName = null, bool ifDelSource = true)
        {
            string[] allFolders = CommonClass.getAllFoldersInDirectory(topFolderPath);
            foreach (string folderName in allFolders)
            {
                zipFolder(folderName, dbName, ifDelSource);
            }
        }

        public static void unZip(string zipFile, string extractPath)
        {
            ZipFile.ExtractToDirectory(zipFile, extractPath);
        }

        //如果檔名已存在， 使用此程式決定新檔名，以避免舊檔被覆蓋
        public static string addSeqToFileName(string FileName)
        {
            string rtnNewFileName = getFileInfo(FileName, "name");
            string destDir = getFileInfo(FileName, "directory");
            List<string> listRtnNewFileName = rtnNewFileName.Split('.').ToList();
            string newestFile = NewestFileofDirectory(destDir, listRtnNewFileName[0], listRtnNewFileName[1]);
            List<string> newlistRtnNewFileName = newestFile.Split('.').ToList();
            int seqQuoteIdx = newlistRtnNewFileName[0].LastIndexOf('(');
            if (seqQuoteIdx < 0)
                rtnNewFileName = listRtnNewFileName[0] + "(1)." + listRtnNewFileName[1];
            else
            {
                string lastIdx = extractStrBetwn2Strings(newlistRtnNewFileName[0], "(", ")");
                if (IsNumeric(lastIdx))
                    rtnNewFileName = listRtnNewFileName[0] + "(" + (int.Parse(lastIdx) + 1) + ")." + listRtnNewFileName[1];
                else
                    rtnNewFileName = listRtnNewFileName[0] + "(1)." + listRtnNewFileName[1];
            }
            return destDir + @"\" + rtnNewFileName;
        }

        public static bool ifFileExists(string FileName)
        {
            return System.IO.File.Exists(FileName);
        }

        //sourceFile = @"c:\test.txt", destFile = @"c:\test\foo.txt"
        public static bool copyFile(string sourceFile, string destFile)
        {
            bool retrnBool = false;
            try
            {
                File.Copy(sourceFile, destFile, true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        /// Returns recently written File from the specified directory.
        /// If the directory does not exist or doesn't contain any file, null is returned.
        /// </summary>
        /// <param name="directoryInfo">Path of the directory that needs to be scanned</param>
        /// <returns></returns>
        // 找directoryPath 下最新的檔案
        // 檔案一定要包含的名字， 例如 HL, 20200101HL.sql
        public static string NewestFileofDirectory(string directoryPath, string fileName = null, string extension = null)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            if (directoryInfo == null || !directoryInfo.Exists)
                return null;

            FileInfo[] files = directoryInfo.GetFiles();
            DateTime recentWrite = DateTime.MinValue;
            FileInfo recentFile = null;

            foreach (FileInfo file in files)
            {

                if (!string.IsNullOrEmpty(fileName) && !file.Name.Contains(fileName))
                    continue;
                if (!string.IsNullOrEmpty(extension) && file.Extension != "." + extension)
                    continue;
                if (file.LastWriteTime > recentWrite)
                {
                    recentWrite = file.LastWriteTime;
                    recentFile = file;
                }
            }
            return recentFile.Name;
        }

        // 找directoryPath 下最新的目錄
        // 目錄一定要包含的名字， 例如 HL, 20200101HL
        public static string NewestSubDirOfDirectory(string directoryPath, string fileName = null)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            if (directoryInfo == null || !directoryInfo.Exists)
                return null;

            DirectoryInfo[] files = directoryInfo.GetDirectories();
            DateTime recentWrite = DateTime.MinValue;
            DirectoryInfo recentFile = null;

            foreach (DirectoryInfo file in files)
            {

                if (!string.IsNullOrEmpty(fileName) && !file.Name.Contains(fileName))
                    continue;

                if (file.LastWriteTime > recentWrite)
                {
                    recentWrite = file.LastWriteTime;
                    recentFile = file;
                }
            }
            return recentFile.Name;
        }

        public static int[] getImgSize(string imgFilePath)
        {
            int[] imgSize = new int[2];

            System.Drawing.Image img = System.Drawing.Image.FromFile(imgFilePath);
            imgSize[0] = img.Width;
            imgSize[1] = img.Height;
            return imgSize;
        }

        public static string getFileInfo(string inputFile, string infoType)
        {
            string outputInfo = null;
            try
            {
                switch (infoType.ToLower())
                {
                    case "name":
                        outputInfo = Path.GetFileName(inputFile);
                        break;
                    case "path":
                        outputInfo = Path.GetFullPath(inputFile);
                        break;
                    case "extension":
                        outputInfo = Path.GetExtension(inputFile);
                        break;
                    case "directory":
                        outputInfo = Path.GetDirectoryName(inputFile);
                        break;
                    case "filenamewithoutextension":
                        outputInfo = Path.GetFileNameWithoutExtension(inputFile);
                        break;
                }
            }
            catch (Exception ex)
            {
                outputInfo = null;
            }
            return outputInfo;
        }

        public static string getCurrDir()
        {
            return Directory.GetCurrentDirectory();
        }

        //刪除日期以前的所有檔案
        //呼叫範例： deleteFiles(excelPath, DateTime.Now.AddDays(1), "bmp");
        public static int deleteFiles(string dirName, DateTime theDate, string extension = "*")
        {
            int delCnt = 0;
            string[] files = Directory.GetFiles(dirName);
            foreach (string file in files)
            {
                if (extension == "*" || getPathInfo(file, "fileextension").Contains(extension))
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastAccessTime < theDate)
                    {
                        try
                        {
                            fi.Delete();
                            delCnt++;
                        }
                        catch (Exception ex) { }
                    }
                }
            }
            return delCnt;
        }

        //excludeExt : 副檔名為指定值則不刪除的
        //呼叫範例： deleteFilesExclude(excelPath, DateTime.Now.AddDays(1), "zip");
        public static int deleteFilesExclude(string dirName, DateTime theDate, string excludeExt = null)
        {
            int delCnt = 0;
            string[] files = Directory.GetFiles(dirName);
            foreach (string file in files)
            {
                if (string.IsNullOrEmpty(excludeExt) || !getPathInfo(file, "fileextension").Contains(excludeExt))
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastAccessTime < theDate)
                    {
                        try
                        {
                            fi.Delete();
                            delCnt++;
                        }
                        catch (Exception ex) { }
                    }
                }
            }
            return delCnt;
        }

        public static string[] getAllFoldersInDirectory(string fullPath)
        {
            string[] folders = Directory.GetDirectories(fullPath);
            return folders;
        }

        public static string[] getAllfilesInDirectory(string fullPath, string extension = "*.xlsx")
        {
            string[] files = Directory.GetFiles(fullPath, extension, SearchOption.AllDirectories);
            return files;
        }

        //dirType, C:\排單資料\標準\MFO_Sheet.xlsx
        //  fullpath: C:\排單資料\標準\MFO_Sheet.xlsx
        //  root: C:\
        //  pathinfo:C:\排單資料\標準
        //  filepath:MFO_Sheet.xlsx
        //  filename:MFO_Sheet
        //  fileextension: xlsx
        //ifTopDelete:刪除本目錄 與其下所有內容， 含檔案與目錄
        public static void deleteFoldersAndFiles(string targetFolders, bool ifTopDelete = false, bool onlyFiles = false)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(targetFolders);
            if (ifTopDelete)
            {
                di.Delete(true); //刪除本目錄 與其下所有內容， 含檔案與目錄
                return;
            }

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            if (onlyFiles) //是否只砍檔案不砍目錄
            {
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
        }

        public static string getPathInfo(string strPath, string dirType)
        {
            String rtnStr = null;

            switch (dirType)
            {
                case "fullpath":
                    rtnStr = Path.GetFullPath(strPath);
                    break;
                case "root":
                    rtnStr = Path.GetPathRoot(strPath);
                    break;
                case "pathinfo":
                    rtnStr = Path.GetDirectoryName(strPath);
                    break;
                case "filepath":
                    rtnStr = Path.GetFileName(strPath);
                    break;
                case "filename":
                    rtnStr = Path.GetFileNameWithoutExtension(strPath);
                    break;
                case "fileextension":
                    rtnStr = Path.GetExtension(strPath);
                    break;

            }
            return rtnStr;
        }

        public static void createIfMissing(string path)
        {
            if (!Directory.Exists(path))
            {
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
            }
        }

        public static bool moveFilesToFolder(string sourcePath, string targetPath, bool ifMove = true, bool ifOverWrite = true, bool ifShowMessage = true)
        {
            string fileName, destFile;
            bool rtnBool = false;
            createIfMissing(targetPath);
            try
            {
                if (System.IO.Directory.Exists(sourcePath))
                {
                    string[] files = System.IO.Directory.GetFiles(sourcePath);

                    // Copy the files and overwrite destination files if they already exist.
                    foreach (string s in files)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        fileName = System.IO.Path.GetFileName(s);
                        destFile = System.IO.Path.Combine(targetPath, fileName);
                        if (ifMove)
                            System.IO.File.Move(s, destFile);
                        else
                            System.IO.File.Copy(s, destFile, ifOverWrite); //第三個參數為直接覆蓋
                    }
                    rtnBool = true;
                }
                else
                {
                    if (ifShowMessage)
                        Console.WriteLine("Source path " + sourcePath + " 不存在");
                }
            }
            catch (Exception ex)
            {

            }
            return rtnBool;

        }

        //destPath : C:\ConutriPrd 
        public static void writeStringtoFileEncode(string outputStr, string destPath, string destFile, string enCode = "UTF-8")
        {
            if (!string.IsNullOrEmpty(destPath))
            {
                createIfMissing(destPath);
                destPath += @"\";
            }

            using (var sw = new StreamWriter(File.Open(@destPath + destFile, FileMode.CreateNew), Encoding.GetEncoding(enCode)))
            {
                sw.WriteLine(outputStr);
            }
        }

        //fileOrFolder 0:file, 1:folder
        public static bool ifFileorFolderExists(string inputPath, int fileOrFolder = 0)
        {
            if (fileOrFolder == 0)
            {
                return File.Exists(inputPath);
            }
            else
            {
                return Directory.Exists(inputPath);
            }
        }

        public static string theMostRecentFileInDirectory(string inputDir)
        {
            if (string.IsNullOrEmpty(inputDir))
                return null;
            var directory = new DirectoryInfo(inputDir);
            FileInfo theFile = (from f in directory.GetFiles()
                                orderby f.LastWriteTime descending
                                select f).First();
            if (theFile == null)
                return null;
            else
                return theFile.ToString();
        }

        public static string theMostRecentFolderInDirectory(string inputDir)
        {
            if (string.IsNullOrEmpty(inputDir))
                return null;
            var directory = new DirectoryInfo(inputDir);
            DirectoryInfo theFolder = (from f in directory.GetDirectories()
                                       orderby f.LastWriteTime descending
                                       select f).First();
            if (theFolder == null)
                return null;
            else
                return theFolder.ToString();
        }
        #endregion

        #region Mail
        //這段一定要, 跳過憑證無效用的 //透過Office365寄信，Added by Michael, 2019.12.27
        public static bool ValidateServerCertificate(Object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }


        public static void dsendmail(String receiverAdd, String receiverName, String mailSubject, String mailBody, String senderAdd = "sr.system@firstohm.com.tw", String senderName = "系統管理員", String smtpServer = "smtp.office365.com")
        {
            // 這段一定要, 要寫這個才可以跳過 "根據驗證程序,遠端憑證是無效的" 的錯誤 //透過Office365寄信，Modified by Michael, 2019.12.27
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
            System.Net.Mail.SmtpClient sc = new SmtpClient("smtp.office365.com");//<-宣告的時候可以先給主機名稱~記住喔~這是發送端的主機名稱~
            sc.Port = 587; // 365 好像都是用587
            sc.EnableSsl = true;  //這一定要設 true
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential("sr.system@firstohm.com.tw", "F!rst0hm@2020"); //365 帳號密碼
            sc.Credentials = credentials;

            //這物件只是用來設定郵件帳號而已~
            MailAddress senderAddress = new MailAddress("sr.system@firstohm.com.tw", "系統管理員");
            MailAddress receiverAddress = new MailAddress(receiverAdd, receiverName);
            //這物件是郵件訊息的部分~需設定寄件人跟收件人~可直接打郵件帳號也可以使用MailAddress物件~
            MailMessage mail = new MailMessage(senderAddress, receiverAddress);
            mail.Subject = mailSubject;
            mail.Body = mailBody;
            mail.IsBodyHtml = true;

            sc.Send(mail);//<-這樣就送出去拉~
            mail.Dispose();
            sc = null;
        }


        public static void sendmail(KeyValuePair<string, string> toAddr, KeyValuePair<string, string> fromAddr, Dictionary<string, string> ccAddr, Dictionary<string, string> bccAddr, string subject, string body, bool IsBodyHtml = false)
        {
            try
            {
                // 這段一定要, 要寫這個才可以跳過 "根據驗證程序,遠端憑證是無效的" 的錯誤 //透過Office365寄信，Modified by Michael, 2019.12.27
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
                System.Net.Mail.SmtpClient sc = new SmtpClient("smtp.office365.com");//<-宣告的時候可以先給主機名稱~記住喔~這是發送端的主機名稱~
                //sc.TargetName = "STARTTLS/smtp.office365.com";
                sc.Port = 587; // 365 好像都是用587
                sc.EnableSsl = true;  //這一定要設 true

                System.Net.NetworkCredential credentials =
                    new System.Net.NetworkCredential("sr.system@firstohm.com.tw", "F!rst0hm@2020"); //365 帳號密碼
                sc.Credentials = credentials;

                //這物件只是用來設定郵件帳號而已~
                MailAddress senderAddress = new MailAddress(fromAddr.Key, fromAddr.Value);
                MailAddress receiverAddress = new MailAddress(toAddr.Key, toAddr.Value);

                //sc.UseDefaultCredentials = true;
                //sc.Credentials = new NetworkCredential("sr.system", "F!rst0hm@2020");

                //MailAddress receiverAddress = new MailAddress("shihchuan.wang@firstohm.com.tw", "王工程師");
                //這物件是郵件訊息的部分~需設定寄件人跟收件人~可直接打郵件帳號也可以使用MailAddress物件~
                MailMessage mail = new MailMessage(senderAddress, receiverAddress);
                mail.Subject = subject;
                if (ccAddr != null)
                {
                    foreach (KeyValuePair<string, string> entry in ccAddr)
                    {
                        mail.CC.Add(new MailAddress(entry.Key, entry.Value));
                    }
                }

                if (bccAddr != null)
                {
                    foreach (KeyValuePair<string, string> entry in bccAddr)
                    {
                        mail.Bcc.Add(new MailAddress(entry.Key, entry.Value));
                    }
                }

                mail.Body = body;
                //如果要這封郵件吃html的話~這屬性就把他設為true
                mail.IsBodyHtml = IsBodyHtml;

                sc.Send(mail);//<-這樣就送出去拉~
                mail.Dispose();
                sc = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "sendmail");

            }
            //Console.ReadLine();
        }

        public static void sendmailEazy(string body, string subject = "FirsrohmService 發生嚴重錯誤!!!", string toAddr = Constants.severeEmailList, string fromAddr = "FirstohmService@mail.Firstohm.com", bool IsBodyHtml = false)
        {
            try
            {
                SmtpClient sc = new SmtpClient("mail.firstohm.com.tw");//<-宣告的時候可以先給主機名稱~記住喔~這是發送端的主機名稱~
                sc.Port = 25;

                //這物件只是用來設定郵件帳號而已~
                MailAddress senderAddress = new MailAddress(fromAddr);
                MailAddress receiverAddress = new MailAddress(toAddr);

                sc.UseDefaultCredentials = true;
                sc.Credentials = new NetworkCredential("sr.system", "sr.system");
                //MailAddress receiverAddress = new MailAddress("shihchuan.wang@firstohm.com.tw", "王工程師");
                //這物件是郵件訊息的部分~需設定寄件人跟收件人~可直接打郵件帳號也可以使用MailAddress物件~
                MailMessage mail = new MailMessage(senderAddress, receiverAddress);

                mail.Body = body;
                //如果要這封郵件吃html的話~這屬性就把他設為true
                mail.IsBodyHtml = IsBodyHtml;
                sc.Send(mail);//<-這樣就送出去拉~
            }
            catch (Exception ex)
            {
                //eventLogCMC.WriteEntry(ex.Message, EventLogEntryType.Information, eventId++);
                writeLog("MFOFlow_Service", "sendmailEazy()", 4, ex.Message);
            }
            //Console.ReadLine();
        }

        public static string smtpSendMail(string body, Dictionary<string, string> toEmailAddDic = null,
                 bool IsBodyHtml = false, string subject = "Firstohm Service 自動通知", string attach = null, DataTable dt = null, Dictionary<string, string> Ccs = null, Dictionary<string, string> Bccs = null, string file = null)
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, cert, chain, sslPolicyErrors) => true;
            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                foreach (KeyValuePair<string, string> toEmailAdd in toEmailAddDic)
                {
                    msg.To.Add(toEmailAdd.Value);
                }
                body = body.Replace("\n", "<br>");
                //msg.To.Add("b@b.com");可以發送給多人
                //msg.CC.Add("c@c.com");
                //msg.CC.Add("c@c.com");可以抄送副本給多人 
                //這裡可以隨便填，不是很重要
                msg.From = new MailAddress("sysagent@conutri.com", "系統自動發信服務", System.Text.Encoding.UTF8);
                /* 上面3個參數分別是發件人地址（可以隨便寫），發件人姓名，編碼*/
                msg.Subject = subject;//郵件標題
                msg.SubjectEncoding = System.Text.Encoding.UTF8;//郵件標題編碼
                msg.Body = body; //郵件內容
                msg.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
                if (!string.IsNullOrEmpty(attach) && ifFileorFolderExists(attach))
                    msg.Attachments.Add(new Attachment(attach));  //附件
                msg.IsBodyHtml = IsBodyHtml;//是否是HTML郵件 
                                            //msg.Priority = MailPriority.High;//郵件優先級 
                if (dt != null)
                {
                    string htmlfromdt = dttoHtml(dt);
                    msg.Body = body + "<br>" + htmlfromdt;
                }
                if (file != null)
                {
                    msg.Attachments.Add(new System.Net.Mail.Attachment(file)); //選擇附檔路徑
                }
                if (file != null)
                {
                    System.Net.Mail.AlternateView View = System.Net.Mail.AlternateView.CreateAlternateViewFromString(body + "<br><img src=cid:BOX><br>", null, "text/html");
                    System.Net.Mail.LinkedResource log = new System.Net.Mail.LinkedResource(file);
                    log.ContentId = "BOX";
                    View.LinkedResources.Add(log);
                    if (file.Contains(".jpg"))
                        msg.AlternateViews.Add(View);//"<img src=cid:BOX>"
                }
                if (Ccs != null)
                {
                    foreach (KeyValuePair<string, string> cc in Ccs)
                    {
                        MailAddress cc1 = new MailAddress(cc.Value, cc.Key);
                        msg.CC.Add(cc1);
                    }
                }
                if (Bccs != null)
                {
                    foreach (KeyValuePair<string, string> bcc in Bccs)
                    {
                        MailAddress cc1 = new MailAddress(bcc.Value, bcc.Key);
                        msg.Bcc.Add(cc1);
                    }
                }

                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("sysagent@conutri.com", "F932031617"); //這裡要填正確的帳號跟密碼
                client.Host = "211.23.138.231"; //設定smtp Server
                client.Port = 25; //設定Port
                client.EnableSsl = false; //gmail預設開啟驗證
                client.Send(msg); //寄出信件
                client.Dispose();
                msg.Dispose();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static string dttoHtml(DataTable dt)
        {
            try
            {
                //messageBody = 內容主體
                string messageBody = "";
                if (dt.Rows.Count == 0)
                {
                    return messageBody;  //dt為empty的情況
                }
                //設定html格式
                string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                string htmlTableEnd = "</table>";
                string htmlHeaderRowStart = "<tr style =\"background-color:#6FA1D2; color:#ffffff;\">";
                string htmlHeaderRowEnd = "</tr>";
                string htmlTrStart = "<tr style =\"color:#555555;\">";
                string htmlTrEnd = "</tr>";
                string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTdEnd = "</td>";

                messageBody += htmlTableStart;
                messageBody += htmlHeaderRowStart;
                //設定dt欄位(Column)名稱
                foreach (DataColumn dtcol in dt.Columns)
                {
                    messageBody += htmlTdStart + " " + dtcol.ColumnName + " " + htmlTdEnd;
                }
                messageBody += htmlHeaderRowEnd;
                //將dt每一行的資料依序Traverse、存取。
                foreach (DataRow Row in dt.Rows)
                {
                    messageBody += htmlTrStart;
                    foreach (DataColumn dtcol in dt.Columns)
                    {
                        messageBody += htmlTdStart + Row[dtcol.ColumnName] + htmlTdEnd;
                    }
                    messageBody += htmlTrEnd;
                }
                messageBody += htmlTableEnd;
                return messageBody;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region Regular Expression
        public static MatchCollection strExtract(string inputStr, string pattern, bool ignoreCase = true)
        {
            //string pattern = @"(?\sc[\w]*)";
            //宣告 Regex 忽略大小寫 
            Regex regex;
            if (ignoreCase)
                regex = new Regex(pattern, RegexOptions.IgnoreCase);
            else
                regex = new Regex(pattern);
            //將比對後集合傳給 MatchCollection 
            MatchCollection matches = regex.Matches(inputStr);
            return matches;
            //int index = 0;
            //// 一一取出 MatchCollection 內容
            //foreach (Match match in matches)
            //{
            //    // 將 Match 內所有值的集合傳給 GroupCollection groups
            //    GroupCollection groups = match.Groups;
            //    // 印出 Group 內 word 值
            //    Console.WriteLine(++index + ": " + groups["word"].Value.Trim());
            //}
        }
        //getMatchsInQuote("This is a {test}", "{", "}") 
        public static MatchCollection getMatchsInQuote(string inputStr, string quoatLeft, string quoatRight)
        {
            string pattern = quoatLeft + ".*?" + quoatRight;
            return strExtract(inputStr, pattern);
        }
        #endregion

        #region sysConfigueation/system configuration 
        static public string getSysConfig(string configName, string defaultVal = null)
        {
            string strSql = "SELECT `jsonParam` FROM `sysConfigueation` where `configName`='" + configName + "'";
            var rtnParam = getSQLScalar(strSql);
            if (rtnParam == null || rtnParam == DBNull.Value)
                return defaultVal;
            else
                return rtnParam.ToString();
        }
        #endregion

        #region 統計
        //產生中位數
        //ifUpper 如果筆數是雙號： true, 傳回高值, false 傳回低值
        public static double genMidian(DataTable dt, string columnName)
        {
            if (dt == null || dt.Rows.Count == 0)
                return -99999;
            if (dt.Rows.Count == 1)
            {
                return Convert.ToDouble(dt.Rows[0][columnName]);
            }
            DataTable sortedDt = DataTableSort(dt, columnName);
            int rowID = sortedDt.Rows.Count / 2 - 1;
            if (sortedDt.Rows.Count % 2 == 0)
                return (Convert.ToDouble(dt.Rows[rowID + 1][columnName]) + Convert.ToDouble(dt.Rows[rowID][columnName])) / 2;
            else
                return Convert.ToDouble(dt.Rows[rowID + 1][columnName]);
        }

        //計算眾數
        public static Dictionary<T, int> CalcZhongShu<T>(List<T> source)
        {
            Dictionary<T, int> dictionary = new Dictionary<T, int>();
            foreach (T item in source)
            {
                if (!dictionary.ContainsKey(item))
                {
                    dictionary.Add(item, 1);
                }
                else
                {
                    dictionary[item]++;
                }
            }

            // 眾數不存在
            int first = dictionary.OrderBy(m => m.Value).First().Value;
            int last = dictionary.OrderBy(m => m.Value).Last().Value;
            if (last == 1)
            {
                return null;
            }
            if (last == first)
            {
                return null;
            }

            // 獲取眾數
            Dictionary<T, int> result = new Dictionary<T, int>();
            foreach (KeyValuePair<T, int> kvp in dictionary)
            {
                if (kvp.Value == last)
                {
                    result.Add(kvp.Key, kvp.Value);
                }
            }
            return result;
        }
        #endregion
    }
}