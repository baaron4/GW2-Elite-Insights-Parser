using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static LuckParser.Models.Statistics;

namespace LuckParser.Models.ParseModels
{
    public class Target : AbstractMasterActor
    {

        private List<double> _avgConditions;
        private List<double> _avgBoons;
        private List<Dictionary<long, FinalTargetBuffs>> _buffs;
        // Constructors
        public Target(AgentItem agent) : base(agent)
        {
        }

        public int Health { get; set; } = -1;
        public List<Point> HealthOverTime { get; } = new List<Point>();

        /*public void AddCustomCastLog(long time, long skillID, int expDur, ParseEnum.Activation startActivation, int actDur, ParseEnum.Activation endActivation, ParsedLog log)
        {
            if (CastLogs.Count == 0)
            {
                GetCastLogs(log, 0, log.FightData.FightEnd);
            }
            CastLogs.Add(new CastLog(time, skillID, expDur, startActivation, actDur, endActivation, Agent, InstID));
        }*/

        private void SetAvgBoonsConditions(ParsedLog log)
        {
            _avgBoons = new List<double>();
            _avgConditions = new List<double>();
            for (int phaseIndex = 0; phaseIndex < log.FightData.GetPhases(log).Count; phaseIndex++)
            {
                PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
                double avgBoon = 0;
                foreach (long duration in GetBuffPresence(log, phaseIndex).Where(x => Boon.BoonsByIds[x.Key].Nature == Boon.BoonNature.Boon).Select(x => x.Value))
                {
                    avgBoon += duration;
                }
                avgBoon /= phase.DurationInMS;
                _avgBoons.Add(avgBoon);

                double avgCondi = 0;
                foreach (long duration in GetBuffPresence(log, phaseIndex).Where(x => Boon.BoonsByIds[x.Key].Nature == Boon.BoonNature.Condition).Select(x => x.Value))
                {
                    avgCondi += duration;
                }
                avgCondi /= phase.DurationInMS;
                _avgConditions.Add(avgCondi);
            }
        }

        public double GetAverageBoons(ParsedLog log, int phaseIndex)
        {
            if (_avgBoons == null)
            {
                SetAvgBoonsConditions(log);
            }
            return _avgBoons[phaseIndex];
        }

        public List<double> GetAverageBoons(ParsedLog log)
        {
            if (_avgBoons == null)
            {
                SetAvgBoonsConditions(log);
            }
            return _avgBoons;
        }

        public double GetAverageConditions(ParsedLog log, int phaseIndex)
        {
            if (_avgConditions == null)
            {
                SetAvgBoonsConditions(log);
            }
            return _avgConditions[phaseIndex];
        }

        public List<double> GetAverageConditions(ParsedLog log)
        {
            if (_avgConditions == null)
            {
                SetAvgBoonsConditions(log);
            }
            return _avgConditions;
        }

        public Dictionary<long, FinalTargetBuffs> GetBuffs(ParsedLog log, int phaseIndex)
        {
            if (_buffs == null)
            {
                SetBuffs(log);
            }
            return _buffs[phaseIndex];
        }

        public List<Dictionary<long, FinalTargetBuffs>> GetBuffs(ParsedLog log)
        {
            if (_buffs == null)
            {
                SetBuffs(log);
            }
            return _buffs;
        }

        private void SetBuffs(ParsedLog log)
        {
            _buffs = new List<Dictionary<long, FinalTargetBuffs>>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                BoonDistribution boonDistribution = GetBoonDistribution(log, phaseIndex);
                Dictionary<long, FinalTargetBuffs> rates = new Dictionary<long, FinalTargetBuffs>();
                _buffs.Add(rates);
                Dictionary<long, long> buffPresence = GetBuffPresence(log, phaseIndex);

                PhaseData phase = phases[phaseIndex];
                long fightDuration = phase.DurationInMS;

                foreach (Boon boon in TrackedBoons)
                {
                    if (boonDistribution.ContainsKey(boon.ID))
                    {
                        FinalTargetBuffs buff = new FinalTargetBuffs(log.PlayerList);
                        rates[boon.ID] = buff;
                        if (boon.Type == Boon.BoonType.Duration)
                        {
                            buff.Uptime = Math.Round(100.0 * boonDistribution.GetUptime(boon.ID) / fightDuration, 2);
                            foreach (Player p in log.PlayerList)
                            {
                                long gen = boonDistribution.GetGeneration(boon.ID, p.AgentItem);
                                buff.Generated[p] = Math.Round(100.0 * gen / fightDuration, 2);
                                buff.Overstacked[p] = Math.Round(100.0 * (boonDistribution.GetOverstack(boon.ID, p.AgentItem) + gen) / fightDuration, 2);
                                buff.Wasted[p] = Math.Round(100.0 * boonDistribution.GetWaste(boon.ID, p.AgentItem) / fightDuration, 2);
                                buff.UnknownExtension[p] = Math.Round(100.0 * boonDistribution.GetUnknownExtension(boon.ID, p.AgentItem) / fightDuration, 2);
                                buff.Extension[p] = Math.Round(100.0 * boonDistribution.GetExtension(boon.ID, p.AgentItem) / fightDuration, 2);
                                buff.Extended[p] = Math.Round(100.0 * boonDistribution.GetExtended(boon.ID, p.AgentItem) / fightDuration, 2);
                            }
                        }
                        else if (boon.Type == Boon.BoonType.Intensity)
                        {
                            buff.Uptime = Math.Round((double)boonDistribution.GetUptime(boon.ID) / fightDuration, 2);
                            foreach (Player p in log.PlayerList)
                            {
                                long gen = boonDistribution.GetGeneration(boon.ID, p.AgentItem);
                                buff.Generated[p] = Math.Round((double)gen / fightDuration, 2);
                                buff.Overstacked[p] = Math.Round((double)(boonDistribution.GetOverstack(boon.ID, p.AgentItem) + gen) / fightDuration, 2);
                                buff.Wasted[p] = Math.Round((double)boonDistribution.GetWaste(boon.ID, p.AgentItem) / fightDuration, 2);
                                buff.UnknownExtension[p] = Math.Round((double)boonDistribution.GetUnknownExtension(boon.ID, p.AgentItem) / fightDuration, 2);
                                buff.Extension[p] = Math.Round((double)boonDistribution.GetExtension(boon.ID, p.AgentItem) / fightDuration, 2);
                                buff.Extended[p] = Math.Round((double)boonDistribution.GetExtended(boon.ID, p.AgentItem) / fightDuration, 2);
                            }
                            if (buffPresence.TryGetValue(boon.ID, out long presenceValueBoon))
                            {
                                buff.Presence = Math.Round(100.0 * presenceValueBoon / fightDuration, 2);
                            }
                        }
                    }
                }
            }
        }

        protected override void SetAdditionalCombatReplayData(ParsedLog log)
        {
            CombatReplay.Icon = GeneralHelper.GetNPCIcon(ID);
            log.FightData.Logic.ComputeAdditionalTargetData(this, log);
            if (CombatReplay.Rotations.Any())
            {
                CombatReplay.Actors.Add(new FacingActor(((int)CombatReplay.TimeOffsets.start, (int)CombatReplay.TimeOffsets.end), new AgentConnector(this), CombatReplay.PolledRotations));
            }
        }

        public void OverrideName(string name)
        {
            Character = name;
        }

        public List<double[]> Get1SHealthGraph(ParsedLog log, List<PhaseData> phases)
        {
            List<double[]> res = new List<double[]>();
            // fill the graph, full precision
            List<double> listFull = new List<double>();
            for (int i = 0; i <= phases[0].DurationInMS; i++)
            {
                listFull.Add(100.0);
            }
            int totalTime = 0;
            double curHealth = 100.0;
            foreach (Point p in HealthOverTime)
            {
                int time = p.X;
                if (time < 0)
                {
                    continue;
                }
                if (time > phases[0].DurationInMS)
                {
                    break;
                }
                for (; totalTime < time; totalTime++)
                {
                    listFull[totalTime] = curHealth;
                }
                curHealth = p.Y / 100.0;
                listFull[time] = curHealth;
            }
            // fill
            for (; totalTime <= phases[0].DurationInMS; totalTime++)
            {
                listFull[totalTime] = curHealth;
            }
            foreach (PhaseData phase in phases)
            {
                int seconds = (int)phase.DurationInS;
                bool needsLastPoint = seconds * 1000 != phase.DurationInMS;
                double[] hps = new double[seconds + (needsLastPoint ? +2 : 1)];
                int time = (int)phase.Start;
                int i = 0;
                for (i = 0; i <= seconds; i++)
                {
                    hps[i] = listFull[time];
                    time += 1000;
                }
                if (needsLastPoint)
                {
                    hps[i] = listFull[(int)phase.End];
                }
                res.Add(hps);
            }
            return res;
        }
        

        //
        private class TargetSerializable : AbstractMasterActorSerializable
        {
            public long Start { get; set; }
            public long End { get; set; }
        }

        public override AbstractMasterActorSerializable GetCombatReplayJSON(CombatReplayMap map)
        {
            TargetSerializable aux = new TargetSerializable
            {
                Img = CombatReplay.Icon,
                Type = "Target",
                ID = GetCombatReplayID(),
                Start = CombatReplay.TimeOffsets.start,
                End = CombatReplay.TimeOffsets.end,
                Positions = new double[2 * CombatReplay.Positions.Count]
            };
            int i = 0;
            foreach (Point3D pos in CombatReplay.Positions)
            {
                (double x, double y) = map.GetMapCoord(pos.X, pos.Y);
                aux.Positions[i++] = x;
                aux.Positions[i++] = y;
            }
            return aux;
        }

        /*protected override void setHealingLogs(ParsedLog log)
        {
            // nothing to do
        }

        protected override void setHealingReceivedLogs(ParsedLog log)
        {
            // nothing to do
        }*/
    }
}