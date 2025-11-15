using server.Models;

namespace server.Repositories.Interface
{
    public interface IQueue
    {
        Task<string> GenerateQueue();
        Task<Queue?> GetLastestQueue();
        Task<QueueResponse?> GetQueueDetail(string q);
        Task ClearQueue();
    }
}
