using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.HTMLActors
{
    internal class TargetDto : ActorDto
    {
        public long HbWidth { get; set; }
        public long HbHeight { get; set; }
        public double HpLeftPercent { get; set; }
        public int HpLeft { get; set; }

        public TargetDto(AbstractSingleActor target, ParsedEvtcLog log, ActorDetailsDto details) : base(target, log, details)
        {
            HbHeight = target.HitboxHeight;
            HbWidth = target.HitboxWidth;
            HpLeftPercent = 100.0;
            if (log.FightData.Success)
            {
                HpLeftPercent = 0;
            }
            else
            {
                IReadOnlyList<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(target.AgentItem);
                if (hpUpdates.Count > 0)
                {
                    HpLeftPercent = hpUpdates.Last().HPPercent;
                }
            }
            HpLeft = target.GetCurrentHealth(log, HpLeftPercent);
        }
    }
}
