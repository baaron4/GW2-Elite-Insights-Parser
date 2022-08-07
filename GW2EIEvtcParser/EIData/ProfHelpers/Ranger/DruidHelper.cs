using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class DruidHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(EnterCelestialAvatar,CelestialAvatar), // Celestial Avatar
            new BuffLossCastFinder(ExitCelestialAvatar,CelestialAvatar), // Release Celestial Avatar
            new DamageCastFinder(GlyphOfEquality, GlyphOfEquality).UsingChecker((evt, log) => log.GetEffectEvents().Count == 0), // Disable this one when effect events are present
            new EffectCastFinderByDst(GlyphOfEqualityCA, EffectGUIDs.DruidGlyphOfEqualityCA).UsingChecker((evt, log) => evt.Dst.Spec == Spec.Druid),
            new EffectCastFinder(GlyphOfEquality, EffectGUIDs.DruidGlyphOfEquality).UsingChecker((evt, log) => evt.Src.Spec == Spec.Druid)
        };

        private static readonly HashSet<long> _celestialAvatar = new HashSet<long>
        {
            EnterCelestialAvatar, ExitCelestialAvatar
        };

        public static bool IsCelestialAvatarTransform(long id)
        {
            return _celestialAvatar.Contains(id);
        }

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Celestial Avatar", CelestialAvatar, Source.Druid, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/59/Celestial_Avatar.png"),
                new Buff("Ancestral Grace", AncestralGrace, Source.Druid, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/4b/Ancestral_Grace.png"),
                new Buff("Glyph of Empowerment", GlyphOfEmpowerment, Source.Druid, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/d/d7/Glyph_of_the_Stars.png", 0 , GW2Builds.April2019Balance),
                new Buff("Glyph of Unity", GlyphOfUnity, Source.Druid, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b1/Glyph_of_Unity.png"),
                new Buff("Glyph of Unity (CA)", GlyphOfUnityCA, Source.Druid, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/4/4c/Glyph_of_Unity_%28Celestial_Avatar%29.png"),
                new Buff("Glyph of the Stars", GlyphOfTheStars, Source.Druid, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/d/d7/Glyph_of_the_Stars.png", GW2Builds.April2019Balance, GW2Builds.EndOfLife),
                new Buff("Glyph of the Stars (CA)", GlyphOfTheStarsCA, Source.Druid, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/d/d7/Glyph_of_the_Stars.png", GW2Builds.April2019Balance, GW2Builds.EndOfLife),
                new Buff("Natural Mender",NaturalMender, Source.Druid, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e9/Natural_Mender.png"),
                new Buff("Lingering Light",LingeringLight, Source.Druid, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5d/Lingering_Light.png"),
        };

    }
}
