using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;

namespace GW2EIParser
{

    internal abstract class OperationController : ParserController
    {
        /// <summary>
        /// Status of the parse operation
        /// </summary>
        public string Status { get; protected set; }
        /// <summary>
        /// Location of the file being parsed
        /// </summary>
        public string Location { get; }
        /// <summary>
        /// Files/directories to open when open button is clicked
        /// </summary>
        public HashSet<string> PathsToOpen { get; }
        /// <summary>
        /// Location of the generated files
        /// </summary>
        public List<string> GeneratedFiles { get; }

        internal string Elapsed { get; set; } = "";

        public OperationController(string parserName, Version parserVersion, string location, string status) : base(parserName, parserVersion)
        {
            Status = status;
            Location = location;
            PathsToOpen = new HashSet<string>();
            GeneratedFiles = new List<string>();
        }

        public void FinalizeStatus(string prefix)
        {
            StatusList.Insert(0, Elapsed);
            Status = StatusList.LastOrDefault() ?? "";
            foreach (string generatedFile in GeneratedFiles)
            {
                Console.WriteLine("Generated" +$": {generatedFile}" + Environment.NewLine);
            }
            Console.WriteLine(prefix + $"{Location}: {Status}" + Environment.NewLine);
        }
    }
}
