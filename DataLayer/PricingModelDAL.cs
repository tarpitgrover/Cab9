using Cab9.DataLayer.Common;
using e9.Debugging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Cab9.DataLayer
{
    public class PricingModelDAL : DAL
    {
        public static SqlDataReader Select(int? ID = null, int? CompanyID = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID)               
                };
            return SqlHelper.ExecuteReader(ConnectionString, "PricingModels_Select", parameters);
        }

        public static SqlDataReader SelectByID(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "PricingModels_SelectByID", parameters);
        }

        public static int Insert(int CompanyID, string Name, string Description, int ZoneMode, bool UseSizeModifiers, decimal PricePerMile, decimal StandingCharge, decimal MinimumCharge, short RoundTo, TimeSpan PeakStart, TimeSpan PeakEnd, decimal PeakMultiplier, decimal WaitingCharge, decimal WaitingPeriod)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("Name", Name),               
                    new SqlParameter("Description", Description),               
                    new SqlParameter("ZoneMode", ZoneMode),               
                    new SqlParameter("UseSizeModifiers", UseSizeModifiers),               
                    new SqlParameter("PricePerMile", PricePerMile),               
                    new SqlParameter("StandingCharge", StandingCharge),               
                    new SqlParameter("MinimumCharge", MinimumCharge),               
                    new SqlParameter("RoundTo", RoundTo),               
                    new SqlParameter("PeakStart", PeakStart),               
                    new SqlParameter("PeakEnd", PeakEnd),               
                    new SqlParameter("PeakMultiplier", PeakMultiplier),               
                    new SqlParameter("WaitingCharge", WaitingCharge),               
                    new SqlParameter("WaitPeriod", WaitingPeriod)               
                };
            object result;
            try
            {
                result = SqlHelper.ExecuteScalar(ConnectionString, "PricingModels_Insert", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return -1;
            }
            return Convert.ToInt32(result);
        }

        public static bool Update(int ID, int CompanyID, string Name, string Description, int ZoneMode, bool UseSizeModifiers, decimal PricePerMile, decimal StandingCharge, decimal MinimumCharge, short RoundTo, TimeSpan PeakStart, TimeSpan PeakEnd, decimal PeakMultiplier, decimal WaitingCharge, decimal WaitingPeriod)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("Name", Name),               
                    new SqlParameter("Description", Description),               
                    new SqlParameter("ZoneMode", ZoneMode),               
                    new SqlParameter("UseSizeModifiers", UseSizeModifiers),               
                    new SqlParameter("PricePerMile", PricePerMile),               
                    new SqlParameter("StandingCharge", StandingCharge),               
                    new SqlParameter("MinimumCharge", MinimumCharge),               
                    new SqlParameter("RoundTo", RoundTo),               
                    new SqlParameter("PeakStart", PeakStart),               
                    new SqlParameter("PeakEnd", PeakEnd),               
                    new SqlParameter("PeakMultiplier", PeakMultiplier),               
                    new SqlParameter("WaitingCharge", WaitingCharge),               
                    new SqlParameter("WaitPeriod", WaitingPeriod)               
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "PricingModels_Update", parameters);
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
                SqlHelper.ExecuteNonQuery(ConnectionString, "PricingModels_Delete", parameters);
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