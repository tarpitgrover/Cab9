using Cab9.DataLayer;
using Cab9.EventHandlers.Common;
using e9.Debugging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Cab9.Model
{
    public class Vehicle
    {
        #region Properties

        public int ID { get; set; }
        public int CompanyID { get; set; }
        public string Registration { get; set; }
        public int VehicleTypeID { get; set; }
        public int? OwnerID { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Colour { get; set; }
        public short? PAX { get; set; }
        public short? BAX { get; set; }
        public string OfficeNotes { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public bool Active { get; set; }
        public string InactiveReason { get; set; }

        #endregion

        #region ForeignObjects

        public int? Mileage { get; set; }
        public VehicleType VehicleType { get; set; }

        #endregion

        #region Events

        public delegate void VehicleInsertedEvent(Vehicle sender, HubEventArgs e);
        public static event VehicleInsertedEvent VehicleInserted;

        public delegate void VehicleUpdatedEvent(Vehicle sender, HubEventArgs e);
        public static event VehicleUpdatedEvent VehicleUpdated;

        public delegate void VehicleDeletedEvent(Vehicle sender, HubEventArgs e);
        public static event VehicleDeletedEvent VehicleDeleted;

        #endregion

        #region Constructors

        public Vehicle()
        {
        }

        private Vehicle(SqlDataReader row)
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

                if (row["Registration"] != DBNull.Value)
                {
                    this.Registration = (string)(row["Registration"]);
                }

                if (row["VehicleTypeID"] != DBNull.Value)
                {
                    this.VehicleTypeID = (int)row["VehicleTypeID"];
                }

                if (row["OwnerID"] != DBNull.Value)
                {
                    this.OwnerID = (int)(row["OwnerID"]);
                }

                if (row["Make"] != DBNull.Value)
                {
                    this.Make = (string)(row["Make"]);
                }

                if (row["Model"] != DBNull.Value)
                {
                    this.Model = (string)(row["Model"]);
                }

                if (row["Colour"] != DBNull.Value)
                {
                    this.Colour = (string)(row["Colour"]);
                }

                if (row["PAX"] != DBNull.Value)
                {
                    this.PAX = (short)(row["PAX"]);
                }

                if (row["BAX"] != DBNull.Value)
                {
                    this.BAX = (short)(row["BAX"]);
                }

                if (row["OfficeNotes"] != DBNull.Value)
                {
                    this.OfficeNotes = (string)(row["OfficeNotes"]);
                }

                if (row["StartDate"] != DBNull.Value)
                {
                    this.StartDate = (DateTime)(row["StartDate"]);
                }

                if (row["FinishDate"] != DBNull.Value)
                {
                    this.FinishDate = (DateTime)(row["FinishDate"]);
                }

                if (row["Active"] != DBNull.Value)
                {
                    this.Active = (bool)(row["Active"]);
                }

                if (row["InactiveReason"] != DBNull.Value)
                {
                    this.InactiveReason = (string)(row["InactiveReason"]);
                }

                if (row["Mileage"] != DBNull.Value)
                {
                    this.Mileage = (int)(row["Mileage"]);
                }
                if (row["VehicleTypeID"] != DBNull.Value)
                {
                    this.VehicleTypeID = (int)row["VehicleTypeID"];
                    this.VehicleType = VehicleType.SelectByID((int)row["VehicleTypeID"]);
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

        public static List<Vehicle> Select(int? id = null, int? companyId = null, string registration = null, int? vehicleType = null, int? ownerId = null, bool? active = null)
        {
            var result = new List<Vehicle>();
            using (SqlDataReader dr = VehicleDAL.Select(id, companyId, registration, vehicleType, ownerId, active))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new Vehicle(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static Vehicle SelectByID(int id)
        {
            Vehicle result = null;
            using (SqlDataReader dr = VehicleDAL.SelectByID(id))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new Vehicle(dr);
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = VehicleDAL.Insert(CompanyID, Registration, VehicleTypeID, OwnerID, Make, Model, Colour, PAX, BAX, OfficeNotes, StartDate, FinishDate, Active, InactiveReason);
            if (ID == -1) return false;

            if (VehicleInserted != null) VehicleInserted(this, new HubEventArgs(CompanyID, 0));
            return true;
        }

        public bool Update()
        {
            if (VehicleDAL.Update(ID, CompanyID, Registration, VehicleTypeID, OwnerID, Make, Model, Colour, PAX, BAX, OfficeNotes, StartDate, FinishDate, Active, InactiveReason))
            {
                if (VehicleUpdated != null) VehicleUpdated(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        public bool Delete()
        {
            if (VehicleDAL.Delete(ID))
            {
                if (VehicleDeleted != null) VehicleDeleted(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        #endregion

        #region Methods

        #endregion

    }
}