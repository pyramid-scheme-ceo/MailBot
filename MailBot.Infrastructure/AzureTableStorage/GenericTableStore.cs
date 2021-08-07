using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailBot.Domain.BusinessObjects.ValueObjects;
using MailBot.Domain.Interfaces;
using Microsoft.Azure.Cosmos.Table;

namespace MailBot.Infrastructure.AzureTableStorage
{
    public class GenericTableStore<T> : ITableStore<T>
    {
        private readonly IEntityConverter<T> _entityConverter;
        private readonly CloudTable _table;
        
        public GenericTableStore(TableStorageConfiguration configuration, IEntityConverter<T> entityConverter)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(configuration.ConnectionString);
            var tableName = entityConverter.TableName;
            var tableClient = cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var table = tableClient.GetTableReference(tableName);

            table.CreateIfNotExists();
            
            _entityConverter = entityConverter;
            _table = table;
        }
        
        public Task<T> GetById(string partitionKey, string rowKey, CancellationToken cancel)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Insert(T entity, CancellationToken cancel)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> InsertOrMergeBatch(IEnumerable<T> entities, CancellationToken cancel)
        {
            var operation = new TableBatchOperation();

            foreach (var entity in entities)
            {
                operation.InsertOrReplace(_entityConverter.ToTableEntity(entity));
            }

            var result = await _table.ExecuteBatchAsync(operation, cancel);

            return result.All(r => r.RequestCharge.HasValue);
        }
    }
}