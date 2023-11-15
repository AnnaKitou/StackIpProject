using IpStackAPI.DTOS;
using IpStackAPI.Entities;
using IpStackAPI.GenericRepository;
using IpStackAPI.Interfaces;
using System.Collections.Concurrent;
using static IpStackAPI.RepositoryServices.QueuedBackgroundService;

namespace IpStackAPI.RepositoryServices
{
    public partial class QueuedBackgroundService : BackgroundService, IQueuedBackgroundService
    {


        private sealed class JobQueueItem
        {
            public Guid JobId { get; set; }
            public DetailsOfIpDTO detailsOfIpDTO { get; set; }
        }

        private readonly IComputationWorkService _workService;
        private readonly IJobStatusService _jobStatusService;

        // Shared between BackgroundService and IQueuedBackgroundService.
        // The queueing mechanism could be moved out to a singleton service. I am doing
        // it this way for simplicity's sake.
        private static readonly ConcurrentQueue<JobQueueItem> _queue =
            new ConcurrentQueue<JobQueueItem>();
        private static readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public QueuedBackgroundService(IComputationWorkService workService,
            IJobStatusService jobStatusService)
        {
            _workService = workService;
            _jobStatusService = jobStatusService;
        }

        /// <summary>
        /// Transient method via IQueuedBackgroundService
        /// </summary>
        public async Task<JobCreateModel> PostWorkItemAsync(JobParametersModel jobParameters)
        {
             jobId = await _jobStatusService.CreateJobAsync(jobParameters).ConfigureAwait(false);
            _queue.Enqueue(new JobQueueItem { JobId = jobId, JobParameters = jobParameters });
            _signal.Release(); // signal for background service to start working on the job
            return new JobCreateModel { JobId = jobId, QueuePosition = _queue.Count };
        }

        /// <summary>
        /// Long running task via BackgroundService
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                JobQueueItem jobQueueItem = null;
                try
                {
                    // wait for the queue to signal there is something that needs to be done
                    await _signal.WaitAsync(stoppingToken).ConfigureAwait(false);

                    // dequeue the item
                    jobQueueItem = _queue.TryDequeue(out var workItem) ? workItem : null;

                    if (jobQueueItem != null)
                    {
                        // put the job in to a "processing" state
                        await _jobStatusService.UpdateJobStatusAsync(
                            jobQueueItem.JobId, JobStatus.Processing).ConfigureAwait(false);

                        // the heavy lifting is done here...
                        var result = await _workService.DoWorkAsync(
                            jobQueueItem.JobId, jobQueueItem.JobParameters,
                            stoppingToken).ConfigureAwait(false);

                        // store the result of the work and set the status to "finished"
                        await _jobStatusService.StoreJobResultAsync(
                            jobQueueItem.JobId, result, JobStatus.Success).ConfigureAwait(false);
                    }
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    try
                    {
                        // something went wrong. Put the job in to an errored state and continue on
                        await _jobStatusService.StoreJobResultAsync(jobQueueItem.JobId, new JobResultModel
                        {
                            Exception = new JobExceptionModel(ex)
                        }, JobStatus.Errored).ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        // TODO: log this
                    }
                }
            }
        }
    }

}
