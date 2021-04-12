using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;
using Newtonsoft.Json;

namespace GW2EIBuilders.JsonModels
{
    /// <summary>
    /// Base class for Players and NPCs
    /// </summary>
    /// <seealso cref="JsonPlayerBuilder"/> 
    /// <seealso cref="JsonNPCBuilder"/>
    internal static class JsonActorBuilder
    {

        public static void FillJsonActor(JsonActor jsonActor, AbstractSingleActor actor, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            IReadOnlyList<PhaseData> phases = log.FightData.GetNonDummyPhases(log);
            //
            jsonActor.Name = actor.Character;
            jsonActor.TotalHealth = actor.GetHealth(log.CombatData);
            jsonActor.Toughness = actor.Toughness;
            jsonActor.Healing = actor.Healing;
            jsonActor.Concentration = actor.Concentration;
            jsonActor.Condition = actor.Condition;
            jsonActor.HitboxHeight = actor.HitboxHeight;
            jsonActor.HitboxWidth = actor.HitboxWidth;
            jsonActor.InstanceID = actor.InstID;
            //
            jsonActor.DpsAll = phases.Select(phase => JsonStatisticsBuilder.BuildJsonDPS(actor.GetDPSStats(log, phase.Start, phase.End))).ToArray();
            jsonActor.StatsAll = phases.Select(phase => JsonStatisticsBuilder.BuildJsonGameplayStatsAll(actor.GetGameplayStats(log, phase.Start, phase.End))).ToArray();
            jsonActor.Defenses = phases.Select(phase => JsonStatisticsBuilder.BuildJsonDefensesAll(actor.GetDefenseStats(log, phase.Start, phase.End))).ToArray();
            //
            IReadOnlyDictionary<long, Minions> minionsList = actor.GetMinions(log);
            if (minionsList.Values.Any())
            {
                jsonActor.Minions = minionsList.Values.Select(x => JsonMinionsBuilder.BuildJsonMinions(x, log, skillDesc, buffDesc)).ToList();
            }
            //
            var skillByID = actor.GetIntersectingCastEvents(log, 0, log.FightData.FightEnd).GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            if (skillByID.Any())
            {
                jsonActor.Rotation = JsonRotationBuilder.BuildJsonRotationList(log, skillByID, skillDesc);
            }
            //
            if (settings.RawFormatTimelineArrays)
            {
                var damage1S = new IReadOnlyList<int>[phases.Count];
                var powerDamage1S = new IReadOnlyList<int>[phases.Count];
                var conditionDamage1S = new IReadOnlyList<int>[phases.Count];
                var breakbarDamage1S = new IReadOnlyList<double>[phases.Count];
                for (int i = 0; i < phases.Count; i++)
                {
                    PhaseData phase = phases[i];
                    damage1S[i] = actor.Get1SDamageList(log, phase.Start, phase.End, null, ParserHelper.DamageType.All);
                    powerDamage1S[i] = actor.Get1SDamageList(log, phase.Start, phase.End, null, ParserHelper.DamageType.Power);
                    conditionDamage1S[i] = actor.Get1SDamageList(log, phase.Start, phase.End, null, ParserHelper.DamageType.Condition);
                    breakbarDamage1S[i] = actor.Get1SBreakbarDamageList(log, phase.Start, phase.End, null);
                }
                jsonActor.Damage1S = damage1S;
                jsonActor.PowerDamage1S = powerDamage1S;
                jsonActor.ConditionDamage1S = conditionDamage1S;
                jsonActor.BreakbarDamage1S = breakbarDamage1S;
            }
            if (!log.CombatData.HasBreakbarDamageData)
            {
                jsonActor.BreakbarDamage1S = null;
            }
            //
            jsonActor.TotalDamageDist = BuildDamageDistData(actor, phases, log, skillDesc, buffDesc);
            jsonActor.TotalDamageTaken = BuildDamageTakenDistData(actor, phases, log, skillDesc, buffDesc);
            //
            if (settings.RawFormatTimelineArrays)
            {
                Dictionary<long, BuffsGraphModel> buffGraphs = actor.GetBuffGraphs(log);
                jsonActor.BoonsStates = JsonBuffsUptimeBuilder.GetBuffStates(buffGraphs[Buff.NumberOfBoonsID]);
                jsonActor.ConditionsStates = JsonBuffsUptimeBuilder.GetBuffStates(buffGraphs[Buff.NumberOfConditionsID]);
                if (buffGraphs.TryGetValue(Buff.NumberOfActiveCombatMinions, out BuffsGraphModel states))
                {
                    jsonActor.ActiveCombatMinions = JsonBuffsUptimeBuilder.GetBuffStates(states);
                }
                // Health
                jsonActor.HealthPercents = actor.GetHealthUpdates(log).Select(x => new double[2] { x.Start, x.Value }).ToList();
                jsonActor.BarrierPercents = actor.GetBarrierUpdates(log).Select(x => new double[2] { x.Start, x.Value }).ToList();
            }
        }

        private static List<JsonDamageDist>[] BuildDamageDistData(AbstractSingleActor actor, IReadOnlyList<PhaseData> phases, ParsedEvtcLog log, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var res = new List<JsonDamageDist>[phases.Count];
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                res[i] = JsonDamageDistBuilder.BuildJsonDamageDistList(actor.GetDamageEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillDesc, buffDesc);
            }
            return res;
        }

        private static List<JsonDamageDist>[] BuildDamageTakenDistData(AbstractSingleActor actor, IReadOnlyList<PhaseData> phases, ParsedEvtcLog log, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var res = new List<JsonDamageDist>[phases.Count];
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                res[i] = JsonDamageDistBuilder.BuildJsonDamageDistList(actor.GetDamageTakenEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillDesc, buffDesc);
            }
            return res;
        }
    }
}
