using IpStackAPI.Interfaces;

namespace IpStackAPI.FactoryPattern
{
    public interface IBatchUpdateServiceFactory
    {
        IBatchUpdateService Create();
    }
}
