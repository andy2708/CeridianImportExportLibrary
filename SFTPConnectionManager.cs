using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ziptrek.CeridianLibrary
{
    /// <summary>
    /// This handles the connection to the server
    /// Sometimes we make multiple connections very quickly and this checks to see whether we are connected first before 
    /// establishing another connection
    /// </summary>
    public class SFTPConnectionManager
    {
        protected Renci.SshNet.SftpClient _sftpClient;
        public Renci.SshNet.SftpClient makeConnection(SFTPAuthentication auth)
        {
            //Check to see whether we are connected already
            if (_sftpClient != null)
            {
                if (!_sftpClient.IsConnected)
                {
                     return newConnection(auth);
                }
                else
                {
                    return _sftpClient;
                }
            }
            else
                return newConnection(auth);
            //return the already connected client if we are connected
           
        }
        protected Renci.SshNet.SftpClient newConnection(SFTPAuthentication auth)
        {
            //Create a new connection if we aren't
            _sftpClient = new Renci.SshNet.SftpClient(auth.host, auth.username, auth.password);
            try
            {
                //Try and connect
                _sftpClient.Connect();
                return _sftpClient;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void closeConnection()
        {
            if (_sftpClient != null)
                _sftpClient.Disconnect();
                _sftpClient.Dispose();
            
        }
    }
}
