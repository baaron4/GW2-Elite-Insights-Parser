using System.Collections.Generic;

namespace LuckParser.Builders.JsonModels
{
    /// <summary>
    /// Class corresponding to the regrouping of the same type of minions
    /// </summary>
    public class JsonMinions
    {
        /// <summary>
        /// Name of the minion
        /// </summary>
        public string Name;
        /// <summary>
        /// Total Damage done by minions \n
        /// Length == # of phases
        /// </summary>
        public List<int> TotalDamage;
        /// <summary>
        /// Damage done by minions against targets \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        public List<int>[] TotalTargetDamage;
        /// <summary>
        /// Total Shield Damage done by minions \n
        /// Length == # of phases
        /// </summary>
        public List<int> TotalShieldDamage;
        /// <summary>
        /// Shield Damage done by minions against targets \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        public List<int>[] TotalTargetShieldDamage;
        /// <summary>
        /// Total Damage distribution array \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<JsonDamageDist>[] TotalDamageDist;
        /// <summary>
        /// Per Target Damage distribution array \n
        /// Length == # of targets and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<JsonDamageDist>[][] TargetDamageDist;
        /// <summary>
        /// Rotation data
        /// </summary>
        /// <seealso cref="JsonRotation"/>
        public List<JsonRotation> Rotation;
    }
}
