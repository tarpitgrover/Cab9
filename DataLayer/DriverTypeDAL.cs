using Cab9.DataLayer.Common;
using e9.Debugging;
using System;
using System.Data.SqlClient;

namespace Cab9.DataLayer
{
    public class DriverTypeDAL : DAL
    {
        public static SqlDataReader Select(int? ID = null, int? CompanyID = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID)   
                };
            return SqlHelper.ExecuteReader(ConnectionString, "DriverTypes_Select", parameters);
        }

        public static SqlDataReader SelectByID(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "DriverTypes_SelectByID", parameters);
        }

        public static int Insert(int CompanyID, string Name, string Description, decimal OwnCarCashCommission, decimal OwnCarAccountCommission, decimal CompanyCarCashCommission, decimal CompanyCarAccountCommission, decimal WeeklyRent)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("Name", Name),                	
					new SqlParameter("Description", Description),            
					new SqlParameter("OwnCarCashCommission", OwnCarCashCommission),      
					new SqlParameter("OwnCarCashCommission", OwnCarAccountCommission),      
					new SqlParameter("OwnCarCashCommission", CompanyCarCashCommission),      
					new SqlParameter("OwnCarCashCommission", CompanyCarAccountCommission),      
					new SqlParameter("OwnCarCashCommission", WeeklyRent)      
                };
            object result;
            try
            {
                result = SqlHelper.ExecuteScalar(ConnectionString, "DriverTypes_Insert", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return -1;
            }
            return Convert.ToInt32(result);
        }

        public static bool Update(int ID, int CompanyID, string Name, string Description, decimal OwnCarCashCommission, decimal OwnCarAccountCommission, decimal CompanyCarCashCommission, decimal CompanyCarAccountCommission, decimal WeeklyRent)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("Name", Name),                	
					new SqlParameter("Description", Description),            
					new SqlParameter("OwnCarCashCommission", OwnCarCashCommission),      
					new SqlParameter("OwnCarCashCommission", OwnCarAccountCommission),      
					new SqlParameter("OwnCarCashCommission", CompanyCarCashCommission),      
					new SqlParameter("OwnCarCashCommission", CompanyCarAccountCommission),      
					new SqlParameter("OwnCarCashCommission", WeeklyRent)  
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "DriverTypes_Update", parameters);
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
                SqlHelper.ExecuteNonQuery(ConnectionString, "DriverTypes_Delete", parameters);
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