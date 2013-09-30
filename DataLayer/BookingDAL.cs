using Cab9.DataLayer.Common;
using e9.Debugging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Cab9.DataLayer
{
    public class BookingDAL : DAL
    {
            
        public static SqlDataReader Select(long? ID = null, int? CompanyID = null, int? Priority = null, int? Status = null, int? InvoiceID = null, long? ShiftID = null, DateTime? BookedFrom = null, DateTime? BookedTo = null, int? ClientID = null, int? CarType = null, int? DriverID = null, int? VehicleID = null, int? PaymentMethod = null, int? BookedBy = null, int? EditedBy = null, DateTime? LeadFrom = null, DateTime? LeadTo = null, bool? AutoDispatch = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),           
					new SqlParameter("Priority", Priority),              	
					new SqlParameter("Status", Status),                 	
					new SqlParameter("InvoiceID", InvoiceID),                 	
					new SqlParameter("ShiftID", ShiftID),                 	
					new SqlParameter("BookedFrom", BookedFrom),                 	
					new SqlParameter("BookedTo", BookedTo),                 	
					new SqlParameter("ClientID", ClientID),                 	
					new SqlParameter("CarType", CarType),                 	
					new SqlParameter("DriverID", DriverID),                 	
					new SqlParameter("VehicleID", VehicleID),                 	
					new SqlParameter("PaymentMethod", PaymentMethod),                 	
					new SqlParameter("BookedBy", BookedBy),                 	
					new SqlParameter("EditedBy", EditedBy),                 	
					new SqlParameter("LeadFrom", LeadFrom),                 	
					new SqlParameter("LeadTo", LeadTo),                 	
					new SqlParameter("AutoDispatch", AutoDispatch)             	                	
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Bookings_Select", parameters);
        }

        public static SqlDataReader SelectByID(long ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Bookings_SelectByID", parameters);
        }
        public static long Insert(int CompanyID, int Priority, int Status, int? InvoiceID, long? ShiftID, DateTime BookedDateTime, int? ClientID, string PassengerName, string ContactNumber, string From, string To, string Via, short? PAX, short? BAX, int? CarType, string CustomerComment, string OfficeComment, string DriverComment, int? DriverID, int? VehicleID, int? PaymentMethod, string PaymentRef, decimal? VATRate, decimal? CalculatedFare, decimal? ActualFare, int BookedBy, int? EditedBy, DateTime Timestamp, DateTime? Editstamp, DateTime LeadTime, bool AutoDispatch, long? LinkedBookingID, DateTime? POBTime, DateTime? ClearTime)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID",CompanyID),
                    new SqlParameter("Priority",Priority),
                    new SqlParameter("Status",Status),
                    new SqlParameter("InvoiceID",InvoiceID),
                    new SqlParameter("ShiftID",ShiftID),
                    new SqlParameter("BookedDateTime",BookedDateTime),
                    new SqlParameter("ClientID",ClientID),
                    new SqlParameter("PassengerName",PassengerName),
                    new SqlParameter("ContactNumber",ContactNumber),
                    new SqlParameter("From",From),
                    new SqlParameter("To",To),
                    new SqlParameter("Via",Via),
                    new SqlParameter("PAX",PAX),
                    new SqlParameter("BAX",BAX),
                    new SqlParameter("CarType",CarType),
                    new SqlParameter("CustomerComment",CustomerComment),
                    new SqlParameter("OfficeComment",OfficeComment),
                    new SqlParameter("DriverComment",DriverComment),
                    new SqlParameter("DriverID",DriverID),
                    new SqlParameter("VehicleID",VehicleID),
                    new SqlParameter("PaymentMethod",PaymentMethod),
                    new SqlParameter("PaymentRef",PaymentRef),
                    new SqlParameter("VATRate",VATRate),
                    new SqlParameter("CalculatedFare",CalculatedFare),
                    new SqlParameter("ActualFare",ActualFare),
                    new SqlParameter("BookedBy",BookedBy),
                    new SqlParameter("EditedBy",EditedBy),
                    new SqlParameter("Timestamp",Timestamp),
                    new SqlParameter("Editstamp",Editstamp),
                    new SqlParameter("LeadTime",LeadTime),
                    new SqlParameter("AutoDispatch",AutoDispatch),
                    new SqlParameter("LinkedBookingID",LinkedBookingID),
                    new SqlParameter("POBTime",POBTime),
                    new SqlParameter("ClearTime",ClearTime)
                };
            object result;
            try
            {
                result = SqlHelper.ExecuteScalar(ConnectionString, "Bookings_Insert", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return -1;
            }
            return Convert.ToInt32(result);
        }

        public static bool Update(long ID, int CompanyID, int Priority, int Status, int? InvoiceID, long? ShiftID, DateTime BookedDateTime, int? ClientID, string PassengerName, string ContactNumber, string From, string To, string Via, short? PAX, short? BAX, int? CarType, string CustomerComment, string OfficeComment, string DriverComment, int? DriverID, int? VehicleID, int? PaymentMethod, string PaymentRef, decimal? VATRate, decimal? CalculatedFare, decimal? ActualFare, int BookedBy, int? EditedBy, DateTime Timestamp, DateTime? Editstamp, DateTime LeadTime, bool AutoDispatch, long? LinkedBookingID, DateTime? POBTime, DateTime? ClearTime)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID",CompanyID),
                    new SqlParameter("Priority",Priority),
                    new SqlParameter("Status",Status),
                    new SqlParameter("InvoiceID",InvoiceID),
                    new SqlParameter("ShiftID",ShiftID),
                    new SqlParameter("BookedDateTime",BookedDateTime),
                    new SqlParameter("ClientID",ClientID),
                    new SqlParameter("PassengerName",PassengerName),
                    new SqlParameter("ContactNumber",ContactNumber),
                    new SqlParameter("From",From),
                    new SqlParameter("To",To),
                    new SqlParameter("Via",Via),
                    new SqlParameter("PAX",PAX),
                    new SqlParameter("BAX",BAX),
                    new SqlParameter("CarType",CarType),
                    new SqlParameter("CustomerComment",CustomerComment),
                    new SqlParameter("OfficeComment",OfficeComment),
                    new SqlParameter("DriverComment",DriverComment),
                    new SqlParameter("DriverID",DriverID),
                    new SqlParameter("VehicleID",VehicleID),
                    new SqlParameter("PaymentMethod",PaymentMethod),
                    new SqlParameter("PaymentRef",PaymentRef),
                    new SqlParameter("VATRate",VATRate),
                    new SqlParameter("CalculatedFare",CalculatedFare),
                    new SqlParameter("ActualFare",ActualFare),
                    new SqlParameter("BookedBy",BookedBy),
                    new SqlParameter("EditedBy",EditedBy),
                    new SqlParameter("Timestamp",Timestamp),
                    new SqlParameter("Editstamp",Editstamp),
                    new SqlParameter("LeadTime",LeadTime),
                    new SqlParameter("AutoDispatch",AutoDispatch),
                    new SqlParameter("LinkedBookingID",LinkedBookingID),
                    new SqlParameter("POBTime",POBTime),
                    new SqlParameter("ClearTime",ClearTime)
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "Bookings_Update", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return false;
            }
            return true;
        }

        public static bool Delete(long ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "Bookings_Delete", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return false;
            }
            return true;
        }

        public static SqlDataReader Search(int companyId, string name, string number)
        {
            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("CompanyID", companyId),
                new SqlParameter("PassengerName", name),
                new SqlParameter("ContactNumber", number)
            };

            return SqlHelper.ExecuteReader(ConnectionString, "Bookings_Search", parameters);
        }
    }
}