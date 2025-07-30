using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.HTMLActors;

internal class TargetDto : ActorDto
{
    public long HbWidth { get; set; }
    public long HbHeight { get; set; }
    public double HpLeftPercent { get; set; }
    public int HpLeft { get; set; }
    public double BarrierLeftPercent { get; set; }
    public int BarrierLeft { get; set; }

    public TargetDto(SingleActor target, ParsedEvtcLog log, ActorDetailsDto details) : base(target, log, details)
    {
        HbHeight = target.HitboxHeight;
        HbWidth = target.HitboxWidth;
        HpLeftPercent = 100.0;
        if (log.FightData.Success)
        {
            HpLeftPercent = 0;
            BarrierLeftPercent = 0;
        }
        else
        {
            var hpUpdates = target.GetHealthUpdates(log);
            if (hpUpdates.Count > 0)
            {
                HpLeftPercent = hpUpdates.Last().Value;
            }
            var barrierUpdates = target.GetBarrierUpdates(log);
            if (barrierUpdates.Count > 0)
            {
                BarrierLeftPercent = barrierUpdates.Last().Value;
            }
        }
        HpLeft = target.GetCurrentHealth(log, HpLeftPercent);
        BarrierLeft = target.GetCurrentBarrier(log, BarrierLeftPercent, log.FightData.FightEnd);
    }
}
