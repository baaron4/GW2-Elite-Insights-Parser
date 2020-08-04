using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public abstract class ProfHelper
    {

        public const long NumberOfConditionsID = -3;
        public const long NumberOfBoonsID = -2;
        public const long NumberOfActiveCombatMinions= -17;
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

        protected static void AttachMasterToGadgetByCastData(Dictionary<long, List<AbstractCastEvent>> castData, HashSet<AgentItem> gadgets, List<long> castIDS, long castEndThreshold)
        {
            var possibleCandidates = new HashSet<AgentItem>();
            var gadgetSpawnCastData = new List<AbstractCastEvent>();
            foreach (long id in castIDS)
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
                long end = castEvent.EndTime;
                possibleCandidates.Add(castEvent.Caster);
                foreach (AgentItem gadget in gadgets)
                {
                    if (gadget.FirstAware >= start && gadget.FirstAware <= end + castEndThreshold)
                    {
                        // more than one candidate, put to unknown and drop the search
                        if (gadget.Master != null && gadget.GetFinalMaster() != castEvent.Caster.GetFinalMaster())
                        {
                            gadget.SetMaster(ParseHelper.UnknownAgent);
                            break;
                        }
                        gadget.SetMaster(castEvent.Caster.GetFinalMaster());
                    }
                }
            }
            if (possibleCandidates.Count == 1)
            {
                foreach (AgentItem gadget in gadgets)
                {
                    if (gadget.Master == null)
                    {
                        gadget.SetMaster(possibleCandidates.First().GetFinalMaster());
                    }
                }
            }
        }

        protected static HashSet<AgentItem> GetOffensiveGadgetAgents(Dictionary<long, List<AbstractDamageEvent>> damageData, long damageSkillID, HashSet<AgentItem> playerAgents)
        {
            var res = new HashSet<AgentItem>();
            if (damageData.TryGetValue(damageSkillID, out List<AbstractDamageEvent> list))
            {
                foreach (AbstractDamageEvent evt in list)
                {
                    // dst must no be a gadget nor a friendly player
                    // src must be a masterless gadget
                    if (!playerAgents.Contains(evt.To.GetFinalMaster()) && evt.To.Type != AgentItem.AgentType.Gadget && evt.From.Type == AgentItem.AgentType.Gadget && evt.From.Master == null)
                    {
                        res.Add(evt.From);
                    }
                }
            }
            return res;
        }

        protected static void SetGadgetMaster(HashSet<AgentItem> gadgets, AgentItem master)
        {
            foreach (AgentItem gadget in gadgets)
            {
                gadget.SetMaster(master);
            }
        }

        //

        public static void AttachMasterToRacialGadgets(List<Player> players, Dictionary<long, List<AbstractDamageEvent>> damageData, Dictionary<long, List<AbstractCastEvent>> castData)
        {
            var playerAgents = new HashSet<AgentItem>(players.Select(x => x.AgentItem));
            // Sylvari stuff
            HashSet<AgentItem> seedTurrets = GetOffensiveGadgetAgents(damageData, 12455, playerAgents);
            HashSet<AgentItem> graspingWines = GetOffensiveGadgetAgents(damageData, 1290, playerAgents);
            AttachMasterToGadgetByCastData(castData, seedTurrets, new List<long> { 12456, 12457 }, 1000);
            AttachMasterToGadgetByCastData(castData, graspingWines, new List<long> { 12453 }, 1000);
            // melandru avatar works fine already
        }
    }
}
