using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///Checking If upload is a valid upload??
/// </summary>
public class check_upload
{
    //checking if the file content type has been changed or not and is a valid file for upload.
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