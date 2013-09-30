using Cab9.DataLayer.Common;
using e9.Debugging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Cab9.DataLayer
{
    public class NoteDAL : DAL
    {
        public static SqlDataReader Select(int? ID = null, int? CompanyID = null, string OwnerType = null, int? OwnerID = null, DateTime? @TimeStampFrom = null, DateTime? @TimeStampTo = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("OwnerType", OwnerType),               
                    new SqlParameter("OwnerID", OwnerID),               
                    new SqlParameter("TimeStampFrom", TimeStampFrom),               
                    new SqlParameter("TimeStampTo", TimeStampTo)                           
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Notes_Select", parameters);
        }

        public static SqlDataReader SelectByID(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Notes_SelectByID", parameters);
        }



        //TODO: Make sure all data layer inserts and updates parameters are not optional when allow nulls not set in db
        public static int Insert(int CompanyID, string OwnerType, int OwnerID, string Body, DateTime TimeStamp, int AddedByID, int? EditedByID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("OwnerType", OwnerType),               
                    new SqlParameter("OwnerID", OwnerID),               
                    new SqlParameter("Body", Body),               
                    new SqlParameter("TimeStamp", TimeStamp),
                    new SqlParameter("AddedByID", AddedByID),
                    new SqlParameter("EditedByID", EditedByID)
                };
            object result;
            try
            {
                result = SqlHelper.ExecuteScalar(ConnectionString, "Notes_Insert", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                return -1;
            }
            return Convert.ToInt32(result);
        }

        public static bool Update(int ID, int CompanyID, string OwnerType, int OwnerID, string Body, DateTime TimeStamp, int AddedByID, int? EditedByID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("OwnerType", OwnerType),               
                    new SqlParameter("OwnerID", OwnerID),               
                    new SqlParameter("Body", Body),               
                    new SqlParameter("TimeStamp", TimeStamp),
                    new SqlParameter("AddedByID", AddedByID),
                    new SqlParameter("EditedByID", EditedByID)
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "Notes_Update", parameters);
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
                SqlHelper.ExecuteNonQuery(ConnectionString, "Notes_Delete", parameters);
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