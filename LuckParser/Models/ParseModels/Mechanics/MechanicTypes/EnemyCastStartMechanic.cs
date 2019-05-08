using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{

    public class EnemyCastStartMechanic : Mechanic
    {

        public EnemyCastStartMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, List<MechanicChecker> conditions, TriggerRule rule) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, conditions, rule)
        {
        }

        public EnemyCastStartMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, List<MechanicChecker> conditions, TriggerRule rule) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, conditions, rule)
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

        public override void CheckMechanic(ParsedEvtcContainer evtcContainer, Dictionary<Mechanic, List<MechanicLog>> mechanicLogs, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            CombatData combatData = evtcContainer.CombatData;
            HashSet<ushort> playersIds = evtcContainer.PlayerIDs;
            foreach (CombatItem c in evtcContainer.CombatData.GetCastDataById(SkillId))
            {
                DummyActor amp = null;
                if (c.IsActivation.StartCasting() && Keep(c, evtcContainer))
                {
                    Target target = evtcContainer.FightData.Logic.Targets.Find(x => x.InstID == c.SrcInstid && x.FirstAware <= c.Time && x.LastAware >= c.Time);
                    if (target != null)
                    {
                        amp = target;
                    }
                    else
                    {
                        AgentItem a = evtcContainer.AgentData.GetAgent(c.SrcAgent, c.Time);
                        if (playersIds.Contains(a.InstID))
                        {
                            continue;
                        }
                        else if (a.MasterAgent != 0)
                        {
                            AgentItem m = evtcContainer.AgentData.GetAgent(a.MasterAgent, c.Time);
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
                    mechanicLogs[this].Add(new MechanicLog(evtcContainer.FightData.ToFightSpace(c.Time), this, amp));
                }
            }
        }
    }
}