using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Data.Resources;
using Dorisoy.Pan.Domain;

namespace Dorisoy.Pan.Repository
{
    public class NLogRespository : GenericRepository<NLog, DocumentContext>,
          INLogRespository
    {
        private readonly IPropertyMappingService _propertyMappingService;
        public NLogRespository(IUnitOfWork<DocumentContext> uow,
            IPropertyMappingService propertyMappingService) : base(uow)
        {
            _propertyMappingService = propertyMappingService;
        }

        public async Task<NLogList> GetNLogsAsync(NLogResource nLogResource)
        {
            var collectionBeforePaging = All;
            collectionBeforePaging =
               collectionBeforePaging.ApplySort(nLogResource.OrderBy,
               _propertyMappingService.GetPropertyMapping<NLogDto, NLog>());

            if (!string.IsNullOrWhiteSpace(nLogResource.Message))
            {
                collectionBeforePaging = collectionBeforePaging
                    .Where(c => EF.Functions.Like(c.Message, $"%{nLogResource.Message.Trim()}%"));
            }

            if (!string.IsNullOrWhiteSpace(nLogResource.Level))
            {
                collectionBeforePaging = collectionBeforePaging
                    .Where(c => c.Level == nLogResource.Level);
            }

            if (!string.IsNullOrWhiteSpace(nLogResource.Source))
            {
                collectionBeforePaging = collectionBeforePaging
                    .Where(c => c.Source == nLogResource.Source);
            }

            var nLogList = new NLogList();
            return await nLogList.Create(
                collectionBeforePaging,
                nLogResource.Skip,
                nLogResource.PageSize
                );
        }
    }
}
