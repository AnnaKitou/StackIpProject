using IpStackAPI.Entities;
using IpStackAPI.Interfaces;
using static IpStackAPI.RepositoryServices.JodStatusService;

namespace IpStackAPI.RepositoryServices
{
    public class JodStatusService
    {
        public sealed class ComputationJobStatusService : IJobStatusService
        {
            private const string DatabaseKey = "_ComputationJobStatus";
            private readonly IJobStorageService _jobStorage;

            public ComputationJobStatusService(IJobStorageService jobStorageService)
            {
                _jobStorage = jobStorageService;
            }

            public async Task<string> CreateJobAsync(JobParametersModel jobParameters)
            {
                // find a jobId that isn't in use (this may be pointless, but could you imagine?)
                string jobId;
                do
                {
                    jobId = Guid.NewGuid().ToString();
                } while (await _jobStorage.ExistsAsync(DatabaseKey, jobId).ConfigureAwait(false));

                await WriteAsync(new JobModel
                {
                    JobId = jobId,
                    StartTime = DateTime.UtcNow,
                    Status = JobStatus.Pending,
                    WorkParameters = jobParameters
                }).ConfigureAwait(false);

                return jobId;
            }

            public async Task<JobModel> GetJobAsync(string jobId)
            {
                if (!await _jobStorage.ExistsAsync(DatabaseKey, jobId).ConfigureAwait(false))
                {
                    return null;
                }
                return await _jobStorage.ReadAsync(DatabaseKey, jobId).ConfigureAwait(false);
            }

            public Task<IReadOnlyDictionary<string, JobModel>> GetAllJobsAsync()
            {
                return _jobStorage.GetAllAsync(DatabaseKey);
            }

            public async Task UpdateJobStatusAsync(string jobId, JobStatus jobStatus)
            {
                var job = await _jobStorage.ReadAsync(DatabaseKey, jobId).ConfigureAwait(false);
                job.Status = jobStatus;
                await WriteAsync(job).ConfigureAwait(false);
            }

            public async Task StoreJobResultAsync(string jobId, JobResultModel result, JobStatus jobStatus)
            {
                var job = await _jobStorage.ReadAsync(DatabaseKey, jobId).ConfigureAwait(false);
                job.Status = jobStatus;
                job.JobResult = result;
                await WriteAsync(job).ConfigureAwait(false);
            }

            public async Task UpdateJobProgressInformationAsync(string jobId, string value, double percentage)
            {
                var job = await _jobStorage.ReadAsync(DatabaseKey, jobId).ConfigureAwait(false);
                job.ProgressInformation = value;
                job.ProgressPercentage = percentage;
                await WriteAsync(job).ConfigureAwait(false);
            }

            private Task WriteAsync(JobModel job)
            {
                job.LastUpdate = DateTime.UtcNow;
                return _jobStorage.WriteAsync(DatabaseKey, job);
            }

            public Task ClearAllJobsAsync()
            {
                return _jobStorage.ClearAllAsync(DatabaseKey);
            }
        }
    }
}
