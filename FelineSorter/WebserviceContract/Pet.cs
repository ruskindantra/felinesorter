using Newtonsoft.Json;

namespace FelineSorter.WebserviceContract
{
    public class Pet
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}