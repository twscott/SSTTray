using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace firstohmWebApi.Models
{
    public class HtmlClass
    {
        //dataTable to report in HTML
        public string GenerateSimpleHtmlReport(DataTable dataTable, string reportTitle, DateTime reportDate)
        {
            StringBuilder html = new StringBuilder();

            // Add report title and date
            html.AppendLine($"<h1>{reportTitle}</h1>");
            html.AppendLine($"<p>Report Date: {reportDate.ToShortDateString()}</p>");

            // Add table headers
            html.AppendLine("<table>");
            html.AppendLine("<thead>");
            html.AppendLine("<tr>");
            foreach (DataColumn column in dataTable.Columns)
            {
                html.AppendLine($"<th>{column.ColumnName}</th>");
            }
            html.AppendLine("</tr>");
            html.AppendLine("</thead>");

            // Add table data
            html.AppendLine("<tbody>");
            foreach (DataRow row in dataTable.Rows)
            {
                html.AppendLine("<tr>");
                foreach (DataColumn column in dataTable.Columns)
                {
                    html.AppendLine($"<td>{row[column]}</td>");
                }
                html.AppendLine("</tr>");
            }
            html.AppendLine("</tbody>");
            html.AppendLine("</table>");

            return html.ToString();
        }


        //print out html to specified printer and direction
        public void PrintHtml(string html, string printerName, bool isLandscape)
        {
            // Create a web browser control to display the HTML
            WebBrowser browser = new WebBrowser();
            browser.DocumentText = html;
            browser.DocumentCompleted += (s, e) =>
            {
                // When the document is finished loading, print it
                PrintDocument document = new PrintDocument();
                document.PrinterSettings.PrinterName = printerName;
                document.DefaultPageSettings.Landscape = isLandscape;
                document.PrintPage += (s2, e2) =>
                {
                    browser.Print();
                    e2.HasMorePages = false;
                };
                document.Print();
            };
        }

        public string composeToHtml(string htmlStr)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset='UTF-8'>");
            html.AppendLine("    <title>My Report</title>");
            html.AppendLine("    <link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css' integrity='sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T' crossorigin='anonymous'>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine(htmlStr);
            html.AppendLine("</body>");
            return html.ToString();
        }

        public string GenerateHtmlReport(DataTable dataTable, List<int> columnCounts, List<Dictionary<string, string>> titleSections)
        {
            StringBuilder html = new StringBuilder();


            // Add title section
            html.AppendLine("<div class='container'>");
            for (int i = 0; i < titleSections.Count; i++)
            {
                Dictionary<string, string> section = titleSections[i];
                int columnCount = columnCounts[i];
                html.AppendLine("<div class='row'>");
                for (int j = 0; j < columnCount; j++)
                {
                    int sectionIndex = j + (i * columnCount);
                    if (sectionIndex < titleSections.Count)
                    {
                        Dictionary<string, string> sectionContent = titleSections[sectionIndex];
                        html.AppendLine("<div class='col-sm'>");
                        foreach (KeyValuePair<string, string> entry in sectionContent)
                        {
                            html.AppendLine($"<p><strong>{entry.Key}:</strong> {entry.Value}</p>");
                        }
                        html.AppendLine("</div>");
                    }
                }
                html.AppendLine("</div>");
            }
            html.AppendLine("</div>");

            // Add table section
            html.AppendLine("<div class='container'>");
            html.AppendLine("<table class='table'>");
            html.AppendLine("<thead>");
            html.AppendLine("<tr>");
            foreach (DataColumn column in dataTable.Columns)
            {
                html.AppendLine($"<th>{column.ColumnName}</th>");
            }
            html.AppendLine("</tr>");
            html.AppendLine("</thead>");
            html.AppendLine("<tbody>");
            foreach (DataRow row in dataTable.Rows)
            {
                html.AppendLine("<tr>");
                foreach (DataColumn column in dataTable.Columns)
                {
                    html.AppendLine($"<td>{row[column]}</td>");
                }
                html.AppendLine("</tr>");
            }
            html.AppendLine("</tbody>");
            html.AppendLine("</table>");
            html.AppendLine("</div>");

            // Add CSS styles
            html.AppendLine("<style>");
            html.AppendLine(".container { max-width: 800px; margin: auto; }");
            html.AppendLine("</style>");

            return html.ToString();
        }

    }
}