using Cab9.DataLayer.Common;
using e9.Debugging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Cab9.DataLayer
{
    public class LocationDAL : DAL
    {
        public static SqlDataReader Select(int? ID = null, int? CompanyID = null, string Search = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("SearchTerm", Search)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Locations_Select", parameters);
        }

        public static SqlDataReader SelectByID(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Locations_SelectByID", parameters);
        }



        //TODO: Make sure all data layer inserts and updates parameters are not optional when allow nulls not set in db
        public static int Insert(int CompanyID, string Name, string Type, decimal? Latitude, decimal? Longitude, string Searchable, string Note)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("Name", Name),               
                    new SqlParameter("Type", Type),               
                    new SqlParameter("Latitude", Latitude),               
                    new SqlParameter("Longitude", Longitude),
                    new SqlParameter("Searchable", Searchable),
                    new SqlParameter("Note", Note)
                };
            object result;
            try
            {
                result = SqlHelper.ExecuteScalar(ConnectionString, "Locations_Insert", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                return -1;
            }
            return Convert.ToInt32(result);
        }

        public static bool Update(int ID, int CompanyID, string Name, string Type, decimal? Latitude, decimal? Longitude, string Searchable, string Note)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("Name", Name),               
                    new SqlParameter("Type", Type),               
                    new SqlParameter("Latitude", Latitude),               
                    new SqlParameter("Longitude", Longitude),
                    new SqlParameter("Searchable", Searchable),
                    new SqlParameter("Note", Note)
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "Locations_Update", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                return false;
            }
            return true;
        }

        public static bool Delete(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "Locations_Delete", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                return false;
            }
            return true;
        }

    }
}