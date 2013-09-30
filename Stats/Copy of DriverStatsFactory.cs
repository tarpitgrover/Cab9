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
            
        [JsonIgnore]
        private IEnumerable<KeyValuePair<DateTime, DateTime>> Periods { get; set; }

        [JsonIgnore]
        private List<Booking> AllBookings { get; set; }

        [JsonIgnore]
        private List<DriverShift> AllShifts { get; set; }

        public static DriverStatsFactory Generate(int driverid, int companyId, DateTime? from, DateTime? to, string grouping, string timeformat)
        {
            //Set last defaults
            DateTime
                rFrom = DateTime.SpecifyKind(from ?? DateTime.Today.AddYears(-5), DateTimeKind.Utc),
                rTo = DateTime.SpecifyKind(to ?? DateTime.Today, DateTimeKind.Utc).AddDays(1);
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
            Periods = BuildPeriods(from, to, grouping);
            AllBookings = Booking.Select(driverID: ((driverid >= 0) ? driverid : 0), companyId: companyId, bookedFrom: from, bookedTo: to);
            List<long> ShiftsIds;
            if (AllBookings.Count > 0)
            {
                //ShiftsIds = AllBookings.Select(x => x.ShiftID ?? 0).Distinct().Where(x => x != 0).ToList();
                //AllShifts = ShiftsIds.Select(x => DriverShift.SelectByID(x)).ToList();
            }
            else
            {
                AllShifts = new List<DriverShift>();
            }

            switch (driverid)
            {
                case -1:
                    BuildForAverageDriver(timeformat);
                    break;
                case -2:
                    BuildForIdealDriver(timeformat);
                    break;
                default:
                    BuildForDriver(timeformat);
                    break;
            }
        }

        private void BuildForDriver(string timeformat)
        {
            foreach (var Period in Periods)
            {
                var stat = new DriverStatsPeriod();
                stat.Grouping.From = FormatDateTime(Period.Key, timeformat);
                stat.Grouping.To = FormatDateTime(Period.Value, timeformat);

                var Bookings = AllBookings.Where(x => x.BookedDateTime >= Period.Key && x.BookedDateTime <= Period.Value).ToList();
                var Shifts = new List<DriverShift>();
                if (Bookings.Count() > 0)
                {
                    List<long> ShiftsIds = Bookings.Select(x => x.ShiftID ?? 0).Distinct().Where(x => x != 0).ToList();
                    Shifts.AddRange(ShiftsIds.Select(x => DriverShift.SelectByID(x)).Where(x => x != null));

                    stat.DriverStats.TotalDrivers = Bookings.Select(x => x.DriverID).Distinct().Count();

                    stat.BookingStats.TotalBookings = Bookings.Count();
                    stat.BookingStats.BookingsValue = Bookings.Sum(x => x.ActualFare ?? 0M);

                    stat.ShiftStats.TotalShifts = Shifts.Count();
                    if (stat.ShiftStats.TotalShifts > 0)
                    {
                        stat.ShiftStats.AverageShiftBookings = stat.BookingStats.TotalBookings / stat.ShiftStats.TotalShifts;
                        stat.ShiftStats.AverageLengthOfShift = Convert.ToDecimal(new TimeSpan((long)Shifts.Select(x => (x.ShiftEnd.HasValue) ? x.ShiftEnd.Value.Ticks - x.ShiftStart.Ticks : 0L).Average()).TotalHours);
                    }
                }
                Result.Add(stat);
            }
        }

        private void BuildForAverageDriver(string timeformat)
        {
            foreach (var Period in Periods)
            {
                var stat = new DriverStatsPeriod();
                stat.Grouping.From = FormatDateTime(Period.Key, timeformat);
                stat.Grouping.To = FormatDateTime(Period.Value, timeformat);

                var Bookings = AllBookings.Where(x => x.BookedDateTime >= Period.Key && x.BookedDateTime <= Period.Value);
                var Shifts = new List<DriverShift>();
                if (Bookings.Count() > 0)
                {
                    List<long> ShiftsIds = Bookings.Select(x => x.ShiftID ?? 0).Distinct().Where(x => x != 0).ToList();
                    Shifts.AddRange(ShiftsIds.Select(x => DriverShift.SelectByID(x)).Where(x => x != null));

                    stat.DriverStats.TotalDrivers = Bookings.Select(x => x.DriverID).Distinct().Count();

                    if (stat.DriverStats.TotalDrivers != 0)
                    {
                        stat.BookingStats.TotalBookings = Bookings.Count() / stat.DriverStats.TotalDrivers;
                        stat.BookingStats.BookingsValue = Bookings.Sum(x => x.ActualFare ?? 0M) / stat.DriverStats.TotalDrivers;

                        stat.ShiftStats.TotalShifts = Shifts.Count() / stat.DriverStats.TotalDrivers;
                        if (stat.ShiftStats.TotalShifts > 0)
                        {
                            stat.ShiftStats.AverageShiftBookings = stat.BookingStats.TotalBookings / stat.ShiftStats.TotalShifts;
                            stat.ShiftStats.AverageLengthOfShift = Convert.ToDecimal(new TimeSpan((long)Shifts.Select(x => (x.ShiftEnd.HasValue) ? x.ShiftEnd.Value.Ticks - x.ShiftStart.Ticks : 0L).Average()).TotalHours);
                        }
                    }
                }

                Result.Add(stat);
            }
        }

        private void BuildForIdealDriver(string timeformat)
        {
            foreach (var Period in Periods)
            {
                var stat = new DriverStatsPeriod();
                stat.Grouping.From = FormatDateTime(Period.Key, timeformat);
                stat.Grouping.To = FormatDateTime(Period.Value, timeformat);

                var Bookings = AllBookings.Where(x => x.DriverID != 0 && x.ShiftID != 0 && x.BookedDateTime >= Period.Key && x.BookedDateTime <= Period.Value);
                var Shifts = new List<DriverShift>();
                if (Bookings.Count() > 0)
                {
                    List<long> ShiftsIds = Bookings.Select(x => x.ShiftID ?? 0).Distinct().Where(x => x != 0).ToList();
                    Shifts.AddRange(ShiftsIds.Select(x => DriverShift.SelectByID(x)).Where(x => x != null));

                    stat.DriverStats.TotalDrivers = Bookings.Select(x => x.DriverID).Distinct().Count();

                    if (stat.DriverStats.TotalDrivers != 0)
                    {
                        stat.BookingStats.TotalBookings = Bookings.GroupBy(b => b.DriverID ?? 0).Select(gb => gb.Count()).Max();
                        stat.BookingStats.BookingsValue = Bookings.GroupBy(b => b.DriverID ?? 0).Select(gb => gb.Sum(b => b.ActualFare ?? 0M)).Max();

                        stat.ShiftStats.TotalShifts = Shifts.GroupBy(s => s.DriverID).Select(gs => gs.Count()).Max();
                        stat.ShiftStats.AverageShiftBookings = Bookings.GroupBy(b => b.DriverID ?? 0).Select(gb => (Shifts.Count(s => s.DriverID == gb.Key) == 0) ? 0M : gb.Count() / Shifts.Count(s => s.DriverID == gb.Key)).Max();
                        stat.ShiftStats.AverageLengthOfShift = Shifts.GroupBy(s => s.DriverID).Select(gs => Convert.ToDecimal(new TimeSpan((long)Shifts.Select(x => (x.ShiftEnd.HasValue) ? x.ShiftEnd.Value.Ticks - x.ShiftStart.Ticks : 0L).Average()).TotalHours)).Max();
                    }
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