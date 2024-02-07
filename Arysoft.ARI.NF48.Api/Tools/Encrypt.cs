using System.Security.Cryptography;
using System.Text;

namespace Arysoft.ARI.NF48.Api.Tools
{
    public partial class Encrypt
    {
        public static string GetSHA256(string value)
        {
            SHA256 sha256 = SHA256.Create();
            var encoding = new ASCIIEncoding();
            byte[] stream = sha256.ComputeHash(encoding.GetBytes(value)); ;
            var sb = new StringBuilder();

            for (int i = 0; i < stream.Length; i++)
            {
                sb.AppendFormat("{0:x2}", stream[i]);
            }

            return sb.ToString();
        }
    }
}