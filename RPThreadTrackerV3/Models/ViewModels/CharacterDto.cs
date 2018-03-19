namespace RPThreadTrackerV3.Models.ViewModels
{
	using System;
	using System.Text.RegularExpressions;
	using Infrastructure.Enums;
	using Infrastructure.Exceptions;

	public class CharacterDto
	{
		public int CharacterId { get; set; }
		public string UserId { get; set; }
		public string CharacterName { get; set; }
		public string UrlIdentifier { get; set; }
		public bool IsOnHiatus { get; set; }
		public Platform PlatformId { get; set; }
		public string HomeUrl { get; set; }

		public void AssertIsValid()
		{
			if (!Enum.IsDefined(typeof(Platform), PlatformId))
			{
				throw new InvalidCharacterException();
			}
			if (string.IsNullOrEmpty(UrlIdentifier))
			{
				throw new InvalidCharacterException();
			}
			var regex = new Regex(@"^[A-z\d-]+$");
			if (!regex.IsMatch(UrlIdentifier))
			{
				throw new InvalidCharacterException();
			}
		}
	}
}
