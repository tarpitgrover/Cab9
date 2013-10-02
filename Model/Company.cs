using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Cab9.DataLayer;
using Cab9.EventHandlers.Common;
using e9.Debugging;

namespace Cab9.Model
{
    public class Company
    {
        #region Properties

        public int ID { get; set; }
        public string Name { get; set; }
        public int? DefaultPricingModelID { get; set; }

        public int DistanceModifier { get; set; }
        public int ClearingModifier { get; set; }
        public int ShiftBookingsModifier { get; set; }
        public int CapacityOverModifier { get; set; }
        public int CapacityUnderModifier { get; set; }
        public int RankModifier { get; set; }


        #endregion

        #region Events

        public delegate void CompanyInsertedEvent(Company sender, HubEventArgs e);
        public static event CompanyInsertedEvent CompanyInserted;

        public delegate void CompanyUpdatedEvent(Company sender, HubEventArgs e);
        public static event CompanyUpdatedEvent CompanyUpdated;

        public delegate void CompanyDeletedEvent(Company sender, HubEventArgs e);
        public static event CompanyDeletedEvent CompanyDeleted;

        #endregion
        
        #region Constructors

        public Company()
        {
        }

        private Company(SqlDataReader row)
        {
            try
            {
                if (row["ID"] != DBNull.Value) { this.ID = (int)(row["ID"]); }
                if (row["Name"] != DBNull.Value) { this.Name = (string)(row["Name"]); }
                if (row["DefaultPricingModelID"] != DBNull.Value) { this.DefaultPricingModelID = (int?)(row["DefaultPricingModelID"]); }
            }
            catch (Exception exc)
            {
                SystemLog.LogNewError(exc, LogType.RetreiveError, this);
                throw exc;
            }
        }
       
        #endregion

        #region CRUD

        public static List<Company> Select(int? id = null)
        {
            var result = new List<Company>();
            using (SqlDataReader dr = CompanyDAL.Select(id))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new Company(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static Company SelectByID(int id)
        {
#if DEBUG
            if (id == 1) return new Company() { 
                ID = 1,
                DefaultPricingModelID = 7,
                ClearingModifier = 100,
                DistanceModifier = 50,
                ShiftBookingsModifier = 15,
                CapacityOverModifier = 10,
                CapacityUnderModifier = 2000
            };
#endif
            Company result = null;
            using (SqlDataReader dr = CompanyDAL.SelectByID(id))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new Company(dr);
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = CompanyDAL.Insert(Name, DefaultPricingModelID);
            if (ID == -1) return false;

            if (CompanyInserted != null) CompanyInserted(this, new HubEventArgs(ID, 0));
            return true;
        }

        public bool Update()
        {
            if (CompanyDAL.Update(ID, Name, DefaultPricingModelID))
            {
                if (CompanyUpdated != null) CompanyUpdated(this, new HubEventArgs(ID, 0));
                return true;
            }
            return false;
        }

        public bool Delete()
        {
            if (CompanyDAL.Delete(ID))
            {
                if (CompanyDeleted != null) CompanyDeleted(this, new HubEventArgs(ID, 0));
                return true;
            }
            return false;
        }

        #endregion

        #region Methods

        #endregion
    }
}