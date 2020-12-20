using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels
{
    internal class TargetDto : ActorDto
    {
        public string Icon { get; set; }
        public long Health { get; set; }
        public long HbWidth { get; set; }
        public long HbHeight { get; set; }
        public double Percent { get; set; }
        public double HpLeft { get; set; }

        public TargetDto(NPC target, ParsedEvtcLog log, ActorDetailsDto details) : base(target, log, details)
        {
            Icon = target.GetIcon();
            Health = target.GetHealth(log.CombatData);
            HbHeight = target.HitboxHeight;
            HbWidth = target.HitboxWidth;
            HpLeft = 100.0;
            if (log.FightData.Success)
            {
                HpLeft = 0;
            }
            else
            {
                IReadOnlyList<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(target.AgentItem);
                if (hpUpdates.Count > 0)
                {
                    HpLeft = hpUpdates.Last().HPPercent;
                }
            }
            Percent = Math.Round(100.0 - HpLeft, 2);
        }
    }
}
