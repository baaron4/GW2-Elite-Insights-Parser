using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Parser
{
    public abstract class ParseItem
    {
        // Commons
        public long Time { get; set; }
        public ulong SrcAgent { get; set; }
        public ulong DstAgent { get; set; }
        public long SkillID { get; }
        public ushort SrcInstid { get; set; }
        public ushort DstInstid { get; set; }
        public ushort SrcMasterInstid { get; set; }
        public ushort DstMasterInstid { get; set; }
        public byte IsNinety { get; }
        public byte IsFifty { get; }
        public byte IsMoving { get; }
        public byte IsFlanking { get; }
        public ParseEnum.IFF IFF { get; }
    }
}
