namespace GW2EIEvtcParser.ParsedData
{
    public class SkillTiming
    {

        public ulong Action { get; }

        public ulong AtMillisecond { get; }

        public SkillTiming(CombatItem evtcItem)
        {
            Action = evtcItem.SrcAgent;
            AtMillisecond = evtcItem.DstAgent;
        }

    }
}
