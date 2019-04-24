using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.ParseModels.Boon;

namespace LuckParser.Models.ParseModels
{
    public class BoonsContainer
    {

        public static Dictionary<long, Boon> BoonsByIds = AllBoons.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.First());
        public static Dictionary<BoonNature, List<Boon>> BoonsByNature = AllBoons.GroupBy(x => x.Nature).ToDictionary(x => x.Key, x => x.ToList());
        public static Dictionary<BoonSource, List<Boon>> BoonsBySource = AllBoons.GroupBy(x => x.Source).ToDictionary(x => x.Key, x => x.ToList());
        public static Dictionary<BoonType, List<Boon>> BoonsByType = AllBoons.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
        private static Dictionary<string, Boon> _boonsByName = AllBoons.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList().Count > 1 ? throw new InvalidOperationException(x.First().Name) : x.First());
        public static Dictionary<int, List<Boon>> BoonsByCapacity = AllBoons.GroupBy(x => x.Capacity).ToDictionary(x => x.Key, x => x.ToList());

        public static Boon GetBoonByName(string name)
        {
            if (_boonsByName.TryGetValue(name, out Boon buff))
            {
                return buff;
            }
            throw new InvalidOperationException("Buff " + name + " does not exist");
        }

        // Conditions
        public List<Boon> GetCondiBoonList()
        {
            return BoonsByNature[BoonNature.Condition];
        }
        // Boons
        public List<Boon> GetBoonList()
        {
            return BoonsByNature[BoonNature.Boon];
        }
        // Shareable buffs
        public List<Boon> GetOffensiveTableList()
        {
            return BoonsByNature[BoonNature.OffensiveBuffTable];
        }
        public List<Boon> GetDefensiveTableList()
        {
            return BoonsByNature[BoonNature.DefensiveBuffTable];
        }
        // Consumables (Food and Utility)
        public List<Boon> GetConsumableList()
        {
            return BoonsByNature[BoonNature.Consumable];
        }
        // Enemy
        public List<Boon> GetEnemyBoonList()
        {
            return BoonsBySource[BoonSource.Enemy];
        }
        // All buffs
        public List<Boon> GetAllBuffList()
        {
            List<Boon> res = new List<Boon>();
            // correct order for the boon graph
            res.AddRange(BoonsByNature[BoonNature.Boon]);
            res.AddRange(BoonsByNature[BoonNature.DefensiveBuffTable]);
            res.AddRange(BoonsByNature[BoonNature.OffensiveBuffTable]);
            res.AddRange(BoonsByNature[BoonNature.GraphOnlyBuff]);
            return res;
        }
        // Non shareable buffs
        public List<Boon> GetRemainingBuffsList()
        {
            return BoonsByNature[BoonNature.GraphOnlyBuff];
        }
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
