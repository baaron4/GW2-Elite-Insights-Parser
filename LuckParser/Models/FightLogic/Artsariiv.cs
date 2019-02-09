using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class Artsariiv : FractalLogic
    {
        public Artsariiv(ushort triggerID) : base(triggerID)
        {           
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerBoonApplyMechanic(38880, "Corporeal Reassignment", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "Skull","Exploding Skull mechanic application","Corporeal Reassignment",0),
            new HitOnPlayerMechanic(38977, "Vault", new MechanicPlotlySetting("triangle-down-open","rgb(255,200,0)"), "Vault","Vault from Big Adds", "Vault (Add)",0),
            new HitOnPlayerMechanic(39925, "Slam", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Slam","Slam (Vault) from Boss", "Vault (Arts)",0),
            new HitOnPlayerMechanic(39469, "Teleport Lunge", new MechanicPlotlySetting("star-triangle-down-open","rgb(255,140,0)"), "3 Jump","Triple Jump Mid->Edge", "Triple Jump",0),
            new HitOnPlayerMechanic(39035, "Astral Surge", new MechanicPlotlySetting("circle-open","rgb(255,200,0)"), "Floor Circle","Different sized spiraling circles", "1000 Circles",0),
            new HitOnPlayerMechanic(39029, "Red Marble", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Marble","Red KD Marble after Jump", "Red Marble",0), 
            new HitOnPlayerMechanic(39863, "Red Marble", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Marble","Red KD Marble after Jump", "Red Marble",0), 
            new PlayerBoonApplyMechanic(791, "Fear", new MechanicPlotlySetting("square-open","rgb(255,0,0)"), "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)" ,0, new List<MechanicChecker>{ new CombatItemValueChecker(3000, MechanicChecker.ValueCompare.EQ) }, Mechanic.TriggerRule.AND), //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new SpawnMechanic(17630, "Spark", new MechanicPlotlySetting("star","rgb(0,255,255)"),"Spark","Spawned a Spark (missed marble)", "Spark",0),
            });
            Extension = "arts";
            IconUrl = "https://wiki.guildwars2.com/images/b/b4/Artsariiv.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/4wmuc8B.png",
                            (914, 914),
                            (8991, 112, 11731, 2812),
                            (-24576, -24576, 24576, 24576),
                            (11204, 4414, 13252, 6462));
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                TemporalAnomaly,
                Spark,
                Artsariiv1,
                Artsariiv2,
                Artsariiv3
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)TemporalAnomaly:
                case (ushort)Spark:
                case (ushort)Artsariiv1:
                case (ushort)Artsariiv2:
                case (ushort)Artsariiv3:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            CombatReplay replay = target.CombatReplay;
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Artsariiv:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void SetSuccess(ParsedLog log)
        {
            Target mainTarget = Targets.Find(x => x.ID == TriggerID);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            HashSet<ushort> pIds = log.PlayerIDs;
            int combatExits = log.CombatData.GetStatesData(mainTarget.InstID, ParseEnum.StateChange.ExitCombat, mainTarget.FirstAware, mainTarget.LastAware).Count;
            CombatItem lastDamageTaken = log.CombatData.GetDamageTakenData(mainTarget.InstID, mainTarget.FirstAware, mainTarget.LastAware).LastOrDefault(x => (x.Value > 0 || x.BuffDmg > 0) && pIds.Contains(x.SrcInstid));
            if (combatExits == 3 && lastDamageTaken != null)
            {
                CombatItem lastPlayerExit = log.CombatData.GetStates(ParseEnum.StateChange.ExitCombat).Where(x => pIds.Contains(x.SrcInstid)).LastOrDefault();
                CombatItem lastTargetExit = log.CombatData.GetStatesData(mainTarget.InstID, ParseEnum.StateChange.ExitCombat, mainTarget.FirstAware, mainTarget.LastAware).LastOrDefault();
                log.FightData.Success = lastPlayerExit != null && lastTargetExit != null && lastPlayerExit.Time - lastTargetExit.Time > 1000 ? true : false;
                log.FightData.FightEnd = lastDamageTaken.Time;
            }
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
        }
    }
}
