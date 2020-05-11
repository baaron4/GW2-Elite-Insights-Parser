using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.Builders.HtmlModels
{

    public abstract class ActorDto
    {
        public int CombatReplayID { get; set; }
        public string Name { get; set; }
        public uint Tough { get; set; }
        public List<MinionDto> Minions { get; } = new List<MinionDto>();
        public ActorDetailsDto Details { get; set; }

        protected ActorDto(AbstractSingleActor actor, ParsedLog log, bool cr, ActorDetailsDto details)
        {
            Name = actor.Character;
            Tough = actor.Toughness;
            Details = details;
            if (cr)
            {
                CombatReplayID = actor.CombatReplayID;
            }
            foreach (KeyValuePair<long, Minions> pair in actor.GetMinions(log))
            {
                Minions.Add(new MinionDto()
                {
                    Id = pair.Key,
                    Name = pair.Value.Character
                });
            }
        }
    }
}
