using Cab9.DataLayer;
using Cab9.EventHandlers.Common;
using Cab9.Geography;
using Cab9.Model.Common;
using e9.Debugging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Cab9.Model
{
    public class Driver
    {
        #region Properties

        public int ID { get; set; }
        public int CompanyID { get; set; }
        public string CallSign { get; set; }
        public string Forename { get; set; }
        public string Middlenames { get; set; }
        public string Surname { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Area { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string Mobile { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public string InactiveReason { get; set; }
        public string OfficeNotes { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string ImageUrl { get; set; }
        public decimal? CreditLimit { get; set; }
        public int DriverTypeID { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public DriverStatus Status { get; set; }

        #endregion

        #region ForeignObjects

        public DriverType DriverType { get; set; }

        public Point LastKnownPosition { get; set; }
        public long? CurrentShiftID { get; set; }
        public long? CurrentBookingID { get; set; }

        #endregion

        #region Events

        public delegate void DriverInsertedEvent(Driver sender, HubEventArgs e);
        public static event DriverInsertedEvent DriverInserted;

        public delegate void DriverUpdatedEvent(Driver sender, HubEventArgs e);
        public static event DriverUpdatedEvent DriverUpdated;

        public delegate void DriverDeletedEvent(Driver sender, HubEventArgs e);
        public static event DriverDeletedEvent DriverDeleted;

        #endregion

        #region Constructors

        public Driver()
        {
        }

        private Driver(SqlDataReader row)
        {
            try
            {
                if (row["ID"] != DBNull.Value)
                {
                    this.ID = (int)(row["ID"]);
                }

                if (row["CompanyID"] != DBNull.Value)
                {
                    this.CompanyID = (int)(row["CompanyID"]);
                }

                if (row["CallSign"] != DBNull.Value)
                {
                    this.CallSign = (string)(row["CallSign"]);
                }

                if (row["DriverType"] != DBNull.Value)
                {
                    this.DriverTypeID = (int)row["DriverType"];
                    this.DriverType = DriverType.SelectByID((int)row["DriverType"]);
                }

                if (row["Forename"] != DBNull.Value)
                {
                    this.Forename = (string)(row["Forename"]);
                }

                if (row["Middlenames"] != DBNull.Value)
                {
                    this.Middlenames = (string)(row["Middlenames"]);
                }

                if (row["Surname"] != DBNull.Value)
                {
                    this.Surname = (string)(row["Surname"]);
                }

                if (row["Address1"] != DBNull.Value)
                {
                    this.Address1 = (string)(row["Address1"]);
                }

                if (row["Address2"] != DBNull.Value)
                {
                    this.Address2 = (string)(row["Address2"]);
                }

                if (row["Area"] != DBNull.Value)
                {
                    this.Area = (string)(row["Area"]);
                }

                if (row["Town"] != DBNull.Value)
                {
                    this.Town = (string)(row["Town"]);
                }

                if (row["County"] != DBNull.Value)
                {
                    this.County = (string)(row["County"]);
                }

                if (row["Postcode"] != DBNull.Value)
                {
                    this.Postcode = (string)(row["Postcode"]);
                }

                if (row["Mobile"] != DBNull.Value)
                {
                    this.Mobile = (string)(row["Mobile"]);
                }

                if (row["Telephone"] != DBNull.Value)
                {
                    this.Telephone = (string)(row["Telephone"]);
                }

                if (row["Email"] != DBNull.Value)
                {
                    this.Email = (string)(row["Email"]);
                }

                if (row["Status"] != DBNull.Value)
                {
                    this.Status = (DriverStatus)(int)row["Status"];
                }

                if (row["Active"] != DBNull.Value)
                {
                    this.Active = (bool)(row["Active"]);
                }

                if (row["InactiveReason"] != DBNull.Value)
                {
                    this.InactiveReason = (string)(row["InactiveReason"]);
                }

                if (row["OfficeNotes"] != DBNull.Value)
                {
                    this.OfficeNotes = (string)(row["OfficeNotes"]);
                }

                if (row["DateOfBirth"] != DBNull.Value)
                {
                    this.DateOfBirth = (DateTime)(row["DateOfBirth"]);
                }

                if (row["StartDate"] != DBNull.Value)
                {
                    this.StartDate = (DateTime)(row["StartDate"]);
                }

                if (row["FinishDate"] != DBNull.Value)
                {
                    this.FinishDate = (DateTime)(row["FinishDate"]);
                }

                if (row["ImageURL"] != DBNull.Value)
                {
                    this.ImageUrl = (string)(row["ImageURL"]);
                }

                if (row["CreditLimit"] != DBNull.Value)
                {
                    this.CreditLimit = (decimal)(row["CreditLimit"]);
                }

                if (row["CurrentShiftID"] != DBNull.Value)
                {
                    this.CurrentShiftID = (long)(row["CurrentShiftID"]);
                }

                if (row["CurrentBookingID"] != DBNull.Value)
                {
                    this.CurrentBookingID = (long)(row["CurrentBookingID"]);
                }

                if (row["LastKnownPositionLat"] != DBNull.Value && row["LastKnownPositionLong"] != DBNull.Value)
                {
                    this.LastKnownPosition = new Point((decimal)(row["LastKnownPositionLat"]), (decimal)(row["LastKnownPositionLong"]));
                }
            }
            catch (Exception exc)
            {
                SystemLog.LogNewError(exc, LogType.RetreiveError, this);
                throw exc;
            }
        }
       
        #endregion

        #region CRUD

        public static List<Driver> Select(int? id = null, int? companyId = null, string callSign = null, string mobile = null, string telephone = null, string email = null, bool? active = null, DateTime? dateOfBirth = null, int? DriverType = null, int? status = null)
        {
            var result = new List<Driver>();
            using (SqlDataReader dr = DriverDAL.Select(id, companyId, callSign, mobile, telephone, email, active, dateOfBirth, DriverType, status))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new Driver(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static Driver SelectByID(int id)
        {
            Driver result = null;
            using (SqlDataReader dr = DriverDAL.SelectByID(id))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new Driver(dr);
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = DriverDAL.Insert(CompanyID, CallSign, DriverTypeID, Forename, Middlenames, Surname, Address1, Address2, Area, Town, County, Postcode, Mobile, Telephone, Email, (int)Status, Active, InactiveReason, DateOfBirth, OfficeNotes, StartDate, FinishDate, ImageUrl, CreditLimit, CurrentShiftID, CurrentBookingID, (LastKnownPosition != null) ? (decimal?)LastKnownPosition.latitude : null, (LastKnownPosition != null) ? (decimal?)LastKnownPosition.longitude : null);
            if (ID == -1) return false;

            if (DriverInserted != null) DriverInserted(this, new HubEventArgs(CompanyID, 0));
            return true;
        }

        public bool Update()
        {
            if (DriverDAL.Update(ID, CompanyID, CallSign, DriverTypeID, Forename, Middlenames, Surname, Address1, Address2, Area, Town, County, Postcode, Mobile, Telephone, Email, (int)Status, Active, InactiveReason, DateOfBirth, OfficeNotes, StartDate, FinishDate, ImageUrl, CreditLimit, CurrentShiftID, CurrentBookingID, (LastKnownPosition != null) ? (decimal?)LastKnownPosition.latitude : null, (LastKnownPosition != null) ? (decimal?)LastKnownPosition.longitude : null))
            {
                if (DriverUpdated != null) DriverUpdated(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        public bool Delete()
        {
            if (DriverDAL.Delete(ID))
            {
                if (DriverDeleted != null) DriverDeleted(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        public static SqlDataReader Stats(DateTime From, DateTime To, int? CompanyID = null, int? DriverID = null)
        {
            return DriverDAL.Stats(CompanyID, DriverID, From, To);
        }

        #endregion

        #region Methods

        public DriverShift StartNewShift(int vehicleID, int mileage)
        {
            DriverShift shift = new DriverShift()
            {
                Active = true,
                CompanyID = CompanyID,
                DriverID = ID,
                EncodedRoute = "",
                MileageStart = mileage,
                ShiftStart = DateTime.Now,
                VehicleID = vehicleID
            };
            shift.Insert();
            CurrentShiftID = shift.ID;
            Status = DriverStatus.Available;
            Update();
            return shift;
        }

        public DriverShift EndCurrentShift(int mileage)
        {
            if (!CurrentShiftID.HasValue) 
                throw new InvalidOperationException("Driver is not currently on shift.");

            DriverShift shift = DriverShift.SelectByID(CurrentShiftID.Value);
            shift.MileageEnd = mileage;
            shift.ShiftEnd = DateTime.Now;
            shift.Update();
            CurrentShiftID = null;
            Status = DriverStatus.OffDuty;
            Update();
            return shift;
        }

        public void AcceptBooking(Booking booking)
        {
            if (booking == null)
                throw new ArgumentNullException("Booking not supplied.");

            if (!CurrentShiftID.HasValue) 
                throw new InvalidOperationException("Driver is not currently on shift.");

            DriverShift shift = DriverShift.SelectByID(CurrentShiftID.Value);

            booking.DriverID = ID;
            booking.VehicleID = shift.VehicleID;
            booking.Status = BookingStatus.Dispatched;
            booking.Update();

            Status = DriverStatus.PickingUp;
            CurrentBookingID = booking.ID;
            Update();
        }

        public void CompleteBooking(Booking booking)
        {

        }


        #endregion
    }
}