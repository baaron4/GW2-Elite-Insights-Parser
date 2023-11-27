using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class DamageModifiersContainer
    {

        public IReadOnlyDictionary<ParserHelper.Source, IReadOnlyList<DamageModifier>> DamageModifiersPerSource { get; }

        public IReadOnlyDictionary<string, DamageModifier> DamageModifiersByName { get; }
        public IReadOnlyDictionary<ParserHelper.Source, IReadOnlyList<DamageModifier>> IncomingDamageModifiersPerSource { get; }

        public IReadOnlyDictionary<string, DamageModifier> IncomingDamageModifiersByName { get; }

        internal DamageModifiersContainer(CombatData combatData, FightLogic.ParseMode mode, EvtcParserSettings parserSettings)
        {
            var AllDamageModifiers = new List<List<DamageModifierDescriptor>>
            {
                CommonDamageModifiers.ItemDamageModifiers,
                CommonDamageModifiers.GearDamageModifiers,
                CommonDamageModifiers.SharedDamageModifiers,
                CommonDamageModifiers.FightSpecificDamageModifiers,
                //
                RevenantHelper.DamageMods,
                HeraldHelper.DamageMods,
                RenegadeHelper.DamageMods,
                VindicatorHelper.DamageMods,
                //
                WarriorHelper.DamageMods,
                BerserkerHelper.DamageMods,
                SpellbreakerHelper.DamageMods,
                BladeswornHelper.DamageMods,
                //
                GuardianHelper.DamageMods,
                DragonhunterHelper.DamageMods,
                FirebrandHelper.DamageMods,
                WillbenderHelper.DamageMods,
                //
                EngineerHelper.DamageMods,
                ScrapperHelper.DamageMods,
                HolosmithHelper.DamageMods,
                MechanistHelper.DamageMods,
                //
                ThiefHelper.DamageMods,
                DaredevilHelper.DamageMods,
                DeadeyeHelper.DamageMods,
                SpecterHelper.DamageMods,
                //
                RangerHelper.DamageMods,
                DruidHelper.DamageMods,
                SoulbeastHelper.DamageMods,
                UntamedHelper.DamageMods,
                //
                MesmerHelper.DamageMods,
                ChronomancerHelper.DamageMods,
                MirageHelper.DamageMods,
                VirtuosoHelper.DamageMods,
                //
                NecromancerHelper.DamageMods,
                ReaperHelper.DamageMods,
                ScourgeHelper.DamageMods,
                HarbingerHelper.DamageMods,
                //
                ElementalistHelper.DamageMods,
                TempestHelper.DamageMods,
                WeaverHelper.DamageMods,
                CatalystHelper.DamageMods,
            };
            var currentDamageMods = new List<DamageModifier>();
            foreach (List<DamageModifierDescriptor> boons in AllDamageModifiers)
            {
                currentDamageMods.AddRange(boons.Where(x => x.Available(combatData) && x.Keep(mode, parserSettings)).Select(x => new OutgoingDamageModifier(x)));
            }
            DamageModifiersPerSource = currentDamageMods.GroupBy(x => x.Src).ToDictionary(x => x.Key, x => (IReadOnlyList<DamageModifier>)x.ToList());
            DamageModifiersByName = currentDamageMods.GroupBy(x => x.Name).ToDictionary(x => x.Key, x =>
            {
                var list = x.ToList();
                if (list.Count > 1)
                {
                    throw new InvalidDataException("Same name present multiple times in damage mods - " + x.First().Name);
                }
                return list.First();
            });
            //
            var AllIncomingDamageModifiers = new List<List<DamageModifierDescriptor>>
            {
            };
            var currentIncomingDamageMods = new List<DamageModifier>();
            foreach (List<DamageModifierDescriptor> boons in AllIncomingDamageModifiers)
            {
                currentIncomingDamageMods.AddRange(boons.Where(x => x.Available(combatData) && x.Keep(mode, parserSettings)).Select(x => new IncomingDamageModifier(x)));
            }
            IncomingDamageModifiersPerSource = currentIncomingDamageMods.GroupBy(x => x.Src).ToDictionary(x => x.Key, x => (IReadOnlyList<DamageModifier>)x.ToList());
            IncomingDamageModifiersByName = currentIncomingDamageMods.GroupBy(x => x.Name).ToDictionary(x => x.Key, x =>
            {
                var list = x.ToList();
                if (list.Count > 1)
                {
                    throw new InvalidDataException("Same name present multiple times in damage mods - " + x.First().Name);
                }
                return list.First();
            });
        }

        public IReadOnlyList<DamageModifier> GetModifiersPerSpec(ParserHelper.Spec spec)
        {
            var res = new List<DamageModifier>();
            IReadOnlyList<ParserHelper.Source> srcs = ParserHelper.SpecToSources(spec);
            foreach (ParserHelper.Source src in srcs)
            {
                if (DamageModifiersPerSource.TryGetValue(src, out IReadOnlyList<DamageModifier> list))
                {
                    res.AddRange(list);
                }
            }
            return res;
        }

        public IReadOnlyList<DamageModifier> GetIncomingModifiersPerSpec(ParserHelper.Spec spec)
        {
            var res = new List<DamageModifier>();
            IReadOnlyList<ParserHelper.Source> srcs = ParserHelper.SpecToSources(spec);
            foreach (ParserHelper.Source src in srcs)
            {
                if (IncomingDamageModifiersPerSource.TryGetValue(src, out IReadOnlyList<DamageModifier> list))
                {
                    res.AddRange(list);
                }
            }
            return res;
        }
    }
}
