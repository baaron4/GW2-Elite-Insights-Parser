using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class MAMA : Nightmare
    {
        public MAMA(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstHitMechanic(new long[]{ Blastwave1, Blastwave2 }, "Blastwave", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "KB","Blastwave (Spinning Knockback)", "KB Spin",0),
            new PlayerDstHitMechanic(TantrumMAMA, "Tantrum", new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.Green), "Tantrum","Tantrum (Double hit or Slams)", "Dual Spin/Slams",700),
            new PlayerDstHitMechanic(Leap, "Leap", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Red), "Jump","Leap (<33% only)", "Leap",0),
            new PlayerDstHitMechanic(Shoot, "Shoot", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Brown), "Shoot","Toxic Shoot (Green Bullets)", "Toxic Shoot",0),
            new PlayerDstHitMechanic(ExplosiveImpact, "Explosive Impact", new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Knight Jump","Explosive Impact (Knight Jump)", "Knight Jump",0),
            new PlayerDstHitMechanic(SweepingStrikes, "Sweeping Strikes", new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.Red), "Sweep","Swings (Many rapid front spins)", "Sweeping Strikes",200),
            new PlayerDstHitMechanic(NigthmareMiasmaMAMA, "Nightmare Miasma", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Magenta), "Goo","Nightmare Miasma (Poison Puddle)", "Poison Goo",700),
            new PlayerDstHitMechanic(GrenadeBarrare, "Grenade Barrage", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Barrage","Grenade Barrage (many projectiles in all directions)", "Ball Barrage",0),
            new PlayerDstHitMechanic(RedBallShot, "Red Ball Shot", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Ball","Small Red Bullets", "Bullet",0),
            new PlayerDstHitMechanic(Extraction, "Extraction", new MechanicPlotlySetting(Symbols.Bowtie,Colors.LightOrange), "Pull","Extraction (Knight Pull Circle)", "Knight Pull",0),
            new PlayerDstHitMechanic(HomingGrenades, "Homing Grenades", new MechanicPlotlySetting(Symbols.StarTriangleDownOpen,Colors.Red), "Grenades","Homing Grenades", "Homing Grenades",0),
            new PlayerDstHitMechanic(new long[] { CascadeOfTorment1, CascadeOfTorment2 }, "Cascade of Torment", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0),
            new PlayerDstHitMechanic(KnightsGaze, "Knight's Daze", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.LightPurple), "Daze","Knight's Daze", "Daze", 0),

            });
            Extension = "mama";
            Icon = EncounterIconMAMA;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayMAMA,
                            (664, 407),
                            (1653, 4555, 5733, 7195)/*,
                            (-6144, -6144, 9216, 9216),
                            (11804, 4414, 12444, 5054)*/);
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterMode.CMNoName;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mama = Targets.FirstOrDefault(x => x.IsSpecy(ArcDPSEnums.TargetID.MAMA));
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
                        phase.Name = PhaseNames.TryGetValue(phaseTar.ID, out string phaseName) ? phaseName : "Unknown";
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

        private readonly IReadOnlyDictionary<int, string> PhaseNames = new Dictionary<int, string>()
        {
            { (int)ArcDPSEnums.TrashID.GreenKnight, "Green Knight" },
            { (int)ArcDPSEnums.TrashID.RedKnight, "Red Knight" },
            { (int)ArcDPSEnums.TrashID.BlueKnight, "Blue Knight" }
        };
    }
}
