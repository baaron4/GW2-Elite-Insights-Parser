using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{  
    public class DmgDistributionDto
    {     
        public long contributedDamage;     
        public long totalDamage;      
        public List<double[]> distribution;
    }
}
