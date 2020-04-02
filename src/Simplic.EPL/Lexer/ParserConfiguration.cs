using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.EPL.Lexer
{
    /// <summary>
    /// Contains all parser, tokenizer configurations
    /// </summary>
    internal class ParserConfiguration : IEPLLexerConstants
    {
        #region Internal constants
        /// <summary>
        /// List of chars, with wich a function could start
        /// </summary>
        public static readonly char[] FunctionBeginParameterChars = new char[]
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '_'
        };

        /// <summary>
        /// Chars which are allowed in function or parameter names
        /// </summary>
        public static readonly char[] FunctionParameterChars = new char[]
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '_', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        /// <summary>
        /// Chars, which are part of <seealso cref="FunctionParameterChars"/> but not allowed at the beginning of a function or parameter
        /// </summary>
        public static readonly char[] NotBeginParameterChars = new char[]
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };
        #endregion

        #region Public Methods
        /// <summary>
        /// Proof, wether the input value is a valid class, function or variable name
        /// </summary>
        /// <param name="value">String to proof</param>
        /// <returns>True if is valid</returns>
        public bool IsValidLanguageIndependentIdentifier(string value)
        {
            return System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(value);
        }
        #endregion

        #region Public Member
        /// <summary>
        /// Seperator tokens
        /// </summary>
        public char[] TokenSeperator
        {
            get { return new char[] { ' ', '+', '-', '*', '/', '^', '.', '[', ']', ':', '>', '<', '=', '!', '{', '}', '\r', '\n', '(', ')', ',', '@', ';' }; }
        }

        /// <summary>
        /// Following tokens, like >= out of > and =
        /// </summary>
        public string[] FollowingTokens
        {
            get { return new string[] { ">=", "<=", "==", "!=", "\r\n", "/*", "*/", "//", "::", "@{", "}@" }; }
        }

        /// <summary>
        /// Complex tokens are tokens which has a specific start and end token, like a string: "Hello World"
        /// </summary>
        public char[] ComplexToken
        {
            get { return new char[] { '"' }; }
        }

        /// <summary>
        /// String starting a single line comment
        /// </summary>
        public string SingleLineComment
        {
            get { return ";"; }
        }

        /// <summary>
        /// String starting a multiline comment
        /// </summary>
        public string StartMultilineComment
        {
            get { return "/*"; }
        }

        /// <summary>
        /// String ending a multiline comment
        /// </summary>
        public string EndMultilineComment
        {
            get { return "*/"; }
        }
        #endregion
    }
}