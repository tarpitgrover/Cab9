using Cab9.DataLayer.Common;
using e9.Debugging;
using System;
using System.Data.SqlClient;

namespace Cab9.DataLayer
{
    public class VehicleDAL : DAL
    {
        public static SqlDataReader Select(int? ID = null, int? CompanyID = null, string Registration = null, int? VehicleType = null, int? OwnerId = null, bool? Active = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("Registration", Registration),                	
					new SqlParameter("VehicleType", VehicleType),            
					new SqlParameter("OwnerId", OwnerId),                	
					new SqlParameter("Active", Active)      
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Vehicles_Select", parameters);
        }

        public static SqlDataReader SelectByID(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Vehicles_SelectByID", parameters);
        }

        public static int Insert(int CompanyID, string Registration, int VehicleType, int? OwnerId, string Make, string Model, string Colour, short? PAX, short? BAX, string OfficeNotes, DateTime StartDate, DateTime FinishDate, bool Active, string InactiveReason)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("Registration", Registration),                	
					new SqlParameter("VehicleType", VehicleType),            
					new SqlParameter("OwnerId", OwnerId),                	
					new SqlParameter("Make", Make),          
					new SqlParameter("Model", Model),                  	
					new SqlParameter("Colour", Colour),                	
					new SqlParameter("PAX", PAX),                	
					new SqlParameter("BAX", BAX),                        	
					new SqlParameter("OfficeNotes", OfficeNotes),                        
                    new SqlParameter("StartDate", StartDate), 
                    new SqlParameter("FinishDate", FinishDate), 
					new SqlParameter("Active", Active),      
                    new SqlParameter("InactiveReason", InactiveReason),
                };
            object result;
            try
            {
                result = SqlHelper.ExecuteScalar(ConnectionString, "Vehicles_Insert", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return -1;
            }
            return Convert.ToInt32(result);
        }

        public static bool Update(int ID, int CompanyID, string Registration, int VehicleType, int? OwnerId, string Make, string Model, string Colour, short? PAX, short? BAX, string OfficeNotes, DateTime StartDate, DateTime FinishDate, bool Active, string InactiveReason)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("Registration", Registration),                	
					new SqlParameter("VehicleType", VehicleType),            
					new SqlParameter("OwnerId", OwnerId),                	
					new SqlParameter("Make", Make),          
					new SqlParameter("Model", Model),                  	
					new SqlParameter("Colour", Colour),                	
					new SqlParameter("PAX", PAX),                	
					new SqlParameter("BAX", BAX),                        	
					new SqlParameter("OfficeNotes", OfficeNotes),   
                    new SqlParameter("StartDate", StartDate), 
                    new SqlParameter("FinishDate", FinishDate),  
					new SqlParameter("Active", Active),     
                    new SqlParameter("InactiveReason", InactiveReason),
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "Vehicles_Update", parameters);
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
                SqlHelper.ExecuteNonQuery(ConnectionString, "Vehicles_Delete", parameters);
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