using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class GW2APISpec
    {
        // [Serializable]
       
            public int id { get; set; }
            public string name { get; set; }
            public string profession { get; set; }
            public bool elite { get; set; }
            //minor_traits
            //major_traits
            public string icon { get; set; }
            public string background { get; set; }

            public GW2APISpec() { }
            public bool getElite() { return elite; }
        


        
    }
}