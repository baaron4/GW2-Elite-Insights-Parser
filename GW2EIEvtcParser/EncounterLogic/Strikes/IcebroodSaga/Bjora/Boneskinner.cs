using System.Linq;
using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Boneskinner : Bjora
    {
        public Boneskinner(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                // TODO find this
                //new HitOnPlayerMechanic(58811, "Grasp", new MechanicPlotlySetting(Symbols.Hexagram,Colors.Red), "Grasp","Grasp (hit by claw AoE)", "Grasp",4000),
                new PlayerDstHitMechanic(BoneskinnerCharge, "Charge", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "H.Charge","Hit by Charge", "Charge hit",0),
                new PlayerDstHitMechanic(DeathWind, "Death Wind", new MechanicPlotlySetting(Symbols.Star,Colors.Red), "Launched","Launched by Death Wind", "Launched",0).UsingChecker((de, log) => !de.To.HasBuff(log, SkillIDs.Stability, de.Time - ParserHelper.ServerDelayConstant)),
                new EnemyCastEndMechanic(BoneskinnerCharge, "Charge", new MechanicPlotlySetting(Symbols.Hexagram,Colors.LightRed), "D.Torch","Destroyed a Torch", "Destroyed a Torch",0).UsingChecker((ce, log) => ce.Status != AbstractCastEvent.AnimationStatus.Interrupted),
                new EnemyCastEndMechanic(DeathWind, "Death Wind", new MechanicPlotlySetting(Symbols.Square,Colors.Orange), "D.Wind","Death Wind (extinguished torches)", "Death Wind",0).UsingChecker((ce, log) => ce.Status != AbstractCastEvent.AnimationStatus.Interrupted),
                new EnemyCastEndMechanic(DouseInDarkness, "Douse in Darkness", new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "D.Darkness","Douse in Darkness (extinguished torches)", "Douse in Darkness",0).UsingChecker((ce, log) => ce.Status != AbstractCastEvent.AnimationStatus.Interrupted),
            }
            );
            Extension = "boneskin";
            Icon = EncounterIconBoneskinner;
            EncounterCategoryInformation.InSubCategoryOrder = 2;
            EncounterID |= 0x000004;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayBoneskinner,
                            (905, 789),
                            (-1013, -1600, 2221, 1416)/*,
                            (-0, -0, 0, 0),
                            (0, 0, 0, 0)*/);
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(UnnaturalAura, UnnaturalAura), // Unnatural Aura
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.VigilTactician,
                ArcDPSEnums.TrashID.VigilRecruit,
                ArcDPSEnums.TrashID.PrioryExplorer,
                ArcDPSEnums.TrashID.PrioryScholar,
                ArcDPSEnums.TrashID.AberrantWisp,
            };
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);
            IReadOnlyList<AbstractBuffEvent> holdOntoTheLight = log.CombatData.GetBuffData(AchievementEligibilityHoldOntoTheLight);

            if (holdOntoTheLight.Any() && log.FightData.Success)
            {
                foreach (Player p in log.PlayerList)
                {
                    if (p.HasBuff(log, AchievementEligibilityHoldOntoTheLight, log.FightData.FightEnd - ServerDelayConstant))
                    {
                        InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityHoldOntoTheLight], 1));
                        break;
                    }
                }
            }
        }
    }
}
