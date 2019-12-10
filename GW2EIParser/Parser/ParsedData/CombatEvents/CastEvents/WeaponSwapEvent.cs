namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class WeaponSwapEvent : AbstractCastEvent
    {
        // Swaps
        public int SwappedTo { get; protected set; }

        public WeaponSwapEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            SwappedTo = (int)evtcItem.DstAgent;
            Skill = skillData.Get(SkillItem.WeaponSwapId);
            ExpectedDuration = 50;
            Duration = 50;
        }
    }
}
