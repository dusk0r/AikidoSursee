using System;
using System.Linq;

namespace AikidoWebsite.Common
{

    public interface IPasswordHelper
    {

        string GeneratePassword(int length);

        string CreateHashAndSalt(string str);

        bool VerifyPassword(string sentPassword, string storedHash);

        bool SatisfiesComplexity(string password);
    }

    public class PasswordHelper : IPasswordHelper
    {
        private static char[] passwordChars = new char[]
            { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
              'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
              '%', '&', '_', '-', '$', '+' };
        private static Random random = new Random();

        public string GeneratePassword(int length)
        {
            Check.Argument(length > 5 && length < 15, "Ungültige Passwortlänge");

            var chars = new char[length];
            for (var i = 0; i < chars.Length; i++)
            {
                chars[i] = passwordChars[random.Next(passwordChars.Length)];
            }

            return new String(chars);
        }

        public string CreateHashAndSalt(string str)
        {
            Check.StringHasValue(str, "Kein String");

            return BCrypt.Net.BCrypt.HashPassword(str);
        }

        public bool VerifyPassword(string sentPassword, string storedHash)
        {
            if (sentPassword == null)
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(sentPassword, storedHash);
        }

        public bool SatisfiesComplexity(string password)
        {
            if (String.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            if (password.Length < 8)
            {
                return false;
            }

            if (!password.Any(c => Char.IsUpper(c)))
            {
                return false;
            }

            if (!password.Any(c => Char.IsSymbol(c) || Char.IsPunctuation(c) || Char.IsNumber(c)))
            {
                return false;
            }

            return true;
        }

    }
}
