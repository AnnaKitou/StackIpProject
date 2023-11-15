using IpStackAPI.Context;
using IpStackAPI.Entities;
using IpStackAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IpStackAPI.RepositoryServices
{
    public class StackIpService : IStackIpService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public StackIpService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<DetailsOfIp?> GetDetailsOfIp(string ip)
        {

            var result = await _applicationDbContext.DetailsOfIp.Where(x => x.Ip == ip).FirstOrDefaultAsync();
            return result;
        }
        public bool AddDetail(DetailsOfIp detailsOfIp)
        {
            _applicationDbContext.Add(detailsOfIp);
            return (_applicationDbContext.SaveChanges() >= 0);
        }

    }
}
