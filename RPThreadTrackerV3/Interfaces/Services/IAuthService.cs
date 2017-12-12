namespace RPThreadTrackerV3.Interfaces.Services
{
	using System.Collections.Generic;
	using System.IdentityModel.Tokens.Jwt;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

	public interface IAuthService
    {
	    Task<IdentityUser> GetUserByUsernameOrEmail(string modelUsername, UserManager<IdentityUser> userManager);
	    Task<JwtSecurityToken> GenerateJwt(IdentityUser user, string key, string issuer, string audience, UserManager<IdentityUser> userManager);

    }
}
