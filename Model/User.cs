using Cab9.DataLayer;
using Cab9.Providers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web.Security;
using Newtonsoft.Json;

namespace Cab9.Model
{
    public class User
    {
        public int ID { get; set; }
        public string ApplicationName { get; set; }
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        
        private string Hashword { get; set; }
        private bool Active { get; set; }
        private string InactiveReason { get; set; }
        private DateTime? LastLogin { get; set; }
        private int? LoginAttempts { get; set; }
        private string UnlockCode { get; set; }
        private string ImageURL { get; set; }
        private string Address1 { get; set; }
        private string Address2 { get; set; }
        private string Area { get; set; }
        private string Town { get; set; }
        private string Postcode { get; set; }
        public string[] Roles 
        {
            get { return new Cab9RoleProvider().GetRolesForUser(Email); }
        }
        public string EntityType { get; set; }
        public int? EntityID { get; set; }

        public User()
        {
        }

        public User(SqlDataReader row)
        {
            if (row["ID"] != DBNull.Value) { ID = (int)row["ID"]; }
            if (row["ApplicationName"] != DBNull.Value) { ApplicationName = (string)row["ApplicationName"]; }
            if (row["CompanyID"] != DBNull.Value) { CompanyID = (int)row["CompanyID"]; }
            if (row["Name"] != DBNull.Value) { Name = (string)row["Name"]; }
            if (row["Mobile"] != DBNull.Value) { Mobile = (string)row["Mobile"]; }
            if (row["Email"] != DBNull.Value) { Email = (string)row["Email"]; }
            if (row["Hashword"] != DBNull.Value) { Hashword = (string)row["Hashword"]; }
            if (row["Active"] != DBNull.Value) { Active = (bool)row["Active"]; }
            if (row["InactiveReason"] != DBNull.Value) { InactiveReason = (string)row["InactiveReason"]; }
            if (row["LastLogin"] != DBNull.Value) { LastLogin = (DateTime)row["LastLogin"]; }
            if (row["LoginAttempts"] != DBNull.Value) { LoginAttempts = (int)row["LoginAttempts"]; }
            if (row["UnlockCode"] != DBNull.Value) { UnlockCode = (string)row["UnlockCode"]; }
            if (row["ImageURL"] != DBNull.Value) { ImageURL = (string)row["ImageURL"]; }
            if (row["Address1"] != DBNull.Value) { Address1 = (string)row["Address1"]; }
            if (row["Address2"] != DBNull.Value) { Address2 = (string)row["Address2"]; }
            if (row["Area"] != DBNull.Value) { Area = (string)row["Area"]; }
            if (row["Town"] != DBNull.Value) { Town = (string)row["Town"]; }
            if (row["Postcode"] != DBNull.Value) { Postcode = (string)row["Postcode"]; }
            if (row["EntityType"] != DBNull.Value) { EntityType = (string)row["EntityType"]; }
            if (row["EntityID"] != DBNull.Value) { EntityID = (int?)row["EntityID"]; }
        }

        #region Events

        public delegate void UserInsertedEvent(User sender, EventArgs e);
        public static event UserInsertedEvent UserInserted;

        public delegate void UserUpdatedEvent(User sender, EventArgs e);
        public static event UserUpdatedEvent UserUpdated;

        public delegate void UserDeletedEvent(User sender, EventArgs e);
        public static event UserDeletedEvent UserDeleted;

        public delegate void UserLoggedInEvent(User sender, EventArgs e);
        public static event UserLoggedInEvent UserLoggedIn;

        public delegate void UserLoggedOutEvent(User sender, EventArgs e);
        public static event UserLoggedOutEvent UserLoggedOut;

        #endregion


        public static List<User> Select(int? id = null, int? companyId = null, string mobile = null, string email = null, bool? active = null)
        {
            var result = new List<User>();
            using (SqlDataReader dr = UserDAL.Select(id, companyId, mobile, email, active))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add(new User(dr));
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static User SelectByID(int id)
        {
            User result = null;
            using (SqlDataReader dr = UserDAL.SelectByID(id))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new User(dr);
                }
                dr.Close();
            }
            return result;
        }

        public static User SelectByEmail(string email)
        {
            User result = null;
            using (SqlDataReader dr = UserDAL.SelectByEmail(email))
            {
                if (dr.HasRows && dr.Read())
                {
                    result = new User(dr);
                }
                dr.Close();
            }
            return result;
        }

        public bool Insert()
        {
            ID = UserDAL.Insert(ApplicationName, CompanyID, Name, Mobile, Email, Hashword, Active, InactiveReason, LastLogin, LoginAttempts, UnlockCode, ImageURL, Address1, Address2, Area, Town, Postcode, EntityType, EntityID);
            if (ID == -1) return false;

            if (UserInserted != null) UserInserted(this, EventArgs.Empty);
            return true;
        }

        public bool Update()
        { 
            if (UserDAL.Update(ID, ApplicationName, CompanyID, Name, Mobile, Email, Hashword, Active, InactiveReason, LastLogin, LoginAttempts, UnlockCode, ImageURL, Address1, Address2, Area, Town, Postcode, EntityType, EntityID))
            {
                if (UserUpdated != null) UserUpdated(this, EventArgs.Empty);
                return true;
            }
            return false;

        }

        public bool Delete()
        {
            if (UserDAL.Delete(ID))
            {
                if (UserDeleted != null) UserDeleted(this, EventArgs.Empty);
                return true;
            }
            return false;

        }

        public void LockUser(string reason)
        {
            Active = false;
            InactiveReason = reason;
            Update();
        }

        public void UnlockUser()
        {
            Active = true;
            InactiveReason = null;
            Update();
        }

        public string GenerateUnlockCode()
        {
            UnlockCode = "E9ROCKS";
            Update();
            return UnlockCode;
        }

        public static User ValidateUser(string email, string passWord, out string reason)
        {
            var result = SelectByEmail(email.ToLower());
            if (result == null)
                throw new ArgumentException("No user with this email");

            if (result.Active && result.Hashword == passWord)
            {
                result.LastLogin = DateTime.Now;
                result.LoginAttempts = 0;
                result.Update();
                reason = "Success";
                return result;
            }
            else if (!result.Active)
            {
                reason = result.InactiveReason ?? "";
                return null;
            }
            else
            {
                result.LoginAttempts++;
                if (result.LoginAttempts >= Cab9Config.MaxRetries)
                {
                    result.Active = false;
                    result.InactiveReason = "Password Attempts Limit Reached";
                    result.Update();
                    reason = "Password Attempts Limit Reached";
                    return null;
                }
                else
                {
                    result.Update();
                    reason = "Password Incorrect";
                    return null;
                }
            }
        }

        public static User FromTicket(string p)
        {
            return SelectByEmail(FormsAuthentication.Decrypt(p).Name);
        }
    }

}
