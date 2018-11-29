using System.Collections.Generic;
using System.ComponentModel;

namespace LuckParser.Models.HtmlModels
{  
    public class DeathRecapDto
    {
        public long id;
        [DefaultValue(null)]
        public long time;
        public List<object[]> toDown;
        public List<object[]> toKill;
    }
}
