using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Artsariiv : ShatteredObservatory
    {
        public Artsariiv(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstBuffApplyMechanic(CorporealReassignmentBuff, "Corporeal Reassignment", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "Skull","Exploding Skull mechanic application","Corporeal Reassignment",0),
            new PlayerDstHitMechanic(VaultArtsariiv, "Vault", new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Yellow), "Vault","Vault from Big Adds", "Vault (Add)",0),
            new PlayerDstHitMechanic(SlamArtsariiv, "Slam", new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Slam","Slam (Vault) from Boss", "Vault (Arts)",0),
            new PlayerDstHitMechanic(TeleportLunge, "Teleport Lunge", new MechanicPlotlySetting(Symbols.StarTriangleDownOpen,Colors.LightOrange), "3 Jump","Triple Jump Mid->Edge", "Triple Jump",0),
            new PlayerDstHitMechanic(AstralSurge, "Astral Surge", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Floor Circle","Different sized spiraling circles", "1000 Circles",0),
            new PlayerDstHitMechanic(new long[] { RedMarble1, RedMarble2 }, "Red Marble", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Marble","Red KD Marble after Jump", "Red Marble",0),
            new PlayerDstBuffApplyMechanic(Fear, "Fear", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Red), "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)" ,0).UsingChecker((ba, log) => ba.AppliedDuration == 3000), //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new SpawnMechanic((int)TrashID.Spark, "Spark", new MechanicPlotlySetting(Symbols.Star,Colors.Teal),"Spark","Spawned a Spark (missed marble)", "Spark",0),
            });
            Extension = "arts";
            Icon = EncounterIconArtsariiv;
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayArtsariiv,
                            (914, 914),
                            (8991, 112, 11731, 2812)/*,
                            (-24576, -24576, 24576, 24576),
                            (11204, 4414, 13252, 6462)*/);
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>()
            {
                (int)TargetID.Artsariiv,
                (int)TrashID.CloneArtsariiv
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            var trashIDs = new List<ArcDPSEnums.TrashID>
            {
                TrashID.TemporalAnomaly,
                TrashID.Spark,
                TrashID.SmallArtsariiv,
                TrashID.MediumArtsariiv,
                TrashID.BigArtsariiv,
            };
            trashIDs.AddRange(base.GetTrashMobsIDs());
            return trashIDs;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            // generic method for fractals
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor artsariiv = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Artsariiv));
            if (artsariiv == null)
            {
                throw new MissingKeyActorsException("Artsariiv not found");
            }
            phases[0].AddTarget(artsariiv);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, Determined762, artsariiv, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    phase.Name = "Split " + (i) / 2;
                    var ids = new List<int>
                    {
                       (int)TrashID.CloneArtsariiv,
                    };
                    AddTargetsToPhaseAndFit(phase, ids, log);
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(artsariiv);
                }
            }
            return phases;
        }

        static readonly List<(string, Point3D)> CloneLocations = new List<(string, Point3D)> {
            ("M", new Point3D(10357.898f, 1466.580f)),
            ("NE", new Point3D(11431.998f, 2529.760f)),
            ("NW", new Point3D(9286.878f, 2512.429f)),
            ("SW", new Point3D(9284.729f, 392.916f)),
            ("SE", new Point3D(11422.698f, 401.501f)),
            ("N", new Point3D(10369.498f, 2529.010f)),
            ("E", new Point3D(11_432.598f, 1460.400f)),
            ("S", new Point3D(10_388.698f, 390.419f)),
            ("W", new Point3D(9295.668f, 1450.060f)),
        };

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            var artsariivs = new List<AgentItem>(agentData.GetNPCsByID(TargetID.Artsariiv));
            if (artsariivs.Any())
            {
                artsariivs.Remove(artsariivs.MaxBy(x => x.LastAware - x.FirstAware));
                if (artsariivs.Any())
                {
                    foreach (AgentItem subartsariiv in artsariivs)
                    {
                        subartsariiv.OverrideID(TrashID.CloneArtsariiv);
                    }
                }
                agentData.Refresh();
            }
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, extensions);
            foreach (NPC trashMob in _trashMobs)
            {
                if (trashMob.IsSpecies(TrashID.SmallArtsariiv))
                {
                    trashMob.OverrideName("Small " + trashMob.Character);
                }
                if (trashMob.IsSpecies(TrashID.MediumArtsariiv))
                {
                    trashMob.OverrideName("Medium " + trashMob.Character);
                }
                if (trashMob.IsSpecies(TrashID.BigArtsariiv))
                {
                    trashMob.OverrideName("Big " + trashMob.Character);
                }
            }

            var nameCount = new Dictionary<string, int> {
                    { "M", 1 }, { "NE", 1 }, { "NW", 1 }, { "SW", 1 }, { "SE", 1 }, // both split clones start at 1
                    { "N", 2 }, { "E", 2 }, { "S", 2 }, { "W", 2 }, // second split clones start at 2
            };
            foreach (NPC target in _targets)
            {
                if (target.IsSpecies(TrashID.CloneArtsariiv))
                {
                    string suffix = AddNameSuffixBasedOnInitialPosition(target, combatData, CloneLocations);
                    if (suffix != null && nameCount.ContainsKey(suffix))
                    {
                        // deduplicate name
                        target.OverrideName(target.Character + " " + (nameCount[suffix]++));
                    }
                }
            }
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterMode.CMNoName;
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            return GetFightOffsetByFirstInvulFilter(fightData, agentData, combatData, (int)TargetID.Artsariiv, Determined762);
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            // reward or death worked
            if (fightData.Success)
            {
                return;
            }
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Artsariiv));
            if (target == null)
            {
                throw new MissingKeyActorsException("Artsariiv not found");
            }
            SetSuccessByBuffCount(combatData, fightData, GetParticipatingPlayerAgents(target, combatData, playerAgents), target, Determined762, 4);
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            // Corporeal Reassignment
            IEnumerable<Segment> corpReass = p.GetBuffStatus(log, CorporealReassignmentBuff, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
            replay.AddOverheadIcons(corpReass, p, ParserIcons.SkullOverhead);
        }
    }
}
