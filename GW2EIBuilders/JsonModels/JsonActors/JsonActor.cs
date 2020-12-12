using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using Newtonsoft.Json;

namespace GW2EIBuilders.JsonModels
{
    /// <summary>
    /// Base class for Players and NPCs
    /// </summary>
    /// <seealso cref="JsonPlayer"/> 
    /// <seealso cref="JsonNPC"/>
    public abstract class JsonActor
    {

        [JsonProperty]
        /// <summary>
        /// Name of the actor
        /// </summary>
        public string Name { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Condition damage score
        /// </summary>
        public uint Condition { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Concentration score
        /// </summary>
        public uint Concentration { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Healing Power score
        /// </summary>
        public uint Healing { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Toughness score
        /// </summary>
        public uint Toughness { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Height of the hitbox
        /// </summary>
        public uint HitboxHeight { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Width of the hitbox
        /// </summary>
        public uint HitboxWidth { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// ID of the actor in the instance
        /// </summary>
        public ushort InstanceID { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// List of minions
        /// </summary>
        /// <seealso cref="JsonMinions"/>
        public List<JsonMinions> Minions { get; internal set; }
        [JsonProperty]

        /// <summary>
        /// Array of Total DPS stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDPS"/>
        public JsonStatistics.JsonDPS[] DpsAll { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Stats against all  \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonGameplayStatsAll"/>
        public JsonStatistics.JsonGameplayStatsAll[] StatsAll { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Defensive stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDefensesAll"/>
        public JsonStatistics.JsonDefensesAll[] Defenses { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Total Damage distribution array \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<JsonDamageDist>[] TotalDamageDist { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Damage taken array
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<JsonDamageDist>[] TotalDamageTaken { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Rotation data
        /// </summary>
        /// <seealso cref="JsonRotation"/>
        public List<JsonRotation> Rotation { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Array of int representing 1S damage points \n
        /// Length == # of phases
        /// </summary>
        /// <remarks>
        /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
        /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
        /// </remarks>
        public List<int>[] Damage1S { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Array of double representing 1S breakbar damage points \n
        /// Length == # of phases
        /// </summary>
        /// <remarks>
        /// If the duration of the phase in seconds is non integer, the last point of this array will correspond to the last point  \n
        /// ex: duration === 15250ms, the array will have 17 elements [0, 1000,...,15000,15250]
        /// </remarks>
        public List<double>[] BreakbarDamage1S { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Array of int[2] that represents the number of conditions \n
        /// Array[i][0] will be the time, Array[i][1] will be the number of conditions present from Array[i][0] to Array[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight \n
        /// </summary>
        public List<int[]> ConditionsStates { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Array of int[2] that represents the number of boons \n
        /// Array[i][0] will be the time, Array[i][1] will be the number of boons present from Array[i][0] to Array[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public List<int[]> BoonsStates { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Array of int[2] that represents the number of active combat minions \n
        /// Array[i][0] will be the time, Array[i][1] will be the number of active combat minions present from Array[i][0] to Array[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public List<int[]> ActiveCombatMinions { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Array of double[2] that represents the health status of the actor \n
        /// Array[i][0] will be the time, Array[i][1] will be health % \n
        /// If i corresponds to the last element that means the health did not change for the remainder of the fight \n
        /// </summary>
        public List<double[]> HealthPercents { get; internal set; }


        [JsonConstructor]
        internal JsonActor()
        {

        }

        protected JsonActor(AbstractSingleActor actor, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            //
            Name = actor.Character;
            Toughness = actor.Toughness;
            Healing = actor.Healing;
            Concentration = actor.Concentration;
            Condition = actor.Condition;
            HitboxHeight = actor.HitboxHeight;
            HitboxWidth = actor.HitboxWidth;
            InstanceID = actor.InstID;
            //
            DpsAll = actor.GetDPSAll(log).Select(x => new JsonStatistics.JsonDPS(x)).ToArray();
            StatsAll = actor.GetGameplayStats(log).Select(x => new JsonStatistics.JsonGameplayStatsAll(x)).ToArray();
            Defenses = actor.GetDefenses(log).Select(x => new JsonStatistics.JsonDefensesAll(x)).ToArray();
            //
            Dictionary<long, Minions> minionsList = actor.GetMinions(log);
            if (minionsList.Values.Any())
            {
                Minions = minionsList.Values.Select(x => new JsonMinions(x, log, skillDesc, buffDesc)).ToList();
            }
            //
            var skillByID = actor.GetIntersectingCastLogs(log, 0, log.FightData.FightEnd).GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            if (skillByID.Any())
            {
                Rotation = JsonRotation.BuildJsonRotationList(log, skillByID, skillDesc);
            }
            //
            if (settings.RawFormatTimelineArrays)
            {
                Damage1S = new List<int>[phases.Count];
                BreakbarDamage1S = new List<double>[phases.Count];
                for (int i = 0; i < phases.Count; i++)
                {
                    Damage1S[i] = actor.Get1SDamageList(log, i, phases[i], null);
                    BreakbarDamage1S[i] = actor.Get1SBreakbarDamageList(log, i, phases[i], null);
                }
            }
            if (!log.CombatData.HasBreakbarDamageData)
            {
                BreakbarDamage1S = null;
            }
            //
            TotalDamageDist = BuildDamageDistData(actor, phases, log, skillDesc, buffDesc);
            TotalDamageTaken = BuildDamageTakenDistData(actor, phases, log, skillDesc, buffDesc);
            //
            if (settings.RawFormatTimelineArrays)
            {
                Dictionary<long, BuffsGraphModel> buffGraphs = actor.GetBuffGraphs(log);
                BoonsStates = JsonBuffsUptime.GetBuffStates(buffGraphs[Buff.NumberOfBoonsID]);
                ConditionsStates = JsonBuffsUptime.GetBuffStates(buffGraphs[Buff.NumberOfConditionsID]);
                if (buffGraphs.TryGetValue(Buff.NumberOfActiveCombatMinions, out BuffsGraphModel states))
                {
                    ActiveCombatMinions = JsonBuffsUptime.GetBuffStates(states);
                }
            }
            // Health
            if (settings.RawFormatTimelineArrays)
            {
                HealthPercents = actor.GetHealthUpdates(log).Select(x => new double[2] { x.Start, x.Value }).ToList();
            }
        }

        protected static List<JsonDamageDist>[] BuildDamageDistData(AbstractSingleActor actor, List<PhaseData> phases, ParsedEvtcLog log, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var res = new List<JsonDamageDist>[phases.Count];
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                res[i] = JsonDamageDist.BuildJsonDamageDistList(actor.GetDamageLogs(null, log, phase.Start, phase.End).GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillDesc, buffDesc);
            }
            return res;
        }

        protected static List<JsonDamageDist>[] BuildDamageTakenDistData(AbstractSingleActor actor, List<PhaseData> phases, ParsedEvtcLog log, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var res = new List<JsonDamageDist>[phases.Count];
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                res[i] = JsonDamageDist.BuildJsonDamageDistList(actor.GetDamageTakenLogs(null, log, phase.Start, phase.End).GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillDesc, buffDesc);
            }
            return res;
        }
    }
}
