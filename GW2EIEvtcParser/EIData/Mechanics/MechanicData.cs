using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public class MechanicData
    {
        private readonly Dictionary<Mechanic, List<MechanicEvent>> _mechanicLogs = new Dictionary<Mechanic, List<MechanicEvent>>();

        private List<HashSet<Mechanic>> _presentOnPlayerMechanics;
        private List<HashSet<Mechanic>> _presentOnEnemyMechanics;
        private List<HashSet<Mechanic>> _presentMechanics;
        private List<List<AbstractActor>> _enemyList;

        internal MechanicData(List<Mechanic> fightMechanics)
        {
            foreach (Mechanic m in fightMechanics)
            {
                _mechanicLogs.Add(m, new List<MechanicEvent>());
            }
        }

        private void CheckMechanics(ParsedEvtcLog log)
        {
            var regroupedMobs = new Dictionary<int, AbstractSingleActor>();
            foreach (Mechanic mech in _mechanicLogs.Keys)
            {
                mech.CheckMechanic(log, _mechanicLogs, regroupedMobs);
            }
            // regroup same mechanics with diff ids
            var altNames = new Dictionary<string, Mechanic>();
            var toRemove = new List<Mechanic>();
            foreach (Mechanic mech in _mechanicLogs.Keys)
            {
                if (altNames.ContainsKey(mech.ShortName))
                {
                    _mechanicLogs[altNames[mech.ShortName]].AddRange(_mechanicLogs[mech]);
                    toRemove.Add(mech);
                }
                else
                {
                    altNames.Add(mech.ShortName, mech);
                }
            }
            foreach (Mechanic mech in toRemove)
            {
                _mechanicLogs.Remove(mech);
            }
        }

        private void ProcessMechanics(ParsedEvtcLog log)
        {
            if (_presentMechanics != null)
            {
                return;
            }
            _presentOnPlayerMechanics = new List<HashSet<Mechanic>>();
            _presentOnEnemyMechanics = new List<HashSet<Mechanic>>();
            _presentMechanics = new List<HashSet<Mechanic>>();
            _enemyList = new List<List<AbstractActor>>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                _presentOnPlayerMechanics.Add(new HashSet<Mechanic>());
                _presentOnEnemyMechanics.Add(new HashSet<Mechanic>());
                _presentMechanics.Add(new HashSet<Mechanic>());
                _enemyList.Add(new List<AbstractActor>());
            }
            CheckMechanics(log);
            // ready present mechanics
            int i = 0;
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                foreach (KeyValuePair<Mechanic, List<MechanicEvent>> pair in _mechanicLogs)
                {
                    if (pair.Value.Any(x => phase.InInterval(x.Time)))
                    {
                        _presentMechanics[i].Add(pair.Key);
                        if (pair.Key.IsEnemyMechanic)
                        {
                            _presentOnEnemyMechanics[i].Add(pair.Key);
                        }
                        else if (pair.Key.ShowOnTable)
                        {
                            _presentOnPlayerMechanics[i].Add(pair.Key);
                        }
                    }
                }
                // ready enemy list
                foreach (Mechanic m in _mechanicLogs.Keys.Where(x => x.IsEnemyMechanic))
                {
                    foreach (AbstractActor p in _mechanicLogs[m].Where(x => phase.InInterval(x.Time)).Select(x => x.Actor).Distinct())
                    {
                        if (_enemyList[i].FirstOrDefault(x => x.AgentItem == p.AgentItem) == null)
                        {
                            _enemyList[i].Add(p);
                        }
                    }
                }
                i++;
            }
            var emptyMechanic = _mechanicLogs.Where(pair => pair.Value.Count == 0).Select(pair => pair.Key).ToList();
            foreach (Mechanic m in emptyMechanic)
            {
                _mechanicLogs.Remove(m);
            }
        }

        public IReadOnlyDictionary<Mechanic, IReadOnlyList<MechanicEvent>> GetAllMechanics(ParsedEvtcLog log)
        {
            ProcessMechanics(log);
            return (IReadOnlyDictionary<Mechanic, IReadOnlyList<MechanicEvent>>)_mechanicLogs;
        }

        public IReadOnlyList<MechanicEvent> GetMechanicLogs(ParsedEvtcLog log, Mechanic mech)
        {
            ProcessMechanics(log);
            if (_mechanicLogs.TryGetValue(mech, out List<MechanicEvent> list))
            {
                return list;
            }
            return new List<MechanicEvent>();
        }

        public IReadOnlyList<MechanicEvent> GetMechanicLogs(ParsedEvtcLog log, long id)
        {
            ProcessMechanics(log);
            Mechanic mech = _mechanicLogs.Keys.FirstOrDefault(x => x.SkillId == id);
            if (mech != null)
            {
                return _mechanicLogs[mech];
            }
            return new List<MechanicEvent>();
        }

        public IReadOnlyCollection<Mechanic> GetPresentEnemyMechs(ParsedEvtcLog log, int phaseIndex)
        {
            ProcessMechanics(log);
            return _presentOnEnemyMechanics[phaseIndex];
        }
        public IReadOnlyCollection<Mechanic> GetPresentPlayerMechs(ParsedEvtcLog log, int phaseIndex)
        {
            ProcessMechanics(log);
            return _presentOnPlayerMechanics[phaseIndex];
        }
        public IReadOnlyCollection<Mechanic> GetPresentMechanics(ParsedEvtcLog log, int phaseIndex)
        {
            ProcessMechanics(log);
            return _presentMechanics[phaseIndex];
        }

        public IReadOnlyList<AbstractActor> GetEnemyList(ParsedEvtcLog log, int phaseIndex)
        {
            ProcessMechanics(log);
            return _enemyList[phaseIndex];
        }
    }
}
