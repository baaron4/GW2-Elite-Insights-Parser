using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Sabir : TheKeyOfAhdashim
    {
        public Sabir(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
                new SkillOnPlayerMechanic(56202, "Dire Drafts", new MechanicPlotlySetting("circle",Colors.Orange), "B.Tornado", "Hit by big tornado", "Big Tornado Hit", 500, (de, log) => de.HasDowned || de.HasKilled),
                new SkillOnPlayerMechanic(56643, "Unbridled Tempest", new MechanicPlotlySetting("hexagon",Colors.Pink), "Shockwave", "Hit by Shockwave", "Shockwave Hit", 0, (de, log) => de.HasDowned || de.HasKilled),
                new SkillOnPlayerMechanic(56372, "Fury of the Storm", new MechanicPlotlySetting("circle",Colors.Purple), "Arena AoE", "Hit by Arena wide AoE", "Arena AoE hit", 0, (de, log) => de.HasDowned || de.HasKilled ),
                new HitOnPlayerMechanic(56055, "Dynamic Deterrent", new MechanicPlotlySetting("y-up-open",Colors.Pink), "Pushed", "Pushed by rotating breakbar", "Pushed", 0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
                new EnemyCastStartMechanic(56349, "Regenerative Breakbar", new MechanicPlotlySetting("diamond-wide",Colors.Magenta), "Reg.Breakbar","Regenerating Breakbar", "Regenerative Breakbar", 0),
                new EnemyBuffRemoveMechanic(56100, "Regenerative Breakbar Broken", new MechanicPlotlySetting("diamond-wide",Colors.DarkTeal), "Reg.Breakbar Brkn", "Regenerative Breakbar Broken", "Regenerative Breakbar Broken", 2000),
                new EnemyBuffApplyMechanic(56172, "Rotating Breakbar", new MechanicPlotlySetting("diamond-tall",Colors.Magenta), "Rot.Breakbar","Rotating Breakbar", "Rotating Breakbar", 0),
                new EnemyBuffRemoveMechanic(56172, "Rotating Breakbar Broken", new MechanicPlotlySetting("diamond-tall",Colors.DarkTeal), "Rot.Breakbar Brkn","Rotating Breakbar Broken", "Rotating Breakbar Broken", 0),
            });
            // rotating cc 56403
            // interesting stuff 56372 (big AoE?)
            Extension = "sabir";
            Icon = "https://wiki.guildwars2.com/images/f/fc/Mini_Air_Djinn.png";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>()
            {
                ArcDPSEnums.TrashID.ParalyzingWisp,
                ArcDPSEnums.TrashID.VoltaicWisp,
                ArcDPSEnums.TrashID.SmallKillerTornado,
                ArcDPSEnums.TrashID.SmallJumpyTornado,
                ArcDPSEnums.TrashID.BigKillerTornado
            };
        }

        internal override List<AbstractHealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, SkillData skillData)
        {
            NegateDamageAgainstBarrier(combatData, Targets.Select(x => x.AgentItem).ToList());
            return new List<AbstractHealthDamageEvent>();
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Sabir);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Sabir not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            IReadOnlyList<AbstractCastEvent> cls = mainTarget.GetCastEvents(log, 0, log.FightData.FightEnd);
            var wallopingWinds = cls.Where(x => x.SkillId == 56094).ToList();
            long start = 0, end = 0;
            for (int i = 0; i < wallopingWinds.Count; i++)
            {
                AbstractCastEvent wallopinbWind = wallopingWinds[i];
                end = wallopinbWind.Time;
                var phase = new PhaseData(start, end, "Phase " + (i + 1));
                phase.AddTarget(mainTarget);
                phases.Add(phase);
                AbstractCastEvent nextAttack = cls.FirstOrDefault(x => x.Time >= wallopinbWind.EndTime && (x.SkillId == 56620 || x.SkillId == 56629 || x.SkillId == 56307));
                if (nextAttack == null)
                {
                    break;
                }
                start = nextAttack.Time;
                if (i == wallopingWinds.Count - 1)
                {
                    phase = new PhaseData(start, log.FightData.FightEnd, "Phase " + (i + 2));
                    phase.AddTarget(mainTarget);
                    phases.Add(phase);
                }
            }

            return phases;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/wesgoc6.png",
                            (1000, 910),
                            (-14122, 142, -9199, 4640)/*,
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970)*/);
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TrashID.BigKillerTornado:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 420, (start, end), "rgba(255, 150, 0, 0.4)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.SmallKillerTornado:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 120, (start, end), "rgba(255, 150, 0, 0.4)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.SmallJumpyTornado:
                case (int)ArcDPSEnums.TrashID.ParalyzingWisp:
                case (int)ArcDPSEnums.TrashID.VoltaicWisp:
                    break;
                default:
                    break;

            }
        }
        internal override long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            // Find target
            AgentItem target = agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.Sabir).FirstOrDefault();
            if (target == null)
            {
                throw new MissingKeyActorsException("Sabir not found");
            }
            CombatItem enterCombat = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.EnterCombat && x.SrcMatchesAgent(target));
            return enterCombat != null ? enterCombat.Time : fightData.LogStart;
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Sabir);
            if (target == null)
            {
                throw new MissingKeyActorsException("Sabir not found");
            }
            return (target.GetHealth(combatData) > 32e6) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }
    }
}
