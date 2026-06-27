using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.IO;


namespace PMC.Common
{
    /// <summary>
    /// Summary description for create_charts
    /// </summary>
    public class create_charts
    {
        string strcon = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
        /************************************************************
         * Purpose  ::  Create Ward Wise Bar Chart
         * Author   ::  Arnav
         * Date     ::  24-03-2017
         * ********************************************************/
        public string create_barchart_for_all_wards()
        {
            string data = "";
            StringBuilder sb = new StringBuilder();
            sb.Append("<script type='text/javascript'>");
            sb.Append("google.charts.load('current', { 'packages': ['corechart'] });");
            sb.Append("google.charts.setOnLoadCallback(drawVisualization);");
            sb.Append("function drawVisualization() {");
            // Some raw data (nsb = not necessarily accurate)
            sb.Append("var data = google.visualization.arrayToDataTable([");
            sb.Append("['Ward', 'allcomplains', 'AT SI', 'AT CSI', 'Closed', 'Over Time', 'Average'],");
            sb.Append("['ward1', 165, 938, 522, 998, 450, 614.6],");
            sb.Append("['ward2', 135, 1120, 599, 1268, 288, 682],");
            sb.Append("['ward3', 157, 1167, 587, 807, 397, 623],");
            sb.Append("['ward4', 139, 1110, 615, 968, 215, 609.4],");
            sb.Append("['ward5', 136, 691, 629, 1026, 366, 569.6],");

            sb.Append("['ward6', 165, 938, 522, 998, 450, 614.6],");
            sb.Append("['ward7', 135, 1120, 599, 1268, 288, 682],");
            sb.Append("['ward8', 157, 1167, 587, 807, 397, 623],");
            sb.Append("['ward9', 139, 1110, 615, 968, 215, 609.4],");
            sb.Append("['ward10', 136, 691, 629, 1026, 366, 569.6]");

            sb.Append("]);");

            sb.Append("var options = {");
            sb.Append("title: 'Ward Wise Complain Details',");
            sb.Append("vAxis: { title: 'Complaints' },");
            sb.Append("hAxis: { title: 'Ward' },");
            sb.Append("seriesType: 'bars',");
            sb.Append("series: { 5: { type: 'line'} }");
            sb.Append("};");

            sb.Append("var chart = new google.visualization.ComboChart(document.getElementById('chart_div'));");
            sb.Append("chart.draw(data, options);");
            sb.Append("}");
            sb.Append("</script>");
            data = sb.ToString();
            return data;
        }


        public string create_barchart_for_all_wards_between_dates()
        {
            string data = "";
            StringBuilder sb = new StringBuilder();
            sb.Append("<script type='text/javascript'>");
            sb.Append("google.charts.load('current', { 'packages': ['corechart'] });");
            sb.Append("google.charts.setOnLoadCallback(drawVisualization);");
            sb.Append("function drawVisualization() {");
            // Some raw data (nsb = not necessarily accurate)
            sb.Append("var data = google.visualization.arrayToDataTable([");
            sb.Append("['Ward', 'allcomplains', 'AT SI', 'AT CSI', 'Closed', 'Over Time', 'Average'],");
            sb.Append("['ward1', 165, 938, 522, 998, 450, 614.6],");
            sb.Append("['ward2', 135, 1120, 599, 1268, 288, 682],");
            sb.Append("['ward3', 157, 1167, 587, 807, 397, 623],");
            sb.Append("['ward4', 139, 1110, 615, 968, 215, 609.4],");
            sb.Append("['ward5', 136, 691, 629, 1026, 366, 569.6]");
            sb.Append("]);");

            sb.Append("var options = {");
            sb.Append("title: 'Ward Wise Complain Details',");
            sb.Append("vAxis: { title: 'Complaints' },");
            sb.Append("hAxis: { title: 'Ward' },");
            sb.Append("seriesType: 'bars',");
            sb.Append("series: { 5: { type: 'line'} }");
            sb.Append("};");

            sb.Append("var chart = new google.visualization.ComboChart(document.getElementById('chart_div'));");
            sb.Append("chart.draw(data, options);");
            sb.Append("}");
            sb.Append("</script>");
            data = sb.ToString();
            return data;
        }

        /***********************************************************
         * Till Here
         * *********************************************************/
    }
}