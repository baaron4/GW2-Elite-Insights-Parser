using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using GW2EIUtils;

namespace GW2EIParser
{

    public abstract class OperationController : OperationTracer
    {
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

        public OperationController(string location, string status) : base(status)
        {
            Location = location;
            PathsToOpen = new HashSet<string>();
            GeneratedFiles = new List<string>();
        }

        public override void FinalizeStatus(string prefix)
        {
            Status = StatusList.LastOrDefault() ?? "";
            foreach (string generatedFile in GeneratedFiles)
            {
                Console.WriteLine("Generated" +$": {generatedFile}" + Environment.NewLine);
            }
            Console.WriteLine(prefix + $"{Location}: {Status}" + Environment.NewLine);
        }
    }
}
