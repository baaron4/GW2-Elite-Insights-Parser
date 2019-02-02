using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.HtmlModels
{
    public class BoonChartDataDto
    {
        public long Id;
        public string Color;
        public bool Visible;
        public List<object[]> States;

        public BoonChartDataDto(BoonsGraphModel bgm, List<BoonsGraphModel.Segment> bChart, PhaseData phase)
        {
            Id = bgm.Boon.ID;
            Visible = (bgm.Boon.Name == "Might" || bgm.Boon.Name == "Quickness" || bgm.Boon.Name == "Vulnerability");
            Color = GeneralHelper.GetLink("Color-" + bgm.Boon.Name);
            States = new List<object[]>(bChart.Count + 1);
            foreach (BoonsGraphModel.Segment seg in bChart)
            {
                double segStart = Math.Round(Math.Max(seg.Start - phase.Start, 0) / 1000.0, 3);
                States.Add(new object[] { segStart, seg.Value });
            }
            BoonsGraphModel.Segment lastSeg = bChart.Last();
            double segEnd = Math.Round(Math.Min(lastSeg.End - phase.Start, phase.End - phase.Start) / 1000.0, 3);
            States.Add(new object[] { segEnd, lastSeg.Value });
        }
    }
}
