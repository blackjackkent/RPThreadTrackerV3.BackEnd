// <copyright file="CharacterControllerTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Controllers
{
    using System;
    using System.Collections.Generic;
    using AutoMapper;
    using BackEnd.Controllers;
    using BackEnd.Infrastructure.Enums;
    using BackEnd.Infrastructure.Exceptions.Characters;
    using BackEnd.Models.Configuration;
    using BackEnd.Models.DomainModels;
    using BackEnd.Models.ViewModels;
    using FluentAssertions;
    using Interfaces.Data;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using TestHelpers;
    using Xunit;
    using Entities = BackEnd.Infrastructure.Data.Entities;

    [Trait("Class", "CharacterController")]
    public class CharacterControllerTests : ControllerTests<CharacterController>
    {
        private readonly Mock<ICharacterService> _mockCharacterService;
        private readonly Mock<IRepository<Entities.Character>> _mockCharacterRepository;
        private readonly Mock<IMapper> _mockMapper;

        public CharacterControllerTests()
        {
            var mockLogger = new Mock<ILogger<CharacterController>>();
            _mockCharacterService = new Mock<ICharacterService>();
            _mockCharacterRepository = new Mock<IRepository<Entities.Character>>();
            _mockMapper = new Mock<IMapper>();
            _mockMapper.Setup(m => m.Map<CharacterDto>(It.IsAny<Character>()))
                .Returns((Character model) => new CharacterDto
                {
                    UserId = model.UserId,
                    CharacterId = model.CharacterId,
                    CharacterName = model.CharacterName,
                    IsOnHiatus = model.IsOnHiatus,
                    PlatformId = model.PlatformId,
                    UrlIdentifier = model.UrlIdentifier
                });
            _mockMapper.Setup(m => m.Map<Character>(It.IsAny<CharacterDto>()))
                .Returns((CharacterDto dto) => new Character
                {
                    UserId = dto.UserId,
                    CharacterId = dto.CharacterId,
                    CharacterName = dto.CharacterName,
                    IsOnHiatus = dto.IsOnHiatus,
                    PlatformId = dto.PlatformId,
                    UrlIdentifier = dto.UrlIdentifier
                });
            var mockConfig = new AppSettings();
            var configWrapper = new Mock<IOptions<AppSettings>>();
            configWrapper.SetupGet(c => c.Value).Returns(mockConfig);
            Controller = new CharacterController(mockLogger.Object, _mockMapper.Object, _mockCharacterService.Object, _mockCharacterRepository.Object);
            InitControllerContext();
        }

        public class GetCharacters : CharacterControllerTests
        {
            [Fact]
            public void ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockCharacterService.Setup(s => s.GetCharacters("12345", _mockCharacterRepository.Object, _mockMapper.Object, It.IsAny<bool>()))
                    .Throws<NullReferenceException>();

                // Act
                var result = Controller.Get();

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public void ReturnsOkWithCharacterListWhenRequestSuccessful()
            {
                // Arrange
                var characters = new List<Character>
                {
                    new Character
                    {
                        CharacterId = 12345
                    },
                    new Character
                    {
                        CharacterId = 54321
                    }
                };
                _mockCharacterService.Setup(s => s.GetCharacters("12345", _mockCharacterRepository.Object, _mockMapper.Object, It.IsAny<bool>()))
                    .Returns(characters);

                // Act
                var result = Controller.Get();
                var body = ((OkObjectResult)result).Value as List<CharacterDto>;

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                body.Should().HaveCount(2)
                    .And.Contain(c => c.CharacterId == 12345)
                    .And.Contain(c => c.CharacterId == 54321);
            }
        }

        public class Post : CharacterControllerTests
        {
            private readonly CharacterDto _validRequest;

            public Post()
            {
                _validRequest = new CharacterDto
                {
                    CharacterId = 54321,
                    UserId = "12345",
                    PlatformId = Platform.Tumblr,
                    UrlIdentifier = "my-character"
                };
            }

            [Fact]
            public void ReturnsBadRequestWhenCharacterIsInvalid()
            {
                // Arrange
                var character = new Mock<CharacterDto>();
                character.Setup(c => c.AssertIsValid()).Throws<InvalidCharacterException>();

                // Act
                var result = Controller.Post(character.Object);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                _mockCharacterService.Verify(s => s.CreateCharacter(It.IsAny<Character>(), _mockCharacterRepository.Object, _mockMapper.Object), Times.Never);
            }

            [Fact]
            public void ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockCharacterService.Setup(s => s.CreateCharacter(It.IsAny<Character>(), _mockCharacterRepository.Object, _mockMapper.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = Controller.Post(_validRequest);

                // Assert
                result.Should().BeOfType<ObjectResult>();
                ((ObjectResult)result).StatusCode.Should().Be(500);
            }

            [Fact]
            public void ReturnsOkWhenRequestIsSuccessful()
            {
                // Arrange
                _mockCharacterService.Setup(s =>
                        s.CreateCharacter(It.IsAny<Character>(), _mockCharacterRepository.Object, _mockMapper.Object))
                    .Returns((Character model, IRepository<Entities.Character> repo, IMapper mapper) => model);

                // Act
                var result = Controller.Post(_validRequest);
                var body = ((OkObjectResult)result).Value as CharacterDto;

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                body.CharacterId.Should().Be(54321);
            }
        }

        public class Put : CharacterControllerTests
        {
            private readonly CharacterDto _validRequest;

            public Put()
            {
                _validRequest = new CharacterDto
                {
                    CharacterId = 54321,
                    UserId = "12345",
                    PlatformId = Platform.Tumblr,
                    UrlIdentifier = "my-character"
                };
            }

            [Fact]
            public void ReturnsBadRequestWhenCharacterIsInvalid()
            {
                // Arrange
                var character = new Mock<CharacterDto>();
                character.Setup(c => c.AssertIsValid()).Throws<InvalidCharacterException>();

                // Act
                var result = Controller.Put(54321, character.Object);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                _mockCharacterService.Verify(s => s.UpdateCharacter(It.IsAny<Character>(), _mockCharacterRepository.Object, _mockMapper.Object), Times.Never);
            }

            [Fact]
            public void ReturnsBadRequestWhenCharacterDoesNotExistForUser()
            {
                // Arrange
                _mockCharacterService
                    .Setup(s => s.AssertUserOwnsCharacter(54321, "12345", _mockCharacterRepository.Object))
                    .Throws<CharacterNotFoundException>();

                // Act
                var result = Controller.Put(54321, _validRequest);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                _mockCharacterService.Verify(s => s.UpdateCharacter(It.IsAny<Character>(), _mockCharacterRepository.Object, _mockMapper.Object), Times.Never);
            }

            [Fact]
            public void ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockCharacterService.Setup(s => s.UpdateCharacter(It.IsAny<Character>(), _mockCharacterRepository.Object, _mockMapper.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = Controller.Put(54321, _validRequest);

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public void ReturnsOkWhenRequestIsSuccessful()
            {
                // Arrange
                _mockCharacterService.Setup(s =>
                        s.UpdateCharacter(It.IsAny<Character>(), _mockCharacterRepository.Object, _mockMapper.Object))
                    .Returns((Character model, IRepository<Entities.Character> repo, IMapper mapper) => model);

                // Act
                var result = Controller.Put(54321, _validRequest);
                var body = ((OkObjectResult)result).Value as CharacterDto;

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                body.CharacterId.Should().Be(54321);
            }
        }

        public class Delete : CharacterControllerTests
        {
            [Fact]
            public void ReturnsNotFoundWhenCharacterDoesNotExistForUser()
            {
                // Arrange
                _mockCharacterService
                    .Setup(s => s.AssertUserOwnsCharacter(54321, "12345", _mockCharacterRepository.Object))
                    .Throws<CharacterNotFoundException>();

                // Act
                var result = Controller.Delete(54321);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
                _mockCharacterService.Verify(s => s.DeleteCharacter(It.IsAny<int>(), _mockCharacterRepository.Object), Times.Never);
            }

            [Fact]
            public void ReturnsServerErrorWhenUnexpectedErrorOccurs()
            {
                // Arrange
                _mockCharacterService.Setup(s => s.DeleteCharacter(54321, _mockCharacterRepository.Object))
                    .Throws<NullReferenceException>();

                // Act
                var result = Controller.Delete(54321);

                // Assert
                result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            }

            [Fact]
            public void ReturnsOkWhenRequestIsSuccessful()
            {
                // Act
                var result = Controller.Delete(54321);

                // Assert
                result.Should().BeOfType<OkResult>();
            }
        }
    }
}
