// <copyright file="PublicViewManagementController.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure.Data.Documents;
    using Infrastructure.Exceptions.PublicViews;
    using Interfaces.Data;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.ViewModels.PublicViews;

    /// <summary>
    /// Controller class for behavior related to a user's public views.
    /// </summary>
    /// <seealso cref="BaseController" />
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class PublicViewManagementController : BaseController
    {
        private readonly ILogger<PublicViewManagementController> _logger;
        private readonly IMapper _mapper;
        private readonly IPublicViewService _publicViewService;
        private readonly IDocumentRepository<PublicView> _publicViewRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicViewManagementController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="publicViewService">The public view service.</param>
        /// <param name="publicViewRepository">The public view repository.</param>
        public PublicViewManagementController(
            ILogger<PublicViewManagementController> logger,
            IMapper mapper,
            IPublicViewService publicViewService,
            IDocumentRepository<PublicView> publicViewRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _publicViewService = publicViewService;
            _publicViewRepository = publicViewRepository;
        }

        /// <summary>
        /// Processes a request for all public views belonging to the logged-in user.
        /// </summary>
        /// <returns>
        /// HTTP response containing the results of the request and, if successful,
        /// a list of <see cref="PublicViewDto" /> objects in the response body.<para />
        /// <list type="table">
        /// <item><term>200 OK</term><description>Response code for successful retrieval of public view information</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item>
        /// </list>
        /// </returns>
        public async Task<IActionResult> Get()
        {
            try
            {
                var characters = await _publicViewService.GetPublicViews(UserId, _publicViewRepository, _mapper);
                var result = characters.Select(_mapper.Map<PublicViewDto>).ToList();
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500, "An unknown error occurred.");
            }
        }

        /// <summary>
        /// Processes a request to create a new public view for the logged-in user.
        /// </summary>
        /// <param name="model">Information about the public view to be created.</param>
        /// <returns>
        /// HTTP response containing the results of the request and, if successful,
        /// the created public view represented as a <see cref="PublicViewDto" /> in the
        /// response body.<para />
        /// <list type="table">
        /// <item><term>200 OK</term><description>Response code for successful retrieval of public view information</description></item>
        /// <item><term>400 Bad Request</term><description>Response code for invalid public view creation request</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PublicViewDto model)
        {
            try
            {
                model.AssertIsValid();
                model.UserId = UserId;
                var publicView = _mapper.Map<Models.DomainModels.PublicViews.PublicView>(model);
                var createdView =
                    await _publicViewService.CreatePublicView(publicView, _publicViewRepository, _mapper);
                return Ok(createdView);
            }
            catch (PublicViewSlugExistsException)
            {
                _logger.LogWarning($"User {UserId} attempted to add public view with existing slug {model.Slug}.");
                return BadRequest("A public view configuration already exists with this slug.");
            }
            catch (InvalidPublicViewException)
            {
                _logger.LogWarning($"User {UserId} attempted to add invalid public view {model}.");
                return BadRequest("The supplied public view configuration is invalid.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error creating public view {model}: {e.Message}", e);
                return StatusCode(500, "An unknown error occurred.");
            }
        }

        /// <summary>
        /// Processes a request to update an existing public view belonging to the logged-in user.
        /// </summary>
        /// <param name="publicViewId">The unique ID of the public view to be updated.</param>
        /// <param name="viewModel">Information about the public view to be updated.</param>
        /// <returns>
        /// HTTP response containing the results of the request and, if successful,
        /// the updated public view represented as a <see cref="PublicViewDto" /> in the
        /// response body.<para />
        /// <list type="table"><item><term>200 OK</term><description>Response code for successful update of public view data</description></item>
        /// <item><term>400 Bad Request</term><description>Response code for invalid public view update request</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
        [HttpPut]
        [Route("{publicViewId}")]
        public async Task<IActionResult> Put(string publicViewId, [FromBody]PublicViewDto viewModel)
        {
            try
            {
                viewModel.AssertIsValid();
                viewModel.UserId = UserId;
                await _publicViewService.AssertUserOwnsPublicView(publicViewId, UserId, _publicViewRepository);
                var model = _mapper.Map<Models.DomainModels.PublicViews.PublicView>(viewModel);
                var updatedCharacter = await _publicViewService.UpdatePublicView(model, _publicViewRepository, _mapper);
                return Ok(updatedCharacter);
            }
            catch (PublicViewSlugExistsException)
            {
                _logger.LogWarning($"User {UserId} attempted to add public view with existing slug {viewModel.Slug}.");
                return BadRequest("A public view configuration already exists with this slug.");
            }
            catch (InvalidPublicViewException)
            {
                _logger.LogWarning($"User {UserId} attempted to update invalid public view {viewModel}.");
                return BadRequest("The supplied view model configuration is invalid.");
            }
            catch (PublicViewNotFoundException)
            {
                _logger.LogWarning($"User {UserId} attempted to update public view {viewModel.Id} illegally.");
                return BadRequest("You do not have permission to update this character.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error updating public view {viewModel}: {e.Message}", e);
                return StatusCode(500, "An unknown error occurred.");
            }
        }

        /// <summary>
        /// Processes a request to delete an existing public view belonging to the logged-in user.
        /// </summary>
        /// <param name="publicViewId">The unique ID of the public view to be deleted.</param>
        /// <returns>
        /// HTTP response containing the results of the request.<para />
        /// <list type="table">
        /// <item><term>200 OK</term><description>Response code for successful deletion of public view</description></item>
        /// <item><term>404 Not Found</term><description>Response code if public view does not exist or does not belong to logged-in user</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
        [HttpDelete]
        [Route("{publicViewId}")]
        public async Task<IActionResult> Delete(string publicViewId)
        {
            try
            {
                await _publicViewService.AssertUserOwnsPublicView(publicViewId, UserId, _publicViewRepository);
                await _publicViewService.DeletePublicView(publicViewId, _publicViewRepository);
                return Ok();
            }
            catch (PublicViewNotFoundException)
            {
                _logger.LogWarning($"User {UserId} attempted to delete public view {publicViewId} illegally.");
                return NotFound("A public view configuration with that ID does not exist.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error deleting public view {publicViewId}: {e.Message}", e);
                return StatusCode(500, "An unknown error occurred.");
            }
        }
    }
}
