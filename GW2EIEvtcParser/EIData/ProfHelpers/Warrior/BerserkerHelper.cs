using System.Collections.Generic;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class BerserkerHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(KingOfFires, KingOfFires).WithBuilds(GW2Builds.July2019Balance).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new EffectCastFinder(Outrage, EffectGUIDs.BerserkerOutrage)
                .UsingSrcSpecChecker(Spec.Berserker),
        };

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(AlwaysAngry, "Always Angry", "7% per stack", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Berserker, ByPresence, BuffImages.AlwaysAngry, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.April2019Balance),
            new BuffOnActorDamageModifier(Berserk, "Bloody Roar", "10% while in berserk", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Berserker, ByPresence, BuffImages.BloodyRoar, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.April2019Balance),
            new BuffOnActorDamageModifier(Berserk, "Bloody Roar", "20% while in berserk", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Berserker, ByPresence, BuffImages.BloodyRoar, DamageModifierMode.All).WithBuilds(GW2Builds.April2019Balance, GW2Builds.July2019Balance),
            new BuffOnActorDamageModifier(Berserk, "Bloody Roar", "20% while in berserk", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Berserker, ByPresence, BuffImages.BloodyRoar, DamageModifierMode.PvE).WithBuilds(GW2Builds.July2019Balance, GW2Builds.August2022Balance),
            new BuffOnActorDamageModifier(Berserk, "Bloody Roar", "25% while in berserk", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Berserker, ByPresence, BuffImages.BloodyRoar, DamageModifierMode.PvE).WithBuilds(GW2Builds.August2022Balance, GW2Builds.June2023Balance),
            new BuffOnActorDamageModifier(Berserk, "Bloody Roar", "20% while in berserk", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Berserker, ByPresence, BuffImages.BloodyRoar, DamageModifierMode.PvE).WithBuilds(GW2Builds.June2023Balance, GW2Builds.January2024Balance),
            new BuffOnActorDamageModifier(Berserk, "Bloody Roar", "15% while in berserk", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Berserker, ByPresence, BuffImages.BloodyRoar, DamageModifierMode.PvE).WithBuilds(GW2Builds.January2024Balance),
            new BuffOnActorDamageModifier(Berserk, "Bloody Roar", "15% while in berserk", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Berserker, ByPresence, BuffImages.BloodyRoar, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.July2019Balance, GW2Builds.October2022Balance),
            new BuffOnActorDamageModifier(Berserk, "Bloody Roar", "10% while in berserk", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Berserker, ByPresence, BuffImages.BloodyRoar, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.October2022Balance),

        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new CounterOnActorDamageModifier(FeelNoPainSavageInstinct, "Feel No Pain", "-100%", DamageSource.NoPets, DamageType.StrikeAndCondition, DamageType.All, Source.Berserker, BuffImages.SavageInstinct, DamageModifierMode.All)
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Berserk", Berserk, Source.Berserker, BuffClassification.Other, BuffImages.Berserk),
            new Buff("Flames of War", FlamesOfWar, Source.Berserker, BuffClassification.Other, BuffImages.FlamesOfWarWarrior).WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOBetaAndSilentSurfNM),
            new Buff("Blood Reckoning", BloodReckoning , Source.Berserker, BuffClassification.Other, BuffImages.BloodReckoning).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2022Balance),
            new Buff("Blood Reckoning", BloodReckoning , Source.Berserker, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.BloodReckoning).WithBuilds(GW2Builds.October2022Balance),
            new Buff("Rock Guard", RockGuard , Source.Berserker, BuffClassification.Other, BuffImages.ShatteringBlow),
            new Buff("Feel No Pain (Savage Instinct)", FeelNoPainSavageInstinct, Source.Berserker, BuffClassification.Other, BuffImages.SavageInstinct).WithBuilds(GW2Builds.April2019Balance),
            new Buff("Always Angry", AlwaysAngry, Source.Berserker, BuffClassification.Other, BuffImages.AlwaysAngry).WithBuilds(GW2Builds.StartOfLife, GW2Builds.April2019Balance),
            new Buff("Heat the Soul", HeatTheSoulBuff, Source.Berserker, BuffClassification.Other, BuffImages.HeatTheSoul),
        };

    }
}
