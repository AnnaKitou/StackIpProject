using IpStackAPI.DTOS;
using IpStackAPI.Entities;
using IpStackAPI.GenericRepository;
using IpStackAPI.Interfaces;
using Microsoft.OpenApi.Extensions;
using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;
using static IpStackAPI.RepositoryServices.BatchUpdateService;

namespace IpStackAPI.RepositoryServices
{
    public class BatchUpdateService : IBatchUpdateService
    {
        // private static ConcurrentQueue<DetailsOfIpDTO> _updatesQueue = new ConcurrentQueue<DetailsOfIpDTO>();

        private readonly IGenericRepository<DetailsOfIp> _stackIpRepo;
        private readonly BufferBlock<BatchUpdateItem> _buffer = new BufferBlock<BatchUpdateItem>();
        private readonly Dictionary<Guid, BatchUpdateStatus> _statusMap = new Dictionary<Guid, BatchUpdateStatus>();
        private const int BatchSize = 10;

        private ConcurrentQueue<Func<CancellationToken, BatchUpdateItem>> _workItems = new ConcurrentQueue<Func<CancellationToken, BatchUpdateItem>>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);


        public BatchUpdateService(IGenericRepository<DetailsOfIp> stackIpRepo /*IStackIpService service*/)
        {
            _stackIpRepo = stackIpRepo;

        }

        public async Task Enqueue(Guid batchId, DetailsOfIpDTO[] updates)
        {
            // Break updates into batches of 10
            var batches = updates.Select((value, index) => new { value, index })
                                 .GroupBy(x => x.index / BatchSize)
                                 .Select(g => g.Select(x => x.value).ToArray());

            foreach (var batch in batches)
            {
                var batchUpdateItem = new BatchUpdateItem(batchId, batch);
                _buffer.Post(batchUpdateItem);
            }

            _statusMap[batchId] = BatchUpdateStatus.Processing;
        }

        public async Task<BatchUpdateItem?> TryDequeue()
        {
            if (_buffer.TryReceive(out var batchUpdateItem))
            {
                try
                {
                    foreach (var itemDetails in batchUpdateItem.DetailsForUpdate)
                    {
                        var ipDetailsEntity = await _stackIpRepo.GetDetailsOfIp(itemDetails.Ip);

                        ipDetailsEntity.Ip = ipDetailsEntity.Ip;
                        ipDetailsEntity.Longitude = itemDetails.Longitude;
                        ipDetailsEntity.Latitude = itemDetails.Latitude;
                        ipDetailsEntity.Country = itemDetails.Country;
                        ipDetailsEntity.City = itemDetails.City;

                        await _stackIpRepo.UpdateDetail(ipDetailsEntity);
                    }

                    _statusMap[batchUpdateItem.BatchId] = BatchUpdateStatus.Completed;
                }
                catch (Exception ex)
                {
                    // Handle exceptions, log errors, etc.
                }

                return batchUpdateItem;
            }

            return null;
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

        public async Task<BatchUpdateStatus> GetUpdateStatus(Guid batchId)
        {
            return _statusMap.GetValueOrDefault(batchId, BatchUpdateStatus.Processing);
        }


    }

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
        [Display(Name = "Queued")]
        Queued,

        [Display(Name = "Processing")]
        Processing,

        [Display(Name = "Completed")]
        Completed,

        [Display(Name = "Failed")]
        Failed,

        [Display(Name = "Unknown")]
        Unknown
    }
}

