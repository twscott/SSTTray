using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using FirstOhm;

using Newtonsoft.Json;

namespace TaskTrayApplication
{
    public partial class 良率統計分析 : Form
    {
        LineLib linelib = null;
        DataTable dgv1Dt = null;
        public 良率統計分析()
        {
            InitializeComponent();
        }

        private void btnCaulc_Click(object sender, EventArgs e)
        {
            string sqlStr = $"SELECT `工令單號`,`批次號`,`完成日期`,`起始量`,`完成量`,`外檢1良率`,`全檢1良率`,`全檢2良率`,`外檢2良率`, 流程單良率 " +
                 $" FROM `view_basicyield` " +
                 $" where `完成量` > 0 and `完成日期` >= '{dtp1.Text}' and `完成日期` <= '{dtp2.Text}' " +
                 $" and `RTYPE`='{txtRtype.Text}' and `VAL`='{txtVal.Text}' and `TOL`='{txtTol.Text}' AND PPM='{txtPPM.Text}'";
            dgv1Dt = CommonClass.getSQLDataTable(sqlStr);
            UI_CommonClass.dtTodgv(dgv1, dgv1Dt);

            Dictionary<string, string> staticDict = FirstohmPrds.yieldStatistic(dgv1Dt, dtp1.Text, dtp2.Text, txtRtype.Text,
            txtVal.Text, txtTol.Text, txtPPM.Text);
            //$" order by 最低良率";
            //sqlStr = $"SELECT `RTYPE`,`Tol`,`VAL`,`PPM`, count(*) 流程單數, " +
            //    $" SUM(`起始量`) 總起始量, SUM(`完成量`) 總完成量, AVG(if(`外檢2良率`=0, " +
            //    $" 全檢2良率, 外檢2良率)) 平均良率, SUM(外檢2良率*`起始量`)/SUM(起始量) 加權平均良率, " +
            //    $" min(`外檢2良率`) 最低良率, min(`外檢2良率`) 最高良率 " +
            //    $" FROM `view_basicyield` " +
            //    $" where `完成量` > 0 and  `完成日期` >= '{dtp1.Text}' and `完成日期` <= '{dtp2.Text}' " +
            //    $" and `RTYPE`='{txtRtype.Text}' and `VAL`='{txtVal.Text}' and `TOL`='{txtTol.Text}' AND PPM='{txtPPM.Text}'";
            //DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            //if (dt.Rows.Count == 0 || dt.Rows[0][0]==DBNull.Value)
            //{
            //    MessageBox.Show("查無資料");
            //    return;
            //}

            //lblFlowCnt.Text = Convert.ToInt32(dt.Rows[0]["流程單數"]).ToString("0.#");
            //lblStartQuant.Text = Convert.ToInt32(dt.Rows[0]["總起始量"]).ToString();
            //lblEndQuant.Text = Convert.ToInt32(dt.Rows[0]["總完成量"]).ToString();
            //lblAvg.Text = Convert.ToDouble(dt.Rows[0]["平均良率"]).ToString("0.#");
            //lblWeightAvg.Text = Convert.ToDouble(dt.Rows[0]["加權平均良率"]).ToString("0.#");
            //lblMidian.Text = CommonClass.genMidian(dgv1Dt, "流程單良率").ToString("0.#");
            //lblLowestYield.Text = Convert.ToDouble(dt.Rows[0]["最低良率"]).ToString("0.#");
            //lblHeighestYield.Text = Convert.ToDouble(dt.Rows[0]["最高良率"]).ToString("0.#");

            //List<Int32> yieldsList = CommonClass.dtToIntList(dgv1Dt, "流程單良率");
            //Dictionary<int, int> zhongShuDict = CommonClass.CalcZhongShu(yieldsList);
            //if (zhongShuDict == null)
            //    lblZhongShu.Text = "樣品數不足分析眾數";
            //else
            //{
            //    int idx = 0;
                
            //    lblZhongShu.Text = JsonConvert.SerializeObject(zhongShuDict);
            //    if (zhongShuDict.Count > 1)
            //    { //多峰 眾數
            //        StringBuilder whereStr = new StringBuilder(" Select Round(SUM(外檢2良率*`起始量`)/SUM(起始量),1) 加權平均良率 From view_basicyield where (");
            //        foreach (double targetYile in zhongShuDict.Keys)
            //        {
            //            if (idx++ > 0)
            //                whereStr.Append(" Or ");
            //            whereStr.Append($" (流程單良率 >={targetYile - 0.5} and  流程單良率 < {targetYile + 0.5})");
            //        }
            //        whereStr.Append(")");
            //        whereStr.Append($" and `完成日期` >= '{dtp1.Text}' and `完成日期` <= '{dtp2.Text}' ");
            //        whereStr.Append($" and `RTYPE`='{txtRtype.Text}' and `VAL`='{txtVal.Text}' and `TOL`='{txtTol.Text}' AND PPM='{txtPPM.Text}'");
            //        lblZhongShuAvg.Text = CommonClass.getSQLScalar(whereStr.ToString()).ToString();
            //    }
            //    else
            //        lblZhongShuAvg.Text = lblZhongShu.Text;
            //}
                
            if(staticDict==null)
            {
                MessageBox.Show("查無資料");
                return;
            } else
            {
                lblFlowCnt.Text = staticDict["流程單數"];
                lblStartQuant.Text = staticDict["總起始量"];
                lblEndQuant.Text = staticDict["總完成量"];
                lblAvg.Text = staticDict["平均良率"];
                lblWeightAvg.Text = staticDict["加權平均良率"];

            }
            drawChart();
        }

        private void 良率統計分析_Load(object sender, EventArgs e)
        {
            dtp1.Text =  DateTime.Now.AddYears(-2).ToString("yyyy-MM-dd");
            dtp2.Text =  DateTime.Now.ToString("yyyy-MM-dd");
            dtp3.Text = dtp4.Text =  DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
            dtp6.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            dtp5.Text = dtp6.Value.AddDays(-7).ToString("yyyy-MM-dd");
            string sqlstr = "SELECT `name`,`username` " +
            " FROM `xyz_users` a Join xyz_hikashop_user b on a.id = b.user_cms_id where b.line_userid > '' and b.workstatus <> 1 ";
            DataTable dt = CommonClass.getSQLDataTable(sqlstr, Constants.portalConnString);
            Dictionary<string, string> lineUserDict = CommonClass.dtToDictionary(dt, "name", "username");
            UI_CommonClass.fillInComboByDict(comboUser, lineUserDict);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (linelib == null)
                linelib = new LineLib();
            string lineMsg = $"資料範圍：{dtp1.Text}~{dtp2.Text} {System.Environment.NewLine} " +
                $"型號：{txtRtype.Text} {System.Environment.NewLine}" +
                $"阻值：{txtVal.Text} {System.Environment.NewLine}" +
                $"誤差值：{txtTol.Text}% {System.Environment.NewLine}" +
                $"PPM：{txtPPM.Text} {System.Environment.NewLine}" +
                $"流程單數：{lblFlowCnt.Text} {System.Environment.NewLine}" +
                $"總發料量：{lblStartQuant.Text} {System.Environment.NewLine}" +
                $"總完成量：{lblEndQuant.Text} {System.Environment.NewLine}" +
                $"平均良率：{lblAvg.Text} {System.Environment.NewLine}" +
                $"加權平均良率：{lblWeightAvg.Text} {System.Environment.NewLine}" +
                $"良率中位數：{lblMidian.Text} {System.Environment.NewLine}" +
                $"良率眾數：{lblZhongShu.Text} {System.Environment.NewLine}" +
                $"加權平均眾數：{lblZhongShuAvg.Text} {System.Environment.NewLine}" +
                $"最高良率：{lblHeighestYield.Text} {System.Environment.NewLine}" +
                $"最低良率：{lblLowestYield.Text} {System.Environment.NewLine}";
            linelib.pushTextMessageByWebapiAsync(comboUser.SelectedValue.ToString(), lineMsg);
            MessageBox.Show("發送完成!!");
        }

        private void drawChart()
        {
            LibChart chartLib = new LibChart();
            chart1.Series.Clear();
            //SELECT truncate(外檢2良率,0) AS Range_Start, count(*) FROM view_basicyield where `RTYPE`= 'CM207' and `VAL`= '1M' and `TOL`= '5' AND `完成日期` > '2021-01-01' GROUP BY truncate(外檢2良率, 0)
            string sqlStr = $"SELECT Round(外檢2良率,0) AS Range_Start, count(*) Cnt " +
                $" FROM view_basicyield " +
                $" where `RTYPE`='{txtRtype.Text}' and `VAL`='{txtVal.Text}' and `TOL`='{txtTol.Text}' And PPM = '{txtPPM.Text}' " +
                $" AND `完成日期` >= '{dtp1.Text}' and `完成日期` <= '{dtp2.Text}' " +
                $" GROUP BY Round(外檢2良率,0) order by 1 ";
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            int[] xAxis = CommonClass.dtToIntList(dt, "Range_Start").ToArray();
            List<int> dataList = CommonClass.dtToIntList(dt, "Cnt");
            int[] yAxis = CommonClass.dtToIntList(dt, "Cnt").ToArray();
            List<int[]> chartXList = new List<int[]>();
            chartXList.Add(xAxis);
            List<int[]> chartYList = new List<int[]>();
            chartYList.Add(yAxis);
            List<string> lineTitle = new List<string>() { $"{txtRtype.Text}/{txtVal.Text}/{txtTol.Text}%/{txtPPM.Text}" };
            chartLib.draw_chart2(chart1, chartYList, chartXList, "良率分佈圖", lineTitle, 100, SeriesChartType.Column);
        }

        private void txtTol_TextChanged(object sender, EventArgs e)
        {
            if (CommonClass.IsNumeric(txtTol.Text))
                return;
            if (FirstohmPrds.TolDict.ContainsKey(txtTol.Text))
                txtTol.Text = FirstohmPrds.TolDict[txtTol.Text].ToString("0.##");
            else
                txtTol.Text = "";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string sqlStr = $"SELECT `RTYPE`,`TOLER`,`VAL`,`PPM`," +
                $" `SIZE`,`yield_Date` FROM `mfo_yield_footprint` " +
                $" where `yield_Date` >= '{dtp3.Text}' and `yield_Date` <= '{dtp4.Text}' ";
            string updStr;
            DataTable dt = CommonClass.getSQLDataTable(sqlStr);
            Dictionary<string, string> staticDict;
            DataTable dt2;
            foreach (DataRow dr in dt.Rows)
            {
                sqlStr = $"SELECT `工令單號`,`批次號`,`完成日期`,`起始量`,`完成量`," +
                 $" `外檢1良率`,`全檢1良率`,`全檢2良率`,`外檢2良率`, 流程單良率,  " +
                 $" subDate('{Convert.ToDateTime(dr["yield_Date"]).ToString("yyyy-MM-dd")}', interval {comboPeriodMonth.Text} Month) sDate, " +
                 $" '{Convert.ToDateTime(dr["yield_Date"]).ToString("yyyy-MM-dd")}' eDate " +
                 $" FROM `view_basicyield` " + 
                 $" where `完成量` > 0 and `完成日期` >= subDate('{Convert.ToDateTime(dr["yield_Date"]).ToString("yyyy-MM-dd")}', interval {comboPeriodMonth.Text} Month) " +
                 $" and `完成日期` <= '{Convert.ToDateTime(dr["yield_Date"]).ToString("yyyy-MM-dd")}' " +
                 $" and `RTYPE`='{dr["RTYPE"]}' and `VAL`='{dr["VAL"]}' " +
                 $" and `TOL`='{dr["TOLER"]}' AND PPM='{dr["PPM"]}'";
                dt2 = CommonClass.getSQLDataTable(sqlStr);
                if (dt2.Rows.Count == 0)
                    continue;
                staticDict = staticDict = FirstohmPrds.yieldStatistic(dt2, dt2.Rows[0]["sDate"].ToString(), dt2.Rows[0]["eDate"].ToString(), dr["RTYPE"].ToString(),
                dr["VAL"].ToString(), dr["TOLER"].ToString(), dr["PPM"].ToString());
                updStr = $"update  `mfo_yield_footprint` set `avgYield`='{staticDict["平均良率"]}', `weightedAvgYield`='{staticDict["加權平均良率"]}' , " +
                    $" `midianYield`='{staticDict["中位數"]}', `zhongShu`='{staticDict["眾數"]}', `zhongShuAvg`='{staticDict["眾數加權平均"]}', " +
                    $" `hyStartQuant`='{staticDict["總起始量"]}', `hyDoneQuant`='{staticDict["總完成量"]}', subflowCnt= '{staticDict["流程單數"]}' " +
                    $" where `RTYPE`='{dr["RTYPE"]}' and `TOLER`='{dr["TOLER"]}' and `VAL`='{dr["VAL"]}' " +
                    $" and `PPM`='{dr["PPM"]}' and `yield_Date`='{Convert.ToDateTime(dr["yield_Date"]).ToString("yyyy-MM-dd")}'";
                CommonClass.execSQLNonQuery(updStr);
                CommonClass.waitInMs(10);
            }
            MessageBox.Show("計算完成!!!");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string sqlStr = "SELECT `工令單號`,`subflowID`,`批次號`,`RTYPE`,`Tol`," +
                " `VAL`,`PPM`,`流程單良率`, `完成日期`, " +
                " 0.0 avgYield, 0.0 weightedAvgYield, 0.0 medianYield, 0.0 zungshu, " +
                " '' baseLineType ,0.0 baseLine ,0.0 Deviate  " +
                " FROM `view_basicyield` a " +
                $" where 完成日期 >= '{dtp5.Text}' and 完成日期 <= '{dtp6.Text}' " +
                $" group by `RTYPE`,`Tol`,`VAL`,`PPM` having min(流程單良率)  ";
            DataTable CompareDt = CommonClass.getSQLDataTable(sqlStr);
            DataTable dt;
            double yield, avgYield, weightedAvgYield, medianYield, zungshu;
            for (int i=0; i< CompareDt.Rows.Count; i++)
            {
                sqlStr = "SELECT `avgYield`,`weightedAvgYield`,`midianYield`,`zhongShuAvg`,yield_Date " +
                    $" FROM `mfo_yield_footprint` " +
                    $" WHERE `RTYPE`='{CompareDt.Rows[i]["RTYPE"]}' and `TOLER`='{CompareDt.Rows[i]["Tol"]}' and " +
                    $"`VAL`='{CompareDt.Rows[i]["Val"]}' and `PPM`='{CompareDt.Rows[i]["PPM"]}' and subflowCnt > 0 " +
                    $" group by `RTYPE`,`TOLER`,`VAL`,`PPM` having max(`yield_Date`) ";
                dt = CommonClass.getSQLDataTable(sqlStr);
                if (dt.Rows.Count == 0)
                    continue;
                if (CompareDt.Rows[i]["流程單良率"] == DBNull.Value)
                    continue;
                CompareDt.Rows[i]["avgYield"] = dt.Rows[0]["avgYield"];
                CompareDt.Rows[i]["weightedAvgYield"] = dt.Rows[0]["weightedAvgYield"];
                CompareDt.Rows[i]["medianYield"] = dt.Rows[0]["midianYield"];
                CompareDt.Rows[i]["zungshu"] = dt.Rows[0]["zhongShuAvg"];
                yield = Convert.ToDouble(CompareDt.Rows[i]["流程單良率"]);
                avgYield = Convert.ToDouble(dt.Rows[0]["avgYield"]);
                weightedAvgYield =  Convert.ToDouble(dt.Rows[0]["weightedAvgYield"]);
                medianYield = Convert.ToDouble(dt.Rows[0]["midianYield"]);
                zungshu = Convert.ToDouble(dt.Rows[0]["zhongShuAvg"]);
                if (zungshu > 0)
                {
                    CompareDt.Rows[i]["baseLineType"] = "眾數加權平均";
                    CompareDt.Rows[i]["baseLine"] = zungshu;
                    CompareDt.Rows[i]["Deviate"] = zungshu - yield;
                }
                else if (medianYield > 0)
                {
                    CompareDt.Rows[i]["baseLineType"] = "中位數";
                    CompareDt.Rows[i]["baseLine"] = medianYield;
                    CompareDt.Rows[i]["Deviate"] = medianYield - yield;
                }
                else
                {
                    CompareDt.Rows[i]["baseLineType"] = "加權平均良率";
                    CompareDt.Rows[i]["baseLine"] = weightedAvgYield;
                    CompareDt.Rows[i]["Deviate"] = weightedAvgYield - yield;
                }
            }
            CompareDt = CommonClass.DataTableSort(CompareDt, "Deviate");
            負乖離.Focus();
            UI_CommonClass.dtTodgv(dgv2, CompareDt);
            StringBuilder lineMsg = new StringBuilder();
            for (int i=0; i < 5; i++)
            {
                lineMsg.Append($"資料範圍：{dtp1.Text}~{dtp2.Text} {System.Environment.NewLine} " +
                $"型號：{CompareDt.Rows[i]["RTYPE"]} {System.Environment.NewLine}" +
                $"阻值：{CompareDt.Rows[i]["Val"]} {System.Environment.NewLine}" +
                $"誤差值：{CompareDt.Rows[i]["Tol"]}% {System.Environment.NewLine}" +
                $"PPM：{CompareDt.Rows[i]["PPM"]} {System.Environment.NewLine}" +
                $"完工日：{CompareDt.Rows[i]["完成日期"]} {System.Environment.NewLine}" +
                $"良率：{CompareDt.Rows[i]["流程單良率"]} {System.Environment.NewLine}" +
                $"良率基準：{CompareDt.Rows[i]["baseLineType"]} {CompareDt.Rows[i]["baseLine"]} {System.Environment.NewLine}" +
                $"乖離：{CompareDt.Rows[i]["Deviate"]} {System.Environment.NewLine}" +
                "====================");
            }
            //if (linelib == null)
            //    linelib = new LineLib();
            //linelib.pushTextMessageByWebapiAsync(comboUser.SelectedValue.ToString(), lineMsg.ToString());
            //MessageBox.Show("發送完成!!");
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
