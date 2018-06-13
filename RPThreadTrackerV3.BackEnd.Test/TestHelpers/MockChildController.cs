namespace RPThreadTrackerV3.BackEnd.Test.TestHelpers
{
    using BackEnd.Controllers;

    public class MockChildController : BaseController
    {
        public string RetrieveUserId()
        {
            return UserId;
        }
    }
}
