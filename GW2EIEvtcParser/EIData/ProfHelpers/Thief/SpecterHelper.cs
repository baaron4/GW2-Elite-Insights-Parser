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
    internal static class SpecterHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(EnterShadowShroud, ShadowShroud).UsingBeforeWeaponSwap(true), // Shadow Shroud Enter
            new BuffLossCastFinder(ExitShadowShroud, ShadowShroud).UsingBeforeWeaponSwap(true), // Shadow Shroud Exit
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
            new Buff("Shadow Shroud", ShadowShroud, Source.Specter, BuffClassification.Other, BuffImages.EnterShadowShroud),
            new Buff("Endless Night", EndlessNight, Source.Specter, BuffClassification.Other, BuffImages.EndlessNight),
            new Buff("Shrouded Ally", ShroudedAlly, Source.Specter, BuffClassification.Other, BuffImages.Siphon),
            new Buff("Rot Wallow Venom", RotWallowVenom, Source.Specter, BuffStackType.StackingConditionalLoss, 100, BuffClassification.Offensive, BuffImages.DarkSentry),
            new Buff("Consume Shadows", ConsumeShadows, Source.Specter, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, BuffImages.ConsumeShadows),
        };

        private static HashSet<int> Minions = new HashSet<int>()
        {
            (int)MinionID.Specter1,
            (int)MinionID.Specter2,
            (int)MinionID.Specter3,
            (int)MinionID.Specter4,
            (int)MinionID.Specter5,
            (int)MinionID.Specter6,
            (int)MinionID.Specter7,
            (int)MinionID.Specter8,
            (int)MinionID.Specter9,
            (int)MinionID.Specter10,
        };

        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }

    }
}
