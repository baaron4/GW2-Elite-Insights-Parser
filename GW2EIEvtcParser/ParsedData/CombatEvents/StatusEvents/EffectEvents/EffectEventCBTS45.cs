using System;
using System.Linq;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class EffectEventCBTS45 : EffectEvent
    {

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

        internal EffectEventCBTS45(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            Orientation = ReadOrientation(evtcItem);
        }

        protected override long ComputeEndTime(ParsedEvtcLog log, long maxDuration, AgentItem agent = null, long? associatedBuff = null)
        {
            if (associatedBuff != null)
            {
                BuffRemoveAllEvent remove = log.CombatData.GetBuffDataByIDByDst(associatedBuff.Value, agent)
                    .OfType<BuffRemoveAllEvent>()
                    .FirstOrDefault(x => x.Time >= Time);
                if (remove != null)
                {
                    return remove.Time;
                }
            }
            return Time + maxDuration;
        }

        public override (long, long) ComputeDynamicLifespan(ParsedEvtcLog log, long defaultDuration, AgentItem agent = null, long? associatedBuff = null)
        {
            return base.ComputeDynamicLifespan(log, 0, agent, associatedBuff);
        }

    }
}
