using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class BoonSourceFinder
    {
        public abstract ushort TryFindSrc(AbstractActor a, List<CastLog> castsToCheck, long time, long extension, ParsedLog log);

    }
}
