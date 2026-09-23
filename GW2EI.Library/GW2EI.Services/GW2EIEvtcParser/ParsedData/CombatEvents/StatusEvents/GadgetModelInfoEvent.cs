namespace GW2EIEvtcParser.ParsedData;

public class GadgetModelInfoEvent : StatusEvent
{
    public readonly byte HidingBits;

    public readonly ulong Model;
    public readonly ulong PropID;

    internal unsafe GadgetModelInfoEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        HidingBits = evtcItem.IsFlanking;
        Model = evtcItem.DstAgent;

        var propID = stackalloc byte[8];

        *(Int32*)(propID) = evtcItem.Value;
        *(Int32*)(propID + sizeof(Int32)) = evtcItem.BuffDmg;
        PropID = *(UInt64*)(propID);
    }

}
