using IpStackAPI.Context;
using IpStackAPI.DTOS;
using IpStackAPI.Entities;
using IpStackAPI.GenericRepository;
using IpStackAPI.RepositoryServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StackIpProject.Interfaces;
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



        public DetailsOfIpController(IMemoryCache memoryCache, ApplicationDbContext context, IIPInfoProvider provider, IGenericRepository<DetailsOfIp> stackIpRepo)
        {

            _cache = memoryCache;
            _provider = provider;
            _stackIpRepo = stackIpRepo;
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
        public async Task<IActionResult> UpdateApiDetails(DetailsOfIpDTO detailsOfIpDTO, string ip)
        {
            var ipDetailsEntity = await _stackIpRepo.GetDetailsOfIp(ip);
            if (ipDetailsEntity == null)
            {
                return NotFound();
            }

            var finalIp = new DetailsOfIp();
            finalIp.Ip = ip;
            finalIp.Longitude = detailsOfIpDTO.Longitude;
            finalIp.Latitude = detailsOfIpDTO.Latitude;
            finalIp.Country = detailsOfIpDTO.Country;
            finalIp.City = detailsOfIpDTO.City;

            _stackIpRepo.AddDetail(finalIp);
            return Ok(finalIp);
        }

    }
}
