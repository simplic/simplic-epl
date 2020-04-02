using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.EPL
{
    /// <summary>
    /// Label width  command
    /// </summary>
    public class LabelWidthCommand : EPLCommand
    {
        /// <summary>
        /// Create
        /// </summary>
        public LabelWidthCommand()
        {
            Parameter = new string[] { "0" };
        }

        /// <summary>
        /// Label width
        /// </summary>
        [Description("The q command will cuase the image bugger to reformat and position to math the selected label (p1).")]
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
        /// Command: R
        /// </summary>
        public override string CommandName
        {
            get
            {
                return "q";
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
                return 1;
            }
        }

        /// <summary>
        /// Min parameter: 1
        /// </summary>
        public override int MinParameter
        {
            get
            {
                return 1;
            }
        }
    }
}
