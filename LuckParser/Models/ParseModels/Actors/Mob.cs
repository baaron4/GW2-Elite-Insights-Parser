using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Mob : AbstractMasterActor
    {
        // Constructors
        public Mob(AgentItem agent) : base(agent)
        {        
        }

        //setters

        protected override void InitAdditionalCombatReplayData(ParsedLog log)
        {
            log.FightData.Logic.ComputeMobCombatReplayActors(this, log, CombatReplay);
        }
        //
        private class MobSerializable : AbstractMasterActorSerializable
        {
            public long Start { get; set; }
            public long End { get; set; }
        }

        public override AbstractMasterActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            MobSerializable aux = new MobSerializable
            {
                Img = CombatReplay.Icon,
                Type = "Mob",
                Positions = new double[2 * CombatReplay.Positions.Count],
                Start = CombatReplay.TimeOffsets.start,
                End = CombatReplay.TimeOffsets.end,
                ID = GetCombatReplayID(log)
            };
            int i = 0;
            foreach (Point3D pos in CombatReplay.Positions)
            {
                (double x, double y) = map.GetMapCoord(pos.X, pos.Y);
                aux.Positions[i++] = x;
                aux.Positions[i++] = y;
            }

            return aux;
        }

        protected override void InitCombatReplay(ParsedLog log)
        {
            if (!log.CanCombatReplay)
            {
                // no combat replay support on fight
                return;
            }
            CombatReplay = new CombatReplay
            {
                Icon = GeneralHelper.GetNPCIcon(ID)
            };
            SetMovements(log);
            CombatReplay.PollingRate(log.FightData.FightDuration, false);
            CombatItem despawnCheck = log.CombatData.GetStatesData(InstID, ParseEnum.StateChange.Despawn, FirstAware, LastAware).LastOrDefault();
            CombatItem spawnCheck = log.CombatData.GetStatesData(InstID, ParseEnum.StateChange.Spawn, FirstAware, LastAware).LastOrDefault();
            CombatItem deathCheck = log.CombatData.GetStatesData(InstID, ParseEnum.StateChange.ChangeDead, FirstAware, LastAware).LastOrDefault();
            if (deathCheck != null)
            {
                CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAware), log.FightData.ToFightSpace(deathCheck.Time));
            }
            else if (despawnCheck != null && (spawnCheck == null || spawnCheck.Time < despawnCheck.Time))
            {
                CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAware), log.FightData.ToFightSpace(despawnCheck.Time));
            }
            else
            {
                CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAware), log.FightData.ToFightSpace(AgentItem.LastAware));
            }
        }
    }
}
