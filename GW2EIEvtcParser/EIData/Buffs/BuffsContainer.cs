using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.EIData.BuffSourceFinders;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData;

public class BuffsContainer
{

    public readonly IReadOnlyDictionary<long, Buff> BuffsByIDs;
    public readonly IReadOnlyDictionary<BuffClassification, IReadOnlyList<Buff>> BuffsByClassification;
    public readonly IReadOnlyDictionary<ParserHelper.Source, IReadOnlyList<Buff>> BuffsBySource;
    private readonly Dictionary<string, Buff> _buffsByName;

    private readonly BuffSourceFinder _buffSourceFinder;


    internal BuffsContainer(CombatData combatData, SkillData skillData, ParserController operation)
    {
        var AllBuffs = new List<IReadOnlyList<Buff>>()
        {
            CommonBuffs.Boons,
            CommonBuffs.Conditions,
            CommonBuffs.Commons,
            CommonBuffs.Gear,
            FoodBuffs.NormalFoods,
            FoodBuffs.AscendedFood,
            FoodBuffs.FoodProcs,
            UtilityBuffs.Utilities,
            UtilityBuffs.SlayingPotions,
            UtilityBuffs.Writs,
            UtilityBuffs.OtherConsumables,
            UtilityBuffs.UtilityProcs,
            EncounterBuffs.EncounterSpecific,
            EncounterBuffs.FractalInstabilities,
            WvWBuffs.Commons,
            //
            RevenantHelper.Buffs,
            HeraldHelper.Buffs,
            RenegadeHelper.Buffs,
            VindicatorHelper.Buffs,
            ConduitHelper.Buffs,
            //
            WarriorHelper.Buffs,
            BerserkerHelper.Buffs,
            SpellbreakerHelper.Buffs,
            BladeswornHelper.Buffs,
            ParagonHelper.Buffs,
            //
            GuardianHelper.Buffs,
            DragonhunterHelper.Buffs,
            FirebrandHelper.Buffs,
            WillbenderHelper.Buffs,
            LuminaryHelper.Buffs,
            //
            RangerHelper.Buffs,
            DruidHelper.Buffs,
            SoulbeastHelper.Buffs,
            UntamedHelper.Buffs,
            GaleshotHelper.Buffs,
            //
            ThiefHelper.Buffs,
            DaredevilHelper.Buffs,
            DeadeyeHelper.Buffs,
            SpecterHelper.Buffs,
            AntiquaryHelper.Buffs,
            //
            EngineerHelper.Buffs,
            ScrapperHelper.Buffs,
            HolosmithHelper.Buffs,
            MechanistHelper.Buffs,
            AmalgamHelper.Buffs,
            //
            MesmerHelper.Buffs,
            ChronomancerHelper.Buffs,
            MirageHelper.Buffs,
            VirtuosoHelper.Buffs,
            TroubadourHelper.Buffs,
            //
            NecromancerHelper.Buffs,
            ReaperHelper.Buffs,
            ScourgeHelper.Buffs,
            HarbingerHelper.Buffs,
            RitualistHelper.Buffs,
            //
            ElementalistHelper.Buffs,
            TempestHelper.Buffs,
            WeaverHelper.Buffs,
            CatalystHelper.Buffs,
            EvokerHelper.Buffs,
        };
        var currentBuffs = new List<Buff>();
        foreach (IReadOnlyList<Buff> buffs in AllBuffs)
        {
            currentBuffs.AddRange(buffs.Where(x => x.Available(combatData)));
        }
        _buffsByName = currentBuffs.GroupBy(x => x.Name).ToDictionary(x => x.Key, x =>
        {
            if (x.Count() > 1)
            {
                throw new InvalidDataException("Same name present multiple times in buffs - " + x.First().Name);
            }
            return x.First();
        });
        // Unknown consumables
        var buffIDs = new HashSet<long>(currentBuffs.Select(x => x.ID));
        List<BuffInfoEvent> foodInfoEvents = FoodBuffs.NormalFoods.Select(x => combatData.GetBuffInfoEvent(x.ID)).Where(x => x != null).ToList()!;
        foodInfoEvents.AddRange(FoodBuffs.AscendedFood.Select(x => combatData.GetBuffInfoEvent(x.ID)).Where(x => x != null)!);
        var foodIDs = foodInfoEvents.Select(x => x.CategoryByte).Distinct().ToList();
        if (foodIDs.Count == 1)
        {
            var foodID = foodIDs[0];
            foreach (BuffInfoEvent buffInfoEvent in combatData.GetBuffInfoEvent(foodID))
            {
                if (!buffIDs.Contains(buffInfoEvent.BuffID))
                {
                    operation.UpdateProgressWithCancellationCheck("Parsing: Creating nourishement " + buffInfoEvent.BuffID);
                    currentBuffs.Add(CreateCustomBuff("Parsing: Unknown Nourishment", buffInfoEvent.BuffID, ItemImages.NourishmentEffect, buffInfoEvent.MaxStacks, BuffClassification.Nourishment));
                }
            }
        }
        List<BuffInfoEvent> enhancementInfoEvents = UtilityBuffs.Utilities.Select(x => combatData.GetBuffInfoEvent(x.ID)).Where(x => x != null).ToList()!;
        enhancementInfoEvents.AddRange(UtilityBuffs.Writs.Select(x => combatData.GetBuffInfoEvent(x.ID)).Where(x => x != null)!);
        enhancementInfoEvents.AddRange(UtilityBuffs.SlayingPotions.Select(x => combatData.GetBuffInfoEvent(x.ID)).Where(x => x != null)!);
        var enhancementIDs = enhancementInfoEvents.Select(x => x.CategoryByte).Distinct().ToList();
        if (enhancementIDs.Count == 1)
        {
            var enhancementID = enhancementIDs[0];
            foreach (BuffInfoEvent buffInfoEvent in combatData.GetBuffInfoEvent(enhancementID))
            {
                if (!buffIDs.Contains(buffInfoEvent.BuffID))
                {
                    operation.UpdateProgressWithCancellationCheck("Creating enhancement " + buffInfoEvent.BuffID);
                    currentBuffs.Add(CreateCustomBuff("Unknown Enhancement", buffInfoEvent.BuffID, ItemImages.EnhancementEffect, buffInfoEvent.MaxStacks, BuffClassification.Enhancement));
                }
            }
        }
        //
        BuffsByIDs = currentBuffs.GroupBy(x => x.ID).ToDictionary(x => x.Key, x =>
        {
            if (x.Count() > 1 && x.Key != SkillIDs.NoBuff && x.Key != SkillIDs.Unknown)
            {
                throw new InvalidDataException("Same id present multiple times in buffs - " + x.First().ID);
            }
            return x.First();
        });
        operation.UpdateProgressWithCancellationCheck("Parsing: Adjusting Skill icons using buffs");
        foreach (var pair in BuffsByIDs)
        {
            if (skillData.TryGet(pair.Key, out var skill))
            {
                skill.OverrideFromBuff(pair.Value);
            }
        }
        operation.UpdateProgressWithCancellationCheck("Parsing: Adjusting Buffs");
        BuffInfoSolver.AdjustBuffs(combatData, BuffsByIDs, operation);
        foreach (Buff buff in currentBuffs)
        {
            BuffInfoEvent? buffInfoEvt = combatData.GetBuffInfoEvent(buff.ID);
            if (buffInfoEvt != null)
            {
                foreach (BuffFormula formula in buffInfoEvt.Formulas)
                {
                    if (formula.Attr1 == BuffAttribute.Unknown)
                    {
                        operation.UpdateProgressWithCancellationCheck("Parsing: Unknown Formula for " + buff.Name + ": " + formula.GetDescription(true, BuffsByIDs, buff));
                    }
                }
            }
        }
        BuffsByClassification = currentBuffs.GroupBy(x => x.Classification).ToDictionary(x => x.Key, x => (IReadOnlyList<Buff>)x.ToList());
        BuffsBySource = currentBuffs.GroupBy(x => x.Source).ToDictionary(x => x.Key, x => (IReadOnlyList<Buff>)x.ToList());
        //
        _buffSourceFinder = GetBuffSourceFinder(combatData, new HashSet<long>(BuffsByClassification[BuffClassification.Boon].Select(x => x.ID)));
        // Band aid for the stack type situation with fake inactive/infinite durations
        if (combatData.HasStackIDs)
        {
            var stackTypeBuffs = currentBuffs.Where(x => x.StackType == BuffStackType.StackingConditionalLoss || x.StackType == BuffStackType.Stacking);
            foreach (Buff buff in stackTypeBuffs)
            {
                IReadOnlyList<BuffEvent> buffData = combatData.GetBuffData(buff.ID);
                if (buffData.OfType<BuffRemoveSingleEvent>().Any(x => !x.OverstackOrNaturalEnd))
                {
                    foreach (var group in buffData.GroupBy(x => x.To))
                    {
                        var buffs = group.ToList();
                        var appliesPerInstanceID = buffs.OfType<BuffApplyEvent>().GroupBy(x => x.BuffInstance).ToDictionary(x => x.Key, x => x.ToList());
                        var removeSinglesPerInstanceID = buffs.OfType<BuffRemoveSingleEvent>().Where(x => !x.OverstackOrNaturalEnd).GroupBy(x => x.BuffInstance);
                        foreach (var removePair in removeSinglesPerInstanceID)
                        {
                            if (appliesPerInstanceID.TryGetValue(removePair.Key, out var applyList))
                            {
                                foreach (BuffRemoveSingleEvent remove in removePair)
                                {
                                    BuffApplyEvent? apply = applyList.LastOrDefault(x => x.Time <= remove.Time); //TODO(Rennorb) @perf
                                    if (apply != null && apply.OriginalAppliedDuration == remove.RemovedDuration)
                                    {
                                        int activeTime = apply.OriginalAppliedDuration - apply.AppliedDuration;
                                        int elapsedTime = (int)(remove.Time - apply.Time);
                                        remove.OverrideRemovedDuration(remove.RemovedDuration - activeTime - elapsedTime);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public bool TryGetBuffByName(string name, out Buff? buff)
    {
        return _buffsByName.TryGetValue(name, out buff);
    }

    internal AgentItem TryFindSrc(AgentItem dst, long time, long extension, ParsedEvtcLog log, long buffID, uint buffInstance)
    {
        return _buffSourceFinder.TryFindSrc(dst, time, extension, log, buffID, buffInstance);
    }

    // Non shareable buffs
    public IReadOnlyList<Buff> GetPersonalBuffsList(ParserHelper.Spec spec)
    {
        var result = new List<Buff>();
        foreach (ParserHelper.Source src in ParserHelper.SpecToSources(spec))
        {
            if (BuffsBySource.TryGetValue(src, out var buffs))
            {
                result.AddRange(buffs.Where(x => x.Classification == BuffClassification.Other));
            }
        }
        return result;
    }
}
