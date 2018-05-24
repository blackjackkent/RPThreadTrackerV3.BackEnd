// <copyright file="PublicTurnFilterDto.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Models.ViewModels.PublicViews
{
    using RPThreadTrackerV3.Infrastructure.Exceptions.PublicViews;

    /// <summary>
    /// View model representation of a public view's turn-based filter settings.
    /// </summary>
    public class PublicTurnFilterDto
    {
        /// <summary>
        /// Gets or sets a value indicating whether the public view should include threads for which it is the user's turn.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the public view should include threads for which it is the user's turn; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeMyTurn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the public view should include threads for which it is not the user's turn.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the public view should include threads for which it is not the user's turn; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeTheirTurn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the public view should include threads which are marked queued.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the public view should include threads which are marked queued; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeQueued { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the public view should include threads which are archived.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the public view should include threads which are archived; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeArchived { get; set; }

        /// <summary>
        /// Throws an exception if the public view model is not valid.
        /// </summary>
        /// <exception cref="InvalidPublicViewException">Thrown if the public view model is not valid.</exception>
        public void AssertIsValid()
        {
            if (!IncludeMyTurn && !IncludeTheirTurn && !IncludeQueued && !IncludeArchived)
            {
                throw new InvalidPublicViewException();
            }
        }
    }
}
