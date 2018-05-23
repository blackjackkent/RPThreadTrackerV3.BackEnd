﻿// <copyright file="CharacterNotFoundException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Exceptions.Characters
{
    using System;

    public class CharacterNotFoundException : Exception
    {
		public CharacterNotFoundException()
		    : base("The requested character does not exist for the current user.") { }
    }
}
