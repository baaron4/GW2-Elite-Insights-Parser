using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models
{
    public class Qadim : RaidLogic
    {
        public Qadim(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(52242, "Shattering Impact", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle',color:'rgb(255,200,0)'", "Stun","Shattering Impact (Stunning flame bolt)", "Flame Bolt Stun",0),
            new Mechanic(52814, "Flame Wave", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle-open',color:'rgb(255,0,0)'", "Shwv","Flame Wave (Knockback)", "Shockwave",0),
            new Mechanic(52520, "Elemental Breath", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'triangle-left',color:'rgb(255,0,0)'", "H.Brth","Elemental Breath (Hydra Breath)", "Hydra Breath",0),
            new Mechanic(53013, "Fireball", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle-open',color:'rgb(255,150,0)',size:10,", "H.Fb","Fireball (Hydra)", "Hydra Fireball",0),
            new Mechanic(52941, "Fiery Meteor", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle-open',color:'rgb(255,150,0)'", "H.Mtr","Fiery Meteor (Hydra)", "Hydra Meteor",0),
            new Mechanic(53051, "Teleport", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle',color:'rgb(150,0,200)'", "H.KB","Teleport Knockback (Hydra)", "Hydra TP KB",0),

            });
            Extension = "qadim";
            IconUrl = "https://wiki.guildwars2.com/images/f/f2/Mini_Qadim.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/mlkXYPT.png",
                            Tuple.Create(3277, 2845),
                            Tuple.Create(-10801, 8969, -4035, 15145),
                            Tuple.Create(-21504, -21504, 24576, 24576),
                            Tuple.Create(13440, 14336, 15360, 16256));
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.BossIDS.Qadim,
                (ushort)AncientInvokedHydra,
                (ushort)WyvernMatriarch,
                (ushort)WyvernPatriarch,
                (ushort)ApocalypseBringer,
            };
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long start = 0;
            long end = 0;
            List<PhaseData> phases = GetInitialPhase(log);
            Boss qadim = Targets.Find(x => x.ID == (ushort)ParseEnum.BossIDS.Qadim);
            if (qadim == null)
            {
                throw new InvalidOperationException("Nikare not found");
            }
            phases[0].Targets.Add(qadim);
            if (!requirePhases)
            {
                return phases;
            }
            List<long> moltenArmor = log.GetBoonDataByDst(qadim.InstID).Where(x => x.SkillID == 52329).Select(x => x.Time - log.FightData.FightStart).Distinct().ToList();
            for (int i = 1; i < moltenArmor.Count; i++)
            {
                if (i % 2 == 0)
                {
                    end = Math.Min(moltenArmor[i], log.FightData.FightDuration);
                    phases.Add(new PhaseData(start, end));
                } else
                {
                    start = moltenArmor[i];
                    phases.Add(new PhaseData(end, start));
                    if (i == moltenArmor.Count - 1)
                    {
                        phases.Add(new PhaseData(start, log.FightData.FightDuration));
                    }
                }
            }
            string[] names = { "Hydra","Qadim P1","Apocalypse", "Qadim P2","Wyvern", "Qadim P3" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = names[i - 1];
                switch (i)
                {
                    case 2:
                    case 4:
                    case 6:
                        phase.Targets.Add(qadim);
                        break;
                    default:
                        List<ushort> ids = new List<ushort>
                        {
                           (ushort) WyvernMatriarch,
                           (ushort) WyvernPatriarch,
                           (ushort) AncientInvokedHydra,
                           (ushort) ApocalypseBringer
                        };
                        AddTargetsToPhase(phase, ids, log);
                        phase.DrawArea = true;
                        phase.DrawEnd = true;
                        phase.DrawStart = true;
                        break;
                }
            }
            return phases;
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>()
            {
                LavaElemental1,
                LavaElemental2,
                IcebornHydra,
                GreaterMagmaElemental1,
                GreaterMagmaElemental2,
                FireElemental,
                FireImp,
                PyreGuardian,
                ReaperofFlesh,
                IceElemental,
                Zommoros
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)LavaElemental1:
                case (ushort)LavaElemental2:
                case (ushort)IcebornHydra:
                case (ushort)GreaterMagmaElemental1:
                case (ushort)GreaterMagmaElemental2:
                case (ushort)FireElemental:
                case (ushort)FireImp:
                case (ushort)PyreGuardian:
                case (ushort)ReaperofFlesh:
                case (ushort)IceElemental:
                case (ushort)Zommoros:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalBossData(Boss boss, ParsedLog log)
        {
            switch (boss.ID)
            {
                case (ushort)ParseEnum.BossIDS.Qadim:
                    break;
                case (ushort)AncientInvokedHydra:
                case (ushort)WyvernMatriarch:
                case (ushort)WyvernPatriarch:
                case (ushort)ApocalypseBringer:
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
            return 0; //Check via Hydra HP or (>27e6)
        }
        
    }
}
