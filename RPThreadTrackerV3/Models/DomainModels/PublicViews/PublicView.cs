// <copyright file="PublicView.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Models.DomainModels.PublicViews
{
    using System.Collections.Generic;

    /// <summary>
    /// Domain-layer representation of a user's settings for a particular public thread view.
    /// </summary>
    public class PublicView
    {
        /// <summary>
        /// Gets the unique identifier of the public view.
        /// </summary>
        /// <value>
        /// The unique identifier of the public view.
        /// </value>
        public string Id { get; }

        /// <summary>
        /// Gets the name of the public view.
        /// </summary>
        /// <value>
        /// The name of the public view.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the unique URL slug of the public view.
        /// </summary>
        /// <value>
        /// The URL slug.
        /// </value>
        public string Slug { get; }

        /// <summary>
        /// Gets the unique identifier of the user who created this public view.
        /// </summary>
        /// <value>
        /// The unique identifier of the user who created this public view.
        /// </value>
        public string UserId { get; }

        /// <summary>
        /// Gets a list of strings representing which data columns should be displayed in this public view.
        /// </summary>
        /// <value>
        /// A list of strings representing which data columns should be displayed in this public view.
        /// </value>
        public List<string> Columns { get; }

        /// <summary>
        /// Gets a string representing which data column the public view should be sorted by.
        /// </summary>
        /// <value>
        /// A string representing which data column the public view should be sorted by.
        /// </value>
        public string SortKey { get; }

        /// <summary>
        /// Gets a value indicating whether this public view should sort in descending order.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this public view should sort in descending order; otherwise, <c>false</c>.
        /// </value>
        public bool SortDescending { get; }

        /// <summary>
        /// Gets an object representing this public view's turn-based filter settings.
        /// </summary>
        /// <value>
        /// An object representing this public view's turn-based filter settings.
        /// </value>
        public PublicTurnFilter TurnFilter { get; }

        /// <summary>
        /// Gets a list of IDs representing which characters' threads should be displayed in this public view.
        /// </summary>
        /// <value>
        /// A list of IDs representing which characters' threads should be displayed in this public view.
        /// </value>
        public List<int> CharacterIds { get; }

        /// <summary>
        /// Gets a list of tag strings which should be used to filter which threads should be displayed in this public view.
        /// </summary>
        /// <value>
        /// A list of tag strings which should be used to filter which threads should be displayed in this public view.
        /// </value>
        public List<string> Tags { get; }
    }
}
