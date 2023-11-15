using IpStackAPI.DTOS;
using IpStackAPI.FactoryPattern;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace IpStackAPI.RepositoryServices
{
    public class BatchUpdateBackgroundService : BackgroundService
    {

        private readonly IBatchUpdateServiceFactory _batchUpdateServiceFactory;
        private readonly Dictionary<Guid, BatchUpdateStatus> _statusMap = new Dictionary<Guid, BatchUpdateStatus>();
        public BatchUpdateBackgroundService(IBatchUpdateServiceFactory batchUpdateServiceFactory)
        {
            _batchUpdateServiceFactory = batchUpdateServiceFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
         

            while (!stoppingToken.IsCancellationRequested)
            {
                var batchUpdateService = _batchUpdateServiceFactory.Create();
               // batchUpdateService.QueueUpdates();

                // Wait some time before next processing
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
