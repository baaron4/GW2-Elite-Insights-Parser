using System;
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

        internal EffectEventCBTS45(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            Orientation = ReadOrientation(evtcItem);
            if (evtcItem.IsFlanking > 0 || EffectID == 0)
            {
                TrackingID = ReadDuration(evtcItem);
            }
            else
            {
                Duration = ReadDuration(evtcItem);
            }
        }

    }
}
