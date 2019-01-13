using System.Collections.Generic;
using System.ComponentModel;

namespace LuckParser.Models.HtmlModels
{  
    public class DeathRecapDto
    {
        [DefaultValue(null)]
        public long Time;
        public List<object[]> ToDown = null;
        public List<object[]> ToKill = null;
    }
}
