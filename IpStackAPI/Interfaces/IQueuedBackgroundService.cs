using IpStackAPI.Entities;

namespace IpStackAPI.RepositoryServices
{
    public partial class QueuedBackgroundService
    {
        public interface IQueuedBackgroundService
        {
            Task<JobCreateModel> PostWorkItemAsync(JobParametersModel jobParameters);
        }
    }
}
