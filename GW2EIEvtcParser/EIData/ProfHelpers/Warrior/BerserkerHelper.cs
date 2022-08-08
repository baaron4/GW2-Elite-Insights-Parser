using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class BerserkerHelper
    {
        /////////////////////
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(KingOfFires, KingOfFires).WithBuilds(GW2Builds.July2019Balance), // King of Fires
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(AlwaysAngry, "Always Angry", "7% per stack", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Berserker, ByPresence, "https://wiki.guildwars2.com/images/6/63/Always_Angry.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.April2019Balance),
            new BuffDamageModifier(Berserk, "Bloody Roar", "10% while in berserk", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Berserker, ByPresence, "https://wiki.guildwars2.com/images/e/e1/Bloody_Roar.png", DamageModifierMode.PvE).UsingApproximate(true).WithBuilds(GW2Builds.StartOfLife, GW2Builds.April2019Balance),
            new BuffDamageModifier(Berserk, "Bloody Roar", "20% while in berserk", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Berserker, ByPresence, "https://wiki.guildwars2.com/images/e/e1/Bloody_Roar.png", DamageModifierMode.PvE).UsingApproximate(true).WithBuilds(GW2Builds.April2019Balance, GW2Builds.July2019Balance),
            new BuffDamageModifier(Berserk, "Bloody Roar", "20% while in berserk", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Berserker, ByPresence, "https://wiki.guildwars2.com/images/e/e1/Bloody_Roar.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.July2019Balance),
            new BuffDamageModifier(Berserk, "Bloody Roar", "15% while in berserk", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Berserker, ByPresence, "https://wiki.guildwars2.com/images/e/e1/Bloody_Roar.png", DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.July2019Balance),

        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Berserk",Berserk, Source.Berserker, BuffClassification.Other,"https://wiki.guildwars2.com/images/4/44/Berserk.png"),
                new Buff("Flames of War", FlamesOfWar, Source.Berserker, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/6f/Flames_of_War_%28warrior_skill%29.png"),
                new Buff("Blood Reckoning", BloodReckoning , Source.Berserker, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d6/Blood_Reckoning.png"),
                new Buff("Rock Guard", RockGuard , Source.Berserker, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c7/Shattering_Blow.png"),
                new Buff("Feel No Pain (Savage Instinct)",FeelNoPainSavageInstinct, Source.Berserker, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/4d/Savage_Instinct.png", GW2Builds.April2019Balance, GW2Builds.EndOfLife),
                new Buff("Always Angry",AlwaysAngry, Source.Berserker, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/63/Always_Angry.png", 0 , GW2Builds.April2019Balance),
        };


    }
}
