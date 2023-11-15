using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;

namespace IpStackAPI.Entities
{

    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum JobStatus
    {
        Pending,
        Processing,
        Success,
        Errored
    }

    public sealed class JobModel
    {
        [JsonProperty("jobId")]
        public string JobId { get; set; }

        [JsonProperty("workParameters")]
        public JobParametersModel WorkParameters { get; set; }

        [JsonProperty("jobResult")]
        public JobResultModel JobResult { get; set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        [JsonProperty("lastUpdate")]
        public DateTime LastUpdate { get; set; }

        [JsonProperty("progressInformation")]
        public string ProgressInformation { get; set; }

        [JsonProperty("progressPercentage")]
        public double ProgressPercentage { get; set; }

        [JsonProperty("status")]
        public JobStatus Status { get; set; }
    }

}
