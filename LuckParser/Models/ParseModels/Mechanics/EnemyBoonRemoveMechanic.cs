using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{

    public class EnemyBoonRemoveMechanic : Mechanic
    {

        public EnemyBoonRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, CheckTriggerCondition condition = null) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public EnemyBoonRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CheckTriggerCondition condition = null) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
            IsEnemyMechanic = true;
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            MechanicData mechData = log.MechanicData;
            CombatData combatData = log.CombatData;
            HashSet<ushort> playersIds = log.PlayerIDs;
            foreach (CombatItem c in log.CombatData.GetBoonData(SkillId))
            {
                if (!Keep(c))
                {
                    continue;
                }
                DummyActor amp = null;
                if (c.IsBuffRemove == ParseEnum.BuffRemove.Manual)
                {
                    Target target = log.FightData.Logic.Targets.Find(x => x.InstID == c.SrcInstid && x.FirstAware <= c.Time && x.LastAware >= c.Time);
                    if (target != null)
                    {
                        amp = target;
                    }
                    else
                    {
                        AgentItem a = log.AgentData.GetAgent(c.SrcAgent, c.Time);
                        if (playersIds.Contains(a.InstID))
                        {
                            continue;
                        }
                        else if (a.MasterAgent != 0)
                        {
                            AgentItem m = log.AgentData.GetAgent(a.MasterAgent, c.Time);
                            if (playersIds.Contains(m.InstID))
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
                    mechData[this].Add(new MechanicLog(log.FightData.ToFightSpace(c.Time), this, amp));
                }
            }
        }
    }
}