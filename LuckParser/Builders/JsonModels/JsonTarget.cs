using System.Collections.Generic;

namespace LuckParser.Builders.JsonModels
{
    public class JsonTarget : JsonActor
    {
        /// <summary>
        /// Game ID of the target
        /// </summary>
        public ushort Id { get; set; }
        /// <summary>
        /// Total health of the target
        /// </summary>
        public int TotalHealth { get; set; }
        /// <summary>
        /// Final health of the target
        /// </summary>
        public int FinalHealth { get; set; }
        /// <summary>
        /// % of health burned
        /// </summary>
        public double HealthPercentBurned { get; set; }
        /// <summary>
        /// Time at which target became active
        /// </summary>
        public int FirstAware { get; set; }
        /// <summary>
        /// Time at which target became inactive 
        /// </summary>
        public int LastAware { get; set; }
        /// <summary>
        /// Array of average number of boons on the target \n
        /// Length == # of phases
        /// </summary>
        public List<double> AvgBoons { get; set; }
        /// <summary>
        /// Array of average number of conditions on the target \n
        /// Length == # of phases
        /// </summary>
        public List<double> AvgConditions { get; set; }
        /// <summary>
        /// Array of double[2] that represents the health status of the target \n
        /// Value[i][0] will be the time, value[i][1] will be health % \n
        /// If i corresponds to the last element that means the health did not change for the remainder of the fight \n
        /// </summary>
        public List<double[]> HealthPercents { get; set; }
        /// <summary>
        /// List of buff status
        /// </summary>
        /// <seealso cref="JsonTargetBuffs"/>
        public List<JsonTargetBuffs> Buffs { get; set; }
    }
}
