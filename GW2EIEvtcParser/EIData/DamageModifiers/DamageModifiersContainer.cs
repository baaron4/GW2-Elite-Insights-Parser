using GW2EIEvtcParser.LogLogic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class DamageModifiersContainer
{
    public readonly IReadOnlyDictionary<ParserHelper.Source, IReadOnlyList<OutgoingDamageModifier>> OutgoingDamageModifiersPerSource;
    public readonly IReadOnlyDictionary<int, OutgoingDamageModifier> OutgoingDamageModifiersByID;

    public readonly IReadOnlyDictionary<ParserHelper.Source, IReadOnlyList<IncomingDamageModifier>> IncomingDamageModifiersPerSource;
    public readonly IReadOnlyDictionary<int, IncomingDamageModifier> IncomingDamageModifiersByID;

    internal DamageModifiersContainer(CombatData combatData, LogLogic.LogLogic.ParseModeEnum parseMode, LogLogic.LogLogic.SkillModeEnum skillMode, EvtcParserSettings parserSettings)
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
            ConduitHelper.OutgoingDamageModifiers,
            //
            WarriorHelper.OutgoingDamageModifiers,
            BerserkerHelper.OutgoingDamageModifiers,
            SpellbreakerHelper.OutgoingDamageModifiers,
            BladeswornHelper.OutgoingDamageModifiers,
            ParagonHelper.OutgoingDamageModifiers,
            //
            GuardianHelper.OutgoingDamageModifiers,
            DragonhunterHelper.OutgoingDamageModifiers,
            FirebrandHelper.OutgoingDamageModifiers,
            WillbenderHelper.OutgoingDamageModifiers,
            LuminaryHelper.OutgoingDamageModifiers,
            //
            EngineerHelper.OutgoingDamageModifiers,
            ScrapperHelper.OutgoingDamageModifiers,
            HolosmithHelper.OutgoingDamageModifiers,
            MechanistHelper.OutgoingDamageModifiers,
            AmalgamHelper.OutgoingDamageModifiers,
            //
            ThiefHelper.OutgoingDamageModifiers,
            DaredevilHelper.OutgoingDamageModifiers,
            DeadeyeHelper.OutgoingDamageModifiers,
            SpecterHelper.OutgoingDamageModifiers,
            AntiquaryHelper.OutgoingDamageModifiers,
            //
            RangerHelper.OutgoingDamageModifiers,
            DruidHelper.OutgoingDamageModifiers,
            SoulbeastHelper.OutgoingDamageModifiers,
            UntamedHelper.OutgoingDamageModifiers,
            GaleshotHelper.OutgoingDamageModifiers,
            //
            MesmerHelper.OutgoingDamageModifiers,
            ChronomancerHelper.OutgoingDamageModifiers,
            MirageHelper.OutgoingDamageModifiers,
            VirtuosoHelper.OutgoingDamageModifiers,
            TroubadourHelper.OutgoingDamageModifiers,
            //
            NecromancerHelper.OutgoingDamageModifiers,
            ReaperHelper.OutgoingDamageModifiers,
            ScourgeHelper.OutgoingDamageModifiers,
            HarbingerHelper.OutgoingDamageModifiers,
            RitualistHelper.OutgoingDamageModifiers,
            //
            ElementalistHelper.OutgoingDamageModifiers,
            TempestHelper.OutgoingDamageModifiers,
            WeaverHelper.OutgoingDamageModifiers,
            CatalystHelper.OutgoingDamageModifiers,
            EvokerHelper.OutgoingDamageModifiers,
        ];
        var currentOutgoingDamageMods = new List<OutgoingDamageModifier>(10);
        foreach (var modifierDescriptor in allOutgoingDamageModifiers)
        {
            currentOutgoingDamageMods.AddRange(modifierDescriptor.Where(x => x.Available(combatData) && x.Keep(parseMode, skillMode, parserSettings)).Select(x => new OutgoingDamageModifier(x)));
        }
        var outDamageModsPerSource = new Dictionary<ParserHelper.Source, List<OutgoingDamageModifier>>();
        foreach (var incDamageMod in currentOutgoingDamageMods)
        {
            foreach (var source in incDamageMod.Srcs)
            {
                if (outDamageModsPerSource.TryGetValue(source, out var list))
                {
                    list.Add(incDamageMod);
                }
                else
                {
                    outDamageModsPerSource[source] = [incDamageMod];
                }
            }
        }
        OutgoingDamageModifiersPerSource = outDamageModsPerSource.ToDictionary(x => x.Key, x => (IReadOnlyList<OutgoingDamageModifier>)x.Value);
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
            ConduitHelper.IncomingDamageModifiers,
            //
            WarriorHelper.IncomingDamageModifiers,
            BerserkerHelper.IncomingDamageModifiers,
            SpellbreakerHelper.IncomingDamageModifiers,
            BladeswornHelper.IncomingDamageModifiers,
            ParagonHelper.IncomingDamageModifiers,
            //
            GuardianHelper.IncomingDamageModifiers,
            DragonhunterHelper.IncomingDamageModifiers,
            FirebrandHelper.IncomingDamageModifiers,
            WillbenderHelper.IncomingDamageModifiers,
            LuminaryHelper.IncomingDamageModifiers,
            //
            EngineerHelper.IncomingDamageModifiers,
            ScrapperHelper.IncomingDamageModifiers,
            HolosmithHelper.IncomingDamageModifiers,
            MechanistHelper.IncomingDamageModifiers,
            AmalgamHelper.IncomingDamageModifiers,
            //
            ThiefHelper.IncomingDamageModifiers,
            DaredevilHelper.IncomingDamageModifiers,
            DeadeyeHelper.IncomingDamageModifiers,
            SpecterHelper.IncomingDamageModifiers,
            AntiquaryHelper.IncomingDamageModifiers,
            //
            RangerHelper.IncomingDamageModifiers,
            DruidHelper.IncomingDamageModifiers,
            SoulbeastHelper.IncomingDamageModifiers,
            UntamedHelper.IncomingDamageModifiers,
            GaleshotHelper.IncomingDamageModifiers,
            //
            MesmerHelper.IncomingDamageModifiers,
            ChronomancerHelper.IncomingDamageModifiers,
            MirageHelper.IncomingDamageModifiers,
            VirtuosoHelper.IncomingDamageModifiers,
            TroubadourHelper.IncomingDamageModifiers,
            //
            NecromancerHelper.IncomingDamageModifiers,
            ReaperHelper.IncomingDamageModifiers,
            ScourgeHelper.IncomingDamageModifiers,
            HarbingerHelper.IncomingDamageModifiers,
            RitualistHelper.IncomingDamageModifiers,
            //
            ElementalistHelper.IncomingDamageModifiers,
            TempestHelper.IncomingDamageModifiers,
            WeaverHelper.IncomingDamageModifiers,
            CatalystHelper.IncomingDamageModifiers,
            EvokerHelper.IncomingDamageModifiers,
        ];
        var currentIncomingDamageMods = new List<IncomingDamageModifier>(20);
        foreach (var boons in allIncomingDamageModifiers)
        {
            currentIncomingDamageMods.AddRange(boons.Where(x => x.Available(combatData) && x.Keep(parseMode, skillMode, parserSettings)).Select(x => new IncomingDamageModifier(x)));
        }
        var incDamageModsPerSource = new Dictionary<ParserHelper.Source, List<IncomingDamageModifier>>();
        foreach (var incDamageMod in currentIncomingDamageMods)
        {
            foreach (var source in incDamageMod.Srcs)
            {
                if (incDamageModsPerSource.TryGetValue(source, out var list))
                {
                    list.Add(incDamageMod);
                }
                else
                {
                    incDamageModsPerSource[source] = [incDamageMod];
                }
            }
        }
        IncomingDamageModifiersPerSource = incDamageModsPerSource.ToDictionary(x => x.Key, x => (IReadOnlyList<IncomingDamageModifier>)x.Value);
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
        var res = new List<OutgoingDamageModifier>(srcs.Count); //TODO_PERF(Rennorb) @find average complexity
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
        var res = new List<IncomingDamageModifier>(srcs.Count); //TODO_PERF(Rennorb) @find average complexity
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
