using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Ziptrek.CeridianLibrary
{
    //This handles the exporting of schedules from Ceridian
    //The class deserializes from XML so the property names are case sensitive 
    public class ScheduleExport : CeridianBase
    {
            public string schedHash { get; set; }
            [XmlIgnore]
            public int EmployeeNumber { get; set; }
            [XmlElement("EmployeeNumber")]
            public string EmployeeNumberSerializedDoNotUse
            {
                get { return EmployeeNumber.ToString(); }
                set
                {
                    if (value == "")
                        EmployeeNumber = 237122;
                    else
                        EmployeeNumber = Convert.ToInt16(value);
                }
            }
            public string EmployeeFirstName { get; set; }
            public string EmployeeLastName { get; set; }
            public string OrgName { get; set; }
            public string SchedShiftType { get; set; }
            //Fun with dates as Ceridian's dates aren't XML standard and cause the deserialization to break
            //We use custom getters and setters to format the date into something useable
            [XmlIgnore]
            DateTime SchedTimeStart { get; set; }
            [XmlElement("SchedTimeStart")]
            public string SchedTimeStartSerializedDoNotUse
            {
                get { return SchedTimeStart.ToString(); }
                set { SchedTimeStart = DateTime.ParseExact(value, "M/d/yyyy h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture); }
            }
            [XmlIgnore]
            public DateTime SchedTimeEnd { get; set; }
            [XmlElement("SchedTimeEnd")]
            public string SchedTimeEndSerializedDoNotUse
            {
                get { return SchedTimeEnd.ToString(); }
                set { SchedTimeEnd = DateTime.ParseExact(value, "M/d/yyyy h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture); }
            }
                  
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="auth">Takes the authentication for the server it is going to connect to</param>
        public ScheduleExport(SFTPAuthentication auth)
        {
           //Set the location of the folder that we are exporting/importing to. This is from the base class
           folderLocation = @"Export/ScheduleExport/archive";
           //Create a new instance of our export Method. This allows us to use the CeridianExportMethod for all Export functions 
           scheduleExportCurrent = new CeridianExportMethod<ScheduleExport>(auth, folderLocation,"Next");
           scheduleExportNext = new CeridianExportMethod<ScheduleExport>(auth,folderLocation,"Current");
        }
        /// <summary>
        /// Parameterless constrructor for deserializing 
        /// </summary>
        public ScheduleExport() { }
        
        //Create a new instance of our export Method. This allows us to use the CeridianExportMethod for all Export functions 
        private CeridianExportMethod<ScheduleExport> scheduleExportCurrent;
        private CeridianExportMethod<ScheduleExport> scheduleExportNext;
        /// <summary>
        /// Get the data from the SFTP site and make it usable. Not sure what we are doing with it yet but it works.
        /// </summary>
        public IEnumerable<ScheduleExport> GetScheduleData()
        {
            List<ScheduleExport> scheduleData = new List<ScheduleExport>();
            scheduleExportCurrent.getXML();
            scheduleData.AddRange(scheduleExportCurrent.export.Record);
            scheduleExportNext.getXML();
            scheduleData.AddRange(scheduleExportNext.export.Record);            
           
            return scheduleData;           
            
        }
        public IEnumerable<ScheduleExport> GetScheduleDataByDate(DateTime date)
        {
            return GetScheduleData().Where(x => x.SchedTimeStart.Date == date).ToList();
        }
        public ScheduleExport GetScheduleDataByDateAndEmployeeID(DateTime date, int employeeId)
        {
            return GetScheduleData().Where(x => x.EmployeeNumber == employeeId).Where(x => x.SchedTimeStart.Date == date).First();
        }

        
    }
}
