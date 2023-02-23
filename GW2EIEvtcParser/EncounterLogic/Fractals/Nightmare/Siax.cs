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
    internal class Siax : Nightmare
    {
        public Siax(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(VileSpit, "Vile Spit", new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "Spit","Vile Spit (green goo)", "Poison Spit",0),
            new HitOnPlayerMechanic(TailLashSiax, "Tail Lash", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Yellow), "Tail","Tail Lash (half circle Knockback)", "Tail Lash",0),
            new SpawnMechanic((int)ArcDPSEnums.TrashID.NightmareHallucinationSiax, "Nightmare Hallucination", new MechanicPlotlySetting(Symbols.StarOpen,Colors.Black), "Hallu","Nightmare Hallucination Spawn", "Hallucination",0),
            new HitOnPlayerMechanic(new long[] {CascadeOfTorment1, CascadeOfTorment2 }, "Cascade of Torment", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0),
            new EnemyCastStartMechanic(CausticExplosion1Siax, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Yellow), "Phase","Phase Start", "Phase", 0),
            new EnemyCastEndMechanic(CausticExplosion1Siax, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "Phase Fail","Phase Fail (Failed to kill Echos in time)", "Phase Fail", 0, (ce,log) => ce.ActualDuration >= 20649), //
            new EnemyCastStartMechanic(CausticExplosion2Siax, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.DarkTeal), "CC","Breakbar Start", "Breakbar", 0),
            new EnemyCastEndMechanic(CausticExplosion2Siax, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.Red), "CC Fail","Failed to CC in time", "CC Fail", 0, (ce,log) => ce.ActualDuration >= 15232),
            new PlayerDstBuffApplyMechanic(FixatedNightmare, "Fixated", new MechanicPlotlySetting(Symbols.StarOpen,Colors.Magenta), "Fixate", "Fixated by Volatile Hallucination", "Fixated",0),
            });
            Extension = "siax";
            Icon = EncounterIconSiax;
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplaySiax,
                            (476, 548),
                            (663, -4127, 3515, -997)/*,
                            (-6144, -6144, 9216, 9216),
                            (11804, 4414, 12444, 5054)*/);
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.SiaxHallucination,
                ArcDPSEnums.TrashID.NightmareHallucinationSiax
            };
        }
        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Siax,
                (int)ArcDPSEnums.TrashID.EchoOfTheUnclean,
            };
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterMode.CMNoName;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor siax = Targets.FirstOrDefault(x => x.IsSpecy(ArcDPSEnums.TargetID.Siax));
            if (siax == null)
            {
                throw new MissingKeyActorsException("Siax not found");
            }
            phases[0].AddTarget(siax);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, Determined762, siax, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    var ids = new List<int>
                    {
                       (int) ArcDPSEnums.TrashID.EchoOfTheUnclean,
                    };
                    AddTargetsToPhaseAndFit(phase, ids, log);
                    phase.Name = "Caustic Explosion " + (i/2);
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(siax);
                }
            }
            return phases;
        }

    }
}
