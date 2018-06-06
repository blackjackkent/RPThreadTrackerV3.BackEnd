// <copyright file="ICharacterService.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Interfaces.Services
{
    using System.Collections.Generic;
    using AutoMapper;
    using Data;
    using Models.DomainModels;
    using Entities = Infrastructure.Data.Entities;

    /// <summary>
    /// Service for data manipulation relating to characters.
    /// </summary>
    public interface ICharacterService
    {
        /// <summary>
        /// Throws an exception if the given user does not own the given character.
        /// </summary>
        /// <param name="characterId">The unique ID of the character.</param>
        /// <param name="userId">The unique ID of the user.</param>
        /// <param name="characterRepository">The character repository.</param>
        void AssertUserOwnsCharacter(int characterId, string userId, IRepository<Entities.Character> characterRepository);

        /// <summary>
        /// Gets all characters associated with a given user.
        /// </summary>
        /// <param name="userId">The unique ID of the user.</param>
        /// <param name="characterRepository">The character repository.</param>
        /// <param name="mapper">The application's object mapper.</param>
        /// <param name="includeHiatused">If set to <c>true</c>, includes characters who are marked as on hiatus.</param>
        /// <returns>List of <see cref="Character"/> objects belonging to the given user.</returns>
        IEnumerable<Character> GetCharacters(string userId, IRepository<Entities.Character> characterRepository, IMapper mapper, bool includeHiatused = true);

        /// <summary>
        /// Creates the passed character.
        /// </summary>
        /// <param name="model">The model containing character information.</param>
        /// <param name="characterRepository">The character repository.</param>
        /// <param name="mapper">The application's object mapper.</param>
        /// <returns>The newly created character object.</returns>
        Character CreateCharacter(Character model, IRepository<Entities.Character> characterRepository, IMapper mapper);

        /// <summary>
        /// Updates the passed character.
        /// </summary>
        /// <param name="model">The model containing character information.</param>
        /// <param name="characterRepository">The character repository.</param>
        /// <param name="mapper">The application's object mapper.</param>
        /// <returns>The updated character object</returns>
        Character UpdateCharacter(Character model, IRepository<Entities.Character> characterRepository, IMapper mapper);

        /// <summary>
        /// Deletes the character with the passed ID.
        /// </summary>
        /// <param name="characterId">The unique ID of the character to delete.</param>
        /// <param name="characterRepository">The character repository.</param>
        void DeleteCharacter(int characterId, IRepository<Entities.Character> characterRepository);
    }
}
