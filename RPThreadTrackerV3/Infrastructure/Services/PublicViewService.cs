using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RPThreadTrackerV3.Infrastructure.Exceptions;
using RPThreadTrackerV3.Interfaces.Data;
using RPThreadTrackerV3.Interfaces.Services;
using RPThreadTrackerV3.Models.DomainModels.Public;
using Documents = RPThreadTrackerV3.Infrastructure.Data.Documents;

namespace RPThreadTrackerV3.Infrastructure.Services
{
    public class PublicViewService : IPublicViewService
    {
        public async Task<IEnumerable<PublicView>> GetPublicViews(string userId,
            IDocumentRepository<Documents.PublicView> publicViewRepository, IMapper mapper)
        {
            var documents = await publicViewRepository.GetItemsAsync(v => v.UserId == userId);
            return documents.Select(mapper.Map<PublicView>).ToList();
        }

        public async Task<PublicView> CreatePublicView(PublicView model, string userId,
            IDocumentRepository<Documents.PublicView> publicViewRepository, IMapper mapper)
        {
            var document = mapper.Map<Documents.PublicView>(model);
            var existingDocuments = await publicViewRepository.GetItemsAsync(v => v.Slug == model.Slug);
            if (existingDocuments.Any())
            {
                throw new PublicViewSlugExistsException();
            }
            var createdDocument = await publicViewRepository.CreateItemAsync(document);
            return mapper.Map<PublicView>(createdDocument);
        }

        public async Task AssertUserOwnsPublicView(string publicViewId, string userId,
            IDocumentRepository<Documents.PublicView> publicViewRepository)
        {
            var view = await publicViewRepository.GetItemAsync(publicViewId);
            if (view == null || view.UserId != userId)
            {
                throw new PublicViewNotFoundException();
            }
        }

        public async Task<PublicView> UpdatePublicView(PublicView model, string userId, IDocumentRepository<Documents.PublicView> publicViewRepository, IMapper mapper)
        {
            var entity = mapper.Map<Documents.PublicView>(model);
            var existingDocuments = await publicViewRepository.GetItemsAsync(v => v.Slug == model.Slug);
            var existingDocument = existingDocuments.FirstOrDefault();
            if (existingDocument != null && existingDocument.Id != model.Id)
            {
                throw new PublicViewSlugExistsException();
            }
            var result = await publicViewRepository.UpdateItemAsync(model.Id, entity);
            return mapper.Map<PublicView>(result);
        }

        public async Task DeletePublicView(string publicViewId, IDocumentRepository<Documents.PublicView> publicViewRepository)
        {
            await publicViewRepository.DeleteItemAsync(publicViewId);
        }
    }
}
