using System.Collections.Generic;


namespace GW2EIJSON;

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
    public IReadOnlyList<int> Targets;

    /// <summary>
    /// Index of secondary targets tracked during the phase
    /// </summary>
    /// <seealso cref="JsonLog.Targets"/>
    public IReadOnlyList<int> SecondaryTargets;

    /// <summary>
    /// Index of sub phases
    /// </summary>
    /// <seealso cref="JsonLog.Phases"/>
    public IReadOnlyList<int> SubPhases;

    /// <summary>
    /// Indicates that the phase is a breakbar phase \n
    /// Only one target will be present in <see cref="JsonPhase.Targets"/> \n
    /// The targets breakbar will be active 2 seconds after the start of the phase
    /// </summary>
    public bool BreakbarPhase;
}
