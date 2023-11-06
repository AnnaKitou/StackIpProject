using IpStackAPI.Context;
using IpStackAPI.RepositoryServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IpStackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailsOfIpController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IStackIpService _stackIpService;

        public DetailsOfIpController(ApplicationDbContext applicationDbContext, IStackIpService stackIpService)
        {
            _applicationDbContext = applicationDbContext;
            _stackIpService = stackIpService;
        }
    }
}
