using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class GainComputer
    {
        public bool Multiplier { get; protected set; }

        public abstract double ComputeGain(double gainPerStack, int stack);
    }
}
