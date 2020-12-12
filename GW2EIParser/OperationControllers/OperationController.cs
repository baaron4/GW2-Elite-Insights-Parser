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
        public string InputFile { get; }
        /// <summary>
        /// Location of the output
        /// </summary>
        public string OutLocation { get; set; }
        /// <summary>
        /// Location of the generated files
        /// </summary>
        public List<string> GeneratedFiles { get; }
        /// <summary>
        /// Location of the openable files
        /// </summary>
        public List<string> OpenableFiles { get; }
        /// <summary>
        /// Link to dps.report
        /// </summary>
        public string DPSReportLink { get; set; }
        /// <summary>
        /// Time elapsed parsing
        /// </summary>
        public string Elapsed { get; set; } = "";

        public OperationController(string location, string status)
        {
            Status = status;
            InputFile = location;
            GeneratedFiles = new List<string>();
            OpenableFiles = new List<string>();
        }

        public override void Reset()
        {
            base.Reset();
            DPSReportLink = null;
            OutLocation = null;
            Elapsed = "";
            GeneratedFiles.Clear();
            OpenableFiles.Clear();
        }

        public void FinalizeStatus(string prefix)
        {
            StatusList.Insert(0, Elapsed);
            Status = StatusList.LastOrDefault() ?? "";
            foreach (string generatedFile in GeneratedFiles)
            {
                Console.WriteLine("Generated" + $": {generatedFile}" + Environment.NewLine);
            }
            Console.WriteLine(prefix + $"{InputFile}: {Status}" + Environment.NewLine);
        }
    }
}
