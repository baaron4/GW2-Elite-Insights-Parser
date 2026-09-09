using GW2EIEvtcParser;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIParserAvalonia.InspectorContent;

public sealed class CombatItemModel
{
    public long Time { get; }
    public ulong SrcAgent { get; }
    public ulong DstAgent { get; }
    public int Value { get; }
    public int BuffDmg { get; }
    public uint OverstackValue { get; }
    public uint SkillID { get; }
    public ushort SrcInstid { get; }
    public ushort DstInstid { get; }
    public ushort SrcMasterInstid { get; }
    public ushort DstMasterInstid { get; }
    public byte IFFByte { get; }
    public IFF IFF { get; }
    public byte IsBuff { get; }
    public byte Result { get; }
    public byte IsActivationByte { get; }
    public Activation IsActivation { get; }
    public byte IsBuffRemoveByte { get; }
    public BuffRemove IsBuffRemove { get; }
    public byte IsNinety { get; }
    public byte IsFifty { get; }
    public byte IsMoving { get; }
    public StateChange IsStateChange { get; }
    public byte IsFlanking { get; }
    public byte IsShields { get; }
    public byte IsOffcycle { get; }
    public uint Pad { get; }
    public byte Pad1 { get; }
    public byte Pad2 { get; }
    public byte Pad3 { get; }
    public byte Pad4 { get; }

    public CombatItemModel(CombatItem item)
    {
        Time = item.Time;
        SrcAgent = item.SrcAgent;
        DstAgent = item.DstAgent;
        Value = item.Value;
        BuffDmg = item.BuffDmg;
        OverstackValue = item.OverstackValue;
        SkillID = item.SkillID;

        SrcInstid = item.SrcInstid;
        DstInstid = item.DstInstid;
        SrcMasterInstid = item.SrcMasterInstid;
        DstMasterInstid = item.DstMasterInstid;

        IFFByte = item.IFFByte;
        IFF = item.IFF;

        IsBuff = item.IsBuff;
        Result = item.Result;

        IsActivationByte = item.IsActivationByte;
        IsActivation = item.IsActivation;

        IsBuffRemoveByte = item.IsBuffRemoveByte;
        IsBuffRemove = item.IsBuffRemove;

        IsNinety = item.IsNinety;
        IsFifty = item.IsFifty;
        IsMoving = item.IsMoving;

        IsStateChange = item.IsStateChange;

        IsFlanking = item.IsFlanking;
        IsShields = item.IsShields;
        IsOffcycle = item.IsOffcycle;

        Pad = item.Pad;
        Pad1 = item.Pad1;
        Pad2 = item.Pad2;
        Pad3 = item.Pad3;
        Pad4 = item.Pad4;
    }
}
