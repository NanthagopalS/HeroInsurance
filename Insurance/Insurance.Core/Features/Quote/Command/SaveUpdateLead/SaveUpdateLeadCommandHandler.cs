using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Insurance.Core.Features.Quote.Command.SaveUpdateLead
{
    public class SaveUpdateLeadCommand : ProposalRequestModel, IRequest<HeroResult<SaveUpdateLeadVm>>
    {

    }
    public class SaveUpdateLeadCommandHandler : IRequestHandler<SaveUpdateLeadCommand, HeroResult<SaveUpdateLeadVm>>
    {
        private readonly IMapper _mapper;
        private readonly IQuoteRepository _quoteRepository;


        public SaveUpdateLeadCommandHandler(IMapper mapper, IQuoteRepository quoteRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
        }

        public async Task<HeroResult<SaveUpdateLeadVm>> Handle(SaveUpdateLeadCommand request, CancellationToken cancellationToken)
        {
            var proposalRequestModel = _mapper.Map<ProposalRequestModel>(request);

            var result = await _quoteRepository.ProposalRequest(proposalRequestModel, cancellationToken);
            
            if (result != null)
                return HeroResult<SaveUpdateLeadVm>.Success(result);
            else
                return HeroResult<SaveUpdateLeadVm>.Fail("Failed to save lead");
        }
    }
}
