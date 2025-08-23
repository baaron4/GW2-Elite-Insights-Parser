using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class DamageModifiersUtils
{

    public enum DamageModifierMode { PvE, PvEInstanceOnly, sPvP, WvW, All, sPvPWvW, PvEWvW, PvEsPvP };
    public enum DamageSource { All, NoPets, PetsOnly, Incoming };

    internal delegate bool DamageLogChecker(HealthDamageEvent dl, ParsedEvtcLog log);
    internal delegate bool ActorChecker(SingleActor actor, ParsedEvtcLog log);

    /// <summary>
    /// When the modifier is active based on the presence of a buff. Stacks amount don't count. Additive.
    /// </summary>
    internal static readonly GainComputerByPresence ByPresence = new();
    /// <summary>
    /// When the modifier is active based on the presence of multiple buffs. Additive.
    /// </summary>
    internal static readonly GainComputerByMultiPresence ByMultiPresence = new();
    /// <summary>
    /// When the modifier is increased by each stack of buff. Additive.
    /// </summary>
    internal static readonly GainComputerByStack ByStack = new();
    /// <summary>
    /// When the modifier is increased by each stack of buff. Multiplicative.
    /// </summary>
    internal static readonly GainComputerByMultiplyingStack ByMultipliyingStack = new();
    /// <summary>
    /// When the modifier is active based on a missing buff.
    /// </summary>
    internal static readonly GainComputerByAbsence ByAbsence = new();

    internal static double VulnerabilityAdjuster(HealthDamageEvent dl, ParsedEvtcLog log)
    {
        var target = log.FindActor(dl.To);
        if (target.GetBuffGraphs(log).TryGetValue(Vulnerability, out var bgm))
        {
            return 1.0 / (1.0 + 0.01 * bgm.GetStackCount(dl.Time));
        }
        return 1.0;
    }

    internal static bool VulnerabilityAdditiveChecker(HealthDamageEvent dl, ParsedEvtcLog log, long buffID, double gainPerStack)
    {
        var target = log.FindActor(dl.To);
        var buffSegment = target.GetBuffStatus(log, buffID, dl.Time);
        var vulnSegment = target.GetBuffStatus(log, Vulnerability, dl.Time);
        double gain = buffSegment.Value * gainPerStack - vulnSegment.Value;
        if (gain >= 100)
        {
            return false;
        }
        return true;
    }
}
