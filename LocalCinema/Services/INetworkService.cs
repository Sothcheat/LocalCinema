using System.Threading.Tasks;

namespace LocalCinema.Services
{
    public interface INetworkService
    {
        Task<bool> IsServerAvailableAsync();
        Task<string> GetStreamUrlAsync(int movieId);
        Task StartServerAsync();
        Task StopServerAsync();
    }
}