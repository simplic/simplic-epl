using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.EPL.Lexer
{
    /// <summary>
    /// Lexer configuration
    /// </summary>
    public interface IEPLLexerConstants
    {
        /// <summary>
        /// Seperator tokens
        /// </summary>
        char[] TokenSeperator
        {
            get;
        }

        /// <summary>
        /// Following tokens, like >= out of > and =
        /// </summary>
        string[] FollowingTokens
        {
            get;
        }

        /// <summary>
        /// Complex tokens are tokens which has a specific start and end token, like a string: "Hello World"
        /// </summary>
        char[] ComplexToken
        {
            get;
        }

        /// <summary>
        /// String starting a single line comment
        /// </summary>
        string SingleLineComment
        {
            get;
        }

        /// <summary>
        /// String starting a multiline comment
        /// </summary>
        string StartMultilineComment
        {
            get;
        }

        /// <summary>
        /// String ending a multiline comment
        /// </summary>
        string EndMultilineComment
        {
            get;
        }
    }
}
