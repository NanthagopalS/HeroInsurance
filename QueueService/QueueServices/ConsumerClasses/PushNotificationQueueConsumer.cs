using Newtonsoft.Json.Linq;
using QueueServices.Configuration;
using QueueServices.Features.PushNotification.Commands.UpdatePushNotificationBroadcastStatus;
using QueueServices.Features.PushNotification.Commands.UpdatePushNotificationMasterStatus;
using QueueServices.Features.PushNotification.Queries.Admin_GetNotificationBroadcastById;
using QueueServices.Features.PushNotification.Queries.Admin_GetPushNotificationById;
using QueueServices.Models.Queue;
using QueueServices.Models.Quque;
using ThirdPartyUtilities.Implementation;

namespace QueueServices.ConsumerClasses
{
    public class PushNotificationQueueConsumer
    {

        public PushNotificationQueueConsumer() {
        }
        public async Task<Admin_GetPushNotificationByIdResponseModel> PushNotificationConsume(JObject message,CancellationToken cancellationToken,ApplicationDBContext appDB)
        {

            string notificationID = message["notification_id"].ToString();
            Console.WriteLine(value: $"message recieved  :  {notificationID}");
            var req = new Admin_GetPushNotificationByIdQuery()
            {
                NotificationId = notificationID
            };
            QueueRepository repo = new QueueRepository(appDB);
            Admin_GetPushNotificationByIdQueryHandler handler = new Admin_GetPushNotificationByIdQueryHandler(repo);
            Admin_GetPushNotificationByIdResponseModel masterRec = await handler.Handle(req, cancellationToken);
            Admin_GetNotificationBroadcastByIdQueryHandler BroadcastRecordsHandler = new Admin_GetNotificationBroadcastByIdQueryHandler(repo);
            IEnumerable<Admin_GetPushNotificationBroadcastByIdResponseModel> BroadcastRecords = await BroadcastRecordsHandler.Handle(new Admin_GetNotificationBroadcastByIdQuery() { NotificationId = notificationID }, cancellationToken);
            JObject firebaseMasterObject = new JObject();
            List<string> fmcTokens= new List<string>();
            List<JObject> firebaseTokenToUserIdMapping=  new List<JObject>();
            JObject tempArr = new JObject();
            foreach (var brSingle in BroadcastRecords)
            {
                firebaseMasterObject[brSingle.RecipientUserId] = new JObject();
                firebaseMasterObject[brSingle.RecipientUserId]["broadCastId"] = brSingle.NotificationBoradcastId;
                string browserId = brSingle.BrowserId.ToString();
                string mobileId = brSingle.MobileDeviceId.ToString();
                string reciepentId = brSingle.RecipientUserId;
                if(browserId!=null && browserId!="")
                {
                    fmcTokens.Add(browserId);
                    firebaseMasterObject[reciepentId]["browser"] = new JObject();
                    firebaseMasterObject[reciepentId]["browser"]["fmctoken"] = browserId;
                    tempArr = new JObject();
                    tempArr["userId"] = reciepentId;
                    tempArr["type"] = "browser";
                    tempArr["fmctoken"] = browserId;
                    firebaseTokenToUserIdMapping.Add(tempArr);
                }
                if (mobileId != null && mobileId != "")
                {
                    fmcTokens.Add(mobileId);
                    firebaseMasterObject[reciepentId]["mobile"] = new JObject();
                    firebaseMasterObject[reciepentId]["mobile"]["fmctoken"] = mobileId;
                    tempArr = new JObject();
                    tempArr["userId"] = reciepentId;
                    tempArr["type"] = "mobile";
                    tempArr["fmctoken"] = mobileId;
                    firebaseTokenToUserIdMapping.Add(tempArr);
                }
            }
            if (fmcTokens.Count() > 0)
            {
                FirebaseService firebase = new FirebaseService();
                string NotificationTitle = masterRec.NotificationTitle.ToString();
                string NotificationBody = masterRec.Description.ToString();
                var firebaseProcess = await firebase.SendNotificationToAll(fmcTokens, NotificationTitle, NotificationBody).Result;
                
                    int index = 0;
                    foreach (var item in firebaseProcess.Responses)
                    {
                        string userId = firebaseTokenToUserIdMapping[index]["userId"].ToString();
                        string mappingKeyFmc = firebaseTokenToUserIdMapping[index]["fmctoken"].ToString();
                        string browserFmcTOken = firebaseMasterObject[userId]["browser"] == null? "": firebaseMasterObject[userId]["browser"]["fmctoken"].ToString();
                        string mobileFmcToken = firebaseMasterObject[userId]["mobile"] == null ? "": firebaseMasterObject[userId]["mobile"]["fmctoken"].ToString();
                        if (browserFmcTOken == mappingKeyFmc)
                        {
                            firebaseMasterObject[userId]["browser"]["ex"] = item.Exception==null ? null : item.Exception.ToString();
                            firebaseMasterObject[userId]["browser"]["isSuccess"] = item.IsSuccess;
                            firebaseMasterObject[userId]["browser"]["messageId"] = item.MessageId;
                        }
                        if (mobileFmcToken == mappingKeyFmc)
                        {
                            firebaseMasterObject[userId]["mobile"]["ex"] = item.Exception == null ? null : item.Exception.ToString();
                            firebaseMasterObject[userId]["mobile"]["isSuccess"] = item.IsSuccess;
                            firebaseMasterObject[userId]["mobile"]["messageId"] = item.MessageId;
                        }
                    index++;
                    }

                UpdatePushNotificationBroadcastStatusCommandHandler updateBroadcastHandler = new UpdatePushNotificationBroadcastStatusCommandHandler(repo);
                foreach (KeyValuePair<string,JToken> resRecord in firebaseMasterObject)
                {
                  var singleUpdate =   await updateBroadcastHandler.Handle(new UpdatePushNotificationBroadcastStatusCommand()
                     {
                         FirebaseQueueId = resRecord.Value.ToString(),
                         NotificationId = resRecord.Value["broadCastId"].ToString()
                     }, cancellationToken);
                    Console.WriteLine(singleUpdate);
                }
            }
            UpdatePushNotificationMasterStatusCommandHandler updateMasterHandler = new  UpdatePushNotificationMasterStatusCommandHandler(repo);
            await updateMasterHandler.Handle(new UpdatePushNotificationMasterStatusCommand() { NotificationId = masterRec.NotificationId },cancellationToken);

            Console.WriteLine(firebaseMasterObject.ToString());
            return masterRec;
        }
    }
}
