using RPThreadTrackerV3.Infrastructure.Exceptions;

namespace RPThreadTrackerV3.Models.ViewModels.Public
{
    using Infrastructure.Exceptions.Public;

    public class PublicTurnFilterDto
    {
        public bool IncludeMyTurn { get; set; }
        public bool IncludeTheirTurn { get; set; }
        public bool IncludeQueued { get; set; }
        public bool IncludeArchived { get; set; }

        public void AssertIsValid()
        {
            if (!IncludeMyTurn && !IncludeTheirTurn && !IncludeQueued && !IncludeArchived)
            {
                throw new InvalidPublicViewException();
            }
        }
    }
}
