using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RPThreadTrackerV3.Infrastructure.Exceptions;

namespace RPThreadTrackerV3.Models.ViewModels.Public
{
    using Infrastructure.Exceptions.Public;

    public class PublicViewDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string UserId { get; set; }
        public List<string> Columns { get; set; }
        public string SortKey { get; set; }
        public bool SortDescending { get; set; }
        public PublicTurnFilterDto TurnFilter { get; set; }
        public List<int> CharacterIds { get; set; }
        public List<string> Tags { get; set; }

        public void AssertIsValid()
        {
            TurnFilter.AssertIsValid();
            var slugRegex = new Regex(@"^[A-Za-z0-9]+(?:-[A-Za-z0-9]+)*$");
            var invalid =
                string.IsNullOrEmpty(Name)
                || string.IsNullOrEmpty(Slug)
                || !slugRegex.IsMatch(Slug)
                || !Columns.Any()
                || !CharacterIds.Any();
            if (invalid)
            {
                throw new InvalidPublicViewException();
            }

        }
    }
}
