using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace FirstOhm
{
    class LibChart
    {
        public void resetChart(Chart chart1)
        {
            while (chart1.Series.Count > 0) { chart1.Series.RemoveAt(0); }
        }

        //折線圖, 可畫多條線
        //x 軸 固定為 0,1,2,3,4...
        //用 兩個 List 畫出線圖, chartData:y軸, lineTitles:x軸
        //https://stackoverflow.com/questions/10648828/see-values-of-chart-points-when-the-mouse-is-on-points
        public string draw_chart(Chart chart1, List<int[]> chartData, string title, 
            List<string> lineTitles, int maxNumber, SeriesChartType chartType)
        {
            Random rnd = new Random();
            string rtnStr = null;
            if (chartData.Count != lineTitles.Count)
                return "List 內容數量不符";
            List<Series> seriesList = new List<Series>();
            for (int i = 0; i < lineTitles.Count; i++)
                seriesList.Add(new Series());
            //標題 最大數值
            try
            {
                int currintIdx = 0;
                foreach (Series series in seriesList)
                {
                    //設定線條顏色
                    series.Color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    //設定字型
                    series.Font = new System.Drawing.Font("標楷體", 14);
                    //折線圖
                    //series2.ChartType = SeriesChartType.Line;
                    series.ChartType = chartType;
                    //將數值顯示在線上
                    series.IsValueShownAsLabel = true;
                    //將數值新增至序列
                    for (int index = 0; index < chartData[currintIdx].Length; index++)
                    {
                        series.Points.AddXY(index, chartData[currintIdx][index]);
                    }
                    chart1.Series.Add(series);
                    currintIdx++;
                }
                //標題
                chart1.Titles.Add(title);
            }
            catch (Exception ex)
            {
                rtnStr = ex.StackTrace;
            }
            return rtnStr;
        }

        //折線圖, 可畫多條線
        //x 軸由xChartData 的內容決定
        public string draw_chart2(Chart chart1, List<int[]> yChartData, List<int[]> xChartData, string title,
    List<string> lineTitles, int maxNumber, SeriesChartType chartType)
        {
            Random rnd = new Random();
            string rtnStr = null;
            if (yChartData.Count != lineTitles.Count)
                return "List 內容數量不符";
            List<Series> seriesList = new List<Series>();

            for (int i = 0; i < lineTitles.Count; i++)
                seriesList.Add(new Series());
            //標題 最大數值
            try
            {
                int currintIdx = 0;
                foreach (Series series in seriesList)
                {
                    //設定線條顏色
                    series.Color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    //設定字型
                    series.Font = new System.Drawing.Font("標楷體", 14);
                    //折線圖
                    //series2.ChartType = SeriesChartType.Line;
                    series.ChartType = chartType;
                    //將數值顯示在線上
                    series.IsValueShownAsLabel = false;
                    series.Name = lineTitles[currintIdx];
                    //將數值新增至序列
                    for (int index = 0; index < xChartData[currintIdx].Length; index++)
                    {
                        series.Points.AddXY(xChartData[currintIdx][index], yChartData[currintIdx][index]);
                    }
                    chart1.Series.Add(series);
                    currintIdx++;
                }
                //標題
                //chart1.Titles.Add(title);
            }
            catch (Exception ex)
            {
                rtnStr = ex.StackTrace;
            }
            return rtnStr;
        }

        //charRow 為 每一條線的名字
        //[charRow,1,2,3,4,5]
        //[charRow,2,3,1,2,3]....
        public void lineChatByDt(Chart Chart1, DataTable dt, string charRow)
        {
            double YVal = 0;
            resetChart(Chart1);
            foreach (DataRow row in dt.Rows)
            {
                // For each Row add a new series
                //string seriesName = row["SalesRep"].ToString();
                string seriesName = row[charRow].ToString();
                Chart1.Series.Add(seriesName);
                //Chart1.Series[seriesName].ChartType = SeriesChartType.Line;
                //Chart1.Series[seriesName].BorderWidth = 2;
                foreach (Series series in Chart1.Series)
                {
                    series.ChartType = SeriesChartType.Line;
                    series.BorderWidth = 2;
                }
                for (int colIndex = 1; colIndex < dt.Columns.Count; colIndex++)
                {
                    // For each column (column 1 and onward) add the value as a point
                    string columnName = dt.Columns[colIndex].ColumnName;
                    if (row[columnName] == DBNull.Value)
                        YVal = 0;
                    else
                        YVal = Convert.ToDouble(row[columnName]);

                    Chart1.Series[seriesName].Points.AddXY(columnName, YVal);
                }
            }
        }
        //charRow 為 每一條線的名字
        //[charRow,1,2,3,4,5]
        //[charRow,2,3,1,2,3]....
        public void barChatByDt(Chart Chart1, DataTable dt, string charRow)
        {
            int YVal = 0;
            resetChart(Chart1);
            foreach (DataRow row in dt.Rows)
            {
                // For each Row add a new series
                //string seriesName = row["SalesRep"].ToString();
                string seriesName = row[charRow].ToString();
                Chart1.Series.Add(seriesName);
                //Chart1.Series[seriesName].BorderWidth = 2;
                foreach (Series series in Chart1.Series)
                {
                    series.ChartType = SeriesChartType.Bar;
                    series.BorderWidth = 2;
                }
                for (int colIndex = 1; colIndex < dt.Columns.Count; colIndex++)
                {
                    // For each column (column 1 and onward) add the value as a point
                    string columnName = dt.Columns[colIndex].ColumnName;
                    if (row[columnName] == DBNull.Value)
                        YVal = 0;
                    else
                        YVal = Convert.ToInt32(row[columnName]);

                    Chart1.Series[seriesName].Points.AddXY(columnName, YVal);
                }
            }
        }
    }

}
