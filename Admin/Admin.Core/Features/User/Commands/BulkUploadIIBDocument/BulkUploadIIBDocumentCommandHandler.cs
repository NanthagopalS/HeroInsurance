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

namespace Admin.Core.Features.User.Commands
{
    public class BulkUploadIIBDocumentCommand : IRequest<HeroResult<bool>>
    {
        public string UserId { get; set; }
        public string IIBStatus { get; set; }
        public string IIBUploadStatus { get; set; }
    }

    public class BulkUploadIIBDocumentCommandHandler : IRequestHandler<BulkUploadIIBDocumentCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userrepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="userrepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BulkUploadIIBDocumentCommandHandler(IUserRepository userrepository, IMapper mapper)
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
        public async Task<HeroResult<bool>> Handle(BulkUploadIIBDocumentCommand bulkUpload, CancellationToken cancellationToken)
        {
            var iibUploaddetailmodel = _mapper.Map<IIBBulkUploadDocument>(bulkUpload);
            var result = await _userrepository.BulkUploadIIBDocument(iibUploaddetailmodel, cancellationToken);
            return HeroResult<bool>.Success(result);
        }

    }
}
