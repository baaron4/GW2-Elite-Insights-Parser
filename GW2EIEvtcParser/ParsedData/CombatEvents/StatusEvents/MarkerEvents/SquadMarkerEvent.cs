using System.Numerics;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class SquadMarkerEvent : StatusEvent
{
    public Vector3 Position { get; protected set; } = Vector3.Zero;

    internal bool IsEnd => Position.Length() == float.PositiveInfinity;
    public long EndTime { get; protected set; } = int.MaxValue;

    public readonly SquadMarkerIndex MarkerIndex;

    private readonly uint _markerIndex;
    internal bool EndNotSet => EndTime == int.MaxValue;

    //TODO(Rennorb) @cleanup: replace with union
    internal static unsafe Vector3 ReadPosition(CombatItem evtcItem)
    {
        var srcAgent = evtcItem.SrcAgent;
        var dstAgent = evtcItem.DstAgent;
        return new(*(float*)&srcAgent, *((float*)&srcAgent + 1), *(float*)&dstAgent);
    }

    internal SquadMarkerEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        Src = ParserHelper._unknownAgent;
        _markerIndex = evtcItem.SkillID;
        MarkerIndex = GetSquadMarkerIndex((byte)evtcItem.SkillID);
        Position = ReadPosition(evtcItem);
    }

    internal void SetEndTime(long endTime)
    {
        //Debug.Assert(!EndNotSet); TODO(Rennorb): this trips 20240609-201008.zevtc 20240609-203221.zevtc, keep the check below
        if(!EndNotSet)
        {
            return;
        }


        EndTime = endTime;
    }

}
