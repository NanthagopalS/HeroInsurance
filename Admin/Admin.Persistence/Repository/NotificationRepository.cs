using Admin.Core.Contracts.Persistence;
using Admin.Domain.Notification;
using Admin.Persistence.Configuration;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdPartyUtilities.Abstraction;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Admin.Persistence.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly ISignzyService _signzyService;
        private readonly ISmsService _sMSService;
        private readonly IEmailService _emailService;
        private readonly IMongoDBService _mongodbService;
        private readonly IConfiguration _configuration;



        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public NotificationRepository(ApplicationDBContext context, ISignzyService signzyService, ISmsService sMSService, IEmailService emailService, IMongoDBService mongodbService, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _sMSService = sMSService ?? throw new ArgumentNullException(nameof(sMSService));
            _signzyService = signzyService ?? throw new ArgumentNullException(nameof(signzyService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        public async Task<IEnumerable<GetAdminAlertTypeModel>> GetAdminAlertType(CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<GetAdminAlertTypeModel>("[dbo].[Admin_GetAlertType]", commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<GetAdminRecipientTypeModel>> GetAdminRecipientType(CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<GetAdminRecipientTypeModel>("[dbo].[Admin_GetRecipientType]", commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<GetNotificationDetailByIdModel>> GetNotificationDetailById(string? NotificationId, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("NotificationId", NotificationId, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<GetNotificationDetailByIdModel>("[dbo].[Admin_GetNotificationDetailById]", parameters,
                         commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return result;
        }

        public async Task<IEnumerable<GetNotificationRecordByIdModel>> GetNotificationRecordById(string? NotificationId, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("NotificationId", NotificationId, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<GetNotificationRecordByIdModel>("[dbo].[Admin_GetNotificationRecordById]", parameters,commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return result;
        }

        public async Task<GetNotificationByIdAndTypeModel> GetNotificationByIdAndType(string? UserId, string? NotificationCategory, CancellationToken cancellationToken)
        {
            var TimeLimit = _configuration.GetSection("Notification").GetSection("TimeLimit").Value;
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
            parameters.Add("NotificationCategory", NotificationCategory, DbType.String, ParameterDirection.Input);
            parameters.Add("@TimeLimit", TimeLimit, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetNotificationByIdAndType]", parameters,
                         commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            GetNotificationByIdAndTypeModel response = new()
            {
                NotificationListNew = result.Read<NotificationListNew>(),
                NotificationListOld = result.Read<NotificationListOld>(),
            };

            return response;
        }

        public async Task<bool> UpdateNotificationViewStatus(string? NotificationBoradcastId, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("NotificationBoradcastId", NotificationBoradcastId, DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateNotificationViewStatus]", parameters,
                         commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return (result > 0);
        }
    }

}
