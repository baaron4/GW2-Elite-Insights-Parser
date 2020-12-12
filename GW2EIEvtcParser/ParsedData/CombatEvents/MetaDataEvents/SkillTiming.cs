namespace GW2EIEvtcParser.ParsedData
{
    public class SkillTiming
    {

        public byte ActionAction { get; }

        public ArcDPSEnums.SkillAction Action { get; }

        public ulong AtMillisecond { get; }

        internal SkillTiming(CombatItem evtcItem)
        {
            ActionAction = (byte)evtcItem.SrcAgent;
            Action = ArcDPSEnums.GetSkillAction(ActionAction);
            AtMillisecond = evtcItem.DstAgent;
        }

    }
}
