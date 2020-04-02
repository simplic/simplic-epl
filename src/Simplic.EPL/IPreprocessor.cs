using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.EPL
{
    /// <summary>
    /// Processor interface for writing custom epl (xepl) processors
    /// </summary>
    public interface IPreprocessor
    {
        /// <summary>
        /// Process a statement, which result will be embedded into some epl command
        /// </summary>
        /// <param name="statement">Statement to process</param>
        /// <returns>String to embedd</returns>
        string ProcessStatement(string statement);

        /// <summary>
        /// Process a code block which has no result
        /// </summary>
        /// <param name="code">Code block to process</param>
        /// <param name="formular">EPL formular which calls the processing code</param>
        /// <param name="commandIndex">Current command index, will be needed for adding commands dynamically after or before a specific index</param>
        void Process(string code, EPLFormular formular, int commandIndex);
    }
}
