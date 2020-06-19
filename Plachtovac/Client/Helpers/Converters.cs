using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Plachtovac.Client.Helpers
{
    public class Converters
    {
        public static TimeSpan FromTimeStr(string str)
        {
            var parts = str.Split(":");
            return new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]), 0);
        }

        public static string ToBase64(string s)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(s);
            return System.Convert.ToBase64String(buffer);
        }

        public static string FromBase64(string s)
        {
            byte[] buffer = System.Convert.FromBase64String(s);
            return System.Text.Encoding.UTF8.GetString(buffer);
        }

        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string CreateIdentifier(string text)
        {
            var regex  = new Regex("\\s");
            return regex.Replace(RemoveDiacritics(text), "");
        }
    }
}
