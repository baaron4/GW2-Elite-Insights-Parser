using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class BoonFactory
    {
        // Factory
        public static AbstractBoon makeBoon(Boon boon)
        {
            if (boon.getType() == "intensity")
            {
                return new Intensity(boon.getCapacity());
            }
            else if (boon.getType() == "duration")
            {
                return new Duration(boon.getCapacity());
            }
            else
            {
                return null;
            }
        }
    }
}