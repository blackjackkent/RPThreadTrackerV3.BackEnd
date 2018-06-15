namespace RPThreadTrackerV3.BackEnd.Test.TestHelpers
{
    using BackEnd.Infrastructure.Data;
    using Data;
    using Microsoft.EntityFrameworkCore;

    public class MockTrackerContext : TrackerContext
    {
        public virtual DbSet<MockEntityPoco> MockEntityPocos { get; set; }

        public virtual DbSet<MockEntityNavigationProperty> MockEntityNavigationProperties { get; set; }
    }
}
