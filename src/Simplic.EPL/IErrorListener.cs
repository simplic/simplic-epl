using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.EPL
{
    /// <summary>
    /// Error listener
    /// </summary>
    public interface IErrorListener
    {
        /// <summary>
        /// Write some error messages
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="line">Line number</param>
        /// <param name="index">Starting point</param>
        /// <param name="length">End point</param>
        void Error(string message, int line, int index, int length);
    }
}
