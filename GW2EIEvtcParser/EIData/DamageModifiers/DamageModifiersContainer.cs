﻿using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class DamageModifiersContainer
{
    public readonly IReadOnlyDictionary<ParserHelper.Source, IReadOnlyList<OutgoingDamageModifier>> OutgoingDamageModifiersPerSource;
    public readonly IReadOnlyDictionary<int, OutgoingDamageModifier> OutgoingDamageModifiersByID;

    public readonly IReadOnlyDictionary<ParserHelper.Source, IReadOnlyList<IncomingDamageModifier>> IncomingDamageModifiersPerSource;
    public readonly IReadOnlyDictionary<int, IncomingDamageModifier> IncomingDamageModifiersByID;

    internal DamageModifiersContainer(CombatData combatData, FightLogic.ParseModeEnum parseMode, FightLogic.SkillModeEnum skillMode, EvtcParserSettings parserSettings)
    {
        IEnumerable<IReadOnlyList<DamageModifierDescriptor>> allOutgoingDamageModifiers = 
        [
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
        ];
        var currentOutgoingDamageMods = new List<OutgoingDamageModifier>(10);
        foreach (var modifierDescriptor in allOutgoingDamageModifiers)
        {
            currentOutgoingDamageMods.AddRange(modifierDescriptor.Where(x => x.Available(combatData) && x.Keep(parseMode, skillMode, parserSettings)).Select(x => new OutgoingDamageModifier(x)));
        }
        OutgoingDamageModifiersPerSource = currentOutgoingDamageMods.GroupBy(x => x.Src).ToDictionary(x => x.Key, x => (IReadOnlyList<OutgoingDamageModifier>)x.ToList());
        OutgoingDamageModifiersByID = currentOutgoingDamageMods.GroupBy(x => x.ID).ToDictionary(x => x.Key, x =>
        {
            var e = x.GetEnumerator(); e.MoveNext();
            var first = e.Current;
            if (e.MoveNext()) { throw new InvalidDataException("Same id present multiple times in damage mods - " + first.ID); }
            return first;
        });
        //
        IEnumerable<IReadOnlyList<DamageModifierDescriptor>> allIncomingDamageModifiers =
        [
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
        ];
        var currentIncomingDamageMods = new List<IncomingDamageModifier>(20);
        foreach (var boons in allIncomingDamageModifiers)
        {
            currentIncomingDamageMods.AddRange(boons.Where(x => x.Available(combatData) && x.Keep(parseMode, skillMode, parserSettings)).Select(x => new IncomingDamageModifier(x)));
        }
        IncomingDamageModifiersPerSource = currentIncomingDamageMods.GroupBy(x => x.Src).ToDictionary(x => x.Key, x => (IReadOnlyList<IncomingDamageModifier>)x.ToList());
        IncomingDamageModifiersByID = currentIncomingDamageMods.GroupBy(x => x.ID).ToDictionary(x => x.Key, x =>
        {
            var e = x.GetEnumerator(); e.MoveNext();
            var first = e.Current;
            if (e.MoveNext()) { throw new InvalidDataException("Same id present multiple times in damage mods - " + first.ID); }
            return first;
        });
    }

    public List<OutgoingDamageModifier> GetOutgoingModifiersPerSpec(ParserHelper.Spec spec)
    {
        var srcs = ParserHelper.SpecToSources(spec);
        var res = new List<OutgoingDamageModifier>(srcs.Count); //TODO(Rennorb) @perf: find average complexity
        foreach (ParserHelper.Source src in srcs)
        {
            if (OutgoingDamageModifiersPerSource.TryGetValue(src, out var damageMods))
            {
                res.AddRange(damageMods);
            }
        }
        return res;
    }

    public List<IncomingDamageModifier> GetIncomingModifiersPerSpec(ParserHelper.Spec spec)
    {
        var srcs = ParserHelper.SpecToSources(spec);
        var res = new List<IncomingDamageModifier>(srcs.Count); //TODO(Rennorb) @perf: find average complexity
        foreach (ParserHelper.Source src in srcs)
        {
            if (IncomingDamageModifiersPerSource.TryGetValue(src, out var damageMods))
            {
                res.AddRange(damageMods);
            }
        }
        return res;
    }
}
