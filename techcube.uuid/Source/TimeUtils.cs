using System;
using System.Diagnostics;

namespace techcube.uuid
{
    internal static class TimeUtils
    {
        private static long unixTimeZeroTicks = new DateTime(1970, 1, 1).Ticks;
        public static long GetNanoseconds()
        {
            long timestamp = DateTime.UtcNow.Ticks - unixTimeZeroTicks;
            double nanoseconds = 1_000_000_000.0 * timestamp / Stopwatch.Frequency;

            return (long)nanoseconds;
        }
    }
}