using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class XunlaiJadeJunkyard : EndOfDragonsStrike
    {
        public XunlaiJadeJunkyard(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(GraspingHorror, "GraspingHorror", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.DarkRed), "Hands.H", "Hit by Hands AoE", "Hands Hit", 150),
                new PlayerDstHitMechanic(DeathsEmbraceSkill, "Death's Embrace", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.DarkRed), "AoE.H", "Hit by Pull AoE", "Pull AoE Hit", 150),
                new PlayerDstHitMechanic(DeathsHand1, "Death's Hand", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.DarkRed), "Sctn.AoE.H", "Hit by in between Sections AoE", "Section AoE Hit", 150),
                //new HitOnPlayerMechanic(DeathsHand2, "Death's Hand", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.DarkRed), "Sctn.AoE.H", "Hit by in between Sections AoE", "Section AoE Hit", 150),
                new PlayerDstHitMechanic(WallOfFear, "Wall of Fear", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Yellow), "Krait.H", "Hit by Kraits", "Krait Hit", 150),
                new PlayerDstHitMechanic(WaveOfTorment, "Wave of Torment", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.LightPurple), "Quaggan.H", "Hit by Quaggan Explosion", "Quaggan Hit", 150),
                new EnemyDstBuffApplyMechanic(PowerOfTheVoid, "Power of the Void", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "Pwrd.Up", "Ankka has powered up", "Ankka powered up", 150)
            }
            );
            Icon = EncounterIconXunlaiJadeJunkyard;
            Extension = "xunjadejunk";
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayXunlaiJadeJunkyard,
                            (1485, 1292),
                            (-7090, -2785, 3647, 6556)/*,
                            (-15360, -36864, 15360, 39936),
                            (3456, 11012, 4736, 14212)*/);
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor ankka = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Ankka));
            if (ankka == null)
            {
                throw new MissingKeyActorsException("Ankka not found");
            }
            phases[0].AddTarget(ankka);
            if (!requirePhases)
            {
                return phases;
            }
            List<PhaseData> subPhases = GetPhasesByInvul(log, AnkkaPlateformChanging, ankka, false, true);
            for (int i = 0; i < subPhases.Count; i++)
            {
                subPhases[i].Name = "Location " + (i + 1);
                subPhases[i].AddTarget(ankka);
            }
            phases.AddRange(subPhases);
            List<PhaseData> subSubPhases = GetPhasesByInvul(log, Determined895, ankka, false, false);
            subSubPhases.RemoveAll(x => subPhases.Any(y => Math.Abs(y.Start - x.Start) < ParserHelper.ServerDelayConstant && Math.Abs(y.End - x.End) < ParserHelper.ServerDelayConstant));
            int curSubSubPhaseID = 0;
            PhaseData previousSubPhase = null;
            for (int i = 0; i < subSubPhases.Count; i++)
            {
                PhaseData subsubPhase = subSubPhases[i];
                PhaseData subPhase = subPhases.FirstOrDefault(x => x.Start - ParserHelper.ServerDelayConstant <= subsubPhase.Start && x.End + ParserHelper.ServerDelayConstant >= subsubPhase.End);
                if (previousSubPhase != subPhase)
                {
                    previousSubPhase = subPhase;
                    curSubSubPhaseID = 0;
                }
                if (subPhase != null)
                {
                    int index = subPhases.IndexOf(subPhase);
                    subsubPhase.OverrideStart(Math.Max(subsubPhase.Start, subPhase.Start));
                    subsubPhase.OverrideEnd(Math.Min(subsubPhase.End, subPhase.End));
                    subsubPhase.Name = "Location " + (index + 1) + " - " + (++curSubSubPhaseID);
                    subsubPhase.AddTarget(ankka);
                }
            }
            phases.AddRange(subSubPhases);
            //
            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                AbstractSingleActor ankka = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Ankka));
                if (ankka == null)
                {
                    throw new MissingKeyActorsException("Ankka not found");
                }
                var buffApplies = combatData.GetBuffData(Determined895).OfType<BuffApplyEvent>().Where(x => x.To == ankka.AgentItem && !x.Initial && x.AppliedDuration > int.MaxValue / 2 && x.Time >= fightData.FightStart + 5000).ToList();
                if (buffApplies.Count == 3)
                {
                    fightData.SetSuccess(true, buffApplies.LastOrDefault().Time);
                }
            }
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.Ankka,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Ankka,
                ArcDPSEnums.TrashID.ReanimatedHatred,
                ArcDPSEnums.TrashID.ReanimatedMalice1,
                ArcDPSEnums.TrashID.ReanimatedMalice2,
                ArcDPSEnums.TrashID.ReanimatedHatred,
                ArcDPSEnums.TrashID.ZhaitansReach,
                ArcDPSEnums.TrashID.AnkkaHallucination1,
                ArcDPSEnums.TrashID.AnkkaHallucination2,
                ArcDPSEnums.TrashID.AnkkaHallucination3,
            };
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor ankka = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Ankka));
            if (ankka == null)
            {
                throw new MissingKeyActorsException("Ankka not found");
            }
            MapIDEvent map = combatData.GetMapIDEvents().FirstOrDefault();
            if (map != null && map.MapID == 1434)
            {
                return FightData.EncounterMode.Story;
            }
            return ankka.GetHealth(combatData) > 50e6 ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);
            IReadOnlyList<AbstractBuffEvent> gitv = log.CombatData.GetBuffData(AchievementEligibilityGazeIntoTheVoid);
            bool hasGitvBeenAdded = false;

            if (log.FightData.Success)
            {
                if (gitv.Any())
                {
                    foreach (Player p in log.PlayerList)
                    {
                        if (p.HasBuff(log, AchievementEligibilityGazeIntoTheVoid, log.FightData.FightEnd - ServerDelayConstant))
                        {
                            InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityGazeIntoTheVoid], 1));
                            hasGitvBeenAdded = true;
                            break;
                        }
                    }
                }
                if (!hasGitvBeenAdded && CustomCheckGazeIntoTheVoidEligibility(log))
                {
                    InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityGazeIntoTheVoid], 1));
                }
            }
            
        }

        private static bool CustomCheckGazeIntoTheVoidEligibility(ParsedEvtcLog log)
        {
            IReadOnlyList<AgentItem> agents = log.AgentData.GetNPCsByID((int)ArcDPSEnums.TargetID.Ankka);

            foreach (AgentItem agent in agents)
            {
                IReadOnlyDictionary<long, BuffsGraphModel> bgms = log.FindActor(agent).GetBuffGraphs(log);
                if (bgms != null && bgms.ContainsKey(PowerOfTheVoid))
                {
                    bgms.TryGetValue(PowerOfTheVoid, out BuffsGraphModel bgm);
                    foreach (Segment s in bgm.BuffChart)
                    {
                        if (s.Value == 6)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
