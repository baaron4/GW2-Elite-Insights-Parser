﻿using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{

    public class EnemyCastMechanic : CastMechanic
    {

        public EnemyCastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, bool end, List<CastChecker> conditions, TriggerRule rule) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, end, conditions, rule)
        {
        }

        public EnemyCastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, bool end, List<CastChecker> conditions, TriggerRule rule) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, end, conditions, rule)
        {
            IsEnemyMechanic = true;
        }

        public EnemyCastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, bool end) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, end)
        {
        }

        public EnemyCastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, bool end) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, end)
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
                        else if (c.MasterCaster != null)
                        {
                            if (playerAgents.Contains(c.MasterCaster))
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
                    mechanicLogs[this].Add(new MechanicEvent(EndCast ? c.Time + c.ActualDuration : c.Time, this, amp));
                }
            }
        }
    }
}