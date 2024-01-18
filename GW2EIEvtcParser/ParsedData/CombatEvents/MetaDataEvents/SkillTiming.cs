namespace GW2EIEvtcParser.ParsedData
{
    public class SkillTiming
    {

        public byte ActionByte { get; }

        public ArcDPSEnums.SkillAction Action { get; }

        public ulong AtMillisecond { get; }

        internal SkillTiming(CombatItem evtcItem)
        {
            ActionByte = (byte)evtcItem.SrcAgent;
            Action = ArcDPSEnums.GetSkillAction(ActionByte);
            AtMillisecond = evtcItem.DstAgent;
        }

    }
}
