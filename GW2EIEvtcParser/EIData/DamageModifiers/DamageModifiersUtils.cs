using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal static class DamageModifiersUtils
    {

        public enum DamageModifierMode { PvE, PvEInstanceOnly, sPvP, WvW, All, sPvPWvW, PvEWvW };
        public enum DamageSource { All, NoPets };

        internal delegate bool DamageLogChecker(AbstractHealthDamageEvent dl, ParsedEvtcLog log);

        internal static readonly GainComputerByPresence ByPresence = new GainComputerByPresence();
        internal static readonly GainComputerByMultiPresence ByMultiPresence = new GainComputerByMultiPresence();
        internal static readonly GainComputerByStack ByStack = new GainComputerByStack();
        internal static readonly GainComputerByMultiplyingStack ByMultipliyingStack = new GainComputerByMultiplyingStack();
        internal static readonly GainComputerByAbsence ByAbsence = new GainComputerByAbsence();
    }
}
