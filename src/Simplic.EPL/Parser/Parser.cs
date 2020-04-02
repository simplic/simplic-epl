using Simplic.EPL.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Simplic.EPL
{
    /// <summary>
    /// List of states in which the parser can be
    /// </summary>
    public enum ParserState
    {
        /// <summary>
        /// No state
        /// </summary>
        None = 0,

        /// <summary>
        /// Next must be new command/line break
        /// </summary>
        ExpectNewCommand = 1,

        /// <summary>
        /// Next must be seperator
        /// </summary>
        ExpectParamSeperator = 2,

        /// <summary>
        /// Next must be some parameter
        /// </summary>
        ExpectParameter = 4
    }

    /// <summary>
    /// Simple parser for parsing epl code to command objects
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Create list of tokens from epl
        /// </summary>
        /// <param name="epl">EPL code</param>
        /// <param name="listener">Error listener</param>
        /// <returns>List of tokens</returns>
        public static IList<EPLCommand> Parse(string epl, IErrorListener listener)
        {
            if (epl == null)
            {
                throw new ArgumentNullException("epl");
            }

            if (!epl.EndsWith(Environment.NewLine))
            {
                epl += Environment.NewLine;
            }

            IList<EPLCommand> commands = new List<EPLCommand>();

            // Get list of commands, sorted by length descending
            string[] commandList = EPLCommandHelper.CommandDefinition.Select(item => item.Key).OrderByDescending(item => item.Length).ToArray();

            var eplTokenizer = new Lexer.Tokenizer(new Lexer.ParserConfiguration(), listener);
            eplTokenizer.Parse(epl);

            var tokens = eplTokenizer.Tokens;
            RawToken lastToken = null;
            RawToken token = null;

            EPLCommand lastCommand = null;

            int lineNumber = 1;
            int paramCounter = 0;

            ParserState state = ParserState.ExpectNewCommand;

            int _qCommandExists = 0;
            int _RCommandExists = 0;

            while (tokens.Count > 0)
            {
                lastToken = token;
                token = tokens.PopFirst();

                // Parse x-command
                if (token.Content.StartsWith(";"))
                {
                    lineNumber++;
                    state = ParserState.ExpectNewCommand;
                }
                else if (token.Content == "{" && state == ParserState.ExpectParameter)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{");

                    while (tokens.Count > 0)
                    {
                        token = tokens.PopFirst();

                        if (token.Content == "}")
                        {
                            sb.Append(token.Content);
                            break;
                        }
                        else if (token.Content == Environment.NewLine)
                        {
                            listener.Error("Expected token }", lineNumber, token.Index.Item1, token.Index.Item2);
                        }
                        else
                        {
                            sb.Append(token.Content);
                        }
                    }

                    lastCommand.Data = sb.ToString();
                    state = ParserState.ExpectParamSeperator | ParserState.ExpectNewCommand;
                }
                // Parse command data
                else if (token.Content.StartsWith("\"") && token.Content.EndsWith("\"") && state == ParserState.ExpectParameter && token.Content.Length > 1)
                {
                    if (lastCommand != null)
                    {
                        lastCommand.Data = token.Content.Remove(0, 1);
                        lastCommand.Data = lastCommand.Data.Remove(lastCommand.Data.Length - 1, 1);
                    }

                    state = ParserState.ExpectParamSeperator | ParserState.ExpectNewCommand;
                }
                else if (token.Content == "@{" && state == ParserState.ExpectNewCommand)
                {
                    StringBuilder sb = new StringBuilder();

                    while (tokens.Count > 0)
                    {
                        token = tokens.PopFirst();

                        if (token.Content == "}@")
                        {
                            break;
                        }
                        else
                        {
                            if (!sb.ToString().EndsWith(Environment.NewLine))
                            {
                                sb.Append(" ");
                            }

                            sb.Append(token.Content);
                        }
                    }

                    commands.Add(new ScriptCommand() { Data = sb.ToString() });
                }
                else if (token.Content == "," && (state & ParserState.ExpectParamSeperator) == ParserState.ExpectParamSeperator)
                {
                    state = ParserState.ExpectParameter;
                }
                else if (token.Content == Environment.NewLine && (state == ParserState.ExpectNewCommand || (state & ParserState.ExpectParamSeperator) == ParserState.ExpectParamSeperator))
                {
                    state = ParserState.ExpectNewCommand;
                    lineNumber++;
                }
                else if (IsCommand(commandList, token.Content) && (state & ParserState.ExpectNewCommand) == ParserState.ExpectNewCommand || (state & ParserState.ExpectParamSeperator) == ParserState.ExpectParamSeperator)
                {
                    // go through the list of commands
                    lastCommand = null;
                    paramCounter = 0;
                    foreach (string cmd in commandList)
                    {
                        if (token.Content.StartsWith(cmd))
                        {
                            lastCommand = EPLCommandHelper.GetInstance(cmd);
                            lastCommand.Token = token;

                            if (lastCommand is UnkownCommand)
                            {
                                (lastCommand as UnkownCommand).CommandName = cmd;
                            }
                            else if (lastCommand is LabelWidthCommand)
                            {
                                _qCommandExists = lineNumber;
                            }
                            else if (lastCommand is RCommand)
                            {
                                _RCommandExists = lineNumber;
                            }

                            string newTokenContent = token.Content.Remove(0, cmd.Length);
                            
                            if (newTokenContent == "" && (tokens.Count == 0 || tokens.PeekFirst().Content == Environment.NewLine))
                            {
                                state = ParserState.ExpectNewCommand;
                            }
                            else
                            {
                                state = ParserState.ExpectParameter;
                                // Add first parameter value again
                                tokens.PushFront(new Lexer.RawToken(newTokenContent, new Tuple<int, int>(0, 0), new Tuple<int, int>(0, 0)));
                            }

                            break;
                        }
                    }

                    if (lastCommand != null)
                    {
                        commands.Add(lastCommand);
                    }
                }
                else if (state == ParserState.ExpectParameter && token.Content != Environment.NewLine)
                {
                    if (lastCommand != null)
                    {
                        string[] newParamArr = new string[paramCounter + 1];

                        newParamArr[paramCounter] = token.Content;

                        if (lastCommand.Parameter != null)
                        {
                            for (int i = 0; i < paramCounter; i++)
                            {
                                newParamArr[i] = lastCommand.Parameter[i];
                            }
                        }

                        // Set token content
                        lastCommand.Parameter = newParamArr;
                        paramCounter++;
                        state = ParserState.ExpectNewCommand | ParserState.ExpectParamSeperator;
                    }
                }
                else
                {
                    listener.Error("Unexpected token near: " + (lastToken ?? token).Content, lineNumber, 0, 0);
                }
            }

            if (_qCommandExists > 0 && _RCommandExists > 0)
            {
                if (_qCommandExists < _RCommandExists)
                {
                    listener.Error("If the R-Command is sent after the q-command, the image buffer will be reformatted to printer width.", _RCommandExists, 0, 0);
                }
                else
                {
                    listener.Error("The R-command forces the printer to use the full width of the print head as the width of the label/image buffer.", _qCommandExists, 0, 0);
                }
            }

            return commands;
        }

        /// <summary>
        /// Check wehter token is command
        /// </summary>
        /// <param name="cmds">Command</param>
        /// <param name="token">Command</param>
        /// <returns></returns>
        private static bool IsCommand(string[] cmds, string token)
        {
            foreach (string cmd in cmds)
            {
                if (token.StartsWith(cmd))
                {
                    return true;
                }
            }

            return  false;
        }
    }
}
