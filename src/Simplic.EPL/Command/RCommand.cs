using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.EPL
{
    /// <summary>
    /// Set starting point
    /// </summary>
    public class RCommand : EPLCommand
    {
        /// <summary>
        /// Create
        /// </summary>
        public RCommand()
        {
            Parameter = new string[] { "0", "0" };
        }

        /// <summary>
        /// Horizontal starting point
        /// </summary>
        [Description("Horizontal (left) margin measured in dots.")]
        public string P1
        {
            get
            {
                return GetParameter(0);
            }
            set
            {
                Parameter[0] = value;
            }
        }

        /// <summary>
        /// Vertical starting point
        /// </summary>
        [Description("Vertical (left) margin measured in dots.")]
        public string P2
        {
            get
            {
                return GetParameter(1);
            }
            set
            {
                Parameter[1] = value;
            }
        }

        /// <summary>
        /// Command: R
        /// </summary>
        public override string CommandName
        {
            get
            {
                return "R";
            }
            internal set { }
        }

        /// <summary>
        /// Max parameter: 2
        /// </summary>
        public override int MaxParameter
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Min parameter: 2
        /// </summary>
        public override int MinParameter
        {
            get
            {
                return 2;
            }
        }
    }
}
