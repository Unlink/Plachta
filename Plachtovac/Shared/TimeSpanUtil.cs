using System;
using System.Collections.Generic;
using System.Text;

namespace Plachtovac.Shared
{
    public static class TimeSpanUtil
    {
        public static TimeSpan RoundUp(this TimeSpan dt, TimeSpan d)
        {
            var delta = (d.Ticks - (dt.Ticks % d.Ticks)) % d.Ticks;
            return new TimeSpan(dt.Ticks + delta);
        }

        public static TimeSpan RoundDown(this TimeSpan dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            return new TimeSpan(dt.Ticks - delta);
        }

        public static TimeSpan RoundToNearest(this TimeSpan dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            bool roundUp = delta > d.Ticks / 2;

            return roundUp ? dt.RoundUp(d) : dt.RoundDown(d);
        }
    }
}
