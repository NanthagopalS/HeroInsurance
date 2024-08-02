using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.HelpAndSupport.InsertDeactivatePospDetails;
using Admin.Core.Features.HelpAndSupport.RaiseRequest;
//using NETCore.MailKit.Core;
using Admin.Domain.HelpAndSupport;
using Admin.Domain.User;
using Admin.Persistence.Configuration;
using Dapper;
using System.Data;
using ThirdPartyUtilities.Abstraction;

namespace Admin.Persistence.Repository;
public class HelAndSupportRepository : IHelpAndSupportRepository
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
    public HelAndSupportRepository(ApplicationDBContext context, ISignzyService signzyService, ISmsService sMSService, IEmailService emailService, IMongoDBService mongodbService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _sMSService = sMSService ?? throw new ArgumentNullException(nameof(sMSService));
        _signzyService = signzyService ?? throw new ArgumentNullException(nameof(signzyService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
    }

    public async Task<IEnumerable<GetConcernTypeResponseModel>> GetConcernType(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<GetConcernTypeResponseModel>("[dbo].[Admin_GetConcernType]", commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<IEnumerable<GetSubConcernTypeResponseModel>> GetSubConcernType(string? concernTypeId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@ConcernTypeId", concernTypeId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<GetSubConcernTypeResponseModel>("[dbo].[Admin_GetSubConcernType]", parameters, commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<GetAllHelpAndSupportResponseModel> GetAllHelpAndSupport(string? searchtext,  string? UserId, string? startDate, string? endDate, int? currentpageIndex, int? currentpageSize, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@SearchText", searchtext, DbType.String, ParameterDirection.Input);
        parameters.Add("@UserId", UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("@StartDate", startDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", endDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@CurrentPageIndex", currentpageIndex, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@CurrentPageSize", currentpageSize, DbType.Int64, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetAllHelpAndSupport]", parameters,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        GetAllHelpAndSupportResponseModel response = new()
        {
            GetAllHelpAndSupportModel = result.Read<GetAllHelpAndSupportModel>(),
            GetAllHelpAndSupportPagingModel = result.Read<GetAllHelpAndSupportPagingModel>()

        };
        return response;
    }

    public async Task<bool> InsertRaiseRequest(RaiseRequestCommand cmd, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("ConcernTypeId", cmd.ConcernTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("SubConcernTypeId", cmd.SubConcernTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("SubjectText", cmd.SubjectText, DbType.String, ParameterDirection.Input);
        parameters.Add("DetailText", cmd.DetailText, DbType.String, ParameterDirection.Input);
        parameters.Add("DocumentId", cmd.DocumentId, DbType.String, ParameterDirection.Input);
        parameters.Add("UserId", cmd.UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Admin_InsertRaiseRequest]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return (result > 0);
    }

    public async Task<IEnumerable<ParticularHelpAndSupportModel>> GetParticularHelpAndSupport(string requestId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@RequestId", requestId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<ParticularHelpAndSupportModel>("[dbo].[Admin_GetParticularHelpAndSupport]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }
    public async Task<bool> DeleteHelpAndSupport(string requestId, CancellationToken cancellationToken)
    {
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("RequestId", requestId, DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Admin_DeleteHelpAndSupport]", parameters,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return (result > 0);

        }
    }

    public async Task<bool> InsertDeactivatePospDetails(InsertDeactivatePospDetailsCommand command, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("POSPId", command.POSPId, DbType.String, ParameterDirection.Input);
        parameters.Add("remark", command.Remark, DbType.String, ParameterDirection.Input);
        parameters.Add("DocumentId1", command.DocumentId1, DbType.String, ParameterDirection.Input);
        parameters.Add("DocumentId2", command.DocumentId2, DbType.String, ParameterDirection.Input);
        parameters.Add("Status", command.Status, DbType.String, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Admin_InsertDeactivatePospDetails]", parameters,
            commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return (result > 0);

    }
}

