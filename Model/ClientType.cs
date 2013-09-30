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
    public class ClientType
    {
        #region Properties

        public int ID { get; set; }
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? DefaultPricingModelID { get; set; }

        #endregion

        #region Events

        public delegate void ClientTypeInsertedEvent(ClientType sender, HubEventArgs e);
        public static event ClientTypeInsertedEvent ClientTypeInserted;

        public delegate void ClientTypeUpdatedEvent(ClientType sender, HubEventArgs e);
        public static event ClientTypeUpdatedEvent ClientTypeUpdated;

        public delegate void ClientTypeDeletedEvent(ClientType sender, HubEventArgs e);
        public static event ClientTypeDeletedEvent ClientTypeDeleted;

        #endregion

        #region Constructors

        public ClientType()
        {
        }

        private ClientType(SqlDataReader row)
        {
            try
            {
                if (row["ID"] != DBNull.Value) { this.ID = (int)(row["ID"]); }
                if (row["CompanyID"] != DBNull.Value) { this.CompanyID = (int)(row["CompanyID"]); }
                if (row["Name"] != DBNull.Value) { this.Name = (string)(row["Name"]); }
                if (row["Description"] != DBNull.Value) { this.Description = (string)row["Description"]; }
                if (row["DefaultPricingModelID"] != DBNull.Value) { this.DefaultPricingModelID = (int)row["DefaultPricingModelID"]; }
            }
            catch (Exception exc)
            {
                SystemLog.LogNewError(exc, LogType.RetreiveError, this);
                throw exc;
            }
        }
       
        #endregion

        #region CRUD

        public static List<ClientType> Select(int? id = null, int? companyId = null)
        {
            var result = new List<ClientType>();
            using (SqlDataReader dr = ClientTypeDAL.Select(id, companyId))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new ClientType(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static ClientType SelectByID(int id)
        {
            ClientType result = null;
            using (SqlDataReader dr = ClientTypeDAL.SelectByID(id))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new ClientType(dr);
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = ClientTypeDAL.Insert(CompanyID, Name, Description, DefaultPricingModelID);
            if (ID == -1) return false;

            if (ClientTypeInserted != null) ClientTypeInserted(this, new HubEventArgs(CompanyID, 0));
            return true;
        }

        public bool Update()
        {
            if (ClientTypeDAL.Update(ID, CompanyID, Name, Description, DefaultPricingModelID))
            {
                if (ClientTypeUpdated != null) ClientTypeUpdated(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        public bool Delete()
        {
            if (ClientTypeDAL.Delete(ID))
            {
                if (ClientTypeDeleted != null) ClientTypeDeleted(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        #endregion

        #region Methods

        #endregion

    }
}