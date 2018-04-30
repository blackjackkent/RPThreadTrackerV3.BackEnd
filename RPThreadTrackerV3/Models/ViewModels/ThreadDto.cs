namespace RPThreadTrackerV3.Models.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using Infrastructure.Exceptions;
	using Infrastructure.Exceptions.Thread;

    public class ThreadDto
	{
		public int? ThreadId { get; set; }
		public int CharacterId { get; set; }
		public CharacterDto Character { get; set; }
		public string PostId { get; set; }
		public string UserTitle { get; set; }
		public string PartnerUrlIdentifier { get; set; }
		public bool IsArchived { get; set; }
		public DateTime? DateMarkedQueued { get; set; }
		public string ThreadHomeUrl { get; set; }
		public List<ThreadTagDto> ThreadTags { get; set; }

		public void AssertIsValid()
		{
			if (string.IsNullOrEmpty(UserTitle))
			{
				throw new InvalidThreadException();
			}
			var regex = new Regex(@"^(\d)+$");
			if (!string.IsNullOrEmpty(PostId) && !regex.IsMatch(PostId))
			{
				throw new InvalidThreadException();
			}
		}
	}
}
