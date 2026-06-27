using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class deed_checkupload
{
    public int is_valid_upload(string file_name)
    {
        file_name = file_name.Trim();
        int number = 0;
        string[] k = file_name.Split('.');

        if (k.Length == 2)
        {
            number = 1;
        }
        return number;
    }
}
