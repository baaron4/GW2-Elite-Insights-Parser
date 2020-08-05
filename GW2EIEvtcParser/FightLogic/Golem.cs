using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIUtils;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Golem : FightLogic
    {
        public Golem(int id) : base(id)
        {
            Mode = ParseMode.Benchmark;
            switch (id)
            {
                case 16202:
                    Extension = "MassiveGolem";
                    Icon = "https://wiki.guildwars2.com/images/3/33/Mini_Snuggles.png";
                    break;
                case 16177:
                    Extension = "AvgGolem";
                    Icon = "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                    break;
                case 19676:
                    Extension = "LGolem";
                    Icon = "https://wiki.guildwars2.com/images/4/47/Mini_Baron_von_Scrufflebutt.png";
                    break;
                case 19645:
                    Extension = "MedGolem";
                    Icon = "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                    break;
                case 16199:
                    Extension = "StdGolem";
                    Icon = "https://wiki.guildwars2.com/images/8/8f/Mini_Professor_Mew.png";
                    break;
            }
        }

        internal override long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            CombatItem pov = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.PointOfView);
            if (pov != null)
            {
                // to make sure that the logging starts when the PoV starts attacking (in case there is a slave with them)
                CombatItem enterCombat = combatData.FirstOrDefault(x => x.SrcAgent == pov.SrcAgent && x.IsStateChange == ArcDPSEnums.StateChange.EnterCombat);
                if (enterCombat != null)
                {
                    fightData.OverrideOffset(enterCombat.Time);
                }
            }
            return fightData.FightOffset;
        }

        internal override void EIEvtcParse(FightData fightData, AgentData agentData, List<CombatItem> combatData, List<Player> playerList)
        {
            AgentItem target = agentData.GetNPCsByID(GenericTriggerID).FirstOrDefault();
            foreach (CombatItem c in combatData)
            {
                // redirect all attacks to the main golem
                if (c.DstAgent == 0 && c.DstInstid == 0 && c.IsStateChange == ArcDPSEnums.StateChange.None && c.IFF == ArcDPSEnums.IFF.Foe && c.IsActivation == ArcDPSEnums.Activation.None && c.IsBuffRemove == ArcDPSEnums.BuffRemove.None)
                {
                    c.OverrideDstAgent(target.Agent);
                }
            }
            ComputeFightTargets(agentData, combatData);
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.Find(x => x.ID == GenericTriggerID);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Golem not found");
            }
            phases[0].Name = "Final Number";
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            List<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(mainTarget.AgentItem);
            if (hpUpdates.Count > 0)
            {
                long fightDuration = log.FightData.FightEnd;
                var thresholds = new List<double> { 80, 60, 40, 20, 0 };
                var numberNames = new string[] { "First Number", "Second Number", "Third Number", "Fourth Number" };
                // Fifth number would the equivalent of full fight phase
                for (int j = 0; j < thresholds.Count - 1; j++)
                {
                    HealthUpdateEvent hpUpdate = hpUpdates.FirstOrDefault(x => x.HPPercent <= thresholds[j]);
                    if (hpUpdate != null)
                    {
                        var phase = new PhaseData(0, hpUpdate.Time, numberNames[j])
                        {
                            CanBeSubPhase = false
                        };
                        phase.Targets.Add(mainTarget);
                        phases.Add(phase);
                    }
                }
                phases.AddRange(GetPhasesByHealthPercent(log, mainTarget, thresholds));
            }
            
            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            NPC mainTarget = Targets.Find(x => x.ID == GenericTriggerID);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Golem not found");
            }
            AbstractDamageEvent lastDamageTaken = combatData.GetDamageTakenData(mainTarget.AgentItem).LastOrDefault(x => x.Damage > 0);
            long fightEndLogTime = fightData.FightEnd;
            bool success = false;
            if (lastDamageTaken != null)
            {
                fightEndLogTime = lastDamageTaken.Time;
            }
            List<HealthUpdateEvent> hpUpdates = combatData.GetHealthUpdateEvents(mainTarget.AgentItem);
            if (hpUpdates.Count > 0)
            {
                success = hpUpdates.Last().HPPercent < 2.00;
            }
            fightData.SetSuccess(success, fightEndLogTime);
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                GenericTriggerID
            };
        }
    }
}
