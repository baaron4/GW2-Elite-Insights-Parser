using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class DamageModifiersContainer
    {

        public IReadOnlyDictionary<ParserHelper.Source, IReadOnlyList<OutgoingDamageModifier>> OutgoingDamageModifiersPerSource { get; }

        public IReadOnlyDictionary<string, OutgoingDamageModifier> OutgoingDamageModifiersByName { get; }
        public IReadOnlyDictionary<ParserHelper.Source, IReadOnlyList<IncomingDamageModifier>> IncomingDamageModifiersPerSource { get; }

        public IReadOnlyDictionary<string, IncomingDamageModifier> IncomingDamageModifiersByName { get; }

        internal DamageModifiersContainer(CombatData combatData, FightLogic.ParseMode mode, EvtcParserSettings parserSettings)
        {
            var AllOutgoingDamageModifiers = new List<List<DamageModifierDescriptor>>
            {
                ItemDamageModifiers.OutgoingDamageModifiers,
                EncounterDamageModifiers.OutgoingDamageModifiers,
                GearDamageModifiers.OutgoingDamageModifiers,
                SharedDamageModifiers.OutgoingDamageModifiers,
                //
                RevenantHelper.OutgoingDamageModifiers,
                HeraldHelper.OutgoingDamageModifiers,
                RenegadeHelper.OutgoingDamageModifiers,
                VindicatorHelper.OutgoingDamageModifiers,
                //
                WarriorHelper.OutgoingDamageModifiers,
                BerserkerHelper.OutgoingDamageModifiers,
                SpellbreakerHelper.OutgoingDamageModifiers,
                BladeswornHelper.OutgoingDamageModifiers,
                //
                GuardianHelper.OutgoingDamageModifiers,
                DragonhunterHelper.OutgoingDamageModifiers,
                FirebrandHelper.OutgoingDamageModifiers,
                WillbenderHelper.OutgoingDamageModifiers,
                //
                EngineerHelper.OutgoingDamageModifiers,
                ScrapperHelper.OutgoingDamageModifiers,
                HolosmithHelper.OutgoingDamageModifiers,
                MechanistHelper.OutgoingDamageModifiers,
                //
                ThiefHelper.OutgoingDamageModifiers,
                DaredevilHelper.OutgoingDamageModifiers,
                DeadeyeHelper.OutgoingDamageModifiers,
                SpecterHelper.OutgoingDamageModifiers,
                //
                RangerHelper.OutgoingDamageModifiers,
                DruidHelper.OutgoingDamageModifiers,
                SoulbeastHelper.OutgoingDamageModifiers,
                UntamedHelper.OutgoingDamageModifiers,
                //
                MesmerHelper.OutgoingDamageModifiers,
                ChronomancerHelper.OutgoingDamageModifiers,
                MirageHelper.OutgoingDamageModifiers,
                VirtuosoHelper.OutgoingDamageModifiers,
                //
                NecromancerHelper.OutgoingDamageModifiers,
                ReaperHelper.OutgoingDamageModifiers,
                ScourgeHelper.OutgoingDamageModifiers,
                HarbingerHelper.OutgoingDamageModifiers,
                //
                ElementalistHelper.OutgoingDamageModifiers,
                TempestHelper.OutgoingDamageModifiers,
                WeaverHelper.OutgoingDamageModifiers,
                CatalystHelper.OutgoingDamageModifiers,
            };
            var currentOutgoingDamageMods = new List<OutgoingDamageModifier>();
            foreach (List<DamageModifierDescriptor> modifierDescriptor in AllOutgoingDamageModifiers)
            {
                currentOutgoingDamageMods.AddRange(modifierDescriptor.Where(x => x.Available(combatData) && x.Keep(mode, parserSettings)).Select(x => new OutgoingDamageModifier(x)));
            }
            OutgoingDamageModifiersPerSource = currentOutgoingDamageMods.GroupBy(x => x.Src).ToDictionary(x => x.Key, x => (IReadOnlyList<OutgoingDamageModifier>)x.ToList());
            OutgoingDamageModifiersByName = currentOutgoingDamageMods.GroupBy(x => x.Name).ToDictionary(x => x.Key, x =>
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
                ItemDamageModifiers.IncomingDamageModifiers,
                EncounterDamageModifiers.IncomingDamageModifiers,
                GearDamageModifiers.IncomingDamageModifiers,
                SharedDamageModifiers.IncomingDamageModifiers,
                //
                RevenantHelper.IncomingDamageModifiers,
                HeraldHelper.IncomingDamageModifiers,
                RenegadeHelper.IncomingDamageModifiers,
                VindicatorHelper.IncomingDamageModifiers,
                //
                WarriorHelper.IncomingDamageModifiers,
                BerserkerHelper.IncomingDamageModifiers,
                SpellbreakerHelper.IncomingDamageModifiers,
                BladeswornHelper.IncomingDamageModifiers,
                //
                GuardianHelper.IncomingDamageModifiers,
                DragonhunterHelper.IncomingDamageModifiers,
                FirebrandHelper.IncomingDamageModifiers,
                WillbenderHelper.IncomingDamageModifiers,
                //
                EngineerHelper.IncomingDamageModifiers,
                ScrapperHelper.IncomingDamageModifiers,
                HolosmithHelper.IncomingDamageModifiers,
                MechanistHelper.IncomingDamageModifiers,
                //
                ThiefHelper.IncomingDamageModifiers,
                DaredevilHelper.IncomingDamageModifiers,
                DeadeyeHelper.IncomingDamageModifiers,
                SpecterHelper.IncomingDamageModifiers,
                //
                RangerHelper.IncomingDamageModifiers,
                DruidHelper.IncomingDamageModifiers,
                SoulbeastHelper.IncomingDamageModifiers,
                UntamedHelper.IncomingDamageModifiers,
                //
                MesmerHelper.IncomingDamageModifiers,
                ChronomancerHelper.IncomingDamageModifiers,
                MirageHelper.IncomingDamageModifiers,
                VirtuosoHelper.IncomingDamageModifiers,
                //
                NecromancerHelper.IncomingDamageModifiers,
                ReaperHelper.IncomingDamageModifiers,
                ScourgeHelper.IncomingDamageModifiers,
                HarbingerHelper.IncomingDamageModifiers,
                //
                ElementalistHelper.IncomingDamageModifiers,
                TempestHelper.IncomingDamageModifiers,
                WeaverHelper.IncomingDamageModifiers,
                CatalystHelper.IncomingDamageModifiers,
            };
            var currentIncomingDamageMods = new List<IncomingDamageModifier>();
            foreach (List<DamageModifierDescriptor> boons in AllIncomingDamageModifiers)
            {
                currentIncomingDamageMods.AddRange(boons.Where(x => x.Available(combatData) && x.Keep(mode, parserSettings)).Select(x => new IncomingDamageModifier(x)));
            }
            IncomingDamageModifiersPerSource = currentIncomingDamageMods.GroupBy(x => x.Src).ToDictionary(x => x.Key, x => (IReadOnlyList<IncomingDamageModifier>)x.ToList());
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

        public IReadOnlyList<OutgoingDamageModifier> GetOutgoingModifiersPerSpec(ParserHelper.Spec spec)
        {
            var res = new List<OutgoingDamageModifier>();
            IReadOnlyList<ParserHelper.Source> srcs = ParserHelper.SpecToSources(spec);
            foreach (ParserHelper.Source src in srcs)
            {
                if (OutgoingDamageModifiersPerSource.TryGetValue(src, out IReadOnlyList<OutgoingDamageModifier> list))
                {
                    res.AddRange(list);
                }
            }
            return res;
        }

        public IReadOnlyList<IncomingDamageModifier> GetIncomingModifiersPerSpec(ParserHelper.Spec spec)
        {
            var res = new List<IncomingDamageModifier>();
            IReadOnlyList<ParserHelper.Source> srcs = ParserHelper.SpecToSources(spec);
            foreach (ParserHelper.Source src in srcs)
            {
                if (IncomingDamageModifiersPerSource.TryGetValue(src, out IReadOnlyList<IncomingDamageModifier> list))
                {
                    res.AddRange(list);
                }
            }
            return res;
        }
    }
}
