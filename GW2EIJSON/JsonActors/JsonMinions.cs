using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIJSON
{
    /// <summary>
    /// Class corresponding to the regrouping of the same type of minions
    /// </summary>
    public class JsonMinions
    {
        
        /// <summary>
        /// Name of the minion
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Total Damage done by minions \n
        /// Length == # of phases
        /// </summary>
        public IReadOnlyList<int> TotalDamage { get; set; }
        
        /// <summary>
        /// Damage done by minions against targets \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        public IReadOnlyList<IReadOnlyList<int>> TotalTargetDamage { get; set; }
        
        /// <summary>
        /// Total Breakbar Damage done by minions \n
        /// Length == # of phases
        /// </summary>
        public IReadOnlyList<double> TotalBreakbarDamage { get; set; }
        
        /// <summary>
        /// Breakbar Damage done by minions against targets \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        public IReadOnlyList<IReadOnlyList<double>> TotalTargetBreakbarDamage { get; set; }
        
        /// <summary>
        /// Total Shield Damage done by minions \n
        /// Length == # of phases
        /// </summary>
        public IReadOnlyList<int> TotalShieldDamage { get; set; }
        
        /// <summary>
        /// Shield Damage done by minions against targets \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        public IReadOnlyList<IReadOnlyList<int>> TotalTargetShieldDamage { get; set; }
        
        /// <summary>
        /// Total Damage distribution array \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public IReadOnlyList<IReadOnlyList<JsonDamageDist>> TotalDamageDist { get; set; }
        
        /// <summary>
        /// Per Target Damage distribution array \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public IReadOnlyList<IReadOnlyList<IReadOnlyList<JsonDamageDist>>> TargetDamageDist { get; set; }
        
        /// <summary>
        /// Rotation data
        /// </summary>
        /// <seealso cref="JsonRotation"/>
        public IReadOnlyList<JsonRotation> Rotation { get; set; }

        
        public JsonMinions()
        {

        }
    }
}
