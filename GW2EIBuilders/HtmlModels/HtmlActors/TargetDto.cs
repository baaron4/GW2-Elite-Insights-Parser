using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLActors;

internal class TargetDto : ActorDto
{
    public long HbWidth { get; set; }
    public long HbHeight { get; set; }
    public double HpLeftPercent { get; set; }
    public int HpLeft { get; set; }
    public double BarrierLeftPercent { get; set; }
    public int BarrierLeft { get; set; }

    public List<double[]> HpBars { get; set; }

    private const int HPBarConsumed = 0;
    private const int HPBarActive = 1;
    private const int HPBarUntouched = 2;

    public TargetDto(SingleActor target, ParsedEvtcLog log, ActorDetailsDto details) : base(target, log, details)
    {
        HbHeight = target.HitboxHeight;
        HbWidth = target.HitboxWidth;
        HpLeftPercent = 100.0;
        var targetEncounterPhase = log.LogData.GetEncounterPhases(log).FirstOrDefault(x => x.Targets.ContainsKey(target));
        if (targetEncounterPhase != null && targetEncounterPhase.Success)
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
        BarrierLeft = target.GetCurrentBarrier(log, BarrierLeftPercent, log.LogData.LogEnd);
        var healthBars = target.GetHealthBars();
        if (healthBars != null)
        {
            Health = 0;
            HpBars = new(healthBars.Count);
            bool activeFound = false;
            foreach (var (maxPercent, minPercent, hpValue, active) in healthBars)
            {
                Health += (long)(hpValue * (maxPercent - minPercent) / 100);
                var behaviorValue = HPBarConsumed;
                if (active)
                {
                    activeFound = true;
                    behaviorValue = HPBarActive;
                    HpLeft += (int)(hpValue * Math.Max(HpLeftPercent - minPercent, 0.0) / 100);
                } 
                else if (activeFound)
                {
                    behaviorValue = HPBarUntouched;
                    HpLeft += (int)(hpValue * (maxPercent - minPercent) / 100);
                }
                HpBars.Add([minPercent, maxPercent, hpValue, behaviorValue]);
            }
        } 
        else
        {
            HpLeft = target.GetCurrentHealth(log, HpLeftPercent);
        }
    }
}
