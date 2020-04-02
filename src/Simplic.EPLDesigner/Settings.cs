using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.EPLDesigner
{
    /// <summary>
    /// General application settings
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Printer header width
        /// </summary>
        public double PrinterHeaderWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Default label width
        /// </summary>
        public double DefaultLabelWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Default label height
        /// </summary>
        public double DefaultLabelHeight
        {
            get;
            set;
        }

        /// <summary>
        /// List of recent files
        /// </summary>
        public IList<string> RecentFiles
        {
            get;
            set;
        }
    }
}
