using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;
using System.Globalization;

namespace Insurance.Core.Features.InsuranceMaster.Queries.GetVehicleDetails;

/// <summary>
/// GetVehicleDetails Query
/// </summary>
public record GetVehicleDetailsQuery : IRequest<HeroResult<VehicleDetailVm>>
{
    public string VehicleNumber { get; set; }
    public string VehicleTypeId { get; set; }
    public bool IsQuote { get; set; }
}

public class GetVehicleDetailsQueryHandler : IRequestHandler<GetVehicleDetailsQuery, HeroResult<VehicleDetailVm>>
{
    private readonly IInsuranceMasterRepository _quoteRepository;
    private readonly IMapper _mapper;

    public GetVehicleDetailsQueryHandler(IInsuranceMasterRepository quoteRepository, IMapper mapper)
    {
        _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<HeroResult<VehicleDetailVm>> Handle(GetVehicleDetailsQuery request, CancellationToken cancellationToken)
    {
        var vehicleDetails = await _quoteRepository.GetVehicleDetails(request.VehicleNumber, request.VehicleTypeId, cancellationToken);
        if (vehicleDetails != null)
        {
            if (request.IsQuote && vehicleDetails.RegistrationDate != null)
            {
                DateTime currentDate = DateTime.Now;

                DateTime registrationDate = DateTime.ParseExact(vehicleDetails.RegistrationDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None);//Convert.ToDateTime(vehicleDetails.RegistrationDate);

                int yearsDifference = currentDate.Year - registrationDate.Year;

                // Check if the registration is greater than 15 years ago
                if (yearsDifference > 15 || (yearsDifference == 15 && currentDate.Month > registrationDate.Month))
                {
                    return HeroResult<VehicleDetailVm>.Fail("vehicleismorethan15yearsold");
                }
            }
            var result = _mapper.Map<VehicleDetailVm>(vehicleDetails);
            return HeroResult<VehicleDetailVm>.Success(result);
        }

        return HeroResult<VehicleDetailVm>.Fail("Vehicle details not found");
    }
}
