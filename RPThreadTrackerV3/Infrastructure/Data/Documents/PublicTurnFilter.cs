using RPThreadTrackerV3.Interfaces.Data;

namespace RPThreadTrackerV3.Infrastructure.Data.Documents
{
    public class PublicTurnFilter : IDocument
    {
        public bool IncludeMyTurn { get; set; }
        public bool IncludeTheirTurn { get; set; }
        public bool IncludeQueued { get; set; }
        public bool IncludeArchived { get; set; }
    }
}
