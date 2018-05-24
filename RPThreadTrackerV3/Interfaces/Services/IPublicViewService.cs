// <copyright file="IPublicViewService.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Interfaces.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Data;
    using Models.DomainModels.PublicViews;
    using Documents = Infrastructure.Data.Documents;

    /// <summary>
    /// Service for data manipulation relating to public views.
    /// </summary>
    public interface IPublicViewService
    {
        /// <summary>
        /// Gets the public views belonging to the passed user.
        /// </summary>
        /// <param name="userId">The unique ID of the user.</param>
        /// <param name="publicViewRepository">The public view repository.</param>
        /// <param name="mapper">The application object mapper.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the public views belonging to the user.
        /// </returns>
        Task<IEnumerable<PublicView>> GetPublicViews(string userId, IDocumentRepository<Documents.PublicView> publicViewRepository, IMapper mapper);

        /// <summary>
        /// Creates the passed public view.
        /// </summary>
        /// <param name="model">The model containing public view information.</param>
        /// <param name="publicViewRepository">The public view repository.</param>
        /// <param name="mapper">The application's object mapper.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the newly created public view object
        /// </returns>
        Task<PublicView> CreatePublicView(PublicView model, IDocumentRepository<Documents.PublicView> publicViewRepository, IMapper mapper);

        /// <summary>
        /// Throws an exception if the given user does not own the given public view.
        /// </summary>
        /// <param name="publicViewId">The unique ID of the public view.</param>
        /// <param name="userId">The unique ID of the user.</param>
        /// <param name="publicViewRepository">The public view repository.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task AssertUserOwnsPublicView(string publicViewId, string userId, IDocumentRepository<Documents.PublicView> publicViewRepository);

        /// <summary>
        /// Updates the passed public view.
        /// </summary>
        /// <param name="model">The model containing public view information.</param>
        /// <param name="publicViewRepository">The public view repository.</param>
        /// <param name="mapper">The application's object mapper.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the updated public view object
        /// </returns>
        Task<PublicView> UpdatePublicView(PublicView model, IDocumentRepository<Documents.PublicView> publicViewRepository, IMapper mapper);

        /// <summary>
        /// Deletes the public view with the passed ID.
        /// </summary>
        /// <param name="publicViewId">The unique ID of the public view to delete.</param>
        /// <param name="publicViewRepository">The public view repository.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task DeletePublicView(string publicViewId, IDocumentRepository<Documents.PublicView> publicViewRepository);

        /// <summary>
        /// Gets the public view with the passed URL slug.
        /// </summary>
        /// <param name="slug">The unique URL slug of the view to be retrieved.</param>
        /// <param name="publicViewRepository">The public view repository.</param>
        /// <param name="mapper">The application object mapper.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the requested public view.
        /// </returns>
        Task<PublicView> GetViewBySlug(string slug, IDocumentRepository<Documents.PublicView> publicViewRepository, IMapper mapper);
    }
}
