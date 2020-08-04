using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIParser.Builders.JsonModels.JsonBuffsUptime;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class representing an NPC
    /// </summary>
    public class JsonNPC : JsonActor
    {
        /// <summary>
        /// Game ID of the target
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// Total health of the target
        /// </summary>
        public int TotalHealth { get; }
        /// <summary>
        /// Final health of the target
        /// </summary>
        public int FinalHealth { get; }
        /// <summary>
        /// % of health burned
        /// </summary>
        public double HealthPercentBurned { get; }
        /// <summary>
        /// Time at which target became active
        /// </summary>
        public int FirstAware { get; }
        /// <summary>
        /// Time at which target became inactive 
        /// </summary>
        public int LastAware { get; }
        /// <summary>
        /// List of buff status
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsUptime> Buffs { get; protected set; }
        /// <summary>
        /// Array of double[2] that represents the breakbar percent of the actor \n
        /// Value[i][0] will be the time, value[i][1] will be breakbar % \n
        /// If i corresponds to the last element that means the breakbar did not change for the remainder of the fight \n
        /// </summary>
        public List<double[]> BreakbarPercents { get; }

        public JsonNPC(NPC npc, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc) : base(npc, log, settings, skillDesc, buffDesc)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            //
            Id = npc.ID;
            List<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(npc.AgentItem);
            TotalHealth = npc.GetHealth(log.CombatData);
            FirstAware = (int)npc.FirstAware;
            LastAware = (int)npc.LastAware;
            double hpLeft = 0.0;
            if (log.FightData.Success)
            {
                hpLeft = 0;
            }
            else
            {
                if (hpUpdates.Count > 0)
                {
                    hpLeft = hpUpdates.Last().HPPercent;
                }
            }
            HealthPercentBurned = 100.0 - hpLeft;
            FinalHealth = (int)Math.Round(TotalHealth * hpLeft / 100.0);
            //
            Buffs = GetNPCJsonBuffsUptime(npc, npc.GetBuffs(log), npc.GetBuffsDictionary(log), log, settings, buffDesc);
            // Breakbar
            List<BreakbarPercentEvent> breakbarPercentUpdates = log.CombatData.GetBreakbarPercentEvents(npc.AgentItem);
            if (settings.RawFormatTimelineArrays)
            {
                BreakbarPercents = breakbarPercentUpdates.Select(x => new double[2] { x.Time, x.BreakbarPercent }).ToList();
            }
        }

        private static List<JsonBuffsUptime> GetNPCJsonBuffsUptime(NPC npc, List<Dictionary<long, FinalBuffs>> buffs, List<Dictionary<long, FinalBuffsDictionary>> buffsDictionary, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var res = new List<JsonBuffsUptime>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            foreach (KeyValuePair<long, FinalBuffs> pair in buffs[0])
            {
                var data = new List<JsonBuffsUptimeData>();
                for (int i = 0; i < phases.Count; i++)
                {
                    var value = new JsonBuffsUptimeData(buffs[i][pair.Key], buffsDictionary[i][pair.Key]);
                    data.Add(value);
                }
                res.Add(new JsonBuffsUptime(npc, pair.Key, log, settings, data, buffDesc));
            }
            return res;
        }
    }
}
