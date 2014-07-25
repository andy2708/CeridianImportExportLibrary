using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ziptrek.CeridianLibrary
{
    /// <summary>
    /// This class uses an SSH library that allows us to implement SFTP.
    /// Visit http://sshnet.codeplex.com/ for extra details on how to use and extend 
    /// </summary>
    public class SFTPUpload
    {
        //Create the auth variable
        protected SFTPAuthentication auth;
        
        //Create the SFTP Client Connection
        protected Renci.SshNet.SftpClient _sftpClient;
        
        /// <summary>
        /// Constructor for the upload
        /// </summary>
        /// <param name="auth">Takes an SFTP Authentication</param>
        public SFTPUpload(SFTPAuthentication auth)
        {
            this.auth = auth;
            //Create a new client in the constructor. This means we dont have to create one every time we do something in the class
            _sftpClient = new Renci.SshNet.SftpClient(auth.host, auth.username, auth.password);
        }
        /// <summary>
        /// Upload a file from the file system
        /// </summary>
        /// <param name="fileLocation">The location of the file on the computer</param>
        /// <param name="targetFolderName">The target folder on the SFTP server. Use string.empty for root</param>
        /// <param name="fileName">The name of the file</param>
        /// <param name="overwriteIfFileExists">Whether or not the file should be overwritten if it already exists</param>
        public void UploadFileFromFileSystem(string fileLocation, string targetFolderName, string fileName, bool overwriteIfFileExists)
        {
            SFTPConnectionManager c = new SFTPConnectionManager();
            _sftpClient = c.makeConnection(auth);
            try
            {
                //Try to upload the file.
                _sftpClient.UploadFile(System.IO.File.Open(fileLocation, System.IO.FileMode.Open), targetFolderName + "/" + fileName, overwriteIfFileExists);
                c.closeConnection();
            }
            //Throw a specific exception to handle this :TODO handle this better
            catch (Renci.SshNet.Common.SshException exSFTP)
            {
                c.closeConnection(); 
                throw;
            }
                //Throw a generic catching execption
            catch (Exception ex)
            {
                c.closeConnection();
                throw;
            }
        }
        protected internal void UploadFileFromStream(System.IO.Stream stream,string targetFolder,string fileName,bool overwriteIfFileExists)
        {
            SFTPConnectionManager c = new SFTPConnectionManager();
            _sftpClient = c.makeConnection(auth);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            try
            {
                _sftpClient.UploadFile(stream, targetFolder + "/" + fileName, overwriteIfFileExists);
                c.closeConnection();
            }
            catch (Exception ex)
            {
                c.closeConnection();
                throw;
            }
        }
        /// <summary>
        /// Rename a file on the SFTP server
        /// </summary>
        /// <param name="oldFileName">Name of the oldFile</param>
        /// <param name="newFileName">Name of the new file.</param>
        /// <param name="oldFolderPath">Name of the old Folder. Ensure this is the correct path from the root directory</param>
        /// <param name="newFolderName">Name of the new folder. Ensure this is the correct path from the root directory.</param>
        public void RenameFile(string oldFileName, string newFileName, string newFolderPath ="", string oldFolderPath="")
        {
            SFTPConnectionManager c = new SFTPConnectionManager();
            _sftpClient = c.makeConnection(auth);
            SFTPGetFile getFile = new SFTPGetFile(auth);
            //Create the empty file
            Renci.SshNet.Sftp.SftpFile file = getFile.GetFile(oldFolderPath + "/" + oldFileName);
            try
            {
                //Use Move to to handle the "rename"
                _sftpClient.RenameFile(oldFolderPath + "/" + oldFileName, newFolderPath + "/" + newFileName); 
                c.closeConnection();
            }
            catch
            {
                c.closeConnection();
                throw;
            }
            
        }
        /// <summary>
        /// This assumes that the folder is at the root level and needs to be renamed to .ready for ceridian purposes.
        /// This could be abstracted out if need be.
        /// </summary>
        /// <param name="oldFileName">The name of the file that we are changing</param>
        public void RenameFile(string oldFileName)
        {
            SFTPConnectionManager c = new SFTPConnectionManager();
            _sftpClient = c.makeConnection(auth);
            SFTPGetFile getFile = new SFTPGetFile(auth);
            Renci.SshNet.Sftp.SftpFile file = getFile.GetFile(oldFileName);
            try
            {
                //Use Move to to handle the "rename"
                _sftpClient.RenameFile(oldFileName, oldFileName + ".ready");
                c.closeConnection();
            }
            catch
            {
                c.closeConnection();
                throw;
            }
        }
       
    }
}
