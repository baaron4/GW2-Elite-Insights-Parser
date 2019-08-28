using System.Collections.Generic;
using LuckParser.EIData;
using LuckParser.Models;

namespace LuckParser.Builders.HtmlModels
{
    public class BoonData
    {
        public double Avg { get; set; }
        public List<List<object>> Data { get; set; } = new List<List<object>>();

        public BoonData(Dictionary<long, Statistics.FinalBuffs> boons, List<Boon> listToUse, double avg)
        {
            Avg = avg;
            foreach (Boon boon in listToUse)
            {
                var boonVals = new List<object>();
                Data.Add(boonVals);

                if (boons.TryGetValue(boon.ID, out Statistics.FinalBuffs uptime))
                {
                    boonVals.Add(uptime.Uptime);
                    if (boon.Type == Boon.BoonType.Intensity && uptime.Presence > 0)
                    {
                        boonVals.Add(uptime.Presence);
                    }
                }
            }
        }

        public BoonData(Dictionary<long, Statistics.FinalTargetBuffs> boons, List<Boon> listToUse, double avg)
        {
            Avg = avg;
            foreach (Boon boon in listToUse)
            {
                var boonVals = new List<object>();
                Data.Add(boonVals);

                if (boons.TryGetValue(boon.ID, out Statistics.FinalTargetBuffs uptime))
                {
                    boonVals.Add(uptime.Uptime);
                    if (boon.Type == Boon.BoonType.Intensity && uptime.Presence > 0)
                    {
                        boonVals.Add(uptime.Presence);
                    }
                }
            }
        }

        public BoonData(Dictionary<long, Statistics.FinalTargetBuffs> boons, List<Boon> listToUse, Player player)
        {
            foreach (Boon boon in listToUse)
            {
                var boonData = new List<object>();
                if (boons.TryGetValue(boon.ID, out Statistics.FinalTargetBuffs toUse))
                {
                    boonData.Add(toUse.Generated[player]);
                    boonData.Add(toUse.Overstacked[player]);
                    boonData.Add(toUse.Wasted[player]);
                    boonData.Add(toUse.UnknownExtension[player]);
                    boonData.Add(toUse.Extension[player]);
                    boonData.Add(toUse.Extended[player]);
                }
                Data.Add(boonData);
            }
        }

        public BoonData(List<Boon> listToUse, Dictionary<long, Statistics.FinalBuffs> uptimes)
        {
            foreach (Boon boon in listToUse)
            {
                if (uptimes.TryGetValue(boon.ID, out Statistics.FinalBuffs uptime))
                {
                    Data.Add(new List<object>()
                        {
                            uptime.Generation,
                            uptime.Overstack,
                            uptime.Wasted,
                            uptime.UnknownExtended,
                            uptime.ByExtension,
                            uptime.Extended
                        });
                }
                else
                {
                    Data.Add(new List<object>()
                        {
                            0,
                            0,
                            0,
                            0,
                            0,
                            0
                        });
                }
            }
        }

        public BoonData(string prof, Dictionary<string, List<Boon>> boonsBySpec, Dictionary<long, Statistics.FinalBuffs> boons)
        {
            foreach (Boon boon in boonsBySpec[prof])
            {
                var boonVals = new List<object>();
                Data.Add(boonVals);
                if (boons.TryGetValue(boon.ID, out Statistics.FinalBuffs uptime))
                {
                    boonVals.Add(uptime.Uptime);
                    if (boon.Type == Boon.BoonType.Intensity && uptime.Presence > 0)
                    {
                        boonVals.Add(uptime.Presence);
                    }
                }
                else
                {
                    boonVals.Add(0);
                }
            }
        }
    }
}
