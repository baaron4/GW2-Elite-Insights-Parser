using System;
using System.Collections.Generic;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Instance : FightLogic
    {
        public bool StartedLate { get; private set; }
        public bool EndedBeforeExpectedEnd { get; private set; }
        public Instance(int id) : base(id)
        {
            Extension = "instance";
            Targetless = true;
            Icon = "https://wiki.guildwars2.com/images/d/d2/Guild_emblem_004.png";
            EncounterCategoryInformation.Category = FightCategory.UnknownEncounter;
            EncounterCategoryInformation.SubCategory = SubFightCategory.UnknownEncounter;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            AgentItem dummyAgent = agentData.AddCustomNPCAgent(0, fightData.FightEnd, "Dummy Instance Target", ParserHelper.Spec.NPC, (int)ArcDPSEnums.TargetID.Instance, true);
            ComputeFightTargets(agentData, combatData, extensions);
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            fightData.SetSuccess(true, fightData.FightEnd);
            InstanceStartEvent evt = combatData.GetInstanceStartEvent();
            if (evt == null)
            {
                StartedLate = true;
            }
            else
            {
                StartedLate = Math.Abs(evt.OffsetFromInstanceCreation - fightData.LogOffset) < 20000;
            }
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>();
        }
    }
}
