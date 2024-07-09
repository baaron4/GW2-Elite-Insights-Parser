using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
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
                .UsingChecker((evt, combatData, skillData, agentData) => !CombatData.FindRelatedEvents(combatData.GetBuffData(DesertShroudBuff).OfType<BuffRemoveAllEvent>(), evt.Time, 50).Any()) // collides with sandstorm shroud
                .UsingChecker((evt, combatData, skillData, agentData) => !combatData.HasRelatedHit(UnholyBurst, evt.Src, evt.Time))
                .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),

            new BuffGainCastFinder(SpectralArmorSkill, SpectralArmorBuff).WithBuilds(GW2Builds.December2018Balance),
            new BuffGainCastFinder(SpectralWalkSkill, SpectralWalkOldBuff).WithBuilds(GW2Builds.StartOfLife, GW2Builds.December2018Balance),
            new BuffGainCastFinder(SpectralWalkSkill, SpectralWalkBuff).WithBuilds(GW2Builds.December2018Balance),
            new BuffLossCastFinder(SpectralRecallSkill, SpectralWalkTeleportBuff)
                .UsingChecker((evt, combatData, skillData, agentData) => !CombatData.FindRelatedEvents(combatData.GetBuffData(SpectralWalkBuff).OfType<BuffRemoveAllEvent>(), evt.Time + 120).Any())
                .WithBuilds(GW2Builds.December2018Balance),
            new EffectCastFinderByDst(PlagueSignetSkill, EffectGUIDs.NecromancerPlagueSignet).UsingDstBaseSpecChecker(Spec.Necromancer),
            
            // Minions
            new MinionCommandCastFinder(RigorMortisSkill, (int) MinionID.BoneFiend),
            new MinionCommandCastFinder(HauntSkill, (int) MinionID.ShadowFiend),
            new MinionCommandCastFinder(NecroticTraversal, (int) MinionID.FleshWurm),
            // new BuffGainWithMinionsCastFinder(RigorMortisSkill, RigorMortisEffect),
            // new EffectCastFinder(NecroticTraversal, EffectGUIDs.NecromancerNecroticTraversal),
            // Spear
            new EffectCastFinder(DistressSkill, EffectGUIDs.NecromancerSpearDistress).UsingChecker((effectEvent, combatData, agentData, skillData) => CombatData.FindRelatedEvents(combatData.GetBuffRemoveAllData(DistressBuff).OfType<BuffRemoveAllEvent>(), effectEvent.Time).Any()),
        };

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            // Spite
            new BuffOnFoeDamageModifier(NumberOfBoons, "Spiteful Talisman", "10% on boonless target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByAbsence, BuffImages.SpitefulTalisman, DamageModifierMode.All),
            new BuffOnActorDamageModifier(Downed, "Death's Embrace", "25% on while downed", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.DeathsEmbrace, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2020Balance),
            new BuffOnActorDamageModifier(Downed, "Death's Embrace", "25% on while downed", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.DeathsEmbrace, DamageModifierMode.PvE).WithBuilds(GW2Builds.February2020Balance),
            new BuffOnActorDamageModifier(Downed, "Death's Embrace", "5% on while downed", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.DeathsEmbrace, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2020Balance),
            new BuffOnFoeDamageModifier(Fear, "Dread", "20% on feared target", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.UnholyFervor, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2018Balance),
            new BuffOnFoeDamageModifier(Fear, "Dread", "33% on feared target", DamageSource.NoPets, 33.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.UnholyFervor, DamageModifierMode.All).WithBuilds(GW2Builds.August2018Balance, GW2Builds.February2020Balance),
            new BuffOnFoeDamageModifier(Fear, "Dread", "33% on feared target", DamageSource.NoPets, 33.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.UnholyFervor, DamageModifierMode.PvE).WithBuilds(GW2Builds.February2020Balance, GW2Builds.July2020Balance),
            new BuffOnFoeDamageModifier(Fear, "Dread", "15% on feared target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.UnholyFervor, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2020Balance, GW2Builds.July2020Balance),
            new DamageLogDamageModifier("Close to Death", "20% below 50% HP", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Necromancer, BuffImages.CloseToDeath, (x, log) => x.AgainstUnderFifty, DamageModifierMode.All),
            // Soul Reaping
            new BuffOnActorDamageModifier(SoulBarbs, "Soul Barbs", "10% after entering or exiting shroud", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.SoulBarbs, DamageModifierMode.All).WithBuilds(GW2Builds.December2018Balance, GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(SoulBarbs, "Soul Barbs", "10% after entering or exiting shroud", DamageSource.NoPets, 10.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Necromancer, ByPresence, BuffImages.SoulBarbs, DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(new long[] { DeathShroud, ReapersShroud, HarbingerShroud }, "Death Perception", "15% crit damage while in shroud", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.DeathPerception, DamageModifierMode.All).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.June2022Balance), // no tracked for Scourge
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(DeathShroud, "Death Shroud", "-33%", DamageSource.NoPets, -33, DamageType.StrikeAndCondition, DamageType.All, Source.Necromancer, ByPresence, BuffImages.DeathShroud, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DeathShroud, "Death Shroud", "-50%", DamageSource.NoPets, -50, DamageType.StrikeAndCondition, DamageType.All, Source.Necromancer, ByPresence, BuffImages.DeathShroud, DamageModifierMode.sPvPWvW),
            new BuffOnActorDamageModifier(DeathsCarapace, "Beyond the Veil", "-10%", DamageSource.NoPets, -10, DamageType.Condition, DamageType.All, Source.Necromancer, ByPresence, BuffImages.BeyondTheVeil, DamageModifierMode.PvE)
                .UsingChecker((dl, log) => dl.To.GetBuffStatus(log, DeathsCarapace, dl.Time).Value >= 10)
                .WithBuilds(GW2Builds.October2019Balance),
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
            new Buff("Signet of Undeath", SignetOfUndeathBuff, Source.Necromancer, BuffClassification.Other, BuffImages.SignetOfUndeath),
            new Buff("Signet of Undeath (Shroud)", SignetOfUndeathShroud, Source.Necromancer, BuffClassification.Other, BuffImages.SignetOfUndeath),
            // Skills
            new Buff("Spectral Walk", SpectralWalkOldBuff, Source.Necromancer, BuffClassification.Other, BuffImages.NecroticTraversal).WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2018Balance),
            new Buff("Spectral Walk", SpectralWalkOldBuff, Source.Necromancer, BuffClassification.Other, BuffImages.SpectralWalk).WithBuilds(GW2Builds.July2018Balance, GW2Builds.December2018Balance),
            new Buff("Spectral Walk", SpectralWalkBuff, Source.Necromancer, BuffClassification.Other, BuffImages.SpectralWalk).WithBuilds(GW2Builds.December2018Balance),
            new Buff("Spectral Walk (Teleport)", SpectralWalkTeleportBuff, Source.Necromancer, BuffClassification.Other, BuffImages.SpectralWalk).WithBuilds(GW2Builds.December2018Balance),
            new Buff("Spectral Armor", SpectralArmorBuff, Source.Necromancer, BuffClassification.Other, BuffImages.SpectralArmor),
            new Buff("Locust Swarm", LocustSwarm, Source.Necromancer, BuffClassification.Other, BuffImages.LocustSwarm),
            new Buff("Grim Specter", GrimSpecterBuff, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.GrimSpecter),
            new Buff("Grim Specter (Target)", GrimSpecterTargetBuff, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.GrimSpecter),
            // Traits
            new Buff("Corrupter's Defense", CorruptersDefense, Source.Necromancer, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.CorruptersFervor).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2019Balance),
            new Buff("Death's Carapace", DeathsCarapace, Source.Necromancer, BuffStackType.Stacking, 30, BuffClassification.Other, BuffImages.DeathsCarapace).WithBuilds(GW2Builds.October2019Balance),
            new Buff("Flesh of the Master", FleshOfTheMaster, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.FleshOfTheMaster).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2019Balance),
            new Buff("Vampiric Aura", VampiricAura, Source.Necromancer, BuffClassification.Defensive, BuffImages.VampiricPresence),
            new Buff("Vampiric Strikes", VampiricStrikes, Source.Necromancer, BuffClassification.Other, BuffImages.VampiricPresence),
            new Buff("Last Rites", LastRites, Source.Necromancer, BuffClassification.Defensive, BuffImages.LastRites),
            new Buff("Soul Barbs", SoulBarbs, Source.Necromancer, BuffClassification.Other, BuffImages.SoulBarbs),
            // Spear
            new Buff("Extirpation", Extirpation, Source.Necromancer, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Soul Shards", SoulShards, Source.Necromancer, BuffStackType.StackingConditionalLoss, 6, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Distress", DistressBuff, Source.Necromancer, BuffClassification.Other, BuffImages.MonsterSkill),
            new Buff("Dark Stalker", DarkStalker, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.MonsterSkill),
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

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Necromancer;

            // Well of Blood
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerWellOfBlood, out IReadOnlyList<EffectEvent> wellOfBloods))
            {
                var skill = new SkillModeDescriptor(player, Spec.Necromancer, WellOfBlood, SkillModeCategory.Heal);
                foreach (EffectEvent effect in wellOfBloods)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectWellOfBlood);
                }
            }
            // Well of Suffering
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerWellOfSuffering, out IReadOnlyList<EffectEvent> wellOfSufferings))
            {
                var skill = new SkillModeDescriptor(player, Spec.Necromancer, WellOfSuffering);
                foreach (EffectEvent effect in wellOfSufferings)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 6000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectWellOfSuffering);
                }
            }
            // Well of Darkness
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerWellOfDarkness, out IReadOnlyList<EffectEvent> wellOfDarknesses))
            {
                var skill = new SkillModeDescriptor(player, Spec.Necromancer, WellOfDarkness);
                foreach (EffectEvent effect in wellOfDarknesses)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectWellOfDarkness);
                }
            }
            // Well of Corruption
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerWellOfCorruption, out IReadOnlyList<EffectEvent> wellOfCorruptions))
            {
                var skill = new SkillModeDescriptor(player, Spec.Necromancer, WellOfCorruption);
                foreach (EffectEvent effect in wellOfCorruptions)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectWellOfCorruption);
                }
            }
            // Corrosive Poison Cloud
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerCorrosivePoisonCloud, out IReadOnlyList<EffectEvent> poisonClouds))
            {
                var skill = new SkillModeDescriptor(player, Spec.Necromancer, CorrosivePoisonCloud, SkillModeCategory.ProjectileManagement);
                foreach (EffectEvent effect in poisonClouds)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 8000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectCorrosivePoisonCloud);
                }
            }
            // Plaguelands
            if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, new string[] {
                            EffectGUIDs.NecromancerPlaguelandsPulse1,
                            EffectGUIDs.NecromancerPlaguelandsPulse2,EffectGUIDs.NecromancerPlaguelandsPulse3
                            }, out IReadOnlyList<EffectEvent> plaguelandPulses))
            {
                var skill = new SkillModeDescriptor(player, Spec.Necromancer, Plaguelands);
                foreach (EffectEvent effect in plaguelandPulses)
                {
                    int start = (int)effect.Time - 500;
                    int end = (int)effect.Time;
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, (start, end), color, 0.5, connector).UsingGrowingEnd(end).UsingSkillMode(skill));
                }
            }
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerPlaguelands, out IReadOnlyList<EffectEvent> plaguelands))
            {
                var skill = new SkillModeDescriptor(player, Spec.Necromancer, Plaguelands);
                foreach (EffectEvent effect in plaguelands)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 10000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectPlaguelands, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }

            // Mark of Blood or Chillblains (Staff 2/3)
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerMarkOfBloodOrChillblains, out IReadOnlyList<EffectEvent> markOfBloodOrChillblains))
            {
                var markCasts = player.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.SkillId == MarkOfBlood || x.SkillId == Chillblains || x.Skill.IsDodge(log.SkillData)).ToList();
                foreach (EffectEvent effect in markOfBloodOrChillblains)
                {
                    SkillModeDescriptor skill;
                    string icon;
                    bool fromDodge = false;
                    var markCastsOnEffect = markCasts.Where(x => effect.Time - ServerDelayConstant > x.Time && x.EndTime > effect.Time + ServerDelayConstant).ToList();
                    if (markCastsOnEffect.Count == 1)
                    {
                        skill = new SkillModeDescriptor(player, Spec.Necromancer, markCastsOnEffect.FirstOrDefault().SkillId);
                        if (skill.SkillID != MarkOfBlood && skill.SkillID != Chillblains)
                        {
                            fromDodge = true;
                            skill = new SkillModeDescriptor(player, Spec.Necromancer, MarkOfBlood);
                        }
                        icon = skill.SkillID == Chillblains ? ParserIcons.EffectChillblains : ParserIcons.EffectMarkOfBlood;
                    }
                    else
                    {
                        skill = new SkillModeDescriptor(player, Spec.Necromancer, MarkOfBloodOrChillblains);
                        icon = ParserIcons.EffectMarkOfBloodOrChillblains;
                    }
                    (long, long) lifespan = effect.ComputeDynamicLifespan(log, fromDodge ? 6000 : 30000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, icon);
                }
            }
            // Mark of Blood Activated (Staff 2)
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerMarkOfBloodActivated1, out IReadOnlyList<EffectEvent> marksOfBloodActivated))
            {
                var skill = new SkillModeDescriptor(player, Spec.Necromancer, MarkOfBlood);
                foreach (EffectEvent effect in marksOfBloodActivated)
                {
                    (long, long) lifespan = ((int)effect.Time, (int)effect.Time + 500);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 300, ParserIcons.EffectMarkOfBlood);
                }
            }
            // Chillblains Activated (Staff 3)
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerChillblainsActivated, out IReadOnlyList<EffectEvent> chillblainsActivated))
            {
                var skill = new SkillModeDescriptor(player, Spec.Necromancer, Chillblains);
                foreach (EffectEvent effect in chillblainsActivated)
                {
                    (long, long) lifespan = ((int)effect.Time, (int)effect.Time + 500);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 300, ParserIcons.EffectChillblains);
                }
            }

            // Putrid Mark (Staff 4)
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerPutridMark, out IReadOnlyList<EffectEvent> putridMarks))
            {
                var skill = new SkillModeDescriptor(player, Spec.Necromancer, PutridMark);
                foreach (EffectEvent effect in putridMarks)
                {
                    (long, long) lifespan = effect.ComputeDynamicLifespan(log, 30000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectPutridMark);
                }
            }
            // Putrid Mark (Staff 4) Activated
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerPutridMarkActivated1, out IReadOnlyList<EffectEvent> putridMarksActivated))
            {
                var skill = new SkillModeDescriptor(player, Spec.Necromancer, PutridMark);
                foreach (EffectEvent effect in putridMarksActivated)
                {
                    (long, long) lifespan = ((int)effect.Time, (int)effect.Time + 500);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 300, ParserIcons.EffectPutridMark);
                }
            }

            // Reaper's Mark (Staff 5)
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerReapersMark, out IReadOnlyList<EffectEvent> reapersMarks))
            {
                var skill = new SkillModeDescriptor(player, Spec.Necromancer, ReapersMark);
                foreach (EffectEvent effect in reapersMarks)
                {
                    (long, long) lifespan = effect.ComputeDynamicLifespan(log, 30000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectReapersMark);
                }
            }
            // Reaper's Mark (Staff 5) Activated
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerReapersMarkActivated, out IReadOnlyList<EffectEvent> reapersMarksActivated))
            {
                var skill = new SkillModeDescriptor(player, Spec.Necromancer, ReapersMark, SkillModeCategory.CC);
                foreach (EffectEvent effect in reapersMarksActivated)
                {
                    (long, long) lifespan = ((int)effect.Time, (int)effect.Time + 500);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 300, ParserIcons.EffectReapersMark);
                }
            }

            // Signet of Undeath
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerSignetOfUndeathGroundMark, out IReadOnlyList<EffectEvent> signetOfUndeath))
            {
                var skill = new SkillModeDescriptor(player, Spec.Necromancer, SignetOfUndeathSkill, SkillModeCategory.Heal);
                foreach (EffectEvent effect in signetOfUndeath)
                {
                    (long, long) lifespan = ((int)effect.Time, (int)effect.Time + 500);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, ParserIcons.EffectSignetOfUndeath);
                }
            }

            // Spectral Ring
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerSpectralRing, out IReadOnlyList<EffectEvent> spectralRings))
            {
                var skill = new SkillModeDescriptor(player, Spec.Necromancer, SpectralRing, SkillModeCategory.CC);
                foreach (EffectEvent effect in spectralRings)
                {
                    long duration = log.FightData.Logic.SkillMode == EncounterLogic.FightLogic.SkillModeEnum.WvW ? 5000 : 8000;
                    (long, long) lifespan = effect.ComputeDynamicLifespan(log, duration);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new DoughnutDecoration(180, 200, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectSpectralRing, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
        }
    }
}
