namespace RPThreadTrackerV3.Infrastructure.Services
{
	using System;
	using System.Collections.Generic;
	using System.IdentityModel.Tokens.Jwt;
	using System.Linq;
	using System.Security.Claims;
	using System.Text;
	using System.Threading.Tasks;
	using Interfaces.Services;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.IdentityModel.Tokens;

	public class AuthService : IAuthService
    {
	    public async Task<IdentityUser> GetUserByUsernameOrEmail(string usernameOrEmail, UserManager<IdentityUser> userManager)
	    {
			var user = await userManager.FindByNameAsync(usernameOrEmail) ?? await userManager.FindByEmailAsync(usernameOrEmail);
		    return user;
	    }

	    public async Task<JwtSecurityToken> GenerateJwt(IdentityUser user, string key, string issuer, string audience, UserManager<IdentityUser> userManager)
	    {
		    var claims = await GetUserClaims(user, userManager);
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
		    var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
		    var token = new JwtSecurityToken(
			    issuer,
			    audience,
			    claims,
			    expires: DateTime.UtcNow.AddMinutes(15),
			    signingCredentials: creds);
		    return token;
	    }

	    private static async Task<IEnumerable<Claim>> GetUserClaims(IdentityUser user, UserManager<IdentityUser> userManager)
	    {
			var userClaims = await userManager.GetClaimsAsync(user);
		    var claims = new[]
		    {
			    new Claim(ClaimTypes.Name, user.UserName),
			    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
			    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			    new Claim(JwtRegisteredClaimNames.Email, user.Email),
		    }.Union(userClaims);
		    return claims;
	    }
    }
}
