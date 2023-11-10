using IpStackAPI.Context;
using IpStackAPI.Entities;
using IpStackAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IpStackAPI.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IHasIpProperty
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private DbSet<T> table;
        public GenericRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            table = _applicationDbContext.Set<T>();
        }

        public async Task<T?> GetDetailsOfIp(string ip)
        {
            var result = await table.FirstOrDefaultAsync(entity => entity.Ip == ip);
            return result;
        }
        public bool AddDetail(T detailsOfIp)
        {
            if (detailsOfIp == null)
            {
                throw new ArgumentNullException("entity");
            }
            _applicationDbContext.Add(detailsOfIp);
            return (_applicationDbContext.SaveChanges() >= 0);
        }



        public async Task<Guid> UpdateDetail(T detailsOfIps)
        {
           
            if (detailsOfIps != null)
            {
                table.Update(detailsOfIps);
               await _applicationDbContext.SaveChangesAsync();
            }


            return Guid.Empty;
        }


    }
}
