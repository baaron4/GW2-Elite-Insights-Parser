using System.Linq;
using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
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
                new PlayerDstHitMechanic(Grasp, "Grasp", new MechanicPlotlySetting(Symbols.Circle, Colors.Grey), "Grasp.H", "Grasp (Claw AoE)", "Grasp Hit", 0),
                new PlayerDstHitMechanic(Cascade, "Cascade", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkRed), "Cascade.H", "Cascade (Rectangle AoEs from paws stomp)", "Cascade Hit", 0),
                new PlayerDstHitMechanic(BoneskinnerCharge, "Charge", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "Charge.H", "Hit by Charge", "Charge hit", 0),
                new PlayerDstHitMechanic(CrushingCruelty, "Crushing Cruelty", new MechanicPlotlySetting(Symbols.Star, Colors.DarkGreen), "Crush.Cru.H", "Hit by Crushing Cruelty (Jump middle after Charge)", "Crushing Cruelty Hit", 0),
                new PlayerDstHitMechanic(DeathWind, "Death Wind", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Orange), "DeathWind.H", "Hit by Death Wind", "Death Wind Hit", 0), // This attack removes stability
                new PlayerDstHitMechanic(DouseInDarkness, "Douse in Darkness", new MechanicPlotlySetting(Symbols.Cross, Colors.DarkTeal), "DouseDarkness.H", "Hit by Douse in Darkness", "Douse in Darkness Hit", 0),
                new PlayerDstHitMechanic(BarrageWispBoneskinner, "Barrage", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Green), "Barrage.H", "Hit by Barrage (Wisp AoE)", "Barrage Hit", 0),
                new PlayerDstBuffApplyMechanic(UnrelentingPainEffect, "Unrelenting Pain", new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.Pink), "UnrelPain.A", "Unreleting Pain Applied", "Unrelenting Pain Applied", 0),
                new EnemyCastEndMechanic(BoneskinnerCharge, "Charge", new MechanicPlotlySetting(Symbols.Hexagram, Colors.LightRed), "Charge", "Charged a torch", "Charge", 0).UsingChecker((ce, log) => ce.Status != AbstractCastEvent.AnimationStatus.Interrupted),
                new EnemyCastEndMechanic(DeathWind, "Death Wind", new MechanicPlotlySetting(Symbols.TriangleUpOpen, Colors.LightOrange), "Death Wind", "Cast Death Wind (extinguished one torch)", "Death Wind", 0).UsingChecker((ce, log) => ce.Status != AbstractCastEvent.AnimationStatus.Interrupted),
                new EnemyCastEndMechanic(DouseInDarkness, "Douse in Darkness", new MechanicPlotlySetting(Symbols.Cross, Colors.Teal), "Douse in Darkness", "Cast Douse in Darkness (extinguished all torches)", "Douse in Darkness", 0).UsingChecker((ce, log) => ce.Status != AbstractCastEvent.AnimationStatus.Interrupted),
                new EnemyCastStartMechanic(BoneskinnerBreakbar, "Breakbar", new MechanicPlotlySetting(Symbols.Square, Colors.Purple), "Breakbar", "Casting a Breakbar", "Breakbar", 0),
                new EnemyDstBuffApplyMechanic(Exposed31589, "Exposed" , new MechanicPlotlySetting(Symbols.SquareOpen, Colors.Pink), "Exposed", "Gained Exposed (Breakbar broken)", "Exposed", 0),
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
                new DamageCastFinder(UnnaturalAura, UnnaturalAura),
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
                ArcDPSEnums.TrashID.Torch,
            };
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            var torches = combatData.Where(x => x.DstAgent == 14940 && x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxHeight == 500 && x.HitboxWidth >= 250).ToList();
            foreach (AgentItem torch in torches)
            {
                torch.OverrideType(AgentItem.AgentType.NPC);
                torch.OverrideID(ArcDPSEnums.TrashID.Torch);
                torch.OverrideAwareTimes(fightData.LogStart, fightData.LogEnd);
            }
            agentData.Refresh();
            ComputeFightTargets(agentData, combatData, extensions);
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
