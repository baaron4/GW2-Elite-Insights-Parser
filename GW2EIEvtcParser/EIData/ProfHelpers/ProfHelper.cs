using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class ProfHelper
    {

        private static readonly List<InstantCastFinder> _genericInstantCastFinders = new List<InstantCastFinder>()
        {
            new DamageCastFinder(SigilOfEarth, SigilOfEarth).UsingICD(500), // Earth Sigil
            new DamageCastFinder(SigilOfAir, SigilOfAir).UsingICD(500), // Air Sigil
            new DamageCastFinder(SigilOfHydromancy, SigilOfHydromancy).UsingICD(500), // Hydro Sigil
            new EXTHealingCastFinder(WaterBlastCombo1, WaterBlastCombo1), // Water Blast Combo
        };

        internal static void AttachMasterToGadgetByCastData(CombatData combatData, IReadOnlyCollection<AgentItem> gadgets, IReadOnlyList<long> castIDS, long castEndThreshold)
        {
            var possibleCandidates = new HashSet<AgentItem>();
            var gadgetSpawnCastData = new List<AnimatedCastEvent>();
            foreach (long id in castIDS)
            {
                gadgetSpawnCastData.AddRange(combatData.GetAnimatedCastData(id));
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

        internal static HashSet<AgentItem> GetOffensiveGadgetAgents(CombatData combatData, long damageSkillID, IReadOnlyCollection<AgentItem> playerAgents)
        {
            var res = new HashSet<AgentItem>();
            foreach (AbstractHealthDamageEvent evt in combatData.GetDamageData(damageSkillID))
            {
                // dst must no be a gadget nor a friendly player
                // src must be a masterless gadget
                if (!playerAgents.Contains(evt.To.GetFinalMaster()) && evt.To.Type != AgentItem.AgentType.Gadget && evt.From.Type == AgentItem.AgentType.Gadget && evt.From.Master == null)
                {
                    res.Add(evt.From);
                }
            }
            return res;
        }

        internal static void SetGadgetMaster(IReadOnlyCollection<AgentItem> gadgets, AgentItem master)
        {
            foreach (AgentItem gadget in gadgets)
            {
                gadget.SetMaster(master);
            }
        }

        internal static void AttachMasterToRacialGadgets(IReadOnlyList<Player> players, CombatData combatData)
        {
            var playerAgents = new HashSet<AgentItem>(players.Select(x => x.AgentItem));
            // Sylvari stuff
            HashSet<AgentItem> seedTurrets = GetOffensiveGadgetAgents(combatData, SeedTurretDamage, playerAgents);
            HashSet<AgentItem> graspingWines = GetOffensiveGadgetAgents(combatData, GraspingVinesDamage, playerAgents);
            AttachMasterToGadgetByCastData(combatData, seedTurrets, new List<long> { SeedTurret, TakeRootSkill }, 1000);
            AttachMasterToGadgetByCastData(combatData, graspingWines, new List<long> { GraspingVines }, 1000);
            // melandru avatar works fine already
        }

        //
        public static IReadOnlyList<InstantCastEvent> ComputeInstantCastEvents(IReadOnlyList<Player> players, CombatData combatData, SkillData skillData, AgentData agentData, FightLogic logic)
        {
            var instantCastFinders = new HashSet<InstantCastFinder>(_genericInstantCastFinders);
            logic.GetInstantCastFinders().ForEach(x => instantCastFinders.Add(x));
            var res = new List<InstantCastEvent>();
            foreach (Player p in players)
            {
                switch (p.Spec)
                {
                    //
                    case Spec.Elementalist:
                        ElementalistHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Tempest:
                        ElementalistHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        TempestHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Weaver:
                        ElementalistHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        WeaverHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Catalyst:
                        ElementalistHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        CatalystHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case Spec.Necromancer:
                        NecromancerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Reaper:
                        NecromancerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        ReaperHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Scourge:
                        NecromancerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        ScourgeHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Harbinger:
                        NecromancerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        HarbingerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case Spec.Mesmer:
                        MesmerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Chronomancer:
                        MesmerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        ChronomancerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Mirage:
                        MesmerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        MirageHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Virtuoso:
                        MesmerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        VirtuosoHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case Spec.Thief:
                        ThiefHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Daredevil:
                        ThiefHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        DaredevilHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Deadeye:
                        ThiefHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        DeadeyeHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Specter:
                        ThiefHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        SpecterHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case Spec.Engineer:
                        EngineerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Scrapper:
                        EngineerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        ScrapperHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Holosmith:
                        EngineerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        HolosmithHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Mechanist:
                        EngineerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        MechanistHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case Spec.Ranger:
                        RangerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Druid:
                        RangerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        DruidHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Soulbeast:
                        RangerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        SoulbeastHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Untamed:
                        RangerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        UntamedHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case Spec.Revenant:
                        RevenantHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Herald:
                        RevenantHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        HeraldHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Renegade:
                        RevenantHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        RenegadeHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Vindicator:
                        RevenantHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        VindicatorHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case Spec.Guardian:
                        GuardianHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Dragonhunter:
                        GuardianHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        DragonhunterHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Firebrand:
                        GuardianHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        FirebrandHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Willbender:
                        GuardianHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        WillbenderHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    //
                    case Spec.Warrior:
                        WarriorHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Berserker:
                        WarriorHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        BerserkerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Spellbreaker:
                        WarriorHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        SpellbreakerHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                    case Spec.Bladesworn:
                        WarriorHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        BladeswornHelper.InstantCastFinder.ForEach(x => instantCastFinders.Add(x));
                        break;
                }
            }
            res.AddRange(ComputeInstantCastEvents(combatData, skillData, agentData, instantCastFinders.ToList()));
            return res;
        }

        private static IReadOnlyList<InstantCastEvent> ComputeInstantCastEvents(CombatData combatData, SkillData skillData, AgentData agentData, IReadOnlyList<InstantCastFinder> instantCastFinders)
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

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            return;
        }

        internal static void DEBUG_ComputeProfessionCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            var knownEffects = new HashSet<long>();
            CombatReplay.DebugEffects(p, log, replay, knownEffects);
        }


        private static readonly HashSet<Spec> _canSummonClones = new HashSet<Spec>()
        {
            Spec.Mesmer,
            Spec.Chronomancer,
            Spec.Mirage
        };

        internal static bool CanSummonClones(Spec spec)
        {
            return _canSummonClones.Contains(spec);
        }

        private static HashSet<long> CommonMinions = new HashSet<long>()
        {
            (int)ArcDPSEnums.MinionID.RuneJaggedHorror,
            (int)ArcDPSEnums.MinionID.RuneMarkIGolem,
        };

        internal static bool IsKnownMinionID(AgentItem minion, Spec spec)
        {
            if (minion.Type == AgentItem.AgentType.Gadget)
            {
                return false;
            }
            long id = minion.ID;
            bool res = CommonMinions.Contains(id);
            switch (spec)
            {
                //
                case Spec.Elementalist:
                case Spec.Tempest:
                case Spec.Weaver:
                case Spec.Catalyst:
                    res |= ElementalistHelper.IsKnownMinionID(id);
                    break;
                //
                case Spec.Necromancer:
                case Spec.Scourge:
                case Spec.Harbinger:
                    res |= NecromancerHelper.IsKnownMinionID(id);
                    break;
                case Spec.Reaper:
                    res |= NecromancerHelper.IsKnownMinionID(id);
                    res |= ReaperHelper.IsKnownMinionID(id);
                    break;
                //
                case Spec.Mesmer:
                    res |= MesmerHelper.IsKnownMinionID(id);
                    break;
                case Spec.Chronomancer:
                    res |= MesmerHelper.IsKnownMinionID(id);
                    res |= ChronomancerHelper.IsKnownMinionID(id);
                    break;
                case Spec.Mirage:
                    res |= MesmerHelper.IsKnownMinionID(id);
                    res |= MirageHelper.IsKnownMinionID(id);
                    break;
                case Spec.Virtuoso:
                    res |= MesmerHelper.IsKnownMinionID(id);
                    res |= VirtuosoHelper.IsKnownMinionID(id);
                    break;
                //
                case Spec.Thief:
                    res |= ThiefHelper.IsKnownMinionID(id);
                    break;
                case Spec.Daredevil:
                    res |= ThiefHelper.IsKnownMinionID(id);
                    res |= DaredevilHelper.IsKnownMinionID(id);
                    break;
                case Spec.Deadeye:
                    res |= ThiefHelper.IsKnownMinionID(id);
                    res |= DeadeyeHelper.IsKnownMinionID(id);
                    break;
                case Spec.Specter:
                    res |= ThiefHelper.IsKnownMinionID(id);
                    res |= SpecterHelper.IsKnownMinionID(id);
                    break;
                //
                case Spec.Engineer:
                case Spec.Scrapper:
                case Spec.Holosmith:
                    res |= EngineerHelper.IsKnownMinionID(id);
                    break;
                case Spec.Mechanist:
                    res |= EngineerHelper.IsKnownMinionID(id);
                    res |= MechanistHelper.IsKnownMinionID(id);
                    break;
                //
                case Spec.Ranger:
                case Spec.Druid:
                case Spec.Soulbeast:
                case Spec.Untamed:
                    res |= RangerHelper.IsKnownMinionID(id);
                    break;
                //
                case Spec.Revenant:
                case Spec.Herald:
                case Spec.Vindicator:
                    res |= RevenantHelper.IsKnownMinionID(id);
                    break;
                case Spec.Renegade:
                    res |= RevenantHelper.IsKnownMinionID(id);
                    res |= RenegadeHelper.IsKnownMinionID(id);
                    break;
                //
                case Spec.Guardian:
                case Spec.Dragonhunter:
                case Spec.Firebrand:
                case Spec.Willbender:
                    res |= GuardianHelper.IsKnownMinionID(id);
                    break;
            }
            return res;
        }

        public static void ComputeMinionCombatReplayActors(AbstractSingleActor minion, AbstractSingleActor master, ParsedEvtcLog log, CombatReplay combatReplay)
        {

        }

    }
}
