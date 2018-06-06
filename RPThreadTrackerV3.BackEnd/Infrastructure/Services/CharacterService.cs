// <copyright file="CharacterService.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Services
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using AutoMapper;
    using Exceptions.Characters;
    using Interfaces.Data;
    using Interfaces.Services;
    using Models.DomainModels;
    using Entities = Data.Entities;

    /// <inheritdoc />
    public class CharacterService : ICharacterService
    {
        /// <inheritdoc />
        /// <exception cref="CharacterNotFoundException">Thrown if the given character
        /// cannot be found or is not associated with the given user.</exception>
        public void AssertUserOwnsCharacter(int characterId, string userId, IRepository<Entities.Character> characterRepository)
	    {
		    var characterExistsForUser =
			    characterRepository.ExistsWhere(c => c.UserId == userId && c.CharacterId == characterId);
		    if (!characterExistsForUser)
		    {
			    throw new CharacterNotFoundException();
		    }
		}

        /// <inheritdoc />
        public IEnumerable<Character> GetCharacters(string userId, IRepository<Entities.Character> characterRepository, IMapper mapper, bool includeHiatused = true)
	    {
		    var entities = characterRepository.GetWhere(c => c.UserId == userId).ToList();
		    return entities.Select(mapper.Map<Character>).ToList();
	    }

        /// <inheritdoc />
        public Character CreateCharacter(Character model, IRepository<Entities.Character> characterRepository, IMapper mapper)
        {
			var entity = mapper.Map<Entities.Character>(model);
			var createdEntity = characterRepository.Create(entity);
			return mapper.Map<Character>(createdEntity);
		}

        /// <inheritdoc />
        public Character UpdateCharacter(Character model, IRepository<Entities.Character> characterRepository, IMapper mapper)
        {
			var entity = mapper.Map<Entities.Character>(model);
			var result = characterRepository.Update(model.CharacterId.ToString(CultureInfo.CurrentCulture), entity);
			return mapper.Map<Character>(result);
		}

        /// <exception cref="CharacterNotFoundException">Thrown if a character with the given ID could not be found.</exception>
        /// <inheritdoc />
        public void DeleteCharacter(int characterId, IRepository<Entities.Character> characterRepository)
		{
			var entity = characterRepository.GetWhere(t => t.CharacterId == characterId).FirstOrDefault();
			if (entity == null)
			{
				throw new CharacterNotFoundException();
			}
			characterRepository.Delete(entity);
		}
    }
}
