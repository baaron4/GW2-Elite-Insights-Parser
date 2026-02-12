using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

internal class EffectCastFinder : CheckedCastFinder<EffectEvent>
{
    protected bool Minions = false;
    private readonly GUID _effectGUID;
    private int _speciesID = 0;

    public EffectCastFinder WithMinions()
    {
        Minions = true;
        return this;
    }

    protected AgentItem GetAgent(EffectEvent effectEvent)
    {
        return Minions ? GetKeyAgent(effectEvent).GetFinalMaster() : GetKeyAgent(effectEvent);
    }

    protected AgentItem GetOtherAgent(EffectEvent effectEvent)
    {
        return Minions ? GetOtherKeyAgent(effectEvent).GetFinalMaster() : GetOtherKeyAgent(effectEvent);
    }

    protected virtual AgentItem GetKeyAgent(EffectEvent effectEvent)
    {
        return effectEvent.Src;
    }

    protected virtual AgentItem GetOtherKeyAgent(EffectEvent effectEvent)
    {
        return effectEvent.Dst;
    }

    public EffectCastFinder(long skillID, GUID effectGUID) : base(skillID)
    {
        UsingNotAccurate();
        UsingEnable((combatData) => combatData.HasEffectData);
        _effectGUID = effectGUID;
    }

    internal EffectCastFinder UsingSrcBaseSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.Src.BaseSpec == spec);
        return this;
    }

    internal EffectCastFinder UsingDstBaseSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.IsAroundDst && evt.Dst.BaseSpec == spec);
        return this;
    }

    internal EffectCastFinder UsingSrcNotBaseSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.Src.BaseSpec != spec);
        return this;
    }

    internal EffectCastFinder UsingDstNotBaseSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.IsAroundDst && evt.Dst.BaseSpec != spec);
        return this;
    }

    internal EffectCastFinder UsingSrcSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.Src.Spec == spec);
        return this;
    }

    internal EffectCastFinder UsingSrcNotSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.Src.Spec != spec);
        return this;
    }

    internal EffectCastFinder UsingSrcSpecsChecker(HashSet<Spec> specs)
    {
        UsingChecker((evt, combatData, agentData, skillData) => specs.Contains(evt.Src.Spec));
        return this;
    }
    internal EffectCastFinder UsingSrcNotSpecsChecker(HashSet<Spec> specs)
    {
        UsingChecker((evt, combatData, agentData, skillData) => !specs.Contains(evt.Src.Spec));
        return this;
    }

    internal EffectCastFinder UsingDstSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.IsAroundDst && evt.Dst.Spec == spec);
        return this;
    }

    internal EffectCastFinder UsingDstNotSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.IsAroundDst && evt.Dst.Spec != spec);
        return this;
    }

    internal EffectCastFinder UsingDstSpecsChecker(HashSet<Spec> specs)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.IsAroundDst && specs.Contains(evt.Dst.Spec));
        return this;
    }

    internal EffectCastFinder UsingDstNotSpecsChecker(HashSet<Spec> specs)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.IsAroundDst && !specs.Contains(evt.Dst.Spec));
        return this;
    }

    internal EffectCastFinder UsingIsAroundDstChecker()
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.IsAroundDst);
        return this;
    }

    internal EffectCastFinder UsingNotIsAroundDstChecker()
    {
        UsingChecker((evt, combatData, agentData, skillData) => !evt.IsAroundDst);
        return this;
    }

    internal EffectCastFinder UsingDurationChecker(long duration)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.Duration == duration);
        return this;
    }

    internal EffectCastFinder UsingDurationChecker(long min, long max)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.Duration >= min && evt.Duration <= max);
        return this;
    }

    internal EffectCastFinder UsingSecondaryEffectSameSrcChecker(GUID effect, long timeOffset = 0, long epsilon = ServerDelayConstant)
    {
        UsingChecker((evt, combatData, agentData, skillData) =>
        {
            if (combatData.TryGetEffectEventsByGUID(effect, out var effectEvents))
            {
                return effectEvents.Any(other => other != evt && GetAgent(other).Is(GetAgent(evt)) && Math.Abs(other.Time - timeOffset - evt.Time) < epsilon);
            }
            return false;
        });
        return this;
    }
    internal EffectCastFinder UsingSecondaryEffectInvertedSrcChecker(GUID effect, long timeOffset = 0, long epsilon = ServerDelayConstant)
    {
        UsingChecker((evt, combatData, agentData, skillData) =>
        {
            if (combatData.TryGetEffectEventsByGUID(effect, out var effectEvents))
            {
                return effectEvents.Any(other => other != evt && GetOtherAgent(other).Is(GetAgent(evt)) && Math.Abs(other.Time - timeOffset - evt.Time) < epsilon);
            }
            return false;
        });
        return this;
    }

    internal EffectCastFinder UsingSecondaryEffectSameSrcInvertedTypeChecker(GUID effect, long timeOffset = 0, long epsilon = ServerDelayConstant)
    {
        UsingChecker((evt, combatData, agentData, skillData) =>
        {
            if (combatData.TryGetEffectEventsByGUID(effect, out var effectEvents))
            {
                return effectEvents.Any(other => other != evt && evt.IsAroundDst != other.IsAroundDst && GetAgent(other).Is(GetAgent(evt)) && Math.Abs(other.Time - timeOffset - evt.Time) < epsilon);
            }
            return false;
        });
        return this;
    }

    internal EffectCastFinder UsingSecondaryEffectInvertedSrcInvertedTypeChecker(GUID effect, long timeOffset = 0, long epsilon = ServerDelayConstant)
    {
        UsingChecker((evt, combatData, agentData, skillData) =>
        {
            if (combatData.TryGetEffectEventsByGUID(effect, out var effectEvents))
            {
                return effectEvents.Any(other => other != evt && evt.IsAroundDst != other.IsAroundDst && GetOtherAgent(other).Is(GetAgent(evt)) && Math.Abs(other.Time - timeOffset - evt.Time) < epsilon);
            }
            return false;
        });
        return this;
    }

    internal EffectCastFinder UsingSecondaryEffectSameSrcSameTypeChecker(GUID effect, long timeOffset = 0, long epsilon = ServerDelayConstant)
    {
        UsingChecker((evt, combatData, agentData, skillData) =>
        {
            if (combatData.TryGetEffectEventsByGUID(effect, out var effectEvents))
            {
                return effectEvents.Any(other => other != evt && evt.IsAroundDst == other.IsAroundDst && GetAgent(other).Is(GetAgent(evt)) && Math.Abs(other.Time - timeOffset - evt.Time) < epsilon);
            }
            return false;
        });
        return this;
    }

    internal EffectCastFinder UsingSecondaryEffectInvertedSrcSameTypeChecker(GUID effect, long timeOffset = 0, long epsilon = ServerDelayConstant)
    {
        UsingChecker((evt, combatData, agentData, skillData) =>
        {
            if (combatData.TryGetEffectEventsByGUID(effect, out var effectEvents))
            {
                return effectEvents.Any(other => other != evt && evt.IsAroundDst == other.IsAroundDst && GetOtherAgent(other).Is(GetAgent(evt)) && Math.Abs(other.Time - timeOffset - evt.Time) < epsilon);
            }
            return false;
        });
        return this;
    }

    internal EffectCastFinder UsingNoSecondaryEffectSameSrcChecker(GUID effect, long timeOffset = 0, long epsilon = ServerDelayConstant)
    {
        UsingChecker((evt, combatData, agentData, skillData) =>
        {
            if (combatData.TryGetEffectEventsByGUID(effect, out var effectEvents))
            {
                return !effectEvents.Any(other => other != evt && GetAgent(other).Is(GetAgent(evt)) && Math.Abs(other.Time - timeOffset - evt.Time) < epsilon);
            }
            return true;
        });
        return this;
    }
    internal EffectCastFinder UsingNoSecondaryEffectInvertedSrcChecker(GUID effect, long timeOffset = 0, long epsilon = ServerDelayConstant)
    {
        UsingChecker((evt, combatData, agentData, skillData) =>
        {
            if (combatData.TryGetEffectEventsByGUID(effect, out var effectEvents))
            {
                return !effectEvents.Any(other => other != evt && GetOtherAgent(other).Is(GetAgent(evt)) && Math.Abs(other.Time - timeOffset - evt.Time) < epsilon);
            }
            return true;
        });
        return this;
    }

    internal EffectCastFinder UsingNoSecondaryEffectSameSrcInvertedTypeChecker(GUID effect, long timeOffset = 0, long epsilon = ServerDelayConstant)
    {
        UsingChecker((evt, combatData, agentData, skillData) =>
        {
            if (combatData.TryGetEffectEventsByGUID(effect, out var effectEvents))
            {
                return !effectEvents.Any(other => other != evt && evt.IsAroundDst != other.IsAroundDst && GetAgent(other).Is(GetAgent(evt)) && Math.Abs(other.Time - timeOffset - evt.Time) < epsilon);
            }
            return true;
        });
        return this;
    }

    internal EffectCastFinder UsingNoSecondaryEffectInvertedSrcInvertedTypeChecker(GUID effect, long timeOffset = 0, long epsilon = ServerDelayConstant)
    {
        UsingChecker((evt, combatData, agentData, skillData) =>
        {
            if (combatData.TryGetEffectEventsByGUID(effect, out var effectEvents))
            {
                return !effectEvents.Any(other => other != evt && evt.IsAroundDst != other.IsAroundDst && GetOtherAgent(other).Is(GetAgent(evt)) && Math.Abs(other.Time - timeOffset - evt.Time) < epsilon);
            }
            return true;
        });
        return this;
    }

    internal EffectCastFinder UsingNoSecondaryEffectSameSrcSameTypeChecker(GUID effect, long timeOffset = 0, long epsilon = ServerDelayConstant)
    {
        UsingChecker((evt, combatData, agentData, skillData) =>
        {
            if (combatData.TryGetEffectEventsByGUID(effect, out var effectEvents))
            {
                return !effectEvents.Any(other => other != evt && evt.IsAroundDst == other.IsAroundDst && GetAgent(other).Is(GetAgent(evt)) && Math.Abs(other.Time - timeOffset - evt.Time) < epsilon);
            }
            return true;
        });
        return this;
    }

    internal EffectCastFinder UsingNoSecondaryEffectInvertedSrcSameTypeChecker(GUID effect, long timeOffset = 0, long epsilon = ServerDelayConstant)
    {
        UsingChecker((evt, combatData, agentData, skillData) =>
        {
            if (combatData.TryGetEffectEventsByGUID(effect, out var effectEvents))
            {
                return !effectEvents.Any(other => other != evt && evt.IsAroundDst == other.IsAroundDst && GetOtherAgent(other).Is(GetAgent(evt)) && Math.Abs(other.Time - timeOffset - evt.Time) < epsilon);
            }
            return true;
        });
        return this;
    }

    internal EffectCastFinder UsingNoAnimatedCastChecker(long skillID, long timeOffset = 0, long epsilon = ServerDelayConstant)
    {
        UsingChecker((effect, combatData, agentData, skillData) => !combatData.IsCasting(skillID, effect.Src, effect.Time + timeOffset, epsilon));
        return this;
    }

#if DEBUG
    protected virtual bool DebugEffectChecker(EffectEvent evt, CombatData combatData, AgentData agentData, SkillData skillData)
    {
        var test = combatData.GetEffectEventsBySrc(evt.Src).Where(x => Math.Abs(x.Time - evt.Time) <= ServerDelayConstant && x.EffectID != evt.EffectID);
        var testGUIDs = test.Select(x => x.GUIDEvent);
        var test2 = combatData.GetEffectEventsByDst(evt.Src).Where(x => Math.Abs(x.Time - evt.Time) <= ServerDelayConstant && x.EffectID != evt.EffectID);
        var test2GUIDs = test2.Select(x => x.GUIDEvent);
        return true;
    }

    internal EffectCastFinder UsingDebugEffectChecker()
    {
        UsingChecker(DebugEffectChecker);
        return this;
    }
#endif

    internal EffectCastFinder UsingAgentRedirectionIfUnknown(int speciesID)
    {
        _speciesID = speciesID;
        return this;
    }

    public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
    {
        var res = new List<InstantCastEvent>();
        var effectGUIDEvent = combatData.GetEffectGUIDEventByGUID(_effectGUID);
        var effects = combatData.GetEffectEventsByEffectID(effectGUIDEvent.ContentID).GroupBy(GetAgent);
        foreach (var group in effects)
        {
            if (group.Key.IsUnknown)
            {
                continue;
            }
            long lastTime = int.MinValue;
            foreach (EffectEvent effectEvent in group)
            {
                if (CheckCondition(effectEvent, combatData, agentData, skillData))
                {
                    if (effectEvent.Time - lastTime < ICD)
                    {
                        lastTime = effectEvent.Time;
                        continue;
                    }
                    lastTime = effectEvent.Time;
                    var caster = group.Key;
                    if (_speciesID > 0 && caster.IsUnamedSpecies())
                    {
                        AgentItem? agent = agentData.GetNPCsByID(_speciesID).FirstOrDefault(x => x.LastAware >= effectEvent.Time && x.FirstAware <= effectEvent.Time);
                        if (agent != null)
                        {
                            caster = agent;
                        }
                    }
                    res.Add(new InstantCastEvent(GetTime(effectEvent, GetKeyAgent(effectEvent), combatData), skillData.Get(SkillID), caster));
                }
            }
        }
        return res;
    }
}
