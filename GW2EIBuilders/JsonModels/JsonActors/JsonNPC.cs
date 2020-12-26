using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using Newtonsoft.Json;
using static GW2EIBuilders.JsonModels.JsonBuffsUptime;

namespace GW2EIBuilders.JsonModels
{
    /// <summary>
    /// Class representing an NPC
    /// </summary>
    public class JsonNPC : JsonActor
    {
        [JsonProperty]
        /// <summary>
        /// Game ID of the target
        /// </summary>
        public int Id { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Total health of the target
        /// </summary>
        public int TotalHealth { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Final health of the target
        /// </summary>
        public int FinalHealth { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// % of health burned
        /// </summary>
        public double HealthPercentBurned { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Time at which target became active
        /// </summary>
        public int FirstAware { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Time at which target became inactive 
        /// </summary>
        public int LastAware { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// List of buff status
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public IReadOnlyList<JsonBuffsUptime> Buffs { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Array of double[2] that represents the breakbar percent of the actor \n
        /// Value[i][0] will be the time, value[i][1] will be breakbar % \n
        /// If i corresponds to the last element that means the breakbar did not change for the remainder of the fight \n
        /// </summary>
        public IReadOnlyList<IReadOnlyList<double>> BreakbarPercents { get; internal set; }

        [JsonConstructor]
        internal JsonNPC()
        {

        }

        internal JsonNPC(NPC npc, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc) : base(npc, log, settings, skillDesc, buffDesc)
        {
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            //
            Id = npc.ID;
            IReadOnlyList<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(npc.AgentItem);
            TotalHealth = npc.GetHealth(log.CombatData);
            FirstAware = (int)npc.FirstAware;
            LastAware = (int)npc.LastAware;
            double hpLeft = 100.0;
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
            Buffs = GetNPCJsonBuffsUptime(npc, log, settings, buffDesc);
            // Breakbar
            if (settings.RawFormatTimelineArrays)
            {
                BreakbarPercents = npc.GetBreakbarPercentUpdates(log).Select(x => new double[2] { x.Start, x.Value }).ToList();
            }
        }

        private static List<JsonBuffsUptime> GetNPCJsonBuffsUptime(NPC npc, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var res = new List<JsonBuffsUptime>();
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            var buffs = phases.Select(x => npc.GetBuffs(log, x.Start, x.End)).ToList();
            foreach (KeyValuePair<long, FinalBuffs> pair in buffs[0])
            {
                var data = new List<JsonBuffsUptimeData>();
                for (int i = 0; i < phases.Count; i++)
                {
                    PhaseData phase = phases[i];
                    Dictionary<long, FinalBuffsDictionary> buffsDictionary = npc.GetBuffsDictionary(log, phase.Start, phase.End);
                    if (buffs[i].TryGetValue(pair.Key, out FinalBuffs val))
                    {
                        var value = new JsonBuffsUptimeData(val, buffsDictionary[pair.Key]);
                        data.Add(value);
                    }
                    else
                    {
                        var value = new JsonBuffsUptimeData();
                        data.Add(value);
                    }
                }
                res.Add(new JsonBuffsUptime(npc, pair.Key, log, settings, data, buffDesc));
            }
            return res;
        }
    }
}
