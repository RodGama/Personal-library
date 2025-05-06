using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Common.Features
{
    public class BCryptor
    {
        public static string Encrypt(string input)
        {
            var encryptedText = BCrypt.Net.BCrypt.HashPassword(input);
            return encryptedText;
        }

        public static bool InputIsCorrect(string input, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(input, storedHash);
        }
    }
}
