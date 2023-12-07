using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class River : HallOfChains
    {
        private static readonly Point3D ChestOfSoulsPosition = new Point3D(7906.54f, 2147.48f, -5746.19f);
        public River(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(Bombshell, "Bombshell", new MechanicPlotlySetting(Symbols.Circle,Colors.Orange),"Bomb Hit", "Hit by Hollowed Bomber Exlosion", "Hit by Bomb", 0 ),
                new PlayerDstHitMechanic(TimedBomb, "Timed Bomb", new MechanicPlotlySetting(Symbols.Square,Colors.Orange),"Stun Bomb", "Stunned by Mini Bomb", "Stun Bomb", 0).UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
            }
            );
            GenericFallBackMethod = FallBackMethod.ChestGadget;
            ChestID = ArcDPSEnums.ChestID.ChestOfSouls;
            Extension = "river";
            Targetless = true;
            Icon = EncounterIconRiver;
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayRiver,
                            (1000, 387),
                            (-12201, -4866, 7742, 2851)/*,
                            (-21504, -12288, 24576, 12288),
                            (19072, 15484, 20992, 16508)*/);
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Enervator,
                ArcDPSEnums.TrashID.HollowedBomber,
                ArcDPSEnums.TrashID.RiverOfSouls,
                ArcDPSEnums.TrashID.SpiritHorde1,
                ArcDPSEnums.TrashID.SpiritHorde2,
                ArcDPSEnums.TrashID.SpiritHorde3
            };
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>()
            {
                (int)ArcDPSEnums.TargetID.Desmina
            };
        }

        protected override List<int> GetFriendlyNPCIDs()
        {
            return new List<int>()
            {
                (int)ArcDPSEnums.TargetID.Desmina
            };
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            long startToUse = GetGenericFightOffset(fightData);
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogStartNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                IReadOnlyList<AgentItem> enervators = agentData.GetNPCsByID(ArcDPSEnums.TrashID.Enervator);
                if (!enervators.Any())
                {
                    throw new MissingKeyActorsException("Enervators not found");
                }
                AgentItem enervator = enervators.MinBy(x => x.FirstAware);
                if (enervator != null)
                {
                    startToUse = enervator.FirstAware;
                }
            }
            return startToUse;
        }

        internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AgentItem desmina = agentData.GetNPCsByID(ArcDPSEnums.TargetID.Desmina).FirstOrDefault();
            if (desmina == null)
            {
                throw new MissingKeyActorsException("Desmina not found");
            }
            var desminaInitialPosition = new Point3D(-9239.706f, 635.445435f, -813.8115f);
            if (!combatData.GetMovementData(desmina).Any(x => x is PositionEvent pe && pe.Time < desmina.FirstAware + MinimumInCombatDuration && pe.GetParametricPoint3D().Distance2DToPoint(desminaInitialPosition) < 100))
            {
                return FightData.EncounterStartStatus.Late;
            }
            return FightData.EncounterStartStatus.Normal;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            FindChestGadget(ChestID, agentData, combatData, ChestOfSoulsPosition, (agentItem) => agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100);
            agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "River of Souls", Spec.NPC, (int)ArcDPSEnums.TargetID.DummyTarget, true);
            ComputeFightTargets(agentData, combatData, extensions);
        }

        internal override FightLogic AdjustLogic(AgentData agentData, List<CombatItem> combatData)
        {
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogStartNPCUpdate);
            // Handle potentially wrongly associated logs
            if (logStartNPCUpdate != null)
            {
                if (agentData.GetNPCsByID(ArcDPSEnums.TargetID.BrokenKing).Any(brokenKing => combatData.Any(evt => evt.IsDamagingDamage() && evt.DstMatchesAgent(brokenKing))))
                {
                    return new StatueOfIce((int)ArcDPSEnums.TargetID.BrokenKing);
                }
                if (agentData.GetNPCsByID(ArcDPSEnums.TargetID.EaterOfSouls).Any(soulEater => combatData.Any(evt => evt.IsDamagingDamage() && evt.DstMatchesAgent(soulEater))))
                {
                    return new StatueOfDeath((int)ArcDPSEnums.TargetID.EaterOfSouls);
                }
                if (agentData.GetNPCsByID(ArcDPSEnums.TargetID.EyeOfFate).Any(eyeOfFate => combatData.Any(evt => evt.IsDamagingDamage() && evt.DstMatchesAgent(eyeOfFate))))
                {
                    return new StatueOfDarkness((int)ArcDPSEnums.TargetID.EyeOfFate);
                }
                if (agentData.GetNPCsByID(ArcDPSEnums.TargetID.EyeOfJudgement).Any(eyeOfJudgement => combatData.Any(evt => evt.IsDamagingDamage() && evt.DstMatchesAgent(eyeOfJudgement))))
                {
                    return new StatueOfDarkness((int)ArcDPSEnums.TargetID.EyeOfJudgement);
                }
            }
            return base.AdjustLogic(agentData, combatData);
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
            // TODO bombs dual following circle actor (one growing, other static) + dual static circle actor (one growing with min radius the final radius of the previous, other static). Missing buff id
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Desmina:
                    var asylums = target.GetBuffStatus(log, FollowersAsylum, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
                    foreach (Segment asylum in asylums)
                    {
                        replay.Decorations.Add(new CircleDecoration(300, asylum, "rgba(0, 160, 255, 0.3)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.HollowedBomber:
                    ParametricPoint3D firstBomberMovement = replay.Velocities.FirstOrDefault(x => x.Length() != 0);
                    if (firstBomberMovement != null)
                    {
                        replay.Trim(firstBomberMovement.Time - 1000, replay.TimeOffsets.end);
                    }
                    var bomberman = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.SkillId == Bombshell).ToList();
                    foreach (AbstractCastEvent bomb in bomberman)
                    {
                        int startCast = (int)bomb.Time;
                        int endCast = (int)bomb.EndTime;
                        int expectedEnd = Math.Max(startCast + bomb.ExpectedDuration, endCast);
                        var circle = new CircleDecoration(480, (startCast, endCast), "rgba(180,250,0,0.3)", new AgentConnector(target));
                        replay.AddDecorationWithGrowing(circle, expectedEnd);
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.RiverOfSouls:
                    ParametricPoint3D firstRiverMovement = replay.Velocities.FirstOrDefault(x => x.Length() != 0);
                    if (firstRiverMovement != null)
                    {
                        replay.Trim(firstRiverMovement.Time - 1000, replay.TimeOffsets.end);
                    }
                    if (replay.Rotations.Any())
                    {
                        int start = (int)replay.TimeOffsets.start;
                        int end = (int)replay.TimeOffsets.end;
                        replay.Decorations.Add(new RectangleDecoration( 160, 390, (start, end), "rgba(255,100,0,0.5)", new AgentConnector(target)).UsingRotationConnector(new AgentFacingConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.Enervator:
                // TODO Line actor between desmina and enervator. Missing skillID
                case (int)ArcDPSEnums.TrashID.SpiritHorde1:
                case (int)ArcDPSEnums.TrashID.SpiritHorde2:
                case (int)ArcDPSEnums.TrashID.SpiritHorde3:
                    break;
                default:
                    break;
            }

        }
        internal override int GetTriggerID()
        {
            return (int)ArcDPSEnums.TargetID.Desmina;
        }
        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "River of Souls";
        }
    }
}
