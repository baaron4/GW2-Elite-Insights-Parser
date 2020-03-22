using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public abstract class ProfHelper
    {

        public const long NumberOfConditionsID = -3;
        public const long NumberOfBoonsID = -2;
        public const long NoBuff = -4;

        // Weaver attunements
        public const long FireWater = -5;
        public const long FireAir = -6;
        public const long FireEarth = -7;
        public const long WaterFire = -8;
        public const long WaterAir = -9;
        public const long WaterEarth = -10;
        public const long AirFire = -11;
        public const long AirWater = -12;
        public const long AirEarth = -13;
        public const long EarthFire = -14;
        public const long EarthWater = -15;
        public const long EarthAir = -16;

        public const long FireDual = 43470;
        public const long WaterDual = 41166;
        public const long AirDual = 42264;
        public const long EarthDual = 44857;

        protected static void AttachMasterToGadgetByCastData(Dictionary<long, List<AbstractCastEvent>> castData, HashSet<AgentItem> gadgets, List<long> ids, long castEndThreshold)
        {
            var possibleCandidates = new HashSet<AgentItem>();
            var gadgetSpawnCastData = new List<AbstractCastEvent>();
            foreach (long id in ids)
            {
                if (castData.TryGetValue(id, out List<AbstractCastEvent> list))
                {
                    gadgetSpawnCastData.AddRange(list);
                }
            }
            gadgetSpawnCastData.Sort((x, y) => x.Time.CompareTo(y.Time));
            foreach (AbstractCastEvent castEvent in gadgetSpawnCastData)
            {
                long start = castEvent.Time;
                long end = start + castEvent.ActualDuration;
                possibleCandidates.Add(castEvent.Caster);
                foreach (AgentItem gadget in gadgets)
                {
                    if (gadget.FirstAware >= start && gadget.FirstAware <= end + castEndThreshold)
                    {
                        // more than one candidate, put to unknown and drop the search
                        if (gadget.Master != null && gadget.GetFinalMaster() != castEvent.Caster)
                        {
                            gadget.SetMaster(GeneralHelper.UnknownAgent);
                            break;
                        }
                        gadget.SetMaster(castEvent.Caster);
                    }
                }
            }
            if (possibleCandidates.Count == 1)
            {
                foreach (AgentItem gadget in gadgets)
                {
                    if (gadget.Master == null)
                    {
                        gadget.SetMaster(possibleCandidates.First());
                    }
                }
            }
        }

        protected static void SetGadgetMaster(HashSet<AgentItem> gadgets, AgentItem master)
        {
            foreach (AgentItem gadget in gadgets)
            {
                gadget.SetMaster(master);
            }
        }
    }
}
