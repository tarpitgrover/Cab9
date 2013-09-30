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
    public class Client
    {
        #region Properties

        public int ID { get; set; }
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public int ClientTypeID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public DateTime ClientSince { get; set; }
        public string OfficeNotes { get; set; }
        public string LogoURL { get; set; }
        public int? DefaultPricingModelID { get; set; }
        public InvoiceMethod InvoiceMethod { get; set; }
        public InvoicePeriod InvoicePeriod { get; set; }
        public string EmailConfirmations { get; set; }
        public string PassPhrase { get; set; }


        #endregion

        #region ForeignObjects

        public ClientType ClientType { get; set; }

        #endregion

        #region Events

        public delegate void ClientInsertedEvent(Client sender, HubEventArgs e);
        public static event ClientInsertedEvent ClientInserted;

        public delegate void ClientUpdatedEvent(Client sender, HubEventArgs e);
        public static event ClientUpdatedEvent ClientUpdated;

        public delegate void ClientDeletedEvent(Client sender, HubEventArgs e);
        public static event ClientDeletedEvent ClientDeleted;

        #endregion

        #region Constructors

        public Client()
        {
        }

        private Client(SqlDataReader row)
        {
            try
            {
                if (row["ID"] != DBNull.Value)
                {
                    this.ID = (int)(row["ID"]);
                }

                if (row["Name"] != DBNull.Value)
                {
                    this.Name = (string)(row["Name"]);
                }

                if (row["CompanyID"] != DBNull.Value)
                {
                    this.CompanyID = (int)(row["CompanyID"]);
                }

                if (row["ClientTypeID"] != DBNull.Value)
                {
                    this.ClientTypeID = (int)row["ClientTypeID"];
                    this.ClientType = ClientType.SelectByID((int)row["ClientTypeID"]);
                }

                if (row["Address1"] != DBNull.Value)
                {
                    this.Address1 = (string)(row["Address1"]);
                }

                if (row["Address2"] != DBNull.Value)
                {
                    this.Address2 = (string)(row["Address2"]);
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

                if (row["Country"] != DBNull.Value)
                {
                    this.Country = (string)(row["Country"]);
                }

                if (row["ContactName"] != DBNull.Value)
                {
                    this.ContactName = (string)(row["ContactName"]);
                }

                if (row["Phone"] != DBNull.Value)
                {
                    this.Phone = (string)(row["Phone"]);
                }

                if (row["Fax"] != DBNull.Value)
                {
                    this.Fax = (string)(row["Fax"]);
                }

                if (row["Email"] != DBNull.Value)
                {
                    this.Email = (string)(row["Email"]);
                }

                if (row["Active"] != DBNull.Value)
                {
                    this.Active = (bool)(row["Active"]);
                }

                if (row["ClientSince"] != DBNull.Value)
                {
                    this.ClientSince = (DateTime)(row["ClientSince"]);
                }

                if (row["OfficeNotes"] != DBNull.Value)
                {
                    this.OfficeNotes = (string)(row["OfficeNotes"]);
                }

                if (row["LogoURL"] != DBNull.Value)
                {
                    this.LogoURL = (string)(row["LogoURL"]);
                }

                if (row["DefaultPricingModelID"] != DBNull.Value)
                {
                    this.DefaultPricingModelID = (int)row["DefaultPricingModelID"];
                }

                if (row["InvoiceMethod"] != DBNull.Value)
                {
                    this.InvoiceMethod = (InvoiceMethod)(int)row["InvoiceMethod"];
                }

                if (row["InvoicePeriod"] != DBNull.Value)
                {
                    this.InvoicePeriod = (InvoicePeriod)(int)row["InvoicePeriod"];
                }

                if (row["EmailConfirmations"] != DBNull.Value)
                {
                    this.EmailConfirmations = (string)row["EmailConfirmations"];
                }

                if (row["PassPhrase"] != DBNull.Value)
                {
                    this.PassPhrase = (string)(row["PassPhrase"]);
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

        public static List<Client> Select(int? id = null, int? companyId = null, bool? active = null)
        {
            var result = new List<Client>();
            using (SqlDataReader dr = ClientDAL.Select(id, companyId, active))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new Client(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static Client SelectByID(int id)
        {
            Client result = null;
            using (SqlDataReader dr = ClientDAL.SelectByID(id))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new Client(dr);
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = ClientDAL.Insert(CompanyID, Name, ClientTypeID, Active, Address1, Address2, Town, County, Postcode, Country, ContactName, Phone, Fax, Email, ClientSince, OfficeNotes, LogoURL, DefaultPricingModelID, (int)InvoiceMethod, (int)InvoicePeriod, EmailConfirmations, PassPhrase);
            if (ID == -1) return false;

            if (ClientInserted != null) ClientInserted(this, new HubEventArgs(CompanyID, 0));
            return true;
        }

        public bool Update()
        {
            if (ClientDAL.Update(ID, CompanyID, Name, ClientTypeID, Active, Address1, Address2, Town, County, Postcode, Country, ContactName, Phone, Fax, Email, ClientSince, OfficeNotes, LogoURL, DefaultPricingModelID, (int)InvoiceMethod, (int)InvoicePeriod, EmailConfirmations, PassPhrase))
            {
                if (ClientUpdated != null) ClientUpdated(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        public bool Delete()
        {
            if (ClientDAL.Delete(ID))
            {
                if (ClientDeleted != null) ClientDeleted(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        public static SqlDataReader Stats(DateTime From, DateTime To, int? CompanyID = null, int? ClientID = null)
        {
            return ClientDAL.Stats(CompanyID, ClientID, From, To);
        }


        #endregion

        #region Methods

        #endregion

    }
}