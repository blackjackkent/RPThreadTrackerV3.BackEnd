// <copyright file="PublicTurnFilter.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Data.Documents
{
    using RPThreadTrackerV3.Interfaces.Data;

    public class PublicTurnFilter : IDocument
    {
        public bool IncludeMyTurn { get; set; }
        public bool IncludeTheirTurn { get; set; }
        public bool IncludeQueued { get; set; }
        public bool IncludeArchived { get; set; }
    }
}
