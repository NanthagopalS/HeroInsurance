using Dapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.Banner;
using Identity.Persistence.Configuration;
using System.Data;
using ThirdPartyUtilities.Abstraction;

namespace Identity.Persistence.Repository;

public class BannerRepository : IBannerRepository
{
    private readonly ApplicationDBContext _context;
    private readonly IMongoDBService _mongodbService;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public BannerRepository(ApplicationDBContext context, IMongoDBService mongodbService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
    }

    public async Task<bool> BannerUpload(BannerDetailModel bannerdetailmodel)
    {
        Stream stream = new MemoryStream(bannerdetailmodel.BannnerImage);
        var id = await _mongodbService.MongoUpload(bannerdetailmodel.BannerFileName, stream, bannerdetailmodel.BannnerImage);
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("BannerFileName", bannerdetailmodel.BannerFileName, DbType.String, ParameterDirection.Input);
        parameters.Add("BannerStoragePath", bannerdetailmodel.BannerStoragePath, DbType.String, ParameterDirection.Input);
        parameters.Add("BannerType", bannerdetailmodel.BannerType, DbType.String, ParameterDirection.Input);
        parameters.Add("DocumentId", id, DbType.String, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Identity_InsertBannerDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return (result > 0);
    }

    public async Task<IEnumerable<BannerDetailModel>> GetBannerDetail(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<BannerDetailModel>("[dbo].[Identity_BannerDetail]", commandType: CommandType.StoredProcedure);
        foreach (var item in result)
        {
            //item.Image64 = await _mongodbService.MongoDownload("63b670462b67c64123803a07");
            item.Image64 = await _mongodbService.MongoDownload(item.DocumentId);
        }

        return result;
    }
}

