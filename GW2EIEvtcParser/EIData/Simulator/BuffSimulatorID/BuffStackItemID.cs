using System;
using System.Collections.Generic;
using System.IO;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffStackItemID : BuffStackItem
    {

        public long StackID { get; protected set; } = 0;

        public bool Active { get; set; } = false;

        public BuffStackItemID(long start, long boonDuration, AgentItem src, bool active, long stackID) : base(start, boonDuration, src)
        {
            Active = active;
            StackID = stackID;
        }

        public void Activate()
        {
            Active = true;
        }

        public void Disable()
        {
            Active = false;
        }

        public override void Shift(long startShift, long durationShift)
        {
            if (!Active)
            {
                base.Shift(startShift, 0);
                return;
            }
            base.Shift(startShift, durationShift);
        }
    }
}

