using LuckParser.Models.ParseModels;
using System.Collections.Generic;
using System.ComponentModel;

namespace LuckParser.Models.HtmlModels
{   
    public class BoonData
    {
        [DefaultValue(null)]
        public double Avg;       
        public List<List<object>> Data = new List<List<object>>();

        public BoonData()
        {

        }

        public BoonData(Dictionary<long, Statistics.FinalBuffs> boons, List<Boon> listToUse, long fightDuration, double avg)
        {
            Avg = avg;
            foreach (Boon boon in listToUse)
            {
                List<object> boonVals = new List<object>();
                Data.Add(boonVals);

                if (boons.TryGetValue(boon.ID, out var uptime))
                {
                    boonVals.Add(uptime.Uptime);
                    if (boon.Type == Boon.BoonType.Intensity && uptime.Presence > 0)
                    {
                        boonVals.Add(uptime.Presence);
                    }
                }
            }
        }

        public BoonData(List<Boon> listToUse, Dictionary<long, Statistics.FinalBuffs> uptimes)
        {
            foreach (Boon boon in listToUse)
            {
                if (uptimes.TryGetValue(boon.ID, out var uptime))
                {
                    Data.Add(new List<object>()
                        {
                            uptime.Generation,
                            uptime.Overstack,
                            uptime.Wasted,
                            uptime.UnknownExtension,
                            uptime.Extension,
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
                            0
                        });
                }
            }
        }

        public BoonData(string prof, Dictionary<string, List<Boon>> boonsBySpec, Dictionary<long, Statistics.FinalBuffs> boons)
        {
            foreach (Boon boon in boonsBySpec[prof])
            {
                List<object> boonVals = new List<object>();
                Data.Add(boonVals);
                if (boons.TryGetValue(boon.ID, out var uptime))
                {
                    boonVals.Add(uptime.Uptime);
                }
                else
                {
                    boonVals.Add(0);
                }
            }
        }

        public static List<object[]> GetDamageModifierData(List<long> boonToUse, Dictionary<long, List<AbstractMasterActor.ExtraBoonData>> extraBoonData, int phaseIndex)
        {
            List<object[]> data = new List<object[]>();
            foreach (long id in boonToUse)
            {
                if (extraBoonData.TryGetValue(id, out var extraDataList))
                {
                    var extraData = extraDataList[phaseIndex];
                    data.Add(new object[]
                                {
                                    extraData.HitCount,
                                    extraData.TotalHitCount,
                                    extraData.DamageGain,
                                    extraData.Multiplier ? extraData.TotalDamage : 0
                                });
                }
                else
                {
                    data.Add(new object[]
                                {
                                    0,
                                    0,
                                    0,
                                    0
                                });
                }
            }
            return data;
        }
    }
}
