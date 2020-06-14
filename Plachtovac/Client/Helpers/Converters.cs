using System;

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
    }
}
