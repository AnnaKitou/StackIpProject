using IpStackAPI.Entities;

namespace IpStackAPI.GenericRepository
{
    public interface IGenericRepository<T>where T :class
    {
        public Task<T?> GetDetailsOfIp(string ip);
        public bool AddDetail(T detailsOfIp);
        public Task<Guid> UpdateDetail(T detailsOfIps);
    }
}
