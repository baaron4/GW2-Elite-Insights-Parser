using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models
{
    public class ConjuredAmalgamate : RaidLogic
    {
        public ConjuredAmalgamate(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(52173, "Pulverize", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ConjuredAmalgamate, "symbol:'square',color:'rgb(255,140,0)'", "Plvrz","Pulverize", "Pulverize",0),
                
            });
            Extension = "ca";
            IconUrl = "";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/9PJB5Ky.png",
                            Tuple.Create(1414, 2601),
                            Tuple.Create(-5064, -15030, -2864, -10830),
                            Tuple.Create(-21504, -21504, 24576, 24576),
                            Tuple.Create(13440, 14336, 15360, 16256));
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.BossIDS.ConjuredAmalgamate,
                (ushort)ParseEnum.BossIDS.CARightArm,
                (ushort)ParseEnum.BossIDS.CALeftArm
            };
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>()
            {
                ConjuredGreatsword,
                ConjuredShield
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)ConjuredGreatsword:
                case (ushort)ConjuredShield:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long start = 0;
            long end = 0;
            List<PhaseData> phases = GetInitialPhase(log);
            Boss ca = Targets.Find(x => x.ID == (ushort)ParseEnum.BossIDS.ConjuredAmalgamate);
            if (ca == null)
            {
                throw new InvalidOperationException("Conjurate Amalgamate not found");
            }
            phases[0].Targets.Add(ca);
            if (!requirePhases)
            {
                return phases;
            }
            return phases;
        }

        public override void ComputeAdditionalBossData(Boss boss, ParsedLog log)
        {
            switch (boss.ID)
            {
                case (ushort)ParseEnum.BossIDS.ConjuredAmalgamate:
                case (ushort)ParseEnum.BossIDS.CALeftArm:
                case (ushort)ParseEnum.BossIDS.CARightArm:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {

        }

        public override int IsCM(ParsedLog log)
        {
            Boss target = Targets.Find(x => x.ID == (ushort)ParseEnum.BossIDS.ConjuredAmalgamate);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return log.CombatData.GetBoonData(53075).Count > 0 ? 1 : 0;
        }
    }
}
