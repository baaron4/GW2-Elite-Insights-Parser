using LuckParser.Parser;
using LuckParser.Parser.ParsedData;
using LuckParser.Parser.ParsedData.CombatEvents;
using System.Collections.Generic;

namespace LuckParser.EIData
{

    public class SkillByEnemyMechanic : SkillMechanic
    {

        public SkillByEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, SkillChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public SkillByEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public SkillByEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public SkillByEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            HashSet<AgentItem> playerAgents = log.PlayerAgents;
            foreach (AbstractDamageEvent c in log.CombatData.GetDamageDataById(SkillId))
            {
                DummyActor amp = null;
                if (Keep(c, log))
                {
                    Target target = log.FightData.Logic.Targets.Find(x => x.AgentItem == c.From);
                    if (target != null)
                    {
                        amp = target;
                    }
                    else
                    {
                        AgentItem a = c.From;
                        if (playerAgents.Contains(a))
                        {
                            continue;
                        }
                        else if (c.MasterFrom != null)
                        {
                            if (playerAgents.Contains(c.MasterFrom))
                            {
                                continue;
                            }
                        }
                        if (!regroupedMobs.TryGetValue(a.ID, out amp))
                        {
                            amp = new DummyActor(a);
                            regroupedMobs.Add(a.ID, amp);
                        }
                    }
                }
                if (amp != null)
                {
                    mechanicLogs[this].Add(new MechanicEvent(c.Time, this, amp));
                }
            }
        }
    }
}
