using GW2EIEvtcParser.ParsedData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal class RangerHelper : ProfHelper
    {

        internal static readonly List<InstantCastFinder> RangerInstantCastFinders = new List<InstantCastFinder>()
        {
            new DamageCastFinder(12573,12573,InstantCastFinder.DefaultICD), // Hunter's Shot
            new DamageCastFinder(12507,12507,InstantCastFinder.DefaultICD), // Crippling Shot
            new BuffGiveCastFinder(33902,12633,InstantCastFinder.DefaultICD), // "Sic 'Em!"
            new BuffGiveCastFinder(56923,12633,InstantCastFinder.DefaultICD), // "Sic 'Em!" PvP
            new BuffGainCastFinder(12500,12543,InstantCastFinder.DefaultICD, (evt, combatData) => Math.Abs(evt.AppliedDuration - 6000) < ParserHelper.ServerDelayConstant), // Signet of Stone
            new BuffGainCastFinder(42470,12543,InstantCastFinder.DefaultICD, (evt, combatData) => Math.Abs(evt.AppliedDuration - 5000) < ParserHelper.ServerDelayConstant), // Lesser Signet of Stone
            new BuffGainCastFinder(12537,12536,InstantCastFinder.DefaultICD), // Sharpening Stone
        };

        public static void AttachMasterToRangerGadgets(List<Player> players, Dictionary<long, List<AbstractDamageEvent>> damageData, Dictionary<long, List<AbstractCastEvent>> castData)
        {
            var playerAgents = new HashSet<AgentItem>(players.Select(x => x.AgentItem));
            // entangle works fine already
            HashSet<AgentItem> jacarandaEmbraces = GetOffensiveGadgetAgents(damageData, 1286, playerAgents);
            HashSet<AgentItem> blackHoles = GetOffensiveGadgetAgents(damageData, 31436, playerAgents);
            var rangers = players.Where(x => x.Prof == "Ranger" || x.Prof == "Soulbeast" || x.Prof == "Druid").ToList();
            // if only one ranger, could only be that one
            if (rangers.Count == 1)
            {
                Player ranger = rangers[0];
                SetGadgetMaster(jacarandaEmbraces, ranger.AgentItem);
                SetGadgetMaster(blackHoles, ranger.AgentItem);
            }
            else if (rangers.Count > 1)
            {
                AttachMasterToGadgetByCastData(castData, jacarandaEmbraces, new List<long> { 44980 }, 1000);
                AttachMasterToGadgetByCastData(castData, blackHoles, new List<long> { 31503 }, 1000);
            }
        }

    }
}
