using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Mordremoth : FightLogic
    {
        public Mordremoth(int triggerID) : base(triggerID)
        {
            Extension = "mordr";
            Icon = "https://i.imgur.com/4pNive1.png";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterCategoryInformation.Category = EncounterCategory.FightCategory.Story;
            EncounterCategoryInformation.SubCategory = EncounterCategory.SubFightCategory.Story;
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.SmotheringShadow,
            };
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/HHDVDPb.png",
                            (899, 1172),
                            (-9059, 1171, -6183, 13149));
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Mordremoth);
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
            phases.AddRange(GetPhasesByInvul(log, 762, mainTarget, false, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = "Phase " + i;
                phase.AddTarget(mainTarget);
            }
            return phases;
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Mordremoth,
                //(int)ArcDPSEnums.TrashID.Canach, // to be put to friendly
                //(int)ArcDPSEnums.TrashID.Braham,
                //(int)ArcDPSEnums.TrashID.Caithe,
                (int)ArcDPSEnums.TrashID.BlightedRytlock,
                //ArcDPSEnums.TrashID.BlightedCanach,
                //ArcDPSEnums.TrashID.BlightedBraham,
                (int)ArcDPSEnums.TrashID.BlightedMarjory,
                (int)ArcDPSEnums.TrashID.BlightedCaithe,
                (int)ArcDPSEnums.TrashID.BlightedForgal,
                //ArcDPSEnums.TrashID.BlightedSieran,
                //ArcDPSEnums.TrashID.BlightedTybalt,
                //ArcDPSEnums.TrashID.BlightedPaleTree,
                //ArcDPSEnums.TrashID.BlightedTrahearne,
                //ArcDPSEnums.TrashID.BlightedEir,
            };
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            AbstractSingleActor mordremoth = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Mordremoth);
            if (mordremoth == null)
            {
                throw new EvtcAgentException("Mordremoth not found");
            }
            BuffApplyEvent buffApply = combatData.GetBuffData(895).OfType<BuffApplyEvent>().LastOrDefault(x => x.To == mordremoth.AgentItem);
            AbstractDamageEvent finisher = combatData.GetDamageData(21825).LastOrDefault(x => x.To == mordremoth.AgentItem);
            if (buffApply != null && finisher != null)
            {
                fightData.SetSuccess(true, finisher.Time);
            }
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor mordremoth = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Mordremoth);
            if (mordremoth == null)
            {
                throw new MissingKeyActorsException("Mordremoth not found");
            }
            return (mordremoth.GetHealth(combatData) > 9e6) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>()
            {
                (int)ArcDPSEnums.TargetID.Mordremoth, 
                (int)ArcDPSEnums.TrashID.Canach,
                (int)ArcDPSEnums.TrashID.Braham,
                //(int)ArcDPSEnums.TrashID.Caithe,
                (int)ArcDPSEnums.TrashID.BlightedRytlock,
                //ArcDPSEnums.TrashID.BlightedCanach,
                //ArcDPSEnums.TrashID.BlightedBraham,
                (int)ArcDPSEnums.TrashID.BlightedMarjory,
                (int)ArcDPSEnums.TrashID.BlightedCaithe,
                (int)ArcDPSEnums.TrashID.BlightedForgal,
                //ArcDPSEnums.TrashID.BlightedSieran,
                //ArcDPSEnums.TrashID.BlightedTybalt,
                //ArcDPSEnums.TrashID.BlightedPaleTree,
                //ArcDPSEnums.TrashID.BlightedTrahearne,
                //ArcDPSEnums.TrashID.BlightedEir,           
            };
        }
    }
}
