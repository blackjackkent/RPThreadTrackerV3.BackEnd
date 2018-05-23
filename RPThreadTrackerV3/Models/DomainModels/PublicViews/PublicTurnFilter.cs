namespace RPThreadTrackerV3.Models.DomainModels.PublicViews
{
    public class PublicTurnFilter
    {
        public bool IncludeMyTurn { get; set; }
        public bool IncludeTheirTurn { get; set; }
        public bool IncludeQueued { get; set; }
        public bool IncludeArchived { get; set; }
    }
}
