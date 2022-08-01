using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class SpecterHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(EnterShadowShroud, ShadowShroud), // Shadow Shroud Enter
            new BuffLossCastFinder(ExitShadowShroud, ShadowShroud), // Shadow Shroud Exit
        };

        private static readonly HashSet<long> _shroudTransform = new HashSet<long>
        {
            EnterShadowShroud, ExitShadowShroud,
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
            new Buff("Shadow Shroud",ShadowShroud, Source.Specter, BuffClassification.Other, "https://wiki.guildwars2.com/images/f/f3/Enter_Shadow_Shroud.png"),
            new Buff("Endless Night",EndlessNight, Source.Specter, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/9e/Endless_Night.png"),
            new Buff("Shrouded Ally",ShroudedAlly, Source.Specter, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/3a/Siphon.png"),
            new Buff("Rot Wallow Venom",RotWallowVenom, Source.Specter, ArcDPSEnums.BuffStackType.StackingConditionalLoss, 100, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/5/57/Dark_Sentry.png"),
            new Buff("Consume Shadows", ConsumeShadows, Source.Specter, ArcDPSEnums.BuffStackType.Stacking, 5, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/94/Consume_Shadows.png"),
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
