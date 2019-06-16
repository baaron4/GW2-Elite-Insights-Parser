using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;
using System.Drawing;

namespace LuckParser.Models.Logic
{
    public class PeerlessQadim : RaidLogic
    {
        public PeerlessQadim(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
            });
            Extension = "prlqadim";
            IconUrl = "https://wiki.guildwars2.com/images/8/8b/Mini_Qadim_the_Peerless.png";
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>()
            {
                Pylon1,
                Pylon2,
                EntropicDistortion
            };
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.PeerlessQadim);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            List<long> phaseStarts = new List<long>();
            List<long> phaseEnds = new List<long>();
            //
            List<AbstractBuffEvent> magmaDrops = log.CombatData.GetBoonData(56475).Where(x => x is BuffApplyEvent).ToList();
            foreach (AbstractBuffEvent magmaDrop in magmaDrops)
            {
                if (phaseEnds.Count > 0)
                {
                    if (Math.Abs(phaseEnds.Last() - magmaDrop.Time) > 1000)
                    {
                        phaseEnds.Add(magmaDrop.Time);
                    }
                }
                else
                {
                    phaseEnds.Add(magmaDrop.Time);
                }
            }
            List<AbstractCastEvent> pushes = log.CombatData.GetCastDataById(56405);
            if (pushes.Count > 0)
            {
                AbstractCastEvent push = pushes[0];
                phaseStarts.Add(push.Time + push.ActualDuration);
                foreach (long magmaDrop in phaseEnds)
                {
                    push = pushes.FirstOrDefault(x => x.Time >= magmaDrop);
                    if (push == null)
                    {
                        break;
                    }
                    phaseStarts.Add(push.Time + push.ActualDuration);
                }
            }
            // rush to pylon
            phaseEnds.AddRange(log.CombatData.GetCastDataById(56616).Select(x => x.Time).ToList());
            phaseEnds.Add(log.FightData.FightDuration);
            // tp to middle after pylon destruction
            phaseStarts.AddRange(log.CombatData.GetCastDataById(56375).Select(x => x.Time + x.ActualDuration));
            if (phaseEnds.Count < phaseStarts.Count)
            {
                return phases;
            }
            for (int i = 0; i < phaseStarts.Count; i++)
            {
                PhaseData phase = new PhaseData(phaseStarts[i], phaseEnds[i])
                {
                    Name = "Phase " + (i + 1)
                };
                phase.Targets.Add(mainTarget);
                phases.Add(phase);
            }
            return phases;
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("",
                            (800, 800),
                            (-21504, -21504, 24576, 24576),
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970));
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.PeerlessQadim);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return (target.GetHealth(combatData) > 48e6) ? 1 : 0;
        }
    }
}
