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
        public Skorvald(ushort triggerID, AgentData agentData) : base(triggerID, agentData)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new SkillOnPlayerMechanic(39916, "Combustion Rush", new MechanicPlotlySetting("triangle-left","rgb(255,0,255)"), "Charge","Combustion Rush", "Charge",0),
            new SkillOnPlayerMechanic(39615, "Combustion Rush", new MechanicPlotlySetting("triangle-left","rgb(255,0,255)"), "Charge","Combustion Rush", "Charge",0),
            new SkillOnPlayerMechanic(39581, "Combustion Rush", new MechanicPlotlySetting("triangle-left","rgb(255,0,255)"), "Charge","Combustion Rush", "Charge ",0),
            new SkillOnPlayerMechanic(39910, "Punishing Kick", new MechanicPlotlySetting("triangle-right-open","rgb(200,0,200)"), "Add Kick","Punishing Kick (Single purple Line, Add)", "Kick (Add)",0),
            new SkillOnPlayerMechanic(38896, "Punishing Kick", new MechanicPlotlySetting("triangle-right","rgb(200,0,200)"), "Kick","Punishing Kick (Single purple Line)", "Kick",0),
            new SkillOnPlayerMechanic(39534, "Cranial Cascade", new MechanicPlotlySetting("triangle-right-open","rgb(255,200,0)"), "Add Cone KB","Cranial Cascade (3 purple Line Knockback, Add)", "Small Cone KB (Add)",0),
            new SkillOnPlayerMechanic(39686, "Cranial Cascade", new MechanicPlotlySetting("triangle-right","rgb(255,200,0)"), "Cone KB","Cranial Cascade (3 purple Line Knockback)", "Small Cone KB",0),
            new SkillOnPlayerMechanic(39845, "Radiant Fury", new MechanicPlotlySetting("octagon","rgb(255,0,0)"), "Burn Circle","Radiant Fury (expanding burn circles)", "Expanding Circles",0),
            new SkillOnPlayerMechanic(38926, "Radiant Fury", new MechanicPlotlySetting("octagon","rgb(255,0,0)"), "Burn Circle","Radiant Fury (expanding burn circles)", "Expanding Circles",0),
            new SkillOnPlayerMechanic(39257, "Focused Anger", new MechanicPlotlySetting("triangle-down","rgb(255,100,0)"), "Large Cone KB","Focused Anger (Large Cone Overhead Crosshair Knockback)", "Large Cone Knockback",0),
            new SkillOnPlayerMechanic(39031, "Horizon Strike", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Horizon Strike","Horizon Strike (turning pizza slices)", "Horizon Strike",0), // 
            new SkillOnPlayerMechanic(39507, "Horizon Strike", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Horizon Strike","Horizon Strike (turning pizza slices)", "Horizon Strike",0), // 
            new SkillOnPlayerMechanic(39846, "Crimson Dawn", new MechanicPlotlySetting("circle","rgb(50,0,0)"), "Horizon Strike End","Crimson Dawn (almost Full platform attack after Horizon Strike)", "Horizon Strike (last)",0),
            new SkillOnPlayerMechanic(39228, "Solar Cyclone", new MechanicPlotlySetting("asterisk-open","rgb(140,0,140)"), "Cyclone","Solar Cyclone (Circling Knockback)", "KB Cyclone",0),
            new SkillOnPlayerMechanic(39228, "Solar Cyclone", new MechanicPlotlySetting("asterisk-open","rgb(140,0,140)"), "Cyclone","Solar Cyclone (Circling Knockback)", "KB Cyclone",0),
            new PlayerBoonApplyMechanic(791, "Fear", new MechanicPlotlySetting("square-open","rgb(255,0,0)"), "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)",0, new List<MechanicChecker>{ new CombatItemValueChecker(3000, MechanicChecker.ValueCompare.EQ) }, Mechanic.TriggerRule.AND), //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new PlayerBoonApplyMechanic(39131, "Fixate", new MechanicPlotlySetting("star-open","rgb(255,0,255)"), "Bloom Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new SkillOnPlayerMechanic(39491, "Explode", new MechanicPlotlySetting("circle","rgb(255,200,0)"), "Bloom Expl","Hit by Solar Bloom Explosion", "Bloom Explosion",0), //shockwave, not damage? (damage is 50% max HP, not tracked)
            new SkillOnPlayerMechanic(39911, "Spiral Strike", new MechanicPlotlySetting("circle-open","rgb(0,200,0)"), "Spiral","Hit after Warp (Jump to Player with overhead bomb)", "Spiral Strike",0),
            new SkillOnPlayerMechanic(39133, "Wave of Mutilation", new MechanicPlotlySetting("triangle-sw","rgb(0,200,0)"), "KB Jump","Hit by KB Jump (player targeted)", "Knockback jump",0),
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

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Skorvald);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return (target.GetHealth(combatData) == 5551340) ? 1 : 0;
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
    }
}
