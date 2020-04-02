using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.EPL
{
    /// <summary>
    /// Label command
    /// </summary>
    public class TextCommand : EPLCommand
    {
        /// <summary>
        /// Create label command
        /// </summary>
        public TextCommand()
        {
            Parameter = new string[] { "0", "0", "1", "1", "1", "N" };
            Data = "";
        }

        /// <summary>
        /// Horizontal start position (X) in dots
        /// </summary>
        [Description("Horizontal start position (X) in dots")]
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
        /// Vertical start position (X) in dots
        /// </summary>
        [Description("Vertical start position (X) in dots")]
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
        /// Rotation (0 = noram, 1 = 90 degrees, 2 = 180 degrees, 3 = 270 degrees)
        /// </summary>
        [Description("Rotation (0 = noram, 1 = 90 degrees, 2 = 180 degrees, 3 = 270 degrees)")]
        public string P3
        {
            get
            {
                return GetParameter(2);
            }
            set
            {
                Parameter[2] = value;
            }
        }

        /// <summary>
        /// Font selection
        /// </summary>
        [Description("Font selection")]
        public string P4
        {
            get
            {
                return GetParameter(3);
            }
            set
            {
                Parameter[3] = value;
            }
        }

        /// <summary>
        /// Horizontal multiplier expands the text horizontally
        /// </summary>
        [Description("Horizontal multiplier expands the text horizontally")]
        public string P5
        {
            get
            {
                return GetParameter(4);
            }
            set
            {
                Parameter[4] = value;
            }
        }

        /// <summary>
        /// Vertical multiplier expands the text vertically
        /// </summary>
        [Description("Vertical multiplier expands the text vertically")]
        public string P6
        {
            get
            {
                return GetParameter(5);
            }
            set
            {
                Parameter[5] = value;
            }
        }

        /// <summary>
        /// Riverse image (N = Normal, R = reverse image)
        /// </summary>
        [Description("Riverse image (N = Normal, R = reverse image)")]
        public string P7
        {
            get
            {
                return GetParameter(6);
            }
            set
            {
                Parameter[6] = value;
            }
        }

        /// <summary>
        /// Command name A
        /// </summary>
        public override string CommandName
        {
            get
            {
                return "A";
            }
            internal set { }
        }

        /// <summary>
        /// Max Parameter: 7
        /// </summary>
        public override int MaxParameter
        {
            get
            {
                return 7;
            }
        }

        /// <summary>
        /// Min Parameter: 8
        /// </summary>
        public override int MinParameter
        {
            get
            {
                return 7;
            }
        }
    }
}
