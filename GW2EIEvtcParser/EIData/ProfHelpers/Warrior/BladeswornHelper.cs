using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class BladeswornHelper
    {
        /////////////////////
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffLossCastFinder(62861, 62769, EIData.InstantCastFinder.DefaultICD, GW2Builds.EODBeta2, ulong.MaxValue), // Gunsaber sheath
            new BuffGainCastFinder(62745, 62769, EIData.InstantCastFinder.DefaultICD, GW2Builds.EODBeta2, ulong.MaxValue), // Gunsaber         
            new DamageCastFinder(62847, 62847, EIData.InstantCastFinder.DefaultICD, GW2Builds.EODBeta2, ulong.MaxValue), // Unseen Sword
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            //new BuffDamageModifier(62811, "Fierce as Fire", "?%", DamageSource.NoPets, 0.0, DamageType.Strike, DamageType.All, Source.Bladesworn, ByStack, "https://wiki.guildwars2.com/images/8/8e/Fierce_as_Fire.png", 119939, ulong.MaxValue, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Gunsaber Mode", 62769, Source.Bladesworn, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/f/f0/Unsheathe_Gunsaber.png"),
            new Buff("Dragon Trigger", 62823, Source.Bladesworn, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/b/b1/Dragon_Trigger.png"),
            new Buff("Positive Flow", 62836, Source.Bladesworn, BuffStackType.StackingConditionalLoss, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/f9/Attribute_bonus.png"),
            //new Buff("Fierce as Fire", 62836, Source.Bladesworn, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8e/Fierce_as_Fire.png", 119939, ulong.MaxValue),
            new Buff("Stim State", 62846, Source.Bladesworn, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/a/ad/Combat_Stimulant.png"),
            new Buff("Guns and Glory", 62743, Source.Bladesworn, BuffStackType.Queue, 9, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/7/72/Guns_and_Glory.png"),
        };


    }
}
