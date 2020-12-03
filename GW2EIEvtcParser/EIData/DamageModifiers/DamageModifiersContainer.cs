using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.Exceptions;

namespace GW2EIEvtcParser.EIData
{
    public class DamageModifiersContainer
    {

        public Dictionary<ParserHelper.Source, List<DamageModifier>> DamageModifiersPerSource { get; }

        public Dictionary<string, DamageModifier> DamageModifiersByName { get; }

        internal DamageModifiersContainer(ulong build, FightLogic.ParseMode mode, EvtcParserSettings parserSettings)
        {
            var AllDamageModifiers = new List<List<DamageModifier>>
            {
                DamageModifier.ItemDamageModifiers,
                DamageModifier.CommonDamageModifiers,
                DamageModifier.FightSpecificDamageModifiers,
                //
                RevenantHelper.DamageMods,
                HeraldHelper.DamageMods,
                RenegadeHelper.DamageMods,
                //
                WarriorHelper.DamageMods,
                BerserkerHelper.DamageMods,
                SpellbreakerHelper.DamageMods,
                //
                GuardianHelper.DamageMods,
                DragonhunterHelper.DamageMods,
                FirebrandHelper.DamageMods,
                //
                EngineerHelper.DamageMods,
                ScrapperHelper.DamageMods,
                HolosmithHelper.DamageMods,
                //
                ThiefHelper.DamageMods,
                DaredevilHelper.DamageMods,
                DeadeyeHelper.DamageMods,
                //
                RangerHelper.DamageMods,
                DruidHelper.DamageMods,
                SoulbeastHelper.DamageMods,
                //
                MesmerHelper.DamageMods,
                ChronomancerHelper.DamageMods,
                MirageHelper.DamageMods,
                //
                NecromancerHelper.DamageMods,
                ReaperHelper.DamageMods,
                ScourgeHelper.DamageMods,
                //
                ElementalistHelper.DamageMods,
                TempestHelper.DamageMods,
                WeaverHelper.DamageMods,
            };
            var currentDamageMods = new List<DamageModifier>();
            foreach (List<DamageModifier> boons in AllDamageModifiers)
            {
                currentDamageMods.AddRange(boons.Where(x => x.Available(build) && x.Keep(mode, parserSettings))) ;
            }
            DamageModifiersPerSource = currentDamageMods.GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());
            DamageModifiersByName = currentDamageMods.GroupBy(x => x.Name).ToDictionary(x => x.Key, x =>
            {
                var list = x.ToList();
                if (list.Count > 1)
                {
                    throw new InvalidDataException("Same name present multiple times in damage mods - " + x.First().Name);
                }
                return list.First();
            });
        }

        public List<DamageModifier> GetModifiersPerProf(string prof)
        {
            var res = new List<DamageModifier>();
            List<ParserHelper.Source> srcs = ParserHelper.ProfToEnum(prof);
            foreach (ParserHelper.Source src in srcs)
            {
                if (DamageModifiersPerSource.TryGetValue(src, out List<DamageModifier> list))
                {
                    res.AddRange(list);
                }
            }
            return res;
        }
    }
}
