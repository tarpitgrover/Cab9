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
    public class DriverShiftDAL : DAL
    {
        public static SqlDataReader Select(long? ID = null, int? CompanyID = null, int? DriverID = null, int? VehicleID = null, bool? Active = null, DateTime? ShiftFrom = null, DateTime? ShiftTo = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("DriverID", DriverID),
                    new SqlParameter("VehicleID", VehicleID),
                    new SqlParameter("Active", Active),        
                    new SqlParameter("ShiftFrom", ShiftFrom),
                    new SqlParameter("ShiftTo", ShiftTo)           
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Driver_Shifts_Select", parameters);
        }

        public static SqlDataReader SelectByID(long ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Driver_Shifts_SelectByID", parameters);
        }

        //TODO: Make sure all data layer inserts and updates parameters are not optional when allow nulls not set in db
        public static long Insert(int CompanyID, int DriverID, int VehicleID, DateTime ShiftStart, DateTime? ShiftEnd, int? MileageStart, int? MileageEnd, bool Active, string EncodedRoute, string ShiftNotes)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID", CompanyID),
					new SqlParameter("DriverID",DriverID),
					new SqlParameter("VehicleID",VehicleID),
					new SqlParameter("ShiftStart",ShiftStart),
					new SqlParameter("ShiftEnd",ShiftEnd),
					new SqlParameter("MileageStart",MileageStart),
					new SqlParameter("MileageEnd",MileageEnd),
					new SqlParameter("Active",Active),
					new SqlParameter("EncodedRoute",EncodedRoute),
					new SqlParameter("ShiftNotes",ShiftNotes)
                };
            object result;
            try
            {
                result = SqlHelper.ExecuteScalar(ConnectionString, "Driver_Shifts_Insert", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return -1;
            }
            return Convert.ToInt32(result);
        }
        public static bool Update(long ID, int CompanyID, int DriverID, int VehicleID, DateTime ShiftStart, DateTime? ShiftEnd, int? MileageStart, int? MileageEnd, bool Active, string EncodedRoute, string ShiftNotes)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
					new SqlParameter("DriverID",DriverID),
					new SqlParameter("VehicleID",VehicleID),
					new SqlParameter("ShiftStart",ShiftStart),
					new SqlParameter("ShiftEnd",ShiftEnd),
					new SqlParameter("MileageStart",MileageStart),
					new SqlParameter("MileageEnd",MileageEnd),
					new SqlParameter("Active",Active),
					new SqlParameter("EncodedRoute",EncodedRoute),
					new SqlParameter("ShiftNotes",ShiftNotes)
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "Driver_Shifts_Update", parameters);
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
                SqlHelper.ExecuteNonQuery(ConnectionString, "Driver_Shifts_Delete", parameters);
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