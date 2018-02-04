namespace RPThreadTrackerV3.Controllers
{
	using Microsoft.AspNetCore.Mvc;
	using TumblrClient.Infrastructure.Providers;

	[ServiceFilter(typeof(GlobalExceptionHandler))]
	public class BaseController : Controller
	{
	}
}

