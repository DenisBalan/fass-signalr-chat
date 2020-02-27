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
    public class ImageProcessor
    {
        [FunctionName(nameof(ImageProcessor))]
        public void Run(
            [QueueTrigger("queue-chatxx-images")] Message message,
            [CosmosDB("db", "updates", Id = "id", ConnectionStringSetting = "CosmosDB")]
            out Message document
            )
        {
            message.Content = $"<img src=\"{message.Tags[1]}\" width=\"150px\" />";
            document = message;
        }

    }
}