using Cab9.DataLayer.Common;
using e9.Debugging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Cab9.DataLayer
{
    public class DocumentDAL : DAL
    {
        public static SqlDataReader Select(int? ID = null, int? CompanyID = null, string DocumentType = null, string OwnerType = null, int? OwnerID = null, DateTime? ExpiryFrom = null, DateTime? ExpiryTo = null, string IdentificationNo = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("DocumentType", DocumentType),               
                    new SqlParameter("OwnerType", OwnerType),               
                    new SqlParameter("OwnerID", OwnerID),               
                    new SqlParameter("ExpiryDateFrom", ExpiryFrom),               
                    new SqlParameter("ExpiryDateTo", ExpiryTo),               
                    new SqlParameter("IdentificationNo", IdentificationNo),               
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Documents_Select", parameters);
        }

        public static SqlDataReader SelectByID(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Documents_SelectByID", parameters);
        }



        //TODO: Make sure all data layer inserts and updates parameters are not optional when allow nulls not set in db
        public static int Insert(int CompanyID, string DocumentType, string OwnerType, int OwnerID, string Name, DateTime? ExpiryDate = null, string IdentificationNo = null, string Description = null, string DocumentURL = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("DocumentType", DocumentType),               
                    new SqlParameter("OwnerType", OwnerType),               
                    new SqlParameter("OwnerID", OwnerID),               
                    new SqlParameter("ExpiryDate", ExpiryDate),               
                    new SqlParameter("Name", Name),               
                    new SqlParameter("IdentificationNo", IdentificationNo),               
                    new SqlParameter("Description", Description),               
                    new SqlParameter("DocumentURL", DocumentURL),               
                };
            object result;
            try
            {
                result = SqlHelper.ExecuteScalar(ConnectionString, "Documents_Insert", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return -1;
            }
            return Convert.ToInt32(result);
        }

        public static bool Update(int ID, int CompanyID, string DocumentType, string OwnerType, int OwnerID, string Name, DateTime? ExpiryDate = null, string IdentificationNo = null, string Description = null, string DocumentURL = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("DocumentType", DocumentType),               
                    new SqlParameter("OwnerType", OwnerType),               
                    new SqlParameter("OwnerID", OwnerID),               
                    new SqlParameter("ExpiryDate", ExpiryDate),               
                    new SqlParameter("Name", Name),               
                    new SqlParameter("IdentificationNo", IdentificationNo),               
                    new SqlParameter("Description", Description),               
                    new SqlParameter("DocumentURL", DocumentURL),               
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "Documents_Update", parameters);
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
                SqlHelper.ExecuteNonQuery(ConnectionString, "Documents_Delete", parameters);
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