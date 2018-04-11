namespace RPThreadTrackerV3.Infrastructure.Mappers
{
    using AutoMapper;
    using Models.DomainModels.Public;
    using Models.ViewModels.Public;

    public class PublicViewMapper : Profile
    {
        public PublicViewMapper()
        {
            CreateMap<PublicView, Data.Documents.PublicView>()
                .ReverseMap();
            CreateMap<PublicView, PublicViewDto>()
                .ReverseMap();
            CreateMap<PublicTurnFilter, Data.Documents.PublicTurnFilter>()
                .ReverseMap();
            CreateMap<PublicTurnFilter, PublicTurnFilterDto>()
                .ReverseMap();
        }
    }
}
