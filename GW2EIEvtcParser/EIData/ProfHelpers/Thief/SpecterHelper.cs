using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class SpecterHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(63155, 63239, EIData.InstantCastFinder.DefaultICD), // Shadow Shroud Enter
            new BuffLossCastFinder(63251, 63239, EIData.InstantCastFinder.DefaultICD), // Shadow Shroud Exit
        };

        private static readonly HashSet<long> _shroudTransform = new HashSet<long>
        {
            63155, 63251,
        };

        public static bool IsShroudTransform(long id)
        {
            return _shroudTransform.Contains(id);
        }

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Shadow Shroud",63239, Source.Specter, BuffClassification.Other, "https://wiki.guildwars2.com/images/f/f3/Enter_Shadow_Shroud.png"),
            new Buff("Shrouded Ally",63207, Source.Specter, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/3a/Siphon.png"),
            new Buff("Rot Wallow Venom",63168, Source.Specter, ArcDPSEnums.BuffStackType.StackingConditionalLoss, 100, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/5/57/Dark_Sentry.png"),
            new Buff("Consume Shadows", 63456, Source.Specter, ArcDPSEnums.BuffStackType.Stacking, 5, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/94/Consume_Shadows.png"),
        };

        private static HashSet<long> Minions = new HashSet<long>()
        {
        };
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }

    }
}
