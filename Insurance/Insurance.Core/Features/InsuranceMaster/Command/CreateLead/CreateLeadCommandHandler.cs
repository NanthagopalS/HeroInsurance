using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain;
using Insurance.Domain.InsuranceMaster;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Insurance.Core.Features.InsuranceMaster.Command.LeadDetails;
/// <summary>
/// Command for CreateLead
/// </summary>
public record CreateLeadCommand : IRequest<HeroResult<IEnumerable<LeadModel>>>
{
    /// <summary>
    /// Vehicle Type Id
    /// </summary>
    [Required]
    public string VehicleTypeId { get; set; }

    /// <summary>
    /// Vehicle Number
    /// </summary>
    public string VehicleNumber { get; set; }

    /// <summary>
    /// Variant Id
    /// </summary>
    public string VariantId { get; set; }

    /// <summary>
    /// Year Id
    /// </summary>
    public string YearId { get; set; }

    /// <summary>
    /// Lead Name
    /// </summary>
    public string LeadName { get; set; }

    /// <summary>
    /// Phone Number
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Email 
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// PrevPolicyTypeId 
    /// </summary>
    public string PrevPolicyTypeId { get; set; }
    /// <summary>
    /// CarrierType 
    /// </summary>
    public string CarrierType { get; set; }
    /// <summary>
    /// UsageNatureId 
    /// </summary>
    public string UsageNatureId { get; set; }
    /// <summary>
    /// VehicleBodyId 
    /// </summary>
    public string VehicleBodyId { get; set; }
    /// <summary>
    /// HazardousVehicleUse 
    /// </summary>
    public string HazardousVehicleUse { get; set; }
    /// <summary>
    /// IfTrailer 
    /// </summary>
    public string IfTrailer { get; set; }
    /// <summary>
    /// TrailerIDV 
    /// </summary>
    public string TrailerIDV { get; set; }

    public string RTOId { get; set; }
    public string LeadID { get; set; }
    public bool IsBrandNew { get; set; }
    public string RefLeadId { get; set; }
    public string CategoryId { get; set; }
    public string SubCategoryId { get; set; }
    public string UsageType { get; set; }
    public string PCVVehicleCategory { get; set; }
}
/// <summary>
/// Handler for CreateLead
/// </summary>
public class CreateLeadCommandHandler : IRequestHandler<CreateLeadCommand, HeroResult<IEnumerable<LeadModel>>>
{
    private readonly IInsuranceMasterRepository _insuranceMasterRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="insuranceMasterRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CreateLeadCommandHandler(IInsuranceMasterRepository insuranceMasterRepository, IMapper mapper)
    {
        _insuranceMasterRepository = insuranceMasterRepository ?? throw new ArgumentNullException(nameof(insuranceMasterRepository));
        _mapper = mapper;
    }

    /// <summary>
    /// Handler
    /// </summary>
    /// <param name="leadDetailsCommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HeroResult<IEnumerable<LeadModel>>> Handle(CreateLeadCommand createLeadCommand, CancellationToken cancellationToken)
    {
        var createLeadModel = _mapper.Map<CreateLeadModel>(createLeadCommand);

        if(createLeadModel != null && !createLeadModel.IsBrandNew)
        {
            var response = await _insuranceMasterRepository.GetLeadPreviousPolicyType(createLeadModel.VehicleTypeId, createLeadModel.VehicleNumber, createLeadModel.PrevPolicyTypeId, createLeadModel.YearId, cancellationToken);
            if (response != null)
            {
                createLeadModel.PrevPolicyTypeId = response.PreviousPolicyTypeId;
                createLeadModel.YearId = response.YearId;
            }
        }
        

        var result = await _insuranceMasterRepository.CreateLead(createLeadModel, cancellationToken);
        if (result.Any())
        {
            var leadId = _mapper.Map<IEnumerable<LeadModel>>(result);
            return HeroResult<IEnumerable<LeadModel>>.Success(leadId);
        }

        return HeroResult<IEnumerable<LeadModel>>.Fail("Failed To Insert Lead");
    }
}