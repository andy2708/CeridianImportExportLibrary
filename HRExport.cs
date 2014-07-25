using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Ziptrek.CeridianLibrary
{
    /// <summary>
    /// This handles exporting HR data from Ceridian
    /// //The class deserializes from XML so the property names are case sensitive 
    /// </summary>
    public class HRExport : CeridianBase 
    {
        public int EmployeeNumber { get; set; }
        public string EmployeeXref { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public double PayRate { get; set; }
        public string Department { get; set; }
        public int DepartmentId { get; set; }
        /// <summary>
        /// Public parameterless constructor is necessary
        /// </summary>
        public HRExport(){}
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="auth">Takes an authenication package with the details of the server to connect to</param>
        public HRExport(SFTPAuthentication auth)
        {
            //Set the location of the folder on the server that we will export/import from
            folderLocation = @"Export/HRExport/archive";
            //Create a new instance of our export Method. This allows us to use the CeridianExportMethod for all Export functions 
            hrExport = new CeridianExportMethod<HRExport>(auth, folderLocation);
        }
        //Create a new instance of our export Method. This allows us to use the CeridianExportMethod for all Export functions 
        private CeridianExportMethod<HRExport> hrExport;
        /// <summary>
        /// Get the data from the server.
        /// </summary>
        public IEnumerable<HRExport> GetStaffData()
        {
            hrExport.getXML();
            return hrExport.export.Record;
        }
        public HRExport GetStaffDataByEmployeeID(int employeeId)
        {
            return GetStaffData().Where(x => x.EmployeeNumber == employeeId).First();
        }
    }
    
    
}
