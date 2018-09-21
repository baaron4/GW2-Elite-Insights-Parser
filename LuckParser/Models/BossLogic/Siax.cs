using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;

namespace LuckParser.Models
{
    public class Siax : FractalLogic
    {
        public Siax(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(37477, "Vile Spit", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Siax, "symbol:'circle',color:'rgb(70,150,0)',", "Spit","Vile Spit (green goo)", "Poison Spit",0),
            new Mechanic(37488, "Tail Lash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Siax, "symbol:'triangle-left',color:'rgb(255,200,0)',", "Tail","Tail Lash (half circle Knockback)", "Tail Lash",0),
            new Mechanic(16911, "Nightmare Hallucination", Mechanic.MechType.Spawn, ParseEnum.BossIDS.Siax, "symbol:'star-open',color:'rgb(0,0,0)',", "NgtmHlc","Nightmare Hallucination Spawn", "Hallucination",0),
            new Mechanic(37303, "Cascade of Torment", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Siax, "symbol:'circle-open',color:'rgb(255,140,0)',", "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0),
            new Mechanic(36984, "Cascade of Torment", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Siax, "symbol:'circle-open',color:'rgb(255,140,0)',", "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0),
            new Mechanic(37320, "Caustic Explosion", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Siax, "symbol:'diamond-tall',color:'rgb(255,200,0)',", "Phase","Phase Start", "Phase", 0),
            new Mechanic(37320, "Caustic Explosion", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Siax, "symbol:'diamond-tall',color:'rgb(255,0,0)',", "Ph.Fail","Phase Fail (Failed to kill Echos in time)", "Phase Fail", 0, (condition=> condition.CombatItem.Value >=20649)), //
            new Mechanic(36929, "Caustic Explosion", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Siax, "symbol:'diamond-wide',color:'rgb(0,160,150)',", "CC","Breakbar Start", "Breakbar", 0),
            new Mechanic(36929, "Caustic Explosion", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Siax, "symbol:'diamond-wide',color:'rgb(255,0,0)',", "CC.Fail","Failed to CC in time", "CC Fail", 0, (condition => condition.CombatItem.Value >=15232)), 
            new Mechanic(36998, "Fixated", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Siax, "symbol:'star-open',color:'rgb(200,0,200)',", "Fix", "Fixated by Volatile Hallucination", "Fixated",0),
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

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                ParseEnum.TrashIDS.Hallucination
            };
        }

        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/5C60cQb.png";
        }
    }
}
