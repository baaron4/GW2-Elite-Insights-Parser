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
            new BuffGainCastFinder(-1, -1, EIData.InstantCastFinder.DefaultICD, ulong.MaxValue, ulong.MaxValue), // Gunsaber
            new BuffLossCastFinder(-1, -1, EIData.InstantCastFinder.DefaultICD, ulong.MaxValue, ulong.MaxValue), // Gunsaber sheath
            new BuffGainCastFinder(-1, -1, EIData.InstantCastFinder.DefaultICD, ulong.MaxValue, ulong.MaxValue), // Dragon trigger
            new BuffLossCastFinder(-1, -1, EIData.InstantCastFinder.DefaultICD, ulong.MaxValue, ulong.MaxValue), // Dragon trigger end
            new DamageCastFinder(-1, -1, EIData.InstantCastFinder.DefaultICD, ulong.MaxValue, ulong.MaxValue), // Unseen Sword
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {

        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Gunsaber", -1, Source.Bladesworn, BuffNature.GraphOnlyBuff,"", ulong.MaxValue, ulong.MaxValue),
            new Buff("Dragon Trigger", -1, Source.Bladesworn, BuffNature.GraphOnlyBuff,"", ulong.MaxValue, ulong.MaxValue),
            new Buff("Stim State", -1, Source.Bladesworn, BuffNature.GraphOnlyBuff,"", ulong.MaxValue, ulong.MaxValue),
            new Buff("Guns and Glory", -1, Source.Bladesworn, BuffStackType.Queue, 5, BuffNature.GraphOnlyBuff,"", ulong.MaxValue, ulong.MaxValue),
        };


    }
}
