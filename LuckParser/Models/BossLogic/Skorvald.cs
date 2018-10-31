using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models
{
    public class Skorvald : FractalLogic
    {
        public Skorvald(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(39916, "Combustion Rush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-left',color:'rgb(255,0,255)'", "Chrg","Combustion Rush", "Charge",0),
            new Mechanic(39615, "Combustion Rush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-left',color:'rgb(255,0,255)'", "Chrg","Combustion Rush", "Charge",0),
            new Mechanic(39581, "Combustion Rush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-left',color:'rgb(255,0,255)'", "Chrg","Combustion Rush", "Charge ",0),
            new Mechanic(39910, "Punishing Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right-open',color:'rgb(200,0,200)'", "A.Kick","Punishing Kick (Single purple Line, Add)", "Kick (Add)",0),
            new Mechanic(38896, "Punishing Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right',color:'rgb(200,0,200)'", "Kick","Punishing Kick (Single purple Line)", "Kick",0),
            new Mechanic(39534, "Cranial Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right-open',color:'rgb(255,200,0)'", "A.CnKB","Cranial Cascade (3 purple Line Knockback, Add)", "Small Cone KB (Add)",0),
            new Mechanic(39686, "Cranial Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-right',color:'rgb(255,200,0)'", "CnKB","Cranial Cascade (3 purple Line Knockback)", "Small Cone KB",0),
            new Mechanic(39845, "Radiant Fury", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'octagon',color:'rgb(255,0,0)'", "BrnCrcl","Radiant Fury (expanding burn circles)", "Expanding Circles",0),
            new Mechanic(38926, "Radiant Fury", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'octagon',color:'rgb(255,0,0)'", "BrnCrcl","Radiant Fury (expanding burn circles)", "Expanding Circles",0),
            new Mechanic(39257, "Focused Anger", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-down',color:'rgb(255,100,0)'", "LCnKB","Focused Anger (Large Cone Overhead Crosshair Knockback)", "Large Cone Knockback",0),
            new Mechanic(39031, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(255,140,0)'", "HS","Horizon Strike (turning pizza slices)", "Horizon Strike",0), // 
            new Mechanic(39507, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(255,140,0)'", "HS","Horizon Strike (turning pizza slices)", "Horizon Strike",0), // 
            new Mechanic(39846, "Crimson Dawn", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(50,0,0)'", "HS.end","Crimson Dawn (almost Full platform attack after Horizon Strike)", "Horizon Strike (last)",0),
            new Mechanic(39228, "Solar Cyclone", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'asterisk-open',color:'rgb(140,0,140)'", "Cycln","Solar Cyclone (Circling Knockback)", "KB Cyclone",0),
            new Mechanic(39228, "Solar Cyclone", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'asterisk-open',color:'rgb(140,0,140)'", "Cycln","Solar Cyclone (Circling Knockback)", "KB Cyclone",0),
            new Mechanic(791, "Fear", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Skorvald, "symbol:'square-open',color:'rgb(255,0,0)'", "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)",0,(condition => condition.CombatItem.Value == 3000)), //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new Mechanic(39131, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Skorvald, "symbol:'star-open',color:'rgb(255,0,255)'", "Blm.Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new Mechanic(39491, "Explode", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle',color:'rgb(255,200,0)'", "Blm.Exp","Hit by Solar Bloom Explosion", "Bloom Explosion",0), //shockwave, not damage? (damage is 50% max HP, not tracked)
            new Mechanic(39911, "Spiral Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'circle-open',color:'rgb(0,200,0)'", "SprlStr","Hit after Warp (Jump to Player with overhead bomb)", "Spiral Strike",0),
            new Mechanic(39133, "Wave of Mutilation", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Skorvald, "symbol:'triangle-sw',color:'rgb(0,200,0)'", "KBJmp","Hit by KB Jump (player targeted)", "Knockback jump",0),
            });
            Extension = "skorv";
            IconUrl = "https://wiki.guildwars2.com/images/c/c1/Skorvald_the_Shattered.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/PO3aoJD.png",
                            Tuple.Create(1759, 1783),
                            Tuple.Create(-22267, 14955, -17227, 20735),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
        }

        public override int IsCM(ParsedLog log)
        {
            Boss target = Targets.Find(x => x.ID == (ushort)ParseEnum.BossIDS.Skorvald);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            OverrideMaxHealths(log);
            return (target.Health == 5551340) ? 1 : 0;
        }

        public override void ComputeAdditionalBossData(Boss boss, ParsedLog log)
        {
            CombatReplay replay = boss.CombatReplay;
            List<CastLog> cls = boss.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (boss.ID)
            {
                case (ushort)ParseEnum.BossIDS.Skorvald:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                FluxAnomaly4,
                FluxAnomaly3,
                FluxAnomaly2,
                FluxAnomaly1,
                SolarBloom
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)FluxAnomaly4:
                case (ushort)FluxAnomaly3:
                case (ushort)FluxAnomaly2:
                case (ushort)FluxAnomaly1:
                case (ushort)SolarBloom:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void SetSuccess(ParsedLog log)
        {
            // check reward
            Boss mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.BossIDS.Skorvald);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            CombatItem reward = log.CombatData.GetStatesData(ParseEnum.StateChange.Reward).LastOrDefault();
            CombatItem lastDamageTaken = log.CombatData.GetDamageTakenData(mainTarget.InstID).LastOrDefault(x => x.Value > 0);
            if (lastDamageTaken != null)
            {
                if (reward != null && lastDamageTaken.Time - reward.Time < 100)
                {
                    log.LogData.Success = true;
                    log.FightData.FightEnd = Math.Min(lastDamageTaken.Time, reward.Time);
                }
                else
                {
                    SetSuccessByDeath(log);
                    if (log.LogData.Success)
                    {
                        log.FightData.FightEnd = Math.Min(log.FightData.FightEnd, lastDamageTaken.Time);
                    }
                }
            }

        }
    }
}
