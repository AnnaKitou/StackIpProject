using IpStackAPI.DTOS;
using IpStackAPI.Entities;
using IpStackAPI.GenericRepository;
using Quartz;

namespace IpStackAPI.Quartz
{
    public class UpdateDetailsOfIpJob : IJob
    {
        private readonly IGenericRepository<DetailsOfIp> _detailsOfIpRepository; // Replace with your repository

        public UpdateDetailsOfIpJob(IGenericRepository<DetailsOfIp> detailsOfIpRepository)
        {
            _detailsOfIpRepository = detailsOfIpRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // Get the job ID (GUID)
            var jobId = context.JobDetail.Key.Name;

            // Get the job data map to retrieve parameters
            var jobDataMap = context.JobDetail.JobDataMap;
            var detailsOfIpDTO = jobDataMap.Get("DetailsOfIpDTO") as DetailsOfIpDTO[];

            // Your logic to update DetailsOfIp
            // This is where you would call  for each item

            foreach (var detail in detailsOfIpDTO)
            {
                var detailOfIp = await _detailsOfIpRepository.GetDetailsOfIp(detail.Ip);
                await _detailsOfIpRepository.UpdateDetail(detailOfIp);
            }

            // Log or perform any other necessary actions

            // Example: Logging the completion of the job
            // Log.Information($"Job {jobId} completed successfully.");
        }
    }
}