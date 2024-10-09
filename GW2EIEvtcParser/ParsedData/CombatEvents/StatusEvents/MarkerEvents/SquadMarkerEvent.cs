using System;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public class SquadMarkerEvent : AbstractStatusEvent
    {
        public Point3D Position { get; protected set; } = new Point3D(0, 0, 0);

        internal bool IsEnd => Position.Length() == float.PositiveInfinity;
        public long EndTime { get; protected set; } = int.MaxValue;

        public SquadMarkerIndex MarkerIndex { get; }

        private readonly uint _markerIndex;
        internal bool EndNotSet => EndTime == int.MaxValue;
        internal static unsafe Point3D ReadPosition(CombatItem evtcItem)
        {
            // 8 
            var srcAgent = evtcItem.SrcAgent;
            // 8 
            var dstAgent = evtcItem.DstAgent;
            return new Point3D(*(float*)&srcAgent, *((float*)&srcAgent + 1), *(float*)&dstAgent);
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
            // Sanity check
            if (!EndNotSet)
            {
                return;
            }
            EndTime = endTime;
        }

    }
}
