// <copyright file="PublicViewManagementController.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure.Exceptions.PublicViews;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.ViewModels.PublicViews;
    using RPThreadTrackerV3.Infrastructure.Data.Documents;
    using RPThreadTrackerV3.Infrastructure.Exceptions;
    using RPThreadTrackerV3.Interfaces.Data;
    using RPThreadTrackerV3.Interfaces.Services;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class PublicViewManagementController : BaseController
    {
        private readonly ILogger<PublicViewManagementController> _logger;
        private readonly IMapper _mapper;
        private readonly IPublicViewService _publicViewService;
        private readonly IDocumentRepository<PublicView> _publicViewRepository;

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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PublicViewDto model)
        {
            try
            {
                model.AssertIsValid();
                model.UserId = UserId;
                var publicView = _mapper.Map<Models.DomainModels.PublicViews.PublicView>(model);
                var createdView =
                await _publicViewService.CreatePublicView(publicView, UserId, _publicViewRepository, _mapper);
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
                var updatedCharacter = await _publicViewService.UpdatePublicView(model, UserId, _publicViewRepository, _mapper);
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
