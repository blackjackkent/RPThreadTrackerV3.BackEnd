// <copyright file="PublicTurnFilter.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Data.Documents
{
    using Interfaces.Data;
    using Microsoft.Azure.Documents;

    /// <summary>
    /// Data-layer representation of a public view's turn-based filter settings.
    /// </summary>
    /// <seealso cref="IDocument" />
    public class PublicTurnFilter
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
    }
}
