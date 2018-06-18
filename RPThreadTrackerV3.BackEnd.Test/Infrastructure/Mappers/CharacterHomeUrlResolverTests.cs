namespace RPThreadTrackerV3.BackEnd.Test.Infrastructure.Mappers
{
    using AutoMapper;
    using BackEnd.Infrastructure.Enums;
    using BackEnd.Infrastructure.Mappers.Resolvers;
    using BackEnd.Models.DomainModels;
    using BackEnd.Models.ViewModels;
    using FluentAssertions;
    using Moq;
    using Xunit;

    [Trait("Class", "CharacterHomeUrlResolver")]
    public class CharacterHomeUrlResolverTests
    {
        private readonly CharacterHomeUrlResolver _resolver;
        private readonly ResolutionContext _mockContext;

        public CharacterHomeUrlResolverTests()
        {
            var mockOptions = new Mock<IMappingOperationOptions<Character, CharacterDto>>();
            var mockMapper = new Mock<IRuntimeMapper>();
            _mockContext = new ResolutionContext(mockOptions.Object, mockMapper.Object);
            _resolver = new CharacterHomeUrlResolver();
        }

        public class Resolve : CharacterHomeUrlResolverTests
        {
            [Fact]
            public void ReturnsTumblrUrlWhenPlatformIsTumblr()
            {
                // Arrange
                var character = new Character
                {
                    CharacterId = 1,
                    CharacterName = "Test Character",
                    PlatformId = Platform.Tumblr,
                    UrlIdentifier = "my-test-character"
                };

                // Act
                var result = _resolver.Resolve(character, new CharacterDto(), "HomeUrl", _mockContext);

                // Assert
                result.Should().Be("http://my-test-character.tumblr.com");
            }

            [Fact]
            public void ReturnsNullWhenPlatformIdDoesNotExist()
            {

                // Arrange
                var character = new Character
                {
                    CharacterId = 1,
                    CharacterName = "Test Character",
                    PlatformId = (Platform)2,
                    UrlIdentifier = "my-test-character"
                };

                // Act
                var result = _resolver.Resolve(character, new CharacterDto(), "HomeUrl", _mockContext);

                // Assert
                result.Should().BeNull();
            }
        }
    }
}
