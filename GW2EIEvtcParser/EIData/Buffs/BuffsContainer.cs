using System;
using System.Collections.Generic;
using System.Linq;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class BuffsContainer
    {

        public Dictionary<long, Buff> BuffsByIds { get; }
        public Dictionary<BuffNature, List<Buff>> BuffsByNature { get; }
        public Dictionary<ParserHelper.Source, List<Buff>> BuffsBySource { get; }
        private readonly Dictionary<string, Buff> _buffsByName;

        private readonly BuffSourceFinder _buffSourceFinder;


        internal BuffsContainer(ulong build, CombatData combatData, ParserController operation)
        {
            var AllBuffs = new List<List<Buff>>()
            {
                Boons,
                Conditions,
                Commons,
                Gear,
                Consumables,
                FightSpecific,
                FractalInstabilities,
                //
                RevenantHelper.Buffs,
                HeraldHelper.Buffs,
                RenegadeHelper.Buffs,
                //
                WarriorHelper.Buffs,
                BerserkerHelper.Buffs,
                SpellbreakerHelper.Buffs,
                //
                GuardianHelper.Buffs,
                DragonhunterHelper.Buffs,
                FirebrandHelper.Buffs,
                //
                RangerHelper.Buffs,
                DruidHelper.Buffs,
                SoulbeastHelper.Buffs,
                //
                ThiefHelper.Buffs,
                DaredevilHelper.Buffs,
                DeadeyeHelper.Buffs,
                //
                EngineerHelper.Buffs,
                ScrapperHelper.Buffs,
                HolosmithHelper.Buffs,
                //
                MesmerHelper.Buffs,
                ChronomancerHelper.Buffs,
                MirageHelper.Buffs,
                //
                NecromancerHelper.Buffs,
                ReaperHelper.Buffs,
                ScourgeHelper.Buffs,
                //
                ElementalistHelper.Buffs,
                TempestHelper.Buffs,
                WeaverHelper.Buffs,
            };
            var currentBuffs = new List<Buff>();
            foreach (List<Buff> buffs in AllBuffs)
            {
                currentBuffs.AddRange(buffs.Where(x => x.Available(build)));
            }
            _buffsByName = currentBuffs.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList().Count > 1 ? throw new InvalidOperationException("Same name present multiple times in buffs - " + x.First().Name) : x.First());
            // Unknown consumables
            var buffIDs = new HashSet<long>(currentBuffs.Select(x => x.ID));
            var foodAndUtility = new List<BuffInfoEvent>(combatData.GetBuffInfoEvent(BuffCategory.Enhancement));
            foodAndUtility.AddRange(combatData.GetBuffInfoEvent(BuffCategory.Food));
            foreach (BuffInfoEvent buffInfoEvent in foodAndUtility)
            {
                if (!buffIDs.Contains(buffInfoEvent.BuffID))
                {
                    string name = buffInfoEvent.Category == BuffCategory.Enhancement ? "Utility" : "Food";
                    string link = buffInfoEvent.Category == BuffCategory.Enhancement ? "https://wiki.guildwars2.com/images/2/23/Nourishment_utility.png" : "https://wiki.guildwars2.com/images/c/ca/Nourishment_food.png";
                    operation.UpdateProgressWithCancellationCheck("Unknown " + name + " " + buffInfoEvent.BuffID);
                    currentBuffs.Add(CreateCustomConsumable(name, buffInfoEvent.BuffID, link, buffInfoEvent.MaxStacks));
                }
            }
            //
            BuffsByIds = currentBuffs.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.First());
            BuffInfoSolver.AdjustBuffs(combatData, BuffsByIds, operation);
            foreach (Buff buff in currentBuffs)
            {
                BuffInfoEvent buffInfoEvt = combatData.GetBuffInfoEvent(buff.ID);
                if (buffInfoEvt != null)
                {
                    foreach (BuffFormula formula in buffInfoEvt.Formulas)
                    {
                        if (formula.Attr1 == BuffAttribute.Unknown)
                        {
                            operation.UpdateProgressWithCancellationCheck("Unknown Formula for " + buff.Name + ": " + formula.GetDescription(true, BuffsByIds));
                        }
                    }
                }
            }
            BuffsByNature = currentBuffs.GroupBy(x => x.Nature).ToDictionary(x => x.Key, x => x.ToList());
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

        internal AgentItem TryFindSrc(AgentItem dst, long time, long extension, ParsedEvtcLog log, long buffID)
        {
            return _buffSourceFinder.TryFindSrc(dst, time, extension, log, buffID);
        }

        // Non shareable buffs
        public List<Buff> GetRemainingBuffsList(string source)
        {
            var result = new List<Buff>();
            foreach (ParserHelper.Source src in ParserHelper.ProfToEnum(source))
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
