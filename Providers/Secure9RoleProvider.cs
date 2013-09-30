using Cab9.DataLayer;
using Cab9.DataLayer.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace Cab9.Providers
{
    public class Cab9RoleProvider : RoleProvider
    {
        public string ConnectionString
        {
            get
            {
                return Cab9Config.ConnectionString;
            }
        }

        public override string ApplicationName
        {
            get
            {
                return Cab9Config.ApplicationName;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            if (!RoleExists(roleName))
            {
                RoleDAL.Insert(ApplicationName, roleName);
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            var currentUsers = GetUsersInRole(roleName);
            if (currentUsers.Length > 0)
            {
                if (throwOnPopulatedRole)
                    return false;

                RemoveUsersFromRoles(currentUsers, new string[] { roleName });
            }

            return RoleDAL.Delete(ApplicationName, roleName);
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return RoleDAL.SelectUsersInRoles(ApplicationName, roleName, usernameToMatch);
        }

        public override string[] GetAllRoles()
        {
            return RoleDAL.Select(ApplicationName, null);
        }

        public override string[] GetRolesForUser(string username)
        {
            return RoleDAL.SelectRolesInUsers(ApplicationName, null, username);
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return RoleDAL.SelectUsersInRoles(ApplicationName, roleName, null);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            foreach (string user in usernames)
            {
                foreach (string role in roleNames)
                {
                    RoleDAL.AddUserInRole(ApplicationName, user, role);
                }
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var data = RoleDAL.SelectUsersInRoles(ApplicationName, roleName, username);
            if (data.Length > 0)
                return true;
            else
                return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            foreach (string user in usernames)
            {
                foreach (string role in roleNames)
                {
                    RoleDAL.RemoveUserInRole(ApplicationName, user, role);
                }
            }
        }

        public override bool RoleExists(string roleName)
        {
            var data = RoleDAL.Select(ApplicationName, roleName);
            if (data.Length > 0)
                return true;
            else
                return false;
        }
    }
}