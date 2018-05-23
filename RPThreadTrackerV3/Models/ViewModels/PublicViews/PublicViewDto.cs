namespace RPThreadTrackerV3.Models.ViewModels.PublicViews
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Infrastructure.Exceptions.PublicViews;

    public class PublicViewDto
    {
        public string Id { get; }
        public string Name { get; }
        public string Slug { get; }
        public string Url { get; }
        public string UserId { get; set; }
        public List<string> Columns { get; }
        public string SortKey { get; }
        public bool SortDescending { get; }
        public PublicTurnFilterDto TurnFilter { get; }
        public List<int> CharacterIds { get; }
        public List<string> Tags { get; }

        public void AssertIsValid()
        {
            TurnFilter.AssertIsValid();
            var slugRegex = new Regex(@"^[A-Za-z0-9]+(?:-[A-Za-z0-9]+)*$");
            List<string> reservedSlugs = new List<string> { "myturn", "yourturn", "theirturn", "archived", "queued", "legacy" };
            var invalid =
                string.IsNullOrEmpty(Name)
                || string.IsNullOrEmpty(Slug)
                || !slugRegex.IsMatch(Slug)
                || !Columns.Any()
                || !CharacterIds.Any()
                || reservedSlugs.Contains(Slug);
            if (invalid)
            {
                throw new InvalidPublicViewException();
            }

        }
    }
}
