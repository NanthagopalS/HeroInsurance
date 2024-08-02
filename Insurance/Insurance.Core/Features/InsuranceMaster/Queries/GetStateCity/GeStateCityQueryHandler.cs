using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.InsuranceMaster;
using MediatR;

namespace Insurance.Core.Features.InsuranceMaster.Queries.GetStateCity;


public record GeStateCityQuery : IRequest<HeroResult<MasterCityModel>>
{
    public string InsurerId { get; set; }
    public string Pincode { get; set; }
    public string State { get; set; }
}
public class GeStateCityQueryHandler : IRequestHandler<GeStateCityQuery, HeroResult<MasterCityModel>>
{
    private readonly IInsuranceMasterRepository _insuranceMaster;

    public GeStateCityQueryHandler(IInsuranceMasterRepository insuranceMaster)
    {
        _insuranceMaster = insuranceMaster;
    }

    public async Task<HeroResult<MasterCityModel>> Handle(GeStateCityQuery request, CancellationToken cancellationToken)
    {
        var stateCity = await _insuranceMaster.GetStateCity(request.InsurerId,request.Pincode, request.State,cancellationToken);
        if (stateCity != null && stateCity.DefaultCity != null)
        {
            return HeroResult<MasterCityModel>.Success(stateCity);
        }
        return HeroResult<MasterCityModel>.Fail("Please provide valid pincode and state");
    }
}
