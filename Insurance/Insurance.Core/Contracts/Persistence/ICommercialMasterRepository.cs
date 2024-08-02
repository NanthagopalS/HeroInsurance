using Insurance.Core.Features.CommercialMaster.Query.GetCommercialVehicleOtherDetailsAskOptions;
using Insurance.Domain.CommercialMaster;
using Insurance.Domain.CommercialVehicle;

namespace Insurance.Core.Contracts.Persistence
{
	public interface ICommercialMasterRepository
    {
        Task<CommercialVehicleCategory> GetCommercialCategory(CancellationToken cancellationToken);
        Task<CommercialVehicleAskAdditionalsDetailsModel> GetCommercialVehicleOtherDetailsAskOptions(GetCommercialVehicleOtherDetailsAskOptionsQuery getCommercialVehicleOtherDetailsAskOptionsQuery, CancellationToken cancellationToken);
    }
}
