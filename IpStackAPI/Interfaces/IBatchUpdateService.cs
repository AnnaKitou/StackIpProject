using IpStackAPI.DTOS;
using IpStackAPI.Entities;
using IpStackAPI.GenericRepository;
using IpStackAPI.RepositoryServices;
using static IpStackAPI.RepositoryServices.BatchUpdateService;

namespace IpStackAPI.Interfaces
{
    public interface IBatchUpdateService
    {
        //void  Enqueue(DetailsOfIpDTO update);
        //bool TryDequeue(out DetailsOfIpDTO update);


        void Enqueue(Guid batchUpdateId, DetailsOfIpDTO[] detailsOfIpDTO);
        bool TryDequeue(out BatchUpdateItem item);



        //public Task QueueUpdates(Guid batchUpdateId, DetailsOfIpDTO[] detailsOfIpDTO);
        //public Task<BatchUpdateStatus> GetUpdateStatus(Guid batchId);
        public Task ProcessUpdates(DetailsOfIpDTO detailsOfIpDTO);
    }
}
