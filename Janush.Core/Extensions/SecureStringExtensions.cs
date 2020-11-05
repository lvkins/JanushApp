using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Janush.Core
{
    /// <summary>
    /// The extension methods for the <see cref="SecureString"/>.
    /// </summary>
    public static class SecureStringExtensions
    {
        /// <summary>
        /// Compares two <see cref="SecureString"/> objects for equality.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool IsEqualTo(this SecureString s1, SecureString s2)
        {
            if (s1.Length != s2.Length)
            {
                return false;
            }

            var bstr1 = IntPtr.Zero;
            var bstr2 = IntPtr.Zero;
            try
            {
                bstr1 = Marshal.SecureStringToBSTR(s1);
                bstr2 = Marshal.SecureStringToBSTR(s2);
                var length1 = Marshal.ReadInt32(bstr1, -4);
                var length2 = Marshal.ReadInt32(bstr2, -4);
                if (length1 == length2)
                {
                    for (var x = 0; x < length1; ++x)
                    {
                        var b1 = Marshal.ReadByte(bstr1, x);
                        var b2 = Marshal.ReadByte(bstr2, x);
                        if (b1 != b2)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
            finally
            {
                if (bstr2 != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(bstr2);
                }

                if (bstr1 != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(bstr1);
                }
            }
        }


        /// <summary>
        /// Via <see cref="https://stackoverflow.com/a/25190648"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T GetHash<T>(this SecureString src, Func<byte[], T> func)
        {
            var bstr = IntPtr.Zero;
            byte[] workArray = null;
            GCHandle? handle = null;
            try
            {
                /*** PLAINTEXT EXPOSURE BEGINS HERE ***/
                bstr = Marshal.SecureStringToBSTR(src);
                unsafe
                {
                    var bstrBytes = (byte*)bstr;
                    workArray = new byte[src.Length * 2];
                    handle = GCHandle.Alloc(workArray, GCHandleType.Pinned);
                    for (var i = 0; i < workArray.Length; i++)
                    {
                        workArray[i] = *bstrBytes++;
                    }
                }

                return func(workArray);
            }
            finally
            {
                if (workArray != null)
                {
                    for (var i = 0; i < workArray.Length; i++)
                    {
                        workArray[i] = 0;
                    }
                }

                handle?.Free();
                if (bstr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(bstr);
                }
                /*** PLAINTEXT EXPOSURE ENDS HERE ***/
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static (byte[], byte[]) RNGCryptoEncrypt(this SecureString password)
        {
            // Generate additional entropy (will be used as the Initialization vector)
            var entropy = new byte[20];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }

            // Return entropy along with the Unicode encoded cipher text
            return (entropy, password.GetHash(barr => ProtectedData.Protect(Encoding.Unicode.GetBytes(
                Encoding.Unicode.GetString(barr)), entropy, DataProtectionScope.CurrentUser)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="data"></param>
        /// <param name="entropy"></param>
        /// <returns></returns>
        public static SecureString RNGCryptoDecrypt(this SecureString src, byte[] data, byte[] entropy)
        {
            try
            {
                src.Clear();

                // We rely on decrypted representations of data stored in byte or char arrays
                // so that we can easily clear them out when done in contrast to strings that
                // can't be changed and they stay in memory until they are garbage collected
                // at some indeterminate future time.

                var decryptedData = Encoding.Unicode.GetChars(ProtectedData.Unprotect(
                    data,
                    entropy,
                    DataProtectionScope.CurrentUser));

                // Append to SecureString
                for (var i = 0; i < decryptedData.Length; i++)
                {
                    if (decryptedData[i] != '\0')
                    {
                        src.AppendChar(decryptedData[i]);
                    }
                }

                // Clear data from memory as soon as possible
                Array.Clear(decryptedData, 0, decryptedData.Length);

                // Seal the SecureString
                src.MakeReadOnly();

                return src;
            }
            catch
            {
                return new SecureString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToInsecureString(this SecureString input)
        {
            return new NetworkCredential("", input).Password;
        }
    }
}
