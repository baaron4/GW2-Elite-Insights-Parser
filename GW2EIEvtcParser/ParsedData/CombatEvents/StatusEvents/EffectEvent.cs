using System;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class EffectEvent : AbstractStatusEvent
    {

        public AgentItem Dst { get; }

        public bool IsAroundDst => Dst != null;

        public Point3D Position { get; }

        public Point3D Orientation { get; }

        public long EffectID { get; }

        public ushort Duration { get; }

        internal EffectEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
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

                Orientation = new Point3D(orientationFloats[0], orientationFloats[1], BitConverter.ToSingle(BitConverter.GetBytes(evtcItem.Pad), 0));
            }
            EffectID = evtcItem.SkillID;
            {
                byte[] durationBytes = new byte[sizeof(ushort)];
                int offset = 0;
                durationBytes[offset++] = evtcItem.IsShields;
                durationBytes[offset++] = evtcItem.IsOffcycle;


                ushort[] durationUShort = new ushort[1];
                Buffer.BlockCopy(durationBytes, 0, durationUShort, 0, durationBytes.Length);

                Duration = durationUShort[0];
            }
        }

    }
}
