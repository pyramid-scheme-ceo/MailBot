using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailBot.Domain.BusinessObjects.ValueObjects;
using MailBot.Domain.Interfaces;
using Microsoft.Azure.Cosmos.Table;

namespace MailBot.Infrastructure.AzureTableStorage
{
    public class GenericTableStore<T> : ITableStore<T> where T : class
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
        
        public async Task<T> GetById(string partitionKey, string rowKey, CancellationToken cancel)
        {
            var operation = TableOperation.Retrieve(partitionKey, rowKey);
            var result = await _table.ExecuteAsync(operation, cancel);

            if (result.Result is DynamicTableEntity entity)
            {
                return _entityConverter.FromTableEntity(entity);
            }

            return null;
        }

        public async Task<bool> InsertOrReplace(T entity, CancellationToken cancel)
        {
            var operation = TableOperation.InsertOrReplace(_entityConverter.ToTableEntity(entity));

            try
            {
                await _table.ExecuteAsync(operation, cancel);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
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