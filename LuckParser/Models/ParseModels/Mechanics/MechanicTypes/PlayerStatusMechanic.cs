using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    
    public class PlayerStatusMechanic : Mechanic
    {

        public PlayerStatusMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public PlayerStatusMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            ShowOnTable = false;
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicLog>> mechanicLogs, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            HashSet<ushort> playersIds = log.PlayerIDs;
            foreach (Player p in log.PlayerList)
            {
                List<CombatItem> cList = new List<CombatItem>();
                switch (SkillId)
                {
                    case SkillItem.DeathId:
                        cList = combatData.GetStatesData(p.InstID, ParseEnum.StateChange.ChangeDead, log.FightData.FightStart, log.FightData.FightEnd);
                        break;
                    case SkillItem.DCId:
                        cList = combatData.GetStatesData(p.InstID, ParseEnum.StateChange.Despawn, log.FightData.FightStart, log.FightData.FightEnd);
                        break;
                    case SkillItem.RespawnId:
                        cList = combatData.GetStatesData(p.InstID, ParseEnum.StateChange.Spawn, log.FightData.FightStart, log.FightData.FightEnd);
                        break;
                    case SkillItem.AliveId:
                        cList = combatData.GetStatesData(p.InstID, ParseEnum.StateChange.ChangeUp, log.FightData.FightStart, log.FightData.FightEnd);
                        break;
                    case SkillItem.DownId:
                        cList = combatData.GetStatesData(p.InstID, ParseEnum.StateChange.ChangeDown, log.FightData.FightStart, log.FightData.FightEnd);
                        List<CombatItem> downByVaporForm = combatData.GetBoonData(5620).Where(x => x.SrcInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.All).ToList();
                        foreach (CombatItem c in downByVaporForm)
                        {
                            cList.RemoveAll(x => Math.Abs(x.Time - c.Time) < 20);
                        }
                        break;
                    case SkillItem.ResurrectId:
                        cList = log.CombatData.GetCastData(p.InstID, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.SkillID == SkillItem.ResurrectId && x.IsActivation.StartCasting()).ToList();
                        break;
                }
                foreach (CombatItem mechItem in cList)
                {
                    mechanicLogs[this].Add(new MechanicLog(log.FightData.ToFightSpace(mechItem.Time), this, p));
                }
            }
        }
    }
}
