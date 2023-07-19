using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.EIData.CastFinderHelpers;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class NecromancerHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(EnterDeathShroud, DeathShroud).UsingBeforeWeaponSwap(true),
            new BuffLossCastFinder(ExitDeathShroud, DeathShroud).UsingBeforeWeaponSwap(true),
            new DamageCastFinder(LesserEnfeeble, LesserEnfeeble).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new DamageCastFinder(LesserSpinalShivers, LesserSpinalShivers).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),

            // distinguish unholy burst & spiteful spirit using hit, unholy burst will only ever trigger if a target is hit
            new DamageCastFinder(UnholyBurst, UnholyBurst),
            new DamageCastFinder(SpitefulSpirit, SpitefulSpirit).UsingDisableWithEffectData().UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new EffectCastFinder(SpitefulSpirit, EffectGUIDs.NecromancerUnholyBurst)
                .UsingSrcBaseSpecChecker(Spec.Necromancer)
                .UsingChecker((evt, combatData, skillData, agentData) => !HasRelatedHit(combatData, UnholyBurst, evt.Src, evt.Time))
                .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),

            new BuffGainCastFinder(SpectralArmorSkill, SpectralArmorBuff).WithBuilds(GW2Builds.December2018Balance),
            new BuffGainCastFinder(SpectralWalkSkill, SpectralWalkOldBuff).WithBuilds(GW2Builds.StartOfLife, GW2Builds.December2018Balance),
            new BuffGainCastFinder(SpectralWalkSkill, SpectralWalkBuff).WithBuilds(GW2Builds.December2018Balance),
            new BuffLossCastFinder(SpectralRecallSkill, SpectralWalkTeleportBuff)
                .UsingChecker((evt, combatData, skillData, agentData) => !FindRelatedEvents(combatData.GetBuffData(SpectralWalkBuff).OfType<BuffRemoveAllEvent>(), evt.Time + 120).Any())
                .WithBuilds(GW2Builds.December2018Balance),
            new EffectCastFinderByDst(PlagueSignetSkill, EffectGUIDs.NecromancerPlagueSignet).UsingDstBaseSpecChecker(Spec.Necromancer),
            
            // Minions
            new MinionCommandCastFinder(RigorMortisSkill, (int) MinionID.BoneFiend),
            new MinionCommandCastFinder(HauntSkill, (int) MinionID.ShadowFiend),
            new MinionCommandCastFinder(NecroticTraversal, (int) MinionID.FleshWurm),
            // new BuffGainWithMinionsCastFinder(RigorMortisSkill, RigorMortisEffect),
            // new EffectCastFinder(NecroticTraversal, EffectGUIDs.NecromancerNecroticTraversal),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Spite
            new BuffDamageModifierTarget(NumberOfBoons, "Spiteful Talisman", "10% on boonless target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByAbsence, BuffImages.SpitefulTalisman, DamageModifierMode.All),
            new BuffDamageModifier(Downed, "Death's Embrace", "25% on while downed", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.DeathsEmbrace, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2020Balance),
            new BuffDamageModifier(Downed, "Death's Embrace", "25% on while downed", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.DeathsEmbrace, DamageModifierMode.PvE).WithBuilds(GW2Builds.February2020Balance),
            new BuffDamageModifier(Downed, "Death's Embrace", "5% on while downed", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.DeathsEmbrace, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2020Balance),
            new BuffDamageModifierTarget(Fear, "Dread", "20% on feared target", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.UnholyFervor, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2018Balance),
            new BuffDamageModifierTarget(Fear, "Dread", "33% on feared target", DamageSource.NoPets, 33.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.UnholyFervor, DamageModifierMode.All).WithBuilds(GW2Builds.August2018Balance, GW2Builds.February2020Balance),
            new BuffDamageModifierTarget(Fear, "Dread", "33% on feared target", DamageSource.NoPets, 33.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.UnholyFervor, DamageModifierMode.PvE).WithBuilds(GW2Builds.February2020Balance, GW2Builds.July2020Balance),
            new BuffDamageModifierTarget(Fear, "Dread", "15% on feared target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.UnholyFervor, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2020Balance, GW2Builds.July2020Balance),
            new DamageLogDamageModifier("Close to Death", "20% below 50% HP", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Necromancer, BuffImages.CloseToDeath, (x, log) => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All),
            // Soul Reaping
            new BuffDamageModifier(SoulBarbs, "Soul Barbs", "10% after entering or exiting shroud", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.SoulBarbs, DamageModifierMode.All).WithBuilds(GW2Builds.December2018Balance, GW2Builds.May2021Balance),
            new BuffDamageModifier(SoulBarbs, "Soul Barbs", "10% after entering or exiting shroud", DamageSource.NoPets, 10.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Necromancer, ByPresence, BuffImages.SoulBarbs, DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance),
            new BuffDamageModifier(new long[] { DeathShroud, ReapersShroud, HarbingerShroud }, "Death Perception", "15% crit damage while in shroud", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.DeathPerception, DamageModifierMode.All).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.June2022Balance), // no tracked for Scourge
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {     
            // Forms
            new Buff("Lich Form", LichForm, Source.Necromancer, BuffClassification.Other, BuffImages.LichForm),
            new Buff("Death Shroud", DeathShroud, Source.Necromancer, BuffClassification.Other, BuffImages.DeathShroud),
            // Signets
            new Buff("Signet of Vampirism", SignetOfVampirism, Source.Necromancer, BuffClassification.Other, BuffImages.SignetOfVampirism),
            new Buff("Vampiric Mark", VampiricMark, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.SignetOfVampirism),
            new Buff("Signet of Vampirism (Shroud)", SignetOfVampirismShroud, Source.Necromancer, BuffClassification.Other, BuffImages.SignetOfVampirism),
            new Buff("Plague Signet", PlagueSignetBuff, Source.Necromancer, BuffClassification.Other, BuffImages.PlagueSignet),
            new Buff("Plague Sending", PlagueSending, Source.Necromancer, BuffClassification.Other, BuffImages.PlagueSending),
            new Buff("Plague Signet (Shroud)", PlagueSignetShroud, Source.Necromancer, BuffClassification.Other, BuffImages.PlagueSignet),
            new Buff("Signet of Spite", SignetOfSpite, Source.Necromancer, BuffClassification.Other, BuffImages.SignetOfSpite),
            new Buff("Signet of Spite (Shroud)", SignetOfSpiteShroud, Source.Necromancer, BuffClassification.Other,BuffImages.SignetOfSpite),
            new Buff("Signet of the Locust", SignetOfTheLocust, Source.Necromancer, BuffClassification.Other, BuffImages.SignetOfTheLocust),
            new Buff("Signet of the Locust (Shroud)", SignetOfTheLocustShroud, Source.Necromancer, BuffClassification.Other, BuffImages.SignetOfTheLocust),
            new Buff("Signet of Undeath", SignetOfUndeath, Source.Necromancer, BuffClassification.Other, BuffImages.SignetOfUndeath),
            new Buff("Signet of Undeath (Shroud)", SignetOfUndeathShroud, Source.Necromancer, BuffClassification.Other, BuffImages.SignetOfUndeath),
            // Skills
            new Buff("Spectral Walk", SpectralWalkOldBuff, Source.Necromancer, BuffClassification.Other, BuffImages.NecroticTraversal).WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2018Balance),
            new Buff("Spectral Walk", SpectralWalkOldBuff, Source.Necromancer, BuffClassification.Other, BuffImages.SpectralWalk).WithBuilds(GW2Builds.July2018Balance, GW2Builds.December2018Balance),
            new Buff("Spectral Walk", SpectralWalkBuff, Source.Necromancer, BuffClassification.Other, BuffImages.SpectralWalk).WithBuilds(GW2Builds.December2018Balance, GW2Builds.EndOfLife),
            new Buff("Spectral Walk (Teleport)", SpectralWalkTeleportBuff, Source.Necromancer, BuffClassification.Other, BuffImages.SpectralWalk).WithBuilds(GW2Builds.December2018Balance, GW2Builds.EndOfLife),
            new Buff("Spectral Armor", SpectralArmorBuff, Source.Necromancer, BuffClassification.Other, BuffImages.SpectralArmor),
            new Buff("Locust Swarm", LocustSwarm, Source.Necromancer, BuffClassification.Other, BuffImages.LocustSwarm),
            new Buff("Grim Specter", GrimSpecterBuff, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.GrimSpecter),
            // Traits
            new Buff("Corrupter's Defense", CorruptersDefense, Source.Necromancer, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.CorruptersFervor).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2019Balance),
            new Buff("Death's Carapace", DeathsCarapace, Source.Necromancer, BuffStackType.Stacking, 30, BuffClassification.Other, BuffImages.DeathsCarapace).WithBuilds(GW2Builds.October2019Balance, GW2Builds.EndOfLife),
            new Buff("Flesh of the Master", FleshOfTheMaster, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.FleshOfTheMaster).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2019Balance),
            new Buff("Vampiric Aura", VampiricAura, Source.Necromancer, BuffClassification.Defensive, BuffImages.VampiricPresence),
            new Buff("Vampiric Strikes", VampiricStrikes, Source.Necromancer, BuffClassification.Other, BuffImages.VampiricPresence),
            new Buff("Last Rites", LastRites, Source.Necromancer, BuffClassification.Defensive, BuffImages.LastRites),
            new Buff("Soul Barbs", SoulBarbs, Source.Necromancer, BuffClassification.Other, BuffImages.SoulBarbs),
        };

        private static readonly HashSet<long> _shroudTransform = new HashSet<long>
        {
            EnterDeathShroud, ExitDeathShroud,
        };

        public static bool IsShroudTransform(long id)
        {
            return _shroudTransform.Contains(id) 
                || ReaperHelper.IsReaperShroudTransform(id) 
                || HarbingerHelper.IsHarbingerShroudTransform(id);
        }

        private static HashSet<int> Minions = new HashSet<int>()
        {
            (int)MinionID.BloodFiend,
            (int)MinionID.FleshGolem,
            (int)MinionID.ShadowFiend,
            (int)MinionID.FleshWurm,
            (int)MinionID.BoneFiend,
            (int)MinionID.BoneMinion,
            (int)MinionID.UnstableHorror,
        };
        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }
    }
}
