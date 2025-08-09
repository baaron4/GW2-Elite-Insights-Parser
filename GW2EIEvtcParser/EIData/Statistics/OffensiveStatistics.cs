using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EIData;

public class OffensiveStatistics
{
    public readonly int TotalDamageEventCount;
    public readonly int TotalDamageEventDamage;

    public readonly int DirectDamageEventCount;
    public readonly int DirectDamageEventDamage;

    public readonly int DamageCount;
    public readonly int Damage;

    public readonly int DirectDamageCount;
    public readonly int DirectDamage;

    public readonly int PowerDamageCount;
    public readonly int PowerDamage;
    public readonly int PowerDamageAbove90HPCount;
    public readonly int PowerDamageAbove90HP;

    public readonly int LifeLeechDamageCount;
    public readonly int LifeLeechDamage;

    public readonly int ConditionDamageCount;
    public readonly int ConditionDamage;
    public readonly int ConditionDamageAbove90HPCount;
    public readonly int ConditionDamageAbove90HP;

    public readonly int CritableDirectDamageCount;
    public readonly int CriticalDamageCount;
    public readonly int CriticalDamage;

    public readonly int FlankingCount;
    public readonly int GlancingCount;
    public readonly int AgainstMovingCount;
    public readonly int MissedCount;
    public readonly int BlockedCount;
    public readonly int EvadedCount;
    public readonly int InterruptCount;
    public readonly int InvulnedCount;

    public readonly int KilledCount;
    public readonly int DownedCount;

    public readonly int AgainstDownedDamageCount;
    public readonly int AgainstDownedDamage;

    public readonly int DownContribution;
    public readonly int AppliedCrowdControlDownContribution;
    public readonly double AppliedCrowdControlDurationDownContribution;

    public readonly int AppliedCrowdControl;
    public readonly double AppliedCrowdControlDuration;


    internal OffensiveStatistics(ParsedEvtcLog log, long start, long end, SingleActor actor, SingleActor? target)
    {
        var dls = actor.GetDamageEvents(target, log, start, end);
        foreach (HealthDamageEvent dl in dls)
        {
            if (dl.From.Is(actor.AgentItem))
            {
                if (!dl.IsNotADamageEvent)
                {
                    TotalDamageEventCount++;
                    TotalDamageEventDamage += dl.HealthDamage;
                    if (dl is DirectHealthDamageEvent)
                    {
                        DirectDamageEventCount++;
                        DirectDamageEventDamage += dl.HealthDamage;
                    }
                }

                if (dl.HasHit)
                {
                    DamageCount++;
                    Damage += dl.HealthDamage;
                    // Derive down contribution from health updates as they are available after this build
                    if (log.LogMetadata.EvtcBuild < ArcDPSBuilds.Last90BeforeDownRetired)
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
                    if (dl.ConditionDamageBased(log))
                    {
                        ConditionDamageCount++;
                        ConditionDamage += dl.HealthDamage;
                        if (dl.IsOverNinety)
                        {
                            ConditionDamageAbove90HPCount++;
                            ConditionDamageAbove90HP += dl.HealthDamage;
                        }
                    }
                    else
                    {
                        if (dl is NonDirectHealthDamageEvent ndhd)
                        {
                            if (ndhd.IsLifeLeech)
                            {
                                LifeLeechDamageCount++;
                                LifeLeechDamage += dl.HealthDamage;
                            }
                        } 
                        else
                        {
                            if (SkillItem.CanCrit(dl.SkillID, log.LogMetadata.GW2Build))
                            {
                                if (dl.HasCrit)
                                {
                                    CriticalDamageCount++;
                                    CriticalDamage += dl.HealthDamage;
                                }
                                CritableDirectDamageCount++;
                            }
                            DirectDamageCount++;
                            DirectDamage += dl.HealthDamage;
                            if (dl.IsFlanking)
                            {
                                FlankingCount++;
                            }
                            if (dl.HasGlanced)
                            {
                                GlancingCount++;
                            }
                        }
                        PowerDamageCount++;
                        PowerDamage += dl.HealthDamage;
                        if (dl.IsOverNinety)
                        {
                            PowerDamageAbove90HPCount++;
                            PowerDamageAbove90HP += dl.HealthDamage;
                        }
                    }
                    if (dl.AgainstDowned)
                    {
                        AgainstDownedDamageCount++;
                        AgainstDownedDamage += dl.HealthDamage;
                    }
                    if (dl.AgainstMoving)
                    {
                        AgainstMovingCount++;
                    }
                }
                if (dl.IsAbsorbed)
                {
                    InvulnedCount++;
                }
                if (dl.IsBlind)
                {
                    MissedCount++;
                }
                if (dl.IsEvaded)
                {
                    EvadedCount++;
                }
                if (dl.IsBlocked)
                {
                    BlockedCount++;
                }
            }
            if (dl.HasInterrupted)
            {
                InterruptCount++;
            }
            if (dl.HasKilled)
            {
                KilledCount++;
            }
            if (dl.HasDowned)
            {
                DownedCount++;
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
