using Admin.Core.Contracts.Persistence;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.UpdateBUDetail
{
    public class UpdateBUDetailCommand : IRequest<HeroResult<bool>>
    {
        public string? BUId { get; set; }
        public string? BUName { get; set; }
        public string? BUHeadId { get; set; }
        public string? HierarchyLevelId { get; set; }
        public bool? IsSales { get; set; }

    }
    public class UpdateBUDetailCommandHandler : IRequestHandler<UpdateBUDetailCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// UpdateBUDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdateBUDetailCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="updateBUDetailCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(UpdateBUDetailCommand updateBUDetailCommand, CancellationToken cancellationToken)
        {
            var updateBUDetail = _mapper.Map<UpdateBUStatusResonse>(updateBUDetailCommand);
            var result = await _userRepository.UpdateBUDetail(updateBUDetail, cancellationToken);
            return HeroResult<bool>.Success(result);

        }
    }
}
