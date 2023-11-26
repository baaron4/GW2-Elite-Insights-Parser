using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EncounterLogic;

namespace GW2EIParser
{

    internal abstract class OperationController : ParserController
    {

        public class OperationBasicMetaData
        {
            public OperationBasicMetaData(ParsedEvtcLog log)
            {
                FightDuration = log.FightData.DurationString;
                FightName = log.FightData.FightName;
                FightSuccess = log.FightData.Success;
                FightCategory = log.FightData.Logic.EncounterCategoryInformation;
                Icon = log.FightData.Logic.Icon;
                LogStart = log.LogData.LogStartStd;
                LogEnd = log.LogData.LogEndStd;
            }

            public string FightDuration { get; }
            public string FightName { get; }
            public bool FightSuccess { get; set; }
            public EncounterCategory FightCategory { get; }
            public string Icon { get; }
            public string LogStart { get; }
            public string LogEnd { get; }
        }

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
        internal string OutLocation { get; set; }
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
        internal string DPSReportLink { get; set; }

        public OperationBasicMetaData BasicMetaData { get; set; }

        /// <summary>
        /// Time elapsed parsing
        /// </summary>
        public string Elapsed { get; private set; } = "";


        private readonly Stopwatch _stopWatch = new Stopwatch();

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
            BasicMetaData = null;
            DPSReportLink = null;
            OutLocation = null;
            Elapsed = "";
            GeneratedFiles.Clear();
            OpenableFiles.Clear();
        }

        public void Start()
        {
            _stopWatch.Restart();
            _stopWatch.Start(); 
        }

        public void Stop() { 
            _stopWatch.Stop();
            Elapsed = ("Elapsed " + _stopWatch.ElapsedMilliseconds + " ms");
            _stopWatch.Restart();
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
