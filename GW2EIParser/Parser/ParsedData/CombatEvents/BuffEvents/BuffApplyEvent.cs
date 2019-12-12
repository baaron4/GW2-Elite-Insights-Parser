using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BuffApplyEvent : AbstractBuffEvent
    {
        public bool Initial { get; }
        public int AppliedDuration { get; }

        private uint _overstackDuration;
        public uint BuffInstance { get; }
        private readonly bool _addedActive;

        public BuffApplyEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, skillData)
        {
            Initial = evtcItem.IsStateChange == ParseEnum.StateChange.BuffInitial;
            AppliedDuration = evtcItem.Value;
            By = agentData.GetAgent(evtcItem.SrcAgent);
            if (By.Master != null)
            {
                ByMinion = By;
                By = By.Master;
            }
            To = agentData.GetAgent(evtcItem.DstAgent);
            BuffInstance = evtcItem.Pad;
            _addedActive = evtcItem.IsShields > 0;
            _overstackDuration = evtcItem.OverstackValue;
        }

        public BuffApplyEvent(AgentItem by, AgentItem to, long time, int duration, SkillItem buffSkill, uint id, bool addedActive) : base(buffSkill, time)
        {
            AppliedDuration = duration;
            By = by;
            if (By.Master != null)
            {
                ByMinion = By;
                By = By.Master;
            }
            To = to;
            BuffInstance = id;
            _addedActive = addedActive;
        }

        public override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return BuffID != ProfHelper.NoBuff;
        }

        public override void TryFindSrc(ParsedLog log)
        {
        }

        public override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Add(AppliedDuration, By, Time, BuffInstance, _addedActive, _overstackDuration);
        }

        public override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffApplyEvent && !(abe is BuffExtensionEvent))
            {
                return 0;
            }
            return -1;
        }
    }
}
