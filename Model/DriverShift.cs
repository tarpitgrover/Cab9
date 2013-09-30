using Cab9.DataLayer;
using Cab9.EventHandlers.Common;
using Cab9.Geography;
using Cab9.Model.Common;
using e9.Debugging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Cab9.Model
{
    public class DriverShift
    {
        #region Properties

        public long ID { get; set; }
        public int CompanyID { get; set; }
        public int DriverID { get; set; }
        public int VehicleID { get; set; }
        public DateTime ShiftStart { get; set; }
        public DateTime? ShiftEnd { get; set; }
        public int? MileageStart { get; set; }
        public int? MileageEnd { get; set; }
        public bool Active { get; set; }
        public string EncodedRoute { get; set; }
        public string ShiftNotes { get; set; }

        public Vehicle Vehicle { get; set; }
        public Driver Driver { get; set; }

        #endregion

        #region Events

        public delegate void DriverShiftInsertedEvent(DriverShift sender, HubEventArgs e);
        public static event DriverShiftInsertedEvent DriverShiftInserted;

        public delegate void DriverShiftUpdatedEvent(DriverShift sender, HubEventArgs e);
        public static event DriverShiftUpdatedEvent DriverShiftUpdated;

        public delegate void DriverShiftDeletedEvent(DriverShift sender, HubEventArgs e);
        public static event DriverShiftDeletedEvent DriverShiftDeleted;

        public delegate void DriverShiftRouteUpdatedEvent(DriverShift sender, HubEventArgs e);
        public static event DriverShiftRouteUpdatedEvent DriverShiftRouteUpdated;


        #endregion

        #region Constructors

        public DriverShift()
        {
            Active = true;
        }

        private DriverShift(SqlDataReader row)
        {
            try
            {
                if (row["ID"] != DBNull.Value) { ID = (long)(row["ID"]); }
                if (row["CompanyID"] != DBNull.Value) { CompanyID = (int)(row["CompanyID"]); }
                if (row["DriverID"] != DBNull.Value) { DriverID = (int)(row["DriverID"]); Driver = Driver.SelectByID(DriverID); }
                if (row["VehicleID"] != DBNull.Value) { VehicleID = (int)(row["VehicleID"]); Vehicle = Vehicle.SelectByID(VehicleID); }
                if (row["ShiftStart"] != DBNull.Value) { ShiftStart = (DateTime)(row["ShiftStart"]); }
                if (row["ShiftEnd"] != DBNull.Value) { ShiftEnd = (DateTime?)(row["ShiftEnd"]); }
                if (row["MileageStart"] != DBNull.Value) { MileageStart = (int?)(row["MileageStart"]); }
                if (row["MileageEnd"] != DBNull.Value) { MileageEnd = (int?)(row["MileageEnd"]); }
                if (row["Active"] != DBNull.Value) { Active = (bool)(row["Active"]); }
                if (row["EncodedRoute"] != DBNull.Value) { EncodedRoute = (string)(row["EncodedRoute"]); }
                if (row["ShiftNotes"] != DBNull.Value) { ShiftNotes = (string)(row["ShiftNotes"]); }
            }
            catch (Exception exc)
            {
                SystemLog.LogNewError(exc, LogType.RetreiveError, this);
                throw exc;
            }
        }
       
        #endregion

        #region CRUD

        public static List<DriverShift> Select(long? id = null, int? companyId = null, int? driverId = null, int? vehicleId = null, bool? active = null, DateTime? shiftFrom = null, DateTime? shiftTo = null)
        {
            var result = new List<DriverShift>();
            using (SqlDataReader dr = DriverShiftDAL.Select(id, companyId, driverId, vehicleId, active, shiftFrom, shiftTo))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new DriverShift(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static DriverShift SelectByID(long id)
        {
            DriverShift result = null;
            using (SqlDataReader dr = DriverShiftDAL.SelectByID(id))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new DriverShift(dr);
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = DriverShiftDAL.Insert(CompanyID, DriverID, VehicleID, ShiftStart, ShiftEnd, MileageStart, MileageEnd, Active, EncodedRoute, ShiftNotes);
            if (ID == -1) return false;

            if (DriverShiftInserted != null) DriverShiftInserted(this, new HubEventArgs(CompanyID, 0));
            return true;
        }

        public bool Update(bool invokeEvent = true)
        {
            if (DriverShiftDAL.Update(ID, CompanyID, DriverID, VehicleID, ShiftStart, ShiftEnd, MileageStart, MileageEnd, Active, EncodedRoute, ShiftNotes))
            {
                if (DriverShiftUpdated != null && invokeEvent) DriverShiftUpdated(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        public bool Delete()
        {
            if (DriverShiftDAL.Delete(ID))
            {
                if (DriverShiftDeleted != null) DriverShiftDeleted(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        #endregion

        #region Methods

        public Polyline GetRoute()
        {
            return new Polyline(EncodedRoute);
        }

        public List<Booking> GetBookings()
        {
            return Booking.Select(shiftID: ID);
        }

        public bool AddNewPoint(decimal latitude, decimal longitude)
        {
            return AddNewPoint(new Point(latitude, longitude));
        }

        public bool AddNewPoint(Point point)
        {
            var route = GetRoute();
            route.Points.Add(point);
            EncodedRoute = route.EncodedPolyline;
            if (Update(false))
            {
                if (DriverShiftRouteUpdated != null) DriverShiftRouteUpdated(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            else
            {
                return false;
            }
        }

        public int MileageUsed()
        {
            if (MileageEnd == -1 || MileageStart == -1) return 0;
            else return this.MileageEnd ?? 0 - this.MileageStart ?? 0;
        }

        #endregion

    }
}