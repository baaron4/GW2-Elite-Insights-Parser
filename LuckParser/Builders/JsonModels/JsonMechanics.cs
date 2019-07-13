using System.Collections.Generic;

namespace LuckParser.Builders.JsonModels
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
            /// Time a which the event happened
            /// </summary>
            public long Time;
            /// <summary>
            /// The actor who was hit by the mechanic
            /// </summary>
            public string Actor;
        }

        /// <summary>
        /// List of mechanics application
        /// </summary>
        public List<JsonMechanic> MechanicsData;
        /// <summary>
        /// Name of the mechanic
        /// </summary>
        public string Name;
    }
}
