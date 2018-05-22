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
        protected int boonid = -1;
        protected List<Point> boonChart = new List<Point>();

        // Constructor
        public BoonsGraphModel(string boonname, List<Point> boonChart, int boonid)
        {
            this.boonname = boonname;
            this.boonChart = boonChart;
            this.boonid = boonid;
        }
        //getters
        public string getBoonName() {
            return this.boonname;
        }
        public int getBoonId()
        {
            return this.boonid;
        }
        public List<Point> getBoonChart()
        {
            return this.boonChart;
        }

    }
}
