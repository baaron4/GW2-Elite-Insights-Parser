﻿using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class Ensolyss : FractalLogic
    {
        public Ensolyss(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(37154, "Lunge", new MechanicPlotlySetting("triangle-right-open","rgb(255,140,0)"), "Charge","Lunge (KB charge over arena)", "Charge",150), 
            new HitOnPlayerMechanic(37278, "Upswing", new MechanicPlotlySetting("circle","rgb(255,100,0)"), "Smash 1","High damage Jump hit", "First Smash",0),
            new HitOnPlayerMechanic(36927, "Lunge", new MechanicPlotlySetting("triangle-right-open","rgb(255,140,0)"), "Charg","Lunge (KB charge over arena)", "Charge",150), 
            new HitOnPlayerMechanic(36962, "Upswing", new MechanicPlotlySetting("circle","rgb(255,200,0)"), "Smash 2","Torment Explosion from Illusions after jump", "Torment Smash",50),
            new HitOnPlayerMechanic(37466, "Nightmare Miasma", new MechanicPlotlySetting("circle-open","rgb(255,0,255)"), "Goo","Nightmare Miasma (Goo)", "Miasma",0),
            new HitOnPlayerMechanic(36944, "Nightmare Miasma", new MechanicPlotlySetting("circle-open","rgb(255,0,255)"), "Goo","Nightmare Miasma (Goo)", "Miasma",0),
            new EnemyCastMechanic(37096, "Caustic Explosion", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "CC","After Phase CC", "Breakbar", 0, false),
            new EnemyCastMechanic(37096, "Caustic Explosion", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC Fail","After Phase CC Failed", "CC Fail", 0, true, new List<CastMechanic.CastChecker>{ (ce,log) => ce.ActualDuration >= 15260 }, Mechanic.TriggerRule.AND),
            new EnemyCastMechanic(37096, "Caustic Explosion", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","After Phase CC Success", "CCed", 0, true, new List<CastMechanic.CastChecker>{ (ce, log) => ce.ActualDuration < 15260 }, Mechanic.TriggerRule.AND),
            new HitOnPlayerMechanic(37096, "Caustic Explosion", new MechanicPlotlySetting("bowtie","rgb(255,200,0)"), "CC KB","Knockback hourglass during CC", "CC KB", 0),
            new EnemyCastMechanic(37523, "Nightmare Devastation", new MechanicPlotlySetting("square-open","rgb(0,0,255)"), "Bubble","Nightmare Devastation (bubble attack)", "Bubble",0, false), 
            new EnemyCastMechanic(37050, "Nightmare Devastation", new MechanicPlotlySetting("square-open","rgb(0,0,255)"), "Bubble","Nightmare Devastation (bubble attack)", "Bubble",0, false),
            new HitOnPlayerMechanic(37151, "Nightmare Miasma", new MechanicPlotlySetting("circle-open","rgb(0,0,255)"), "Goo","Nightmare Miasma (Goo)", "Miasma",0), 
            new HitOnPlayerMechanic(37003, "Tail Lash", new MechanicPlotlySetting("triangle-left","rgb(255,200,0)"), "Tail","Tail Lash (half circle Knockback)", "Tail Lash",0), 
            new HitOnPlayerMechanic(37434, "Rampage", new MechanicPlotlySetting("asterisk-open","rgb(255,0,0)"), "Rampage","Rampage (asterisk shaped Arrow attack)", "Rampage",150),
            new HitOnPlayerMechanic(37045, "Caustic Grasp", new MechanicPlotlySetting("star-diamond","rgb(255,140,0)"), "Pull","Caustic Grasp (Arena Wide Pull)", "Pull",0), 
            new HitOnPlayerMechanic(36926, "Tormenting Blast", new MechanicPlotlySetting("diamond","rgb(255,255,0)"), "Quarter","Tormenting Blast (Two Quarter Circle attacks)", "Quarter circle",0), 
            });
            Extension = "ensol";
            IconUrl = "https://wiki.guildwars2.com/images/5/57/Champion_Toxic_Hybrid.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/kjelZ4t.png",
                            (366, 366),
                            (252, 1, 2892, 2881),
                            (-6144, -6144, 9216, 9216),
                            (11804, 4414, 12444, 5054));
        }
        

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            // Find target
            AgentItem target = agentData.GetAgentsByID((ushort)ParseEnum.TargetIDS.Ensolyss).FirstOrDefault();
            if (target == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            // enter combat
            CombatItem invulLost = combatData.FirstOrDefault(x => x.DstInstid == target.InstID && x.IsStateChange == ParseEnum.StateChange.None && x.IsBuffRemove != ParseEnum.BuffRemove.None && x.SkillID == 762);
            if (invulLost != null && invulLost.LogTime - fightData.FightStartLogTime < 5000)
            {
                fightData.OverrideStart(invulLost.LogTime);
            }
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                NightmareHallucination1,
                NightmareHallucination2
            };
        }
    }
}
