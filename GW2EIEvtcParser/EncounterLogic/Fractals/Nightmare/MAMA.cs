using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class MAMA : Nightmare
    {
        public MAMA(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(new long[]{ Blastwave1, Blastwave2 }, "Blastwave", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "KB","Blastwave (Spinning Knockback)", "KB Spin",0),
            new HitOnPlayerMechanic(TantrumMAMA, "Tantrum", new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.Green), "Tantrum","Tantrum (Double hit or Slams)", "Dual Spin/Slams",700),
            new HitOnPlayerMechanic(Leap, "Leap", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Red), "Jump","Leap (<33% only)", "Leap",0),
            new HitOnPlayerMechanic(Shoot, "Shoot", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Brown), "Shoot","Toxic Shoot (Green Bullets)", "Toxic Shoot",0),
            new HitOnPlayerMechanic(ExplosiveImpact, "Explosive Impact", new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Knight Jump","Explosive Impact (Knight Jump)", "Knight Jump",0),
            new HitOnPlayerMechanic(SweepingStrikes, "Sweeping Strikes", new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.Red), "Sweep","Swings (Many rapid front spins)", "Sweeping Strikes",200),
            new HitOnPlayerMechanic(NigthmareMiasmaMAMA, "Nightmare Miasma", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Magenta), "Goo","Nightmare Miasma (Poison Puddle)", "Poison Goo",700),
            new HitOnPlayerMechanic(GrenadeBarrare, "Grenade Barrage", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Barrage","Grenade Barrage (many projectiles in all directions)", "Ball Barrage",0),
            new HitOnPlayerMechanic(RedBallShot, "Red Ball Shot", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Ball","Small Red Bullets", "Bullet",0),
            new HitOnPlayerMechanic(Extraction, "Extraction", new MechanicPlotlySetting(Symbols.Bowtie,Colors.LightOrange), "Pull","Extraction (Knight Pull Circle)", "Knight Pull",0),
            new HitOnPlayerMechanic(HomingGrenades, "Homing Grenades", new MechanicPlotlySetting(Symbols.StarTriangleDownOpen,Colors.Red), "Grenades","Homing Grenades", "Homing Grenades",0),
            new HitOnPlayerMechanic(new long[] {CascadeOfTorment1, CascadeOfTorment2 }, "Cascade of Torment", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0),
            new HitOnPlayerMechanic(KnightsGaze, "Knight's Daze", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.LightPurple), "Daze","Knight's Daze", "Daze", 0),

            });
            Extension = "mama";
            Icon = "https://i.imgur.com/9URW7wh.png";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/lFGNKuf.png",
                            (664, 407),
                            (1653, 4555, 5733, 7195)/*,
                            (-6144, -6144, 9216, 9216),
                            (11804, 4414, 12444, 5054)*/);
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.CMStatus.CMnoName;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mama = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.MAMA);
            if (mama == null)
            {
                throw new MissingKeyActorsException("MAMA not found");
            }
            phases[0].AddTarget(mama);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, Determined762, mama, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    var ids = new List<int>
                    {
                       (int) ArcDPSEnums.TrashID.GreenKnight,
                       (int) ArcDPSEnums.TrashID.RedKnight,
                       (int) ArcDPSEnums.TrashID.BlueKnight,
                    };
                    AddTargetsToPhaseAndFit(phase, ids, log);
                    if (phase.Targets.Count > 0)
                    {
                        AbstractSingleActor phaseTar = phase.Targets[0];
                        switch (phaseTar.ID)
                        {
                            case (int)ArcDPSEnums.TrashID.GreenKnight:
                                phase.Name = "Green Knight";
                                break;
                            case (int)ArcDPSEnums.TrashID.RedKnight:
                                phase.Name = "Red Knight";
                                break;
                            case (int)ArcDPSEnums.TrashID.BlueKnight:
                                phase.Name = "Blue Knight";
                                break;
                            default:
                                phase.Name = "Unknown";
                                break;
                        }
                    }
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(mama);
                }
            }
            return phases;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.MAMA,
                (int)ArcDPSEnums.TrashID.GreenKnight,
                (int)ArcDPSEnums.TrashID.RedKnight,
                (int)ArcDPSEnums.TrashID.BlueKnight
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.TwistedHorror
            };
        }

        internal override long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            return GetFightOffsetByFirstInvulFilter(fightData, agentData, combatData, (int)ArcDPSEnums.TargetID.MAMA, Determined762, 1500);
        }
    }
}
