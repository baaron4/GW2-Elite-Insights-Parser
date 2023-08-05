using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class MursaatOverseer : BastionOfThePenitent
    {
        public MursaatOverseer(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
            new PlayerDstHitMechanic(JadeSoldierAura, "Soldier's Aura", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Jade","Jade Soldier's Aura hit", "Jade Aura",0),
            new PlayerDstHitMechanic(JadeSoldierExplosion, "Jade Explosion", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Jade Expl","Jade Soldier's Death Explosion", "Jade Explosion",0),
            //new Mechanic(ClaimSAK, "Claim", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, new MechanicPlotlySetting(Symbols.Square,Colors.Yellow), "Claim",0), //Buff remove only
            //new Mechanic(DispelSAK, "Dispel", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Dispel",0), //Buff remove only
            //new Mechanic(ProtectSAK, "Protect", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, new MechanicPlotlySetting(Symbols.Circle,Colors.Teal), "Protect",0), //Buff remove only
            new PlayerDstBuffApplyMechanic(Invulnerability757, "Invulnerability", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Teal), "Protect","Protected by the Protect Shield","Protect Shield",0).UsingChecker((ba, log) => ba.AppliedDuration == 1000),
            new PlayerDstBuffApplyMechanic(ProtectBuff, "Protect (SAK)", new MechanicPlotlySetting(Symbols.Circle,Colors.Blue), "Protect (SAK)","Took protect","Protect (SAK)",0),
            new PlayerDstBuffApplyMechanic(DispelBuff, "Dispel (SAK)", new MechanicPlotlySetting(Symbols.Circle,Colors.Purple), "Dispel (SAK)","Took dispel","Dispel (SAK)",0),
            new PlayerDstBuffApplyMechanic(ClaimBuff, "Claim (SAK)", new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Claim (SAK)","Took claim","Claim (SAK)",0),
            new EnemyDstBuffApplyMechanic(MursaatOverseersShield, "Mursaat Overseer's Shield", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Shield","Jade Soldier Shield", "Soldier Shield",0),
            new EnemyDstBuffRemoveMechanic(MursaatOverseersShield, "Mursaat Overseer's Shield", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Yellow), "Dispel","Dispelled Jade Soldier Shield", "Dispel",0),
            //new Mechanic(EnemyTile, "Enemy Tile", ParseEnum.BossIDS.MursaatOverseer, new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Yellow), "Floor","Enemy Tile damage", "Tile dmg",0) //Fixed damage (3500), not trackable
            });
            Extension = "mo";
            Icon = EncounterIconMursaatOverseer;
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayMursaatOverseer,
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
                new DamageCastFinder(PunishementAura, PunishementAura),
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.MursaatOverseer));
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
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TrashID.Jade:
                    var shields = target.GetBuffStatus(log, MursaatOverseersShield, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
                    int shieldRadius = 100;
                    foreach (Segment seg in shields)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, shieldRadius, seg, "rgba(255, 200, 0, 0.3)", new AgentConnector(target)));
                    }
                    var explosion = cls.Where(x => x.SkillId == JadeSoldierExplosion).ToList();
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

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            IEnumerable<Segment> claims = player.GetBuffStatus(log, ClaimBuff, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
            replay.AddOverheadIcons(claims, player, ParserIcons.FixationPurpleOverhead);
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.MursaatOverseer));
            if (target == null)
            {
                throw new MissingKeyActorsException("Mursaat Overseer not found");
            }
            return (target.GetHealth(combatData) > 25e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }
    }
}
