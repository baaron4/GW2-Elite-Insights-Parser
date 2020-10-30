using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels
{
    public class TargetDto : ActorDto
    {
        public string Icon { get; internal set; }
        public long Health { get; internal set; }
        public long HbWidth { get; internal set; }
        public long HbHeight { get; internal set; }
        public double Percent { get; internal set; }
        public double HpLeft { get; internal set; }

        internal TargetDto(NPC target, ParsedEvtcLog log, ActorDetailsDto details) : base(target, log, details)
        {
            Icon = target.GetIcon();
            Health = target.GetHealth(log.CombatData);
            HbHeight = target.HitboxHeight;
            HbWidth = target.HitboxWidth;
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
        }
    }
}
