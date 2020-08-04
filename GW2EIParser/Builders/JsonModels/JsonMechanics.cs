using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIParser.Builders.JsonModels
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
            public long Time { get; }
            /// <summary>
            /// The actor who is concerned by the mechanic
            /// </summary>
            public string Actor { get; }

            public JsonMechanic(MechanicEvent ml)
            {
                Time = ml.Time;
                Actor = ml.Actor.Character;
            }

        }

        /// <summary>
        /// List of mechanics application
        /// </summary>
        public List<JsonMechanic> MechanicsData { get; }
        /// <summary>
        /// Name of the mechanic
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Description of the mechanic
        /// </summary>
        public string Description { get; }

        protected JsonMechanics(string name, string description, List<JsonMechanic> data)
        {
            Name = name;
            Description = description;
            MechanicsData = data;
        }

        public static List<JsonMechanics> GetJsonMechanicsList(List<MechanicEvent> mechanicLogs)
        {
            var mechanics = new List<JsonMechanics>();
            var dict = new Dictionary<string, (string desc, List<JsonMechanic> data)>();
            foreach (MechanicEvent ml in mechanicLogs)
            {
                var mech = new JsonMechanic(ml);
                if (dict.TryGetValue(ml.ShortName, out (string _, List<JsonMechanic> data) jsonMechData))
                {
                    jsonMechData.data.Add(mech);
                }
                else
                {
                    dict[ml.ShortName] = (ml.Description, new List<JsonMechanic> { mech});
                }
            }
            foreach (KeyValuePair<string, (string desc, List<JsonMechanic> data)> pair in dict)
            {
                mechanics.Add(new JsonMechanics(pair.Key, pair.Value.desc, pair.Value.data));
            }
            return mechanics;
        }

    }
}
