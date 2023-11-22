using IpStackAPI.DTOS;
using IpStackAPI.Entities;
using IpStackAPI.GenericRepository;
using IpStackAPI.Interfaces;
using System.Collections.Concurrent;

namespace IpStackAPI.RepositoryServices
{
    public class BatchUpdateService : IBatchUpdateService
    {
        // private static ConcurrentQueue<DetailsOfIpDTO> _updatesQueue = new ConcurrentQueue<DetailsOfIpDTO>();

        private readonly ConcurrentQueue<BatchUpdateItem> _queue = new ConcurrentQueue<BatchUpdateItem>();
        private readonly Dictionary<Guid, BatchUpdateStatus> _statusMap = new Dictionary<Guid, BatchUpdateStatus>();
        private readonly IGenericRepository<DetailsOfIp> _stackIpRepo;

        public BatchUpdateService(IGenericRepository<DetailsOfIp> stackIpRepo /*IStackIpService service*/)
        {
            _stackIpRepo = stackIpRepo;
            //_service = service;
        }

        //public void Enqueue(DetailsOfIpDTO update)
        //{
        //    _updatesQueue.Enqueue(update);
        //}

        //public bool TryDequeue(out DetailsOfIpDTO update)
        //{
        //    return _updatesQueue.TryDequeue(out update);
        //}


       
        //    
        //    private readonly IGenericRepository<DetailsOfIp> _stackIpRepo;
        //    private readonly IStackIpService _service;

        //    public BatchUpdateService(IGenericRepository<DetailsOfIp> stackIpRepo, IStackIpService service)
        //    {
        //        _stackIpRepo = stackIpRepo;
        //        _service = service;
        //    }

        public void Enqueue(Guid batchId, DetailsOfIpDTO[] updates)
        {

            _queue.Enqueue(new BatchUpdateItem(batchId, updates));

            _statusMap[batchId] = BatchUpdateStatus.Queued;
            //return;
        }

        public bool TryDequeue(out BatchUpdateItem item)
        {
            Console.WriteLine($"Items in queue: {_queue.Count}");
            return _queue.TryDequeue(out item);
        }


        //    // Method to be called by a background service to process updates
        public async Task ProcessUpdates(DetailsOfIpDTO detailsOfIpDTO)
        {
            //while (_queue.TryDequeue(out BatchUpdateItem item))
            //{
            //    try
            //    {
            //await Console.Out.WriteLineAsync("alalala");
            //foreach (var itemDetails in item.DetailsForUpdate)
            //{
            //    var test = itemDetails;

            var ipDetailsEntity = await _stackIpRepo.GetDetailsOfIp(detailsOfIpDTO.Ip);


            ipDetailsEntity.Ip = ipDetailsEntity.Ip;
            ipDetailsEntity.Longitude = detailsOfIpDTO.Longitude;
            ipDetailsEntity.Latitude = detailsOfIpDTO.Latitude;
            ipDetailsEntity.Country = detailsOfIpDTO.Country;
            ipDetailsEntity.City = detailsOfIpDTO.City;

            await _stackIpRepo.UpdateDetail(ipDetailsEntity);
            //}
            //        // // Process the update


            //        // Update the individual item status (if needed)
            //        // Update the batch status
            //        _statusMap[item.BatchId] = BatchUpdateStatus.Processing;
            //    }
            //    catch (Exception ex)
            //    {
            //        // Handle exceptions, log errors, etc.
            //        _statusMap[item.BatchId] = BatchUpdateStatus.Failed;
            //    }
            //}
            // Optionally update the batch status to Completed outside the loop
        }

        //    public async Task<BatchUpdateStatus> GetUpdateStatus(Guid batchId)
        //    {
        //        return _statusMap.GetValueOrDefault(batchId, BatchUpdateStatus.Unknown);
        //    }


        //}

        public class BatchUpdateItem
        {
            public Guid BatchId { get; }
            public DetailsOfIpDTO[] DetailsForUpdate { get; }

            public BatchUpdateItem(Guid batchId, DetailsOfIpDTO[] updateDetails)
            {
                BatchId = batchId;
                DetailsForUpdate = updateDetails;
            }
        }

        public enum BatchUpdateStatus
        {
            Queued,
            Processing,
            Completed,
            Failed,
            Unknown
        }
    }
}
