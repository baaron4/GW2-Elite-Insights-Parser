using System;
using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.Parser.ParseEnum.TrashIDS;

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
            NPC mainTarget = Targets.Find(x => x.ID == (int)ParseEnum.TargetIDS.Freezie);
            NPC heartTarget = Targets.Find(x => x.ID == (int)FreeziesFrozenHeart);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Error Encountered: Freezie not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 895, mainTarget, true, true));
            string[] namesFreezie = new[] { "Phase 1", "Heal 1", "Phase 2", "Heal 2", "Phase 3", "Heal 3" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = namesFreezie[i - 1];
                if (i == 1 || i == 3 || i == 5)
                {
                    phase.Targets.Add(mainTarget);
                }
                else
                {
                    phase.Targets.Add(heartTarget);
                }
            }
            return phases;
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                (int)ParseEnum.TargetIDS.Freezie,
                (int)FreeziesFrozenHeart
            };
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ParseEnum.TargetIDS.Freezie,
                (int)FreeziesFrozenHeart
            };
        }
    }
}
