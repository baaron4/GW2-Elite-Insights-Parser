using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;

namespace GW2EIParser.Builders.HtmlModels
{
    public class BuffChartDataDto
    {
        public long Id { get; set; }
        public string Color { get; set; }
        public bool Visible { get; set; }
        public List<object[]> States { get; set; }

        public BuffChartDataDto(BuffsGraphModel bgm, List<Segment> bChart, PhaseData phase)
        {
            Id = bgm.Buff.ID;
            Visible = (bgm.Buff.Name == "Might" || bgm.Buff.Name == "Quickness" || bgm.Buff.Name == "Vulnerability");
            Color = GeneralHelper.GetLink("Color-" + bgm.Buff.Name);
            States = new List<object[]>(bChart.Count + 1);
            foreach (Segment seg in bChart)
            {
                double segStart = Math.Round(Math.Max(seg.Start - phase.Start, 0) / 1000.0, GeneralHelper.TimeDigit);
                States.Add(new object[] { segStart, seg.Value });
            }
            Segment lastSeg = bChart.Last();
            double segEnd = Math.Round(Math.Min(lastSeg.End - phase.Start, phase.End - phase.Start) / 1000.0, GeneralHelper.TimeDigit);
            States.Add(new object[] { segEnd, lastSeg.Value });
        }
    }
}
