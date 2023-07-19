using System;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class EffectEvent : AbstractStatusEvent
    {

        public AgentItem Dst { get; } = null;
        public Point3D Orientation { get; protected set; }
        public Point3D Position { get; } = new Point3D(0, 0, 0);

        public bool IsAroundDst => Dst != null;
        public bool IsEnd => EffectID == 0;

        public long EffectID { get; }
        public long TrackingID { get; protected set; }

        public long Duration { get; protected set; }

        internal EffectEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            EffectID = evtcItem.SkillID;
            if (evtcItem.DstAgent != 0)
            {
                Dst = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
            } 
            else
            {
                Position = new Point3D(
                    BitConverter.ToSingle(BitConverter.GetBytes(evtcItem.Value), 0),
                    BitConverter.ToSingle(BitConverter.GetBytes(evtcItem.BuffDmg), 0),
                    BitConverter.ToSingle(BitConverter.GetBytes(evtcItem.OverstackValue), 0)
               );
            }
        }

    }
}
