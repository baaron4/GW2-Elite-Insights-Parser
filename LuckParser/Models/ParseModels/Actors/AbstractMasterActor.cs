using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static LuckParser.Models.Statistics;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractMasterActor : AbstractActor
    {
        public bool IsFakeActor { get; protected set; }
        // Boons
        private readonly List<BoonDistribution> _boonDistribution = new List<BoonDistribution>();
        private readonly List<Dictionary<long, long>> _buffPresence = new List<Dictionary<long, long>>();
        private readonly List<Dictionary<AgentItem, Dictionary<long, List<long>>>> _condiCleanse = new List<Dictionary<AgentItem, Dictionary<long, List<long>>>>();
        // damage list
        private Dictionary<int, List<int>> _damageList1S = new Dictionary<int, List<int>>();
        private Dictionary<PhaseData, Dictionary<AbstractActor, List<DamageLog>>> _selfDamageLogsPerPhasePerTarget = new Dictionary<PhaseData, Dictionary<AbstractActor, List<DamageLog>>>();
        // Minions
        private Dictionary<string, Minions> _minions;
        // Replay
        public CombatReplay CombatReplay { get; protected set; }
        // Statistics
        private List<FinalDPS> _dpsAll;

        protected AbstractMasterActor(AgentItem agent) : base(agent)
        {

        }

        public Dictionary<string, Minions> GetMinions(ParsedLog log)
        {
            if (_minions == null)
            {
                SetMinions(log);
            }
            return _minions;
        }

        public List<int> Get1SDamageList(ParsedLog log, int phaseIndex, PhaseData phase, AbstractActor target)
        {
            ulong targetId = target != null ? target.Agent : 0;
            int id = (phaseIndex + "_" + targetId + "_1S").GetHashCode();
            if (_damageList1S.TryGetValue(id, out List<int> res))
            {
                return res;
            }
            List<int> dmgList = new List<int>();
            List<DamageLog> damageLogs = GetDamageLogs(target, log, phase);
            // fill the graph, full precision
            List<int> dmgListFull = new List<int>();
            for (int i = 0; i <= phase.DurationInMS; i++)
            {
                dmgListFull.Add(0);
            }
            int totalTime = 1;
            int totalDamage = 0;
            foreach (DamageLog dl in damageLogs)
            {
                int time = (int)(dl.Time - phase.Start);
                // fill
                for (; totalTime < time; totalTime++)
                {
                    dmgListFull[totalTime] = totalDamage;
                }
                totalDamage += dl.Damage;
                dmgListFull[totalTime] = totalDamage;
            }
            // fill
            for (; totalTime <= phase.DurationInMS; totalTime++)
            {
                dmgListFull[totalTime] = totalDamage;
            }
            //
            dmgList.Add(0);
            for (int i = 1; i <= phase.DurationInS; i++)
            {
                dmgList.Add(dmgListFull[1000 * i]);
            }
            if (phase.DurationInS * 1000 != phase.DurationInMS)
            {
                int lastDamage = dmgListFull[(int)phase.DurationInMS];
                dmgList.Add(lastDamage);
            }
            _damageList1S[id] = dmgList;
            return dmgList;
        }

        public BoonDistribution GetBoonDistribution(ParsedLog log, int phaseIndex)
        {
            if (BoonPoints == null)
            {
                SetBoonStatus(log);
            }
            return _boonDistribution[phaseIndex];
        }

        public Dictionary<long, long> GetBuffPresence(ParsedLog log, int phaseIndex)
        {
            if (BoonPoints == null)
            {
                SetBoonStatus(log);
            }
            return _buffPresence[phaseIndex];
        }

        protected Dictionary<long, List<long>> GetCondiCleanse(ParsedLog log, int phaseIndex, AgentItem src)
        {
            if (BoonPoints == null)
            {
                SetBoonStatus(log);
            }
            if (_condiCleanse[phaseIndex].TryGetValue(src, out Dictionary<long, List<long>> dict))
            {
                return dict;
            }
            return new Dictionary<long, List<long>>();
        }

        public FinalDPS GetDPSAll(ParsedLog log, int phaseIndex)
        {
            if (_dpsAll == null)
            {
                _dpsAll = new List<FinalDPS>();
                foreach (PhaseData phase in log.FightData.GetPhases(log))
                {
                    _dpsAll.Add(GetFinalDPS(log, phase, null));
                }
            }
            return _dpsAll[phaseIndex];
        }

        public List<FinalDPS> GetDPSAll(ParsedLog log)
        {
            if (_dpsAll == null)
            {
                _dpsAll = new List<FinalDPS>();
                foreach (PhaseData phase in log.FightData.GetPhases(log))
                {
                    _dpsAll.Add(GetFinalDPS(log, phase, null));
                }
            }
            return _dpsAll;
        }

        protected FinalDPS GetFinalDPS(ParsedLog log, PhaseData phase, Target target)
        {
            double phaseDuration = (phase.DurationInMS) / 1000.0;
            int damage;
            double dps = 0.0;
            FinalDPS final = new FinalDPS();
            //DPS
            damage = GetDamageLogs(target, log, phase).Sum(x => x.Damage);

            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.Dps = (int)Math.Round(dps);
            final.Damage = damage;
            //Condi DPS
            damage = GetDamageLogs(target, log, phase).Sum(x => x.IsCondi ? x.Damage : 0);

            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.CondiDps = (int)Math.Round(dps);
            final.CondiDamage = damage;
            //Power DPS
            damage = final.Damage - final.CondiDamage;
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.PowerDps = (int)Math.Round(dps);
            final.PowerDamage = damage;
            // Actor DPS
            damage = GetJustPlayerDamageLogs(target, log, phase).Sum(x => x.Damage);

            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.ActorDps = (int)Math.Round(dps);
            final.ActorDamage = damage;
            //Actor Condi DPS
            damage = GetJustPlayerDamageLogs(target, log, phase).Sum(x => x.IsCondi ? x.Damage : 0);

            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.ActorCondiDps = (int)Math.Round(dps);
            final.ActorCondiDamage = damage;
            //Actor Power DPS
            damage = final.ActorDamage - final.ActorCondiDamage;
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.ActorPowerDps = (int)Math.Round(dps);
            final.ActorPowerDamage = damage;
            return final;
        }

        public void InitCombatReplay(ParsedLog log, int pollingRate, bool trim, bool forceInterpolate)
        {
            if (!log.FightData.Logic.CanCombatReplay)
            {
                // no combat replay support on fight
                return;
            }
            if (CombatReplay == null)
            {
                CombatReplay = new CombatReplay();
                SetMovements(log);
                CombatReplay.PollingRate(pollingRate, log.FightData.FightDuration, forceInterpolate);
                if (trim)
                {
                    CombatItem despawnCheck = log.CombatData.GetStatesData(InstID, ParseEnum.StateChange.Despawn, FirstAware, LastAware).LastOrDefault();
                    CombatItem spawnCheck = log.CombatData.GetStatesData(InstID, ParseEnum.StateChange.Spawn, FirstAware, LastAware).LastOrDefault();
                    CombatItem deathCheck = log.CombatData.GetStatesData(InstID, ParseEnum.StateChange.ChangeDead, FirstAware, LastAware).LastOrDefault();
                    if (deathCheck != null)
                    {
                        CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAware), log.FightData.ToFightSpace(deathCheck.Time));
                    }
                    else if (despawnCheck != null && (spawnCheck == null || spawnCheck.Time < despawnCheck.Time))
                    {
                        CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAware), log.FightData.ToFightSpace(despawnCheck.Time));
                    }
                    else
                    {
                        CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAware), log.FightData.ToFightSpace(AgentItem.LastAware));
                    }
                }
                //SetAdditionalCombatReplayData(log);
            }
        }

        public void ComputeAdditionalCombatReplayData(ParsedLog log)
        {
            if (CombatReplay != null && CombatReplay.Actors.Count == 0)
            {
                SetAdditionalCombatReplayData(log);
            }
        }

        public List<DamageLog> GetJustPlayerDamageLogs(AbstractActor target, ParsedLog log, PhaseData phase)
        {
            if (!_selfDamageLogsPerPhasePerTarget.TryGetValue(phase, out Dictionary<AbstractActor, List<DamageLog>> targetDict))
            {
                targetDict = new Dictionary<AbstractActor, List<DamageLog>>();
                _selfDamageLogsPerPhasePerTarget[phase] = targetDict;
            }
            if (!targetDict.TryGetValue(target??GeneralHelper.NullActor, out List<DamageLog> dls))
            {
                dls = GetDamageLogs(target, log, phase).Where(x => x.SrcInstId == InstID).ToList();
                targetDict[target ?? GeneralHelper.NullActor] = dls;
            }
            return dls;
        }

        // private setters
        private void SetMovements(ParsedLog log)
        {
            foreach (CombatItem c in log.CombatData.GetMovementData(InstID, FirstAware, LastAware))
            {
                long time = log.FightData.ToFightSpace(c.Time);
                byte[] xy = BitConverter.GetBytes(c.DstAgent);
                float x = BitConverter.ToSingle(xy, 0);
                float y = BitConverter.ToSingle(xy, 4);
                if (c.IsStateChange == ParseEnum.StateChange.Position)
                {
                    CombatReplay.Positions.Add(new Point3D(x, y, c.Value, time));
                }
                else if (c.IsStateChange == ParseEnum.StateChange.Velocity)
                {
                    CombatReplay.Velocities.Add(new Point3D(x, y, c.Value, time));
                }
                else if (c.IsStateChange == ParseEnum.StateChange.Rotation)
                {
                    CombatReplay.Rotations.Add(new Point3D(x, y, c.Value, time));
                }
            }
        }

        protected override void InitBoonStatusData(ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                _boonDistribution.Add(new BoonDistribution());
                _buffPresence.Add(new Dictionary<long, long>());
                _condiCleanse.Add(new Dictionary<AgentItem, Dictionary<long, List<long>>>());
            }
        }

        protected override void SetBoonStatusCleanseWasteData(ParsedLog log, BoonSimulator simulator, long boonid, bool updateCondiPresence)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            List<AbstractBoonSimulationItem> extraSimulations = new List<AbstractBoonSimulationItem>(simulator.OverstackSimulationResult);
            extraSimulations.AddRange(simulator.WasteSimulationResult);
            foreach (AbstractBoonSimulationItem simul in extraSimulations)
            {
                for (int i = 0; i < phases.Count; i++)
                {
                    PhaseData phase = phases[i];
                    simul.SetBoonDistributionItem(_boonDistribution[i], phase.Start, phase.End, boonid, log);
                }
            }

            if (updateCondiPresence)
            {
                foreach (BoonSimulationItemCleanse simul in simulator.CleanseSimulationResult)
                {
                    for (int i = 0; i < phases.Count; i++)
                    {
                        PhaseData phase = phases[i];
                        simul.SetCleanseItem(_condiCleanse[i], phase.Start, phase.End, boonid, log);
                    }
                }
            }
        }

        protected override void SetBoonStatusGenerationData(ParsedLog log, BoonSimulationItem simul, long boonid)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            Boon boon = Boon.BoonsByIds[boonid];
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                Add(_buffPresence[i], boonid, simul.GetClampedDuration(phase.Start, phase.End));
                simul.SetBoonDistributionItem(_boonDistribution[i], phase.Start, phase.End, boonid, log);
            }
        }

        private void SetMinions(ParsedLog log)
        {
            _minions = new Dictionary<string, Minions>();
            List<AgentItem> combatMinion = log.AgentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.MasterAgent == AgentItem.Agent).ToList();
            Dictionary<string, Minions> auxMinions = new Dictionary<string, Minions>();
            foreach (AgentItem agent in combatMinion)
            {
                string id = agent.Name;
                if (!auxMinions.ContainsKey(id))
                {
                    auxMinions[id] = new Minions(id.GetHashCode());
                }
                auxMinions[id].Add(new Minion(agent));
            }
            foreach (KeyValuePair<string, Minions> pair in auxMinions)
            {
                if (pair.Value.GetDamageLogs(null, log, log.FightData.ToFightSpace(FirstAware), log.FightData.ToFightSpace(LastAware)).Count > 0 || pair.Value.GetCastLogs(log, log.FightData.ToFightSpace(FirstAware), log.FightData.ToFightSpace(LastAware)).Count > 0)
                {
                    _minions[pair.Key] = pair.Value;
                }
            }
        }
        protected override void SetDamageLogs(ParsedLog log)
        {
            foreach (CombatItem c in log.CombatData.GetDamageData(InstID, FirstAware, LastAware))
            {
                long time = log.FightData.ToFightSpace(c.Time);
                AddDamageLog(time, c);
            }
            Dictionary<string, Minions> minionsList = GetMinions(log);
            foreach (Minions mins in minionsList.Values)
            {
                DamageLogs.AddRange(mins.GetDamageLogs(null, log, log.FightData.ToFightSpace(FirstAware), log.FightData.ToFightSpace(LastAware)));
            }
            DamageLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
        }

        public int GetCombatReplayID()
        {
            return (InstID + "_" + CombatReplay.TimeOffsets.start + "_" + CombatReplay.TimeOffsets.end).GetHashCode();
        }

        // abstracts
        protected abstract void SetAdditionalCombatReplayData(ParsedLog log);


        public abstract class AbstractMasterActorSerializable
        {
            public string Img { get; set; }
            public string Type { get; set; }
            public int ID { get; set; }
            public double[] Positions { get; set; }
        }

        public abstract AbstractMasterActorSerializable GetCombatReplayJSON(CombatReplayMap map);
    }
}