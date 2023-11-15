using Newtonsoft.Json;

namespace IpStackAPI.Entities
{
    public class JobParametersModel
    {
        [JsonProperty("iterations")]
        public Guid guid { get; set; }

        [JsonProperty("seedData")]
        public uint SeedData { get; set; }
    }
}
