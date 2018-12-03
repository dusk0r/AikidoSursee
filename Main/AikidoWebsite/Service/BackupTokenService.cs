using System;
using System.Security.Cryptography;
using System.Text;
using AikidoWebsite.Common;

namespace AikidoWebsite.Web.Service
{
    public class BackupTokenService
    {
        private readonly string _secret;
        private readonly IClock _clock;

        public BackupTokenService(string secret, IClock clock)
        {
            this._secret = secret;
            this._clock = clock;
        }

        public string GenerateToken(DateTime? date = null) {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secret)))
            {
                var now = date ?? _clock.Now;
                var dateBytes = Encoding.UTF8.GetBytes(now.ToString("yyyy-MM-dd"));
                var hashBytes = hmac.TransformFinalBlock(dateBytes, 0, dateBytes.Length);
                return BitConverter.ToString(hashBytes).Replace("-", String.Empty).ToUpperInvariant();
            }      
        }

        public bool CheckToken(string token, DateTime? date = null)
        {
            var expected = GenerateToken(date);
            if (String.IsNullOrWhiteSpace(token) || expected.Length != token.Length)
            {
                return false;
            }

            uint diff = 0;
            for (int i = 0; i < expected.Length; i++)
            {
                diff |= (uint)expected[i] ^ (uint)token[i]; 
            }

            return diff == 0;
        }
    }
}
