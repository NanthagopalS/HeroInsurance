using FluentValidation;

namespace Insurance.Core.Features.InsuranceMaster.Queries.GetVehicleDetails;
public class GetVehicleDetailsValidator :AbstractValidator<GetVehicleDetailsQuery>
{
	public GetVehicleDetailsValidator()
	{
		//RuleFor(x => x.VehicleNumber).NotEmpty().Length(10).WithMessage("V101");
	}
}
