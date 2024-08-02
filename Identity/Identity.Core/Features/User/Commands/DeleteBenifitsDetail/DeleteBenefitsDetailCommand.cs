using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Commands.DeleteBenifitsDetail;


public record DeleteBenefitsDetailQuery : IRequest<HeroResult<bool>>
{
    public string Id { get; set; }
}

public class DeleteBenefitsDetailCommand : IRequestHandler<DeleteBenefitsDetailQuery, HeroResult<bool>>
{
    private readonly IUserRepository _benifitsRepository;
    private readonly IMapper _mapper;
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="quoteRepository"></param>
    /// <param name="mapper"></param>
    public DeleteBenefitsDetailCommand(IUserRepository BenifitsRepository, IMapper mapper)
    {
        _benifitsRepository = BenifitsRepository;
        _mapper = mapper;
    }

    public async Task<HeroResult<bool>> Handle(DeleteBenefitsDetailQuery request, CancellationToken cancellationToken)
    {
        var result = await _benifitsRepository.DeleteBenefitsDetail(request.Id,cancellationToken).ConfigureAwait(false);

        return HeroResult<bool>.Success(result);
    }
}


