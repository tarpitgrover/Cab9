using Cab9.DataLayer.Common;
using e9.Debugging;
using System;
using System.Data.SqlClient;

namespace Cab9.DataLayer
{
    public class VehicleTypeDAL : DAL
    {
        public static SqlDataReader Select(int? ID = null, int? CompanyID = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID)   
                };
            return SqlHelper.ExecuteReader(ConnectionString, "VehicleTypes_Select", parameters);
        }

        public static SqlDataReader SelectByID(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "VehicleTypes_SelectByID", parameters);
        }

        public static int Insert(int CompanyID, string Name, string Description, decimal BaseMultiplier)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("Name", Name),                	
					new SqlParameter("Description", Description),            
					new SqlParameter("BaseMultiplier", BaseMultiplier)      
                };
            object result;
            try
            {
                result = SqlHelper.ExecuteScalar(ConnectionString, "VehicleTypes_Insert", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return -1;
            }
            return Convert.ToInt32(result);
        }

        public static bool Update(int ID, int CompanyID, string Name, string Description, decimal BaseMultiplier)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("Name", Name),                	
					new SqlParameter("Description", Description),            
					new SqlParameter("BaseMultiplier", BaseMultiplier)
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "VehicleTypes_Update", parameters);
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
                SqlHelper.ExecuteNonQuery(ConnectionString, "VehicleTypes_Delete", parameters);
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