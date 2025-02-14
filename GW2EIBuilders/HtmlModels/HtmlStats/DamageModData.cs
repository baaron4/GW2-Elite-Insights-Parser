using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIBuilders.HtmlModels.HTMLStats;

using DamageModDataItem = List<double>;

internal class DamageModData
{
    public readonly List<DamageModDataItem> Data;
    public readonly List<List<DamageModDataItem>> DataTarget;

    private DamageModData(SingleActor actor, ParsedEvtcLog log, IReadOnlyList<OutgoingDamageModifier> listToUse, PhaseData phase)
    {
        var dModData = actor.GetOutgoingDamageModifierStats(null, log, phase.Start, phase.End);

        Data = new(listToUse.Count);
        foreach (OutgoingDamageModifier dMod in listToUse)
        {
            if (dModData.TryGetValue(dMod.ID, out var data))
            {
                Data.Add(
                [
                    data.HitCount,
                    data.TotalHitCount,
                    data.DamageGain,
                    data.TotalDamage
                ]);
            }
            else
            {
                Data.Add(
                [
                    0,
                    dMod.GetHitDamageEvents(actor, log, null, phase.Start, phase.End).Count(),
                    0,
                    dMod.GetTotalDamage(actor, log, null, phase.Start, phase.End)
                ]);
            }
        }

        var allTargets = phase.Targets;
        DataTarget = new(allTargets.Count);
        foreach (SingleActor target in allTargets.Keys)
        {
            var pTarget = new List<DamageModDataItem>(1 + listToUse.Count);
            DataTarget.Add(pTarget);
            dModData = actor.GetOutgoingDamageModifierStats(target, log, phase.Start, phase.End);
            foreach (OutgoingDamageModifier dMod in listToUse)
            {
                if (dModData.TryGetValue(dMod.ID, out var data))
                {
                    pTarget.Add(
                    [
                        data.HitCount,
                        data.TotalHitCount,
                        data.DamageGain,
                        data.TotalDamage
                    ]);
                }
                else
                {
                    pTarget.Add(
                    [
                        0,
                        dMod.GetHitDamageEvents(actor, log, target, phase.Start, phase.End).Count(),
                        0,
                        dMod.GetTotalDamage(actor, log, target, phase.Start, phase.End)
                    ]);
                }
            }
        }
    }
    private DamageModData(SingleActor actor, ParsedEvtcLog log, IReadOnlyList<IncomingDamageModifier> listToUse, PhaseData phase)
    {
        var dModData = actor.GetIncomingDamageModifierStats(null, log, phase.Start, phase.End);

        Data = new(listToUse.Count);
        foreach (IncomingDamageModifier dMod in listToUse)
        {
            if (dModData.TryGetValue(dMod.ID, out var data))
            {
                Data.Add(
                [
                    data.HitCount,
                    data.TotalHitCount,
                    data.DamageGain,
                    data.TotalDamage
                ]);
            }
            else
            {
                Data.Add(
                [
                    0,
                    dMod.GetHitDamageEvents(actor, log, null, phase.Start, phase.End).Count(),
                    0,
                    dMod.GetTotalDamage(actor, log, null, phase.Start, phase.End)
                ]);
            }
        }

        DataTarget = new(phase.Targets.Count);
        foreach (SingleActor target in phase.Targets.Keys)
        {
            var pTarget = new List<DamageModDataItem>(listToUse.Count);
            DataTarget.Add(pTarget);
            dModData = actor.GetIncomingDamageModifierStats(target, log, phase.Start, phase.End);
            foreach (IncomingDamageModifier dMod in listToUse)
            {
                if (dModData.TryGetValue(dMod.ID, out var data))
                {
                    pTarget.Add(
                    [
                        data.HitCount,
                        data.TotalHitCount,
                        data.DamageGain,
                        data.TotalDamage
                    ]);
                }
                else
                {
                    pTarget.Add(
                    [
                        0,
                        dMod.GetHitDamageEvents(actor, log, target, phase.Start, phase.End).Count(),
                        0,
                        dMod.GetTotalDamage(actor, log, target, phase.Start, phase.End)
                    ]);
                }
            }
        }
    }
    public static List<DamageModData> BuildOutgoingDmgModifiersData(ParsedEvtcLog log, PhaseData phase, IReadOnlyList<OutgoingDamageModifier> damageModsToUse)
    {
        var pData = new List<DamageModData>(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            pData.Add(new DamageModData(actor, log, damageModsToUse, phase));
        }
        return pData;
    }

    public static List<DamageModData> BuildIncomingDmgModifiersData(ParsedEvtcLog log, PhaseData phase, IReadOnlyList<IncomingDamageModifier> damageModsToUse)
    {
        var pData = new List<DamageModData>(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            pData.Add(new DamageModData(actor, log, damageModsToUse, phase));
        }
        return pData;
    }

    public static List<DamageModData> BuildPersonalOutgoingDmgModifiersData(ParsedEvtcLog log, PhaseData phase, IReadOnlyDictionary<Spec, IReadOnlyList<OutgoingDamageModifier>> damageModsToUse)
    {
        var pData = new List<DamageModData>(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            pData.Add(new DamageModData(actor, log, damageModsToUse[actor.Spec], phase));
        }
        return pData;
    }

    public static List<DamageModData> BuildPersonalIncomingDmgModifiersData(ParsedEvtcLog log, PhaseData phase, IReadOnlyDictionary<Spec, IReadOnlyList<IncomingDamageModifier>> damageModsToUse)
    {
        var pData = new List<DamageModData>(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            pData.Add(new DamageModData(actor, log, damageModsToUse[actor.Spec], phase));
        }
        return pData;
    }
}
