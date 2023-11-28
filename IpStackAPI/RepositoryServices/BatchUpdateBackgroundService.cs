using IpStackAPI.DTOS;
using IpStackAPI.Entities;
using IpStackAPI.FactoryPattern;
using IpStackAPI.Interfaces;
using static IpStackAPI.RepositoryServices.BatchUpdateService;

namespace IpStackAPI.RepositoryServices
{
    public class BatchUpdateBackgroundService : BackgroundService
    {
        private readonly IBatchUpdateServiceFactory _batchUpdateServiceFactory;

        public BatchUpdateBackgroundService(IBatchUpdateServiceFactory batchUpdateServiceFactory)
        {
            _batchUpdateServiceFactory = batchUpdateServiceFactory;

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var batchUpdateService = _batchUpdateServiceFactory.Create();
                ////while (batchUpdateService.TryDequeue(out DetailsOfIpDTO update))
                ////{

                ////    // Process the update
                ////    await batchUpdateService.ProcessUpdates(update);
                ////}

                //while (batchUpdateService.TryDequeue(out BatchUpdateItem item))
                //{
                //    foreach (var details in item.DetailsForUpdate)
                //    {
                //        // Process the update
                //        await batchUpdateService.ProcessUpdates(details);
                //    }

                //}
                var batchUpdateItem = await batchUpdateService.TryDequeue();

                if (batchUpdateItem != null)
                {
                    await ProcessBatchUpdate(batchUpdateItem);
                }
                // Wait some time before checking the queue again
                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }

        }
        private async Task ProcessBatchUpdate(BatchUpdateItem batchUpdateItem)
        {
            try
            {
                foreach (var itemDetails in batchUpdateItem.DetailsForUpdate)
                {
                    //var ipDetailsEntity = await _stackIpRepo.GetDetailsOfIp(itemDetails.Ip);

                    //ipDetailsEntity.Ip = ipDetailsEntity.Ip;
                    //ipDetailsEntity.Longitude = itemDetails.Longitude;
                    //ipDetailsEntity.Latitude = itemDetails.Latitude;
                    //ipDetailsEntity.Country = itemDetails.Country;
                    //ipDetailsEntity.City = itemDetails.City;

                    //await _stackIpRepo.UpdateDetail(ipDetailsEntity);
                }

            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, etc.

            }
        }
        #region To Delete
        //private readonly IBatchUpdateServiceFactory _batchUpdateServiceFactory;
        //private readonly Dictionary<Guid, BatchUpdateStatus> _statusMap = new Dictionary<Guid, BatchUpdateStatus>();
        //public BatchUpdateBackgroundService(IBatchUpdateServiceFactory batchUpdateServiceFactory)
        //{
        //    _batchUpdateServiceFactory = batchUpdateServiceFactory;
        //}

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{


        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        var batchUpdateService = _batchUpdateServiceFactory.Create();
        //        //batchUpdateService.QueueUpdates();
        //      //  batchUpdateService.ProcessUpdates();

        //        // Wait some time before next processing
        //        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        //    }
        //} 
        #endregion
    }
}
