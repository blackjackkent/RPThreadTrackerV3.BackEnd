// <copyright file="CharacterServiceTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using AutoMapper;
    using BackEnd.Infrastructure.Data.Entities;
    using BackEnd.Infrastructure.Exceptions.Characters;
    using BackEnd.Infrastructure.Services;
    using FluentAssertions;
    using Interfaces.Data;
    using Moq;
    using Xunit;

    [Trait("Class", "CharacterService")]
    public class CharacterServiceTests
    {
        private readonly Mock<IRepository<Character>> _mockCharacterRepository;
        private readonly CharacterService _characterService;
        private Mock<IMapper> _mockMapper;

        public CharacterServiceTests()
        {
            _mockCharacterRepository = new Mock<IRepository<Character>>();
            _mockMapper = new Mock<IMapper>();
            _mockMapper.Setup(m => m.Map<BackEnd.Models.DomainModels.Character>(It.IsAny<Character>()))
                .Returns((Character entity) => new BackEnd.Models.DomainModels.Character
                {
                    UserId = entity.UserId,
                    CharacterId = entity.CharacterId,
                    CharacterName = entity.CharacterName,
                    IsOnHiatus = entity.IsOnHiatus
                });
            _mockMapper.Setup(m => m.Map<Character>(It.IsAny<BackEnd.Models.DomainModels.Character>()))
                .Returns((BackEnd.Models.DomainModels.Character model) => new Character
                {
                    UserId = model.UserId,
                    CharacterId = model.CharacterId,
                    CharacterName = model.CharacterName,
                    IsOnHiatus = model.IsOnHiatus
                });
            _characterService = new CharacterService();
        }

        public class AssertUserOwnsCharacter : CharacterServiceTests
        {
            [Fact]
            public void ThrowsExceptionIfCharacterDoesNotExistForUser()
            {
                // Arrange
                var character = new Character
                {
                    UserId = "12345",
                    CharacterId = 13579
                };
                _mockCharacterRepository.Setup(r => r.ExistsWhere(It.Is<Expression<Func<Character, bool>>>(y => y.Compile()(character)))).Returns(true);
                _mockCharacterRepository.Setup(r => r.ExistsWhere(It.Is<Expression<Func<Character, bool>>>(y => !y.Compile()(character)))).Returns(false);

                // Act/Assert
                Assert.Throws<CharacterNotFoundException>(() => _characterService.AssertUserOwnsCharacter(13579, "98765", _mockCharacterRepository.Object));
            }

            [Fact]
            public void ThrowsNoExceptionIfCharacterExistsForUser()
            {
                // Arrange
                var character = new Character
                {
                    UserId = "12345",
                    CharacterId = 13579
                };
                _mockCharacterRepository.Setup(r => r.ExistsWhere(It.Is<Expression<Func<Character, bool>>>(y => y.Compile()(character)))).Returns(true);
                _mockCharacterRepository.Setup(r => r.ExistsWhere(It.Is<Expression<Func<Character, bool>>>(y => !y.Compile()(character)))).Returns(false);

                // Act
                _characterService.AssertUserOwnsCharacter(13579, "12345", _mockCharacterRepository.Object);

                // Assert
                _mockCharacterRepository.Verify(r => r.ExistsWhere(It.Is<Expression<Func<Character, bool>>>(y => y.Compile()(character))), Times.Once);
            }
        }

        public class GetCharacters : CharacterServiceTests
        {
            [Fact]
            public void ReturnsAllCharactersIfIncludeHiatusedIsTrue()
            {
                // Arrange
                var character1 = new Character
                {
                    UserId = "12345",
                    CharacterId = 13579,
                    IsOnHiatus = true
                };
                var character2 = new Character
                {
                    UserId = "12345",
                    CharacterId = 97531,
                    IsOnHiatus = false
                };
                var characterList = new List<Character> { character1, character2 };
                _mockCharacterRepository.Setup(r => r.GetWhere(It.Is<Expression<Func<Character, bool>>>(y => y.Compile()(character1) && y.Compile()(character2)), It.IsAny<List<string>>())).Returns(characterList);

                // Act
                var characters = _characterService.GetCharacters("12345", _mockCharacterRepository.Object, _mockMapper.Object);

                // Assert
                characters.Should().HaveCount(2);
                characters.Should().Contain(c => c.CharacterId == 13579);
                characters.Should().Contain(c => c.CharacterId == 97531);
                characters.Should().Contain(c => c.IsOnHiatus);
                characters.Should().Contain(c => !c.IsOnHiatus);
            }

            [Fact]
            public void ReturnsActiveCharactersIfIncludeHiatusedIsFalse()
            {
                // Arrange
                var character1 = new Character
                {
                    UserId = "12345",
                    CharacterId = 13579,
                    IsOnHiatus = true
                };
                var character2 = new Character
                {
                    UserId = "12345",
                    CharacterId = 97531,
                    IsOnHiatus = false
                };
                var characterList = new List<Character> { character1, character2 };
                _mockCharacterRepository.Setup(r => r.GetWhere(It.Is<Expression<Func<Character, bool>>>(y => y.Compile()(character1) && y.Compile()(character2)), It.IsAny<List<string>>())).Returns(characterList);

                // Act
                var characters = _characterService.GetCharacters("12345", _mockCharacterRepository.Object, _mockMapper.Object, false);

                // Assert
                characters.Should().HaveCount(1);
                characters.Should().NotContain(c => c.CharacterId == 13579);
                characters.Should().Contain(c => c.CharacterId == 97531);
                characters.Should().NotContain(c => c.IsOnHiatus);
                characters.Should().Contain(c => !c.IsOnHiatus);
            }
        }

        public class CreateCharacter : CharacterServiceTests
        {
            [Fact]
            public void InsertsNewCharacterInRepository()
            {
                // Arrange
                var character = new BackEnd.Models.DomainModels.Character
                {
                    UserId = "12345",
                    CharacterName = "Test Character"
                };
                _mockCharacterRepository.Setup(r => r.Create(It.IsAny<Character>())).Returns((Character entity) => entity);

                // Act
                var result = _characterService.CreateCharacter(character, _mockCharacterRepository.Object, _mockMapper.Object);

                // Assert
                _mockCharacterRepository.Verify(r => r.Create(It.Is<Character>(c => c.CharacterName == "Test Character" && c.UserId == "12345")), Times.Once);
                result.UserId.Should().Be("12345");
                result.CharacterName.Should().Be("Test Character");
            }
        }

        public class UpdateCharacter : CharacterServiceTests
        {
            [Fact]
            public void UpdatesCharacterInRepository()
            {
                // Arrange
                var character = new BackEnd.Models.DomainModels.Character
                {
                    CharacterId = 13579,
                    UserId = "12345",
                    CharacterName = "Test Character"
                };
                _mockCharacterRepository.Setup(r => r.Update("13579", It.IsAny<Character>())).Returns((string characterId, Character entity) => entity);

                // Act
                var result = _characterService.UpdateCharacter(character, _mockCharacterRepository.Object, _mockMapper.Object);

                // Assert
                _mockCharacterRepository.Verify(r => r.Update("13579", It.Is<Character>(c => c.CharacterId == 13579 && c.CharacterName == "Test Character" && c.UserId == "12345")), Times.Once);
                result.UserId.Should().Be("12345");
                result.CharacterName.Should().Be("Test Character");
                result.CharacterId.Should().Be(13579);
            }
        }

        public class DeleteCharacter : CharacterServiceTests
        {
            [Fact]
            public void ThrowsExceptionIfCharacterDoesNotExist()
            {
                // Arrange
                _mockCharacterRepository.Setup(r => r.GetWhere(It.IsAny<Expression<Func<Character, bool>>>(), It.IsAny<List<string>>())).Returns(new List<Character>());

                // Act/Assert
                Assert.Throws<CharacterNotFoundException>(() => _characterService.DeleteCharacter(13579, _mockCharacterRepository.Object));
            }

            [Fact]
            public void DeletesCharacterIfExists()
            {
                // Arrange
                var character = new Character
                {
                    CharacterId = 13579,
                    CharacterName = "Test Character"
                };
                _mockCharacterRepository.Setup(r => r.GetWhere(It.Is<Expression<Func<Character, bool>>>(y => y.Compile()(character)), It.IsAny<List<string>>())).Returns(new List<Character> { character });

                // Act
                _characterService.DeleteCharacter(13579, _mockCharacterRepository.Object);

                // Assert
                _mockCharacterRepository.Verify(r => r.Delete(character), Times.Once);
            }
        }
    }
}
