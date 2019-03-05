using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BoonSourceFinder05032019 : BoonSourceFinder
    {
        public BoonSourceFinder05032019()
        {
            ExtensionIDS = new HashSet<long>()
            {
                10236,
                51696,
                29453
            };
            DurationToIDs = new Dictionary<long, HashSet<long>>
            {
                {3000, new HashSet<long> { 51696 , 10236 , 29453 } }, // SoI, Treated TN, SandSquall
                {2000, new HashSet<long> { 51696 } }, // TN
            };
        }
    }
}
