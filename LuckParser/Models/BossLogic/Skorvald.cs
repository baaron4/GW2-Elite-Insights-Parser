using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class Skorvald : FractalLogic
    {
        public Skorvald()
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(39916, "Combustion Rush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-left',color:'rgb(255,0,255)',", "Chrg","Combustion Rush", "Charge",0),
            new Mechanic(39615, "Combustion Rush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-left',color:'rgb(255,0,255)',", "Chrg","Combustion Rush", "Charge",0),
            new Mechanic(39581, "Combustion Rush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-left',color:'rgb(255,0,255)',", "Chrg","Combustion Rush", "Charge ",0),
            new Mechanic(39910, "Punishing Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right-open',color:'rgb(200,0,200)',", "A.Kick","Punishing Kick (Single purple Line, Add)", "Kick (Add)",0),
            new Mechanic(38896, "Punishing Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right',color:'rgb(200,0,200)',", "Kick","Punishing Kick (Single purple Line)", "Kick",0),
            new Mechanic(39534, "Cranial Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right-open',color:'rgb(255,200,0)',", "A.CnKB","Cranial Cascade (3 purple Line Knockback, Add)", "Small Cone KB (Add)",0),
            new Mechanic(39686, "Cranial Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right',color:'rgb(255,200,0)',", "CnKB","Cranial Cascade (3 purple Line Knockback)", "Small Cone KB",0),
            new Mechanic(39845, "Radiant Fury", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'octagon',color:'rgb(255,0,0)',", "BrnCrcl","Radiant Fury (expanding burn circles)", "Expanding Circles",0),
            new Mechanic(38926, "Radiant Fury", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'octagon',color:'rgb(255,0,0)',", "BrnCrcl","Radiant Fury (expanding burn circles)", "Expanding Circles",0),
            new Mechanic(39257, "Focused Anger", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-down',color:'rgb(255,100,0)',", "LCnKB","Focused Anger (Large Cone Overhead Crosshair Knockback)", "Large Cone Knockback",0),
            new Mechanic(39031, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(255,140,0)',", "HS","Horizon Strike (turning pizza slices)", "Horizon Strike",0), // 
            new Mechanic(39507, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(255,140,0)',", "HS","Horizon Strike (turning pizza slices)", "Horizon Strike",0), // 
            new Mechanic(39846, "Crimson Dawn", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(50,0,0)',", "HS.end","Crimson Dawn (almost Full platform attack after Horizon Strike)", "Horizon Strike (last)",0),
            new Mechanic(39228, "Solar Cyclone", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'asterisk-open',color:'rgb(140,0,140)',", "Cycln","Solar Cyclone (Circling Knockback)", "KB Cyclone",0),
            new Mechanic(39228, "Solar Cyclone", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'asterisk-open',color:'rgb(140,0,140)',", "Cycln","Solar Cyclone (Circling Knockback)", "KB Cyclone",0),
            new Mechanic(791, "Fear", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Skorvald, "symbol:'square-open',color:'rgb(255,0,0)',", "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)",0,(condition => condition.CombatItem.Value == 3000)), //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new Mechanic(39131, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Skorvald, "symbol:'star-open',color:'rgb(255,0,255)',", "Blm.Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new Mechanic(39491, "Explode", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(255,200,0)',", "Blm.Exp","Hit by Solar Bloom Explosion", "Bloom Explosion",0), //shockwave, not damage? (damage is 50% max HP, not tracked)
            new Mechanic(39911, "Spiral Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle-open',color:'rgb(0,200,0)',", "SprlStr","Hit after Warp (Jump to Player with overhead bomb)", "Spiral Strike",0),
            new Mechanic(39133, "Wave of Mutilation", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-sw',color:'rgb(0,200,0)',", "KBJmp","Hit by KB Jump (player targeted)", "Knockback jump",0),
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

        public override void SetSuccess(CombatData combatData, LogData logData, FightData fightData, List<Player> pList)
        {
            // check reward
            CombatItem reward = combatData.LastOrDefault(x => x.IsStateChange == ParseEnum.StateChange.Reward);
            CombatItem lastDamageTaken = combatData.GetDamageTakenData(fightData.InstID).LastOrDefault(x => x.Value > 0);
            if (lastDamageTaken != null)
            {
                if (reward != null && lastDamageTaken.Time - reward.Time < 100)
                {
                    logData.Success = true;
                    fightData.FightEnd = Math.Min(lastDamageTaken.Time, reward.Time);
                }
                else
                {
                    SetSuccessByDeath(combatData,logData,fightData,pList);
                    if (logData.Success)
                    {
                        fightData.FightEnd = Math.Min(fightData.FightEnd, lastDamageTaken.Time);
                    }
                }
            }

        }
    }
}
