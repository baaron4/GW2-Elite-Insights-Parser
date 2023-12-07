using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using GW2EIEvtcParser.Exceptions;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class StatueOfIce : HallOfChains
    {
        // TODO - add CR icons and some mechanics
        public StatueOfIce(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstHitMechanic(KingsWrath, "King's Wrath", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.LightBlue), "Cone Hit","King's Wrath (Auto Attack Cone Part)", "Cone Auto Attack",0),
            new PlayerDstHitMechanic(NumbingBreach, "Numbing Breach", new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.LightBlue), "Cracks","Numbing Breach (Ice Cracks in the Ground)", "Cracks",0),
            new PlayerDstBuffApplyMechanic(FrozenWind, "Frozen Wind", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Green), "Green","Frozen Wind (Stood in Green)", "Green Stack",0),
            }
            );
            Extension = "brokenking";
            Icon = EncounterIconStatueOfIce;
            EncounterCategoryInformation.InSubCategoryOrder = 2;
            EncounterID |= 0x000003;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayStatueOfIce,
                            (999, 890),
                            (2497, 5388, 7302, 9668)/*,
                            (-21504, -12288, 24576, 12288),
                            (19072, 15484, 20992, 16508)*/);
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            AgentItem brokenKing = agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.BrokenKing).FirstOrDefault();
            if (brokenKing == null)
            {
                throw new MissingKeyActorsException("Broken King not found");
            }
            long startToUse = GetGenericFightOffset(fightData);
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogStartNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                CombatItem initialCast = combatData.FirstOrDefault(x => x.StartCasting() && x.SkillID == BrokenKingFirstCast && x.SrcMatchesAgent(brokenKing));
                if (initialCast != null)
                {
                    startToUse = initialCast.Time;
                } 
                else
                {
                    startToUse = GetPostLogStartNPCUpdateDamageEventTime(fightData, agentData, combatData, logStartNPCUpdate.Time, brokenKing);
                }
            }
            return startToUse;
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
            var green = log.CombatData.GetBuffData(FrozenWind).Where(x => x.To == p.AgentItem && x is BuffApplyEvent).ToList();
            foreach (AbstractBuffEvent c in green)
            {
                int duration = 45000;
                AbstractBuffEvent removedBuff = log.CombatData.GetBuffRemoveAllData(FrozenWind).FirstOrDefault(x => x.To == p.AgentItem && x.Time > c.Time && x.Time < c.Time + duration);
                int start = (int)c.Time;
                int end = start + duration;
                if (removedBuff != null)
                {
                    end = (int)removedBuff.Time;
                }
                replay.Decorations.Add(new CircleDecoration(100, (start, end), "rgba(100, 200, 255, 0.25)", new AgentConnector(p)));
            }
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.BrokenKing:
                    var Cone = cls.Where(x => x.SkillId == KingsWrath).ToList();
                    foreach (AbstractCastEvent c in Cone)
                    {
                        int start = (int)c.Time;
                        int end = (int)c.EndTime;
                        int range = 450;
                        int angle = 100;
                        Point3D facing = target.GetCurrentRotation(log, start + 1000);
                        if (facing == null)
                        {
                            continue;
                        }
                        var connector = new AgentConnector(target);
                        var rotationConnector = new AngleConnector(facing);
                        replay.Decorations.Add(new PieDecoration(range, angle, (start, end), "rgba(0,100,255,0.2)", connector).UsingRotationConnector(rotationConnector));
                        replay.Decorations.Add(new PieDecoration(range, angle, (start + 1900, end), "rgba(0,100,255,0.3)", connector).UsingRotationConnector(rotationConnector));
                    }
                    break;
                default:
                    break;
            }

        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(BitingAura, BitingAura), // Biting Aura
            };
        }
        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            NoBouncyChestGenericCheckSucess(combatData, agentData, fightData, playerAgents);
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Statue of Ice";
        }
    }
}
