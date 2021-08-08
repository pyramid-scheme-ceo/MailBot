using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MailBot.Domain.Interfaces
{
    public interface ITableStore<T>
    {
        Task<T> GetById(string partitionKey, string rowKey, CancellationToken cancel);

        Task<bool> InsertOrReplace(T entity, CancellationToken cancel);

        Task<bool> InsertOrMergeBatch(IEnumerable<T> entities, CancellationToken cancel);
    }
}