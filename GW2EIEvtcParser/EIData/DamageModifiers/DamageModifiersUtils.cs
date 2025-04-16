using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class DamageModifiersUtils
{

    public enum DamageModifierMode { PvE, PvEInstanceOnly, sPvP, WvW, All, sPvPWvW, PvEWvW, PvEsPvP };
    public enum DamageSource { All, NoPets, NotApplicable };

    internal delegate bool DamageLogChecker(HealthDamageEvent dl, ParsedEvtcLog log);
    internal delegate bool ActorChecker(SingleActor actor, ParsedEvtcLog log);

    internal static readonly GainComputerByPresence ByPresence = new();
    internal static readonly GainComputerByMultiPresence ByMultiPresence = new();
    internal static readonly GainComputerByStack ByStack = new();
    internal static readonly GainComputerByMultiplyingStack ByMultipliyingStack = new();
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
