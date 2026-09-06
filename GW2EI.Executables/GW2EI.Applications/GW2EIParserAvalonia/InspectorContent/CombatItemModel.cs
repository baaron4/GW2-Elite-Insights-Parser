using GW2EIEvtcParser;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIParserAvalonia.InspectorContent;

public sealed class CombatItemModel
{
    public long Time => _item.Time;
    public ulong SrcAgent => _item.SrcAgent;
    public ulong DstAgent => _item.DstAgent;
    public int Value => _item.Value;
    public int BuffDmg => _item.BuffDmg;
    public uint OverstackValue => _item.OverstackValue;
    public uint SkillID => _item.SkillID;
    public ushort SrcInstid => _item.SrcInstid;
    public ushort DstInstid => _item.DstInstid;
    public ushort SrcMasterInstid => _item.SrcMasterInstid;
    public ushort DstMasterInstid => _item.DstMasterInstid;
    public byte IFFByte => _item.IFFByte;
    public IFF IFF => _item.IFF;
    public byte IsBuff => _item.IsBuff;
    public byte Result => _item.Result;
    public byte IsActivationByte => _item.IsActivationByte;
    public Activation IsActivation => _item.IsActivation;
    public byte IsBuffRemoveByte => _item.IsBuffRemoveByte;
    public BuffRemove IsBuffRemove => _item.IsBuffRemove;
    public byte IsNinety => _item.IsNinety;
    public byte IsFifty => _item.IsFifty;
    public byte IsMoving => _item.IsMoving;
    public StateChange IsStateChange => _item.IsStateChange;
    public byte IsFlanking => _item.IsFlanking;
    public byte IsShields => _item.IsShields;
    public byte IsOffcycle => _item.IsOffcycle;
    public uint Pad => _item.Pad;
    public byte Pad1 => _item.Pad1;
    public byte Pad2 => _item.Pad2;
    public byte Pad3 => _item.Pad3;
    public byte Pad4 => _item.Pad4;

    private readonly CombatItem _item;

    public CombatItemModel(CombatItem item)
    {
        _item = item;
    }
}

