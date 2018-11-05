// <copyright file="PublicViewManagementController.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Controllers
{
    using System;
    using System.Collections.Generic;
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
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<PublicViewDto>))]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogInformation($"Received request to get public views for user {UserId}");
                var views = await _publicViewService.GetPublicViews(UserId, _publicViewRepository, _mapper);
                var result = views.Select(_mapper.Map<PublicViewDto>).ToList();
                _logger.LogInformation($"Processed request to get public views for user {UserId}");
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
        [ProducesResponseType(200, Type = typeof(PublicViewDto))]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Post([FromBody] PublicViewDto model)
        {
            try
            {
                _logger.LogInformation($"Received request to get create public view for user {UserId}");
                model.AssertIsValid();
                model.UserId = UserId;
                var publicView = _mapper.Map<Models.DomainModels.PublicViews.PublicView>(model);
                var createdView =
                    await _publicViewService.CreatePublicView(publicView, _publicViewRepository, _mapper);
                _logger.LogInformation($"Processed request to get public views for user {UserId}");
                return Ok(_mapper.Map<PublicViewDto>(createdView));
            }
            catch (InvalidPublicViewSlugException)
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
        [ProducesResponseType(200, Type = typeof(PublicViewDto))]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Put(string publicViewId, [FromBody]PublicViewDto viewModel)
        {
            try
            {
                _logger.LogInformation($"Received request to update public view {publicViewId} for user {UserId}");
                viewModel.AssertIsValid();
                viewModel.UserId = UserId;
                await _publicViewService.AssertUserOwnsPublicView(publicViewId, UserId, _publicViewRepository);
                var model = _mapper.Map<Models.DomainModels.PublicViews.PublicView>(viewModel);
                var updatedView = await _publicViewService.UpdatePublicView(model, _publicViewRepository, _mapper);
                _logger.LogInformation($"Processed request to update public view {publicViewId} for user {UserId}");
                return Ok(_mapper.Map<PublicViewDto>(updatedView));
            }
            catch (InvalidPublicViewSlugException)
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
                return BadRequest("You do not have permission to update this view.");
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
        [ProducesResponseType(200)]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete(string publicViewId)
        {
            try
            {
                _logger.LogInformation($"Received request to delete public view {publicViewId} for user {UserId}");
                await _publicViewService.AssertUserOwnsPublicView(publicViewId, UserId, _publicViewRepository);
                await _publicViewService.DeletePublicView(publicViewId, _publicViewRepository);
                _logger.LogInformation($"Processed request to delete public view {publicViewId} for user {UserId}");
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

        /// <summary>
        /// Processes a request to verify whether a public view slug is valid.
        /// </summary>
        /// <param name="slug">The slug to be verified.</param>
        /// <param name="viewId">The unique identifier of a view being edited, if applicable. Defaults to null.</param>
        /// <returns>
        /// HTTP response containing the results of the request.<para />
        /// <list type="table">
        /// <item><term>200 OK</term><description>Response code for successful deletion of public view</description></item>
        /// <item><term>404 Not Found</term><description>Response code if public view does not exist or does not belong to logged-in user</description></item>
        /// <item><term>500 Internal Server Error</term><description>Response code for unexpected errors</description></item></list>
        /// </returns>
        [HttpGet]
        [Route("isvalidslug/{slug}/{viewId?}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> CheckIsValidSlug(string slug, string viewId = null)
        {
            try
            {
                _logger.LogInformation($"Received request to check if {slug} is a valid slug for view {viewId}");
                await _publicViewService.AssertSlugIsValid(slug, viewId, UserId, _publicViewRepository);
                _logger.LogInformation($"Slug {slug} was deemed valid for viewID {viewId} and user {UserId}");
                return Ok();
            }
            catch (InvalidPublicViewSlugException)
            {
                _logger.LogInformation($"Slug {slug} was deemed invalid for viewID {viewId} and user {UserId}.");
                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while verifying validity of slug {slug} for view {viewId} and user {UserId}: {e.Message}");
                return StatusCode(500, "An unknown error occurred.");
            }
        }
    }
}
