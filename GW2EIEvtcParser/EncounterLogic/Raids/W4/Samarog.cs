using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Samarog : BastionOfThePenitent
    {
        public Samarog(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new HitOnPlayerMechanic(37996, "Shockwave", new MechanicPlotlySetting(Symbols.Circle,Colors.Blue), "Schk.Wv","Shockwave from Spears", "Shockwave",0,(de, log) => !de.To.HasBuff(log, SkillIDs.Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new HitOnPlayerMechanic(38168, "Prisoner Sweep", new MechanicPlotlySetting(Symbols.Hexagon,Colors.Blue), "Swp","Prisoner Sweep (horizontal)", "Sweep",0,(de, log) => !de.To.HasBuff(log, SkillIDs.Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new HitOnPlayerMechanic(37797, "Trampling Rush", new MechanicPlotlySetting(Symbols.TriangleRight,Colors.Red), "Trpl","Trampling Rush (hit by stampede towards home)", "Trampling Rush",0),
            new HitOnPlayerMechanic(38305, "Bludgeon", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Blue), "Slam","Bludgeon (vertical Slam)", "Slam",0),
            new PlayerBuffApplyMechanic(FixatedSamarog, "Fixate: Samarog", new MechanicPlotlySetting(Symbols.Star,Colors.Magenta), "S.Fix","Fixated by Samarog", "Fixate: Samarog",0),
            new PlayerBuffApplyMechanic(FixatedGuldhem, "Fixate: Guldhem", new MechanicPlotlySetting(Symbols.StarOpen,Colors.Orange), "G.Fix","Fixated by Guldhem", "Fixate: Guldhem",0),
            new PlayerBuffApplyMechanic(FixatedRigom, "Fixate: Rigom", new MechanicPlotlySetting(Symbols.StarOpen,Colors.Red), "R.Fix","Fixated by Rigom", "Fixate: Rigom",0),
            new PlayerBuffApplyMechanic(InevitableBetrayalBig, "Big Hug", new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "B.Gr","Big Green (friends mechanic)", "Big Green",0),
            new PlayerBuffApplyMechanic(InevitableBetrayalSmall, "Small Hug", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.DarkGreen), "S.Gr","Small Green (friends mechanic)", "Small Green",0),
            new HitOnPlayerMechanic(38180, "Spear Return", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Red), "S.Rt","Hit by Spear Return", "Spear Return",0),
            new HitOnPlayerMechanic(new long[] {38260, 37851}, "Inevitable Betrayal", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Gr.Fl","Inevitable Betrayal (failed Green)", "Failed Green",0),
            new HitOnPlayerMechanic(37901, "Effigy Pulse", new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Red), "S.Pls","Effigy Pulse (Stood in Spear AoE)", "Spear Aoe",0),
            new HitOnPlayerMechanic(37816, "Spear Impact", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Red), "S.Spwn","Spear Impact (hit by spawning Spear)", "Spear Spawned",0),
            new PlayerBuffApplyMechanic(Brutalize, "Brutalize", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Magenta),"Brtlz","Brutalize (jumped upon by Samarog->Breakbar)", "Brutalize",0),
            new EnemyCastEndMechanic(38136, "Brutalize (Jump End)", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal),"CC","Brutalize (Breakbar)", "Breakbar",0),
            new SkillOnPlayerMechanic(38013, "Brutalize (Killed)", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail","Brutalize (Failed CC)", "CC Fail",0, (de, log) => de.HasKilled),
            new EnemyBuffRemoveMechanic(Brutalize, "Brutalize (End)", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CC End","Ended Brutalize", "CC Ended",0),
            //new PlayerBoonRemoveMechanic(38199, "Brutalize", ParseEnum.BossIDS.Samarog, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed","Ended Brutalize (Breakbar broken)", "CCEnded",0),//(condition => condition.getCombatItem().IsBuffRemove == ParseEnum.BuffRemove.Manual)),
            //new Mechanic(38199, "Brutalize", Mechanic.MechType.EnemyBoonStrip, ParseEnum.BossIDS.Samarog, new MechanicPlotlySetting(Symbols.DiamondTall,"rgb(110,160,0)"), "CCed1","Ended Brutalize (Breakbar broken)", "CCed1",0),//(condition => condition.getCombatItem().IsBuffRemove == ParseEnum.BuffRemove.All)),
            new PlayerBuffApplyMechanic(SoulSwarm, "Soul Swarm", new MechanicPlotlySetting(Symbols.XThinOpen,Colors.Teal),"Wall","Soul Swarm (stood in or beyond Spear Wall)", "Spear Wall",0),
            new HitOnPlayerMechanic(38231, "Impaling Stab", new MechanicPlotlySetting(Symbols.Hourglass,Colors.Blue),"Shck.Wv Ctr","Impaling Stab (hit by Spears causing Shockwave)", "Shockwave Center",0),
            new HitOnPlayerMechanic(38314, "Anguished Bolt", new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange),"Stun","Anguished Bolt (AoE Stun Circle by Guldhem)", "Guldhem's Stun",0),
            
            //  new Mechanic(37816, "Brutalize", ParseEnum.BossIDS.Samarog, new MechanicPlotlySetting(Symbols.StarSquare,Color.Red), "CC Target", casted without dmg odd
            });
            Extension = "sam";
            Icon = "https://wiki.guildwars2.com/images/f/f0/Mini_Samarog.png";
            EncounterCategoryInformation.InSubCategoryOrder = 2;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/znpsqsI.png",
                            (1000, 959),
                            (-6526, 1218, -2423, 5146)/*,
                            (-27648, -9216, 27648, 12288),
                            (11774, 4480, 14078, 5376)*/);
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(38258, 38258, InstantCastFinder.DefaultICD), // brutal aura
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Samarog);
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
            phases.AddRange(GetPhasesByInvul(log, 762, mainTarget, true, true));
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

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, extensions);
            int curGuldhem = 1;
            int curRigom = 1;
            foreach (AbstractSingleActor target in Targets)
            {
                if (target.ID == (int)ArcDPSEnums.TrashID.Guldhem)
                {
                    target.OverrideName(target.Character + " " + curGuldhem++);
                }
                if (target.ID == (int)ArcDPSEnums.TrashID.Rigom)
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


        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            // TODO: facing information (shock wave)
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Samarog:
                    List<AbstractBuffEvent> brutalize = GetFilteredList(log.CombatData, Brutalize, target, true, true);
                    int brutStart = 0;
                    foreach (AbstractBuffEvent c in brutalize)
                    {
                        if (c is BuffApplyEvent)
                        {
                            brutStart = (int)c.Time;
                        }
                        else
                        {
                            int brutEnd = (int)c.Time;
                            replay.Decorations.Add(new CircleDecoration(true, 0, 120, (brutStart, brutEnd), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.Rigom:
                case (int)ArcDPSEnums.TrashID.Guldhem:
                    break;
                default:
                    break;
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            // big bomb
            var bigbomb = log.CombatData.GetBuffData(SkillIDs.InevitableBetrayalBig).Where(x => (x.To == p.AgentItem && x is BuffApplyEvent)).ToList();
            foreach (AbstractBuffEvent c in bigbomb)
            {
                int bigStart = (int)c.Time;
                int bigEnd = bigStart + 6000;
                replay.Decorations.Add(new CircleDecoration(true, 0, 300, (bigStart, bigEnd), "rgba(150, 80, 0, 0.2)", new AgentConnector(p)));
                replay.Decorations.Add(new CircleDecoration(true, bigEnd, 300, (bigStart, bigEnd), "rgba(150, 80, 0, 0.2)", new AgentConnector(p)));
            }
            // small bomb
            var smallbomb = log.CombatData.GetBuffData(SkillIDs.InevitableBetrayalSmall).Where(x => (x.To == p.AgentItem && x is BuffApplyEvent)).ToList();
            foreach (AbstractBuffEvent c in smallbomb)
            {
                int smallStart = (int)c.Time;
                int smallEnd = smallStart + 6000;
                replay.Decorations.Add(new CircleDecoration(true, 0, 80, (smallStart, smallEnd), "rgba(80, 150, 0, 0.3)", new AgentConnector(p)));
            }
            // fixated
            List<AbstractBuffEvent> fixatedSam = GetFilteredList(log.CombatData, FixatedSamarog, p, true, true);
            int fixatedSamStart = 0;
            foreach (AbstractBuffEvent c in fixatedSam)
            {
                if (c is BuffApplyEvent)
                {
                    fixatedSamStart = Math.Max((int)c.Time, 0);
                }
                else
                {
                    int fixatedSamEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(true, 0, 80, (fixatedSamStart, fixatedSamEnd), "rgba(255, 80, 255, 0.3)", new AgentConnector(p)));
                }
            }
            //fixated Ghuldem
            List<AbstractBuffEvent> fixatedGuldhem = GetFilteredList(log.CombatData, FixatedGuldhem, p, true, true);
            int fixationGuldhemStart = 0;
            AbstractSingleActor guldhem = null;
            foreach (AbstractBuffEvent c in fixatedGuldhem)
            {
                if (c is BuffApplyEvent)
                {
                    fixationGuldhemStart = (int)c.Time;
                    long logTime = c.Time;
                    guldhem = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TrashID.Guldhem && logTime >= x.FirstAware && logTime <= x.LastAware);
                }
                else
                {
                    int fixationGuldhemEnd = (int)c.Time;
                    if (guldhem != null)
                    {
                        replay.Decorations.Add(new LineDecoration(0, (fixationGuldhemStart, fixationGuldhemEnd), "rgba(255, 100, 0, 0.3)", new AgentConnector(p), new AgentConnector(guldhem)));
                    }
                }
            }
            //fixated Rigom
            List<AbstractBuffEvent> fixatedRigom = GetFilteredList(log.CombatData, FixatedRigom, p, true, true);
            int fixationRigomStart = 0;
            AbstractSingleActor rigom = null;
            foreach (AbstractBuffEvent c in fixatedRigom)
            {
                if (c is BuffApplyEvent)
                {
                    fixationRigomStart = (int)c.Time;
                    long logTime = c.Time;
                    rigom = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TrashID.Rigom && logTime >= x.FirstAware && logTime <= x.LastAware);
                }
                else
                {
                    int fixationRigomEnd = (int)c.Time;
                    if (rigom != null)
                    {
                        replay.Decorations.Add(new LineDecoration(0, (fixationRigomStart, fixationRigomEnd), "rgba(255, 0, 0, 0.3)", new AgentConnector(p), new AgentConnector(rigom)));
                    }
                }
            }
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Samarog);
            if (target == null)
            {
                throw new MissingKeyActorsException("Samarog not found");
            }
            return (target.GetHealth(combatData) > 30e6) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }
    }
}
