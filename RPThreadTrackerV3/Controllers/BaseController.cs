namespace RPThreadTrackerV3.Controllers
{
	using System.Linq;
	using System.Security.Claims;
	using Infrastructure.Providers;
	using Microsoft.AspNetCore.Mvc;

	[ServiceFilter(typeof(GlobalExceptionHandler))]
	public class BaseController : Controller
	{
		protected string UserId => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
	}
}
