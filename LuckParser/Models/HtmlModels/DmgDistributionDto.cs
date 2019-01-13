using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{  
    public class DmgDistributionDto
    {     
        public long ContributedDamage;     
        public long TotalDamage;      
        public List<object[]> Distribution;
    }
}
