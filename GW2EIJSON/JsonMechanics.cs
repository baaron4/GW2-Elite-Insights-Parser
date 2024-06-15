using System.Collections.Generic;


namespace GW2EIJSON
{
    /// <summary>
    /// Class corresponding to mechanics
    /// </summary>
    public class JsonMechanics
    {
        /// <summary>
        /// Class corresponding to a mechanic event
        /// </summary>
        public class JsonMechanic
        {

            /// <summary>
            /// Time at which the event happened
            /// </summary>
            public long Time { get; set; }

            /// <summary>
            /// The actor who is concerned by the mechanic
            /// </summary>
            public string Actor { get; set; }


            public JsonMechanic()
            {

            }

        }


        /// <summary>
        /// List of mechanics application
        /// </summary>
        public IReadOnlyList<JsonMechanic> MechanicsData { get; set; }

        /// <summary>
        /// Name of the mechanic, this is the short name as it appears on EI HTML Mechanic tables.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Non reduced name of the mechanic, this is the full name as it appears on EI HTML Graphs.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Description of the mechanic, this is the description that appears on hover on EI HTML Mechanic tables.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// If true, then the mechanic represent an achievement eligibility mechanic. \n
        /// Will only appear on successful encounters. \n
        /// Any Player who appears in <see cref="JsonMechanics.MechanicsData"/> will not be eligible for the achievement.
        /// </summary>
        public bool IsAchievementEligibility { get; set; }


        public JsonMechanics()
        {

        }

    }
}
