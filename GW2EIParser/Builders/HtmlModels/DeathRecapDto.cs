using System.Collections.Generic;
using GW2EIParser.EIData;

namespace GW2EIParser.Builders.HtmlModels
{
    public class DeathRecapDto
    {
        public long Time { get; set; }
        public List<object[]> ToDown { get; set; } = null;
        public List<object[]> ToKill { get; set; } = null;

        public static List<object[]> BuildDeathRecapItemList(List<Player.DeathRecap.DeathRecapDamageItem> list)
        {
            var data = new List<object[]>();
            foreach (Player.DeathRecap.DeathRecapDamageItem item in list)
            {
                data.Add(new object[]
                {
                            item.Time,
                            item.ID,
                            item.Damage,
                            item.Src,
                            item.IndirectDamage
                });
            }
            return data;
        }
    }
}
