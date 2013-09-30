using Cab9.DataLayer.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Cab9.DataLayer
{
    internal class RoleDAL : DAL
    {
        internal static string[] Select(string ApplicationName, string RoleName = null)
        {
            var result = new List<string>();
            var Parameters = new SqlParameter[]
                {
                    new SqlParameter("ApplicationName", ApplicationName),
                    new SqlParameter("Role", RoleName)
                };
            using (var dr = SqlHelper.ExecuteReader(ConnectionString, "Cab9_Roles_Select", Parameters))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add((string)dr["Role"]);
                    }
                }
                dr.Close();
            }

            return result.ToArray();
        }

        internal static bool Insert(string ApplicationName, string RoleName)
        {
            var Parameters = new SqlParameter[]
                {
                    new SqlParameter("ApplicationName", ApplicationName),
                    new SqlParameter("Role", RoleName)
                };
            SqlHelper.ExecuteNonQuery(ConnectionString, "Cab9_Roles_Insert", Parameters);
            return true;
        }

        internal static bool Delete(string ApplicationName, string RoleName)
        {
            var Parameters = new SqlParameter[]
                {
                    new SqlParameter("ApplicationName", ApplicationName),
                    new SqlParameter("Role", RoleName)
                };
            SqlHelper.ExecuteNonQuery(ConnectionString, "Cab9_Roles_Delete", Parameters);
            return true;
        }

        internal static string[] SelectUsersInRoles(string ApplicationName, string roleName, string userName)
        {
            var result = new List<string>();
            var Parameters = new SqlParameter[]
                {
                    new SqlParameter("ApplicationName", ApplicationName),
                    new SqlParameter("User", userName),
                    new SqlParameter("Role", roleName)
                };
            using (var dr = SqlHelper.ExecuteReader(ConnectionString, "Cab9_UsersInRoles_Select", Parameters))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add((string)dr["User"]);
                    }
                }
                dr.Close();
            }
            return result.ToArray();
        }

        internal static string[] SelectRolesInUsers(string ApplicationName, string roleName, string userName)
        {
            var result = new List<string>();
            var Parameters = new SqlParameter[]
                {
                    new SqlParameter("ApplicationName", ApplicationName),
                    new SqlParameter("User", userName),
                    new SqlParameter("Role", roleName)
                };
            using (var dr = SqlHelper.ExecuteReader(ConnectionString, "Cab9_UsersInRoles_Select", Parameters))
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        result.Add((string)dr["Role"]);
                    }
                }
                dr.Close();
            }
            return result.ToArray();
        }

        internal static void AddUserInRole(string ApplicationName, string user, string role)
        {
            var Parameters = new SqlParameter[]
                {
                    new SqlParameter("ApplicationName", ApplicationName),
                    new SqlParameter("User", user),
                    new SqlParameter("Role", role)
                };
            SqlHelper.ExecuteNonQuery(ConnectionString, "Cab9_UsersInRoles_Insert", Parameters);
        }

        internal static void RemoveUserInRole(string ApplicationName, string user, string role)
        {
            var Parameters = new SqlParameter[]
                {
                    new SqlParameter("ApplicationName", ApplicationName),
                    new SqlParameter("User", user),
                    new SqlParameter("Role", role)
                };
            SqlHelper.ExecuteNonQuery(ConnectionString, "Cab9_UsersInRoles_Delete", Parameters);
        }
    }
}
