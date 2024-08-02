using Admin.Core.Contracts.Persistence;
//using NETCore.MailKit.Core;
using Admin.Domain.Banners;
using Admin.Persistence.Configuration;
using Dapper;
using System.Data;
using ThirdPartyUtilities.Abstraction;

namespace Admin.Persistence.Repository;
public class BannerRepository : IBannerRepository
{
    private readonly ApplicationDBContext _context;
    private readonly ISignzyService _signzyService;
    private readonly ISmsService _sMSService;
    private readonly IEmailService _emailService;
    private readonly IMongoDBService _mongodbService;


    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public BannerRepository(ApplicationDBContext context, ISignzyService signzyService, ISmsService sMSService, IEmailService emailService, IMongoDBService mongodbService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _sMSService = sMSService ?? throw new ArgumentNullException(nameof(sMSService));
        _signzyService = signzyService ?? throw new ArgumentNullException(nameof(signzyService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
    }

    public async Task<IEnumerable<GetBannerDetailModel>> GetBannerDetail(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<GetBannerDetailModel>("[dbo].[Admin_GetBannerDetail]",
            commandType: CommandType.StoredProcedure);
        if (result.Any())
        {
            result.FirstOrDefault().ImageStream = await _mongodbService.MongoDownload(result.FirstOrDefault().DocumentId);
        }
        return result;
    }

}

