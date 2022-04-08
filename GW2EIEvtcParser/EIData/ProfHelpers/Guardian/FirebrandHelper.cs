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
            new DamageCastFinder(46618,46618,EIData.InstantCastFinder.DefaultICD, 0, GW2Builds.May2021Balance), // Flame Rush
            new DamageCastFinder(46616,46616,EIData.InstantCastFinder.DefaultICD, 0, GW2Builds.May2021Balance), // Flame Surge
            //new DamageCastFinder(42360,42360,InstantCastFinder.DefaultICD, 0, GW2Builds.May2021Balance), // Echo of Truth
            //new DamageCastFinder(44008,44008,InstantCastFinder.DefaultICD, 0, GW2Builds.May2021Balance), // Voice of Truth
            new DamageCastFinder(46148,46618,EIData.InstantCastFinder.DefaultICD, GW2Builds.May2021Balance, GW2Builds.EndOfLife), // Mantra of Flame
            new DamageCastFinder(44080,46508,EIData.InstantCastFinder.DefaultICD, GW2Builds.May2021Balance, GW2Builds.EndOfLife), // Mantra of Truth
            new EXTHealingCastFinder(41714, 41714, EIData.InstantCastFinder.DefaultICD, GW2Builds.May2021Balance, GW2Builds.EndOfLife), // Mantra of Solace
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
