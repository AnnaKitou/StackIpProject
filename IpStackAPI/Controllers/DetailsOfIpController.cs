using IpStackAPI.Context;
using IpStackAPI.DTOS;
using IpStackAPI.Entities;
using IpStackAPI.FactoryPattern;
using IpStackAPI.GenericRepository;
using IpStackAPI.Interfaces;
using IpStackAPI.Quartz;
using IpStackAPI.RepositoryServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using Quartz;
using StackIpProject.Interfaces;
using StackIpProject.Model;
using System.Net;
using System.Net.WebSockets;
namespace IpStackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailsOfIpController : ControllerBase
    {
        //private readonly IStackIpService _stackIpService;
        private readonly IGenericRepository<DetailsOfIp> _stackIpRepo;
        private readonly IMemoryCache _cache;
        private readonly IIPInfoProvider _provider;
        private readonly IBatchUpdateService _batchUpdateService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IBatchUpdateServiceFactory _serviceProvider;
        private readonly ISchedulerFactory _schedulerFactory;

        public DetailsOfIpController(IMemoryCache memoryCache, ApplicationDbContext context, IIPInfoProvider provider, IGenericRepository<DetailsOfIp> stackIpRepo, IBatchUpdateService batchUpdateService, IServiceScopeFactory serviceScopeFactory, IBatchUpdateServiceFactory serviceProvider, ISchedulerFactory scheduler)
        {

            _cache = memoryCache;
            _provider = provider;
            _stackIpRepo = stackIpRepo;
            _batchUpdateService = batchUpdateService;
            _serviceScopeFactory = serviceScopeFactory;
            _serviceProvider = serviceProvider;
            _schedulerFactory = scheduler;
        }

        [HttpGet]
        public async Task<ActionResult<DetailsOfIp>> Get(string ip)
        {
            IPAddress address;
            bool isValid = IPAddress.TryParse(ip, out address);
            if (!isValid)
            {
                throw new Exception("The IP Address supplied is invalid.");
            }
            else
            {
                if (!_cache.TryGetValue("ipCacheKey", out DetailsOfIp? detailsOfIp))
                {
                    //detailsOfIp = await _stackIpService.GetDetailsOfIp(ip);
                    detailsOfIp = await _stackIpRepo.GetDetailsOfIp(ip);

                    if (detailsOfIp != null)
                    {
                        _cache.Set("ipCacheKey", detailsOfIp, TimeSpan.FromMinutes(1));
                        return detailsOfIp;
                    }
                    else
                    {
                        try
                        {
                            var iPDetails = await _provider.GetIPDetailsAsync(ip);
                            if (iPDetails != null)
                            {
                                _cache.Set("ipCacheKey", iPDetails, TimeSpan.FromMinutes(1));
                                var result = new DetailsOfIp()
                                {
                                    Ip = ip,
                                    City = iPDetails.City,
                                    Country = iPDetails.Country,
                                    Latitude = iPDetails.Latitude,
                                    Longitude = iPDetails.Longitude,
                                };
                                //_stackIpService.AddDetail(result);
                                _stackIpRepo.AddDetail(result);
                                return Ok(result);
                            }
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                        }

                    }
                }
                return Ok(detailsOfIp);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(DetailsOfIpDTO))]
        public async Task<IActionResult> UpdateApiDetails([FromBody] DetailsOfIpDTO[] detailsOfIpDTO)
        {
            var jobId = Guid.NewGuid().ToString();

            // Schedule Quartz job
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.Start();

            var jobDataMap = new JobDataMap();
            jobDataMap.Put("DetailsOfIpDTO", detailsOfIpDTO);

            var job = JobBuilder.Create<UpdateDetailsOfIpJob>()
                .WithIdentity(jobId)
                .UsingJobData(jobDataMap)
                .StoreDurably(true)
                .RequestRecovery(true)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{jobId}_trigger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            // Return the job ID
            return Ok(jobId);



            //var batchUpdateId = Guid.NewGuid();
            //await _batchUpdateService.Enqueue(batchUpdateId, detailsOfIpDTO);


            //return Ok(batchUpdateId);

            #region Testing Purposes
            // Return the unique identifier


            //// if (ipDetailsEntity == null)
            //// {
            ////     return NotFound();
            //// }
            //DetailsOfIp[] detailsOf = new DetailsOfIp[] { };

            //    foreach (var detailsOfIp in detailsOfIpDTO)
            //    {

            //        var ipDetailsEntity = await _stackIpRepo.GetDetailsOfIp(detailsOfIp.Ip);


            //    ipDetailsEntity.Ip = ipDetailsEntity.Ip;
            //    ipDetailsEntity.Longitude = detailsOfIp.Longitude;
            //    ipDetailsEntity.Latitude = detailsOfIp.Latitude;
            //    ipDetailsEntity.Country = detailsOfIp.Country;
            //    ipDetailsEntity.City = detailsOfIp.City;

            //        await _stackIpRepo.UpdateDetail(ipDetailsEntity);

            //    //_batchUpdateService.ProcessUpdates();

            //}
            // await _batchUpdateService.QueueUpdates(batchUpdateId, detailsOfIpDTO); 
            #endregion


        }

        [HttpGet]
        [Route("CheckStatus")]
        public async Task<ActionResult<BatchUpdateStatus>> GetUpdateStatus(Guid guid)
        {

           

            // Schedule Quartz job
            var scheduler = await _schedulerFactory.GetScheduler();
            JobKey jobKey = new JobKey(guid.ToString());

            IJobDetail jobDetail = await scheduler.GetJobDetail(jobKey);
         var a=   jobDetail.JobDataMap.Values.ToList();

            _batchUpdateService.TryDequeue(a);
            //await scheduler.Start();

            //var jobDataMap = new JobDataMap();
            //jobDataMap.Put("JobUpdateStatus", guid);

            //var job = JobBuilder.Create<JobUpdateStatus>()
            //    .WithIdentity(guid.ToString())
            //    .UsingJobData(jobDataMap)
            //    .Build();

            //var trigger = TriggerBuilder.Create()
            //    .WithIdentity($"{guid}_trigger")
            //    .StartNow()
            //    .Build();

            //await scheduler.ScheduleJob(job, trigger);
            //foreach (var j in jobs)
            //{
            //    Console.WriteLine("Progress of {0} is {1}",
            //        j.JobDetail.Key,
            //        j.JobDetail.JobDataMap["progress"]);
            //}

            return await _batchUpdateService.GetUpdateStatus(guid);
        }

    }
}
