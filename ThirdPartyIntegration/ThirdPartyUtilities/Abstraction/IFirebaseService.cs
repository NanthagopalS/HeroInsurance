using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThirdPartyUtilities.Abstraction
{
    public interface IFirebaseService
    {
        Task SendNotification(string FMCtoken, string title, string body);
        Task<Task<BatchResponse>> SendNotificationToAll(List<string> tokens, string title, string body);
    }
}
