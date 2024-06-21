using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EncounterLogic;

namespace GW2EIParserCommons
{

    public abstract class OperationController : ParserController
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
        public string OutLocation { get; internal set; }

        private readonly List<string> _GeneratedFiles;
        /// <summary>
        /// Location of the generated files
        /// </summary>
        public IReadOnlyList<string> GeneratedFiles => _GeneratedFiles;

        private readonly List<string> _OpenableFiles;
        /// <summary>
        /// Location of the openable files
        /// </summary>
        public IReadOnlyList<string> OpenableFiles => _OpenableFiles;
        /// <summary>
        /// Link to dps.report
        /// </summary>
        public string DPSReportLink { get; internal set; }

        public OperationBasicMetaData BasicMetaData { get; set; }

        /// <summary>
        /// Time elapsed parsing
        /// </summary>
        public string Elapsed { get; private set; } = "";


        private readonly Stopwatch _stopWatch = new Stopwatch();

        protected OperationController(string location, string status)
        {
            Status = status;
            InputFile = location;
            _GeneratedFiles = new List<string>();
            _OpenableFiles = new List<string>();
        }

        public override void Reset()
        {
            base.Reset();
            BasicMetaData = null;
            DPSReportLink = null;
            OutLocation = null;
            Elapsed = "";
            _GeneratedFiles.Clear();
            _OpenableFiles.Clear();
        }

        public void Start()
        {
            _stopWatch.Restart();
            _stopWatch.Start();
        }

        public void Stop()
        {
            _stopWatch.Stop();
            Elapsed = ("Elapsed " + _stopWatch.ElapsedMilliseconds + " ms");
            _stopWatch.Restart();
        }

        public void AddOpenableFile(string path)
        {
            _GeneratedFiles.Add(path);
            _OpenableFiles.Add(path);
        }

        public void AddFile(string path)
        {
            _GeneratedFiles.Add(path);
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
