namespace RPThreadTrackerV3.Infrastructure.Mappers
{
    using AutoMapper;
    using Models.DomainModels.Public;
    using Models.ViewModels.Public;
    using Resolvers;

    public class PublicViewMapper : Profile
    {
        public PublicViewMapper()
        {
            CreateMap<PublicView, Data.Documents.PublicView>()
                .ReverseMap();
            CreateMap<PublicView, PublicViewDto>()
                .ForMember(d => d.Url, o => o.ResolveUsing<PublicViewUrlResolver>())
                .ReverseMap();
            CreateMap<PublicTurnFilter, Data.Documents.PublicTurnFilter>()
                .ReverseMap();
            CreateMap<PublicTurnFilter, PublicTurnFilterDto>()
                .ReverseMap();
        }
    }
}
