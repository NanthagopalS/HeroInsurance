using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Features.Chola.Command.CKYC
{
    public class CholaCKYCCommand: CKYCModel,IRequest<HeroResult<SaveCKYCResponse>>
    {
        public string QuoteTransactionId { get; set; }
    }
    public class CholaCKYCCommandHandler : IRequestHandler<CholaCKYCCommand, HeroResult<SaveCKYCResponse>>
    {
        private readonly ICholaRepository _cholaRepository;
        public CholaCKYCCommandHandler(ICholaRepository cholaRepository)
        {
            _cholaRepository = cholaRepository;
        }
        public async Task<HeroResult<SaveCKYCResponse>> Handle(CholaCKYCCommand request, CancellationToken cancellationToken)
        {
            var response = await _cholaRepository.GetCKYCDetails(request, cancellationToken);
            if (response == null)
                return HeroResult<SaveCKYCResponse>.Fail("CKYC Failed");
            return HeroResult<SaveCKYCResponse>.Success(response);
        }
    }

}
