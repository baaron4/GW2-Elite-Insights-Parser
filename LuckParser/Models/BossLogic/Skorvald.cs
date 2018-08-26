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
            new Mechanic(39910, "Punishing Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right',color:'rgb(255,0,0)',", "A.Kick",0), // Punishing Kick (Single purple Line, Add), Kick (Add)
            new Mechanic(38896, "Punishing Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right',color:'rgb(255,0,0)',", "Kick",0), // Punishing Kick (Single purple Line, Add), Kick (Add)
            new Mechanic(39534, "Cranial Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right',color:'rgb(255,200,0)',", "A.CnKB",0), //Cranial Cascade (3 purple Line Knockback, Add), Small Cone KB (Add)
            new Mechanic(39686, "Cranial Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right',color:'rgb(255,200,0)',", "CnKB",0), //Cranial Cascade (3 purple Line Knockback), Small Cone KB
            new Mechanic(39845, "Radiant Fury", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'octagon',color:'rgb(255,0,0)',", "BrnCrcl",0), //Radiant Fury (expanding burn circles), Expanding Circles
            new Mechanic(38926, "Radiant Fury", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'octagon',color:'rgb(255,0,0)',", "BrnCrcl",0), //Radiant Fury (expanding burn circles), Expanding Circles
            new Mechanic(39257, "Focused Anger", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right',color:'rgb(255,100,0)',", "LCnKB",0), //Focused Anger (Large Cone Overhead Crosshair Knockback), Large Cone Knockback
            new Mechanic(39031, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(128,0,0)',", "HS",0), // Horizon Strike (turning pizza slices), Horizon Strike
            new Mechanic(39507, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(128,0,0)',", "HS",0), // Horizon Strike (turning pizza slices), Horizon Strik
            new Mechanic(39846, "Crimson Dawn", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(50,0,0)',", "HS.end",0), //Crimson Dawn (almost Full platform attack after Horizon Strike), Horzion Strike (last)
            new Mechanic(39228, "Solar Cyclone", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'asterisk-open',color:'rgb(140,0,140)',", "Cycle",0), //Solar Cyclone (Circling Knockback), KB Cyclone
            //new Mechanic(39442, "Blinding Radiance", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'diamond-wide',color:'rgb(255,0,0)',", "Eye",0), //Hit by the Overhead Eye Fear, Eye (Fear)
            //new Mechanic(39238, "Beaming Smile", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(255,0,0)',", "Death Ray",0), ID from Arts, probably incorrect
            new Mechanic(791, "Fear", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Skorvald, "symbol:'square-open',color:'rgb(255,0,0)',", "Eye",0,delegate(long value){return value == 3000;}), //Hit by the Overhead Eye Fear, Eye (Fear) //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new Mechanic(39131, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Skorvald, "symbol:'star-open',color:'rgb(255,0,255)',", "Blm.Fix",0), // Fixated by Solar Bloom, Bloom Fixate
            new Mechanic(39491, "Explode", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(255,200,0)',", "Blm.Exp",0), //Hit by Solar Bloom Explosion, Bloom Explosion    //shockwave, not damage? (damage is 50% max HP, not tracked)
            new Mechanic(39911, "Spiral Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(0,200,0)',", "SprlStr",0), //Hit by Solar Bloom Explosion, Bloom Explosion    //shockwave, not damage? (damage is 50% max HP, not tracked)
            new Mechanic(39133, "Wave of Mutilation", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(0,200,0)',", "KBJmp",0), //Hit by Solar Bloom Explosion, Bloom Explosion    //shockwave, not damage? (damage is 50% max HP, not tracked)

            //38896 Punishing Kick, Skorv
            //39673,39332 Crimson Dawn 
            //39220 Cranial Cascade
            //39770 HS 
            //39313 Solar Bolt (small balls on ground)
            //38926,Radiant Fury (Add) 1
            //39795,Focused Anger
            //39298,Solar Stomp (Jump)
            //39691,Solar Discharge (Shockwave)
            //39575,Mist Smash (Add) 2 (Attack)
            //39847,Pulsar Blast (Add) 3 (Evading KB)
            //38846,Mist-Charged Chop (Add) 3 (knockback jump)
            //39531,Skorvald's Ire (Target for Combustion Rush)
            //39416,Blinding Radiance Death Ray instead of Eye only?
            //37595, Blinding Radiance? (Fear+Blind from Eye)
            //39911, Spiral Strike (after Warp <= Bomb above Head)
            //35502,Chaos Stability Don't track, basically any knockback (probably against multiKB?), only from log PoV
            //39133,Wave of Mutilation (knockback jump, last phase)
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
