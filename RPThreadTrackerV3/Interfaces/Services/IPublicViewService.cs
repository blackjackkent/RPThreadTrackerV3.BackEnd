namespace RPThreadTrackerV3.Interfaces.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Models.DomainModels.PublicViews;
    using RPThreadTrackerV3.Interfaces.Data;
    using Documents = RPThreadTrackerV3.Infrastructure.Data.Documents;

    public interface IPublicViewService
    {
        Task<IEnumerable<PublicView>> GetPublicViews(string userId, IDocumentRepository<Documents.PublicView> publicViewRepository, IMapper mapper);
        Task<PublicView> CreatePublicView(PublicView model, string userId, IDocumentRepository<Documents.PublicView> publicViewRepository, IMapper mapper);
        Task AssertUserOwnsPublicView(string publicViewId, string userId, IDocumentRepository<Documents.PublicView> publicViewRepository);
        Task<PublicView> UpdatePublicView(PublicView model, string userId, IDocumentRepository<Documents.PublicView> publicViewRepository, IMapper mapper);
        Task DeletePublicView(string publicViewId, IDocumentRepository<Documents.PublicView> publicViewRepository);
	    Task<PublicView> GetViewBySlug(string slug, IDocumentRepository<Documents.PublicView> publicViewRepository, IMapper mapper);
    }
}