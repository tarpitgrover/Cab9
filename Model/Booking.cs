using Cab9.DataLayer;
using Cab9.EventHandlers.Common;
using Cab9.Hubs;
using Cab9.Model.Common;
using e9.Debugging;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Cab9.Model
{
    public class Booking
    {
        #region Properties

        public long ID { get; set; }
        public int CompanyID { get; set; }
        public int Priority { get; set; }
        public BookingStatus Status { get; set; }
        public int? InvoiceID { get; set; }
        public long? ShiftID { get; set; }
        public DateTime BookedDateTime { get; set; }
        public int? ClientID { get; set; }
        public string PassengerName { get; set; }
        public string ContactNumber { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Via { get; set; }
        public short? PAX { get; set; }
        public short? BAX { get; set; }
        public int? CarType { get; set; }
        public string CustomerComment { get; set; }
        public string OfficeComment { get; set; }
        public string DriverComment { get; set; }
        public int? DriverID { get; set; }
        public int? VehicleID { get; set; }
        public int? PaymentMethod { get; set; }
        public string PaymentRef { get; set; }
        public decimal? VATRate { get; set; }
        public decimal? CalculatedFare { get; set; }
        public decimal? ActualFare { get; set; }
        public int BookedBy { get; set; }
        public int? EditedBy { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime? Editstamp { get; set; }
        public DateTime LeadTime { get; set; }
        public bool AutoDispatch { get; set; }
        public long? LinkedBookingID { get; set; }
        public DateTime? POBTime { get; set; }
        public DateTime? ClearTime { get; set; }

        public DriverShift Shift { get; set; }
        [JsonIgnore]
        public Driver Driver 
        {
            get
            {
                return (Shift == null) ? null : Shift.Driver;
            }
        }
        [JsonIgnore]
        public Vehicle Vehicle 
        { 
            get
            {
                return (Shift == null) ? null : Shift.Vehicle;
            }
        }

        public Client Client { get; set; }

        #endregion

        #region Events

        public delegate void BookingInsertedEvent(Booking sender, HubEventArgs e);
        public static event BookingInsertedEvent BookingInserted;

        public delegate void BookingUpdatedEvent(Booking sender, HubEventArgs e);
        public static event BookingUpdatedEvent BookingUpdated;

        public delegate void BookingDeletedEvent(Booking sender, HubEventArgs e);
        public static event BookingDeletedEvent BookingDeleted;

        #endregion

        #region Constructors

        public Booking()
        {
        }

        private Booking(SqlDataReader row)
        {
            try
            {
                if (row["ID"] != DBNull.Value) ID = (long)row["ID"];
                if (row["CompanyID"] != DBNull.Value) CompanyID = (int)row["CompanyID"];
                if (row["Priority"] != DBNull.Value) Priority = (int)row["Priority"];
                if (row["Status"] != DBNull.Value) Status = (BookingStatus)Enum.ToObject(typeof(BookingStatus), row["Status"]);
                if (row["InvoiceID"] != DBNull.Value) InvoiceID = (int)row["InvoiceID"];
                if (row["ShiftID"] != DBNull.Value) { ShiftID = (long)row["ShiftID"]; Shift = DriverShift.SelectByID(ShiftID.Value); }
                if (row["BookedDateTime"] != DBNull.Value) BookedDateTime = (DateTime)row["BookedDateTime"];
                if (row["ClientID"] != DBNull.Value) { ClientID = (int)row["ClientID"]; Client = Client.SelectByID(ClientID.Value); };
                if (row["PassengerName"] != DBNull.Value) PassengerName = (string)row["PassengerName"];
                if (row["ContactNumber"] != DBNull.Value) ContactNumber = (string)row["ContactNumber"];
                if (row["From"] != DBNull.Value) From = (string)row["From"];
                if (row["To"] != DBNull.Value) To = (string)row["To"];
                if (row["Via"] != DBNull.Value) Via = (string)row["Via"];
                if (row["PAX"] != DBNull.Value) PAX = (short)row["PAX"];
                if (row["BAX"] != DBNull.Value) BAX = (short)row["BAX"];
                if (row["CarType"] != DBNull.Value) CarType = (int)row["CarType"];
                if (row["CustomerComment"] != DBNull.Value) CustomerComment = (string)row["CustomerComment"];
                if (row["OfficeComment"] != DBNull.Value) OfficeComment = (string)row["OfficeComment"];
                if (row["DriverComment"] != DBNull.Value) DriverComment = (string)row["DriverComment"];
                if (row["DriverID"] != DBNull.Value) { DriverID = (int)row["DriverID"]; }
                if (row["VehicleID"] != DBNull.Value) { VehicleID = (int)row["VehicleID"];}
                if (row["PaymentMethod"] != DBNull.Value) PaymentMethod = (int?)row["PaymentMethod"];
                if (row["PaymentRef"] != DBNull.Value) PaymentRef = (string)row["PaymentRef"];
                if (row["VATRate"] != DBNull.Value) VATRate = (decimal)row["VATRate"];
                if (row["CalculatedFare"] != DBNull.Value) CalculatedFare = (decimal)row["CalculatedFare"];
                if (row["ActualFare"] != DBNull.Value) ActualFare = (decimal)row["ActualFare"];
                if (row["BookedBy"] != DBNull.Value) BookedBy = (int)row["BookedBy"];
                if (row["EditedBy"] != DBNull.Value) EditedBy = (int)row["EditedBy"];
                if (row["Timestamp"] != DBNull.Value) Timestamp = (DateTime)row["Timestamp"];
                if (row["Editstamp"] != DBNull.Value) Editstamp = (DateTime)row["Editstamp"];
                if (row["LeadTime"] != DBNull.Value) LeadTime = (DateTime)row["LeadTime"];
                if (row["AutoDispatch"] != DBNull.Value) AutoDispatch = (bool)row["AutoDispatch"];
                if (row["LinkedBookingID"] != DBNull.Value) LinkedBookingID = (long)row["LinkedBookingID"];
                if (row["POBTime"] != DBNull.Value) POBTime = (DateTime)row["POBTime"];
                if (row["ClearTime"] != DBNull.Value) ClearTime = (DateTime)row["ClearTime"];
            }
            catch (Exception exc)
            {
                SystemLog.LogNewError(exc, LogType.RetreiveError, this);
                throw exc;
            }
        }
       
        #endregion

        #region CRUD

        public static List<Booking> Select(long? id = null, int? companyId = null, int? priority = null, int? status = null, int? invoiceID = null, long? shiftID = null, DateTime? bookedFrom = null, DateTime? bookedTo = null, int? clientID = null, int? carType = null, int? driverID = null, int? vehicleID = null, int? paymentMethod = null, int? bookedBy = null, int? editedBy = null, DateTime? leadFrom = null, DateTime? leadTo = null, bool? autoDispatch = null)
        {
            var result = new List<Booking>();
            using (SqlDataReader dr = BookingDAL.Select(id, companyId, priority, status, invoiceID, shiftID, bookedFrom, bookedTo, clientID, carType, driverID, vehicleID, paymentMethod, bookedBy, editedBy, leadFrom, leadTo, autoDispatch))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new Booking(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static Booking SelectByID(long id)
        {
            Booking result = null;
            using (SqlDataReader dr = BookingDAL.SelectByID(id))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new Booking(dr);
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = BookingDAL.Insert(CompanyID, Priority, (int)Status, InvoiceID, ShiftID, BookedDateTime, ClientID, PassengerName, ContactNumber, From, To, Via, PAX, BAX, CarType, CustomerComment, OfficeComment, DriverComment, DriverID, VehicleID, PaymentMethod, PaymentRef, VATRate, CalculatedFare, ActualFare, BookedBy, EditedBy, Timestamp, Editstamp, LeadTime, AutoDispatch, LinkedBookingID, POBTime, ClearTime);
            if (ID == -1) return false;

            if (BookingInserted != null) BookingInserted(this, new HubEventArgs(CompanyID, 0));
            return true;
        }

        public bool Update()
        {
            if (BookingDAL.Update(ID, CompanyID, Priority, (int)Status, InvoiceID, ShiftID, BookedDateTime, ClientID, PassengerName, ContactNumber, From, To, Via, PAX, BAX, CarType, CustomerComment, OfficeComment, DriverComment, DriverID, VehicleID, PaymentMethod, PaymentRef, VATRate, CalculatedFare, ActualFare, BookedBy, EditedBy, Timestamp, Editstamp, LeadTime, AutoDispatch, LinkedBookingID, POBTime, ClearTime))
            {
                if (BookingUpdated != null) BookingUpdated(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        public bool Delete()
        {
            if (BookingDAL.Delete(ID))
            {
                if (BookingDeleted != null) BookingDeleted(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        #endregion

        #region Methods

        public static List<dynamic> Search(int companyId, string name, string number)
        {
            List<object> Result = new List<object>();
            using (var dr = BookingDAL.Search(companyId, name, number))
            {
                while (dr.Read())
                {
                    Result.Add(new { 
                        BookedDateTime = (dr["BookedDateTime"] == DBNull.Value) ? null : (DateTime?)dr["BookedDateTime"],
                        PassengerName = (dr["PassengerName"] == DBNull.Value) ? null : (string)dr["PassengerName"],
                        ContactNumber = (dr["ContactNumber"] == DBNull.Value) ? null : (string)dr["ContactNumber"],
                        From = (dr["From"] == DBNull.Value) ? null : (string)dr["From"],
                        To = (dr["To"] == DBNull.Value) ? null : (string)dr["To"],
                    });
                }
                dr.Close();
            }
            return Result;
        }

        public void Push(int driverid)
        {
            var driver = Driver.SelectByID(driverid);

            if (driver == null || driver.CompanyID != CompanyID) throw new Exception("Driver not found within your company.");

            IHubContext Hub = GlobalHost.ConnectionManager.GetHubContext<DriverHub>();

            var bo = new BookingOffer();
            bo.OfferDateTime = DateTime.Now;
            bo.BookingID = ID;
            bo.DriverID = driverid;
            bo.Insert();

            Hub.Clients.All.offerBooking(driverid, new
            { 
                ID = bo.ID,
                DriverID = driverid,
                BookingID = ID,
                Booking = new { 
                    From = this.From, 
                    To = this.To, 
                    Fare = this.ActualFare, 
                    BookedDateTime = this.BookedDateTime,
                    PaymentMethod = this.PaymentMethod
                } 
            });
        }

        #endregion
    }
}