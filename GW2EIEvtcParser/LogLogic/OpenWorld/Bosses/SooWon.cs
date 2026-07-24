using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Mechanic;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.EIData.Mechanic.MechanicSeverity; 
using static GW2EIEvtcParser.MechanicIDs;

namespace GW2EIEvtcParser.LogLogic.OpenWorld;

internal class SooWon : OpenWorldLogic
{
    public SooWon(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
        
        new PlayerDstHealthDamageHitMechanic(TsunamiSlamOW, Mech_SooWonSlam, new (Symbols.TriangleDown,Colors.DarkRed), new("Slam", "Soo-Won slams the ground in front of her creating a circular tsunami", "Tsunami Slam"), Sev1),
        new PlayerDstHealthDamageHitMechanic(VoidPurgeOW, Mech_SooWonAcidPool, new (Symbols.Circle,Colors.DarkPurple), new("Acid", "Player took damage from an acid pool", "Acid Pool"), Sev1),
        new PlayerDstHealthDamageMechanic(ClawSlapOW, Mech_SooWonClawSlap, new (Symbols.TriangleUp,Colors.Orange), new("Claw Slap", "Soo-Won swipes in an arc in front of her knocking players back", "Claw Slap"), Sev0)
            .UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant) ^ de.HasDowned ^ de.HasKilled),
        new PlayerDstHealthDamageHitMechanic(TailSlap, Mech_SooWonTailSlap, new (Symbols.Square,Colors.Orange),new( "Tail Slap", "Soo-Won slaps the majority of the platform, opposite her head, with her tail", "Tail Slap"), Sev1),
        new PlayerDstHealthDamageHitMechanic(BiteOW, Mech_SooWonBite, new (Symbols.Diamond,Colors.Orange), new("Bite", "Soo-Won bites half the platform while swapping sides", "Bite"), Sev1),
        new MechanicGroup([

            new PlayerDstHealthDamageMechanic(NightmareDevastationOW1, Mech_SooWonWaveHalf, new (Symbols.Square,Colors.Purple), new("Wave (Half)", "Tidal wave that covers one half of the platform", "Tidal Wave (Half)"), Sev0),
            new PlayerDstHealthDamageMechanic(NightmareDevastationOW2, Mech_SooWonWaveFull, new (Symbols.Square,Colors.DarkPurple), new("Wave (Full)", "Tidal wave that covers the entire platform", "Tidal Wave (Full)"), Sev0),
        ]),
        new PlayerDstBuffApplyMechanic(WispForm, Mech_SooWonWisp, new (Symbols.Circle,Colors.Green), new("Wisp", "Wisp Form from standing in a green circle", "Wisp Form"), Sev1),
        new PlayerDstHealthDamageMechanic(SeveredFromBody, Mech_SooWonGreenFailed, new (Symbols.Circle,Colors.Red), new("Failed Green", "Player failed to return to the top of the Harvest Temple after becoming a wisp", "Failed Green"), Sev0).UsingChecker((de, log) => de.HasKilled),
        new MechanicGroup([
            new PlayerDstBuffApplyMechanic(Drown1, Mech_SooWonBubble, new (Symbols.Circle,Colors.LightBlue), new("Bubble", "Player was trapped in a bubble by Soo-Won's Tail", "Bubble"), Sev1),
            new PlayerDstBuffApplyMechanic(Drown2, Mech_SooWonWhirlpool, new (Symbols.Circle,Colors.DarkTeal), new("Whirlpool", "Player was trapped in a whirlpool", "Whirlpool"), Sev1),
        ]),
        new MechanicGroup([
            new EnemyDstBuffApplyMechanic(HardenedShell, Mech_SooWonTailSpawn, new (Symbols.DiamondWide, Colors.DarkTeal), new("Tail", "Soo-Won's Tail spawned", "Tail"), Sev1),
            new EnemyDstBuffRemoveMechanic(HardenedShell, Mech_SooWonTailKilled, new (Symbols.DiamondWide, Colors.DarkGreen), new("Tail Killed", "Soo-Won's Tail killed", "Tail Killed"), Sev0)
                .UsingChecker((bre, log) => !bre.To.HasBuff(log, Invulnerability757, bre.Time - ParserHelper.ServerDelayConstant + 500)),
            new EnemyDstBuffRemoveMechanic(HardenedShell, Mech_SooWonTailDespawn, new (Symbols.DiamondWide, Colors.Yellow), new("Tail Despawned", "Soo-Won's Tail despawned due to phase change", "Tail Despawned"), Sev0)
                .UsingChecker((bre, log) => bre.To.HasBuff(log, Invulnerability757, bre.Time - ParserHelper.ServerDelayConstant + 500)),
        ]),
        new EnemyDstBuffApplyMechanic(DamageImmunity1, Mech_SooWonSideSwap, new (Symbols.Diamond, Colors.Pink), new("Side Swap", "Soo-Won breifly becomes invulnerable and switches sides of the arena", "Side Swap"), Sev2),
        new EnemyDstBuffApplyMechanic(OldExposed, Mech_SooWonCC, new (Symbols.DiamondTall, Colors.DarkGreen), new("CCed", "Breakbar successfully broken", "CCed"), Sev0)
            .UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.SooWonOW) & !bae.To.HasBuff(log, OldExposed, bae.Time - ParserHelper.ServerDelayConstant)),
        ]));
        Extension = "soowon";
        Icon = EncounterIconSooWon;
        LogCategoryInformation.InSubCategoryOrder = 0;
        LogID |= 0x000401;
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = GetGenericLogOffset(logData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            return GetFirstDamageEventTime(logData, agentData, combatData, agentData.GetVolatileSpeciesByID(TargetID.SooWonOW).FirstOrDefault() ?? throw new MissingKeyActorsException("SooWon not found"));
        }
        return startToUse;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor? mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.SooWonOW));
        SingleActor? tailTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.SooWonTailOW));
        if (mainTarget == null)
        {
            throw new MissingKeyActorsException("Soo-Won not found");
        }

        phases[0].AddTarget(mainTarget, log);
        if (!requirePhases)
        {
            return phases;
        }

        phases.AddRange(GetSubPhasesByInvul(log, new long[] { Invulnerability757, SooWonSpearPhaseInvul }, mainTarget, true, true, log.LogData.LogStart,
            log.LogData.LogEnd));

        int phaseOffset = GetPhaseOffset(log, mainTarget);
        InitPhases(phases, mainTarget, tailTarget, log, phaseOffset);
        for (int i = 1; i < phases.Count; i++)
        {
            phases[i].AddParentPhase(phases[0]);
        }
        return phases;
    }

    /// <summary>
    /// Calculates on which phase the log started by checking Soo-Won's initial health and invulnerability buffs.
    /// </summary>
    ///
    /// <returns>An integer indicating on which phase the log started:
    /// <code>
    ///  0: 100% - 80%
    ///  1: First Greens
    ///  2: 80% - 60%
    ///  3: First Spear
    ///  4: First Champions
    ///  5: 60% - 40%
    ///  6: Second Greens
    ///  7: 40% - 20%
    ///  8: Second Spear
    ///  9: Second Champions
    /// 10: 20% - 0%
    /// </code>
    /// </returns>
    private static int GetPhaseOffset(ParsedEvtcLog log, SingleActor mainTarget)
    {
        double initialHealth = mainTarget.GetCurrentHealthPercent(log, 0);
        Func<Func<BuffApplyEvent, bool>, BuffApplyEvent> targetBuffs = log.CombatData.GetBuffApplyDataByDst(mainTarget.AgentItem).OfType<BuffApplyEvent>().FirstOrDefault!;
        BuffEvent initialInvuln = targetBuffs(x => x.Initial && x.BuffID == Invulnerability757);
        BuffEvent initialDmgImmunity = targetBuffs(x => x.Initial && x.BuffID == SooWonSpearPhaseInvul); // spear phase

        int offset = 0;
        if (initialHealth <= 80 && initialHealth > 60)
        {
            offset = 2;
        }
        else if (initialHealth <= 60 && initialHealth > 40)
        {
            offset = 5;
        }
        else if (initialHealth <= 40 && initialHealth > 20)
        {
            offset = 7;
        }
        else if (initialHealth <= 20 && initialHealth > 0)
        {
            offset = 10;
        }

        if (offset > 0)
        {
            if (initialInvuln != null)
            {
                offset--;
            }
            else if (initialDmgImmunity != null)
            {
                offset++;
            }
        }

        return offset;
    }

    private void InitPhases(List<PhaseData> phases, SingleActor mainTarget, SingleActor? tailTarget, ParsedEvtcLog log, int phaseOffset)
    {
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            switch (i + phaseOffset)
            {
                case 1:
                    phase.Name = "100% - 80%";
                    phase.AddTarget(mainTarget, log);
                    break;
                case 2:
                    phase.Name = "First Greens";
                    phase.AddTarget(mainTarget, log);
                    break;
                case 3:
                    phase.Name = "80% - 60%";
                    phase.AddTarget(mainTarget, log);
                    phase.AddTarget(tailTarget, log);
                    break;
                case 4:
                    phase.Name = "First Spear";
                    phase.AddTarget(mainTarget, log);
                    break;
                case 5:
                    phase.Name = "First Champions";
                    phase.AddTargets(Targets.Where(x =>
                        x.IsSpecies(TargetID.VoidGiantOW) ||
                        x.IsSpecies(TargetID.VoidTimeCasterOW)), log);
                    break;
                case 6:
                    phase.Name = "60% - 40%";
                    phase.AddTarget(mainTarget, log);
                    phase.AddTarget(tailTarget, log);
                    break;
                case 7:
                    phase.Name = "Second Greens";
                    phase.AddTarget(mainTarget, log);
                    break;
                case 8:
                    phase.Name = "40% - 20%";
                    phase.AddTarget(mainTarget, log);
                    phase.AddTarget(tailTarget, log);
                    break;
                case 9:
                    phase.Name = "Second Spear";
                    phase.AddTarget(mainTarget, log);
                    break;
                case 10:
                    phase.Name = "Second Champions";
                    phase.AddTargets(Targets.Where(x =>
                        x.IsSpecies(TargetID.VoidBrandstalkerOW) ||
                        x.IsSpecies(TargetID.VoidColdsteelOW1) ||
                        x.IsSpecies(TargetID.VoidObliteratorOW)), log);
                    break;
                case 11:
                    phase.Name = "20% - 0%";
                    phase.AddTarget(mainTarget, log);
                    phase.AddTarget(tailTarget, log);
                    break;
            }
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData,
        AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        IReadOnlyList<AgentItem> sooWons = agentData.GetVolatileSpeciesByID(TargetID.SooWonOW);
        if (!sooWons.Any())
        {
            throw new MissingKeyActorsException("Soo-Won not found");
        }

        foreach (AgentItem sooWon in sooWons)
        {
            sooWon.OverrideID(TargetID.SooWonOW, agentData);
        }

        IReadOnlyList<AgentItem> sooWonTails = agentData.GetVolatileSpeciesByID(TargetID.SooWonTailOW);
        foreach (AgentItem sooWonTail in sooWonTails)
        {
            sooWonTail.OverrideID(TargetID.SooWonTailOW, agentData);
        }
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData,
        IReadOnlyCollection<AgentItem> playerAgents, LogData.LogSuccessHandler successHandler)
    {
        RewardEvent? reward = combatData.GetRewardEvents().FirstOrDefault(x => x.RewardType == RewardTypes.Daily && x.Time > logData.LogStart);
        if (reward != null)
        {
            successHandler.SetSuccess(true, reward.Time);
        }
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.SooWonOW,
            TargetID.SooWonTailOW,
            TargetID.VoidGiantOW,
            TargetID.VoidTimeCasterOW,
            TargetID.VoidBrandstalkerOW,
            TargetID.VoidColdsteelOW1,
            TargetID.VoidObliteratorOW,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.VoidAbomination,
            TargetID.VoidAbominationOW,
            TargetID.VoidBomberOW,
            TargetID.VoidBrandbeastOW,
            TargetID.VoidBrandchargerOW1,
            TargetID.VoidBrandchargerOW2,
            TargetID.VoidBrandfangOW1,
            TargetID.VoidBrandfangOW2,
            TargetID.VoidBrandscaleOW1,
            TargetID.VoidBrandscaleOW2,
            TargetID.VoidColdsteel,
            TargetID.VoidColdsteelOW2,
            TargetID.VoidCorpseknitterOW1,
            TargetID.VoidCorpseknitterOW2,
            TargetID.VoidDespoilerOW1,
            TargetID.VoidDespoilerOW2,
            TargetID.VoidFiendOW1,
            TargetID.VoidFiendOW2,
            TargetID.VoidFoulmawOW,
            TargetID.VoidFrostwingOW,
            TargetID.VoidGlacierOW1,
            TargetID.VoidGlacierOW2,
            TargetID.VoidInfestedOW1,
            TargetID.VoidInfestedOW2,
            TargetID.VoidMelterOW1,
            TargetID.VoidMelterOW2,
            TargetID.VoidRimewolfOW1,
            TargetID.VoidRimewolfOW2,
            TargetID.VoidRotspinnerOW1,
            TargetID.VoidRotswarmer,
            TargetID.VoidStormOW,
            TargetID.VoidStormseer,
            TargetID.VoidStormseerOW1,
            TargetID.VoidStormseerOW2,
            TargetID.VoidTangler,
            TargetID.VoidTanglerOW,
            TargetID.VoidThornheartOW1,
            TargetID.VoidThornheartOW2,
            TargetID.VoidWarforgedVeteran,
            TargetID.VoidWormOW,
        ];
    }
}
