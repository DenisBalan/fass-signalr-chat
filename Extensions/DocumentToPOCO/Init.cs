using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Hosting;
using Newtonsoft.Json;

[assembly: WebJobsStartup(typeof(chatxx.CosmosDBExtensionsWebJobsStartup))]
namespace chatxx
{
    public class CosmosDBExtensionsWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension<CosmosDBExtensionExtensionsConfigProvider>();
        }
    }
    [Extension("CosmosDBExtensions")]
    internal class CosmosDBExtensionExtensionsConfigProvider : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            context.AddOpenConverter<IReadOnlyList<Document>,
            IReadOnlyList<OpenType>>(typeof(GenericDocumentConverter<>));
        }
    }
    internal class GenericDocumentConverter<T> : IConverter<IReadOnlyList<Document>, IReadOnlyList<T>>
    {
        public IReadOnlyList<T> Convert(IReadOnlyList<Document> input)
        {
            List<T> output = new List<T>(input.Count);

            foreach (Document item in input)
            {
                output.Add(Convert(item));
            }

            return output.AsReadOnly();
        }

        private static T Convert(Document document)
        {
            return JsonConvert.DeserializeObject<T>(document.ToString());
        }
    }
}