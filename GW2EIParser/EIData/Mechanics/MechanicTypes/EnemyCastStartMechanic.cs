using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{

    public class EnemyCastStartMechanic : CastMechanic
    {

        public EnemyCastStartMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, CastChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public EnemyCastStartMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CastChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
            IsEnemyMechanic = true;
        }

        public EnemyCastStartMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public EnemyCastStartMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            IsEnemyMechanic = true;
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            HashSet<AgentItem> playerAgents = log.PlayerAgents;
            foreach (AbstractCastEvent c in log.CombatData.GetCastDataById(SkillId))
            {
                DummyActor amp = null;
                if (Keep(c, log))
                {
                    Target target = log.FightData.Logic.Targets.Find(x => x.AgentItem == c.Caster);
                    if (target != null)
                    {
                        amp = target;
                    }
                    else
                    {
                        AgentItem a = c.Caster;
                        if (playerAgents.Contains(a))
                        {
                            continue;
                        }
                        else if (c.Caster.Master != null)
                        {
                            if (playerAgents.Contains(c.Caster.Master))
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
                    mechanicLogs[this].Add(new MechanicEvent(GetTime(c), this, amp));
                }
            }
        }
    }
}
