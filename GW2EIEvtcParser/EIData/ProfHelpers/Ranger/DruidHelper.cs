using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class DruidHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(EnterCelestialAvatar, CelestialAvatar).UsingBeforeWeaponSwap(true),
            new BuffLossCastFinder(ExitCelestialAvatar, CelestialAvatar).UsingBeforeWeaponSwap(true),
            new EffectCastFinder(SeedOfLife, EffectGUIDs.DruidSeedOfLife).UsingSrcSpecChecker(Spec.Druid).WithBuilds(GW2Builds.October2022Balance),
            new DamageCastFinder(GlyphOfEquality, GlyphOfEquality).UsingDisableWithEffectData(),
            new EffectCastFinderByDst(GlyphOfEqualityCA, EffectGUIDs.DruidGlyphOfEqualityCA).UsingDstSpecChecker(Spec.Druid),
            new EffectCastFinder(GlyphOfEquality, EffectGUIDs.DruidGlyphOfEquality).UsingSrcSpecChecker(Spec.Druid)
        };

        private static readonly HashSet<long> _celestialAvatar = new HashSet<long>
        {
            EnterCelestialAvatar, ExitCelestialAvatar
        };

        public static bool IsCelestialAvatarTransform(long id)
        {
            return _celestialAvatar.Contains(id);
        }

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(NaturalBalance, "Natural Balance", "10% after leaving or entering Celestial Avatar", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Druid, ByPresence, BuffImages.NaturalBalance, DamageModifierMode.All).WithBuilds(GW2Builds.June2023Balance),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(NaturalBalance, "Natural Balance", "-10% after leaving or entering Celestial Avatar", DamageSource.NoPets, -10.0, DamageType.Strike, DamageType.All, Source.Druid, ByPresence, BuffImages.NaturalBalance, DamageModifierMode.All).WithBuilds(GW2Builds.June2023Balance),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Celestial Avatar", CelestialAvatar, Source.Druid, BuffClassification.Other, BuffImages.CelestialAvatar),
            new Buff("Ancestral Grace", AncestralGraceBuff, Source.Druid, BuffClassification.Other, BuffImages.AncestralGrace).WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOBetaAndSilentSurfNM),
            new Buff("Glyph of Empowerment", GlyphOfEmpowerment, Source.Druid, BuffClassification.Offensive, BuffImages.GlyphOfTheStars).WithBuilds(GW2Builds.StartOfLife, GW2Builds.April2019Balance),
            new Buff("Glyph of Unity", GlyphOfUnityBuff, Source.Druid, BuffClassification.Other, BuffImages.GlyphOfUnity),
            new Buff("Glyph of Unity (CA)", GlyphOfUnityCABuff, Source.Druid, BuffClassification.Other, BuffImages.GlyphOfUnityCelestialAvatar),
            new Buff("Glyph of the Stars", GlyphOfTheStars, Source.Druid, BuffClassification.Defensive, BuffImages.GlyphOfTheStars).WithBuilds(GW2Builds.April2019Balance, GW2Builds.October2022Balance),
            new Buff("Glyph of the Stars (CA)", GlyphOfTheStarsCA, Source.Druid, BuffClassification.Defensive, BuffImages.GlyphOfEmpowermentCelestialAvatar).WithBuilds(GW2Builds.April2019Balance, GW2Builds.EndOfLife),
            new Buff("Natural Mender", NaturalMender, Source.Druid, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.NaturalMender).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2022Balance),
            new Buff("Lingering Light", LingeringLight, Source.Druid, BuffClassification.Other, BuffImages.LingeringLight).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2023Balance),
            new Buff("Natural Balance", NaturalBalance, Source.Druid, BuffClassification.Other, BuffImages.NaturalBalance).WithBuilds(GW2Builds.June2023Balance),
        };
    }
}
