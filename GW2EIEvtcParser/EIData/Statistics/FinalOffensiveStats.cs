using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EIData;

public class FinalOffensiveStats
{
    public readonly int TotalDamageCount;
    public readonly int TotalDmg;
    public readonly int DirectDamageCount;
    public readonly int DirectDmg;
    public readonly int ConnectedDamageCount;
    public readonly int ConnectedDmg;
    public readonly int ConnectedDirectDamageCount;
    public readonly int ConnectedDirectDmg;
    public readonly int CritableDirectDamageCount;
    public readonly int CriticalCount;
    public readonly int CriticalDmg;
    public readonly int FlankingCount;
    public readonly int GlanceCount;
    public readonly int AgainstMovingCount;
    public readonly int Missed;
    public readonly int Blocked;
    public readonly int Evaded;
    public readonly int Interrupts;
    public readonly int Invulned;
    public readonly int Killed;
    public readonly int Downed;

    public readonly int AgainstDownedCount;
    public readonly int AgainstDownedDamage;

    public readonly int ConnectedPowerCount;
    public readonly int ConnectedPowerAbove90HPCount;
    public readonly int ConnectedConditionCount;
    public readonly int ConnectedConditionAbove90HPCount;

    public readonly int DownContribution;

    public readonly int AppliedCrowdControl;
    public readonly double AppliedCrowdControlDuration;
    public readonly int AppliedCrowdControlDownContribution;
    public readonly double AppliedCrowdControlDurationDownContribution;


    internal FinalOffensiveStats(ParsedEvtcLog log, long start, long end, SingleActor actor, SingleActor? target)
    {
        var dls = actor.GetDamageEvents(target, log, start, end);
        foreach (HealthDamageEvent dl in dls)
        {
            if (dl.From == actor.AgentItem)
            {
                if (!(dl is NonDirectHealthDamageEvent))
                {
                    if (dl.HasHit)
                    {
                        if (SkillItem.CanCrit(dl.SkillId, log.LogData.GW2Build))
                        {
                            if (dl.HasCrit)
                            {
                                CriticalCount++;
                                CriticalDmg += dl.HealthDamage;
                            }
                            CritableDirectDamageCount++;
                        }
                        if (dl.IsFlanking)
                        {
                            FlankingCount++;
                        }

                        if (dl.HasGlanced)
                        {
                            GlanceCount++;
                        }
                        ConnectedDirectDamageCount++;
                        ConnectedDirectDmg += dl.HealthDamage;
                    }

                    if (dl.IsBlind)
                    {
                        Missed++;
                    }
                    if (dl.IsEvaded)
                    {
                        Evaded++;
                    }
                    if (dl.IsBlocked)
                    {
                        Blocked++;
                    }
                    if (!dl.DoubleProcHit)
                    {
                        DirectDamageCount++;
                        DirectDmg += dl.HealthDamage;
                    }
                }
                if (dl.IsAbsorbed)
                {
                    Invulned++;
                }
                if (!dl.DoubleProcHit)
                {
                    TotalDamageCount++;
                    TotalDmg += dl.HealthDamage;
                }

                if (dl.HasHit)
                {
                    ConnectedDamageCount++;
                    ConnectedDmg += dl.HealthDamage;
                    // Derive down contribution from health updates as they are available after this build
                    if (log.LogData.EvtcBuild < ArcDPSBuilds.Last90BeforeDownRetired)
                    {
                        IReadOnlyList<Last90BeforeDownEvent> last90BeforeDownEvents = log.CombatData.GetLast90BeforeDownEvents(dl.To);
                        if (last90BeforeDownEvents.Any(x => dl.Time <= x.Time && dl.Time >= x.Time - x.TimeSinceLast90))
                        {
                            DownContribution += dl.HealthDamage;
                        }
                    }
                    else
                    {
                        if (dl.To.IsDownedBeforeNext90(log, dl.Time))
                        {
                            DownContribution += dl.HealthDamage;
                        }
                    }
                    if (dl.AgainstMoving)
                    {
                        AgainstMovingCount++;
                    }
                    if (dl.ConditionDamageBased(log))
                    {
                        ConnectedConditionCount++;
                        if (dl.IsOverNinety)
                        {
                            ConnectedConditionAbove90HPCount++;
                        }
                    }
                    else
                    {
                        ConnectedPowerCount++;
                        if (dl.IsOverNinety)
                        {
                            ConnectedPowerAbove90HPCount++;
                        }
                    }
                    if (dl.AgainstDowned)
                    {
                        AgainstDownedCount++;
                        AgainstDownedDamage += dl.HealthDamage;
                    }
                }
            }

            if (!(dl is NonDirectHealthDamageEvent))
            {
                if (dl.HasInterrupted)
                {
                    Interrupts++;
                }
            }
            if (dl.HasKilled)
            {
                Killed++;
            }
            if (dl.HasDowned)
            {
                Downed++;
            }
        }
        var ccs = actor.GetOutgoingCrowdControlEvents(target, log, start, end);
        foreach (CrowdControlEvent cc in ccs)
        {
            AppliedCrowdControl++;
            AppliedCrowdControlDuration += cc.Duration;
            if (cc.To.IsDownedBeforeNext90(log, cc.Time))
            {
                AppliedCrowdControlDownContribution++;
                AppliedCrowdControlDurationDownContribution += cc.Duration;
            }
        }
    }
}
