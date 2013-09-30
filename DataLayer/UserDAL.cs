using Cab9.DataLayer.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Cab9.DataLayer
{
    internal class UserDAL : DAL
    {
        public static SqlDataReader Select(int? id = null, int? companyId = null, string mobile = null, string email = null, bool? active = null)
        {
            var Parameters = new SqlParameter[]
            {
                new SqlParameter("ID", id),
                new SqlParameter("CompanyID", companyId),
                new SqlParameter("Mobile", mobile),
                new SqlParameter("Email", email),
                new SqlParameter("Active", active)
            };
            return SqlHelper.ExecuteReader(ConnectionString, "Cab9_Users_Select", Parameters);
        }

        public static SqlDataReader SelectByID(int id)
        {
            var Parameters = new SqlParameter[]
            {
                new SqlParameter("ID", id)
            };
            return SqlHelper.ExecuteReader(ConnectionString, "Cab9_Users_SelectByID", Parameters);
        }

        public static SqlDataReader SelectByEntity(string type, int id)
        {
            var Parameters = new SqlParameter[]
            {
                new SqlParameter("Type", type),
                new SqlParameter("ID", id)
            };
            return SqlHelper.ExecuteReader(ConnectionString, "Cab9_Users_SelectByEntity", Parameters);
        }

        public static SqlDataReader SelectByEmail(string email)
        {
            var Parameters = new SqlParameter[]
            {
                new SqlParameter("Email", email)
            };
            return SqlHelper.ExecuteReader(ConnectionString, "Cab9_Users_SelectByEmail", Parameters);
        }

        public static int Insert(string appName, int companyId, string name, string mobile, string email, string hashWord, bool active, string inactiveReason, DateTime? lastLogin, int? loginAttempts, string unlockCode, string imageUrl, string address1, string address2, string area, string town, string postcode, string entityType, int? entityId)
        {
            var Parameters = new SqlParameter[]
            {
                new SqlParameter("ApplicationName", appName),
                new SqlParameter("CompanyID", companyId),
                new SqlParameter("Name", name),
                new SqlParameter("Mobile", mobile),
                new SqlParameter("Email", email.ToLower()),
                new SqlParameter("Hashword", hashWord),
                new SqlParameter("Active", active),
                new SqlParameter("InactiveReason", inactiveReason),
                new SqlParameter("LastLogin", lastLogin),
                new SqlParameter("LoginAttempts", loginAttempts),
                new SqlParameter("UnlockCode", unlockCode),
                new SqlParameter("ImageURL", imageUrl),
                new SqlParameter("Address1", address1),                	
				new SqlParameter("Address2", address2),                	
				new SqlParameter("Area", area),                        	
				new SqlParameter("Town", town),                        
				new SqlParameter("Postcode", postcode),             
				new SqlParameter("EntityType", entityType),                        
				new SqlParameter("EntityID", entityId)             
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, "Cab9_Users_Insert", Parameters));
        }

        public static bool Update(int id, string appName, int companyId, string name, string mobile, string email, string hashWord, bool active, string inactiveReason, DateTime? lastLogin, int? loginAttempts, string unlockCode, string imageUrl, string address1, string address2, string area, string town, string postcode, string entityType, int? entityId)
        {
            var Parameters = new SqlParameter[]
            {
                new SqlParameter("ID", id),
                new SqlParameter("ApplicationName", appName),
                new SqlParameter("CompanyID", companyId),
                new SqlParameter("Name", name),
                new SqlParameter("Mobile", mobile),
                new SqlParameter("Email", email.ToLower()),
                new SqlParameter("Hashword", hashWord),
                new SqlParameter("Active", active),
                new SqlParameter("InactiveReason", inactiveReason),
                new SqlParameter("LastLogin", lastLogin),
                new SqlParameter("LoginAttempts", loginAttempts),
                new SqlParameter("UnlockCode", unlockCode),
                new SqlParameter("ImageURL", imageUrl),
                new SqlParameter("Address1", address1),                	
				new SqlParameter("Address2", address2),                	
				new SqlParameter("Area", area),                        	
				new SqlParameter("Town", town),                        
				new SqlParameter("Postcode", postcode),             
				new SqlParameter("EntityType", entityType),                        
				new SqlParameter("EntityID", entityId)             
            };
            SqlHelper.ExecuteNonQuery(ConnectionString, "Cab9_Users_Update", Parameters);
            return true;
        }

        public static bool Delete(int id)
        {
            var Parameters = new SqlParameter[]
            {
                new SqlParameter("ID", id),
            };
            SqlHelper.ExecuteNonQuery(ConnectionString, "Cab9_Users_Delete", Parameters);
            return true;
        }
    }
}
