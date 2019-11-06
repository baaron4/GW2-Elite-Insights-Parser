using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BuffRemoveSingleEvent : AbstractBuffRemoveEvent
    {
        private readonly ParseEnum.IFF _iff;
        public uint BuffInstance { get; protected set; }
        public BuffRemoveSingleEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, agentData, skillData, offset)
        {
            _iff = evtcItem.IFF;
            BuffInstance = evtcItem.Pad;
        }

        public BuffRemoveSingleEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill, uint id, ParseEnum.IFF iff) : base(by, to, time, removedDuration, buffSkill)
        {
            _iff = iff;
            BuffInstance = id;
        }

        public override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return BuffID != ProfHelper.NoBuff &&
                    (hasStackIDs ||
                        (!(_iff == ParseEnum.IFF.Unknown && By == GeneralHelper.UnknownAgent && !hasStackIDs) && // overstack or natural end removals
                        !(RemovedDuration <= 50 && RemovedDuration != 0 && !hasStackIDs) &&// low value single stack remove that can mess up with the simulator if server delay
                        Time <= fightEnd - 50)); // don't take into account removal that are close to the end of the fight));

        }

        public override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Remove(By, RemovedDuration, 1, Time, ParseEnum.BuffRemove.Single, BuffInstance);
        }
        public override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffRemoveSingleEvent)
            {
                return 0;
            }
            if (abe is BuffRemoveAllEvent || abe is AbstractBuffStackEvent)
            {
                return -1;
            }
            return 1;
        }
    }
}
