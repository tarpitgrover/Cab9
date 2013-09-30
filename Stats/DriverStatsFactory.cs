using Cab9.Model;
using Cab9.Stats.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cab9.Stats
{
    public class DriverStatsFactory : StatsFactory
    {
        public List<DriverStatsPeriod> Result = new List<DriverStatsPeriod>();

        public static DriverStatsFactory Generate(int driverid, int companyId, DateTime? from, DateTime? to, string grouping, string timeformat)
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

            return new DriverStatsFactory(driverid, companyId, rFrom, rTo, grouping, timeformat);
        }

        private DriverStatsFactory(int driverid, int companyId, DateTime from, DateTime to, string grouping, string timeformat)
        {
            //Set RequestParamters
            RequestParameters.Add("driverid", driverid.ToString());
            RequestParameters.Add("from", from.ToString());
            RequestParameters.Add("to", to.ToString());
            RequestParameters.Add("grouping", grouping);
            RequestParameters.Add("timeformat", timeformat);


            //Initialise
            var periods = BuildPeriods(from, to, grouping);

            switch (driverid)
            {
                case -1:
                    BuildForAverageDriver(companyId, driverid, timeformat, periods);
                    break;
                case -2:
                    BuildForIdealDriver(companyId, driverid, timeformat, periods);
                    break;
                default:
                    BuildForDriver(companyId, driverid, timeformat, periods);
                    break;
            }
        }

        private void BuildForDriver(int? companyId, int? driverId, string timeformat, IEnumerable<KeyValuePair<DateTime, DateTime>> Periods)
        {
            foreach (var Period in Periods)
            {
                var stat = new DriverStatsPeriod();
                stat.Grouping.From = FormatDateTime(Period.Key, timeformat);
                stat.Grouping.To = FormatDateTime(Period.Value, timeformat);
                stat.DriverStats.TotalDrivers = 1;

                using (var reader = Driver.Stats(Period.Key, Period.Value, companyId, driverId))
                {
                    if (reader.HasRows && reader.Read())
                    {
                        if (reader["BookingsCount"] != DBNull.Value)
                            stat.BookingStats.TotalBookings = Convert.ToDecimal(reader["BookingsCount"]);

                        if (reader["BookingsValue"] != DBNull.Value)
                            stat.BookingStats.BookingsValue = Convert.ToDecimal(reader["BookingsValue"]);

                        if (reader["ShiftsCount"] != DBNull.Value)
                            stat.ShiftStats.TotalShifts = Convert.ToDecimal(reader["ShiftsCount"]);

                        if (reader["AverageBookingsPerShift"] != DBNull.Value)
                            stat.ShiftStats.AverageShiftBookings = Convert.ToDecimal(reader["AverageBookingsPerShift"]);

                        if (reader["AverageTimePerShift"] != DBNull.Value)
                            stat.ShiftStats.AverageLengthOfShift = Convert.ToDecimal(reader["AverageTimePerShift"]);
                    }
                    reader.Close();
                }
                
                Result.Add(stat);
            }
        }

        private void BuildForAverageDriver(int? companyId, int? driverId, string timeformat, IEnumerable<KeyValuePair<DateTime, DateTime>> Periods)
        {
            foreach (var Period in Periods)
            {
                var stat = new DriverStatsPeriod();
                stat.Grouping.From = FormatDateTime(Period.Key, timeformat);
                stat.Grouping.To = FormatDateTime(Period.Value, timeformat);

                using (var reader = Driver.Stats(Period.Key, Period.Value, companyId, null))
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            stat.DriverStats.TotalDrivers++;

                            if (reader["BookingsCount"] != DBNull.Value)
                                stat.BookingStats.TotalBookings += Convert.ToDecimal(reader["BookingsCount"]);

                            if (reader["BookingsValue"] != DBNull.Value)
                                stat.BookingStats.BookingsValue += Convert.ToDecimal(reader["BookingsValue"]);

                            if (reader["ShiftsCount"] != DBNull.Value)
                                stat.ShiftStats.TotalShifts += Convert.ToDecimal(reader["ShiftsCount"]);

                            if (reader["AverageBookingsPerShift"] != DBNull.Value)
                                stat.ShiftStats.AverageShiftBookings += Convert.ToDecimal(reader["AverageBookingsPerShift"]);

                            if (reader["AverageTimePerShift"] != DBNull.Value)
                                stat.ShiftStats.AverageLengthOfShift += Convert.ToDecimal(reader["AverageTimePerShift"]);
                        }
                        stat.BookingStats.TotalBookings /= stat.DriverStats.TotalDrivers;
                        stat.BookingStats.BookingsValue /= stat.DriverStats.TotalDrivers;
                        stat.ShiftStats.TotalShifts /= stat.DriverStats.TotalDrivers;
                        stat.ShiftStats.AverageLengthOfShift /= stat.DriverStats.TotalDrivers;
                        stat.ShiftStats.AverageShiftBookings /= stat.DriverStats.TotalDrivers;
                    }
                    reader.Close();
                }

                Result.Add(stat);
            }
        }

        private void BuildForIdealDriver(int? companyId, int? driverId, string timeformat, IEnumerable<KeyValuePair<DateTime, DateTime>> Periods)
        {
            foreach (var Period in Periods)
            {
                var stat = new DriverStatsPeriod();
                stat.Grouping.From = FormatDateTime(Period.Key, timeformat);
                stat.Grouping.To = FormatDateTime(Period.Value, timeformat);

                using (var reader = Driver.Stats(Period.Key, Period.Value, companyId, null))
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            stat.DriverStats.TotalDrivers++;

                            if (reader["BookingsCount"] != DBNull.Value)
                                if (stat.BookingStats.TotalBookings < Convert.ToDecimal(reader["BookingsCount"]))
                                    stat.BookingStats.TotalBookings = Convert.ToDecimal(reader["BookingsCount"]);

                            if (reader["BookingsValue"] != DBNull.Value)
                                if (stat.BookingStats.BookingsValue < Convert.ToDecimal(reader["BookingsValue"]))
                                    stat.BookingStats.BookingsValue = Convert.ToDecimal(reader["BookingsValue"]);

                            if (reader["ShiftsCount"] != DBNull.Value)
                                if (stat.ShiftStats.TotalShifts < Convert.ToDecimal(reader["ShiftsCount"]))
                                    stat.ShiftStats.TotalShifts = Convert.ToDecimal(reader["ShiftsCount"]);

                            if (reader["AverageBookingsPerShift"] != DBNull.Value)
                                if (stat.ShiftStats.AverageShiftBookings < Convert.ToDecimal(reader["AverageBookingsPerShift"]))
                                    stat.ShiftStats.AverageShiftBookings = Convert.ToDecimal(reader["AverageBookingsPerShift"]);

                            if (reader["AverageTimePerShift"] != DBNull.Value)
                                if (stat.ShiftStats.AverageLengthOfShift <= Convert.ToDecimal(reader["AverageTimePerShift"]))
                                    stat.ShiftStats.AverageLengthOfShift = Convert.ToDecimal(reader["AverageTimePerShift"]);
                        }
                    }
                    reader.Close();
                }
                Result.Add(stat);
            }
        }

        public class DriverStatsPeriod
        {
            public DriverStatsPeriodGrouping Grouping = new DriverStatsPeriodGrouping();
            public class DriverStatsPeriodGrouping
            {
                public dynamic From;
                public dynamic To;
            }

            public DriverStatsPeriodDriverStats DriverStats = new DriverStatsPeriodDriverStats();
            public class DriverStatsPeriodDriverStats
            {
                public decimal TotalDrivers = 0M;
            };

            public DriverStatsPeriodBookingStats BookingStats = new DriverStatsPeriodBookingStats();
            public class DriverStatsPeriodBookingStats
            {
                public decimal TotalBookings = 0M;
                public decimal BookingsValue = 0M;
            };

            public DriverStatsPeriodShiftStats ShiftStats = new DriverStatsPeriodShiftStats();
            public class DriverStatsPeriodShiftStats
            {
                public decimal TotalShifts = 0M;
                public decimal AverageShiftBookings = 0M;
                public decimal AverageLengthOfShift = 0M;
            };
        }
    }
}