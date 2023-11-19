using System;
using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractEffectEvent : AbstractStatusEvent
    {

        /// <summary>
        /// The effect's rotation around each axis in <b>radians</b>.
        /// Use <see cref="Rotation"/> for degrees.
        /// </summary>
        public Point3D Orientation { get; protected set; }

        /// <summary>
        /// The effect's rotation around each axis in <b>degrees</b>.
        /// Like <see cref="Orientation"/> but using degrees.
        /// </summary>
        public Point3D Rotation => new Point3D(RadianToDegreeF(Orientation.X), RadianToDegreeF(Orientation.Y), RadianToDegreeF(Orientation.Z));

        /// <summary>
        /// The effect's position in the game's coordinate system, if <see cref="IsAroundDst"/> is <c>false</c>.
        /// </summary>
        public Point3D Position { get; } = new Point3D(0, 0, 0);

        /// <summary>
        /// Whether the effect location is following <see cref="Dst"/> or located at <see cref="Position"/>.
        /// </summary>
        public bool IsAroundDst => Dst != null;
        /// <summary>
        /// The agent the effect is located at, if <see cref="IsAroundDst"/> is <c>true</c>.
        /// </summary>
        public AgentItem Dst { get; } = null;

        /// <summary>
        /// Unique id for tracking a created effect.
        /// </summary>
        protected long TrackingID { get; set; }

        internal AbstractEffectEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
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
