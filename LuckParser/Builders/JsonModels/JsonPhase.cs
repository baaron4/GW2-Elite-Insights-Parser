using LuckParser.EIData;
using System.Collections.Generic;

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
        public long Start;
        /// <summary>
        /// End time of the phase
        /// </summary>
        public long End;
        /// <summary>
        /// Name of the phase
        /// </summary>
        public string Name;
        /// <summary>
        /// Index of targets tracked during the phase
        /// </summary>
        /// <seealso cref="JsonLog.Targets"/>
        public List<int> Targets;
        /// <summary>
        /// Index of sub phases
        /// </summary>
        /// <seealso cref="JsonLog.Phases"/>
        public List<int> SubPhases;

        public JsonPhase(PhaseData phase)
        {
            Start = phase.Start;
            End = phase.End;
            Name = phase.Name;
            Targets = new List<int>();
        }
    }
}
