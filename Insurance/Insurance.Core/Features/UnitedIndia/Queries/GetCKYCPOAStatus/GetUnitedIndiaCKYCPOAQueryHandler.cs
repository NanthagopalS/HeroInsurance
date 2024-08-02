using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.UnitedIndia;
using MediatR;

namespace Insurance.Core.Features.UnitedIndia.Queries.GetCKYCPOAStatus;

public class GetUnitedIndiaCKYCPOAQuery : UnitedIndiaCKYCResponseModel, IRequest<HeroResult<CreateLeadModel>>
{
    public string CKYCSatus { get; set; }
}
public class GetUnitedIndiaCKYCPOAQueryHandler : IRequestHandler<GetUnitedIndiaCKYCPOAQuery, HeroResult<CreateLeadModel>>
{
    private readonly IUnitedIndiaRepository _unitedIndiaRepository;
    public GetUnitedIndiaCKYCPOAQueryHandler(IUnitedIndiaRepository unitedIndiaRepository)
    {
        _unitedIndiaRepository = unitedIndiaRepository;
    }
    public async Task<HeroResult<CreateLeadModel>> Handle(GetUnitedIndiaCKYCPOAQuery request, CancellationToken cancellationToken)
    {
        CreateLeadModel createLeadModel = new CreateLeadModel();
        request.CKYCSatus = "FAILED";
        if (request.Status.Equals("auto_approved"))
        {
            request.CKYCSatus = "Done";
        }
        var leadDetails = await _unitedIndiaRepository.UpdateCKYCPOAStatus(request, cancellationToken);
        if (leadDetails != null)
        {
            leadDetails.CKYCstatus = request.CKYCSatus;
            return HeroResult<CreateLeadModel>.Success(leadDetails);
        }
        return HeroResult<CreateLeadModel>.Fail("Update UIIC CKYC POA Status Failed");
    }
}
