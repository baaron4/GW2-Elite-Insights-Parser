using System.Collections.Generic;
using LuckParser.EIData;

namespace LuckParser.Builders.JsonModels
{
    /// <summary>
    /// Class corresponding to a phase
    /// </summary>
    public class JsonPhase
    {
        /// <summary>
        /// Start time of the phase
        /// </summary>
        public long Start { get; set; }
        /// <summary>
        /// End time of the phase
        /// </summary>
        public long End { get; set; }
        /// <summary>
        /// Name of the phase
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Index of targets tracked during the phase
        /// </summary>
        /// <seealso cref="JsonLog.Targets"/>
        public List<int> Targets { get; set; }
        /// <summary>
        /// Index of sub phases
        /// </summary>
        /// <seealso cref="JsonLog.Phases"/>
        public List<int> SubPhases { get; set; }

        public JsonPhase(PhaseData phase)
        {
            Start = phase.Start;
            End = phase.End;
            Name = phase.Name;
            Targets = new List<int>();
        }
    }
}
