using System.Collections.Generic;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class FirebrandHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(FlameRushOld, FlameRushOld)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance)
                .UsingDisableWithEffectData(),
            new DamageCastFinder(FlameRush, FlameRush)
                .WithBuilds(GW2Builds.February2023Balance),
            new DamageCastFinder(FlameSurgeOld, FlameSurgeOld)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance)
                .UsingDisableWithEffectData(),
            new DamageCastFinder(FlameSurge, FlameSurge)
                .WithBuilds(GW2Builds.February2023Balance),
            //new DamageCastFinder(42360,42360,InstantCastFinder.DefaultICD, 0, GW2Builds.May2021Balance), // Echo of Truth
            //new DamageCastFinder(44008,44008,InstantCastFinder.DefaultICD, 0, GW2Builds.May2021Balance), // Voice of Truth
            new DamageCastFinder(MantraOfFlameCast, MantraOfFlameDamage)
                .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance)
                .UsingDisableWithEffectData(),
            new DamageCastFinder(MantraOfTruthCast, MantraOfTruthDamage)
                .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance)
                .UsingDisableWithEffectData(),
            //
            new EXTHealingCastFinder(MantraOfSolace, MantraOfSolace)
                .WithBuilds(GW2Builds.May2021Balance)
                .UsingDisableWithEffectData(),
            new EffectCastFinderByDst(MantraOfFlameCast, EffectGUIDs.FirebrandMantraOfFlameSymbol)
                .UsingDstSpecChecker(Spec.Firebrand)
                .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),
            new EffectCastFinderByDst(MantraOfSolace, EffectGUIDs.FirebrandMantraOfSolaceSymbol)
                .UsingDstSpecChecker(Spec.Firebrand)
                .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),
            new EffectCastFinderByDst(MantraOfTruthCast, EffectGUIDs.FirebrandMantraOfTruthSymbol)
                .UsingDstSpecChecker(Spec.Firebrand)
                .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),
            new EffectCastFinderByDst(MantraOfLiberation, EffectGUIDs.FirebrandMantraOfLiberationSymbol)
                .UsingDstSpecChecker(Spec.Firebrand)
                .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),
            new EffectCastFinderByDst(MantraOfLore, EffectGUIDs.FirebrandMantraOfLoreSymbol)
                .UsingDstSpecChecker(Spec.Firebrand)
                .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),
            new EffectCastFinderByDst(MantraOfPotence, EffectGUIDs.FirebrandMantraOfPotenceSymbol)
                .UsingDstSpecChecker(Spec.Firebrand)
                .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),
            new EffectCastFinderByDst(RestoringReprieveOrRejunevatingRespite, EffectGUIDs.FirebrandMantraOfSolaceSymbol)
                .UsingDstSpecChecker(Spec.Firebrand)
                .WithBuilds(GW2Builds.February2023Balance),
            //
            new DamageCastFinder(EchoOfTrue, EchoOfTrue)
                .WithBuilds(GW2Builds.February2023Balance),
            new DamageCastFinder(VoiceOfTruth, VoiceOfTruth)
                .WithBuilds(GW2Builds.February2023Balance),
            //
            new EffectCastFinderByDst(PortentOfFreedomOrUnhinderedDelivery, EffectGUIDs.FirebrandMantraOfLiberationSymbol)
                .UsingDstSpecChecker(Spec.Firebrand)
                .WithBuilds( GW2Builds.February2023Balance),
            new EffectCastFinderByDst(OpeningPassageOrClarifiedConclusion, EffectGUIDs.FirebrandMantraOfLoreSymbol)
                .UsingDstSpecChecker(Spec.Firebrand)
                .WithBuilds(GW2Builds.February2023Balance),
            new EffectCastFinderByDst(PotentHasteOrOverwhelmingCelerity, EffectGUIDs.FirebrandMantraOfPotenceSymbol)
                .UsingDstSpecChecker(Spec.Firebrand)
                .WithBuilds(GW2Builds.February2023Balance),
            // tomes
            new BuffGainCastFinder(TomeOfJusticeSkill, TomeOfJusticeOpen)
                .WithBuilds(GW2Builds.November2022Balance)
                .UsingBeforeWeaponSwap(true),
            new BuffGainCastFinder(TomeOfResolveSkill, TomeOfResolveOpen)
                .WithBuilds(GW2Builds.November2022Balance)
                .UsingBeforeWeaponSwap(true),
            new BuffGainCastFinder(TomeOfCourageSkill, TomeOfCourageOpen)
                .WithBuilds(GW2Builds.November2022Balance)
                .UsingBeforeWeaponSwap(true),
            new BuffLossCastFinder(StowTome, TomeOfJusticeOpen)
                .WithBuilds(GW2Builds.November2022Balance)
                .UsingBeforeWeaponSwap(true),
            new BuffLossCastFinder(StowTome, TomeOfResolveOpen)
                .WithBuilds(GW2Builds.November2022Balance)
                .UsingBeforeWeaponSwap(true),
            new BuffLossCastFinder(StowTome, TomeOfCourageOpen)
                .WithBuilds(GW2Builds.November2022Balance)
                .UsingBeforeWeaponSwap(true),
        };

        private static readonly HashSet<long> _firebrandTomes = new HashSet<long>
        {
            TomeOfJusticeSkill,
            TomeOfResolveSkill,
            TomeOfCourageSkill,
            StowTome,
        };

        public static bool IsFirebrandTome(long id)
        {
            return _firebrandTomes.Contains(id);
        }


        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Ashes of the Just", AshesOfTheJust, Source.Firebrand, BuffStackType.Stacking, 25, BuffClassification.Offensive, BuffImages.EpilogueAshesOfTheJust),
            new Buff("Eternal Oasis", EternalOasis, Source.Firebrand, BuffClassification.Defensive, BuffImages.EpilogueEternalOasis),
            new Buff("Unbroken Lines", UnbrokenLines, Source.Firebrand, BuffStackType.Stacking, 3, BuffClassification.Defensive, BuffImages.EpilogueUnbrokenLines),
            new Buff("Tome of Justice", TomeOfJusticeBuff, Source.Firebrand, BuffClassification.Other, BuffImages.TomeOfJustice),
            new Buff("Tome of Courage", TomeOfCourageBuff, Source.Firebrand, BuffClassification.Other, BuffImages.TomeOfCourage),
            new Buff("Tome of Resolve", TomeOfResolveBuff, Source.Firebrand, BuffClassification.Other, BuffImages.TomeOfResolve),
            new Buff("Quickfire", Quickfire, Source.Firebrand, BuffClassification.Other, BuffImages.Quickfire),
            new Buff("Dormant Justice", DormantJustice, Source.Firebrand, BuffClassification.Other, BuffImages.DormantJustice),
            new Buff("Dormant Courage", DormantCourage, Source.Firebrand, BuffClassification.Other, BuffImages.DormantCourage),
            new Buff("Dormant Resolve", DormantResolve, Source.Firebrand, BuffClassification.Other, BuffImages.DormantResolve),
        };

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Guardian;

            // Valiant Bulwark
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.FirebrandValiantBulwark, out IReadOnlyList<EffectEvent> valiantBulwarks))
            {
                var skill = new SkillModeDescriptor(player, Spec.Firebrand, Chapter3ValiantBulwark, SkillModeCategory.ProjectileManagement);
                foreach (EffectEvent effect in valiantBulwarks)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectValiantBulwark);
                }
            }

            // Stalwart Stand
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.FirebrandStalwartStand1, out IReadOnlyList<EffectEvent> stalwartStands))
            {
                var skill = new SkillModeDescriptor(player, Spec.Firebrand, Chapter4StalwartStand, SkillModeCategory.ImportantBuffs);
                foreach (EffectEvent effect in stalwartStands)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, ParserIcons.EffectStalwartStand);
                }
            }

            // Shining River
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.FirebrandShiningRiver1, out IReadOnlyList<EffectEvent> shiningRiver))
            {
                var skill = new SkillModeDescriptor(player, Spec.Firebrand, Chapter4ShiningRiver, SkillModeCategory.Heal);
                foreach (EffectEvent effect in shiningRiver)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, ParserIcons.EffectShiningRiver);
                }
            }

            // Scorched Aftermath
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.FirebrandScorchedAftermath1, out IReadOnlyList<EffectEvent> scorchedAftermath))
            {
                var skill = new SkillModeDescriptor(player, Spec.Firebrand, Chapter4ScorchedAftermath, SkillModeCategory.ShowOnSelect);
                foreach (EffectEvent effect in scorchedAftermath)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, ParserIcons.EffectScorchedAftermath);
                }
            }
        }
    }
}
