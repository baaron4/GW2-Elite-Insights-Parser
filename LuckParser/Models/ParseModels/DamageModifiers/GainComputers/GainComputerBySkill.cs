using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class GainComputerBySkill : GainComputer
    {
        public GainComputerBySkill()
        {
            Multiplier = true;
        }

        public override double ComputeGain(double gainPerStack, int stack)
        {
            return stack > 0 ? 1.0 : 0.0;
        }
    }
}
