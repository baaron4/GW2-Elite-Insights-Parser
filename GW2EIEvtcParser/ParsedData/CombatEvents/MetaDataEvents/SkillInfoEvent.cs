using System;
using System.Collections.Generic;
using System.IO;

namespace GW2EIEvtcParser.ParsedData
{
    public class SkillInfoEvent : AbstractMetaDataEvent
    {
        public uint SkillID { get; }

        public float Recharge { get; protected set; }

        public float Range0 { get; protected set; }

        public float Range1 { get; protected set; }

        public float TooltipTime { get; protected set; }

        public List<SkillTiming> SkillTimings { get; } = new List<SkillTiming>();

        internal SkillInfoEvent(CombatItem evtcItem) : base(evtcItem)
        {
            SkillID = evtcItem.SkillID;
            CompleteSkillInfoEvent(evtcItem);
        }

        internal void CompleteSkillInfoEvent(CombatItem evtcItem)
        {
            if (evtcItem.SkillID != SkillID)
            {
                throw new InvalidOperationException("Non matching buff id in BuffDataEvent complete method");
            }
            switch (evtcItem.IsStateChange)
            {
                case ArcDPSEnums.StateChange.SkillTiming:
                    BuildFromSkillTiming(evtcItem);
                    break;
                case ArcDPSEnums.StateChange.SkillInfo:
                    BuildFromSkillInfo(evtcItem);
                    break;
                default:
                    throw new InvalidDataException("Invalid combat event in BuffDataEvent complete method");
            }
        }

        private void BuildFromSkillInfo(CombatItem evtcItem)
        {
            byte[] skillInfoBytes = new byte[4 * sizeof(float)];
            int offset = 0;
            // 2 
            foreach (byte bt in BitConverter.GetBytes(evtcItem.Time))
            {
                skillInfoBytes[offset++] = bt;
            }
            // 2
            foreach (byte bt in BitConverter.GetBytes(evtcItem.SrcAgent))
            {
                skillInfoBytes[offset++] = bt;
            }
            //
            float[] skillInfoFloats = new float[4];
            Buffer.BlockCopy(skillInfoBytes, 0, skillInfoFloats, 0, skillInfoBytes.Length);
            Recharge = skillInfoFloats[0];
            Range0 = skillInfoFloats[1];
            Range1 = skillInfoFloats[2];
        }

        private void BuildFromSkillTiming(CombatItem evtcItem)
        {
            SkillTimings.Add(new SkillTiming(evtcItem));
        }

    }
}
