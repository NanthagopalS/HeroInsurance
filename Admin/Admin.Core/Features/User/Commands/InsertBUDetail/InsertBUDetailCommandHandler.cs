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

namespace Admin.Core.Features.User.Commands.InsertBUDetail
{
    public class InsertBUDetailCommand : IRequest<HeroResult<bool>>
    {
        public string? BUName { get; set; }
        public string? BUHeadId { get; set; }
        public string? HierarchyLevelId { get; set; }
        public bool? IsSales { get; set; }
    }

    public class InsertBUDetailCommandHandler : IRequestHandler<InsertBUDetailCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userrepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="userDocumentRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InsertBUDetailCommandHandler(IUserRepository userrepository, IMapper mapper)
        {
            _userrepository = userrepository ?? throw new ArgumentNullException(nameof(userrepository));
            _mapper = mapper;
        }

        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<HeroResult<bool>> Handle(InsertBUDetailCommand insertBUDetailcommand, CancellationToken cancellationToken)
        {
            var budetailModel =_mapper.Map<BuResponsePermissionModel>(insertBUDetailcommand);
            var result = await _userrepository.InsertBUDetail(budetailModel, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }
}
