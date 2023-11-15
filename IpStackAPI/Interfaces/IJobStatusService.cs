using IpStackAPI.Entities;

namespace IpStackAPI.Interfaces
{
    public interface IJobStatusService
    {
        Task<Guid> CreateJobAsync(JobParametersModel jobParameters);
        Task StoreJobResultAsync(Guid jobId, JobResultModel result, JobStatus jobStatus);
        Task UpdateJobProgressInformationAsync(Guid jobId, string value, double percentage);
        Task UpdateJobStatusAsync(Guid jobId, JobStatus jobStatus);
        Task<JobModel> GetJobAsync(Guid jobId);
        Task<IReadOnlyDictionary<Guid, JobModel>> GetAllJobsAsync();
        Task ClearAllJobsAsync();
    }
}
