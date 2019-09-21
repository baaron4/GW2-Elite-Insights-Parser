using System;
using System.Collections.Generic;
using LuckParser.EIData;
using LuckParser.Parser;

namespace LuckParser.Logic
{
    public class IcebroodConstruct : StrikeMissionLogic
    {
        public IcebroodConstruct(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            }
            );
            Extension = "icebrood";
            Icon = "https://wiki.guildwars2.com/images/thumb/e/e2/Icebrood_Construct.jpg/320px-Icebrood_Construct.jpg";
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.IcebroodConstruct);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Invul check
            phases.AddRange(GetPhasesByInvul(log, 757, mainTarget, false, true));
            string[] phaseNames = new[] { "Phase 1", "Phase 2"};
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = phaseNames[i - 1];
                phase.Targets.Add(mainTarget);
            }
            return phases;
        }
    }
}
