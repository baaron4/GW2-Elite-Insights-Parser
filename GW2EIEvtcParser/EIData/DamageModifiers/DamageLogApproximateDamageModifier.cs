using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public class DamageLogApproximateDamageModifier : DamageLogDamageModifier
    {

        internal DamageLogApproximateDamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, string icon, DamageLogChecker checker, GainComputer gainComputer, ulong minBuild, ulong maxBuild, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, checker, gainComputer, minBuild, maxBuild, mode)
        {
            Approximate = true;
            Tooltip += "<br>Approximate";
        }

        internal DamageLogApproximateDamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, string icon, DamageLogChecker checker, GainComputer gainComputer, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, checker, gainComputer, ulong.MinValue, ulong.MaxValue, mode)
        {
            Approximate = true;
            Tooltip += "<br>Approximate";
        }
    }
}
