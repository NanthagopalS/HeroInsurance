using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetRecipientList
{
    public class GetRecipientListQuery : IRequest<HeroResult<IEnumerable<GetRecipientListVm>>>
    {
        public string? SearchText { get; set; }
        public string? RecipientType { get; set; }
    }
    public class GetRecipientListQueryHandler : IRequestHandler<GetRecipientListQuery, HeroResult<IEnumerable<GetRecipientListVm>>>
    {


        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public GetRecipientListQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<GetRecipientListVm>>> Handle(GetRecipientListQuery request, CancellationToken cancellationToken)
        {
            var modelResult = await _userRepository.GetRecipientList(request.SearchText, request.RecipientType, cancellationToken).ConfigureAwait(false);
            if (modelResult.Any())
            {
                var listInsurerModel = _mapper.Map<IEnumerable<GetRecipientListVm>>(modelResult);
                return HeroResult<IEnumerable<GetRecipientListVm>>.Success(listInsurerModel);
            }
            return HeroResult<IEnumerable<GetRecipientListVm>>.Fail("No Record Found");
        }
    }
}
