using Cab9.Model;
using Cab9.Stats.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cab9.Stats
{
    public class ClientStatsFactory : StatsFactory
    {
        public List<ClientStatsPeriod> Result = new List<ClientStatsPeriod>();

        public static ClientStatsFactory Generate(int clientid, int companyId, DateTime? from, DateTime? to, string grouping, string timeformat)
        {
            //Set last defaults
            DateTime
                rFrom = DateTime.SpecifyKind(from ?? DateTime.Today.AddYears(-5), DateTimeKind.Utc),
                rTo = DateTime.SpecifyKind(to ?? DateTime.Today, DateTimeKind.Utc);

            rFrom = rFrom.Subtract(rFrom.TimeOfDay);
            var time = rTo.TimeOfDay;
            var add = new TimeSpan(23 - time.Hours, 59 - time.Minutes, 59 - time.Seconds);
            rTo = rTo.Add(add);
            if (grouping.ToLower() == "smart") grouping = DetermineGrouping(rFrom, rTo);

            return new ClientStatsFactory(clientid, companyId, rFrom, rTo, grouping, timeformat);
        }

        private ClientStatsFactory(int clientid, int companyId, DateTime from, DateTime to, string grouping, string timeformat)
        {
            //Set RequestParamters
            RequestParameters.Add("clientid", clientid.ToString());
            RequestParameters.Add("from", from.ToString());
            RequestParameters.Add("to", to.ToString());
            RequestParameters.Add("grouping", grouping);
            RequestParameters.Add("timeformat", timeformat);


            //Initialise
            var periods = BuildPeriods(from, to, grouping);

            switch (clientid)
            {
                case -1:
                    BuildForAverageClient(companyId, clientid, timeformat, periods);
                    break;
                case -2:
                    BuildForIdealClient(companyId, clientid, timeformat, periods);
                    break;
                default:
                    BuildForClient(companyId, clientid, timeformat, periods);
                    break;
            }
        }

        private void BuildForClient(int? companyId, int? ClientId, string timeformat, IEnumerable<KeyValuePair<DateTime, DateTime>> Periods)
        {
            foreach (var Period in Periods)
            {
                var stat = new ClientStatsPeriod();
                stat.Grouping.From = FormatDateTime(Period.Key, timeformat);
                stat.Grouping.To = FormatDateTime(Period.Value, timeformat);
                stat.ClientStats.TotalClients = 1;

                using (var reader = Client.Stats(Period.Key, Period.Value, companyId, ClientId))
                {
                    if (reader.HasRows && reader.Read())
                    {
                        if (reader["BookingsCount"] != DBNull.Value)
                            stat.BookingStats.TotalBookings = Convert.ToDecimal(reader["BookingsCount"]);

                        if (reader["BookingsValue"] != DBNull.Value)
                            stat.BookingStats.BookingsValue = Convert.ToDecimal(reader["BookingsValue"]);

                        if (reader["AvgValuePerBooking"] != DBNull.Value)
                            stat.BookingStats.AvgValuePerBooking = Convert.ToDecimal(reader["AvgValuePerBooking"]);

                    }
                    reader.Close();
                }

                Result.Add(stat);
            }
        }

        private void BuildForAverageClient(int? companyId, int? ClientId, string timeformat, IEnumerable<KeyValuePair<DateTime, DateTime>> Periods)
        {
            foreach (var Period in Periods)
            {
                var stat = new ClientStatsPeriod();
                stat.Grouping.From = FormatDateTime(Period.Key, timeformat);
                stat.Grouping.To = FormatDateTime(Period.Value, timeformat);

                using (var reader = Client.Stats(Period.Key, Period.Value, companyId, null))
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            stat.ClientStats.TotalClients++;

                            if (reader["BookingsCount"] != DBNull.Value)
                                stat.BookingStats.TotalBookings += Convert.ToDecimal(reader["BookingsCount"]);

                            if (reader["BookingsValue"] != DBNull.Value)
                                stat.BookingStats.BookingsValue += Convert.ToDecimal(reader["BookingsValue"]);

                            if (reader["AvgValuePerBooking"] != DBNull.Value)
                                stat.BookingStats.AvgValuePerBooking += Convert.ToDecimal(reader["AvgValuePerBooking"]);
                        }
                        stat.BookingStats.TotalBookings /= stat.ClientStats.TotalClients;
                        stat.BookingStats.BookingsValue /= stat.ClientStats.TotalClients;
                        stat.BookingStats.AvgValuePerBooking /= stat.ClientStats.TotalClients;
                    }
                    reader.Close();
                }

                Result.Add(stat);
            }
        }

        private void BuildForIdealClient(int? companyId, int? ClientId, string timeformat, IEnumerable<KeyValuePair<DateTime, DateTime>> Periods)
        {
            foreach (var Period in Periods)
            {
                var stat = new ClientStatsPeriod();
                stat.Grouping.From = FormatDateTime(Period.Key, timeformat);
                stat.Grouping.To = FormatDateTime(Period.Value, timeformat);

                using (var reader = Client.Stats(Period.Key, Period.Value, companyId, null))
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            stat.ClientStats.TotalClients++;
                            if (reader["BookingsCount"] != DBNull.Value)
                                if (stat.BookingStats.TotalBookings < Convert.ToDecimal(reader["BookingsCount"]))
                                    stat.BookingStats.TotalBookings = Convert.ToDecimal(reader["BookingsCount"]);

                            if (reader["BookingsValue"] != DBNull.Value)
                                if (stat.BookingStats.BookingsValue < Convert.ToDecimal(reader["BookingsValue"]))
                                    stat.BookingStats.BookingsValue = Convert.ToDecimal(reader["BookingsValue"]);

                            if (reader["AvgValuePerBooking"] != DBNull.Value)
                                if (stat.BookingStats.AvgValuePerBooking < Convert.ToDecimal(reader["AvgValuePerBooking"]))
                                    stat.BookingStats.AvgValuePerBooking = Convert.ToDecimal(reader["AvgValuePerBooking"]);
                        }
                    }
                    reader.Close();
                }
                Result.Add(stat);
            }
        }

        public class ClientStatsPeriod
        {
            public ClientStatsPeriodGrouping Grouping = new ClientStatsPeriodGrouping();
            public class ClientStatsPeriodGrouping
            {
                public dynamic From;
                public dynamic To;
            }

            public ClientStatsPeriodClientStats ClientStats = new ClientStatsPeriodClientStats();
            public class ClientStatsPeriodClientStats
            {
                public decimal TotalClients = 0M;
            };

            public ClientStatsPeriodBookingStats BookingStats = new ClientStatsPeriodBookingStats();
            public class ClientStatsPeriodBookingStats
            {
                public decimal TotalBookings = 0M;
                public decimal BookingsValue = 0M;
                public decimal AvgValuePerBooking = 0M;
            };
        }
    }
}