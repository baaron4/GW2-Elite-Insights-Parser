using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BoonsGraphModel
    {
        protected string boonname = null;
        protected List<Point> boonChart = new List<Point>();

        // Constructor
        public BoonsGraphModel(String boonname, List<Point> boonChart)
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
