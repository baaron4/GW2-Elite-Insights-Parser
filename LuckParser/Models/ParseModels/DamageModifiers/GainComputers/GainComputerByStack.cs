using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class GainComputerByStack : GainComputer
    {
        public GainComputerByStack()
        {
            Multiplier = true;
        }

        public override double ComputeGain(double gainPerStack, int stack)
        {
            return gainPerStack * stack / (100 + stack * gainPerStack);
        }
    }
}
