using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Cab9.DataLayer;
using Cab9.EventHandlers.Common;
using e9.Debugging;

namespace Cab9.Model
{
    public class DriverType
    {
        #region Properties

        public int ID { get; set; }
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal OwnCarCashCommission { get; set; }
        public decimal OwnCarAccountCommission { get; set; }
        public decimal CompanyCarCashCommission { get; set; }
        public decimal CompanyCarAccountCommission { get; set; }
        public decimal WeeklyRent { get; set; }

        #endregion

        #region Events

        public delegate void DriverTypeInsertedEvent(DriverType sender, HubEventArgs e);
        public static event DriverTypeInsertedEvent DriverTypeInserted;

        public delegate void DriverTypeUpdatedEvent(DriverType sender, HubEventArgs e);
        public static event DriverTypeUpdatedEvent DriverTypeUpdated;

        public delegate void DriverTypeDeletedEvent(DriverType sender, HubEventArgs e);
        public static event DriverTypeDeletedEvent DriverTypeDeleted;

        #endregion
        
        #region Constructors

        public DriverType()
        {
        }

        private DriverType(SqlDataReader row)
        {
            try
            {
                if (row["ID"] != DBNull.Value) { this.ID = (int)(row["ID"]); }
                if (row["CompanyID"] != DBNull.Value) { this.CompanyID = (int)(row["CompanyID"]); }
                if (row["Name"] != DBNull.Value) { this.Name = (string)(row["Name"]); }
                if (row["Description"] != DBNull.Value) { this.Description = (string)(row["Description"]); }
                if (row["OwnCarCashCommission"] != DBNull.Value) { this.OwnCarCashCommission = (decimal)(row["OwnCarCashCommission"]); }
                if (row["OwnCarAccountCommission"] != DBNull.Value) { this.OwnCarAccountCommission = (decimal)(row["OwnCarAccountCommission"]); }
                if (row["CompanyCarCashCommission"] != DBNull.Value) { this.CompanyCarCashCommission = (decimal)(row["CompanyCarCashCommission"]); }
                if (row["CompanyCarAccountCommission"] != DBNull.Value) { this.CompanyCarAccountCommission = (decimal)(row["CompanyCarAccountCommission"]); }
                if (row["WeeklyRent"] != DBNull.Value) { this.WeeklyRent = (decimal)(row["WeeklyRent"]); }
            }
            catch (Exception exc)
            {
                SystemLog.LogNewError(exc, LogType.RetreiveError, this);
                throw exc;
            }
        }
       
        #endregion

        #region CRUD

        public static List<DriverType> Select(int? id = null, int? companyId = null)
        {
            var result = new List<DriverType>();
            using (SqlDataReader dr = DriverTypeDAL.Select(id, companyId))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new DriverType(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static DriverType SelectByID(int id)
        {
            DriverType result = null;
            using (SqlDataReader dr = DriverTypeDAL.SelectByID(id))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new DriverType(dr);
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = DriverTypeDAL.Insert(CompanyID, Name, Description, OwnCarCashCommission, OwnCarAccountCommission, CompanyCarCashCommission, CompanyCarAccountCommission, WeeklyRent);
            if (ID == -1) return false;

            if (DriverTypeInserted != null) DriverTypeInserted(this, new HubEventArgs(CompanyID, 0));
            return true;
        }

        public bool Update()
        {
            if (DriverTypeDAL.Update(ID, CompanyID, Name, Description, OwnCarCashCommission, OwnCarAccountCommission, CompanyCarCashCommission, CompanyCarAccountCommission, WeeklyRent))
            {
                if (DriverTypeUpdated != null) DriverTypeUpdated(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        public bool Delete()
        {
            if (DriverTypeDAL.Delete(ID))
            {
                if (DriverTypeDeleted != null) DriverTypeDeleted(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        #endregion

        #region Methods

        #endregion
    }
}