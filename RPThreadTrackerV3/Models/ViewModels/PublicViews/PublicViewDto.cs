// <copyright file="PublicViewDto.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Models.ViewModels.PublicViews
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Infrastructure.Exceptions.PublicViews;

    /// <summary>
    /// View model representation of a user's settings for a particular public thread view.
    /// </summary>
    public class PublicViewDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the public view.
        /// </summary>
        /// <value>
        /// The unique identifier of the public view.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the public view.
        /// </summary>
        /// <value>
        /// The name of the public view.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the unique URL slug of the public view.
        /// </summary>
        /// <value>
        /// The URL slug.
        /// </value>
        public string Slug { get; set; }

        /// <summary>
        /// Gets or sets the full URL at which this public view can be accessed.
        /// </summary>
        /// <value>
        /// The full URL at which this public view can be accessed.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who created this public view.
        /// </summary>
        /// <value>
        /// The unique identifier of the user who created this public view.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets a list of strings representing which data columns should be displayed in this public view.
        /// </summary>
        /// <value>
        /// A list of strings representing which data columns should be displayed in this public view.
        /// </value>
        public List<string> Columns { get; set; }

        /// <summary>
        /// Gets or sets a string representing which data column the public view should be sorted by.
        /// </summary>
        /// <value>
        /// A string representing which data column the public view should be sorted by.
        /// </value>
        public string SortKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this public view should sort in descending order.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this public view should sort in descending order; otherwise, <c>false</c>.
        /// </value>
        public bool SortDescending { get; set; }

        /// <summary>
        /// Gets or sets an object representing this public view's turn-based filter settings.
        /// </summary>
        /// <value>
        /// An object representing this public view's turn-based filter settings.
        /// </value>
        public PublicTurnFilterDto TurnFilter { get; set; }

        /// <summary>
        /// Gets or sets a list of IDs representing which characters' threads should be displayed in this public view.
        /// </summary>
        /// <value>
        /// A list of IDs representing which characters' threads should be displayed in this public view.
        /// </value>
        public List<int> CharacterIds { get; set; }

        /// <summary>
        /// Gets or sets a list of tag strings which should be used to filter which threads should be displayed in this public view.
        /// </summary>
        /// <value>
        /// A list of tag strings which should be used to filter which threads should be displayed in this public view.
        /// </value>
        public List<string> Tags { get; set; }

        /// <summary>
        /// Throws an exception if the public view model is not valid.
        /// </summary>
        /// <exception cref="InvalidPublicViewException">Thrown if the public view model is not valid.</exception>
        public void AssertIsValid()
        {
            TurnFilter.AssertIsValid();
            var slugRegex = new Regex(@"^[A-Za-z0-9]+(?:-[A-Za-z0-9]+)*$");
            List<string> reservedSlugs = new List<string> { "myturn", "yourturn", "theirturn", "archived", "queued", "legacy" };
            var invalid =
                string.IsNullOrEmpty(Name)
                || string.IsNullOrEmpty(Slug)
                || !slugRegex.IsMatch(Slug)
                || !Columns.Any()
                || !CharacterIds.Any()
                || reservedSlugs.Contains(Slug);
            if (invalid)
            {
                throw new InvalidPublicViewException();
            }
        }
    }
}
