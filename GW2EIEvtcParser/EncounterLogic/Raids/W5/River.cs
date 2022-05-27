using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class River : HallOfChains
    {
        public River(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new HitOnPlayerMechanic(48272, "Bombshell", new MechanicPlotlySetting(Symbols.Circle,Colors.Orange),"Bomb Hit", "Hit by Hollowed Bomber Exlosion", "Hit by Bomb", 0 ),
                new HitOnPlayerMechanic(47258, "Timed Bomb", new MechanicPlotlySetting(Symbols.Square,Colors.Orange),"Stun Bomb", "Stunned by Mini Bomb", "Stun Bomb", 0, (de, log) => !de.To.HasBuff(log, SkillIDs.Stability, de.Time - ParserHelper.ServerDelayConstant)),
            }
            );
            GenericFallBackMethod = FallBackMethod.None;
            Extension = "river";
            Targetless = true;
            Icon = "https://wiki.guildwars2.com/images/thumb/7/7b/Gold_River_of_Souls_Trophy.jpg/220px-Gold_River_of_Souls_Trophy.jpg";
            EncounterCategoryInformation.InSubCategoryOrder = 1;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/BMqQKqb.png",
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

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                AgentItem desmina = agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.Desmina).FirstOrDefault();
                if (desmina == null)
                {
                    throw new MissingKeyActorsException("Desmina not found");
                }
                ExitCombatEvent ooc = combatData.GetExitCombatEvents(desmina).LastOrDefault();
                if (ooc != null)
                {
                    long time = 0;
                    foreach (NPC mob in TrashMobs)
                    {
                        time = Math.Max(mob.LastAware, time);
                    }
                    DespawnEvent dspwn = combatData.GetDespawnEvents(desmina).LastOrDefault();
                    if (time != 0 && dspwn == null && time + 500 <= desmina.LastAware)
                    {
                        if (!AtLeastOnePlayerAlive(combatData, fightData, time, playerAgents))
                        {
                            return;
                        }
                        fightData.SetSuccess(true, time);
                    }
                }
            }
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            agentData.AddCustomNPCAgent(0, fightData.FightEnd, "River of Souls", Spec.NPC, (int)ArcDPSEnums.TargetID.DummyTarget, true);
            ComputeFightTargets(agentData, combatData, extensions);
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            // TODO bombs dual following circle actor (one growing, other static) + dual static circle actor (one growing with min radius the final radius of the previous, other static). Missing buff id
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Desmina:
                    List<AbstractBuffEvent> asylums = GetFilteredList(log.CombatData, FollowersAsylum, target, true, true);
                    int asylumStart = 0;
                    foreach (AbstractBuffEvent asylum in asylums)
                    {
                        if (asylum is BuffApplyEvent)
                        {
                            asylumStart = (int)asylum.Time;
                        }
                        else
                        {
                            int asylumEnd = (int)asylum.Time;
                            replay.Decorations.Add(new CircleDecoration(true, 0, 300, (asylumStart, asylumEnd), "rgba(0, 160, 255, 0.3)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.HollowedBomber:
                    Point3D firstBomberMovement = replay.Velocities.FirstOrDefault(x => x.Length() != 0);
                    if (firstBomberMovement != null)
                    {
                        replay.Trim(firstBomberMovement.Time - 1000, replay.TimeOffsets.end);
                    }
                    var bomberman = target.GetCastEvents(log, 0, log.FightData.FightEnd).Where(x => x.SkillId == 48272).ToList();
                    foreach (AbstractCastEvent bomb in bomberman)
                    {
                        int startCast = (int)bomb.Time;
                        int endCast = (int)bomb.EndTime;
                        int expectedEnd = Math.Max(startCast + bomb.ExpectedDuration, endCast);
                        replay.Decorations.Add(new CircleDecoration(true, 0, 480, (startCast, endCast), "rgba(180,250,0,0.3)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, expectedEnd, 480, (startCast, endCast), "rgba(180,250,0,0.3)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.RiverOfSouls:
                    Point3D firstRiverMovement = replay.Velocities.FirstOrDefault(x => x.Length() != 0);
                    if (firstRiverMovement != null)
                    {
                        replay.Trim(firstRiverMovement.Time - 1000, replay.TimeOffsets.end);
                    }
                    if (replay.Rotations.Count > 0)
                    {
                        int start = (int)replay.TimeOffsets.start;
                        int end = (int)replay.TimeOffsets.end;
                        replay.Decorations.Add(new FacingRectangleDecoration((start, end), new AgentConnector(target), replay.PolledRotations, 160, 390, "rgba(255,100,0,0.5)"));
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

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "River of Souls";
        }
    }
}
