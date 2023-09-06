﻿// <copyright file="IDocument.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Interfaces.Data
{
    /// <summary>
    /// Categorization interface for all representations of NoSQL documents.
    /// </summary>
    public interface IDocument
    {
        #pragma warning disable SA1300 // Element must begin with upper-case letter
        string id { get; set;  }
        #pragma warning restore SA1300 // Element must begin with upper-case letter
    }
}
