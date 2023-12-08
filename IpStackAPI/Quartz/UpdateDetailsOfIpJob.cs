using IpStackAPI.DTOS;
using IpStackAPI.Entities;
using IpStackAPI.GenericRepository;
using IpStackAPI.Interfaces;
using Newtonsoft.Json.Linq;
using Quartz;

namespace IpStackAPI.Quartz
{
    public class UpdateDetailsOfIpJob : IJob
    {
        private readonly IGenericRepository<DetailsOfIp> _detailsOfIpRepository;
        private readonly IBatchUpdateService _batchUpdateService;
        public UpdateDetailsOfIpJob(IGenericRepository<DetailsOfIp> detailsOfIpRepository, IBatchUpdateService batchUpdateService)
        {
            _detailsOfIpRepository = detailsOfIpRepository;
            _batchUpdateService = batchUpdateService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // Get the job ID (GUID)
            var jobId = context.JobDetail.Key.Name;

            // Get the job data map to retrieve parameters
            var jobDataMap = context.JobDetail.JobDataMap;
            var detailsOfIpDTO = jobDataMap.Get("DetailsOfIpDTO") as DetailsOfIpDTO[];

            await _batchUpdateService.Enqueue(new Guid(jobId), detailsOfIpDTO);
      
            // Your logic to update DetailsOfIp
            // This is where you would call for each item

            //foreach (var detail in detailsOfIpDTO)
            //{
            //    var detailOfIp = await _detailsOfIpRepository.GetDetailsOfIp(detail.Ip);

            //    detailOfIp.Ip = detailOfIp.Ip;
            //    detailOfIp.Longitude = detail.Longitude;
            //    detailOfIp.Latitude = detail.Latitude;
            //    detailOfIp.Country = detail.Country;
            //    detailOfIp.City = detail.City;

            //    await _detailsOfIpRepository.UpdateDetail(detailOfIp);
            //}

            // Log or perform any other necessary actions

            // Example: Logging the completion of the job
            // Log.Information($"Job {jobId} completed successfully.");


        }

    
    }
}