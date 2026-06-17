using FirstOhm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskTrayApplication
{
    public partial class 問卷調查結果分析 : Form
    {
        public 問卷調查結果分析()
        {
            InitializeComponent();
        }

        private void 問卷調查結果分析_Load(object sender, EventArgs e)
        {
            cmbQuestionnaireTitle.SelectedIndex = 0;
        }

        private void btmQuery_Click(object sender, EventArgs e)
        {
            string sqlStr = null;
            if(cmbQuestionnaireTitle.SelectedIndex == 0)
            {
                MessageBox.Show("請選擇問卷!!!", "警示訊息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            sqlStr = getQuestionnairesqlStr(cmbQuestionnaireTitle.Text);
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.portalConnString);
            UI_CommonClass.dtToDgvWithSelectAllCheckBox(dt, dgv1, null);
            dgv1.AutoResizeColumns();
            
        }
        private string getQuestionnairesqlStr(string questionnaireTitle)
        {
            string sqlStr = null;
            switch (cmbQuestionnaireTitle.Text)
            {
                case "111年度員工旅遊滿意度問卷"://fromid=35
                    sqlStr = "SELECT a.`form_id`, a.`user_id` , c.`department` , b.`name` , a.`group_id` , a.`element_label` , a.`element_value` , a.`date` " +
                        " FROM `xyz_formmaker_submits` a " +
                        " JOIN `xyz_users` b ON a.`user_id` = b.`id` " +
                        " LEFT JOIN `xyz_hikashop_user` c ON a.`user_id` = c.`user_cms_id` " +
                        " WHERE `form_id` = 35 " +
                        " ORDER BY `group_id` , `element_label`; ";
                    break;
            }
            return sqlStr;
        }

        private void btnPrintExcel_Click(object sender, EventArgs e)
        {
            switch (cmbQuestionnaireTitle.Text)
            {
                case "111年度員工旅遊滿意度問卷":
                    printExcelEmployeeTravel_111();
                    break;
            }
        }
        private void printExcelEmployeeTravel_111()
        {
            Dictionary<string, string> satisfactionDic = new Dictionary<string, string>()
            {
                {"1","很滿意" },{"2","滿意" },{"3","普通" },{"4","不滿意" },{"5","非常不滿意" }
            };
            Dictionary<string, string> questionDic = new Dictionary<string, string>()
            {
                {"groupid","group_id" },{"name","姓名" },
                {"16","一、關於旅遊行程意見:" },{"16-1","1.請問您對於本次旅遊的整體評價如何?" },{"16-2","2.關於旅遊行程活動安排是否滿意?" },
                {"17","二、關於旅遊行程餐飲安排意見:" },{"17-1","1.請對您對旅遊行程餐飲環境是否滿意?" },{"17-2","2.請對您對旅遊行程餐飲菜色是否滿意?" },{"17-3","3.請對您對旅遊行程餐飲份量是否滿意?" },
                {"18","三、關於旅遊行程導遊/領隊/遊覽車意見:" },{"18-1","1.請問您對導遊/領隊服務態度是否滿意?" },{"18-2","2.請問您對導遊/領隊解說能力是否滿意?" },{"18-3","3.請問您對導遊/領隊專業知識是否滿意?" },{"18-4","4.請問您對司機服務態度是否滿意?" },{"18-5","5.請問您對遊覽車品質是否滿意?" },
                {"21","四、請問您是否有對旅遊規劃的建議?" },
                {"22","五、請問您是否有對旅行社的建議?" },
                {"23","六、請問您是否有對公司旅遊安排的建議?" }
            };
            //answerDic : dic<"userid",dic<"問題編號","答案">>
            Dictionary<string, Dictionary<string, string>> answerDicByUserid = new Dictionary<string, Dictionary<string, string>>();
            string sqlStr = "SELECT a.`form_id`, a.`user_id` , c.`department` , b.`name` , a.`group_id` , a.`element_label` , a.`element_value` , a.`date` " +
                " FROM `xyz_formmaker_submits` a " +
                " JOIN `xyz_users` b ON a.`user_id` = b.`id` " +
                " LEFT JOIN `xyz_hikashop_user` c ON a.`user_id` = c.`user_cms_id` " +
                " WHERE `form_id` = 35 " +
                " ORDER BY `group_id` , `element_label`; ";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr, Constants.portalConnString);
            Dictionary<string, string> answerDic = new Dictionary<string, string>();
            foreach (DataRow dr in dt.Rows)
            {
                List<string> valueList;
                string elementValue = dr["element_value"].ToString();
                if (dr["element_label"].ToString() == "16")
                {
                    answerDic = new Dictionary<string, string>();
                    answerDic.Add("groupid", dr["group_id"].ToString());
                    answerDic.Add("name", dr["name"].ToString());
                    answerDic.Add("department", dr["department"].ToString());
                    valueList = new List<string>(elementValue.Split('*'));
                    valueList.RemoveAll(s => s == "");

                    answerDic.Add("16-1", valueList[10].Split('_')[1]);
                    answerDic.Add("16-2", valueList[11].Split('_')[1]);
                }
                else if (dr["element_label"].ToString() == "17")
                {
                    valueList = new List<string>(elementValue.Split('*'));
                    valueList.RemoveAll(s => s == "");

                    answerDic.Add("17-1", valueList[11].Split('_')[1]);
                    answerDic.Add("17-2", valueList[12].Split('_')[1]);
                    answerDic.Add("17-3", valueList[13].Split('_')[1]);
                }
                else if (dr["element_label"].ToString() == "18")
                {
                    valueList = new List<string>(elementValue.Split('*'));
                    valueList.RemoveAll(s => s == "");

                    answerDic.Add("18-1", valueList[13].Split('_')[1]);
                    answerDic.Add("18-2", valueList[14].Split('_')[1]);
                    answerDic.Add("18-3", valueList[15].Split('_')[1]);
                    answerDic.Add("18-4", valueList[16].Split('_')[1]);
                    answerDic.Add("18-5", valueList[17].Split('_')[1]);
                }
                else if (dr["element_label"].ToString() == "21")
                {
                    answerDic.Add("21", elementValue);
                }
                else if (dr["element_label"].ToString() == "22")
                {
                    answerDic.Add("22", elementValue);
                }
                else if (dr["element_label"].ToString() == "23")
                {
                    answerDic.Add("23", elementValue);
                    if (answerDicByUserid.ContainsKey(dr["user_id"].ToString()))
                        answerDicByUserid[dr["user_id"].ToString()] = answerDic;
                    else
                        answerDicByUserid.Add(dr["user_id"].ToString(), answerDic);
                }
            }
            Dictionary<string, Dictionary<string,int>> totalCount = new Dictionary<string, Dictionary<string, int>>() 
            {
                {"16-1", new Dictionary<string, int>(){ { "1", 0 }, { "2", 0 }, { "3", 0 }, { "4", 0 }, { "5", 0 } } },
                {"16-2", new Dictionary<string, int>(){ { "1", 0 }, { "2", 0 }, { "3", 0 }, { "4", 0 }, { "5", 0 } } },
                {"17-1", new Dictionary<string, int>(){ { "1", 0 }, { "2", 0 }, { "3", 0 }, { "4", 0 }, { "5", 0 } } },
                {"17-2", new Dictionary<string, int>(){ { "1", 0 }, { "2", 0 }, { "3", 0 }, { "4", 0 }, { "5", 0 } } },
                {"17-3", new Dictionary<string, int>(){ { "1", 0 }, { "2", 0 }, { "3", 0 }, { "4", 0 }, { "5", 0 } } },
                {"18-1", new Dictionary<string, int>(){ { "1", 0 }, { "2", 0 }, { "3", 0 }, { "4", 0 }, { "5", 0 } } },
                {"18-2", new Dictionary<string, int>(){ { "1", 0 }, { "2", 0 }, { "3", 0 }, { "4", 0 }, { "5", 0 } } },
                {"18-3", new Dictionary<string, int>(){ { "1", 0 }, { "2", 0 }, { "3", 0 }, { "4", 0 }, { "5", 0 } } },
                {"18-4", new Dictionary<string, int>(){ { "1", 0 }, { "2", 0 }, { "3", 0 }, { "4", 0 }, { "5", 0 } } },
                {"18-5", new Dictionary<string, int>(){ { "1", 0 }, { "2", 0 }, { "3", 0 }, { "4", 0 }, { "5", 0 } } },
            };
            Dictionary<string, Dictionary<string, string>> totalCountText = new Dictionary<string, Dictionary<string, string>>()
            {
                {"21", new Dictionary<string, string>() },
                {"22", new Dictionary<string, string>() },
                {"23", new Dictionary<string, string>() }
            };
            List<string> answerUser = new List<string>();
            foreach ( KeyValuePair<string,Dictionary<string,string>> dic in answerDicByUserid)
            {
                string userid = dic.Key;
                Dictionary<string, string> answer = dic.Value;
                totalCount["16-1"][answer["16-1"]] += 1;
                totalCount["16-2"][answer["16-2"]] += 1;
                totalCount["17-1"][answer["17-1"]] += 1;
                totalCount["17-2"][answer["17-2"]] += 1;
                totalCount["17-3"][answer["17-3"]] += 1;
                totalCount["18-1"][answer["18-1"]] += 1;
                totalCount["18-2"][answer["18-2"]] += 1;
                totalCount["18-3"][answer["18-3"]] += 1;
                totalCount["18-4"][answer["18-4"]] += 1;
                totalCount["18-5"][answer["18-5"]] += 1;
                if(answer["21"] != "")
                    totalCountText["21"].Add(answer["name"], answer["21"]);
                if (answer["22"] != "")
                    totalCountText["22"].Add(answer["name"], answer["22"]);
                if (answer["23"] != "")
                    totalCountText["23"].Add($"{answer["department"]}-{answer["name"]}", answer["23"]);
                answerUser.Add($"{answer["department"]}-{answer["name"]}");
            }


            ExcelLib excel = new ExcelLib();
            ExcelElements ee = excel.openExcel(null);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = excel.getWorksheet(ee, "工作表1");
            Microsoft.Office.Interop.Excel.Range range;
            int excelrowidx = 0;
            excelrowidx++;
            range = worksheet.get_Range($"A{excelrowidx}",$"F{excelrowidx}");
            range.Merge();
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = questionDic["16"];
            excelrowidx++;
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("B")] = satisfactionDic["1"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("C")] = satisfactionDic["2"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("D")] = satisfactionDic["3"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("E")] = satisfactionDic["4"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("F")] = satisfactionDic["5"];
            excelrowidx++;
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = questionDic["16-1"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("B")] = totalCount["16-1"]["1"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("C")] = totalCount["16-1"]["2"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("D")] = totalCount["16-1"]["3"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("E")] = totalCount["16-1"]["4"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("F")] = totalCount["16-1"]["5"];
            excelrowidx++;
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = questionDic["16-2"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("B")] = totalCount["16-2"]["1"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("C")] = totalCount["16-2"]["2"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("D")] = totalCount["16-2"]["3"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("E")] = totalCount["16-2"]["4"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("F")] = totalCount["16-2"]["5"];
            excelrowidx++;
            range = worksheet.get_Range($"A{excelrowidx}", $"F{excelrowidx}");
            range.Merge();
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = questionDic["17"];
            excelrowidx++;
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("B")] = satisfactionDic["1"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("C")] = satisfactionDic["2"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("D")] = satisfactionDic["3"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("E")] = satisfactionDic["4"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("F")] = satisfactionDic["5"];
            excelrowidx++;
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = questionDic["17-1"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("B")] = totalCount["17-1"]["1"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("C")] = totalCount["17-1"]["2"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("D")] = totalCount["17-1"]["3"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("E")] = totalCount["17-1"]["4"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("F")] = totalCount["17-1"]["5"];
            excelrowidx++;
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = questionDic["17-2"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("B")] = totalCount["17-2"]["1"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("C")] = totalCount["17-2"]["2"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("D")] = totalCount["17-2"]["3"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("E")] = totalCount["17-2"]["4"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("F")] = totalCount["17-2"]["5"];
            excelrowidx++;
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = questionDic["17-3"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("B")] = totalCount["17-3"]["1"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("C")] = totalCount["17-3"]["2"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("D")] = totalCount["17-3"]["3"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("E")] = totalCount["17-3"]["4"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("F")] = totalCount["17-3"]["5"];
            excelrowidx++;
            range = worksheet.get_Range($"A{excelrowidx}", $"F{excelrowidx}");
            range.Merge();
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = questionDic["18"];
            excelrowidx++;
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("B")] = satisfactionDic["1"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("C")] = satisfactionDic["2"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("D")] = satisfactionDic["3"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("E")] = satisfactionDic["4"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("F")] = satisfactionDic["5"];
            excelrowidx++;
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = questionDic["18-1"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("B")] = totalCount["18-1"]["1"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("C")] = totalCount["18-1"]["2"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("D")] = totalCount["18-1"]["3"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("E")] = totalCount["18-1"]["4"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("F")] = totalCount["18-1"]["5"];
            excelrowidx++;
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = questionDic["18-2"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("B")] = totalCount["18-2"]["1"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("C")] = totalCount["18-2"]["2"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("D")] = totalCount["18-2"]["3"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("E")] = totalCount["18-2"]["4"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("F")] = totalCount["18-2"]["5"];
            excelrowidx++;
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = questionDic["18-3"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("B")] = totalCount["18-3"]["1"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("C")] = totalCount["18-3"]["2"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("D")] = totalCount["18-3"]["3"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("E")] = totalCount["18-3"]["4"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("F")] = totalCount["18-3"]["5"];
            excelrowidx++;
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = questionDic["18-4"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("B")] = totalCount["18-4"]["1"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("C")] = totalCount["18-4"]["2"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("D")] = totalCount["18-4"]["3"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("E")] = totalCount["18-4"]["4"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("F")] = totalCount["18-4"]["5"];
            excelrowidx++;
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = questionDic["18-5"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("B")] = totalCount["18-5"]["1"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("C")] = totalCount["18-5"]["2"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("D")] = totalCount["18-5"]["3"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("E")] = totalCount["18-5"]["4"];
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("F")] = totalCount["18-5"]["5"];
            excelrowidx++;
            range = worksheet.get_Range($"A{excelrowidx}", $"F{excelrowidx}");
            range.Merge();
            excelrowidx++;
            range = worksheet.get_Range($"A{excelrowidx}", $"F{excelrowidx}");
            range.Merge();
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = questionDic["21"];
            foreach(KeyValuePair<string,string> dic in totalCountText["21"])
            {
                excelrowidx++;
                range = worksheet.get_Range($"A{excelrowidx}", $"F{excelrowidx}");
                range.Merge();
                worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = $"{dic.Key} : {dic.Value}";
            }
            excelrowidx++;
            range = worksheet.get_Range($"A{excelrowidx}", $"F{excelrowidx}");
            range.Merge();

            excelrowidx++;
            range = worksheet.get_Range($"A{excelrowidx}", $"F{excelrowidx}");
            range.Merge();
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = questionDic["22"];
            foreach (KeyValuePair<string, string> dic in totalCountText["22"])
            {
                excelrowidx++;
                range = worksheet.get_Range($"A{excelrowidx}", $"F{excelrowidx}");
                range.Merge();
                worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = $"{dic.Key} : {dic.Value}";
            }
            excelrowidx++;
            range = worksheet.get_Range($"A{excelrowidx}", $"F{excelrowidx}");
            range.Merge();

            excelrowidx++;
            range = worksheet.get_Range($"A{excelrowidx}", $"F{excelrowidx}");
            range.Merge();
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = questionDic["23"];
            foreach (KeyValuePair<string, string> dic in totalCountText["23"])
            {
                excelrowidx++;
                range = worksheet.get_Range($"A{excelrowidx}", $"F{excelrowidx}");
                range.Merge();
                worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("A")] = $"{dic.Key} : {dic.Value}";
            }
            excelrowidx++;
            range = worksheet.get_Range($"A{excelrowidx}", $"F{excelrowidx}");
            range.Merge();

            range = worksheet.get_Range("A1", $"F{excelrowidx}");
            range.Columns.AutoFit();
            range.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;

            excelrowidx = 0;
            excelrowidx++;
            worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("H")] = "已填寫之員工";
            foreach(string name in answerUser)
            {
                excelrowidx++;
                worksheet.Cells[excelrowidx, excel.NumberFromExcelColumn("H")] = name;
            }
            
            range = worksheet.get_Range("H1", $"H{excelrowidx}");
            range.Columns.AutoFit();
            range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            range.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;

        }
    }
}
