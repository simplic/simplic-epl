using Simplic.Printing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.EPL
{
    /// <summary>
    /// Represents a complete EPL form
    /// </summary>
    public class EPLFormular : IPrintable
    {
        private IList<EPLCommand> commands;
        private IErrorListener listener;
        private IPreprocessor processor;

        /// <summary>
        /// Create new epl formular
        /// </summary>
        /// <param name="listener">Error listener</param>
        /// <param name="epl">Optional epl code</param>
        public EPLFormular(IErrorListener listener, string epl = null)
        {
            this.listener = listener;

            if (!string.IsNullOrWhiteSpace(epl))
            {
                commands = Parser.Parse(epl, listener);
            }
            else
            {
                this.commands = new List<EPLCommand>();
            }
        }

        /// <summary>
        /// Fill epl formular
        /// </summary>
        /// <param name="epl">epl code</param>
        public void Load(string epl)
        {
            commands = Parser.Parse(epl, listener);
        }

        /// <summary>
        /// Print the current label
        /// </summary>
        /// <param name="device">Label</param>
        public void Print(PrinterDevice device)
        {
            string epl = GetEPLCode();
            RawPrinterHelper.SendStringToPrinter(device.QueueName, epl);
        }

        /// <summary>
        /// Get the list of commands as EPL-Code
        /// </summary>
        /// <param name="callPreprocessor">If set to true, the preprocessor will be used for processing data</param>
        /// <returns>EPL code with or without processed information</returns>
        public string GetEPLCode(bool callPreprocessor = true)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            int i = 0;

            foreach (var cmd in commands)
            {
                // Script commands, they are not EPL-Compatible
                if (cmd is ScriptCommand)
                {
                    // Just process
                    if (callPreprocessor && processor != null)
                    {
                        var result = processor.Process(cmd.Data, this, i);
                        if (!string.IsNullOrWhiteSpace(result))
                            sb.Append(result);
                    }

                    continue;
                }

                // if not empty, create new line
                if (sb.Length > 0 || isFirst)
                {
                    sb.Append("\r\n");
                    isFirst = false;
                }

                // Command
                sb.Append(cmd.CommandName);

                // Parameter
                int p = 0;
                if (cmd.Parameter != null)
                {
                    foreach (var param in cmd.Parameter)
                    {
                        if (p > 0)
                        {
                            sb.Append(",");
                        }

                        if (!callPreprocessor || processor == null)
                        {
                            sb.Append(param);
                        }
                        else
                        {
                            if (param.StartsWith("{") && param.EndsWith("}"))
                            {
                                // Remove { <...> }
                                string toAppend = param.Remove(0, 1);
                                toAppend = toAppend.Remove(toAppend.Length - 1, 1);

                                // call preprocessor and append
                                sb.Append(processor.ProcessStatement(toAppend));
                            }
                            else
                            {
                                // Just append, no x-command
                                sb.Append(param);
                            }
                        }
                        p++;
                    }
                }

                // Data
                if (cmd.Data != null)
                {
                    if (p > 0)
                    {
                        sb.Append(",");
                    }

                    if (!callPreprocessor || processor == null)
                    {
                        sb.Append("\"" + cmd.Data + "\"");
                    }
                    else
                    {
                        if (cmd.Data.StartsWith("{") && cmd.Data.EndsWith("}"))
                        {
                            // Remove { <...> }
                            string toAppend = cmd.Data.Remove(0, 1);
                            toAppend = toAppend.Remove(toAppend.Length - 1, 1);

                            // call preprocessor and append
                            sb.Append("\"" + processor.ProcessStatement(toAppend) + "\"");
                        }
                        else
                        {
                            // Just append, no x-command
                            sb.Append("\"" + cmd.Data + "\"");
                        }
                    }
                }

                i++;
            }

            // end with empty line
            sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// Get list of EPL-Commands
        /// </summary>
        public IList<EPLCommand> Commands
        {
            get
            {
                return commands;
            }
        }

        /// <summary>
        /// EPL processor
        /// </summary>
        public IPreprocessor Processor
        {
            get
            {
                return processor;
            }

            set
            {
                processor = value;
            }
        }
    }
}
