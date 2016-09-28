using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using ArduinoWrapper;

namespace ArduinoStudio
{
    /// <summary>
    /// Store board configuration
    /// </summary>
    public class BoardConfig
    {
        // Class that allows one to identify a board (even when it moves over ports)
        public BoardIdentifier COM { get; set; }
        // Board configuration (Arduino compiler --board string)
        public CompilerConfig CompilerConfiguration { get; set; }
        // Short board name
        public string Name { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BoardConfig()
        {
            Name = "Board name";            
        }

        /// <summary>
        /// Board option
        /// </summary>
        public string CompilerBoardOption
        {
            get { return CompilerConfiguration.CompilerBoardOption();  }
        }


        /// <summary>
        /// Return COM port
        /// </summary>
        public string COMPort
        {
            get { return COM.LastKnownCOM; }
        }
        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

    }

    /// <summary>
    /// Compiler board config option, storage and generation
    /// </summary>
    public class CompilerConfig
    {
        public string Package { get; set; }
        public string Architecture { get; set; }
        public string Board { get; set; }
        public Dictionary<string, string> Options;

        /// <summary>
        /// Constructor
        /// </summary>
        public CompilerConfig()
        {
            Package = "arduino";
            Architecture = "avr";
            Board = "nano";
            Options = new Dictionary<string, string>();
        }

        /// <summary>
        /// Option for the --board compiler parameter
        /// </summary>
        /// <returns></returns>
        public string CompilerBoardOption()
        {
            StringBuilder sb = new StringBuilder();
            // Package:Architecture:Board format
            sb.AppendFormat("{0}:{1}:{2}", Package, Architecture, Board);            
            // Add options when they are there
            if (Options.Count > 0)
            {
                sb.Append(":");
                int no = Options.Count;
                foreach (KeyValuePair<string,string> kv in Options)
                {
                    sb.AppendFormat("{0}={1}", kv.Key, kv.Value);
                    if ((no--) > 0)
                    {
                        sb.Append(",");
                    }
                }
            }
            // Return string
            return sb.ToString();
        }


        /// <summary>
        /// Return board config parameter as ToString representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return CompilerBoardOption();
        }
    }
}
