using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public abstract class ProfHelper
    {

        private static readonly List<InstantCastFinder> _genericInstantCastFinders = new List<InstantCastFinder>()
        {
            new DamageCastFinder(9433, 9433, 500), // Earth Sigil
            new DamageCastFinder(9292, 9292, 500), // Air Sigil
            new DamageCastFinder(9428, 9428, 500), // Hydro Sigil
        };

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

        internal static void AttachMasterToRacialGadgets(List<Player> players, Dictionary<long, List<AbstractDamageEvent>> damageData, Dictionary<long, List<AbstractCastEvent>> castData)
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
                        ElementalistHelper.ElementalistInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Tempest":
                        ElementalistHelper.ElementalistInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        TempestHelper.TempestInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Weaver":
                        ElementalistHelper.ElementalistInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        WeaverHelper.WeaverInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case "Necromancer":
                        NecromancerHelper.NecromancerInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Reaper":
                        NecromancerHelper.NecromancerInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        ReaperHelper.ReaperInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Scourge":
                        NecromancerHelper.NecromancerInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        ScourgeHelper.ScourgeInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case "Mesmer":
                        MesmerHelper.MesmerInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Chronomancer":
                        MesmerHelper.MesmerInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        ChronomancerHelper.ChronomancerInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Mirage":
                        MesmerHelper.MesmerInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        MirageHelper.MirageInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        res.AddRange(MirageHelper.TranslateMirageCloak(combatData.GetBuffData(40408), skillData));
                        break;
                    //
                    case "Thief":
                        ThiefHelper.ThiefInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Daredevil":
                        ThiefHelper.ThiefInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        DaredevilHelper.DaredevilInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Deadeye":
                        ThiefHelper.ThiefInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        DeadeyeHelper.DeadeyeInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case "Engineer":
                        EngineerHelper.EngineerInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Scrapper":
                        EngineerHelper.EngineerInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        ScrapperHelper.ScrapperInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Holosmith":
                        EngineerHelper.EngineerInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        HolosmithHelper.HolosmithInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case "Ranger":
                        RangerHelper.RangerInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Druid":
                        RangerHelper.RangerInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        DruidHelper.DruidInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Soulbeast":
                        RangerHelper.RangerInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        SoulbeastHelper.SoulbeastInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case "Revenant":
                        RevenantHelper.RevenantInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Herald":
                        RevenantHelper.RevenantInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        HeraldHelper.HeraldInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Renegade":
                        RevenantHelper.RevenantInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        RenegadeHelper.RenegadeInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case "Guardian":
                        GuardianHelper.GuardianInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Dragonhunter":
                        GuardianHelper.GuardianInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        DragonhunterHelper.DragonhunterInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Firebrand":
                        GuardianHelper.GuardianInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        FirebrandHelper.FirebrandInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case "Warrior":
                        WarriorHelper.WarriorInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Berserker":
                        WarriorHelper.WarriorInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        BerserkerHelper.BerserkerInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case "Spellbreaker":
                        WarriorHelper.WarriorInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
                        SpellbreakerHelper.SpellbreakerInstantCastFinders.ForEach(x => instantCastFinders.Add(x));
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
                    if (icf.IgnoreInequality)
                    {
                        skillData.IgnoreInequalities.Add(icf.SkillID);
                    }
                    res.AddRange(icf.ComputeInstantCast(combatData, skillData, agentData));
                }
            }
            return res;
        }
    }
}
