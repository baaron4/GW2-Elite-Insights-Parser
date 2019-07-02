using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{

    public class EnemyBoonRemoveMechanic : BoonRemoveMechanic
    {

        public EnemyBoonRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, List<BoonRemoveChecker> conditions) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, conditions)
        {
        }

        public EnemyBoonRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, List<BoonRemoveChecker> conditions) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, conditions)
        {
            IsEnemyMechanic = true;
        }

        public EnemyBoonRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public EnemyBoonRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
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
                        else if (a.MasterAgent != null)
                        {
                            AgentItem m = a.MasterAgent;
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