using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.EPLEditor
{
    /// <summary>
    /// EPL-Editor instance
    /// </summary>
    public class EPLError
    {
        /// <summary>
        /// Message
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Line number
        /// </summary>
        public int Line
        {
            get;
            set;
        }

        /// <summary>
        /// Index of the token/error
        /// </summary>
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Length of the token/error
        /// </summary>
        public int Length
        {
            get;
            set;
        }

        /// <summary>
        /// Error message
        /// </summary>
        public string Text
        {
            get
            {
                return "Error: " + Message + " @ Zeile: " + Line;
            }
        }
    }
}
