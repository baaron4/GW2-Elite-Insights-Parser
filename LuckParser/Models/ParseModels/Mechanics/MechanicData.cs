using LuckParser.Parser;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class MechanicData
    {
        private readonly Dictionary<Mechanic, List<MechanicLog>> _mechanicLogs = new Dictionary<Mechanic, List<MechanicLog>>();

        private List<HashSet<Mechanic>> _presentOnPlayerMechanics;
        private List<HashSet<Mechanic>> _presentOnEnemyMechanics;
        private List<HashSet<Mechanic>> _presentMechanics;
        private List<List<DummyActor>> _enemyList;

        public MechanicData(List<Mechanic> fightMechanics, ParsedEvtcContainer evtcContainer)
        {
            foreach(Mechanic m in fightMechanics)
            {
                _mechanicLogs.Add(m, new List<MechanicLog>());
            }
            CombatData combatData = evtcContainer.CombatData;
            HashSet<ushort> playersIds = evtcContainer.PlayerIDs;
            Dictionary<ushort, DummyActor> regroupedMobs = new Dictionary<ushort, DummyActor>();
            foreach (Mechanic mech in fightMechanics)
            {
                mech.CheckMechanic(evtcContainer, _mechanicLogs, regroupedMobs);
            }
            // regroup same mechanics with diff ids
            Dictionary<string, Mechanic> altNames = new Dictionary<string, Mechanic>();
            List<Mechanic> toRemove = new List<Mechanic>();
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
            _presentOnPlayerMechanics = new List<HashSet<Mechanic>>();
            _presentOnEnemyMechanics = new List<HashSet<Mechanic>>();
            _presentMechanics = new List<HashSet<Mechanic>>();
            _enemyList = new List<List<DummyActor>>();
            // ready present mechanics
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                HashSet<Mechanic> toAddPlayer = new HashSet<Mechanic>();
                HashSet<Mechanic> toAddEnemy = new HashSet<Mechanic>();
                HashSet<Mechanic> toAddAll = new HashSet<Mechanic>();
                _presentOnPlayerMechanics.Add(toAddPlayer);
                _presentOnEnemyMechanics.Add(toAddEnemy);
                _presentMechanics.Add(toAddAll);
                foreach (KeyValuePair<Mechanic, List<MechanicLog>> pair in _mechanicLogs)
                {
                    if (pair.Value.Count(x => phase.InInterval(x.Time)) > 0)
                    {
                        toAddAll.Add(pair.Key);
                        if (pair.Key.IsEnemyMechanic)
                        {
                            toAddEnemy.Add(pair.Key);
                        } else if (pair.Key.ShowOnTable)
                        {
                            toAddPlayer.Add(pair.Key);
                        }
                    }
                }
                // ready enemy list
                List<DummyActor> toAdd = new List<DummyActor>();
                _enemyList.Add(toAdd);
                foreach(Mechanic m in _mechanicLogs.Keys.Where(x=> x.IsEnemyMechanic))
                {
                    foreach (DummyActor p in _mechanicLogs[m].Where(x => phase.InInterval(x.Time)).Select(x => x.Actor).Distinct())
                    {
                        if (toAdd.FirstOrDefault(x => x.InstID == p.InstID) == null)
                        {
                            toAdd.Add(p);
                        }
                    }
                }
            }
            List<Mechanic> emptyMechanic = _mechanicLogs.Where(pair => pair.Value.Count == 0).Select(pair => pair.Key).ToList();
            foreach (Mechanic m in emptyMechanic)
            {
                _mechanicLogs.Remove(m);
            }
        }

        public Dictionary<Mechanic, List<MechanicLog>>.ValueCollection GetAllMechanics()
        {
            return _mechanicLogs.Values;
        }

        public List<MechanicLog> GetMechanicLogs(Mechanic mech)
        {
            if (_mechanicLogs.TryGetValue(mech, out var list))
            {
                return list;
            }
            return new List<MechanicLog>();
        }

        public List<MechanicLog> GetMechanicLogs(long id)
        {
            Mechanic mech = _mechanicLogs.Keys.FirstOrDefault(x => x.SkillId == id);
            if ( mech != null)
            {
                return _mechanicLogs[mech];
            }
            return new List<MechanicLog>();
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

        public List<DummyActor> GetEnemyList(ParsedLog log, int phaseIndex)
        {
            ProcessMechanics(log);
            return _enemyList[phaseIndex];
        }
    }
}
