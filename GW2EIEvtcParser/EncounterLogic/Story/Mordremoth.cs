using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Mordremoth : StoryInstance
    {
        public Mordremoth(int triggerID) : base(triggerID)
        {
            Extension = "mordr";
            Icon = EncounterIconMordremoth;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000201;
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.SmotheringShadow,
            };
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayMordremoth,
                            (899, 1172),
                            (-9059, 10171, -6183, 13149));
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Mordremoth));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Vale Guardian not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Invul check
            phases.AddRange(GetPhasesByInvul(log, Determined762, mainTarget, false, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = "Phase " + i;
                phase.AddTarget(mainTarget);
            }
            return phases;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Mordremoth,
                (int)ArcDPSEnums.TrashID.BlightedRytlock,
                //ArcDPSEnums.TrashID.BlightedCanach,
                (int)ArcDPSEnums.TrashID.BlightedBraham,
                (int)ArcDPSEnums.TrashID.BlightedMarjory,
                (int)ArcDPSEnums.TrashID.BlightedCaithe,
                (int)ArcDPSEnums.TrashID.BlightedForgal,
                (int)ArcDPSEnums.TrashID.BlightedSieran,
                //ArcDPSEnums.TrashID.BlightedTybalt,
                //ArcDPSEnums.TrashID.BlightedPaleTree,
                //ArcDPSEnums.TrashID.BlightedTrahearne,
                //ArcDPSEnums.TrashID.BlightedEir,
            };
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            AbstractSingleActor mordremoth = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Mordremoth));
            if (mordremoth == null)
            {
                throw new EvtcAgentException("Mordremoth not found");
            }
            BuffApplyEvent buffApply = combatData.GetBuffData(Determined895).OfType<BuffApplyEvent>().LastOrDefault(x => x.To == mordremoth.AgentItem);
            if (buffApply != null)
            {
                fightData.SetSuccess(true, mordremoth.LastAware);
            }
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor mordremoth = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Mordremoth));
            if (mordremoth == null)
            {
                throw new MissingKeyActorsException("Mordremoth not found");
            }
            return (mordremoth.GetHealth(combatData) > 9e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Story;
        }

        protected override List<int> GetFriendlyNPCIDs()
        {
            return new List<int>()
            {
                (int)ArcDPSEnums.TrashID.Canach,
                (int)ArcDPSEnums.TrashID.Braham,
                (int)ArcDPSEnums.TrashID.Caithe,
            };
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>()
            {
                (int)ArcDPSEnums.TargetID.Mordremoth, 
                (int)ArcDPSEnums.TrashID.Canach,
                (int)ArcDPSEnums.TrashID.Braham,
                (int)ArcDPSEnums.TrashID.Caithe,
                (int)ArcDPSEnums.TrashID.BlightedRytlock,
                //ArcDPSEnums.TrashID.BlightedCanach,
                (int)ArcDPSEnums.TrashID.BlightedBraham,
                (int)ArcDPSEnums.TrashID.BlightedMarjory,
                (int)ArcDPSEnums.TrashID.BlightedCaithe,
                (int)ArcDPSEnums.TrashID.BlightedForgal,
                (int)ArcDPSEnums.TrashID.BlightedSieran,
                //ArcDPSEnums.TrashID.BlightedTybalt,
                //ArcDPSEnums.TrashID.BlightedPaleTree,
                //ArcDPSEnums.TrashID.BlightedTrahearne,
                //ArcDPSEnums.TrashID.BlightedEir,           
            };
        }
    }
}
