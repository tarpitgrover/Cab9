using Cab9.DataLayer.Common;
using e9.Debugging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Cab9.DataLayer
{
    public class DriverDAL : DAL
    {
        public static SqlDataReader Select(int? ID = null, int? CompanyID = null, string CallSign = null, string Mobile = null, string Telephone = null, string Email = null, bool? Active = null, DateTime? DateOfBirth = null, int? DriverType = null, int? Status = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("CallSign", CallSign),                	
					new SqlParameter("Mobile", Mobile),                    
					new SqlParameter("Telephone", Telephone),              	
					new SqlParameter("Email", Email),  
                    new SqlParameter("Active", Active),                    
					new SqlParameter("DateOfBirth", DateOfBirth),          	
                    new SqlParameter("DriverTypeID", DriverType),            
					new SqlParameter("Status", Status)                 	
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Drivers_Select", parameters);
        }

        public static SqlDataReader SelectByID(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Drivers_SelectByID", parameters);
        }

        public static int Insert(int CompanyID, string CallSign, int DriverType, string Forename, string Middlenames, string Surname, string Address1, string Address2, string Area, string Town, string County, string Postcode, string Mobile, string Telephone, string Email, int Status, bool Active, string InactiveReason, DateTime? DateOfBirth, string OfficeNotes, DateTime? StartDate, DateTime? FinishDate, string ImageURL, decimal? CreditLimit, long? CurrentShiftID, long? CurrentBookingID, decimal? Latitude, decimal? Longitude)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("CallSign", CallSign),                	
					new SqlParameter("DriverType", DriverType),            
					new SqlParameter("Forename", Forename),                	
					new SqlParameter("Middlenames", Middlenames),          
					new SqlParameter("Surname", Surname),                  	
					new SqlParameter("Address1", Address1),                	
					new SqlParameter("Address2", Address2),                	
					new SqlParameter("Area", Area),                        	
					new SqlParameter("Town", Town),                        
					new SqlParameter("County", County),                    
					new SqlParameter("Postcode", Postcode),                
					new SqlParameter("Mobile", Mobile),                    
					new SqlParameter("Telephone", Telephone),              	
					new SqlParameter("Email", Email),                      	
					new SqlParameter("Status", Status),                    	
					new SqlParameter("Active", Active),                    
					new SqlParameter("InactiveReason", InactiveReason),    
					new SqlParameter("DateOfBirth", DateOfBirth),          	
					new SqlParameter("OfficeNotes", OfficeNotes),          
					new SqlParameter("StartDate", StartDate),              
					new SqlParameter("FinishDate", FinishDate),            
					new SqlParameter("ImageURL", ImageURL),                	
					new SqlParameter("CreditLimit", CreditLimit),
					new SqlParameter("CurrentShiftID", CurrentShiftID),              
					new SqlParameter("CurrentBookingID", CurrentBookingID),            
					new SqlParameter("LastKnownLatitude", Latitude),                	
					new SqlParameter("LastKnownLongitude", Longitude)

                };
            object result;
            try
            {
                result = SqlHelper.ExecuteScalar(ConnectionString, "Drivers_Insert", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return -1;
            }
            return Convert.ToInt32(result);
        }

        public static bool Update(int ID, int CompanyID, string CallSign, int DriverType, string Forename, string Middlenames, string Surname, string Address1, string Address2, string Area, string Town, string County, string Postcode, string Mobile, string Telephone, string Email, int Status, bool Active, string InactiveReason, DateTime? DateOfBirth, string OfficeNotes, DateTime? StartDate, DateTime? FinishDate, string ImageURL, decimal? CreditLimit, long? CurrentShiftID, long? CurrentBookingID, decimal? Latitude, decimal? Longitude)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("CallSign", CallSign),                	
					new SqlParameter("DriverType", DriverType),            
					new SqlParameter("Forename", Forename),                	
					new SqlParameter("Middlenames", Middlenames),          
					new SqlParameter("Surname", Surname),                  	
					new SqlParameter("Address1", Address1),                	
					new SqlParameter("Address2", Address2),                	
					new SqlParameter("Area", Area),                        	
					new SqlParameter("Town", Town),                        
					new SqlParameter("County", County),                    
					new SqlParameter("Postcode", Postcode),                
					new SqlParameter("Mobile", Mobile),                    
					new SqlParameter("Telephone", Telephone),              	
					new SqlParameter("Email", Email),                      	
					new SqlParameter("Status", Status),                    	
					new SqlParameter("Active", Active),                    
					new SqlParameter("InactiveReason", InactiveReason),    
					new SqlParameter("DateOfBirth", DateOfBirth),          	
					new SqlParameter("OfficeNotes", OfficeNotes),          
					new SqlParameter("StartDate", StartDate),              
					new SqlParameter("FinishDate", FinishDate),            
					new SqlParameter("ImageURL", ImageURL),                	
					new SqlParameter("CreditLimit", CreditLimit),
					new SqlParameter("CurrentShiftID", CurrentShiftID),              
					new SqlParameter("CurrentBookingID", CurrentBookingID),            
					new SqlParameter("LastKnownLatitude", Latitude),                	
					new SqlParameter("LastKnownLongitude", Longitude)
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "Drivers_Update", parameters);
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
                SqlHelper.ExecuteNonQuery(ConnectionString, "Drivers_Delete", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return false;
            }
            return true;
        }

        public static SqlDataReader Stats(int? CompanyID, int? DriverID, DateTime From, DateTime To)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("DriverID", DriverID),
                    new SqlParameter("From", From),
                    new SqlParameter("To", To)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Driver_Stats", parameters);
        }
    }
}