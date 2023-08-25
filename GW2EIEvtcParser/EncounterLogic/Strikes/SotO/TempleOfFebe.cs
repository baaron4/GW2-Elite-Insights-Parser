using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class TempleOfFebe : SecretOfTheObscureStrike
    {
        public TempleOfFebe(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            }
            );
            Icon = EncounterIconTempleOfFebe;
            Extension = "tmplfeb";
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayTempleOfFebe,
                            // TODO
                            (1008, 1008),
                            (0,0,0,0));
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>()
            {
                (int)ArcDPSEnums.TargetID.Cerus,
                (int)ArcDPSEnums.TrashID.EmbodimentOfDespair,
                (int)ArcDPSEnums.TrashID.EmbodimentOfEnvy,
                (int)ArcDPSEnums.TrashID.EmbodimentOfGluttony,
                (int)ArcDPSEnums.TrashID.EmbodimentOfMalice,
                (int)ArcDPSEnums.TrashID.EmbodimentOfRage,
                (int)ArcDPSEnums.TrashID.EmbodimentOfRegret,
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Cerus));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Cerus not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Invul check
            phases.AddRange(GetPhasesByInvul(log, InvulnerabilityCerus, mainTarget, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    phase.Name = "Split " + (i) / 2;
                    var ids = new List<int>
                    {
                        (int)ArcDPSEnums.TrashID.EmbodimentOfDespair,
                        (int)ArcDPSEnums.TrashID.EmbodimentOfEnvy,
                        (int)ArcDPSEnums.TrashID.EmbodimentOfGluttony,
                        (int)ArcDPSEnums.TrashID.EmbodimentOfMalice,
                        (int)ArcDPSEnums.TrashID.EmbodimentOfRage,
                        (int)ArcDPSEnums.TrashID.EmbodimentOfRegret,
                    };
                    AddTargetsToPhaseAndFit(phase, ids, log);
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(mainTarget);
                }
            }
            return phases;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, extensions);
            int curDespair = 1;
            int curEnvy = 1;
            int curGluttony = 1;
            int curMalice = 1;
            int curRage = 1;
            int curRegret = 1;
            foreach (AbstractSingleActor target in Targets)
            {
                if (target.IsSpecies(ArcDPSEnums.TrashID.EmbodimentOfDespair))
                {
                    target.OverrideName(target.Character + " " + curDespair++);
                }
                if (target.IsSpecies(ArcDPSEnums.TrashID.EmbodimentOfEnvy))
                {
                    target.OverrideName(target.Character + " " + curEnvy++);
                }
                if (target.IsSpecies(ArcDPSEnums.TrashID.EmbodimentOfGluttony))
                {
                    target.OverrideName(target.Character + " " + curGluttony++);
                }
                if (target.IsSpecies(ArcDPSEnums.TrashID.EmbodimentOfMalice))
                {
                    target.OverrideName(target.Character + " " + curMalice++);
                }
                if (target.IsSpecies(ArcDPSEnums.TrashID.EmbodimentOfRage))
                {
                    target.OverrideName(target.Character + " " + curRage++);
                }
                if (target.IsSpecies(ArcDPSEnums.TrashID.EmbodimentOfRegret))
                {
                    target.OverrideName(target.Character + " " + curRegret++);
                }
            }
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>() { ArcDPSEnums.TrashID.MaliciousShadow };
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Temple of Febe";
        }
    }
}
