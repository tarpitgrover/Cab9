using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace e9.Debugging
{
    #region Enums

    public enum LogType
    {
        Undefined = 0,
        InsertError = 100,
        UpdateError = 200,
        RetreiveError = 300,
        DeletionError = 400,
        OtherError = 999,
        FileDirectoryError = 450
    }

    #endregion

    public class SystemLog
    {
        #region Public Members

        public int ID { get; set; }
        public LogType LogEntryType { get; set; }
        public DateTime ErrorDateTime { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string SystemUser { get; set; }
        public string Data { get; set; }
        public bool Resolved { get; set; }

        #endregion

        internal static void LogException(Exception exc)
        {
        }

        internal static void LogNewError(Exception exc, LogType type, Object obj = null)
        {
            string objData;
            string username;

            //if (Membership.GetUser().UserName != null) 
            //    username = Membership.GetUser().UserName;
            //else
            username = "Anonymous User";

            if (obj != null)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(obj.GetType());

                    using (StringWriter writer = new StringWriter())
                    {
                        serializer.Serialize(writer, obj);

                        objData = writer.ToString();
                    }
                }
                catch (Exception)
                {
                    objData = "Data could not be serialised";
                }
            }
            else if (exc.Source != null)
            {
                objData = exc.Source;
            }
            else
            {
                objData = "No Data Available";
            }

            SystemLog logEntry = new SystemLog()
            {
                LogEntryType = type,
                ErrorDateTime = DateTime.Now,
                Title = exc.Message,
                Body = exc.StackTrace,
                Data = objData,
                SystemUser = username
            };
            try
            {
                logEntry.Insert();
            }
            catch (Exception)
            {
            }
        }

        private void Insert()
        {
        }

    }
}