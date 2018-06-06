// <copyright file="CharacterNotFoundException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Exceptions.Characters
{
    using System;

    /// <summary>
    /// The exception that is thrown when there was an error retrieving a character.
    /// </summary>
    /// <seealso cref="Exception" />
    public class CharacterNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterNotFoundException"/> class.
        /// </summary>
        public CharacterNotFoundException()
            : base("The requested character does not exist for the current user.")
        {
        }
    }
}
