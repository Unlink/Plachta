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
    }
}
