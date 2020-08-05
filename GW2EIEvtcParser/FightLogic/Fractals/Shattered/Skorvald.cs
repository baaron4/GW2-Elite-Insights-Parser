using System;
using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIUtils;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Skorvald : ShatteredFractal
    {
        public Skorvald(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(39916, "Combustion Rush", new MechanicPlotlySetting("triangle-left","rgb(255,0,255)"), "Charge","Combustion Rush", "Charge",0),
            new HitOnPlayerMechanic(39615, "Combustion Rush", new MechanicPlotlySetting("triangle-left","rgb(255,0,255)"), "Charge","Combustion Rush", "Charge",0),
            new HitOnPlayerMechanic(39581, "Combustion Rush", new MechanicPlotlySetting("triangle-left","rgb(255,0,255)"), "Charge","Combustion Rush", "Charge ",0),
            new HitOnPlayerMechanic(39910, "Punishing Kick", new MechanicPlotlySetting("triangle-right-open","rgb(200,0,200)"), "Add Kick","Punishing Kick (Single purple Line, Add)", "Kick (Add)",0),
            new HitOnPlayerMechanic(38896, "Punishing Kick", new MechanicPlotlySetting("triangle-right","rgb(200,0,200)"), "Kick","Punishing Kick (Single purple Line)", "Kick",0),
            new HitOnPlayerMechanic(39534, "Cranial Cascade", new MechanicPlotlySetting("triangle-right-open","rgb(255,200,0)"), "Add Cone KB","Cranial Cascade (3 purple Line Knockback, Add)", "Small Cone KB (Add)",0),
            new HitOnPlayerMechanic(39686, "Cranial Cascade", new MechanicPlotlySetting("triangle-right","rgb(255,200,0)"), "Cone KB","Cranial Cascade (3 purple Line Knockback)", "Small Cone KB",0),
            new HitOnPlayerMechanic(39845, "Radiant Fury", new MechanicPlotlySetting("octagon","rgb(255,0,0)"), "Burn Circle","Radiant Fury (expanding burn circles)", "Expanding Circles",0),
            new HitOnPlayerMechanic(38926, "Radiant Fury", new MechanicPlotlySetting("octagon","rgb(255,0,0)"), "Burn Circle","Radiant Fury (expanding burn circles)", "Expanding Circles",0),
            new HitOnPlayerMechanic(39257, "Focused Anger", new MechanicPlotlySetting("triangle-down","rgb(255,100,0)"), "Large Cone KB","Focused Anger (Large Cone Overhead Crosshair Knockback)", "Large Cone Knockback",0),
            new HitOnPlayerMechanic(39031, "Horizon Strike", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Horizon Strike","Horizon Strike (turning pizza slices)", "Horizon Strike",0), // 
            new HitOnPlayerMechanic(39507, "Horizon Strike", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Horizon Strike","Horizon Strike (turning pizza slices)", "Horizon Strike",0), // 
            new HitOnPlayerMechanic(39846, "Crimson Dawn", new MechanicPlotlySetting("circle","rgb(50,0,0)"), "Horizon Strike End","Crimson Dawn (almost Full platform attack after Horizon Strike)", "Horizon Strike (last)",0),
            new HitOnPlayerMechanic(39228, "Solar Cyclone", new MechanicPlotlySetting("asterisk-open","rgb(140,0,140)"), "Cyclone","Solar Cyclone (Circling Knockback)", "KB Cyclone",0),
            new HitOnPlayerMechanic(39228, "Solar Cyclone", new MechanicPlotlySetting("asterisk-open","rgb(140,0,140)"), "Cyclone","Solar Cyclone (Circling Knockback)", "KB Cyclone",0),
            new PlayerBuffApplyMechanic(791, "Fear", new MechanicPlotlySetting("square-open","rgb(255,0,0)"), "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)",0, (ba, log) => ba.AppliedDuration == 3000), //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new PlayerBuffApplyMechanic(39131, "Fixate", new MechanicPlotlySetting("star-open","rgb(255,0,255)"), "Bloom Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new HitOnPlayerMechanic(39491, "Explode", new MechanicPlotlySetting("circle","rgb(255,200,0)"), "Bloom Expl","Hit by Solar Bloom Explosion", "Bloom Explosion",0), //shockwave, not damage? (damage is 50% max HP, not tracked)
            new HitOnPlayerMechanic(39911, "Spiral Strike", new MechanicPlotlySetting("circle-open","rgb(0,200,0)"), "Spiral","Hit after Warp (Jump to Player with overhead bomb)", "Spiral Strike",0),
            new HitOnPlayerMechanic(39133, "Wave of Mutilation", new MechanicPlotlySetting("triangle-sw","rgb(0,200,0)"), "KB Jump","Hit by KB Jump (player targeted)", "Knockback jump",0),
            });
            Extension = "skorv";
            Icon = "https://wiki.guildwars2.com/images/c/c1/Skorvald_the_Shattered.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/PO3aoJD.png",
                            (1759, 1783),
                            (-22267, 14955, -17227, 20735),
                            (-24576, -24576, 24576, 24576),
                            (11204, 4414, 13252, 6462));
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            NPC target = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.Skorvald);
            if (target == null)
            {
                throw new InvalidOperationException("Skorvald not found");
            }
            return (target.GetHealth(combatData) == 5551340) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.FluxAnomaly4,
                ArcDPSEnums.TrashID.FluxAnomaly3,
                ArcDPSEnums.TrashID.FluxAnomaly2,
                ArcDPSEnums.TrashID.FluxAnomaly1,
                ArcDPSEnums.TrashID.SolarBloom
            };
        }
    }
}
