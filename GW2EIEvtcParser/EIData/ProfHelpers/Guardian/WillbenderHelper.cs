using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class WillbenderHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(-1, "Lethal Tempo", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Reaper, ByPresence, "", ulong.MaxValue, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(-1, "Tyrant's Lethal Tempo", "5% per stack", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Reaper, ByPresence, "", ulong.MaxValue, ulong.MaxValue, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                //virtues
                new Buff("Rushing Justice", -1, Source.Willbender, BuffNature.GraphOnlyBuff, "", ulong.MaxValue, ulong.MaxValue),
                new Buff("Flowing Resolve", -1, Source.Willbender, BuffNature.GraphOnlyBuff, "", ulong.MaxValue, ulong.MaxValue),
                new Buff("Crashing Courage", -1, Source.Willbender, BuffNature.GraphOnlyBuff, "", ulong.MaxValue, ulong.MaxValue),
                new Buff("Rushing Justice (Active)", -1, Source.Willbender, BuffNature.GraphOnlyBuff, "", ulong.MaxValue, ulong.MaxValue),
                new Buff("Flowing Resolve (Active)", -1, Source.Willbender, BuffNature.GraphOnlyBuff, "", ulong.MaxValue, ulong.MaxValue),
                new Buff("Crashing Courage (Active)", -1, Source.Willbender, BuffNature.GraphOnlyBuff, "", ulong.MaxValue, ulong.MaxValue),
                //
                new Buff("Repose", -1, Source.Willbender, BuffNature.GraphOnlyBuff, "", ulong.MaxValue, ulong.MaxValue),
                new Buff("Lethal Tempo", -1, Source.Willbender, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "", ulong.MaxValue, ulong.MaxValue),
                new Buff("Tyrant's Lethal Tempo", -1, Source.Willbender, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "", ulong.MaxValue, ulong.MaxValue),
        };
    }
}
