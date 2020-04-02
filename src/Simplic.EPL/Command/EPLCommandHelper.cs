using System;
using System.Collections.Generic;

namespace Simplic.EPL
{
    /// <summary>
    /// Provides functions to work with epl commands
    /// </summary>
    public static class EPLCommandHelper
    {
        private static IDictionary<string, Type> commandDefinition;

        /// <summary>
        /// Initialize
        /// </summary>
        static EPLCommandHelper()
        {
            commandDefinition = new Dictionary<string, Type>();

            // Register all available commands
            commandDefinition.Add("A", typeof(TextCommand));
            commandDefinition.Add("AUTOFR", typeof(UnkownCommand));
            commandDefinition.Add("B", typeof(BarcodeCommand));
            commandDefinition.Add("b", typeof(UnkownCommand));
            commandDefinition.Add("C", typeof(UnkownCommand));
            commandDefinition.Add("D", typeof(UnkownCommand));
            commandDefinition.Add("dump", typeof(UnkownCommand));
            commandDefinition.Add("EI", typeof(UnkownCommand));
            commandDefinition.Add("EK", typeof(UnkownCommand));
            commandDefinition.Add("eR", typeof(UnkownCommand));
            commandDefinition.Add("ES", typeof(UnkownCommand));
            commandDefinition.Add("f", typeof(UnkownCommand));
            commandDefinition.Add("fB", typeof(UnkownCommand));
            commandDefinition.Add("FE", typeof(UnkownCommand));
            commandDefinition.Add("FI", typeof(UnkownCommand));
            commandDefinition.Add("FK", typeof(UnkownCommand));
            commandDefinition.Add("FR", typeof(UnkownCommand));
            commandDefinition.Add("FS", typeof(UnkownCommand));
            commandDefinition.Add("GG", typeof(UnkownCommand));
            commandDefinition.Add("GI", typeof(UnkownCommand));
            commandDefinition.Add("GK", typeof(UnkownCommand));
            commandDefinition.Add("GM", typeof(UnkownCommand));
            commandDefinition.Add("GW", typeof(UnkownCommand));
            commandDefinition.Add("i", typeof(UnkownCommand));
            commandDefinition.Add("I", typeof(UnkownCommand));
            commandDefinition.Add("JB", typeof(UnkownCommand));
            commandDefinition.Add("JC", typeof(UnkownCommand));
            commandDefinition.Add("JF", typeof(UnkownCommand));
            commandDefinition.Add("LE", typeof(UnkownCommand));
            commandDefinition.Add("LO", typeof(UnkownCommand));
            commandDefinition.Add("LS", typeof(UnkownCommand));
            commandDefinition.Add("LW", typeof(UnkownCommand));
            commandDefinition.Add("M", typeof(UnkownCommand));
            commandDefinition.Add("N", typeof(UnkownCommand));
            commandDefinition.Add("o", typeof(UnkownCommand));
            commandDefinition.Add("oB", typeof(UnkownCommand));
            commandDefinition.Add("oE", typeof(UnkownCommand));
            commandDefinition.Add("oH", typeof(UnkownCommand));
            commandDefinition.Add("oM", typeof(UnkownCommand));
            commandDefinition.Add("oR", typeof(UnkownCommand));
            commandDefinition.Add("oW", typeof(UnkownCommand));
            commandDefinition.Add("O", typeof(UnkownCommand));
            commandDefinition.Add("OEPL1", typeof(UnkownCommand));
            commandDefinition.Add("P", typeof(UnkownCommand));
            commandDefinition.Add("PA", typeof(UnkownCommand));
            commandDefinition.Add("q", typeof(LabelWidthCommand));
            commandDefinition.Add("Q", typeof(LabelHeightCommand));
            commandDefinition.Add("r", typeof(UnkownCommand));
            commandDefinition.Add("R", typeof(RCommand));
            commandDefinition.Add("S", typeof(UnkownCommand));
            commandDefinition.Add("T", typeof(UnkownCommand));
            commandDefinition.Add("TS", typeof(UnkownCommand));
            commandDefinition.Add("TT", typeof(UnkownCommand));
            commandDefinition.Add("U", typeof(UnkownCommand));
            commandDefinition.Add("UA", typeof(UnkownCommand));
            commandDefinition.Add("UB", typeof(UnkownCommand));
            commandDefinition.Add("UE", typeof(UnkownCommand));
            commandDefinition.Add("UF", typeof(UnkownCommand));
            commandDefinition.Add("UG", typeof(UnkownCommand));
            commandDefinition.Add("UI", typeof(UnkownCommand));
            commandDefinition.Add("UM", typeof(UnkownCommand));
            commandDefinition.Add("UN", typeof(UnkownCommand));
            commandDefinition.Add("UP", typeof(UnkownCommand));
            commandDefinition.Add("UQ", typeof(UnkownCommand));
            commandDefinition.Add("US", typeof(UnkownCommand));
            commandDefinition.Add("UT", typeof(UnkownCommand));
            commandDefinition.Add("U%", typeof(UnkownCommand));
            commandDefinition.Add("U$", typeof(UnkownCommand));
            commandDefinition.Add("V", typeof(UnkownCommand));
            commandDefinition.Add("W", typeof(UnkownCommand));
            commandDefinition.Add("xa", typeof(UnkownCommand));
            commandDefinition.Add("X", typeof(UnkownCommand));
            commandDefinition.Add("Y", typeof(UnkownCommand));
            commandDefinition.Add("Z", typeof(UnkownCommand));
            commandDefinition.Add("?", typeof(UnkownCommand));
            commandDefinition.Add("^@", typeof(UnkownCommand));
            commandDefinition.Add("^default", typeof(UnkownCommand));
            commandDefinition.Add("^ee", typeof(UnkownCommand));
            commandDefinition.Add(";", typeof(UnkownCommand));
            commandDefinition.Add(Environment.NewLine, typeof(UnkownCommand));
        }

        /// <summary>
        /// Get an instance of an epl-command
        /// </summary>
        /// <param name="name">Command name</param>
        /// <returns>EPL-Command instance</returns>
        public static EPLCommand GetInstance(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name can not be null or empty");
            }

            if (!commandDefinition.ContainsKey(name))
            {
                throw new MissingMethodException("Could not found epl command: " + name);
            }
            else
            {
                return (EPLCommand)Activator.CreateInstance(commandDefinition[name]);
            }
        }

        /// <summary>
        /// Get a list of available epl commands
        /// </summary>
        public static IDictionary<string, Type> CommandDefinition
        {
            get
            {
                return commandDefinition;
            }
        }
    }
}
