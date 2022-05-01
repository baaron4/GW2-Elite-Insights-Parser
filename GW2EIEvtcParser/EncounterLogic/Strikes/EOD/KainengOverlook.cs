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
                new HitOnPlayerMechanic(DragonSlashWave, "Dragon Slash - Wave", new MechanicPlotlySetting("triangle-left", Colors.DarkRed), "Wave.H", "Hit by Wave", "Wave Hit", 150),
                new HitOnPlayerMechanic(DragonSlashBurst, "Dragon Slash - Burst", new MechanicPlotlySetting("triangle-up", Colors.DarkRed), "Burst.H", "Hit by Burst", "Burst Hit", 150),
                new HitOnPlayerMechanic(DragonSlashRush1, "Dragon Slash - Rush", new MechanicPlotlySetting("triangle-down", Colors.DarkRed), "Rush.H", "Hit by Rush", "Rush Hit", 150),
                new HitOnPlayerMechanic(DragonSlashRush2, "Dragon Slash - Rush", new MechanicPlotlySetting("triangle-down", Colors.DarkRed), "Rush.H", "Hit by Rush", "Rush Hit", 150),
                new SkillOnPlayerMechanic(TargetedExpulsion, "Targeted Expulsion", new MechanicPlotlySetting("square", Colors.Purple), "Bomb.D", "Downed by Bomb", "Bomb Downed", 150, (evt, log) => evt.HasDowned),
                new PlayerBuffApplyMechanic(SharedDestructionLi, "Shared Destruction", new MechanicPlotlySetting("circle", Colors.Green), "Green", "Selected for Green", "Green", 150),
                new HitOnPlayerMechanic(EnforcerRushingJustice, "Rushing Justice", new MechanicPlotlySetting("square", Colors.Orange), "Flames.S", "Stood in Flames", "Stood in Flames", 150),
                new HitOnPlayerMechanic(BoomingCommandOverlap, "Booming Command", new MechanicPlotlySetting("circle", Colors.Red), "Red.O", "Red circle overlap", "Red Circle", 150),
                new HitOnPlayerMechanic(StormOfSwords1, "Storm of Swords", new MechanicPlotlySetting("circle", Colors.Pink), "Storm.H", "Hit by bladestorm", "Bladestorm Hit", 150),
                new HitOnPlayerMechanic(StormOfSwords2, "Storm of Swords", new MechanicPlotlySetting("circle", Colors.Pink), "Storm.H", "Hit by bladestorm", "Bladestorm Hit", 150),
                new HitOnPlayerMechanic(StormOfSwords3, "Storm of Swords", new MechanicPlotlySetting("circle", Colors.Pink), "Storm.H", "Hit by bladestorm", "Bladestorm Hit", 150),
                new HitOnPlayerMechanic(JadeBusterCannon, "Jade Buster Cannon", new MechanicPlotlySetting("triangle-right", Colors.Orange), "Laser.H", "Hit by Big Laser", "Laser Hit", 150),
                new EnemyBuffApplyMechanic(DestructiveAura, "Destructive Aura", new MechanicPlotlySetting("circle", Colors.Purple), "Pwrd.Up", "Powered Up", "Powered Up", 150)
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
                (int)ArcDPSEnums.TrashID.TheEnforcer,
                (int)ArcDPSEnums.TrashID.TheMindblade,
                (int)ArcDPSEnums.TrashID.TheMechRider,
                (int)ArcDPSEnums.TrashID.TheRitualist,
                (int)ArcDPSEnums.TrashID.TheSniper,
            };
        }

        protected override List<int> GetSuccessCheckIds()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.MinisterLi,
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

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor ministerLi = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.MinisterLi);
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
            AbstractSingleActor enforcer = Targets.LastOrDefault(x => x.ID == (int)ArcDPSEnums.TrashID.TheEnforcer);
            AbstractSingleActor mindblade = Targets.LastOrDefault(x => x.ID == (int)ArcDPSEnums.TrashID.TheMindblade);
            AbstractSingleActor mechRider = Targets.LastOrDefault(x => x.ID == (int)ArcDPSEnums.TrashID.TheMechRider);
            AbstractSingleActor sniper = Targets.LastOrDefault(x => x.ID == (int)ArcDPSEnums.TrashID.TheSniper);
            AbstractSingleActor ritualist = Targets.LastOrDefault(x => x.ID == (int)ArcDPSEnums.TrashID.TheRitualist);
            AddSplitPhase(phases, new List<AbstractSingleActor>() { enforcer, mindblade, ritualist }, ministerLi, log, 1);
            AddSplitPhase(phases, new List<AbstractSingleActor>() { mechRider, sniper }, ministerLi, log, 2);
            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                AbstractSingleActor ministerLi = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.MinisterLi);
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
        }
    }
}
