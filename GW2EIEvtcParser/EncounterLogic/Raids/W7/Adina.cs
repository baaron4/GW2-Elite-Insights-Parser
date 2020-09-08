using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Adina : RaidLogic
    {
        public Adina(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
                new PlayerBuffApplyMechanic(56593, "Radiant Blindness", new MechanicPlotlySetting("circle","rgb(255,0,255)"), "R.Blind", "Unremovable blindness", "Radiant Blindness", 0),
                new PlayerBuffApplyMechanic(56440, "Eroding Curse", new MechanicPlotlySetting("square","rgb(200,140,255)"), "Curse", "Stacking damage debuff from Hand of Erosion", "Eroding Curse", 0),
                new HitOnPlayerMechanic(56648, "Boulder Barrage", new MechanicPlotlySetting("hexagon","rgb(255,0,0)"), "Boulder", "Hit by boulder thrown during pillars", "Boulder Barrage", 0),
                new HitOnPlayerMechanic(56390, "Perilous Pulse", new MechanicPlotlySetting("triangle-right","rgb(255,150,0)"), "Perilous Pulse", "Perilous Pulse", "Perilous Pulse", 0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
                new HitOnPlayerMechanic(56141, "Stalagmites", new MechanicPlotlySetting("pentagon","rgb(255,0,0)"), "Mines", "Hit by mines", "Mines", 0),
                new HitOnPlayerMechanic(56114, "Diamond Palisade", new MechanicPlotlySetting("star-diamond","rgb(255,150,0)"), "Eye", "Looked at Eye", "Looked at Eye", 0),
                new HitOnPlayerMechanic(56035, "Quantum Quake", new MechanicPlotlySetting("hourglass","rgb(120,100,0)"), "S.Thrower", "Hit by rotating SandThrower", "SandThrower", 0),
                new HitOnPlayerMechanic(56381, "Quantum Quake", new MechanicPlotlySetting("hourglass","rgb(120,100,0)"), "S.Thrower", "Hit by rotating SandThrower", "SandThrower", 0),
            });
            Extension = "adina";
            Icon = "https://wiki.guildwars2.com/images/a/a0/Mini_Earth_Djinn.png";
        }

        internal override void EIEvtcParse(FightData fightData, AgentData agentData, List<CombatItem> combatData, List<Player> playerList)
        {
            var attackTargets = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.AttackTarget).ToList();
            long first = fightData.FightStart;
            long final = fightData.FightEnd;
            foreach (CombatItem at in attackTargets)
            {
                AgentItem hand = agentData.GetAgent(at.DstAgent);
                AgentItem atAgent = agentData.GetAgent(at.SrcAgent);
                var attackables = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.Targetable && x.SrcAgent == atAgent.Agent && x.Time <= atAgent.LastAware && x.Time >= atAgent.FirstAware).ToList();
                var attackOn = attackables.Where(x => x.DstAgent == 1 && x.Time >= first + 2000).Select(x => x.Time).ToList();
                var attackOff = attackables.Where(x => x.DstAgent == 0 && x.Time >= first + 2000).Select(x => x.Time).ToList();
                var posFacingHP = combatData.Where(x => x.SrcAgent == hand.Agent && x.Time >= hand.FirstAware && hand.LastAware >= x.Time && (x.IsStateChange == ArcDPSEnums.StateChange.Position || x.IsStateChange == ArcDPSEnums.StateChange.Rotation || x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate)).ToList();
                CombatItem pos = posFacingHP.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.Position);
                int id = (int)ArcDPSEnums.TrashID.HandOfErosion;
                if (pos != null)
                {
                    (float x, float y, _) = AbstractMovementEvent.UnpackMovementData(pos.DstAgent, 0);
                    if ((Math.Abs(x - 15570.5) < 10 && Math.Abs(y + 693.117) < 10) ||
                            (Math.Abs(x - 14277.2) < 10 && Math.Abs(y + 2202.52) < 10))
                    {
                        id = (int)ArcDPSEnums.TrashID.HandOfEruption;
                    }
                }
                for (int i = 0; i < attackOn.Count; i++)
                {
                    long start = attackOn[i];
                    long end = final;
                    if (i <= attackOff.Count - 1)
                    {
                        end = attackOff[i];
                    }
                    AgentItem extra = agentData.AddCustomAgent(start, end, AgentItem.AgentType.NPC, hand.Name, hand.Prof, id, hand.Toughness, hand.Healing, hand.Condition, hand.Concentration, hand.HitboxWidth, hand.HitboxHeight);
                    foreach (CombatItem c in combatData)
                    {
                        if (c.Time >= extra.FirstAware && c.Time <= extra.LastAware)
                        {
                            if (c.IsStateChange.SrcIsAgent() && c.SrcAgent == hand.Agent)
                            {
                                c.OverrideSrcAgent(extra.Agent);
                            }
                            if (c.IsStateChange.DstIsAgent() && c.DstAgent == hand.Agent)
                            {
                                c.OverrideDstAgent(extra.Agent);
                            }
                        }
                    }
                    foreach (CombatItem c in posFacingHP)
                    {
                        var cExtra = new CombatItem(c);
                        cExtra.OverrideTime(extra.FirstAware);
                        cExtra.OverrideSrcAgent(extra.Agent);
                        combatData.Add(cExtra);
                    }
                }
            }
            combatData.Sort((x, y) => x.Time.CompareTo(y.Time));
            ComputeFightTargets(agentData, combatData);
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>()
            {
                (int)ArcDPSEnums.TargetID.Adina,
                (int)ArcDPSEnums.TrashID.HandOfErosion,
                (int)ArcDPSEnums.TrashID.HandOfEruption
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.Adina);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Adina not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            var quantumQuakes = mainTarget.GetCastLogs(log, 0, log.FightData.FightEnd).Where(x => x.SkillId == 56035 || x.SkillId == 56381).ToList();
            List<AbstractBuffEvent> invuls = GetFilteredList(log.CombatData, 762, mainTarget, true);
            long start = 0, end = 0;
            for (int i = 0; i < invuls.Count; i++)
            {
                AbstractBuffEvent be = invuls[i];
                if (be is BuffApplyEvent)
                {
                    start = be.Time;
                    if (i == invuls.Count - 1)
                    {
                        phases.Add(new PhaseData(start, log.FightData.FightEnd, "Split " + (i / 2 + 1)));
                    }
                }
                else
                {
                    end = be.Time;
                    phases.Add(new PhaseData(start, end, "Split " + (i / 2 + 1)));
                }
            }
            var mainPhases = new List<PhaseData>();
            start = 0;
            end = 0;
            for (int i = 1; i < phases.Count; i++)
            {
                AbstractCastEvent qQ = quantumQuakes[i - 1];
                end = qQ.Time;
                mainPhases.Add(new PhaseData(start, end, "Phase " + i ));
                PhaseData split = phases[i];
                AddTargetsToPhase(split, new List<int> { (int)ArcDPSEnums.TrashID.HandOfErosion, (int)ArcDPSEnums.TrashID.HandOfEruption }, log);
                start = split.End;
                if (i == phases.Count - 1 && start != log.FightData.FightEnd)
                {
                    mainPhases.Add(new PhaseData(start, log.FightData.FightEnd, "Phase " + (i + 1)));
                }
            }
            foreach (PhaseData phase in mainPhases)
            {
                phase.Targets.Add(mainTarget);
            }
            phases.AddRange(mainPhases);
            phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            GetCombatMap(log).MatchMapsToPhases(new List<string> {
                "https://i.imgur.com/3IBkNM6.png",
                "https://i.imgur.com/iMrhTt6.png",
                "https://i.imgur.com/zaZftSk.png",
                "https://i.imgur.com/KkYdspd.png",
                "https://i.imgur.com/wqgFO7Z.png",
                "https://i.imgur.com/DroFhFc.png",
                "https://i.imgur.com/QsEFkNO.png"
            }, phases, log.FightData.FightEnd);
            return phases;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/3IBkNM6.png",
                            (1436, 1659),
                            (13840, -2698, 15971, -248),
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970));
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            NPC target = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.Adina);
            if (target == null)
            {
                throw new InvalidOperationException("Adina not found");
            }
            return (target.GetHealth(combatData) > 23e6) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }
    }
}
