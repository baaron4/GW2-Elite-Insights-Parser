using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Minion : AbstractActor
    {

        public Minion(AgentItem agent) : base(agent)
        {
        }

        protected override void SetDamageLogs(ParsedLog log)
        {
            foreach (CombatItem c in log.GetDamageData(InstID, FirstAware, LastAware))
            {
                long time = log.FightData.ToFightSpace(c.Time);
                AddDamageLog(time, c);
            }
        }

        protected override void SetBoonDistribution(ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            BoonMap toUse = GetBoonMap(log);
            long dur = log.FightData.FightDuration;
            int fightDuration = (int)(dur) / 1000;
            for (int i = 0; i < phases.Count; i++)
            {
                BoonDistribution.Add(new BoonDistribution());
            }

            long death = GetDeath(log, 0, dur);
            foreach (Boon boon in TrackedBoons)
            {
                long boonid = boon.ID;
                if (toUse.TryGetValue(boonid, out List<BoonLog> logs) && logs.Count != 0)
                {
                    if (BoonDistribution[0].ContainsKey(boonid))
                    {
                        continue;
                    }
                    BoonSimulator simulator = boon.CreateSimulator(log);
                    simulator.Simulate(logs, dur);
                    if (death > 0)
                    {
                        simulator.Trim(death);
                    }
                    else
                    {
                        simulator.Trim(log.FightData.ToFightSpace(LastAware));
                    }
                    GenerationSimulationResult generationSimulation = simulator.GenerationSimulationResult;
                    List<BoonsGraphModel.Segment> graphSegments = new List<BoonsGraphModel.Segment>();
                    foreach (BoonSimulationItem simul in generationSimulation.Items)
                    {
                        for (int i = 0; i < phases.Count; i++)
                        {
                            PhaseData phase = phases[i];
                            simul.SetBoonDistributionItem(BoonDistribution[i], phase.Start, phase.End, boonid, log);
                        }
                        BoonsGraphModel.Segment segment = simul.ToSegment();
                        if (graphSegments.Count == 0)
                        {
                            graphSegments.Add(new BoonsGraphModel.Segment(0, segment.Start, 0));
                        }
                        else if (graphSegments.Last().End != segment.Start)
                        {
                            graphSegments.Add(new BoonsGraphModel.Segment(graphSegments.Last().End, segment.Start, 0));
                        }
                        graphSegments.Add(segment);
                    }
                    List<AbstractBoonSimulationItem> extraSimulations = new List<AbstractBoonSimulationItem>(simulator.OverstackSimulationResult);
                    extraSimulations.AddRange(simulator.WasteSimulationResult);
                    foreach (AbstractBoonSimulationItem simul in extraSimulations)
                    {
                        for (int i = 0; i < phases.Count; i++)
                        {
                            PhaseData phase = phases[i];
                            simul.SetBoonDistributionItem(BoonDistribution[i], phase.Start, phase.End, boonid, log);
                        }
                    }
                    
                    if (graphSegments.Count > 0)
                    {
                        graphSegments.Add(new BoonsGraphModel.Segment(graphSegments.Last().End, dur, 0));
                    }
                    else
                    {
                        graphSegments.Add(new BoonsGraphModel.Segment(0, dur, 0));
                    }
                    BoonPoints[boonid] = new BoonsGraphModel(boon, graphSegments);
                }
            }
        }

        /*protected override void setHealingLogs(ParsedLog log)
        {
            long time_start = log.getBossData().getFirstAware();
            long min_time = Math.Max(time_start, agent.getFirstAware());
            long max_time = Math.Min(log.getBossData().getLastAware(), agent.getLastAware());
            foreach (CombatItem c in log.getHealingData())
            {
                if (agent.getInstid() == c.getSrcInstid() && c.getTime() > min_time && c.getTime() < max_time)//selecting minion as caster
                {
                    long time = c.getTime() - time_start;
                    addHealingLog(time, c);
                }
            }
        }

        protected override void setHealingReceivedLogs(ParsedLog log)
        {
            //nothing to do
        }*/
    }
}
