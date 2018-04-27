using RPThreadTrackerV3.Interfaces.Data;

namespace RPThreadTrackerV3.Infrastructure.Data.Entities
{
	using System;
	using Microsoft.AspNetCore.Identity;

	public class RefreshToken : IEntity
	{
	    public string Id { get; set; }
	    public DateTime IssuedUtc { get; set; }
	    public DateTime ExpiresUtc { get; set; }
	    public string Token { get; set; }

	    public string UserId { get; set; }
	    public IdentityUser User { get; set; }
	}
}
