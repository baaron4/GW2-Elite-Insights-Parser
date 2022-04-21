using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Arkk : ShatteredObservatory
    {
        public Arkk(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(HorizonStrike1, "Horizon Strike", new MechanicPlotlySetting("circle", "rgb(255,140,0)"), "Horizon Strike","Horizon Strike (turning pizza slices)", "Horizon Strike",0),
            new HitOnPlayerMechanic(HorizonStrike2, "Horizon Strike", new MechanicPlotlySetting("circle", "rgb(255,140,0)"), "Horizon Strike","Horizon Strike (turning pizza slices)", "Horizon Strike",0),
            new HitOnPlayerMechanic(DiffractiveEdge1, "Diffractive Edge", new MechanicPlotlySetting("star","rgb(255,200,0)"), "5 Cone","Diffractive Edge (5 Cone Knockback)", "Five Cones",0),
            new HitOnPlayerMechanic(DiffractiveEdge2, "Diffractive Edge", new MechanicPlotlySetting("star","rgb(255,200,0)"), "5 Cone","Diffractive Edge (5 Cone Knockback)", "Five Cones",0),
            new HitOnPlayerMechanic(SolarFury, "Solar Fury", new MechanicPlotlySetting("circle","rgb(128,0,0)"), "Ball","Stood in Red Overhead Ball Field", "Red Ball Aoe",0),
            new HitOnPlayerMechanic(FocusedRage, "Focused Rage", new MechanicPlotlySetting("triangle-down","rgb(255,100,0)"), "Cone KB","Knockback in Cone with overhead crosshair", "Knockback Cone",0),
            new HitOnPlayerMechanic(SolarDischarge, "Solar Discharge", new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Shockwave","Knockback shockwave after Overhead Balls", "Shockwave",0),
            new HitOnPlayerMechanic(StarbustCascade1, "Starburst Cascade", new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Float Ring","Starburst Cascade (Expanding/Retracting Lifting Ring)", "Float Ring",500),
            new HitOnPlayerMechanic(StarbustCascade2, "Starburst Cascade", new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Float Ring","Starburst Cascade (Expanding/Retracting Lifting Ring)", "Float Ring",500),
            new HitOnPlayerMechanic(HorizonStrikeNormal, "Horizon Strike Normal", new MechanicPlotlySetting("circle","rgb(80,0,0)"), "Horizon Strike norm","Horizon Strike (normal)", "Horizon Strike (normal)",0),
            new HitOnPlayerMechanic(OverheadSmash, "Overhead Smash", new MechanicPlotlySetting("triangle-left","rgb(200,0,0)"), "Smash","Overhead Smash","Overhead Smash",0),
            new PlayerBuffApplyMechanic(CorporealReassignment, "Corporeal Reassignment", new MechanicPlotlySetting("diamond","rgb(255,0,0)"), "Skull","Exploding Skull mechanic application", "Corporeal Reassignment",0),
            new HitOnPlayerMechanic(ExplodeArkk, "Explode", new MechanicPlotlySetting("circle","rgb(255,200,0)"), "Bloom Explode","Hit by Solar Bloom explosion", "Bloom Explosion",0),
            new PlayerBuffApplyMechanic(FixatedBloom1, "Fixate", new MechanicPlotlySetting("star-open","rgb(255,0,255)"), "Bloom Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new PlayerBuffApplyMechanic(FixatedBloom2, "Fixate", new MechanicPlotlySetting("star-open","rgb(255,0,255)"), "Bloom Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new PlayerBuffApplyMechanic(FixatedBloom3, "Fixate", new MechanicPlotlySetting("star-open","rgb(255,0,255)"), "Bloom Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new PlayerBuffApplyMechanic(FixatedBloom4, "Fixate", new MechanicPlotlySetting("star-open","rgb(255,0,255)"), "Bloom Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new PlayerBuffApplyMechanic(CosmicMeteor, "Cosmic Meteor", new MechanicPlotlySetting("circle-open","rgb(0,255,0)"), "Green","Temporal Realignment (Green) application", "Green",0),
            new PlayerBuffApplyMechanic(Fear, "Fear", new MechanicPlotlySetting("square-open","rgb(255,0,0)"), "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)",0, (ba, log) => ba.AppliedDuration == 3000), // //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new EnemyCastStartMechanic(ArkkBreakbarCast, "Breakbar Start", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "Breakbar","Start Breakbar", "CC",0),
            new EnemyBuffApplyMechanic(Exposed31589, "Breakbar End", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC.Fail","Breakbar (Failed CC)", "CC Fail",0, (bae,log) => bae.To.ID == (int)ArcDPSEnums.TargetID.Arkk && !log.CombatData.GetAnimatedCastData(ArkkBreakbarCast).Any(x => bae.To == x.Caster && x.Time < bae.Time && bae.Time < x.ExpectedEndTime + ParserHelper.ServerDelayConstant)),
            new EnemyBuffApplyMechanic(Exposed31589, "Breakbar End", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","Breakbar broken", "CCed",0, (bae,log) => bae.To.ID == (int)ArcDPSEnums.TargetID.Arkk && log.CombatData.GetAnimatedCastData(ArkkBreakbarCast).Any(x => bae.To == x.Caster && x.Time < bae.Time && bae.Time < x.ExpectedEndTime + ParserHelper.ServerDelayConstant)),
            new HitOnPlayerMechanic(OverheadSmashArchdiviner, "Overhead Smash", new MechanicPlotlySetting("triangle-left-open","rgb(200,0,0)"), "A.Smsh","Overhead Smash (Arcdiviner)", "Smash (Add)",0),
            new HitOnPlayerMechanic(RollingChaos, "Rolling Chaos", new MechanicPlotlySetting("circle","rgb(255,50,50)"), "KD Marble","Rolling Chaos (Arrow marble)", "KD Marble",0),
            new HitOnPlayerMechanic(SolarStomp, "Solar Stomp", new MechanicPlotlySetting("triangle-up","rgb(200,0,200)"), "Stomp","Solar Stomp (Evading Stomp)", "Evading Jump",0),
            new EnemyCastStartMechanic(CosmicStreaks, "Cosmic Streaks", new MechanicPlotlySetting("diamond-open","rgb(255,0,100)"), "DDR Beam","Triple Death Ray Cast (last phase)", "Death Ray Cast",0),
            new HitOnPlayerMechanic(WhirlingDevastation, "Whirling Devastation", new MechanicPlotlySetting("star-diamond-open","rgb(180,0,100)"), "Whirl","Whirling Devastation (Gladiator Spin)", "Gladiator Spin",300),
            new EnemyCastStartMechanic(PullCharge, "Pull Charge", new MechanicPlotlySetting("bowtie","rgb(0,160,150)"), "Pull","Pull Charge (Gladiator Pull)", "Gladiator Pull",0), //
            new EnemyCastEndMechanic(PullCharge, "Pull Charge", new MechanicPlotlySetting("bowtie","rgb(255,0,0)"), "Pull CC Fail","Pull Charge CC failed", "CC fail (Gladiator)",0, (ce,log) => ce.ActualDuration > 3200), //
            new EnemyCastEndMechanic(PullCharge, "Pull Charge", new MechanicPlotlySetting("bowtie","rgb(0,160,0)"), "Pull CCed","Pull Charge CCed", "CCed (Gladiator)",0, (ce, log) => ce.ActualDuration < 3200), //
            new HitOnPlayerMechanic(SpinningCut, "Spinning Cut", new MechanicPlotlySetting("star-square-open","rgb(200,140,255)"), "Daze","Spinning Cut (3rd Gladiator Auto->Daze)", "Gladiator Daze",0), //
            });
            Extension = "arkk";
            Icon = "https://i.imgur.com/glLH8n8.png";
            EncounterCategoryInformation.InSubCategoryOrder = 2;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/BIybWJe.png",
                            (914, 914),
                            (-19231, -18137, -16591, -15677)/*,
                            (-24576, -24576, 24576, 24576),
                            (11204, 4414, 13252, 6462)*/);
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.TemporalAnomaly2,
                ArcDPSEnums.TrashID.BLIGHT,
                ArcDPSEnums.TrashID.Fanatic,
                ArcDPSEnums.TrashID.SolarBloom,
                ArcDPSEnums.TrashID.PLINK,
                ArcDPSEnums.TrashID.DOC,
                ArcDPSEnums.TrashID.CHOP,
                ArcDPSEnums.TrashID.ProjectionArkk
            };
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.CMStatus.CMnoName;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Arkk,
                (int)ArcDPSEnums.TrashID.Archdiviner,
                (int)ArcDPSEnums.TrashID.BrazenGladiator
            };
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            // reward or death worked
            if (fightData.Success)
            {
                return;
            }
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Arkk);
            if (target == null)
            {
                throw new MissingKeyActorsException("Arkk not found");
            }
            HashSet<AgentItem> adjustedPlayers = GetParticipatingPlayerAgents(target, combatData, playerAgents);
            // missing buff apply events fallback, some phases will be missing
            // removes should be present
            if (SetSuccessByBuffCount(combatData, fightData, adjustedPlayers, target, Determined762, 10))
            {
                var invulsRemoveTarget = combatData.GetBuffData(Determined762).OfType<BuffRemoveAllEvent>().Where(x => x.To == target.AgentItem).ToList();
                if (invulsRemoveTarget.Count == 5)
                {
                    SetSuccessByCombatExit(new List<AbstractSingleActor> { target }, combatData, fightData, adjustedPlayers);
                }
            }
        }
    }
}
