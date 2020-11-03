﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class TwistedCastle : RaidLogic
    {
        public TwistedCastle(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerBuffApplyMechanic(34918, "Spatial Distortion", new MechanicPlotlySetting("circle","rgb(255,0,255)"), "Statue TP", "Teleported by Statue", "Statue Teleport", 500),
                new PlayerBuffApplyMechanic(35106, "Still Waters", new MechanicPlotlySetting("diamond-tall","rgb(255,0,255)"), "Still Waters (Immunity)", "Used a fountain for immunity", "Still Waters (Immunity)", 0, (evt, log) => log.CombatData.GetBuffData(34955).Exists(x => x is BuffApplyEvent ba && ba.To == evt.To && Math.Abs(ba.Time - evt.Time) < 500)),
                new PlayerBuffApplyMechanic(35106, "Still Waters", new MechanicPlotlySetting("diamond-tall","rgb(255,0,255)"), "Still Waters (Removal)", "Used a fountain for stack removal", "Still Waters (Removal)", 0, (evt, log) => !log.CombatData.GetBuffData(34955).Exists(x => x is BuffApplyEvent ba && ba.To == evt.To && Math.Abs(ba.Time - evt.Time) < 500)),
                new PlayerBuffApplyMechanic(35006, "Madness", new MechanicPlotlySetting("square","rgb(200,140,255)"), "Madness", "Stacking debuff", "Madness", 0),
                new PlayerBuffApplyMechanic(34963, "Chaotic Haze", new MechanicPlotlySetting("hexagon","rgb(255,0,0)"), "Chaotic Haze", "Damaging Debuff from bombardement", "Chaotic Haze", 500),
            }
            );
            Extension = "twstcstl";
            GenericFallBackMethod = FallBackMethod.None;
            Targetless = true;
            Icon = "https://i.imgur.com/xpQnu35.png";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/2RkzdmL.png",
                            (1353, 1748),
                            (-8058, -4321, 819, 7143),
                            (-12288, -27648, 12288, 27648),
                            (1920, 12160, 2944, 14464));
        }

        internal override void EIEvtcParse(FightData fightData, AgentData agentData, List<CombatItem> combatData, List<Player> playerList)
        {
            agentData.AddCustomAgent(fightData.FightStart, fightData.FightEnd, AgentItem.AgentType.NPC, "Twisted Castle", "", (int)ArcDPSEnums.TargetID.TwistedCastle);
            ComputeFightTargets(agentData, combatData);
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
               ArcDPSEnums.TrashID.HauntingStatue,
               //ParseEnum.TrashIDS.CastleFountain
            };
        }

        internal override void ComputeNPCCombatReplayActors(NPC npc, ParsedEvtcLog log, CombatReplay replay)
        {
            switch (npc.ID)
            {
                case (int)ArcDPSEnums.TrashID.HauntingStatue:
                    var lifespan = ((int)replay.TimeOffsets.start, (int)replay.TimeOffsets.end);
                    if (replay.Rotations.Any())
                    {
                        replay.Decorations.Add(new FacingDecoration(lifespan, new AgentConnector(npc), replay.PolledRotations));
                    }
                    break;
                //case (ushort)ParseEnum.TrashIDS.CastleFountain:
                //    break;
                default:
                    break;
            }
        }

        internal override string GetLogicName(ParsedEvtcLog log)
        {
            return "Twisted Castle";
        }
    }
}
