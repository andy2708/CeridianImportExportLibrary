using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Ziptrek.CeridianLibrary
{
    /// <summary>
    /// A class to handle importing schedule data to ceridian.
    /// This serilalizes and must match the XML format as provided by Ceridian.
    /// </summary>
    public class ScheduleImport
    {
        //Location of the folder that the file needs to get copied to be imported into Ceridian
        internal const string folderLocation = "Import/EmployeeScheduleImport";
        SFTPAuthentication auth = new SFTPAuthentication();
        [XmlRoot("EmployeeScheduleImport")]
        public class EmployeeScheduleImport
        {
            [XmlElement(Order = 1)]
            public DateTime StartTime { get; set; }
            [XmlElement(Order = 2)]
            public DateTime EndTime { get; set; }
            [XmlElement(Order = 3)]
            public string DeleteLevel { get; set; }
            [XmlElement (Order = 4)]
            public string ValidationLevel { get; set; }
            public class EmployeeSchedule
            {
                [XmlElement(Order=1)]
                public string EmployeeXrefCode { get; set; }
                [XmlElement(Order = 2)]
                public DateTime StartTime { get; set; }
                [XmlElement(Order = 3)]
                public DateTime EndTime { get; set; }
                public class Break
                {
                    public DateTime StartTime { get; set; }
                    public DateTime EndTime { get; set; }
                    public string BreakType { get; set; }
                }
                public class Activity
                {
                    public DateTime StartTime { get; set; }
                    public DateTime EndTime { get; set; }
                    public string ActivityXrefCode { get; set; }
                }
                [XmlElement("Activity",Order=4)]
                public Activity _activity = new Activity();
                //[XmlElement("Break",Order=5)]
                //public Break _break  = new Break();
               
            }
            [XmlElement("EmployeeSchedule",Order=5)]
            public List<EmployeeSchedule> employeeSchedules = new List<EmployeeSchedule>();
        }
        //Need a parameterless constructor for the serialization
        private ScheduleImport() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="auth">Takes the authentication of the server it is going to connect to.</param>
        public ScheduleImport(SFTPAuthentication auth)
        {
            this.auth = auth;
        }

        public bool ImportXML(ScheduleImport.EmployeeScheduleImport emp)
        {
            // Create a new instance of the xmlSerializer
            XmlSerializer xmlSerializer = new XmlSerializer(emp.GetType());
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            System.IO.MemoryStream memStream = new System.IO.MemoryStream();
            //Serialize the data
            xmlSerializer.Serialize(memStream, emp, ns);
            //Upload the file to the server using the Ziptrek SFTP library
            SFTPUpload _sftpUpload = new SFTPUpload(this.auth);
            _sftpUpload.UploadFileFromStream(memStream, folderLocation, "Test.xml", true);
            //Rename the file after it is copied so that it can be imported into ceridian
            _sftpUpload.RenameFile("Test.xml", "Test.xml.ready", folderLocation, folderLocation);
            return true;
        }
    }
}
