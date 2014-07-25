using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ziptrek.CeridianLibrary
{
    /// <summary>
    /// A class that is common across all import and export operations
    /// </summary>
    public class CeridianBase
    {
        /// <summary>
        /// The only common link between these operations is that they all need to have a folder to either import to or export from
        /// </summary>
        internal string folderLocation { get; set; }
    }
}
