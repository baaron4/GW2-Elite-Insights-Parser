using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    
    public class FirstHitSkillOnPlayerMechanic : Mechanic
    {

        public FirstHitSkillOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, List<MechanicChecker> conditions, TriggerRule rule) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, conditions, rule)
        {
        }

        public FirstHitSkillOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, List<MechanicChecker> conditions, TriggerRule rule) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, conditions, rule)
        {
        }

        public FirstHitSkillOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public FirstHitSkillOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicLog>> mechanicLogs, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            AgentData agentData = log.AgentData;
            foreach (Player p in log.PlayerList)
            {
                List<CombatItem> combatitems = combatData.GetDamageTakenData(p.InstID, p.FirstAware, p.LastAware);
                foreach (CombatItem c in combatitems)
                {
                    if ( !(c.SkillID == SkillId) || (c.IsBuff == 0 && !c.ResultEnum.IsHit()) || (c.IsBuff > 0 && c.Result > 0) || !Keep(c, log))
                    {
                        continue;
                    }
                    List<CombatItem> hits = log.CombatData.AllCombatItems.Where(x =>
                    x.IsStateChange == ParseEnum.StateChange.Normal && x.IsActivation == ParseEnum.Activation.None && x.IsBuffRemove == ParseEnum.BuffRemove.None
                    && ((x.IsBuff != 0 && x.Value == 0) || (x.IsBuff == 0))
                    && x.SrcInstid == c.SrcInstid
                    && agentData.GetAgent(x.DstAgent, x.Time).Type == AgentItem.AgentType.Player
                    ).ToList();

                    if (hits.First() != c) continue;

                    mechanicLogs[this].Add(new MechanicLog(log.FightData.ToFightSpace(c.Time), this, p));
                }
            }
        }
    }
}
