using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Commands.UserDocumentUpload;



/// <summary>
/// Command for UserDocumentUpload
/// </summary>
public record UserDocumentUploadCommand : IRequest<HeroResult<bool>>
{


    /// <summary>
    /// UserId
    /// </summary>
    //public string UserId { get; set; }
    //[Required]
    public string UserId { get; set; }

    /// <summary>
    /// DocumentTypeId
    /// </summary>
    //public string DocumentTypeId { get; set; }
    //[Required]
    public string DocumentTypeId { get; set; }

    /// <summary>
    /// DocumentFileName
    /// </summary>
    //[Required]
    public string DocumentFileName { get; set; }


    /// <summary>
    /// ImageStream
    /// </summary>
    public byte[] ImageStream { get; set; }

    /// <summary>
    /// FileSize
    /// </summary>
    public string FileSize { get; set; }
    public bool IsAdminUpdating { get; set; }
}
public class UserDocumentUploadCommandHandler : IRequestHandler<UserDocumentUploadCommand, HeroResult<bool>>
{
    private readonly IUserRepository _userDocumentRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="userDocumentRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public UserDocumentUploadCommandHandler(IUserRepository userDocumentRepository, IMapper mapper)
    {
        _userDocumentRepository = userDocumentRepository ?? throw new ArgumentNullException(nameof(userDocumentRepository));
        _mapper = mapper;
    }

    /// <summary>
    /// Handler
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<HeroResult<bool>> Handle(UserDocumentUploadCommand userdocumentuploadcommand, CancellationToken cancellationToken)
    {
        var userdocumentdetailmodel = _mapper.Map<UserDocumentDetailModel>(userdocumentuploadcommand);
        var result =  await _userDocumentRepository.UserDocumentUpload(userdocumentdetailmodel);
        return HeroResult<bool>.Success(result);
    }
}