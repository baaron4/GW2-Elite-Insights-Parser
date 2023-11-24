using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class DamageModifiersUtils
    {

        public enum DamageModifierMode { PvE, PvEInstanceOnly, sPvP, WvW, All, sPvPWvW, PvEWvW };

        internal delegate bool DamageLogChecker(AbstractHealthDamageEvent dl, ParsedEvtcLog log);

        internal static readonly GainComputerByPresence ByPresence = new GainComputerByPresence();
        internal static readonly GainComputerByMultiPresence ByMultiPresence = new GainComputerByMultiPresence();
        internal static readonly GainComputerByStack ByStack = new GainComputerByStack();
        internal static readonly GainComputerByMultiplyingStack ByMultipliyingStack = new GainComputerByMultiplyingStack();
        internal static readonly GainComputerByAbsence ByAbsence = new GainComputerByAbsence();
    }
}
