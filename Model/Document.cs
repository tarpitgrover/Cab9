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
    public class Document
    {
        #region Properties

        public int ID { get; set; }
        public int CompanyID { get; set; }
        public string DocumentType { get; set; }
        public string OwnerType { get; set; }
        public int OwnerID { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Name { get; set; }
        public string IdentificationNo { get; set; }
        public string Description { get; set; }
        public string DocumentURL { get; set; }

        #endregion

        #region Events

        public delegate void DocumentInsertedEvent(Document sender, HubEventArgs e);
        public static event DocumentInsertedEvent DocumentInserted;

        public delegate void DocumentUpdatedEvent(Document sender, HubEventArgs e);
        public static event DocumentUpdatedEvent DocumentUpdated;

        public delegate void DocumentDeletedEvent(Document sender, HubEventArgs e);
        public static event DocumentDeletedEvent DocumentDeleted;

        #endregion

        #region Constructors

        public Document()
        {
        }

        private Document(SqlDataReader row)
        {
            try
            {
                if (row["ID"] != DBNull.Value) ID = (int)row["ID"];
                if (row["CompanyID"] != DBNull.Value) CompanyID = (int)row["CompanyID"];
                if (row["DocumentType"] != DBNull.Value) DocumentType = (string)row["DocumentType"];
                if (row["OwnerType"] != DBNull.Value) OwnerType = (string)row["OwnerType"];
                if (row["OwnerID"] != DBNull.Value) OwnerID = (int)row["OwnerID"];
                if (row["ExpiryDate"] != DBNull.Value) ExpiryDate = (DateTime)row["ExpiryDate"];
                if (row["Name"] != DBNull.Value) Name = (string)row["Name"];
                if (row["IdentificationNo"] != DBNull.Value) IdentificationNo = (string)row["IdentificationNo"];
                if (row["Description"] != DBNull.Value) Description = (string)row["Description"];
                if (row["DocumentURL"] != DBNull.Value) DocumentURL = (string)row["DocumentURL"];
            }
            catch (Exception exc)
            {
                SystemLog.LogNewError(exc, LogType.RetreiveError, this);
                throw exc;
            }
        }
       
        #endregion

        #region CRUD

        public static List<Document> Select(int? id = null, int? companyId = null, string documentType = null, string ownerType = null, int? ownerId = null, DateTime? expiryFrom = null, DateTime? expiryTo = null, string identificationNo = null)
        {
            var result = new List<Document>();
            using (SqlDataReader dr = DocumentDAL.Select(id, companyId, documentType, ownerType, ownerId, expiryFrom, expiryTo, identificationNo))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new Document(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static Document SelectByID(int id)
        {
            Document result = null;
            using (SqlDataReader dr = DocumentDAL.SelectByID(id))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new Document(dr);
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = DocumentDAL.Insert(CompanyID, DocumentType, OwnerType, OwnerID, Name, ExpiryDate, IdentificationNo, Description, DocumentURL);
            if (ID == -1) return false;

            if (DocumentInserted != null) DocumentInserted(this, new HubEventArgs(CompanyID, 0));
            return true;
        }

        public bool Update()
        {
            if (DocumentDAL.Update(ID, CompanyID, DocumentType, OwnerType, OwnerID, Name, ExpiryDate, IdentificationNo, Description, DocumentURL))
            {
                if (DocumentUpdated != null) DocumentUpdated(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        public bool Delete()
        {
            if (DocumentDAL.Delete(ID))
            {
                if (DocumentDeleted != null) DocumentDeleted(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        #endregion

        #region Methods

        #endregion

    }
}