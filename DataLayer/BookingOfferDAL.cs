using Cab9.DataLayer.Common;
using e9.Debugging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace Cab9.DataLayer
{
    public class BookingOfferDAL : DAL
    {
        public static SqlDataReader Select(long? ID = null, int? CompanyID = null, int? DriverID = null, long? BookingID = null, bool? Response = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("DriverID", DriverID),          
                    new SqlParameter("BookingID", BookingID),               
                    new SqlParameter("Response", Response)                
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Booking_Offers_Select", parameters);
        }

        public static SqlDataReader SelectByID(long ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Booking_Offers_SelectByID", parameters);
        }

        //TODO: Make sure all data layer inserts and updates parameters are not optional when allow nulls not set in db
        public static int Insert(int CompanyID, DateTime OfferDateTime, int DriverID, long BookingID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID", CompanyID),
					new SqlParameter("OfferDateTime",OfferDateTime),
					new SqlParameter("DriverID",DriverID),
					new SqlParameter("BookingID",BookingID)
                };
            object result;
            try
            {
                result = SqlHelper.ExecuteScalar(ConnectionString, "Booking_Offers_Insert", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return -1;
            }
            return Convert.ToInt32(result);
        }

        public static bool Update(long ID, bool Response, string Reason)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("Response", Response),
                    new SqlParameter("Reason", Reason),
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "Booking_Offers_Update", parameters);
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