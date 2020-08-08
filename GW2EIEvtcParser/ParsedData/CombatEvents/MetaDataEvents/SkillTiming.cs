namespace GW2EIEvtcParser.ParsedData
{
    public class SkillTiming
    {

        public ulong Action { get; }

        public ulong AtMillisecond { get; }

        internal SkillTiming(CombatItem evtcItem)
        {
            Action = evtcItem.SrcAgent;
            AtMillisecond = evtcItem.DstAgent;
        }

    }
}
