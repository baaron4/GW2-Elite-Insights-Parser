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
        /// Name of the mechanic
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Description of the mechanic
        /// </summary>
        public string Description { get; set; }

        
        public JsonMechanics()
        {

        }

    }
}
