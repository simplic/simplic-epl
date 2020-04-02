using Simplic.EPL.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.EPL
{
    /// <summary>
    /// Represents a basic EPL-Command or xEPL command
    /// </summary>
    public abstract class EPLCommand
    {
        private string data;

        /// <summary>
        /// Get parameter value without exception
        /// </summary>
        /// <param name="index">0-based index</param>
        /// <returns>parameter as string</returns>
        public virtual string GetParameter(int index)
        {
            if (Parameter == null)
            {
                return "";
            }
            else if (Parameter.Length > index)
            {
                return Parameter[index];
            }

            return "";
        }

        /// <summary>
        /// Data command content
        /// </summary>
        public virtual string Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        /// <summary>
        /// Check if the parameter sum is correct
        /// </summary>
        public virtual bool CheckParameterAmount
        {
            get
            {
                return MinParameter >= (this.Parameter ?? new string[0]).Length && MaxParameter <= (this.Parameter ?? new string[0]).Length;
            }
        }

        /// <summary>
        /// Contains a list of all available parameter
        /// </summary>
        public string[] Parameter
        {
            get;
            set;
        }

        /// <summary>
        /// Minimum parameter amount
        /// </summary>
        public abstract int MinParameter
        {
            get;
        }

        /// <summary>
        /// Maximum parameter amount
        /// </summary>
        public abstract int MaxParameter
        {
            get;
        }

        /// <summary>
        /// Command name
        /// </summary>
        public abstract string CommandName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Represents the token for the current EPL-Command
        /// </summary>
        public RawToken Token
        {
            get;
            set;
        }
    }
}
