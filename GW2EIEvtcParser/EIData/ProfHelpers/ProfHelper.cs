using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class ProfHelper
    {

        private static readonly List<InstantCastFinder> _genericInstantCastFinders = new List<InstantCastFinder>()
        {
            // Sigils
            new DamageCastFinder(RingOfEarth_MinorSigilOfGeomancy, RingOfEarth_MinorSigilOfGeomancy)
                .UsingICD(500)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new DamageCastFinder(RingOfEarth_MajorSigilOfGeomancy, RingOfEarth_MajorSigilOfGeomancy)
                .UsingICD(500)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new DamageCastFinder(RingOfEarth_SuperiorSigilOfGeomancy, RingOfEarth_SuperiorSigilOfGeomancy)
                .UsingICD(500)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new DamageCastFinder(LightningStrike_SigilOfAir, LightningStrike_SigilOfAir)
                .UsingICD(500)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new DamageCastFinder(FlameBlast_SigilOfFire, FlameBlast_SigilOfFire)
                .UsingICD(500)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new DamageCastFinder(Snowball_SigilOfMischief, Snowball_SigilOfMischief)
                .UsingICD(500)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new DamageCastFinder(FrostBurst_MinorSigilOfHydromancy, FrostBurst_MinorSigilOfHydromancy)
                .UsingICD(500)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new DamageCastFinder(FrostBurst_MajorSigilOfHydromancy, FrostBurst_MajorSigilOfHydromancy)
                .UsingICD(500)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new DamageCastFinder(FrostBurst_SuperiorSigilOfHydromancy, FrostBurst_SuperiorSigilOfHydromancy)
                .UsingICD(500)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(SuperiorSigilOfDraining, SuperiorSigilOfDraining)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new BuffGainCastFinder(SuperiorSigilOfSeverance, SuperiorSigilOfSeverance)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new BuffGainCastFinder(MinorSigilOfDoom, MinorSigilOfDoom)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new BuffGainCastFinder(MajorSigilOfDoom, MajorSigilOfDoom)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new BuffGainCastFinder(SuperiorSigilOfDoom, SuperiorSigilOfDoom)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new BuffGainCastFinder(MajorSigilOfLeeching, MajorSigilOfLeeching)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new BuffGainCastFinder(SuperiorSigilOfLeeching, LeechBuff)
                .WithBuilds(ArcDPSEnums.GW2Builds.November2018Rune)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new BuffGainCastFinder(SuperiorSigilOfVision, SuperiorSigilOfVision)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new BuffGainCastFinder(SuperiorSigilOfConcentration, SuperiorSigilOfConcentration)
                .WithBuilds(ArcDPSEnums.GW2Builds.StartOfLife, ArcDPSEnums.GW2Builds.November2018Rune)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(WaveOfHealing_MinorSigilOfWater, WaveOfHealing_MinorSigilOfWater)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(WaveOfHealing_MajorSigilOfWater, WaveOfHealing_MajorSigilOfWater)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(WaveOfHealing_SuperiorSigilOfWater, WaveOfHealing_SuperiorSigilOfWater)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(WaveOfHealing_MinorSigilOfRenewal, WaveOfHealing_MinorSigilOfRenewal)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(WaveOfHealing_MajorSigilOfRenewal, WaveOfHealing_MajorSigilOfRenewal)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(WaveOfHealing_SuperiorSigilOfRenewal, WaveOfHealing_SuperiorSigilOfRenewal)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(MajorSigilOfRestoration, MajorSigilOfRestoration)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(SuperiorSigilOfRestoration, SuperiorSigilOfRestoration)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(MinorSigilOfBlood, MinorSigilOfBlood)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(MajorSigilOfBlood, MajorSigilOfBlood)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(SuperiorSigilOfBlood, SuperiorSigilOfBlood)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            // Runes
            new EffectCastFinderByDst(RuneOfNightmare, EffectGUIDs.RuneOfNightmare)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            // Combos
            new EXTHealingCastFinder(WaterBlastCombo1, WaterBlastCombo1),
            new EXTHealingCastFinder(WaterBlastCombo2, WaterBlastCombo2),
            new EXTHealingCastFinder(WaterLeapCombo, WaterLeapCombo),
            new BreakbarDamageCastFinder(LightningLeapCombo, LightningLeapCombo),
            // Misc
            new BuffGainCastFinder(PortalEntranceWhiteMantleWatchwork, PortalWeavingWhiteMantleWatchwork),
            new BuffGainCastFinder(PortalExitWhiteMantleWatchwork, PortalUsesWhiteMantleWatchwork)
                .UsingBeforeWeaponSwap(true),
            new BreakbarDamageCastFinder(Technobabble, Technobabble),
            // Relics
            new BuffGainCastFinder(RelicOfVass, RelicOfVass)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new BuffGainCastFinder(RelicOfTheFirebrand, RelicOfTheFirebrand)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new BuffGainCastFinder(NouryssHungerDamageBuff, NouryssHungerDamageBuff)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new BuffGainCastFinder(MabonsStrength, MabonsStrength)
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
            new EffectCastFinder(RelicOfPeithaBlade, EffectGUIDs.RelicOfPeitha)
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
            new EffectCastFinder(RelicOfTheTwinGenerals, EffectGUIDs.RelicOfTheTwinGenerals)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(RelicOfKarakosaHealing, RelicOfKarakosaHealing)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(RelicOfNayosHealing, RelicOfNayosHealing)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(RelicOfTheDefenderHealing, RelicOfTheDefenderHealing)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTHealingCastFinder(RelicOfTheFlock, RelicOfTheFlock)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTBarrierCastFinder(RelicOfTheFlockBarrier, RelicOfTheFlockBarrier)
                .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
            new EXTBarrierCastFinder(RelicOfTheFoundingBarrier, RelicOfTheFoundingBarrier)
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
                    (long, long) lifespan = effect.ComputeLifespan(log, 60000, player.AgentItem, PortalWeavingWhiteMantleWatchwork);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(90, lifespan, color, 0.2, connector).UsingSkillMode(skill));
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
                        (long, long) lifespan = effect.ComputeLifespan(log, 10000, player.AgentItem, PortalUsesWhiteMantleWatchwork);
                        var connector = new PositionConnector(effect.Position);
                        replay.Decorations.Add(new CircleDecoration(90, lifespan, color, 0.3, connector).UsingSkillMode(skill));
                        GenericAttachedDecoration decoration = new IconDecoration(ParserIcons.PortalWhiteMantleSkill, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.7f, lifespan, connector).UsingSkillMode(skill);
                        if (first == null)
                        {
                            first = decoration;
                        }
                        else
                        {
                            replay.Decorations.Add(first.LineTo(decoration, color, 0.3).UsingSkillMode(skill));
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
                    RangerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Druid:
                    RangerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    DruidHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Soulbeast:
                    RangerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Untamed:
                    RangerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                // Revenant
                case Spec.Revenant:
                case Spec.Herald:
                    RevenantHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
                case Spec.Renegade:
                    RevenantHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    RenegadeHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                    break;
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

        private static readonly HashSet<Spec> _canUseRangerPets = new HashSet<Spec>()
        {
            Spec.Ranger,
            Spec.Druid,
            Spec.Soulbeast,
            Spec.Untamed,
        };

        internal static bool CanUseRangerPets(Spec spec)
        {
            return _canUseRangerPets.Contains(spec);
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


        public delegate bool EffectCastEventsChecker(IReadOnlyList<EffectEvent> effects, EffectEvent effect, CombatData combatData, SkillData skillData);

        /// <summary>
        /// Computes AnimatedCastEvents based on provided effectGUID
        /// </summary>
        /// <param name="actor">actor who is the source of the effect</param>
        /// <param name="combatData"></param>
        /// <param name="skillData"></param>
        /// <param name="skillID"></param>
        /// <param name="effectGUID"></param>
        /// <param name="startOffset">offset to be applied to the time value of the effect</param>
        /// <param name="castDuration"></param>
        /// <returns></returns>
        public static IReadOnlyList<AnimatedCastEvent> ComputeEffectCastEvents(AbstractSingleActor actor, CombatData combatData, SkillData skillData, long skillID, string effectGUID, long startOffset, long castDuration, EffectCastEventsChecker checker = null)
        {
            var res = new List<AnimatedCastEvent>();
            if (combatData.GetAnimatedCastData(skillID).Count > 0)
            {
                // Already present in the log
                return res;
            }
            SkillItem skill = skillData.Get(skillID);
            if (combatData.TryGetEffectEventsBySrcWithGUID(actor.AgentItem, effectGUID, out IReadOnlyList<EffectEvent> effects))
            {
                skillData.NotAccurate.Add(skillID);
                foreach (EffectEvent effect in effects)
                {
                    if (checker == null || checker(effects, effect, combatData, skillData))
                    {
                        res.Add(new AnimatedCastEvent(actor.AgentItem, skill, effect.Time + startOffset, castDuration));
                    }
                }
            }
            return res;
        }


        private static IReadOnlyList<AnimatedCastEvent> ComputeUnderBuffCastEvents(CombatData combatData, IReadOnlyList<AbstractBuffEvent> buffs, SkillItem skill)
        {
            var res = new List<AnimatedCastEvent>();
            if (combatData.GetAnimatedCastData(skill.ID).Count > 0)
            {
                return res;
            }
            var applies = buffs.OfType<BuffApplyEvent>().ToList();
            var removals = buffs.OfType<BuffRemoveAllEvent>().ToList();
            for (int i = 0; i < applies.Count && i < removals.Count; i++)
            {
                res.Add(new AnimatedCastEvent(applies[i].To, skill, applies[i].Time, removals[i].Time - applies[i].Time));
            }
            return res;
        }

        private static IReadOnlyList<AnimatedCastEvent> ComputeEndWithBuffApplyCastEvents(CombatData combatData, IReadOnlyList<BuffApplyEvent> buffs, SkillItem skill, long startOffset, long skillDuration)
        {
            var res = new List<AnimatedCastEvent>();
            if (combatData.GetAnimatedCastData(skill.ID).Count > 0)
            {
                return res;
            }
            foreach (BuffApplyEvent bae in buffs)
            {
                res.Add(new AnimatedCastEvent(bae.To, skill, bae.Time - startOffset, skillDuration));
            }
            return res;
        }

        internal static IReadOnlyList<AnimatedCastEvent> ComputeUnderBuffCastEvents(AbstractSingleActor actor, CombatData combatData, SkillData skillData, long skillId, long buffId)
        {
            SkillItem skill = skillData.Get(skillId);
            return ComputeUnderBuffCastEvents(combatData, combatData.GetBuffDataByIDByDst(buffId, actor.AgentItem), skill);
        }

        internal static IReadOnlyList<AnimatedCastEvent> ComputeEndWithBuffApplyCastEvents(AbstractSingleActor actor, CombatData combatData, SkillData skillData, long skillId, long startOffset, long skillDuration, long buffId)
        {
            SkillItem skill = skillData.Get(skillId);
            return ComputeEndWithBuffApplyCastEvents(combatData, combatData.GetBuffDataByIDByDst(buffId, actor.AgentItem).OfType<BuffApplyEvent>().ToList(), skill, startOffset, skillDuration);
        }

        internal static IReadOnlyList<AnimatedCastEvent> ComputeUnderBuffCastEvents(CombatData combatData, SkillData skillData, long skillId, long buffId)
        {
            SkillItem skill = skillData.Get(skillId);
            var dict = combatData.GetBuffData(buffId).GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            var res = new List<AnimatedCastEvent>();
            foreach (KeyValuePair<AgentItem, List<AbstractBuffEvent>> pair in dict)
            {
                res.AddRange(ComputeUnderBuffCastEvents(combatData, pair.Value, skill));
            }
            return res;
        }

        /// <summary>
        /// Adds generic circle decorations, to be used for player skills.
        /// </summary>
        /// <param name="replay">The Combat Replay.</param>
        /// <param name="effect">The mine effect.</param>
        /// <param name="color">The specialization color.</param>
        /// <param name="skill">The source skill.</param>
        /// <param name="icon">The skill icon.</param>
        /// <param name="lifespan">Decoration lifespan.</param>
        /// <param name="radius">Circle radius.</param>
        /// <param name="icon">The skill icon.</param>
        internal static void AddCircleSkillDecoration(CombatReplay replay, EffectEvent effect, Color color, SkillModeDescriptor skill, (long start, long end) lifespan, uint radius, string icon)
        {
            var connector = new PositionConnector(effect.Position);
            replay.Decorations.Add(new CircleDecoration(radius, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
            replay.Decorations.Add(new IconDecoration(icon, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
        }
    }
}
