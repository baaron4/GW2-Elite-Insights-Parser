using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;

namespace LuckParser.Models
{
    public class Skorvald : FractalLogic
    {
        public Skorvald()
        { 
            mechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(39916, "Combustion Rush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-left',color:'rgb(255,0,255)',", "Chrg",0), //Combustion Rush (platform charge), Charge
            new Mechanic(39615, "Combustion Rush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-left',color:'rgb(255,0,255)',", "Chrg",0), //Combustion Rush (platform charge), Charge
            new Mechanic(39581, "Combustion Rush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-left',color:'rgb(255,0,255)',", "Chrg",0), //Combustion Rush (platform charge), Charge
            new Mechanic(39910, "Punishing Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right',color:'rgb(255,0,0)',", "Kick",0), // Punishing Kick (Single purple Line), Kick
            new Mechanic(39534, "Cranial Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right',color:'rgb(255,200,0)',", "CnKB",0), //Cranial Cascade (3 purple Line Knockback), Small Cone KB
            new Mechanic(39686, "Cranial Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right',color:'rgb(255,200,0)',", "CnKB",0), //Cranial Cascade (3 purple Line Knockback), Small Cone KB
            new Mechanic(39845, "Radiant Fury", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'octagon',color:'rgb(255,0,0)',", "BrnCrcl",0), //Radiant Fury (expanding burn circles), Expanding Circles
            new Mechanic(39257, "Focused Rage", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right',color:'rgb(255,100,0)',", "LCnKB",0), //Focused Rage (Large Cone Overhead Crosshair Knockback), Large Cone Knockback
            new Mechanic(39031, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(128,0,0)',", "HS",0), // Horizon Strike (turning pizza slices), Horizon Strike
            new Mechanic(39507, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(128,0,0)',", "HS",0), // Horizon Strike (turning pizza slices), Horizon Strik
            new Mechanic(39846, "Crimson Dawn", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(50,0,0)',", "HS.end",0), //Crimson Dawn (almost Full platform attack after Horizon Strike), Horzion Strike (last)
            new Mechanic(39228, "Solar Cyclone", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'asterisk-open',color:'rgb(140,0,140)',", "Cycle",0), //Solar Cyclone (Circling Knockback), KB Cyclone
            new Mechanic(39442, "Blinding Radiance", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'diamond-wide',color:'rgb(255,0,0)',", "Eye",0), //Hit by the Overhead Eye Fear, Eye (Fear)
            //new Mechanic(39238, "Beaming Smile", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(255,0,0)',", "Death Ray",0), ID from Arts, probably incorrect
            new Mechanic(39131, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Skorvald, "symbol:'star-open',color:'rgb(255,0,255)',", "Blm.Fix",0), // Fixated by Solar Bloom, Bloom Fixate
            new Mechanic(39491, "Explode", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(255,200,0)',", "Blm.Exp",0), //Hit by Solar Bloom Explosion, Bloom Explosion
            });         
        }

        public override CombatReplayMap GetCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/PO3aoJD.png",
                            Tuple.Create(1759, 1783),
                            Tuple.Create(-22267, 14955, -17227, 20735),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
        }

        public override int IsCM(List<CombatItem> clist, int health)
        {
            return (health == 5551340) ? 1 : 0;
        }

        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/IOPAHRE.png";
        }
    }
}
