using Cab9.DataLayer;
using Cab9.EventHandlers.Common;
using Cab9.Model.Common;
using e9.Debugging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Cab9.Geography;

namespace Cab9.Model
{
    public class PricingModel
    {
        #region Properties

        public int ID { get; set; }
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ZoneMode ZoneMode { get; set; }
        public bool IgnoreSizeModifiers { get; set; }
        public decimal PricePerMile { get; set; }
        public decimal StandingCharge { get; set; }
        public decimal MinimumCharge { get; set; }
        public short RoundTo { get; set; }
        public TimeSpan PeakStart { get; set; }
        public TimeSpan PeakEnd { get; set; }
        public decimal PeakMultiplier { get; set; }
        public decimal WaitingCharge { get; set; }
        public short WaitingPeriod { get; set; }

        private List<PricingZone> _zones { get; set; }
        [JsonIgnore]
        public List<PricingZone> Zones
        {
            get
            {
                if (_zones == null)
                    _zones = PricingZone.Select().Where(x => x.PricingModelID == ID).ToList();
                return _zones;
            }
            private set
            {
                _zones = value;
            }
        }

        private List<PricingFixed> _fixed { get; set; }
        [JsonIgnore]
        public List<PricingFixed> FixedPricings
        {
            get
            {
                if (_fixed == null)
                    _fixed = PricingFixed.Select(pricingModelID: ID);
                return _fixed;
            }
        }

        #endregion

        #region Events

        public delegate void PricingModelInsertedEvent(PricingModel sender, HubEventArgs e);
        public static event PricingModelInsertedEvent PricingModelInserted;

        public delegate void PricingModelUpdatedEvent(PricingModel sender, HubEventArgs e);
        public static event PricingModelUpdatedEvent PricingModelUpdated;

        public delegate void PricingModelDeletedEvent(PricingModel sender, HubEventArgs e);
        public static event PricingModelDeletedEvent PricingModelDeleted;

        #endregion

        #region Constructors

        public PricingModel()
        {
        }

        private PricingModel(SqlDataReader row)
        {
            try
            {
                if (row["ID"] != DBNull.Value) ID = (int)row["ID"];
                if (row["CompanyID"] != DBNull.Value) CompanyID = (int)row["CompanyID"];
                if (row["Name"] != DBNull.Value) Name = (string)row["Name"];
                if (row["Description"] != DBNull.Value) Description = (string)row["Description"];
                if (row["ZoneMode"] != DBNull.Value) ZoneMode = (ZoneMode)Enum.ToObject(typeof(ZoneMode), row["ZoneMode"]);
                if (row["UseSizeModifiers"] != DBNull.Value) IgnoreSizeModifiers = (bool)row["UseSizeModifiers"];
                if (row["PricePerMile"] != DBNull.Value) PricePerMile = (decimal)row["PricePerMile"];
                if (row["StandingCharge"] != DBNull.Value) StandingCharge = (decimal)row["StandingCharge"];
                if (row["MinimumCharge"] != DBNull.Value) MinimumCharge = (decimal)row["MinimumCharge"];
                if (row["RoundTo"] != DBNull.Value) RoundTo = (short)row["RoundTo"];
                if (row["PeakStart"] != DBNull.Value) PeakStart = (TimeSpan)row["PeakStart"];
                if (row["PeakEnd"] != DBNull.Value) PeakEnd = (TimeSpan)row["PeakEnd"];
                if (row["PeakMulitplier"] != DBNull.Value) PeakMultiplier = (decimal)row["PeakMulitplier"];
                if (row["WaitingCharge"] != DBNull.Value) WaitingCharge = (decimal)row["WaitingCharge"];
                if (row["WaitPeriod"] != DBNull.Value) WaitingPeriod = (short)row["WaitPeriod"];        
            }
            catch (Exception exc)
            {
                SystemLog.LogNewError(exc, LogType.RetreiveError, this);
                throw exc;
            }
        }
       
        #endregion

        #region CRUD

        public static List<PricingModel> Select(int? id = null, int? companyId = null)
        {
            var result = new List<PricingModel>();
            using (SqlDataReader dr = PricingModelDAL.Select(id, companyId))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new PricingModel(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static PricingModel SelectByID(int id)
        {
            PricingModel result = null;
            using (SqlDataReader dr = PricingModelDAL.SelectByID(id))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new PricingModel(dr);
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = PricingModelDAL.Insert(CompanyID, Name, Description, (int)ZoneMode, IgnoreSizeModifiers, PricePerMile, StandingCharge, MinimumCharge, RoundTo, PeakStart, PeakEnd, PeakMultiplier, WaitingCharge, WaitingPeriod);
            if (ID == -1) return false;

            if (PricingModelInserted != null) PricingModelInserted(this, new HubEventArgs(CompanyID, 0));
            return true;
        }

        public bool Update()
        {
            if (PricingModelDAL.Update(ID, CompanyID, Name, Description, (int)ZoneMode, IgnoreSizeModifiers, PricePerMile, StandingCharge, MinimumCharge, RoundTo, PeakStart, PeakEnd, PeakMultiplier, WaitingCharge, WaitingPeriod))
            {
                if (PricingModelUpdated != null) PricingModelUpdated(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        public bool Delete()
        {
            if (PricingModelDAL.Delete(ID))
            {
                Zones.ForEach(x => x.Delete());
                FixedPricings.ForEach(x => x.Delete());
                if (PricingModelDeleted != null) PricingModelDeleted(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        #endregion

        #region Methods

        public bool CheckForFixedPriceBetween(Point p1, Point p2, out decimal? price, out int? oZone, out int? dZone)
        {
            price = oZone = dZone = null;

            if (FixedPricings == null && FixedPricings.Count == 0)
                return false;

            var startingZones = new List<PricingZone>();
            var endingZones = new List<PricingZone>();
            foreach (PricingZone zone in Zones)
            {
                if (zone.ContainsPoint(p1)) startingZones.Add(zone);
                if (zone.ContainsPoint(p2)) endingZones.Add(zone);
            }

            //If origin and destin are in zones then check for fixed pricings
            if (startingZones.Count > 0 && endingZones.Count > 0)
            {
                var orderedStartingZones = startingZones.OrderBy(x => x.Area());
                var orderedEndingZones = endingZones.OrderBy(y => y.Area());
                for (int i = 0, j = 0; i < startingZones.Count && j < orderedEndingZones.Count(); )
                {
                    if (FixedPricings.Any(x => x.PricingZoneFromID == orderedStartingZones.ElementAt(i).ID && x.PricingZoneToID == orderedEndingZones.ElementAt(j).ID))
                    {
                        price = FixedPricings.First(x => x.PricingZoneFromID == orderedStartingZones.ElementAt(i).ID && x.PricingZoneToID == orderedEndingZones.ElementAt(j).ID).Price;
                        oZone = i;
                        dZone = j;
                        return true;
                    }

                    if (i + 1 < orderedStartingZones.Count() && j + 1 < orderedEndingZones.Count())
                        if (i < j) i++; else j++;
                    else if (i + 1 < orderedStartingZones.Count())
                        i++;
                    else if (j + 1 < orderedEndingZones.Count())
                        j++;
                    else
                        break;
                }
            }
            return false;
        }

        #endregion

    }
}