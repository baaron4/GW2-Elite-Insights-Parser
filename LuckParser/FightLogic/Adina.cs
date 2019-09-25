using System;
using System.Collections.Generic;
using System.Linq;
using LuckParser.EIData;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData;
using LuckParser.Parser.ParsedData.CombatEvents;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Logic
{
    public class Adina : RaidLogic
    {
        public Adina(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
                new PlayerBoonApplyMechanic(56593, "Radiant Blindness", new MechanicPlotlySetting("circle","rgb(255,0,255)"), "R.Blind", "Unremovable blindness", "Radiant Blindness", 0),
                new PlayerBoonApplyMechanic(56440, "Eroding Curse", new MechanicPlotlySetting("square","rgb(200,140,255)"), "Curse", "Stacking damage debuff from Hand of Erosion", "Eroding Curse", 0),
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

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            var attackTargets = combatData.Where(x => x.IsStateChange == ParseEnum.StateChange.AttackTarget).ToList();
            long first = combatData.Count > 0 ? combatData.First().LogTime : 0;
            long final = combatData.Count > 0 ? combatData.Last().LogTime : 0;
            foreach (CombatItem at in attackTargets)
            {
                AgentItem hand = agentData.GetAgent(at.DstAgent);
                AgentItem atAgent = agentData.GetAgent(at.SrcAgent);
                var attackables = combatData.Where(x => x.IsStateChange == ParseEnum.StateChange.Targetable && x.SrcAgent == atAgent.Agent && x.LogTime <= atAgent.LastAwareLogTime && x.LogTime >= atAgent.FirstAwareLogTime).ToList();
                var attackOn = attackables.Where(x => x.DstAgent == 1 && x.LogTime >= first + 2000).Select(x => x.LogTime).ToList();
                var attackOff = attackables.Where(x => x.DstAgent == 0 && x.LogTime >= first + 2000).Select(x => x.LogTime).ToList();
                var posFacingHP = combatData.Where(x => x.SrcAgent == hand.Agent && x.LogTime >= hand.FirstAwareLogTime && hand.LastAwareLogTime >= x.LogTime && (x.IsStateChange == ParseEnum.StateChange.Position || x.IsStateChange == ParseEnum.StateChange.Rotation || x.IsStateChange == ParseEnum.StateChange.MaxHealthUpdate)).ToList();
                CombatItem pos = posFacingHP.FirstOrDefault(x => x.IsStateChange == ParseEnum.StateChange.Position);
                ushort id = (ushort)HandOfErosion;
                if (pos != null)
                {
                    byte[] xy = BitConverter.GetBytes(pos.DstAgent);
                    float x = BitConverter.ToSingle(xy, 0);
                    float y = BitConverter.ToSingle(xy, 4);
                    if ((Math.Abs(x - 15570.5) < 10 && Math.Abs(y + 693.117) < 10) ||
                            (Math.Abs(x - 14277.2) < 10 && Math.Abs(y + 2202.52) < 10))
                    {
                        id = (ushort)HandOfEruption;
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
                    AgentItem extra = agentData.AddCustomAgent(start, end, AgentItem.AgentType.Gadget, hand.Name, hand.Prof, id, hand.Toughness, hand.Healing, hand.Condition, hand.Concentration, hand.HitboxWidth, hand.HitboxHeight);
                    foreach (CombatItem c in combatData.Where(x => x.SrcAgent == hand.Agent && x.LogTime >= extra.FirstAwareLogTime && x.LogTime <= extra.LastAwareLogTime))
                    {
                        c.OverrideSrcValues(extra.Agent, extra.InstID);
                    }
                    foreach (CombatItem c in combatData.Where(x => x.DstAgent == hand.Agent && x.LogTime >= extra.FirstAwareLogTime && x.LogTime <= extra.LastAwareLogTime))
                    {
                        c.OverrideDstValues(extra.Agent, extra.InstID);
                    }
                    foreach (CombatItem c in posFacingHP)
                    {
                        var cExtra = new CombatItem(c);
                        cExtra.OverrideTime(extra.FirstAwareLogTime);
                        cExtra.OverrideSrcValues(extra.Agent, extra.InstID);
                        combatData.Add(cExtra);
                    }
                }
            }
            combatData.Sort((x, y) => x.LogTime.CompareTo(y.LogTime));
            ComputeFightTargets(agentData, combatData);
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>()
            {
                (ushort)ParseEnum.TargetIDS.Adina,
                (ushort)HandOfErosion,
                (ushort)HandOfEruption
            };
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Adina && x.AgentItem.Type == AgentItem.AgentType.NPC);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            var quantumQuakes = mainTarget.GetCastLogs(log, 0, log.FightData.FightDuration).Where(x => x.SkillId == 56035 || x.SkillId == 56381).ToList();
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
                        phases.Add(new PhaseData(start, log.FightData.FightDuration)
                        {
                            Name = "Split " + (i / 2 + 1)
                        });
                    }
                }
                else
                {
                    end = be.Time;
                    phases.Add(new PhaseData(start, end)
                    {
                        Name = "Split " + (i / 2 + 1)
                    });
                }
            }
            var mainPhases = new List<PhaseData>();
            start = 0;
            end = 0;
            for (int i = 1; i < phases.Count; i++)
            {
                AbstractCastEvent qQ = quantumQuakes[i - 1];
                end = qQ.Time;
                mainPhases.Add(new PhaseData(start, end)
                {
                    Name = "Phase " + i
                });
                PhaseData split = phases[i];
                AddTargetsToPhase(split, new List<ushort> { (ushort)HandOfErosion, (ushort)HandOfEruption }, log);
                start = split.End;
                if (i == phases.Count - 1 && start != log.FightData.FightDuration)
                {
                    mainPhases.Add(new PhaseData(start, log.FightData.FightDuration)
                    {
                        Name = "Phase " + (i + 1)
                    });
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
            }, phases, log.FightData.FightDuration);
            return phases;
        }

        public override string GetFightName()
        {
            Target target = Targets.Find(x => x.ID == TriggerID && x.AgentItem.Type == AgentItem.AgentType.NPC);
            if (target == null)
            {
                return "UNKNOWN";
            }
            return target.Character;
        }


        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/3IBkNM6.png",
                            (1436, 1659),
                            (13840, -2698, 15971, -248),
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970));
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Adina && x.AgentItem.Type == AgentItem.AgentType.NPC);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return (target.GetHealth(combatData) > 23e6) ? 1 : 0;
        }
    }
}
