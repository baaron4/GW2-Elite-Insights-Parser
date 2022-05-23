using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Ensolyss : Nightmare
    {
        public Ensolyss(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(new long[] { Lunge1, Lunge2 }, "Lunge", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.LightOrange), "Charge","Lunge (KB charge over arena)", "Charge",150),
            new HitOnPlayerMechanic(new long[] { Upswing1, Upswing2 }, "Upswing", new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "Smash 1","High damage Jump hit", "First Smash",0),
            new HitOnPlayerMechanic(new long[] { NigthmareMiasmaEnsolyss1, NigthmareMiasmaEnsolyss2, NigthmareMiasmaEnsolyss3 }, "Nightmare Miasma", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Magenta), "Goo","Nightmare Miasma (Goo)", "Miasma",0),
            new EnemyCastStartMechanic(CausticExplosionEnsolyss, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC","After Phase CC", "Breakbar", 0),
            new EnemyCastEndMechanic(CausticExplosionEnsolyss, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail","After Phase CC Failed", "CC Fail", 0, (ce,log) => ce.ActualDuration >= 15260),
            new EnemyCastEndMechanic(CausticExplosionEnsolyss, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed","After Phase CC Success", "CCed", 0, (ce, log) => ce.ActualDuration < 15260),
            new HitOnPlayerMechanic(CausticExplosionEnsolyss, "Caustic Explosion", new MechanicPlotlySetting(Symbols.Bowtie,Colors.Yellow), "CC KB","Knockback hourglass during CC", "CC KB", 0),
            new EnemyCastStartMechanic(new long[] { NightmareDevastation1, NightmareDevastation2 }, "Nightmare Devastation", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Blue), "Bubble","Nightmare Devastation (bubble attack)", "Bubble",0),
            new HitOnPlayerMechanic(TailLashEnsolyss, "Tail Lash", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Yellow), "Tail","Tail Lash (half circle Knockback)", "Tail Lash",0),
            new HitOnPlayerMechanic(RampageEnsolyss, "Rampage", new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.Red), "Rampage","Rampage (asterisk shaped Arrow attack)", "Rampage",150),
            new HitOnPlayerMechanic(CausticGrasp, "Caustic Grasp", new MechanicPlotlySetting(Symbols.StarDiamond,Colors.LightOrange), "Pull","Caustic Grasp (Arena Wide Pull)", "Pull",0),
            new HitOnPlayerMechanic(TormentingBlast, "Tormenting Blast", new MechanicPlotlySetting(Symbols.Diamond,Colors.Yellow), "Quarter","Tormenting Blast (Two Quarter Circle attacks)", "Quarter circle",0),
            });
            Extension = "ensol";
            Icon = "https://i.imgur.com/pqRYRGi.png";
            EncounterCategoryInformation.InSubCategoryOrder = 2;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/kjelZ4t.png",
                            (366, 366),
                            (252, 1, 2892, 2881)/*,
                            (-6144, -6144, 9216, 9216),
                            (11804, 4414, 12444, 5054)*/);
        }
        internal override long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            return GetFightOffsetByFirstInvulFilter(fightData, agentData, combatData, (int)ArcDPSEnums.TargetID.Ensolyss, Determined762, 1500);
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.CMStatus.CMnoName;
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.NightmareHallucination1,
                ArcDPSEnums.TrashID.NightmareHallucination2
            };
        }
    }
}
