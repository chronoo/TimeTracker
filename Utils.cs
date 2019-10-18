using System;

namespace TimeTracker
{
    public class Utils
    {
        public static string Base64Encode(string plainText)
        {
            byte[] byt = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(byt);
        }
    }
}
