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
            new BuffDamageModifier(62509, "Lethal Tempo", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/1/10/Lethal_Tempo.png", 118697, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(62509, "Tyrant's Lethal Tempo", "5% per stack", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/c/c4/Tyrant%27s_Momentum.png", 118697, ulong.MaxValue, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                //virtues
                new Buff("Rushing Justice", 62529, Source.Willbender, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/74/Rushing_Justice.png", 118697, ulong.MaxValue),
                new Buff("Flowing Resolve", 62632, Source.Willbender, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/be/Flowing_Resolve.png", 118697, ulong.MaxValue),
                new Buff("Crashing Courage", 62615, Source.Willbender, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/95/Crashing_Courage.png", 118697, ulong.MaxValue),
                //
                new Buff("Repose", 62638, Source.Willbender, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/80/Repose.png", 118697, ulong.MaxValue),
                new Buff("Lethal Tempo", 62509, Source.Willbender, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/10/Lethal_Tempo.png", 118697, ulong.MaxValue),
                //new Buff("Tyrant's Lethal Tempo", 62657, Source.Willbender, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c4/Tyrant%27s_Momentum.png", 118697, ulong.MaxValue),
        };
    }
}
