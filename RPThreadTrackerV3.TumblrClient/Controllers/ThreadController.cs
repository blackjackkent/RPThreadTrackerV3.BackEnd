namespace RPThreadTrackerV3.TumblrClient.Controllers
{
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Mvc;

	[Route("api/[controller]")]
	public class ThreadController : Controller
    {
	    [HttpGet]
	    public Task<IActionResult> Get()
	    {
		    return new string[] { "value1", "value2" };
	    }
	}
}
