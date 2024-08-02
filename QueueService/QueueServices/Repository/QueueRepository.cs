using System;
using System.Data;
using Dapper;
using QueueServices.Contracts.Persistence;
using QueueServices.Models.Queue;
using QueueServices.Models.Quque;
using QueueServices.Features.PushNotification.Commands.UpdatePushNotificationBroadcastStatus;
using QueueServices.Features.PushNotification.Commands.UpdatePushNotificationMasterStatus;

namespace QueueServices.Configuration
{
    public class QueueRepository : IQueueRepository
    {
        private readonly ApplicationDBContext _context;


        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public QueueRepository(ApplicationDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        /*code to get notification broadcast records for respective users */
        public async Task<IEnumerable<Admin_GetPushNotificationBroadcastByIdResponseModel>> Admin_GetBroadcastNotificationDetailById(string? NotificationId, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("NotificationId", NotificationId, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<Admin_GetPushNotificationBroadcastByIdResponseModel>("[dbo].[Admin_GetBroadcastNotificationDetailById]", parameters,
                         commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return result;
        }

        /* update a single record for NotificationBroadcast table */
        public async Task<bool> UpdatePushNotificationBroadcastStatus(UpdatePushNotificationBroadcastStatusCommand updateCmd, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("NotificationBoradcastId", updateCmd.NotificationId, DbType.String, ParameterDirection.Input);
            parameters.Add("FirebaseQueueId", updateCmd.FirebaseQueueId, DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Admin_UpdatePushNotificationBroadcastStatus]", parameters,
                         commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return result > 0;
        }

        /* update a master record for Notification table */
        public async Task<bool> UpdatePushNotificationMasterStatus(UpdatePushNotificationMasterStatusCommand updateCmd, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("NotificationId", updateCmd.NotificationId, DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Admin_UpdatePushNotificationMasterStatus]", parameters,
                         commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return result > 0;
        }

        /* code to fetch master push notification to be fetched*/
        public async Task<Admin_GetPushNotificationByIdResponseModel> GetPushNotificationbyId(string? NotificationId, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("NotificationId", NotificationId, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<Admin_GetPushNotificationByIdResponseModel>("[dbo].[Admin_GetPushNotificationById]", parameters,
                         commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            if(result.Count<Admin_GetPushNotificationByIdResponseModel>() > 0)
            {
                return result.First<Admin_GetPushNotificationByIdResponseModel>();
            }
            return new Admin_GetPushNotificationByIdResponseModel();
        }

        public async Task<bool> InsertTriggerNotification(string? AlertTypeId,string? RecipientId,string? RecipientUserids, string? EventId, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@AlertTypeId", AlertTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("@RecipientId", RecipientId, DbType.String, ParameterDirection.Input);
            parameters.Add("@RecipientUserids", RecipientUserids, DbType.String, ParameterDirection.Input);
            parameters.Add("@NotificationEventId", EventId, DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Admin_InsertTriggerNotification]", parameters,
                         commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return (result > 0);
        }
    }
}
