using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using POSP.Domain.POSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Commands.InsertExamDetail;


public record InsertExamDetailCommand : IRequest<HeroResult<ExamDetailResponse>>
{
    public string? UserId { get; set; }
    public string? ExamStatus { get; set; }

}

public  class InsertExamDetailCommandHandler : IRequestHandler<InsertExamDetailCommand, HeroResult<ExamDetailResponse>>
{
    private readonly IPOSPRepository _pospRepository;
    private readonly IMapper _mapper;


    /// <summary>
    /// InsertBenefitsDetailCommandHandler
    /// </summary>
    /// <param name="pospRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public InsertExamDetailCommandHandler(IPOSPRepository pospRepository, IMapper mapper)
    {
        _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
        _mapper = mapper;
    }


    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="insertExamInstructionsDetailCommand"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HeroResult<ExamDetailResponse>> Handle(InsertExamDetailCommand insertexamdetailcommand, CancellationToken cancellationToken)
    {
        var trainingInstructionsDetailModel = _mapper.Map<ExamDetailResponse>(insertexamdetailcommand);
        var result = await _pospRepository.InsertPOSPExamDetail(insertexamdetailcommand.UserId,insertexamdetailcommand.ExamStatus, cancellationToken);
        return HeroResult<ExamDetailResponse>.Success(result);
    }
}
