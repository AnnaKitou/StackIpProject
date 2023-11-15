using Newtonsoft.Json;

namespace IpStackAPI.Entities
{
    public class JobCreateModel
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }

        [JsonProperty("queuePosition")]
        public int QueuePosition { get; set; }
    }
}
