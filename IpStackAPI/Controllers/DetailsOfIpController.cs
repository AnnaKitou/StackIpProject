using IpStackAPI.Context;
using IpStackAPI.Entities;
using IpStackAPI.RepositoryServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StackIpProject.Interfaces;

namespace IpStackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailsOfIpController : ControllerBase
    {
        private readonly IStackIpService _stackIpService;
        private readonly IMemoryCache _cache;
        private readonly IIPInfoProvider _provider;



        public DetailsOfIpController(IStackIpService stackIpService, IMemoryCache memoryCache, ApplicationDbContext context, IIPInfoProvider provider)
        {
            _stackIpService = stackIpService;
            _cache = memoryCache;
            _provider = provider;
        }

        [HttpGet]
        public async Task<ActionResult<DetailsOfIp>> Get(string ip)
        {

            if (!_cache.TryGetValue("ipCacheKey", out DetailsOfIp? detailsOfIp))
            {
                detailsOfIp = await _stackIpService.GetDetailsOfIp(ip);
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
                            _stackIpService.AddDetail(result);
                           
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
}
