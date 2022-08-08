using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class HarbingerHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(EnterHarbingerShroud, HarbingerShroud), // Harbinger shroud
            new BuffLossCastFinder(ExitHarbingerShroud, HarbingerShroud), // Harbinger shroud
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(Blight, "Wicked Corruption", "1% per blight stack", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Harbinger, ByStack, "https://wiki.guildwars2.com/images/0/00/Wicked_Corruption.png", DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta1, GW2Builds.EODBeta4),
            new BuffDamageModifier(Blight, "Septic Corruption", "1% per blight stack", DamageSource.NoPets, 1.0, DamageType.Condition, DamageType.All, Source.Harbinger, ByStack, "https://wiki.guildwars2.com/images/f/f7/Septic_Corruption.png", DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta1, GW2Builds.EODBeta4),
            new BuffDamageModifier(Blight, "Wicked Corruption", "0.5% per blight stack", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Harbinger, ByStack, "https://wiki.guildwars2.com/images/0/00/Wicked_Corruption.png", DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta4),
            new BuffDamageModifier(Blight, "Septic Corruption", "0.5% per blight stack", DamageSource.NoPets, 0.5, DamageType.Condition, DamageType.All, Source.Harbinger, ByStack, "https://wiki.guildwars2.com/images/f/f7/Septic_Corruption.png", DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta4),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Harbinger Shroud",HarbingerShroud, Source.Harbinger, BuffClassification.Other, "https://render.guildwars2.com/file/C9CA706909A104A509F594AADA150D680AA948BC/2479400.png"),
                new Buff("Blight",Blight, Source.Harbinger, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/ac/Blight.png"),
        };
    }
}
