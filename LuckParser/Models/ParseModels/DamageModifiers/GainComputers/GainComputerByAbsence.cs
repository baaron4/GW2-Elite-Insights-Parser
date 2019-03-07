using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class GainComputerByAbsence : GainComputer
    {
        public GainComputerByAbsence()
        {
            Multiplier = true;
        }

        public override double ComputeGain(double gainPerStack, int stack)
        {
            return stack == 0 ? gainPerStack / (100 + gainPerStack) : 0;
        }
    }
}
