namespace GW2EIEvtcParser.ParsedData
{
    public class WeaponSwapEvent : AbstractCastEvent
    {
        // Swaps
        public int SwappedTo { get; protected set; }

        internal WeaponSwapEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            Status = AnimationStatus.Instant;
            SwappedTo = (int)evtcItem.DstAgent;
            Skill = skillData.Get(SkillItem.WeaponSwapId);
            ActualDuration = 0;
            ExpectedDuration = 0;
        }
    }
}
