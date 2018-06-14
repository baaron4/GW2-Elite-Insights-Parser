using System.Collections.Generic;
using System.Drawing;

namespace LuckParser.Models.ParseModels
{
    public class BoonsGraphModel
    {
        private string boonname = null;
        private List<Point> boonChart = new List<Point>();

        // Constructor
        public BoonsGraphModel(string boonname)
        {
            this.boonname = boonname;
        }
        public BoonsGraphModel(string boonname, List<Point> boonChart)
        {
            this.boonname = boonname;
            this.boonChart = boonChart;
        }
        //getters
        public string getBoonName() {
            return this.boonname;
        }
        public List<Point> getBoonChart()
        {
            return this.boonChart;
        }

    }
}
