using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirstOhm;
using Newtonsoft.Json;

namespace TaskTrayApplication
{
    public partial class 考核表報表 : Form
    {
        DataTable dt = null;
        string stdOutputPath = @"D:\ReportOut";
        public 考核表報表()
        {
            InitializeComponent();
        }

        private void 考核表報表_Load(object sender, EventArgs e)
        {
            setYearText();
            txtOutput.Text =  stdOutputPath = Constants.getProperty("考核表output", @"D:\ReportOut");
        }

        private void btnPrintRpt_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> chkRows = UI_CommonClass.GetAllCheckedRows(dgv1, "選取");
            if(chkRows.Count == 0)
            {
                MessageBox.Show("請勾選列印項目");
                return;
            }
            string propertyJson = Constants.getProperty("NJ_考核表", null);
            if(propertyJson == null)
            {
                MessageBox.Show("Property NJ_考核表");
                return;
            }
            Dictionary<string,string> dic = JsonConvert.DeserializeObject<Dictionary<string,string>>(propertyJson);
            string excelFile;
            if (dic.ContainsKey("excelFile"))
                excelFile = dic["excelFile"];
            else
            {
                excelFile = "";
                MessageBox.Show("Property找無母版位置");
                return;
            }
            string yearType;
            foreach(DataGridViewRow dr in chkRows)
            {
                yearType = null;
                if (dr.Cells["half"].Value.ToString() == "1")
                    yearType = "上半年";
                else if (dr.Cells["half"].Value.ToString() == "2")
                    yearType = "下半年";
                Dictionary<string, string> receiveDict = new Dictionary<string, string>()
                {
                    {"year",dr.Cells["year"].Value.ToString() },{"yearType",yearType },{"excelFile", excelFile},{"OutputPath",txtOutput.Text },{"userid",dr.Cells["user_id"].Value.ToString() }
                };
                CommonApp.do_assessment(receiveDict);
            }
            #region 舊程式碼
            //if (dt == null || dt.Rows.Count == 0)
            //    button1.PerformClick();

            //if (dt == null || dt.Rows.Count == 0)
            //    return;
            //Dictionary<string, String> dictExcelData = new Dictionary<string, String>();
            //string rptTitle = txtDataYear.Text + " " + (radioDown.Checked ? "下半年" : "上半年");
            //List<List<Dictionary<string, String>>> dynaInput;
            //string jsonDynamic;
            //int jsonIdx = 0;
            //double personalTotal = 0;
            //double tempDouble = 0;
            //ExcelLib excel = new ExcelLib();
            //ExcelElements ee = new ExcelElements();            
            ////stdOutputPath += @"\" + txtDataYear.Text + (radioDown.Checked?"下半年": "上半年")+"_";
            //stdOutputPath += @"\" + txtDataYear.Text + (radioUpHalf.Checked?"上半年":"下半年") ;
            //string 母版Path = Constants.getProperty("考核表母版", @"C:\sysTray\母版\考核表母版.xlsx") ;
            //string outputFile;
            //bool saveAsPdf = true;
            //if (!CommonClass.ifFileorFolderExists(stdOutputPath,1))
            //{
            //    CommonClass.createIfMissing(stdOutputPath);
            //}

            //CommonClass.deleteFoldersAndFiles(stdOutputPath, false, true);

            //IEnumerable<DataGridViewRow> selectedDt = UI_CommonClass.getDgvChkedRows(dgv1);
            //DataRow dr = null;
            //DataTable tempDt = null;
            //foreach (DataGridViewRow selDr in selectedDt)
            //{
            //    tempDt = CommonClass.DataTableFilterSort1(dt, $"user_id={selDr.Cells["user_id"].Value}");
            //    if (tempDt != null && tempDt.Rows.Count == 1)
            //        dr = tempDt.Rows[0];
            //    ee = excel.openExcel(母版Path, true);
            //    ee.mWSheet = excel.getWorksheet(ee, "母版");
            //    dictExcelData.Clear();
            //    if (dr["dynamicInput"] == DBNull.Value || dr["dynamicInput"].ToString() == "")
            //        continue;
            //    saveAsPdf = true;
            //    outputFile = stdOutputPath + @"\";
            //    //ee = excel.openExcel(stdOutputPath + (dr["部門"] == DBNull.Value ? "" : dr["部門"].ToString()+"_") + (dr["姓名"] == DBNull.Value ? "" : dr["姓名"].ToString()) + ".xslx");
            //    jsonDynamic = dr["dynamicInput"].ToString().Replace("\"", "'");
            //    dictExcelData.Add("A1", rptTitle);
            //    dictExcelData.Add("B2", dr["部門"] ==DBNull.Value?"":dr["部門"].ToString());
            //    dictExcelData.Add("J2", dr["職稱"] == DBNull.Value ? "" : dr["職稱"].ToString());
            //    dictExcelData.Add("B3", dr["姓名"] == DBNull.Value ? "" : dr["姓名"].ToString());
            //    dictExcelData.Add("F3", dr["到職日"] == DBNull.Value ? "" : dr["到職日"].ToString());
            //    dictExcelData.Add("J3", dr["年齡"] == DBNull.Value ? "" : dr["年齡"].ToString());
            //    dictExcelData.Add("C4", dr["工作內容"] == DBNull.Value ? "" : dr["工作內容"].ToString());
            //    //do Json
            //    dynaInput = JsonConvert.DeserializeObject<List<List<Dictionary<string, String>>>>(jsonDynamic);
            //    jsonIdx = 0;
            //    personalTotal = 0;
            //    for (int i=0; i< dynaInput.Count;i++)
            //    {
            //        jsonIdx = i + 6;
            //        if(jsonIdx >= 19)
            //        {
            //            saveAsPdf = false;
            //            break;
            //        }
            //        if(dynaInput[i].Count >0)
            //            dictExcelData.Add("C" + jsonIdx, dynaInput[i][0]["工作目標"]);
            //        if (dynaInput[i].Count > 1)
            //            dictExcelData.Add("H" + jsonIdx, dynaInput[i][1]["期限"]);
            //        if (dynaInput[i].Count > 2)
            //            dictExcelData.Add("I" + jsonIdx, dynaInput[i][2]["比例"] + " %");
            //        if (dynaInput[i].Count > 3)
            //            dictExcelData.Add("J" + jsonIdx, dynaInput[i][3]["完成日期"]);
            //        if (dynaInput[i].Count > 4)
            //        {
            //            dictExcelData.Add("K" + jsonIdx, dynaInput[i][4]["執行成果"] + " %");
            //            if (double.TryParse(dynaInput[i][4]["執行成果"], out tempDouble))
            //                personalTotal += tempDouble;
            //        }
            //    }
            //    dictExcelData.Add("C19", personalTotal.ToString("0.##") + " %");
            //    dictExcelData.Add("C20", dr["train"] == DBNull.Value ? "" : dr["train"].ToString());
            //    dictExcelData.Add("C25", dr["otherResults"] == DBNull.Value ? "" : dr["otherResults"].ToString());
            //    dictExcelData.Add("C26", dr["job_change"] == DBNull.Value ? "" : dr["job_change"].ToString());
            //    dictExcelData.Add("C27", dr["suggestion"] == DBNull.Value ? "" : dr["suggestion"].ToString());

            //    dictExcelData.Add("C29", dr["supervisor_rank"] == DBNull.Value ? "" : dr["supervisor_rank"].ToString());
            //    dictExcelData.Add("E29", dr["supervisor_comment"] == DBNull.Value ? "" : dr["supervisor_comment"].ToString().Replace("<br>", Environment.NewLine));
            //    dictExcelData.Add("C31", dr["final_result"] == DBNull.Value ? "" : dr["final_result"].ToString().Replace("<br>", Environment.NewLine));
            //    excel.dictToExcel(ee.mWSheet, dictExcelData);
            //    if(saveAsPdf)
            //    {
            //        excel.saveToPdf(ee, outputFile + (dr["部門"] == DBNull.Value ? "" : dr["部門"].ToString() + "_") + (dr["姓名"] == DBNull.Value ? "" : dr["姓名"].ToString()) + ".pdf");
            //        excel.closeExcel(ee, null, false);
            //    } else
            //    {
            //        excel.closeExcel(ee, outputFile + (dr["部門"] == DBNull.Value ? "" : dr["部門"].ToString() + "_") + (dr["姓名"] == DBNull.Value ? "" : dr["姓名"].ToString()) + ".xlsx", false);
            //    }


            //}
            #endregion
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sqlStr = "select `id_apply`,`form_type_id`,`user_id`,`year`,`half`,`title` 職稱,`dept` 部門,`name` 姓名," +
                " `onBordDate` 到職日,`age` 年齡, `jobDiscript` 工作內容,`dynamicInput`,`train` 發展培養,`suggestion` 建議,`otherResults` 其他成果," +
                " train, otherResults, job_change, suggestion, supervisor_rank, supervisor_comment, job_change, final_result  " +
                " from xyz_uj_assessmentrtp_views where `half`=" + (radioDown.Checked ? "2" : "1") +
                " and year = " + txtDataYear.Text +
                " order by `dept` ";
            dt = CommonClass.getSQLDataTable(sqlStr, Constants.portalConnString);
            List<string> hideCols = new List<string>() { "年齡", "工作內容", "發展培養", "建議", "其他成果" };
            UI_CommonClass.dtTodgvWithChkbox(dt, dgv1, "選取", checkBox1.Checked );
            UI_CommonClass.dgvHideColumn(dgv1, hideCols);
            
        }

        private void setYearText()
        {
            if (radioDown.Checked && DateTime.Now.Month <= 2)
                txtDataYear.Text = (DateTime.Now.Year - 1).ToString();
            else
                txtDataYear.Text = (DateTime.Now.Year).ToString();
        }

        private void radioDown_CheckedChanged(object sender, EventArgs e)
        {
            setYearText();
        }

        private void radioUpHalf_CheckedChanged(object sender, EventArgs e)
        {
            setYearText();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UI_CommonClass.dgvChkAll(dgv1, "選取", checkBox1.Checked);
        }
    }
}
