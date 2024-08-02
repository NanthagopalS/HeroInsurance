using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Domain.User;
using Admin.Core.Responses;
using MediatR;
using System.Data;
using Microsoft.AspNetCore.Http;

namespace Admin.Core.Features.User.Commands.RoleModulePermission
{
    
    public record ExamBulkUploadCommand : IRequest<HeroResult<bool>>
    {
        public int? SequenceNo { get; set; }
        public string? ExamModuleType { get; set; }
        public string? ExamTitle { get; set; }

        public string? QuestionValue { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public List<ExamBulkUploadCommandInsertModel>? ExamBulkUploadCommandInsertModel { get; set; }
        //public List<ExampBulkUploadCommandInsert>? ExampBulkUploadCommandInserts { get; set; }
    }
    //public record ExampBulkUploadCommandInsert 
    //{
    //    public int? SrNo { get; set; }
    //    public string? QuestionId { get; set; }
    //    public int? OptionIndex { get; set; }
    //    public string? OptionValue { get; set; }
    //    public bool IsCorrectAnswer { get; set; }
    //    public bool IsActive { get; set; }
    //}
   
    public class ExamBulkUploadCommandHandler : IRequestHandler<ExamBulkUploadCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _bulkUploadRepository;
        private readonly IMapper _mapper;

        public ExamBulkUploadCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _bulkUploadRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(ExamBulkUploadCommand command, CancellationToken cancellationToken)
        {
            var uploadModel = _mapper.Map<ExamBulkUploadModel>(command);
            var result = await _bulkUploadRepository.ExampBulkUpload(uploadModel, cancellationToken);
            return HeroResult<bool>.Success(result);
        }

    }
}
