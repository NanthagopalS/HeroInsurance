using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.ICICI;
using MediatR;

namespace Insurance.Core.Features.ICICI.Command.CreateIMBroker;

public class CreateIMBrokerCommand : ICICICreateIMBrokerModel, IRequest<HeroResult<string>>
{
}
public class CreateIMBrokerCommandHandler : IRequestHandler<CreateIMBrokerCommand, HeroResult<string>>
{
    private readonly IICICIRepository _iCICIRepository;
    public CreateIMBrokerCommandHandler(IICICIRepository iCICIRepository)
    {
        _iCICIRepository = iCICIRepository;
    }

    public async Task<HeroResult<string>> Handle(CreateIMBrokerCommand createIMBrokerCommand, CancellationToken cancellationToken)
    {
        var response = await _iCICIRepository.CreateIMBroker(createIMBrokerCommand, cancellationToken);
        if(response != null)
        {
            return HeroResult<string>.Success(response);
        }
        return HeroResult<string>.Fail("Failed to create IM broker");
    }
}
