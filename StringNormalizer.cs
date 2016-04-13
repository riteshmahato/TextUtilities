//Author: Ritesh Mahato
//Program to normalize strings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiteshMahato.Utilities
{
    class StringNormalizer
    {
        public static string GetNormalizedText(string text)
        {
            text = text.ToLower();
            string[] symbols = { "`", "!", "~", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "-", "+", "=", "{", "}", ":", ";", "\"", "'", "<", ",", ">", ".", "?", "/", "?", "|", "\\", " " };

            return string.Join(" ", text.Split(symbols, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
