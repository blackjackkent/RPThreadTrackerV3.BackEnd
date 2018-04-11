using System.Collections.Generic;
using Newtonsoft.Json;
using RPThreadTrackerV3.Interfaces.Data;

namespace RPThreadTrackerV3.Infrastructure.Data.Documents
{
    public class PublicView : IDocument
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string UserId { get; set; }
        public List<string> Columns { get; set; }
        public string SortKey { get; set; }
        public Models.DomainModels.Public.PublicTurnFilter TurnFilter { get; set; }
        public List<int> CharacterIds { get; set; }
        public List<string> Tags { get; set; }
    }
}
