using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public class NPC : AbstractSingleActor
    {
        private List<Dictionary<long, FinalBuffs>> _buffs;
        // Constructors
        public NPC(AgentItem agent) : base(agent)
        {
        }

        private int _health = -1;
        private readonly List<double[]> _healthUpdates = new List<double[]>();

        public int GetHealth(CombatData combatData)
        {
            if (_health == -1)
            {
                List<MaxHealthUpdateEvent> maxHpUpdates = combatData.GetMaxHealthUpdateEvents(AgentItem);
                _health = maxHpUpdates.Count > 0 ? maxHpUpdates.Max(x => x.MaxHealth) : 1;
            }
            return _health;
        }

        public void OverrideName(string name)
        {
            Character = name;
        }

        public void SetManualHealth(int health)
        {
            _health = health;
        }

        /*public void AddCustomCastLog(long time, long skillID, int expDur, ParseEnum.Activation startActivation, int actDur, ParseEnum.Activation endActivation, ParsedLog log)
        {
            if (CastLogs.Count == 0)
            {
                GetCastLogs(log, 0, log.FightData.FightEnd);
            }
            CastLogs.Add(new CastLog(time, skillID, expDur, startActivation, actDur, endActivation, Agent, InstID));
        }*/

        public Dictionary<long, FinalBuffs> GetBuffs(ParsedLog log, int phaseIndex)
        {
            if (_buffs == null)
            {
                SetBuffs(log);
            }
            return _buffs[phaseIndex];
        }

        public List<Dictionary<long, FinalBuffs>> GetBuffs(ParsedLog log)
        {
            if (_buffs == null)
            {
                SetBuffs(log);
            }
            return _buffs;
        }

        private void SetBuffs(ParsedLog log)
        {
            _buffs = new List<Dictionary<long, FinalBuffs>>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                BuffDistribution buffDistribution = GetBuffDistribution(log, phaseIndex);
                var rates = new Dictionary<long, FinalBuffs>();
                _buffs.Add(rates);
                Dictionary<long, long> buffPresence = GetBuffPresence(log, phaseIndex);

                PhaseData phase = phases[phaseIndex];
                long phaseDuration = phase.DurationInMS;

                foreach (Buff buff in TrackedBuffs)
                {
                    if (buffDistribution.ContainsKey(buff.ID))
                    {
                        rates[buff.ID] = new FinalBuffs(buff, buffDistribution, buffPresence, phaseDuration);
                    }
                }
            }
        }

        protected override void InitAdditionalCombatReplayData(ParsedLog log)
        {
            log.FightData.Logic.ComputeNPCCombatReplayActors(this, log, CombatReplay);
            if (CombatReplay.Rotations.Any() && log.FightData.Logic.Targets.Contains(this))
            {
                CombatReplay.Decorations.Add(new FacingDecoration(((int)CombatReplay.TimeOffsets.start, (int)CombatReplay.TimeOffsets.end), new AgentConnector(this), CombatReplay.PolledRotations));
            }
        }

        public List<double[]> Get1SHealthGraph(ParsedLog log)
        {
            if (_healthUpdates.Count == 0)
            {
                List<PhaseData> phases = log.FightData.GetPhases(log);
                // fill the graph, full precision
                var listFull = new List<double>();
                for (int i = 0; i <= phases[0].DurationInMS; i++)
                {
                    listFull.Add(100.0);
                }
                int totalTime = 0;
                double curHealth = 100.0;
                List<HealthUpdateEvent> hpEvents = log.CombatData.GetHealthUpdateEvents(AgentItem);
                foreach (HealthUpdateEvent e in log.CombatData.GetHealthUpdateEvents(AgentItem))
                {
                    int time = (int)e.Time;
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
                    curHealth = e.HPPercent;
                    listFull[time] = curHealth;
                }
                curHealth = hpEvents.Count > 0 ? hpEvents.Last().HPPercent : curHealth;
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
                    _healthUpdates.Add(hps);
                }
            }
            return _healthUpdates;
        }


        //

        public override AbstractSingleActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return new NPCSerializable(this, log, map, CombatReplay);
        }

        protected override void InitCombatReplay(ParsedLog log)
        {
            if (!log.CombatData.HasMovementData)
            {
                // no combat replay support on fight
                return;
            }
            CombatReplay = new CombatReplay();
            SetMovements(log);
            CombatReplay.PollingRate(log.FightData.FightEnd, log.FightData.GetMainTargets(log).Contains(this));
            TrimCombatReplay(log);
        }
    }
}
