using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Net;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Data.OleDb;
using Microsoft.Win32;
using System.IO;
//Nuget Microsoft.CSharp
//Nuget office core, interop excel
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
//DLL DLL : C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\
//System.Windows.Media -- PresentationCore.dll
//System.Data.DataSetExtensions.dll
using System.Windows.Media;
using System.Data.Common;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace FirstOhm
{
    public class StdExcelReport
    {
        public string reportType;
        public Dictionary<string, string> headerAndFooter; //例如：{ { "header", "1:7"},{ "footer", "10:10"}, { "dataRows","31"} };
        public string userSQL;
        public string dgvSQL;
        public string excelSQL;
        public string optionSQL;
        public string winFormTitle;
        public string exeFunction; //額外執行的程式
        public string specialFooterSQL = null; //各人不一樣的 Footer, 參考 切割 報表
        public List<string> removeDbCol;
        public string mergeSQL = null; //印表時與 excelSQL 合併
        public List<string> mergeKey = null; //mergeSQL 中 key {0}{1} 的來源

        // List<string> sizeList = new List<string>() { "1.7x5.4", "1x3.15", "1.7x5.2" };
        // Dictionary<double, double> tolSlot = new Dictionary<double, double>() { { 1, 0 }, { 0.5, 0 }, { 0.25, 0 }, { 0.1, 0 } };
        public List<string> sizeList = null; //size 需區分 幾個 Toler 的分類
        public List<double> tolSlot = null; //哪些 size 需做 Toler 區分
        public string extraComment = null;


        public StdExcelReport(string ReportType, Dictionary<string, string> HeaderAndFooter,
               string UserSQL, string DgvSQL, string ExcelSQL, string WinFormTitle, string ExeFunction = null, List<string> RemoveDbCol = null)
        {
            reportType = ReportType;
            headerAndFooter = HeaderAndFooter;
            userSQL = UserSQL;
            dgvSQL = DgvSQL;
            excelSQL = ExcelSQL;
            winFormTitle = WinFormTitle;
            exeFunction = ExeFunction;
            removeDbCol = RemoveDbCol;
        }
    }

    class ExcelElements
    {
        public Microsoft.Office.Interop.Excel.Application oXL;
        public Microsoft.Office.Interop.Excel.Workbook mWorkBook;
        public Microsoft.Office.Interop.Excel.Sheets mWorkSheets;
        public Microsoft.Office.Interop.Excel.Worksheet mWSheet;
    }

    struct RegExcel
    {
        public string Path;
        public string DefaultValue;
        public bool Exist;

        public RegExcel(string path, string value, bool exist)
        {
            this.Path = path;
            this.DefaultValue = value;
            this.Exist = exist;
        }
    };

    class ExcelCellPos
    {
        int CloPos;       // Col
        int RowPos;       // Row
        public ExcelCellPos(int col, int row)
        {
            CloPos = col;
            RowPos = row;
        }
        public int colPos
        {
            get { return this.CloPos; }
            set { CloPos = value; }
        }
        public int rowPos
        {
            get { return this.RowPos; }
            set { RowPos = value; }
        }
    }

    //excel 中放置圖片
    class ExcelPics
    {
        int PosX;       // Position x
        int PosY;       // Position x
        int Width;
        int Heigh;
        string picPath;     //圖檔的 位置+名稱
        public ExcelPics(int posX, int posY, int width, int heigh, string pic)
        {
            PosX = posX;       // Position x
            PosY = posY;       // Position x
            Width = width;
            Heigh = heigh;
            picPath = pic;
        }
        public int posX
        {
            get { return this.PosX; }
            set { PosX = value; }
        }
        public int posY
        {
            get { return this.PosY; }
            set { PosY = value; }
        }
        public int width
        {
            get { return this.Width; }
            set { Width = value; }
        }
        public int heigh
        {
            get { return this.Heigh; }
            set { Heigh = value; }
        }
        public string PicPath
        {
            get { return this.picPath; }
            set { picPath = value; }
        }
    }
    class ExcelLib
    {
        #region excel欄位文字與數字轉換
        public int NumberFromExcelColumn(string column)
        {
            int retVal = 0;
            string col = column.ToUpper();
            for (int iChar = col.Length - 1; iChar >= 0; iChar--)
            {
                char colPiece = col[iChar];
                int colNum = colPiece - 64;
                retVal = retVal + colNum * (int)Math.Pow(26, col.Length - (iChar + 1));
            }
            return retVal;
        }
        public string ExcelColumnFromNumber(int column)
        {
            string columnString = "";
            decimal columnNumber = column;
            while (columnNumber > 0)
            {
                decimal currentLetterNumber = (columnNumber - 1) % 26;
                char currentLetter = (char)(currentLetterNumber + 65);
                columnString = currentLetter + columnString;
                columnNumber = (columnNumber - (currentLetterNumber + 1)) / 26;
            }
            return columnString;
        }
        #endregion

        #region configuration Operation
        public Dictionary<string, string> getConfigInSheet(ExcelElements targetExcel, String configSheetNemt = "參數設定")
        {
            if (string.IsNullOrEmpty(configSheetNemt) || ifWorksheetExists(targetExcel.mWorkBook.Worksheets, configSheetNemt))
            {
                return null;
            }
            Microsoft.Office.Interop.Excel.Worksheet configWorkSheet = targetExcel.mWorkBook.Worksheets[configSheetNemt];
            Dictionary<string, string> configData = new Dictionary<string, string>();
            System.Data.DataTable dt = getWorksheetAsDataTable(configWorkSheet);
            configData = CommonClass.dtToDictionary(dt, "Name", "Data");
            return configData;
        }

        public void setConfigInSheet(Dictionary<string, string> configData, ExcelElements targetExcel, String configSheetNemt = "參數設定")
        {
            targetExcel = openExcelSheet2(targetExcel, configSheetNemt);
            System.Data.DataTable currentConfig = getWorksheetAsDataTable(targetExcel.mWSheet);
            if (currentConfig.Rows.Count == 0)
                return;
            int rowIdx = -1;
            foreach (KeyValuePair<string, string> entry in configData)
            {
                rowIdx = CommonClass.searchInDTRtnIdx(currentConfig, "Name = '" + entry.Key + "'");
                if (rowIdx >= 0)
                    currentConfig.Rows[rowIdx]["Data"] = entry.Value.ToString();
            }
            clearWorksheetContent(targetExcel, configSheetNemt);

            dtToExcel(currentConfig, targetExcel, configSheetNemt, true);

        }
        #endregion

        private IEnumerable<DataColumn> GatherColumnNamesFromExcel(Microsoft.Office.Interop.Excel.Worksheet worksheet, int[] mustHaveDataRow1, int startCol = 1, bool useCellData = true)
        {
            string[] colIdx = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                                "AA","AB","AC","AD","AE","AF","AG","AH","AI","AJ","AK","AL","AM","AN","AO","AP","AQ","AR","AS","AT","AU","AV","AW","AX","AY","AZ",
                                "BA","BB","BC","BD","BE","BF","BG","BH","BI","BJ","BK","BL","BM","BN","BO","BP","BQ","BR","BS","BT","BU","BV","BW","BX","BY","BZ",
                                "CA","CB","CC","CD","CE","CF","CG","CH","CI","CJ","CK","CL","CM","CN","CO","CP","CQ","CR","CS","CT","CU","CV","CW","CX","CY","CZ",
                                "DA","DB","DC","DD","DE","DF","DG","DH","DI","DJ","DK","DL","DM","DN","DO","DP","DQ","DR","DS","DT","DU","DV","DW","DX","DY","DZ",
                                "EA","EB","EC","ED","EE","EF","EG","EH","EI","EJ","EK","EL","EM","EN","EO","EP","EQ","ER","ES","ET","EU","EV","EW","EX","EY","EZ",
                                "FA","FB","FC","FD","FE","FF","FG","FH","FI","FJ","FK","FL","FM","FN","FO","FP","FQ","FR","FS","FT","FU","FV","FW","FX","FY","FZ"
                              };
            var columns = new List<DataColumn>();
            string columnName = getExcelCellValue(worksheet, mustHaveDataRow1, startCol);
            while (columnName != null)
            {
                if (useCellData)
                    columns.Add(new DataColumn(columnName));
                else
                    columns.Add(new DataColumn(colIdx[startCol - 1]));
                startCol++;
                columnName = getExcelCellValue(worksheet, mustHaveDataRow1, startCol);
            }
            return columns;
        }

        public bool keepPrint = true; //是否中斷進行中的工作
        Microsoft.Office.Interop.Excel.Range getRange(Microsoft.Office.Interop.Excel._Worksheet salWorkSheet, string StartCell, string endCell = null)
        {
            if (endCell == null)
                return salWorkSheet.get_Range(StartCell, System.Type.Missing);
            else
                return salWorkSheet.get_Range(StartCell, endCell);
        }

        public string getExcelCellValue(Microsoft.Office.Interop.Excel.Worksheet mWSheet, int[] rowIndex, int colIdx = 1)
        {
            if (rowIndex.Length == 0)
                return null;
            string CellValue = null;

            for (int i = 0; i <= rowIndex.Length; i++)
            {
                object tmpValue;
                try
                {
                    tmpValue = ((Microsoft.Office.Interop.Excel.Range)mWSheet.Cells[rowIndex[i], colIdx]).Value2;
                }
                catch (Exception ex)
                {
                    tmpValue = null;
                }

                if (tmpValue != null)
                    CellValue = Convert.ToString(tmpValue);
                else
                    CellValue = null;
                if (!string.IsNullOrEmpty(CellValue))
                    break;
            }
            return CellValue;
        }
        //指定Excel某格內容
        //例如：fillExcelCellValue(FileFullPath, "薪資表", "B6", myRow["EMPNAME"].ToString());
        public void fillExcelAllSolution(Microsoft.Office.Interop.Excel.Worksheet templateWS, string CellPos, string CellValue,
        string excelPath = @"d:\temp", float qrSize = 1, int qrTopMargin = 5, int picWidth=80, int picHeight = 40)
        {
            string barFileResult = null;
            string tempBarCodeFile;
            if (excelPath!="" && CommonClass.Right(excelPath, 1) != @"\")
                tempBarCodeFile = excelPath + @"\" + "tempBarcode";
            else
                tempBarCodeFile = excelPath + "tempBarcode";
            tempBarCodeFile = tempBarCodeFile.Replace("/", @"\");
            if (CellValue.Length > 6 && CellValue.Substring(0, 6).ToUpper() == "QRCODE")
            {
                String barCodeInfo = CellValue.Substring(7).Replace("/", @"\");
                //tempBarCodeFile += ".bmp";
                //QrCode 的 Size 為 50*size pixel
                barFileResult = BarCodeLib.generateBARCode("QR", barCodeInfo, tempBarCodeFile, qrSize);
                int[] imgSize = CommonClass.getImgSize(barFileResult);
                insertPicToCell(templateWS, CellPos, barFileResult, (int)(imgSize[0] + 10), (int)(imgSize[1] + 10), qrTopMargin);
                Thread.Sleep(0);
            }
            ////barCode:Content,Width,Heigh
            else if (CellValue.Length > 7 && CellValue.Substring(0, 7).ToUpper() == "BARCODE")
            {
                string barCodeInfo = CellValue.Substring(8).Replace("/", @"\");
                barFileResult = BarCodeLib.generateBARCode("BAR", barCodeInfo, tempBarCodeFile, qrSize);
                int[] imgSize = CommonClass.getImgSize(barFileResult);
                insertPicToCell(templateWS, CellPos, barFileResult, (int)(imgSize[0]), (int)(imgSize[1]), qrTopMargin);
                Thread.Sleep(0);
            }
            else if (CellValue.Length > 6 && CellValue.Substring(0, 6).ToUpper() == "ATTPIC")
            {
                String barCodeInfo = CellValue.Substring(7).Replace("/", @"\");
                //System.Drawing.Image thumbnail = System.Drawing.Image.FromFile(barCodeInfo);
                //Pictures p = templateWS.Pictures(System.Type.Missing);
                //var pic = p.Insert(barCodeInfo, System.Type.Missing);
                //pic.ShapeRange.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoCTrue;
                //pic.ShapeRange.Width = picWidth;
                //pic.ShapeRange.Height = picHeight;

                //templateWS.Cells[CellPos] = templateWS.Shapes.AddPicture(barCodeInfo, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 75, 75, picWidth, picHeight);
                insertPicToCell(templateWS, CellPos, barCodeInfo, picWidth, picHeight, qrTopMargin);
                Thread.Sleep(0);
            }
            else
                fillExcelCellValue(templateWS, CellPos, CellValue);
        }

        public void fillExcelCellValue(Microsoft.Office.Interop.Excel._Worksheet salWorkSheet, string ExcelCell, string CellValue)
        {
            salWorkSheet.get_Range(ExcelCell, System.Type.Missing).Value = CellValue;
        }
        public void fillExcelCellValue(string FileName, string xlSheetName, string ExcelCell, string CellValue)
        {
            Microsoft.Office.Interop.Excel.Application xlexcel;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            //object misValue = System.Reflection.Missing.Value;
            xlexcel = new Microsoft.Office.Interop.Excel.Application();
            //xlWorkBook = xlexcel.Workbooks.Add(misValue);
            xlWorkBook = xlexcel.Application.Workbooks.Open(FileName);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets[xlSheetName];
            xlWorkSheet.get_Range(ExcelCell, System.Type.Missing).Value = CellValue;
            xlWorkBook.Save();
            xlexcel.Quit();
        }

        public void fillExcelCellValue(Microsoft.Office.Interop.Excel._Worksheet salWorkSheet, Dictionary<string, string> dataToFillin)
        {
            foreach (KeyValuePair<string, string> entry in dataToFillin)
            {
                salWorkSheet.get_Range(entry.Key, System.Type.Missing).Value = entry.Value;
            }
        }

        public void fillExcelCellValue(string FileName, string xlSheetName, Dictionary<string, string> dataToFillin)
        {
            Microsoft.Office.Interop.Excel.Application xlexcel;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            //object misValue = System.Reflection.Missing.Value;
            xlexcel = new Microsoft.Office.Interop.Excel.Application();
            //xlWorkBook = xlexcel.Workbooks.Add(misValue);
            xlWorkBook = xlexcel.Application.Workbooks.Open(FileName);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets[xlSheetName];
            foreach (KeyValuePair<string, string> entry in dataToFillin)
            {
                xlWorkSheet.get_Range(entry.Key, System.Type.Missing).Value = entry.Value;
            }

            xlWorkBook.Save();
            xlWorkBook.Close();
            Marshal.FinalReleaseComObject(xlWorkBook);

            xlexcel.Quit();
            Marshal.FinalReleaseComObject(xlexcel);
        }
        public void fillExcelCellValue(Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet, Dictionary<string, string> dataToFillin)
        {
            foreach (KeyValuePair<string, string> entry in dataToFillin)
            {
                xlWorkSheet.get_Range(entry.Key, System.Type.Missing).Value = entry.Value;
            }
        }
        public void clearWorksheetContent(ExcelElements excel2, String sheetName)
        {
            if (string.IsNullOrEmpty(sheetName) || ifWorksheetExists(excel2.mWorkBook.Worksheets, sheetName))
            {
                return;
            }
            excel2.oXL.DisplayAlerts = false;
            Microsoft.Office.Interop.Excel.Worksheet targetWorkSheet = excel2.mWorkBook.Worksheets[sheetName];
            targetWorkSheet.Delete();
            excel2.mWSheet.Cells.ClearContents();
            excel2.oXL.DisplayAlerts = true;
        }
        public void setHyperlink(Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet, string url, string cellStart, string cellEnd = null)
        {
            if (string.IsNullOrEmpty(cellEnd))
                cellEnd = cellStart;

            Microsoft.Office.Interop.Excel.Range _range = xlWorkSheet.get_Range(cellStart, cellEnd);
            xlWorkSheet.Hyperlinks.Add(_range, url);
        }

        //return size of a worksheet [row, col]
        public int[] getWorkSheetSize(Microsoft.Office.Interop.Excel.Worksheet worksheet)
        {
            int[] rtnSize = new int[2];
            // Find the last real row
            try
            {
                rtnSize[0] = worksheet.Cells.Find("*", System.Reflection.Missing.Value,
                                           System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                                           Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows,
                                           Microsoft.Office.Interop.Excel.XlSearchDirection.xlPrevious,
                                           false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;
            }
            catch (Exception ex)
            {
                rtnSize[0] = 0;
            }


            // Find the last real column
            try
            {
                rtnSize[1] = worksheet.Cells.Find("*", System.Reflection.Missing.Value,
                               System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                               Microsoft.Office.Interop.Excel.XlSearchOrder.xlByColumns,
                               Microsoft.Office.Interop.Excel.XlSearchDirection.xlPrevious,
                               false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Column;
            }
            catch (Exception ex)
            {
                rtnSize[1] = 0;
            }
            return rtnSize;
        }

        public object LineStyleType { get; private set; }
        //將Cell Insex 由數字轉成文字 (2,10)=>(B10)
        public string cellIdxToStr(int row, int col)
        {
            if (col <= 26)
                return CommonClass.Number2String(col) + row.ToString();
            else if (col > 26 && col <= 52)
            {
                return "A" + CommonClass.Number2String(col - 26) + row.ToString();
            }
            else if (col > 52 && col <= 78)
            {
                return "B" + CommonClass.Number2String(col - 52) + row.ToString();
            }
            else if (col > 78 && col <= 104)
                return "C" + CommonClass.Number2String(col - 78) + row.ToString();
            else
                return "D" + CommonClass.Number2String(col - 104) + row.ToString();

        }

        public KeyValuePair<int, int> getCellPosition(ref Microsoft.Office.Interop.Excel.Worksheet objSheet, int col, int row, int leftAdd = 10, int topAdd = 10)
        {
            Microsoft.Office.Interop.Excel.Range oRange = (Microsoft.Office.Interop.Excel.Range)objSheet.Cells[col, row];
            int Left = (int)((double)oRange.Left);
            int Top = (int)((double)oRange.Top);
            return new KeyValuePair<int, int>(Top, Left);
        }
        public void columnDelete(ref Microsoft.Office.Interop.Excel.Worksheet objSheet, string cell)
        {
            Microsoft.Office.Interop.Excel.Range objRange =
                (Microsoft.Office.Interop.Excel.Range)objSheet.get_Range(cell, Missing.Value);
            objRange.EntireColumn.Delete(Missing.Value);
            Marshal.ReleaseComObject(objRange);
        }

        public void dtToExcel(Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet, System.Data.DataTable dt, List<string> textCol = null)
        {
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                //注意Excel表的行和列的索引都是從1開始的
                xlWorkSheet.Cells[1, i + 1] = dt.Columns[i].ColumnName;
            }
            //遍歷DataTable，給Excel賦值
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    //從第二行第一列開始寫入資料
                    if (textCol != null && textCol.Contains(dt.Columns[j].ColumnName))
                        xlWorkSheet.Cells[i + 2, j + 1] = "'" + dt.Rows[i][j];
                    else
                        xlWorkSheet.Cells[i + 2, j + 1] = dt.Rows[i][j];
                }
            }
        }

        public void dtToExcel(System.Data.DataTable dt, List<string> textCol = null)
        {
            Microsoft.Office.Interop.Excel.Application xlexcel;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlexcel = new Microsoft.Office.Interop.Excel.Application();
            xlexcel.Visible = true;
            xlWorkBook = xlexcel.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            dtToExcel(xlWorkSheet, dt, textCol);
        }

        /** 簡單常用  **/
        public void dgvToExcel(DataGridView dgv, bool withBorder = false, int rowStart = 1, int rangeEnd = 1, string title = null, int fontSize = 12)
        {
            int titlePos = title == null ? 0 : 1;
            string cellStart = cellIdxToStr(rowStart + titlePos, rangeEnd);
            string cellEmd = cellIdxToStr(rowStart + dgv.Rows.Count + titlePos - 1, dgv.ColumnCount + 1);

            Microsoft.Office.Interop.Excel.Application xlexcel;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlexcel = new Microsoft.Office.Interop.Excel.Application();
            xlexcel.Visible = true;
            xlWorkBook = xlexcel.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlexcel.StandardFontSize = fontSize;
            rowStart = dgv.Rows.Count / dgv.ColumnCount;
            rowStart = rowStart == 0 ? 1 : rowStart;
            if (withBorder)
            {
                setBorderToRange(xlWorkSheet, cellStart, cellEmd);
            }
            copyAlltoClipboard(dgv);

            writeDgvToExcelHeader(dgv, xlWorkSheet);
            Microsoft.Office.Interop.Excel.Range CR = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[2, 1];
            CR.Select();

            xlWorkSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);
            xlWorkSheet.Cells.EntireColumn.AutoFit();
            //xlWorkSheet.Cells.EntireColumn.ColumnWidth = 10;
            xlWorkSheet.Columns["C"].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
            xlWorkSheet.Columns["C"].ColumnWidth = 20;
            columnDelete(ref xlWorkSheet, "A1");
            insertEmptyRow(xlWorkSheet);
            excelMerge(xlWorkSheet, "A1", cellEmd.Substring(0, 1) + "1");
            writeCell(xlWorkSheet, "A1", title);
        }

        //Dictionary<string, string> headerAndFooter -
        //{ {"reportType", "貼帶報表"}, { "header", "1:7" }, { "footer", "10:10" }, { "pageDataRows", "31" }, { "dataRowStart", "8" }, { "columnEnd", "N"}, { "additionalRowCnt", "8"}}
        //reportType ： 系統到 Property 找 Template
        //headerRows,  Row 1:5, header 在 Template 的 Rows
        //footerRows,  Row 10:10, footer 在 Template 的 Rows
        //dataRowStart, 資料開始的 RowNO
        //回傳 Footer 的行數
        public int report_dtToExcel(System.Data.DataTable dt, ExcelElements ee, Dictionary<string, string> headerAndFooter,
                                      int excelStartRow = 1, bool copyFormat = true,
                                      string targetSheet = "報表", string template = "Template", bool useDgvHear = false,
                                      bool withBorder = true, int fontSize = 12)
        {
            if (excelStartRow <= 0)
                excelStartRow = 1;
            Microsoft.Office.Interop.Excel.Worksheet targetWS = getWorksheet(ee, targetSheet);
            Microsoft.Office.Interop.Excel.Worksheet templateWS = getWorksheet(ee, template);
            //處理 Header
            copyPasteRange(templateWS, targetWS, headerAndFooter["header"], "A" + excelStartRow.ToString(), false);

            if (copyFormat)
                copyPasteSpecial(templateWS, targetWS, null, null, XlPasteType.xlPasteFormats);

            ee.oXL.StandardFontSize = fontSize;
            //int rowStart = dt.Rows.Count / dt.Columns.Count;
            //rowStart = rowStart == 0 ? 1 : rowStart;
            int printRowIdx = int.Parse(headerAndFooter["dataRowStart"]) + excelStartRow - 1;
            int excelFinishRow = printRowIdx + dt.Rows.Count - 1;
            if (withBorder)
            {
                setBorderToRange(targetWS, "A" + printRowIdx, headerAndFooter["columnEnd"] + excelFinishRow);
            }
            //string cellStart = "1";
            //string cellEmd = headerAndFooter.ContainsKey("columnCnt")? headerAndFooter["columnCnt"]:dt.Columns.Count.ToString();

            //處理 Data 
            dataTableToExcel(dt, targetWS, false, printRowIdx, 1);

            if (headerAndFooter.Keys.Contains("footer"))
            {
                string[] footerInfo = headerAndFooter["footer"].Split(':');
                int footerRows = 1;
                if (footerInfo.Length >= 2)
                {
                    footerRows = int.Parse(footerInfo[1]) - int.Parse(footerInfo[0]);
                }

                copyPasteRange(templateWS, targetWS, headerAndFooter["footer"], "A" + (excelFinishRow + 2), false);
                excelFinishRow = excelFinishRow + 2 + footerRows;
                targetWS.HPageBreaks.Add(targetWS.Range[(excelFinishRow + 3 + footerRows).ToString() + ":" + (excelFinishRow + 3 + footerRows).ToString()]);
                //targetWS.PageSetup.PrintArea = "$A$1:$H$" + (excelFinishRow + 2 + footerRows).ToString();
            }
            else
                excelFinishRow = excelFinishRow + 1;
            return excelFinishRow;
        }

        public void dtToExcel(System.Data.DataTable dt, ExcelElements excel2, string sheetName, bool ifBrandNew = false)
        {
            int[] sheetSize = getWorkSheetSize(excel2.mWSheet);
            if (ifBrandNew)
            {
                int j = 1;
                clearWorksheetContent(excel2, sheetName);
                for (j = 1; j <= dt.Columns.Count; j++)
                {
                    excel2.mWSheet.Cells[1, j] = dt.Columns[j - 1].ToString();
                }
                sheetSize = new int[] { 1, j };
            }
            for (int i = 1; i <= dt.Rows.Count; i++)
            {
                for (int j = 1; j <= dt.Columns.Count; j++)
                {
                    excel2.mWSheet.Cells[i + sheetSize[0], j] = dt.Rows[i - 1][j - 1];
                }
            }
        }



        public ExcelElements dgvToExcel2(DataGridView dgv, bool withBorder = false, int rowStart = 1, int rangeEnd = 1,
            string title = null, int fontSize = 12, List<string> StyleColumns = null, int Colwidth = 20)
        {
            ExcelElements excel = new ExcelElements();
            int titlePos = title == null ? 0 : 1;
            string cellStart = cellIdxToStr(rowStart + titlePos, rangeEnd);
            string cellEmd = cellIdxToStr(rowStart + dgv.Rows.Count + titlePos - 1, dgv.ColumnCount + 1);

            object misValue = System.Reflection.Missing.Value;
            excel.oXL = new Microsoft.Office.Interop.Excel.Application();
            excel.oXL.Visible = true;
            excel.mWorkBook = excel.oXL.Workbooks.Add(misValue);
            excel.mWSheet = (Microsoft.Office.Interop.Excel.Worksheet)excel.mWorkBook.Worksheets.get_Item(1);
            excel.oXL.StandardFontSize = fontSize;
            //rowStart = dgv.Rows.Count / dgv.ColumnCount;
            //rowStart = rowStart == 0 ? 1 : rowStart;
            dgvToExcel3(dgv, excel, withBorder, rowStart - 1, rangeEnd, title = null, fontSize, StyleColumns, Colwidth);
            return excel;
        }

        public void dgvToExcel3(DataGridView dgv, ExcelElements ee, bool withBorder = false, int rowStart = 1, int rangeEnd = 1,
                string title = null, int fontSize = 12, List<string> StyleColumns = null, int Colwidth = 20)
        {
            int titlePos = title == null ? 0 : 1;
            rowStart = rowStart == 0 ? 1 : rowStart;
            string cellStart = cellIdxToStr(rowStart + titlePos, rangeEnd);
            string cellEnd = cellIdxToStr(rowStart + dgv.Rows.Count + titlePos - 1, dgv.ColumnCount + 1);

            object misValue = System.Reflection.Missing.Value;

            ee.oXL.Visible = true;
            ee.oXL.StandardFontSize = fontSize;
            rowStart = dgv.Rows.Count / dgv.ColumnCount;
            rowStart = rowStart == 0 ? 1 : rowStart;
            if (withBorder && dgv.Rows.Count > 0)
            {
                setBorderToRange(ee.mWSheet, cellStart, cellEnd);
            }
            copyAlltoClipboard(dgv);

            writeDgvToExcelHeader(dgv, ee.mWSheet);
            Microsoft.Office.Interop.Excel.Range CR = (Microsoft.Office.Interop.Excel.Range)ee.mWSheet.Cells[2, 1];
            //oRange.EntireColumn.AutoFit();

            CR.Select();

            ee.mWSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);
            ee.mWSheet.Cells.EntireColumn.AutoFit();
            if (StyleColumns != null)
            {
                foreach (string cellnum in StyleColumns)
                {
                    ee.mWSheet.Columns[cellnum].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                    ee.mWSheet.Columns[cellnum].ColumnWidth = 20;
                }
            }
            columnDelete(ref ee.mWSheet, "A1");
            insertEmptyRow(ee.mWSheet);
            //excelMerge(ee.mWSheet, "A1", cellEnd.Substring(0, 1) + "1");
            writeCell(ee.mWSheet, "A1", title);
        }

        public void dgvToExcel4(DataGridView dgv, ExcelElements ee, bool withBorder = false, int rowStart = 1, bool witheader = true, int rangeEnd = 1,
            int fontSize = 12, int Colwidth = 20)
        {
            string cellStart = cellIdxToStr(rowStart, rangeEnd);
            string cellEnd = cellIdxToStr(rowStart + dgv.Rows.Count - 1, dgv.ColumnCount + 1);

            object misValue = System.Reflection.Missing.Value;

            ee.oXL.Visible = true;
            ee.oXL.StandardFontSize = fontSize;
            rowStart = dgv.Rows.Count / dgv.ColumnCount + rowStart;
            rowStart = rowStart == 0 ? 1 : rowStart;
            if (withBorder && dgv.Rows.Count > 0)
            {
                setBorderToRange(ee.mWSheet, cellStart, cellEnd);
            }
            copyAlltoClipboard(dgv);

            if (witheader)
                writeDgvToExcelHeader(dgv, ee.mWSheet, rowStart);
            Microsoft.Office.Interop.Excel.Range CR = (Microsoft.Office.Interop.Excel.Range)ee.mWSheet.Cells[rowStart + 1, 1];
            //oRange.EntireColumn.AutoFit();

            CR.Select();

            ee.mWSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);
            ee.mWSheet.Cells.EntireColumn.AutoFit();
        }


        public ExcelElements dgvToExcelRtnExel(DataGridView dgv, bool withBorder = false, int rowStart = 1, int rangeEnd = 1, string title = null, int fontSize = 12,
                List<string> StyleColumns = null, int Colwidth = 20)
        {
            ExcelElements rtnExcel = new ExcelElements();
            int titlePos = title == null ? 0 : 1;
            object misValue = System.Reflection.Missing.Value;
            rtnExcel.oXL = new Microsoft.Office.Interop.Excel.Application();
            rtnExcel.oXL.Visible = true;
            rtnExcel.mWorkBook = rtnExcel.oXL.Workbooks.Add(misValue);
            rtnExcel.mWSheet = (Microsoft.Office.Interop.Excel.Worksheet)rtnExcel.mWorkBook.Worksheets.get_Item(1);
            rtnExcel.oXL.StandardFontSize = fontSize;
            rowStart = (dgv.Rows.Count / dgv.ColumnCount) + rowStart - 1;
            rowStart = rowStart == 0 ? 1 : rowStart;
            string cellStart = cellIdxToStr(rowStart + titlePos, 1);
            string cellEmd = cellIdxToStr(rowStart + dgv.Rows.Count + titlePos - 1, dgv.ColumnCount + 1);
            if (withBorder)
            {
                setBorderToRange(rtnExcel.mWSheet, cellStart, cellEmd);
            }

            copyAlltoClipboard(dgv);

            writeDgvToExcelHeader(dgv, rtnExcel.mWSheet, rowStart - 1);
            Microsoft.Office.Interop.Excel.Range CR = (Microsoft.Office.Interop.Excel.Range)rtnExcel.mWSheet.Cells[rowStart, 1];
            CR.Select();

            rtnExcel.mWSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);
            rtnExcel.mWSheet.Cells.EntireColumn.AutoFit();
            if (StyleColumns != null)
            {
                foreach (string cellnum in StyleColumns)
                {
                    rtnExcel.mWSheet.Columns[cellnum].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                    rtnExcel.mWSheet.Columns[cellnum].ColumnWidth = 20;
                }
            }

            columnDelete(ref rtnExcel.mWSheet, "A1");
            insertEmptyRow(rtnExcel.mWSheet);
            excelMerge(rtnExcel.mWSheet, "A1", cellEmd.Substring(0, 1) + "1");
            writeCell(rtnExcel.mWSheet, "A1", title);
            return rtnExcel;
        }


        //依照 nameOrder 的順序， 將DataTable資料寫入 Excel中
        public int dataTableToExcel(System.Data.DataTable dt, Microsoft.Office.Interop.Excel._Worksheet xlWorkSheet,
                                       bool showColumnName = true, int startRow = 1, int startCol = 1, int dtIdx = 0, int maxData = int.MaxValue,
                                       string dataColsIdx = null)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Range xlRange = null;
                xlWorkSheet.Activate();
                xlRange = xlWorkSheet.get_Range("A1", "Z9999");
                Dictionary<string, string> arrDataColsIdx=null; 
                int i = 0;
                int j = 0;
                if(!string.IsNullOrEmpty(dataColsIdx))
                {
                    arrDataColsIdx = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataColsIdx);
                }
                if (showColumnName)
                {
                    //Header
                    for (i = 0; i < dt.Columns.Count; i++)
                    {
                        if (arrDataColsIdx == null)
                            xlRange.Cells[startRow, i + 1] = dt.Columns[i].ColumnName;
                        else if(arrDataColsIdx.ContainsKey(dt.Columns[i].ColumnName))
                            fillExcelCellValue(xlWorkSheet, $"{arrDataColsIdx[dt.Columns[i].ColumnName]}{startRow}", dt.Columns[i].ColumnName);
                    }
                }

                //Datas
                DataRow dr;
                i = showColumnName ? startRow + 1 : startRow;
                for(int k= dtIdx; k<dt.Rows.Count;k++)
                {
                    j = 0;
                    foreach (object dc in dt.Rows[k].ItemArray)
                    {
                        if (arrDataColsIdx == null)
                            xlRange.Cells[i, startCol + j++] = (dc == DBNull.Value ? "" : dc.ToString());
                        else if (arrDataColsIdx.ContainsKey(dt.Columns[j].ColumnName))
                            fillExcelCellValue(xlWorkSheet, $"{arrDataColsIdx[dt.Columns[j++].ColumnName]}{i}", (dc == DBNull.Value ? "" : dc.ToString()));
                        else
                            j++;
                    }

                    i++;
                    if (i >= maxData + startRow)
                        break;
                    dtIdx++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return dtIdx;
        }

        public bool dataTableToExcel(System.Data.DataTable dt, string FileParth, string SheetName)
        {
            try
            {
                if (dt == null || dt.Columns.Count == 0)
                {
                    throw new Exception("ExportToExcel: Null or empty input table!\n");
                }

                Microsoft.Office.Interop.Excel.Application xlApp;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel._Worksheet xlWorkSheet;
                Microsoft.Office.Interop.Excel.Range xlRange = null;
                object misValue = Missing.Value;

                xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlApp.Visible = true;

                xlWorkBook = xlApp.Workbooks.Open(FileParth, misValue, false, misValue, misValue, misValue, true, misValue, misValue, misValue, misValue, misValue, false, misValue, misValue);
                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Sheets[SheetName];
                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Sheets[1];
                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.ActiveSheet;
                xlWorkSheet.Activate();

                xlRange = xlWorkSheet.get_Range("A1", "Z9999");
                int i = 0;
                int j = 0;
                //Header
                for (i = 0; i < dt.Columns.Count; i++)
                {
                    xlRange.Cells[1, i + 1] = dt.Columns[i].ColumnName;

                }
                //Datas
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    for (j = 0; j < dt.Columns.Count; j++)
                    {
                        xlRange.Cells[i + 2, j + 1] = dt.Rows[i][j];
                    }
                }
                if (FileParth != null || FileParth != "")
                {
                    try
                    {
                        //xlApp.ActiveWorkbook.SaveAs(FileParth);
                        xlApp.ActiveWorkbook.SaveAs(FileParth, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook, Missing.Value,
                        Missing.Value, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                        Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true,
                        Missing.Value, Missing.Value, Missing.Value);
                        xlWorkBook.Close();
                        Marshal.FinalReleaseComObject(xlWorkBook);

                        xlApp.Quit();
                        Marshal.FinalReleaseComObject(xlApp);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Can not save file" + ex.Message);
                    }
                }
                else
                {
                    xlApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }

        //Dictionary<string, string> headerAndFooter --{{"header", "1:5"},{"footer", "6"}, {"dataRows",31}}
        //useDgvHear 要不要寫 dgv 的欄位名稱
        //headerRows,  Row 1~5
        //footerRows,  Row 6
        //dataRows:每頁 31 行資料
        public void report_dgvToExcel(DataGridView dgv, ExcelElements ee, Dictionary<string, string> headerAndFooter, int dataRowStart,
                                      string targetSheet = "報表", string template = "Template", bool useDgvHear = false,
                                      bool withBorder = true, int fontSize = 12)
        {
            Microsoft.Office.Interop.Excel.Worksheet targetWS = getWorksheet(ee, targetSheet);
            Microsoft.Office.Interop.Excel.Worksheet templateWS = getWorksheet(ee, template);

            copyPasteRange(templateWS, targetWS, headerAndFooter["header"], "A1", false);
            //處理 Header
            copyPasteSpecial(templateWS, targetWS, null, null, XlPasteType.xlPasteFormats);

            ee.oXL.StandardFontSize = fontSize;
            int rowStart = dgv.Rows.Count / dgv.ColumnCount;
            rowStart = rowStart == 0 ? 1 : rowStart;
            string cellStart = cellIdxToStr(rowStart, 1);
            string cellEmd = cellIdxToStr(rowStart + dgv.Rows.Count + rowStart - 1, dgv.ColumnCount + 1);
            if (withBorder)
            {
                setBorderToRange(targetWS, cellStart, cellEmd);
            }

            //處理 Data 
            copyAlltoClipboard(dgv);
            if (useDgvHear)
                writeDgvToExcelHeader(dgv, targetWS);
            //Microsoft.Office.Interop.Excel.Range CR = (Microsoft.Office.Interop.Excel.Range)targetWS.Cells[1,dataRowStart];
            Microsoft.Office.Interop.Excel.Range CR = targetWS.Range["A8"];

            CR.Select();

            targetWS.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);
            //targetWS.Cells.EntireColumn.AutoFit();
            //xlWorkSheet.Cells.EntireColumn.ColumnWidth = 10;
            targetWS.Columns["C"].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
            targetWS.Columns["C"].ColumnWidth = 20;
            columnDelete(ref targetWS, "A1");
            copyPasteRange(templateWS, targetWS, headerAndFooter["footer"], "A35", false);
        }

        public System.Data.DataTable getWorksheetAsDataTable(Microsoft.Office.Interop.Excel.Worksheet worksheet, int[] headerRows = null, int startOfHeaderCol = 1, int dataRowStart = 2, bool useCellData = true)
        {
            var dt = new System.Data.DataTable(worksheet.Name);
            if (headerRows == null)
            {
                headerRows = new int[] { 1 }; //預設第一個 Row 為 Header
            }
            dt.Columns.AddRange(GatherColumnNamesFromExcel(worksheet, headerRows, startOfHeaderCol, useCellData).ToArray());
            //dt.Columns.AddRange(GetDataColumns(worksheet).ToArray());
            //var width = dt.Columns.Count;
            //var depth = GetTableDepth(worksheet, headerOffset);
            int[] worksheetSize = getWorkSheetSize(worksheet);  //int[] worksheetSize :[row, col]
            var depth = worksheetSize[0];
            var width = worksheetSize[1];
            for (var i = dataRowStart; i <= depth; i++)
            {
                var row = dt.NewRow();
                for (var j = 0; j < width - startOfHeaderCol + 1; j++)
                {
                    var currentValue = worksheet.Cells[i, j + startOfHeaderCol].Value;

                    //have to decrement b/c excel is 1 based and datatable is 0 based.
                    try
                    {
                        row[j] = currentValue == null ? null : currentValue.ToString();
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                dt.Rows.Add(row);
            }
            return dt;
        }


        //將請假資料加入 Excel(可跨月)
        //sheetCol : 決定SheetName 的 ColName
        //dataRow 要加入資料的 Excel Row ID (Excel 的最後一個 Row)
        //hyperlinkCol : 要設置 Hyperlink 的 Col
        public void appendJsonRowsToExcel(ExcelElements excel2, string jsonDataToExcel, string sheetColDate, int hyperlinkCol = -1, int substrStart = 0, int substrLen = 7, string templateSheet = "母版")
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic obj = serializer.Deserialize(jsonDataToExcel, typeof(object));
            bool isJsonObjArray = false;

            int dataRow = 0;
            Type valueType = obj.GetType();
            if (valueType.IsArray)
            {
                isJsonObjArray = true;
                jsonDataToExcel = CommonClass.getMatchsInQuote(jsonDataToExcel, "{", "}")[0].Value;
            }
            //將第一個物件, 轉為 Dictionary, 主要是為了得到 Key
            Dictionary<string, string> jsonDict = DynamicJsonConverter.jsonToDictionary(jsonDataToExcel);
            object misValue = Missing.Value;
            int[] worksheetSize = null;
            string sheetName;
            //------以下 Data
            int i = 0, j = 0;
            if (isJsonObjArray)
            {
                for (i = 0; i < obj.Length; i++, j = 0)
                {
                    sheetName = obj[i][sheetColDate].ToString().Substring(substrStart, substrLen);
                    worksheetSize = getWorkSheetSize(excel2.mWSheet);
                    //if (dataRow <= worksheetSize[0])
                    dataRow = worksheetSize[0] + 1;
                    foreach (KeyValuePair<string, string> kvp in jsonDict)
                    {
                        //xlWorkSheet.Cells[Col][ROW]]
                        excel2.mWSheet.Cells[j + 1][dataRow].Value = obj[i][kvp.Key];
                        if (hyperlinkCol > 0 && (j + 1 == hyperlinkCol))
                        {
                            string cellIdx = cellIdxToStr(dataRow, j + 1);
                            setHyperlink(excel2.mWSheet, Convert.ToString(obj[i][kvp.Key]), cellIdx);
                        }

                        j++;
                    }
                    //dataRow++;
                }
            }
            else
            {
                foreach (KeyValuePair<string, string> kvp in jsonDict)
                {
                    excel2.mWSheet.Cells[j + 1][dataRow].Value = obj[kvp.Key];
                    if (hyperlinkCol > 0 && (j + 1 == hyperlinkCol))
                    {
                        //setHyperlink(excel2.mWSheet, obj[kvp.Key], dataRow, j + 1);
                        string cellIdx = cellIdxToStr(dataRow, j + 1);
                        setHyperlink(excel2.mWSheet, Convert.ToString(obj[kvp.Key]), cellIdx);
                    }
                    j++;
                }
            }
        }

        //header 例如：1:40
        //取得每一页的大小
        public int getPageSize(string header)
        {
            string[] headerIdx = header.Split(':');
            return int.Parse(headerIdx[1]) - int.Parse(headerIdx[0]) + 1;
        }


        //Dictionary<string, string> headerAndFooter -
        //{ {"reportType", "貼帶報表"}, { "header", "1:7" }, { "footer", "10:10" }, { "pageDataRows", "31" }, { "dataRowStart", "8" }, { "columnEnd", "N"}, { "additionalRowCnt", "8"}}
        //reportType ： 系統到 Property 找 Template
        //headerRows,  Row 1:5, header 在 Template 的 Rows
        //footerRows,  Row 10:10, footer 在 Template 的 Rows
        //dataRowStart, 資料開始的 RowNO
        //pageLength : 
        //回傳 已經 寫到第幾頁
        public int report_dtPageMode(System.Data.DataTable dt, ExcelElements ee, Dictionary<string, string> headerAndFooter,
                                     int currPage = 1, bool copyFormat = true,
                                      string targetSheet = "報表", string template = "Template", bool useDgvHear = false,
                                      bool withBorder = true, int fontSize = 12, string dataColsIdx=null)
        {
            int currDataIdx = 0;
            if (currPage <= 0)
                currPage = 1;
            int pageSize = getPageSize(headerAndFooter["header"]);
            int rowsPerPage = int.Parse(headerAndFooter["PageRows"]);
            int dataPerPage = int.Parse(headerAndFooter["pageDataRows"]);
            int excelStartRow = ((pageSize + 1) * (currPage - 1)) + 1;
            Microsoft.Office.Interop.Excel.Worksheet targetWS = getWorksheet(ee, targetSheet);
            Microsoft.Office.Interop.Excel.Worksheet templateWS = getWorksheet(ee, template);
            ee.oXL.StandardFontSize = fontSize;

            while (true)
            {
                excelStartRow = ((pageSize + 1) * (currPage - 1)) + 1;
                //處理 Header
                copyPasteRange(templateWS, targetWS, headerAndFooter["header"], "A" + excelStartRow.ToString(), false);
                if (excelStartRow == 1)
                    copyPasteSpecial(templateWS, targetWS, null, null, XlPasteType.xlPasteFormats);
                //int rowStart = dt.Rows.Count / dt.Columns.Count;
                //rowStart = rowStart == 0 ? 1 : rowStart;
                int printRowIdx = int.Parse(headerAndFooter["dataRowStart"]) + excelStartRow - 1;

                //處理 Data 
                //bool showColumnName = true, int startRow = 1, int startCol = 1, int dtIdx = 0, int maxData = int.MaxValue)
                currDataIdx = dataTableToExcel(dt, targetWS, false, printRowIdx, 1, currDataIdx, dataPerPage, dataColsIdx);
                if (dt.Rows.Count > currDataIdx)
                    currPage++;
                else
                    break;
                currDataIdx++;
                targetWS.HPageBreaks.Add(targetWS.Range[(rowsPerPage + 2).ToString() + ":" + (rowsPerPage + 1).ToString()]);
            }
            return currPage;
        }
        public int report_dtPageModeNewpageDontPlusOneRow(System.Data.DataTable dt, ExcelElements ee, Dictionary<string, string> headerAndFooter,
                                     int currPage = 1, bool copyFormat = true,
                                      string targetSheet = "報表", string template = "Template", bool useDgvHear = false,
                                      bool withBorder = true, int fontSize = 12, string dataColsIdx = null)
        {
            int currDataIdx = 0;
            if (currPage <= 0)
                currPage = 1;
            int pageSize = getPageSize(headerAndFooter["header"]);
            int rowsPerPage = int.Parse(headerAndFooter["PageRows"]);
            int dataPerPage = int.Parse(headerAndFooter["pageDataRows"]);
            int excelStartRow = ((pageSize + 1) * (currPage - 1)) + 1;
            Microsoft.Office.Interop.Excel.Worksheet targetWS = getWorksheet(ee, targetSheet);
            Microsoft.Office.Interop.Excel.Worksheet templateWS = getWorksheet(ee, template);
            ee.oXL.StandardFontSize = fontSize;

            while (true)
            {
                excelStartRow = ((pageSize) * (currPage - 1)) + 1;
                //處理 Header
                copyPasteRange(templateWS, targetWS, headerAndFooter["header"], "A" + excelStartRow.ToString(), false);
                if (excelStartRow == 1)
                    copyPasteSpecial(templateWS, targetWS, null, null, XlPasteType.xlPasteFormats);
                //int rowStart = dt.Rows.Count / dt.Columns.Count;
                //rowStart = rowStart == 0 ? 1 : rowStart;
                int printRowIdx = int.Parse(headerAndFooter["dataRowStart"]) + excelStartRow - 1;

                //處理 Data 
                //bool showColumnName = true, int startRow = 1, int startCol = 1, int dtIdx = 0, int maxData = int.MaxValue)
                currDataIdx = dataTableToExcel(dt, targetWS, false, printRowIdx, 1, currDataIdx, dataPerPage, dataColsIdx);
                if (dt.Rows.Count > currDataIdx)
                    currPage++;
                else
                    break;
                currDataIdx++;
                targetWS.HPageBreaks.Add(targetWS.Range[(rowsPerPage + 2).ToString() + ":" + (rowsPerPage + 1).ToString()]);
            }
            return currPage;
        }
        //工令單格式:{'PageMode':'1','PageRows':'33', 'pageRange':'1:33','header':'1:8','pageDataRows':'24','dataRowStart':'9'}
        //setBorder excledata是否要畫線
        public int report_dtPageMode_new(System.Data.DataTable dt, ExcelElements ee, Dictionary<string, string> headerAndFooter, Dictionary<string, string> cellsData,
                                    int currPage = 1, bool copyFormat = true,
                                     string targetSheet = "報表", string template = "Template", string copytemplate = "copyTemplate", bool useDgvHear = false,
                                     bool withBorder = true, int fontSize = 12, bool setBorder = false, bool setBottomline = false, XlLineStyle xlLineStyle = XlLineStyle.xlDash)
        {
            int currDataIdx = 0;
            if (currPage <= 0)
                currPage = 1;
            int endofdatarows = 0;
            int headerSize = getPageSize(headerAndFooter["header"]);
            int pageSize = int.Parse(headerAndFooter["PageRows"]);
            int dataPerPage = int.Parse(headerAndFooter["pageDataRows"]);
            int pageMode = int.Parse(headerAndFooter["PageMode"]);
            int excelStartRow = ((pageSize + 1) * (currPage - 1)) + 1;
            string columnEnd = headerAndFooter["columnEnd"];
            Microsoft.Office.Interop.Excel.Worksheet targetWS = getWorksheet(ee, targetSheet);
            Microsoft.Office.Interop.Excel.Worksheet templateWS = getWorksheet(ee, template);
            Microsoft.Office.Interop.Excel.Worksheet copytemplateWS = getWorksheet(ee, copytemplate);
            List<Dictionary<string, String>> toExcelList = new List<Dictionary<string, String>>();
            toExcelList.Add(cellsData);
            ee.oXL.StandardFontSize = fontSize;
            //設定欄寬
            //targetWS.Cells.EntireColumn.AutoFit();
            //xlWorkSheet.Cells.EntireColumn.ColumnWidth = 10;
            //targetWS.Columns["C"].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
            //targetWS.Columns["C"].ColumnWidth = 20;
            //columnDelete(ref targetWS, "A1");
            while (true)
            {
                excelStartRow = ((pageSize) * (currPage - 1)) + 1;
                //將母板被寫過的資料表用新的蓋過
                copyPasteRange(copytemplateWS, templateWS, headerAndFooter["pageRange"], "A" + pageMode.ToString(), false);
                int remainDtRows = dt.Rows.Count - currDataIdx;
                if (remainDtRows > dataPerPage)
                    endofdatarows = headerSize + dataPerPage + 1;
                else
                    endofdatarows = headerSize + remainDtRows + 1;

                outputToExcel4(templateWS, targetWS, toExcelList, endofdatarows);

                copyPasteRange(templateWS, targetWS, headerAndFooter["pageRange"], "A" + excelStartRow.ToString(), false);
                if (excelStartRow == 1)
                    copyPasteSpecial(copytemplateWS, targetWS, null, null, XlPasteType.xlPasteFormats);
                //int rowStart = dt.Rows.Count / dt.Columns.Count;
                //rowStart = rowStart == 0 ? 1 : rowStart;
                int printRowIdx = int.Parse(headerAndFooter["dataRowStart"]) + excelStartRow - 1;

                //處理 Data 
                //bool showColumnName = true, int startRow = 1, int startCol = 1, int dtIdx = 0, int maxData = int.MaxValue)
                currDataIdx = dataTableToExcel(dt, targetWS, false, printRowIdx, 1, currDataIdx, dataPerPage);
                if (setBorder == true)
                {
                    setBorderToRange(targetWS, $"A{printRowIdx}", columnEnd + Convert.ToString(endofdatarows - 1), xlLineStyle: xlLineStyle);
                }
                if (setBottomline == true)
                {
                    Microsoft.Office.Interop.Excel.Range tRange = targetWS.get_Range("A" + Convert.ToString(endofdatarows - 1), columnEnd + Convert.ToString(endofdatarows - 1));
                    tRange.Borders[XlBordersIndex.xlEdgeTop].LineStyle = 1;
                    tRange.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                }
                if (dt.Rows.Count > currDataIdx)
                    currPage++;
                else
                    break;
                if (currPage == 1)
                    targetWS.HPageBreaks.Add(targetWS.Range[(pageSize + 2).ToString() + ":" + (pageSize + 1).ToString()]);
                else
                    targetWS.HPageBreaks.Add(targetWS.Range[(pageSize + 1).ToString() + ":" + (pageSize + 1).ToString()]);

            }
            return currPage;
        }


        //合併儲存格
        //align 0:left, 1:middle, 2:right
        public void excelMerge(Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet, string startCell, string endCell, bool convertToTextFormate = true, int align = 0)
        {
            Microsoft.Office.Interop.Excel.Range tRange = xlWorkSheet.get_Range(startCell, endCell);
            if (convertToTextFormate)
                tRange.NumberFormat = "@";
            tRange.Merge(tRange.MergeCells); //儲存格合併動作
            switch (align)
            {
                case 0:
                    tRange.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                    break;
                case 1:
                    tRange.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    break;
                case 2:
                    tRange.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                    break;
            }
        }
        //Alien, LeftT/Top:0. Center:1, Right/Bottom:2, 
        //For Example writeCell(xlWorkSheet, "A1", title,1,1);
        public void writeCell(Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet, string theCell, string theText, int verticalAlien = 1, int horizontalAlien = 1)
        {
            fillExcelCellValue(xlWorkSheet, theCell, theText);
            switch (verticalAlien)
            {
                case 0:
                    xlWorkSheet.Range[theCell].Style.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignTop;
                    break;
                case 1:
                    xlWorkSheet.Range[theCell].Style.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                    break;
                case 2:
                    xlWorkSheet.Range[theCell].Style.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignBottom;
                    break;
            }
            switch (horizontalAlien)
            {
                case 0:
                    xlWorkSheet.Range[theCell].Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                    break;
                case 1:
                    xlWorkSheet.Range[theCell].Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    break;
                case 2:
                    xlWorkSheet.Range[theCell].Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                    break;
            }

        }

        //劃出 Excel border
        //rangeStart: "A2",  rangeEnd: "G120"
        //回傳 Worksheet 的 Col 數量 於 Row 數量
        public static Dictionary<string, int> getExcelSize(Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet)
        {
            Range xlRange = xlWorkSheet.UsedRange;
            int rowCount = xlRange.Rows.Count;
            int columnCount = xlRange.Columns.Count;
            return new Dictionary<string, int>() { { "Col", columnCount}, { "Row", rowCount } };
        }


        public static void setBorderToRange(Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet, string rangeStart, string rangeEnd, bool convertToTextFormate = true, XlLineStyle xlLineStyle = XlLineStyle.xlContinuous, bool bordersIndex = false, XlBordersIndex xlEdgeType = XlBordersIndex.xlEdgeBottom)
        {
            Microsoft.Office.Interop.Excel.Range tRange = xlWorkSheet.get_Range(rangeStart, rangeEnd);
            if (convertToTextFormate)
                tRange.NumberFormat = "@";
            if (bordersIndex)
                tRange.get_Item(xlEdgeType).LineStyle = xlLineStyle;
            else
                tRange.Borders.LineStyle = xlLineStyle;
            tRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
        }
        public void setFontToRange(Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet, string rangeStart = null, string rangeEnd = null, int fontSize = 14, bool fontBold = false)
        {
            Microsoft.Office.Interop.Excel.Range tRange = null;
            if (rangeStart == null)
                tRange = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.UsedRange;
            else
                tRange = xlWorkSheet.get_Range(rangeStart, rangeEnd);
            tRange.Font.Size = fontSize;
            tRange.Font.Bold = fontBold;
        }
        private void copyAlltoClipboard(DataGridView dgv)
        {
            dgv.SelectAll();
            DataObject dataObj = dgv.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }
        private void writeDgvToExcelHeader(DataGridView dgv, Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet, int startRow = 1)
        {
            /*var startCell = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[1, 1];
            var endCell = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[1, 1];
            var writeRange = xlWorkSheet.Range[startCell, endCell];*/

            xlWorkSheet.Cells[1, 1] = dgv.TopLeftHeaderCell.FormattedValue;
            for (int j = 0; j <= dgv.ColumnCount - 1; j++)
            {
                var startCell = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[startRow, j + 2];
                var endCell = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[startRow, j + 2];
                var writeRange = xlWorkSheet.Range[startCell, endCell];
                writeRange.Value2 = dgv.Columns[j].HeaderText;
            }
        }

        //DataGridView 輸出 Excel

        public bool ifWorksheetExists(Microsoft.Office.Interop.Excel.Sheets mWorkSheets, string worksheetName)
        {
            bool ifFound = false;
            // Loop through all worksheets in the workbook
            foreach (Microsoft.Office.Interop.Excel.Worksheet sheet in mWorkSheets)
            {
                // Check the name of the current sheet
                if (sheet.Name == worksheetName)
                {
                    ifFound = true;
                    break; // Exit the loop now
                }
            }
            return ifFound;
        }

        public void dtToExcel(Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet, System.Data.DataTable dt)
        {
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                //注意Excel表的行和列的索引都是從1開始的
                xlWorkSheet.Cells[1, i + 1] = dt.Columns[i].ColumnName;
            }
            //遍歷DataTable，給Excel賦值
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    //從第二行第一列開始寫入資料
                    xlWorkSheet.Cells[i + 2, j + 1] = dt.Rows[i][j];
                }
            }
        }

        public void dtToExcel(System.Data.DataTable dt)
        {
            Microsoft.Office.Interop.Excel.Application xlexcel;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlexcel = new Microsoft.Office.Interop.Excel.Application();
            xlexcel.Visible = true;
            xlWorkBook = xlexcel.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            dtToExcel(xlWorkSheet, dt);
        }

        public void dtToExcel(System.Data.DataTable dt, string savePath)
        {
            ExcelElements ee = openExcel("", true);
            dtToExcel(ee.mWSheet, dt);
            closeExcel(ee, savePath, false);
        }

        //標準前置
        //String FileFullPath = Constants.excelFilePath + "標準\\" + "MFO_Sheet.xlsx";
        //Microsoft.Office.Interop.Excel.Application oXL = null;
        //object misValue = System.Reflection.Missing.Value;
        //Microsoft.Office.Interop.Excel.Workbook mWorkBook = null;
        public Microsoft.Office.Interop.Excel.Worksheet openExcelSheet(ref Microsoft.Office.Interop.Excel.Application oXL, ref Microsoft.Office.Interop.Excel.Workbook mWorkBook,
                String fileExcel, String workSheet, bool isVisible = false)
        {
            if (oXL == null)
                oXL = new Microsoft.Office.Interop.Excel.Application();
            oXL.Visible = isVisible;
            oXL.DisplayAlerts = false;
            if (mWorkBook == null)
                mWorkBook = oXL.Workbooks.Open(fileExcel, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            //Get all the sheets in the workbook
            Microsoft.Office.Interop.Excel.Sheets mWorkSheets = mWorkBook.Worksheets;
            //Get the allready exists sheet
            return (Microsoft.Office.Interop.Excel.Worksheet)mWorkSheets.get_Item(workSheet);
        }

        public ExcelElements openExcelSheet2(ExcelElements excelObj, String workSheet)
        {
            if (string.IsNullOrEmpty(workSheet) || ifWorksheetExists(excelObj.mWorkBook.Worksheets, workSheet))
                excelObj.mWSheet = excelObj.mWorkBook.Worksheets["workSheet"];
            return excelObj;
        }

        public ExcelElements openExcelSheet2(String fileExcel, String workSheet)
        {
            ExcelElements excelObj = openExcel(fileExcel);
            return openExcelSheet2(excelObj, workSheet);
        }

        //從傳入的 Excel 物件取得 worksheet, 如果不存在則 新增 一個 worksheet 
        public Microsoft.Office.Interop.Excel.Worksheet getWorksheet(ExcelElements ee, String workSheetName)
        {
            Microsoft.Office.Interop.Excel.Worksheet wks;
            try
            {
                wks = ee.mWorkBook.Worksheets[workSheetName];
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                wks = ee.mWorkSheets.Add(Type.Missing, ee.mWorkSheets[ee.mWorkSheets.Count], 1, XlSheetType.xlWorksheet) as Worksheet;
                wks.Name = workSheetName;
            }
            return wks;
        }

        //從傳入的 Excel 物件取得 worksheet, 如果不存在則 新增 一個 worksheet 
        public Microsoft.Office.Interop.Excel.Worksheet openExcelSheet(ExcelElements excelEE, String workSheetName)
        {
            return getWorksheet(excelEE, workSheetName);
        }

        //新增一個新的 worksheet
        public ExcelElements createExcelSheet(ExcelElements excelEE, String workSheetName)
        {
            var xlNewSheet = excelEE.mWorkSheets.Add(Type.Missing, Type.Missing, Type.Missing);
            xlNewSheet.Name = workSheetName;
            return excelEE;
        }

        public ExcelElements copyExcelSheet(ExcelElements excelEE, String fromWorkSheetName, String toWorkSheetName)
        {
            var xlNewSheet = excelEE.mWorkSheets[fromWorkSheetName].Copy(excelEE.mWorkSheets[excelEE.mWorkSheets.Count]);
            excelEE.mWorkSheets[excelEE.mWorkSheets.Count - 1].Name = toWorkSheetName;
            return excelEE;
        }

        public ExcelElements openExcel(String fileExcel, bool ifVisible = true)
        {
            ExcelElements rtnEE = new ExcelElements();
            rtnEE.oXL = new Microsoft.Office.Interop.Excel.Application();
            rtnEE.oXL.Visible = ifVisible;
            rtnEE.oXL.DisplayAlerts = false;
            object misValue = System.Reflection.Missing.Value;
            if (fileExcel == null)
                rtnEE.mWorkBook = rtnEE.oXL.Workbooks.Add(misValue);
            else
                rtnEE.mWorkBook = rtnEE.oXL.Workbooks.Open(fileExcel, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);

            //Get all the sheets in the workbook
            rtnEE.mWorkSheets = rtnEE.mWorkBook.Worksheets;
            rtnEE.mWSheet = rtnEE.mWorkBook.Worksheets[1];
            //Get the allready exists sheet
            return rtnEE;
        }

       
        public void closeExcel(ref Microsoft.Office.Interop.Excel.Application oXL,
            ref Microsoft.Office.Interop.Excel.Workbook mWorkBook, string saveAsPath = null, bool ifSave = true)
        {
            if (saveAsPath != null)
            {
                //mWorkBook.SaveAs(saveAsPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
                mWorkBook.SaveAs(saveAsPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook, Missing.Value,
                Missing.Value, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true,
                Missing.Value, Missing.Value, Missing.Value);
            }
            else if (ifSave)
                mWorkBook.Save();

            object misValue = System.Reflection.Missing.Value;
            mWorkBook.Close(false, misValue, misValue);

            Marshal.FinalReleaseComObject(mWorkBook);
            oXL.Quit();
            Marshal.FinalReleaseComObject(oXL);
            oXL = null;
            mWorkBook = null;
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public void closeExcel(ExcelElements excelEE, string saveAsPath = null, bool ifSave = true)
        {
            
            closeExcel(ref excelEE.oXL, ref excelEE.mWorkBook, saveAsPath, ifSave);
        }

        public void KillSpecificExcelFileProcess(string processName, string fileName)
        {
            var processes = from p in Process.GetProcessesByName("Microsoft Excel")
                            select p;

            foreach (var process in processes)
            {
                if (process.MainWindowTitle == "Microsoft Excel - " + fileName)
                    process.Kill();
            }
        }

        public void killAllEccels(bool ifCheckProperty = true)
        {
            if (ifCheckProperty && Constants.getProperty("ifKillAllExcel") == "0")
                return;
            CommonClass.execCmd(@"C:\Windows\System32\taskkill.exe", "/f /t /im EXCEL.EXE");
        }

        //startRow： 從第幾個Row 開始寫起
        public bool verticalDictionaryToExcel(Dictionary<string, string> dt, string FileParth, string SheetName, string startRow)
        {
            if (dt == null || dt.Count == 0)
            {
                throw new Exception("ExportToExcel: Null or empty input table!\n");
            }

            Microsoft.Office.Interop.Excel.Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel._Worksheet xlWorkSheet;
            Microsoft.Office.Interop.Excel.Range xlRange = null;
            object misValue = Missing.Value;

            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlApp.Visible = true;

            xlWorkBook = xlApp.Workbooks.Open(FileParth, misValue, false, misValue, misValue, misValue, true, misValue, misValue, misValue, misValue, misValue, false, misValue, misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Sheets[SheetName];
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Sheets[1];
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.ActiveSheet;
            xlWorkSheet.Activate();

            xlRange = xlWorkSheet.get_Range("A" + startRow, "ZZ9999");
            int i = 0;

            //Datas

            foreach (KeyValuePair<string, string> entry in dt)
            {
                xlRange.Cells[i + 2, 1] = entry.Key;
                xlRange.Cells[i + 2, 2] = entry.Value;
                i++;
            }
            if (FileParth != null || FileParth != "")
            {
                try
                {
                    //xlApp.ActiveWorkbook.SaveAs(FileParth);
                    xlApp.ActiveWorkbook.SaveAs(FileParth, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook, Missing.Value,
                    Missing.Value, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                    Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true,
                    Missing.Value, Missing.Value, Missing.Value);
                    xlWorkBook.Close();
                    Marshal.FinalReleaseComObject(xlWorkBook);

                    xlApp.Quit();
                    Marshal.FinalReleaseComObject(xlApp);
                }
                catch (Exception ex)
                {
                    throw new Exception("Can not save file" + ex.Message);
                }
            }
            else
            {
                xlApp.Visible = true;
            }
            return true;
        }
        public bool horizontalDictionaryToExcel(Dictionary<string, string> dt, string FileParth, string SheetName, int startCol)
        {
            if (dt == null || dt.Count == 0)
            {
                throw new Exception("ExportToExcel: Null or empty input table!\n");
            }

            Microsoft.Office.Interop.Excel.Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel._Worksheet xlWorkSheet;
            Microsoft.Office.Interop.Excel.Range xlRange = null;
            object misValue = Missing.Value;

            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlApp.Visible = true;

            xlWorkBook = xlApp.Workbooks.Open(FileParth, misValue, false, misValue, misValue, misValue, true, misValue, misValue, misValue, misValue, misValue, false, misValue, misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Sheets[SheetName];
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Sheets[1];
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.ActiveSheet;
            xlWorkSheet.Activate();
            char c = 'A';
            c += (char)startCol;
            xlRange = xlWorkSheet.get_Range(c.ToString(), "ZZ9999");
            int i = 0;

            //Datas

            foreach (KeyValuePair<string, string> entry in dt)
            {
                xlRange.Cells[1, i + 1] = entry.Key;
                xlRange.Cells[2, i + 2] = entry.Value;
                i++;
            }
            if (FileParth != null || FileParth != "")
            {
                try
                {
                    //xlApp.ActiveWorkbook.SaveAs(FileParth);
                    xlApp.ActiveWorkbook.SaveAs(FileParth, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook, Missing.Value,
                    Missing.Value, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                    Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true,
                    Missing.Value, Missing.Value, Missing.Value);
                    xlWorkBook.Close();
                    Marshal.FinalReleaseComObject(xlWorkBook);

                    xlApp.Quit();
                    Marshal.FinalReleaseComObject(xlApp);
                }
                catch (Exception ex)
                {
                    throw new Exception("Can not save file" + ex.Message);
                }
            }
            else
            {
                xlApp.Visible = true;
            }
            return true;
        }

        /// <summary>
        /// 取得 Excel 文件中所有工作表名
        /// </summary>
        /// <param name="excelFile">Excel 檔案路徑</param>
        /// <returns>string[] 工作表名稱集合</returns>
        private String[] GetExcelSheetNames(string excelFile)
        {
            System.Data.DataTable dt = null;
            try
            {
                //string connString = "Provider=Microsoft.Jet.OleDb.4.0;" + "data source=" + excelFile + ";Extended Properties='Excel 8.0; HDR=NO; IMEX=1'"; //此連接只能操作Excel2007之前(.xls)文件
                string connString = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + excelFile + ";Extended Properties='Excel 12.0; HDR=YES; IMEX=1'"; //此連接可以操作.xls與.xlsx文件

                // 建立連結
                using (OleDbConnection objConn = new OleDbConnection(connString))
                {
                    objConn.Open();

                    // 取得 Excel 資料結構
                    dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                        return null;

                    // 取得 Excel 資料表中的工作表
                    var excelSheets = dt.AsEnumerable().Where(r => r["TABLE_NAME"].ToString().Contains("$")).Select(s => s["TABLE_NAME"].ToString()).ToArray();
                    if (dt != null)
                        dt.Dispose();
                    return excelSheets;
                }
            }
            catch (Exception ex)
            {
                if (dt != null)
                    dt.Dispose();
                throw;
            }
        }

        /// 取得 Excel 文件中指定工作表的內容
        /// </summary>
        /// <param name="FileFullPath">檔案路徑</param>
        /// <param name="SheetName">工作表名稱</param>
        /// <returns>DataTable 工作表內容</returns>
        /// 記得要在 SheetName 後面加上 $, 例如 薪資表$
        public System.Data.DataTable GetExcelSheetData(string FileFullPath, string SheetName)
        {

            string strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + FileFullPath + ";Extended Properties='Excel 12.0; HDR=YES; IMEX=1'"; //此連接可以操作.xls與.xlsx文件
            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                conn.Open();
                DataSet ds = new DataSet();

                OleDbDataAdapter odda = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}]", SheetName), conn);
                odda.Fill(ds, SheetName);
                return ds.Tables[0];
            }
        }

        //excelFile 檔案名稱
        //startRow: 從第幾個 Row 開始讀起， 第一個 Row 為 0
        public string[][] importFromEcxel(String excelFile, String destTable)
        {
            if (excelFile == null || excelFile == "")
            {
                return null;
            }
            string[][] stringArr = null;
            RegExcel[] RegExcelPath = {new RegExcel(@"SOFTWARE\Microsoft\Jet\3.5\Engines\Excel" ,"",false)
                                                    , new RegExcel(@"SOFTWARE\Microsoft\Jet\4.0\Engines\Excel","",false)
                                                    , new RegExcel(@"SOFTWARE\Wow6432Node\Microsoft\Jet\4.0\Engines\Excel","",false)
                                                    , new RegExcel(@"SOFTWARE\Wow6432Node\Microsoft\Office\14.0\Access Connectivity Engine\Engines\Excel","",false)
                                                    , new RegExcel(@"SOFTWARE\Wow6432Node\Microsoft\Office\12.0\Access Connectivity Engine\Engines\Excel","",false)
                                                    , new RegExcel(@"SOFTWARE\Microsoft\Office\14.0\Access Connectivity Engine\Engines\Excel","",false) };
            string[] SheetName = GetExcelSheetNames(excelFile);
            switch (System.IO.Path.GetExtension(excelFile))
            {
                case ".xls":
                case ".xlsx":
                    //設定 excel 匯入資料的模式
                    //for (int i = 0; i < RegExcelPath.Length; i++)
                    //{
                    //    using (RegistryKey myKey = Registry.LocalMachine.OpenSubKey(RegExcelPath[i].Path, true))
                    //    {
                    //        if (myKey != null)
                    //        //檢查子機碼是否存在，檢查資料夾是否存在。
                    //        {
                    //            //若目錄存在，則取出 key=cnstr 的值。
                    //            RegExcelPath[i].DefaultValue = myKey.GetValue("TypeGuessRows").ToString();
                    //            RegExcelPath[i].Exist = true;
                    //            myKey.SetValue("TypeGuessRows", "0", RegistryValueKind.DWord);
                    //        }
                    //    }
                    //}

                    System.Data.DataTable dataTable = GetExcelSheetData(excelFile, SheetName[0]);
                    stringArr = dataTable.AsEnumerable().Select(r => r.ItemArray.Select(ra => ra.ToString()).ToArray()).ToArray();
                    dataTable.Dispose();
                    break;
                case ".txt":
                    using (StreamReader sr = new StreamReader(excelFile, Encoding.Default))
                    {
                        sr.ReadLine();//去掉標題列
                        stringArr = (sr.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.None)).ToArray().Select(r => r.Trim().Split('\t')).ToArray();
                    }
                    break;
            }
            if (stringArr.Length <= 0)
            {
                MessageBox.Show("匯入資料不符合格式，請重新選擇檔案進行匯入！");
                return null;
            }
            // Excel 資料欄位至少大於2欄
            if (stringArr[0].Length < 2)
            {
                MessageBox.Show("匯入資料不符合格式，請重新選擇檔案進行匯入！");
                return null;
            }
            return stringArr;
        }

        //
        public void printExcel(Microsoft.Office.Interop.Excel.Worksheet mWSheet)
        {
            mWSheet.PrintOut(
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        }

        //指定印表機列印
        //printer： 印表機名稱( 從 Control panel 那邊拿)， 例如 printer = @"\\192.168.1.150\Argox CP-3140EX PPLZ"
        public void printExcelBySpecifyPrinter(Microsoft.Office.Interop.Excel.Application _xlApp, 
            Microsoft.Office.Interop.Excel.Workbook _xlBook,
            Microsoft.Office.Interop.Excel.Worksheet mWSheet, string printer)
        {
            // Find correct printerport
            string port = null;
            string path = @"Software\Microsoft\Windows NT\CurrentVersion\Devices";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path))
            {
                if (key != null)
                {
                    object value = key.GetValue(printer);
                    if (value != null)
                    {
                        string[] values = value.ToString().Split(',');
                        if (values.Length >= 2) port = values[1];
                    }
                }
            }

            // Set ActivePrinter if not already set
            if (!_xlApp.ActivePrinter.StartsWith(printer))
            {
                // Get current concatenation string ('on' in enlgish, 'op' in dutch, etc..)
                var split = _xlApp.ActivePrinter.Split(' ');
                if (split.Length >= 3)
                {
                    _xlApp.ActivePrinter = String.Format("{0} {1} {2}",
                        printer,
                        split[split.Length - 2],
                        port);
                }
            }

            // Print document
            _xlBook.PrintOutEx();
        }

        public string printExcelBySpecifyPrinter(ExcelElements excelEE, string printer)
        {
            try
            {
                // Find correct printerport
                var port = String.Empty;
                string path = @"Software\Microsoft\Windows NT\CurrentVersion\Devices";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(printer);
                        if (value != null)
                        {
                            string[] values = value.ToString().Split(',');
                            if (values.Length >= 2) port = values[1];
                        }
                    }
                }

                // Set ActivePrinter if not already set
                if (!excelEE.oXL.ActivePrinter.StartsWith(printer))
                {
                    // Get current concatenation string ('on' in enlgish, 'op' in dutch, etc..)
                    var split = excelEE.oXL.ActivePrinter.Split(' ');
                    if (split.Length >= 3)
                    {
                        excelEE.oXL.ActivePrinter = String.Format("{0} {1} {2}",
                            printer,
                            split[split.Length - 2],
                            port);
                    }
                }
                // Print document
                //excelEE.mWorkBook.PrintOutEx();
                excelEE.mWSheet.PrintOutEx();
                return null;
            } catch(Exception ex)
            {
                return "printExcelBySpecifyPrinter(" + printer + ") 選取印表機錯誤：" + ex.Message;
            }
        }
        public void removeWorksheet(ExcelElements excel2, String sheetName)
        {
            if (string.IsNullOrEmpty(sheetName) || ifWorksheetExists(excel2.mWorkBook.Worksheets, sheetName))
            {
                return;
            }
            excel2.oXL.DisplayAlerts = false;
            Microsoft.Office.Interop.Excel.Worksheet targetWorkSheet = excel2.mWorkBook.Worksheets[sheetName];
            targetWorkSheet.Delete();
            excel2.oXL.DisplayAlerts = true;
        }

        public void deleteRowsFromExcel(ExcelElements excel2, DataRow myRow, string sheetName)
        {
            System.Data.DataTable excelDT = null;
            int rowIdx = -1;

            //excel2 = openExcelSheet(excel2, myRow["sheetName"].ToString(), "母版");
            excelDT = getWorksheetAsDataTable(excel2.mWorkSheets.get_Item(myRow["sheetName"]), new int[] { 1 }, 1, 2);
            rowIdx = CommonClass.searchInDTRtnIdx(excelDT, "`pdf 檔`='" + myRow["pdf檔"].ToString() + "'");
            if (rowIdx >= 0)
            {
                delExcelRow(excel2.mWorkSheets.get_Item(myRow["sheetName"]), rowIdx + 1);
            }
            int[] wsize = getWorkSheetSize(excel2.mWorkSheets.get_Item(myRow["sheetName"]));
            if (wsize[0] == 1)
                removeWorksheet(excel2, myRow["sheetName"].ToString());

        }

        public void delAllExcelRowAfter(Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet, int ExcelRow)
        {
            int lastRow = xlWorkSheet.UsedRange.Row + xlWorkSheet.UsedRange.Rows.Count - 1;
            Microsoft.Office.Interop.Excel.Range TempRange = xlWorkSheet.get_Range("A" + (ExcelRow + 1).ToString(), "A" + lastRow.ToString());
            TempRange.EntireRow.Delete(null);
        }

        //Dictionary 的 Key 為 Cell indrx, 例如 A1, A2,Z8.., Value 則為填入 Cell 的1值
        public void dictToExcel(Microsoft.Office.Interop.Excel.Worksheet templateWS, Dictionary<string, string> cellDataDict)
        {
            foreach (var paramItem in cellDataDict)
            {
                fillExcelCellValue(templateWS, paramItem.Key, paramItem.Value);
            }
        }

        //pageBreak : 每 Copy/Paste 幾次, 加一個 跳頁符號
        //excelPath--barcode 檔案位置, 如果為 null, 則以 saveAsPath 的目錄為路徑
        //rowOffset 每個 Label 佔幾個 Row
        //colOffset 每個 Label 佔幾個 Col
        //rowRepeat : 每個 Row 印幾個 Label
        //qrSize : 20pixel 的 N 倍
        public int outputToExcel(Microsoft.Office.Interop.Excel.Worksheet templateWS, Microsoft.Office.Interop.Excel.Worksheet targetWS,
               Microsoft.Office.Interop.Excel.Worksheet cleanFormWS,
               List<Dictionary<string, String>> toExcelList, string tempRange, int pageBreak = 1,
               bool ifPrint = false, bool ifClose = true, int colOffset = 0, int rowOffset = 0, int rowRepeat = 1, string excelPath = null,
               float qrSize = 1, int qrTopMargin = 5)
        {
            int rtnLineCnt = 0;
            bool skipLabel = false; //skipLabel : 保留空白不列印
            int tempRangeRows = this.getRangeRows(tempRange); //tenplate 涵蓋範圍的 ROW number
            string barFileResult = null;
            int currPageRows = 0;
            rowRepeat = rowRepeat == 0 ? 1 : rowRepeat;
            //Microsoft.Office.Interop.Excel.Worksheet mWSheet1;          //Target Worksheet
            //Microsoft.Office.Interop.Excel.Worksheet mWSheet2 = null;   //範本
            if (string.IsNullOrEmpty(excelPath))
                excelPath = CommonClass.getCurrDir();

            string tempBarCodeFile;
            if (CommonClass.Right(excelPath, 1) != @"\")
                tempBarCodeFile = excelPath + @"\" + "tempBarcode";
            else
                tempBarCodeFile = excelPath + "tempBarcode";

            Microsoft.Office.Interop.Excel.Range range = targetWS.UsedRange;
            int currColOffset = 0; //標籤寬
            int currRowOffset = 0; //標籤長
            int listidx = 0;
            foreach (Dictionary<string, string> cellDataDict in toExcelList)
            {
                if (cellDataDict == null)
                {
                    skipLabel = true; //skipLabel : 保留空白不列印
                }
                else
                {
                    foreach (var paramItem in cellDataDict)
                    {
                        //QRCode:Content,size, 其中 size 為標準 size 的倍數
                        //例如 BARCODE: 1234567,1 ==> 內容,n倍
                        if (paramItem.Value.Length > 6 && paramItem.Value.Substring(0, 6).ToUpper() == "QRCODE")
                        {
                            String barCodeInfo = paramItem.Value.Substring(7);
                            //tempBarCodeFile += ".bmp";
                            //QrCode 的 Size 為 50*size pixel
                            barFileResult = BarCodeLib.generateBARCode("QR", barCodeInfo, tempBarCodeFile, qrSize);
                            int[] imgSize = CommonClass.getImgSize(barFileResult);
                            insertPicToCell(templateWS, paramItem.Key, barFileResult, (int)(imgSize[0] + 10), (int)(imgSize[1] + 10), qrTopMargin);
                            Thread.Sleep(0);
                        }
                        ////barCode:Content,Width,Heigh
                        else if (paramItem.Value.Length > 7 && paramItem.Value.Substring(0, 7).ToUpper() == "BARCODE")
                        {
                            string barCodeInfo = paramItem.Value.Substring(8);
                            barFileResult = BarCodeLib.generateBARCode("BAR", barCodeInfo, tempBarCodeFile, qrSize);
                            int[] imgSize = CommonClass.getImgSize(barFileResult);
                            insertPicToCell(templateWS, paramItem.Key, barFileResult, (int)(imgSize[0]), (int)(imgSize[1]), qrTopMargin);
                            Thread.Sleep(0);
                        }
                        else
                            fillExcelCellValue(templateWS, paramItem.Key, paramItem.Value);
                    }
                    skipLabel = false;
                }

                if (rowRepeat > 1) //每一個 Row 印幾張， 例如， 薪資單， 一張 A4 印 2X2 張， 故 rowReapeat = 2
                {
                    currColOffset = (listidx) % rowRepeat;
                }
                currRowOffset = listidx / rowRepeat;
                listidx++;

                if (currColOffset == 0)
                {   ////每個Row 的第一個以上
                    if (!skipLabel) //skipLabel : 保留空白不列印
                    {
                        //Copy Cell Height
                        copyPasteSpecial(cleanFormWS, targetWS, "1:" + rowOffset, (currRowOffset * rowOffset + 1) + ":" + ((currRowOffset + 1) * rowOffset), XlPasteType.xlPasteFormats);
                        copyPasteRange(templateWS, targetWS, tempRange, "A" + (currRowOffset * rowOffset + 1), false);
                        copyPasteRange(cleanFormWS, templateWS, tempRange, "A1", true);
                    }
                    currPageRows += tempRangeRows;
                }
                else //每個Row 第二個以上
                {
                    if (!skipLabel) //skipLabel : 保留空白不列印
                    {
                        copyPasteRange(templateWS, targetWS, tempRange, cellIdxToStr((currRowOffset * rowOffset + 1), colOffset + 1), false);
                        copyPasteRange(cleanFormWS, templateWS, tempRange, "A1", true);
                    }
                }

                if (listidx % pageBreak == 0)
                {
                    targetWS.HPageBreaks.Add(targetWS.Range["A" + ((listidx / rowRepeat) * rowOffset + 1).ToString()]);
                }
                rtnLineCnt++;
                System.Threading.Thread.Sleep(0);
                if (!keepPrint)
                {
                    MessageBox.Show("使用者中斷列印", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
            delAllExcelRowAfter(targetWS, currPageRows);
            CommonClass.deleteFiles(CommonClass.getFileInfo(excelPath, "directory"), DateTime.Now.AddDays(1), "bmp");
            if (ifPrint)
            {
                this.printExcel(targetWS);
            }
            keepPrint = true;
            return rtnLineCnt;
        }

        //public bool outputToExcel(string fileExcel, string workSheet, List<Dictionary<ExcelCellPos, String>> toExcelList, string saveAsPath, string templateSheet,
        //    string tempRange, int pageBreak = 0, bool ifPrint = false, bool ifClose = true, int colOffset = 0, int rowOffset = 0, int rowRepeat = 0)
        //{
        //    bool rtnBool = false;
        //    int tempRangeRows = this.getRangeRows(tempRange); //tenplate 涵蓋範圍的 ROW number
        //    int currPageRows = 0;
        //    Microsoft.Office.Interop.Excel.Workbook mWorkBook;
        //    Microsoft.Office.Interop.Excel.Sheets mWorkSheets;
        //    Microsoft.Office.Interop.Excel.Worksheet mWSheet1;
        //    Microsoft.Office.Interop.Excel.Worksheet mWSheet2 = null;
        //    Microsoft.Office.Interop.Excel.Application oXL;
        //    object misvalue = System.Reflection.Missing.Value;
        //    string path = fileExcel;
        //    oXL = new Microsoft.Office.Interop.Excel.Application();
        //    oXL.Visible = false;
        //    oXL.DisplayAlerts = false;
        //    mWorkBook = oXL.Workbooks.Open(path, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
        //    //Get all the sheets in the workbook
        //    mWorkSheets = mWorkBook.Worksheets;
        //    //Get the allready exists sheet
        //    mWSheet1 = (Microsoft.Office.Interop.Excel.Worksheet)mWorkSheets.get_Item(workSheet);       //資料來源
        //    if (templateSheet != null && templateSheet != "")
        //    {
        //        mWSheet2 = (Microsoft.Office.Interop.Excel.Worksheet)mWorkSheets.get_Item(templateSheet);   //範本
        //        ClearExcelWorkSheet(mWSheet1);
        //        insertExcelRow(mWSheet2, mWSheet1, tempRange, 0);
        //        currPageRows += tempRangeRows;
        //    }

        //    Microsoft.Office.Interop.Excel.Range range = mWSheet1.UsedRange;
        //    int currColOffset = 0;
        //    int currRowOffset = 0;
        //    int listidx = 0;
        //    foreach (Dictionary<ExcelCellPos, string> cellDataDict in toExcelList)
        //    {
        //        foreach (var paramItem in cellDataDict)
        //        {
        //            mWSheet1.Cells[paramItem.Key.colPos + rowOffset * currRowOffset, paramItem.Key.rowPos + colOffset * currColOffset] = paramItem.Value;
        //        }
        //        listidx++;
        //        if (rowRepeat != 0) //每一個 Row 印幾張， 例如， 薪資單， 一張 A4 印 2X2 張， 故 rowReapeat = 2
        //        {
        //            currColOffset = listidx % rowRepeat;
        //            currRowOffset = listidx / rowRepeat;
        //        }

        //        if (currColOffset == 0)
        //        {
        //            insertExcelRow(mWSheet2, mWSheet1, tempRange, (currRowOffset) * 25);
        //            currPageRows += tempRangeRows;
        //        }
        //        if (pageBreak != 0 && currPageRows % pageBreak == 0)
        //        {
        //            mWSheet1.HPageBreaks.Add(mWSheet1.Range["A" + currPageRows.ToString()]);
        //        }
        //    }
        //    if (ifPrint)
        //    {
        //        this.printExcel(mWSheet1);
        //    }
        //    //mWorkBook.SaveAs(saveAsPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
        //    mWorkBook.SaveAs(saveAsPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook, Missing.Value,
        //    Missing.Value, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
        //    Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true,
        //    Missing.Value, Missing.Value, Missing.Value);
        //    Thread.Sleep(2000);
        //    if (ifClose)
        //    {
        //        mWorkBook.Close();
        //        Marshal.FinalReleaseComObject(mWorkBook);
        //        mWSheet1 = null;
        //        mWorkBook = null;
        //        oXL.Quit();
        //        Marshal.FinalReleaseComObject(oXL);
        //        GC.WaitForPendingFinalizers();
        //        GC.Collect();
        //        GC.WaitForPendingFinalizers();
        //        GC.Collect();
        //    }
        //    rtnBool = true;
        //    return rtnBool;
        //}

        //public bool outputToExcel(Microsoft.Office.Interop.Excel.Worksheet mWSheetDest, Microsoft.Office.Interop.Excel.Worksheet mWSheetTemplate,
        //    List<Dictionary<ExcelCellPos, String>> toExcelList, string saveAsPath, string tempRange,
        //    bool clearWorksheet = false, int pageBreak = 0, bool ifPrint = false, bool ifClose = true, int colOffset = 0, int rowOffset = 0,
        //    int rowRepeat = 0)
        //{
        //    bool rtnBool = false;
        //    int tempRangeRows = this.getRangeRows(tempRange); //tenplate 涵蓋範圍的 ROW number
        //    int currPageRows = 0;
        //    int maxPages = rowRepeat == 0 ? toExcelList.Count : toExcelList.Count / rowRepeat;
        //    object misvalue = System.Reflection.Missing.Value;
        //    if (clearWorksheet)
        //    {
        //        ClearExcelWorkSheet(mWSheetDest);
        //        insertExcelRow(mWSheetTemplate, mWSheetDest, tempRange, 1);
        //    }
        //    //try
        //    //{
        //    Microsoft.Office.Interop.Excel.Range range = mWSheetDest.UsedRange;
        //    int currColOffset = 0;
        //    int currRowOffset = 0;
        //    int listidx = 0;
        //    foreach (Dictionary<ExcelCellPos, string> cellDataDict in toExcelList)
        //    {
        //        foreach (var paramItem in cellDataDict)
        //        {
        //            mWSheetDest.Cells[paramItem.Key.colPos + rowOffset * currRowOffset, paramItem.Key.rowPos + colOffset * currColOffset] = paramItem.Value;
        //        }
        //        listidx++;
        //        if (listidx >= 127)
        //        {
        //            Console.WriteLine(listidx);
        //        }
        //        if (rowRepeat != 0) //每一個 Row 印幾張， 例如， 薪資單， 一張 A4 印 2X2 張， 故 rowReapeat = 2
        //        {
        //            currColOffset = listidx % rowRepeat;
        //            currRowOffset = listidx / rowRepeat;
        //        }
        //        else
        //        {
        //            currRowOffset = listidx;
        //        }

        //        if (currColOffset == 0 && maxPages > currRowOffset)
        //        {
        //            insertExcelRow(mWSheetTemplate, mWSheetDest, tempRange, (currRowOffset) * tempRangeRows);
        //            currPageRows += tempRangeRows;
        //        }
        //        if (pageBreak != 0 && currPageRows % pageBreak == 0)
        //        {
        //            mWSheetDest.HPageBreaks.Add(mWSheetDest.Range["A" + (pageBreak * currRowOffset + 1).ToString()]);
        //        }
        //    }
        //    if (ifPrint)
        //    {
        //        this.printExcel(mWSheetDest);
        //    }
        //    rtnBool = true;
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    MessageBox.Show(ex.Message, "outputToExcel()");
        //    //}
        //    return rtnBool;
        //}

        //from 薪資單
        //pageBreak : 每幾個 Label Row 要跳頁, 例如 SS Label : 8
        //tempRange : "A1:G13"
        //每一張寬幅 cells, 例如 7
        //第一張 Row 數,  rowOffset: 13
        //rowRepeat : 每一個 Row 印幾張
        //skipKey : 當這個 key 的資料為空白, 則會跳掉這一格, 完全不印
        public int outputToExcel2(string fileExcel, string workSheet, List<Dictionary<ExcelCellPos, String>> toExcelList, string saveAsPath, string templateSheet,
                string tempRange, int pageBreak = 0, bool ifPrint = false, bool ifClose = true, int colOffset = 0, int rowOffset = 0, int rowRepeat = 0)
        {
            int rtnLineCnt = 0;
            bool skipLabel = false;
            int tempRangeRows = this.getRangeRows(tempRange); //tenplate 涵蓋範圍的 ROW number
            int currPageRows = 0;
            Microsoft.Office.Interop.Excel.Workbook mWorkBook;
            Microsoft.Office.Interop.Excel.Sheets mWorkSheets;
            Microsoft.Office.Interop.Excel.Worksheet mWSheet1;          //Target Worksheet
            Microsoft.Office.Interop.Excel.Worksheet mWSheet2 = null;   //範本
            Microsoft.Office.Interop.Excel.Application oXL;
            object misvalue = System.Reflection.Missing.Value;
            string path = fileExcel;
            oXL = new Microsoft.Office.Interop.Excel.Application();
            oXL.Visible = false;
            oXL.DisplayAlerts = false;
            mWorkBook = oXL.Workbooks.Open(path, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            //Get all the sheets in the workbook
            mWorkSheets = mWorkBook.Worksheets;
            //Get the allready exists sheet
            mWSheet1 = (Microsoft.Office.Interop.Excel.Worksheet)mWorkSheets.get_Item(workSheet);       //Target worksheet
            if (templateSheet != null && templateSheet != "")
            {
                mWSheet2 = (Microsoft.Office.Interop.Excel.Worksheet)mWorkSheets.get_Item(templateSheet);   //範本
                ClearExcelWorkSheet(mWSheet1);
                insertExcelRow(mWSheet2, mWSheet1, tempRange, 0);
                currPageRows += tempRangeRows;
            }

            Microsoft.Office.Interop.Excel.Range range = mWSheet1.UsedRange;
            int currColOffset = 0;
            int currRowOffset = 0;
            int listidx = 0;
            foreach (Dictionary<ExcelCellPos, string> cellDataDict in toExcelList)
            {
                if (cellDataDict == null)
                {
                    skipLabel = true;
                }
                else
                {
                    foreach (var paramItem in cellDataDict)
                    {
                        //mWSheet2.Cells[paramItem.Key.colPos + rowOffset * currRowOffset, paramItem.Key.rowPos + colOffset * currColOffset] = paramItem.Value;
                        mWSheet2.Cells[paramItem.Key.colPos, paramItem.Key.rowPos] = paramItem.Value;
                    }
                    skipLabel = false;
                }

                if (rowRepeat > 1) //每一個 Row 印幾張， 例如， 薪資單， 一張 A4 印 2X2 張， 故 rowReapeat = 2
                {
                    currColOffset = (listidx) % rowRepeat;
                    currRowOffset = listidx / rowRepeat;
                }
                //int colOffset = 0, int rowOffset = 0
                listidx++;

                if (currColOffset == 0)
                {
                    if (!skipLabel)
                        copyPasteRange(mWSheet2, mWSheet1, tempRange, "A" + (currRowOffset * rowOffset + 1), true);
                    currPageRows += tempRangeRows;
                }
                else
                {
                    if (!skipLabel)
                        copyPasteRange(mWSheet2, mWSheet1, tempRange, cellIdxToStr((currRowOffset * rowOffset + 1), colOffset + 1), true);
                }

                if (listidx % pageBreak == 0)
                {
                    mWSheet1.HPageBreaks.Add(mWSheet1.Range["A" + ((listidx / rowRepeat) * rowOffset).ToString()]);
                }
                rtnLineCnt++;
            }
            delAllExcelRowAfter(mWSheet1, currPageRows - rowOffset);
            if (ifPrint)
            {
                this.printExcel(mWSheet1);
            }

            //如果 Save 到 copyVersion 則存到 母版檔案
            if (!string.IsNullOrEmpty(saveAsPath))
                mWorkBook.SaveAs(saveAsPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook, Missing.Value,
                Missing.Value, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true,
                Missing.Value, Missing.Value, Missing.Value);
            else
                mWorkBook.Save();
            Thread.Sleep(2000);
            if (ifClose)
            {
                mWorkBook.Close();
                mWSheet1 = null;
                mWorkBook = null;
                oXL.Quit();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            return rtnLineCnt;
        }


        //pageBreak : 每 Copy/Paste 幾次, 加一個 跳頁符號
        //excelPath--barcode 檔案位置, 如果為 null, 則以 saveAsPath 的目錄為路徑
        //rowOffset 每個 Label 佔幾個 Row
        //colOffset 每個 Label 佔幾個 Col
        //rowRepeat : 每個 Row 印幾個 Label
        //qrSize : 20pixel 的 N 倍
        public int outputToExcel3(Microsoft.Office.Interop.Excel.Worksheet templateWS, Microsoft.Office.Interop.Excel.Worksheet targetWS,
               Microsoft.Office.Interop.Excel.Worksheet cleanFormWS,
               List<Dictionary<string, String>> toExcelList, string tempRange, int pageBreak = 1,
               bool ifPrint = false, bool ifClose = true, int colOffset = 0, int rowOffset = 0, int rowRepeat = 1, string excelPath = null,
               float qrSize = 1, int qrTopMargin = 5 ,string targetPrinter=null, ExcelElements ee=null,float newqrSize = 30 , int qrLeftMargin = 5)
        {
            int rtnLineCnt = 0;
            bool skipLabel = false; //skipLabel : 保留空白不列印
            int tempRangeRows = this.getRangeRows(tempRange); //tenplate 涵蓋範圍的 ROW number
            string barFileResult = null;
            int currPageRows = 0;
            rowRepeat = rowRepeat == 0 ? 1 : rowRepeat;
            //Microsoft.Office.Interop.Excel.Worksheet mWSheet1;          //Target Worksheet
            //Microsoft.Office.Interop.Excel.Worksheet mWSheet2 = null;   //範本
            if (string.IsNullOrEmpty(excelPath))
                excelPath = CommonClass.getCurrDir();

            string tempBarCodeFile;
            if (CommonClass.Right(excelPath, 1) != @"\")
                tempBarCodeFile = excelPath + @"\" + "tempBarcode";
            else
                tempBarCodeFile = excelPath + "tempBarcode";

            Microsoft.Office.Interop.Excel.Range range = targetWS.UsedRange;
            int currColOffset = 0; //標籤寬
            int currRowOffset = 0; //標籤長
            int listidx = 0;
            foreach (Dictionary<string, string> cellDataDict in toExcelList)
            {
                if (cellDataDict == null)
                {
                    skipLabel = true; //skipLabel : 保留空白不列印
                }
                else
                {
                    foreach (var paramItem in cellDataDict)
                    {
                        //QRCode:Content,size, 其中 size 為標準 size 的倍數
                        //例如 BARCODE: 1234567,1 ==> 內容,n倍
                        if (paramItem.Value.Length > 6 && paramItem.Value.Substring(0, 6).ToUpper() == "QRCODE")
                        {
                            String barCodeInfo = paramItem.Value.Substring(7);
                            //tempBarCodeFile += ".bmp";
                            //QrCode 的 Size 為 50*size pixel
                            barFileResult = BarCodeLib.generateBARCode("QR", barCodeInfo, tempBarCodeFile, qrSize);
                            int[] imgSize = CommonClass.getImgSize(barFileResult);
                            insertPicToCell(templateWS, paramItem.Key, barFileResult, (int)(imgSize[0]), (int)(imgSize[1]), qrTopMargin);
                            Thread.Sleep(0);
                        }
                        else if (paramItem.Value.Length > 9 && paramItem.Value.Substring(0, 9).ToUpper() == "NEWQRCODE")
                        {
                            String barCodeInfo = paramItem.Value.Substring(10);
                            barFileResult = BarCodeLib.generateBARCode("QR", barCodeInfo, tempBarCodeFile, qrSize);
                            insertPicToCell(templateWS, paramItem.Key, barFileResult, newqrSize, newqrSize, topMarging: qrTopMargin, leftMargin: qrLeftMargin);
                            Thread.Sleep(0);
                        }
                        ////barCode:Content,Width,Heigh
                        else if (paramItem.Value.Length > 7 && paramItem.Value.Substring(0, 7).ToUpper() == "BARCODE")
                        {
                            string barCodeInfo = paramItem.Value.Substring(8);
                            barFileResult = BarCodeLib.generateBARCode("BAR", barCodeInfo, tempBarCodeFile, qrSize);
                            int[] imgSize = CommonClass.getImgSize(barFileResult);
                            insertPicToCell(templateWS, paramItem.Key, barFileResult, (int)(imgSize[0]), (int)(imgSize[1]), qrTopMargin);
                            Thread.Sleep(0);
                        }
                        else
                            fillExcelCellValue(templateWS, paramItem.Key, "'" + paramItem.Value);
                    }
                    skipLabel = false;
                }

                if (rowRepeat > 1) //每一個 Row 印幾張， 例如， 薪資單， 一張 A4 印 2X2 張， 故 rowReapeat = 2
                {
                    currColOffset = (listidx) % rowRepeat;
                }
                currRowOffset = listidx / rowRepeat;
                listidx++;

                if (currColOffset == 0)
                {   ////每個Row 的第一個以上
                    if (!skipLabel) //skipLabel : 保留空白不列印
                    {
                        copyPasteRange(templateWS, targetWS, tempRange, "A" + (currRowOffset * rowOffset + 1), false);
                        copyPasteRange(cleanFormWS, templateWS, tempRange, "A1", true);
                        //Copy Cell Height
                        copyPasteSpecial(cleanFormWS, targetWS, "1:" + rowOffset, (currRowOffset * rowOffset + 1) + ":" + ((currRowOffset + 1) * rowOffset), XlPasteType.xlPasteFormats);
                    }
                    currPageRows += tempRangeRows;
                }
                else //每個Row 第二個以上
                {
                    if (!skipLabel) //skipLabel : 保留空白不列印
                    {
                        copyPasteRange(templateWS, targetWS, tempRange, cellIdxToStr((currRowOffset * rowOffset + 1), colOffset + 1), false);
                        copyPasteRange(cleanFormWS, templateWS, tempRange, "A1", true);
                    }
                }

                if (listidx % pageBreak == 0)
                {
                    targetWS.HPageBreaks.Add(targetWS.Range["A" + ((listidx / rowRepeat) * rowOffset + 1).ToString()]);
                }
                rtnLineCnt++;
                System.Threading.Thread.Sleep(0);
                if (!keepPrint)
                {
                    MessageBox.Show("使用者中斷列印", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
            targetWS.get_Range("A1", "Z99").NumberFormatLocal = "@";
            delAllExcelRowAfter(targetWS, currPageRows);
            CommonClass.deleteFiles(excelPath, DateTime.Now.AddDays(1), "bmp");
            if (ifPrint)
            {
                if (string.IsNullOrEmpty(targetPrinter))
                    this.printExcel(targetWS);
                else
                    printExcelBySpecifyPrinter(ee, targetPrinter);

            }
            keepPrint = true;
            return rtnLineCnt;
        }


        //excelPath--barcode 檔案位置, 如果為 null, 則以 saveAsPath 的目錄為路徑
        //pageSize header有幾列
        //endofdatarows endofdata 放在哪一列
        public int outputToExcel4(Microsoft.Office.Interop.Excel.Worksheet templateWS, Microsoft.Office.Interop.Excel.Worksheet targetWS,
               /*Microsoft.Office.Interop.Excel.Worksheet cleanFormWS,*/
               List<Dictionary<string, String>> toExcelList, int endofdatarows = 0,
               bool ifPrint = false, bool ifClose = true, string excelPath = null,
               float qrSize = 1, int qrTopMargin = 5)
        {
            bool skipLabel = true;
            int rtnLineCnt = 0;
            string barFileResult = null;
            if (string.IsNullOrEmpty(excelPath))

                excelPath = CommonClass.getCurrDir();

            string tempBarCodeFile;
            if (CommonClass.Right(excelPath, 1) != @"\")
                tempBarCodeFile = excelPath + @"\" + "tempBarcode";
            else
                tempBarCodeFile = excelPath + "tempBarcode";

            Microsoft.Office.Interop.Excel.Range range = targetWS.UsedRange;

            foreach (Dictionary<string, string> cellDataDict in toExcelList)
            {
                if (cellDataDict == null)
                {
                    skipLabel = true; //skipLabel : 保留空白不列印
                }
                else
                {
                    foreach (var paramItem in cellDataDict)
                    {
                        //QRCode:Content,size, 其中 size 為標準 size 的倍數
                        //例如 BARCODE: 1234567,1 ==> 內容,n倍
                        if (paramItem.Value.Length > 6 && paramItem.Value.Substring(0, 6).ToUpper() == "QRCODE")
                        {
                            string[] barCodeInfo = paramItem.Value.Substring(7).Split(',');
                            float size = 1;
                            if (barCodeInfo.Length == 2)
                            {
                                size = float.Parse(barCodeInfo[1]);
                            }
                            else
                            {
                                size = qrSize;
                            }
                            //tempBarCodeFile += ".bmp";
                            //QrCode 的 Size 為 50*size pixel
                            barFileResult = BarCodeLib.generateBARCode("QR", barCodeInfo[0], tempBarCodeFile, size);
                            int[] imgSize = CommonClass.getImgSize(barFileResult);
                            insertPicToCell(templateWS, paramItem.Key, barFileResult, (int)(imgSize[0]), (int)(imgSize[1]), qrTopMargin);
                            Thread.Sleep(0);
                        }
                        ////barCode:Content,Width,Heigh
                        else if (paramItem.Value.Length > 7 && paramItem.Value.Substring(0, 7).ToUpper() == "BARCODE")
                        {

                            string barCodeInfo = paramItem.Value.Substring(8);
                            barFileResult = BarCodeLib.generateBARCode("BAR", barCodeInfo, tempBarCodeFile);
                            int[] imgSize = CommonClass.getImgSize(barFileResult);
                            insertPicToCell(templateWS, paramItem.Key, barFileResult, (int)(imgSize[0] * qrSize), (int)(imgSize[1] * qrSize), qrTopMargin);
                            Thread.Sleep(0);
                        }
                        else if (paramItem.Value.Length > 9 && paramItem.Value.Substring(0, 9).ToUpper() == "ENDOFDATA")
                        {

                            string endofDataInfo = paramItem.Value.Substring(10);
                            string excelcell = paramItem.Key;
                            //取得值放在EXCEL哪一個欄位 例:A1 的 A  By Ronny 20210318
                            excelcell = excelcell.Substring(0, 1);
                            excelcell = excelcell + endofdatarows.ToString();
                            fillExcelCellValue(templateWS, excelcell, endofDataInfo);

                        }
                        else if (paramItem.Value.Length > 10 && paramItem.Value.Substring(0, 10).ToUpper() == "REPORTDATE")
                        {
                            string reportdate = paramItem.Value.Substring(11);
                            string excelcell = paramItem.Key;
                            fillExcelCellValue(templateWS, excelcell, reportdate);
                        }
                        else
                            fillExcelCellValue(templateWS, paramItem.Key, paramItem.Value);
                    }
                    skipLabel = false;
                }
            }
            //delAllExcelRowAfter(targetWS, currPageRows);
            CommonClass.deleteFiles(CommonClass.getFileInfo(excelPath, "directory"), DateTime.Now.AddDays(1), "bmp");
            if (ifPrint)
            {
                this.printExcel(targetWS);
            }
            keepPrint = true;
            return rtnLineCnt;
        }
        


        //刪除Excel一列
        public void delExcelRow(string FileName, int ExcelRow)
        {
            Microsoft.Office.Interop.Excel.Application xlexcel;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlexcel = new Microsoft.Office.Interop.Excel.Application();
            xlexcel.Visible = false;
            xlWorkBook = xlexcel.Workbooks.Add(misValue);
            xlWorkBook = xlexcel.Application.Workbooks.Open(FileName);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet.Rows[ExcelRow].Delete();
            xlWorkBook.Save();
            xlWorkBook.Close();
            Marshal.FinalReleaseComObject(xlWorkBook);
            xlexcel.Quit();
            Marshal.FinalReleaseComObject(xlexcel);
        }

        public string getDestRange(string sourceRange, int startRow)
        {

            string[] ranges = sourceRange.Split(':');
            if (ranges.Length != 2)
                return null;
            int Tempint = 0;
            bool ifOnlyRows = int.TryParse(ranges[0], out Tempint);
            string newX1;
            string newX2;
            if (ifOnlyRows)
            {
                newX1 = (int.Parse(ranges[0]) + startRow).ToString();
                newX2 = (int.Parse(ranges[1]) + startRow).ToString();
            }
            else
            {
                newX1 = (int.Parse(ranges[0].Substring(1, ranges[0].Length - 1)) + startRow).ToString();
                newX2 = (int.Parse(ranges[1].Substring(1, ranges[1].Length - 1)) + startRow).ToString();
            }

            if (ifOnlyRows)
                return newX1 + ":" + newX2;
            else
                return ranges[0].Substring(0, 1) + newX1 + ":" + ranges[1].Substring(0, 1) + newX2;
        }

        //傳進來的 Range 包含幾個 Rows
        public int getRangeRows(string sourceRange)
        {
            string[] ranges = sourceRange.Split(':');
            if (ranges.Length != 2)
                return 0;
            int tempInt = 0;
            bool ifOnlyRows = int.TryParse(ranges[0], out tempInt);
            int newX1 = 0;
            int newX2 = 0;
            if (ifOnlyRows)
            {
                newX1 = int.Parse(ranges[0]);
                newX2 = int.Parse(ranges[1]);
            }
            else
            {
                newX1 = int.Parse(ranges[0].Substring(1));
                newX2 = int.Parse(ranges[1].Substring(1));
            }

            return newX2 - newX1 + 1;
        }

        #region copyPaste
        public bool copyPasteWorksheet(ref Microsoft.Office.Interop.Excel.Worksheet sourceSheet, ref Microsoft.Office.Interop.Excel.Worksheet destSheet, int pasteX = 1, int pasteY = 1)
        {
            Microsoft.Office.Interop.Excel.Range xlRange = sourceSheet.UsedRange;
            xlRange.Copy(Type.Missing);
            Microsoft.Office.Interop.Excel.Range targetRange = destSheet.Cells[pasteX, pasteY];   //initial cell for Paste
            destSheet.Paste(targetRange);
            return true;
        }

        public bool copyPasteWorksheet(Microsoft.Office.Interop.Excel.Worksheet sourceSheet, Microsoft.Office.Interop.Excel.Workbook destWorkbook, string destWorksheetName)
        {
            try
            {
                var newWorkbook = sourceSheet.Application.Workbooks.Add(System.Reflection.Missing.Value);
                var newSheet = (Microsoft.Office.Interop.Excel.Worksheet)newWorkbook.Worksheets.Add();
                newSheet.Name = destWorksheetName;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        //cut/paste, copy/paste Row
        public void copyPasteRow(Microsoft.Office.Interop.Excel._Worksheet salWorkSheet, int sourceExcelRow, int targetExcelRow, bool ifCopy = true)
        {
            Microsoft.Office.Interop.Excel.Range copyRange = salWorkSheet.Range[sourceExcelRow + ":" + sourceExcelRow];
            Microsoft.Office.Interop.Excel.Range insertRange = salWorkSheet.Range[targetExcelRow + ":" + targetExcelRow];
            if (ifCopy)
                insertRange.Insert(Microsoft.Office.Interop.Excel.XlInsertShiftDirection.xlShiftToRight, copyRange.Copy());
            else
                insertRange.Insert(Microsoft.Office.Interop.Excel.XlInsertShiftDirection.xlShiftToRight, copyRange.Cut());
        }

        //public void copyPasteRow(Microsoft.Office.Interop.Excel._Worksheet salWorkSheet, int sourceExcelRow, int targetExcelRow, bool ifCopy = true)
        //{
        //    Microsoft.Office.Interop.Excel.Range copyRange = salWorkSheet.Range[sourceExcelRow + ":" + sourceExcelRow];
        //    Microsoft.Office.Interop.Excel.Range insertRange = salWorkSheet.Range[targetExcelRow + ":" + targetExcelRow];
        //    if(ifCopy)
        //        insertRange.Insert(Microsoft.Office.Interop.Excel.XlInsertShiftDirection.xlShiftToRight, copyRange.Copy());
        //    else
        //        insertRange.Insert(Microsoft.Office.Interop.Excel.XlInsertShiftDirection.xlShiftToRight, copyRange.Cut());
        //}

        //ifCopy:true--Copy, false--move
        public void copyPasteRange(Microsoft.Office.Interop.Excel.Worksheet sourceWorkSheet,
            Microsoft.Office.Interop.Excel.Worksheet targetWorkSheet, string sourceStartCell,
            string sourceEndCell, string targetStartCell, bool ifCopy = true)
        {
            Microsoft.Office.Interop.Excel.Range copyRange = sourceWorkSheet.Range[sourceStartCell + ":" + sourceEndCell];
            //Excel.Range insertRange = targetWorkSheet.Range[targetStartCell + ":" + targetEndCell];
            Microsoft.Office.Interop.Excel.Range insertRange = targetWorkSheet.Range[targetStartCell];
            if (ifCopy)
                insertRange.Insert(Microsoft.Office.Interop.Excel.XlInsertShiftDirection.xlShiftToRight, copyRange.Copy());
            else
                insertRange.Insert(Microsoft.Office.Interop.Excel.XlInsertShiftDirection.xlShiftToRight, copyRange.Cut());
        }

        //ifCopy:true--Copy, false--move
        public void copyPasteSpecial(Microsoft.Office.Interop.Excel.Worksheet sourceWorkSheet,
            Microsoft.Office.Interop.Excel.Worksheet targetWorkSheet, string sourceRange,
            string targetStartCell, XlPasteType pasteType = XlPasteType.xlPasteAll)
        {

            Microsoft.Office.Interop.Excel.Range copyRange;
            Microsoft.Office.Interop.Excel.Range pasteRange;
            if (string.IsNullOrEmpty(sourceRange)) //沒有 Range 就是 worksheet 全部
                //copyRange = sourceWorkSheet.get_Range("a1").EntireRow.EntireColumn.Select();
                copyRange = sourceWorkSheet.Cells;
            else
                copyRange = sourceWorkSheet.Range[sourceRange];
            //Excel.Range insertRange = targetWorkSheet.Range[targetStartCell + ":" + targetEndCell];
            if (string.IsNullOrEmpty(sourceRange)) //沒有 Range 就是 worksheet 全部
                pasteRange = targetWorkSheet.Cells;
            else
                pasteRange = targetWorkSheet.Range[targetStartCell];
            copyRange.Copy(Missing.Value);
            // Paste special (with Addition) the value over cells A2 to J11
            pasteRange.PasteSpecial(pasteType);
        }

        //ifCopy:true--Copy, false--move
        public void copyPasteRange(Microsoft.Office.Interop.Excel.Worksheet sourceWorkSheet,
            Microsoft.Office.Interop.Excel.Worksheet targetWorkSheet, string sourceRange,
            string targetStartCell, bool ifCopy = true)
        {
            if (ifCopy)
                ClearExcelWorkSheet(targetWorkSheet);
            Microsoft.Office.Interop.Excel.Range copyRange = sourceWorkSheet.Range[sourceRange];
            //Excel.Range insertRange = targetWorkSheet.Range[targetStartCell + ":" + targetEndCell];
            Microsoft.Office.Interop.Excel.Range insertRange = targetWorkSheet.Range[targetStartCell];

            insertRange.Insert(Microsoft.Office.Interop.Excel.XlInsertShiftDirection.xlShiftDown, copyRange.Copy());
            //insertRange.Insert(Microsoft.Office.Interop.Excel.XlInsertShiftDirection.xlShiftDown, copyRange.Cut());
        }

        #endregion //Copy Paste

        //從一個Sheet的 Range Copy 到另一個Sheet的Range
        //sourceRange 例如 "A1:G24"
        public void insertExcelRow(Microsoft.Office.Interop.Excel._Worksheet sourceWorkSheet, Microsoft.Office.Interop.Excel._Worksheet destWorkSheet,
            string sourceRange, int startRow)
        {
            sourceWorkSheet.Range[sourceRange].Copy();
            string destReang = getDestRange(sourceRange, startRow);
            destWorkSheet.Range[destReang].Insert(Shift: (Microsoft.Office.Interop.Excel.XlDirection.xlDown));
        }

        public void insertEmptyRow(Microsoft.Office.Interop.Excel._Worksheet sourceWorkSheet, string InsertRow = "1", int insertRows = 1)
        {
            for (int i = 0; i < insertRows; i++)
                sourceWorkSheet.Rows[InsertRow].Insert();
        }


        public void insertExcelRow(Microsoft.Office.Interop.Excel._Worksheet salWorkSheet, int sourceExcelRow, int targetExcelRow)
        {
            salWorkSheet.Rows[sourceExcelRow].Copy();
            salWorkSheet.Rows[targetExcelRow].Insert(Shift: (Microsoft.Office.Interop.Excel.XlDirection.xlDown));
        }

        public void insertExcelRow(string FileName, string xlSheetName, int sourceExcelRow, int targetExcelRow)
        {
            Microsoft.Office.Interop.Excel.Application xlexcel;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            //object misValue = System.Reflection.Missing.Value;
            xlexcel = new Microsoft.Office.Interop.Excel.Application();

            //xlWorkBook = xlexcel.Workbooks.Add(misValue);
            xlWorkBook = xlexcel.Application.Workbooks.Open(FileName);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets[xlSheetName];
            insertExcelRow(xlWorkSheet, sourceExcelRow, targetExcelRow);
            //xlexcel.CutCopyMode = (Microsoft.Office.Interop.Excel.XlCutCopyMode)0;
            //xlWorkSheet.get_Range("A6", "A6").Select();
            xlWorkBook.Save();
            xlWorkBook.Close();
            Marshal.FinalReleaseComObject(xlWorkBook);
            xlexcel.Quit();
            Marshal.FinalReleaseComObject(xlexcel);
        }


        //取得 Ecxel 表中資料的最後一筆的 RowIndex
        public int getLastEmptyRowIdx(Microsoft.Office.Interop.Excel._Worksheet salWorkSheet, string columnNum)
        {
            int rtnInt = 0;
            int numRows = salWorkSheet.UsedRange.Rows.Count;
            for (rtnInt = 1; rtnInt <= numRows; rtnInt++)
            {
                if (salWorkSheet.get_Range(columnNum + rtnInt).Value2 == null)
                    continue;
                if (salWorkSheet.get_Range(columnNum + rtnInt).Value2.GetType() == typeof(int))
                    break;
                if (salWorkSheet.get_Range(columnNum + rtnInt).Value2 == "主管")
                    break;

            }
            return rtnInt - 2;
        }

        public int getLastEmptyRowIdx(string FileName, string workSheetName, string columnNum)
        {
            int rtnInt = 0;
            Microsoft.Office.Interop.Excel.Application xlexcel;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlexcel = new Microsoft.Office.Interop.Excel.Application();
            xlWorkBook = xlexcel.Workbooks.Add(misValue);
            xlWorkBook = xlexcel.Application.Workbooks.Open(FileName);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(workSheetName);
            int numRows = xlWorkSheet.UsedRange.Rows.Count;
            for (rtnInt = 1; rtnInt <= numRows; rtnInt++)
            {
                if (xlWorkSheet.get_Range(columnNum + rtnInt).Value2 == null)
                    continue;
                if (xlWorkSheet.get_Range(columnNum + rtnInt).Value2.GetType() == typeof(int))
                    break;
                if (xlWorkSheet.get_Range(columnNum + rtnInt).Value2 == "主管")
                    break;

            }
            xlWorkBook.Close();
            Marshal.FinalReleaseComObject(xlWorkBook);

            xlexcel.Quit();
            Marshal.FinalReleaseComObject(xlexcel);
            return rtnInt - 2;
        }

        //查詢Excel單格的值
        public string excelCellValue(string FileName, string ExcelCell)
        {
            Microsoft.Office.Interop.Excel.Application xlexcel;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlexcel = new Microsoft.Office.Interop.Excel.Application();

            xlWorkBook = xlexcel.Workbooks.Add(misValue);
            xlWorkBook = xlexcel.Application.Workbooks.Open(FileName);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            string CellValue = xlWorkSheet.get_Range(ExcelCell, ExcelCell).Value2;
            xlWorkBook.Close();
            Marshal.FinalReleaseComObject(xlWorkBook);

            xlexcel.Quit();
            Marshal.FinalReleaseComObject(xlexcel);

            return CellValue;
        }

        public void KillSpecificExcelFileProcess(string excelFileName)
        {
            var processes = from p in Process.GetProcessesByName("EXCEL.EXE")
                            select p;

            foreach (var process in processes)
            {
                if (process.MainWindowTitle == "Microsoft Excel - " + excelFileName)
                    process.Kill();
            }
        }

        private void KillProcess(string processName)
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

        public void ClearExcelWorkSheet(Microsoft.Office.Interop.Excel.Worksheet workSheet)
        {
            workSheet.Cells.ClearContents();
        }

        public void ClearExcelWorkSheet(string excelFile, string workSheet)
        {
            var excel = new Microsoft.Office.Interop.Excel.Application();
            var workbook = excel.Workbooks.Open(excelFile);

            try
            {
                workbook.Worksheets[workSheet].Cells.ClearContents();
                workbook.Save();
            }
            finally
            {
                workbook.Close();
                Marshal.FinalReleaseComObject(workbook);
                excel.Quit();
                Marshal.FinalReleaseComObject(excel);
            }
        }
        

        public void writeDataToExcelSheet(string FileFullPath, string SheetName, System.Data.DataTable Data)
        {
            string strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + FileFullPath + ";Extended Properties='Excel 12.0; HDR=YES; IMEX=0'"; //此連接可以操作.xls與.xlsx文件
            string strSQLSeconPart = "";
            int SQLCountTotal = 0;
            string strSQLFirstPart = "INSERT INTO [" + SheetName + "$] (" + String.Join(",", Data.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray()) + ") VALUES (";
            string StrSQL;
            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                conn.Open();
                for (int i = 0; i < Data.Rows.Count; i++)
                {

                    for (int j = 0; j < Data.Columns.Count; j++)
                    {
                        strSQLSeconPart += "'" + Data.Rows[i][j] + "', ";
                    }

                    StrSQL = strSQLFirstPart + strSQLSeconPart.Substring(0, strSQLSeconPart.Length - 2) + ")";
                    OleDbCommand Cmd = new OleDbCommand(StrSQL, conn);
                    int SQLCount = Cmd.ExecuteNonQuery();
                    strSQLSeconPart = "";
                    SQLCountTotal += SQLCount;
                }
                if (SQLCountTotal != Data.Rows.Count)
                    MessageBox.Show("匯出資料中 " + (Data.Rows.Count - (SQLCountTotal)) + " 筆資料失敗");
            }
        }


        public DataRow searchDataTable(System.Data.DataTable dt, string searchColumn, string searchTarget)
        {
            foreach (DataRow myRow in dt.Rows)
            {
                if (myRow[searchColumn].ToString() == searchTarget)
                    return myRow;
            }
            return null;
        }

        public DataRow deleteDataTable(System.Data.DataTable dt, string searchColumn, string searchTarget)
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

        //從 startRow 取 numColumn 的所有的值， 放入 LIST 中
        public List<string> getExcelColumnData(String filePath, String sheetName, string numColumn, int startRow = 1)
        {
            Microsoft.Office.Interop.Excel.Application xl = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workbook = xl.Workbooks.Open(filePath);
            Microsoft.Office.Interop.Excel.Worksheet sheet = workbook.Sheets[sheetName];


            int numRows = sheet.UsedRange.Rows.Count;
            //int numColumns = 2;     // according to your sample

            List<string> record = new List<string>();
            for (int rowIndex = startRow; rowIndex <= numRows; rowIndex++)  // assuming the data starts at 1,1
            {
                Range cell = (Range)sheet.Cells[rowIndex, numColumn];
                if (cell.Value != null)
                {
                    record.Add(Convert.ToString(cell.Value));
                }
            }

            workbook.Close();
            Marshal.FinalReleaseComObject(workbook);

            xl.Quit();
            Marshal.FinalReleaseComObject(xl);
            return record;
        }

        public System.Data.DataTable GetDataTableFromCsv(string path, bool isFirstRowHeader)
        {
            string header = isFirstRowHeader ? "Yes" : "No";

            string pathOnly = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            string sql = @"SELECT * FROM [" + fileName + "]";

            using (OleDbConnection connection = new OleDbConnection(
                      @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly +
                      ";Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand command = new OleDbCommand(sql, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                System.Data.DataTable dataTable = new System.Data.DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        //用於 Header 格式不整齊的 WorkSheet
        //headerRow : Title Row 的行數
        //dataRowStart : 1 代表第 1 Row 開始為 Data 
        //bool useCellData : true:用 CellData 作為欄位名稱, false:用 Excel A,B,C,D... 取得 Column Name
        //例如 System.Data.DataTable dt = excelToDatatable(pathToExcel, "工作表1");
        public System.Data.DataTable excelToDatatable(string fileFullPath, string SheetName = null, int headerRow = 0, int dataRowStart = 1, bool ifRemoveCols = true, bool ifRemoveRows = false)
        {
            //string strConn2 = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + fileFullPath + ";Extended Properties='Excel 12.0; HDR=YES; IMEX=0'"; //選取薪資表Sheet的B欄姓名欄所有資料
            string strConn2 = String.Format(@"
                Provider=Microsoft.ACE.OLEDB.12.0;
                Data Source={0};
                Extended Properties=""Excel 12.0 Xml;HDR=YES""
                ", fileFullPath);
            System.Data.DataTable dt = new System.Data.DataTable();
            using (OleDbConnection conn = new OleDbConnection(strConn2))
            {
                conn.Open();
                if (SheetName == null)
                {
                    System.Data.DataTable dtSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    SheetName = dtSchema.Rows[0].Field<string>("TABLE_NAME");
                }
                string StrSQLAllNames = "SELECT * FROM [" + SheetName + "]";
                //string temp = null;

                OleDbDataAdapter odda = new OleDbDataAdapter(string.Format(StrSQLAllNames, SheetName), conn);
                odda.Fill(dt);
            }
            if (headerRow > 0)
            {
                //changeDTColNameByDtData(ref System.Data.DataTable dt, int ColNameRow = -1, bool ifDelCol = false, bool ifDelRow=false)
                CommonClass.changeDTColNameByDtData(ref dt, headerRow, ifRemoveCols, ifRemoveRows);
            }
            return dt;
        }

        public System.Data.DataTable excelToDatatable(Microsoft.Office.Interop.Excel.Worksheet mWSheet)
        {
            var dt = new System.Data.DataTable(mWSheet.Name);
            dt.Columns.AddRange(GetDataColumns(mWSheet).ToArray());
            var headerOffset = 1; //have to skip header row
            var width = dt.Columns.Count;
            var depth = GetTableDepth(mWSheet, headerOffset);
            for (var i = 1; i <= depth; i++)
            {
                var row = dt.NewRow();
                for (var j = 1; j <= width; j++)
                {
                    var currentValue = mWSheet.Cells[i + headerOffset, j].Value;

                    //have to decrement b/c excel is 1 based and datatable is 0 based.
                    row[j - 1] = currentValue == null ? null : currentValue.ToString();
                }

                dt.Rows.Add(row);
            }

            return dt;
        }

        //titleIdx: csv Title 的位置， 如果 titleIdx=0， 代表此檔案只有 Data, 沒有 Title, datatable 的 Column name 用 col1, col2.. 代替
        //rowIdxDetector: 用來判斷所有的資料都已經讀完
        public System.Data.DataTable new_excelToDatatable(string excelFile, string sheetName, 
             int titleIdx = 1, int rowIdxDetector = 1)
        {
            ExcelElements excelEE = openExcel(excelFile);
            excelEE.mWSheet = getWorksheet(excelEE, sheetName);
            string tempStr;
            Dictionary<string, int> excelSize = getExcelSize(excelEE.mWSheet);
            // Copy the data from the worksheet into a DataTable
            System.Data.DataTable dt = new System.Data.DataTable();
            int colCnt = excelSize["Col"];
            if (titleIdx==0)  //dt header
            {
                for (int i = 1; i <= excelSize["Col"]; i++)
                    dt.Columns.Add($"col{i}");
            } else
            {
                for (int i = 1; i <= excelSize["Col"]; i++)
                {
                    if (excelEE.mWSheet.Cells[1, i].Value == null)
                    {
                        break;
                    }
                    else
                        tempStr = excelEE.mWSheet.Cells[1, i].Value.ToString();
                    dt.Columns.Add(new DataColumn(tempStr));
                }
            }
            colCnt = dt.Columns.Count;
            for (int i = 2; i <= excelSize["Row"]; i++)
            {
                DataRow row = dt.NewRow();
                for (int j = 1; j <= colCnt; j++)
                {
                    if (excelEE.mWSheet.Cells[i, j].Value == null)
                        tempStr = "";
                    else
                        tempStr = excelEE.mWSheet.Cells[i, j].Value.ToString();
                    row[j - 1] = tempStr;
                }
                dt.Rows.Add(row);
            }

            closeExcel(excelEE, null, false);
            return dt;
        }

        //headerDict:資料的 Tital位置及名稱, 例如 {"B5":"年","B6":"月","B6":"日"}
        //dataRowStart:資料開始的行數， 從 0 開始算起
        //sheetName：工作表名稱
        //public string excelToJson(string pathToExcel, Dictionary<string, string> headerDict, int dataRowStart = 1, string sheetName = "工作表1")
        public string excelToJson(string pathToExcel, Dictionary<string, string> headerDict, int dataRowStart = 1, string sheetName = "工作表1")
        {
            //This connection string works if you have Office 2007+ installed and your 
            //data is saved in a .xlsx file
            var connectionString = String.Format(@"
                Provider=Microsoft.ACE.OLEDB.12.0;
                Data Source={0};
                Extended Properties=""Excel 12.0 Xml;HDR=YES""
                ", pathToExcel);

            //Creating and opening a data connection to the Excel sheet 
            using (var conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = String.Format(
                    @"SELECT * FROM [{0}$]",
                    sheetName
                    );

                using (var rdr = cmd.ExecuteReader())
                {
                    //LINQ query - when executed will create anonymous objects for each row
                    int rowNo = 0;
                    var query =
                        (from DbDataRecord row in rdr
                         select row).Select(x =>
                         {
                             Dictionary<string, object> item = new Dictionary<string, object>();
                                 for (int i = 0; i < headerDict.Keys.Count; i++)
                                 {

                                     if (headerDict.ElementAt(i).Value != "")
                                         item.Add(headerDict.ElementAt(i).Value, x[i]);
                                 }
                                 //item.Add(rdr.GetName(0), x[i]);
                                 return item;
                         });
                    //Generates JSON from the LINQ query
                    var json = JsonConvert.SerializeObject(query);
                    //List<Dictionary<string, string>> rtnDict =
                    //    JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
                    //for (int i = dataRowStart-1; i >= 0; i--)
                    //{
                    //    rtnDict.RemoveAt(i);
                    //}
                    //return JsonConvert.SerializeObject(rtnDict);
                    return json;
                }
            }
        }
        //headerList
        //skipData: Excel 產生 Dictionary 之後, 比對 skipData 的資料， 決定資料是否合法， 若不合法則刪除該 Dictionary
        public string excelToJson(string pathToExcel, List<string> headerList, Dictionary<string, string> skipData=null, string sheetName = "工作表1")
        {
            //This connection string works if you have Office 2007+ installed and your 
            //data is saved in a .xlsx file
            var connectionString = String.Format(@"
                Provider=Microsoft.ACE.OLEDB.12.0;
                Data Source={0};
                Extended Properties=""Excel 12.0 Xml;HDR=YES""
                ", pathToExcel);

            string connString = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + pathToExcel + ";Extended Properties='Excel 12.0; HDR=YES; IMEX=1'"; //此連接可以操作.xls與.xlsx文件

            //Creating and opening a data connection to the Excel sheet 
            using (var conn = new OleDbConnection(connString))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = String.Format(
                    @"SELECT * FROM [{0}$]",
                    sheetName
                    );

                using (var rdr = cmd.ExecuteReader())
                {
                    //LINQ query - when executed will create anonymous objects for each row
                    int rowNo = 0;
                    var query =
                        (from DbDataRecord row in rdr
                         select row).Select(x =>
                         {
                             Dictionary<string, object> item = new Dictionary<string, object>();
                             for (int i = 0; i < headerList.Count; i++)
                             {

                                 if (headerList[i] != "")
                                     item.Add(headerList[i], x[i]);
                             }
                             return item;
                         });
                    //Generates JSON from the LINQ query
                    var json = JsonConvert.SerializeObject(query);
                    List<Dictionary<string, string>> rtnDict =
                        JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
                    if (skipData != null)
                    {
                        for (int i = rtnDict.Count - 1; i >= 0; i--)
                        {
                            foreach (KeyValuePair<string, string> skipItem in skipData)
                            {
                                if (rtnDict[i][skipItem.Key] == skipItem.Value)
                                {
                                    rtnDict.RemoveAt(i);
                                    break;
                                } else if (skipItem.Value == "" && (rtnDict[i][skipItem.Key] == null || rtnDict[i][skipItem.Key]==""))
                                {
                                    rtnDict.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                    }
                    return JsonConvert.SerializeObject(rtnDict);
                }
            }
        }

        public string excelToJson(Dictionary<string, string> excelDict)
        {
            List<string> headerList = JsonConvert.DeserializeObject<List<string>>(excelDict["DataCol"]);
            Dictionary<string, string> skipData = JsonConvert.DeserializeObject<Dictionary<string, string>>(excelDict["skipDataDict"]);
            return excelToJson(excelDict["excelFile"], headerList, skipData, excelDict["workSheet"]);
        }
        public string excelToCsv(string excelFile)
        {
            try
            {
                excelFile = excelFile.Replace("/", "\\");
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(excelFile);
                string csvFile = excelFile.Replace(".xlsx", ".csv");
                workbook.Save(csvFile);
                return csvFile;
            }
            catch (Exception ex)
            {
                string sendmailbody = "ExcelToCsv錯誤\n" +
                          ex.Message + "\n" + ex.StackTrace.ToString();
                CommonClass.smtpSendMail(sendmailbody,
                            new Dictionary<string, string>() { { "資訊部", "id@firstohm.com.tw" } },
                            true, "火箭自動通知 列印考核表發生嚴重錯誤");
                return "";
            }
        }


        public string  SaveExcelToCSVWithDelimiter(string excelFile, string sheetName,  string delimiter=",", int titleIdx=1,  int rowIdxDetector=1)
        {
            System.Data.DataTable dt = new_excelToDatatable(excelFile, sheetName, 1, 2);

            string csvFilePath = excelFile.Replace(".xlsx", ".csv");
            if (CommonClass.ifFileExists(csvFilePath))
                File.Delete(csvFilePath);
            List<String> csvLine = new List<String>();
            foreach (DataRow dr in dt.Rows)
            {
                csvLine.Clear();
                foreach (object oObject in dr.ItemArray)
                {
                    csvLine.Add(oObject.ToString());
                }
                if (csvLine.Count == 0)
                    continue;
                File.AppendAllText(csvFilePath, string.Join(delimiter, csvLine) + Environment.NewLine);
            }

            CommonClass.removeEmptyLines(csvFilePath, csvFilePath);
            return csvFilePath;
        }

       
        private static IEnumerable<DataColumn> GetDataColumns(Microsoft.Office.Interop.Excel.Worksheet worksheet)
        {
            return GatherColumnNames(worksheet).Select(x => new DataColumn(x));
        }

        private static IEnumerable<string> GatherColumnNames(Microsoft.Office.Interop.Excel.Worksheet worksheet)
        {
            var columns = new List<string>();

            var i = 1;
            var j = 1;
            var columnName = worksheet.Cells[i, j].Value;
            while (columnName != null)
            {
                columns.Add(GetUniqueColumnName(columns, columnName.ToString()));
                j++;
                columnName = worksheet.Cells[i, j].Value;
            }

            return columns;
        }

        private static string GetUniqueColumnName(IEnumerable<string> columnNames, string columnName)
        {
            var colName = columnName;
            var i = 1;
            while (columnNames.Contains(colName))
            {
                colName = columnName + i.ToString();
                i++;
            }

            return colName;
        }

        /// <summary>
        /// Assumption: There are no null or empty cells in the first column
        /// </summary>
        /// <param name="worksheet"></param>
        /// <returns></returns>
        private static int GetTableDepth(Microsoft.Office.Interop.Excel.Worksheet worksheet, int headerOffset)
        {
            var i = 1;
            var j = 1;
            var cellValue = worksheet.Cells[i + headerOffset, j].Value;
            while (cellValue != null)
            {
                i++;
                cellValue = worksheet.Cells[i + headerOffset, j].Value;
            }

            return i - 1; //subtract one because we're going from rownumber (1 based) to depth (0 based)
        }

        public bool insertPicsToEcel(string fileExcel, List<ExcelPics> barCodePics, bool ifclose = true, bool ifPrint = false, int offSet = 0, string saveAsPath = null)
        {
            bool rtnBool = false;
            if (saveAsPath == null)
                saveAsPath = fileExcel;
            Microsoft.Office.Interop.Excel.Workbook mWorkBook;
            Microsoft.Office.Interop.Excel.Sheets mWorkSheets;
            Microsoft.Office.Interop.Excel.Worksheet mWSheet1;
            Microsoft.Office.Interop.Excel.Application oXL;
            object misvalue = System.Reflection.Missing.Value;
            try
            {
                string path = fileExcel;
                oXL = new Microsoft.Office.Interop.Excel.Application();
                oXL.DisplayAlerts = false;
                mWorkBook = oXL.Workbooks.Open(path, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                //Get all the sheets in the workbook
                mWorkSheets = mWorkBook.Worksheets;
                //Get the allready exists sheet
                mWSheet1 = (Microsoft.Office.Interop.Excel.Worksheet)mWorkSheets.get_Item("流程單");
                Microsoft.Office.Interop.Excel.Range range = mWSheet1.UsedRange;
                int listidx = 0;
                foreach (ExcelPics barcode in barCodePics)
                {
                    mWSheet1.Shapes.AddPicture(barcode.PicPath, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, barcode.posX, barcode.posY + listidx++ * offSet, barcode.width, barcode.heigh);
                }
                if (ifPrint)
                {
                    this.printExcel(mWSheet1);
                }
                mWorkBook.SaveAs(saveAsPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
                if (ifclose)
                {
                    mWorkBook.Close();
                    mWSheet1 = null;
                    mWorkBook = null;
                    mWorkBook.Close();
                    Marshal.FinalReleaseComObject(mWorkBook);

                    oXL.Quit();
                    Marshal.FinalReleaseComObject(oXL);
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
                rtnBool = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "outputToExcel()");
            }
            return rtnBool;
        }

        //cellPos  例如 A1, B10...
        public bool insertPicToCell(Microsoft.Office.Interop.Excel.Worksheet mWSheet1, string cellPos, string barCodePic, int width, int heigh, int topMargingY = 110, int topMargingX = 2)
        {
            Microsoft.Office.Interop.Excel.Range range = getRange(mWSheet1, cellPos);
            float posX = (float)range.Left + topMargingX;
            float posY = (float)range.Top + topMargingY;
            mWSheet1.Shapes.AddPicture(barCodePic, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, posX, posY, width, heigh);
            return true;
        }
        //Ronny version, 這版比較好
        public bool insertPicToCell(Microsoft.Office.Interop.Excel.Worksheet mWSheet1, string cellPos, string barCodePic, float width, float heigh, int topMarging = 0, int leftMargin = 0)
        {
            Microsoft.Office.Interop.Excel.Range range = getRange(mWSheet1, cellPos);
            float posX = (float)range.Left + leftMargin;
            float posY = (float)range.Top + topMarging;
            mWSheet1.Shapes.AddPicture(barCodePic, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, posX, posY, width, heigh);
            return true;
        }

        //offSet: 每一張的長度
        //listidx: 從第幾張開始列印 Barcode, 
        public bool insertPicsToExcel(Microsoft.Office.Interop.Excel.Worksheet mWSheet1, List<ExcelPics> barCodePics, bool ifPrint = false, int offSet = 0, int listidx = 0)
        {
            Microsoft.Office.Interop.Excel.Range range = mWSheet1.UsedRange;
            foreach (ExcelPics barcode in barCodePics)
            {
                mWSheet1.Shapes.AddPicture(barcode.PicPath, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, barcode.posX, barcode.posY + listidx++ * offSet, barcode.width, barcode.heigh);
            }
            if (ifPrint)
            {
                this.printExcel(mWSheet1);
            }

            return true;
        }

        #region Save to PDF
        //save html, xlsx, css.. To pdf file
        //int pageOrientation=0 直印, 1：橫印  
        //pageOrientation 符合頁高或 頁寬， 0:不需要, 1:直印, 2：橫印  3：直印+橫印
        //newXlsxFilePathAndName -- 目標 pdf 檔名
        //如果 newXlsxFilePathAndName 為null， 則以 htmlFilePathAndName 作為 pdf 檔名
        public bool saveHtmlToPdf(string htmlFilePathAndName, out string returnStr, string newXlsxFilePathAndName = null, int pageOrientation = 0, int fitPage = 2)
        {
            //string htmlFilePathAndName = @"C:\test.html";
            //string newXlsxFilePathAndName = @"C:\test.xlsx";
            try
            {
                if (!File.Exists(htmlFilePathAndName))
                {
                    returnStr = "查無來源檔案，" + htmlFilePathAndName + " 請確認!!";
                    return false;
                }
                if (!string.IsNullOrEmpty(newXlsxFilePathAndName))
                {
                    newXlsxFilePathAndName = newXlsxFilePathAndName.Replace(Path.GetExtension(htmlFilePathAndName).ToLower(), ".pdf");
                }
                else
                {
                    newXlsxFilePathAndName = htmlFilePathAndName.Replace(Path.GetExtension(htmlFilePathAndName).ToLower(), ".pdf");
                }

                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook xls;
                xls = excel.Workbooks.Open(htmlFilePathAndName);

                var _with1 = xls.ActiveSheet.PageSetup;
                if (pageOrientation == 1) //橫印
                    _with1.Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape;
                else //直印
                    _with1.Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlPortrait;
                switch (fitPage)
                {
                    case 1:
                        _with1.Zoom = false;
                        _with1.FitToPagesTall = 1;
                        break;
                    case 2:
                        _with1.Zoom = false;
                        _with1.FitToPagesWide = 1;
                        break;
                    case 3:
                        _with1.Zoom = false;
                        _with1.FitToPagesTall = 1;
                        _with1.FitToPagesWide = 1;
                        break;
                }
                xls.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, newXlsxFilePathAndName);
                closeExcel(ref excel, ref xls, null, false);
                returnStr = newXlsxFilePathAndName;
                return true;
            }
            catch (Exception ex)
            {
                returnStr = ex.Message;
                return false;
            }
        }

        //Save worksheet to pdf
        public void saveToPdf(ExcelElements ee, string pdfFile, string worksheetName = null, bool ifIgnorePrintArea=false)
        {
            const int xlQualityStandard = 0;
            if (!string.IsNullOrEmpty(worksheetName))
                ee.mWSheet = ee.mWorkSheets[worksheetName];
            ee.mWSheet.ExportAsFixedFormat(
                    XlFixedFormatType.xlTypePDF,
                    pdfFile, xlQualityStandard, true, ifIgnorePrintArea,
                    Type.Missing, Type.Missing, false, Type.Missing);
        }
        #endregion

        #region excel macro
        //https://dotblogs.com.tw/killysss/2015/10/01/153471
        /// <summary>
        /// 執行Excel中的module
        /// </summary>
        /// <param name="excelFilePath">Excel文件路徑</param>
        /// <param name="macroName">module名稱</param>
        /// <param name="parameters">module參數組</param>
        /// <param name="rtnValue">module返回值</param>
        /// <param name="isShowExcel">執行時是否顯示Excel</param>
        public void RunExcelMacro(
                                            string excelFilePath,
                                            string macroName,
                                            object[] parameters,
                                            out object rtnValue,
                                            bool isShowExcel
                                        )
        {
            try
            {
                #region 檢查入參

                // 檢查文件是否存在
                if (!File.Exists(excelFilePath))
                {
                    throw new System.Exception(excelFilePath + " 文件不存在");
                }

                // 檢查是否輸入module名稱
                if (string.IsNullOrEmpty(macroName))
                {
                    throw new System.Exception("請輸入module的名稱");
                }

                #endregion

                #region 調用module處理

                // 準備打開Excel文件時的缺省參數對象
                object oMissing = System.Reflection.Missing.Value;

                // 根據參數組是否為空，準備參數組對象
                object[] paraObjects;

                if (parameters == null)
                {
                    paraObjects = new object[] { macroName };
                }
                else
                {
                    // module參數組長度
                    int paraLength = parameters.Length;

                    paraObjects = new object[paraLength + 1];

                    paraObjects[0] = macroName;
                    for (int i = 0; i < paraLength; i++)
                    {
                        paraObjects[i + 1] = parameters[i];
                    }
                }

                // 創建Excel對象示例
                Microsoft.Office.Interop.Excel.Application oExcel = new Microsoft.Office.Interop.Excel.Application();

                // 判斷是否要求執行時Excel可見
                if (isShowExcel)
                {
                    // 使創建的對象可見
                    oExcel.Visible = true;
                }

                // 創建Workbooks對象
                Microsoft.Office.Interop.Excel.Workbooks oBooks = oExcel.Workbooks;

                // 創建Workbook對象
                Microsoft.Office.Interop.Excel._Workbook oBook = null;

                // 打開指定的Excel文件
                oBook = oBooks.Open(
                                        excelFilePath,
                                        oMissing,
                                        oMissing,
                                        oMissing,
                                        oMissing,
                                        oMissing,
                                        oMissing,
                                       oMissing,
                                       oMissing,
                                       oMissing,
                                       oMissing,
                                       oMissing,
                                       oMissing,
                                       oMissing,
                                       oMissing
                                  );

                // 執行Excel中的module
                rtnValue = this.RunMacro(oExcel, paraObjects);

                // 保存更改
                oBook.Save();

                // 退出Workbook
                oBook.Close(false, oMissing, oMissing);

                #endregion

                #region 釋放對象

                // 釋放Workbook對象
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBook);
                oBook = null;

                // 釋放Workbooks對象
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBooks);
                oBooks = null;

                // 關閉Excel
                oExcel.Quit();

                // 釋放Excel對象
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel);
                oExcel = null;

                // 調用垃圾回收
                GC.Collect();

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 執行module
        /// </summary>
        /// <param name="oApp">Excel對象</param>
        /// <param name="oRunArgs">參數（第一個參數為指定module名稱，後面為指定module的參數值）</param>
        /// <returns>module返回值</returns>
        private object RunMacro(object oApp, object[] oRunArgs)
        {
            try
            {
                // 聲明一個返回對象
                object objRtn;

                // 反射方式執行module
                objRtn = oApp.GetType().InvokeMember(
                                                        "Run",
                                                        System.Reflection.BindingFlags.Default |
                                                        System.Reflection.BindingFlags.InvokeMethod,
                                                        null,
                                                        oApp,
                                                        oRunArgs
                                                     );
                // 返回值
                return objRtn;
            }
            catch (Exception ex)
            {
                // 如果有底層異常，拋出底層異常
                if (ex.InnerException.Message.ToString().Length > 0)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw ex;
                }
            }
        }
        #endregion
    }
}
