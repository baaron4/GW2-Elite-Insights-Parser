using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class Skorvald : FractalLogic
    {
        public Skorvald() : base()
        { 
            mechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(39916, "Combustion Rush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-left',color:'rgb(255,0,255)',", "Chrg",0), //Combustion Rush (3rd platform charge), Charge
            new Mechanic(39615, "Combustion Rush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-left',color:'rgb(255,0,255)',", "Chrg",0), //Combustion Rush (2nd platform charge), Charge
            new Mechanic(39581, "Combustion Rush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-left',color:'rgb(255,0,255)',", "Chrg",0), //Combustion Rush (1st platform charge), Charge 
            new Mechanic(39910, "Punishing Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right-open',color:'rgb(200,0,200)',", "A.Kick",0), // Punishing Kick (Single purple Line, Add), Kick (Add)
            new Mechanic(38896, "Punishing Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right',color:'rgb(200,0,200)',", "Kick",0), // Punishing Kick (Single purple Line), Kick
            new Mechanic(39534, "Cranial Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right-open',color:'rgb(255,200,0)',", "A.CnKB",0), //Cranial Cascade (3 purple Line Knockback, Add), Small Cone KB (Add)
            new Mechanic(39686, "Cranial Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right',color:'rgb(255,200,0)',", "CnKB",0), //Cranial Cascade (3 purple Line Knockback), Small Cone KB
            new Mechanic(39845, "Radiant Fury", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'octagon',color:'rgb(255,0,0)',", "BrnCrcl",0), //Radiant Fury (expanding burn circles), Expanding Circles
            new Mechanic(38926, "Radiant Fury", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'octagon',color:'rgb(255,0,0)',", "BrnCrcl",0), //Radiant Fury (expanding burn circles), Expanding Circles
            new Mechanic(39257, "Focused Anger", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-down',color:'rgb(255,100,0)',", "LCnKB",0), //Focused Anger (Large Cone Overhead Crosshair Knockback), Large Cone Knockback
            new Mechanic(39031, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(255,140,0)',", "HS",0), // Horizon Strike (turning pizza slices), Horizon Strike
            new Mechanic(39507, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(255,140,0)',", "HS",0), // Horizon Strike (turning pizza slices), Horizon Strik
            new Mechanic(39846, "Crimson Dawn", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(50,0,0)',", "HS.end",0), //Crimson Dawn (almost Full platform attack after Horizon Strike), Horzion Strike (last)
            new Mechanic(39228, "Solar Cyclone", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'asterisk-open',color:'rgb(140,0,140)',", "Cycln",0), //Solar Cyclone (Circling Knockback), KB Cyclone
            new Mechanic(39228, "Solar Cyclone", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'asterisk-open',color:'rgb(140,0,140)',", "Cycln",0), //Solar Cyclone (Circling Knockback), KB Cyclone
            new Mechanic(791, "Fear", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Skorvald, "symbol:'square-open',color:'rgb(255,0,0)',", "Eye",0,(value=> value == 3000)), //Hit by the Overhead Eye Fear, Eye (Fear) //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new Mechanic(39131, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Skorvald, "symbol:'star-open',color:'rgb(255,0,255)',", "Blm.Fix",0), // Fixated by Solar Bloom, Bloom Fixate
            new Mechanic(39491, "Explode", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(255,200,0)',", "Blm.Exp",0), //Hit by Solar Bloom Explosion, Bloom Explosion    //shockwave, not damage? (damage is 50% max HP, not tracked)
            new Mechanic(39911, "Spiral Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle-open',color:'rgb(0,200,0)',", "SprlStr",0), //Hit after Warp (Jump to Player with overhead bomb), Spiral Strike
            new Mechanic(39133, "Wave of Mutilation", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-sw',color:'rgb(0,200,0)',", "KBJmp",0), //Hit by KB Jump (player targeted), Knockback jump
            });         
        }

        public override CombatReplayMap getCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/PO3aoJD.png",
                            Tuple.Create(1759, 1783),
                            Tuple.Create(-22267, 14955, -17227, 20735),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
        }

        public override int isCM(List<CombatItem> clist, int health)
        {
            return (health == 5551340) ? 1 : 0;
        }

        public override string getReplayIcon()
        {
            return "https://i.imgur.com/IOPAHRE.png";
        }
    }
}
