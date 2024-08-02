using Admin.Core.Features.Mmv.GetHeroVariantLists;
using Admin.Core.Features.Mmv.ResetMvvMappingForIcVariant;
using Admin.Core.Features.Mmv.UpdateVariantsMapping;
using Admin.Core.Features.Mmv.VariantMappingStatus;
using Admin.Domain.Mmv;
using System.Data;

namespace Admin.Core.Contracts.Persistence
{
    public interface IMmvRepository
    {
        Task<GetHeroVariantListsResponceModel> GetHeroVariantLists(GetHeroVariantListsQuery getHeroVariantListsQuery, CancellationToken cancellationToken);
        Task<UpdateVariantsMappingResponceModel> UpdateVariantsMapping(UpdateVariantsMappingCommand updateVariantsMappingCommand, DataTable updateDataTable, DataTable newRecordDataTable, CancellationToken cancellationToken);
		Task<IEnumerable<GetCustomMmvSearchResponseModel>> GetAllVariantForCustomModel(GetCustomMmvSearchQuery getVariantMappingStatusQuery, CancellationToken cancellationToken);
        Task<bool> ResetMvvMappingForIcVariant(ResetMvvMappingForIcVariantCommand resetMvvMappingForIcVariantCommand, CancellationToken cancellationToken);

    }
}
