using Cab9.DataLayer.Common;
using e9.Debugging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Cab9.DataLayer
{
    public class PricingZoneDAL : DAL
    {
        public static SqlDataReader Select(int? ID = null, int? CompanyID = null, int? PricingModelID = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),               
                    new SqlParameter("PricingModelID", PricingModelID)               
                };
            return SqlHelper.ExecuteReader(ConnectionString, "PricingZone_Select", parameters);
        }

        public static SqlDataReader SelectByID(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "PricingZone_SelectByID", parameters);
        }

        public static int Insert(int CompanyID, string Title, string Description, string EncodedZone, int PricingModelID, decimal? OriginCharge, decimal? EntryCharge, decimal? DestinationCharge, decimal? PricePerMile, decimal? WaitingCharge)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("Title", Title),               
                    new SqlParameter("Description", Description),               
                    new SqlParameter("EncodedZone", EncodedZone),               
                    new SqlParameter("PricingModelID", PricingModelID),               
                    new SqlParameter("OriginCharge", OriginCharge),               
                    new SqlParameter("EntryCharge", EntryCharge),               
                    new SqlParameter("DestinationCharge", DestinationCharge),               
                    new SqlParameter("PricePerMile", PricePerMile),               
                    new SqlParameter("WaitingCharge", WaitingCharge)            
                };
            object result;
            try
            {
                result = SqlHelper.ExecuteScalar(ConnectionString, "PricingZone_Insert", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return -1;
            }
            return Convert.ToInt32(result);
        }

        public static bool Update(int ID, int CompanyID, string Title, string Description, string EncodedZone, int PricingModelID, decimal? OriginCharge, decimal? EntryCharge, decimal? DestinationCharge, decimal? PricePerMile, decimal? WaitingCharge)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("Title", Title),               
                    new SqlParameter("Description", Description),               
                    new SqlParameter("EncodedZone", EncodedZone),               
                    new SqlParameter("PricingModelID", PricingModelID),               
                    new SqlParameter("OriginCharge", OriginCharge),               
                    new SqlParameter("EntryCharge", EntryCharge),               
                    new SqlParameter("DestinationCharge", DestinationCharge),               
                    new SqlParameter("PricePerMile", PricePerMile),               
                    new SqlParameter("WaitingCharge", WaitingCharge)            
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "PricingZone_Update", parameters);
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
                SqlHelper.ExecuteNonQuery(ConnectionString, "PricingZone_Delete", parameters);
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