using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.ICICI;
using Insurance.Domain.Quote;
using MediatR;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Insurance.Core.Features.HDFC.Command.CreatePOSP;

public record CreatePOSPCommand : IRequest<HeroResult<bool>>
{
    public string POSPId { get; set; }
    public string MobileNumber { get; set; }
    public string AadharNumber { get; set; }
    public string EmailId { get; set; }
    public string Name { get; set; }
    public string PanNumber { get; set; }
    public string State { get; set; }
}
public class CreatePOSPCommandHandler : IRequestHandler<CreatePOSPCommand, HeroResult<bool>>
{
    private readonly IHDFCRepository _iHDFCRepository;
    private readonly IMapper _mapper;
    public CreatePOSPCommandHandler(IHDFCRepository iHDFCRepository, IMapper mapper)
    {
        _iHDFCRepository = iHDFCRepository;
        _mapper = mapper;
    }


    public async Task<HeroResult<bool>> Handle(CreatePOSPCommand createPOSPCommand, CancellationToken cancellationToken)
    {
        var createPOSPModel = _mapper.Map<HDFCCreateIMBrokerRequestDto>(createPOSPCommand);
        var response = await _iHDFCRepository.CreatePOSP(createPOSPModel, cancellationToken);
        if(response != false)
        {
            return HeroResult<bool>.Success(response);
        }
        return HeroResult<bool>.Fail("Failed To Update HDFC POSPId");
    }
}
