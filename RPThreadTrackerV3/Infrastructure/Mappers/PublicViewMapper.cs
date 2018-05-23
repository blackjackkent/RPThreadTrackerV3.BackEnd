namespace RPThreadTrackerV3.Infrastructure.Mappers
{
    using AutoMapper;
    using Models.DomainModels.PublicViews;
    using Models.ViewModels.PublicViews;
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
