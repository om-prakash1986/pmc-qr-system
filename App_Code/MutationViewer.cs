using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using System.Web;
using PMC.DAL;
using System.Web.UI.WebControls;
using System.Web.UI;

public class MutationViewer
{
    public void ViewMore(string queryParameter, Control parentControl, string placeholderId)
    {
        string viewMoreID = HttpContext.Current.Request.QueryString[queryParameter];
        DataTable dt;
        List<SqlParameter> param;
        DataAccessLayer dac;
        string data = "";
        StringBuilder sb;
        dt = new DataTable();
        sb = new StringBuilder();
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PTax"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                string selectQuery = @"SELECT mut.id,mut.pid,mut.Mutation_date,mut.property_area,mut.App_date, mut.token_no,app.guardian_name, app.name,app.mobile,app.address,app.aadhar_no,
                      app.applicante_type, mut.mutation_type, mut.status,mut.Status_update_date,mut.mobile_no,
                    mut.sale_dead_file, mut.waste_receipt_file1, mut.death_cert_file, 
                    mut.family_tree_file, mut.holding_file,
                    mut.noc_file, mut.waste_receipt_file2, mut.court_order_file, 
                    mut.family_tree, mut.Death_file
                    FROM tbl_mutation_form AS mut
                    JOIN tbl_applicant AS app
                    ON mut.id = app.mutation_id WHERE mut.id = @id";

                dac = new DataAccessLayer();
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@id", viewMoreID));
                dt = dac.GetDataTable(selectQuery, param);
                data = "";
                sb = new StringBuilder();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        sb.Append("<tr>");
                        sb.Append("<th>PId</th>");
                        sb.Append("<td>" + dt.Rows[i]["pid"].ToString() + "</td>");
                        sb.Append("<th>Acknowledgement No</th>");
                        sb.Append("<td>" + dt.Rows[i]["token_no"].ToString() + "</td>");
                        sb.Append("<th>Applicant Name</th>");
                        sb.Append("<td>" + dt.Rows[i]["name"].ToString() + "</td>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<th>Applicant Gaurdian Name</th>");
                        sb.Append("<td>" + dt.Rows[i]["guardian_name"].ToString() + "</td>");
                        sb.Append("<th>Applicant Address</th>");
                        sb.Append("<td>" + dt.Rows[i]["address"].ToString() + "</td>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<th>Applicant Mobile No</th>");
                        sb.Append("<td>" + dt.Rows[i]["mobile_no"].ToString() + "</td>");
                        sb.Append("<th>Applicant Aadhar No</th>");
                        sb.Append("<td>" + dt.Rows[i]["aadhar_no"].ToString() + "</td>");
                        sb.Append("<th>Mutation Type</th>");
                        sb.Append("<td>" + dt.Rows[i]["mutation_type"].ToString() + "</td>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<th>Mutation Date</th>");
                        sb.Append("<td>" + dt.Rows[i]["Mutation_date"].ToString() + "</td>");
                        sb.Append("<th>Property Area(Sq Ft)</th>");
                        sb.Append("<td>" + dt.Rows[i]["property_area"].ToString() + "</td>");
                        sb.Append("<th>Application Date</th>");
                        sb.Append("<td>" + dt.Rows[i]["App_date"].ToString() + "</td>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<th>Status</th>");
                        sb.Append("<td>" + dt.Rows[i]["status"].ToString() + "</td>");
                        sb.Append("<th>Status Update Date</th>");
                        sb.Append("<td>" + dt.Rows[i]["Status_update_date"].ToString() + "</td>");
                        sb.Append("</tr>");

                        if (!string.IsNullOrEmpty(dt.Rows[i]["sale_dead_file"].ToString()))
                        {
                            sb.Append("<tr>");
                            sb.Append("<th>Sale Dead File</th>");

                            sb.Append("<td>");
                            string filePath = dt.Rows[i]["sale_dead_file"].ToString();
                            string basePath = "C:\\Users\\hp\\OneDrive\\Desktop\\PMC New\\document\\";
                            string relativePath = filePath.Replace(basePath, "");
                            string finalpath = "/document/" + relativePath + "SaleDeed.pdf";
                            sb.Append("<button type='button' class='btn btn-primary' onclick='openPdf(event, \"" + finalpath + "\")'>View Document</button>");
                            sb.Append("</td>");
                            sb.Append("<th>Sale Deed Verified</th>");
                            sb.Append("<td colspan='3'>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio1_" + i + "' name='radioGroup1' value='Sale Deed Verified' />");
                            sb.Append("Click Check Box if <b>Sale Deed</b> Is Verified.");
                            sb.Append("</p>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio2_" + i + "' name='radioGroup1' value='Sale Deed Not Verified' />");
                            sb.Append("Click Check Box if <b>Sale Deed</b> Is Not Verified.");
                            sb.Append("</p>");
                            sb.Append("<label for='TextArea1_" + i + "'>Remarks</label><br />");
                            sb.Append("<textarea id='TextArea1_" + i + "' rows='2' name='TextArea1_0' cols='50'></textarea>");
                            sb.Append("</td>");
                            sb.Append("</tr>");
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[i]["waste_receipt_file1"].ToString()))
                        {
                            sb.Append("<tr>");
                            sb.Append("<th>Waste Receipt File</th>");
                            // Append HTML buttons within the same <tr>
                            sb.Append("<td>");
                            string filePath = dt.Rows[i]["waste_receipt_file1"].ToString();
                            string basePath = "C:\\Users\\hp\\OneDrive\\Desktop\\PMC New\\document\\";
                            string relativePath = filePath.Replace(basePath, "");
                            string finalpath = "/document/" + relativePath + "WasteReceipt.pdf";
                            sb.Append("<button type='button' class='btn btn-primary' onclick='openPdf(event, \"" + finalpath + "\")'>View Document</button>");
                            sb.Append("</td>");
                            sb.Append("<th>Verification</th>");
                            sb.Append("<td colspan='3'>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio3_" + i + "' name='radioGroup2' value='Waste Receipt Verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Completed.");
                            sb.Append("</p>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio4_" + i + "' name='radioGroup2' value='Waste Receipt Not Verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Not Completed.");
                            sb.Append("</p>");
                            sb.Append("<label for='TextArea_" + i + "'>Remarks</label><br />");
                            sb.Append("<textarea id='TextArea2_" + i + "' name='wastereceipt' rows='2' cols='50'></textarea>");
                            sb.Append("</td>");
                            sb.Append("</tr>");
                        }

                        if (!string.IsNullOrEmpty(dt.Rows[i]["death_cert_file"].ToString()))
                        {
                            sb.Append("<tr>");
                            sb.Append("<th>Death Certificate File</th>");
                            sb.Append("<td>");
                            string filePath = dt.Rows[i]["death_cert_file"].ToString();
                            string basePath = "C:\\Users\\hp\\OneDrive\\Desktop\\PMC New\\document\\";
                            string relativePath = filePath.Replace(basePath, "");
                            string finalpath = "/document/" + relativePath + "SaleDeed.pdf";
                            sb.Append("<button type='button' class='btn btn-primary' onclick='openPdf(event, \"" + finalpath + "\")'>View Document</button>");
                            sb.Append("</td>");
                            sb.Append("<th>Verification</th>");
                            sb.Append("<td colspan='3'>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio5_" + i + "' name='radioGroup3' value='death cert. verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Completed.");
                            sb.Append("</p>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio6_" + i + "' name='radioGroup3' value='death cert. not  verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Not Completed.");
                            sb.Append("</p>");
                            sb.Append("<label for='TextArea_" + i + "'>Remarks</label><br />");
                            sb.Append("<textarea id='TextArea3_" + i + "' rows='2' name='death' cols='50'></textarea>");
                            sb.Append("</td>");
                            sb.Append("</tr>");
                        }

                        if (!string.IsNullOrEmpty(dt.Rows[i]["family_tree_file"].ToString()))
                        {
                            sb.Append("<tr>");
                            sb.Append("<th>Family Tree File</th>");
                            sb.Append("<td>");
                            string filePath = dt.Rows[i]["family_tree_file"].ToString();
                            string basePath = "C:\\Users\\hp\\OneDrive\\Desktop\\PMC New\\document\\";
                            string relativePath = filePath.Replace(basePath, "");
                            string finalpath = "/document/" + relativePath + "SaleDeed.pdf";
                            sb.Append("<button type='button' class='btn btn-primary' onclick='openPdf(event, \"" + finalpath + "\")'>View Document</button>");
                            sb.Append("</td>");
                            sb.Append("<th>Verification</th>");
                            sb.Append("<td colspan='3'>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio7_" + i + "' name='radioGroup4' value='Family Tree verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Completed.");
                            sb.Append("</p>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio8_" + i + "' name='radioGroup4' value='Family tree not verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Not Completed.");
                            sb.Append("</p>");
                            sb.Append("<label for='TextArea_" + i + "'>Remarks</label><br />");
                            sb.Append("<textarea id='TextArea4_" + i + "' rows='2' name='familytree' cols='50'></textarea>");
                            sb.Append("</td>");
                            sb.Append("</tr>");
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[i]["holding_file"].ToString()))
                        {
                            sb.Append("<tr>");
                            sb.Append("<th>Holding File</th>");
                            sb.Append("<td style='display:none;'>" + dt.Rows[i]["holding_file"].ToString() + "</td>");
                            // Append HTML buttons within the same <tr>
                            sb.Append("<td>");
                            sb.Append("<button type='button' class='btn btn-primary' onclick='openPdf" + i + "(event)' data-file='" + dt.Rows[i]["holding_file"].ToString() + "'>View Document</button>");
                            sb.Append("</td>");
                            sb.Append("<th>Verification</th>");
                            sb.Append("<td colspan='3'>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio9_" + i + "' name='radioGroup5' value='Holding file verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Completed.");
                            sb.Append("</p>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio10_" + i + "' name='radioGroup5' value= 'Holding file not verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Not Completed.");
                            sb.Append("</p>");
                            sb.Append("<label for='TextArea_" + i + "'>Remarks</label><br />");
                            sb.Append("<textarea id='TextArea5_" + i + "' rows='2' name='holding' cols='50'></textarea>");
                            sb.Append("</td>");
                            sb.Append("</tr>");
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[i]["noc_file"].ToString()))
                        {
                            sb.Append("<tr>");
                            sb.Append("<th>NOC File</th>");
                            sb.Append("<td style='display:none;'>" + dt.Rows[i]["noc_file"].ToString() + "</td>");
                            // Append HTML buttons within the same <tr>
                            sb.Append("<td>");
                            sb.Append("<button type='button' class='btn btn-primary' onclick='openPdf" + i + "(event)' data-file='" + dt.Rows[i]["noc_file"].ToString() + "'>View Document</button>");
                            sb.Append("</td>");
                            sb.Append("<th>Verification</th>");
                            sb.Append("<td colspan='3'>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio11_" + i + "' name='radioGroup6' value='Noc file verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Completed.");
                            sb.Append("</p>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio12_" + i + "' name='radioGroup6' value='Noc file not verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Not Completed.");
                            sb.Append("</p>");
                            sb.Append("<label for='TextArea_" + i + "'>Remarks</label><br />");
                            sb.Append("<textarea id='TextArea6_" + i + "' rows='2' name='noc' cols='50'></textarea>");
                            sb.Append("</td>");
                            sb.Append("</tr>");
                        }

                        if (!string.IsNullOrEmpty(dt.Rows[i]["waste_receipt_file2"].ToString()))
                        {
                            sb.Append("<tr>");
                            sb.Append("<th>Waste Receipt File 2</th>");
                            sb.Append("<td>");
                            string filePath = dt.Rows[i]["waste_receipt_file2"].ToString();
                            string basePath = "C:\\Users\\hp\\OneDrive\\Desktop\\PMC New\\document\\";
                            string relativePath = filePath.Replace(basePath, "");
                            string finalpath = "/document/" + relativePath + "SaleDeed.pdf";
                            sb.Append("<button type='button' class='btn btn-primary' onclick='openPdf" + i + "(event)' data-file='" + dt.Rows[i]["waste_receipt_file2"].ToString() + "'>View Document</button>");
                            sb.Append("</td>");
                            sb.Append("<th>Verification</th>");
                            sb.Append("<td colspan='3'>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio13_" + i + "' name='radioGroup7' value='Waste Receipt file verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Completed.");
                            sb.Append("</p>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio14_" + i + "' name='radioGroup7' value='Waste Receipt file not verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Not Completed.");
                            sb.Append("</p>");
                            sb.Append("<label for='TextArea_" + i + "'>Remarks</label><br />");
                            sb.Append("<textarea id='TextArea7_" + i + "' rows='2' name='swm' cols='50'></textarea>");
                            sb.Append("</td>");
                            sb.Append("</tr>");
                        }

                        if (!string.IsNullOrEmpty(dt.Rows[i]["court_order_file"].ToString()))
                        {
                            sb.Append("<tr>");
                            sb.Append("<th>Court Order File</th>");
                            sb.Append("<td>");
                            string filePath = dt.Rows[i]["court_order_file"].ToString();
                            string basePath = "C:\\Users\\hp\\OneDrive\\Desktop\\PMC New\\document\\";
                            string relativePath = filePath.Replace(basePath, "");
                            string finalpath = "/document/" + relativePath + "SaleDeed.pdf";
                            sb.Append("<button type='button' class='btn btn-primary' onclick='openPdf(event, \"" + finalpath + "\")'>View Document</button>");
                            sb.Append("</td>");
                            sb.Append("<th>Verification</th>");
                            sb.Append("<td colspan='3'>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio15_" + i + "' name='radioGroup8' value='court order file verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Completed.");
                            sb.Append("</p>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio16_" + i + "' name='radioGroup8' value='court order file not verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Not Completed.");
                            sb.Append("</p>");
                            sb.Append("<label for='TextArea_" + i + "'>Remarks</label><br />");
                            sb.Append("<textarea id='TextArea8_" + i + "' rows='2' name='courtorder' cols='50'></textarea>");
                            sb.Append("</td>");
                            sb.Append("</tr>");
                        }

                        if (!string.IsNullOrEmpty(dt.Rows[i]["family_tree"].ToString()))
                        {
                            sb.Append("<tr>");
                            sb.Append("<th>Family Tree</th>");
                            sb.Append("<td>");
                            string filePath = dt.Rows[i]["family_tree"].ToString();
                            string basePath = "C:\\Users\\hp\\OneDrive\\Desktop\\PMC New\\document\\";
                            string relativePath = filePath.Replace(basePath, "");
                            string finalpath = "/document/" + relativePath + "SaleDeed.pdf";
                            sb.Append("<button type='button' class='btn btn-primary' onclick='openPdf(event, \"" + finalpath + "\")'>View Document</button>");
                            sb.Append("</td>");
                            sb.Append("<button type='button' class='btn btn-primary' onclick='openPdf" + i + "(event)' data-file='" + dt.Rows[i]["family_tree"].ToString() + "'>View Document</button>");
                            sb.Append("</td>");
                            sb.Append("<th>Verification</th>");
                            sb.Append("<td colspan='3'>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio17_" + i + "' name='radioGroup9' value='family tree file verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Completed.");
                            sb.Append("</p>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio18_" + i + "' name='radioGroup9' value='family tree file not verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Not Completed.");
                            sb.Append("</p>");
                            sb.Append("<label for='TextArea_" + i + "'>Remarks</label><br />");
                            sb.Append("<textarea id='TextArea9_" + i + "' rows='2' name='familyroad1' cols='50'></textarea>");
                            sb.Append("</td>");
                            sb.Append("</tr>");
                        }

                        if (!string.IsNullOrEmpty(dt.Rows[i]["Death_file"].ToString()))
                        {
                            sb.Append("<tr>");
                            sb.Append("<th>Death File</th>");
                            sb.Append("<td>");
                            string filePath = dt.Rows[i]["Death_file"].ToString();
                            string basePath = "C:\\Users\\hp\\OneDrive\\Desktop\\PMC New\\document\\";
                            string relativePath = filePath.Replace(basePath, "");
                            string finalpath = "/document/" + relativePath + "SaleDeed.pdf";
                            sb.Append("<button type='button' class='btn btn-primary' onclick='openPdf(event, \"" + finalpath + "\")'>View Document</button>");
                            sb.Append("</td>");
                            sb.Append("<button type='button' class='btn btn-primary' onclick='openPdf" + i + "(event)' data-file='" + dt.Rows[i]["Death_file"].ToString() + "'>View Document</button>");
                            sb.Append("</td>");
                            sb.Append("<th>Verification</th>");
                            sb.Append("<td colspan='3'>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio19_" + i + "' name='radioGroup10' value = 'death file verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Completed.");
                            sb.Append("</p>");
                            sb.Append("<p>");
                            sb.Append("<input type='radio' id='radio20_" + i + "' name='radioGroup10' value = 'death file not verified' />");
                            sb.Append("Click Check Box if <b>Verification</b> Is Not Completed.");
                            sb.Append("</p>");
                            sb.Append("<label for='TextArea_" + i + "'>Remarks</label><br />");
                            sb.Append("<textarea id='TextArea10_" + i + "' rows='2' name ='courtdeath' cols='50'></textarea>");
                            sb.Append("</td>");
                            sb.Append("</tr>");
                        }

                        
                    }

                    data = sb.ToString();

                    // Find the placeholder control using the provided ID
                    Control placeholderControl = parentControl.FindControl(placeholderId);

                    // Add the Literal control to the placeholder
                    if (placeholderControl != null && placeholderControl is PlaceHolder)
                    {
                        ((PlaceHolder)placeholderControl).Controls.Add(new Literal { Text = data });
                    }
                    else
                    {
                        // Handle the case where the placeholder with the provided ID is not found
                        throw new InvalidOperationException("Placeholder control not found with the provided ID.");
                    }
                }
                else
                {
                    sb.Append("<tr><td colspan='6'>No record found</td></tr>");
                }
            }
        }
        catch (Exception error)
        {
            // Handle the exception
        }
    }
}

// Example usage in another part of your application
//MutationViewer mutationViewer = new MutationViewer();
//mutationViewer.ViewMore();
