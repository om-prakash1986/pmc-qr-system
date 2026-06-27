using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PMC.Common
{
    /// <summary>
    /// Summary description for check_string
    /// </summary>
    public class check_string
    {
        //checking all user input 
        public string check_user_input(string stra)
        {
            string str = stra;
            // Create  a string array and add the special characters you want to remove
            string[] chars = new string[] { ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "(", ")", ":", "|", "[", "]" };
            //Iterate the number of times based on the String array length.
            for (int i = 0; i < chars.Length; i++)
            {
                if (str.Contains(chars[i]))
                {
                    str = str.Replace(chars[i], "");
                }
            }
            return str;
        }

        //allow email id
        public string check_user_email_id(string stra)
        {
            string str = stra;
            string[] chars = new string[] { ",", "/", "!", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "(", ")", ":", "|", "[", "]" };
            //Iterate the number of times based on the String array length.
            for (int i = 0; i < chars.Length; i++)
            {
                if (str.Contains(chars[i]))
                {
                    str = str.Replace(chars[i], "");
                }
            }
            return str;
        }
    }
}