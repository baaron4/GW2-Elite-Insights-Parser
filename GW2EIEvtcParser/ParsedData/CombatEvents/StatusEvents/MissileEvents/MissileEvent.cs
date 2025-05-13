using System.Numerics;
using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser.ParsedData;

public class MissileEvent : StatusEvent
{
    /*
        ev->src_agent = (uintptr_t)src_ag;
        int16_t* i16 = (int16_t*)&ev->value;
        i16[0] = float_to_int16_nonprecise(xyz_origin[0], 10.0f); // 10 is the multiplier to use to get the original value eg. 50,000 in game data would be 5,000 in the i16
        i16[1] = float_to_int16_nonprecise(xyz_origin[1], 10.0f);
        i16[2] = float_to_int16_nonprecise(xyz_origin[2], 10.0f);
        ev->skillid = skillid;
        *(float*)&ev->iff = something_range;
        ev->is_statechange = CBTS_MISSILECREATE;
        ev->is_shields = flags0;
        ev->is_offcycle = flags1;
        *(uint32_t*)&ev->pad61 = trackable_id;
    */
    public readonly Vector3 Origin;
    public readonly SkillItem Skill;
    public long SkillID => Skill.ID;

    public readonly byte Flag0;
    public readonly byte Flag1;
    public readonly float SomethingRange;

    private readonly List<MissileLaunchEvent> _launchEvents = [];
    public IReadOnlyList<MissileLaunchEvent> LaunchEvents => _launchEvents;
    public MissileRemoveEvent? RemoveEvent { get; private set; }
    internal MissileEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData)
    {
        var originBytes = new ByteBuffer(stackalloc byte[4 * sizeof(short)]);
        // 1 
        originBytes.PushNative(evtcItem.Value);
        // 1
        originBytes.PushNative(evtcItem.BuffDmg);
        unsafe
        {
            fixed (byte* ptr = originBytes.Span)
            {
                var originShorts = (short*)ptr;
                Origin = new(
                        originShorts[0] * 10,
                        originShorts[1] * 10,
                        originShorts[2] * 10
                    );
            }
        }
        Skill = skillData.Get(evtcItem.SkillID);
        Flag0 = evtcItem.IsShields;
        Flag1 = evtcItem.IsOffcycle;
        var somethingRangeBytes = new ByteBuffer(stackalloc byte[sizeof(float)]);
        // 0.25
        somethingRangeBytes.PushNative(evtcItem.IFFByte);
        // 0.25
        somethingRangeBytes.PushNative(evtcItem.IsBuff);
        // 0.25
        somethingRangeBytes.PushNative(evtcItem.Result);
        // 0.25
        somethingRangeBytes.PushNative(evtcItem.IsActivationByte);
        SomethingRange = BitConverter.ToSingle(somethingRangeBytes);
    }

    internal void AddLaunchEvent(MissileLaunchEvent launchEvent)
    {
        _launchEvents.Add(launchEvent);
        launchEvent.Missile = this;
    }

    internal void SetRemoveEvent(MissileRemoveEvent removeEvent)
    {
        RemoveEvent = removeEvent;
        removeEvent.Missile = this;
    }

}
