using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ziptrek.CeridianLibrary
{
    /// <summary>
    /// This class handles downloading files from an SFTP site
    /// It uses a SFTPClient called Renci SSHNet
    /// </summary>
    public class SFTPDownload
    {
        protected SFTPAuthentication auth;
        protected Renci.SshNet.SftpClient _sftpClient;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="auth">Takes the authentication info for the server</param>
        public SFTPDownload(SFTPAuthentication auth)
        {
            this.auth = auth;
            //Create a new instance of the client everytime we create a new instance
            this._sftpClient = new Renci.SshNet.SftpClient(auth.host, auth.username, auth.password);
        }
        /// <summary>
        /// Saves a file to disk
        /// </summary>
        /// <param name="fileName">The name of the file </param>
        /// <param name="folderName">The path to the folder from the root of the SFTP server</param>
        /// <param name="downloadLocation">The path on the local computer to save the file to.</param>
        public void DownloadFileToDisk(string fileName, string folderName, string downloadLocation)
        {
            //Get the file and load it into a memory stream 
            System.IO.MemoryStream msOutput = DownloadFileToMemoryStream(fileName, folderName);
            //Create a new file and write the data from the memory stream to it and save
            using(System.IO.FileStream file = new System.IO.FileStream(downloadLocation  + "\\" + fileName,System.IO.FileMode.Create,System.IO.FileAccess.Write))
            {
                byte[] bytes = new byte[msOutput.Length];
                msOutput.Read(bytes, 0,(int)msOutput.Length);
                file.Write(bytes, 0, (int)msOutput.Length);
                msOutput.Close();
            }
            
        }
        /// <summary>
        /// Gets a file from a SFTP server and loads into a memory
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="folderName">Optional param Path to the folder from the root of the SFTP site </param>
        /// <returns>A memory stream with the files contents included </returns>
        public System.IO.MemoryStream DownloadFileToMemoryStream(string fileName, string folderName = "")
        {
            //Create a new connection to the specified SFTP server
            SFTPConnectionManager conn = new SFTPConnectionManager();
            _sftpClient = conn.makeConnection(auth);
            //Create an emty stream to hold the output
            System.IO.MemoryStream msOutput = new System.IO.MemoryStream();
            try
            {
                //Download the file using the SFTP Client
                _sftpClient.DownloadFile(folderName + "/" + fileName, msOutput);
                conn.closeConnection();
                return msOutput;
            }
            catch
            {
                conn.closeConnection();
                throw;
            }
        }


    }
}
