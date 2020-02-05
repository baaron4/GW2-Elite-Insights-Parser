using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Logic
{
    public class WhisperOfJormag : StrikeMissionLogic
    {
        public WhisperOfJormag(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            }
            );
            Extension = "woj";
            Icon = "https://i.imgur.com/8GLwgfL.png";
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                ParseEnum.TrashIDS.WhisperEcho,
                ParseEnum.TrashIDS.DoppelgangerGuardian1,
                ParseEnum.TrashIDS.DoppelgangerGuardian2,
                ParseEnum.TrashIDS.DoppelgangerNecro,
                ParseEnum.TrashIDS.DoppelgangerRevenant,
                ParseEnum.TrashIDS.DoppelgangerThief1,
                ParseEnum.TrashIDS.DoppelgangerThief2,
                ParseEnum.TrashIDS.DoppelgangerWarrior,
            };
        }
    }
}
