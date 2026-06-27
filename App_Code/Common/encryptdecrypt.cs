using System;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Summary description for encryptdecrypt
/// </summary>
public class encryptdecrypt
{
	public encryptdecrypt()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    string Cipher = "aes-256-ctr";
    string EncryptionKey = "1f115e6022acb1d567742447a7de71fc"; //you can change key by call the function GetKey()

    private readonly string Ciphering = "AES-128-CTR";
    private readonly int Options = 0;

    /* AES Authenticated Encryption in CTR mode
     * To Encrypt plain text to cipher text Call encrypt funtion 
    */
    public string Encrypt(string plainText, string type = "url")
    {
        //string base64EncodedBytes = System.Convert.FromBase64String("HxFeYCKssdVndCRHp95x/A==");
        //EncryptionKey = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        byte[] key = System.Convert.FromBase64String("HxFeYCKssdVndCRHp95x/A==");//HexStringToByteArray(EncryptionKey);
        int nonceSize = Cipher == "aes-256-ctr" ? 16 : 8;
        byte[] nonce = new byte[nonceSize];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(nonce);
        }
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        using (var cipher = new AesCryptoServiceProvider())
        {
            cipher.Mode = CipherMode.ECB;
            cipher.Padding = PaddingMode.PKCS7; // Use PKCS7 padding
            cipher.KeySize = key.Length * 8;
            cipher.Key = key;
            cipher.IV = nonce;
            ICryptoTransform encryptor = cipher.CreateEncryptor(cipher.Key, cipher.IV);
            byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            byte[] encryptedBytes = new byte[nonce.Length + cipherBytes.Length];
            Buffer.BlockCopy(nonce, 0, encryptedBytes, 0, nonce.Length);
            Buffer.BlockCopy(cipherBytes, 0, encryptedBytes, nonce.Length, cipherBytes.Length);
            string encryptedText = Convert.ToBase64String(encryptedBytes);
            if (type == "url")
                return Uri.EscapeDataString(encryptedText);
            else
                return encryptedText;
        }


    }
    /* AES Authenticated Decryption in CTR mode
	 * To Decrypt cipher text to plain text Call decrypt funtion 
	*/
	
    private byte[] HexStringToByteArray(string hex)
    {
        byte[] byteArray = new byte[hex.Length / 2];
        for (int i = 0; i < byteArray.Length; i++)
        {
            byteArray[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return byteArray;
    }
    public string EncodePasswordToBase64(string password)
    {
        try
        {
            byte[] encData_byte = new byte[password.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
            string encodedData = Convert.ToBase64String(encData_byte);
            return encodedData;
        }
        catch (Exception ex)
        {
            throw new Exception("Error in base64Encode" + ex.Message);
        }
    }
    //this function Convert to Decord your Password
    public string DecodeFrom64(string encodedData)
    {
        System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
        System.Text.Decoder utf8Decode = encoder.GetDecoder();
        byte[] todecode_byte = Convert.FromBase64String(encodedData);
        int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
        char[] decoded_char = new char[charCount];
        utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
        string result = new String(decoded_char);
        return result;
    }
}