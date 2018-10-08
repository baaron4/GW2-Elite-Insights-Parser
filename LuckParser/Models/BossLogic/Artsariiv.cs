using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models
{
    public class Artsariiv : FractalLogic
    {
        public Artsariiv(ushort triggerID) : base(triggerID)
        {           
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(38880, "Corporeal Reassignment", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Artsariiv, "symbol:'diamond-tall',color:'rgb(255,0,0)'", "Skull","Exploding Skull mechanic application","Corporeal Reassignment",0),
            new Mechanic(38977, "Vault", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'triangle-down-open',color:'rgb(255,200,0)'", "Vlt","Vault from Big Adds", "Vault (Add)",0),
            new Mechanic(39925, "Slam", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,140,0)'", "Slam","Slam (Vault) from Boss", "Vault (Arts)",0),
            new Mechanic(39469, "Teleport Lunge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'star-triangle-down-open',color:'rgb(255,140,0)'", "Jmp","Triple Jump Mid->Edge", "Triple Jump",0),
            new Mechanic(39035, "Astral Surge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle-open',color:'rgb(255,200,0)'", "Flr","Different sized spiraling circles", "1000 Circles",0),
            new Mechanic(39029, "Red Marble", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)'", "Marble","Red KD Marble after Jump", "Red Marble",0), 
            new Mechanic(39863, "Red Marble", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)'", "Marble","Red KD Marble after Jump", "Red Marble",0), 
            new Mechanic(791, "Fear", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Artsariiv, "symbol:'square-open',color:'rgb(255,0,0)'", "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)" ,0,(condition => condition.CombatItem.Value == 3000)), //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new Mechanic(17630, "Spark", Mechanic.MechType.Spawn, ParseEnum.BossIDS.Artsariiv, "symbol: 'star', color: 'rgb(0,255,255)'","Sprk","Spawned a Spark (missed marble)", "Spark",0),
            });
            Extension = "arts";
            IconUrl = "https://wiki.guildwars2.com/images/b/b4/Artsariiv.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/4wmuc8B.png",
                            Tuple.Create(914, 914),
                            Tuple.Create(8991, 112, 11731, 2812),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
        }

        protected override void RegroupTargets(AgentData agentData, List<CombatItem> combatItems)
        {
            RegroupTargetsByID((ushort)ParseEnum.BossIDS.Artsariiv, agentData, combatItems);
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                TemporalAnomaly,
                Spark,
                Artsariiv1,
                Artsariiv2,
                Artsariiv3
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)TemporalAnomaly:
                case (ushort)Spark:
                case (ushort)Artsariiv1:
                case (ushort)Artsariiv2:
                case (ushort)Artsariiv3:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalBossData(Boss boss, ParsedLog log)
        {
            switch (boss.ID)
            {
                case (ushort)ParseEnum.BossIDS.Artsariiv:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void SetSuccess(ParsedLog log)
        {
            SetSuccessOnCombatExit(log, 3, 2000);
        }
    }
}
