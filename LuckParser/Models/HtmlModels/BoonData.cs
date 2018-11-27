using System.Collections.Generic;
using System.ComponentModel;

namespace LuckParser.Models.HtmlModels
{   
    public class BoonData
    {
        [DefaultValue(null)]
        public double avg;       
        public List<List<object>> data = new List<List<object>>();
    }
}
