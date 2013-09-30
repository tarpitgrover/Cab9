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
    public class PricingZone
    {
        #region Properties

        public int ID { get; set; }
        public int CompanyID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string EncodedZone { get; set; }
        public int PricingModelID { get; set; }
        public decimal? OriginCharge { get; set; }
        public decimal? EntryCharge { get; set; }
        public decimal? DestinationCharge { get; set; }
        public decimal? PricePerMile { get; set; }
        public decimal? WaitingCharge { get; set; }

        public PricingModel PricingModel { get; set; }

        private Polygon _zonePolygon = null;
        [JsonIgnore]
        public Polygon ZonePolygon
        {
            get
            {
                if (_zonePolygon == null)
                    _zonePolygon = new Polygon(EncodedZone);
                return _zonePolygon;
            }
            set
            {
                _zonePolygon = value;
                EncodedZone = value.ToEncodedString();
            }
        }
        #endregion

        #region Events

        public delegate void PricingZoneInsertedEvent(PricingZone sender, HubEventArgs e);
        public static event PricingZoneInsertedEvent PricingZoneInserted;

        public delegate void PricingZoneUpdatedEvent(PricingZone sender, HubEventArgs e);
        public static event PricingZoneUpdatedEvent PricingZoneUpdated;

        public delegate void PricingZoneDeletedEvent(PricingZone sender, HubEventArgs e);
        public static event PricingZoneDeletedEvent PricingZoneDeleted;

        #endregion

        #region Constructors

        public PricingZone()
        {
        }

        private PricingZone(SqlDataReader row)
        {
            try
            {
                if (row["ID"] != DBNull.Value) ID = (int)row["ID"];
                if (row["CompanyID"] != DBNull.Value) CompanyID = (int)row["CompanyID"];
                if (row["Title"] != DBNull.Value) Title = (string)row["Title"];
                if (row["Description"] != DBNull.Value) Description = (string)row["Description"];
                if (row["EncodedZone"] != DBNull.Value) EncodedZone = (string)row["EncodedZone"];
                if (row["PricingModelID"] != DBNull.Value) { PricingModelID = (int)row["PricingModelID"]; PricingModel = PricingModel.SelectByID(PricingModelID); }
                if (row["OriginCharge"] != DBNull.Value) OriginCharge = (decimal)row["OriginCharge"];
                if (row["EntryCharge"] != DBNull.Value) EntryCharge = (decimal)row["EntryCharge"];
                if (row["DestinationCharge"] != DBNull.Value) DestinationCharge = (decimal)row["DestinationCharge"];
                if (row["PricePerMile"] != DBNull.Value) PricePerMile = (decimal)row["PricePerMile"];
                if (row["WaitingCharge"] != DBNull.Value) WaitingCharge = (decimal)row["WaitingCharge"];
            }
            catch (Exception exc)
            {
                SystemLog.LogNewError(exc, LogType.RetreiveError, this);
                throw exc;
            }
        }
       
        #endregion

        #region CRUD

        public static List<PricingZone> Select(int? id = null, int? companyId = null, int? pricingmodelid = null)
        {
            var result = new List<PricingZone>();
            using (SqlDataReader dr = PricingZoneDAL.Select(id, companyId, pricingmodelid))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new PricingZone(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static PricingZone SelectByID(int id)
        {
            PricingZone result = null;
            using (SqlDataReader dr = PricingZoneDAL.SelectByID(id))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new PricingZone(dr);
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = PricingZoneDAL.Insert(CompanyID, Title, Description, EncodedZone, PricingModelID, OriginCharge, EntryCharge, DestinationCharge, PricePerMile, WaitingCharge);
            if (ID == -1) return false;

            if (PricingZoneInserted != null) PricingZoneInserted(this, new HubEventArgs(CompanyID, 0));
            return true;
        }

        public bool Update()
        {
            if (PricingZoneDAL.Update(ID, CompanyID, Title, Description, EncodedZone, PricingModelID, OriginCharge, EntryCharge, DestinationCharge, PricePerMile, WaitingCharge))
            {
                if (PricingZoneUpdated != null) PricingZoneUpdated(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        public bool Delete()
        {
            if (PricingZoneDAL.Delete(ID))
            {
                if (PricingZoneDeleted != null) PricingZoneDeleted(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        #endregion

        #region Methods

        public bool ContainsPoint(Point point)
        {
            return ZonePolygon.ContainsPoint(point);
        }

        public bool ContainsPoint(decimal latitude, decimal longitude)
        {
            return ZonePolygon.ContainsPoint(latitude, longitude);
        }

        public decimal Area()
        {
            return ZonePolygon.Area();
        }


        #endregion

    }
}