using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class TempleOfFebe : SecretOfTheObscureStrike
{
    private static readonly HashSet<long> UnboundOptimismSkillIDs =
    [
        WailOfDespairCM, WailOfDespairEmpoweredCM, PoolOfDespairCM, PoolOfDespairEmpoweredCM
    ];

    private static readonly long[] Boons =
    [
        Aegis, Alacrity, Fury, Might, Protection, Quickness, Regeneration, Resistance, Resolution, Stability, Swiftness, Vigor
    ];

    public TempleOfFebe(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(Insatiable, new MechanicPlotlySetting(Symbols.Hourglass, Colors.Pink), "Ins.A", "Insatiable Applied (Absorbed Gluttony Orb)", "Insatiable Application", 0),
                new EnemyCastStartMechanic([InsatiableHungerSmallOrbSkillNM, InsatiableHungerSmallOrbEmpoweredSkillNM, InsatiableHungerSmallOrbSkillCM, InsatiableHungerSmallOrbEmpoweredSkillCM], new MechanicPlotlySetting(Symbols.HourglassOpen, Colors.Pink), "InsHun.C", "Casted Insatiable Hunger", "Insatiable Hunger Cast", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([CrushingRegretNM, CrushingRegretCM], new MechanicPlotlySetting(Symbols.Circle, Colors.DarkGreen), "CrushReg.H", "Hit by Crushing Regret (Green)", "Crushing Regret Hit", 0),
                new PlayerDstHealthDamageHitMechanic([CrushingRegretEmpoweredNM, CrushingRegretEmpoweredCM], new MechanicPlotlySetting(Symbols.Circle, Colors.GreenishYellow), "Emp.CrushReg.H", "Hit by Empowered Crushing Regret (Green)", "Empowered Crushing Regret Hit", 0),
                new PlayerDstEffectMechanic(EffectGUIDs.TempleOfFebeCerusGreen, new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Green), "Green.A", "Crushing Regret Applied (Green)", "Crushing Regret Application", 0),
                new EnemyCastStartMechanic([CrushingRegretNM, CrushingRegretEmpoweredNM, CrushingRegretCM, CrushingRegretEmpoweredCM], new MechanicPlotlySetting(Symbols.CircleOpen, Colors.LightMilitaryGreen), "CrushReg.C", "Casted Crushing Regret", "Crushing Regret Cast", 0),
                new EnemySrcEffectMechanic(EffectGUIDs.TempleOfFebeGreenSuccess, new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "CrushReg.C.S", "Crushing Regret Successful", "Success Crushing Regret", 0),
                new EnemySrcEffectMechanic(EffectGUIDs.TempleOfFebeGreenFailure, new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "CrushReg.C.F", "Crushing Regret Failed", "Failed Crushing Regret", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([WailOfDespairNM, WailOfDespairCM], new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "WailDesp.H", "Hit by Wail of Despair (Spread Player AoE)", "Wail of Despair Hit", 0),
                new PlayerDstHealthDamageHitMechanic([WailOfDespairEmpoweredNM, WailOfDespairEmpoweredCM], new MechanicPlotlySetting(Symbols.Circle, Colors.Orange), "Emp.WailDesp.H", "Hit by Empowered Wail of Despair (Spread Player AoE)", "Empowered Wail of Despair Hit", 0),
                new EnemyCastStartMechanic([WailOfDespairNM, WailOfDespairEmpoweredNM, WailOfDespairCM, WailOfDespairEmpoweredCM], new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.LightOrange), "WailDesp.C", "Casted Wail of Despair", "Wail of Despair Cast", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([PoolOfDespairNM, PoolOfDespairCM], new MechanicPlotlySetting(Symbols.Circle, Colors.Red), "PoolDesp.H", "Hit by Pool of Despair (Spread Ground AoE)", "Pool of Despair Hit", 0),
                new PlayerDstHealthDamageHitMechanic([PoolOfDespairEmpoweredNM, PoolOfDespairEmpoweredCM], new MechanicPlotlySetting(Symbols.Circle, Colors.RedSkin), "Emp.PoolDesp.H", "Hit by Empowered Pool of Despair (Spread Ground AoE)", "Empowered Pool of Despair Hit", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([EnviousGazeNM, EnviousGazeCM], new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Red), "EnvGaz.H", "Hit by Envious Gaze (Wall/Beam)", "Envious Gaze Hit", 0),
                new PlayerDstHealthDamageHitMechanic([EnviousGazeEmpoweredNM, EnviousGazeEmpoweredRearNM, EnviousGazeEmpoweredCM, EnviousGazeEmpoweredRearCM], new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "Emp.EnvGaz.H", "Hit by Empowered Envious Gaze (Double Wall/Beam)", "Empowered Envious Gaze Hit", 0),
                new PlayerDstBuffRemoveMechanic(Boons, new MechanicPlotlySetting(Symbols.Octagon, Colors.Purple), "EnvGaze.Strip", "Boons removed by Envious Gaze (Any)", "Envious Gaze Boon Removal", 100)
                    .UsingChecker((brae, log) => brae.By.IsAnySpecies([(int)TargetID.Cerus, (int)TargetID.EmbodimentOfEnvy, (int)TargetID.PermanentEmbodimentOfEnvy])),
                new EnemyCastStartMechanic([EnviousGazeNM, EnviousGazeCM, EnviousGazeEmpoweredNM, EnviousGazeEmpoweredCM], new MechanicPlotlySetting(Symbols.TriangleDownOpen, Colors.Red), "EnvGaz.C", "Casted Envious Gaze", "Envious Gaze Cast", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([MaliciousIntentSpawnDamageNM, MaliciousIntentSpawnDamageCM], new MechanicPlotlySetting(Symbols.Y, Colors.White), "MalInt.H", "Hit by Malicious Intent (Malicious Shadow Spawn)", "Malicious Intent Hit", 0),
                new PlayerDstBuffApplyMechanic([MaliciousIntentTargetBuff, MaliciousIntentTargetBuffCM], new MechanicPlotlySetting(Symbols.Bowtie, Colors.DarkGreen), "MalInt.A", "Malicious Intent Target", "Targeted by Malicious Intent", 0),
                new EnemyCastStartMechanic([MaliciousIntentNM, MaliciousIntentEmpoweredNM, MaliciousIntentCM, MaliciousIntentEmpoweredCM], new MechanicPlotlySetting(Symbols.Bowtie, Colors.RedSkin), "MalInt.C", "Casted Malicious Intent", "Malicious Intent Cast", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([CryOfRageNM, CryOfRageCM], new MechanicPlotlySetting(Symbols.CircleX, Colors.LightOrange), "CryRage.H", "Hit by Cry of Rage", "Cry of Rage Hit", 0),
                new PlayerDstHealthDamageHitMechanic([CryOfRageEmpoweredNM, CryOfRageEmpoweredCM], new MechanicPlotlySetting(Symbols.CircleX, Colors.Orange), "Emp.CryRage.H", "Hit by Empowered Cry of Rage", "Empowered Cry of Rage Hit", 0),
                new EnemyCastStartMechanic([CryOfRageNM, CryOfRageEmpoweredNM, CryOfRageCM, CryOfRageEmpoweredCM], new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.LightOrange), "CryRage.C", "Casted Cry of Rage", "Cry of Rage Cast", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([EnragedSmashNM, EnragedSmashCM], new MechanicPlotlySetting(Symbols.Star, Colors.Red), "EnrSmash.H", "Hit by Enraged Smash", "Hit by Enraged Smash", 0),
                new PlayerDstHealthDamageHitMechanic([EnragedSmashNM, EnragedSmashCM], new MechanicPlotlySetting(Symbols.Star, Colors.DarkRed), "EnrSmash.D", "Downed to Enraged Smash", "Downed to Enraged Smash", 0)
                    .UsingChecker((ahde, log) => ahde.HasDowned),
                new EnemyCastStartMechanic([EnragedSmashNM, EnragedSmashCM], new MechanicPlotlySetting(Symbols.Star, Colors.Blue), "EnrSmash.C", "Casted Enraged Smash", "Enraged Smash Cast", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(PetrifyDamage, new MechanicPlotlySetting(Symbols.Pentagon, Colors.Teal), "Pet.H", "Hit by Petrify", "Petrify Hit", 0),
                new EnemyCastStartMechanic(PetrifySkill, new MechanicPlotlySetting(Symbols.Pentagon, Colors.Yellow), "Pet.C", "Casted Petrify", "Petrify breakbar start", 0),
                new EnemySrcHealthDamageHitMechanic(PetrifyDamage, new MechanicPlotlySetting(Symbols.Pentagon, Colors.DarkTeal), "Pet.F", "Petrify hit players and healed Cerus", "Petrify breakbar fail", 100),
            ]),
            new NonSpecializedCombatEventListMechanic<TimeCombatEvent>(new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.RedSkin), "UnbOpt.Achiv", "Achievement Eligibility: Unbounded Optimism", "Unbounded Optimism", 0, false, (log, agentItem) =>
                {
                    SingleActor? actor = log.FindActor(agentItem);
                    var eligibilityRemovedEvents = new List<TimeCombatEvent>();
                    if (actor == null)
                    {
                        return eligibilityRemovedEvents;
                    }
                    eligibilityRemovedEvents.AddRange(actor.GetDamageTakenEvents(null, log).Where(x => UnboundOptimismSkillIDs.Contains(x.SkillID) && x.HasHit));
                    IReadOnlyList<DeadEvent> deads = log.CombatData.GetDeadEvents(agentItem);
                    // In case player is dead but death event did not happen during encounter
                    if (agentItem.IsDead(log, log.LogData.LogEnd) && !deads.Any(x => x.Time >= log.LogData.LogStart && x.Time <= log.LogData.LogEnd))
                    {
                        eligibilityRemovedEvents.Add(new PlaceHolderTimeCombatEvent(log.LogData.LogEnd - 1));
                    }
                    else
                    {
                        eligibilityRemovedEvents.AddRange(deads);
                    }
                    IReadOnlyList<DespawnEvent> despawns = log.CombatData.GetDespawnEvents(agentItem);
                    // In case player is DC but DC event did not happen during encounter
                    if (agentItem.IsDC(log, log.LogData.LogEnd) && !despawns.Any(x => x.Time >= log.LogData.LogStart && x.Time <= log.LogData.LogEnd))
                    {
                        eligibilityRemovedEvents.Add(new PlaceHolderTimeCombatEvent(log.LogData.LogEnd - 1));
                    }
                    else
                    {
                        eligibilityRemovedEvents.AddRange(despawns);
                    }
                    eligibilityRemovedEvents.SortByTime();
                    return eligibilityRemovedEvents;
                })
                .UsingEnable(x => x.LogData.IsCM || x.LogData.IsLegendaryCM)
                .UsingAchievementEligibility(),
            new MechanicGroup([
                new EnemyDstBuffApplyMechanic(EmpoweredCerus, new MechanicPlotlySetting(Symbols.Square, Colors.Red), "Emp.A", "Gained Empowered", "Empowered Application", 0),
                new EnemyDstBuffApplyMechanic(EmpoweredDespairCerus, new MechanicPlotlySetting(Symbols.Square, Colors.Black), "EmpDesp.A", "Gained Empowered Despair", "Empowered Despair Application", 0),
                new EnemyDstBuffApplyMechanic(EmpoweredEnvyCerus, new MechanicPlotlySetting(Symbols.Square, Colors.Blue), "EmpEnvy.A", "Gained Empowered Envy", "Empowered Envy Application", 0),
                new EnemyDstBuffApplyMechanic(EmpoweredGluttonyCerus, new MechanicPlotlySetting(Symbols.Square, Colors.Brown), "EmpGlu.A", "Gained Empowered Gluttony", "Empowered Gluttony Application", 0),
                new EnemyDstBuffApplyMechanic(EmpoweredMaliceCerus, new MechanicPlotlySetting(Symbols.Square, Colors.LightBlue), "EmpMal.A", "Gained Empowered Malice", "Empowered Malice Application", 0),
                new EnemyDstBuffApplyMechanic(EmpoweredRageCerus, new MechanicPlotlySetting(Symbols.Square, Colors.LightOrange), "EmpRage.A", "Gained Empowered Rage", "Empowered Rage Application", 0),
                new EnemyDstBuffApplyMechanic(EmpoweredRegretCerus, new MechanicPlotlySetting(Symbols.Square, Colors.LightGrey), "EmpReg.A", "Gained Empowered Regret", "Empowered Regret Application", 0),
            ]),
            new MechanicGroup([
                new EnemyDstBuffApplyMechanic(Invulnerability757, new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightRed), "Despair.K", "Embodiment of Despair Killed", "Despair Killed", 0)
                    .UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.EmbodimentOfDespair) && !bae.To.HasBuff(log, EmpoweredDespairEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightBlue), "Envy.K", "Embodiment of Envy Killed", "Envy Killed", 0)
                    .UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.EmbodimentOfEnvy) && !bae.To.HasBuff(log, EmpoweredEnvyEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightOrange), "Gluttony.K", "Embodiment of Gluttony Killed", "Gluttony Killed", 0)
                    .UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.EmbodimentOfGluttony) && ! bae.To.HasBuff(log, EmpoweredGluttonyEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightGrey), "Malice.K", "Embodiment of Malice Killed", "Malice Killed", 0)
                    .UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.EmbodimentOfMalice) && !bae.To.HasBuff(log, EmpoweredMaliceEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightPurple), "Rage.K", "Embodiment of Rage Killed", "Rage Killed", 0)
                    .UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.EmbodimentOfRage) && !bae.To.HasBuff(log, EmpoweredRageEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, new MechanicPlotlySetting(Symbols.StarOpen, Colors.White), "Regret.K", "Embodiment of Regret Killed", "Regret Killed", 0)
                    .UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.EmbodimentOfRegret) && !bae.To.HasBuff(log, EmpoweredRegretEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, new MechanicPlotlySetting(Symbols.StarOpen, Colors.Red), "Emp.Despair.K", "Empowered Embodiment of Despair Killed", "Empowered Despair Killed", 0)
                    .UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.EmbodimentOfDespair) && bae.To.HasBuff(log, EmpoweredDespairEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, new MechanicPlotlySetting(Symbols.StarOpen, Colors.Blue), "Emp.Envy.K", "Empowered Embodiment of Envy Killed", "Empowered Envy Killed", 0)
                    .UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.EmbodimentOfEnvy) && bae.To.HasBuff(log, EmpoweredEnvyEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, new MechanicPlotlySetting(Symbols.StarOpen, Colors.Orange), "Emp.Gluttony.K", "Empowered Embodiment of Gluttony Killed", "Empowered Gluttony Killed", 0)
                    .UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.EmbodimentOfGluttony) && bae.To.HasBuff(log, EmpoweredGluttonyEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, new MechanicPlotlySetting(Symbols.StarOpen, Colors.Grey), "Emp.Malice.K", "Empowered Embodiment of Malice Killed", "Empowered Malice Killed", 0)
                    .UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.EmbodimentOfMalice) && bae.To.HasBuff(log, EmpoweredMaliceEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, new MechanicPlotlySetting(Symbols.StarOpen, Colors.Purple), "Emp.Rage.K", "Empowered Embodiment of Rage Killed", "Empowered Rage Killed", 0)
                    .UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.EmbodimentOfRage) && bae.To.HasBuff(log, EmpoweredRageEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, new MechanicPlotlySetting(Symbols.StarOpen, Colors.Black), "Emp.Regret.K", "Empowered Embodiment of Regret Killed", "Empowered Regret Killed", 0)
                    .UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.EmbodimentOfRegret) && bae.To.HasBuff(log, EmpoweredRegretEmbodiment, bae.Time)),

            ]),  
        ])
        );
        Icon = EncounterIconTempleOfFebe;
        Extension = "tmplfeb";
        LogCategoryInformation.InSubCategoryOrder = 1;
        LogID |= 0x000002;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (1149, 1149),
                        (-2088, -6124, 2086, -1950));
        arenaDecorations.Add(new ArenaDecoration(CombatReplayTempleOfFebe, crMap));
        return crMap;
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.Cerus,
            TargetID.EmbodimentOfDespair,
            TargetID.EmbodimentOfEnvy,
            TargetID.EmbodimentOfGluttony,
            TargetID.EmbodimentOfMalice,
            TargetID.EmbodimentOfRage,
            TargetID.EmbodimentOfRegret,
            TargetID.PermanentEmbodimentOfDespair,
            TargetID.PermanentEmbodimentOfEnvy,
            TargetID.PermanentEmbodimentOfGluttony,
            TargetID.PermanentEmbodimentOfMalice,
            TargetID.PermanentEmbodimentOfRage,
            TargetID.PermanentEmbodimentOfRegret,
            TargetID.MaliciousShadow,
            TargetID.MaliciousShadowCM,
        ];
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            { TargetID.Cerus, 0 },
            { TargetID.EmbodimentOfDespair, 1 },
            { TargetID.EmbodimentOfEnvy, 1 },
            { TargetID.EmbodimentOfGluttony, 1 },
            { TargetID.EmbodimentOfMalice, 1 },
            { TargetID.EmbodimentOfRage, 1 },
            { TargetID.EmbodimentOfRegret, 1 },
            { TargetID.MaliciousShadow, 2 },
            { TargetID.MaliciousShadowCM, 2 },
            { TargetID.PermanentEmbodimentOfDespair, 3 },
            { TargetID.PermanentEmbodimentOfEnvy, 3 },
            { TargetID.PermanentEmbodimentOfGluttony, 3 },
            { TargetID.PermanentEmbodimentOfMalice, 3 },
            { TargetID.PermanentEmbodimentOfRage, 3 },
            { TargetID.PermanentEmbodimentOfRegret, 3 },
        };
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor cerus = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Cerus)) ?? throw new MissingKeyActorsException("Cerus not found");
        phases[0].AddTarget(cerus, log);
        var embodimentIDs = new List<TargetID>
        {
            TargetID.EmbodimentOfDespair,
            TargetID.EmbodimentOfEnvy,
            TargetID.EmbodimentOfGluttony,
            TargetID.EmbodimentOfMalice,
            TargetID.EmbodimentOfRage,
            TargetID.EmbodimentOfRegret,
        };
        var embodiments = Targets.Where(target => target.IsAnySpecies(embodimentIDs));
        var embodimentsKilled = embodiments.Where(target => log.CombatData.GetBuffDataByIDByDst(Invulnerability757, target.AgentItem).Any());
        phases[0].AddTargets(embodimentsKilled, log, PhaseData.TargetPriority.Blocking);
        if (!requirePhases)
        {
            return phases;
        }
        // Invul check
        List<PhaseData> invulnPhases = GetPhasesByInvul(log, InvulnerabilityCerus, cerus, true, true);
        phases.AddRange(invulnPhases);
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(phases[0]);
            if (i % 2 == 0)
            {
                phase.Name = "Split " + (i) / 2;
                var killed = embodimentsKilled.Where(target =>
                {
                    var invulnApplies = log.CombatData.GetBuffApplyDataByIDByDst(Invulnerability757, target.AgentItem).OfType<BuffApplyEvent>();
                    return invulnApplies.Any(apply => phase.InInterval(apply.Time)); // phase interval is unfitted = based on cerus invuln
                });
                var priority = killed.Any() ? PhaseData.TargetPriority.NonBlocking : PhaseData.TargetPriority.Main; // default to all as main if none killed
                AddTargetsToPhaseAndFit(phase, embodimentIDs, log, priority);
                phase.AddTargets(killed, log); // overwrite priority for killed
            }
            else
            {
                phase.Name = "Phase " + (i + 1) / 2;
                phase.AddTarget(cerus, log);
            }
        }
        // Enraged Smash phase - After 10% bar is broken
        CastEvent? enragedSmash = cerus.GetCastEvents(log).Where(x => x.SkillID == EnragedSmashNM || x.SkillID == EnragedSmashCM).FirstOrDefault();
        if (enragedSmash != null)
        {
            var finalPhase = phases[^1];
            var phase = new SubPhasePhaseData(enragedSmash.Time, log.LogData.LogEnd, "Enraged Smash");
            phase.AddParentPhase(finalPhase);
            phase.AddTarget(cerus, log);
            phases.Add(phase);
            // Sub Phase for 50%-10%
            PhaseData? phase3 = invulnPhases.LastOrDefault(x => x.InInterval(enragedSmash.Time));
            if (phase3 != null)
            {
                var phase50_10 = new SubPhasePhaseData(phase3.Start, enragedSmash.Time, "50%-10%");
                phase50_10.AddParentPhase(finalPhase);
                phase50_10.AddTarget(cerus, log);
                phases.Add(phase50_10);
            }
        }
        return phases;
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = GetGenericLogOffset(logData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            var enterCombatTime = GetEnterCombatTime(logData, agentData, combatData, logStartNPCUpdate.Time, GenericTriggerID, logStartNPCUpdate.DstAgent);
            AgentItem cerus = GetCerusItem(agentData);
            var spawnEvent = combatData.Where(x => x.IsStateChange == StateChange.Spawn && x.SrcMatchesAgent(cerus)).FirstOrDefault();
            if (spawnEvent != null && enterCombatTime >= spawnEvent.Time)
            {
                return spawnEvent.Time;
            } 
            return cerus.FirstAware;
        }
        return startToUse;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var embodimentIDs = new List<TargetID>()
        {
            TargetID.EmbodimentOfDespair,
            TargetID.EmbodimentOfEnvy,
            TargetID.EmbodimentOfGluttony,
            TargetID.EmbodimentOfMalice,
            TargetID.EmbodimentOfRage,
            TargetID.EmbodimentOfRegret,
        };
        AgentItem cerus = GetCerusItem(agentData);
        foreach (TargetID embodimentID in embodimentIDs)
        {
            foreach (AgentItem embodiment in agentData.GetNPCsByID(embodimentID))
            {
                if (Math.Abs(cerus.FirstAware - embodiment.FirstAware) < 50)
                {
                    switch (embodiment.ID)
                    {
                        case (int)TargetID.EmbodimentOfDespair:
                            embodiment.OverrideID(TargetID.PermanentEmbodimentOfDespair, agentData);
                            break;
                        case (int)TargetID.EmbodimentOfEnvy:
                            embodiment.OverrideID(TargetID.PermanentEmbodimentOfEnvy, agentData);
                            break;
                        case (int)TargetID.EmbodimentOfGluttony:
                            embodiment.OverrideID(TargetID.PermanentEmbodimentOfGluttony, agentData);
                            break;
                        case (int)TargetID.EmbodimentOfMalice:
                            embodiment.OverrideID(TargetID.PermanentEmbodimentOfMalice, agentData);
                            break;
                        case (int)TargetID.EmbodimentOfRage:
                            embodiment.OverrideID(TargetID.PermanentEmbodimentOfRage, agentData);
                            break;
                        case (int)TargetID.EmbodimentOfRegret:
                            embodiment.OverrideID(TargetID.PermanentEmbodimentOfRegret, agentData);
                            break;
                        default:
                            break;
                    }
                    break;
                }
            }
        }
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        int[] curEmbodiments = [1, 1, 1, 1, 1, 1];
        foreach (SingleActor target in Targets)
        {
            switch (target.ID)
            {
                case (int)TargetID.EmbodimentOfDespair:
                    CombatItem? despair = combatData.FirstOrDefault(x => x.SkillID == EmpoweredDespairEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                    if (despair != null && Math.Abs(target.FirstAware - despair.Time) <= ServerDelayConstant)
                    {
                        target.OverrideName("Empowered " + target.Character);
                    }
                    break;
                case (int)TargetID.EmbodimentOfEnvy:
                    CombatItem? envy = combatData.FirstOrDefault(x => x.SkillID == EmpoweredEnvyEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                    if (envy != null && Math.Abs(target.FirstAware - envy.Time) <= ServerDelayConstant)
                    {
                        target.OverrideName("Empowered " + target.Character);
                    }
                    break;
                case (int)TargetID.EmbodimentOfGluttony:
                    CombatItem? gluttony = combatData.FirstOrDefault(x => x.SkillID == EmpoweredGluttonyEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                    if (gluttony != null && Math.Abs(target.FirstAware - gluttony.Time) <= ServerDelayConstant)
                    {
                        target.OverrideName("Empowered " + target.Character);
                    }
                    break;
                case (int)TargetID.EmbodimentOfMalice:
                    CombatItem? malice = combatData.FirstOrDefault(x => x.SkillID == EmpoweredMaliceEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                    if (malice != null && Math.Abs(target.FirstAware - malice.Time) <= ServerDelayConstant)
                    {
                        target.OverrideName("Empowered " + target.Character);
                    }
                    break;
                case (int)TargetID.EmbodimentOfRage:
                    CombatItem? rage = combatData.FirstOrDefault(x => x.SkillID == EmpoweredRageEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                    if (rage != null && Math.Abs(target.FirstAware - rage.Time) <= ServerDelayConstant)
                    {
                        target.OverrideName("Empowered " + target.Character);
                    }
                    break;
                case (int)TargetID.EmbodimentOfRegret:
                    CombatItem? regret = combatData.FirstOrDefault(x => x.SkillID == EmpoweredRegretEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                    if (regret != null && Math.Abs(target.FirstAware - regret.Time) <= ServerDelayConstant)
                    {
                        target.OverrideName("Empowered " + target.Character);
                    }
                    break;
                case (int)TargetID.PermanentEmbodimentOfDespair:
                case (int)TargetID.PermanentEmbodimentOfEnvy:
                case (int)TargetID.PermanentEmbodimentOfGluttony:
                case (int)TargetID.PermanentEmbodimentOfMalice:
                case (int)TargetID.PermanentEmbodimentOfRage:
                case (int)TargetID.PermanentEmbodimentOfRegret:
                    target.OverrideName(target.Character + " (Permanent)");
                    break;
                default:
                    break;
            }
        }
    }

    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        SingleActor cerus = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Cerus)) ?? throw new MissingKeyActorsException("Cerus not found");
        var cerusHP = cerus.GetHealth(combatData);
        if (cerusHP > 130e6)
        {
            return LogData.LogMode.LegendaryCM;
        }
        return (cerusHP > 100e6) ? LogData.LogMode.CM : LogData.LogMode.Normal;

    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Temple of Febe";
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        var casts = target.GetAnimatedCastEvents(log).ToList();
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.Cerus:
                foreach (CastEvent cast in casts)
                {
                    switch (cast.SkillID)
                    {
                        // Enraged Smash - 10% attacks
                        case EnragedSmashNM:
                        case EnragedSmashCM:
                            // Cast time is 750, we only show a quick pulse of damage
                            lifespan = (cast.Time + 750, cast.Time + 1000);
                            var circle = new CircleDecoration(2500, lifespan, Colors.RedSkin, 0.1, new AgentConnector(target));
                            replay.Decorations.Add(circle);
                            break;
                        // Petrify - Breakbar 80%, 50%, 10%
                        case PetrifySkill:
                            lifespan = (cast.Time, cast.GetInterruptedByBuffTime(log, InvulnerabilityCerus));
                            replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMajorSizeInPixel, lifespan, Colors.Red, 0.6, Colors.Black, 0.2, [(cast.Time, 0), (cast.Time + 10000, 100)], new AgentConnector(target))
                                .UsingRotationConnector(new AngleConnector(180)));
                            break;
                        default:
                            break;
                    }
                }

                replay.AddHideByBuff(target, log, InvulnerabilityCerus);
                AddCryOfRageDecoration(target, log, replay, casts);
                AddEnviousGazeDecoration(target, log, replay, casts);
                AddInsatiableHungerDecoration(target, log, replay);
                break;
            case (int)TargetID.EmbodimentOfDespair:
                AddDeterminedOverhead(target, log, replay);
                break;
            case (int)TargetID.PermanentEmbodimentOfDespair:
                AddHiddenWhileNotCasting(target, log, replay, 6350);
                break;
            case (int)TargetID.EmbodimentOfEnvy:
                AddDeterminedOverhead(target, log, replay);
                AddEnviousGazeDecoration(target, log, replay, casts);
                break;
            case (int)TargetID.PermanentEmbodimentOfEnvy:
                AddHiddenWhileNotCasting(target, log, replay, 13750);
                AddEnviousGazeDecoration(target, log, replay, casts);
                break;
            case (int)TargetID.EmbodimentOfGluttony:
                AddDeterminedOverhead(target, log, replay);
                AddInsatiableHungerDecoration(target, log, replay);
                break;
            case (int)TargetID.PermanentEmbodimentOfGluttony:
                AddHiddenWhileNotCasting(target, log, replay, 13720);
                AddInsatiableHungerDecoration(target, log, replay);
                break;
            case (int)TargetID.EmbodimentOfMalice:
                AddDeterminedOverhead(target, log, replay);
                break;
            case (int)TargetID.PermanentEmbodimentOfMalice:
                AddHiddenWhileNotCasting(target, log, replay, 3670);
                break;
            case (int)TargetID.EmbodimentOfRage:
                AddDeterminedOverhead(target, log, replay);
                AddCryOfRageDecoration(target, log, replay, casts);
                break;
            case (int)TargetID.PermanentEmbodimentOfRage:
                AddHiddenWhileNotCasting(target, log, replay, 7660);
                AddCryOfRageDecoration(target, log, replay, casts);
                break;
            case (int)TargetID.EmbodimentOfRegret:
                AddDeterminedOverhead(target, log, replay);
                break;
            case (int)TargetID.PermanentEmbodimentOfRegret:
                AddHiddenWhileNotCasting(target, log, replay, 8350);
                break;
            default:
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);

        // Crushing Regret (Green)
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.TempleOfFebeCerusGreen, out var crushingRegrets))
        {
            foreach (EffectEvent effect in crushingRegrets)
            {
                // The skill definition of Crushing Regret has different radiuses but the visual indicator looks about 175 for all of them.
                // Until we know more, we set it to the visual indicator in game.
                // Normal Mode - 360
                // Normal Mode Empowered - 240
                // Challenge Mode - 240
                // Challenge Mode Empowered - 156
                uint radius = 175;
                (long start, long end) lifespan = effect.ComputeLifespan(log, 5000);
                long growing = lifespan.end;
                lifespan = ComputeMechanicLifespanWithCancellationTime(effect.Src, log, lifespan);
                var circle = new CircleDecoration(radius, lifespan, Colors.DarkGreen, 0.2, new AgentConnector(p));
                replay.Decorations.AddWithGrowing(circle, growing, true);
            }
        }

        // Wail of Despair - Spread AoE on player
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.TempleOfFebeWailOfDespair, out var wailsOfDespair))
        {
            foreach (EffectEvent effect in wailsOfDespair)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 5000);
                long growing = lifespan.end;
                lifespan = ComputeMechanicLifespanWithCancellationTime(effect.Src, log, lifespan);
                var circle = new CircleDecoration(120, lifespan, Colors.LightOrange, 0.2, new AgentConnector(p));
                replay.Decorations.AddWithGrowing(circle, growing);
            }
        }

        // Wail of Despair - Empowered - Spread AoE on player
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.TempleOfFebeWailOfDespairEmpowered, out var wailsOfDespairEmpowered))
        {
            foreach (EffectEvent effect in wailsOfDespairEmpowered)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 5000);
                long growing = lifespan.end;
                lifespan = ComputeMechanicLifespanWithCancellationTime(effect.Src, log, lifespan);
                var circle = new CircleDecoration(240, lifespan, Colors.LightOrange, 0.2, new AgentConnector(p));
                replay.Decorations.AddWithGrowing(circle, growing);
            }
        }

        // Malicious Intent - Malice Adds Tether
        var maliciousIntent = GetBuffApplyRemoveSequence(log.CombatData, [MaliciousIntentTargetBuff, MaliciousIntentTargetBuffCM], p, true, true);
        replay.Decorations.AddTether(maliciousIntent, Colors.RedSkin, 0.4, 5, false);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

        // Crushing Regret (Green) End
        var crushingRegretEnds = new List<(GUID GUID, Color Color)>()
        {
            (EffectGUIDs.TempleOfFebeGreenSuccess, Colors.Green),
            (EffectGUIDs.TempleOfFebeGreenFailure, Colors.DarkRed)
        };
        foreach ((GUID guid, Color color) in crushingRegretEnds)
        {
            if (log.CombatData.TryGetEffectEventsByGUID(guid, out var crushingRegrets))
            {
                foreach (EffectEvent effect in crushingRegrets)
                {
                    // The skill definition of Crushing Regret has different radiuses but the visual indicator looks about 175 for all of them.
                    // Until we know more, we set it to the visual indicator in game.
                    // Normal Mode - 360
                    // Normal Mode Empowered - 240
                    // Challenge Mode - 240
                    // Challenge Mode Empowered - 156
                    uint radius = 175;
                    // Show the state for 500 ms, 250 ms before so that failures are actually visible
                    (long start, long end) lifespan = (effect.Time - 250, effect.Time + 250);
                    var circle = new CircleDecoration(radius, lifespan, color, 0.2, new PositionConnector(effect.Position));
                    environmentDecorations.Add(circle);
                }
            }
        }

        // Pool of Despair - Spread AoE on ground
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.TempleOfFebePoolOfDespair, out var poolOfDespair))
        {
            foreach (EffectEvent effect in poolOfDespair)
            {
                int duration = log.LogData.IsCM || log.LogData.IsLegendaryCM ? 120000 : 60000;
                (long start, long end) lifespan = effect.ComputeDynamicLifespan(log, duration);
                var circle = new CircleDecoration(120, lifespan, Colors.RedSkin, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circle.GetBorderDecoration(Colors.Red, 0.2));
            }
        }

        // Pool of Despair - Empowered - Spread AoE on ground
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.TempleOfFebePoolOfDespairEmpowered, out var poolOfDespairEmpowered))
        {
            foreach (EffectEvent effect in poolOfDespairEmpowered)
            {
                (long start, long end) lifespan = effect.ComputeDynamicLifespan(log, 999000);
                var circle = new CircleDecoration(240, lifespan, Colors.RedSkin, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circle.GetBorderDecoration(Colors.Red, 0.2));
            }
        }
    }

    /// <summary>
    /// Hide the Permanent Embodiments in Challenge Mode.<br></br>
    /// The Embodiments are shown during their cast events.
    /// </summary>
    /// <param name="target">The casting Embodiment.</param>
    /// <param name="log">The log.</param>
    /// <param name="replay">The Combat Replay.</param>
    /// <param name="castDuration">The cast duration of the mechanic, roughly +- 20ms leeway.</param>
    private static void AddHiddenWhileNotCasting(NPC target, ParsedEvtcLog log, CombatReplay replay, long castDuration)
    {
        var castEvents = target.GetCastEvents(log).Where(x => x.SkillID != WeaponStow && x.SkillID != WeaponSwap && x.SkillID != WeaponDraw);
        long invisibleStart = log.LogData.EvtcLogStart;
        bool startTrimmed = false;

        AgentItem cerus = GetCerusItem(log.AgentData);
        IEnumerable<Segment> invulnsApply = [];
        if (cerus != null)
        {
            invulnsApply = cerus.GetBuffStatus(log, InvulnerabilityCerus).Where(x => x.Value > 0);
        }

        foreach (CastEvent cast in castEvents)
        {
            // Spawn and despawn the Embodiment 3500 ms before the cast start and end.
            (long start, long end) = (cast.Time - 3500, cast.Time + castDuration + 3500);

            // End the cast early if Cerus gains Invulnerability for the 80% and 50% splits.
            foreach (var invulnApply in invulnsApply)
            {
                if (start <= invulnApply.Start && end > invulnApply.Start)
                {
                    end = invulnApply.Start;
                }
            }

            // If the Embodiment hasn't been trimmed yet, trim the lifespan to start on first cast and end at the log end.
            if (!startTrimmed)
            {
                replay.Trim(start, replay.TimeOffsets.end);
                startTrimmed = true;
            }
            else
            {
                // Once already trimmed, hide them at the end of cast and show at the start of the next.
                replay.Hidden.Add(new Segment(invisibleStart, start));
            }
            invisibleStart = end;
        }
        replay.Trim(replay.TimeOffsets.start, invisibleStart);
    }

    /// <summary>
    /// Adds the <see cref="BuffImages.Determined"/> icon for the invulnerable Embodiments during the split phases.
    /// </summary>
    /// <param name="target">The Embodiment.</param>
    /// <param name="log">The log.</param>
    /// <param name="replay">The Combat Replay.</param>
    private static void AddDeterminedOverhead(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        replay.Decorations.AddOverheadIcons(target.GetBuffStatus(log, InvulnerabilityEmbodiment).Where(x => x.Value > 0), target, BuffImages.Determined);
    }

    /// <summary>
    /// Adds the Cry of Rage mechanic decoration.
    /// </summary>
    /// <param name="target">The target casting.</param>
    /// <param name="log">The log.</param>
    /// <param name="replay">The Combat Replay.</param>
    /// <param name="casts">The cast events.</param>
    private static void AddCryOfRageDecoration(NPC target, ParsedEvtcLog log, CombatReplay replay, IEnumerable<CastEvent> casts)
    {
        var cryOfRage = casts.Where(x => x.SkillID == CryOfRageNM || x.SkillID == CryOfRageCM || x.SkillID == CryOfRageEmpoweredNM || x.SkillID == CryOfRageEmpoweredCM);
        foreach (CastEvent cast in cryOfRage)
        {
            uint radius = 0;
            long defaultCastDuration = 5000;
            (long start, long end) lifespan = (cast.Time, cast.Time + defaultCastDuration);
            long growing = lifespan.end;

            switch (cast.SkillID)
            {
                case CryOfRageNM:
                case CryOfRageCM:
                    radius = 1000;
                    break;
                case CryOfRageEmpoweredNM:
                case CryOfRageEmpoweredCM:
                    radius = 1250;
                    break;
                default:
                    break;
            }

            // Check if quickness was stolen
            double computedDuration = ComputeCastTimeWithQuickness(log, target, cast.Time, defaultCastDuration);
            if (computedDuration > 0)
            {
                lifespan.end = Math.Min(defaultCastDuration, (long)Math.Ceiling(computedDuration));
            }

            // Check if the mechanic got cancelled
            lifespan = ComputeMechanicLifespanWithCancellationTime(target.AgentItem, log, lifespan);

            var circle = new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target));
            replay.Decorations.AddWithGrowing(circle, growing);
        }
    }

    /// <summary>
    /// Adds the Envious Gaze mechanic decoration.
    /// </summary>
    /// <param name="target">The target casting.</param>
    /// <param name="log">The log.</param>
    /// <param name="replay">The Combat Replay.</param>
    /// <param name="casts">The cast events.</param>
    private static void AddEnviousGazeDecoration(NPC target, ParsedEvtcLog log, CombatReplay replay, IEnumerable<CastEvent> casts)
    {
        uint width = 2200;

        var enviousGaze = casts.Where(x => x.SkillID == EnviousGazeNM || x.SkillID == EnviousGazeEmpoweredNM || x.SkillID == EnviousGazeCM || x.SkillID == EnviousGazeEmpoweredCM);

        var isCerus = target.IsSpecies(TargetID.Cerus);
        var isKillableEmbodiment = target.IsSpecies(TargetID.EmbodimentOfEnvy);

        foreach (CastEvent cast in enviousGaze)
        {
            bool isEmpowered = cast.SkillID == EnviousGazeEmpoweredNM || cast.SkillID == EnviousGazeEmpoweredCM;
            long indicatorDuration = 1500;
            (long start, long end) lifespanIndicator = (cast.Time, cast.Time + indicatorDuration);
            long growing = lifespanIndicator.end;
            if (target.TryGetCurrentFacingDirection(log, lifespanIndicator.end, out var facing))
            {
                // Indicator
                // Check if quickness is still applied from a previous steal
                double computedDuration = ComputeCastTimeWithQuickness(log, target, cast.Time, indicatorDuration);
                if (computedDuration > 0)
                {
                    lifespanIndicator.end = cast.Time + Math.Min(indicatorDuration, (long)Math.Ceiling(computedDuration));
                }
                // Check if the indicator is cancelled
                lifespanIndicator = ComputeMechanicLifespanWithCancellationTime(target.AgentItem, log, lifespanIndicator);

                // Frontal indicator
                var rotation = new AngleConnector(facing);
                var agentConnector = (AgentConnector)new AgentConnector(target).WithOffset(new(width / 2, 0, 0), true);
                var rectangle = (RectangleDecoration)new RectangleDecoration(width, 100, lifespanIndicator, Colors.LightOrange, 0.2, agentConnector).UsingRotationConnector(rotation);
                replay.Decorations.AddWithGrowing(rectangle, growing);
                if (isEmpowered)
                {
                    // Opposite Indicator
                    var oppositeAgentConnector = (AgentConnector)new AgentConnector(target).WithOffset(new(-(width / 2), 0, 0), true);
                    var oppositeRectangle = (RectangleDecoration)new RectangleDecoration(width, 100, lifespanIndicator, Colors.LightOrange, 0.2, oppositeAgentConnector).UsingRotationConnector(rotation);
                    replay.Decorations.AddWithGrowing(oppositeRectangle, growing);
                }

                // Check if Petrify is casted between the end of the indicator and the start of the damage beam
                (long start, long end) = ComputeMechanicLifespanWithCancellationTime(target.AgentItem, log, (lifespanIndicator.end, lifespanIndicator.end + 950));
                if (end < lifespanIndicator.end + 950)
                {
                    continue;
                }

                // At 10%, if the embodiment is casting the indicator but the wall hasn't spawend yet and Cerus' bar is broken, the wall is interrupted and doesn't spawn.
                if (!isCerus)
                {
                    IReadOnlyList<HealthUpdateEvent> health = log.CombatData.GetHealthUpdateEvents(GetCerusItem(log.AgentData));
                    HealthUpdateEvent? hp11 = health.FirstOrDefault(x => x.HealthPercent < 11);
                    var petrify = log.CombatData.GetAnimatedCastData(PetrifySkill);
                    if (hp11 != null)
                    {
                        AnimatedCastEvent? pet = petrify.FirstOrDefault(x => x.Time > hp11.Time);
                        // If petrify at 10% finishes casting before the end of the envy indicator, we don't show the rotating beams.
                        if (pet != null && Math.Abs(lifespanIndicator.start - pet.Time) < 5000 && pet.EndTime < lifespanIndicator.end + 950)
                        {
                            continue;
                        }
                    }
                }

                // Frontal Damage Beam
                (long start, long end) lifespanDamage = (lifespanIndicator.end + 950, lifespanIndicator.end + 10750);
                (long start, long end) lifespanDamageCancelled = lifespanDamage;
                lifespanDamageCancelled = ComputeMechanicLifespanWithCancellationTime(target.AgentItem, log, lifespanDamage);
                double millisecondsPerDegree = (double)(lifespanDamage.end - lifespanDamage.start) / 360;
                double degreesRotated = (lifespanDamageCancelled.end - lifespanDamageCancelled.start) / millisecondsPerDegree;
                var rotation2 = new SpinningConnector(facing, (float)degreesRotated);
                var rectangle2 = (RectangleDecoration)new RectangleDecoration(width, 100, lifespanDamageCancelled, Colors.Red, 0.2, agentConnector).UsingRotationConnector(rotation2);
                replay.Decorations.Add(rectangle2);
                if (isEmpowered)
                {
                    // Opposite Damage Beam
                    (long start, long end) lifespanDamageOpposite = (lifespanIndicator.end + 950, lifespanIndicator.end + 5850);
                    (long start, long end) lifespanDamageOppositeCancelled = lifespanDamageOpposite;
                    // In game bug, when ending the 80% and 50% split phases and transitioning back to Cerus, the back wall of the empowered envy does not despawn.
                    // If this gets fixed in game, add a game build versioning check.
                    if (!isKillableEmbodiment)
                    {
                        lifespanDamageOppositeCancelled = ComputeMechanicLifespanWithCancellationTime(target.AgentItem, log, lifespanDamageOpposite);
                    }
                    double millisecondsPerDegreeOpposite = (double)(lifespanDamageOpposite.end - lifespanDamageOpposite.start) / 360;
                    double degreedRotatedOpposite = (lifespanDamageOppositeCancelled.end - lifespanDamageOppositeCancelled.start) / millisecondsPerDegreeOpposite;
                    var rotation3 = new SpinningConnector(facing, (float)degreedRotatedOpposite);
                    
                    // The bug makes the beam continue while the embodiment has despawned, so we use the agent position for a PositionConnector instead of AgentConnector.
                    ParametricPoint3D? position = target.GetCombatReplayActivePolledPositions(log).FirstOrDefault(x => x!= null && x.Value.Time > lifespanDamage.start && x.Value.Time <= lifespanDamage.end);
                    if (position != null)
                    {
                        var connector = new PositionConnector(position.Value.XYZ).WithOffset(new(-(width / 2), 0, 0), true);
                        var rectangle3 = (RectangleDecoration)new RectangleDecoration(width, 100, lifespanDamageOppositeCancelled, Colors.Red, 0.2, connector).UsingRotationConnector(rotation3);
                        replay.Decorations.Add(rectangle3);
                    }
                    else
                    {
                        // Fallback for security
                        var oppositeAgentConnector = (AgentConnector)new AgentConnector(target).WithOffset(new(-(width / 2), 0, 0), true);
                        var rectangle3 = (RectangleDecoration)new RectangleDecoration(width, 100, lifespanDamageOppositeCancelled, Colors.Red, 0.2, oppositeAgentConnector).UsingRotationConnector(rotation3);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Adds the Insatiable Hunger mechanic decoration.
    /// </summary>
    /// <param name="target">The target casting.</param>
    /// <param name="log">The log.</param>
    /// <param name="replay">The Combat Replay.</param>
    private static void AddInsatiableHungerDecoration(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        var orbs = log.CombatData.GetMissileEventsBySrc(target.AgentItem);
        foreach (MissileEvent orb in orbs)
        {
            if (orb.SkillID == InsatiableHungerSmallOrbSkillNM
                    || orb.SkillID == InsatiableHungerSmallOrbSkillCM
                    || orb.SkillID == InsatiableHungerSmallOrbEmpoweredSkillNM
                    || orb.SkillID == InsatiableHungerSmallOrbEmpoweredSkillCM
                    || orb.SkillID == InsatiableHungerPermanentEmbodimentSmallOrbSkillCM
                    || orb.SkillID == InsatiableHungerPermanentEmbodimentSmallOrbEmpoweredSkillCM)
            {
                replay.Decorations.AddNonHomingMissile(log, orb, Colors.RedSkin, 0.8, 10);
            }
            else
            {
                long end = orb.RemoveEvent?.Time ?? log.LogData.LogEnd;
                for (int i = 0; i < orb.LaunchEvents.Count; i++)
                {
                    MissileLaunchEvent launch = orb.LaunchEvents[i];
                    (long start, long end ) lifespan = (launch.Time, i != orb.LaunchEvents.Count - 1 ? orb.LaunchEvents[i + 1].Time : end);
                    var connector = new InterpolationConnector([new ParametricPoint3D(launch.LaunchPosition, lifespan.start), launch.GetFinalPosition(lifespan)], Connector.InterpolationMethod.Linear);
                    replay.Decorations.Add(new CircleDecoration(30, lifespan, Colors.Black, 0.5, connector));
                    replay.Decorations.Add(new DoughnutDecoration(30, 40, lifespan, Colors.RedSkin, 0.8, connector));
                }
            }
        }
    }

    /// <summary>
    /// Computes the lifespan of a mechanic.<br></br>
    /// If Cerus is casting a mechanic and gains a breakbar at 80, 50 or 10%, it gets cancelled.<br></br>
    /// If a Permanent Embodiment is casting a mechanic, it gets cancelled at the start of the split phase, when Cerus gains <see cref="InvulnerabilityCerus"/>.<br></br>
    /// If a Killable Embodiment is casting a mechanic, it gets cancelled at the end of the split phase, when Cerus loses <see cref="InvulnerabilityCerus"/>.
    /// </summary>
    /// <param name="target">The target casting.</param>
    /// <param name="log">The log.</param>
    /// <param name="lifespan">The supposed lifespan.</param>
    /// <returns>The computed lifespan.</returns>
    private static (long start, long end) ComputeMechanicLifespanWithCancellationTime(AgentItem target, ParsedEvtcLog log, (long start, long end) lifespan)
    {
        SingleActor? cerus = log.LogData.GetMainTargets(log).FirstOrDefault(x => x.IsSpecies(TargetID.Cerus));
        if (cerus != null)
        {
            // If Cerus is casting a mechanic, cancel it when he begins casting Petrify
            if (target.IsSpecies(TargetID.Cerus))
            {
                var casts = cerus.GetCastEvents(log).Where(x => x.SkillID == PetrifySkill);
                foreach (CastEvent cast in casts)
                {
                    if (lifespan.start <= cast.Time && lifespan.end > cast.Time)
                    {
                        lifespan.end = Math.Min(lifespan.end, cast.Time);
                    }
                }
            }
            else
            {
                var invuls = cerus.GetBuffStatus(log, InvulnerabilityCerus);
                // If a permanent Embodiment is casting a mechanic, cancel it when Cerus gains invulnerability (start 80% and 50% split phases)
                var invulnsApply = invuls.Where(x => x.Value > 0);
                foreach (var invulnApply in invulnsApply)
                {
                    if (lifespan.start <= invulnApply.Start && lifespan.end > invulnApply.Start)
                    {
                        lifespan.end = Math.Min(lifespan.end, invulnApply.Start);
                    }
                    else
                    {
                        // If a killable Embodiment is casting a mechanic, cancel it when Cerus loses invulnerability (end 80% and 50% split phases)
                        var invulnsRemove = invuls.Where(x => x.Value == 0);
                        foreach (var invulnRemove in invulnsRemove)
                        {
                            if (lifespan.start <= invulnRemove.Start && lifespan.end > invulnRemove.Start)
                            {
                                lifespan.end = Math.Min(lifespan.end, invulnRemove.Start);
                            }
                        }
                    }
                }
            }
        }
        return lifespan;
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);

        if (log.LogData.IsCM || log.LogData.IsLegendaryCM)
        {
            AgentItem cerus = GetCerusItem(log.AgentData);
            if (cerus != null)
            {
                var empoweredBuffs = new List<long>()
                {
                    EmpoweredDespairCerus,
                    EmpoweredEnvyCerus,
                    EmpoweredGluttonyCerus,
                    EmpoweredMaliceCerus,
                    EmpoweredRageCerus,
                    EmpoweredRegretCerus
                };
                if (empoweredBuffs.Count(x => cerus.GetBuffStatus(log, x).Any(x => x.Value > 0)) == 6)
                {
                    InstanceBuffs.Add((log.Buffs.BuffsByIDs[AchievementEligibilityApathetic], 1));
                }
            }
        }
    }

    private static AgentItem GetCerusItem(AgentData agentData)
    {
        return agentData.GetNPCsByID(TargetID.Cerus).FirstOrDefault()! ?? throw new MissingKeyActorsException("Cerus not found");
    }
}
