using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.ResetAdminUserAccountDetail
{
        public record UploadTrainingFileCommand : IRequest<HeroResult<bool>>
        {
            public string? TrainingModuleType { get; set; }
            public string? LessionTitle { get; set; }
            public string? DocumentFileName { get; set; }
            public string? DocumentId { get; set; }
            public byte[]? ImageStream { get; set; }
            public string? MaterialFormatType { get; set; }
            public string? VideoDuration { get; set; }
            public string? LessonNumber { get; set; }
            public string? CreatedBy { get; set; }
    }
        public class UploadTrainingFileCommandHandler : IRequestHandler<UploadTrainingFileCommand, HeroResult<bool>>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;

            /// <summary>
            /// Initialization
            /// </summary>
            /// <param name="AdminRepository"></param>
            /// <param name="mapper"></param>
            /// <exception cref="ArgumentNullException"></exception>
            public UploadTrainingFileCommandHandler(IUserRepository userRepository, IMapper mapper)
            {
                _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
                _mapper = mapper;
            }

            /// <summary>
            /// Handler
            /// </summary>
            /// <param name="request"></param>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            /// <exception cref="NotImplementedException"></exception>
            public async Task<HeroResult<bool>> Handle(UploadTrainingFileCommand request, CancellationToken cancellationToken)
            {
                var result = await _userRepository.UploadTrainingFile(request.TrainingModuleType, request.LessionTitle, request.DocumentId, request.DocumentFileName, request.MaterialFormatType, request.VideoDuration, request.LessonNumber, request.CreatedBy,cancellationToken).ConfigureAwait(false);
                return HeroResult<bool>.Success(result);
            }
        }
    
}
