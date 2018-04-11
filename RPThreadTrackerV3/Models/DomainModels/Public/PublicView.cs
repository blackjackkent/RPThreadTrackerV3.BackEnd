using System.Collections.Generic;

namespace RPThreadTrackerV3.Models.DomainModels.Public
{
    public class PublicView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string UserId { get; set; }
        public List<string> Columns { get; set; }
        public string SortKey { get; set; }
        public PublicTurnFilter TurnFilter { get; set; }
        public List<int> CharacterIds { get; set; }
        public List<string> Tags { get; set; }
    }
}
