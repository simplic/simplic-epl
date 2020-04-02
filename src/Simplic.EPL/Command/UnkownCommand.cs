using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.EPL
{
    /// <summary>
    /// Is able to represent all kind of epl commands, to support the complete format
    /// </summary>
    public class UnkownCommand : EPLCommand
    {
        /// <summary>
        /// Any command name
        /// </summary>
        public override string CommandName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Always true
        /// </summary>
        public override bool CheckParameterAmount
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Always 0
        /// </summary>
        public override int MaxParameter
        {
            get;
        }

        /// <summary>
        /// Always 0
        /// </summary>
        public override int MinParameter
        {
            get;
        }
    }
}
