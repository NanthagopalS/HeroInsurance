using Insurance.Core.Features.ManualPolicy.Query;
using Insurance.Core.Features.ManualPolicy.Query.GetManualPolicyNature;
using Insurance.Domain.ManualPolicy;
using System.Data;


namespace Insurance.Core.Contracts.Persistence
{
    public interface IManualPolicyRepository
    {
        Task<ManualPolicyReponseModel> DumpPolicyExcelRecordsToDatabase(DataTable reqModel, CancellationToken cancellationToken);
        Task<EmailManualPolicyValidationResponce> GetManualPolicyValidationDetails(CancellationToken cancellationToken);
        Task<GetManualPolicyListModel> GetManualPolicyList(GetManualPolicyListQuery request, CancellationToken cancellationToken);
		Task<IEnumerable<GetManualPolicyNatureResponseModel>> GetManualPolicyNatureList(GetManualPolicyNatureQuery request, CancellationToken cancellationToken);
	}
}
