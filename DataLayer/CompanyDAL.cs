using Cab9.DataLayer.Common;
using e9.Debugging;
using System;
using System.Data.SqlClient;

namespace Cab9.DataLayer
{
    public class CompanyDAL : DAL
    {
        public static SqlDataReader Select(int? ID = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Companys_Select", parameters);
        }

        public static SqlDataReader SelectByID(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Companys_SelectByID", parameters);
        }

        public static int Insert(string Name, int? DefaultPricingModelID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("Name", Name),                	
                    new SqlParameter("DefaultPricingModelID", DefaultPricingModelID)
                };
            object result;
            try
            {
                result = SqlHelper.ExecuteScalar(ConnectionString, "Companys_Insert", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return -1;
            }
            return Convert.ToInt32(result);
        }

        public static bool Update(int ID, string Name, int? DefaultPricingModelID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("Name", Name),                	
                    new SqlParameter("DefaultPricingModelID",DefaultPricingModelID)
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "Companys_Update", parameters);
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
                SqlHelper.ExecuteNonQuery(ConnectionString, "Companys_Delete", parameters);
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