using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ziptrek.CeridianLibrary
{
    public class DepartmentExport :CeridianBase 
        
    {
        public string ParentOrgXrefCode { get; set; }
        public string DepartmentXrefCode { get; set; }
        public string DepartmentName { get; set; }

        public DepartmentExport() { }
        public DepartmentExport(SFTPAuthentication auth)
        {
            folderLocation = @"Export/OrgExport/archive";
            departmentExport = new CeridianExportMethod<DepartmentExport>(auth, folderLocation);
        }
        
        private CeridianExportMethod<DepartmentExport> departmentExport;
        public IEnumerable<DepartmentExport> GetDepartments()
        {
            departmentExport.getXML();
            return departmentExport.export.Record;
        }
    }
}
