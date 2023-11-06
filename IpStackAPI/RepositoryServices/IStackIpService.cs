using IpStackAPI.Entities;

namespace IpStackAPI.RepositoryServices
{
    public interface IStackIpService
    {
        public Task<DetailsOfIp> GetDetailsOfIp(string ip);

        public bool AddDetail(DetailsOfIp detailsOfIp);
    }
}
