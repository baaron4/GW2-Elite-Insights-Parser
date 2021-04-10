using System.Collections.Generic;
using Newtonsoft.Json;

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
            [JsonProperty]
            /// <summary>
            /// Time at which the event happened
            /// </summary>
            public long Time { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// The actor who is concerned by the mechanic
            /// </summary>
            public string Actor { get; internal set; }

            [JsonConstructor]
            internal JsonMechanic()
            {

            }

        }

        [JsonProperty]
        /// <summary>
        /// List of mechanics application
        /// </summary>
        public IReadOnlyList<JsonMechanic> MechanicsData { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Name of the mechanic
        /// </summary>
        public string Name { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Description of the mechanic
        /// </summary>
        public string Description { get; internal set; }

        [JsonConstructor]
        internal JsonMechanics()
        {

        }

    }
}
