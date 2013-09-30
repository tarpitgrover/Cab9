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
    public class ClientDAL : DAL
    {
        public static SqlDataReader Select(int? ID = null, int? CompanyID = null, bool? Active = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("Active", Active)                
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Clients_Select", parameters);
        }

        public static SqlDataReader SelectByID(int ID)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Clients_SelectByID", parameters);
        }

        //TODO: Make sure all data layer inserts and updates parameters are not optional when allow nulls not set in db
        public static int Insert(int CompanyID, string Name, int ClientTypeID, bool Active, string Address1 = null, string Address2 = null, string Town = null, string County = null, string Postcode = null, string Country = null, string ContactName = null, string Phone = null, string Fax = null, string Email = null, DateTime? ClientSince = null, string OfficeNotes = null, string LogoURL = null, int? DefaultPricingModelID = null, int? InvoiceMethod = null, int? InvoicePeriod = null, string EmailConfirmations = null, string PassPhrase = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID", CompanyID),
					new SqlParameter("Name",Name),
					new SqlParameter("ClientTypeID",ClientTypeID),
					new SqlParameter("Address1",Address1),
					new SqlParameter("Address2",Address2),
					new SqlParameter("Town",Town),
					new SqlParameter("County",County),
					new SqlParameter("Postcode",Postcode),
					new SqlParameter("Country",Country),
					new SqlParameter("ContactName",ContactName),
					new SqlParameter("Phone",Phone),
					new SqlParameter("Fax",Fax),
					new SqlParameter("Email",Email),
					new SqlParameter("Active",Active),
					new SqlParameter("ClientSince",ClientSince),
					new SqlParameter("OfficeNotes",OfficeNotes),
					new SqlParameter("LogoURL",LogoURL),
					new SqlParameter("DefaultPricingModelID",DefaultPricingModelID),
					new SqlParameter("InvoiceMethod",InvoiceMethod),
					new SqlParameter("InvoicePeriod",InvoicePeriod),
					new SqlParameter("EmailConfirmations",EmailConfirmations),
                    new SqlParameter("PassPhrase",PassPhrase)
                };
            object result;
            try
            {
                result = SqlHelper.ExecuteScalar(ConnectionString, "Clients_Insert", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return -1;
            }
            return Convert.ToInt32(result);
        }

        public static bool Update(int ID, int CompanyID, string Name, int ClientTypeID, bool Active, string Address1 = null, string Address2 = null, string Town = null, string County = null, string Postcode = null, string Country = null, string ContactName = null, string Phone = null, string Fax = null, string Email = null, DateTime? ClientSince = null, string OfficeNotes = null, string LogoURL = null, int? DefaultPricingModelID = null, int? InvoiceMethod = null, int? InvoicePeriod = null, string EmailConfirmations = null, string PassPhrase = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("ID", ID),
                    new SqlParameter("CompanyID", CompanyID),
					new SqlParameter("Name",Name),
					new SqlParameter("ClientTypeID",ClientTypeID),
					new SqlParameter("Address1",Address1),
					new SqlParameter("Address2",Address2),
					new SqlParameter("Town",Town),
					new SqlParameter("County",County),
					new SqlParameter("Postcode",Postcode),
					new SqlParameter("Country",Country),
					new SqlParameter("ContactName",ContactName),
					new SqlParameter("Phone",Phone),
					new SqlParameter("Fax",Fax),
					new SqlParameter("Email",Email),
					new SqlParameter("Active",Active),
					new SqlParameter("ClientSince",ClientSince),
					new SqlParameter("OfficeNotes",OfficeNotes),
					new SqlParameter("LogoURL",LogoURL),
					new SqlParameter("DefaultPricingModelID",DefaultPricingModelID),
					new SqlParameter("InvoiceMethod",InvoiceMethod),
					new SqlParameter("InvoicePeriod",InvoicePeriod),
					new SqlParameter("EmailConfirmations",EmailConfirmations),
                    new SqlParameter("PassPhrase",PassPhrase)
                };
            try
            {
                SqlHelper.ExecuteNonQuery(ConnectionString, "Clients_Update", parameters);
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
                SqlHelper.ExecuteNonQuery(ConnectionString, "Clients_Delete", parameters);
            }
            catch (Exception exc)
            {
                SystemLog.LogException(exc);
                DebugEmailer.Email(exc);
                return false;
            }
            return true;
        }

        public static SqlDataReader Stats(int? CompanyID, int? ClientID, DateTime From, DateTime To)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("CompanyID", CompanyID),
                    new SqlParameter("ClientID", ClientID),
                    new SqlParameter("From", From),
                    new SqlParameter("To", To)
                };
            return SqlHelper.ExecuteReader(ConnectionString, "Client_Stats", parameters);
        }

    }
}