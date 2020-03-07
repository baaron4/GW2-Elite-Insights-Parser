using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Builders.JsonModels.JsonBuffsUptime;

namespace GW2EIParser.Builders.JsonModels
{
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
        /// Array of double[2] that represents the health status of the target \n
        /// Value[i][0] will be the time, value[i][1] will be health % \n
        /// If i corresponds to the last element that means the health did not change for the remainder of the fight \n
        /// </summary>
        public List<double[]> HealthPercents { get; }
        /// <summary>
        /// List of buff status
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public List<JsonBuffsUptime> Buffs { get; protected set; }

        public JsonNPC(NPC npc, ParsedLog log, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc) : base(npc, log, skillDesc, buffDesc)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            //
            Id = npc.ID;
            TotalHealth = npc.GetHealth(log.CombatData);
            List<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(npc.AgentItem);
            if (log.ParserSettings.RawTimelineArrays)
            {
                HealthPercents = hpUpdates.Select(x => new double[2] { x.Time, x.HPPercent }).ToList();
            }
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
            Buffs = GetNPCJsonBuffsUptime(npc, npc.GetBuffs(log), npc.GetBuffsDictionary(log), log, buffDesc);
        }

        private static List<JsonBuffsUptime> GetNPCJsonBuffsUptime(NPC npc, List<Dictionary<long, FinalBuffs>> buffs, List<Dictionary<long, FinalBuffsDictionary>> buffsDictionary, ParsedLog log, Dictionary<string, JsonLog.BuffDesc> buffDesc)
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
                res.Add(new JsonBuffsUptime(npc, pair.Key, log, data, buffDesc));
            }
            return res;
        }
    }
}
