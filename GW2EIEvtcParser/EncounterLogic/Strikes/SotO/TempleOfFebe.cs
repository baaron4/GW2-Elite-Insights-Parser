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
                new EnemyCastStartMechanic(EnragedSmash, "Enraged Smash", new MechanicPlotlySetting(Symbols.Star, Colors.Blue), "EnrSmash.C", "Casted Enraged Smash", "Enraged Smash Cast", 0),
                new EnemyCastStartMechanic(new long [] { InsatiableHunger1, InsatiableHunger2, InsatiableHunger3, InsatiableHunger4, InsatiableHunger5, InsatiableHunger6, InsatiableHunger7, InsatiableHunger8, InsatiableHunger9, InsatiableHunger10 }, "Insatiable Hunger", new MechanicPlotlySetting(Symbols.HourglassOpen, Colors.Pink), "InsHun.C", "Casted Insatiable Hunger", "Insatiable Hunger Cast", 0),
                new EnemyCastStartMechanic(EnviousGaze, "Envious Gaze", new MechanicPlotlySetting(Symbols.TriangleDownOpen, Colors.Red), "EnvGaz.C", "Casted Envious Gaze (One Beam)", "Envious Gaze Cast (One Beam)", 0),
                new EnemyCastStartMechanic(new long [] { EnviousGazeDoubleBeam1, EnviousGazeDoubleBeam2 }, "Envious Gaze", new MechanicPlotlySetting(Symbols.TriangleUpOpen, Colors.Red), "EmpEnvGaz.C", "Casted Envious Gaze (Double Beam)", "Envious Gaze Cast (Double Beam)", 0),
                new EnemyCastStartMechanic(PetrifySkill, "Petrify", new MechanicPlotlySetting(Symbols.Pentagon, Colors.Yellow), "Pet.C", "Casted Petrify", "Petrify Cast", 0),
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
                            // TODO
                            (1008, 1008),
                            (0,0,0,0));
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>()
            {
                (int)ArcDPSEnums.TargetID.Cerus,
                (int)ArcDPSEnums.TrashID.EmbodimentOfDespair,
                (int)ArcDPSEnums.TrashID.EmbodimentOfEnvy,
                (int)ArcDPSEnums.TrashID.EmbodimentOfGluttony,
                (int)ArcDPSEnums.TrashID.EmbodimentOfMalice,
                (int)ArcDPSEnums.TrashID.EmbodimentOfRage,
                (int)ArcDPSEnums.TrashID.EmbodimentOfRegret,
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Cerus));
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
                        (int)ArcDPSEnums.TrashID.EmbodimentOfDespair,
                        (int)ArcDPSEnums.TrashID.EmbodimentOfEnvy,
                        (int)ArcDPSEnums.TrashID.EmbodimentOfGluttony,
                        (int)ArcDPSEnums.TrashID.EmbodimentOfMalice,
                        (int)ArcDPSEnums.TrashID.EmbodimentOfRage,
                        (int)ArcDPSEnums.TrashID.EmbodimentOfRegret,
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

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, extensions);
            int curDespair = 1;
            int curEnvy = 1;
            int curGluttony = 1;
            int curMalice = 1;
            int curRage = 1;
            int curRegret = 1;
            foreach (AbstractSingleActor target in Targets)
            {
                if (target.IsSpecies(ArcDPSEnums.TrashID.EmbodimentOfDespair))
                {
                    target.OverrideName(target.Character + " " + curDespair++);
                }
                if (target.IsSpecies(ArcDPSEnums.TrashID.EmbodimentOfEnvy))
                {
                    target.OverrideName(target.Character + " " + curEnvy++);
                }
                if (target.IsSpecies(ArcDPSEnums.TrashID.EmbodimentOfGluttony))
                {
                    target.OverrideName(target.Character + " " + curGluttony++);
                }
                if (target.IsSpecies(ArcDPSEnums.TrashID.EmbodimentOfMalice))
                {
                    target.OverrideName(target.Character + " " + curMalice++);
                }
                if (target.IsSpecies(ArcDPSEnums.TrashID.EmbodimentOfRage))
                {
                    target.OverrideName(target.Character + " " + curRage++);
                }
                if (target.IsSpecies(ArcDPSEnums.TrashID.EmbodimentOfRegret))
                {
                    target.OverrideName(target.Character + " " + curRegret++);
                }
            }
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>() { ArcDPSEnums.TrashID.MaliciousShadow };
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Temple of Febe";
        }
    }
}
