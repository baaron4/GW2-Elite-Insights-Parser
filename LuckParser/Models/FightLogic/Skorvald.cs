using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class Skorvald : FractalLogic
    {
        public Skorvald(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(39916, "Combustion Rush", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-left","rgb(255,0,255)"), "Charge","Combustion Rush", "Charge",0),
            new Mechanic(39615, "Combustion Rush", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-left","rgb(255,0,255)"), "Charge","Combustion Rush", "Charge",0),
            new Mechanic(39581, "Combustion Rush", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-left","rgb(255,0,255)"), "Charge","Combustion Rush", "Charge ",0),
            new Mechanic(39910, "Punishing Kick", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-right-open","rgb(200,0,200)"), "Add Kick","Punishing Kick (Single purple Line, Add)", "Kick (Add)",0),
            new Mechanic(38896, "Punishing Kick", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-right","rgb(200,0,200)"), "Kick","Punishing Kick (Single purple Line)", "Kick",0),
            new Mechanic(39534, "Cranial Cascade", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-right-open","rgb(255,200,0)"), "Add Cone KB","Cranial Cascade (3 purple Line Knockback, Add)", "Small Cone KB (Add)",0),
            new Mechanic(39686, "Cranial Cascade", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-right","rgb(255,200,0)"), "Cone KB","Cranial Cascade (3 purple Line Knockback)", "Small Cone KB",0),
            new Mechanic(39845, "Radiant Fury", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("octagon","rgb(255,0,0)"), "Burn Circle","Radiant Fury (expanding burn circles)", "Expanding Circles",0),
            new Mechanic(38926, "Radiant Fury", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("octagon","rgb(255,0,0)"), "Burn Circle","Radiant Fury (expanding burn circles)", "Expanding Circles",0),
            new Mechanic(39257, "Focused Anger", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-down","rgb(255,100,0)"), "Large Cone KB","Focused Anger (Large Cone Overhead Crosshair Knockback)", "Large Cone Knockback",0),
            new Mechanic(39031, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Horizon Strike","Horizon Strike (turning pizza slices)", "Horizon Strike",0), // 
            new Mechanic(39507, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Horizon Strike","Horizon Strike (turning pizza slices)", "Horizon Strike",0), // 
            new Mechanic(39846, "Crimson Dawn", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(50,0,0)"), "Horizon Strike End","Crimson Dawn (almost Full platform attack after Horizon Strike)", "Horizon Strike (last)",0),
            new Mechanic(39228, "Solar Cyclone", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("asterisk-open","rgb(140,0,140)"), "Cyclone","Solar Cyclone (Circling Knockback)", "KB Cyclone",0),
            new Mechanic(39228, "Solar Cyclone", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("asterisk-open","rgb(140,0,140)"), "Cyclone","Solar Cyclone (Circling Knockback)", "KB Cyclone",0),
            new Mechanic(791, "Fear", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("square-open","rgb(255,0,0)"), "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)",0,(condition => condition.CombatItem.Value == 3000)), //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new Mechanic(39131, "Fixate", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("star-open","rgb(255,0,255)"), "Bloom Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new Mechanic(39491, "Explode", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(255,200,0)"), "Bloom Expl","Hit by Solar Bloom Explosion", "Bloom Explosion",0), //shockwave, not damage? (damage is 50% max HP, not tracked)
            new Mechanic(39911, "Spiral Strike", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(0,200,0)"), "Spiral","Hit after Warp (Jump to Player with overhead bomb)", "Spiral Strike",0),
            new Mechanic(39133, "Wave of Mutilation", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-sw","rgb(0,200,0)"), "KB Jump","Hit by KB Jump (player targeted)", "Knockback jump",0),
            });
            Extension = "skorv";
            IconUrl = "https://wiki.guildwars2.com/images/c/c1/Skorvald_the_Shattered.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/PO3aoJD.png",
                            (1759, 1783),
                            (-22267, 14955, -17227, 20735),
                            (-24576, -24576, 24576, 24576),
                            (11204, 4414, 13252, 6462));
        }

        public override int IsCM(ParsedLog log)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Skorvald);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            OverrideMaxHealths(log);
            return (target.Health == 5551340) ? 1 : 0;
        }

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            CombatReplay replay = target.CombatReplay;
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Skorvald:
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
    }
}
