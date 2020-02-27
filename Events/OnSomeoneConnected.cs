
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Collections.Generic;
using System.Linq;

namespace chatxx
{
    public static class OnSomeoneConnected
    {
        [FunctionName(nameof(OnSomeoneConnected))]
        public static dynamic Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "chat")] SignalRConnectionInfo connectionInfo,
            [CosmosDB(
                databaseName: "db",
                collectionName: "updates",
                ConnectionStringSetting = "CosmosDB",
                SqlQuery = "SELECT top 25 * FROM c order by c._ts desc")]
                IEnumerable<Message> messages,
        ILogger log)
        {

            var current = $"{req.Scheme}://{req.Host}{req.Path}";
            return new {
                connectionInfo,
                endPoints = new {
                    onImageReceived = current.Replace(nameof(OnSomeoneConnected), nameof(OnImageReceived)),
                    onMessageReceived = current.Replace(nameof(OnSomeoneConnected), nameof(OnMessageReceived))
                },
                messages = messages.Reverse()
            };
        }
    }
}
