using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class TempleOfFebe : SecretOfTheObscureStrike
    {
        private static HashSet<long> UnboundOptimismSkillIDs = new HashSet<long>()
        {
            WailOfDespairCM, WailOfDespairEmpoweredCM, PoolOfDespairCM, PoolOfDespairEmpoweredCM
        };

        public TempleOfFebe(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstBuffApplyMechanic(Insatiable, "Insatiable", new MechanicPlotlySetting(Symbols.Hourglass, Colors.Pink), "Ins.A", "Insatiable Applied (Absorbed Gluttony Orb)", "Insatiable Application", 0),
                new PlayerDstHitMechanic(new long [] { CrushingRegretNM, CrushingRegretCM }, "Crushing Regret", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkGreen), "CrushReg.H", "Hit by Crushing Regret (Green)", "Crushing Regret Hit", 0),
                new PlayerDstHitMechanic(new long [] { CrushingRegretEmpoweredNM, CrushingRegretEmpoweredCM }, "Crushing Regret", new MechanicPlotlySetting(Symbols.Circle, Colors.GreenishYellow), "Emp.CrushReg.H", "Hit by Empowered Crushing Regret (Green)", "Empowered Crushing Regret Hit", 0),
                new PlayerDstEffectMechanic(EffectGUIDs.TempleOfFebeCerusGreen, "Crushing Regret", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Green), "Green.A", "Crushing Regret Applied (Green)", "Crushing Regret Application", 0),
                new PlayerDstHitMechanic(new long [] { WailOfDespairNM, WailOfDespairCM }, "Wail of Despair", new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "WailDesp.H", "Hit by Wail of Despair (Spread Player AoE)", "Wail of Despair Hit", 0),
                new PlayerDstHitMechanic(new long [] { WailOfDespairEmpoweredNM, WailOfDespairEmpoweredCM }, "Wail of Despair", new MechanicPlotlySetting(Symbols.Circle, Colors.Orange), "Emp.WailDesp.H", "Hit by Empowered Wail of Despair (Spread Player AoE)", "Empowered Wail of Despair Hit", 0),
                new PlayerDstHitMechanic(new long [] { PoolOfDespairNM, PoolOfDespairCM }, "Pool of Despair", new MechanicPlotlySetting(Symbols.Circle, Colors.Red), "PoolDesp.H", "Hit by Pool of Despair (Spread Ground AoE)", "Pool of Despair Hit", 0),
                new PlayerDstHitMechanic(new long [] { PoolOfDespairEmpoweredNM, PoolOfDespairEmpoweredCM }, "Pool of Despair", new MechanicPlotlySetting(Symbols.Circle, Colors.RedSkin), "Emp.PoolDesp.H", "Hit by Empowered Pool of Despair (Spread Ground AoE)", "Empowered Pool of Despair Hit", 0),
                new PlayerDstHitMechanic(new long [] { EnviousGazeNM, EnviousGazeCM }, "Envious Gaze", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Red), "EnvGaz.H", "Hit by Envious Gaze (Wall/Beam)", "Envious Gaze Hit", 0),
                new PlayerDstHitMechanic(new long [] { EnviousGazeEmpoweredNM, EnviousGazeEmpoweredRearNM, EnviousGazeEmpoweredCM, EnviousGazeEmpoweredRearCM }, "Envious Gaze", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "Emp.EnvGaz.H", "Hit by Empowered Envious Gaze (Double Wall/Beam)", "Empowered Envious Gaze Hit", 0),
                new PlayerDstHitMechanic(new long [] { MaliciousIntentSpawnDamageNM, MaliciousIntentSpawnDamageCM }, "Malicious Intent", new MechanicPlotlySetting(Symbols.Y, Colors.White), "MalInt.H", "Hit by Malicious Intent (Malicious Shadow Spawn)", "Malicious Intent Hit", 0),
                new PlayerDstHitMechanic(new long [] { CryOfRageNM, CryOfRageCM }, "Cry of Rage", new MechanicPlotlySetting(Symbols.CircleX, Colors.LightOrange), "CryRage.H", "Hit by Cry of Rage", "Cry of Rage Hit", 0),
                new PlayerDstHitMechanic(new long [] { CryOfRageEmpoweredNM, CryOfRageEmpoweredCM }, "Cry of Rage", new MechanicPlotlySetting(Symbols.CircleX, Colors.Orange), "Emp.CryRage.H", "Hit by Empowered Cry of Rage", "Empowered Cry of Rage Hit", 0),
                new PlayerDstHitMechanic(new long [] { EnragedSmashNM, EnragedSmashCM }, "Enraged Smash", new MechanicPlotlySetting(Symbols.Star, Colors.Red), "EnrSmash.H", "Hit by Enraged Smash", "Hit by Enraged Smash", 0),
                new PlayerDstHitMechanic(new long [] { EnragedSmashNM, EnragedSmashCM }, "Enraged Smash", new MechanicPlotlySetting(Symbols.Star, Colors.DarkRed), "EnrSmash.D", "Downed to Enraged Smash", "Downed to Enraged Smash", 0).UsingChecker((ahde, log) => ahde.HasDowned),
                new PlayerDstHitMechanic(PetrifyDamage, "Petrify", new MechanicPlotlySetting(Symbols.Pentagon, Colors.Teal), "Pet.H", "Hit by Petrify", "Petrify Hit", 0),
                new GenericCombatEventListMechanic<AbstractTimeCombatEvent>("Unbounded Optimism", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.RedSkin), "UnbOpt.Achiv", "Achievement Eligibility: Unbounded Optimism", "Unbounded Optimism", 0, false, (log, agentItem) =>
                    {
                        AbstractSingleActor actor = log.FindActor(agentItem);
                        var eligibilityRemovedEvents = new List<AbstractTimeCombatEvent>();
                        eligibilityRemovedEvents.AddRange(actor.GetDamageTakenEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd).Where(x => UnboundOptimismSkillIDs.Contains(x.SkillId) && x.HasHit));
                        IReadOnlyList<DeadEvent> deads = log.CombatData.GetDeadEvents(agentItem);
                        // In case player is dead but death event did not happen during encounter
                        if (agentItem.IsDead(log, log.FightData.FightEnd) && !deads.Any(x => x.Time >= log.FightData.FightStart && x.Time <= log.FightData.FightEnd))
                        {
                            eligibilityRemovedEvents.Add(new PlaceHolderTimeCombatEvent(log.FightData.FightEnd - 1));
                        }
                        else
                        {
                            eligibilityRemovedEvents.AddRange(deads);
                        }
                        IReadOnlyList<DespawnEvent> despawns = log.CombatData.GetDespawnEvents(agentItem);
                        // In case player is DC but DC event did not happen during encounter
                        if (agentItem.IsDC(log, log.FightData.FightEnd) && !despawns.Any(x => x.Time >= log.FightData.FightStart && x.Time <= log.FightData.FightEnd))
                        {
                            eligibilityRemovedEvents.Add(new PlaceHolderTimeCombatEvent(log.FightData.FightEnd - 1));
                        }
                        else
                        {
                            eligibilityRemovedEvents.AddRange(despawns);
                        }
                        eligibilityRemovedEvents = eligibilityRemovedEvents.OrderBy(x => x.Time).ToList();
                        return eligibilityRemovedEvents;
                    })
                    .UsingEnable(x => x.FightData.IsCM || x.FightData.IsLegendaryCM)
                    .UsingAchievementEligibility(true),
                new PlayerDstEffectMechanic(EffectGUIDs.TempleOfFebeMaliciousIntentTether, "Malicious Intent", new MechanicPlotlySetting(Symbols.Bowtie, Colors.DarkGreen), "MalInt.A", "Malicious Intent Target", "Targetted by Malicious Intent", 0),
                new EnemyDstBuffApplyMechanic(EmpoweredCerus, "Empowered", new MechanicPlotlySetting(Symbols.Square, Colors.Red), "Emp.A", "Gained Empowered", "Empowered Application", 0),
                new EnemyDstBuffApplyMechanic(EmpoweredDespairCerus, "Empowered Despair", new MechanicPlotlySetting(Symbols.Square, Colors.Black), "EmpDesp.A", "Gained Empowered Despair", "Empowered Despair Application", 0),
                new EnemyDstBuffApplyMechanic(EmpoweredEnvyCerus, "Empowered Envy", new MechanicPlotlySetting(Symbols.Square, Colors.Blue), "EmpEnvy.A", "Gained Empowered Envy", "Empowered Envy Application", 0),
                new EnemyDstBuffApplyMechanic(EmpoweredGluttonyCerus, "Empowered Gluttony", new MechanicPlotlySetting(Symbols.Square, Colors.Brown), "EmpGlu.A", "Gained Empowered Gluttony", "Empowered Gluttony Application", 0),
                new EnemyDstBuffApplyMechanic(EmpoweredMaliceCerus, "Empowered Malice", new MechanicPlotlySetting(Symbols.Square, Colors.LightBlue), "EmpMal.A", "Gained Empowered Malice", "Empowered Malice Application", 0),
                new EnemyDstBuffApplyMechanic(EmpoweredRageCerus, "Empowered Rage", new MechanicPlotlySetting(Symbols.Square, Colors.LightOrange), "EmpRage.A", "Gained Empowered Rage", "Empowered Rage Application", 0),
                new EnemyDstBuffApplyMechanic(EmpoweredRegretCerus, "Empowered Regret", new MechanicPlotlySetting(Symbols.Square, Colors.LightGrey), "EmpReg.A", "Gained Empowered Regret", "Empowered Regret Application", 0),
                new EnemyDstBuffApplyMechanic(Invulnerability757, "Invulnerability", new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightRed), "Despair.K", "Embodiment of Despair Killed", "Despair Killed", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TrashID.EmbodimentOfDespair) && !bae.To.HasBuff(log, EmpoweredDespairEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, "Invulnerability", new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightBlue), "Envy.K", "Embodiment of Envy Killed", "Envy Killed", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TrashID.EmbodimentOfEnvy) && !bae.To.HasBuff(log, EmpoweredEnvyEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, "Invulnerability", new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightOrange), "Gluttony.K", "Embodiment of Gluttony Killed", "Gluttony Killed", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TrashID.EmbodimentOfGluttony) && ! bae.To.HasBuff(log, EmpoweredGluttonyEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, "Invulnerability", new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightGrey), "Malice.K", "Embodiment of Malice Killed", "Malice Killed", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TrashID.EmbodimentOfMalice) && !bae.To.HasBuff(log, EmpoweredMaliceEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, "Invulnerability", new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightPurple), "Rage.K", "Embodiment of Rage Killed", "Rage Killed", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TrashID.EmbodimentOfRage) && !bae.To.HasBuff(log, EmpoweredRageEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, "Invulnerability", new MechanicPlotlySetting(Symbols.StarOpen, Colors.White), "Regret.K", "Embodiment of Regret Killed", "Regret Killed", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TrashID.EmbodimentOfRegret) && !bae.To.HasBuff(log, EmpoweredRegretEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, "Invulnerability", new MechanicPlotlySetting(Symbols.StarOpen, Colors.Red), "Emp.Despair.K", "Empowered Embodiment of Despair Killed", "Empowered Despair Killed", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TrashID.EmbodimentOfDespair) && bae.To.HasBuff(log, EmpoweredDespairEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, "Invulnerability", new MechanicPlotlySetting(Symbols.StarOpen, Colors.Blue), "Emp.Envy.K", "Empowered Embodiment of Envy Killed", "Empowered Envy Killed", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TrashID.EmbodimentOfEnvy) && bae.To.HasBuff(log, EmpoweredEnvyEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, "Invulnerability", new MechanicPlotlySetting(Symbols.StarOpen, Colors.Orange), "Emp.Gluttony.K", "Empowered Embodiment of Gluttony Killed", "Empowered Gluttony Killed", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TrashID.EmbodimentOfGluttony) && bae.To.HasBuff(log, EmpoweredGluttonyEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, "Invulnerability", new MechanicPlotlySetting(Symbols.StarOpen, Colors.Grey), "Emp.Malice.K", "Empowered Embodiment of Malice Killed", "Empowered Malice Killed", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TrashID.EmbodimentOfMalice) && bae.To.HasBuff(log, EmpoweredMaliceEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, "Invulnerability", new MechanicPlotlySetting(Symbols.StarOpen, Colors.Purple), "Emp.Rage.K", "Empowered Embodiment of Rage Killed", "Empowered Rage Killed", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TrashID.EmbodimentOfRage) && bae.To.HasBuff(log, EmpoweredRageEmbodiment, bae.Time)),
                new EnemyDstBuffApplyMechanic(Invulnerability757, "Invulnerability", new MechanicPlotlySetting(Symbols.StarOpen, Colors.Black), "Emp.Regret.K", "Empowered Embodiment of Regret Killed", "Empowered Regret Killed", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TrashID.EmbodimentOfRegret) && bae.To.HasBuff(log, EmpoweredRegretEmbodiment, bae.Time)),
                new EnemyCastStartMechanic(new long [] { CrushingRegretNM, CrushingRegretEmpoweredNM, CrushingRegretCM, CrushingRegretEmpoweredCM }, "Crushing Regret", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.LightMilitaryGreen), "CrushReg.C", "Casted Crushing Regret", "Crushing Regret Cast", 0),
                new EnemySrcEffectMechanic(EffectGUIDs.TempleOfFebeGreenSuccess, "Crushing Regret Success", new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "CrushReg.C.S", "Crushing Regret Successful", "Success Crushing Regret", 0),
                new EnemySrcEffectMechanic(EffectGUIDs.TempleOfFebeGreenFailure, "Crushing Regret Failed", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "CrushReg.C.F", "Crushing Regret Failed", "Failed Crushing Regret", 0),
                new EnemyCastStartMechanic(new long [] { WailOfDespairNM, WailOfDespairEmpoweredNM, WailOfDespairCM, WailOfDespairEmpoweredCM }, "Wail of Despair", new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.LightOrange), "WailDesp.C", "Casted Wail of Despair", "Wail of Despair Cast", 0),
                new EnemyCastStartMechanic(new long [] { EnviousGazeNM, EnviousGazeCM, EnviousGazeEmpoweredNM, EnviousGazeEmpoweredCM }, "Envious Gaze", new MechanicPlotlySetting(Symbols.TriangleDownOpen, Colors.Red), "EnvGaz.C", "Casted Envious Gaze", "Envious Gaze Cast", 0),
                new EnemyCastStartMechanic(new long [] { MaliciousIntentNM, MaliciousIntentEmpoweredNM, MaliciousIntentCM, MaliciousIntentEmpoweredCM }, "Malicious Intent", new MechanicPlotlySetting(Symbols.Bowtie, Colors.RedSkin), "MalInt.C", "Casted Malicious Intent", "Malicious Intent Cast", 0),
                new EnemyCastStartMechanic(new long [] { InsatiableHungerSkillNM, InsatiableHungerEmpoweredSkillNM, InsatiableHungerSkillCM, InsatiableHungerEmpoweredSkillCM }, "Insatiable Hunger", new MechanicPlotlySetting(Symbols.HourglassOpen, Colors.Pink), "InsHun.C", "Casted Insatiable Hunger", "Insatiable Hunger Cast", 0),
                new EnemyCastStartMechanic(new long [] { CryOfRageNM, CryOfRageEmpoweredNM, CryOfRageCM, CryOfRageEmpoweredCM }, "Cry of Rage", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.LightOrange), "CryRage.C", "Casted Cry of Rage", "Cry of Rage Cast", 0),
                new EnemyCastStartMechanic(new long [] { EnragedSmashNM, EnragedSmashCM }, "Enraged Smash", new MechanicPlotlySetting(Symbols.Star, Colors.Blue), "EnrSmash.C", "Casted Enraged Smash", "Enraged Smash Cast", 0),
                new EnemyCastStartMechanic(PetrifySkill, "Petrify", new MechanicPlotlySetting(Symbols.Pentagon, Colors.Yellow), "Pet.C", "Casted Petrify", "Petrify breakbar start", 0),
                new EnemySrcHitMechanic(PetrifyDamage, "Petrify", new MechanicPlotlySetting(Symbols.Pentagon, Colors.DarkTeal), "Pet.F", "Petrify hit players and healed Cerus", "Petrify breakbar fail", 100),
            }
            );
            Icon = EncounterIconTempleOfFebe;
            Extension = "tmplfeb";
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayTempleOfFebe,
                            (1149, 1149),
                            (-2088, -6124, 2086, -1950));
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>()
            {
                (int)TargetID.Cerus,
                (int)TrashID.EmbodimentOfDespair,
                (int)TrashID.EmbodimentOfEnvy,
                (int)TrashID.EmbodimentOfGluttony,
                (int)TrashID.EmbodimentOfMalice,
                (int)TrashID.EmbodimentOfRage,
                (int)TrashID.EmbodimentOfRegret,
                (int)TrashID.PermanentEmbodimentOfDespair,
                (int)TrashID.PermanentEmbodimentOfEnvy,
                (int)TrashID.PermanentEmbodimentOfGluttony,
                (int)TrashID.PermanentEmbodimentOfMalice,
                (int)TrashID.PermanentEmbodimentOfRage,
                (int)TrashID.PermanentEmbodimentOfRegret,
                (int)TrashID.MaliciousShadow,
                (int)TrashID.MaliciousShadowCM,
            };
        }

        protected override Dictionary<int, int> GetTargetsSortIDs()
        {
            return new Dictionary<int, int>()
            {
                { (int)TargetID.Cerus, 0 },
                { (int)TrashID.EmbodimentOfDespair, 1 },
                { (int)TrashID.EmbodimentOfEnvy, 1 },
                { (int)TrashID.EmbodimentOfGluttony, 1 },
                { (int)TrashID.EmbodimentOfMalice, 1 },
                { (int)TrashID.EmbodimentOfRage, 1 },
                { (int)TrashID.EmbodimentOfRegret, 1 },
                { (int)TrashID.MaliciousShadow, 2 },
                { (int)TrashID.MaliciousShadowCM, 2 },
                { (int)TrashID.PermanentEmbodimentOfDespair, 3 },
                { (int)TrashID.PermanentEmbodimentOfEnvy, 3 },
                { (int)TrashID.PermanentEmbodimentOfGluttony, 3 },
                { (int)TrashID.PermanentEmbodimentOfMalice, 3 },
                { (int)TrashID.PermanentEmbodimentOfRage, 3 },
                { (int)TrashID.PermanentEmbodimentOfRegret, 3 },
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Cerus)) ?? throw new MissingKeyActorsException("Cerus not found");
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Invul check
            List<PhaseData> invulnPhases = GetPhasesByInvul(log, InvulnerabilityCerus, mainTarget, true, true);
            phases.AddRange(invulnPhases);
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    phase.Name = "Split " + (i) / 2;
                    var ids = new List<int>
                    {
                        (int)TrashID.EmbodimentOfDespair,
                        (int)TrashID.EmbodimentOfEnvy,
                        (int)TrashID.EmbodimentOfGluttony,
                        (int)TrashID.EmbodimentOfMalice,
                        (int)TrashID.EmbodimentOfRage,
                        (int)TrashID.EmbodimentOfRegret,
                    };
                    AddTargetsToPhaseAndFit(phase, ids, log);
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(mainTarget);
                }
            }
            // Enraged Smash phase - After 10% bar is broken
            AbstractCastEvent enragedSmash = mainTarget.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.SkillId == EnragedSmashNM || x.SkillId == EnragedSmashCM).FirstOrDefault();
            if (enragedSmash != null)
            {
                var phase = new PhaseData(enragedSmash.Time, log.FightData.FightEnd, "Enraged Smash");
                phase.AddTarget(mainTarget);
                phases.Add(phase);
                // Sub Phase for 50%-10%
                PhaseData phase3 = invulnPhases.LastOrDefault(x => x.InInterval(enragedSmash.Time));
                if (phase3 != null)
                {
                    var phase50_10 = new PhaseData(phase3.Start, enragedSmash.Time, "50%-10%");
                    phase50_10.AddTarget(mainTarget);
                    phases.Add(phase50_10);
                }
            }
            return phases;
        }

        internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            var embodimentIDs = new List<TrashID>()
            {
                TrashID.EmbodimentOfDespair,
                TrashID.EmbodimentOfEnvy,
                TrashID.EmbodimentOfGluttony,
                TrashID.EmbodimentOfMalice,
                TrashID.EmbodimentOfRage,
                TrashID.EmbodimentOfRegret,
            };
            bool refresh = false;
            foreach (TrashID embodimentID in embodimentIDs)
            {
                foreach (AgentItem embodiment in agentData.GetNPCsByID(embodimentID))
                {
                    if (embodiment.FirstAware < 0)
                    {
                        refresh = true;
                        switch (embodiment.ID)
                        {
                            case (int)TrashID.EmbodimentOfDespair:
                                embodiment.OverrideID(TrashID.PermanentEmbodimentOfDespair);
                                break;
                            case (int)TrashID.EmbodimentOfEnvy:
                                embodiment.OverrideID(TrashID.PermanentEmbodimentOfEnvy);
                                break;
                            case (int)TrashID.EmbodimentOfGluttony:
                                embodiment.OverrideID(TrashID.PermanentEmbodimentOfGluttony);
                                break;
                            case (int)TrashID.EmbodimentOfMalice:
                                embodiment.OverrideID(TrashID.PermanentEmbodimentOfMalice);
                                break;
                            case (int)TrashID.EmbodimentOfRage:
                                embodiment.OverrideID(TrashID.PermanentEmbodimentOfRage);
                                break;
                            case (int)TrashID.EmbodimentOfRegret:
                                embodiment.OverrideID(TrashID.PermanentEmbodimentOfRegret);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            if (refresh)
            {
                agentData.Refresh();
            }
            base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
            int[] curEmbodiments = new[] { 1, 1, 1, 1, 1, 1 };
            int curShadow = 1;
            foreach (AbstractSingleActor target in Targets)
            {
                switch (target.ID)
                {
                    case (int)TrashID.EmbodimentOfDespair:
                        CombatItem despair = combatData.FirstOrDefault(x => x.SkillID == EmpoweredDespairEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                        target.OverrideName(target.Character + " (P" + curEmbodiments[0]++ + ")");
                        if (despair != null && Math.Abs(target.FirstAware - despair.Time) <= ServerDelayConstant)
                        {
                            target.OverrideName("Empowered " + target.Character);
                        }
                        break;
                    case (int)TrashID.EmbodimentOfEnvy:
                        CombatItem envy = combatData.FirstOrDefault(x => x.SkillID == EmpoweredEnvyEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                        target.OverrideName(target.Character + " (P" + curEmbodiments[1]++ + ")");
                        if (envy != null && Math.Abs(target.FirstAware - envy.Time) <= ServerDelayConstant)
                        {
                            target.OverrideName("Empowered " + target.Character);
                        }
                        break;
                    case (int)TrashID.EmbodimentOfGluttony:
                        CombatItem gluttony = combatData.FirstOrDefault(x => x.SkillID == EmpoweredGluttonyEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                        target.OverrideName(target.Character + " (P" + curEmbodiments[2]++ + ")");
                        if (gluttony != null && Math.Abs(target.FirstAware - gluttony.Time) <= ServerDelayConstant)
                        {
                            target.OverrideName("Empowered " + target.Character);
                        }
                        break;
                    case (int)TrashID.EmbodimentOfMalice:
                        CombatItem malice = combatData.FirstOrDefault(x => x.SkillID == EmpoweredMaliceEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                        target.OverrideName(target.Character + " (P" + curEmbodiments[3]++ + ")");
                        if (malice != null && Math.Abs(target.FirstAware - malice.Time) <= ServerDelayConstant)
                        {
                            target.OverrideName("Empowered " + target.Character);
                        }
                        break;
                    case (int)TrashID.EmbodimentOfRage:
                        CombatItem rage = combatData.FirstOrDefault(x => x.SkillID == EmpoweredRageEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                        target.OverrideName(target.Character + " (P" + curEmbodiments[4]++ + ")");
                        if (rage != null && Math.Abs(target.FirstAware - rage.Time) <= ServerDelayConstant)
                        {
                            target.OverrideName("Empowered " + target.Character);
                        }
                        break;
                    case (int)TrashID.EmbodimentOfRegret:
                        CombatItem regret = combatData.FirstOrDefault(x => x.SkillID == EmpoweredRegretEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                        target.OverrideName(target.Character + " (P" + curEmbodiments[5]++ + ")");
                        if (regret != null && Math.Abs(target.FirstAware - regret.Time) <= ServerDelayConstant)
                        {
                            target.OverrideName("Empowered " + target.Character);
                        }
                        break;
                    case (int)TrashID.MaliciousShadow:
                    case (int)TrashID.MaliciousShadowCM:
                        target.OverrideName(target.Character + " " + (curShadow++));
                        break;
                    case (int)TrashID.PermanentEmbodimentOfDespair:
                    case (int)TrashID.PermanentEmbodimentOfEnvy:
                    case (int)TrashID.PermanentEmbodimentOfGluttony:
                    case (int)TrashID.PermanentEmbodimentOfMalice:
                    case (int)TrashID.PermanentEmbodimentOfRage:
                    case (int)TrashID.PermanentEmbodimentOfRegret:
                        target.OverrideName(target.Character + " (Permanent)");
                        break;
                    default:
                        break;
                }
            }
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor cerus = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Cerus)) ?? throw new MissingKeyActorsException("Cerus not found");
            var cerusHP = cerus.GetHealth(combatData);
            if (cerusHP > 130e6)
            {
                return FightData.EncounterMode.LegendaryCM;
            }
            return (cerusHP > 100e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;

        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Temple of Febe";
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

            switch (target.ID)
            {
                case (int)TargetID.Cerus:
                    replay.AddHideByBuff(target, log, InvulnerabilityCerus);
                    AddCryOfRageDecoration(target, log, replay, casts);
                    AddEnviousGazeDecoration(target, log, replay, casts);
                    AddMaliciousIntentDecoration(target, log, replay, casts);
                    var enragedSmash = casts.Where(x => x.SkillId == EnragedSmashNM || x.SkillId == EnragedSmashCM).ToList();
                    foreach (AbstractCastEvent cast in enragedSmash)
                    {
                        // Cast time is 750, we only show a quick pulse of damage
                        (long start, long end) lifespan = (cast.Time + 750, cast.Time + 1000);
                        var circle = new CircleDecoration(2500, lifespan, Colors.RedSkin, 0.1, new AgentConnector(target));
                        replay.Decorations.Add(circle);
                    }
                    break;
                case (int)TrashID.EmbodimentOfDespair:
                    AddDeterminedOverhead(target, log, replay);
                    break;
                case (int)TrashID.PermanentEmbodimentOfDespair:
                    AddHiddenWhileNotCasting(target, log, replay, 6350);
                    break;
                case (int)TrashID.EmbodimentOfEnvy:
                    AddDeterminedOverhead(target, log, replay);
                    AddEnviousGazeDecoration(target, log, replay, casts);
                    break;
                case (int)TrashID.PermanentEmbodimentOfEnvy:
                    AddHiddenWhileNotCasting(target, log, replay, 13750);
                    AddEnviousGazeDecoration(target, log, replay, casts);
                    break;
                case (int)TrashID.EmbodimentOfGluttony:
                    AddDeterminedOverhead(target, log, replay);
                    break;
                case (int)TrashID.PermanentEmbodimentOfGluttony:
                    AddHiddenWhileNotCasting(target, log, replay, 13720);
                    break;
                case (int)TrashID.EmbodimentOfMalice:
                    AddDeterminedOverhead(target, log, replay);
                    AddMaliciousIntentDecoration(target, log, replay, casts);
                    break;
                case (int)TrashID.PermanentEmbodimentOfMalice:
                    AddHiddenWhileNotCasting(target, log, replay, 3670);
                    AddMaliciousIntentDecoration(target, log, replay, casts);
                    break;
                case (int)TrashID.EmbodimentOfRage:
                    AddDeterminedOverhead(target, log, replay);
                    AddCryOfRageDecoration(target, log, replay, casts);
                    break;
                case (int)TrashID.PermanentEmbodimentOfRage:
                    AddHiddenWhileNotCasting(target, log, replay, 7660);
                    AddCryOfRageDecoration(target, log, replay, casts);
                    break;
                case (int)TrashID.EmbodimentOfRegret:
                    AddDeterminedOverhead(target, log, replay);
                    break;
                case (int)TrashID.PermanentEmbodimentOfRegret:
                    AddHiddenWhileNotCasting(target, log, replay, 8350);
                    break;
                default:
                    break;
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);

            // Crushing Regret (Green)
            if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.TempleOfFebeCerusGreen, out IReadOnlyList<EffectEvent> crushingRegrets))
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
                    replay.AddDecorationWithGrowing(circle, growing, true);
                }
            }

            // Wail of Despair - Spread AoE on player
            if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.TempleOfFebeWailOfDespair, out IReadOnlyList<EffectEvent> wailsOfDespair))
            {
                foreach (EffectEvent effect in wailsOfDespair)
                {
                    (long start, long end) lifespan = effect.ComputeLifespan(log, 5000);
                    long growing = lifespan.end;
                    lifespan = ComputeMechanicLifespanWithCancellationTime(effect.Src, log, lifespan);
                    var circle = new CircleDecoration(120, lifespan, Colors.LightOrange, 0.2, new AgentConnector(p));
                    replay.AddDecorationWithGrowing(circle, growing);
                }
            }

            // Wail of Despair - Empowered - Spread AoE on player
            if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.TempleOfFebeWailOfDespairEmpowered, out IReadOnlyList<EffectEvent> wailsOfDespairEmpowered))
            {
                foreach (EffectEvent effect in wailsOfDespairEmpowered)
                {
                    (long start, long end) lifespan = effect.ComputeLifespan(log, 5000);
                    long growing = lifespan.end;
                    lifespan = ComputeMechanicLifespanWithCancellationTime(effect.Src, log, lifespan);
                    var circle = new CircleDecoration(240, lifespan, Colors.LightOrange, 0.2, new AgentConnector(p));
                    replay.AddDecorationWithGrowing(circle, growing);
                }
            }
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log);

            // Crushing Regret (Green) End
            var crushingRegretEnds = new List<(string GUID, Color Color)>()
            {
                (EffectGUIDs.TempleOfFebeGreenSuccess, Colors.Green),
                (EffectGUIDs.TempleOfFebeGreenFailure, Colors.DarkRed)
            };
            foreach ((string GUID, Color Color) in crushingRegretEnds)
            {
                if (log.CombatData.TryGetEffectEventsByGUID(GUID, out IReadOnlyList<EffectEvent> crushingRegrets))
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
                        var circle = new CircleDecoration(radius, lifespan, Color, 0.2, new PositionConnector(effect.Position));
                        EnvironmentDecorations.Add(circle);
                    }
                }
            }

            // Pool of Despair - Spread AoE on ground
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.TempleOfFebePoolOfDespair, out IReadOnlyList<EffectEvent> poolOfDespair))
            {
                foreach (EffectEvent effect in poolOfDespair)
                {
                    int duration = log.FightData.IsCM || log.FightData.IsLegendaryCM ? 120000 : 60000;
                    (long start, long end) lifespan = effect.ComputeDynamicLifespan(log, duration);
                    var circle = new CircleDecoration(120, lifespan, Colors.RedSkin, 0.2, new PositionConnector(effect.Position));
                    EnvironmentDecorations.Add(circle);
                    EnvironmentDecorations.Add(circle.GetBorderDecoration(Colors.Red, 0.2));
                }
            }

            // Pool of Despair - Empowered - Spread AoE on ground
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.TempleOfFebePoolOfDespairEmpowered, out IReadOnlyList<EffectEvent> poolOfDespairEmpowered))
            {
                foreach (EffectEvent effect in poolOfDespairEmpowered)
                {
                    (long start, long end) lifespan = effect.ComputeDynamicLifespan(log, 999000);
                    var circle = new CircleDecoration(240, lifespan, Colors.RedSkin, 0.2, new PositionConnector(effect.Position));
                    EnvironmentDecorations.Add(circle);
                    EnvironmentDecorations.Add(circle.GetBorderDecoration(Colors.Red, 0.2));
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
            var castEvents = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.SkillId != WeaponStow && x.SkillId != WeaponSwap && x.SkillId != WeaponDraw).ToList();
            long invisibleStart = log.FightData.LogStart;
            bool startTrimmed = false;

            AbstractSingleActor cerus = log.FightData.GetMainTargets(log).Where(x => x.IsSpecies(TargetID.Cerus)).FirstOrDefault();
            var invulnsApply = new List<BuffApplyEvent>();
            if (cerus != null)
            {
                invulnsApply = GetFilteredList(log.CombatData, InvulnerabilityCerus, cerus, true, true).OfType<BuffApplyEvent>().ToList();
            }

            foreach (AbstractCastEvent cast in castEvents)
            {
                // Spawn and despawn the Embodiment 3500 ms before the cast start and end.
                (long start, long end) = (cast.Time - 3500, cast.Time + castDuration + 3500);

                // End the cast early if Cerus gains Invulnerability for the 80% and 50% splits.
                foreach (BuffApplyEvent invulnApply in invulnsApply)
                {
                    if (start <= invulnApply.Time && end > invulnApply.Time)
                    {
                        end = invulnApply.Time;
                    }
                }

                // If the Embodiment hasn't been trimmed yet, trim the lifespan to start on first cast and end at the fight end.
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
            replay.AddOverheadIcons(target.GetBuffStatus(log, InvulnerabilityEmbodiment, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), target, BuffImages.Determined);
        }

        /// <summary>
        /// Adds the Malicious Intent mechanic decoration.
        /// </summary>
        /// <param name="target">The target casting.</param>
        /// <param name="log">The log.</param>
        /// <param name="replay">The Combat Replay.</param>
        /// <param name="casts">The cast events.</param>
        private static void AddMaliciousIntentDecoration(NPC target, ParsedEvtcLog log, CombatReplay replay, IReadOnlyList<AbstractCastEvent> casts)
        {
            // The Malicious Intent buff is only present in normal mode
            // The effect has no Src but we can check the skill cast
            var maliciousIntent = casts.Where(x => x.SkillId == MaliciousIntentNM || x.SkillId == MaliciousIntentEmpoweredNM || x.SkillId == MaliciousIntentCM || x.SkillId == MaliciousIntentEmpoweredCM).ToList();
            foreach (AbstractCastEvent cast in maliciousIntent)
            {
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.TempleOfFebeMaliciousIntentTether, out IReadOnlyList<EffectEvent> maliciousIntentTethers))
                {
                    // This will only conflict if the embodiment and cerus cast the skill at the same time
                    foreach (EffectEvent effect in maliciousIntentTethers.Where(x => x.Time >= cast.Time && x.Time < cast.Time + 2000))
                    {
                        (long start, long end) lifespan = (effect.Time, effect.Time + 5000);
                        lifespan = ComputeMechanicLifespanWithCancellationTime(target.AgentItem, log, lifespan);
                        var tether = new LineDecoration(lifespan, Colors.RedSkin, 0.4, new AgentConnector(effect.Dst), new AgentConnector(target));
                        replay.Decorations.Add(tether);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the Cry of Rage mechanic decoration.
        /// </summary>
        /// <param name="target">The target casting.</param>
        /// <param name="log">The log.</param>
        /// <param name="replay">The Combat Replay.</param>
        /// <param name="casts">The cast events.</param>
        private static void AddCryOfRageDecoration(NPC target, ParsedEvtcLog log, CombatReplay replay, IReadOnlyList<AbstractCastEvent> casts)
        {
            var cryOfRage = casts.Where(x => x.SkillId == CryOfRageNM || x.SkillId == CryOfRageCM || x.SkillId == CryOfRageEmpoweredNM || x.SkillId == CryOfRageEmpoweredCM).ToList();
            foreach (AbstractCastEvent cast in cryOfRage)
            {
                uint radius = 0;
                long defaultCastDuration = 5000;
                (long start, long end) lifespan = (cast.Time, cast.Time + defaultCastDuration);
                long growing = lifespan.end;

                switch (cast.SkillId)
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
                replay.AddDecorationWithGrowing(circle, growing);
            }
        }

        /// <summary>
        /// Adds the Envious Gaze mechanic decoration.
        /// </summary>
        /// <param name="target">The target casting.</param>
        /// <param name="log">The log.</param>
        /// <param name="replay">The Combat Replay.</param>
        /// <param name="casts">The cast events.</param>
        private static void AddEnviousGazeDecoration(NPC target, ParsedEvtcLog log, CombatReplay replay, IReadOnlyList<AbstractCastEvent> casts)
        {
            uint width = 2200;

            var enviousGaze = casts.Where(x => x.SkillId == EnviousGazeNM || x.SkillId == EnviousGazeEmpoweredNM || x.SkillId == EnviousGazeCM || x.SkillId == EnviousGazeEmpoweredCM).ToList();
            foreach (AbstractCastEvent cast in enviousGaze)
            {
                bool isEmpowered = cast.SkillId == EnviousGazeEmpoweredNM || cast.SkillId == EnviousGazeEmpoweredCM;
                long indicatorDuration = 1500;
                (long start, long end) lifespanIndicator = (cast.Time, cast.Time + indicatorDuration);
                long growing = lifespanIndicator.end;
                Point3D facing = target.GetCurrentRotation(log, lifespanIndicator.end);
                if (facing != null)
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
                    var agentConnector = (AgentConnector)new AgentConnector(target).WithOffset(new Point3D(width / 2, 0), true);
                    var rectangle = (RectangleDecoration)new RectangleDecoration(width, 100, lifespanIndicator, Colors.LightOrange, 0.2, agentConnector).UsingRotationConnector(rotation);
                    replay.AddDecorationWithGrowing(rectangle, growing);
                    if (isEmpowered)
                    {
                        // Opposite Indicator
                        var oppositeAgentConnector = (AgentConnector)new AgentConnector(target).WithOffset(new Point3D(-(width / 2), 0), true);
                        var oppositeRectangle = (RectangleDecoration)new RectangleDecoration(width, 100, lifespanIndicator, Colors.LightOrange, 0.2, oppositeAgentConnector).UsingRotationConnector(rotation);
                        replay.AddDecorationWithGrowing(oppositeRectangle, growing);
                    }

                    // Check if Petrify is casted between the end of the indicator and the start of the damage beam
                    (long start, long end) = ComputeMechanicLifespanWithCancellationTime(target.AgentItem, log, (lifespanIndicator.end, lifespanIndicator.end + 950));
                    if (end < lifespanIndicator.end + 950)
                    {
                        continue;
                    }

                    // Frontal Damage Beam
                    (long start, long end) lifespanDamage = (lifespanIndicator.end + 950, lifespanIndicator.end + 10750);
                    (long start, long end) lifespanDamageCancelled = lifespanDamage;
                    lifespanDamageCancelled = ComputeMechanicLifespanWithCancellationTime(target.AgentItem, log, lifespanDamage);
                    double millisecondsPerDegree = (double)(lifespanDamage.end - lifespanDamage.start) / 360;
                    double degreesRotated = (lifespanDamageCancelled.end - lifespanDamageCancelled.start) / millisecondsPerDegree;
                    var rotation2 = new AngleConnector(facing, (float)degreesRotated);
                    var rectangle2 = (RectangleDecoration)new RectangleDecoration(width, 100, lifespanDamageCancelled, Colors.Red, 0.2, agentConnector).UsingRotationConnector(rotation2);
                    replay.Decorations.Add(rectangle2);
                    if (isEmpowered)
                    {
                        // Opposite Damage Beam
                        (long start, long end) lifespanDamageOpposite = (lifespanIndicator.end + 950, lifespanIndicator.end + 5850);
                        (long start, long end) lifespanDamageOppositeCancelled = lifespanDamage;
                        lifespanDamageOppositeCancelled = ComputeMechanicLifespanWithCancellationTime(target.AgentItem, log, lifespanDamageOpposite);
                        double millisecondsPerDegreeOpposite = (double)(lifespanDamageOpposite.end - lifespanDamageOpposite.start) / 360;
                        double degreedRotatedOpposite = (lifespanDamageOppositeCancelled.end - lifespanDamageOppositeCancelled.start) / millisecondsPerDegreeOpposite;
                        var rotation3 = new AngleConnector(facing, (float)degreedRotatedOpposite);
                        var oppositeAgentConnector = (AgentConnector)new AgentConnector(target).WithOffset(new Point3D(-(width / 2), 0), true);
                        var rectangle3 = (RectangleDecoration)new RectangleDecoration(width, 100, lifespanDamageOppositeCancelled, Colors.Red, 0.2, oppositeAgentConnector).UsingRotationConnector(rotation3);
                        replay.Decorations.Add(rectangle3);
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
            AbstractSingleActor cerus = log.FightData.GetMainTargets(log).Where(x => x.IsSpecies(TargetID.Cerus)).FirstOrDefault();
            if (cerus != null)
            {
                // If Cerus is casting a mechanic, cancel it when he begins casting Petrify
                if (target.IsSpecies(TargetID.Cerus))
                {
                    var casts = cerus.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.SkillId == PetrifySkill).ToList();
                    foreach (AbstractCastEvent cast in casts)
                    {
                        if (lifespan.start <= cast.Time && lifespan.end > cast.Time)
                        {
                            lifespan.end = Math.Min(lifespan.end, cast.Time);
                        }
                    }
                }
                else
                {
                    // If a permanent Embodiment is casting a mechanic, cancel it when Cerus gains invulnerability (start 80% and 50% split phases)
                    var invulnsApply = GetFilteredList(log.CombatData, InvulnerabilityCerus, cerus, true, true).OfType<BuffApplyEvent>().ToList();
                    foreach (BuffApplyEvent invulnApply in invulnsApply)
                    {
                        if (lifespan.start <= invulnApply.Time && lifespan.end > invulnApply.Time)
                        {
                            lifespan.end = Math.Min(lifespan.end, invulnApply.Time);
                        }
                        else
                        {
                            // If a killable Embodiment is casting a mechanic, cancel it when Cerus loses invulnerability (end 80% and 50% split phases)
                            var invulnsRemove = GetFilteredList(log.CombatData, InvulnerabilityCerus, cerus, true, true).OfType<BuffRemoveAllEvent>().ToList();
                            foreach (BuffRemoveAllEvent invulnRemove in invulnsRemove)
                            {
                                if (lifespan.start <= invulnRemove.Time && lifespan.end > invulnRemove.Time)
                                {
                                    lifespan.end = Math.Min(lifespan.end, invulnRemove.Time);
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

            if (log.FightData.IsCM || log.FightData.IsLegendaryCM)
            {
                AgentItem cerus = log.AgentData.GetNPCsByID((int)TargetID.Cerus).FirstOrDefault();
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
                    if (empoweredBuffs.Count(x => cerus.HasBuff(log, x, log.FightData.LogStart, log.FightData.LogEnd)) == 6)
                    {
                        InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityApathetic], 1));
                    }
                }
            }
        }
    }
}
