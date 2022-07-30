using System.Collections.Generic;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class FirebrandHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(FlameRush,FlameRush).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance), // Flame Rush
            new DamageCastFinder(FlameSurge,FlameSurge).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance), // Flame Surge
            //new DamageCastFinder(42360,42360,InstantCastFinder.DefaultICD, 0, GW2Builds.May2021Balance), // Echo of Truth
            //new DamageCastFinder(44008,44008,InstantCastFinder.DefaultICD, 0, GW2Builds.May2021Balance), // Voice of Truth
            new DamageCastFinder(MantraOfFlameCast,MantraOfFlameDamage).WithBuilds(GW2Builds.May2021Balance), // Mantra of Flame
            new DamageCastFinder(MantraOfTruthCast,MantraOfTruthDamage).WithBuilds(GW2Builds.May2021Balance), // Mantra of Truth
            new EXTHealingCastFinder(MantraOfSolace, MantraOfSolace).WithBuilds(GW2Builds.May2021Balance), // Mantra of Solace
            new EffectCastFinderByDst(MantraOfLiberation, EffectGUIDs.FirebrandMantraOfLiberationSymbol).UsingChecker((evt, log) => evt.Dst.Spec == Spec.Firebrand),
            new EffectCastFinderByDst(MantraOfLore, EffectGUIDs.FirebrandMantraOfLoreSymbol).UsingChecker((evt, log) => evt.Dst.Spec == Spec.Firebrand),
            new EffectCastFinderByDst(MantraOfPotence, EffectGUIDs.FirebrandMantraOfPotenceSymbol).UsingChecker((evt, log) => evt.Dst.Spec == Spec.Firebrand),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Ashes of the Just",AshesOfTheJust, Source.Firebrand, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/6/6d/Epilogue-_Ashes_of_the_Just.png"),
                new Buff("Eternal Oasis",EternalOasis, Source.Firebrand, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/5/5f/Epilogue-_Eternal_Oasis.png"),
                new Buff("Unbroken Lines",UnbrokenLines, Source.Firebrand, BuffStackType.Stacking, 3, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/d/d8/Epilogue-_Unbroken_Lines.png"),
                new Buff("Tome of Justice",TomeOfJustice, Source.Firebrand, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/ae/Tome_of_Justice.png"),
                new Buff("Tome of Courage",TomeOfCourage,Source.Firebrand, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/54/Tome_of_Courage.png"),
                new Buff("Tome of Resolve",TomeOfResolve, Source.Firebrand, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a9/Tome_of_Resolve.png"),
                new Buff("Quickfire",Quickfire, Source.Firebrand, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d6/Quickfire.png"),
        };

    }
}
