using IpStackAPI.DTOS;
using IpStackAPI.Entities;
using IpStackAPI.GenericRepository;
using IpStackAPI.Interfaces;
using System.Collections.Concurrent;

namespace IpStackAPI.RepositoryServices
{
    public class BatchUpdateService: IBatchUpdateService
    {
        private readonly ConcurrentQueue<BatchUpdateItem> _queue = new ConcurrentQueue<BatchUpdateItem>();
        private readonly Dictionary<Guid, BatchUpdateStatus> _statusMap = new Dictionary<Guid, BatchUpdateStatus>();
        private readonly IGenericRepository<DetailsOfIp> _stackIpRepo;

        public BatchUpdateService(IGenericRepository<DetailsOfIp> stackIpRepo)
        {
            _stackIpRepo = stackIpRepo;
        }

        public async Task QueueUpdates(Guid batchId, DetailsOfIpDTO[] updates)
        {
            foreach (var update in updates)
            {
                _queue.Enqueue(new BatchUpdateItem(batchId, update));
            }
            _statusMap[batchId] = BatchUpdateStatus.Queued;
            return;
        }

        // Method to be called by a background service to process updates
        public void ProcessUpdates()
        {
            while (_queue.TryDequeue(out BatchUpdateItem item))
            {
                try
                {
                   // // Process the update
                   //_stackIpRepo.UpdateDetail(item.UpdateDetails);

                    // Update the individual item status (if needed)
                    // Update the batch status
                    _statusMap[item.BatchId] = BatchUpdateStatus.Processing;
                }
                catch (Exception ex)
                {
                    // Handle exceptions, log errors, etc.
                    _statusMap[item.BatchId] = BatchUpdateStatus.Failed;
                }
            }
            // Optionally update the batch status to Completed outside the loop
        }

        public async Task<BatchUpdateStatus> GetUpdateStatus(Guid batchId)
        {
            return _statusMap.GetValueOrDefault(batchId, BatchUpdateStatus.Unknown);
        }

       
    }

    public class BatchUpdateItem
    {
        public Guid BatchId { get; }
        public DetailsOfIpDTO UpdateDetails { get; }

        public BatchUpdateItem(Guid batchId, DetailsOfIpDTO updateDetails)
        {
            BatchId = batchId;
            UpdateDetails = updateDetails;
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
