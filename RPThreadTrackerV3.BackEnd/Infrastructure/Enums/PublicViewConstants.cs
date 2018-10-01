﻿// <copyright file="PublicViewConstants.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Enums
{
    using System.Collections.Generic;

    /// <summary>
    /// Static class containing constants relating to public views.
    /// </summary>
    public static class PublicViewConstants
    {
        /// <summary>
        /// Public view slug values which are reserved because of their use in legacy view URLs.
        /// </summary>
        public static List<string> RESERVED_SLUGS = new List<string> { "MYTURN", "YOURTURN", "THEIRTURN", "ARCHIVED", "QUEUED", "LEGACY" };
    }
}