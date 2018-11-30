using System.Collections.Generic;
using System.ComponentModel;

namespace LuckParser.Models.HtmlModels
{  
    public class DeathRecapDto
    {
        [DefaultValue(null)]
        public long time;
        public List<object[]> toDown = null;
        public List<object[]> toKill = null;
    }
}
