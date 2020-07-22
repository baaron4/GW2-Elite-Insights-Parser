using System;
using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.Parser.ParseEnum.TrashID;

namespace GW2EIParser.Logic
{
    internal class Freezie : RaidLogic
    {
        public Freezie(int triggerID) : base(triggerID)
        {
            Extension = "freezie";
            Icon = "https://wiki.guildwars2.com/images/thumb/8/8b/Freezie.jpg/189px-Freezie.jpg";
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.Find(x => x.ID == (int)ParseEnum.TargetID.Freezie);
            NPC heartTarget = Targets.Find(x => x.ID == (int)FreeziesFrozenHeart);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Freezie not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 895, mainTarget, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i%2 == 1)
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.Targets.Add(mainTarget);
                }
                else
                {
                    phase.Name = "Heal " + (i) / 2;
                    phase.Targets.Add(heartTarget);
                }
            }
            return phases;
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                (int)ParseEnum.TargetID.Freezie,
                (int)FreeziesFrozenHeart
            };
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ParseEnum.TargetID.Freezie,
                (int)FreeziesFrozenHeart
            };
        }
    }
}
