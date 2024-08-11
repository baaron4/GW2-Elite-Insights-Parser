using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Eparch : LonelyTower
    {
        public Eparch(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            });
            Extension = "eparch";
            Icon = EncounterIconEparch;
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            ulong build = combatData.GetGW2BuildEvent().Build;
            int healthCMRelease = build >= GW2Builds.June2024Balance ? 22_833_236 : 32_618_906;
            int healthThreshold = (int)(0.95 * healthCMRelease); // fractals lose hp as their scale lowers
            AbstractSingleActor eparch = GetEparchActor();
            if (build >= GW2Builds.June2024LonelyTowerCMRelease && eparch.GetHealth(combatData) >= healthThreshold)
            {
                return FightData.EncounterMode.CM;
            }
            else
            {
                return FightData.EncounterMode.Normal;
            }
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Eparch";
        }

        internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            var dummyEparchs = agentData.GetNPCsByID(TargetID.EparchLonelyTower).Where(eparch =>
            {
                return !combatData.Any(x => x.SrcMatchesAgent(eparch) && x.StartCasting() && x.SkillID != WeaponDraw && x.SkillID != WeaponStow);
            }).ToList();
            dummyEparchs.ForEach(x => x.OverrideID(TrashID.EparchLonelyTowerDummy));
            //
            var riftAgents = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 149400 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.FirstAware > fightData.FightStart + 5000).ToList();
            riftAgents.ForEach(x =>
            {
                x.OverrideID(TrashID.KryptisRift);
                x.OverrideType(AgentItem.AgentType.NPC);
            });
            //
            if (riftAgents.Count != 0 || dummyEparchs.Count != 0)
            {
                agentData.Refresh();
            }

            base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
            int[] miniBossCount = new int[] { 1, 1, 1, 1 };
            foreach (NPC target in _targets)
            {
                switch (target.ID)
                {
                    case (int)TrashID.KryptisRift:
                        target.OverrideName(target.Character + " " + miniBossCount[0]++);
                        break;
                    case (int)TrashID.IncarnationOfCruelty:
                        target.OverrideName(target.Character + " " + miniBossCount[1]++);
                        break;
                    case (int)TrashID.IncarnationOfJudgement:
                        target.OverrideName(target.Character + " " + miniBossCount[2]++);
                        break;
                    case (int)TrashID.AvatarOfSpite:
                        target.OverrideName(target.Character + " " + miniBossCount[3]++);
                        break;
                }
            }
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            AbstractSingleActor eparch = GetEparchActor();
            var determinedApplies = combatData.GetBuffDataByIDByDst(Determined762, eparch.AgentItem).OfType<BuffApplyEvent>().ToList();
            if (fightData.IsCM && determinedApplies.Count >= 3)
            {
                fightData.SetSuccess(true, determinedApplies[2].Time);
            } 
            else if (!fightData.IsCM && determinedApplies.Count >= 1)
            {
                fightData.SetSuccess(true, determinedApplies[0].Time);
            }
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor eparch = GetEparchActor();
            phases[0].AddTarget(eparch);
            if (!requirePhases || !log.FightData.IsCM)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, Determined762, eparch, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    phase.Name = "Split " + i / 2;
                    var ids = new List<int>
                    {
                        (int)TrashID.IncarnationOfCruelty,
                        (int)TrashID.IncarnationOfJudgement,
                        (int)TrashID.KryptisRift,
                    };
                    AddTargetsToPhase(phase, ids);
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(eparch);
                }
            }
            return phases;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)TargetID.EparchLonelyTower,
                (int)TrashID.IncarnationOfCruelty,
                (int)TrashID.IncarnationOfJudgement,
                (int)TrashID.AvatarOfSpite,
                (int)TrashID.KryptisRift,
            };
        }

        protected override Dictionary<int, int> GetTargetsSortIDs()
        {
            return new Dictionary<int, int>()
            {
                {(int)TargetID.EparchLonelyTower, 0},
                {(int)TrashID.KryptisRift, 1},
                {(int)TrashID.IncarnationOfCruelty, 2},
                {(int)TrashID.IncarnationOfJudgement, 2},
                {(int)TrashID.AvatarOfSpite, 3},
            };
        }

        protected override List<TrashID> GetTrashMobsIDs()
        {
            return new List<TrashID>
            {
                TrashID.TheTormentedLonelyTower,
                TrashID.TheCravenLonelyTower,
            };
        }

        private AbstractSingleActor GetEparchActor()
        {
            AbstractSingleActor eparch = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.EparchLonelyTower)) ?? throw new MissingKeyActorsException("Eparch not found");
            return eparch;
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log);

            AddGlobuleDecorations(log);

            // rain of despair pools
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EparchDespairPool, out IReadOnlyList<EffectEvent> pools))
            {
                foreach (EffectEvent effect in pools)
                {
                    (long, long) lifespan = effect.ComputeDynamicLifespan(log, 15000);
                    var position = new PositionConnector(effect.Position);
                    var circle = new CircleDecoration(100, lifespan, Colors.RedSkin, 0.3, position);
                    EnvironmentDecorations.Add(circle);
                    EnvironmentDecorations.Add(circle.GetBorderDecoration(Colors.Red, 0.2));
                }
            }

            // rage fissures
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EparchRageFissure, out IReadOnlyList<EffectEvent> fissures))
            {
                foreach (EffectEvent effect in fissures)
                {
                    (long, long) lifespan = effect.ComputeDynamicLifespan(log, 24000);
                    GeographicalConnector position = new PositionConnector(effect.Position).WithOffset(new Point3D(0.0f, 100.0f), true);
                    var rotation = new AngleConnector(effect.Rotation.Z);
                    EnvironmentDecorations.Add(new RectangleDecoration(40, 220, lifespan, Colors.Orange, 0.2, position).UsingRotationConnector(rotation));
                }
            }
        }

        private void AddGlobuleDecorations(ParsedEvtcLog log)
        {
            AbstractSingleActor eparch = GetEparchActor();
            IReadOnlyList<AnimatedCastEvent> eparchCasts = log.CombatData.GetAnimatedCastData(eparch.AgentItem);

            // globule gadgets as decorations
            var globuleColors = new Dictionary<long, Color> {
                { RainOfDespair, Colors.Blue },
                { WaveOfEnvy, Colors.Green },
                { Inhale, Colors.Orange },
                { SpikeOfMalice, Colors.Purple },
                { EnragedSmashEparch, Colors.Red },
                { RegretSkillEparch, Colors.Yellow },
            };
            foreach (AgentItem gadget in log.AgentData.GetAgentByType(AgentItem.AgentType.Gadget))
            {
                const int globuleHealth = 14_940;
                const uint globuleWidth = 16;
                const uint globuleHeight = 160;
                MaxHealthUpdateEvent health = log.CombatData.GetMaxHealthUpdateEvents(gadget).FirstOrDefault();
                if (gadget.HitboxWidth == globuleWidth && gadget.HitboxHeight == globuleHeight && health?.MaxHealth == globuleHealth)
                {
                    SpawnEvent spawn = log.CombatData.GetSpawnEvents(gadget).FirstOrDefault();
                    DespawnEvent despawn = log.CombatData.GetDespawnEvents(gadget).FirstOrDefault();
                    if (spawn != null && despawn != null)
                    {
                        AnimatedCastEvent currentCast = eparchCasts.LastOrDefault(x => x.Time <= spawn.Time && x.EndTime > spawn.Time);
                        if (currentCast != null && globuleColors.TryGetValue(currentCast.SkillId, out Color color))
                        {
                            Point3D position = gadget.GetCurrentPosition(log, despawn.Time); // position should not change, use despawn to make sure its set
                            (long, long) lifespan = (spawn.Time, despawn.Time);
                            EnvironmentDecorations.Add(new CircleDecoration(globuleWidth, lifespan, color, 0.7, new PositionConnector(position)));
                        }
                    }
                }
            }
        }
    }
}
