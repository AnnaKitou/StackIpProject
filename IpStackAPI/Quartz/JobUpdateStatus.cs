using IpStackAPI.Entities;
using IpStackAPI.GenericRepository;
using IpStackAPI.Interfaces;
using Quartz;
using System;

namespace IpStackAPI.Quartz
{
    public class JobUpdateStatus : IJob
    {
        private readonly IGenericRepository<DetailsOfIp> _detailsOfIpRepository;
        private readonly IBatchUpdateService _batchUpdateService;
        private readonly ISchedulerFactory _schedulerFactory;

        public JobUpdateStatus(IGenericRepository<DetailsOfIp> detailsOfIpRepository, IBatchUpdateService batchUpdateService, ISchedulerFactory schedulerFactory)
        {
            _detailsOfIpRepository = detailsOfIpRepository;
            _batchUpdateService = batchUpdateService;
            _schedulerFactory = schedulerFactory;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            
            var scheduler = await _schedulerFactory.GetScheduler();
          

            await _batchUpdateService.TryDequeue();
            //var jobDataMap = JobDetail.JobDataMap;
            //var detailsOfIpDTO = jobDataMap.Get("DetailsOfIpDTO") as DetailsOfIpDTO[];
        }
    }
}
