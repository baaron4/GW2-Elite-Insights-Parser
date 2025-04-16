using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

internal class EffectCastFinder : CheckedCastFinder<EffectEvent>
{
    protected bool Minions = false;
    private readonly GUID _effectGUID;
    private int _speciesId = 0;

    public EffectCastFinder WithMinions(bool minions)
    {
        Minions = minions;
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
        return effectEvent.IsAroundDst? effectEvent.Dst: ParserHelper._unknownAgent;
    }

    public EffectCastFinder(long skillID, GUID effectGUID) : base(skillID)
    {
        UsingNotAccurate(true); // TODO: confirm if culling is server side logic
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

    internal EffectCastFinder UsingSecondaryEffectChecker(GUID effect, long timeOffset = 0, long epsilon = ServerDelayConstant)
    {
        UsingChecker((evt, combatData, agentData, skillData) =>
        {
            if (combatData.TryGetEffectEventsByGUID(effect, out var effectEvents))
            {
                return effectEvents.Any(other => other != evt && GetAgent(other) == GetAgent(evt) && Math.Abs(other.Time - timeOffset - evt.Time) < epsilon);
            }
            return false;
        });
        return this;
    }
    internal EffectCastFinder UsingSecondaryEffectCheckerInvertedSrc(GUID effect, long timeOffset = 0, long epsilon = ServerDelayConstant)
    {
        UsingChecker((evt, combatData, agentData, skillData) =>
        {
            if (combatData.TryGetEffectEventsByGUID(effect, out var effectEvents))
            {
                return effectEvents.Any(other => other != evt && GetOtherAgent(other) == GetAgent(evt) && Math.Abs(other.Time - timeOffset - evt.Time) < epsilon);
            }
            return false;
        });
        return this;
    }

    protected virtual bool DebugEffectChecker(EffectEvent evt, CombatData combatData, AgentData agentData, SkillData skillData)
    {
        var test = combatData.GetEffectEventsBySrc(evt.Src).Where(x => Math.Abs(x.Time - evt.Time) <= ServerDelayConstant && x.EffectID != evt.EffectID);
        var testGUIDs = test.Select(x => x.GUIDEvent.ContentGUID);
        var test2 = combatData.GetEffectEventsByDst(evt.Src).Where(x => Math.Abs(x.Time - evt.Time) <= ServerDelayConstant && x.EffectID != evt.EffectID);
        var test2GUIDs = test2.Select(x => x.GUIDEvent.ContentGUID);
        return true;
    }

    internal EffectCastFinder UsingDebugEffectChecker()
    {
        UsingChecker(DebugEffectChecker);
        return this;
    }

    internal EffectCastFinder UsingAgentRedirectionIfUnknown(int speciesID)
    {
        _speciesId = speciesID;
        return this;
    }

    public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
    {
        var res = new List<InstantCastEvent>();
        var effectGUIDEvent = combatData.GetEffectGUIDEvent(_effectGUID);
        if (effectGUIDEvent != null)
        {
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
                        if (_speciesId > 0 && caster.IsUnamedSpecies())
                        {
                            AgentItem? agent = agentData.GetNPCsByID(_speciesId).FirstOrDefault(x => x.LastAware >= effectEvent.Time && x.FirstAware <= effectEvent.Time);
                            if (agent != null)
                            {
                                caster = agent;
                            }
                        }
                        res.Add(new InstantCastEvent(GetTime(effectEvent, caster!, combatData), skillData.Get(SkillID), caster!));
                    }
                }
            }
        }
        return res;
    }
}
