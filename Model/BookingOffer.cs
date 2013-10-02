using Cab9.DataLayer;
using Cab9.EventHandlers.Common;
using Cab9.Model.Common;
using e9.Debugging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Cab9.Model
{
    public class BookingOffer
    {
        #region Properties

        public long ID { get; set; }
        public int CompanyID { get; set; }
        public DateTime OfferDateTime { get; set; }
        public int DriverID { get; set; }
        public long BookingID { get; set; }
        public bool? Response { get; set; }
        public string Reason { get; set; }

        #endregion

        #region Constructors

        public BookingOffer()
        {
        }

        private BookingOffer(SqlDataReader row)
        {
            try
            {
                if (row["ID"] != DBNull.Value)
                {
                    this.ID = (long)(row["ID"]);
                }

                if (row["CompanyID"] != DBNull.Value)
                {
                    this.CompanyID = (int)(row["CompanyID"]);
                }

                if (row["OfferDateTime"] != DBNull.Value)
                {
                    this.OfferDateTime = (DateTime)(row["OfferDateTime"]);
                }

                if (row["DriverID"] != DBNull.Value)
                {
                    this.DriverID = (int)(row["DriverID"]);
                }

                if (row["BookingID"] != DBNull.Value)
                {
                    this.BookingID = (long)(row["BookingID"]);
                }

                if (row["Response"] != DBNull.Value)
                {
                    this.Response = (bool?)(row["Response"]);
                }

                if (row["Reason"] != DBNull.Value)
                {
                    this.Reason = (string)(row["Reason"]);
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

        public static List<BookingOffer> Select(long? id = null, int? companyId = null, int? driverId = null, long? bookingId = null, bool? response = null)
        {
            var result = new List<BookingOffer>();
            using (SqlDataReader dr = BookingOfferDAL.Select(id, companyId, driverId, bookingId, response))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new BookingOffer(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = BookingOfferDAL.Insert(CompanyID, OfferDateTime, DriverID, BookingID);
            if (ID == -1) return false;
            return true;
        }

        public bool UpdateResponse(bool response, string reason)
        {
            Response = response;
            Reason = reason;
            return BookingOfferDAL.Update(ID, response, reason);
        }

        #endregion

    }
}