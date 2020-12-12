using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal static class ProfHelper
    {

        private static readonly List<InstantCastFinder> _genericInstantCastFinders = new List<InstantCastFinder>()
        {
            new DamageCastFinder(9433, 9433, 500), // Earth Sigil
            new DamageCastFinder(9292, 9292, 500), // Air Sigil
            new DamageCastFinder(9428, 9428, 500), // Hydro Sigil
        };

        internal static void AttachMasterToGadgetByCastData(Dictionary<long, List<AbstractCastEvent>> castData, HashSet<AgentItem> gadgets, List<long> castIDS, long castEndThreshold)
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
                            gadget.SetMaster(ParserHelper._unknownAgent);
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

        internal static HashSet<AgentItem> GetOffensiveGadgetAgents(Dictionary<long, List<AbstractHealthDamageEvent>> damageData, long damageSkillID, HashSet<AgentItem> playerAgents)
        {
            var res = new HashSet<AgentItem>();
            if (damageData.TryGetValue(damageSkillID, out List<AbstractHealthDamageEvent> list))
            {
                foreach (AbstractHealthDamageEvent evt in list)
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

        internal static void SetGadgetMaster(HashSet<AgentItem> gadgets, AgentItem master)
        {
            foreach (AgentItem gadget in gadgets)
            {
                gadget.SetMaster(master);
            }
        }

        internal static void AttachMasterToRacialGadgets(List<Player> players, Dictionary<long, List<AbstractHealthDamageEvent>> damageData, Dictionary<long, List<AbstractCastEvent>> castData)
        {
            var playerAgents = new HashSet<AgentItem>(players.Select(x => x.AgentItem));
            // Sylvari stuff
            HashSet<AgentItem> seedTurrets = GetOffensiveGadgetAgents(damageData, 12455, playerAgents);
            HashSet<AgentItem> graspingWines = GetOffensiveGadgetAgents(damageData, 1290, playerAgents);
            AttachMasterToGadgetByCastData(castData, seedTurrets, new List<long> { 12456, 12457 }, 1000);
            AttachMasterToGadgetByCastData(castData, graspingWines, new List<long> { 12453 }, 1000);
            // melandru avatar works fine already
        }

        //
        public static List<InstantCastEvent> ComputeInstantCastEvents(List<Player> players, CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var instantCastFinders = new HashSet<InstantCastFinder>(_genericInstantCastFinders);
            var res = new List<InstantCastEvent>();
            foreach (Player p in players)
            {
                switch (p.Prof)
                {
                    //
                    case "Elementalist":
                        ElementalistHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Tempest":
                        ElementalistHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        TempestHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Weaver":
                        ElementalistHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        WeaverHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case "Necromancer":
                        NecromancerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Reaper":
                        NecromancerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        ReaperHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Scourge":
                        NecromancerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        ScourgeHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case "Mesmer":
                        MesmerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Chronomancer":
                        MesmerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        ChronomancerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Mirage":
                        MesmerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        MirageHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case "Thief":
                        ThiefHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Daredevil":
                        ThiefHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        DaredevilHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Deadeye":
                        ThiefHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        DeadeyeHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case "Engineer":
                        EngineerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Scrapper":
                        EngineerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        ScrapperHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Holosmith":
                        EngineerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        HolosmithHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case "Ranger":
                        RangerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Druid":
                        RangerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        DruidHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Soulbeast":
                        RangerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        SoulbeastHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case "Revenant":
                        RevenantHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Herald":
                        RevenantHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        HeraldHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Renegade":
                        RevenantHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        RenegadeHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case "Guardian":
                        GuardianHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Dragonhunter":
                        GuardianHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        DragonhunterHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Firebrand":
                        GuardianHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        FirebrandHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case "Warrior":
                        WarriorHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Berserker":
                        WarriorHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        BerserkerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Spellbreaker":
                        WarriorHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        SpellbreakerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                }
            }
            res.AddRange(ComputeInstantCastEvents(combatData, skillData, agentData, instantCastFinders.ToList()));
            return res;
        }

        private static List<InstantCastEvent> ComputeInstantCastEvents(CombatData combatData, SkillData skillData, AgentData agentData, List<InstantCastFinder> instantCastFinders)
        {
            var res = new List<InstantCastEvent>();
            ulong build = combatData.GetBuildEvent().Build;
            foreach (InstantCastFinder icf in instantCastFinders)
            {
                if (icf.Available(build))
                {
                    if (icf.NotAccurate)
                    {
                        skillData.NotAccurate.Add(icf.SkillID);
                    }
                    res.AddRange(icf.ComputeInstantCast(combatData, skillData, agentData));
                }
            }
            return res;
        }
    }
}
