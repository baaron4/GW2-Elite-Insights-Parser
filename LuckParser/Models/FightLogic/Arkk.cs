using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class Arkk : FractalLogic
    {
        public Arkk(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new DamageOnPlayerMechanic(39685, "Horizon Strike", new MechanicPlotlySetting("circle", "rgb(255,140,0)"), "Horizon Strike","Horizon Strike (turning pizza slices)", "Horizon Strike",0),
            new DamageOnPlayerMechanic(39001, "Horizon Strike", new MechanicPlotlySetting("circle", "rgb(255,140,0)"), "Horizon Strike","Horizon Strike (turning pizza slices)", "Horizon Strike",0),
            new DamageOnPlayerMechanic(39787, "Diffractive Edge", new MechanicPlotlySetting("star","rgb(255,200,0)"), "5 Cone","Diffractive Edge (5 Cone Knockback)", "Five Cones",0),
            new DamageOnPlayerMechanic(39755, "Diffractive Edge", new MechanicPlotlySetting("star","rgb(255,200,0)"), "5 Cone","Diffractive Edge (5 Cone Knockback)", "Five Cones",0),
            new DamageOnPlayerMechanic(39728, "Solar Fury", new MechanicPlotlySetting("circle","rgb(128,0,0)"), "Ball","Stood in Red Overhead Ball Field", "Red Ball Aoe",0),
            new DamageOnPlayerMechanic(39711, "Focused Rage", new MechanicPlotlySetting("triangle-down","rgb(255,100,0)"), "Cone KB","Knockback in Cone with overhead crosshair", "Knockback Cone",0),
            new DamageOnPlayerMechanic(39691, "Solar Discharge", new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Shockwave","Knockback shockwave after Overhead Balls", "Shockwave",0),
            new DamageOnPlayerMechanic(38982, "Starburst Cascade", new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Float Ring","Starburst Cascade (Expanding/Retracting Lifting Ring)", "Float Ring",500),
            new DamageOnPlayerMechanic(39523, "Starburst Cascade", new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Float Ring","Starburst Cascade (Expanding/Retracting Lifting Ring)", "Float Ring",500), 
            new DamageOnPlayerMechanic(39297, "Horizon Strike Normal", new MechanicPlotlySetting("circle","rgb(80,0,0)"), "Horizon Strike norm","Horizon Strike (normal)", "Horizon Strike (normal)",0),
            new DamageOnPlayerMechanic(38844, "Overhead Smash", new MechanicPlotlySetting("triangle-left","rgb(200,0,0)"), "Smash","Overhead Smash","Overhead Smash",0),
            new PlayerBoonApplyMechanic(38880, "Corporeal Reassignment", new MechanicPlotlySetting("diamond","rgb(255,0,0)"), "Skull","Exploding Skull mechanic application", "Corporeal Reassignment",0),
            new DamageOnPlayerMechanic(39849, "Explode", new MechanicPlotlySetting("circle","rgb(255,200,0)"), "Bloom Explode","Hit by Solar Bloom explosion", "Bloom Explosion",0),
            new PlayerBoonApplyMechanic(39558, "Fixate", new MechanicPlotlySetting("star-open","rgb(255,0,255)"), "Bloom Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new PlayerBoonApplyMechanic(39928, "Fixate", new MechanicPlotlySetting("star-open","rgb(255,0,255)"), "Bloom Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new PlayerBoonApplyMechanic(39131, "Fixate", new MechanicPlotlySetting("star-open","rgb(255,0,255)"), "Bloom Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new PlayerBoonApplyMechanic(38985, "Fixate", new MechanicPlotlySetting("star-open","rgb(255,0,255)"), "Bloom Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new PlayerBoonApplyMechanic(39268, "Cosmic Meteor", new MechanicPlotlySetting("circle-open","rgb(0,255,0)"), "Green","Temporal Realignment (Green) application", "Green",0),
            new PlayerBoonApplyMechanic(791, "Fear", new MechanicPlotlySetting("square-open","rgb(255,0,0)"), "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)",0, new List<MechanicChecker>{ new CombatItemValueChecker(3000, MechanicChecker.ValueCompare.EQ) }, Mechanic.TriggerRule.AND), // //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new EnemyCastStartMechanic(39645, "Breakbar Start", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "Breakbar","Start Breakbar", "CC",0),
            new EnemyCastEndMechanic(39645, "Breakbar End", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC.Fail","Breakbar (Failed CC)", "CC Fail",0, new List<MechanicChecker>{ new CombatItemValueChecker(9668, MechanicChecker.ValueCompare.G) }, Mechanic.TriggerRule.AND),
            new EnemyCastEndMechanic(39645, "Breakbar End", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","Breakbar broken", "CCed",0, new List<MechanicChecker>{ new CombatItemValueChecker(9668, MechanicChecker.ValueCompare.L) }, Mechanic.TriggerRule.AND),
            new DamageOnPlayerMechanic(34748, "Overhead Smash", new MechanicPlotlySetting("triangle-left-open","rgb(200,0,0)"), "A.Smsh","Overhead Smash (Arcdiviner)", "Smash (Add)",0),
            new DamageOnPlayerMechanic(39674, "Rolling Chaos", new MechanicPlotlySetting("circle","rgb(255,50,50)"), "KD Marble","Rolling Chaos (Arrow marble)", "KD Marble",0),
            new DamageOnPlayerMechanic(39298, "Solar Stomp", new MechanicPlotlySetting("triangle-up","rgb(200,0,200)"), "Stomp","Solar Stomp (Evading Stomp)", "Evading Jump",0),
            new EnemyCastStartMechanic(39021, "Cosmic Streaks", new MechanicPlotlySetting("diamond-open","rgb(255,0,100)"), "DDR Beam","Triple Death Ray Cast (last phase)", "Death Ray Cast",0),
            new DamageOnPlayerMechanic(35940, "Whirling Devastation", new MechanicPlotlySetting("star-diamond-open","rgb(180,0,100)"), "Whirl","Whirling Devastation (Gladiator Spin)", "Gladiator Spin",300),
            new EnemyCastStartMechanic(35761, "Pull Charge", new MechanicPlotlySetting("bowtie","rgb(0,160,150)"), "Pull","Pull Charge (Gladiator Pull)", "Gladiator Pull",0), //
            new EnemyCastEndMechanic(35761, "Pull Charge", new MechanicPlotlySetting("bowtie","rgb(255,0,0)"), "Pull CC Fail","Pull Charge CC failed", "CC fail (Gladiator)",0, new List<MechanicChecker>{ new CombatItemValueChecker(3200, MechanicChecker.ValueCompare.G) }, Mechanic.TriggerRule.AND), //
            new EnemyCastEndMechanic(35761, "Pull Charge", new MechanicPlotlySetting("bowtie","rgb(0,160,0)"), "Pull CCed","Pull Charge CCed", "CCed (Gladiator)",0, new List<MechanicChecker>{ new CombatItemValueChecker(3200, MechanicChecker.ValueCompare.LEQ) }, Mechanic.TriggerRule.AND), //
            new DamageOnPlayerMechanic(35452, "Spinning Cut", new MechanicPlotlySetting("star-square-open","rgb(200,140,255)"), "Daze","Spinning Cut (3rd Gladiator Auto->Daze)", "Gladiator Daze",0), //
            });
            Extension = "arkk";
            IconUrl = "https://wiki.guildwars2.com/images/5/5f/Arkk.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/BIybWJe.png",
                            (914, 914),
                            (-19231, -18137, -16591, -15677),
                            (-24576, -24576, 24576, 24576),
                            (11204, 4414, 13252, 6462));
        }

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            CombatReplay replay = target.CombatReplay;
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Arkk:
                case (ushort)Archdiviner:
                case (ushort)BrazenGladiator:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)TemporalAnomaly2:
                case (ushort)BLIGHT:
                case (ushort)Fanatic:
                case (ushort)SolarBloom2:
                case (ushort)PLINK:
                case (ushort)DOC:
                case (ushort)CHOP:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                TemporalAnomaly2,
                BLIGHT,
                Fanatic,
                SolarBloom2,
                PLINK,
                DOC,
                CHOP
            };
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.TargetIDS.Arkk,
                (ushort)Archdiviner,
                (ushort)BrazenGladiator
            };
        }

        public override void SetSuccess(ParsedLog log)
        {
            Target mainTarget = Targets.Find(x => x.ID == TriggerID);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            List<CombatItem> invulsTarget = GetFilteredList(log, 762, mainTarget, true);
            if (invulsTarget.Count == 10)
            {
                CombatItem last = invulsTarget.Last();
                if (last.IsBuffRemove != ParseEnum.BuffRemove.None)
                {
                    HashSet<ushort> pIds = log.PlayerIDs;
                    CombatItem lastPlayerExit = log.CombatData.GetStates(ParseEnum.StateChange.ExitCombat).Where(x => pIds.Contains(x.SrcInstid)).LastOrDefault();
                    CombatItem lastTargetExit = log.CombatData.GetStatesData(mainTarget.InstID, ParseEnum.StateChange.ExitCombat, mainTarget.FirstAware, mainTarget.LastAware).LastOrDefault();
                    CombatItem lastDamageTaken = log.CombatData.GetDamageTakenData(mainTarget.InstID, mainTarget.FirstAware, mainTarget.LastAware).LastOrDefault(x => (x.Value > 0 || x.BuffDmg > 0) && pIds.Contains(x.SrcInstid));
                    if (lastTargetExit != null && lastDamageTaken != null && lastPlayerExit != null)
                    {
                        log.FightData.FightEnd = lastDamageTaken.Time;
                        log.FightData.Success = lastPlayerExit.Time > lastTargetExit.Time + 1000;
                    }
                }
            }
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
        }
    }
}
