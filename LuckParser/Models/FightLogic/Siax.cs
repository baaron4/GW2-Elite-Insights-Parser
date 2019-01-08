using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class Siax : FractalLogic
    {
        public Siax(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(37477, "Vile Spit", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(70,150,0)"), "Spit","Vile Spit (green goo)", "Poison Spit",0),
            new Mechanic(37488, "Tail Lash", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-left","rgb(255,200,0)"), "Tail","Tail Lash (half circle Knockback)", "Tail Lash",0),
            new Mechanic(16911, "Nightmare Hallucination", Mechanic.MechType.Spawn, new MechanicPlotlySetting("star-open","rgb(0,0,0)"), "Hallu","Nightmare Hallucination Spawn", "Hallucination",0),
            new Mechanic(37303, "Cascade of Torment", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0),
            new Mechanic(36984, "Cascade of Torment", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0),
            new Mechanic(37320, "Caustic Explosion", Mechanic.MechType.EnemyCastStart, new MechanicPlotlySetting("diamond-tall","rgb(255,200,0)"), "Phase","Phase Start", "Phase", 0),
            new Mechanic(37320, "Caustic Explosion", Mechanic.MechType.EnemyCastEnd, new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "Phase Fail","Phase Fail (Failed to kill Echos in time)", "Phase Fail", 0, (condition=> condition.CombatItem.Value >=20649)), //
            new Mechanic(36929, "Caustic Explosion", Mechanic.MechType.EnemyCastStart, new MechanicPlotlySetting("diamond-wide","rgb(0,160,150)"), "CC","Breakbar Start", "Breakbar", 0),
            new Mechanic(36929, "Caustic Explosion", Mechanic.MechType.EnemyCastEnd, new MechanicPlotlySetting("diamond-wide","rgb(255,0,0)"), "CC Fail","Failed to CC in time", "CC Fail", 0, (condition => condition.CombatItem.Value >=15232)), 
            new Mechanic(36998, "Fixated", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("star-open","rgb(200,0,200)"), "Fixate", "Fixated by Volatile Hallucination", "Fixated",0),
            });
            Extension = "siax";
            IconUrl = "https://wiki.guildwars2.com/images/d/dc/Siax_the_Corrupted.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/UzaQHW9.png",
                            Tuple.Create(476, 548),
                            Tuple.Create(663, -4127, 3515, -997),
                            Tuple.Create(-6144, -6144, 9216, 9216),
                            Tuple.Create(11804, 4414, 12444, 5054));
        }


        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            CombatReplay replay = target.CombatReplay;
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Siax:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                Hallucination
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)Hallucination:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

    }
}
