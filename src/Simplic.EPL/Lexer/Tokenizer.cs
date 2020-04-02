using Simplic.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.EPL.Lexer
{
    /// <summary>
    /// Tokenizer, which split the code into single tokens
    /// </summary>
    public class Tokenizer
    {
        #region Private Member
        private Dequeue<RawToken> tokens;
        private IEPLLexerConstants lexerConstants;
        private IErrorListener errorListener;
        #endregion

        #region Constructor
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="lexerConstants">Lexer constants / config</param>
        /// <param name="errorListener">error listener for error capturing</param>
        public Tokenizer(IEPLLexerConstants lexerConstants, IErrorListener errorListener)
        {
            tokens = new Dequeue<RawToken>();
            this.lexerConstants = lexerConstants;
            this.errorListener = errorListener;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Parse script into tokens
        /// </summary>
        /// <param name="Code"></param>
        public void Parse(string Code)
        {
            string lastToken = "";
            bool lastTokenIsSeperator = false;

            for (int i = 0; i < Code.Length; i++)
            {
                char currentChar = Code[i];

                // Add Seperators directly as a token
                if (lexerConstants.TokenSeperator.Contains(currentChar))
                {
                    if (lastToken.Trim() != "")
                    {
                        tokens.PushBack(new RawToken(lastToken.Trim(), null, new Tuple<int, int>(i - lastToken.Trim().Length, i)));

                        lastToken = "";
                    }

                    if (currentChar != ' ')
                    {
                        string toEnqueu = currentChar.ToString();

                        // Make from two seperate tokens (> =) one token >=
                        if (lastTokenIsSeperator == true)
                        {
                            if (tokens.Count != 0)
                            {
                                RawToken lastTokenChar = tokens.PeekLast();

                                foreach (string fol in lexerConstants.FollowingTokens)
                                {
                                    if ((lastTokenChar.Content + currentChar) == fol)
                                    {
                                        tokens.PopLast();
                                        toEnqueu = fol;
                                        break;
                                    }
                                }
                            }
                        }

                        // Single-Line comment
                        if (lexerConstants.SingleLineComment == toEnqueu)
                        {
                            string commentString = "";

                            for (i = i - (i > 0 ? 1 : 0); i < Code.Length; i++)
                            {
                                commentString += Code[i];

                                if (commentString.EndsWith(Environment.NewLine))
                                {
                                    break;
                                }
                            }

                            lastToken = "";
                        }
                        // Multiline comment
                        else if (lexerConstants.StartMultilineComment == toEnqueu)
                        {
                            string commentString = "";

                            bool commentClosed = false;
                            for (i = i - 1; i < Code.Length; i++)
                            {
                                commentString += Code[i];

                                if (commentString.EndsWith(lexerConstants.EndMultilineComment))
                                {
                                    commentClosed = true;
                                    break;
                                }
                            }

                            // Proof, wether the comment is closed
                            if (commentClosed == false)
                            {
                                errorListener.Error("Multiline comment not closed.", -1, i, i - 3);
                            }

                            lastToken = "";
                        }
                        else
                        {
                            tokens.PushBack(new RawToken(toEnqueu, null, new Tuple<int, int>((i + 1) - toEnqueu.Trim().Length, (i + 1))));

                            lastTokenIsSeperator = true;

                        }
                    }
                }
                else if (lexerConstants.ComplexToken.Contains(currentChar))
                {
                    if (lastToken.Trim() != "")
                    {
                        tokens.PushBack(new RawToken(lastToken.Trim(), null, new Tuple<int, int>(i - lastToken.Trim().Length, i)));
                        lastToken = "";
                    }

                    // Get Brackets like quoated strings
                    QuotedParameterParserResult result = GetNextComplexString(Code.Substring(i, Code.Length - i), currentChar, i);

                    i += (result.Result.Length - 1) + result.RemovedChars;

                    tokens.PushBack(new RawToken(result.Result.Trim(), null, new Tuple<int, int>(i - ((result.Result.Length - 1) + result.RemovedChars), (i + 1))));

                    lastTokenIsSeperator = false;
                }
                else if (currentChar == '\t')
                {
                    // Do nothing then
                    if (lastToken.Trim() != "")
                    {
                        tokens.PushBack(new RawToken(lastToken.Trim(), null, new Tuple<int, int>(i - lastToken.Trim().Length, i)));

                        lastToken = "";
                    }
                }
                else
                {
                    lastToken += currentChar;

                    lastTokenIsSeperator = false;
                }
            }

            if (lastToken.Trim().Length > 0)
            {
                tokens.PushBack(new RawToken(lastToken.Trim(), null, new Tuple<int, int>(Code.Length - lastToken.Trim().Length, Code.Length)));
            }
        }
        #endregion

        #region Private Methods
        private QuotedParameterParserResult GetNextComplexString(string Input, char StartEndChar, int startPos)
        {
            // Define return-value / vars
            QuotedParameterParserResult returnValue = new QuotedParameterParserResult();
            returnValue.RemovedChars = 0;
            returnValue.Result = "";

            bool addNextDirect = false;
            int unescapedQuotes = 0;

            for (int i = 0; i < Input.Length; i++)
            {
                if (addNextDirect)
                {
                    returnValue.Result += Input[i];

                    addNextDirect = false;
                    continue;
                }

                if (Input[i] == StartEndChar)
                {
                    unescapedQuotes++;
                }

                if (Input[i] == '\\')
                {
                    // This char will not be added to the result
                    returnValue.RemovedChars++;

                    addNextDirect = true;
                    continue;
                }
                returnValue.Result += Input[i];

                // Leave if all unescaped quoates are closed
                if (unescapedQuotes == 2)
                {
                    returnValue.Result = returnValue.Result.Substring(0, (i - returnValue.RemovedChars) + 1);
                    break;
                }
            }

            if (unescapedQuotes % 2 != 0)
            {
                errorListener.Error("Expected close token: " + StartEndChar.ToString(), -1, startPos, 1);
            }

            return returnValue;
        }
        #endregion

        #region Public Member
        /// <summary>
        /// Tokens from the current tokenizer
        /// </summary>
        public Dequeue<RawToken> Tokens
        {
            get { return tokens; }
            set { tokens = value; }
        }
        #endregion
    }
}
