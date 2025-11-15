using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using server.Data;
using server.Models;
using server.Repositories.Interface;

namespace server.Repositories.Implement
{
    public class Queue: IQueue
    {
        private readonly QueueContext _context;
        public Queue(QueueContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateQueue()
        {
            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var lastQueue = await _context.Queues
                    .OrderByDescending(q => q.Number)
                    .FirstOrDefaultAsync();

                string? lastQ = lastQueue?.Number;
                string q = string.Empty;
                if (string.IsNullOrEmpty(lastQ))
                {
                    char t = 'A';
                    int num = 0;
                    q = t.ToString() + num.ToString();
                }
                else
                {
                    char t = lastQ[0];
                    char num = lastQ[1];
                    if (num == '9')
                    {
                        t = (char)(t + 1);
                        num = '0';
                    }
                    else
                    {
                        num = (char)(num + 1);
                    }
                    q = t.ToString() + num.ToString();
                }
                var newQueue = new Models.Queue
                {
                    Number = q,
                    QDate = DateTime.Now
                };
                _context.Queues.Add(newQueue);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return newQueue.Number;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Models.Queue?> GetLastestQueue()
        {
            var lastQueue = await _context.Queues
                .OrderByDescending(q => q.Number)
                .FirstOrDefaultAsync();
            return lastQueue;
        }

        public async Task<QueueResponse?> GetQueueDetail(string q)
        {
            var queue = await _context.Queues
                .Where(w => w.Number == q)
                .Select(s => new QueueResponse
                {
                    Id = s.Id,
                    Number = s.Number,
                    QDate = s.QDate.ToString("dd/MM/yyyy HH:mm:ss")
                })
                .FirstOrDefaultAsync();
            return queue;
        }

        public async Task ClearQueue()
        {
            var allQueues = await _context.Queues.ToListAsync();
            _context.Queues.RemoveRange(allQueues);
            await _context.SaveChangesAsync();
        }
    }
}
