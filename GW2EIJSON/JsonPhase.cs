using System.Collections.Generic;


namespace GW2EIJSON
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
        public IReadOnlyList<int> Targets { get; set; }

        /// <summary>
        /// Index of secondary targets tracked during the phase
        /// </summary>
        /// <seealso cref="JsonLog.Targets"/>
        public IReadOnlyList<int> SecondaryTargets { get; set; }

        /// <summary>
        /// Index of sub phases
        /// </summary>
        /// <seealso cref="JsonLog.Phases"/>
        public IReadOnlyList<int> SubPhases { get; set; }

        /// <summary>
        /// Indicates that the phase is a breakbar phase \n
        /// Only one target will be present in <see cref="JsonPhase.Targets"/> \n
        /// The targets breakbar will be active 2 seconds after the start of the phase
        /// </summary>
        public bool BreakbarPhase { get; set; }


        public JsonPhase()
        {

        }
    }
}
