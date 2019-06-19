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
            new HitOnPlayerMechanic(37477, "Vile Spit", new MechanicPlotlySetting("circle","rgb(70,150,0)"), "Spit","Vile Spit (green goo)", "Poison Spit",0),
            new HitOnPlayerMechanic(37488, "Tail Lash", new MechanicPlotlySetting("triangle-left","rgb(255,200,0)"), "Tail","Tail Lash (half circle Knockback)", "Tail Lash",0),
            new SpawnMechanic(16911, "Nightmare Hallucination", new MechanicPlotlySetting("star-open","rgb(0,0,0)"), "Hallu","Nightmare Hallucination Spawn", "Hallucination",0),
            new HitOnPlayerMechanic(37303, "Cascade of Torment", new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0),
            new HitOnPlayerMechanic(36984, "Cascade of Torment", new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0),
            new EnemyCastMechanic(37320, "Caustic Explosion", new MechanicPlotlySetting("diamond-tall","rgb(255,200,0)"), "Phase","Phase Start", "Phase", 0, false),
            new EnemyCastMechanic(37320, "Caustic Explosion", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "Phase Fail","Phase Fail (Failed to kill Echos in time)", "Phase Fail", 0, true, new List<CastMechanic.CastChecker>{ (ce,log) => ce.ActualDuration >= 20649 }, Mechanic.TriggerRule.AND), //
            new EnemyCastMechanic(36929, "Caustic Explosion", new MechanicPlotlySetting("diamond-wide","rgb(0,160,150)"), "CC","Breakbar Start", "Breakbar", 0, false),
            new EnemyCastMechanic(36929, "Caustic Explosion", new MechanicPlotlySetting("diamond-wide","rgb(255,0,0)"), "CC Fail","Failed to CC in time", "CC Fail", 0, true, new List<CastMechanic.CastChecker>{ (ce,log) => ce.ActualDuration >= 15232 }, Mechanic.TriggerRule.AND),
            new PlayerBoonApplyMechanic(36998, "Fixated", new MechanicPlotlySetting("star-open","rgb(200,0,200)"), "Fixate", "Fixated by Volatile Hallucination", "Fixated",0),
            });
            Extension = "siax";
            IconUrl = "https://wiki.guildwars2.com/images/d/dc/Siax_the_Corrupted.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/UzaQHW9.png",
                            (476, 548),
                            (663, -4127, 3515, -997),
                            (-6144, -6144, 9216, 9216),
                            (11804, 4414, 12444, 5054));
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                Hallucination
            };
        }
    }
}
