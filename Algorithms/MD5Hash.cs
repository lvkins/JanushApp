using System.Security.Cryptography;
using System.Text;

namespace PromoSeeker.Algorithms
{
    /// <summary>
    /// MD5 cryptography hash helper methods.
    /// </summary>
    public static class MD5Hash
    {
        public static string FromString(string input)
        {
            using (var md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                var data = md5Hash.ComputeHash(Encoding.ASCII.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes and create a string.
                var sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (var i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }
    }
}
