using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class corresponding to a phase
    /// </summary>
    public class JsonPhase
    {
        /// <summary>
        /// Start time of the phase
        /// </summary>
        public long Start { get; }
        /// <summary>
        /// End time of the phase
        /// </summary>
        public long End { get; }
        /// <summary>
        /// Name of the phase
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Index of targets tracked during the phase
        /// </summary>
        /// <seealso cref="JsonLog.Targets"/>
        public List<int> Targets { get; }
        /// <summary>
        /// Index of sub phases
        /// </summary>
        /// <seealso cref="JsonLog.Phases"/>
        public List<int> SubPhases { get; }

        public JsonPhase(PhaseData phase, ParsedLog log)
        {
            Start = phase.Start;
            End = phase.End;
            Name = phase.Name;
            Targets = new List<int>();
            foreach (NPC tar in phase.Targets)
            {
                Targets.Add(log.FightData.Logic.Targets.IndexOf(tar));
            }
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int j = 1; j < phases.Count; j++)
            {
                PhaseData curPhase = phases[j];
                if (curPhase.Start < Start || curPhase.End > End ||
                     (curPhase.Start == Start && curPhase.End == End) || !curPhase.CanBeSubPhase)
                {
                    continue;
                }
                if (SubPhases == null)
                {
                    SubPhases = new List<int>();
                }
                SubPhases.Add(j);
            }
        }
    }
}
