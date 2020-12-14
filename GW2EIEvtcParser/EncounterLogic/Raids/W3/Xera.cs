using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Xera : RaidLogic
    {

        private long _specialSplit = 0;

        public Xera(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new HitOnPlayerMechanic(35128, "Temporal Shred", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Orb","Temporal Shred (Hit by Red Orb)", "Red Orb",0),
            new HitOnPlayerMechanic(34913, "Temporal Shred", new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Orb Aoe","Temporal Shred (Stood in Orb Aoe)", "Orb AoE",0),
            new PlayerBuffApplyMechanic(35168, "Bloodstone Protection", new MechanicPlotlySetting("hourglass-open","rgb(128,0,128)"), "In Bubble","Bloodstone Protection (Stood in Bubble)", "Inside Bubble",0),
            new EnemyCastStartMechanic(34887, "Summon Fragment Start", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "CC","Summon Fragment (Xera Breakbar)", "Breakbar",0),
            new EnemyCastEndMechanic(34887, "Summon Fragment End", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC Fail","Summon Fragment (Failed CC)", "CC Fail",0, (ce,log) => ce.ActualDuration > 11940),
            new EnemyCastEndMechanic(34887, "Summon Fragment End", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","Summon Fragment (Breakbar broken)", "CCed",0, (ce, log) => ce.ActualDuration <= 11940),
            new PlayerBuffApplyMechanic(34965, "Derangement", new MechanicPlotlySetting("square-open","rgb(200,140,255)"), "Stacks","Derangement (Stacking Debuff)", "Derangement",0),
            new PlayerBuffApplyMechanic(35084, "Bending Chaos", new MechanicPlotlySetting("triangle-down-open","rgb(255,200,0)"), "Button1","Bending Chaos (Stood on 1st Button)", "Button 1",0),
            new PlayerBuffApplyMechanic(35162, "Shifting Chaos", new MechanicPlotlySetting("triangle-ne-open","rgb(255,200,0)"), "Button2","Bending Chaos (Stood on 2nd Button)", "Button 2",0),
            new PlayerBuffApplyMechanic(35032, "Twisting Chaos", new MechanicPlotlySetting("triangle-nw-open","rgb(255,200,0)"), "Button3","Bending Chaos (Stood on 3rd Button)", "Button 3",0),
            new PlayerBuffApplyMechanic(34956, "Intervention", new MechanicPlotlySetting("square","rgb(0,0,255)"), "Shield","Intervention (got Special Action Key)", "Shield",0),
            new PlayerBuffApplyMechanic(34921, "Gravity Well", new MechanicPlotlySetting("circle-x-open","rgb(255,0,255)"), "Gravity Half","Half-platform Gravity Well", "Gravity Well",4000),
            new PlayerBuffApplyMechanic(34997, "Teleport Out", new MechanicPlotlySetting("circle","rgb(0,128,0)"), "TP Out","Teleport Out (Teleport to Platform)","TP",0),
            new PlayerBuffApplyMechanic(35076, "Hero's Return", new MechanicPlotlySetting("circle","rgb(0,200,0)"), "TP Back","Hero's Return (Teleport back)", "TP back",0),
            /*new Mechanic(35000, "Intervention", ParseEnum.BossIDS.Xera, new MechanicPlotlySetting("hourglass","rgb(128,0,128)"), "Bubble",0),*/
            //new Mechanic(35034, "Disruption", ParseEnum.BossIDS.Xera, new MechanicPlotlySetting("square","rgb(0,128,0)"), "TP",0), 
            //Not sure what this (ID 350342,"Disruption") is. Looks like it is the pulsing "orb removal" from the orange circles on the 40% platform. Would fit the name although it's weird it can hit players. 
            });
            Extension = "xera";
            GenericFallBackMethod = FallBackMethod.CombatExit;
            Icon = "https://wiki.guildwars2.com/images/4/4b/Mini_Xera.png";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/BoHwwY6.png",
                            (7112, 6377),
                            (-5992, -5992, 69, -522),
                            (-12288, -27648, 12288, 27648),
                            (1920, 12160, 2944, 14464));
        }

        internal override List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, SkillData skillData)
        {
            NPC mainTarget = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.Xera);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Xera not found");
            }
            var res = new List<AbstractBuffEvent>();
            if (_specialSplit != 0)
            {
                res.Add(new BuffRemoveAllEvent(mainTarget.AgentItem, mainTarget.AgentItem, _specialSplit, int.MaxValue, skillData.Get(762), 1, int.MaxValue));
                res.Add(new BuffRemoveManualEvent(mainTarget.AgentItem, mainTarget.AgentItem, _specialSplit, int.MaxValue, skillData.Get(762)));
            }
            return res;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            long start = 0;
            long fightDuration = log.FightData.FightEnd;
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.Xera);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Xera not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            long end = 0;
            AbstractBuffEvent invulXera = log.CombatData.GetBuffData(762).Find(x => x.To == mainTarget.AgentItem && x is BuffApplyEvent) ?? log.CombatData.GetBuffData(34113).Find(x => x.To == mainTarget.AgentItem && x is BuffApplyEvent);
            if (invulXera != null)
            {
                end = invulXera.Time;
                phases.Add(new PhaseData(start, end));
                // split happened
                if (_specialSplit > 0)
                {
                    mainTarget.SetManualHealth(24085950);
                    start = _specialSplit;
                    //mainTarget.AddCustomCastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None, log);
                    phases.Add(new PhaseData(start, fightDuration));
                }
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = "Phase " + i;
                phases[i].Targets.Add(mainTarget);

            }
            return phases;
        }

        internal override long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            AgentItem target = agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.Xera).FirstOrDefault();
            if (target == null)
            {
                throw new MissingKeyActorsException("Xera not found");
            }
            // enter combat
            CombatItem enterCombat = combatData.Find(x => x.SrcAgent == target.Agent && x.IsStateChange == ArcDPSEnums.StateChange.EnterCombat);
            if (enterCombat != null)
            {
                return enterCombat.Time;
            }
            return fightData.LogStart;
        }

        internal override void EIEvtcParse(FightData fightData, AgentData agentData, List<CombatItem> combatData, List<Player> playerList)
        {
            // find target
            AgentItem target = agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.Xera).FirstOrDefault();
            if (target == null)
            {
                throw new MissingKeyActorsException("Xera not found");
            }
            // find split
            foreach (AgentItem NPC in agentData.GetAgentByType(AgentItem.AgentType.NPC))
            {
                if (NPC.ID == 16286)
                {
                    CombatItem move = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.Position && x.SrcAgent == NPC.Agent && x.Time >= NPC.FirstAware + 500);
                    if (move != null)
                    {
                        _specialSplit = move.Time;
                    }
                    else
                    {
                        _specialSplit = NPC.FirstAware;
                    }
                    target.OverrideAwareTimes(target.FirstAware, NPC.LastAware);
                    agentData.SwapMasters(NPC, target);
                    var agents = new HashSet<ulong>() { NPC.Agent, target.Agent };
                    // update combat data
                    foreach (CombatItem c in combatData)
                    {
                        if (agents.Contains(c.SrcAgent) && c.IsStateChange.SrcIsAgent())
                        {
                            c.OverrideSrcAgent(target.Agent);
                        }
                        if (agents.Contains(c.DstAgent) && c.IsStateChange.DstIsAgent())
                        {
                            c.OverrideDstAgent(target.Agent);
                        }
                    }
                    break;
                }
            }
            ComputeFightTargets(agentData, combatData);
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.WhiteMantleSeeker1,
                ArcDPSEnums.TrashID.WhiteMantleSeeker2,
                ArcDPSEnums.TrashID.WhiteMantleKnight1,
                ArcDPSEnums.TrashID.WhiteMantleKnight2,
                ArcDPSEnums.TrashID.WhiteMantleBattleMage1,
                ArcDPSEnums.TrashID.WhiteMantleBattleMage2,
                ArcDPSEnums.TrashID.ExquisiteConjunction,
                ArcDPSEnums.TrashID.ChargedBloodstone,
                ArcDPSEnums.TrashID.BloodstoneFragment,
                ArcDPSEnums.TrashID.XerasPhantasm,
            };
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastEvents(log, 0, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Xera:
                    var summon = cls.Where(x => x.SkillId == 34887).ToList();
                    foreach (AbstractCastEvent c in summon)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 180, ((int)c.Time, (int)c.EndTime), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
