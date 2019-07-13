using LuckParser.Models;
using System.Collections.Generic;

namespace LuckParser.Builders.HtmlModels
{
    public class DeathRecapDto
    {
        public long Time;
        public List<object[]> ToDown = null;
        public List<object[]> ToKill = null;

        public static List<object[]> BuildDeathRecapItemList(List<Statistics.DeathRecap.DeathRecapDamageItem> list)
        {
            List<object[]> data = new List<object[]>();
            foreach (Statistics.DeathRecap.DeathRecapDamageItem item in list)
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
