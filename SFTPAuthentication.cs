using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ziptrek.CeridianLibrary
{
    /// <summary>
    /// This class handles the authentication for any SFTP server
    /// </summary>
    public class SFTPAuthentication
    {
        /// <summary>
        /// The address of the host to connect to
        /// </summary>
        public string host;
        /// <summary>
        /// The username to use when connecting
        /// </summary>
        public string username;
        /// <summary>
        /// The password to use when connecting
        /// </summary>
        public string password;
        public SFTPAuthentication(string host,string username,string password)
        {
            this.host = host;
            this.username = username;
            this.password = password;
        }
        internal SFTPAuthentication() { }//Todo possible get ride of this.
        
    }
}
