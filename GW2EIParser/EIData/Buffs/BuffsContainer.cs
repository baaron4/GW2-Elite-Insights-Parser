using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.EIData
{
    public class BuffsContainer
    {
        public List<Buff> AllBuffs { get; }
        public Dictionary<long, Buff> BuffsByIds { get; private set; }
        public Dictionary<BuffNature, List<Buff>> BuffsByNature { get; private set; }
        public Dictionary<GeneralHelper.Source, List<Buff>> BuffsBySource { get; private set; }
        public Dictionary<BuffType, List<Buff>> BuffsByType { get; private set; }
        private Dictionary<string, Buff> _buffsByName;
        public Dictionary<int, List<Buff>> BuffsByCapacity { get; private set; }

        private BuffSourceFinder _buffSourceFinder;

        private readonly ulong _build;

        public BuffsContainer(ulong build)
        {
            _build = build;
            AllBuffs = new List<Buff>();
            foreach (List<Buff> buffs in Buff.AllBuffs)
            {
                AllBuffs.AddRange(buffs.Where(x => x.MaxBuild > build && build >= x.MinBuild));
            }
            Refresh();
        }

        public void AddCustomSimulatedBuff(List<Buff> buffs)
        {
            AllBuffs.AddRange(buffs);
            Refresh();
        }

        private void Refresh()
        {
            BuffsByIds = AllBuffs.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.First());
            BuffsByNature = AllBuffs.GroupBy(x => x.Nature).ToDictionary(x => x.Key, x => x.ToList());
            BuffsBySource = AllBuffs.GroupBy(x => x.Source).ToDictionary(x => x.Key, x => x.ToList());
            BuffsByType = AllBuffs.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
            _buffsByName = AllBuffs.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList().Count > 1 ? throw new InvalidOperationException("Same name present multiple times in buffs - " + x.First().Name) : x.First());
            BuffsByCapacity = AllBuffs.GroupBy(x => x.Capacity).ToDictionary(x => x.Key, x => x.ToList());
            _buffSourceFinder = GetBuffSourceFinder(_build, new HashSet<long>(BuffsByNature[BuffNature.Boon].Select(x => x.ID)));
        }

        public Buff GetBuffByName(string name)
        {
            if (_buffsByName.TryGetValue(name, out Buff buff))
            {
                return buff;
            }
            throw new InvalidOperationException("Buff " + name + " does not exist");
        }

        public AgentItem TryFindSrc(AgentItem dst, long time, long extension, ParsedLog log, long buffID)
        {
            return _buffSourceFinder.TryFindSrc(dst, time, extension, log, buffID);
        }

        // Non shareable buffs
        public List<Buff> GetRemainingBuffsList(string source)
        {
            var result = new List<Buff>();
            foreach (GeneralHelper.Source src in GeneralHelper.ProfToEnum(source))
            {
                if (BuffsBySource.TryGetValue(src, out List<Buff> list))
                {
                    result.AddRange(list.Where(x => x.Nature == BuffNature.GraphOnlyBuff));
                }
            }
            return result;
        }
    }
}
