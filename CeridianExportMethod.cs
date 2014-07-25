using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Ziptrek.CeridianLibrary
{
    /// <summary>
    /// Generic Ceridian calls to get data from Ceridian
    /// </summary>
    /// <typeparam name="REQ">A custom Export class that handles exporting data from Ceridian</typeparam>
    public class CeridianExportMethod<EXP> where EXP  : new()
    {
        /// <summary>
        /// The structure of this class must serialize to XML matching the structure of the file exported from Ceridian
        /// </summary>
        /// 
        [XmlRoot("Export")]
        public class CeridianExport
        {
           [XmlElement("Record")]
           public List<EXP> Record = new List<EXP>();
           [XmlIgnore] 
           public string folderPath { get; set; }
           [XmlIgnore] 
           public SFTPAuthentication auth { get; set; }
           [XmlIgnore]
           public string filenameContains { get; set; }
        }
        public CeridianExport export = new CeridianExport();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="auth">The authentication for the SFTP Server that we connect to to pull the data from Ceridian</param>
        /// <param name="folderPath">The folder location inside the SFTP Server</param>
        public CeridianExportMethod(SFTPAuthentication auth,string folderPath)
        {
            export.auth = auth;
            export.folderPath = folderPath;
        }
        public CeridianExportMethod(SFTPAuthentication auth, string folderPath, string filenameContains)
        {
            export.auth = auth;
            export.folderPath = folderPath;
            export.filenameContains = filenameContains;
        }

        /// <summary>
        /// Connect to the SFTP server, get the file and deserialize it 
        /// </summary>
        public void getXML()
        {
            //Use the Ziptrek SFTP Library to access the server
            SFTPGetFile _sftpGetFile = new SFTPGetFile(export.auth);
            string newestFile = _sftpGetFile.GetLastModifiedFileName(export.folderPath);
            //if (string.IsNullOrEmpty(export.filenameContains))
            //{
            //    //Find the file that was modified last and get the name of it.
            //    newestFile = _sftpGetFile.GetLastModifiedFileName(export.folderPath);
            //}
            //else
            //{
            //    newestFile = _sftpGetFile.GetLastModifiedFileName(export.folderPath, export.filenameContains);
            //}
            SFTPDownload _sftpDownload = new SFTPDownload(export.auth);
            //Download the file to a memory stream
            System.IO.MemoryStream memStream = _sftpDownload.DownloadFileToMemoryStream(newestFile, export.folderPath);
            //Make sure we read the stream from the beginning
            memStream.Seek(0, System.IO.SeekOrigin.Begin);
            //string ts = UTF8Encoding.UTF8.GetString(memStream.ToArray());
            //Convert the xml to the correct object
            XmlSerializer xmlExport = new XmlSerializer(export.GetType());
            try
            {
                export = (CeridianExport)xmlExport.Deserialize(memStream);
                               
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Cannot deserialzie: " + ex.Message);
            }
        }
       
    }
}
