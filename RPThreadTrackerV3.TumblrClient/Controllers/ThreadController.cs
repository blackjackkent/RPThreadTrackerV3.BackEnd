namespace RPThreadTrackerV3.TumblrClient.Controllers
{
	using System;
	using System.Threading.Tasks;
	using Infrastructure.Exceptions;
	using Infrastructure.Interfaces;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using RPThreadTrackerV3.Controllers;

	[Route("api/[controller]")]
	public class ThreadController : BaseController
    {
	    private readonly ITumblrClient _client;
	    private readonly ILogger<ThreadController> _logger;

	    public ThreadController(ITumblrClient client, ILogger<ThreadController> logger)
	    {
		    _client = client;
		    _logger = logger;
	    }

	    [HttpGet]
	    public async Task<IActionResult> Get(string postId, string blogShortname, string watchedShortname = null)
	    {
		    try
		    {
			    var post = await _client.GetPost(postId, blogShortname);
			    var response = _client.ParsePost(post, blogShortname, watchedShortname);
			    return Ok(response);
		    }
		    catch (InvalidPostRequestException e)
		    {
			    _logger.LogInformation(
				    $"Invalid post information request: Post ID: {postId}, Blog Shortname: {blogShortname}, Watched Shortname: {watchedShortname}");
			    return BadRequest(e.Message);
		    }
		    catch (PostNotFoundException e)
		    {
			    _logger.LogInformation(
				    $"Nonexistent post request: Post ID: {postId}, Blog Shortname: {blogShortname}, Watched Shortname: {watchedShortname}");
			    return NotFound(e.Message);
		    }
		    catch (Exception e)
		    {
			    _logger.LogError(e, $"Unexpected error retrieving post (Post ID: {postId}, Blog Shortname {blogShortname}, Watched Shortname: {watchedShortname})");
			    return StatusCode(500, $"There was an error retrieving your post.");
		    }
	    }
	}
}
