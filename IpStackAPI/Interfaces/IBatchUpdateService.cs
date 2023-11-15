using IpStackAPI.DTOS;
using IpStackAPI.Entities;
using IpStackAPI.GenericRepository;
using IpStackAPI.RepositoryServices;

namespace IpStackAPI.Interfaces
{
    public interface IBatchUpdateService
    {
        public Task QueueUpdates(Guid batchUpdateId, DetailsOfIpDTO[] detailsOfIpDTO);
        public Task<BatchUpdateStatus> GetUpdateStatus(Guid batchId);
        public void ProcessUpdates();
    }
}
