using GW2EIBuilders.JsonModels.JsonActorUtilities;
using GW2EIBuilders.JsonModels.JsonActorUtilities.JsonExtensions.EXTBarrier;
using GW2EIBuilders.JsonModels.JsonActorUtilities.JsonExtensions.EXTHealing;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using static GW2EIBuilders.JsonModels.JsonLogBuilder;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIBuilders.JsonModels.JsonActors;

/// <summary>
/// Class corresponding to the regrouping of the same type of minions
/// </summary>
internal static class JsonMinionsBuilder
{

    public static JsonMinions BuildJsonMinions(Minions minionGroup, ParsedEvtcLog log, RawFormatSettings settings, UsedJsonData usedData)
    {
        var jsonMinions = new JsonMinions
        {
            Id = minionGroup.ID,
        };
        IReadOnlyList<PhaseData> phases = log.LogData.GetPhases(log);
        bool isEnemyMinion = !log.FriendlyAgents.Contains(minionGroup.Master.AgentItem);
        //
        jsonMinions.Name = minionGroup.Character;
        //
        var totalDamage = new List<int>(phases.Count);
        var totalShieldDamage = new List<int>(phases.Count);
        var totalBreakbarDamage = new List<double>(phases.Count);
        var totalDamageTaken = new List<int>(phases.Count);
        var totalShieldDamageTaken = new List<int>(phases.Count);
        var totalBreakbarDamageTaken = new List<double>(phases.Count);
        foreach (PhaseData phase in phases)
        {
            int tot = 0;
            int shdTot = 0;
            foreach (HealthDamageEvent de in minionGroup.GetDamageEvents(null, log, phase.Start, phase.End))
            {
                tot += de.HealthDamage;
                shdTot += de.ShieldDamage;
            }
            totalDamage.Add(tot);
            totalShieldDamage.Add(shdTot);
            totalBreakbarDamage.Add(Math.Round(minionGroup.GetBreakbarDamageEvents(null, log, phase.Start, phase.End).Sum(x => x.BreakbarDamage), 1));
            //
            int totTaken = 0;
            int shdTotTaken = 0;
            foreach (HealthDamageEvent de in minionGroup.GetDamageTakenEvents(null, log, phase.Start, phase.End))
            {
                totTaken += de.HealthDamage;
                shdTotTaken += de.ShieldDamage;
            }
            totalDamageTaken.Add(totTaken);
            totalShieldDamageTaken.Add(shdTotTaken);
            totalBreakbarDamageTaken.Add(Math.Round(minionGroup.GetBreakbarDamageTakenEvents(null, log, phase.Start, phase.End).Sum(x => x.BreakbarDamage), 1));
        }
        jsonMinions.TotalDamage = totalDamage;
        jsonMinions.TotalShieldDamage = totalShieldDamage;
        jsonMinions.TotalBreakbarDamage = totalBreakbarDamage;
        jsonMinions.TotalDamageTaken = totalDamageTaken;
        jsonMinions.TotalShieldDamageTaken = totalShieldDamageTaken;
        jsonMinions.TotalBreakbarDamageTaken = totalBreakbarDamageTaken;
        if (!isEnemyMinion)
        {
            var totalTargetDamage = new IReadOnlyList<int>[log.LogData.Logic.Targets.Count];
            var totalTargetShieldDamage = new IReadOnlyList<int>[log.LogData.Logic.Targets.Count];
            var totalTargetBreakbarDamage = new IReadOnlyList<double>[log.LogData.Logic.Targets.Count];
            for (int i = 0; i < log.LogData.Logic.Targets.Count; i++)
            {
                SingleActor tar = log.LogData.Logic.Targets[i];
                var totalTarDamage = new List<int>(phases.Count);
                var totalTarShieldDamage = new List<int>(phases.Count);
                var totalTarBreakbarDamage = new List<double>(phases.Count);
                foreach (PhaseData phase in phases)
                {
                    int tot = 0;
                    int shdTot = 0;
                    foreach (HealthDamageEvent de in minionGroup.GetDamageEvents(tar, log, phase.Start, phase.End))
                    {
                        tot += de.HealthDamage;
                        shdTot += de.ShieldDamage;
                    }
                    totalTarDamage.Add(tot);
                    totalTarShieldDamage.Add(shdTot);
                    totalTarBreakbarDamage.Add(Math.Round(minionGroup.GetBreakbarDamageEvents(tar, log, phase.Start, phase.End).Sum(x => x.BreakbarDamage), 1));
                }
                totalTargetDamage[i] = totalTarDamage;
                totalTargetShieldDamage[i] = totalTarShieldDamage;
                totalTargetBreakbarDamage[i] = totalTarBreakbarDamage;
            }
            jsonMinions.TotalTargetShieldDamage = totalTargetShieldDamage;
            jsonMinions.TotalTargetDamage = totalTargetDamage;
            jsonMinions.TotalTargetBreakbarDamage = totalTargetBreakbarDamage;
        }
        //
        var totalDamageDist = new IReadOnlyList<JsonDamageDist>[phases.Count];
        var totalDamageTakenDist = new IReadOnlyList<JsonDamageDist>[phases.Count];
        for (int i = 0; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            totalDamageDist[i] = JsonDamageDistBuilder.BuildJsonDamageDistList(
                minionGroup.GetDamageEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList()),
                minionGroup.GetBreakbarDamageEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList()),
                null,
                log,
                usedData
            );
            totalDamageTakenDist[i] = JsonDamageDistBuilder.BuildJsonDamageDistList(
                minionGroup.GetDamageTakenEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList()),
                minionGroup.GetBreakbarDamageTakenEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList()),
                null,
                log,
                usedData
            );
        }
        jsonMinions.TotalDamageDist = totalDamageDist;
        jsonMinions.TotalDamageTakenDist = totalDamageTakenDist;
        if (!isEnemyMinion)
        {
            var targetDamageDist = new IReadOnlyList<JsonDamageDist>[log.LogData.Logic.Targets.Count][];
            for (int i = 0; i < log.LogData.Logic.Targets.Count; i++)
            {
                SingleActor target = log.LogData.Logic.Targets[i];
                targetDamageDist[i] = new IReadOnlyList<JsonDamageDist>[phases.Count];
                for (int j = 0; j < phases.Count; j++)
                {
                    PhaseData phase = phases[j];
                    targetDamageDist[i][j] = JsonDamageDistBuilder.BuildJsonDamageDistList(
                        minionGroup.GetDamageEvents(target, log, phase.Start, phase.End).GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList()),
                        minionGroup.GetBreakbarDamageEvents(target, log, phase.Start, phase.End).GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList()),
                        null,
                        log,
                        usedData
                    );
                }
            }
            jsonMinions.TargetDamageDist = targetDamageDist;
        }
        //
        if (log.CombatData.HasEXTHealing && !isEnemyMinion)
        {
            jsonMinions.EXTHealingStats = EXTJsonMinionsHealingStatsBuilder.BuildMinionsHealingStats(minionGroup, log, usedData);
        }
        if (log.CombatData.HasEXTBarrier && !isEnemyMinion)
        {
            jsonMinions.EXTBarrierStats = EXTJsonMinionsBarrierStatsBuilder.BuildMinionsBarrierStats(minionGroup, log, usedData);
        }
        if (log.CanCombatReplay)
        {
            jsonMinions.CombatReplayData = minionGroup.MinionList.Select(x => JsonActorCombatReplayDataBuilder.BuildJsonActorCombatReplayDataBuilder(x, log, settings)).ToList();
        }
        if (minionGroup.IsWhitelisted(log))
        {
            var instances = new List<JsonMinionInstance>(minionGroup.MinionList.Count);
            foreach (var minion in minionGroup.MinionList)
            {
                instances.Add(JsonMinionInstanceBuilder.BuildJsonMinionInstance(minion, log, settings, usedData, !isEnemyMinion));
            }
            if (instances.Count > 0)
            {
                jsonMinions.Instances = instances;
            }
        }
        return jsonMinions;
    }

}
