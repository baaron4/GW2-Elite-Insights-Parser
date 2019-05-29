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
                List<long> cList = new List<long>();
                switch (SkillId)
                {
                    case SkillItem.DeathId:
                        cList = combatData.GetStatesData(p.InstID, ParseEnum.StateChange.ChangeDead, log.FightData.FightStart, log.FightData.FightEnd).Select(x => log.FightData.ToFightSpace(x.Time)).ToList();
                        break;
                    case SkillItem.DCId:
                        cList = combatData.GetStatesData(p.InstID, ParseEnum.StateChange.Despawn, log.FightData.FightStart, log.FightData.FightEnd).Select(x => log.FightData.ToFightSpace(x.Time)).ToList();
                        break;
                    case SkillItem.RespawnId:
                        cList = combatData.GetStatesData(p.InstID, ParseEnum.StateChange.Spawn, log.FightData.FightStart, log.FightData.FightEnd).Select(x => log.FightData.ToFightSpace(x.Time)).ToList();
                        break;
                    case SkillItem.AliveId:
                        cList = combatData.GetStatesData(p.InstID, ParseEnum.StateChange.ChangeUp, log.FightData.FightStart, log.FightData.FightEnd).Select(x => log.FightData.ToFightSpace(x.Time)).ToList();
                        break;
                    case SkillItem.DownId:
                        cList = combatData.GetStatesData(p.InstID, ParseEnum.StateChange.ChangeDown, log.FightData.FightStart, log.FightData.FightEnd).Select(x => log.FightData.ToFightSpace(x.Time)).ToList();
                        List<long> downByVaporForm = combatData.GetBoonData(5620).Where(x => x.SrcInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.All).Select(x => log.FightData.ToFightSpace(x.Time)).ToList();
                        foreach (long time in downByVaporForm)
                        {
                            cList.RemoveAll(x => Math.Abs(x - time) < 20);
                        }
                        break;
                    case SkillItem.ResurrectId:
                        cList = log.CombatData.GetCastData(p.AgentItem).Where(x => x.SkillId == SkillItem.ResurrectId).Select(x => x.Time).ToList();
                        break;
                }
                foreach (long time in cList)
                {
                    mechanicLogs[this].Add(new MechanicLog(time, this, p));
                }
            }
        }
    }
}
