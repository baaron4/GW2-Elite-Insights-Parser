using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

internal static class ProfHelper
{

    private static readonly List<InstantCastFinder> _genericInstantCastFinders =
    [
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
            .UsingDisableWithMissileData()
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
            .WithBuilds(GW2Builds.November2018Rune)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new BuffGainCastFinder(SuperiorSigilOfVision, SuperiorSigilOfVision)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new BuffGainCastFinder(SuperiorSigilOfConcentration, SuperiorSigilOfConcentration)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.November2018Rune)
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
        new MissileCastFinder(Snowball_SigilOfMischief, Snowball_SigilOfMischief)
            .UsingICD(500)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        // Runes
        new EffectCastFinderByDst(RuneOfNightmare, EffectGUIDs.RuneOfNightmare)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear)
            .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
        new MinionSpawnCastFinder(RuneLichSpawn, (int)MinionID.JaggedHorror)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
        new MinionSpawnCastFinder(RuneOgreSpawn, (int)MinionID.RockDog)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
        new MinionSpawnCastFinder(RuneGolemancerSpawn, (int)MinionID.MarkIGolem)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
        new MinionSpawnCastFinder(RunePrivateerSpawn, (int)MinionID.TropicalBird)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
        // Combos
        new EXTHealingCastFinder(WaterBlastCombo1, WaterBlastCombo1),
        new EXTHealingCastFinder(WaterBlastCombo2, WaterBlastCombo2),
        new EXTHealingCastFinder(WaterLeapCombo, WaterLeapCombo),
        new BreakbarDamageCastFinder(LightningLeapCombo, LightningLeapCombo),
        // Misc
        new BuffGainCastFinder(PortalEntranceWhiteMantleWatchwork, PortalWeavingWhiteMantleWatchwork),
        new BuffGainCastFinder(PortalExitWhiteMantleWatchwork, PortalUsesWhiteMantleWatchwork)
            .UsingBeforeWeaponSwap(),
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
        new BuffGainCastFinder(BloodstoneFervor, BloodstoneFervor)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new BuffGainCastFinder(SoulOfTheTitan, SoulOfTheTitan)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new BuffGiveCastFinder(RelicOfDagdaHit, RelicOfDagdaBuff)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new BuffGiveCastFinder(RelicOfIsgarrenTargetBuff, RelicOfIsgarrenTargetBuff)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new BuffGiveCastFinder(RelicOfTheDragonhunterTargetBuff, RelicOfTheDragonhunterTargetBuff)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new BuffLossCastFinder(RelicOfFireworksBuffLoss, RelicOfFireworks)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new BuffLossCastFinder(RelicOfTheClawBuffLoss, RelicOfTheClaw)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new EffectCastFinder(RelicOfCerusHit, EffectGUIDs.RelicOfCerusEye)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new EffectCastFinder(RelicOfTheIce, EffectGUIDs.RelicOfIce)
            .UsingICD(1000)
            .UsingDisableWithMissileData()
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new EffectCastFinder(RelicOfFireworks, EffectGUIDs.RelicOfFireworks)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new BuffGainCastFinder(RelicOfTheClaw, RelicOfTheClaw)
            .UsingOverridenDurationChecker(0)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new EffectCastFinder(RelicOfPeithaBlade, EffectGUIDs.RelicOfPeitha)
            .UsingDisableWithMissileData()
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        //new EffectCastFinder(RelicOfTheCitadel, EffectGUIDs.RelicWhiteCircle).UsingChecker((evt, combatData, agentData, skillData) =>
        //{
        //    combatData.TryGetEffectEventsByGUID(EffectGUIDs.RelicOfTheCitadelExplosion, out var effects);
        //    return effects != null && effects.Any(x => x.Time > evt.Time);
        //}).UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        //new EffectCastFinder(RelicOfTheNightmare, EffectGUIDs.RelicWhiteCircle).UsingChecker((evt, combatData, agentData, skillData) =>
        //{
        //    combatData.TryGetEffectEventsByGUID(EffectGUIDs.RelicOfTheNightmare, out var effects);
        //    return effects != null && effects.Any(x => x.Time > evt.Time);
        //}).UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        //new EffectCastFinder(RelicOfTheKrait, EffectGUIDs.RelicWhiteCircle).UsingChecker((evt, combatData, agentData, skillData) =>
        //{
        //    combatData.TryGetEffectEventsByGUID(EffectGUIDs.RelicOfTheKrait, out var effects);
        //    return effects != null && effects.Any(x => x.Time > evt.Time);
        //}).UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new EffectCastFinder(RelicOfTheWizardsTower, EffectGUIDs.RelicWhiteCircle)
            .UsingSecondaryEffectSameSrcChecker(EffectGUIDs.RelicOfTheWizardsTower)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new EffectCastFinder(RelicOfTheTwinGenerals, EffectGUIDs.RelicOfTheTwinGenerals)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new EffectCastFinder(RelicOfSorrowBuff, EffectGUIDs.RelicOfSorrow3)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new EffectCastFinder(RelicOfTheStormsingerChain, EffectGUIDs.RelicOfTheStormsinger)
            .UsingChecker((effectEvent, combatData, agentData, skillData) => combatData.HasLostBuff(RelicOfTheStormsingerBuff, effectEvent.Src, effectEvent.Time))
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new EffectCastFinder(RelicOfTheBeehive, EffectGUIDs.RelicWhiteCircle)
            .UsingSecondaryEffectSameSrcChecker(EffectGUIDs.RelicOfTheBeehive1, 1000)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new EffectCastFinder(RelicOfMountBalrior, EffectGUIDs.RelicOfMountBalrior1)
            .UsingSecondaryEffectSameSrcChecker(EffectGUIDs.RelicOfMountBalrior2)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new EffectCastFinder(RelicOfTheHolosmith, EffectGUIDs.RelicOfTheHolosmith)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new EffectCastFinder(RelicOfTheSteamshrieker, EffectGUIDs.RelicOfTheSteamshrieker)
            .UsingICD(0)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new EffectCastFinder(BloodstoneExplosion, EffectGUIDs.RelicOfBloodstone)
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
        new MinionSpawnCastFinder(RelicLichSpawn, (int)MinionID.JaggedHorror)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear)
            .WithBuilds(GW2Builds.November2024MountBalriorRelease),
        new MinionSpawnCastFinder(RelicOgreSpawn, (int)MinionID.RockDog)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear)
            .WithBuilds(GW2Builds.November2024MountBalriorRelease),
        new MinionSpawnCastFinder(RelicGolemancerSpawn, (int)MinionID.MarkIGolem)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear)
            .WithBuilds(GW2Builds.November2024MountBalriorRelease),
        new MinionSpawnCastFinder(RelicPrivateerSpawn, (int)MinionID.TropicalBird)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear)
            .WithBuilds(GW2Builds.November2024MountBalriorRelease),
        new MissileCastFinder(RelicOfTheIce, RelicOfTheIce)
            .UsingICD(1000)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        new MissileCastFinder(RelicOfPeithaBlade, RelicOfPeithaBlade)
            .UsingOrigin(InstantCastFinder.InstantCastOrigin.Gear),
        // Mounts
        new BuffGainCastFinder(BondOfLifeSkill, BondOfLifeBuff),
        new BuffGainCastFinder(BondOfVigorSkill, BondOfVigorBuff),
        new BuffGainCastFinder(BondOfFaithSkill, EvasionBondOfFaith)
            .UsingBeforeWeaponSwap(),
        new BuffGainCastFinder(StealthMountSkill, StealthMountBuff),
        // Skyscale
        new EffectCastFinderByDst(SkyscaleSkill, EffectGUIDs.SkyscaleLaunch),
        new EffectCastFinder(SkyscaleFireballSkill, EffectGUIDs.SkyscaleFireball),
        new EffectCastFinder(SkyscaleBlastSkill, EffectGUIDs.SkyscaleBlast1)
            .UsingSecondaryEffectSameSrcChecker(EffectGUIDs.SkyscaleBlast2),
    ];

    internal static void AttachMasterToGadgetByCastData(CombatData combatData, IReadOnlyCollection<AgentItem> gadgets, IReadOnlyList<long> castIDS, long castEndThreshold)
    {
        var possibleCandidates = new HashSet<AgentItem>();
        var gadgetSpawnCastData = new List<AnimatedCastEvent>();
        foreach (long id in castIDS)
        {
            gadgetSpawnCastData.AddRange(combatData.GetAnimatedCastData(id));
        }
        gadgetSpawnCastData.Sort((x, y) => x.Time.CompareTo(y.Time));
        foreach (CastEvent castEvent in gadgetSpawnCastData)
        {
            long start = castEvent.Time;
            long end = castEvent.EndTime;
            possibleCandidates.Add(castEvent.Caster);
            foreach (AgentItem gadget in gadgets)
            {
                if (gadget.FirstAware >= start && gadget.FirstAware <= end + castEndThreshold)
                {
                    // more than one candidate, put to unknown and drop the search
                    if (gadget.Master != null && !gadget.GetFinalMaster().Is(castEvent.Caster.GetFinalMaster()))
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
        foreach (HealthDamageEvent evt in combatData.GetDamageData(damageSkillID))
        {
            // dst must no be a gadget nor a friendly player
            // src must be a masterless gadget
            if (!playerAgents.Any(evt.To.IsMaster) && evt.To.Type != AgentItem.AgentType.Gadget && evt.From.Type == AgentItem.AgentType.Gadget && evt.From.Master == null)
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

    internal static void ProcessRacialGadgets(IReadOnlyList<AgentItem> players, CombatData combatData)
    {
        var playerAgents = new HashSet<AgentItem>(players);
        // Sylvari stuff
        HashSet<AgentItem> seedTurrets = GetOffensiveGadgetAgents(combatData, SeedTurretDamage, playerAgents);
        HashSet<AgentItem> graspingWines = GetOffensiveGadgetAgents(combatData, GraspingVinesDamage, playerAgents);
        AttachMasterToGadgetByCastData(combatData, seedTurrets, new List<long> { SeedTurret, TakeRootSkill }, 1000);
        AttachMasterToGadgetByCastData(combatData, graspingWines, new List<long> { GraspingVines }, 1000);
        // melandru avatar works fine already
    }

    //
    internal static IReadOnlyCollection<InstantCastFinder> GetProfessionInstantCastFinders(IReadOnlyList<AgentItem> players)
    {
        List<InstantCastFinder> instantCastFinders = new (500);
        instantCastFinders.AddRange(_genericInstantCastFinders);
        foreach (Spec spec in players.Select(x => x.BaseSpec).Distinct())
        {
            switch (spec)
            {
                //
                case Spec.Elementalist:
                    ElementalistHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                //
                case Spec.Necromancer:
                    NecromancerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                //
                case Spec.Mesmer:
                    MesmerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                //
                case Spec.Thief:
                    ThiefHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                //
                case Spec.Engineer:
                    EngineerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                //
                case Spec.Ranger:
                    RangerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                //
                case Spec.Revenant:
                    RevenantHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                //
                case Spec.Guardian:
                    GuardianHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                //
                case Spec.Warrior:
                    WarriorHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
            }        
        }
        foreach (Spec spec in players.Select(x => x.Spec).Distinct())
        {
            switch (spec)
            {
                //
                case Spec.Tempest:
                    TempestHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Weaver:
                    WeaverHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Catalyst:
                    CatalystHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Evoker:
                    EvokerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                //
                case Spec.Reaper:
                    ReaperHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Scourge:
                    ScourgeHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Harbinger:
                    HarbingerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Ritualist:
                    RitualistHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                //
                case Spec.Chronomancer:
                    ChronomancerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Mirage:
                    MirageHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Virtuoso:
                    VirtuosoHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Troubadour:
                    TroubadourHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                //
                case Spec.Daredevil:
                    DaredevilHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Deadeye:
                    DeadeyeHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Specter:
                    SpecterHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Antiquary:
                    AntiquaryHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                //
                case Spec.Scrapper:
                    ScrapperHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Holosmith:
                    HolosmithHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Mechanist:
                    MechanistHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Amalgam:
                    AmalgamHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                //
                case Spec.Druid:
                    DruidHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Soulbeast:
                    SoulbeastHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Untamed:
                    UntamedHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Galeshot:
                    GaleshotHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                //
                case Spec.Herald:
                    HeraldHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Renegade:
                    RenegadeHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Vindicator:
                    VindicatorHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Conduit:
                    ConduitHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                //
                case Spec.Dragonhunter:
                    DragonhunterHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Firebrand:
                    FirebrandHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Willbender:
                    WillbenderHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Luminary:
                    LuminaryHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                //
                case Spec.Berserker:
                    BerserkerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Spellbreaker:
                    SpellbreakerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Bladesworn:
                    BladeswornHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
                case Spec.Paragon:
                    ParagonHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x.GetInstance()));
                    break;
            }
        }
        return instantCastFinders;
    }

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Blue;

        // White Mantle Portal Device portal locations
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.WhiteMantlePortalInactive, out var whiteMantlePortalInactive))
        {
            var skill = new SkillModeDescriptor(player, PortalEntranceWhiteMantleWatchwork);
            foreach (EffectEvent effect in whiteMantlePortalInactive)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 60000, player.AgentItem, PortalWeavingWhiteMantleWatchwork);
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(90, lifespan, color, 0.2, connector).UsingSkillMode(skill));
                replay.Decorations.Add(new IconDecoration(EffectImages.PortalWhiteMantleSkill, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
            }
        }
        if (log.CombatData.TryGetGroupedEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.WhiteMantlePortalActive, out var whiteMantlePortalActive))
        {
            var skill = new SkillModeDescriptor(player, PortalExitWhiteMantleWatchwork, SkillModeCategory.Portal);
            foreach (var group in whiteMantlePortalActive)
            {
                AttachedDecoration? first = null;
                foreach (EffectEvent effect in group)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 10000, player.AgentItem, PortalUsesWhiteMantleWatchwork);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(90, lifespan, color, 0.3, connector).UsingSkillMode(skill));
                    AttachedDecoration decoration = new IconDecoration(EffectImages.PortalWhiteMantleSkill, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.7f, lifespan, connector).UsingSkillMode(skill);
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

        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RelicOfMountBalrior1, out var mountBalriorEffects))
        {
            var skill = new SkillModeDescriptor(player, RelicOfMountBalrior);
            foreach (var effect in mountBalriorEffects)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 6000);
                AddCircleSkillDecoration(replay, effect, Colors.White, skill, lifespan, 240, EffectImages.EffectRelicOfMountBalrior);
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
            case Spec.Evoker:
                ElementalistHelper.ComputeProfessionCombatReplayActors(player, log, replay);
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
            case Spec.Amalgam:
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
            case Spec.Luminary:
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
                MirageHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                break;
            case Spec.Virtuoso:
                MesmerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                VirtuosoHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                break;
            case Spec.Troubadour:
                MesmerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
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
            case Spec.Ritualist:
                NecromancerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
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
            case Spec.Untamed:
                RangerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                break;
            case Spec.Galeshot:
                RangerHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                GaleshotHelper.ComputeProfessionCombatReplayActors(player, log, replay);
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
            case Spec.Conduit:
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
            case Spec.Antiquary:
                ThiefHelper.ComputeProfessionCombatReplayActors(player, log, replay);
                AntiquaryHelper.ComputeProfessionCombatReplayActors(player, log, replay);
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
            case Spec.Paragon:
                break;
            default:
                break;
        }
    }

    #if DEBUG_EFFECTS
    internal static void DEBUG_ComputeProfessionCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        var knownEffects = new HashSet<GUID>();
        CombatReplay.DebugEffects(p, log, replay.Decorations, knownEffects);
    }
    #endif

    private static readonly HashSet<Spec> _canSummonClones =
    [
        Spec.Mesmer,
        Spec.Chronomancer,
        Spec.Mirage
    ];

    internal static bool CanSummonClones(Spec spec)
    {
        return _canSummonClones.Contains(spec);
    }

    private static readonly HashSet<Spec> _canUseRangerPets =
    [
        Spec.Ranger,
        Spec.Druid,
        Spec.Soulbeast,
        Spec.Untamed,
        Spec.Galeshot,
    ];

    internal static bool CanUseRangerPets(Spec spec)
    {
        return _canUseRangerPets.Contains(spec);
    }

    /// <summary>
    /// Minions that aren't profession-specific bound.
    /// </summary>
    private static readonly HashSet<int> CommonMinions =
    [
        // Racial Summons
        (int)MinionID.HoundOfBalthazar,
        (int)MinionID.SnowWurm,
        (int)MinionID.DruidSpirit,
        (int)MinionID.SylvanHound,
        (int)MinionID.IronLegionSoldier,
        (int)MinionID.IronLegionMarksman,
        (int)MinionID.BloodLegionSoldier,
        (int)MinionID.BloodLegionMarksman,
        (int)MinionID.AshLegionSoldier,
        (int)MinionID.AshLegionMarksman,
        (int)MinionID.STAD007,
        (int)MinionID.STA7012,
        // GW2 Digital Deluxe
        (int)MinionID.MistfireWolf,
        // Rune / Relic Summons
        (int)MinionID.JaggedHorror,
        (int)MinionID.RockDog,
        (int)MinionID.MarkIGolem,
        (int)MinionID.TropicalBird,
        // Consumables with summons
        (int)MinionID.Ember,
        (int)MinionID.HawkeyeGriffon,
        (int)MinionID.SousChef,
        (int)MinionID.SunspearParagonSupport,
        (int)MinionID.RavenSpiritShadow,
    ];

    internal static bool IsKnownMinionID(int id)
    {
        return CommonMinions.Contains(id);
    }

    private static void AddOffensiveBoonsDecorations(SingleActor minion, SingleActor master, ParsedEvtcLog log, CombatReplay replay)
    {
        var quicknessStatus = minion.GetBuffStatus(log, Quickness).Where(x => x.Value > 0);
        replay.Decorations.AddRotatedOverheadIcons(quicknessStatus, minion, BuffImages.Quickness, -180, 15);
        var alacStatus = minion.GetBuffStatus(log, Alacrity).Where(x => x.Value > 0);
        replay.Decorations.AddRotatedOverheadIcons(alacStatus, minion, BuffImages.Alacrity, -120, 15);
        var mightStatus = minion.GetBuffStatus(log, Might).Where(x => x.Value > 0);
        replay.Decorations.AddRotatedOverheadIconsWithValueAsText(mightStatus, minion, BuffImages.Might, -60, 15);
        var furyStatus = minion.GetBuffStatus(log, Fury).Where(x => x.Value > 0);
        replay.Decorations.AddRotatedOverheadIcons(furyStatus, minion, BuffImages.Fury, 0, 15);
    }

    public static void ComputeMinionCombatReplayActors(SingleActor minion, SingleActor master, ParsedEvtcLog log, CombatReplay replay)
    {
        
        switch (minion.ID)
        {
            case (int)MinionID.JadeMech:
                AddOffensiveBoonsDecorations(minion, master, log, replay);
                var signetForceStatus = minion.GetBuffStatus(log, ForceSignet).Where(x => x.Value > 0);
                replay.Decorations.AddRotatedOverheadIcons(signetForceStatus, minion, SkillImages.ForceSignet, 60, 15);
                var signetConductingStatus = minion.GetBuffStatus(log, SuperconductingSignet).Where(x => x.Value > 0);
                replay.Decorations.AddRotatedOverheadIcons(signetConductingStatus, minion, SkillImages.SuperconductingSignet, 120, 15);
                break;
        }
        if (RangerHelper.IsJuvenilePet(minion.AgentItem))
        {
            AddOffensiveBoonsDecorations(minion, master, log, replay);
            var sicEmStatus = minion.GetBuffStatus(log, SicEmBuff).Where(x => x.Value > 0);
            replay.Decorations.AddRotatedOverheadIcons(sicEmStatus, minion, SkillImages.SicEm, 90, 15);
        }
        if (MesmerHelper.IsPhantasm(minion.AgentItem))
        {
            AddOffensiveBoonsDecorations(minion, master, log, replay);
            var phantasmalForceStatus = minion.GetBuffStatus(log, PhantasmalForce).Where(x => x.Value > 0);
            replay.Decorations.AddRotatedOverheadIconsWithValueAsText(phantasmalForceStatus, minion, TraitImages.PhantasmalForce_Mistrust, 90, 15);
        }
        if (NecromancerHelper.IsUndeadMinion(minion.AgentItem) || RitualistHelper.IsSpiritMinion(minion.AgentItem))
        {
            AddOffensiveBoonsDecorations(minion, master, log, replay);
        }
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
        switch (minion.GetFinalMaster().Spec)
        {
            case Spec.Evoker:
                EvokerHelper.AdjustMinionName(minion);
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
    /// <param name="startOffset">offset to be applied to the time value of the effect</param>
    public static IReadOnlyList<AnimatedCastEvent> ComputeEffectCastEvents(AgentItem actor, CombatData combatData, SkillData skillData, long skillID, GUID effect, long startOffset, long castDuration, EffectCastEventsChecker? checker = null)
    {
        var res = new List<AnimatedCastEvent>();
        if (combatData.GetAnimatedCastData(skillID).Count > 0)
        {
            // Already present in the log
            return res;
        }
        SkillItem skill = skillData.Get(skillID);
        if (combatData.TryGetEffectEventsBySrcWithGUID(actor, effect, out var events))
        {
            skillData.NotAccurate.Add(skillID);
            foreach (EffectEvent effectEvent in events)
            {
                if (checker == null || checker(events, effectEvent, combatData, skillData))
                {
                    res.Add(new AnimatedCastEvent(actor, skill, effectEvent.Time + startOffset, castDuration));
                }
            }
        }
        return res;
    }


    private static IReadOnlyList<AnimatedCastEvent> ComputeUnderBuffCastEvents(CombatData combatData, IReadOnlyList<BuffEvent> buffs, SkillItem skill)
    {
        if (combatData.GetAnimatedCastData(skill.ID).Count > 0)
        {
            return [ ];
        }

        var applies = buffs.OfType<BuffApplyEvent>().ToList();
        var removals = buffs.OfType<BuffRemoveAllEvent>().ToList();
        var minCount = Math.Min(applies.Count, removals.Count);
        var res = new List<AnimatedCastEvent>(minCount);

        for (int i = 0; i < minCount; i++)
        {
            res.Add(new AnimatedCastEvent(applies[i].To, skill, applies[i].Time, removals[i].Time - applies[i].Time));
        }

        return res;
    }

    private static IEnumerable<AnimatedCastEvent> ComputeEndWithBuffApplyCastEvents(CombatData combatData, IEnumerable<BuffApplyEvent> buffs, SkillItem skill, long startOffset, long skillDuration)
    {
        if (combatData.GetAnimatedCastData(skill.ID).Count > 0)
        {
            return [ ];
        }

        return buffs.Select(bae => new AnimatedCastEvent(bae.To, skill, bae.Time - startOffset, skillDuration));
    }

    internal static IReadOnlyList<AnimatedCastEvent> ComputeUnderBuffCastEvents(AgentItem actor, CombatData combatData, SkillData skillData, long skillID, long buffID)
    {
        SkillItem skill = skillData.Get(skillID);
        return ComputeUnderBuffCastEvents(combatData, combatData.GetBuffDataByIDByDst(buffID, actor), skill);
    }

    internal static IEnumerable<AnimatedCastEvent> ComputeEndWithBuffApplyCastEvents(AgentItem actor, CombatData combatData, SkillData skillData, long skillID, long startOffset, long skillDuration, long buffID)
    {
        SkillItem skill = skillData.Get(skillID);
        return ComputeEndWithBuffApplyCastEvents(combatData, combatData.GetBuffApplyDataByIDByDst(buffID, actor).OfType<BuffApplyEvent>(), skill, startOffset, skillDuration);
    }

    internal static IReadOnlyList<AnimatedCastEvent> ComputeUnderBuffCastEvents(CombatData combatData, SkillData skillData, long skillID, long buffID)
    {
        SkillItem skill = skillData.Get(skillID);
        var res = new List<AnimatedCastEvent>();
        var dict = combatData.GetBuffData(buffID).GroupBy(x => x.To);
        foreach (var group in dict)
        {
            res.AddRange(ComputeUnderBuffCastEvents(combatData, group.ToList(), skill));
        }
        return res;
    }

    /// <summary>
    /// Adds generic circle decorations, to be used for player skills.
    /// </summary>
    /// <param name="replay">The Combat Replay.</param>
    /// <param name="effect">The Effect Event.</param>
    /// <param name="color">The specialization color.</param>
    /// <param name="skill">The source skill.</param>
    /// <param name="lifespan">Decoration lifespan.</param>
    /// <param name="radius">Circle radius.</param>
    /// <param name="icon">The skill icon.</param>
    internal static void AddCircleSkillDecoration(CombatReplay replay, EffectEvent effect, Color color, SkillModeDescriptor skill, (long start, long end) lifespan, uint radius, string icon)
    {
        var connector = new PositionConnector(effect.Position);
        replay.Decorations.Add(new CircleDecoration(radius, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
        replay.Decorations.Add(new IconDecoration(icon, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
    }

    /// <summary>
    /// Adds generic doughnut decorations, to be used for player skills.
    /// </summary>
    /// <param name="replay">The Combat Replay.</param>
    /// <param name="effect">The Effect Event.</param>
    /// <param name="color">The specialization color.</param>
    /// <param name="skill">The source skill.</param>
    /// <param name="lifespan">Decoration lifespan.</param>
    /// <param name="innerRadius">Inner doughnut radius.</param>
    /// <param name="outerRadius">Outer doughnut radius.</param>
    /// <param name="icon">The skill icon.</param>
    internal static void AddDoughnutSkillDecoration(CombatReplay replay, EffectEvent effect, Color color, SkillModeDescriptor skill, (long start, long end) lifespan, uint innerRadius, uint outerRadius, string icon)
    {
        var connector = new PositionConnector(effect.Position);
        replay.Decorations.Add(new DoughnutDecoration(innerRadius, outerRadius, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
        replay.Decorations.Add(new IconDecoration(icon, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
    }

    /// <summary>
    /// Checks for from HP to be higher than the to HP.
    /// </summary>
    internal static bool FromHigherThanToHPChecker(DamageEvent x, ParsedEvtcLog log)
    {
        double fromHP = x.From.GetCurrentHealthPercent(log, x.Time);
        double toHP = x.To.GetCurrentHealthPercent(log, x.Time);
        if (fromHP < 0.0 || toHP < 0.0)
        {
            return false;
        }
        return fromHP > toHP;
    }

    /// <summary>
    /// Checks for to HP to be between lower and higer.
    /// </summary>
    internal static DamageLogChecker ToHPChecker(double lower, double higher = 101)
    {
        return (x, log) =>
        {
            double toHP = x.To.GetCurrentHealthPercent(log, x.Time);
            if (toHP < 0.0)
            {
                return false;
            }
            return higher > toHP && toHP >= lower;
        };
    }

    /// <summary>
    /// Checks for from HP to be between lower and higer.
    /// </summary>
    internal static DamageLogChecker FromHPChecker(double lower, double higher = 101)
    {
        return (x, log) =>
        {
            double fromHP = x.From.GetCurrentHealthPercent(log, x.Time);
            if (fromHP < 0.0)
            {
                return false;
            }
            return higher > fromHP && fromHP >= lower;
        };
    }

    /// <summary>
    /// Checks the distance between Src and Dst to be less than <paramref name="range"/>.
    /// </summary>
    /// <param name="includeRange">Wether the range value should be included in the distance.</param>
    internal static bool TargetWithinRangeChecker(DamageEvent x, ParsedEvtcLog log, long range, bool includeRange = true)
    {
        x.From.TryGetCurrentPosition(log, x.Time, out var currentPosition);
        x.To.TryGetCurrentPosition(log, x.Time, out var currentTargetPosition);
        var distance = (currentPosition - currentTargetPosition).Length();

        return includeRange ? distance <= range : distance < range;
    }
}
