using LuckParser.Parser;
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
            new Mechanic(37154, "Lunge", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-right-open","rgb(255,140,0)"), "Charge","Lunge (KB charge over arena)", "Charge",150), 
            new Mechanic(37278, "Upswing", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(255,100,0)"), "Smash 1","High damage Jump hit", "First Smash",0),
            new Mechanic(36927, "Lunge", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-right-open","rgb(255,140,0)"), "Charg","Lunge (KB charge over arena)", "Charge",150), 
            new Mechanic(36962, "Upswing", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(255,200,0)"), "Smash 2","Torment Explosion from Illusions after jump", "Torment Smash",50),
            new Mechanic(37466, "Nightmare Miasma", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(255,0,255)"), "Goo","Nightmare Miasma (Goo)", "Miasma",0),
            new Mechanic(36944, "Nightmare Miasma", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(255,0,255)"), "Goo","Nightmare Miasma (Goo)", "Miasma",0),
            new Mechanic(37096, "Caustic Explosion", Mechanic.MechType.EnemyCastStart, new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "CC","After Phase CC", "Breakbar", 0),
            new Mechanic(37096, "Caustic Explosion", Mechanic.MechType.EnemyCastEnd, new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC Fail","After Phase CC Failed", "CC Fail", 0, (condition => condition.CombatItem.Value >=15260)),
            new Mechanic(37096, "Caustic Explosion", Mechanic.MechType.EnemyCastEnd, new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","After Phase CC Success", "CCed", 0, (condition => condition.CombatItem.Value < 15260)),
            new Mechanic(37096, "Caustic Explosion", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("bowtie","rgb(255,200,0)"), "CC KB","Knockback hourglass during CC", "CC KB", 0),
            new Mechanic(37523, "Nightmare Devastation", Mechanic.MechType.EnemyCastStart, new MechanicPlotlySetting("square-open","rgb(0,0,255)"), "Bubble","Nightmare Devastation (bubble attack)", "Bubble",0), 
            new Mechanic(37050, "Nightmare Devastation", Mechanic.MechType.EnemyCastStart, new MechanicPlotlySetting("square-open","rgb(0,0,255)"), "Bubble","Nightmare Devastation (bubble attack)", "Bubble",0),
            new Mechanic(37151, "Nightmare Miasma", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(0,0,255)"), "Goo","Nightmare Miasma (Goo)", "Miasma",0), 
            new Mechanic(37003, "Tail Lash", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-left","rgb(255,200,0)"), "Tail","Tail Lash (half circle Knockback)", "Tail Lash",0), 
            new Mechanic(37434, "Rampage", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("asterisk-open","rgb(255,0,0)"), "Rampage","Rampage (asterisk shaped Arrow attack)", "Rampage",150),
            new Mechanic(37045, "Caustic Grasp", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("star-diamond","rgb(255,140,0)"), "Pull","Caustic Grasp (Arena Wide Pull)", "Pull",0), 
            new Mechanic(36926, "Tormenting Blast", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("diamond","rgb(255,255,0)"), "Quarter","Tormenting Blast (Two Quarter Circle attacks)", "Quarter circle",0), 
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


        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            CombatReplay replay = target.CombatReplay;
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Ensolyss:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            // Find target
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Ensolyss);
            if (target == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            // enter combat
            CombatItem invulLost = combatData.FirstOrDefault(x => x.DstInstid == target.InstID && x.IsStateChange == ParseEnum.StateChange.Normal && x.IsBuffRemove != ParseEnum.BuffRemove.None && x.SkillID == 762);
            if (invulLost != null && invulLost.Time - fightData.FightStart < 5000)
            {
                fightData.FightStart = invulLost.Time;
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

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)NightmareHallucination1:
                case (ushort)NightmareHallucination2:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
        }
    }
}
