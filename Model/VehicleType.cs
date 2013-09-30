using Cab9.DataLayer;
using Cab9.EventHandlers.Common;
using e9.Debugging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Cab9.Model
{
    public class VehicleType
    {
        #region Properties

        public int ID { get; set; }
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BaseMultiplier { get; set; }

        #endregion

        #region Events

        public delegate void VehicleTypeInsertedEvent(VehicleType sender, HubEventArgs e);
        public static event VehicleTypeInsertedEvent VehicleTypeInserted;

        public delegate void VehicleTypeUpdatedEvent(VehicleType sender, HubEventArgs e);
        public static event VehicleTypeUpdatedEvent VehicleTypeUpdated;

        public delegate void VehicleTypeDeletedEvent(VehicleType sender, HubEventArgs e);
        public static event VehicleTypeDeletedEvent VehicleTypeDeleted;

        #endregion

        #region Constructors

        public VehicleType()
        {
        }

        private VehicleType(SqlDataReader row)
        {
            try
            {
                if (row["ID"] != DBNull.Value) { this.ID = (int)(row["ID"]); }
                if (row["CompanyID"] != DBNull.Value) { this.CompanyID = (int)(row["CompanyID"]); }
                if (row["Name"] != DBNull.Value) { this.Name = (string)(row["Name"]); }
                if (row["Description"] != DBNull.Value) { this.Description = (string)row["Description"]; }
                if (row["BaseMultiplier"] != DBNull.Value) { this.BaseMultiplier = (decimal)(row["BaseMultiplier"]); }
            }
            catch (Exception exc)
            {
                SystemLog.LogNewError(exc, LogType.RetreiveError, this);
                throw exc;
            }
        }
       
        #endregion

        #region CRUD

        public static List<VehicleType> Select(int? id = null, int? companyId = null)
        {
            var result = new List<VehicleType>();
            using (SqlDataReader dr = VehicleTypeDAL.Select(id, companyId))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new VehicleType(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static VehicleType SelectByID(int id)
        {
            VehicleType result = null;
            using (SqlDataReader dr = VehicleTypeDAL.SelectByID(id))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new VehicleType(dr);
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = VehicleTypeDAL.Insert(CompanyID, Name, Description, BaseMultiplier);
            if (ID == -1) return false;

            if (VehicleTypeInserted != null) VehicleTypeInserted(this, new HubEventArgs(CompanyID, 0));
            return true;
        }

        public bool Update()
        {
            if (VehicleTypeDAL.Update(ID, CompanyID, Name, Description, BaseMultiplier))
            {
                if (VehicleTypeUpdated != null) VehicleTypeUpdated(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        public bool Delete()
        {
            if (VehicleTypeDAL.Delete(ID))
            {
                if (VehicleTypeDeleted != null) VehicleTypeDeleted(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        #endregion

        #region Methods

        #endregion

    }
}