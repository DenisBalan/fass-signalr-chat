using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Logging;

namespace chatxx
{
    public class OnMessageReceived
    {
        [FunctionName(nameof(OnMessageReceived))]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] Message message,
            // [CosmosDB("db", "updates", Id = "id", ConnectionStringSetting = "CosmosDB")]
            // out dynamic document,
            [SignalR(HubName = "chat")] IAsyncCollector<SignalRMessage> signalRMessages,

            [Queue("queue-chatxx")] IAsyncCollector<Message> queue
            )
        {
            var format = new SignalRMessage{
                Target = "newMessage",
                Arguments = new Message[1]{message}
            };
            
            await signalRMessages.AddAsync(format);
            
            await queue.AddAsync(message);
        }

    }
}