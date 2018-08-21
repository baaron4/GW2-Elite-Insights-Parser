using System.Collections.Generic;
using System.Drawing;

namespace LuckParser.Models.ParseModels
{
    public class BoonsGraphModel
    {
        private string _boonName = null;
        private List<Point> _boonChart = new List<Point>();

        // Constructor
        public BoonsGraphModel(string boonName)
        {
            _boonName = boonName;
        }
        public BoonsGraphModel(string boonName, List<Point> boonChart)
        {
            _boonName = boonName;
            _boonChart = boonChart;
        }
        //getters
        public string GetBoonName() {
            return _boonName;
        }
        public List<Point> GetBoonChart()
        {
            return _boonChart;
        }

    }
}
