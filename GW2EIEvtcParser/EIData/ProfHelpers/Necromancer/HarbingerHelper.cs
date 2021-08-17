using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class HarbingerHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(-1, "Wicked Corruption", "1% per blight stack", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Harbinger, ByPresence, "", ulong.MaxValue, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(-1, "Septic Corruption", "1% per blight stack", DamageSource.NoPets, 1.0, DamageType.Condition, DamageType.All, Source.Harbinger, ByPresence, "", ulong.MaxValue, ulong.MaxValue, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Blight",-1, Source.Harbinger, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "", ulong.MaxValue, ulong.MaxValue),
        };
    }
}
