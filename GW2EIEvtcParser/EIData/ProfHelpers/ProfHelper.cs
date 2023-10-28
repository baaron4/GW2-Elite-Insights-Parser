using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;

namespace GW2EIEvtcParser.EIData
{
    internal static class ProfHelper
    {

        private static readonly List<InstantCastFinder> _genericInstantCastFinders = new List<InstantCastFinder>()
        {
            new BreakbarDamageCastFinder(Technobabble, Technobabble),
            new DamageCastFinder(SigilOfEarth, SigilOfEarth)
                .UsingICD(500)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new DamageCastFinder(LightningStrikeSigil, LightningStrikeSigil)
                .UsingICD(500)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new DamageCastFinder(FlameBlastSigil, FlameBlastSigil)
                .UsingICD(500)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new DamageCastFinder(SigilOfHydromancy, SigilOfHydromancy)
                .UsingICD(500)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(WaterBlastCombo1, WaterBlastCombo1),
            new EXTHealingCastFinder(WaterLeapCombo, WaterLeapCombo),
            new EffectCastFinderByDst(RuneOfNightmare, EffectGUIDs.RuneOfNightmare)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new BuffGainCastFinder(PortalEntranceWhiteMantleWatchwork, PortalWeavingWhiteMantleWatchwork),
            new BuffGainCastFinder(PortalExitWhiteMantleWatchwork, PortalUsesWhiteMantleWatchwork)
                .UsingBeforeWeaponSwap(true),
            // Relics
            new BuffGainCastFinder(RelicOfVass, RelicOfVass)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new BuffGainCastFinder(RelicOfTheFirebrand, RelicOfTheFirebrand)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            //new BuffGainCastFinder(RelicOfIsgarrenTargetBuff, RelicTargetPlayerBuff).UsingChecker((bae, combatData, agentData, skillData) =>
            //{
            //    return combatData.GetBuffData(RelicOfIsgarrenTargetBuff).Where(x => x.CreditedBy == bae.To && Math.Abs(x.Time - bae.Time) < ServerDelayConstant).Any();
            //}).UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            //new BuffGainCastFinder(RelicOfTheDragonhunterTargetBuff, RelicTargetPlayerBuff).UsingChecker((bae, combatData, agentData, skillData) =>
            //{
            //    return combatData.GetBuffData(RelicOfTheDragonhunterTargetBuff).Where(x => x.CreditedBy == bae.To && Math.Abs(x.Time - bae.Time) < ServerDelayConstant).Any();
            //}).UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new BuffLossCastFinder(RelicOfFireworksBuffLoss, RelicOfFireworks)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EffectCastFinder(RelicOfCerusHit, EffectGUIDs.RelicOfCerusEye)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EffectCastFinder(RelicOfTheIce, EffectGUIDs.RelicOfIce)
                .UsingICD(1000)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EffectCastFinder(RelicOfFireworks, EffectGUIDs.RelicOfFireworks)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EffectCastFinder(RelicOfPeithaTargetBuff, EffectGUIDs.RelicOfPeitha)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            //new EffectCastFinder(RelicOfTheCitadel, EffectGUIDs.RelicWhiteCircle).UsingChecker((evt, combatData, agentData, skillData) =>
            //{
            //    combatData.TryGetEffectEventsByGUID(EffectGUIDs.RelicOfTheCitadelExplosion, out IReadOnlyList<EffectEvent> effects);
            //    return effects != null && effects.Any(x => x.Time > evt.Time);
            //}).UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            //new EffectCastFinder(RelicOfTheNightmare, EffectGUIDs.RelicWhiteCircle).UsingChecker((evt, combatData, agentData, skillData) =>
            //{
            //    combatData.TryGetEffectEventsByGUID(EffectGUIDs.RelicOfTheNightmare, out IReadOnlyList<EffectEvent> effects);
            //    return effects != null && effects.Any(x => x.Time > evt.Time);
            //}).UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            //new EffectCastFinder(RelicOfTheKrait, EffectGUIDs.RelicWhiteCircle).UsingChecker((evt, combatData, agentData, skillData) =>
            //{
            //    combatData.TryGetEffectEventsByGUID(EffectGUIDs.RelicOfTheKrait, out IReadOnlyList<EffectEvent> effects);
            //    return effects != null && effects.Any(x => x.Time > evt.Time);
            //}).UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EffectCastFinder(RelicOfTheWizardsTower, EffectGUIDs.RelicWhiteCircle)
                .UsingSecondaryEffectChecker(EffectGUIDs.RelicOfTheWizardsTower)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            // Mounts
            new BuffGainCastFinder(BondOfLifeSkill, BondOfLifeBuff),
            new BuffGainCastFinder(BondOfVigorSkill, BondOfVigorBuff),
            new BuffGainCastFinder(BondOfFaithSkill, EvasionBondOfFaith)
                .UsingBeforeWeaponSwap(true),
            new BuffGainCastFinder(Stealth2Skill, StealthMountBuff),
            // Skyscale
            new EffectCastFinderByDst(SkyscaleSkill, EffectGUIDs.SkyscaleLaunch),
            new EffectCastFinder(SkyscaleFireballSkill, EffectGUIDs.SkyscaleFireball),
            new EffectCastFinder(SkyscaleBlastSkill, EffectGUIDs.SkyscaleBlast1)
                .UsingSecondaryEffectChecker(EffectGUIDs.SkyscaleBlast2),
        };

        internal static void AttachMasterToGadgetByCastData(CombatData combatData, IReadOnlyCollection<AgentItem> gadgets, IReadOnlyList<long> castIDS, long castEndThreshold)
        {
            var possibleCandidates = new HashSet<AgentItem>();
            var gadgetSpawnCastData = new List<AnimatedCastEvent>();
            foreach (long id in castIDS)
            {
                gadgetSpawnCastData.AddRange(combatData.GetAnimatedCastData(id));
            }
            gadgetSpawnCastData.Sort((x, y) => x.Time.CompareTo(y.Time));
            foreach (AbstractCastEvent castEvent in gadgetSpawnCastData)
            {
                long start = castEvent.Time;
                long end = castEvent.EndTime;
                possibleCandidates.Add(castEvent.Caster);
                foreach (AgentItem gadget in gadgets)
                {
                    if (gadget.FirstAware >= start && gadget.FirstAware <= end + castEndThreshold)
                    {
                        // more than one candidate, put to unknown and drop the search
                        if (gadget.Master != null && gadget.GetFinalMaster() != castEvent.Caster.GetFinalMaster())
                        {
                            gadget.SetMaster(_unknownAgent);
                            break;
                        }
                        gadget.SetMaster(castEvent.Caster.GetFinalMaster());
                    }
                }
            }
            if (possibleCandidates.Count == 1)
            {
                foreach (AgentItem gadget in gadgets)
                {
                    if (gadget.Master == null)
                    {
                        gadget.SetMaster(possibleCandidates.First().GetFinalMaster());
                    }
                }
            }
        }

        internal static HashSet<AgentItem> GetOffensiveGadgetAgents(CombatData combatData, long damageSkillID, IReadOnlyCollection<AgentItem> playerAgents)
        {
            var res = new HashSet<AgentItem>();
            foreach (AbstractHealthDamageEvent evt in combatData.GetDamageData(damageSkillID))
            {
                // dst must no be a gadget nor a friendly player
                // src must be a masterless gadget
                if (!playerAgents.Contains(evt.To.GetFinalMaster()) && evt.To.Type != AgentItem.AgentType.Gadget && evt.From.Type == AgentItem.AgentType.Gadget && evt.From.Master == null)
                {
                    res.Add(evt.From);
                }
            }
            return res;
        }

        internal static void SetGadgetMaster(IReadOnlyCollection<AgentItem> gadgets, AgentItem master)
        {
            foreach (AgentItem gadget in gadgets)
            {
                gadget.SetMaster(master);
            }
        }

        internal static void ProcessRacialGadgets(IReadOnlyList<Player> players, CombatData combatData)
        {
            var playerAgents = new HashSet<AgentItem>(players.Select(x => x.AgentItem));
            // Sylvari stuff
            HashSet<AgentItem> seedTurrets = GetOffensiveGadgetAgents(combatData, SeedTurretDamage, playerAgents);
            HashSet<AgentItem> graspingWines = GetOffensiveGadgetAgents(combatData, GraspingVinesDamage, playerAgents);
            AttachMasterToGadgetByCastData(combatData, seedTurrets, new List<long> { SeedTurret, TakeRootSkill }, 1000);
            AttachMasterToGadgetByCastData(combatData, graspingWines, new List<long> { GraspingVines }, 1000);
            // melandru avatar works fine already
        }

        //
        internal static IReadOnlyCollection<InstantCastFinder> GetProfessionInstantCastFinders(IReadOnlyList<Player> players)
        {
            var instantCastFinders = new HashSet<InstantCastFinder>(_genericInstantCastFinders);
            foreach (Player p in players)
            {
                switch (p.Spec)
                {
                    //
                    case Spec.Elementalist:
                        ElementalistHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Tempest:
                        ElementalistHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        TempestHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Weaver:
                        ElementalistHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        WeaverHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Catalyst:
                        ElementalistHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        CatalystHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case Spec.Necromancer:
                        NecromancerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Reaper:
                        NecromancerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        ReaperHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Scourge:
                        NecromancerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        ScourgeHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Harbinger:
                        NecromancerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        HarbingerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case Spec.Mesmer:
                        MesmerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Chronomancer:
                        MesmerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        ChronomancerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Mirage:
                        MesmerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        MirageHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Virtuoso:
                        MesmerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        VirtuosoHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case Spec.Thief:
                        ThiefHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Daredevil:
                        ThiefHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        DaredevilHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Deadeye:
                        ThiefHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        DeadeyeHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Specter:
                        ThiefHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        SpecterHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case Spec.Engineer:
                        EngineerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Scrapper:
                        EngineerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        ScrapperHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Holosmith:
                        EngineerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        HolosmithHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Mechanist:
                        EngineerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        MechanistHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case Spec.Ranger:
                        RangerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Druid:
                        RangerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        DruidHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Soulbeast:
                        RangerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        SoulbeastHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Untamed:
                        RangerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        UntamedHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case Spec.Revenant:
                        RevenantHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Herald:
                        RevenantHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        HeraldHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Renegade:
                        RevenantHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        RenegadeHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Vindicator:
                        RevenantHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        VindicatorHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case Spec.Guardian:
                        GuardianHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Dragonhunter:
                        GuardianHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        DragonhunterHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Firebrand:
                        GuardianHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        FirebrandHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Willbender:
                        GuardianHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        WillbenderHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case Spec.Warrior:
                        WarriorHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Berserker:
                        WarriorHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        BerserkerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Spellbreaker:
                        WarriorHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        SpellbreakerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Bladesworn:
                        WarriorHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        BladeswornHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                }
            }
            return instantCastFinders;
        }

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Blue;

            // White Mantle Portal Device portal locations
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.WhiteMantlePortalInactive, out IReadOnlyList<EffectEvent> whiteMantlePortalInactive))
            {
                var skill = new SkillModeDescriptor(player, PortalEntranceWhiteMantleWatchwork);
                foreach (EffectEvent effect in whiteMantlePortalInactive)
                {
                    (long, long) lifespan = ComputeEffectLifespan(log, effect, 60000, player.AgentItem, PortalWeavingWhiteMantleWatchwork);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(90, lifespan, color.WithAlpha(0.2f).ToString(), connector).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.PortalWhiteMantleSkill, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
            if (log.CombatData.TryGetGroupedEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.WhiteMantlePortalActive, out IReadOnlyList<IReadOnlyList<EffectEvent>> whiteMantlePortalActive))
            {
                var skill = new SkillModeDescriptor(player, PortalExitWhiteMantleWatchwork, SkillModeCategory.Portal);
                foreach (IReadOnlyList<EffectEvent> group in whiteMantlePortalActive)
                {
                    GenericAttachedDecoration first = null;
                    foreach (EffectEvent effect in group)
                    {
                        (long, long) lifespan = ComputeEffectLifespan(log, effect, 10000, player.AgentItem, PortalUsesWhiteMantleWatchwork);
                        var connector = new PositionConnector(effect.Position);
                        replay.Decorations.Add(new CircleDecoration(90, lifespan, color.WithAlpha(0.3f).ToString(), connector).UsingSkillMode(skill));
                        GenericAttachedDecoration decoration = new IconDecoration(ParserIcons.PortalWhiteMantleSkill, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.7f, lifespan, connector).UsingSkillMode(skill);
                        if (first == null)
                        {
                            first = decoration;
                        }
                        else
                        {
                            replay.Decorations.Add(first.LineTo(decoration, color.WithAlpha(0.3f).ToString()).UsingSkillMode(skill));
                        }
                        replay.Decorations.Add(decoration);
                    }
                }
            }


            switch (player.Spec)
            {
                // Elementalist
                case Spec.Elementalist:
                    ElementalistHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Tempest:
                    ElementalistHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    TempestHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Weaver:
                    ElementalistHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Catalyst:
                    ElementalistHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    CatalystHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                // Engineer
                case Spec.Engineer:
                    EngineerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Scrapper:
                    EngineerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    ScrapperHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Holosmith:
                case Spec.Mechanist:
                    EngineerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                // Guardian
                case Spec.Guardian:
                case Spec.Dragonhunter:
                    GuardianHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Firebrand:
                    GuardianHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    FirebrandHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Willbender:
                    GuardianHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                // Mesmer
                case Spec.Mesmer:
                    MesmerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Chronomancer:
                    MesmerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    ChronomancerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Mirage:
                    MesmerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Virtuoso:
                    MesmerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    VirtuosoHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                // Necromancer
                case Spec.Necromancer:
                case Spec.Reaper:
                    NecromancerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Scourge:
                    NecromancerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    ScourgeHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Harbinger:
                    NecromancerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    HarbingerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                // Ranger
                case Spec.Ranger:
                case Spec.Druid:
                case Spec.Soulbeast:
                case Spec.Untamed:
                    RangerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                // Revenant
                case Spec.Revenant:
                case Spec.Herald:
                case Spec.Renegade:
                case Spec.Vindicator:
                    RevenantHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                // Thief
                case Spec.Thief:
                case Spec.Daredevil:
                case Spec.Deadeye:
                    ThiefHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Specter:
                    ThiefHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    SpecterHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                // Warrior
                case Spec.Warrior:
                case Spec.Berserker:
                    break;
                case Spec.Spellbreaker:
                    SpellbreakerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Bladesworn:
                    break;
                default:
                    break;
            }
        }

        internal static void DEBUG_ComputeProfessionCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            var knownEffects = new HashSet<long>();
            CombatReplay.DebugEffects(p, log, replay, knownEffects);
        }

        private static readonly HashSet<Spec> _canSummonClones = new HashSet<Spec>()
        {
            Spec.Mesmer,
            Spec.Chronomancer,
            Spec.Mirage
        };

        internal static bool CanSummonClones(Spec spec)
        {
            return _canSummonClones.Contains(spec);
        }

        /// <summary>
        /// Minions that aren't profession-specific bound.
        /// </summary>
        private static readonly HashSet<int> CommonMinions = new HashSet<int>()
        {
            // Racial Summons
            (int)ArcDPSEnums.MinionID.HoundOfBalthazar,
            (int)ArcDPSEnums.MinionID.SnowWurm,
            (int)ArcDPSEnums.MinionID.DruidSpirit,
            (int)ArcDPSEnums.MinionID.SylvanHound,
            (int)ArcDPSEnums.MinionID.IronLegionSoldier,
            (int)ArcDPSEnums.MinionID.IronLegionMarksman,
            (int)ArcDPSEnums.MinionID.BloodLegionSoldier,
            (int)ArcDPSEnums.MinionID.BloodLegionMarksman,
            (int)ArcDPSEnums.MinionID.AshLegionSoldier,
            (int)ArcDPSEnums.MinionID.AshLegionMarksman,
            (int)ArcDPSEnums.MinionID.STAD007,
            (int)ArcDPSEnums.MinionID.STA7012,
            // GW2 Digital Deluxe
            (int)ArcDPSEnums.MinionID.MistfireWolf,
            // Rune Summons
            (int)ArcDPSEnums.MinionID.RuneJaggedHorror,
            (int)ArcDPSEnums.MinionID.RuneRockDog,
            (int)ArcDPSEnums.MinionID.RuneMarkIGolem,
            (int)ArcDPSEnums.MinionID.RuneTropicalBird,
            // Consumables with summons
            (int)ArcDPSEnums.MinionID.Ember,
            (int)ArcDPSEnums.MinionID.HawkeyeGriffon,
            (int)ArcDPSEnums.MinionID.SousChef,
            (int)ArcDPSEnums.MinionID.SunspearParagonSupport,
            (int)ArcDPSEnums.MinionID.RavenSpiritShadow,
        };

        internal static bool IsKnownMinionID(int id)
        {
            return CommonMinions.Contains(id);
        }

        public static void ComputeMinionCombatReplayActors(AbstractSingleActor minion, AbstractSingleActor master, ParsedEvtcLog log, CombatReplay combatReplay)
        {

        }

        public static void AdjustMinionName(AgentItem minion)
        {
            switch (minion.GetFinalMaster().BaseSpec)
            {
                case Spec.Mesmer:
                    MesmerHelper.AdjustMinionName(minion);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Computes the end time of an effect.
        /// <br/>
        /// When no end event is present, it falls back to buff remove all of associated buff (if passed) first.
        /// Afterwards the effect duration is used, if greater 0 and less than max duration.
        /// Finally, it defaults to max duration.
        /// </summary>
        internal static long ComputeEffectEndTime(ParsedEvtcLog log,EffectEvent effect, long maxDuration, AgentItem agent = null, long? associatedBuff = null)
        {
            if (log.CombatData.TryGetEffectEndByTrackingId(effect.TrackingID, effect.Time, out long end))
            {
                return end;
            }
            if (associatedBuff != null)
            {
                BuffRemoveAllEvent remove = log.CombatData.GetBuffData(associatedBuff.Value)
                    .OfType<BuffRemoveAllEvent>()
                    .FirstOrDefault(x => x.To == agent && x.Time >= effect.Time);
                if (remove != null)
                {
                    return remove.Time;
                }
            }
            if (effect.Duration > 0 && effect.Duration <= maxDuration)
            {
                return effect.Time + effect.Duration;
            }
            return effect.Time + maxDuration;
        }

        /// <summary>
        /// Computes the lifespan of an effect.
        /// Will use default duration if all other methods fail
        /// See <see cref="ComputeEffectEndTime"/> for information about computed end times.
        /// </summary>
        internal static (long, long) ComputeEffectLifespan(ParsedEvtcLog log, EffectEvent effect, long defaultDuration, AgentItem agent = null, long? associatedBuff = null)
        {
            long start = effect.Time;
            long end = ComputeEffectEndTime(log, effect, defaultDuration, agent, associatedBuff);
            return (start, end);
        }

        /// <summary>
        /// Computes the lifespan of an effect.
        /// Will default to 0 duration if all other methods fail.
        /// This method is to be used when the duration of the effect is not static (ex: a trap AoE getting triggered or when a trait can modify the duration).
        /// See <see cref="ComputeEffectEndTime"/> for information about computed end times.
        /// </summary>
        internal static (long, long) ComputeDynamicEffectLifespan(ParsedEvtcLog log, EffectEvent effect, long defaultDuration, AgentItem agent = null, long? associatedBuff = null)
        {
            long durationToUse = defaultDuration;
            if (!(effect is EffectEventCBTS51))
            {
                durationToUse = 0;
            }
            long start = effect.Time;
            long end = ComputeEffectEndTime(log, effect, durationToUse, agent, associatedBuff);
            return (start, end);
        }
    }
}
