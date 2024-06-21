using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.ArcDPSEnums;

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
            const int healthCMRelease = 32_618_906;
            const int healthThreshold = (int)(0.95 * healthCMRelease); // fractals lose hp as their scale lowers
            AbstractSingleActor eparch = GetEparchActor();
            if (combatData.GetBuildEvent().Build >= GW2Builds.June2024LonelyTowerCMRelease && eparch.GetHealth(combatData) >= healthThreshold)
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

        internal override void EIEvtcParse(ulong gw2Build, int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            // remove eparch agents used in roleplay by checking for relevant casts
            IEnumerable<AgentItem> dummyEparchs = agentData.GetNPCsByID(TargetID.EparchLonelyTower).Where(eparch =>
            {
                return !combatData.Any(x => x.SrcMatchesAgent(eparch) && x.StartCasting() && x.SkillID != WeaponDraw && x.SkillID != WeaponStow);
            });
            agentData.RemoveAllFrom(new HashSet<AgentItem>(dummyEparchs));

            // identify rift agents by their health and hitbox dimensions
            var riftAgents = combatData.Where(x => x.IsStateChange == StateChange.MaxHealthUpdate && x.DstAgent == 149400).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 100 && x.HitboxHeight == 1100).ToList();
            if (riftAgents.Count != 0)
            {
                riftAgents.ForEach(x => {
                    x.OverrideID(TrashID.KryptisRift);
                    x.OverrideType(AgentItem.AgentType.NPC);
                });
                agentData.Refresh();
            }

            base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);

            int crueltyCount = 1;
            int judgementCount = 1;
            int avatarCount = 1;
            int riftCount = 1;
            foreach (NPC target in _targets)
            {
                switch (target.ID)
                {
                    case (int)TrashID.KryptisRift:
                        target.OverrideName(target.Character + " " + riftCount++);
                        break;
                    case (int)TrashID.IncarnationOfCruelty:
                        target.OverrideName(target.Character + " " + crueltyCount++);
                        break;
                    case (int)TrashID.IncarnationOfJudgement:
                        target.OverrideName(target.Character + " " + judgementCount++);
                        break;
                    case (int)TrashID.AvatarOfSpite:
                        target.OverrideName(target.Character + " " + avatarCount++);
                        break;
                }
            }
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            AbstractSingleActor eparch = GetEparchActor();
            var determinedApplies = combatData.GetBuffDataByIDByDst(Determined762, eparch.AgentItem).OfType<BuffApplyEvent>().ToList();
            if (determinedApplies.Count >= 3)
            {
                fightData.SetSuccess(true, determinedApplies[2].Time);
            }
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor eparch = GetEparchActor();
            phases[0].AddTarget(eparch);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, Determined762, eparch, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0) {
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
