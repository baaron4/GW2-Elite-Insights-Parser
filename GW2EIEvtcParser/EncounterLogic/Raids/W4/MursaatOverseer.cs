using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class MursaatOverseer : RaidLogic
    {
        public MursaatOverseer(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
            new HitOnPlayerMechanic(37677, "Soldier's Aura", new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Jade","Jade Soldier's Aura hit", "Jade Aura",0),
            new HitOnPlayerMechanic(37788, "Jade Explosion", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Jade Expl","Jade Soldier's Death Explosion", "Jade Explosion",0),
            //new Mechanic(37779, "Claim", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, new MechanicPlotlySetting("square","rgb(255,200,0)"), "Claim",0), //Buff remove only
            //new Mechanic(37697, "Dispel", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, new MechanicPlotlySetting("circle","rgb(255,200,0)"), "Dispel",0), //Buff remove only
            //new Mechanic(37813, "Protect", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, new MechanicPlotlySetting("circle","rgb(0,255,255)"), "Protect",0), //Buff remove only
            new PlayerBuffApplyMechanic(757, "Invulnerability", new MechanicPlotlySetting("circle-open","rgb(0,255,255)"), "Protect","Protected by the Protect Shield","Protect Shield",0, (ba, log) => ba.AppliedDuration == 1000),
            new EnemyBuffApplyMechanic(38155, "Mursaat Overseer's Shield", new MechanicPlotlySetting("circle-open","rgb(255,200,0)"), "Shield","Jade Soldier Shield", "Soldier Shield",0),
            new EnemyBuffRemoveMechanic(38155, "Mursaat Overseer's Shield", new MechanicPlotlySetting("square-open","rgb(255,200,0)"), "Dispel","Dispelled Jade Soldier Shield", "Dispel",0),
            //new Mechanic(38184, "Enemy Tile", ParseEnum.BossIDS.MursaatOverseer, new MechanicPlotlySetting("square-open","rgb(255,200,0)"), "Floor","Enemy Tile damage", "Tile dmg",0) //Fixed damage (3500), not trackable
            });
            Extension = "mo";
            Icon = "https://wiki.guildwars2.com/images/c/c8/Mini_Mursaat_Overseer.png";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/lT1FW2r.png",
                            (889, 889),
                            (1360, 2701, 3911, 5258),
                            (-27648, -9216, 27648, 12288),
                            (11774, 4480, 14078, 5376));
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Jade
            };
        }


        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.MursaatOverseer);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Mursaat Overseer not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByHealthPercent(log, mainTarget, new List<double> { 75, 50, 25, 0 }));
            return phases;
        }

        internal override List<ErrorEvent> GetCustomWarningMessages()
        {
            return new List<ErrorEvent>()
            {
                new ErrorEvent("Missing confusion damage")
            };
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TrashID.Jade:
                    List<AbstractBuffEvent> shield = GetFilteredList(log.CombatData, 38155, target, true);
                    int shieldStart = 0;
                    int shieldRadius = 100;
                    foreach (AbstractBuffEvent c in shield)
                    {
                        if (c is BuffApplyEvent)
                        {
                            shieldStart = (int)c.Time;
                        }
                        else
                        {
                            int shieldEnd = (int)c.Time;
                            replay.Decorations.Add(new CircleDecoration(true, 0, shieldRadius, (shieldStart, shieldEnd), "rgba(255, 200, 0, 0.3)", new AgentConnector(target)));
                        }
                    }
                    var explosion = cls.Where(x => x.SkillId == 37788).ToList();
                    foreach (AbstractCastEvent c in explosion)
                    {
                        int start = (int)c.Time;
                        int precast = 1350;
                        int duration = 100;
                        int radius = 1200;
                        replay.Decorations.Add(new CircleDecoration(true, 0, radius, (start, start + precast + duration), "rgba(255, 0, 0, 0.05)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, radius, (start + precast, start + precast + duration), "rgba(255, 0, 0, 0.25)", new AgentConnector(target)));
                    }
                    break;
                default:
                    break;
            }
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            NPC target = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.MursaatOverseer);
            if (target == null)
            {
                throw new InvalidOperationException("Mursaat Overseer not found");
            }
            return (target.GetHealth(combatData) > 25e6) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }
    }
}
