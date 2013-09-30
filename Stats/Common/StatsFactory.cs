using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cab9.Common;

namespace Cab9.Stats.Common
{
    public class StatsFactory
    {
        public Dictionary<string, string> RequestParameters = new Dictionary<string, string>();

        public static string DetermineGrouping(DateTime from, DateTime to)
        {
            const int days = 10, weeks = 56, months = 180, quarters = 730;

            var difference = to - from;
            if (difference.Days <= days) return "days";
            if (difference.Days > days && difference.Days <= weeks) return "weeks";
            if (difference.Days > weeks && difference.Days <= months) return "months";
            if (difference.Days > months && difference.Days <= quarters) return "quarters";
            if (difference.Days > quarters) return "years";
            return "weeks";
        }

        public static IEnumerable<KeyValuePair<DateTime, DateTime>> BuildPeriods(DateTime from, DateTime to, string grouping)
        {
            var rtn = new Dictionary<DateTime, DateTime>();

            if (grouping.ToLower() == "none")
            {
                rtn.Add(from, to);
                return rtn;
            }

            DateTime periodStart = to, periodEnd = to;

            while (periodStart >= from)
            {
                switch (grouping)
                {
                    case "days":
                        periodStart = periodEnd.AddDays(-1);
                        break;
                    case "weeks":
                        periodStart = periodEnd.AddDays(-7);
                        break;
                    case "months":
                        periodStart = periodEnd.AddMonths(-1);
                        break;
                    case "quarters":
                        periodStart = periodEnd.AddMonths(-3);
                        break;
                    case "years":
                        periodStart = periodEnd.AddYears(-1);
                        break;
                    default:
                        periodStart = from;
                        break;
                }

                rtn.Add(periodStart, periodEnd);
                periodEnd = periodStart;
            }

            return rtn.Reverse();
        }

        public static dynamic FormatDateTime(DateTime date, string type)
        {
            switch (type.ToLower())
            {
                case "utc":
                    return date.ToUniversalTime();
                case "javascript":
                    return date.ToUnixTimeInMs();
                case "local":
                default:
                    return date;
            }
        }
    }
}