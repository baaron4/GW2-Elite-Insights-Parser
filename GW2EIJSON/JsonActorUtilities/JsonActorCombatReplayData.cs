namespace GW2EIJSON;

/// <summary>
/// Class corresponding to an actor's combat replay data
/// </summary>
public class JsonActorCombatReplayData
{
    /// <summary>
    /// Time at which the actor becomes active.
    /// </summary>
    public long Start;
    /// <summary>
    /// Time at which the actor becomes inactive.
    /// </summary>
    public long End;
    /// <summary>
    /// Url to the actor's icon.
    /// </summary>
    public string? IconURL;

    /// <summary>
    /// List of 2D positions in pixels. \n
    /// The corresponding time for a given index i is ceil(<see cref="Start"/> / <see cref="JsonCombatReplayMetaData.PollingRate"/>) * <see cref="JsonCombatReplayMetaData.PollingRate"/>  + i * <see cref="JsonCombatReplayMetaData.PollingRate"/>
    /// </summary>
    public IReadOnlyList<IReadOnlyList<float>>? Positions;
    /// <summary>
    /// List of orientation angles in degree. \n
    /// The corresponding time for a given index i is ceil(<see cref="Start"/> / <see cref="JsonCombatReplayMetaData.PollingRate"/>) * <see cref="JsonCombatReplayMetaData.PollingRate"/>  + i * <see cref="JsonCombatReplayMetaData.PollingRate"/>. \n
    /// Can be null.
    /// </summary>
    public IReadOnlyList<float>? Orientations;
    /// <summary>
    /// List of time intervals between which the actor is dead. \n
    /// Can be null.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<long>>? Dead;
    /// <summary>
    /// List of time intervals between which the actor is in downstate. \n
    /// Can be null.
    /// /// </summary>
    public IReadOnlyList<IReadOnlyList<long>>? Down;
    /// <summary>
    /// List of time intervals between which the actor is disconnected/not spawned. \n
    /// Can be null.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<long>>? Dc;
}
