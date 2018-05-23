﻿namespace RPThreadTrackerV3.Infrastructure.Data.Documents
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using RPThreadTrackerV3.Interfaces.Data;

    public class PublicView : IDocument
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Name { get; }
        public string Slug { get; }
        public string UserId { get; }
        public List<string> Columns { get; }
        public string SortKey { get; }
        public bool SortDescending { get; }
        public PublicTurnFilter TurnFilter { get; }
        public List<int> CharacterIds { get; }
        public List<string> Tags { get; }
    }
}
