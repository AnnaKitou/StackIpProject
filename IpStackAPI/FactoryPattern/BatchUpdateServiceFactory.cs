using IpStackAPI.Interfaces;

namespace IpStackAPI.FactoryPattern
{
    public class BatchUpdateServiceFactory:IBatchUpdateServiceFactory
    {
       private readonly IServiceProvider _serviceProvider;

        public BatchUpdateServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IBatchUpdateService Create()
        {
         return  _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IBatchUpdateService>();
        }
    }
}
