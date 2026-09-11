using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using GW2EIParserAvalonia.Services;

namespace GW2EIParserAvalonia.Models;

public sealed class EventModel
{
    public object Event { get; }
    public long? Time { get; }
    public string Type { get; }

    public long? SkillId { get; }
    public string? SkillName { get; }
    public string? Guid { get; }

    public IReadOnlySet<ulong> AgentIds { get; }

    public EventModel(object @event)
    {
        Event = @event;
        Type = @event.GetType().Name;

        if (@event is TimeCombatEvent timeEvent)
        {
            Time = timeEvent.Time;
        }

        switch (@event)
        {
            case CastEvent cast:
                SkillId = cast.SkillID;
                SkillName = cast.Skill.Name;
                break;

            case SkillEvent skill:
                SkillId = skill.SkillID;
                SkillName = skill.Skill.Name;
                break;

            case BuffEvent buff:
                SkillId = buff.BuffID;
                SkillName = buff.BuffSkill.Name;
                break;

            case IDToGUIDEvent guid:
                Guid = guid.GUID.ToString();
                break;

            case EffectEvent effect:
                Guid = effect.GUIDEvent.GUID.ToString();
                break;
        }

        AgentIds = EventAgentResolver.Resolve(@event);
    }
}
