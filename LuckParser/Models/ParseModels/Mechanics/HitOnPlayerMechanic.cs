using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    
    public class HitOnPlayerMechanic : Mechanic
    {

        public HitOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, CheckTriggerCondition condition = null) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public HitOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CheckTriggerCondition condition = null) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            MechanicData mechData = log.MechanicData;
            CombatData combatData = log.CombatData;
            HashSet<ushort> playersIds = log.PlayerIDs;
            foreach (Player p in log.PlayerList)
            {
                List<CombatItem> combatitems = combatData.GetDamageTakenData(p.InstID, p.FirstAware, p.LastAware);
                foreach (CombatItem c in combatitems)
                {
                    if (!Keep(c))
                    {
                        continue;
                    }
                    if (c.SkillID == SkillId && (c.IsBuff == 0 && c.ResultEnum.IsHit()) || (c.IsBuff > 0 && c.Result == 0))
                    {
                        mechData[this].Add(new MechanicLog(log.FightData.ToFightSpace(c.Time), this, p));

                    }
                }
            }
        }
    }
}
