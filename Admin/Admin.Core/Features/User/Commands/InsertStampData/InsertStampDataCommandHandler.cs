using Admin.Core.Contracts.Persistence;
using Admin.Domain.Roles;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.InsertStampData
{
    public class InsertStampDataCommand : IRequest<HeroResult<bool>>
    {
        //public string? Id { get; set; }
        public string? SrNo { get; set; }
        public string? StampData { get; set; }
    }
    public class InsertStampDataCommandHandler : IRequestHandler<InsertStampDataCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// InsertStampDataCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InsertStampDataCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(InsertStampDataCommand request, CancellationToken cancellationToken)
        {
            var InsertStampData = _mapper.Map<InsertStampDataModel>(request);
            var result = await _userRepository.InsertStampData(request.SrNo, request.StampData, cancellationToken);
            return HeroResult<bool>.Success(result);

        }
    }
}
