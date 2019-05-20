using LuckParser.Models.ParseModels;
using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.HtmlModels
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
        public int HpLeft;
        public ActorDetailsDto Details;

        public TargetDto(Target target, ParsedLog log, bool cr, ActorDetailsDto details)
        {
            Name = target.Character;
            Icon = GeneralHelper.GetNPCIcon(target.ID);
            Health = target.Health;
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
                if (target.HealthOverTime.Count > 0)
                {
                    HpLeft = target.HealthOverTime[target.HealthOverTime.Count - 1].hp;
                }
            }
            Percent = Math.Round(100.0 - HpLeft * 0.01, 2);
            foreach (KeyValuePair<string, Minions> pair in target.GetMinions(log))
            {
                Minions.Add(new MinionDto() { Id = pair.Value.MinionID, Name = pair.Key.TrimEnd(" \0".ToArray()) });
            }
        }
    }
}
