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
            new BuffGainCastFinder(62745, -1, EIData.InstantCastFinder.DefaultICD, 119939, ulong.MaxValue), // Gunsaber
            new BuffLossCastFinder(62861, -1, EIData.InstantCastFinder.DefaultICD, 119939, ulong.MaxValue), // Gunsaber sheath
            new DamageCastFinder(62847, 62847, EIData.InstantCastFinder.DefaultICD, 119939, ulong.MaxValue), // Unseen Sword
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {

        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Gunsaber", -1, Source.Bladesworn, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/f/f0/Unsheathe_Gunsaber.png", 119939, ulong.MaxValue),
            new Buff("Dragon Trigger", -1, Source.Bladesworn, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/b/b1/Dragon_Trigger.png", 119939, ulong.MaxValue),
            new Buff("Positive Flow", -1, Source.Bladesworn, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/f/f9/Attribute_bonus.png", 119939, ulong.MaxValue),
            new Buff("Stim State", -1, Source.Bladesworn, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/a/ad/Combat_Stimulant.png", 119939, ulong.MaxValue),
            new Buff("Guns and Glory", -1, Source.Bladesworn, BuffStackType.Queue, 5, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/7/72/Guns_and_Glory.png", 119939, ulong.MaxValue),
        };


    }
}
