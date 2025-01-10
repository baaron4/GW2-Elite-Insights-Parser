using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

internal class BuffOnFoeDamageModifier : BuffOnActorDamageModifier
{
    private BuffsTracker? _trackerSource = null;
    private GainComputer? _gainComputerSource = null;
    internal BuffOnFoeDamageModifier(int id, long buffID, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(id, buffID, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode)
    {
    }

    internal BuffOnFoeDamageModifier(int id, HashSet<long> buffIDs, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(id, buffIDs, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode)
    {
    }

    internal BuffOnFoeDamageModifier UsingSrcCheckerByAbsence(long buffOnSourceID)
    {
        _trackerSource = new BuffsTrackerSingle(buffOnSourceID);
        _gainComputerSource = ByAbsence;
        return this;
    }

    internal BuffOnFoeDamageModifier UsingSrcCheckerByAbsence(HashSet<long> buffOnSourceIDs)
    {
        _trackerSource = new BuffsTrackerMulti(buffOnSourceIDs);
        _gainComputerSource = ByAbsence;
        return this;
    }

    internal BuffOnFoeDamageModifier UsingSrcCheckerByPresence(long buffOnSourceID)
    {
        _trackerSource = new BuffsTrackerSingle(buffOnSourceID);
        _gainComputerSource = ByPresence;
        return this;
    }

    internal BuffOnFoeDamageModifier UsingSrcCheckerByPresence(HashSet<long> buffOnSourceIDs)
    {
        _trackerSource = new BuffsTrackerMulti(buffOnSourceIDs);
        _gainComputerSource = ByPresence;
        return this;
    }

    protected bool CheckActor(IReadOnlyDictionary<long, BuffsGraphModel> bgmsSource, long time)
    {
        return _gainComputerSource == null || _gainComputerSource.ComputeGain(1.0, _trackerSource!.GetStack(bgmsSource, time)) > 0.0;
    }

    internal override bool Keep(FightLogic.ParseModeEnum parseMode, FightLogic.SkillModeEnum skillMode, EvtcParserSettings parserSettings)
    {
        // Remove target  based damage mods from PvP contexts
        if (parseMode == FightLogic.ParseModeEnum.WvW || parseMode == FightLogic.ParseModeEnum.sPvP)
        {
            return false;
        }
        return base.Keep(parseMode, skillMode, parserSettings);
    }

    internal override List<DamageModifierEvent> ComputeDamageModifier(SingleActor actor, ParsedEvtcLog log, DamageModifier damageModifier)
    {
        IReadOnlyDictionary<long, BuffsGraphModel> bgmsSource = actor.GetBuffGraphs(log);
        if (_trackerSource != null)
        {
            if (Skip(_trackerSource, bgmsSource, _gainComputerSource!))
            {
                return [];
            }
        }
        var res = new List<DamageModifierEvent>();
        var typeHits = damageModifier.GetHitDamageEvents(actor, log, null, log.FightData.FightStart, log.FightData.FightEnd);
        var ignoredTargets = new HashSet<SingleActor>();
        foreach (HealthDamageEvent evt in typeHits)
        {
            SingleActor target = log.FindActor(damageModifier.GetFoe(evt));
            if (ignoredTargets.Contains(target))
            {
                continue;
            }
            IReadOnlyDictionary<long, BuffsGraphModel> bgms = target.GetBuffGraphs(log);
            if (Skip(Tracker, bgms, GainComputer))
            {
                ignoredTargets.Add(target);
                continue;
            }
            if (CheckActor(bgmsSource, evt.Time) && ComputeGain(bgms, evt, log, out double gain) && CheckCondition(evt, log))
            {
                res.Add(new DamageModifierEvent(evt, damageModifier, gain * evt.HealthDamage));
            }
        }
        return res;
    }
}
