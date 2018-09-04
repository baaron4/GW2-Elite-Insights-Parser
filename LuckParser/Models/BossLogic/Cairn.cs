using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class Cairn : RaidLogic
    {
        public Cairn()
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
            new Mechanic(38049, "Shared Agony", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Cairn, "symbol:'circle',color:'rgb(255,0,0)',", "SA","Spectral Agony Debuff Application", "Spectral Agony",0),//could flip
            // new Mechanic(38170, "Shared Agony", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'circle-open',color:'rgb(255,0,0)',", "Agony Damage",0), from old raidheroes logs? 1k damage, seems fixed. Doesn't seem to come from Shared Agony target though.
            // new Mechanic(37768, "Shared Agony", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'circle-open',color:'rgb(255,0,0)',", "Agony Damage",0), from old raidheroes logs? 1k damage, seems fixed. Doesn't seem to come from Shared Agony target though.
            // new Mechanic(37775, "Shared Agony", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'circle-open',color:'rgb(255,0,0)',", "Agony Damage",0), from old raidheroes logs? Is also named "Shared Agony" in the evtc.
            new Mechanic(38210, "Shared Agony", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'circle-open',color:'rgb(255,0,0)',", "SA.dmg","Spectral Agony Damage dealt", "Spectral Agony dmg",0), //could flip. Sometimes doesn't appear at all (even though SA damage was dealt), can have Cairn or a player as source agent.
            new Mechanic(38060, "Energy Surge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'triangle-left',color:'rgb(0,128,0)',", "Leap","Jump between green fields", "Leap",0),
            new Mechanic(37631, "Orbital Sweep", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'diamond-wide',color:'rgb(255,0,255)',", "Sweep","Sword Spin (Knockback)", "Sweep",100),//short cooldown because of multihits. Would still like to register second hit at the end of spin though, thus only 0.1s
            new Mechanic(37910, "Gravity Wave", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Cairn, "symbol:'octagon',color:'rgb(255,0,255)',", "Donut","Expanding Crystal Donut Wave (Knockback)", "Crystal Donut",0)
            });
        }

        public override CombatReplayMap GetCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/NlpsLZa.png",
                            Tuple.Create(607, 607),
                            Tuple.Create(12981, 642, 15725, 3386),
                            Tuple.Create(-27648, -9216, 27648, 12288),
                            Tuple.Create(11774, 4480, 14078, 5376));
        }
        
        public override List<ParseEnum.ThrashIDS> GetAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            // TODO: needs doughnuts (wave) and facing information (sword)
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>();
            return ids;
        }

        public override void GetAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {
            // shared agony
            List<CombatItem> agony = log.GetBoonData(38049).Where(x => (x.DstInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.None)).ToList();
            foreach (CombatItem c in agony)
            {
                int agonyStart = (int)(c.Time - log.GetBossData().GetFirstAware());
                int agonyEnd = agonyStart + 62000;
                replay.AddCircleActor(new CircleActor(false, 0, 220, new Tuple<int, int>(agonyStart, agonyEnd), "rgba(255, 0, 0, 0.5)"));
            }
        }

        public override int IsCM(List<CombatItem> clist, int health)
        {
            return clist.Exists(x => x.SkillID == 38098) ? 1 : 0;
        }

        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/gQY37Tf.png";
        }
    }
}
