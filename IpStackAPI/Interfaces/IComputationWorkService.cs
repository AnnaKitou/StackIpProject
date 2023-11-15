using IpStackAPI.Entities;

namespace IpStackAPI.Interfaces
{
    public interface IComputationWorkService
    {
        Task<JobResultModel> DoWorkAsync(Guid JobId, JobParametersModel work,
            CancellationToken cancellationToken);
    }
}
