using GW2EIEvtcParser;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIParserAvalonia.Models;

public sealed class CombatItemModel
{
    public long Time => _combatItem.Time;
    public ulong SrcAgent => _combatItem.SrcAgent;
    public ulong DstAgent => _combatItem.DstAgent;
    public int Value => _combatItem.Value;
    public int BuffDmg => _combatItem.BuffDmg;
    public uint OverstackValue => _combatItem.OverstackValue;
    public uint SkillID => _combatItem.SkillID;
    public ushort SrcInstid => _combatItem.SrcInstid;
    public ushort DstInstid => _combatItem.DstInstid;
    public ushort SrcMasterInstid => _combatItem.SrcMasterInstid;
    public ushort DstMasterInstid => _combatItem.DstMasterInstid;
    public byte IFF => _combatItem.IFFByte;
    public IFF? IFFEnum => _combatItem.IsStateChange == StateChange.Combat ? _combatItem.IFF : null;
    public byte IsBuff => _combatItem.IsBuff;
    public byte Result => _combatItem.Result;
    public object? ResultEnum => _combatItem.IsDamageEvent() ?
            _combatItem.Build >= ArcDPSBuilds.ResultEnumRework ? 
                GetDamageResult(_combatItem.Result) 
                :
                _combatItem.IsDirectDamageEvent() ? GetDamageResult(_combatItem.Result) : GetConditionResult(_combatItem.Result)
            : 
            null;
    public byte IsActivation => _combatItem.IsActivationByte;
    public Activation? IsActivationEnum => _combatItem.IsCastEvent() ? _combatItem.IsActivation : null;
    public byte IsBuffRemove => _combatItem.IsBuffRemoveByte;
    public BuffRemove? IsBuffRemoveEnum => _combatItem.IsBuffApplyOrRemoveEvent() ? _combatItem.IsBuffRemove : null;
    public byte IsNinety => _combatItem.IsNinety;
    public byte IsFifty => _combatItem.IsFifty;
    public byte IsMoving => _combatItem.IsMoving;
    public StateChange IsStateChange => _combatItem.IsStateChange;
    public byte IsFlanking => _combatItem.IsFlanking;
    public byte IsShields => _combatItem.IsShields;
    public byte IsOffcycle => _combatItem.IsOffcycle;
    public uint Pad => _combatItem.Pad;
    public byte Pad1 => _combatItem.Pad1;
    public byte Pad2 => _combatItem.Pad2;
    public byte Pad3 => _combatItem.Pad3;
    public byte Pad4 => _combatItem.Pad4;

    private readonly CombatItem _combatItem;

    public CombatItemModel(CombatItem item)
    {
        _combatItem = item;
    }
}
