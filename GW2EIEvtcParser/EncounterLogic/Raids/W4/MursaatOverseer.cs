using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class MursaatOverseer : BastionOfThePenitent
    {
        public MursaatOverseer(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
            new HitOnPlayerMechanic(37677, "Soldier's Aura", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Jade","Jade Soldier's Aura hit", "Jade Aura",0),
            new HitOnPlayerMechanic(37788, "Jade Explosion", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Jade Expl","Jade Soldier's Death Explosion", "Jade Explosion",0),
            //new Mechanic(37779, "Claim", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, new MechanicPlotlySetting(Symbols.Square,Colors.Yellow), "Claim",0), //Buff remove only
            //new Mechanic(37697, "Dispel", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Dispel",0), //Buff remove only
            //new Mechanic(37813, "Protect", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, new MechanicPlotlySetting(Symbols.Circle,Colors.Teal), "Protect",0), //Buff remove only
            new PlayerBuffApplyMechanic(Invulnerability757, "Invulnerability", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Teal), "Protect","Protected by the Protect Shield","Protect Shield",0, (ba, log) => ba.AppliedDuration == 1000),
            new PlayerBuffApplyMechanic(ProtectSAK, "Protect (SAK)", new MechanicPlotlySetting(Symbols.Circle,Colors.Blue), "Protect (SAK)","Took protect","Protect (SAK)",0),
            new PlayerBuffApplyMechanic(DispelSAK, "Dispel (SAK)", new MechanicPlotlySetting(Symbols.Circle,Colors.Purple), "Dispel (SAK)","Took dispel","Dispel (SAK)",0),
            new PlayerBuffApplyMechanic(ClaimSAK, "Claim (SAK)", new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Claim (SAK)","Took claim","Claim (SAK)",0),
            new EnemyBuffApplyMechanic(MursaatOverseersShield, "Mursaat Overseer's Shield", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Shield","Jade Soldier Shield", "Soldier Shield",0),
            new EnemyBuffRemoveMechanic(MursaatOverseersShield, "Mursaat Overseer's Shield", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Yellow), "Dispel","Dispelled Jade Soldier Shield", "Dispel",0),
            //new Mechanic(38184, "Enemy Tile", ParseEnum.BossIDS.MursaatOverseer, new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Yellow), "Floor","Enemy Tile damage", "Tile dmg",0) //Fixed damage (3500), not trackable
            });
            Extension = "mo";
            Icon = "https://wiki.guildwars2.com/images/c/c8/Mini_Mursaat_Overseer.png";
            EncounterCategoryInformation.InSubCategoryOrder = 1;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/lT1FW2r.png",
                            (889, 889),
                            (1360, 2701, 3911, 5258)/*,
                            (-27648, -9216, 27648, 12288),
                            (11774, 4480, 14078, 5376)*/);
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Jade
            };
        }
        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(37952, 37952, InstantCastFinder.DefaultICD), // Punishement Aura
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.MursaatOverseer);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Mursaat Overseer not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByHealthPercent(log, mainTarget, new List<double> { 75, 50, 25, 0 }));
            return phases;
        }

        internal override List<ErrorEvent> GetCustomWarningMessages(FightData fightData, int arcdpsVersion)
        {
            List<ErrorEvent> res = base.GetCustomWarningMessages(fightData, arcdpsVersion);
            res.AddRange(GetConfusionDamageMissingMessage(arcdpsVersion));
            return res;
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, 0, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TrashID.Jade:
                    List<AbstractBuffEvent> shield = GetFilteredList(log.CombatData, MursaatOverseersShield, target, true, true);
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
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.MursaatOverseer);
            if (target == null)
            {
                throw new MissingKeyActorsException("Mursaat Overseer not found");
            }
            return (target.GetHealth(combatData) > 25e6) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }
    }
}
