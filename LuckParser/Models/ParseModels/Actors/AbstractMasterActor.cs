using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractMasterActor : AbstractActor
    {
        public class ExtraBoonData
        {
            public int HitCount { get; }
            public int TotalHitCount { get; }
            public int DamageGain { get; }
            public int TotalDamage { get; }
            public bool Multiplier { get; }
            public ExtraBoonData(int hitCount, int totalHitCount, int damageGain, int totalDamage, bool multiplier)
            {
                HitCount = hitCount;
                TotalHitCount = totalHitCount;
                DamageGain = damageGain;
                TotalDamage = totalDamage;
                Multiplier = multiplier;
            }
        };
        // Boons
        private readonly List<BoonDistribution> _boonDistribution = new List<BoonDistribution>();
        private readonly List<Dictionary<long, long>> _boonPresence = new List<Dictionary<long, long>>();
        private readonly List<Dictionary<long, long>> _condiPresence = new List<Dictionary<long, long>>();
        private readonly List<Dictionary<AgentItem, Dictionary<long, List<long>>>> _condiCleanse = new List<Dictionary<AgentItem, Dictionary<long, List<long>>>>();
        private readonly Dictionary<long, List<ExtraBoonData>> _boonExtra = new Dictionary<long, List<ExtraBoonData>>();
        private readonly Dictionary<Target, Dictionary<long, List<ExtraBoonData>>> _boonTargetExtra = new Dictionary<Target, Dictionary<long, List<ExtraBoonData>>>();
        // damage list
        private Dictionary<int, List<int>> _damageList1S = new Dictionary<int, List<int>>();
        // Minions
        private Dictionary<string, Minions> _minions = new Dictionary<string, Minions>();
        // Replay
        public CombatReplay CombatReplay { get; protected set; }
        // Statistics
        private List<Statistics.FinalDPS> _dpsAll;

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
            List<DamageLog> damageLogs = GetDamageLogs(target, log, phase.Start, phase.End);
            // fill the graph, full precision
            List<int> dmgListFull = new List<int>();
            for (int i = 0; i <= phase.GetDuration(); i++)
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
            for (; totalTime <= phase.GetDuration(); totalTime++)
            {
                dmgListFull[totalTime] = totalDamage;
            }
            //
            dmgList.Add(0);
            for (int i = 1; i <= phase.GetDuration("s"); i++)
            {
                dmgList.Add(dmgListFull[1000 * i]);
            }
            if (phase.GetDuration("s") * 1000 != phase.GetDuration())
            {
                int lastDamage = dmgListFull[(int)phase.GetDuration()];
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

        public Dictionary<long, long> GetBoonPresence(ParsedLog log, int phaseIndex)
        {
            if (BoonPoints == null)
            {
                SetBoonStatus(log);
            }
            return _boonPresence[phaseIndex];
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

        public Dictionary<long, List<ExtraBoonData>> GetExtraBoonData(ParsedLog log, Target target)
        {
            if (BoonPoints == null)
            {
                SetBoonStatus(log);
            }
            if (target != null)
            {
                if (_boonTargetExtra.TryGetValue(target, out var res))
                {
                    return res;
                }
                else
                {
                    return new Dictionary<long, List<ExtraBoonData>>();
                }
            }
            return _boonExtra;
        }

        public Dictionary<long, long> GetCondiPresence(ParsedLog log, int phaseIndex)
        {
            if (BoonPoints == null)
            {
                SetBoonStatus(log);
            }
            return _condiPresence[phaseIndex];
        }

        public Statistics.FinalDPS GetDPSAll(ParsedLog log, int phaseIndex)
        {
            if (_dpsAll == null)
            {
                _dpsAll = new List<Statistics.FinalDPS>();
                foreach (PhaseData phase in log.FightData.GetPhases(log))
                {
                    _dpsAll.Add(GetFinalDPS(log, phase, null));
                }
            }
            return _dpsAll[phaseIndex];
        }

        public List<Statistics.FinalDPS> GetDPSAll(ParsedLog log)
        {
            if (_dpsAll == null)
            {
                _dpsAll = new List<Statistics.FinalDPS>();
                foreach (PhaseData phase in log.FightData.GetPhases(log))
                {
                    _dpsAll.Add(GetFinalDPS(log, phase, null));
                }
            }
            return _dpsAll;
        }

        protected Statistics.FinalDPS GetFinalDPS(ParsedLog log, PhaseData phase, Target target)
        {
            double phaseDuration = (phase.GetDuration()) / 1000.0;
            int damage;
            double dps = 0.0;
            Statistics.FinalDPS final = new Statistics.FinalDPS();
            //DPS
            damage = GetDamageLogs(target, log,
                    phase.Start, phase.End).Sum(x => x.Damage);

            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.Dps = (int)Math.Round(dps);
            final.Damage = damage;
            //Condi DPS
            damage = GetDamageLogs(target, log,
                    phase.Start, phase.End).Sum(x => x.IsCondi ? x.Damage : 0);

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


        public List<DamageLog> GetJustPlayerDamageLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            return GetDamageLogs(target, log, start, end).Where(x => x.SrcInstId == AgentItem.InstID).ToList();
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

        protected override void SetExtraBoonStatusData(ParsedLog log)
        {
            HashSet<long> extraDataID = new HashSet<long>
            {
                50421,
                31803
            };
            List<PhaseData> phases = log.FightData.GetPhases(log);
            foreach (long boonid in BoonPoints.Keys)
            {
                if (extraDataID.Contains(boonid))
                {
                    BoonsGraphModel graph = BoonPoints[boonid]; switch (boonid)
                    {
                        // Frost Spirit
                        case 50421:
                            foreach (Target target in log.FightData.Logic.Targets)
                            {
                                if (!_boonTargetExtra.TryGetValue(target, out var extra))
                                {
                                    _boonTargetExtra[target] = new Dictionary<long, List<ExtraBoonData>>();
                                }
                                Dictionary<long, List<ExtraBoonData>> dict = _boonTargetExtra[target];
                                if (!dict.TryGetValue(boonid, out var list))
                                {
                                    List<ExtraBoonData> extraDataList = new List<ExtraBoonData>();
                                    for (int i = 0; i < phases.Count; i++)
                                    {
                                        List<DamageLog> dmLogs = GetJustPlayerDamageLogs(target, log, phases[i].Start, phases[i].End);
                                        int totalDamage = dmLogs.Sum(x => x.Damage);
                                        List<DamageLog> effect = dmLogs.Where(x => graph.GetStackCount(x.Time) > 0 && !x.IsIndirectDamage).ToList();
                                        int damage = (int)Math.Round(effect.Sum(x => x.Damage) / 21.0);
                                        extraDataList.Add(new ExtraBoonData(effect.Count, dmLogs.Count(x => !x.IsIndirectDamage), damage, totalDamage, true));
                                    }
                                    dict[boonid] = extraDataList;
                                }
                            }
                            _boonExtra[boonid] = new List<ExtraBoonData>();
                            for (int i = 0; i < phases.Count; i++)
                            {
                                List<DamageLog> dmLogs = GetJustPlayerDamageLogs(null, log, phases[i].Start, phases[i].End);
                                int totalDamage = dmLogs.Sum(x => x.Damage);
                                List<DamageLog> effect = dmLogs.Where(x => graph.GetStackCount(x.Time) > 0 && !x.IsIndirectDamage).ToList();
                                int damage = (int)Math.Round(effect.Sum(x => x.Damage) / 21.0);
                                _boonExtra[boonid].Add(new ExtraBoonData(effect.Count, dmLogs.Count(x => !x.IsIndirectDamage), damage, totalDamage, true));
                            }
                            break;
                        // GoE
                        case 31803:
                            foreach (Target target in log.FightData.Logic.Targets)
                            {
                                if (!_boonTargetExtra.TryGetValue(target, out var extra))
                                {
                                    _boonTargetExtra[target] = new Dictionary<long, List<ExtraBoonData>>();
                                }
                                Dictionary<long, List<ExtraBoonData>> dict = _boonTargetExtra[target];
                                if (!dict.TryGetValue(boonid, out var list))
                                {
                                    List<ExtraBoonData> extraDataList = new List<ExtraBoonData>();
                                    for (int i = 0; i < phases.Count; i++)
                                    {
                                        List<DamageLog> dmLogs = GetJustPlayerDamageLogs(target, log, phases[i].Start, phases[i].End);
                                        List<DamageLog> effect = dmLogs.Where(x => graph.GetStackCount(x.Time) > 0 && !x.IsIndirectDamage).ToList();
                                        int damage = effect.Sum(x => x.Damage);
                                        extraDataList.Add(new ExtraBoonData(effect.Count, dmLogs.Count(x => !x.IsIndirectDamage), damage, 0, false));
                                    }
                                    dict[boonid] = extraDataList;
                                }

                            }
                            _boonExtra[boonid] = new List<ExtraBoonData>();
                            for (int i = 0; i < phases.Count; i++)
                            {
                                List<DamageLog> dmLogs = GetJustPlayerDamageLogs(null, log, phases[i].Start, phases[i].End);
                                List<DamageLog> effect = dmLogs.Where(x => graph.GetStackCount(x.Time) > 0 && !x.IsIndirectDamage).ToList();
                                int damage = effect.Sum(x => x.Damage);
                                _boonExtra[boonid].Add(new ExtraBoonData(effect.Count, dmLogs.Count(x => !x.IsIndirectDamage), damage, 0, false));
                            }
                            break;
                    }
                }
            }
        }

        protected override void InitBoonStatusData(ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                _boonDistribution.Add(new BoonDistribution());
                _boonPresence.Add(new Dictionary<long, long>());
                _condiPresence.Add(new Dictionary<long, long>());
                _condiCleanse.Add(new Dictionary<AgentItem, Dictionary<long, List<long>>>());
            }
        }

        protected override void SetExtraBoonStatusGenerationData(ParsedLog log, BoonSimulator simulator, long boonid, bool updateCondiPresence)
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

        protected override void SetBoonStatusGenerationData(ParsedLog log, BoonSimulationItem simul, long boonid, bool updateBoonPresence, bool updateCondiPresence)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (updateBoonPresence)
                    Add(_boonPresence[i], boonid, simul.GetClampedDuration(phase.Start, phase.End));
                if (updateCondiPresence)
                    Add(_condiPresence[i], boonid, simul.GetClampedDuration(phase.Start, phase.End));
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