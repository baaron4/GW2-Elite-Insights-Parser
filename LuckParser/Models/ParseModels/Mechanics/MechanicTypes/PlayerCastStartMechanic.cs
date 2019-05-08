using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    
    public class PlayerCastStartMechanic : Mechanic
    {

        public PlayerCastStartMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, List<MechanicChecker> conditions, TriggerRule rule) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, conditions, rule)
        {
        }

        public PlayerCastStartMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public PlayerCastStartMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public PlayerCastStartMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, List<MechanicChecker> conditions, TriggerRule rule) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, conditions, rule)
        {
        }

        public override void CheckMechanic(ParsedEvtcContainer evtcContainer, Dictionary<Mechanic, List<MechanicLog>> mechanicLogs, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            CombatData combatData = evtcContainer.CombatData;
            HashSet<ushort> playersIds = evtcContainer.PlayerIDs;
            foreach (Player p in evtcContainer.PlayerList)
            {
                foreach (CombatItem c in evtcContainer.CombatData.GetCastDataById(SkillId))
                {
                    if (c.IsActivation.StartCasting() && c.SrcInstid == p.InstID && Keep(c, evtcContainer))
                    {
                        mechanicLogs[this].Add(new MechanicLog(evtcContainer.FightData.ToFightSpace(c.Time), this, p));

                    }
                }
            }
        }
    }
}
