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
                TrashID.TemporalAnomalyArtsariiv,
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
            base.ComputePlayerCombatReplayActors(p, log, replay);

            // Corporeal Reassignment (skull)
            IEnumerable<Segment> corpReass = p.GetBuffStatus(log, CorporealReassignmentBuff, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
            replay.AddOverheadIcons(corpReass, p, ParserIcons.SkullOverhead);
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);

            IReadOnlyList<AnimatedCastEvent> casts = log.CombatData.GetAnimatedCastData(target.AgentItem);
            switch (target.ID)
            {
                case (int)TargetID.Artsariiv:
                    foreach (AnimatedCastEvent cast in casts)
                    {
                        switch (cast.SkillId)
                        {
                            case Obliterate:
                                {
                                    int castStart = (int)cast.Time;
                                    int castEnd = castStart + 3160;
                                    replay.AddDecorationWithGrowing(new CircleDecoration(1300, (castStart, castEnd), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)), castEnd);
                                    (float, float)[][] positions = {
                                    // positions taken from effects
                                   new [] { (9286.88f, 2512.43f), (11432.0f, 2529.76f), (11422.7f, 401.501f), (9284.73f, 392.916f) },
                                   new [] { (10941.61f, 2044.3567f), (10934.861f, 889.46716f), (9772.5205f, 880.9314f), (9780.549f, 2030.362f) },
                                   new [] { (10116.815f, 1701.9971f), (10104.783f, 1213.3477f), (10602.564f, 1221.8499f), (10607.577f, 1713.7196f) },
                                   new [] { (10281.519f, 1390.1648f), (10429.899f, 1537.8489f), (10425.812f, 1398.6493f), (10295.681f, 1527.335f) },
                                };
                                    int[] radius = { 400, 290, 180, 70 };
                                    long nextInvul = log.CombatData.GetBuffData(Determined762).OfType<BuffApplyEvent>().FirstOrDefault(x => x.To == target.AgentItem && x.Time >= cast.Time)?.Time ?? log.FightData.FightEnd;
                                    for (int i = 0; i < 4; i++)
                                    {
                                        int start = castEnd + 560 * i;
                                        int end = start + 2450;
                                        if (start >= nextInvul)
                                        {
                                            break;
                                        }
                                        foreach ((float x, float y) in positions[i])
                                        {
                                            var position = new PositionConnector(new Point3D(x, y));
                                            replay.AddDecorationWithGrowing(new CircleDecoration(radius[i], (start, end), "rgba(250, 120, 0, 0.2)", position), end);
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                    break;
            }
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log);

            AddCorporealReassignmentDecorations(log);

            // Beaming Smile
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.ArtsariivBeamingSmileIndicator, out IReadOnlyList<EffectEvent> beamIndicators))
            {
                foreach (EffectEvent effect in beamIndicators)
                {
                    int start = (int)effect.Time;
                    int end = start + 2640;
                    AddBeamingSmileDecoration(effect, (start, end), "rgba(250, 120, 0, 0.2)");
                }
            }
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.ArtsariivBeamingSmile, out IReadOnlyList<EffectEvent> beams))
            {
                foreach (EffectEvent effect in beams)
                {
                    int start = (int)effect.Time;
                    int end = start + 300;
                    AddBeamingSmileDecoration(effect, (start, end), "rgba(255, 0, 0, 0.2)");
                }
            }
        }

        private void AddBeamingSmileDecoration(EffectEvent effect, (int, int) lifespan, string color)
        {
            const int length = 2500;
            const int hitbox = 360;
            var rotation = new AngleConnector(effect.Rotation.Z);
            GeographicalConnector position = new PositionConnector(effect.Position).WithOffset(new Point3D(0.0f, length / 2.0f), true);
            EnvironmentDecorations.Add(new RectangleDecoration(360, length + hitbox, lifespan, color, position).UsingRotationConnector(rotation));
        }

        internal override List<AbstractCastEvent> SpecialCastEventProcess(CombatData combatData, SkillData skillData)
        {
            List<AbstractCastEvent> res = base.SpecialCastEventProcess(combatData, skillData);
            res.AddRange(ProfHelper.ComputeUnderBuffCastEvents(combatData, skillData, NovaLaunchSAK, NovaLaunchBuff));
            return res;
        }
    }
}
