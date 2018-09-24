using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class TwinLargos : RaidLogic
    {
        public TwinLargos(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(51935, "Waterlogged", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Nikare, "symbol:'circle-open',color:'rgb(0,140,255)',", "Wtlg","Waterlogged (stacking water debuff)", "Waterlogged",0),
            new Mechanic(52876, "Vapor Rush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Nikare, "symbol:'triangle-left-open',color:'rgb(0,140,255)',", "Chrg","Vapor Rush (Triple Charge)", "Vapor Rush Charge",0),
            new Mechanic(52812, "Tidal Pool", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Nikare, "symbol:'circle',color:'rgb(0,140,255)',", "Pool","Tidal Pool", "Tidal Pool",0),
            new Mechanic(51977, "Aquatic Barrage Start", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Nikare, "symbol:'diamond-tall',color:'rgb(0,160,150)',", "CC","Breakbar", "Breakbar",0),
            new Mechanic(51977, "Aquatic Barrage End", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Nikare, "symbol:'diamond-tall',color:'rgb(0,160,0)',", "CCed","Breakbar broken", "CCed",0),
            new Mechanic(53018, "Sea Swell", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Nikare, "symbol:'circle-open',color:'rgb(30,30,80)',", "Shkwv","Sea Swell (Shockwave)", "Shockwave",0),
            new Mechanic(53130, "Geyser", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Nikare, "symbol:'hexagon',color:'rgb(0,255,255)',", "Gysr","Geyser", "Geyser",0),
            new Mechanic(53097, "Water Bomb Debuff", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Nikare, "symbol:'diamond',color:'rgb(0,255,255)',", "Psn","Expanding Water Field", "Water Poison",0),
            new Mechanic(52931, "Aquatic Detainment", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Nikare, "symbol:'circle',color:'rgb(0,0,255)',", "Float","Aquatic Detainment (Float Bubble)", "Float Bubble",0),
            });
            CanCombatReplay = false;
            Extension = "twinlargos";
            IconUrl = "https://i.imgur.com/6O5MT7v.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/RMBeXhd.png",
                            Tuple.Create(5760, 7538),
                            Tuple.Create(10896, -2448, 18096, 8352),
                            Tuple.Create(-21504, -21504, 24576, 24576),
                            Tuple.Create(13440, 14336, 15360, 16256));
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>();
        }

        public override void ComputeAdditionalBossData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
        }

        public override void ComputeAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {

        }

        public override int IsCM(ParsedLog log)
        {
            return (log.Boss.Health > 18e6) ? 1 : 0; //Health of Nikare
        }

        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/6yq45Cc.png";
            // For Kenut: https://i.imgur.com/TLykcrJ.png
        }
    }
}
