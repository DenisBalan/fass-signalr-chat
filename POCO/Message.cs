using System;
using Newtonsoft.Json;

namespace chatxx
{
    public class Message
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
        [JsonProperty(PropertyName = "tags")]
        public string[] Tags { get; set; } = new string[0];
        [JsonProperty(PropertyName = "when")]
        public DateTime When { get; set; } = DateTime.Now;
    }
}