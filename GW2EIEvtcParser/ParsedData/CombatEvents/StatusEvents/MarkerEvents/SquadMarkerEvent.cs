using GW2EIEvtcParser.EIData;
using System;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public class SquadMarkerEvent : AbstractStatusEvent
    {
        public Point3D Position { get; protected set; } = new Point3D(0, 0, 0);
        public long EndTime { get; protected set; } = long.MaxValue;

        public SquadMarkerIndex MarkerIndex { get; }

        private readonly uint _markerIndex;
        public bool EndNotSet => EndTime == long.MaxValue;
        internal static Point3D ReadPosition(CombatItem evtcItem)
        {
            var positionBytes = new byte[4 * sizeof(float)];
            int offset = 0;
            // 8 
            foreach (byte bt in BitConverter.GetBytes(evtcItem.SrcAgent))
            {
                positionBytes[offset++] = bt;
            }
            // 8 
            foreach (byte bt in BitConverter.GetBytes(evtcItem.DstAgent))
            {
                positionBytes[offset++] = bt;
            }
            var positionFloat = new float[4];
            Buffer.BlockCopy(positionBytes, 0, positionFloat, 0, positionBytes.Length);

            return new Point3D(positionFloat[0], positionFloat[1], positionFloat[2]);
        }

        internal SquadMarkerEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            Src = ParserHelper._unknownAgent;
            _markerIndex = evtcItem.SkillID;
            MarkerIndex = GetSquadMarkerIndex((byte)evtcItem.SkillID);
            Position = ReadPosition(evtcItem);
        }

    }
}
