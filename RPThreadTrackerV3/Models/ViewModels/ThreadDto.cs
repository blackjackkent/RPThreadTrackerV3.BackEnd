namespace RPThreadTrackerV3.Models.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using Infrastructure.Exceptions;
	using Infrastructure.Exceptions.Thread;

    public class ThreadDto
	{
		public int? ThreadId { get; }
		public int CharacterId { get; }
		public CharacterDto Character { get; }
		public string PostId { get; }
		public string UserTitle { get; }
		public string PartnerUrlIdentifier { get; }
		public bool IsArchived { get; }
		public DateTime? DateMarkedQueued { get; }
		public string ThreadHomeUrl { get; }
		public List<ThreadTagDto> ThreadTags { get; }

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
