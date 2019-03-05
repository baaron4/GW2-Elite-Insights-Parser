using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BoonSourceFinder11122018 : BoonSourceFinder
    {

        public BoonSourceFinder11122018()
        {
            ExtensionIDS = new HashSet<long>()
            {
                10236,
                51696,
                29453
            };
            DurationToIDs = new Dictionary<long, HashSet<long>>
            {
                {5000, new HashSet<long> { 10236 } }, // SoI
                {3000, new HashSet<long> { 51696 } }, // Treated TN
                {2000, new HashSet<long> { 51696 , 29453 } }, // TN, SandSquall
            };
        }

    }
}
