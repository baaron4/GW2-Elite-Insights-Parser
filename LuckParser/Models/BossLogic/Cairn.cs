using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class Cairn : RaidLogic
    {
        public Cairn(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            // (ID, ingame name, Type, BossID, plotly marker, Table header name, ICD, Special condition) // long table hover name, graph legend name
            new Mechanic(38113, "Displacement", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'circle',color:'rgb(255,140,0)',", "TP","Orange Teleport Field", "Orange TP",0), 
            new Mechanic(37611, "Spatial Manipulation", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'circle',color:'rgb(0,255,0)',", "Grn","Green Spatial Manipulation Field (lift)", "Green",0), 
            new Mechanic(37629, "Spatial Manipulation", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'circle',color:'rgb(0,255,0)',", "Grn","Green Spatial Manipulation Field (lift)", "Green",0),
            new Mechanic(37642, "Spatial Manipulation", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'circle',color:'rgb(0,255,0)',", "Grn","Green Spatial Manipulation Field (lift)", "Green",0),
            new Mechanic(37673, "Spatial Manipulation", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'circle',color:'rgb(0,255,0)',", "Grn","Green Spatial Manipulation Field (lift)", "Green",0),
            new Mechanic(38074, "Spatial Manipulation", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'circle',color:'rgb(0,255,0)',", "Grn","Green Spatial Manipulation Field (lift)", "Green",0),
            new Mechanic(38302, "Spatial Manipulation", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'circle',color:'rgb(0,255,0)',", "Grn","Green Spatial Manipulation Field (lift)", "Green",0),
            new Mechanic(38313, "Meteor Swarm", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'diamond-tall',color:'rgb(255,0,0)',", "KB","Knockback Crystals (tornado like)", "KB Crystal",1000),
            new Mechanic(38049, "Shared Agony", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Cairn, "symbol:'circle',color:'rgb(255,0,0)',", "SA","Shared Agony Debuff Application", "Shared Agony",0),//could flip
            new Mechanic(38170, "Shared Agony", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Cairn, "symbol:'star-triangle-up-open',color:'rgb(255,150,0)',", "SA.25","Shared Agony Damage (25% Player's HP)", "SA dmg 25%",0), // Seems to be a (invisible) debuff application for 1 second from the Agony carrier to the closest(?) person in the circle.
            new Mechanic(37768, "Shared Agony", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Cairn, "symbol:'star-diamond-open',color:'rgb(255,50,0)',", "SA.50","Shared Agony Damage (50% Player's HP)", "SA dmg 50%",0), //Chaining from the first person hit by 38170, applying a 1 second debuff to the next person.
            new Mechanic(38209, "Shared Agony", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Cairn, "symbol:'star-open',color:'rgb(200,0,0)',", "SA.75","Shared Agony Damage (75% Player's HP)", "SA dmg 75%",0), //Chaining from the first person hit by 37768, applying a 1 second debuff to the next person.
            // new Mechanic(37775, "Shared Agony", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'circle-open',color:'rgb(255,0,0)',", "Agony Damage",0), from old raidheroes logs? Small damage packets. Is also named "Shared Agony" in the evtc. Doesn't seem to occur anymore.
            // new Mechanic(38210, "Shared Agony", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'circle-open',color:'rgb(255,0,0)',", "SA.dmg","Shared Agony Damage dealt", "Shared Agony dmg",0), //could flip. HP% attack, thus only shows on down/absorb hits.
            new Mechanic(38060, "Energy Surge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'triangle-left',color:'rgb(0,128,0)',", "Leap","Jump between green fields", "Leap",100),
            new Mechanic(37631, "Orbital Sweep", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'diamond-wide',color:'rgb(255,0,255)',", "Sweep","Sword Spin (Knockback)", "Sweep",100),//short cooldown because of multihits. Would still like to register second hit at the end of spin though, thus only 0.1s
            new Mechanic(37910, "Gravity Wave", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'octagon',color:'rgb(255,0,255)',", "Donut","Expanding Crystal Donut Wave (Knockback)", "Crystal Donut",0)

            });
            Extension = "cairn";
            IconUrl = "https://wiki.guildwars2.com/images/b/b8/Mini_Cairn_the_Indomitable.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/NlpsLZa.png",
                            Tuple.Create(607, 607),
                            Tuple.Create(12981, 642, 15725, 3386),
                            Tuple.Create(-27648, -9216, 27648, 12288),
                            Tuple.Create(11774, 4480, 14078, 5376));
        }
        
        public override void ComputeAdditionalBossData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            // TODO: needs doughnuts (wave) and facing information (sword)
        }

        public override void ComputeAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {
            // shared agony
            List<CombatItem> agony = log.GetBoonData(38049).Where(x => (x.DstInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.None)).ToList();
            foreach (CombatItem c in agony)
            {
                int agonyStart = (int)(c.Time - log.FightData.FightStart);
                int agonyEnd = agonyStart + 62000;
                replay.Actors.Add(new CircleActor(false, 0, 220, new Tuple<int, int>(agonyStart, agonyEnd), "rgba(255, 0, 0, 0.5)"));
            }
        }

        public override int IsCM(ParsedLog log)
        {
            return log.CombatData.Exists(x => x.SkillID == 38098) ? 1 : 0;
        }

        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/gQY37Tf.png";
        }
    }
}
