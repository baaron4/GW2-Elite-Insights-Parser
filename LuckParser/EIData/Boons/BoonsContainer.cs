using LuckParser.Parser;
using LuckParser.Parser.ParsedData;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.EIData.Boon;

namespace LuckParser.EIData
{
    public class BoonsContainer
    {

        public Dictionary<long, Boon> BoonsByIds { get; }
        public Dictionary<BoonNature, List<Boon>> BoonsByNature { get; }
        public Dictionary<BoonSource, List<Boon>> BoonsBySource { get; }
        public Dictionary<BoonType, List<Boon>> BoonsByType { get; }
        private readonly Dictionary<string, Boon> _boonsByName;
        public Dictionary<int, List<Boon>> BoonsByCapacity { get; }

        private BoonSourceFinder _boonSourceFinder;

        public BoonsContainer(ulong build)
        {
            List<Boon> currentBoons = new List<Boon>();
            foreach (List<Boon> boons in AllBoons)
            {
                currentBoons.AddRange(boons.Where(x => x.MaxBuild > build && build >= x.MinBuild));
            }
            BoonsByIds = currentBoons.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.First());
            BoonsByNature = currentBoons.GroupBy(x => x.Nature).ToDictionary(x => x.Key, x => x.ToList());
            BoonsBySource = currentBoons.GroupBy(x => x.Source).ToDictionary(x => x.Key, x => x.ToList());
            BoonsByType = currentBoons.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
            _boonsByName = currentBoons.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList().Count > 1 ? throw new InvalidOperationException(x.First().Name) : x.First());
            BoonsByCapacity = currentBoons.GroupBy(x => x.Capacity).ToDictionary(x => x.Key, x => x.ToList());
            _boonSourceFinder = GetBoonSourceFinder(build, new HashSet<long>(BoonsByNature[BoonNature.Boon].Select(x => x.ID)));
        }

        public Boon GetBoonByName(string name)
        {
            if (_boonsByName.TryGetValue(name, out Boon buff))
            {
                return buff;
            }
            throw new InvalidOperationException("Buff " + name + " does not exist");
        }

        public AgentItem TryFindSrc(AgentItem dst, long time, long extension, ParsedLog log, long buffID)
        {
            return _boonSourceFinder.TryFindSrc(dst, time, extension, log, buffID);
        }

        // Non shareable buffs
        private List<Boon> GetRemainingBuffsList(BoonSource source)
        {
            return BoonsBySource[source].Where(x => x.Nature == BoonNature.GraphOnlyBuff).ToList();
        }
        public List<Boon> GetRemainingBuffsList(string source)
        {
            return GetRemainingBuffsList(ProfToEnum(source));
        }
    }
}
