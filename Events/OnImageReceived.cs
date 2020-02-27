using System;
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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;


namespace chatxx
{
    public class OnImageReceived
    {
        [FunctionName(nameof(OnImageReceived))]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [Queue("queue-chatxx-images")] IAsyncCollector<Message> queue,
            [Blob("storage-chatxx/images")] CloudBlobDirectory container
            
            ,[SignalR(HubName = "chat")] IAsyncCollector<SignalRMessage> signalRMessages
            )
        {
            var form = await req.ReadFormAsync();
            var name = form[nameof(Message.Name).ToLowerInvariant()].Single();
            var file = form.Files.Single();  

            var message = new Message{
                Name = "<i>system</i>",
                Content = $"<i>{name} uploaded {file.FileName} {file.Length} bytes, processing..</i>"
            };
            var format = new SignalRMessage{
                Target = "newMessage",
                Arguments = new Message[1]{message}
            };
            
            await signalRMessages.AddAsync(format);
          

            var pref = DateTime.Now.Ticks.ToString("X16");
            var xx = container.GetAppendBlobReference($"{pref}_{file.FileName}");
            await xx.CreateOrReplaceAsync();

            await xx.AppendFromStreamAsync(file.OpenReadStream());
            await queue.AddAsync(new Message
            {
                Name = name,
                Tags = new[] { 
                    nameof(file), xx.Uri.AbsoluteUri, 
                    nameof(file.ContentType), file.ContentType,
                    nameof(file.Length), $"{file.Length} bytes",
                }
            });
            
        }

    }
}