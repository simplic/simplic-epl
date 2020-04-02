using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.EPL
{
    /// <summary>
    /// Barcode command
    /// </summary>
    public class BarcodeCommand : EPLCommand
    {
        /// <summary>
        /// Create label command
        /// </summary>
        public BarcodeCommand()
        {
            Parameter = new string[] { "50", "20", "0", "1", "2", "2", "B" };
            Data = "Simplic";
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
        /// Barcode selection
        /// </summary>
        [Description("Barcode selection")]
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
        /// Narrow bar width
        /// </summary>
        [Description("Narrow bar width")]
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
        /// Wide bar width
        /// </summary>
        [Description("Wide bar width")]
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
        /// Barcode height in dots
        /// </summary>
        [Description("Barcode height in dots")]
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
        /// Print human readable code (B = yes, N = no)
        /// </summary>
        [Description("Print human readable code (B = yes, N = no)")]
        public string P8
        {
            get
            {
                return GetParameter(7);
            }
            set
            {
                Parameter[7] = value;
            }
        }

        /// <summary>
        /// Command name A
        /// </summary>
        public override string CommandName
        {
            get
            {
                return "B";
            }
            internal set { }
        }

        /// <summary>
        /// Max Parameter: 8
        /// </summary>
        public override int MaxParameter
        {
            get
            {
                return 8;
            }
        }

        /// <summary>
        /// Min Parameter: 8
        /// </summary>
        public override int MinParameter
        {
            get
            {
                return 8;
            }
        }
    }
}
