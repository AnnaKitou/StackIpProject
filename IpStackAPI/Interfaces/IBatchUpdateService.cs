using IpStackAPI.DTOS;
using IpStackAPI.Entities;
using IpStackAPI.GenericRepository;
using IpStackAPI.RepositoryServices;
using static IpStackAPI.RepositoryServices.BatchUpdateService;

namespace IpStackAPI.Interfaces
{
    public interface IBatchUpdateService
    {

        Task Enqueue(Guid batchId, DetailsOfIpDTO[] updates);
        Task<BatchUpdateItem> TryDequeue(BatchUpdateItem batchUpdateItem);
        public Task<BatchUpdateStatus> GetUpdateStatus(Guid batchId);
        public Task ProcessUpdates(DetailsOfIpDTO detailsOfIpDTO);
      
    }
}
