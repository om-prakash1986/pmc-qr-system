using System;
using System.Configuration;
using System.Web;
using System.Web.UI;

public partial class CitizenTaxPortal : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // All initialization is handled client-side via AJAX calls to backend handlers
    }

    /// <summary>
    /// Fetches the Razorpay API Key from Web.config.
    /// Falls back to a standard test key if not configured.
    /// </summary>
    /// <returns>Razorpay Key ID string</returns>
    public string GetRazorpayKey()
    {
        string key = ConfigurationManager.AppSettings["RazorpayKeyId"];
        if (string.IsNullOrEmpty(key))
        {
            return "rzp_test_DwNn8bSgBfVskq";
        }
        return key;
    }
}
