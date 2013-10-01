using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cab9.Geography;
using Cab9.Model.Common;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using Newtonsoft.Json;

namespace Cab9.Model
{
    public enum QuoteType
    {
        FixedFare,
        HighestEntered,
        Averaged,
    }

    public class Quote
    {
        public int ID { get; set; }
        public DateTime QuotedAt { get; set; }
        public QuoteType QuoteType { get; set; }
        public Dictionary<string, decimal> Charges { get; set; }
        public decimal FixedFare { get; set; }
        public decimal CalculatedFare { get; set; }
        public decimal TotalFare
        {
            get
            {
                return (IsFixedFare) ? FixedFare : CalculatedFare;
            }
        }
        public Point Origin
        {
            get
            {
                return route.Start;
            }
            internal set { }
        }
        public Point Destination
        {
            get
            {
                return route.End;
            }
        }
        public List<Point> WayPoints
        {
            get
            {
                return route.Points.GetRange(1, route.Points.Count - 2);
            }
        }
        public bool IsSimpleRoute { get; set; }
        public bool IsFixedFare { get; set; }
        public bool IsMinimumFare
        {
            get
            {
                return (!IsFixedFare && CalculatedFare <= model.MinimumCharge);
            }
        }
        public List<RouteLeg> RouteLegs { get; set; }

        private Company company { get; set; }
        private Polyline route { get; set; }
        private VehicleType carType { get; set; }
        private List<Int32> waitingTimesList { get; set; }
        private Client client { get; set; }
        private PricingModel model { get; set; }
        private DateTime? bookedDateTime { get; set; }
        private DirectionsResponse googleResponse { get; set; }
        private List<PricingZone> startZones { get; set; }
        private List<PricingZone> endZones { get; set; }

        public void PopulateQuote(int companyId, string encodedRoute, string waitingTimes = null, int? cartype = 1, int? clientid = null, int? pricingmodel = null, DateTime? bookingtime = null)
        {
            company = Company.SelectByID(companyId);
            route = new Polyline(encodedRoute);
            IsSimpleRoute = (route.Points.Count == 2);
            carType = VehicleType.SelectByID(cartype ?? 1);

            waitingTimesList =
                (String.IsNullOrWhiteSpace(waitingTimes)) ?
                    new List<Int32>()
                    : waitingTimes.Split(',').Select(x => Int32.Parse(x)).ToArray().ToList();

            waitingTimesList.Insert(0, 0);

            client =
                (clientid.HasValue) ?
                    Client.SelectByID(clientid.Value)
                    : null;

            model =
                (pricingmodel.HasValue) ?
                    PricingModel.SelectByID(pricingmodel.Value)
                    : ((clientid.HasValue) ?
                        PricingModel.SelectByID(client.DefaultPricingModelID ?? ClientType.SelectByID(client.ClientTypeID).DefaultPricingModelID ?? company.DefaultPricingModelID ?? 1)
                        : PricingModel.SelectByID(company.DefaultPricingModelID ?? 1));

            bookedDateTime = bookingtime;

            RouteLegs = new List<RouteLeg>();
            startZones = new List<PricingZone>();
            endZones = new List<PricingZone>();
            Charges = new Dictionary<string, decimal>();

            GenerateQuote();
        }

        private void GenerateQuote()
        {
            QuotedAt = DateTime.Now;

            GetGoogleRoute();

            GetStartEndZones();

            if (IsSimpleRoute)
            {
                CheckForFixedPrice();
            }

            ConstructLegs();

            CalculateFinalFare();

        }

        private void GetGoogleRoute()
        {
            DirectionsRequest directionsRequest = new DirectionsRequest();
            directionsRequest.Origin = route.Start.ToString();
            directionsRequest.Destination = route.End.ToString();

            if (!IsSimpleRoute)
            {
                directionsRequest.Waypoints = WayPoints.Select(x => x.ToString()).ToArray();
            }

            googleResponse = GoogleMaps.Directions.Query(directionsRequest);

            ////see here https://github.com/maximn/google-maps/
        }

        private void GetStartEndZones()
        {
            foreach (PricingZone zone in model.Zones)
            {
                if (zone.ContainsPoint(Origin)) startZones.Add(zone);
                if (zone.ContainsPoint(Destination)) endZones.Add(zone);
            }
            startZones = startZones.OrderBy(x => x.Area()).ToList();
            endZones = endZones.OrderBy(x => x.Area()).ToList();
        }

        private void CheckForFixedPrice()
        {
            foreach (PricingZone startZone in startZones)
            {
                foreach (PricingZone endZone in endZones)
                {
                    if (model.FixedPricings.Any(x => x.PricingZoneFromID == startZone.ID && x.PricingZoneToID == endZone.ID))
                    {
                        FixedFare = model.FixedPricings.First(x => x.PricingZoneFromID == startZone.ID && x.PricingZoneToID == endZone.ID).Price;
                        QuoteType = QuoteType.FixedFare;
                        IsFixedFare = true;
                        return;
                    }
                }
            }
        }

        private void ConstructLegs()
        {
            foreach (var leg in googleResponse.Routes.First().Legs)
            {
                var routeLeg = new RouteLeg();
                routeLeg.LegNo = RouteLegs.Count + 1;
                routeLeg.Miles = leg.Distance.Value * 0.00062137M;
                routeLeg.WaitAtDestination = waitingTimesList.ElementAtOrDefault(routeLeg.LegNo);
                routeLeg.Start = new Point((decimal)leg.StartLocation.Latitude, (decimal)leg.StartLocation.Longitude);
                routeLeg.End = new Point((decimal)leg.EndLocation.Latitude, (decimal)leg.EndLocation.Longitude);
                routeLeg.Steps = new List<RouteStep>();
                routeLeg.EnteredZones = new List<PricingZone>();

                if ((decimal)model.WaitingPeriod != 0)
                routeLeg.WaitingCost =
                    Math.Ceiling((decimal)routeLeg.WaitAtDestination / (decimal)model.WaitingPeriod)
                    *
                    (model.Zones.Where(x => x.WaitingCharge.HasValue && x.ZonePolygon.ContainsPoint(routeLeg.End)).OrderBy(x => x.Area()).DefaultIfEmpty(new PricingZone() { WaitingCharge = model.WaitingCharge }).First().WaitingCharge ?? 0.00M);

                foreach (var step in leg.Steps)
                {
                    var routeStep = new RouteStep();
                    routeStep.StepNo = routeLeg.Steps.Count + 1;
                    routeStep.Start = new Point((decimal)step.StartLocation.Latitude, (decimal)step.StartLocation.Longitude);
                    routeStep.End = new Point((decimal)step.EndLocation.Latitude, (decimal)step.EndLocation.Longitude);
                    var polyline = new Polyline(step.PolyLine.Points.Select(x => new Point(Convert.ToDecimal(x.Latitude), Convert.ToDecimal(x.Longitude))).ToList());
                    foreach (var line in polyline.Lines)
                    {
                        routeLeg.EnteredZones.AddRange(model.Zones.Where(z => line.EntersPolygon(z.ZonePolygon)));
                    }
                    routeLeg.Steps.Add(routeStep);
                }

                if (routeLeg.EnteredZones.Count > 0)
                {
                    switch (model.ZoneMode)
                    {
                        case ZoneMode.NoZones:
                            routeLeg.LegCost = routeLeg.Miles * model.PricePerMile;
                            break;
                        case ZoneMode.HighestEntered:
                            routeLeg.LegCost = routeLeg.Miles * routeLeg.EnteredZones.Max(x => (x.PricePerMile ?? model.PricePerMile));
                            break;
                        case ZoneMode.LastEntered:
                            routeLeg.LegCost = routeLeg.Miles * routeLeg.EnteredZones.Select(x => x.PricePerMile ?? model.PricePerMile).Last();
                            break;
                        case ZoneMode.AverageEntered:
                            routeLeg.LegCost = routeLeg.Miles * routeLeg.EnteredZones.Average(x => x.PricePerMile ?? model.PricePerMile);
                            break;
                        default:
                            goto case ZoneMode.HighestEntered;
                    }
                }
                else
                {
                    routeLeg.LegCost = routeLeg.Miles * model.PricePerMile;
                }

                RouteLegs.Add(routeLeg);
            }
        }

        private void CalculateFinalFare()
        {
            Charges.Add("StandingCharge", model.StandingCharge);

            if (startZones.Any(x => x.OriginCharge.HasValue))
                Charges.Add("PickupCharge", startZones.First(x => x.OriginCharge.HasValue).OriginCharge ?? 0.00M);
            else
                Charges.Add("PickupCharge", 0.00M);

            if (endZones.Any(x => x.DestinationCharge.HasValue))
                Charges.Add("DropoffCharge", endZones.First(x => x.DestinationCharge.HasValue).DestinationCharge ?? 0.00M);
            else
                Charges.Add("DropoffCharge", 0.00M);

            if (RouteLegs.Any(x => x.EnteredZones.Count > 0))
                Charges.Add("EntryCharges", RouteLegs.SelectMany(x => x.EnteredZones).Distinct().Sum(x => x.EntryCharge ?? 0.00M));
            else
                Charges.Add("EntryCharges", 0.00M);

            Charges.Add("WaitingCharges", RouteLegs.Sum(x => x.WaitingCost));

            CalculatedFare = ApplyRounding(RouteLegs.Sum(x => x.LegCost) + Charges.Sum(x => x.Value));
            if (IsMinimumFare)
            {
                CalculatedFare = model.MinimumCharge;
            }
            if (!model.IgnoreSizeModifiers)
            {
                CalculatedFare *= carType.BaseMultiplier;
            }

            QuoteType = Model.QuoteType.HighestEntered;
        }

        private decimal ApplyRounding(decimal p)
        {
            decimal roundToPounds = (model.RoundTo / 100M);
            decimal remainder = p % roundToPounds;
            if (remainder < (roundToPounds / 2))
            {
                return p - remainder;
            }
            else
            {
                return p - remainder + roundToPounds;
            }
        }
    }

    public class RouteLeg
    {
        public int LegNo { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
        public decimal Miles { get; set; }
        public decimal LegCost { get; set; }
        public int WaitAtDestination { get; set; }
        public decimal WaitingCost { get; set; }
        public List<RouteStep> Steps { get; set; }

        [JsonIgnore]
        public List<PricingZone> EnteredZones { get; set; }
    }

    public class RouteStep
    {
        public int StepNo { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
    }
}