﻿namespace RPThreadTrackerV3.Models.DomainModels
{
	using Infrastructure.Enums;

	public class Character
    {
	    public int CharacterId { get; set; }
	    public string UserId { get; set; }
		public string CharacterName { get; set; }
	    public string UrlIdentifier { get; set; }
	    public bool IsOnHiatus { get; set; }
		public Platform PlatformId { get; set; }
	}
}