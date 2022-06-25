using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Artsariiv : ShatteredObservatory
    {
        public Artsariiv(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerBuffApplyMechanic(CorporealReassignment, "Corporeal Reassignment", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "Skull","Exploding Skull mechanic application","Corporeal Reassignment",0),
            new HitOnPlayerMechanic(Vault, "Vault", new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Yellow), "Vault","Vault from Big Adds", "Vault (Add)",0),
            new HitOnPlayerMechanic(SlamArtsariiv, "Slam", new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Slam","Slam (Vault) from Boss", "Vault (Arts)",0),
            new HitOnPlayerMechanic(TeleportLunge, "Teleport Lunge", new MechanicPlotlySetting(Symbols.StarTriangleDownOpen,Colors.LightOrange), "3 Jump","Triple Jump Mid->Edge", "Triple Jump",0),
            new HitOnPlayerMechanic(AstralSurge, "Astral Surge", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Floor Circle","Different sized spiraling circles", "1000 Circles",0),
            new HitOnPlayerMechanic(new long[] { RedMarble1, RedMarble2 }, "Red Marble", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Marble","Red KD Marble after Jump", "Red Marble",0),
            new PlayerBuffApplyMechanic(Fear, "Fear", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Red), "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)" ,0, (ba, log) => ba.AppliedDuration == 3000), //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new SpawnMechanic((int)ArcDPSEnums.TrashID.Spark, "Spark", new MechanicPlotlySetting(Symbols.Star,Colors.Teal),"Spark","Spawned a Spark (missed marble)", "Spark",0),
            });
            Extension = "arts";
            Icon = "https://i.imgur.com/aFlYs1I.png";
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/4wmuc8B.png",
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
                (int)ArcDPSEnums.TargetID.Artsariiv,
                (int)ArcDPSEnums.TrashID.CloneArtsariiv
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.TemporalAnomaly,
                ArcDPSEnums.TrashID.Spark,
                ArcDPSEnums.TrashID.SmallArtsariiv,
                ArcDPSEnums.TrashID.MediumArtsariiv,
                ArcDPSEnums.TrashID.BigArtsariiv,
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            // generic method for fractals
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor artsariiv = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Artsariiv);
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
                       (int)ArcDPSEnums.TrashID.CloneArtsariiv,
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

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            var artsariivs = new List<AgentItem>(agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.Artsariiv));
            if (artsariivs.Any())
            {
                artsariivs.Remove(artsariivs.MaxBy(x => x.LastAware - x.FirstAware));
                if (artsariivs.Any())
                {
                    foreach (AgentItem subartsariiv in artsariivs)
                    {
                        subartsariiv.OverrideID(ArcDPSEnums.TrashID.CloneArtsariiv);
                    }
                }
                agentData.Refresh();
            }
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, extensions);
            int count = 0;
            foreach (NPC trashMob in _trashMobs)
            {
                if (trashMob.ID == (int)ArcDPSEnums.TrashID.SmallArtsariiv)
                {
                    trashMob.OverrideName("Small " + trashMob.Character);
                }
                if (trashMob.ID == (int)ArcDPSEnums.TrashID.MediumArtsariiv)
                {
                    trashMob.OverrideName("Medium " + trashMob.Character);
                }
                if (trashMob.ID == (int)ArcDPSEnums.TrashID.BigArtsariiv)
                {
                    trashMob.OverrideName("Big " + trashMob.Character);
                }
            }
            foreach (NPC target in _targets)
            {
                if (target.ID == (int)ArcDPSEnums.TrashID.CloneArtsariiv)
                {
                    target.OverrideName("Clone " + target.Character + " " + (++count));
                }
            }
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterMode.CMNoName;
        }

        internal override long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            return GetFightOffsetByFirstInvulFilter(fightData, agentData, combatData, (int)ArcDPSEnums.TargetID.Artsariiv, Determined762, 1500);
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            // reward or death worked
            if (fightData.Success)
            {
                return;
            }
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Artsariiv);
            if (target == null)
            {
                throw new MissingKeyActorsException("Artsariiv not found");
            }
            SetSuccessByBuffCount(combatData, fightData, GetParticipatingPlayerAgents(target, combatData, playerAgents), target, Determined762, 4);
        }
    }
}
