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

namespace Cab9.Model
{
    public class PricingFixed
    {
        #region Properties

        public int ID { get; set; }
        public int CompanyID { get; set; }
        public int PricingModelID { get; set; }
        public int PricingZoneFromID { get; set; }
        public int PricingZoneToID { get; set; }
        public decimal Price { get; set; }

        #endregion

        #region Events

        public delegate void PricingFixedInsertedEvent(PricingFixed sender, HubEventArgs e);
        public static event PricingFixedInsertedEvent PricingFixedInserted;

        public delegate void PricingFixedUpdatedEvent(PricingFixed sender, HubEventArgs e);
        public static event PricingFixedUpdatedEvent PricingFixedUpdated;

        public delegate void PricingFixedDeletedEvent(PricingFixed sender, HubEventArgs e);
        public static event PricingFixedDeletedEvent PricingFixedDeleted;

        #endregion

        #region Constructors

        public PricingFixed()
        {
        }

        private PricingFixed(SqlDataReader row)
        {
            try
            {
                if (row["ID"] != DBNull.Value) ID = (int)row["ID"];
                if (row["CompanyID"] != DBNull.Value) CompanyID = (int)row["CompanyID"];
                if (row["PricingModelID"] != DBNull.Value) PricingModelID = (int)row["PricingModelID"];
                if (row["PricingZoneFromID"] != DBNull.Value) PricingZoneFromID = (int)row["PricingZoneFromID"];
                if (row["PricingZoneToID"] != DBNull.Value) PricingZoneToID = (int)row["PricingZoneToID"];
                if (row["Price"] != DBNull.Value) Price = (decimal)row["Price"];
     
            }
            catch (Exception exc)
            {
                SystemLog.LogNewError(exc, LogType.RetreiveError, this);
                throw exc;
            }
        }
       
        #endregion

        #region CRUD

        public static List<PricingFixed> Select(int? id = null, int? companyId = null, int? pricingModelID = null, int? pricingZoneFromID = null, int? pricingZoneToID = null)
        {
            var result = new List<PricingFixed>();
            using (SqlDataReader dr = PricingFixedDAL.Select(id, companyId, pricingModelID, pricingZoneFromID, pricingZoneToID))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new PricingFixed(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static PricingFixed SelectByID(int id)
        {
            PricingFixed result = null;
            using (SqlDataReader dr = PricingFixedDAL.SelectByID(id))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new PricingFixed(dr);
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = PricingFixedDAL.Insert(CompanyID, PricingModelID, PricingZoneFromID, PricingZoneToID, Price);
            if (ID == -1) return false;

            if (PricingFixedInserted != null) PricingFixedInserted(this, new HubEventArgs(CompanyID, 0));
            return true;
        }

        public bool Update()
        {
            if (PricingFixedDAL.Update(ID, CompanyID, PricingModelID, PricingZoneFromID, PricingZoneToID, Price))
            {
                if (PricingFixedUpdated != null) PricingFixedUpdated(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        public bool Delete()
        {
            if (PricingFixedDAL.Delete(ID))
            {
                if (PricingFixedDeleted != null) PricingFixedDeleted(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        #endregion

        #region Methods

        #endregion

    }
}