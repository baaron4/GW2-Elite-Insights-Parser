using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class DamageModifiersUtils
    {

        public enum DamageModifierMode { PvE, PvEInstanceOnly, sPvP, WvW, All, sPvPWvW, PvEWvW, PvEsPvP };
        public enum DamageSource { All, NoPets, NotApplicable };

        internal delegate bool DamageLogChecker(AbstractHealthDamageEvent dl, ParsedEvtcLog log);

        internal static readonly GainComputerByPresence ByPresence = new GainComputerByPresence();
        internal static readonly GainComputerByMultiPresence ByMultiPresence = new GainComputerByMultiPresence();
        internal static readonly GainComputerByStack ByStack = new GainComputerByStack();
        internal static readonly GainComputerByMultiplyingStack ByMultipliyingStack = new GainComputerByMultiplyingStack();
        internal static readonly GainComputerByAbsence ByAbsence = new GainComputerByAbsence();

        internal static double VulnerabilityAdjuster(AbstractHealthDamageEvent dl, ParsedEvtcLog log)
        {
            AbstractSingleActor target = log.FindActor(dl.To);
            IReadOnlyDictionary<long, BuffsGraphModel> bgms = target.GetBuffGraphs(log);
            if (bgms.TryGetValue(Vulnerability, out BuffsGraphModel bgm))
            {
                return 1.0 / (1.0 + 0.01 * bgm.GetStackCount(dl.Time));
            }
            return 1.0;
        }

        internal static bool VulnerabilityAdditiveChecker(AbstractHealthDamageEvent dl, ParsedEvtcLog log, long buffID, double gainPerStack)
        {
            AbstractSingleActor target = log.FindActor(dl.To);
            Segment buffSegment = target.GetBuffStatus(log, buffID, dl.Time);
            Segment vulnSegment = target.GetBuffStatus(log, Vulnerability, dl.Time);
            double gain = buffSegment.Value * gainPerStack - vulnSegment.Value;
            if (gain >= 100)
            {
                return false;
            }
            return true;
        }
    }
}
