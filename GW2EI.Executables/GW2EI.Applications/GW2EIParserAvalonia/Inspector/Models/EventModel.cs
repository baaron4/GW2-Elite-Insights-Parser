using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIParserAvalonia.Services.EventAgentResolver;

namespace GW2EIParserAvalonia.Models;

public sealed class EventModel
{
    public object Event { get; }
    public long? Time { get; }
    public string Type { get; }
    public Type EventType { get; }
    public long? SkillId { get; }
    public string? SkillName { get; }
    internal Guid GUIDStruct { get; }
    public string GUID { get; }
    public long ContentID { get; }
    public IReadOnlySet<ulong> AgentIds { get; }
    public IReadOnlySet<ulong> SourceAgentIds { get; }
    public IReadOnlySet<ulong> DestinationAgentIds { get; }
    internal int SourceIndex { get; set; }

    public EventModel(object @event)
    {
        Event = @event;
        EventType = @event.GetType();
        Type = EventType.Name;

        if (@event is TimeCombatEvent timeEvent)
        {
            Time = timeEvent.Time;
        }

        switch (@event)
        {
            case MissileEvent missileEvent:
                SkillId = missileEvent.SkillID;
                SkillName = missileEvent.Skill.Name;
                break;
            case BuffInfoEvent buffInfo:
                SkillId = buffInfo.BuffID;
                SkillName = buffInfo.BuffSkill.Name;
                break;
            case SkillInfoEvent skillInfo:
                SkillId = skillInfo.SkillID;
                SkillName = skillInfo.Skill.Name;
                break;
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
                GUIDStruct = guid.GetGUIDStruct();
                ContentID = guid.ContentID;
                break;

            case EffectEvent effect:
                GUIDStruct = effect.GUIDEvent.GetGUIDStruct();
                ContentID = effect.GUIDEvent.ContentID;
                break;
        }

        GUID = GUIDStruct.ToString("N").ToUpperInvariant();
        var agentResolution = ResolveDetailed(@event);

        AgentIds = agentResolution.All;
        SourceAgentIds = agentResolution.Source;
        DestinationAgentIds = agentResolution.Destination;
    }
}
