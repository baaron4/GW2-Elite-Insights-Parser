using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class TempleOfFebe : SecretOfTheObscureStrike
    {
        public TempleOfFebe(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstBuffApplyMechanic(Insatiable, "Insatiable", new MechanicPlotlySetting(Symbols.Hourglass, Colors.Pink), "Ins.A", "Insatiable Applied (Absorbed Gluttony Orb)", "Insatiable Application", 0),
                new PlayerDstBuffApplyMechanic(MaliciousIntentTargetBuff, "Malicious Intent", new MechanicPlotlySetting(Symbols.Bowtie, Colors.DarkGreen), "MalInt.A", "Malicious Intent Target", "Targetted by Malicious Intent", 0),
                new PlayerDstHitMechanic(new long [] { CrushingRegretEmbodiment, CrushingRegretCerus }, "Crushing Regret", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkGreen), "Green.H", "Hit by Crushing Regret (Green)", "Crushing Regret Hit", 0),
                new PlayerDstHitMechanic(new long [] { WailOfDespair1, WailOfDespair2 }, "Wail of Despair", new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "WailDesp.H", "Hit by Wail of Despair (Small AoE)", "Wail of Despair Hit", 0),
                new PlayerDstHitMechanic(new long [] { PoolOfDespair1, PoolOfDespair2 }, "Pool of Despair", new MechanicPlotlySetting(Symbols.Circle, Colors.Orange), "PoolDesp.H", "Hit by Pool of Despair (Big AoE)", "Pool of Despair Hit", 0),
                new PlayerDstHitMechanic(EnviousGaze, "Envious Gaze", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Red), "EnvGaz.H", "Hit by Envious Gaze (One Beam)", "Envious Gaze Hit (One Beam)", 0),
                new PlayerDstHitMechanic(new long [] { EnviousGazeDoubleBeam1, EnviousGazeDoubleBeam2 }, "Envious Gaze", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "EmpEnvGaz.H", "Hit by Envious Gaze (Double Beam)", "Envious Gaze Hit (Double Beam)", 0),
                new PlayerDstHitMechanic(CryOfRageSmall, "Cry of Rage", new MechanicPlotlySetting(Symbols.CircleX, Colors.LightOrange), "CryRage.H", "Hit by Cry of Rage (Small)", "Cry of Rage Hit (Small)", 0),
                new PlayerDstHitMechanic(CryOfRageLarge, "Cry of Rage", new MechanicPlotlySetting(Symbols.CircleX, Colors.Orange), "LrgCryRage.H", "Hit by Cry of Rage (Large)", "Cry of Rage Hit (Large)", 0),
                new PlayerDstHitMechanic(EnragedSmash, "Enraged Smash", new MechanicPlotlySetting(Symbols.Star, Colors.Red), "EnrSmash.D", "Downed to Enraged Smash", "Downed to Enraged Smash", 0).UsingChecker((ahde, log) => ahde.HasDowned),
                new PlayerDstHitMechanic(MaliciousIntent, "Malicious Intent", new MechanicPlotlySetting(Symbols.Y, Colors.White), "MalInt.H", "Hit by Malicious Intent", "Malicious Intent Hit", 0),
                new PlayerDstHitMechanic(PetrifyDamage, "Petrify", new MechanicPlotlySetting(Symbols.Pentagon, Colors.Teal), "Pet.H", "Hit by Petrify", "Petrify Hit", 0),
                new PlayerDstEffectMechanic(EffectGUIDs.TempleOfFebeCerusGreen, "Crushing Regret", new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "Green.A", "Crushing Regret Applied (Green)", "Crushing Regret Application", 0),
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
                new EnemyCastStartMechanic(EnragedSmash, "Enraged Smash", new MechanicPlotlySetting(Symbols.Star, Colors.Blue), "EnrSmash.C", "Casted Enraged Smash", "Enraged Smash Cast", 0),
                new EnemyCastStartMechanic(new long [] { InsatiableHunger1, InsatiableHunger2, InsatiableHunger3, InsatiableHunger4, InsatiableHunger5, InsatiableHunger6, InsatiableHunger7, InsatiableHunger8, InsatiableHunger9, InsatiableHunger10 }, "Insatiable Hunger", new MechanicPlotlySetting(Symbols.HourglassOpen, Colors.Pink), "InsHun.C", "Casted Insatiable Hunger", "Insatiable Hunger Cast", 0),
                new EnemyCastStartMechanic(EnviousGaze, "Envious Gaze", new MechanicPlotlySetting(Symbols.TriangleDownOpen, Colors.Red), "EnvGaz.C", "Casted Envious Gaze (One Beam)", "Envious Gaze Cast (One Beam)", 0),
                new EnemyCastStartMechanic(new long [] { EnviousGazeDoubleBeam1, EnviousGazeDoubleBeam2 }, "Envious Gaze", new MechanicPlotlySetting(Symbols.TriangleUpOpen, Colors.Red), "EmpEnvGaz.C", "Casted Envious Gaze (Double Beam)", "Envious Gaze Cast (Double Beam)", 0),
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
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Cerus));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Cerus not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Invul check
            phases.AddRange(GetPhasesByInvul(log, InvulnerabilityCerus, mainTarget, true, true));
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
            return phases;
        }

        internal override void EIEvtcParse(ulong gw2Build, int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
            int curDespair = 1;
            int curEnvy = 1;
            int curGluttony = 1;
            int curMalice = 1;
            int curRage = 1;
            int curRegret = 1;
            foreach (AbstractSingleActor target in Targets)
            {
                switch (target.ID)
                {
                    case (int)TrashID.EmbodimentOfDespair:
                        CombatItem despair = combatData.FirstOrDefault(x => x.SkillID == EmpoweredDespairEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                        target.OverrideName(target.Character + " (P" + curDespair++ + ")");
                        if (despair != null && Math.Abs(target.FirstAware - despair.Time) <= ServerDelayConstant)
                        {
                            target.OverrideName("Empowered " + target.Character);
                        }
                        break;
                    case (int)TrashID.EmbodimentOfEnvy:
                        CombatItem envy = combatData.FirstOrDefault(x => x.SkillID == EmpoweredEnvyEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                        target.OverrideName(target.Character + " (P" + curEnvy++ + ")");
                        if (envy != null && Math.Abs(target.FirstAware - envy.Time) <= ServerDelayConstant)
                        {
                            target.OverrideName("Empowered " + target.Character);
                        }
                        break;
                    case (int)TrashID.EmbodimentOfGluttony:
                        CombatItem gluttony = combatData.FirstOrDefault(x => x.SkillID == EmpoweredGluttonyEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                        target.OverrideName(target.Character + " (P" + curGluttony++ + ")");
                        if (gluttony != null && Math.Abs(target.FirstAware - gluttony.Time) <= ServerDelayConstant)
                        {
                            target.OverrideName("Empowered " + target.Character);
                        }
                        break;
                    case (int)TrashID.EmbodimentOfMalice:
                        CombatItem malice = combatData.FirstOrDefault(x => x.SkillID == EmpoweredMaliceEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                        target.OverrideName(target.Character + " (P" + curMalice++ + ")");
                        if (malice != null && Math.Abs(target.FirstAware - malice.Time) <= ServerDelayConstant)
                        {
                            target.OverrideName("Empowered " + target.Character);
                        }
                        break;
                    case (int)TrashID.EmbodimentOfRage:
                        CombatItem rage = combatData.FirstOrDefault(x => x.SkillID == EmpoweredRageEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                        target.OverrideName(target.Character + " (P" + curRage++ + ")");
                        if (rage != null && Math.Abs(target.FirstAware - rage.Time) <= ServerDelayConstant)
                        {
                            target.OverrideName("Empowered " + target.Character);
                        }
                        break;
                    case (int)TrashID.EmbodimentOfRegret:
                        CombatItem regret = combatData.FirstOrDefault(x => x.SkillID == EmpoweredRegretEmbodiment && x.DstMatchesAgent(target.AgentItem) && x.IsBuffApply());
                        target.OverrideName(target.Character + " (P" + curRegret++ + ")");
                        if (regret != null && Math.Abs(target.FirstAware - regret.Time) <= ServerDelayConstant)
                        {
                            target.OverrideName("Empowered " + target.Character);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>() { TrashID.MaliciousShadow };
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Temple of Febe";
        }
        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor cerus = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Cerus));
            if (cerus == null)
            {
                throw new MissingKeyActorsException("Cerus not found");
            }
            return (cerus.GetHealth(combatData) > 100e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }
    }
}
