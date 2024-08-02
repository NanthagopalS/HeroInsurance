using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Features.Leads.GetPaymentStatus
{
    public class GetPaymentStatusListQuery : IRequest<HeroResult<IEnumerable<GetPaymentStatusListVm>>>
    {
       
    }
    public class GetPaymentStatusListQueryHandler : IRequestHandler<GetPaymentStatusListQuery, HeroResult<IEnumerable<GetPaymentStatusListVm>>>
    {
        private readonly ILeadsRepository _leadRepository;
        private readonly IMapper _mapper;

        public GetPaymentStatusListQueryHandler(ILeadsRepository leadRepository, IMapper mapper)
        {
            _leadRepository = leadRepository;
            _mapper = mapper;
        }


        public async Task<HeroResult<IEnumerable<GetPaymentStatusListVm>>> Handle(GetPaymentStatusListQuery request, CancellationToken cancellationToken)
        {
            var allHelpAndRequest = await _leadRepository.GetPaymentStatusList(request, cancellationToken).ConfigureAwait(false);
            if (allHelpAndRequest != null)
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPaymentStatusListVm>>(allHelpAndRequest);
                return HeroResult<IEnumerable<GetPaymentStatusListVm>>.Success(listInsurer);
            }
            return HeroResult<IEnumerable<GetPaymentStatusListVm>>.Fail("No Record Found");
        }
    }
}
