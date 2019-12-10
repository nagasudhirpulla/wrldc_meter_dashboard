using System;
using System.Collections.Generic;
using System.Text;

namespace MeterDataDashboard.Application
{
    public class TimeUtils
    {
        /// UNIX time epoch
        private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// Calculate time (using milliseconds) from UNIX epoch
        public static DateTime FromMillisecondsSinceUnixEpoch(long msec)
        {
            return UnixEpoch + TimeSpan.FromMilliseconds(msec);
        }

        public static double ToMillisSinceUnixEpoch(DateTime time)
        {
            return time.ToUniversalTime().Subtract(UnixEpoch).TotalMilliseconds;
        }
    }
}
