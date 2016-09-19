using System;
using System.Text;

namespace API.Helpers
{
    public class StringHelper
    {
        public static string Base64ForUrlEncode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64ForUrlDecode(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception)
            {

                return  "";
            }
            
        }
        //public static string Base64ForUrlEncode(string str)
        //{
        //    var encbuff = Encoding.UTF8.GetBytes(str);
        //    return HttpServerUtility.UrlTokenEncode(encbuff);
        //}

        //public static string Base64ForUrlDecode(string str)
        //{
        //    var decbuff = HttpServerUtility.UrlTokenDecode(str);
        //    return decbuff != null ? Encoding.UTF8.GetString(decbuff) : null;
        //}
    }
}