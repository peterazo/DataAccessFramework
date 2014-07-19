using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace DAF
{
    /// <summary>
    /// This class provides security related information as user name and password on the time of session creation.
    /// </summary>
    /// <remarks>
    /// 1. Only one security token type register per session kind.
    /// 2. The token expires on session disposal.
    /// </remarks>
    public class SecurityToken : ISecurityToken
    {
        public SecurityToken()
        {
            this.IsAuthenticated = false;
            this.User = string.Empty;
            this.Password = string.Empty;
        }

        public bool IsAuthenticated { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        static public string HashString(string inputString, string hashName)
        {
            HashAlgorithm algorithm = HashAlgorithm.Create(hashName);
            if (algorithm == null)
            {
                throw new ArgumentException("Unrecognized hash name", "hashName");
            }
            byte[] hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("x2").ToUpperInvariant());
            }
            return result.ToString();
        }

        public static string Encrypt(string text, Guid salt)
        {
            byte[] keyArray = salt.ToByteArray();
            byte[] textArray = UTF8Encoding.UTF8.GetBytes(text);

            for (int i = 0; i < textArray.Length; ++i)
            {
                textArray[i] ^= keyArray[i % keyArray.Length];
            }

            return Convert.ToBase64String(textArray);
        }

        public static string Decrypt(string text, Guid salt)
        {
            byte[] keyArray = salt.ToByteArray();
            byte[] textArray = Convert.FromBase64String(text);

            for (int i = 0; i < textArray.Length; ++i)
            {
                textArray[i] ^= keyArray[i % keyArray.Length];
            }

            return UTF8Encoding.UTF8.GetString(textArray, 0, textArray.Length);
        }
    }
}
