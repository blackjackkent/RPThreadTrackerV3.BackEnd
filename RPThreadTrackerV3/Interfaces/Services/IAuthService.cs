namespace RPThreadTrackerV3.Interfaces.Services
{
	using System.Collections.Generic;
	using System.IdentityModel.Tokens.Jwt;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using AutoMapper;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Models.DomainModels;

	public interface IAuthService
    {
	    Task<IdentityUser> GetUserByUsernameOrEmail(string modelUsername, UserManager<IdentityUser> userManager);
	    Task<JwtSecurityToken> GenerateJwt(IdentityUser user, string key, string issuer, string audience, UserManager<IdentityUser> userManager);
	    Task<User> GetCurrentUser(ClaimsPrincipal claimsUser, UserManager<IdentityUser> userManager, IMapper mapper);
    }
}
