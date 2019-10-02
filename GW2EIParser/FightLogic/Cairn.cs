using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Logic
{
    public class Cairn : RaidLogic
    {
        public Cairn(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            // (ID, ingame name, Type, BossID, plotly marker, Table header name, ICD, Special condition) // long table hover name, graph legend name
            new HitOnPlayerMechanic(38113, "Displacement", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Port","Orange Teleport Field", "Orange TP",0),
            new HitOnPlayerMechanic(37611, "Spatial Manipulation", new MechanicPlotlySetting("circle","rgb(0,255,0)"), "Green","Green Spatial Manipulation Field (lift)", "Green (lift)",0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(37629, "Spatial Manipulation", new MechanicPlotlySetting("circle","rgb(0,255,0)"), "Green","Green Spatial Manipulation Field (lift)", "Green (lift)",0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(37642, "Spatial Manipulation", new MechanicPlotlySetting("circle","rgb(0,255,0)"), "Green","Green Spatial Manipulation Field (lift)", "Green (lift)",0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(37673, "Spatial Manipulation", new MechanicPlotlySetting("circle","rgb(0,255,0)"), "Green","Green Spatial Manipulation Field (lift)", "Green (lift)",0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(38074, "Spatial Manipulation", new MechanicPlotlySetting("circle","rgb(0,255,0)"), "Green","Green Spatial Manipulation Field (lift)", "Green (lift)",0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(38302, "Spatial Manipulation", new MechanicPlotlySetting("circle","rgb(0,255,0)"), "Green","Green Spatial Manipulation Field (lift)", "Green (lift)",0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(37611, "Spatial Manipulation", new MechanicPlotlySetting("circle-open","rgb(0,255,0)"), "Stab.Green","Green Spatial Manipulation Field while affected by stability", "Stabilized Green",0, (de, log) => de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(37629, "Spatial Manipulation", new MechanicPlotlySetting("circle-open","rgb(0,255,0)"), "Stab.Green","Green Spatial Manipulation Field while affected by stability", "Stabilized Green",0, (de, log) => de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(37642, "Spatial Manipulation", new MechanicPlotlySetting("circle-open","rgb(0,255,0)"), "Stab.Green","Green Spatial Manipulation Field while affected by stability", "Stabilized Green",0, (de, log) => de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(37673, "Spatial Manipulation", new MechanicPlotlySetting("circle-open","rgb(0,255,0)"), "Stab.Green","Green Spatial Manipulation Field while affected by stability", "Stabilized Green",0, (de, log) => de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(38074, "Spatial Manipulation", new MechanicPlotlySetting("circle-open","rgb(0,255,0)"), "Stab.Green","Green Spatial Manipulation Field while affected by stability", "Stabilized Green",0, (de, log) => de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(38302, "Spatial Manipulation", new MechanicPlotlySetting("circle-open","rgb(0,255,0)"), "Stab.Green","Green Spatial Manipulation Field while affected by stability", "Stabilized Green",0, (de, log) => de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(31875, "Spatial Manipulation", new MechanicPlotlySetting("hexagram","rgb(255,0,0)"), "Slam","Spectral Impact (KB Slam)", "Slam",4000, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(38313, "Meteor Swarm", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "KB","Knockback Crystals (tornado like)", "KB Crystal",1000),
            new PlayerBuffApplyMechanic(38049, "Shared Agony", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Agony","Shared Agony Debuff Application", "Shared Agony",0),//could flip
            new PlayerBuffApplyMechanic(38170, "Shared Agony", new MechanicPlotlySetting("star-triangle-up-open","rgb(255,150,0)"), "Agony 25","Shared Agony Damage (25% Player's HP)", "SA dmg 25%",0), // Seems to be a (invisible) debuff application for 1 second from the Agony carrier to the closest(?) person in the circle.
            new PlayerBuffApplyMechanic(37768, "Shared Agony", new MechanicPlotlySetting("star-diamond-open","rgb(255,50,0)"), "Agony 50","Shared Agony Damage (50% Player's HP)", "SA dmg 50%",0), //Chaining from the first person hit by 38170, applying a 1 second debuff to the next person.
            new PlayerBuffApplyMechanic(38209, "Shared Agony", new MechanicPlotlySetting("star-open","rgb(200,0,0)"), "Agony 75","Shared Agony Damage (75% Player's HP)", "SA dmg 75%",0), //Chaining from the first person hit by 37768, applying a 1 second debuff to the next person.
            // new Mechanic(37775, "Shared Agony", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Agony Damage",0), from old raidheroes logs? Small damage packets. Is also named "Shared Agony" in the evtc. Doesn't seem to occur anymore.
            // new Mechanic(38210, "Shared Agony", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "SA.dmg","Shared Agony Damage dealt", "Shared Agony dmg",0), //could flip. HP% attack, thus only shows on down/absorb hits.
            new HitOnPlayerMechanic(38060, "Energy Surge", new MechanicPlotlySetting("triangle-left","rgb(0,128,0)"), "Leap","Jump between green fields", "Leap",100),
            new HitOnPlayerMechanic(37631, "Orbital Sweep", new MechanicPlotlySetting("diamond-wide","rgb(255,0,255)"), "Sweep","Sword Spin (Knockback)", "Sweep",100),//short cooldown because of multihits. Would still like to register second hit at the end of spin though, thus only 0.1s
            new HitOnPlayerMechanic(37910, "Gravity Wave", new MechanicPlotlySetting("octagon","rgb(255,0,255)"), "Donut","Expanding Crystal Donut Wave (Knockback)", "Crystal Donut",0)
            // Spatial Manipulation IDs correspond to the following: 1st green when starting the fight: 37629;
            // Greens after Energy Surge/Orbital Sweep: 38302
            //100% - 75%: 37611
            // 75% - 50%: 38074
            // 50% - 25%: 37673
            // 25% -  0%: 37642
            });
            Extension = "cairn";
            Icon = "https://wiki.guildwars2.com/images/b/b8/Mini_Cairn_the_Indomitable.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/NlpsLZa.png",
                            (607, 607),
                            (12981, 642, 15725, 3386),
                            (-27648, -9216, 27648, 12288),
                            (11774, 4480, 14078, 5376));
        }

        public override void ComputeTargetCombatReplayActors(Target target, ParsedLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Cairn:
                    var swordSweep = cls.Where(x => x.SkillId == 37631).ToList();
                    foreach (AbstractCastEvent c in swordSweep)
                    {
                        int start = (int)c.Time;
                        int preCastTime = 1400;
                        int initialHitDuration = 850;
                        int sweepDuration = 1100;
                        int width = 1400; int height = 80;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing != null)
                        {
                            int initialDirection = (int)(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI);
                            replay.Actors.Add(new RotatedRectangleActor(true, 0, width, height, initialDirection, width / 2, (start, start + preCastTime), "rgba(200, 0, 255, 0.1)", new AgentConnector(target)));
                            replay.Actors.Add(new RotatedRectangleActor(true, 0, width, height, initialDirection, width / 2, (start + preCastTime, start + preCastTime + initialHitDuration), "rgba(150, 0, 180, 0.5)", new AgentConnector(target)));
                            replay.Actors.Add(new RotatedRectangleActor(true, 0, width, height, initialDirection, width / 2, 360, (start + preCastTime + initialHitDuration, start + preCastTime + initialHitDuration + sweepDuration), "rgba(150, 0, 180, 0.5)", new AgentConnector(target)));
                        }
                    }
                    var wave = cls.Where(x => x.SkillId == 37910).ToList();
                    foreach (AbstractCastEvent c in wave)
                    {
                        int start = (int)c.Time;
                        int preCastTime = 1200;
                        int duration = 600;
                        int firstRadius = 400;
                        int secondRadius = 700;
                        int thirdRadius = 1000;
                        int fourthRadius = 1300;
                        replay.Actors.Add(new DoughnutActor(true, 0, firstRadius, secondRadius, (start + preCastTime, start + preCastTime + duration), "rgba(100,0,155,0.3)", new AgentConnector(target)));
                        replay.Actors.Add(new DoughnutActor(true, 0, secondRadius, thirdRadius, (start + preCastTime + 2 * duration, start + preCastTime + 3 * duration), "rgba(100,0,155,0.3)", new AgentConnector(target)));
                        replay.Actors.Add(new DoughnutActor(true, 0, thirdRadius, fourthRadius, (start + preCastTime + 5 * duration, start + preCastTime + 6 * duration), "rgba(100,0,155,0.3)", new AgentConnector(target)));
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
        {
            // shared agony
            var agony = log.CombatData.GetBuffData(38049).Where(x => (x.To == p.AgentItem && x is BuffApplyEvent)).ToList();
            foreach (AbstractBuffEvent c in agony)
            {
                int agonyStart = (int)c.Time;
                int agonyEnd = agonyStart + 62000;
                replay.Actors.Add(new CircleActor(false, 0, 220, (agonyStart, agonyEnd), "rgba(255, 0, 0, 0.5)", new AgentConnector(p)));
            }
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return combatData.GetSkills().Contains(38098) ? 1 : 0;
        }

        public override string GetFightName()
        {
            return "Cairn";
        }
    }
}
