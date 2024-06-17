using System.Collections.Generic;

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
        /// Game ID of the minion
        /// </summary>
        public int Id { get; set; }

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

        /// <summary>
        /// Healing stats data
        /// </summary>
        public EXTJsonMinionsHealingStats EXTHealingStats { get; set; }

        /// <summary>
        /// Barrier stats data
        /// </summary>
        public EXTJsonMinionsBarrierStats EXTBarrierStats { get; set; }
        /// <summary>
        /// Contains combat replay related data for each individual minion instance
        /// </summary>
        /// <seealso cref="JsonActorCombatReplayData"/>
        public IReadOnlyList<JsonActorCombatReplayData> CombatReplayData { get; set; }


        public JsonMinions()
        {

        }
    }
}
