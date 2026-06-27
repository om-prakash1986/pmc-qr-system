using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// This class is used for doing encryption of passwords and aother stuffs
/// </summary>
public class encryption
{
    /**********************************************************
    * Purpose  ::  Encrypting passwords using custom md5 algo
    * Author   ::  Arnav
    * Date     ::  17-12-2016
    * ********************************************************/
    public string GetMD5(string text)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
        byte[] result = md5.Hash;
        StringBuilder str = new StringBuilder();
        for (int i = 1; i < result.Length; i++)
        {
            str.Append(result[i].ToString("x2"));
        }
        return str.ToString();
    }

}