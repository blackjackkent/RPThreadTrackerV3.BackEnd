namespace RPThreadTrackerV3.Infrastructure.Mappers.Resolvers
{
    using AutoMapper;
    using Microsoft.Extensions.Configuration;
    using Models.DomainModels.Public;
    using Models.ViewModels.Public;

    public class PublicViewUrlResolver : IValueResolver<PublicView, PublicViewDto, string>
    {
        private readonly IConfiguration _config;

        public PublicViewUrlResolver(IConfiguration config)
        {
            _config = config;
        }

        public string Resolve(PublicView source, PublicViewDto destination, string destMember, ResolutionContext context)
        {
            var baseUrl = _config["CorsUrl"];
            return baseUrl + "/public/" + source.Slug;
        }
    }
}
