using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class FraenirOfJormag : StrikeMissionLogic
    {
        public FraenirOfJormag(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new HitOnPlayerMechanic(58811, "Icequake", new MechanicPlotlySetting("hexagram","rgb(255,0,0)"), "Icequake","Icequake", "Icequake",4000, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
                new HitOnPlayerMechanic(58740, "Ice Shock Wave", new MechanicPlotlySetting("square","rgb(255,0,0)"), "Ice Shock Wave","Ice Shock Wave", "Ice Shock Wave",4000, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
                new PlayerBuffApplyMechanic(58376, "Frozen", new MechanicPlotlySetting("circle","rgb(0,0,255)"), "Frozen","Frozen", "Frozen",500),
                new PlayerBuffRemoveMechanic(58376, "Unfrozen", new MechanicPlotlySetting("circle-open","rgb(0,0,255)"), "Unfrozen","Unfrozen", "Unfrozen",500),
                new PlayerBuffApplyMechanic(58276, "Snowblind", new MechanicPlotlySetting("square","rgb(0,0,255)"), "Snowblind","Snowblind", "Snowblind",500),
            }
            );
            Extension = "fraenir";
            Icon = "https://wiki.guildwars2.com/images/thumb/6/67/Fraenir_of_Jormag.jpg/208px-Fraenir_of_Jormag.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/kLjZ7eU.png",
                            (905, 789),
                            (-833, -1780, 2401, 1606),
                            (-0, -0, 0, 0),
                            (0, 0, 0, 0));
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC fraenir = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.FraenirOfJormag);
            if (fraenir == null)
            {
                throw new InvalidOperationException("Fraenir of Jormag not found");
            }
            phases[0].Targets.Add(fraenir);
            NPC icebrood = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.IcebroodConstructFraenir);
            if (icebrood != null)
            {
                phases[0].Targets.Add(icebrood);
            }
            if (!requirePhases)
            {
                return phases;
            }
            AbstractBuffEvent invulApplyFraenir = log.CombatData.GetBuffData(762).Where(x => x.To == fraenir.AgentItem && x is BuffApplyEvent).FirstOrDefault();
            if (invulApplyFraenir != null)
            {
                // split happened
                phases.Add(new PhaseData(0, invulApplyFraenir.Time, "Fraenir 1"));
                if (icebrood != null)
                {
                    // icebrood enters combat
                    EnterCombatEvent enterCombatIce = log.CombatData.GetEnterCombatEvents(icebrood.AgentItem).LastOrDefault();
                    if (enterCombatIce != null)
                    {
                        // icebrood phasing
                        AbstractBuffEvent invulApplyIce = log.CombatData.GetBuffData(757).Where(x => x.To == icebrood.AgentItem && x is BuffApplyEvent).FirstOrDefault();
                        AbstractBuffEvent invulRemoveIce = log.CombatData.GetBuffData(757).Where(x => x.To == icebrood.AgentItem && x is BuffRemoveAllEvent).FirstOrDefault();
                        long icebroodStart = enterCombatIce.Time;
                        long icebroodEnd = log.FightData.FightEnd;
                        if (invulApplyIce != null && invulRemoveIce != null)
                        {
                            long icebrood2Start = invulRemoveIce.Time;
                            phases.Add(new PhaseData(icebroodStart + 1, invulApplyIce.Time, "Icebrood 1"));
                            AbstractBuffEvent invulRemoveFraenir = log.CombatData.GetBuffData(762).Where(x => x.To == fraenir.AgentItem && x is BuffRemoveAllEvent).FirstOrDefault();
                            if (invulRemoveFraenir != null)
                            {
                                // fraenir came back
                                DeadEvent deadIce = log.CombatData.GetDeadEvents(icebrood.AgentItem).LastOrDefault();
                                if (deadIce != null)
                                {
                                    icebroodEnd = deadIce.Time;
                                }
                                else
                                {
                                    icebroodEnd = invulRemoveFraenir.Time - 1;
                                }
                                phases.Add(new PhaseData(invulRemoveFraenir.Time, log.FightData.FightEnd, "Fraenir 2"));
                            }
                            phases.Add(new PhaseData(icebrood2Start, icebroodEnd, "Icebrood 2"));
                        }
                        phases.Add(new PhaseData(icebroodStart, icebroodEnd, "Icebrood"));
                    }
                }
            }
            phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i == 1 || i == 5)
                {
                    phase.Targets.Add(fraenir);
                } else
                {
                    phase.Targets.Add(icebrood);
                }
            }
            return phases;
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.FraenirOfJormag,
                (int)ArcDPSEnums.TargetID.IcebroodConstructFraenir,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.IcebroodElemental
            };
        }
    }
}
