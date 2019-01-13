using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{   
    public class TargetDto
    {
        public string Name;
        public string Icon;
        public long Health;
        public int CombatReplayID;
        public long HbWidth;
        public long HbHeight;
        public uint Tough;
        public readonly List<MinionDto> Minions = new List<MinionDto>();
        public double Percent;
        public int HpLeft;
    }
}
