using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hacker_Tracer
{
    internal class CaesarCipher
    {
        public static string Encrypt(string text, int shift)
        {
            shift = ((shift % 26) + 26) % 26;
            var result = new System.Text.StringBuilder();
            foreach (char c in text)
            {
                if (char.IsLetter(c))
                {
                    char baseChar = char.IsUpper(c) ? 'A' : 'a';
                    result.Append((char)((c - baseChar + shift) % 26 + baseChar));
                }
                else result.Append(c);
            }
            return result.ToString();
        }

        public static string Decrypt(string text, int shift) => Encrypt(text, 26 - shift);
        public static string EncryptForDisplay(string plainText, int shift) => Encrypt(plainText, shift);
    }
}
