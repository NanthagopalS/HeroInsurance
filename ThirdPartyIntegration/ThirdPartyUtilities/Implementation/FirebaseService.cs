using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using ThirdPartyUtilities.Abstraction;
using Newtonsoft.Json.Linq;

namespace ThirdPartyUtilities.Implementation
{
    public class FirebaseService : IFirebaseService
    {
        /* code to create a firebase instance*/
        public FirebaseService()
        {
            var pathToFirebaseServiceAccountJson = "FirebaseServiceAccount.json";
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(pathToFirebaseServiceAccountJson),
                    ProjectId = "hero2notification"
                });
            }

        }

        /* code to send same notification to multiple users at once*/
        public async Task<Task<BatchResponse>> SendNotificationToAll(List<string> tokens, string title, string body)
        {
            var message = new MulticastMessage()
            {
                Tokens = tokens,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                }
            };
            var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            return Task.FromResult(response);
        }

        /* code to send same notification to single user*/
        public Task SendNotification(string FMCtoken, string title, string body)
        {
            var message = new Message()
            {
                Token = FMCtoken,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                }
            };
            string responce = FirebaseMessaging.DefaultInstance.SendAsync(message).Result;
            return Task.FromResult(responce);
        }
        
        /* code to send multiple message to Multiple users at once (max: 500)*/
        /* batchMessage structure = List<
         * {
         * "token":"xyzbc",
         * "title":"title of the message"
         * "body":"message body to be sent"
         * }
         * >  */
        public async Task<Task<BatchResponse>> SendBatchMessage(List<JObject> batchMessages)
        {
            List<Message> messages = new List<Message>();
            foreach (var msg in batchMessages)
            {
                messages.Add(new Message()
                {
                    Notification = new Notification()
                    {
                        Title = msg["title"].ToString(),
                        Body= msg["body"].ToString(),
                    },
                    Token= msg["token"].ToString(),
                });
            }
            var responce = await FirebaseMessaging.DefaultInstance.SendAllAsync(messages);
            return Task.FromResult(responce);
        }
    }
}
