// <copyright file="PublicViewService.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Exceptions.PublicViews;
    using Interfaces.Data;
    using Interfaces.Services;
    using Models.DomainModels.PublicViews;
    using Documents = Data.Documents;

    /// <inheritdoc />
    public class PublicViewService : IPublicViewService
    {
        /// <inheritdoc />
        public async Task<IEnumerable<PublicView>> GetPublicViews(string userId, IDocumentRepository<Documents.PublicView> publicViewRepository, IMapper mapper)
        {
            var documents = await publicViewRepository.GetItemsAsync(v => v.UserId == userId);
            return documents.Select(mapper.Map<PublicView>).ToList();
        }

        /// <inheritdoc />
        /// <exception cref="PublicViewSlugExistsException">Thrown if the user attempts to create a view
        /// with a slug that is already in use.</exception>
        public async Task<PublicView> CreatePublicView(PublicView model, IDocumentRepository<Documents.PublicView> publicViewRepository, IMapper mapper)
        {
            var document = mapper.Map<Documents.PublicView>(model);
            var existingDocuments = await publicViewRepository.GetItemsAsync(v => v.Slug == model.Slug);
            if (existingDocuments.Any())
            {
                throw new PublicViewSlugExistsException();
            }
            var createdDocument = await publicViewRepository.CreateItemAsync(document);
            return mapper.Map<PublicView>(createdDocument);
        }

        /// <inheritdoc />
        /// <exception cref="PublicViewNotFoundException">Thrown if the public view does not exist or
        /// does not belong to the given user.</exception>
        public async Task AssertUserOwnsPublicView(string publicViewId, string userId, IDocumentRepository<Documents.PublicView> publicViewRepository)
        {
            var view = await publicViewRepository.GetItemAsync(publicViewId);
            if (view == null || view.UserId != userId)
            {
                throw new PublicViewNotFoundException();
            }
        }

        /// <inheritdoc />
        /// <exception cref="PublicViewSlugExistsException">Thrown if the user is attempting to update
        /// the public view to a slug already in use by another public view.</exception>
        public async Task<PublicView> UpdatePublicView(PublicView model, IDocumentRepository<Documents.PublicView> publicViewRepository, IMapper mapper)
        {
            var entity = mapper.Map<Documents.PublicView>(model);
            var existingDocuments = await publicViewRepository.GetItemsAsync(v => v.Slug == model.Slug);
            var existingDocument = existingDocuments.FirstOrDefault();
            if (existingDocument != null && existingDocument.Id != model.Id)
            {
                throw new PublicViewSlugExistsException();
            }
            var result = await publicViewRepository.UpdateItemAsync(model.Id, entity);
            return mapper.Map<PublicView>(result);
        }

        /// <inheritdoc />
        public async Task DeletePublicView(string publicViewId, IDocumentRepository<Documents.PublicView> publicViewRepository)
        {
            await publicViewRepository.DeleteItemAsync(publicViewId);
        }

        /// <inheritdoc />
        /// <exception cref="PublicViewNotFoundException">Thrown if a public view with the given slug
        /// does not exist.</exception>
        public async Task<PublicView> GetViewBySlug(string slug, IDocumentRepository<Documents.PublicView> publicViewRepository, IMapper mapper)
	    {
		    var views = await publicViewRepository.GetItemsAsync(v => v.Slug == slug);
		    var view = views.FirstOrDefault();
		    if (view == null)
		    {
			    throw new PublicViewNotFoundException();
		    }
		    return mapper.Map<PublicView>(view);
	    }
    }
}
