using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Samarog : BastionOfThePenitent
    {
        public Samarog(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new PlayerDstHitMechanic(SamarogShockwave, "Shockwave", new MechanicPlotlySetting(Symbols.Circle,Colors.Blue), "Schk.Wv","Shockwave from Spears", "Shockwave",0).UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new PlayerDstHitMechanic(PrisonerSweep, "Prisoner Sweep", new MechanicPlotlySetting(Symbols.Hexagon,Colors.Blue), "Swp","Prisoner Sweep (horizontal)", "Sweep",0).UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new PlayerDstHitMechanic(TramplingRush, "Trampling Rush", new MechanicPlotlySetting(Symbols.TriangleRight,Colors.Red), "Trpl","Trampling Rush (hit by stampede towards home)", "Trampling Rush",0),
            new PlayerDstHitMechanic(Bludgeon , "Bludgeon", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Blue), "Slam","Bludgeon (vertical Slam)", "Slam",0),
            new PlayerDstBuffApplyMechanic(FixatedSamarog, "Fixate: Samarog", new MechanicPlotlySetting(Symbols.Star,Colors.Magenta), "S.Fix","Fixated by Samarog", "Fixate: Samarog",0),
            new PlayerDstBuffApplyMechanic(FixatedGuldhem, "Fixate: Guldhem", new MechanicPlotlySetting(Symbols.StarOpen,Colors.Orange), "G.Fix","Fixated by Guldhem", "Fixate: Guldhem",0),
            new PlayerDstBuffApplyMechanic(FixatedRigom, "Fixate: Rigom", new MechanicPlotlySetting(Symbols.StarOpen,Colors.Red), "R.Fix","Fixated by Rigom", "Fixate: Rigom",0),
            new PlayerDstBuffApplyMechanic(InevitableBetrayalBig, "Big Hug", new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "B.Gr","Big Green (friends mechanic)", "Big Green",0),
            new PlayerDstBuffApplyMechanic(InevitableBetrayalSmall, "Small Hug", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.DarkGreen), "S.Gr","Small Green (friends mechanic)", "Small Green",0),
            new EnemyDstBuffApplyMechanic(StrengthenedBondGuldhem, "Strengthened Bond: Guldhem", new MechanicPlotlySetting(Symbols.TriangleNE,Colors.Orange), "G.Str","Strengthened Bond: Guldhem", "Strengthened: Guldhem",0),
            new EnemyDstBuffApplyMechanic(StrengthenedBondRigom, "Strengthened Bond: Rigom", new MechanicPlotlySetting(Symbols.TriangleNE,Colors.Red), "R.Str","Strengthened Bond: Rigom", "Strengthened: Rigom",0),
            new PlayerDstHitMechanic(SpearReturn, "Spear Return", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Red), "S.Rt","Hit by Spear Return", "Spear Return",0),
            new PlayerDstHitMechanic(new long[] {InevitableBetrayalFailSmall, InevitableBetrayalFailBig}, "Inevitable Betrayal", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Gr.Fl","Inevitable Betrayal (failed Green)", "Failed Green",0),
            new PlayerDstHitMechanic(EffigyPulse, "Effigy Pulse", new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Red), "S.Pls","Effigy Pulse (Stood in Spear AoE)", "Spear Aoe",0),
            new PlayerDstHitMechanic(SpearImpact, "Spear Impact", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Red), "S.Spwn","Spear Impact (hit by spawning Spear)", "Spear Spawned",0),
            new PlayerDstBuffApplyMechanic(BrutalizeBuff, "Brutalized", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Magenta),"Brtlzd","Brutalized (jumped upon by Samarog->Breakbar)", "Brutalized",0),
            new EnemyCastEndMechanic(BrutalizeCast, "Brutalize (Jump End)", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal),"CC","Brutalize (Breakbar)", "Breakbar",0),
            new PlayerDstSkillMechanic(BrutalizeKill, "Brutalize (Killed)", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail","Brutalize (Failed CC)", "CC Fail",0).UsingChecker((de, log) => de.HasKilled),
            new EnemyDstBuffRemoveMechanic(FanaticalResilience, "Brutalize (End)", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CC End","Ended Brutalize", "CC Ended",0),
            //new PlayerBoonRemoveMechanic(BrutalizeEffect, "Brutalize", ParseEnum.BossIDS.Samarog, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed","Ended Brutalize (Breakbar broken)", "CCEnded",0),//(condition => condition.getCombatItem().IsBuffRemove == ParseEnum.BuffRemove.Manual)),
            //new Mechanic(BrutalizeEffect, "Brutalize", Mechanic.MechType.EnemyBoonStrip, ParseEnum.BossIDS.Samarog, new MechanicPlotlySetting(Symbols.DiamondTall,"rgb(110,160,0)"), "CCed1","Ended Brutalize (Breakbar broken)", "CCed1",0),//(condition => condition.getCombatItem().IsBuffRemove == ParseEnum.BuffRemove.All)),
            new PlayerDstBuffApplyMechanic(SoulSwarm, "Soul Swarm", new MechanicPlotlySetting(Symbols.XThinOpen,Colors.Teal),"Wall","Soul Swarm (stood in or beyond Spear Wall)", "Spear Wall",0),
            new PlayerDstHitMechanic(ImpalingStab, "Impaling Stab", new MechanicPlotlySetting(Symbols.Hourglass,Colors.Blue),"Shck.Wv Ctr","Impaling Stab (hit by Spears causing Shockwave)", "Shockwave Center",0),
            new PlayerDstHitMechanic(AnguishedBolt, "Anguished Bolt", new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange),"Stun","Anguished Bolt (AoE Stun Circle by Guldhem)", "Guldhem's Stun",0),
            
            //  new Mechanic(SpearImpact, "Brutalize", ParseEnum.BossIDS.Samarog, new MechanicPlotlySetting(Symbols.StarSquare,Color.Red), "CC Target", casted without dmg odd
            });
            Extension = "sam";
            Icon = EncounterIconSamarog;
            EncounterCategoryInformation.InSubCategoryOrder = 2;
            EncounterID |= 0x000003;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplaySamarog,
                            (1000, 959),
                            (-6526, 1218, -2423, 5146)/*,
                            (-27648, -9216, 27648, 12288),
                            (11774, 4480, 14078, 5376)*/);
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(BrutalAura, BrutalAura),
            };
        }

        internal override List<AbstractHealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, SkillData skillData)
        {
            AbstractSingleActor samarog = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Samarog));
            if (samarog == null)
            {
                throw new MissingKeyActorsException("Samarog not found");
            }
            IReadOnlyList<AbstractHealthDamageEvent> damageTaken = combatData.GetDamageTakenData(samarog.AgentItem);
            var fanaticalResilienceTimes = GetFilteredList(combatData, FanaticalResilience, samarog, true, false).Select(x => x.Time).ToList();
            var fanaticalResilienceSegments = new List<Segment>();
            for (int i = 0; i < fanaticalResilienceTimes.Count; i +=2)
            {
                long start = fanaticalResilienceTimes[i];
                long end = long.MaxValue;
                if (i + 1 < fanaticalResilienceTimes.Count)
                {
                    end = fanaticalResilienceTimes[i + 1];
                }
                fanaticalResilienceSegments.Add(new Segment(start, end, 1));
            }
            foreach (AbstractHealthDamageEvent healthDamageEvent in damageTaken)
            {
                // Can't have been absorbed if not 0 damages
                if (healthDamageEvent.HasHit && healthDamageEvent.HealthDamage == 0 && fanaticalResilienceSegments.Any(x => healthDamageEvent.Time >= x.Start && healthDamageEvent.Time <= x.End))
                {
                    healthDamageEvent.MakeIntoAbsorbed();
                }
            }
            return new List<AbstractHealthDamageEvent>();
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Samarog));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Samarog not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Determined check
            phases.AddRange(GetPhasesByInvul(log, Determined762, mainTarget, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    phase.Name = "Split " + i / 2;
                    var ids = new List<int>
                    {
                       (int) ArcDPSEnums.TrashID.Rigom,
                       (int) ArcDPSEnums.TrashID.Guldhem
                    };
                    AddTargetsToPhaseAndFit(phase, ids, log);
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(mainTarget);
                }
            }
            return phases;
        }

        internal override void EIEvtcParse(ulong gw2Build, int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            // With lingering agents, last aware of the spears are properly set
            if (evtcVersion >= ArcDPSEnums.ArcDPSBuilds.LingeringAgents)
            {
                var spearAgents = combatData.Where(x => x.DstAgent == 104580 && x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 100 && x.HitboxHeight == 300).ToList();
                if (spearAgents.Any())
                {
                    foreach (AgentItem spear in spearAgents)
                    {
                        spear.OverrideType(AgentItem.AgentType.NPC);
                        spear.OverrideID((int)ArcDPSEnums.TrashID.SpearAggressionRevulsion);
                    }
                    agentData.Refresh();
                }
            }
            base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
            int curGuldhem = 1;
            int curRigom = 1;
            foreach (AbstractSingleActor target in Targets)
            {
                if (target.IsSpecies(ArcDPSEnums.TrashID.Guldhem))
                {
                    target.OverrideName(target.Character + " " + curGuldhem++);
                }
                if (target.IsSpecies(ArcDPSEnums.TrashID.Rigom))
                {
                    target.OverrideName(target.Character + " " + curRigom++);
                }
            }
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Samarog,
                (int)ArcDPSEnums.TrashID.Rigom,
                (int)ArcDPSEnums.TrashID.Guldhem,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>() { 
                ArcDPSEnums.TrashID.SpearAggressionRevulsion
            };
        }


        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            // TODO: facing information (shock wave)
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Samarog:
                    var brutalize = target.GetBuffStatus(log, FanaticalResilience, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
                    foreach (Segment seg in brutalize)
                    {
                        replay.Decorations.Add(new CircleDecoration(120, seg, Colors.LightBlue, 0.3, new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.Rigom:
                case (int)ArcDPSEnums.TrashID.Guldhem:
                    break;
                case (int)ArcDPSEnums.TrashID.SpearAggressionRevulsion:
                    var spearLifespan = new Segment(target.FirstAware, target.LastAware, 1);
                    replay.Decorations.Add(new CircleDecoration(240, spearLifespan, Colors.Orange, 0.1, new AgentConnector(target)));
                    if (log.CombatData.GetBuffData(SpearOfAggressionBuff).Any(x => x.To == target.AgentItem))
                    {
                        replay.AddOverheadIcon(spearLifespan, target, BuffImages.Taunt, 15);
                    } 
                    else
                    {
                        replay.AddOverheadIcon(spearLifespan, target, BuffImages.Fear, 15);
                    }
                    break;
                default:
                    break;
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
            // big bomb
            var bigbomb = log.CombatData.GetBuffData(InevitableBetrayalBig).Where(x => (x.To == p.AgentItem && x is BuffApplyEvent)).ToList();
            foreach (AbstractBuffEvent c in bigbomb)
            {
                int bigStart = (int)c.Time;
                int bigEnd = bigStart + 6000;
                var circle = new CircleDecoration(300, (bigStart, bigEnd), "rgba(150, 80, 0, 0.2)", new AgentConnector(p));
                replay.AddDecorationWithGrowing(circle, bigEnd);
            }
            // small bomb
            var smallbomb = log.CombatData.GetBuffData(InevitableBetrayalSmall).Where(x => (x.To == p.AgentItem && x is BuffApplyEvent)).ToList();
            foreach (AbstractBuffEvent c in smallbomb)
            {
                int smallStart = (int)c.Time;
                int smallEnd = smallStart + 6000;
                replay.Decorations.Add(new CircleDecoration(80, (smallStart, smallEnd), "rgba(80, 150, 0, 0.3)", new AgentConnector(p)));
            }
            // fixated Samarog
            var fixatedSam = p.GetBuffStatus(log, FixatedSamarog, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            foreach (Segment seg in fixatedSam)
            {
                replay.Decorations.Add(new CircleDecoration(80, seg, "rgba(255, 80, 255, 0.3)", new AgentConnector(p)));
                replay.AddOverheadIcon(seg, p, ParserIcons.FixationPurpleOverhead);
            }
            List<AbstractBuffEvent> fixatedSamarog = GetFilteredList(log.CombatData, FixatedSamarog, p, true, true);
            replay.AddTether(fixatedSamarog, "rgba(255, 80, 255, 0.3)");
            //fixated Guldhem
            var fixatedGuldhem = p.GetBuffStatus(log, FixatedGuldhem, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            foreach (Segment seg in fixatedGuldhem)
            {
                long mid = (seg.Start + seg.End) / 2;
                AbstractSingleActor guldhem = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TrashID.Guldhem) && mid >= x.FirstAware && mid <= x.LastAware);
                if (guldhem != null)
                {
                    replay.Decorations.Add(new LineDecoration(seg, Colors.Orange, 0.3, new AgentConnector(p), new AgentConnector(guldhem)));
                }
            }
            //fixated Rigom
            var fixatedRigom = p.GetBuffStatus(log, FixatedRigom, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            foreach (Segment seg in fixatedGuldhem)
            {
                long mid = (seg.Start + seg.End) / 2;
                AbstractSingleActor rigom = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TrashID.Rigom) && mid >= x.FirstAware && mid <= x.LastAware);
                if (rigom != null)
                {
                    replay.Decorations.Add(new LineDecoration(seg, Colors.Red, 0.3, new AgentConnector(p), new AgentConnector(rigom)));
                }
            }
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Samarog));
            if (target == null)
            {
                throw new MissingKeyActorsException("Samarog not found");
            }
            return (target.GetHealth(combatData) > 30e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }
    }
}
