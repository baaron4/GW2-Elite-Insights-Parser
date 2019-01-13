using System.Collections.Generic;
using System.ComponentModel;

namespace LuckParser.Models.HtmlModels
{   
    public class BoonData
    {
        [DefaultValue(null)]
        public double Avg;       
        public List<List<object>> Data = new List<List<object>>();
    }
}
