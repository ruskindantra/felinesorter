using Newtonsoft.Json;

namespace FelineSorter.WebserviceContract
{
    public class Owner
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("age")]
        public long Age { get; set; }

        [JsonProperty("pets")]
        public Pet[] Pets { get; set; }
    }
}