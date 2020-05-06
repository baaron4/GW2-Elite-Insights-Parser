using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.EIData
{
    public class BuffsContainer
    {
        public Dictionary<long, Buff> BuffsByIds { get; }
        public Dictionary<BuffNature, List<Buff>> BuffsByNature { get; }
        public Dictionary<GeneralHelper.Source, List<Buff>> BuffsBySource { get; }
        public Dictionary<BuffType, List<Buff>> BuffsByType { get; }
        private readonly Dictionary<string, Buff> _buffsByName;
        public Dictionary<int, List<Buff>> BuffsByCapacity { get; }

        private readonly BuffSourceFinder _buffSourceFinder;


        public BuffsContainer(ulong build, CombatData combatData, OperationController operation)
        {
            var currentBuffs = new List<Buff>();
            foreach (List<Buff> buffs in AllBuffs)
            {
                currentBuffs.AddRange(buffs.Where(x => x.MaxBuild > build && build >= x.MinBuild));
            }
            _buffsByName = currentBuffs.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList().Count > 1 ? throw new InvalidOperationException("Same name present multiple times in buffs - " + x.First().Name) : x.First());
            // TODO: add unknown consumables here if any
            // var buffIDs = new HashSet<long>(currentBuffs.Select(x => x.ID));
            //
            BuffsByIds = currentBuffs.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.First());
            var buffFormulaSolver = new BuffFormulaSolver(combatData, BuffsByIds);
#if DEBUG
            foreach (Buff buff in currentBuffs)
            {
                BuffDataEvent buffDataEvt = combatData.GetBuffDataEvent(buff.ID);
                if (buffDataEvt != null)
                {
                    foreach (BuffDataEvent.BuffFormula formula in buffDataEvt.FormulaList)
                    {
                        if (formula.Attr1 == ParseEnum.BuffAttribute.Unknown)
                        {
                            operation.UpdateProgressWithCancellationCheck("Unknown Formula 1 " + formula.ByteAttr1 + " for " + buff.ID + " " + buff.Name + " " + formula.Param1 + " " + formula.Param2 + " " + formula.Param3 + " " + formula.Type + " " + formula.TraitSelf + " " + formula.TraitSrc);
                        }
                    }
                }
            }
#endif
            BuffsByNature = currentBuffs.GroupBy(x => x.Nature).ToDictionary(x => x.Key, x => x.ToList());
            BuffsByCapacity = currentBuffs.GroupBy(x => x.Capacity).ToDictionary(x => x.Key, x => x.ToList());
            BuffsByType = currentBuffs.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
            BuffsBySource = currentBuffs.GroupBy(x => x.Source).ToDictionary(x => x.Key, x => x.ToList());
            //
            _buffSourceFinder = GetBuffSourceFinder(build, new HashSet<long>(BuffsByNature[BuffNature.Boon].Select(x => x.ID)));
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
