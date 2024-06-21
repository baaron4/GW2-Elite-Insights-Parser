using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class MechanicData
    {
        private readonly Dictionary<Mechanic, List<MechanicEvent>> _mechanicLogs = new Dictionary<Mechanic, List<MechanicEvent>>();

        private CachingCollection<HashSet<Mechanic>> _presentOnFriendliesMechanics;
        private CachingCollection<HashSet<Mechanic>> _presentOnEnemyMechanics;
        private CachingCollection<HashSet<Mechanic>> _presentMechanics;
        private CachingCollection<List<AbstractSingleActor>> _enemyList;

        internal MechanicData(List<Mechanic> fightMechanics)
        {
            var errorMechanicConfig = new Dictionary<string, Dictionary<string, Dictionary<int, List<Mechanic>>>>();
            var errorMechanicNaming = new Dictionary<string, Dictionary<string, Dictionary<string, List<Mechanic>>>>();
            foreach (Mechanic m in fightMechanics.OrderBy(x => !x.IsAchievementEligibility))
            {
                {
                    if (!errorMechanicConfig.TryGetValue(m.PlotlySetting.Symbol, out Dictionary<string, Dictionary<int, List<Mechanic>>> colorDict))
                    {
                        colorDict = new Dictionary<string, Dictionary<int, List<Mechanic>>>();
                        errorMechanicConfig[m.PlotlySetting.Symbol] = colorDict;
                    }
                    if (!colorDict.TryGetValue(m.PlotlySetting.Color, out Dictionary<int, List<Mechanic>> sizeDict))
                    {
                        sizeDict = new Dictionary<int, List<Mechanic>>();
                        colorDict[m.PlotlySetting.Color] = sizeDict;
                    }
                    if (!sizeDict.TryGetValue(m.PlotlySetting.Size, out List<Mechanic> mList))
                    {
                        mList = new List<Mechanic>();
                        sizeDict[m.PlotlySetting.Size] = mList;
                    }
                    mList.Add(m);
                    if (mList.Count > 1)
                    {
                        throw new InvalidDataException(mList[0].FullName + " and " + mList[1].FullName + " share the same plotly configuration");
                    }
                }
                {
                    if (!errorMechanicNaming.TryGetValue(m.FullName, out Dictionary<string, Dictionary<string, List<Mechanic>>> shortNameDict))
                    {
                        shortNameDict = new Dictionary<string, Dictionary<string, List<Mechanic>>>();
                        errorMechanicNaming[m.FullName] = shortNameDict;
                    }
                    if (!shortNameDict.TryGetValue(m.ShortName, out Dictionary<string, List<Mechanic>> descriptionDict))
                    {
                        descriptionDict = new Dictionary<string, List<Mechanic>>();
                        shortNameDict[m.ShortName] = descriptionDict;
                    }
                    if (!descriptionDict.TryGetValue(m.Description, out List<Mechanic> mList))
                    {
                        mList = new List<Mechanic>();
                        descriptionDict[m.Description] = mList;
                    }
                    mList.Add(m);
                    if (mList.Count > 1)
                    {
                        throw new InvalidDataException(mList[0].FullName + " and " + mList[1].FullName + " share the same naming configuration");
                    }
                }
                _mechanicLogs.Add(m, new List<MechanicEvent>());
            }

        }

        private void ComputeMechanics(ParsedEvtcLog log)
        {
            var regroupedMobs = new Dictionary<int, AbstractSingleActor>();
            _mechanicLogs.Keys.Where(x => !x.Available(log)).ToList().ForEach(x => _mechanicLogs.Remove(x));
            foreach (Mechanic mech in _mechanicLogs.Keys)
            {
                mech.CheckMechanic(log, _mechanicLogs, regroupedMobs);
            }
        }

        private void ProcessMechanics(ParsedEvtcLog log)
        {
            if (_presentMechanics != null)
            {
                return;
            }
            _presentOnFriendliesMechanics = new CachingCollection<HashSet<Mechanic>>(log);
            _presentOnEnemyMechanics = new CachingCollection<HashSet<Mechanic>>(log);
            _presentMechanics = new CachingCollection<HashSet<Mechanic>>(log);
            _enemyList = new CachingCollection<List<AbstractSingleActor>>(log);
            ComputeMechanics(log);
            var emptyMechanic = _mechanicLogs.Where(pair => pair.Value.Count == 0).Select(pair => pair.Key).ToList();
            foreach (Mechanic m in emptyMechanic)
            {
                if (m.KeepIfEmpty(log))
                {
                    continue;
                }
                _mechanicLogs.Remove(m);
            }
            foreach (KeyValuePair<Mechanic, List<MechanicEvent>> pair in _mechanicLogs)
            {
                pair.Value.Sort((x, y) => x.Time.CompareTo(y.Time));
            }
        }

        /// <summary>
        /// DEPRECATED, CSV Usage only
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public Dictionary<Mechanic, List<MechanicEvent>>.ValueCollection GetAllMechanicEvents(ParsedEvtcLog log)
        {
            ProcessMechanics(log);
            return _mechanicLogs.Values;
        }

        public IReadOnlyList<MechanicEvent> GetMechanicLogs(ParsedEvtcLog log, Mechanic mech, long start, long end)
        {
            ProcessMechanics(log);
            return _mechanicLogs.TryGetValue(mech, out List<MechanicEvent> list) ? list.Where(x => x.Time >= start && x.Time <= end).ToList() : new List<MechanicEvent>();
        }

        public IReadOnlyList<MechanicEvent> GetMechanicLogs(ParsedEvtcLog log, Mechanic mech, AbstractSingleActor actor, long start, long end)
        {
            return GetMechanicLogs(log, mech, start, end).Where(x => x.Actor == actor).ToList();
        }

        private void ComputeMechanicData(ParsedEvtcLog log, long start, long end)
        {
            var presentMechanics = new HashSet<Mechanic>();
            var presentOnEnemyMechanics = new HashSet<Mechanic>();
            var presentOnFriendliesMechanics = new HashSet<Mechanic>();
            var enemyHash = new HashSet<AbstractSingleActor>();
            foreach (KeyValuePair<Mechanic, List<MechanicEvent>> pair in _mechanicLogs)
            {
                if (pair.Key.KeepIfEmpty(log) || pair.Value.Any(x => x.Time >= start && x.Time <= end))
                {
                    presentMechanics.Add(pair.Key);
                    if (pair.Key.ShowOnTable)
                    {
                        if (pair.Key.IsEnemyMechanic)
                        {
                            presentOnEnemyMechanics.Add(pair.Key);
                        }
                        else
                        {
                            presentOnFriendliesMechanics.Add(pair.Key);
                        }
                    }

                }
            }
            // ready enemy list
            foreach (Mechanic m in _mechanicLogs.Keys.Where(x => x.IsEnemyMechanic))
            {
                foreach (MechanicEvent mechanicEvent in _mechanicLogs[m].Where(x => x.Time >= start && x.Time <= end))
                {
                    enemyHash.Add(mechanicEvent.Actor);
                }
            }
            _presentMechanics.Set(start, end, presentMechanics);
            _presentOnEnemyMechanics.Set(start, end, presentOnEnemyMechanics);
            _presentOnFriendliesMechanics.Set(start, end, presentOnFriendliesMechanics);
            _enemyList.Set(start, end, new List<AbstractSingleActor>(enemyHash));
        }

        public IReadOnlyCollection<Mechanic> GetPresentEnemyMechs(ParsedEvtcLog log, long start, long end)
        {
            ProcessMechanics(log);
            if (!_presentOnEnemyMechanics.HasKeys(start, end))
            {
                ComputeMechanicData(log, start, end);
            }
            return _presentOnEnemyMechanics.Get(start, end);
        }
        public IReadOnlyCollection<Mechanic> GetPresentFriendlyMechs(ParsedEvtcLog log, long start, long end)
        {
            ProcessMechanics(log);
            if (!_presentOnFriendliesMechanics.HasKeys(start, end))
            {
                ComputeMechanicData(log, start, end);
            }
            return _presentOnFriendliesMechanics.Get(start, end);
        }
        public IReadOnlyCollection<Mechanic> GetPresentMechanics(ParsedEvtcLog log, long start, long end)
        {
            ProcessMechanics(log);
            if (!_presentMechanics.HasKeys(start, end))
            {
                ComputeMechanicData(log, start, end);
            }
            return _presentMechanics.Get(start, end);
        }

        public IReadOnlyList<AbstractSingleActor> GetEnemyList(ParsedEvtcLog log, long start, long end)
        {
            ProcessMechanics(log);
            if (!_enemyList.HasKeys(start, end))
            {
                ComputeMechanicData(log, start, end);
            }
            return _enemyList.Get(start, end);
        }
    }
}
