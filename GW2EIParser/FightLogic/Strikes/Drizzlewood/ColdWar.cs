using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Logic
{
    public class ColdWar : StrikeMissionLogic
    {
        public ColdWar(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            }
            );
            Extension = "coldwar";
            Icon = "https://i.imgur.com/r9b2oww.png";
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC varinia = Targets.Find(x => x.ID == (int)ParseEnum.TargetIDS.VariniaStormsounder);
            if (varinia == null)
            {
                throw new InvalidOperationException("Varinia Stormsounder not found");
            }
            phases[0].Targets.Add(varinia);
            //
            // TODO - add phases if applicable
            //
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Targets.Add(varinia);
            }
            return phases;
        }

        // TODO - complete IDs
        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
            };
        }
    }
}
