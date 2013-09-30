using Cab9.DataLayer.Common;
using e9.Debugging;
using System;
using System.Data.SqlClient;

namespace Cab9.DataLayer
{
    public class ClientTypeDAL : DAL
    {
        public static SqlDataReader Select(int? ID = null, int? CompanyID = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID)   
                };
            return SqlHelper.ExecuteReader(ConnectionString, "ClientTypes_Select", parameters);
        }

        public static SqlDataReader SelectByID(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "ClientTypes_SelectByID", parameters);
        }

        public static int Insert(int CompanyID, string Name, string Description, int? DefaultPricingModelID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("Name", Name),                	
					new SqlParameter("Description", Description),
                    new SqlParameter("DefaultPricingModelID", DefaultPricingModelID)
                };
            object result;
            try
            {
                result = SqlHelper.ExecuteScalar(ConnectionString, "ClientTypes_Insert", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return -1;
            }
            return Convert.ToInt32(result);
        }

        public static bool Update(int ID, int CompanyID, string Name, string Description, int? DefaultPricingModelID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("Name", Name),                	
					new SqlParameter("Description", Description),      
                    new SqlParameter("DefaultPricingModelID",DefaultPricingModelID)
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "ClientTypes_Update", parameters);
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
                SqlHelper.ExecuteNonQuery(ConnectionString, "ClientTypes_Delete", parameters);
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