namespace RPThreadTrackerV3.BackEnd.Test.Infrastructure.Mappers
{
    using AutoMapper;
    using BackEnd.Infrastructure.Mappers.Resolvers;
    using BackEnd.Models.Configuration;
    using BackEnd.Models.DomainModels.PublicViews;
    using BackEnd.Models.ViewModels.PublicViews;
    using FluentAssertions;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    [Trait("Class", "PublicViewUrlResolver")]
    public class PublicViewUrlResolverTests
    {
        private readonly PublicViewUrlResolver _resolver;
        private readonly ResolutionContext _mockContext;
        private readonly AppSettings _mockConfig;

        public PublicViewUrlResolverTests()
        {
            var mockOptions = new Mock<IMappingOperationOptions<PublicView, PublicViewDto>>();
            var mockMapper = new Mock<IRuntimeMapper>();
            _mockContext = new ResolutionContext(mockOptions.Object, mockMapper.Object);
            _mockConfig = new AppSettings();
            var configWrapper = new Mock<IOptions<AppSettings>>();
            configWrapper.SetupGet(c => c.Value).Returns(_mockConfig);
            _resolver = new PublicViewUrlResolver(configWrapper.Object);
        }

        public class Resolve : PublicViewUrlResolverTests
        {
            [Fact]
            public void ReturnsUrlUsingSlugAndBaseUrl()
            {
                // Arrange
                _mockConfig.Cors = new CorsAppSettings { CorsUrl = "http://www.test.com,http://www.test2.com" };
                var view = new PublicView
                {
                    Slug = "my-public-view"
                };

                // Act
                var result = _resolver.Resolve(view, new PublicViewDto(), "Url", _mockContext);

                // Assert
                result.Should().Be("http://www.test.com/public/my-public-view");
            }
        }
    }
}
