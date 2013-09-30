using Cab9.DataLayer.Common;
using e9.Debugging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Cab9.DataLayer
{
    public class PricingFixedDAL : DAL
    {
        public static SqlDataReader Select(int? ID = null, int? CompanyID = null, int? PricingModelID = null, int? PricingZoneFromID = null, int? PricingZoneToID = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),          
                    new SqlParameter("PricingModelID", PricingModelID),
                    new SqlParameter("PricingZoneFromID", PricingZoneFromID),
                    new SqlParameter("PricingZoneToID", PricingZoneToID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "PricingFixed_Select", parameters);
        }

        public static SqlDataReader SelectByID(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "PricingFixed_SelectByID", parameters);
        }

        public static int Insert(int CompanyID, int PricingModelID, int PricingZoneFromID, int PricingZoneToID, decimal Price)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID", CompanyID),          
                    new SqlParameter("PricingModelID", PricingModelID),
                    new SqlParameter("PricingZoneFromID", PricingZoneFromID),
                    new SqlParameter("PricingZoneToID", PricingZoneToID),
                    new SqlParameter("Price", Price)               
                };
            object result;
            try
            {
                result = SqlHelper.ExecuteScalar(ConnectionString, "PricingFixed_Insert", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return -1;
            }
            return Convert.ToInt32(result);
        }

        public static bool Update(int ID, int CompanyID, int PricingModelID, int PricingZoneFromID, int PricingZoneToID, decimal Price)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),          
                    new SqlParameter("PricingModelID", PricingModelID),
                    new SqlParameter("PricingZoneFromID", PricingZoneFromID),
                    new SqlParameter("PricingZoneToID", PricingZoneToID),
                    new SqlParameter("Price", Price)
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "PricingFixed_Update", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
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
                SqlHelper.ExecuteNonQuery(ConnectionString, "PricingFixed_Delete", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return false;
            }
            return true;
        }

    }
}