using Cab9.DataLayer;
using Cab9.EventHandlers.Common;
using Cab9.Model.Common;
using e9.Debugging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Cab9.Model
{
    public class Location
    {
        #region Properties

        public int ID { get; set; }
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Searchable { get; set; }
        public string Note { get; set; }

        #endregion

        #region Constructors

        public Location()
        {
        }

        private Location(SqlDataReader row)
        {
            try
            {
                if (row["ID"] != DBNull.Value) ID = (int)row["ID"];
                if (row["CompanyID"] != DBNull.Value) CompanyID = (int)row["CompanyID"];
                if (row["Name"] != DBNull.Value) Name = (string)row["Name"];
                if (row["Type"] != DBNull.Value) Type = (string)row["Type"];
                if (row["Latitude"] != DBNull.Value) Latitude = (decimal)row["Latitude"];
                if (row["Longitude"] != DBNull.Value) Longitude = (decimal)row["Longitude"];
                if (row["Searchable"] != DBNull.Value) Searchable = (string)row["Searchable"];
                if (row["Note"] != DBNull.Value) Note = (string)row["Note"];
                 
            }
            catch (Exception exc)
            {
                SystemLog.LogNewError(exc, LogType.RetreiveError, this);
                throw exc;
            }
        }
       
        #endregion

        #region CRUD

        public static List<Location> Select(int? id = null, int? companyId = null, string search = null)
        {
            var result = new List<Location>();
            using (SqlDataReader dr = LocationDAL.Select(id, companyId, search))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new Location(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static Location SelectByID(int id)
        {
            Location result = null;
            using (SqlDataReader dr = LocationDAL.SelectByID(id))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new Location(dr);
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = LocationDAL.Insert(CompanyID, Name, Type, Latitude, Longitude, Searchable, Note);
            if (ID == -1) return false;

            return true;
        }

        public bool Update()
        {
            if (LocationDAL.Update(ID, CompanyID, Name, Type, Latitude, Longitude, Searchable, Note))
            {
                return true;
            }
            return false;
        }

        public bool Delete()
        {
            if (LocationDAL.Delete(ID))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Methods

        public bool Search(string text)
        {
            if (Name.ToLower().Contains(text.ToLower())) return true;
            if (Type.ToLower().Contains(text.ToLower())) return true;
            if (Searchable.ToLower().Contains(text.ToLower())) return true;
            return false;
        }

        #endregion
    }
}