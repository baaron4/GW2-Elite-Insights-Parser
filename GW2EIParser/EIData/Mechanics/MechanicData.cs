using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public class MechanicData
    {
        private readonly Dictionary<Mechanic, List<MechanicEvent>> _mechanicLogs = new Dictionary<Mechanic, List<MechanicEvent>>();

        private List<HashSet<Mechanic>> _presentOnPlayerMechanics;
        private List<HashSet<Mechanic>> _presentOnEnemyMechanics;
        private List<HashSet<Mechanic>> _presentMechanics;
        private List<List<AbstractActor>> _enemyList;

        public MechanicData(List<Mechanic> fightMechanics)
        {
            foreach (Mechanic m in fightMechanics)
            {
                _mechanicLogs.Add(m, new List<MechanicEvent>());
            }
        }

        private void CheckMechanics(ParsedLog log)
        {
            var regroupedMobs = new Dictionary<ushort, AbstractActor>();
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

        private void ProcessMechanics(ParsedLog log)
        {
            if (_presentMechanics != null)
            {
                return;
            }
            CheckMechanics(log);
            _presentOnPlayerMechanics = new List<HashSet<Mechanic>>();
            _presentOnEnemyMechanics = new List<HashSet<Mechanic>>();
            _presentMechanics = new List<HashSet<Mechanic>>();
            _enemyList = new List<List<AbstractActor>>();
            // ready present mechanics
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                var toAddPlayer = new HashSet<Mechanic>();
                var toAddEnemy = new HashSet<Mechanic>();
                var toAddAll = new HashSet<Mechanic>();
                _presentOnPlayerMechanics.Add(toAddPlayer);
                _presentOnEnemyMechanics.Add(toAddEnemy);
                _presentMechanics.Add(toAddAll);
                foreach (KeyValuePair<Mechanic, List<MechanicEvent>> pair in _mechanicLogs)
                {
                    if (pair.Value.Any(x => phase.InInterval(x.Time)))
                    {
                        toAddAll.Add(pair.Key);
                        if (pair.Key.IsEnemyMechanic)
                        {
                            toAddEnemy.Add(pair.Key);
                        }
                        else if (pair.Key.ShowOnTable)
                        {
                            toAddPlayer.Add(pair.Key);
                        }
                    }
                }
                // ready enemy list
                var toAdd = new List<AbstractActor>();
                _enemyList.Add(toAdd);
                foreach (Mechanic m in _mechanicLogs.Keys.Where(x => x.IsEnemyMechanic))
                {
                    foreach (AbstractActor p in _mechanicLogs[m].Where(x => phase.InInterval(x.Time)).Select(x => x.Actor).Distinct())
                    {
                        if (toAdd.FirstOrDefault(x => x.InstID == p.InstID) == null)
                        {
                            toAdd.Add(p);
                        }
                    }
                }
            }
            var emptyMechanic = _mechanicLogs.Where(pair => pair.Value.Count == 0).Select(pair => pair.Key).ToList();
            foreach (Mechanic m in emptyMechanic)
            {
                _mechanicLogs.Remove(m);
            }
        }

        public Dictionary<Mechanic, List<MechanicEvent>>.ValueCollection GetAllMechanics(ParsedLog log)
        {
            ProcessMechanics(log);
            return _mechanicLogs.Values;
        }

        public List<MechanicEvent> GetMechanicLogs(ParsedLog log, Mechanic mech)
        {
            ProcessMechanics(log);
            if (_mechanicLogs.TryGetValue(mech, out List<MechanicEvent> list))
            {
                return list;
            }
            return new List<MechanicEvent>();
        }

        public List<MechanicEvent> GetMechanicLogs(ParsedLog log, long id)
        {
            ProcessMechanics(log);
            Mechanic mech = _mechanicLogs.Keys.FirstOrDefault(x => x.SkillId == id);
            if (mech != null)
            {
                return _mechanicLogs[mech];
            }
            return new List<MechanicEvent>();
        }

        public HashSet<Mechanic> GetPresentEnemyMechs(ParsedLog log, int phaseIndex)
        {
            ProcessMechanics(log);
            return _presentOnEnemyMechanics[phaseIndex];
        }
        public HashSet<Mechanic> GetPresentPlayerMechs(ParsedLog log, int phaseIndex)
        {
            ProcessMechanics(log);
            return _presentOnPlayerMechanics[phaseIndex];
        }
        public HashSet<Mechanic> GetPresentMechanics(ParsedLog log, int phaseIndex)
        {
            ProcessMechanics(log);
            return _presentMechanics[phaseIndex];
        }

        public List<AbstractActor> GetEnemyList(ParsedLog log, int phaseIndex)
        {
            ProcessMechanics(log);
            return _enemyList[phaseIndex];
        }
    }
}
