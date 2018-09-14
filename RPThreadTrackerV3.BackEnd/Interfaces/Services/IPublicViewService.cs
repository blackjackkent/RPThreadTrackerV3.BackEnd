// <copyright file="IPublicViewService.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Interfaces.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Data;
    using Models.DomainModels;
    using Models.DomainModels.PublicViews;
    using Models.ViewModels.PublicViews;
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
        /// The task result contains the updated public view object.
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

        /// <summary>
        /// Builds a representation of a public view from information about a legacy view.
        /// </summary>
        /// <param name="legacyDto">The legacy view information.</param>
        /// <param name="characters">The belonging to the user who owns the view.</param>
        /// <returns>Representation of a public view equivalent to the passed legacy one.</returns>
        [Obsolete("No longer relevant after removal of legacy views.")]
        PublicView GetViewFromLegacyDto(LegacyPublicViewDto legacyDto, IEnumerable<Character> characters);

        /// <summary>
        /// Throws an exception if the given slug is invalid for a new or edited public view. A slug
        /// is invalid if it belongs to an existing view (that does not match <c>viewId</c>),
        /// if it is a reserved slug, or if it contains invalid characters.
        /// </summary>
        /// <param name="slug">The slug to be verified.</param>
        /// <param name="viewId">The unique identifier of a view being edited, if applicable.</param>
        /// <param name="publicViewRepository">The public view repository.</param>
        Task AssertSlugIsValid(string slug, string viewId, IDocumentRepository<Documents.PublicView> publicViewRepository);
    }
}
