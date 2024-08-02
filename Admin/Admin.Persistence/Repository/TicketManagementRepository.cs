using Admin.Core.Contracts.Persistence;
using Admin.Domain.TicketManagement;
using Admin.Persistence.Configuration;
using Dapper;
using System.Data;
using ThirdPartyUtilities.Abstraction;

namespace Admin.Persistence.Repository
{
    public class TicketManagementRepository : ITicketManagementRepository
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
        public TicketManagementRepository(ApplicationDBContext context, ISignzyService signzyService, ISmsService sMSService, IEmailService emailService, IMongoDBService mongodbService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _sMSService = sMSService ?? throw new ArgumentNullException(nameof(sMSService));
            _signzyService = signzyService ?? throw new ArgumentNullException(nameof(signzyService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
        }

        public async Task<GetTicketManagementDetailResponseModel> GetTicketManagementDetail(string? TicketType, string? SearchText, string? RelationshipManagerIds, string? PolicyType, string? StartDate, string? EndDate, int? CurrentPageIndex, int? CurrentPageSize, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@TicketType", TicketType, DbType.String, ParameterDirection.Input);
            parameters.Add("@SearchText", SearchText, DbType.String, ParameterDirection.Input);
            parameters.Add("@RelationshipManagerIds", RelationshipManagerIds, DbType.String, ParameterDirection.Input);
            parameters.Add("@PolicyType", PolicyType, DbType.String, ParameterDirection.Input);
            parameters.Add("@StartDate", StartDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@EndDate", EndDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@CurrentPageIndex", CurrentPageIndex, DbType.String, ParameterDirection.Input);
            parameters.Add("@CurrentPageSize", CurrentPageSize, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetTicketManagementDetail]",
                parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            GetTicketManagementDetailResponseModel response = new()
            {
                TicketManagementDetailModel = result.Read<TicketManagementDetailModel>(),
                TicketManagementDetailPagingModel = result.Read<TicketManagementDetailPagingModel>()

            };
            return response;
        }

        public async Task<IEnumerable<GetTicketManagementDetailByIdModel>> GetTicketManagementDetailById(string? TicketId, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("TicketId", TicketId, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<GetTicketManagementDetailByIdModel>("[dbo].[Admin_GetTicketManagementDetailById]", parameters,
                         commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return result;
        }

        public async Task<GetDeactivationTicketManagementDetailResponseModel> GetDeactivationTicketManagementDetail(string? SearchText, string? RelationshipManagerId, string? PolicyType, string? StartDate, string? EndDate, int? CurrentPageIndex, int? CurrentPageSize, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@SearchText", SearchText, DbType.String, ParameterDirection.Input);
            parameters.Add("@RelationshipManagerId", RelationshipManagerId, DbType.String, ParameterDirection.Input);
            parameters.Add("@PolicyType", PolicyType, DbType.String, ParameterDirection.Input);
            parameters.Add("@StartDate", StartDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@EndDate", EndDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@CurrentPageIndex", CurrentPageIndex, DbType.String, ParameterDirection.Input);
            parameters.Add("@CurrentPageSize", CurrentPageSize, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryMultipleAsync("[dbo].[Admin_DeActivateTicketManagementDetail]",
                parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            GetDeactivationTicketManagementDetailResponseModel response = new()
            {
                DeActivateTicketManagementDetailModel = result.Read<DeActivateTicketManagementDetailModel>(),
                DeActivateTicketManagementDetailPagingModel = result.Read<DeActivateTicketManagementDetailPagingModel>()

            };
            return response;
        }

        public async Task<IEnumerable<GetPOSPDetailsByIDToDeActivateResponseModel>> GetPOSPDetailsByIDToDeActivate(string? POSPId, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("POSPId", POSPId, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<GetPOSPDetailsByIDToDeActivateResponseModel>("[dbo].[Admin_GetPOSPDetailsByIDToDeActivate]", parameters,
                         commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return result;
        }

        public async Task<bool> UpdateTicketManagementDetailById(string? TicketId,string? Description, string? Status, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("TicketId", TicketId, DbType.String, ParameterDirection.Input);
            parameters.Add("Description", Description, DbType.String, ParameterDirection.Input);
            parameters.Add("Status", Status, DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateTicketManagementDetailById]", parameters,
                         commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return (result > 0);
        }

        public async Task<IEnumerable<GetPOSPDetailsByDeactiveTicketIdResponceModel>> GetPOSPDetailsByDeactiveTicketId(string? DeactiveteId, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@DeactivateId", DeactiveteId, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<GetPOSPDetailsByDeactiveTicketIdResponceModel>("[dbo].[Admin_GetPOSPDetailsByDeactiveTicketId]", parameters,
                         commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return result;
        }

    }
}
