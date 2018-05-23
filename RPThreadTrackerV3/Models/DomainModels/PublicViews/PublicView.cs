namespace RPThreadTrackerV3.Models.DomainModels.PublicViews
{
    using System.Collections.Generic;

    public class PublicView
    {
        public string Id { get; }
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
