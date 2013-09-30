using Cab9.Model;
using Cab9.Stats.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cab9.Stats
{
    public class BookingStatsFactory : StatsFactory
    {
        public List<BookingStatsPeriod> Result = new List<BookingStatsPeriod>();
            
        [JsonIgnore]
        private IEnumerable<KeyValuePair<DateTime, DateTime>> Periods { get; set; }

        [JsonIgnore]
        private List<Booking> AllBookings { get; set; }

        [JsonIgnore]
        private List<DriverShift> AllShifts { get; set; }

        public static BookingStatsFactory Generate(int driverId, int clientId, int companyId, DateTime? from, DateTime? to, string grouping, string timeformat)
        {
            //Set defaults
            DateTime
                rFrom = DateTime.SpecifyKind(from ?? DateTime.Today.AddYears(-5), DateTimeKind.Utc),
                rTo = DateTime.SpecifyKind(to ?? DateTime.Today, DateTimeKind.Utc).AddDays(1);
            if (grouping.ToLower() == "smart") grouping = DetermineGrouping(rFrom, rTo);

            return new BookingStatsFactory(driverId, clientId, companyId, rFrom, rTo, grouping, timeformat);
        }

        private BookingStatsFactory(int driverId, int clientId, int companyId, DateTime from, DateTime to, string grouping, string timeformat)
        {
            //Set RequestParamters
            RequestParameters.Add("driverid", driverId.ToString());
            RequestParameters.Add("clientid", clientId.ToString());
            RequestParameters.Add("from", from.ToString());
            RequestParameters.Add("to", to.ToString());
            RequestParameters.Add("grouping", grouping);
            RequestParameters.Add("timeformat", timeformat);


            //Initialise
            Periods = BuildPeriods(from, to, grouping);
            AllBookings = Booking.Select(driverID: ((driverId >= 0) ? driverId : 0), clientID: clientId, companyId: companyId, bookedFrom: from, bookedTo: to);
            List<long> ShiftsIds;
            if (AllBookings.Count > 0)
            {
                ShiftsIds = AllBookings.Select(x => x.ShiftID ?? 0).Distinct().Where(x => x != 0).ToList();
                AllShifts = ShiftsIds.Select(x => DriverShift.SelectByID(x)).ToList();
            }
            else
            {
                AllShifts = new List<DriverShift>();
            }

            switch (driverId)
            {
                case -1:
                    BuildForAverageDriver();
                    break;
                case -2:
                    BuildForIdealDriver();
                    break;
                default:
                    BuildForDriver();
                    break;
            }
        }

        private void BuildForDriver()
        {
            foreach (var Period in Periods)
            {
                var stat = new BookingStatsPeriod();
                stat.Grouping.From = Period.Key;
                stat.Grouping.To = Period.Value;

                var Bookings = AllBookings.Where(x => x.BookedDateTime >= Period.Key && x.BookedDateTime <= Period.Value);
                var Shifts = new List<DriverShift>();
                if (Bookings.Count() > 0)
                {
                    List<long> ShiftsIds = Bookings.Select(x => x.ShiftID ?? 0).Distinct().Where(x => x != 0).ToList();
                    Shifts.AddRange(AllShifts.Where(x => ShiftsIds.Contains(x.ID)));

                    stat.DriverStats.TotalDrivers = Bookings.Select(x => x.DriverID ?? 0).Distinct().Count();

                    stat.ClientStats.TotalClients = Bookings.Select(x => x.ClientID ?? 0).Distinct().Count();

                    stat.VehicleStats.TotalVehicles = Bookings.Select(x => x.VehicleID ?? 0).Distinct().Count();

                    stat.BookingStats.TotalBookings = Bookings.Count();
                    stat.BookingStats.BookingsValue = Bookings.Sum(x => x.ActualFare ?? 0M);

                    stat.ShiftStats.TotalShifts = Shifts.Count();
                    if (stat.ShiftStats.TotalShifts > 0)
                    {
                        stat.ShiftStats.AverageShiftBookings = stat.BookingStats.TotalBookings / stat.ShiftStats.TotalShifts;
                        stat.ShiftStats.AverageShiftLength = Convert.ToDecimal(new TimeSpan((long)Shifts.Select(x => (x.ShiftEnd ?? DateTime.Now).Ticks - x.ShiftStart.Ticks).DefaultIfEmpty().Average()).TotalDays);
                    }
                }
                Result.Add(stat);
            }
        }

        private void BuildForAverageDriver()
        {
            foreach (var Period in Periods)
            {
                var stat = new BookingStatsPeriod();
                stat.Grouping.From = Period.Key;
                stat.Grouping.To = Period.Value;

                var Bookings = AllBookings.Where(x => x.BookedDateTime >= Period.Key && x.BookedDateTime <= Period.Value);
                var Shifts = new List<DriverShift>();
                if (Bookings.Count() > 0)
                {
                    List<long> ShiftsIds = Bookings.Select(x => x.ShiftID ?? 0).Distinct().Where(x => x != 0).ToList();
                    Shifts.AddRange(AllShifts.Where(x => ShiftsIds.Contains(x.ID)));

                    stat.DriverStats.TotalDrivers = Bookings.Select(x => x.DriverID).Distinct().Count();

                    stat.ClientStats.TotalClients = Bookings.Select(x => x.ClientID ?? 0).Distinct().Count();

                    stat.VehicleStats.TotalVehicles = Bookings.Select(x => x.VehicleID ?? 0).Distinct().Count();

                    if (stat.DriverStats.TotalDrivers != 0)
                    {
                        stat.BookingStats.TotalBookings = Bookings.Count() / stat.DriverStats.TotalDrivers;
                        stat.BookingStats.BookingsValue = Bookings.Sum(x => x.ActualFare ?? 0M) / stat.DriverStats.TotalDrivers;

                        stat.ShiftStats.TotalShifts = Shifts.Count() / stat.DriverStats.TotalDrivers;
                        if (stat.ShiftStats.TotalShifts > 0)
                        {
                            stat.ShiftStats.AverageShiftBookings = stat.BookingStats.TotalBookings / stat.ShiftStats.TotalShifts;
                            stat.ShiftStats.AverageShiftLength = Convert.ToDecimal(new TimeSpan(Convert.ToInt64(Shifts.Sum(s => (s.ShiftEnd ?? DateTime.Now).Ticks - s.ShiftStart.Ticks) / stat.ShiftStats.TotalShifts)).TotalDays);
                        }
                        //stat.ShiftStats.AverageShiftBookings = (stat.BookingStats.TotalBookings == 0) ? 0M : Bookings.GroupBy(b => b.DriverID).Select(gb => { var z = Convert.ToDecimal(gb.Select(s => s.ShiftID).Distinct().Count()); return (z == 0M) ? 0M : gb.Count() / z; }).Average();
                        //stat.ShiftStats.AverageShiftLength = (stat.ShiftStats.TotalShifts == 0) ? 0M : Convert.ToDecimal(new TimeSpan(Convert.ToInt64(Shifts.GroupBy(s => s.DriverID).Select(gs => gs.Sum(s => (s.ShiftEnd ?? DateTime.Now).Ticks - s.ShiftStart.Ticks) / gs.Count()).Average())).TotalDays);
                    }
                }

                Result.Add(stat);
            }
        }

        private void BuildForIdealDriver()
        {
            foreach (var Period in Periods)
            {
                var stat = new BookingStatsPeriod();
                stat.Grouping.From = Period.Key;
                stat.Grouping.To = Period.Value;

                var Bookings = AllBookings.Where(x => x.BookedDateTime >= Period.Key && x.BookedDateTime <= Period.Value);
                var Shifts = new List<DriverShift>();
                if (Bookings.Count() > 0)
                {
                    List<long> ShiftsIds = Bookings.Select(x => x.ShiftID ?? 0).Distinct().Where(x => x != 0).ToList();
                    Shifts.AddRange(AllShifts.Where(x => ShiftsIds.Contains(x.ID)));

                    stat.DriverStats.TotalDrivers = Bookings.Select(x => x.DriverID).Distinct().Count();

                    stat.ClientStats.TotalClients = Bookings.Select(x => x.ClientID ?? 0).Distinct().Count();

                    stat.VehicleStats.TotalVehicles = Bookings.Select(x => x.VehicleID ?? 0).Distinct().Count();

                    if (stat.DriverStats.TotalDrivers != 0)
                    {
                        stat.BookingStats.TotalBookings = Bookings.GroupBy(b => b.DriverID ?? 0).Select(gb => gb.Count()).Max();
                        stat.BookingStats.BookingsValue = Bookings.GroupBy(b => b.DriverID ?? 0).Select(gb => gb.Sum(b => b.ActualFare ?? 0M)).Max();

                        stat.ShiftStats.TotalShifts = Shifts.GroupBy(s => s.DriverID).Select(gs => gs.Count()).Max();
                        stat.ShiftStats.AverageShiftBookings = Bookings.GroupBy(b => b.DriverID ?? 0).Select(gb => gb.Count() / Shifts.Count(s => s.DriverID == gb.Key)).Max();
                        stat.ShiftStats.AverageShiftLength = Shifts.GroupBy(s => s.DriverID).Select(gs => Convert.ToDecimal(new TimeSpan(gs.Sum(s => (s.ShiftEnd ?? DateTime.Now).Ticks - s.ShiftStart.Ticks) / gs.Count()).TotalDays)).Max();
                    }
                }
                Result.Add(stat);
            }
        }

        public class BookingStatsPeriod
        {
            public BookingStatsPeriodGrouping Grouping = new BookingStatsPeriodGrouping();
            public class BookingStatsPeriodGrouping
            {
                public DateTime From;
                public DateTime To;
            }

            public BookingStatsPeriodDriverStats DriverStats = new BookingStatsPeriodDriverStats();
            public class BookingStatsPeriodDriverStats
            {
                public decimal TotalDrivers = 0M;
            };

            public BookingStatsPeriodClientStats ClientStats = new BookingStatsPeriodClientStats();
            public class BookingStatsPeriodClientStats
            {
                public decimal TotalClients = 0M;
            };

            public BookingStatsPeriodVehicleStats VehicleStats = new BookingStatsPeriodVehicleStats();
            public class BookingStatsPeriodVehicleStats
            {
                public decimal TotalVehicles = 0M;
            };

            public BookingStatsPeriodBookingStats BookingStats = new BookingStatsPeriodBookingStats();
            public class BookingStatsPeriodBookingStats
            {
                public decimal TotalBookings = 0M;
                public decimal BookingsValue = 0M;
            };

            public BookingStatsPeriodShiftStats ShiftStats = new BookingStatsPeriodShiftStats();
            public class BookingStatsPeriodShiftStats
            {
                public decimal TotalShifts = 0M;
                public decimal AverageShiftBookings = 0M;
                public decimal AverageShiftLength = 0M;
            };
        }
    }
}