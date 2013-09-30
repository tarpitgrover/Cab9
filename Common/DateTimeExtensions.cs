using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cab9.Common
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1);

        public static double ToUnixTimeInMs(this DateTime dateTime)
        {
            return dateTime.Subtract(DateTimeExtensions.unixEpoch).TotalMilliseconds;
        }
    }

}