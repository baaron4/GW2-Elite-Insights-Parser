using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using Newtonsoft.Json;

namespace GW2EIBuilders.JsonModels
{
    /// <summary>
    /// Class corresponding to a phase
    /// </summary>
    public class JsonPhase
    {
        [JsonProperty]
        /// <summary>
        /// Start time of the phase
        /// </summary>
        public long Start { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// End time of the phase
        /// </summary>
        public long End { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Name of the phase
        /// </summary>
        public string Name { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Index of targets tracked during the phase
        /// </summary>
        /// <seealso cref="JsonLog.Targets"/>
        public IReadOnlyList<int> Targets { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Index of sub phases
        /// </summary>
        /// <seealso cref="JsonLog.Phases"/>
        public IReadOnlyList<int> SubPhases { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Indicates that the phase is a breakbar phase \n
        /// Only one target will be present in <see cref="JsonPhase.Targets"/> \n
        /// The targets breakbar will be active 2 seconds after the start of the phase
        /// </summary>
        public bool BreakbarPhase { get; internal set; }

        [JsonConstructor]
        internal JsonPhase()
        {

        }

        internal JsonPhase(PhaseData phase, ParsedEvtcLog log)
        {
            Start = phase.Start;
            End = phase.End;
            Name = phase.Name;
            var targets = new List<int>();
            BreakbarPhase = phase.BreakbarPhase;
            foreach (NPC tar in phase.Targets)
            {
                targets.Add(log.FightData.Logic.Targets.IndexOf(tar));
            }
            Targets = targets;
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            if (!BreakbarPhase)
            {
                var subPhases = new List<int>();
                for (int j = 1; j < phases.Count; j++)
                {
                    PhaseData curPhase = phases[j];
                    if (curPhase.Start < Start || curPhase.End > End ||
                         (curPhase.Start == Start && curPhase.End == End) || !curPhase.CanBeSubPhase)
                    {
                        continue;
                    }
                    subPhases.Add(j);
                }
                if (subPhases.Any())
                {
                    SubPhases = subPhases;
                }
            }

        }
    }
}
