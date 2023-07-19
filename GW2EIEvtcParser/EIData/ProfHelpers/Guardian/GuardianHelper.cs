using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.EIData.CastFinderHelpers;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class GuardianHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(ShieldOfWrathSkill, ShieldOfWrathBuff),
            new BuffGainCastFinder(ZealotsFlameSkill, ZealotsFlameBuff).UsingICD(0),
            //new BuffLossCastFinder(9115,9114,InstantCastFinder.DefaultICD), // Virtue of Justice
            //new BuffLossCastFinder(9120,9119,InstantCastFinder.DefaultICD), // Virtue of Resolve
            //new BuffLossCastFinder(9118,9113,InstantCastFinder.DefaultICD), // Virtue of Courage

            // Meditations
            new DamageCastFinder(JudgesIntervention, JudgesIntervention).UsingDisableWithEffectData(),
            new BuffGainCastFinder(JudgesIntervention, MercifulAndJudgesInterventionSelfBuff)
                .UsingChecker((evt, combatData, agentData, skillData) => HasRelatedEffectDst(combatData, EffectGUIDs.GuardianGenericTeleport2, evt.To, evt.Time + 120))
                .UsingNotAccurate(true),
            new BuffGainCastFinder(MercifulInterventionSkill, MercifulAndJudgesInterventionSelfBuff)
                .UsingChecker((evt, combatData, agentData, skillData) => HasRelatedEffectDst(combatData, EffectGUIDs.GuardianMercifulIntervention, evt.To, evt.Time + 200))
                .UsingNotAccurate(true),
            new EffectCastFinderByDst(ContemplationOfPurity, EffectGUIDs.GuardianContemplationOfPurity1).UsingDstBaseSpecChecker(Spec.Guardian),
            new DamageCastFinder(SmiteCondition, SmiteCondition),
            new DamageCastFinder(LesserSmiteCondition, LesserSmiteCondition).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            
            // Shouts
            new EffectCastFinderByDst(SaveYourselves, EffectGUIDs.GuardianSaveYourselves)
                .UsingDstBaseSpecChecker(Spec.Guardian)
                .UsingSecondaryEffectChecker(EffectGUIDs.GuardianShout),
            // distinguish by boons, check duration/stacks to counteract pure of voice
            new EffectCastFinderByDst(Advance, EffectGUIDs.GuardianShout)
                .UsingDstBaseSpecChecker(Spec.Guardian)
                .UsingChecker((evt, combatData, agentData, skillData) =>
                {
                    return FindRelatedEvents(combatData.GetBuffData(Aegis).OfType<BuffApplyEvent>(), evt.Time)
                        .Any(apply => apply.By == evt.Dst && apply.To == evt.Dst && apply.AppliedDuration + ServerDelayConstant >= 20000 && apply.AppliedDuration - ServerDelayConstant <= 40000);
                }) // identify advance by self-applied 20s to 40s aegis
                .UsingNotAccurate(true),
            new EffectCastFinderByDst(StandYourGround, EffectGUIDs.GuardianShout)
                .UsingDstBaseSpecChecker(Spec.Guardian)
                .UsingChecker((evt, combatData, agentData, skillData) =>
                {
                    return 5 <= FindRelatedEvents(combatData.GetBuffData(Stability).OfType<BuffApplyEvent>(), evt.Time)
                        .Count(apply => apply.By == evt.Dst && apply.To == evt.Dst);
                }) // identify stand your ground by self-applied 5+ stacks of stability
                .UsingNotAccurate(true),
            // hold the line boons may overlap with save yourselves/pure of voice

            // Signets
            new EffectCastFinderByDst(SignetOfJudgmentSkill, EffectGUIDs.GuardianSignetOfJudgement2).UsingDstBaseSpecChecker(Spec.Guardian),
            new DamageCastFinder(LesserSignetOfWrath, LesserSignetOfWrath).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            
            //new DamageCastFinder(9097,9097), // Symbol of Blades
            new DamageCastFinder(GlacialHeart, GlacialHeart).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new DamageCastFinder(ShatteredAegis, ShatteredAegis).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new EXTHealingCastFinder(SelflessDaring, SelflessDaring).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Zeal
            new BuffDamageModifierTarget(Burning, "Fiery Wrath", "7% on burning target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, BuffImages.FieryWrath, DamageModifierMode.All),
            new BuffDamageModifierTarget(Vulnerability, "Symbolic Exposure", "5% on vuln target", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, BuffImages.SymbolicExposure, DamageModifierMode.All),
            new BuffDamageModifier(SymbolicAvenger, "Symbolic Avenger", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Guardian, ByStack, BuffImages.SymbolicAvenger, DamageModifierMode.All).WithBuilds(GW2Builds.July2019Balance),
            // Radiance
            new BuffDamageModifier(Retaliation, "Retribution", "10% under retaliation", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, BuffImages.RetributionTrait, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffDamageModifier(Resolution, "Retribution", "10% under resolution", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, BuffImages.RetributionTrait, DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance),
            // Virtues
            new BuffDamageModifier(Aegis, "Unscathed Contender", "20% under aegis", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, BuffImages.UnscathedContender, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2023Balance),
            new BuffDamageModifier(Aegis, "Unscathed Contender (Aegis)", "7% under aegis", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, BuffImages.UnscathedContender, DamageModifierMode.All).WithBuilds(GW2Builds.February2023Balance),
            new DamageLogDamageModifier("Unscathed Contender (HP)", "7% if hp >=90%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Guardian, BuffImages.UnscathedContender, (x, log) => x.IsOverNinety, ByPresence, DamageModifierMode.All).WithBuilds( GW2Builds.February2023Balance),
            new BuffDamageModifier(NumberOfBoons, "Power of the Virtuous", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Guardian, ByStack, BuffImages.PowerOfTheVirtuous,  DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffDamageModifier(NumberOfBoons, "Inspired Virtue", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Guardian, ByStack, BuffImages.InspiredVirtue, DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance),
            new BuffDamageModifier(InspiringVirtue, "Inspiring Virtue", "10% (6s) after activating a virtue ", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, BuffImages.VirtuousSolace, DamageModifierMode.All).WithBuilds(GW2Builds.February2020Balance),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {        
            // Skills
            new Buff("Zealot's Flame", ZealotsFlameBuff, Source.Guardian, BuffStackType.Queue, 25, BuffClassification.Other, BuffImages.ZealotsFlame),
            new Buff("Purging Flames", PurgingFlames, Source.Guardian, BuffClassification.Other, BuffImages.PurgingFlames),
            new Buff("Litany of Wrath", LitanyOfWrath, Source.Guardian, BuffClassification.Other, BuffImages.LitanyOfWrath),
            new Buff("Renewed Focus", RenewedFocus, Source.Guardian, BuffClassification.Other, BuffImages.RenewedFocus),
            new Buff("Shield of Wrath", ShieldOfWrathBuff, Source.Guardian, BuffStackType.Stacking, 3, BuffClassification.Other, BuffImages.ShieldOfWrath),
            new Buff("Binding Blade (Self)", BindingBladeSelf, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.BindingBlade),
            new Buff("Binding Blade", BindingBlade, Source.Guardian, BuffClassification.Other, BuffImages.BindingBlade),
            new Buff("Banished", Banished, Source.Guardian, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, BuffImages.Banish),
            new Buff("Merciful Intervention (Self)", MercifulAndJudgesInterventionSelfBuff, Source.Guardian, BuffClassification.Support, BuffImages.MercifulIntervention),
            new Buff("Merciful Intervention (Target)", MercifulInterventionTargetBuff, Source.Guardian, BuffClassification.Support, BuffImages.MercifulIntervention),
            // Signets
            new Buff("Signet of Resolve", SignetOfResolve, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.SignetOfResolve),
            new Buff("Signet of Resolve (Shared)", SignetOfResolveShared, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Defensive, BuffImages.SignetOfResolve).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new Buff("Signet of Resolve (PI)", SignetOfResolvePI, Source.Guardian, BuffClassification.Other, BuffImages.SignetOfResolve).WithBuilds(GW2Builds.June2022Balance, GW2Builds.EndOfLife),
            new Buff("Bane Signet", BaneSignet, Source.Guardian, BuffClassification.Other, BuffImages.BaneSignet),
            new Buff("Bane Signet (PI)", BaneSignetPI, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Offensive, BuffImages.BaneSignet).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new Buff("Bane Signet (PI)", BaneSignetPI, Source.Guardian, BuffClassification.Other, BuffImages.BaneSignet).WithBuilds(GW2Builds.June2022Balance, GW2Builds.EndOfLife),
            new Buff("Signet of Judgment", SignetOfJudgmentBuff, Source.Guardian, BuffClassification.Other, BuffImages.SignetOfJudgment),
            new Buff("Signet of Judgment (PI)", SignetOfJudgmentPI, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Defensive, BuffImages.SignetOfJudgment).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new Buff("Signet of Judgment (PI)", SignetOfJudgmentPI, Source.Guardian, BuffClassification.Other, BuffImages.SignetOfJudgment).WithBuilds(GW2Builds.June2022Balance, GW2Builds.EndOfLife),
            new Buff("Signet of Mercy", SignetOfMercy, Source.Guardian, BuffClassification.Other, BuffImages.SignetOfMercy),
            new Buff("Signet of Mercy (PI)", SignetOfMercyPI, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Defensive, BuffImages.SignetOfMercy).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new Buff("Signet of Mercy (PI)", SignetOfMercyPI, Source.Guardian, BuffClassification.Other, BuffImages.SignetOfMercy).WithBuilds(GW2Builds.June2022Balance, GW2Builds.EndOfLife),
            new Buff("Signet of Wrath", SignetOfWrath, Source.Guardian, BuffClassification.Other, BuffImages.SignetOfWrath),
            new Buff("Signet of Wrath (PI)", SignetOfWrathPI, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Offensive, BuffImages.SignetOfWrath).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new Buff("Signet of Wrath (PI)", SignetOfWrathPI, Source.Guardian, BuffClassification.Other, BuffImages.SignetOfWrath).WithBuilds(GW2Builds.June2022Balance, GW2Builds.EndOfLife),
            new Buff("Signet of Courage", SignetOfCourage, Source.Guardian, BuffClassification.Other, BuffImages.SignetOfCourage),
            new Buff("Signet of Courage (Shared)", SignetOfCourageShared , Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Defensive, BuffImages.SignetOfCourage).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new Buff("Signet of Courage (PI)", SignetOfCouragePI , Source.Guardian, BuffClassification.Other, BuffImages.SignetOfCourage).WithBuilds(GW2Builds.June2022Balance, GW2Builds.EndOfLife),
            // Virtues
            new Buff("Virtue of Justice", VirtueOfJustice, Source.Guardian, BuffClassification.Other, BuffImages.VirtueOfJustice),
            new Buff("Virtue of Courage", VirtueOfCourage, Source.Guardian, BuffClassification.Other, BuffImages.VirtueOfCourage),
            new Buff("Virtue of Resolve", VirtueOfResolve, Source.Guardian, BuffClassification.Other, BuffImages.VirtueOfResolve),
            new Buff("Justice", Justice, Source.Guardian, BuffClassification.Other, BuffImages.VirtueOfJustice),
            // Traits
            new Buff("Strength in Numbers", StrengthinNumbers, Source.Guardian, BuffClassification.Defensive, BuffImages.StrengthInNumbers).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new Buff("Invigorated Bulwark", InvigoratedBulwark, Source.Guardian, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.InvigoratedBulwark),
            new Buff("Virtue of Resolve (Battle Presence)", VirtueOfResolveBattlePresence, Source.Guardian, BuffStackType.Queue, 2, BuffClassification.Defensive, BuffImages.BattlePresence),
            new Buff("Virtue of Resolve (Battle Presence - Absolute Resolve)", VirtueOfResolveBattlePresenceAbsoluteResolve, Source.Guardian, BuffStackType.Queue, 2, BuffClassification.Defensive, BuffImages.VirtueOfResolve),
            new Buff("Symbolic Avenger", SymbolicAvenger, Source.Guardian, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.SymbolicAvenger).WithBuilds(GW2Builds.July2019Balance, GW2Builds.EndOfLife),
            new Buff("Inspiring Virtue", InspiringVirtue, Source.Guardian, BuffStackType.Queue, 99, BuffClassification.Other, BuffImages.VirtuousSolace).WithBuilds(GW2Builds.February2020Balance, GW2Builds.February2020Balance2),
            new Buff("Inspiring Virtue", InspiringVirtue, Source.Guardian, BuffClassification.Other, BuffImages.VirtuousSolace).WithBuilds(GW2Builds.February2020Balance2, GW2Builds.EndOfLife),
            new Buff("Force of Will", ForceOfWill, Source.Guardian, BuffClassification.Other, BuffImages.ForceOfWill),
        };

        private static HashSet<int> Minions = new HashSet<int>()
        {
            (int)MinionID.BowOfTruth,
            (int)MinionID.HammerOfWisdom,
            (int)MinionID.ShieldOfTheAvenger,
            (int)MinionID.SwordOfJustice,
        };
        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }

    }
}
