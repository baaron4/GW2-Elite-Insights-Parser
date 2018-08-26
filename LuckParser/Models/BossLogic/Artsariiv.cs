using LuckParser.Controllers;
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class Artsariiv : FractalLogic
    {
        public Artsariiv() : base()
        {
            mechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(38880, "Corporeal Reassignment", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Artsariiv, "symbol:'square',color:'rgb(0,128,0)',", "Skull",0), //Exploding Skull mechanic application,Corporeal Reassignment
            new Mechanic(38977, "Vault", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'triangle-down-open',color:'rgb(255,200,0)',", "Vlt",0), //Vault from Big Adds, Vault (Add)
            new Mechanic(39925, "Slam", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,140,0)',", "Slam",0), //Slam (Vault) from Boss, Vault (Arts)
            new Mechanic(39469, "Teleport Lunge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'star-triangle-down-open',color:'rgb(255,140,0)',", "Jmp",0), //Triple Jump Mid->Edge, Triple Jump
            new Mechanic(39470, "Obliterate", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)',", "1Sht",0), //After Phase Expanding Oneshot, Oneshot Circle
            new Mechanic(39247, "Obliterate", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)',", "1Sht2",0), //Purple Storm One Shot, Purple 1shot Circles
            new Mechanic(39557, "Mib Ring", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'square',color:'rgb(140,0,140)',", "Goo",0), //Purple Goo below Boss(es), Purple Goo //percentag, not tracked
            new Mechanic(39398, "Mib Ring", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'square',color:'rgb(140,0,140)',", "Goo",0), //Purple Goo below Boss(es), Purple Goo //percentag, not tracked
            new Mechanic(39035, "Astral Surge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle-open',color:'rgb(255,200,0)',", "Flr",0), //Different sized spiraling circles, 1000 Circles
            new Mechanic(39029, "Red Marble", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)',", "Marble",0), // Red KD Marble after Jump, Red Marble
            new Mechanic(39863, "Red Marble", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)',", "Marble",0), // Red KD Marble after Jump, Red Marble
            new Mechanic(39238, "Beaming Smile", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Artsariiv, "symbol:'y-up-open',color:'rgb(255,0,0)',", "DRay",0), // Three Death Rays before jump, Triple Death Ray  // percentage based, only absorbed hits (from downed state or mistlock singularity) appear in logs
            //new Mechanic(39275, "Corporeal Reassignment", Mechanic.MechType.PlayerSkill, ParseEnum.BossIDS.Artsariiv, "symbol:'square',color:'rgb(0,0,0)',", "Bomb",0), //Exploding Skull hit,Corporeal Reassignment hit
            new Mechanic(791, "Fear", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Artsariiv, "symbol:'square-open',color:'rgb(255,0,0)',", "Eye",0,delegate(long value){return value == 3000;}), //Hit by the Overhead Eye Fear, Eye (Fear) //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new Mechanic(17630, "Spark", Mechanic.MechType.Spawn, ParseEnum.BossIDS.Artsariiv, "symbol: 'star', color: 'rgb(0,255,255)',","Sprk",0), //Spawned a Spark (missed marble), Spark
            //new Mechanic(39238, "Beaming Smile", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,100,0)',", "DRay",0), // Three Death Rays before jump, Triple Death Ray  // percentage based, only absorbed hits (from downed state or mistlock singularity) appear in logs
            new Mechanic(39475, "Nightmare Discharge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(0,0,0)',", "KBWave",0), // kb wave //not tracked because no damage?

            // 39475,Nightmare Discharge (Knockback wave)
            // 39275, Corporeal Reassignment (attack hit)
            //new Mechanic(39442, "Blinding Radiance", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Artsariiv, "symbol:'square-open',color:'rgb(255,200,0)',", "B.Eye",0), //Hit by the Overhead Eye Fear, Eye (Fear) //apparently only the cast/buff on boss, not the hit, check 'Fear' for that (see below)
            //new Mechanic(39582, "Head Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(255,0,0)',", "HK",0),
            //new Mechanic(38894, "Head Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(255,100,0)',", "HK2",0),
            //new Mechanic(39609, "Taw Shot", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(255,180,0)',", "TS",0),
            //new Mechanic(38991, "Taw Shot", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(255,255,0)',", "TS2",0),
            //new Mechanic(39171, "Taw Shot", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(255,255,100)',", "TS3",0),
            //new Mechanic(39417, "Taw Shot", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(255,255,180)',", "TS4",0),
            //new Mechanic(39454, "Taw Shot", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(255,100,100)',", "TS5",0),
            //new Mechanic(39748, "Taw Shot", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(255,100,180)',", "TS6",0),
            //new Mechanic(39160, "Taw Shot", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(255,100,255)',", "TS7",0),
            //new Mechanic(39648, "Jab", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(255,180,100)',", "Jab",0),
            //new Mechanic(39915, "Jab", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(255,180,180)',", "Jab2",0),
            //new Mechanic(39466, "Uppercut", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(255,180,255)',", "UC",0),
            //new Mechanic(39375, "Uppercut", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(0,0,0)',", "UC2",0),
            //new Mechanic(39388, "Jab Combo", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(0,0,0)',", "JC",0),
            //new Mechanic(39116, "Jab Combo", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(0,0,100)',", "JC2",0),
            //new Mechanic(39159, "Time Pocket", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(0,0,180)',", "TP",0),
            //new Mechanic(39357, "Globola Marble", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(0,0,255)',", "GM",0),
            //new Mechanic(39745, "Globola Marble", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(0,100,0)',", "GM2",0),
            //new Mechanic(39433, "Flying Knee", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(0,180,0)',", "FK",0),
            //new Mechanic(39504, "Flying Knee", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(0,255,0)',", "FK2",0),
            //new Mechanic(39237, "Cosmic Energy", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(0,100,100)',", "CE",0),
            //new Mechanic(39082, "Lightning Bolt", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(0,100,180)',", "LB",0),
            //new Mechanic(39103, "Electrocute", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(0,100,255)',", "Ele",0),
            //new Mechanic(39539, "Shared Shock", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-open',color:'rgb(0,180,0)',", "SS",0),
            });
        }

        public override CombatReplayMap getCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/4wmuc8B.png",
                            Tuple.Create(914, 914),
                            Tuple.Create(8991, 112, 11731, 2812),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
        }
    
        public override string getReplayIcon()
        {
            return "https://wiki.guildwars2.com/images/b/b4/Artsariiv.jpg";
        }
    }
}
