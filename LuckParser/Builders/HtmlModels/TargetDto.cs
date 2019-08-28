using System;
using System.Collections.Generic;
using System.Linq;
using LuckParser.EIData;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData.CombatEvents;

namespace LuckParser.Builders.HtmlModels
{
    public class TargetDto
    {
        public string Name;
        public string Icon;
        public long Health;
        public int CombatReplayID;
        public long HbWidth;
        public long HbHeight;
        public uint Tough;
        public readonly List<MinionDto> Minions = new List<MinionDto>();
        public double Percent;
        public double HpLeft;
        public ActorDetailsDto Details;

        public TargetDto(Target target, ParsedLog log, bool cr, ActorDetailsDto details)
        {
            Name = target.Character;
            Icon = GeneralHelper.GetNPCIcon(target.ID);
            Health = target.GetHealth(log.CombatData);
            HbHeight = target.HitboxHeight;
            HbWidth = target.HitboxWidth;
            Tough = target.Toughness;
            Details = details;
            if (cr)
            {
                CombatReplayID = target.GetCombatReplayID(log);
            }
            if (log.FightData.Success)
            {
                HpLeft = 0;
            }
            else
            {
                List<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(target.AgentItem);
                if (hpUpdates.Count > 0)
                {
                    HpLeft = hpUpdates.Last().HPPercent;
                }
            }
            Percent = Math.Round(100.0 - HpLeft, 2);
            foreach (KeyValuePair<string, MinionsList> pair in target.GetMinions(log))
            {
                Minions.Add(new MinionDto() { Id = pair.Value.MinionID, Name = pair.Key.TrimEnd(" \0".ToArray()) });
            }
        }
    }
}
