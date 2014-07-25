using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ziptrek.CeridianLibrary
{
    /// <summary>
    /// This class handles getting files and file details using a SFTP client made by Renci
    /// </summary>
    public class SFTPGetFile
    {
        //Create the auth variable
        protected SFTPAuthentication auth;

        //Create the SFTP Client Connection
        protected Renci.SshNet.SftpClient _sftpClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="auth">Takes the authentication info for the server</param>
        public SFTPGetFile(SFTPAuthentication auth)
        {
            this.auth = auth;
            //Create a new instance of the client straight away
            this._sftpClient = new Renci.SshNet.SftpClient(auth.host, auth.username, auth.password);
        }
        /// <summary>
        /// Get and return a file 
        /// </summary>
        /// <param name="fileName">The name of the file on the SFTP server to get</param>
        /// <param name="folderPath">Optional Param. The path to the file from the root of the SFTP server</param>
        /// <returns></returns>
        public Renci.SshNet.Sftp.SftpFile GetFile(string fileName, string folderPath = "")
        {
            //Create a new connection to the server using the authentication provided
            SFTPConnectionManager c = new SFTPConnectionManager();
            _sftpClient = c.makeConnection(auth);
            try
            {
               //Return the file using the SFTP client
                Renci.SshNet.Sftp.SftpFile file = _sftpClient.Get(folderPath + "/" + fileName);
                c.closeConnection();
                return file;

            }
            catch(Renci.SshNet.Common.SshException sshException)
            {
                c.closeConnection();
                throw;
            }
            catch(Exception ex)
            {
                c.closeConnection();
                throw;
            }
        }
        /// <summary>
        /// Gets the last updated file from the folder specified. This allows us to not worry about the name of the file, just what the timestamp is
        /// </summary>
        /// <param name="folderPath">The path to the folder from the root of the SFTP</param>
        /// <returns>A string that contains the name of the last file modified</returns>
        public string GetLastModifiedFileName(string folderPath)
        {
            IEnumerable<Renci.SshNet.Sftp.SftpFile> files = getFileList(folderPath);
            //Use linq to sort the files by write time. We also need to make sure the length isnt 0 as it appears the SFTP client writes some kind of tiny file to the server
            //when it is connected (hence the where clause). Then grab the name of the first one.
            return files.OrderByDescending(x => x.LastAccessTime).Where(x=>x.Length > 10).First().Name;
        }
        public string GetLastModifiedFileName(string folderPath,string filenameContains)
        {
            IEnumerable<Renci.SshNet.Sftp.SftpFile> files = getFileList(folderPath);
            //Use linq to sort the files by write time. We also need to make sure the length isnt 0 as it appears the SFTP client writes some kind of tiny file to the server
            //when it is connected (hence the where clause). Then grab the name of the first one.
            return files.OrderByDescending(x => x.LastAccessTime).Where(x => x.Length > 10).Where(x=>x.Name.Contains(filenameContains)).First().Name;
        }
        private IEnumerable<Renci.SshNet.Sftp.SftpFile> getFileList (string folderPath)
        {
            //Create a new connection to the server using the authentication provided
            SFTPConnectionManager c = new SFTPConnectionManager();
            _sftpClient = c.makeConnection(auth);
            //Get a list of all the files in the folder using the Renci tool
           IEnumerable<Renci.SshNet.Sftp.SftpFile> fileList = _sftpClient.ListDirectory(folderPath);
           c.closeConnection();
           return fileList;
        }

         

    }
}
