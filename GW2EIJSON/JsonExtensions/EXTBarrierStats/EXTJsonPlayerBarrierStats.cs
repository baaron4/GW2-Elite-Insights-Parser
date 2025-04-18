﻿namespace GW2EIJSON;

/// <summary>
/// Class regrouping player related barrier statistics
/// </summary>
public class EXTJsonPlayerBarrierStats
{
    /// <summary>
    /// Array of Total Allied Outgoing Barrier stats \n
    /// Length == # of players and the length of each sub array is equal to # of phases
    /// </summary>
    /// <seealso cref="EXTJsonBarrierStatistics.EXTJsonOutgoingBarrierStatistics"/>
    public IReadOnlyList<IReadOnlyList<EXTJsonBarrierStatistics.EXTJsonOutgoingBarrierStatistics>>? OutgoingBarrierAllies;
    /// <summary>
    /// Array of Total Outgoing Barrier stats \n
    /// Length == # of phases
    /// </summary>
    /// <seealso cref="EXTJsonBarrierStatistics.EXTJsonOutgoingBarrierStatistics"/>
    public IReadOnlyList<EXTJsonBarrierStatistics.EXTJsonOutgoingBarrierStatistics>? OutgoingBarrier;
    /// <summary>
    /// Array of Total Incoming Barrier stats \n
    /// Length == # of phases
    /// </summary>
    /// <seealso cref="EXTJsonBarrierStatistics.EXTJsonIncomingBarrierStatistics"/>
    public IReadOnlyList<EXTJsonBarrierStatistics.EXTJsonIncomingBarrierStatistics>? IncomingBarrier;

    /// <summary>
    /// Array of int representing 1S outgoing allied barrier points \n
    /// Length == # of players and the length of each sub array is equal to # of phases
    /// </summary>
    /// <remarks>
    /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
    /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
    /// </remarks>
    public IReadOnlyList<IReadOnlyList<IReadOnlyList<int>>>? AlliedBarrier1S;

    /// <summary>
    /// Array of int representing 1S outgoing barrier points \n
    /// Length == # of phases
    /// </summary>
    /// <remarks>
    /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
    /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
    /// </remarks>
    public IReadOnlyList<IReadOnlyList<int>>? Barrier1S;

    /// <summary>
    /// Array of int representing 1S incoming barrier points \n
    /// Length == # of phases
    /// </summary>
    /// <remarks>
    /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
    /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
    /// </remarks>
    public IReadOnlyList<IReadOnlyList<int>>? BarrierReceived1S;

    /// <summary>
    /// Total Outgoing Allied Barrier distribution array \n
    /// Length == # of players and the length of each sub array is equal to # of phases
    /// </summary>
    /// <seealso cref="EXTJsonBarrierDist"/>
    public IReadOnlyList<IReadOnlyList<IReadOnlyList<EXTJsonBarrierDist>>>? AlliedBarrierDist;
    /// <summary>
    /// Total Outgoing Barrier distribution array \n
    /// Length == # of phases
    /// </summary>
    /// <seealso cref="EXTJsonBarrierDist"/>
    public IReadOnlyList<IReadOnlyList<EXTJsonBarrierDist>>? TotalBarrierDist;
    /// <summary>
    /// Total Incoming Barrier distribution array \n
    /// Length == # of phases
    /// </summary>
    /// <seealso cref="EXTJsonBarrierDist"/>
    public IReadOnlyList<IReadOnlyList<EXTJsonBarrierDist>>? TotalIncomingBarrierDist;
}
