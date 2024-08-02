using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.HelpAndSupport.InsertDeactivatePospDetails
{
    public class InsertDeactivatePospDetailsCommand : IRequest<HeroResult<bool>>
    {
        public string? POSPId { get; set; }
        public string? Remark { get; set; }
        public string? DocumentId1 { get; set; }
        public string? DocumentId2 { get; set; }
        public string? Status { get; set; }
    }
    public class InsertDeactivatePospDetailsCommandHandler : IRequestHandler<InsertDeactivatePospDetailsCommand, HeroResult<bool>>
    {
        private readonly IHelpAndSupportRepository _IHelpAndSupportRepository;
        private readonly IMapper _mapper;

        public InsertDeactivatePospDetailsCommandHandler(IHelpAndSupportRepository helpAndSupportRepository, IMapper mapper)
        {
            _IHelpAndSupportRepository = helpAndSupportRepository ?? throw new ArgumentNullException(nameof(helpAndSupportRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(InsertDeactivatePospDetailsCommand command, CancellationToken cancellationToken)
        {
            var result = await _IHelpAndSupportRepository.InsertDeactivatePospDetails(command, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }
}
