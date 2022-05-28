using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class KainengOverlook : CanthaStrike
    {
        public KainengOverlook(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new HitOnPlayerMechanic(new long[] { DragonSlashWave, DragonSlashWaveCM }, "Dragon Slash - Wave", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.DarkRed), "Wave.H", "Hit by Wave", "Wave Hit", 150),
                new HitOnPlayerMechanic(new long []{DragonSlashBurst, DragonSlashBurstCM }, "Dragon Slash - Burst", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.DarkRed), "Burst.H", "Hit by Burst", "Burst Hit", 150),
                new HitOnPlayerMechanic(new long[] { DragonSlashRush1, DragonSlashRush2, DragonSlashRush1CM, DragonSlashRush2CM }, "Dragon Slash - Rush", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkRed), "Rush.H", "Hit by Rush", "Rush Hit", 150),
                new SkillOnPlayerMechanic(new long[] {TargetedExpulsion, TargetedExpulsionCM }, "Targeted Expulsion", new MechanicPlotlySetting(Symbols.Square, Colors.Purple), "Bomb.D", "Downed by Bomb", "Bomb Downed", 150, (evt, log) => evt.HasDowned),
                new PlayerBuffApplyMechanic(new long[] {SharedDestructionLi, SharedDestructionLiCM }, "Shared Destruction", new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "Green", "Selected for Green", "Green", 150),
                new HitOnPlayerMechanic(new long[] { EnforcerRushingJustice, EnforcerRushingJusticeCM }, "Rushing Justice", new MechanicPlotlySetting(Symbols.Square, Colors.Orange), "Flames.S", "Stood in Flames", "Stood in Flames", 150),
                new HitOnPlayerMechanic(BoomingCommandOverlap, "Booming Command", new MechanicPlotlySetting(Symbols.Circle, Colors.Red), "Red.O", "Red circle overlap", "Red Circle", 150),
                new PlayerBuffApplyMechanic(FixatedKainengOverlook, "Fixated (Mindblade)", new MechanicPlotlySetting(Symbols.Circle, Colors.Purple), "Fixated.M", "Fixated by The Mindblade", "Fixated Mindblade", 150, (evt, log) => evt.CreditedBy.ID == (int)ArcDPSEnums.TrashID.TheMindblade || evt.CreditedBy.ID == (int)ArcDPSEnums.TrashID.TheMindbladeCM),
                new PlayerBuffApplyMechanic(FixatedKainengOverlook, "Fixated (Enforcer)", new MechanicPlotlySetting(Symbols.Circle, Colors.Purple), "Fixated.E", "Fixated by The Enforcer", "Fixated Enforcer", 150, (evt, log) => evt.CreditedBy.ID == (int)ArcDPSEnums.TrashID.TheEnforcer || evt.CreditedBy.ID == (int)ArcDPSEnums.TrashID.TheEnforcerCM),
                new EnemyBuffApplyMechanic(LethalInspiration, "Lethal Inspiration", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.DarkGreen), "Pwrd.Up1", "Powered Up (Split 1)", "Powered Up 1", 150),
                new HitOnPlayerMechanic(new long[]{StormOfSwords1, StormOfSwords2, StormOfSwords3 }, "Storm of Swords", new MechanicPlotlySetting(Symbols.Circle, Colors.Pink), "Storm.H", "Hit by bladestorm", "Bladestorm Hit", 150),
                new HitOnPlayerMechanic(JadeBusterCannon, "Jade Buster Cannon", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Orange), "Laser.H", "Hit by Big Laser", "Laser Hit", 150),
                new EnemyBuffApplyMechanic(DestructiveAura, "Destructive Aura", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Purple), "Pwrd.Up2", "Powered Up (Split 2)", "Powered Up 2", 150),
                new PlayerBuffApplyMechanic(Debilitated, "Debilitated", new MechanicPlotlySetting(Symbols.Diamond, Colors.DarkRed), "Debilitated", "Debilitated (Reduced outgoing damage)", "Debilitated", 0),
                new PlayerBuffApplyMechanic(Infirmity, "Infirmity", new MechanicPlotlySetting(Symbols.Diamond, Colors.DarkPurple), "Infirmity", "Infirmity (Reduced incoming healing)", "Infirmity", 0),
                new PlayerBuffApplyMechanic(ExposedEODStrike, "Exposed", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Red), "Exposed", "Received Exposed stack", "Exposed", 150),
            }
            );
            Icon = "https://i.imgur.com/7OutZup.png";
            Extension = "kaiover";
            EncounterCategoryInformation.InSubCategoryOrder = 2;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/4QeDiit.png",
                            (942, 835),
                            (-23828, -16565, -20362,-13482)/*,
                            (-15360, -36864, 15360, 39936),
                            (3456, 11012, 4736, 14212)*/);
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.MinisterLi,
                (int)ArcDPSEnums.TargetID.MinisterLiCM,
                (int)ArcDPSEnums.TrashID.TheEnforcer,
                (int)ArcDPSEnums.TrashID.TheMindblade,
                (int)ArcDPSEnums.TrashID.TheMechRider,
                (int)ArcDPSEnums.TrashID.TheRitualist,
                (int)ArcDPSEnums.TrashID.TheSniper,
                (int)ArcDPSEnums.TrashID.TheEnforcerCM,
                (int)ArcDPSEnums.TrashID.TheMindbladeCM,
                (int)ArcDPSEnums.TrashID.TheMechRiderCM,
                (int)ArcDPSEnums.TrashID.TheRitualistCM,
                (int)ArcDPSEnums.TrashID.TheSniperCM,
            };
        }

        protected override List<int> GetSuccessCheckIds()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.MinisterLi,
                (int)ArcDPSEnums.TargetID.MinisterLiCM,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.SpiritOfDestruction,
                ArcDPSEnums.TrashID.SpiritOfPain,
            };
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.MinisterLi,
                (int)ArcDPSEnums.TargetID.MinisterLiCM,
            };
        }

        private static void AddSplitPhase(List<PhaseData> phases, IReadOnlyList<AbstractSingleActor> targets, AbstractSingleActor ministerLi, ParsedEvtcLog log, int phaseID)
        {
            if (targets.All(x => x != null))
            {
                EnterCombatEvent cbtEnter = null;
                foreach (NPC target in targets)
                {
                    cbtEnter = log.CombatData.GetEnterCombatEvents(target.AgentItem).LastOrDefault();
                    if (cbtEnter != null)
                    {
                        break;
                    }
                }
                if (cbtEnter != null)
                {
                    AbstractBuffEvent nextPhaseStartEvt = log.CombatData.GetBuffData(ministerLi.AgentItem).FirstOrDefault(x => x is BuffRemoveAllEvent && x.BuffID == SkillIDs.Determined762 && x.Time > cbtEnter.Time);
                    long phaseEnd = nextPhaseStartEvt != null ? nextPhaseStartEvt.Time : log.FightData.FightEnd;
                    var addPhase = new PhaseData(cbtEnter.Time, phaseEnd, "Split Phase " + phaseID);
                    addPhase.AddTargets(targets);
                    phases.Add(addPhase);
                }
            }
        }

        private AbstractSingleActor GetMinisterLi(FightData fightData)
        {
            return Targets.FirstOrDefault(x => x.ID == (fightData.IsCM ? (int)ArcDPSEnums.TargetID.MinisterLiCM : (int)ArcDPSEnums.TargetID.MinisterLi));
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor ministerLi = GetMinisterLi(log.FightData);
            if (ministerLi == null)
            {
                throw new MissingKeyActorsException("Minister Li not found");
            }
            phases[0].AddTarget(ministerLi);
            if (!requirePhases)
            {
                return phases;
            }
            List<PhaseData> subPhases = GetPhasesByInvul(log, 762, ministerLi, false, true);
            for (int i = 0; i < subPhases.Count; i++)
            {
                subPhases[i].Name = "Phase " + (i + 1);
                subPhases[i].AddTarget(ministerLi);
            }
            phases.AddRange(subPhases);
            //
            AbstractSingleActor enforcer = Targets.LastOrDefault(x => x.ID == (log.FightData.IsCM ? (int)ArcDPSEnums.TrashID.TheEnforcerCM : (int)ArcDPSEnums.TrashID.TheEnforcer));
            AbstractSingleActor mindblade = Targets.LastOrDefault(x => x.ID == (log.FightData.IsCM ? (int)ArcDPSEnums.TrashID.TheMindbladeCM : (int)ArcDPSEnums.TrashID.TheMindblade));
            AbstractSingleActor mechRider = Targets.LastOrDefault(x => x.ID == (log.FightData.IsCM ? (int)ArcDPSEnums.TrashID.TheMechRiderCM : (int)ArcDPSEnums.TrashID.TheMechRider));
            AbstractSingleActor sniper = Targets.LastOrDefault(x => x.ID == (log.FightData.IsCM ? (int)ArcDPSEnums.TrashID.TheSniperCM : (int)ArcDPSEnums.TrashID.TheSniper));
            AbstractSingleActor ritualist = Targets.LastOrDefault(x => x.ID == (log.FightData.IsCM ? (int)ArcDPSEnums.TrashID.TheRitualistCM : (int)ArcDPSEnums.TrashID.TheRitualist));
            AddSplitPhase(phases, new List<AbstractSingleActor>() { enforcer, mindblade, ritualist }, ministerLi, log, 1);
            AddSplitPhase(phases, new List<AbstractSingleActor>() { mechRider, sniper }, ministerLi, log, 2);
            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            AbstractSingleActor ministerLi = GetMinisterLi(fightData);
            if (ministerLi == null)
            {
                throw new MissingKeyActorsException("Minister Li not found");
            }
            var buffApplies = combatData.GetBuffData(SkillIDs.Determined762).OfType<BuffApplyEvent>().Where(x => x.To == ministerLi.AgentItem).ToList();
            if (buffApplies.Count >= 3)
            {
                fightData.SetSuccess(true, buffApplies[2].Time);
            }
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor ministerLiCM = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.MinisterLiCM);
            return ministerLiCM != null ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }
    }
}
