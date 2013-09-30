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
    public class Note
    {
        #region Properties

        public int ID { get; set; }
        public int CompanyID { get; set; }
        public string OwnerType { get; set; }
        public int OwnerID { get; set; }
        public string Body { get; set; }
        public DateTime TimeStamp { get; set; }
        public int AddedByID { get; set; }
        public int? EditedByID { get; set; }

        #endregion

        #region Events

        public delegate void NoteInsertedEvent(Note sender, HubEventArgs e);
        public static event NoteInsertedEvent NoteInserted;

        public delegate void NoteUpdatedEvent(Note sender, HubEventArgs e);
        public static event NoteUpdatedEvent NoteUpdated;

        public delegate void NoteDeletedEvent(Note sender, HubEventArgs e);
        public static event NoteDeletedEvent NoteDeleted;

        #endregion

        #region Constructors

        public Note()
        {
        }

        private Note(SqlDataReader row)
        {
            try
            {
                if (row["ID"] != DBNull.Value) ID = (int)row["ID"];
                if (row["CompanyID"] != DBNull.Value) CompanyID = (int)row["CompanyID"];
                if (row["OwnerType"] != DBNull.Value) OwnerType = (string)row["OwnerType"];
                if (row["OwnerID"] != DBNull.Value) OwnerID = (int)row["OwnerID"];
                if (row["Body"] != DBNull.Value) Body = (string)row["Body"];
                if (row["TimeStamp"] != DBNull.Value) TimeStamp = (DateTime)row["TimeStamp"];
                if (row["AddedByID"] != DBNull.Value) AddedByID = (int)row["AddedByID"];
                if (row["EditedByID"] != DBNull.Value) EditedByID = (int)row["EditedByID"];
            }
            catch (Exception exc)
            {
                SystemLog.LogNewError(exc, LogType.RetreiveError, this);
                throw exc;
            }
        }
       
        #endregion

        #region CRUD

        public static List<Note> Select(int? id = null, int? companyId = null, string ownerType = null, int? ownerId = null, DateTime? timeStampFrom = null, DateTime? timeStampTo = null)
        {
            var result = new List<Note>();
            using (SqlDataReader dr = NoteDAL.Select(id, companyId, ownerType, ownerId, timeStampFrom, timeStampTo))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new Note(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static Note SelectByID(int id)
        {
            Note result = null;
            using (SqlDataReader dr = NoteDAL.SelectByID(id))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new Note(dr);
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = NoteDAL.Insert(CompanyID, OwnerType, OwnerID, Body, TimeStamp, AddedByID, EditedByID);
            if (ID == -1) return false;

            if (NoteInserted != null) NoteInserted(this, new HubEventArgs(CompanyID, 0));
            return true;
        }

        public bool Update()
        {
            if (NoteDAL.Update(ID, CompanyID, OwnerType, OwnerID, Body, TimeStamp, AddedByID, EditedByID))
            {
                if (NoteUpdated != null) NoteUpdated(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        public bool Delete()
        {
            if (NoteDAL.Delete(ID))
            {
                if (NoteDeleted != null) NoteDeleted(this, new HubEventArgs(CompanyID, 0));
                return true;
            }
            return false;
        }

        #endregion

        #region Methods

        #endregion

    }
}