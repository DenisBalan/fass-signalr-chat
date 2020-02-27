using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.Documents.Client;
using System.Collections.Generic;
using Microsoft.Azure.Documents;

namespace chatxx
{
    public static class PushMessageFromCosmos
    {
        [FunctionName(nameof(PushMessageFromCosmos))]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "db",
            collectionName: "updates",
            ConnectionStringSetting = "CosmosDB",
            LeaseCollectionName = nameof(PushMessageFromCosmos),
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Message> documents,
            [SignalR(HubName = "chat")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            var format = new SignalRMessage{
                Target = "newFile",
                Arguments = new Message[1]
            };
            SignalRMessage Create(Message msg) { format.Arguments[0] = msg; return format;}
            foreach(var msg in documents) 
            {
                if (msg.Tags.Length > 1){
                    await signalRMessages.AddAsync(Create(msg));
                }
            }
        }
    }
}
