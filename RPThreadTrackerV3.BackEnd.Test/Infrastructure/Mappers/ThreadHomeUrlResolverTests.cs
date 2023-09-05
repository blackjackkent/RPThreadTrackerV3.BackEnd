// <copyright file="ThreadHomeUrlResolverTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

//namespace RPThreadTrackerV3.BackEnd.Test.Infrastructure.Mappers
//{
//    using AutoMapper;
//    using BackEnd.Infrastructure.Enums;
//    using BackEnd.Infrastructure.Mappers.Resolvers;
//    using BackEnd.Models.DomainModels;
//    using BackEnd.Models.ViewModels;
//    using FluentAssertions;
//    using Moq;
//    using Xunit;

//    [Trait("Class", "ThreadHomeUrlResolver")]
//    public class ThreadHomeUrlResolverTests
//    {
//        private readonly ThreadHomeUrlResolver _resolver;
//        private readonly ResolutionContext _mockContext;

//        public ThreadHomeUrlResolverTests()
//        {
//            var mockOptions = new Mock<IMappingOperationOptions<Character, CharacterDto>>();
//            var mockMapper = new Mock<IRuntimeMapper>();
//            _mockContext = new ResolutionContext(mockOptions.Object, mockMapper.Object);
//            _resolver = new ThreadHomeUrlResolver();
//        }

//        public class Resolve : ThreadHomeUrlResolverTests
//        {
//            [Fact]
//            public void ReturnsTumblrUrlWhenPlatformIsTumblr()
//            {
//                // Arrange
//                var character = new Character
//                {
//                    CharacterId = 1,
//                    CharacterName = "Test Character",
//                    PlatformId = Platform.Tumblr,
//                    UrlIdentifier = "my-test-character"
//                };
//                var thread = new Thread
//                {
//                    CharacterId = 1,
//                    Character = character,
//                    PostId = "13579"
//                };

//                // Act
//                var result = _resolver.Resolve(thread, new ThreadDto(), "HomeUrl", _mockContext);

//                // Assert
//                result.Should().Be("http://my-test-character.tumblr.com/post/13579");
//            }

//            [Fact]
//            public void ReturnsNullWhenPlatformIdDoesNotExist()
//            {
//                // Arrange
//                var character = new Character
//                {
//                    CharacterId = 1,
//                    CharacterName = "Test Character",
//                    PlatformId = (Platform)2,
//                    UrlIdentifier = "my-test-character"
//                };
//                var thread = new Thread
//                {
//                    CharacterId = 1,
//                    Character = character,
//                    PostId = "13579"
//                };

//                // Act
//                var result = _resolver.Resolve(thread, new ThreadDto(), "HomeUrl", _mockContext);

//                // Assert
//                result.Should().BeNull();
//            }

//            [Fact]
//            public void ReturnsNullWhenCharacterIsNull()
//            {
//                // Arrange
//                var thread = new Thread
//                {
//                    CharacterId = 1,
//                    Character = null,
//                    PostId = "13579"
//                };

//                // Act
//                var result = _resolver.Resolve(thread, new ThreadDto(), "HomeUrl", _mockContext);

//                // Assert
//                result.Should().BeNull();
//            }
//        }
//    }
//}
