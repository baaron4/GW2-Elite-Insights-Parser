using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using Newtonsoft.Json;

namespace GW2EIBuilders.JsonModels
{
    /// <summary>
    /// Class corresponding to the regrouping of the same type of minions
    /// </summary>
    public class JsonMinions
    {
        [JsonProperty]
        /// <summary>
        /// Name of the minion
        /// </summary>
        public string Name { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Total Damage done by minions \n
        /// Length == # of phases
        /// </summary>
        public List<int> TotalDamage { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Damage done by minions against targets \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        public List<int>[] TotalTargetDamage { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Total Breakbar Damage done by minions \n
        /// Length == # of phases
        /// </summary>
        public List<double> TotalBreakbarDamage { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Breakbar Damage done by minions against targets \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        public List<double>[] TotalTargetBreakbarDamage { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Total Shield Damage done by minions \n
        /// Length == # of phases
        /// </summary>
        public List<int> TotalShieldDamage { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Shield Damage done by minions against targets \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        public List<int>[] TotalTargetShieldDamage { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Total Damage distribution array \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<JsonDamageDist>[] TotalDamageDist { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Per Target Damage distribution array \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<JsonDamageDist>[][] TargetDamageDist { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Rotation data
        /// </summary>
        /// <seealso cref="JsonRotation"/>
        public List<JsonRotation> Rotation { get; internal set; }

        [JsonConstructor]
        internal JsonMinions()
        {

        }

        internal JsonMinions(Minions minions, ParsedEvtcLog log, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            bool isNPCMinion = minions.Master is NPC;
            //
            Name = minions.Character;
            //
            var totalDamage = new List<int>();
            var totalShieldDamage = new List<int>();
            var totalBreakbarDamage = new List<double>();
            foreach (PhaseData phase in phases)
            {
                int tot = 0;
                int shdTot = 0;
                foreach (AbstractHealthDamageEvent de in minions.GetDamageLogs(null, log, phase.Start, phase.End))
                {
                    tot += de.HealthDamage;
                    shdTot = de.ShieldDamage;
                }
                totalDamage.Add(tot);
                totalShieldDamage.Add(shdTot);
                totalBreakbarDamage.Add(Math.Round(minions.GetBreakbarDamageLogs(null, log, phase.Start, phase.End).Sum(x => x.BreakbarDamage), 1));
            }
            TotalDamage = totalDamage;
            TotalShieldDamage = totalShieldDamage;
            TotalBreakbarDamage = totalBreakbarDamage;
            if (!isNPCMinion)
            {
                var totalTargetDamage = new List<int>[log.FightData.Logic.Targets.Count];
                var totalTargetShieldDamage = new List<int>[log.FightData.Logic.Targets.Count];
                var totalTargetBreakbarDamage = new List<double>[log.FightData.Logic.Targets.Count];
                for (int i = 0; i < log.FightData.Logic.Targets.Count; i++)
                {
                    NPC tar = log.FightData.Logic.Targets[i];
                    var totalTarDamage = new List<int>();
                    var totalTarShieldDamage = new List<int>();
                    var totalTarBreakbarDamage = new List<double>();
                    foreach (PhaseData phase in phases)
                    {
                        int tot = 0;
                        int shdTot = 0;
                        foreach (AbstractHealthDamageEvent de in minions.GetDamageLogs(tar, log, phase.Start, phase.End))
                        {
                            tot += de.HealthDamage;
                            shdTot = de.ShieldDamage;
                        }
                        totalTarDamage.Add(tot);
                        totalTarShieldDamage.Add(shdTot);
                        totalTarBreakbarDamage.Add(Math.Round(minions.GetBreakbarDamageLogs(tar, log, phase.Start, phase.End).Sum(x => x.BreakbarDamage), 1));
                    }
                    totalTargetDamage[i] = totalTarDamage;
                    totalTargetShieldDamage[i] = totalTarShieldDamage;
                    totalTargetBreakbarDamage[i] = totalTarBreakbarDamage;
                }
                TotalTargetShieldDamage = totalTargetShieldDamage;
                TotalTargetDamage = totalTargetDamage;
                TotalTargetBreakbarDamage = totalTargetBreakbarDamage;
            }
            //
            var skillByID = minions.GetIntersectingCastLogs(log, 0, log.FightData.FightEnd).GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            if (skillByID.Any())
            {
                Rotation = JsonRotation.BuildJsonRotationList(log, skillByID, skillDesc);
            }
            //
            TotalDamageDist = new List<JsonDamageDist>[phases.Count];
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                TotalDamageDist[i] = JsonDamageDist.BuildJsonDamageDistList(minions.GetDamageLogs(null, log, phase.Start, phase.End).GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillDesc, buffDesc);
            }
            if (!isNPCMinion)
            {
                TargetDamageDist = new List<JsonDamageDist>[log.FightData.Logic.Targets.Count][];
                for (int i = 0; i < log.FightData.Logic.Targets.Count; i++)
                {
                    NPC target = log.FightData.Logic.Targets[i];
                    TargetDamageDist[i] = new List<JsonDamageDist>[phases.Count];
                    for (int j = 0; j < phases.Count; j++)
                    {
                        PhaseData phase = phases[j];
                        TargetDamageDist[i][j] = JsonDamageDist.BuildJsonDamageDistList(minions.GetDamageLogs(target, log, phase.Start, phase.End).GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillDesc, buffDesc);
                    }
                }
            }
        }

    }
}
