using System;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class EffectEvent : AbstractStatusEvent
    {

        public AgentItem Dst { get; } = null;
        public Point3D Orientation { get; }
        public Point3D Position { get; } = new Point3D(0, 0, 0);

        public bool IsAroundDst => Dst != null;
        public bool IsEnd => EffectID == 0;

        public long EffectID { get; }
        public long TrackingID { get; }

        public ushort Duration { get; }

        private static Point3D ReadOrientation(CombatItem evtcItem)
        {
            byte[] orientationBytes = new byte[2 * sizeof(float)];
            int offset = 0;
            orientationBytes[offset++] = evtcItem.IFFByte;
            orientationBytes[offset++] = evtcItem.IsBuff;
            orientationBytes[offset++] = evtcItem.Result;
            orientationBytes[offset++] = evtcItem.IsActivationByte;
            orientationBytes[offset++] = evtcItem.IsBuffRemoveByte;
            orientationBytes[offset++] = evtcItem.IsNinety;
            orientationBytes[offset++] = evtcItem.IsFifty;
            orientationBytes[offset++] = evtcItem.IsMoving;


            float[] orientationFloats = new float[2];
            Buffer.BlockCopy(orientationBytes, 0, orientationFloats, 0, orientationBytes.Length);

            return new Point3D(orientationFloats[0], orientationFloats[1], -BitConverter.ToSingle(BitConverter.GetBytes(evtcItem.Pad), 0));
        }

        private static ushort ReadDuration(CombatItem evtcItem)
        {
            byte[] durationBytes = new byte[sizeof(ushort)];
            int offset = 0;
            durationBytes[offset++] = evtcItem.IsShields;
            durationBytes[offset++] = evtcItem.IsOffcycle;


            ushort[] durationUShort = new ushort[1];
            Buffer.BlockCopy(durationBytes, 0, durationUShort, 0, durationBytes.Length);
            return durationUShort[0];
        }

        internal EffectEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            EffectID = evtcItem.SkillID;
            Orientation = ReadOrientation(evtcItem);
            if (evtcItem.IsFlanking > 0 || EffectID == 0)
            {
                TrackingID = ReadDuration(evtcItem);
            }
            else
            {
                Duration = ReadDuration(evtcItem);
            }
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
