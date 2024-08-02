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

namespace Identity.Core.Features.User.Commands.InsertBenifitsDetail;

public record InsertBenefitsDetailCommand : IRequest<HeroResult<bool>>
{
    public string Id { get; set; }
    public string BenefitsTitle { get; set; }
    public string BenefitsDescription { get; set; }

}

public class InsertBenefitsDetailCommandHandler : IRequestHandler<InsertBenefitsDetailCommand, HeroResult<bool>>
{
    private readonly IUserRepository _userDetailRepository;
    private readonly IMapper _mapper;


    /// <summary>
    /// BenefitsDetailCommandHandler
    /// </summary>
    /// <param name="benefitsDetailRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public InsertBenefitsDetailCommandHandler(IUserRepository userDetailRepository, IMapper mapper)
    {
        _userDetailRepository = userDetailRepository ?? throw new ArgumentNullException(nameof(userDetailRepository));
        _mapper = mapper;
    }


    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="benefitsDetailCommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HeroResult<bool>> Handle(InsertBenefitsDetailCommand benefitsDetailCommand, CancellationToken cancellationToken)
    {
        var benefitsDetailModel = _mapper.Map<BenefitDetailModel>(benefitsDetailCommand);
        var result = await _userDetailRepository.InsertBenefitsDetailCreationDetail(benefitsDetailModel, cancellationToken);
        return HeroResult<bool>.Success(result);
    }
}

