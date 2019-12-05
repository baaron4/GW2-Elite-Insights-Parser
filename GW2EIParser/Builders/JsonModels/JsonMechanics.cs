using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData.CombatEvents;

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
            /// Time a which the event happened
            /// </summary>
            public long Time { get; }
            /// <summary>
            /// The actor who was hit by the mechanic
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

        protected JsonMechanics(string name, List<JsonMechanic> data)
        {
            Name = name;
            MechanicsData = data;
        }

        public static List<JsonMechanics> GetJsonMechanicsList(List<MechanicEvent> mechanicLogs)
        {
            var mechanics = new List<JsonMechanics>();
            var dict = new Dictionary<string, List<JsonMechanic>>();
            foreach (MechanicEvent ml in mechanicLogs)
            {
                var mech = new JsonMechanic(ml);
                if (dict.TryGetValue(ml.InGameName, out List<JsonMechanic> list))
                {
                    list.Add(mech);
                }
                else
                {
                    dict[ml.InGameName] = new List<JsonMechanic>()
                        {
                            mech
                        };
                }
            }
            foreach (KeyValuePair<string, List<JsonMechanic>> pair in dict)
            {
                mechanics.Add(new JsonMechanics(pair.Key, pair.Value));
            }
            return mechanics;
        }

    }
}
