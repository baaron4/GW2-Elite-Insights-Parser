using System;
using System.Collections.Generic;
using System.Linq;
using LuckParser.EIData;

namespace LuckParser.Builders.HtmlModels
{
    public class BoonChartDataDto
    {
        public long Id { get; set; }
        public string Color { get; set; }
        public bool Visible { get; set; }
        public List<object[]> States { get; set; }

        public BoonChartDataDto(BoonsGraphModel bgm, List<BoonsGraphModel.Segment> bChart, PhaseData phase)
        {
            Id = bgm.Boon.ID;
            Visible = (bgm.Boon.Name == "Might" || bgm.Boon.Name == "Quickness" || bgm.Boon.Name == "Vulnerability");
            Color = GeneralHelper.GetLink("Color-" + bgm.Boon.Name);
            States = new List<object[]>(bChart.Count + 1);
            foreach (BoonsGraphModel.Segment seg in bChart)
            {
                double segStart = Math.Round(Math.Max(seg.Start - phase.Start, 0) / 1000.0, GeneralHelper.TimeDigit);
                States.Add(new object[] { segStart, seg.Value });
            }
            BoonsGraphModel.Segment lastSeg = bChart.Last();
            double segEnd = Math.Round(Math.Min(lastSeg.End - phase.Start, phase.End - phase.Start) / 1000.0, GeneralHelper.TimeDigit);
            States.Add(new object[] { segEnd, lastSeg.Value });
        }
    }
}
