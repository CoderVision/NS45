using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace NtccSteward.Framework
{
    public class CryptoHashProvider
    {
        public static string ComputeHash(string password, string salt)
        {
            var sha = SHA512.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var shaDigest = sha.ComputeHash(bytes);

            var rfc = new Rfc2898DeriveBytes(shaDigest, Encoding.UTF8.GetBytes(salt), 5000);
            var rfcDigest = rfc.GetBytes(512);

            var base64Digest = Convert.ToBase64String(rfcDigest, 0, rfcDigest.Length);
            return base64Digest.Substring(0, base64Digest.Length - 2);
        }
    }
}
