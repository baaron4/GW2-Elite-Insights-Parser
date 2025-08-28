using System.Text.Json.Serialization;

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
    public string? Name;

    /// <summary>
    /// Indicates that this phase has no meaningful targets, only relevant in Instance logs and for Encounter type phases \n
    /// </summary>
    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //public bool? Targetless;

    /// <summary>
    /// The success status of the phase, only relevant in Instance logs and for Encounter type phases
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Success;

    /// <summary>
    /// If the phase is in challenge mode, only relevant in Instance logs and for Encounter type phases.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsCM;
    /// <summary>
    /// If the phase is in legendary challenge mode. Only relevant in Instance logs and for Encounter type phases. \n
    /// If this is true, <see cref="IsCM"/> will also be true
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsLegendaryCM;

    /// <summary>
    /// Encounter ID of the phase, only relevant in Instance logs and for Encounter type phases.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? EIEncounterID;
    /// <summary>
    /// Index of the encounter phase this phase is attached to. Only relevant for Time Frame and Sub Phase type phases. \n
    /// Will be -1 if the phase is not a part of an encounter.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EncounterPhase;

    /// <summary>
    /// DEPRECATED please use <seealso cref="JsonPhase.TargetPriorities"/> instead. \n
    /// Index of targets tracked during the phase
    /// </summary>
    /// <seealso cref="JsonLog.Targets"/>
    public IReadOnlyList<int>? Targets;

    /// <summary>
    /// DEPRECATED please use <seealso cref="JsonPhase.TargetPriorities"/> instead. \n
    /// Index of secondary targets tracked during the phase
    /// </summary>
    /// <seealso cref="JsonLog.Targets"/>
    public IReadOnlyList<int>? SecondaryTargets;


    /// <summary>
    /// Dictionary of index, indicating the priority of the target \n
    /// string in : \n
    /// "MAIN" is the main target of the phase\n
    /// "BLOCKING" is an enemy that needs to be dealt with in order to progress the phase \n
    /// "NONBLOCKING" is a relevant enemy but can be technically ignored \n
    /// </summary>
    /// <seealso cref="JsonLog.Targets"/>
    public IReadOnlyDictionary<int, string>? TargetPriorities;

    /// <summary>
    /// Index of sub phases
    /// </summary>
    /// <seealso cref="JsonLog.Phases"/>
    public IReadOnlyList<int>? SubPhases;
    /// <summary>
    /// Type of the phase in: \n
    /// - "SubPhase" for phases that are a part of an encounter \n
    /// - "TimeFrame" for phases that are a part of a log, focusing on a specific time frame of no mechanical bearing \n
    /// - "Encounter" for phases representing a complete encounter \n
    /// - "Instance" for phases representing a complete instance \n
    /// </summary>
    public string? PhaseType;

    /// <summary>
    /// Indicates that the phase is a breakbar phase \n
    /// Only one target will be present in <see cref="Targets"/> \n
    /// The targets breakbar will be active 2 seconds after the start of the phase
    /// </summary>
    public bool BreakbarPhase;
}
