using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{

    public class EnemyBuffRemoveMechanic : BuffRemoveMechanic
    {

        public EnemyBuffRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, BuffRemoveChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public EnemyBuffRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffRemoveChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
            IsEnemyMechanic = true;
        }

        public EnemyBuffRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public EnemyBuffRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            IsEnemyMechanic = true;
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            HashSet<AgentItem> playerAgents = log.PlayerAgents;
            foreach (AbstractBuffEvent c in log.CombatData.GetBoonData(SkillId))
            {
                DummyActor amp = null;
                if (c is BuffRemoveManualEvent rme && Keep(rme, log))
                {
                    Target target = log.FightData.Logic.Targets.Find(x => x.AgentItem == rme.To);
                    if (target != null)
                    {
                        amp = target;
                    }
                    else
                    {
                        AgentItem a = rme.To;
                        if (playerAgents.Contains(a))
                        {
                            continue;
                        }
                        else if (a.Master != null)
                        {
                            AgentItem m = a.Master;
                            if (playerAgents.Contains(m))
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