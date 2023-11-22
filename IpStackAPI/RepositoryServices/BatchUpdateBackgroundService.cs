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
                //while (batchUpdateService.TryDequeue(out DetailsOfIpDTO update))
                //{

                //    // Process the update
                //    await batchUpdateService.ProcessUpdates(update);
                //}

                while (batchUpdateService.TryDequeue(out BatchUpdateItem item))
                {
                    foreach (var details in item.DetailsForUpdate)
                    {
                        // Process the update
                        await batchUpdateService.ProcessUpdates(details);
                    }

                }

                // Wait some time before checking the queue again
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
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
